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
    class xModule2
    {

        # region 成员
        private vtkUnstructuredGrid uGrid;
        private vtkDataSetMapper mapper;
        private vtkActor actor;
        private string name;

        private double[] bounds;
        private int id;
        //private Color color;
        //private int material;
        # endregion

        # region 属性

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


        # endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public xModule2()
        {
            this.uGrid = vtkUnstructuredGrid.New();
            uGrid.Allocate(1, 1);  
            this.mapper = vtkDataSetMapper.New();
            this.actor = vtkActor.New();
            this.name = "module1";
            //this.material = 0;
            this.bounds = new double[6] { 0, 0, 0, 0, 0, 0 };
        }

        /// <summary>
        /// 解构函数
        /// </summary>
        public virtual void Dispose()
        {
            this.uGrid.Dispose();
            this.mapper.Dispose();
            this.actor.Dispose();
        }

        public void SetVisibility(int i)
        {
            this.actor.SetVisibility(i);
        }

        public void SetInput(List<vtkIdList> ids, vtkPoints points)
        {
            vtkPolyLine aPolyLine = vtkPolyLine.New();
            aPolyLine.GetPointIds().SetNumberOfIds(points.GetNumberOfPoints());
            for (int i = 0; i < points.GetNumberOfPoints(); i++)
            {
                aPolyLine.GetPointIds().SetId(i, i);
            }
            // Cells, very important  
            for (int i = 0; i < ids.Count; i++)
            {
                vtkIdList id = ids[i];
                uGrid.InsertNextCell(aPolyLine.GetCellType(), id);
                id.Dispose();
            }
            uGrid.SetPoints(points);  
  

        }

        public void SetColor(Color c)
        {
            this.actor.GetProperty().SetColor((double)c.R / 100.0, (double)c.G / 100.0, (double)c.B / 100.0);
            this.actor.GetProperty().SetDiffuse(1.0);
            this.actor.GetProperty().SetSpecular(0.0);
        }

        private void UpdateBounds()
        {
            if (this.uGrid == null)
                return;
            this.bounds = this.uGrid.GetBounds();
        }

        public void Update()
        {
            if (this.uGrid == null)
                return;
            this.uGrid.Update();
            this.mapper.SetInput(this.uGrid);
            this.actor.SetMapper(this.mapper);
            UpdateBounds();
        } // 联通Pipeline

    }
}
