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
using System.Collections;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace Nuwa.xClass
{
    using Loops = List<xLoop>;
    class xUnion
    {
        #region 成员

        // 主显示窗口，需要从mainform传递地址
        private RenderWindowControl renderWindowControl;
        public vtkCamera cam;
        private vtkPropPicker picker;
        // 主显示物件
        private List<xModule> objects;
        private List<xModule2> slices;
        // 辅助物件
        private xModule plate;
        private xModule pen;
        public vtkCubeAxesActor axes;

        // 辅助数据 
        public double[] firstPoint = { 0, 0, 0 };
        private double[] bounds = { 0, 0, 0, 0, 0, 0 };
        private double[] lastbounds = { 0, 250, 0, 200, 0, 100 };
        private double[] boundsDefault = { 0, 250, 0, 200, 0, 100 };
        
        // 鼠标选择
        public vtkActor LastPickedActor = null;
        public vtkProperty LastPickedProperty;
        private int selectedModuleIndex = -1;
        bool justpicked = false;

        #endregion 成员

        #region 属性

        public double[] Bounds
        {
            get
            {
                return bounds;
            }
            //set (  ;)
        }

        public int NumberOfObjects
        {
            get { return this.objects.Count; }
            //set (  ;)
        }

        public int NumberOfSlices
        {
            get { return this.slices.Count; }
            //set (  ;)
        }

        public int NumberOfLayers
        {
            get { return xPath.SlcZList.Count; }
            //set (  ;)
        }

        public int SelectedModuleIndex
        {
            get { return this.selectedModuleIndex; }
        }

        public vtkRenderWindow RenWin
        {
            get { return this.renderWindowControl.RenderWindow; }
        }

        public vtkRenderer Ren
        {
            get { return this.RenWin.GetRenderers().GetFirstRenderer(); }
        }

        public vtkRenderWindowInteractor Iren
        {
            get { return this.RenWin.GetInteractor(); }
        }

        public RenderWindowControl RenderWindowControl
        {
            set { this.renderWindowControl = value; }
        }

        #endregion 属性

        /// <summary>
        /// 构造函数
        /// </summary>
        public xUnion()
        {
            this.picker = vtkPropPicker.New();
            this.axes = vtkCubeAxesActor.New();
            this.cam = vtkCamera.New();
            this.LastPickedProperty = vtkProperty.New();
            this.objects = new List<xModule>();
            this.slices = new List<xModule2>();
            this.plate = new xModule();
            this.pen = new xModule();
        }
        /// <summary>
        ///  解构函数
        /// </summary>
        public virtual void Dispose()
        {
            foreach (xModule m in this.objects)
            {
                m.Dispose();
            }
            foreach (xModule2 m in this.slices)
            {
                m.Dispose();
            }
            RemoveAllObjects();
            RemoveAllSlices();
        }
        public void SetUpScene()
        {
            //灰色背景
            Ren.SetBackground(0.2, 0.2, 0.2);
            // 加入box
            CreatePlateActor();
            CreatePenActor();
            CreateAxesActor();
            InitView();
            //// interact
            Iren.LeftButtonPressEvt += new vtkObject.vtkObjectEventHandler(iren_LeftButtonPressEvt);
        }
        public void InitView()
        {
            SetCamera(0, -1, 0, 0, 0, 1);
            cam.Yaw(15);
            cam.Pitch(-15);
            Ren.ResetCamera();
        }
        public void Render()
        {
            RenWin.Render();
        }
        public void SetCamera(float x, float y, float z, float vx, float vy, float vz)
        {
            cam.SetFocalPoint(0.0, 0.0, 0.0);
            cam.SetPosition(x, y, z);
            cam.SetViewUp(vx, vy, vz);
            Ren.SetActiveCamera(cam);
            Ren.ResetCamera();
            RenWin.Render();
        }
        public void ResetSetCamera(double[] b, bool isOffset)
        {
            if (isOffset)
            {
                double offset = 0.02;
                double l = (b[1] - b[0]) * offset; double w = (b[3] - b[2]) * offset; double h = (b[5] - b[4]) * offset;
                Ren.ResetCamera(b[0] - l, b[1] + l, b[2] - w, b[3] + w, b[4] - h, b[5] + h);
            }
            else
                Ren.ResetCamera(b[0], b[1], b[2], b[3], b[4], b[5]);
        }
        public void iren_LeftButtonPressEvt(vtkObject sender, vtkObjectEventArgs e)
        {
            int[] clickPos = Iren.GetEventPosition();
            // Pick from this location.
            vtkPropPicker picker = vtkPropPicker.New();
            picker.Pick(clickPos[0], clickPos[1], 0, Ren);
            SelectPickedActor(picker.GetActor());
            RenWin.Render();
        } // interactor dianxuan 
        public void SelectPickedActor(vtkActor pickedActor)
        {
            if (this.LastPickedActor != null)
            {
                this.LastPickedActor.GetProperty().DeepCopy(this.LastPickedProperty);
                lastbounds = LastPickedActor.GetBounds();
            }
            this.LastPickedActor = pickedActor;
            if (this.LastPickedActor != null)
            {
                for (int i = 0; i < NumberOfObjects; i++)
                {
                    if (pickedActor == this.objects[i].Actor)
                        this.selectedModuleIndex = i;
                }
                MainForm.SelectedModule = this.objects[this.selectedModuleIndex];

                // Save the property of the picked actor so that we can
                // restore it next time
                this.LastPickedProperty.DeepCopy(this.LastPickedActor.GetProperty());
                // Highlight the picked actor by changing its properties
                this.LastPickedActor.GetProperty().SetEdgeColor(1.0, 0.0, 0.0);
                this.LastPickedActor.GetProperty().SetEdgeVisibility(1);
                ResetSetCamera(LastPickedActor.GetBounds(), false);
                AxesUpdate(LastPickedActor.GetBounds());
                justpicked = true;
            }
            else
            {
                this.selectedModuleIndex = -1;
                MainForm.SelectedModule = null;
                if (justpicked)
                {
                    AxesUpdate(boundsDefault);
                    ResetSetCamera(lastbounds, true);
                    justpicked = false;
                }
            }


        }

        #region Object Module 列表的操作

        public void AddObject(xModule object_)
        {
            this.objects.Add(object_);
            Ren.AddActor(object_.Actor);
            object_.ID = this.objects.Count - 1;
            UpdateBounds();
        }//  添加3D物体的方法

        public void RemoveObject(int index)
        {
            if (index == -1) 
               return;
            if (index < this.objects.Count)
            {
                Ren.RemoveActor(objects[index].Actor);
                this.objects.RemoveAt(index);
                UpdateBounds();
            }
            Render();
        }// 从列表中删除3D物体的方法

        public void RemoveAllObjects()
        {
            if (objects.Count > 0)
            {
                for (int i = 0; i < this.NumberOfObjects; i++)
                {
                    Ren.RemoveActor(this.objects[i].Actor);
                }
                objects.RemoveRange(0, this.objects.Count);
                //ren.RemoveAllViewProps();
            }
        }//  从列表中删除所有3D物体的方法

        public xModule GetObjectAt(int index_)
        {
            if (NumberOfObjects >= 1)
                return this.objects[index_];
            else
                return null;
        }//  返回列表中的第i个3D物体

        public string SplitPathForName(string path)
        {
            int i = path.Length;
            while (i > 0)
            {
                char ch = path[i - 1];
                if (ch == '\\' | ch == '/' || ch == ':')
                    break;
                i--;
            }
            //dir = path.Substring(0, i);
            return path.Substring(i);
        }


        public xModule GetObjectFromSTL(string filename_)
        {
            // read from a stl file
            vtkSTLReader stlReader = vtkSTLReader.New();
            stlReader.SetFileName(filename_);
            stlReader.Update();

            xModule object_ = new xModule();
            // Set module name by file name
            object_.ID= NumberOfObjects;
            object_.PolyData = stlReader.GetOutput();
            object_.Update();
            return object_;
        }//  通过STL文件得到3D物体的方法

        public xModule GetObjectFromPLY(string filename_)//  通过PLY文件得到3D物体的方法
        {
            // read from a stl file
            vtkPLYReader PLYReader = vtkPLYReader.New();
            PLYReader.SetFileName(filename_);
            PLYReader.Update();

            xModule object_ = new xModule();
            // Set module name by file name
            object_.ID = NumberOfObjects;
            object_.PolyData = PLYReader.GetOutput();
            object_.Update();
            return object_;
        }

        public void UpdateBounds()
        {
            if (this.objects.Count == 0)
                return;
            // 将Xmin，Xmax， Ymin
            for (int k = 0; k < 3; k++)
            {
                bounds[2 * k] = double.MaxValue;
                bounds[2 * k + 1] = double.MinValue;
            }

            for (int i = 0; i < this.objects.Count; i++)
            {
                double[] mb = this.objects[i].Bounds;
                for (int j = 0; j < 3; j++)
                {
                    bounds[2 * j] = bounds[2 * j] < mb[2 * j] ? bounds[2 * j] : mb[2 * j];
                    bounds[2 * j + 1] = bounds[2 * j + 1] > mb[2 * j + 1] ? bounds[2 * j + 1] : mb[2 * j + 1];
                }
            }
            xPath.Bounds = Bounds;
        }

        #endregion Object

        #region Slice Module 列表的操作

        public void AddSlice(xModule2 slice_)
        {
            this.slices.Add(slice_);
            Ren.AddActor(slice_.Actor);
        }

        public void RemoveSlice(int index)
        {
            if (index < this.slices.Count)
            {
                Ren.RemoveActor(this.slices[index].Actor);
                this.slices.RemoveAt(index);
            }
        }

        public void RemoveAllSlices()
        {
            if (this.slices.Count > 0)
            {
                for (int i = 0; i < this.NumberOfObjects; i++)
                {
                    Ren.RemoveActor(this.slices[i].Actor);
                }
                this.slices.RemoveRange(0, this.slices.Count);
            }
        }

        public xModule2 GetSliceAt(int index_)
        {
            if (this.slices.Count > 0)
                return this.slices[index_];
            else
                return null;
        }

        #endregion Slice

        #region toolpath Module的操作

        public void CreateToolPathActor()
        {
            xModule2 polygon = new xModule2();
            for (int i = 0; i < this.NumberOfLayers; i++)
            {
                xLayer Layer = xPath.Layers[i];  double z = Layer.Z;

                for (int j = 0; j < Layer.NumberOfToolPaths; j++)
                {
                    xToolPath toolPath = Layer.GetToolPathAt(j);
                    for (int k = 0; k < toolPath.Paths.Count; k++)
                    {
                        polygon.AddLine();
                        xLoop polyline = toolPath.Paths[k];
                        for (int m = 0; m < polyline.NumberOfPoints; m++)
                        {
                            xPoint2 pt = polyline.GetPointAt(m);
                            polygon.AddPoint(pt, z);
                        } // for m
                        
                    } // for k
                } // for j
            } // for i
            polygon.Update();
            Ren.AddActor(polygon.Actor);
            Render();
        }

        public void CreateSamplePathActor()
        {
            if (NumberOfSlices == 1)
            {
                RemoveSlice(NumberOfSlices - 1);
            }
            xModule2 polygon = new xModule2();
            AddSlice(polygon);
            for (int i = 0; i < xSample.Path.Count; i++)
            {
                polygon.AddLine();
                List<xSegment2> Layer = xSample.Path[i];
                double z = Config.FirstLayerHeight + Config.LineHeight * i;
                for (int j = 0; j < Layer.Count; j++)
                 {
                     xSegment2 line = Layer[j];
                    if (line.Type == SegmentType.Line)
                    {
                        xPoint2 pt = line.EndPoint;
                        polygon.AddPoint(pt, z);
                    }
                    else
                    {
                        List<xPoint2> pts = line.GetArcPoints();
                       polygon.AddPoints(pts,z);
                    }

                  } // for m

            } // for i
            //polygon.SetInput(ids, points);
            polygon.Update();
            polygon.SetColor(Color.Blue);
            Ren.AddActor(polygon.Actor);
            Render();
        }

        public void CreateToolPathActor_GCode()
        {
            try
            {
                vtkPoints points = vtkPoints.New();
                List<vtkIdList> ids = new List<vtkIdList>();
                int pointNum = 0;
                vtkIdList id = vtkIdList.New();
                for (int i = 0; i < xGCode.Path.Count; i++)
                {
                    xPoint3 pt = xGCode.Path[i].EndPoint;
                    points.InsertNextPoint(pt.X, pt.Y, pt.Z);
                    id.InsertNextId(pointNum);
                    pointNum++;
                } // for m
                ids.Add(id);
                xModule2 polygon = new xModule2();
                //polygon.SetInput(ids, points);
                polygon.Update();
                Ren.AddActor(polygon.Actor);
                Render();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }


        //public void BrowseToolPathActorAt(int value)
        //{
        //    //将所有的点通过CellArray连接在一起   
        //    vtkPolyLine polyLine = vtkPolyLine.New();
        //    polyLine.GetPointIds().SetNumberOfIds(value);
        //    for (int i = 0; i < value; i++)
        //        polyLine.GetPointIds().SetId(i, i);
        //    vtkPoints points = vtkPoints.New();
        //    this.points.GetPoints(polyLine.GetPointIds(), points);
        //    vtkCellArray cells = vtkCellArray.New();
        //    cells.InsertNextCell(polyLine);
        //    // 创建一个Polydata将点坐标和拓扑关系储存在里面
        //    vtkPolyData polyData = toolPath.PolyData;
        //    polyData.SetPoints(points);
        //    polyData.SetLines(cells);
        //    toolPath.Update();
        //    polyLine.Dispose();
        //    points.Dispose();
        //    cells.Dispose();
        //    Render();
        //}

        #endregion -------------------------------

        #region Box ； Axes； Pen等辅助 Module的操作

        public void CreatePlateActor()
        {
            vtkPNGReader pngReader = vtkPNGReader.New();
            pngReader.SetFileName("..\\..\\Resources\\cb.png");
            vtkTexture texture = vtkTexture.New();
            texture.SetInputConnection(pngReader.GetOutputPort());
            texture.InterpolateOn();

            vtkPlaneSource plane1 = vtkPlaneSource.New();
            plane1.SetOrigin(0, 0, 0);
            plane1.SetPoint1(250, 0, 0);
            plane1.SetPoint2(0, 200, 0);
            this.plate.PolyData = plane1.GetOutput();
            this.plate.Update();
            this.plate.Actor.SetTexture(texture);
            Ren.AddActor(this.plate.Actor);
            this.plate.Actor.PickableOff();
        }

        public void SetPenPosition(double x, double y, double z)
        {
            this.pen.Actor.SetPosition(x, y, z);
            Render();
        }
           
        public void ShowPen(bool show)
        {
            if (show)
                this.pen.SetVisibility(1);
            else
                this.pen.SetVisibility(0);
        }

        public void CreatePenActor()
        {
            vtkConeSource cone = vtkConeSource.New();
            cone.SetRadius(0.3);
            cone.SetHeight(0.5);
            cone.SetResolution(10);
            cone.SetDirection(0, 0, -1);
            pen.PolyData = cone.GetOutput();
            pen.Update();
            pen.SetColor(Color.Red);
            pen.Actor.PickableOff();
            pen.Actor.VisibilityOff();
            Ren.AddActor(pen.Actor);
            cone.Dispose();
        }

        public void CreateAxesActor()
        {
            this.axes.SetCamera(cam);
            this.axes.SetFlyModeToOuterEdges();
            this.axes.SetXTitle("X"); //this.axes.SetXUnits("mm");               //设置X轴的标签
            this.axes.SetYTitle("Y"); //this.axes.SetYUnits("mm");
            this.axes.SetZTitle("Z"); //this.axes.SetZUnits("mm");
            this.axes.SetUseBounds(true);
            this.axes.SetBounds(0, 250, 0, 200, 0, 100);
            this.axes.SetXAxisTickVisibility(0); this.axes.SetYAxisTickVisibility(0); this.axes.SetZAxisTickVisibility(0);
            this.axes.GetProperty().SetColor(0.7, 0.7, 0.9);
            this.axes.PickableOff();
            Ren.AddActor(axes);
            //Axes.SetTickLocationToOutside();
        }

        public void AxesUpdate(double[] Bounds)
        {
            this.axes.SetBounds(Bounds[0], Bounds[1], Bounds[2], Bounds[3], Bounds[4], Bounds[5]); // Feed axes with object
            Render();
        } // 根据bounds更新Axes


        # endregion



    } // end class
} // end name space


