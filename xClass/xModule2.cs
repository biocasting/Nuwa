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
        private vtkPolyDataMapper mapper;
        private vtkActor actor;
        private vtkPoints points;
        private string name="module1";
        private List<vtkIdList> idLists;
        private vtkPolyData polydata;
        private double[] bounds = {0,0,0,0,0,0};
        private int id=0;
        private int count=0;
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
            this.points = vtkPoints.New();
            this.idLists = new List<vtkIdList>();
            uGrid.Allocate(1, 1);
            this.polydata = vtkPolyData.New();
            this.mapper = vtkPolyDataMapper.New();
            this.actor = vtkActor.New();
            //this.material = 0;
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


        public void AddLine()
        {
            vtkIdList idlist = new vtkIdList();
            this.idLists.Add(idlist);
        }

        public void AddPoint(xPoint2 pt, double z)
        {
            vtkIdList idlist = this.idLists[this.idLists.Count - 1];
            this.points.InsertNextPoint(pt.X, pt.Y, z);
            idlist.InsertNextId(count);
            count++;
        }
        public void AddPoints(List<xPoint2> pts, double z)
        {
            vtkIdList idlist = this.idLists[this.idLists.Count - 1];
            for (int i= 0; i<pts.Count; i++)
            { 
                this.points.InsertNextPoint(pts[i].X, pts[i].Y, z);
                idlist.InsertNextId(count);
                count++;
            }
        }

        public void SetUpGrid()
        {
            vtkPolyLine aPolyLine = vtkPolyLine.New();
            aPolyLine.GetPointIds().SetNumberOfIds(points.GetNumberOfPoints());
            for (int i = 0; i < this.points.GetNumberOfPoints(); i++)
            {
                aPolyLine.GetPointIds().SetId(i, i);
            }
            // Cells, very important  
            for (int i = 0; i < this.idLists.Count; i++)
            {
                vtkIdList idList = this.idLists[i];
                uGrid.InsertNextCell(aPolyLine.GetCellType(), idList);
            }
            uGrid.SetPoints(points);  
            vtkGeometryFilter geomFilter = vtkGeometryFilter.New();
            geomFilter.SetInput(uGrid);
            //vtkTubeFilter tuber = vtkTubeFilter.New();
            //tuber.SetInput(geomFilter.GetOutput());
            // tuber.SetNumberOfSides(6);
            // tuber.SetRadius(0.1);
             this.polydata = geomFilter.GetOutput();
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
            SetUpGrid();
            this.uGrid.Update();
            this.mapper.SetInput(this.polydata);
            this.actor.SetMapper(this.mapper);
            this.actor.PickableOff();
            UpdateBounds();
        } // 联通Pipeline

    }
}
