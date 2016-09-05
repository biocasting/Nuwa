/*----------------------------------------------------------------
// Copyright (C) 2004 无锡雅宝有限公司
// 版权所有。
//
// 文件名：
// 文件功能描述：
//    
//
// 创建标识：
//
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Kitware.VTK;

namespace Nuwa.xClass
{
    public class xModule
    {

        # region 成员
        private vtkPolyData polydata;
        private vtkPolyDataMapper mapper;
        private vtkActor actor;
        private double[] bounds = { 0, 0, 0, 0, 0, 0 };
        public double[] angle = { 0, 0, 0 };
        private int id;
        private Color color;
        //private int material;
        //public bool IsSliceExisted;
        # endregion

        # region 属性
        public vtkPolyData PolyData
        {
            get { return this.polydata; }
            set { this.polydata = value; UpdateBounds(); }
        }

        public double[] Bounds
        {
            get { return this.bounds; }
        }

        public int ID
        {
            set { this.id = value; }
            get { return this.id; }
        }

        public double[] Dimesion
        {
            get
            {
                double[] dimesion = { 0, 0, 0 };
                dimesion[0] = bounds[1] - bounds[0];
                dimesion[1] = bounds[3] - bounds[2];
                dimesion[2] = bounds[5] - bounds[4];
                return dimesion;
            }
            //set (  ;)
        }

        public double[] Center
        {
            get
            {
                double[] center = { 0, 0, 0 };
                center[0] = -bounds[0] / 2 + bounds[1] / 2;
                center[1] = -bounds[2] / 2 + bounds[3] / 2;
                center[2] = -bounds[4] / 2 + bounds[5] / 2;
                return center;
            }
            //set (  ;)
        }

        public double[] Angle
        {
            get
            {
                return angle;
            }
            //set (  ;)
        }

        public vtkActor Actor
        {
            get { return this.actor; }
        }

        //public int Material
        //{
        //    set { this.material = value; }
        //    get { return this.material; }
        //}

        # endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public xModule()
        {
            this.polydata = vtkPolyData.New();
            this.mapper = vtkPolyDataMapper.New();
            this.actor = vtkActor.New();
            this.color = Color.SkyBlue;
            //IsSliceExisted = false;
        }

        /// <summary>
        /// 解构函数
        /// </summary>
        public virtual void Dispose()
        {
            this.polydata.Dispose();
            this.mapper.Dispose();
            this.actor.Dispose();
        }

        /// <summary>
        ///  设定对象显示还是隐藏
        /// </summary>
        /// <param name="i">i=0为隐藏，i=1为显示</param>
        public void SetVisibility(int i)
        {
            if (this.actor == null)
                return;
            this.actor.SetVisibility(i);
        }

        /// <summary>
        ///  设定对象的颜色
        /// </summary>
        /// <param name="c">Color属性</param>
        public void SetColor(Color c)
        {
            if (this.actor == null)
                return;
            this.actor.GetProperty().SetColor((double)c.R / 100.0, (double)c.G / 100.0, (double)c.B / 100.0);
            this.actor.GetProperty().SetDiffuse(1.0);
            this.actor.GetProperty().SetSpecular(0.0);
        }

        /// <summary>
        ///  更新对象的外周参数
        /// </summary>
        private void UpdateBounds()
        {
            if (this.polydata == null)
                return;
            this.bounds = this.polydata.GetBounds();
        }

        /// <summary>
        ///  更新VTK渲染管道
        /// </summary>
        public void Update()
        {
            if (this.polydata == null)
                return;
            this.polydata.Update();
            this.mapper.SetInput(this.polydata);
            this.actor.SetMapper(this.mapper);
            UpdateBounds();
        } // 联通Pipeline

        /// <summary>
        ///  对对象进行平移，旋转，缩放等转换
        /// </summary>
        /// <param name="x"> X 轴 平移距离，放大系数，和旋转角度</param>
        /// <param name="y"> Y轴 平移距离，放大系数，和旋转角度</param>
        /// <param name="z"> Z 轴 平移距离，放大系数，和旋转角度</param>
        /// <param name="option">0 为 平移，1 为旋转， 2 为缩放</param>
        public void Transform(double x, double y, double z, int option)
        {
            vtkTransform Tran = vtkTransform.New();
            vtkTransformPolyDataFilter Filter = vtkTransformPolyDataFilter.New();
            Tran.Identity();
            switch (option)
            {
                case 0:
                    Tran.Translate(x, y, z);
                    break;
                case 1:
                    Tran.RotateX(x);
                    Tran.RotateY(y);
                    Tran.RotateZ(z);
                    break;
                case 2:
                    Tran.Scale(x, y, z);
                    break;
            }

            Filter.SetInput(PolyData);
            Filter.SetTransform(Tran);
            Filter.Update();
            PolyData = Filter.GetOutput();
            Update();
            Tran.Dispose();
            Filter.Dispose();
        }


    }
}
