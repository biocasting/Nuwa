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
    class xModule
    {

        # region 成员
        private vtkPolyData polydata;
        private vtkPolyDataMapper mapper;
        private vtkActor actor;
        private string name;

        private double[] bounds;
        public bool IsSliceExisted;
        private int id;
        //private Color color;
        //private int material;
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

        public string Name
        {
            set { this.name = value; }
            get { return this.name; }
        }

        public int ID
        {
            set { this.id = value; }
            get { return this.id; }
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
            this.name = "module1";
            //this.material = 0;
            this.bounds = new double[6] { 0, 0, 0, 0, 0, 0 };
            IsSliceExisted = false;
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

        public void SetVisibility(int i)
        {
            this.actor.SetVisibility(i);
        }

        public void SetColor(Color c)
        {
            this.actor.GetProperty().SetColor((double)c.R / 100.0, (double)c.G / 100.0, (double)c.B / 100.0);
            this.actor.GetProperty().SetDiffuse(1.0);
            this.actor.GetProperty().SetSpecular(0.0);
        }

        private void UpdateBounds()
        {
            if (this.polydata == null)
                return;
            this.bounds = this.polydata.GetBounds();
        }

        public void Update()
        {
            if (this.polydata == null)
                return;
            this.polydata.Update();
            this.mapper.SetInput(this.polydata);
            this.actor.SetMapper(this.mapper);
            UpdateBounds();
        } // 联通Pipeline

    }
}
