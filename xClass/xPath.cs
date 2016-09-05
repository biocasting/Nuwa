using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace Nuwa.xClass
{
    using Loops = List<xLoop>;
    public class xPath
    {
        //包含所有点数据的层的列表
         public static List<xLayer> Layers = new List<xLayer>();
         public static List<double> SlcZList= new List<double>();
         public static int NumberOfObjects = 0;
         public static double LayerThickness=0.4;
         public static double[] Bounds = {0,1,0,1,0,1};

         //           假设层厚0.4，物体
         //           _______________________________________   5   Z5     if Z5 =对象最高点， 向下 0.1Heigh     如果1.9  / 1.85位置切      LastLayerHeight
         //           ___________|_____|_______________________ 4   Z4     1.5
         //           _________|________|______________________ 3   Z3     1.1
         //           _______|____________|____________________  2  Z2     0.7    
         //           ______|________________|_________________  1   Z1    0.3   第一层 (0~1 )  是厚度的0.5~0.8,
         //           _____|___________________|_______________  0    Z0    0
         //            层厚=Z1-Z0, 
        public static int NumberOfLayers
        {
            get { return SlcZList.Count; }
            //set (  ;)
        }
        public xPath()   {   }

        #region TooPaths生成算法

        public static void GenerateSlcZList()
        {
            double thickness = Config.LineHeight;
            double height = Bounds[5] - Bounds[4];
            if (thickness <= 0 || height < thickness)
                return;
            // （最大Z-最小Z）/ 层高就是总层数。 如果是5.5， 则5
            double fSlcNum = height / thickness + 1.0;
            int iSlcNumber = (int)fSlcNum;
            LayerThickness = thickness;
            //清空以前的点
             if (SlcZList.Count > 0)
                SlcZList.Clear();
            //将切的点加入到List中
            for (int i = 0; i < iSlcNumber; i++)
            {
                double pos = Bounds[4] + thickness * (i + 0.5);
                SlcZList.Add(pos);
            }
            ////为了防止切到边线，最上层的向下移动10%
            if (SlcZList[iSlcNumber - 1] >= Bounds[5] - thickness * 0.5)
            {
                SlcZList[iSlcNumber - 1] = Bounds[5] - thickness * 0.05;
            }
        }




        public void CreateSubPathsbyLayer(int i)
        {
            //each material is this.assembly pen, from pen1 to pen 4
            xLayer layer = Layers[i];
            for (int j = 0; j < layer.NumberOfToolPaths; j++)
            {
                xToolPath toolpath = layer.GetToolPathAt(j);
                toolpath.Paths.Clear();
                toolpath.Rearange(1);
                toolpath.Paths.AddRange(toolpath.Borders);
            } // end for
        }




        //public void CreateToolPathbyLayer(int i, double[] offset, double[] firstOffset, int[] fillPattern)
        //{
        //    //each material is this.assembly pen, from pen1 to pen 4
        //    xLayer layer = this.GetLayerAt(i);
        //    for (int j = 0; j < layer.NumberOfToolPaths; j++)
        //    {
        //        xToolPath toolpath = layer.GetToolPathAt(j);
        //        int k = toolpath.Material;
        //        if (fillPattern[k] == 0)
        //        {
        //            toolpath.Paths = toolpath.ContourPath(firstOffset[k], offset[k]);
        //        }
        //        else
        //        {
        //            if (fillPattern[k] == 2 && i % 2 == 0)
        //            {
        //                //xVector2D v = new xVector2D(0, 1);
        //                //toolpath.Paths = toolpath.RasterPath(offset[k], v);
        //                xVector2D v = new xVector2D(1, 0);
        //                toolpath.Paths = toolpath.RasterPath(offset[k], v);
        //                foreach(xLoop line in toolpath.Paths)
        //                {
        //                    line.Reverse();
        //                }
        //            }
        //            else
        //            {
        //                xVector2D v = new xVector2D(1, 0);
        //                toolpath.Paths = toolpath.RasterPath(offset[k], v);
        //            }
        //        }
        //    }

        //}  
        // 多种材料的函数
        //public void ExecuteObjShowPattern(int i, int mode)
        //{
        //    switch (mode)
        //    {
        //        case 0:
        //            GetObjectAt(i).SetVisibility(1);
        //            GetSliceAt(i).SetVisibility(0);
        //            ToolPath.SetVisibility(0);
        //            break;
        //        case 1:
        //            GetObjectAt(i).SetVisibility(0);
        //            GetSliceAt(i).SetVisibility(1);
        //            ToolPath.SetVisibility(0);
        //            break;
        //        case 2:
        //            GetObjectAt(i).SetVisibility(0);
        //            GetSliceAt(i).SetVisibility(0);
        //            ToolPath.SetVisibility(1);
        //            break;
        //        case 3:
        //            GetObjectAt(i).SetVisibility(1);
        //            GetSliceAt(i).SetVisibility(1);
        //            ToolPath.SetVisibility(1);
        //            break;
        //    }
        //    Render();
        //}

        #endregion Generate Layers

        #region Layer 对象的操作

        public static void AddLayer(xLayer layer)
        {
            Layers.Add(layer);
            //UpdateBounds();
        }

        public xLayer GetLayerAt(int index_)
        {
            if (xPath.Layers.Count >= 1)
                return xPath.Layers[index_];
            else
                return null;
        }

        #endregion Layer

        public static xBoundry GetBoundary(vtkPolyData slice)
        {
            // 得到切片数据，假设全部是线段
            vtkCellArray cellAry = slice.GetLines();
            // 如果没有得到线段，返回null
            if (cellAry.GetNumberOfCells() == 0)
                return null;
            // 检查是否是开环数据，如果是，跳出错误提示，返回null
            if (cellAry.GetNumberOfCells() != slice.GetNumberOfPoints())
            {
                System.Windows.Forms.MessageBox.Show("Open Loops!");
                return null;
            }

            // 将切片数据全部读入到nodes中
            xBoundry boundry = new xBoundry();
            vtkIdList ids = vtkIdList.New();
            vtkPoints points = slice.GetPoints();
            cellAry.InitTraversal();
            for (int i = 0; i < cellAry.GetNumberOfCells(); i++)
            {
                cellAry.GetNextCell(ids);
                xLoopNode node = new xLoopNode(points.GetPoint(ids.GetId(0)));
                node.HeadPtId = (int)ids.GetId(0);
                node.TailPtId = (int)ids.GetId(1);
                boundry.AddNode(node);
            }
            // 通过process,得到 loops
            boundry.FillLoopsFromNodes();
            boundry.MarkOuterLoops(); ;
            return boundry;
        }








        //
        //public double SlcYPos(int index)
        //{
        //    if (index < this.slcYPosList.Count)
        //    {
        //        return this.slcYPosList[index];
        //    }
        //    else
        //        return 0;
        //}
        //public void CreateSliceYFromObject(int index, double diameter, bool tubeshow = true)
        //{
        //    GenerateSlcYPosList(diameter);
        //    xModule obj = GetObjectAt(index);
        //    // Slice the object one by one
        //    vtkPlane slcPlane = vtkPlane.New();
        //    vtkPolyData data = null;
        //    slcPlane.SetOrigin(0, 0, 0);
        //    slcPlane.SetNormal(0.0, 1.0, 0.0);
        //    vtkCutter slicer = vtkCutter.New();
        //    slicer.SetInput(obj.PolyData);
        //    slicer.SetCutFunction(slcPlane);

        //    slicer.GenerateValues(this.slcYPosList.Count, this.slcYPosList[0], this.slcYPosList[this.slcYPosList.Count - 1]);
        //    slicer.Update();
        //    if (tubeshow)
        //    {
        //        // Get the sliced the ToolPaths with Tube filter
        //        vtkTubeFilter tuber = vtkTubeFilter.New();
        //        tuber.SetNumberOfSides(6);
        //        tuber.SetInput(slicer.GetOutput());
        //        tuber.SetRadius(diameter / 2);
        //        tuber.Update();
        //        data = tuber.GetOutput();
        //    }
        //    else
        //    {
        //        data = slicer.GetOutput();
        //    }

        //    xModule slc = GetSliceAt(index);
        //    slc.PolyData = data;
        //    slc.SetColor(Color.Blue);
        //    slc.Update();
        //}
        //private xBoundry GetYBoundryAt(int SliceIndex, int ModuleIndex)
        //{
        //    // 在位置列表的某点产生切片
        //    vtkPlane slcPlane = vtkPlane.New();
        //        slcPlane.SetOrigin(0, 0, this.slcYPosList[SliceIndex]);
        //        slcPlane.SetNormal(0.0, 1.0, 0.0);
        //    vtkCutter slicer = vtkCutter.New();
        //        slicer.SetInput(this.objects[ModuleIndex].PolyData);
        //        slicer.SetCutFunction(slcPlane);
        //        slicer.Update();
        //        //slicer.Dispose();
        //        //slcPlane.Dispose();

        //    // 得到切片数据，假设全部是线段
        //    vtkPolyData slice = slicer.GetOutput();
        //    vtkCellArray cellAry = slice.GetLines();
        //    // 如果没有得到线段，返回null
        //    if (cellAry.GetNumberOfCells() == 0) 
        //        return null; 
        //    // 检查是否是开环数据，如果是，跳出错误提示，返回null
        //    if (cellAry.GetNumberOfCells() != slice.GetNumberOfPoints()) 
        //    { 
        //         System.Windows.Forms.MessageBox.Show("Open Loops!"); 
        //        return null; 
        //    }
        //    // 将切片数据全部读入到nodes中
        //    xBoundry boundry = new xBoundry();
        //    vtkIdList ids = vtkIdList.New();
        //    vtkPoints points = slice.GetPoints();
        //    cellAry.InitTraversal();
        //    for (int i = 0; i < cellAry.GetNumberOfCells(); i++)
        //    {
        //        cellAry.GetNextCell(ids);
        //        xLoopNode node = new xLoopNode(points.GetPoint(ids.GetId(0)));
        //        node.HeadPtId = ids.GetId(0);
        //        node.TailPtId = ids.GetId(1);
        //        boundry.AddNode(node);
        //    }
        //    // 通过process,得到 loops
        //    boundry.FillLoopsFromNodes();
        //    boundry.MarkOuterLoops(); ;
        //    return boundry;
        //}
    }


    public class xGcPoint : Object
    {
        public long x;
        public long y;
        public long z;
        public long e;

        public xGcPoint()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.e = 0;
        }

        public xGcPoint(int x, int y, int z, int e)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.e = e;
        }

        public static xGcPoint operator -(xGcPoint p1_, xGcPoint p2_)
        {
            xGcPoint result = new xGcPoint();
            result.x = p1_.x - p2_.x;
            result.y = p1_.y - p2_.y;
            result.z = p1_.z - p2_.z;
            result.e = p1_.e - p2_.e;
            return result;
        }
    }


    class xGcLine
    {
        public xGcPoint p1; // 前面的新点
        public xGcPoint p2; //后面的老点
        public int m1; //这条线的模式，
        public int m2; //这条线的旧mode模式。

        public xGcLine()
        {
            p1 = new xGcPoint(0, 0, 0, 0);
            p2 = new xGcPoint(0, 0, 0, 0);
            m1 = -1;
            m2 = -1;
        }

        public void SetP2()
        {
            this.p2.x = this.p1.x;
            this.p2.y = this.p1.y;
            this.p2.z = this.p1.z;
            this.p2.e = this.p1.e;
        } // 将新点的值赋给老点

        public void SetP2_XY()
        {
            this.p2.x = this.p1.x;
            this.p2.y = this.p1.y;
        } // 将新点的值赋给老点


        public void SetP2_Z()
        {
            this.p2.z = this.p1.z;
        } // 将新点的值赋给老点

        public void SetP1(int x, int y, int z, int e)
        {
            this.p1.x = x;
            this.p1.y = y;
            this.p1.z = z;
            this.p1.e = e;
        } // 赋新点的值

        public void SetM1(int m)
        {
            this.m1 = m;
        }

        public void SetM2()
        {
            this.m2 = m1;
        }

        public bool IsModeChanged()
        {
            if (this.m2 == -1)
                return false;
            else
            {
                if (this.m1 == this.m2)
                    return false;
                else
                    return true;
            }
        }

        public bool IsLineZero()
        {
            if (this.p1.x == this.p2.x && this.p1.y == this.p2.y && this.p1.z == this.p2.z)
                return true;
            else
                return false;
        }

        public xGcPoint LI
        {
            get { return this.p1 - this.p2; }
        }

        public string LiString
        {
            get { return "LI " + LI.x.ToString() + "," + LI.y.ToString() + "," + LI.z.ToString() + "," + LI.e.ToString(); }
            //set (  ;)
        }

        public string LiString_XYZ
        {
            get { return "LI " + LI.x.ToString() + "," + LI.y.ToString() + "," + LI.z.ToString(); }
            //set (  ;)
        }


        public double DistanceSq()
        {
            return (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y);
        }

        public double Distance()
        {
            return Math.Sqrt(DistanceSq());
        }


    }

    public struct Code
    {
        public static string StartProgram = "# Program;ST;CSS;CS;SH;SP ,,10000;DP0,0,0;LMABC";
        public static string EndProgram = "ST;GA,,,,T;GR,,,,0;EN";
        public static string PullPen = "GA,,,,C;GR ,,,,3;PR ,,500;BGC;AMC";
        public static string DropPen = "PR ,,-500;BGC;AMC";// 线段结束后，关闭线段---然后落笔--开始下面的点
        public static string StartPolyLine = "VS10000;GA,,,,S;GR,,,,1.5";
        public static string EndPolyLine = "LE;AMS";
        public static string StartJump = "VS10000";
        public static string EndJump = "LE;BGS;AMS";
        public static string LeadLine = "LI-5000,0,0;LI0,15000,0;LI 20000,0,0;LI0,-20000,0;LI-15000,0,0;LI0,5000,0;BGS";

    }

    class Ini
    {

        private static double sDistance = 0;
        public static double SDistance
        {
            set { sDistance = value; }
        }

        private static double lastSDistance = 0;
        private static double executedSDistance = 0;
        public static double ExecutedSDistance
        {
            get
            {
                executedSSpeed = Math.Abs(sDistance - lastSDistance) / 100;
                executedSDistance += executedSSpeed / 10;
                lastSDistance = sDistance;
                return executedSDistance;
            }

        }

        private static double executedSSpeed = 0;
        public static double ExecutedSSpend
        {
            get
            {
                return executedSSpeed;
            }

        }

        private static int sendLineNumber = 0;
        public static int SendLineNumber
        {
            get { return sendLineNumber; }
            set { sendLineNumber = value; }
        }


        private static int executedLineNumber = 0;
        public static int ExecutedLineNumber
        {
            get
            {
                return executedLineNumber;
            }
            set { executedLineNumber = value; }
        }

        //private static bool restartLineNumber = false;
        private static int totalConsumedLineNumber = 0;
        private static int lastConsumedLineNumber = 0;
        private static int consumedLineNumber = 0;
        public static int ConsumedLineNumber
        {
            get
            {
                int i = Math.Abs(lastConsumedLineNumber - consumedLineNumber);
                lastConsumedLineNumber = consumedLineNumber;
                totalConsumedLineNumber += i;
                return totalConsumedLineNumber;
            }
            set { consumedLineNumber = value; }
        }

        private static int bufferedLineNumber = 0;
        public static int BufferedLineNumber
        {
            get
            {
                return bufferedLineNumber;
            }
            set { bufferedLineNumber = value; }
        }

        private static int leftBufferedLineNumber = 0;
        public static int LeftBufferedLineNumber
        {
            get
            {
                return leftBufferedLineNumber;
            }
            set { leftBufferedLineNumber = value; }
        }



        private static int printSpeed = 10000;
        public static double PrintSpeed
        {
            set { printSpeed = (int)(value * 1000); }
        }

        private static int jumpSpeed = 80000;
        public static int JumpSpeed
        {
            set { jumpSpeed = (int)(value * 1000); }
        }

        private static int pullDistance = 1000;
        public static int PullDistance  // mm to count
        {
            set { pullDistance = (int)(value * 1000); }
        }

        private static int pullSpeed = 10000;
        public static double PullSpeed  // mm to count
        {
            set { pullSpeed = (int)(value * 1000); }
        }

        private static int suckGearRatio = 20;
        public static int SuckAmount  // mm to count
        {
            set { suckGearRatio = value * 1000 / pullDistance; }
        }

        private static double gearRatio = 8; // 实际上应该是1.32 ul/s ,  用水测量是0.88 ul/10000脉冲，1.5的齿轮比，10mm（10000脉冲）/s的速度， 因此齿轮比 = 流量 X 3
        public static double FeedFlow
        {
            set { gearRatio = value * 64 * 3; } // *8
        }

        private static string startProgram;
        public static string StartProgram
        {
            get
            {
                startProgram = "# Program\r\nST\r\nCSS\r\nCS\r\nSH\r\nSP ,," + pullSpeed.ToString() + "\r\nDP0,0,0\r\nLMABC\r\n";
                return startProgram;
            }
        }

        private static string startProgramV;
        public static string StartProgramV
        {
            get
            {
                startProgramV = "# Program\r\nST\r\nCSS\r\nCS\r\nSH\r\nSP ,," + pullSpeed.ToString() + "\r\nDP0,0\r\nVMAB\r\n";
                return startProgramV;
            }
        }


        private static string endProgram;
        public static string EndProgram
        {
            get
            {
                endProgram = "ST\r\nGA,,,,T\r\nGR,,,,0\r\nEN";
                return endProgram;
            }
        }

        private static string pullPen;
        public static string PullPen
        {
            get
            {
                //pullPen = "GA,,,,C;GR ,,,," + suckGearRatio.ToString() + ";PR ,," + pullDistance.ToString() + ";BGC;AMC";
                pullPen = "GR ,,,,0\r\nPR ,," + pullDistance.ToString() + "\r\nBGC\r\nAMC\r\n";
                return pullPen;
            }
        }

        private static string dropPen;
        public static string DropPen
        {
            get
            {
                dropPen = "PR ,,-" + pullDistance.ToString() + "\r\nBGC\r\nAMC\r\n";
                return dropPen;
            }
        }

        private static string startPolyLine;
        public static string StartPolyLine
        {
            get
            {
                startPolyLine = "VS" + printSpeed.ToString() + "\r\nGA,,,S\r\nGR,,," + "0.02\r\n"; // gearRatio.ToString() + "\r\n";
                return startPolyLine;
            }
        }

        private static string startPolyLine1;
        public static string StartPolyLine1
        {
            get
            {
                startPolyLine1 = "VS" + printSpeed.ToString() + "\r\nGA,,,,S\r\nGR,,,," + "0.02\r\n"; // gearRatio.ToString() + "\r\n";
                return startPolyLine;
            }
        }

        private static string endPolyLine;
        public static string EndPolyLine
        {
            get
            {
                endPolyLine = "LI 0,1,0\r\nLE\r\nAMS\r\n";
                return endPolyLine;
            }
        }

        private static string endPolyLineV;
        public static string EndPolyLineV
        {
            get
            {
                endPolyLineV = "VE\r\nAMS\r\n";
                return endPolyLineV;
            }
        }

        private static string startJump;
        public static string StartJump
        {
            get
            {
                startJump = "VS8000\r\n";
                return startJump;
            }
        }

        private static string endJump;
        public static string EndJump
        {
            get
            {
                endJump = "LI 0,1,0\r\nLE;BGS\r\nAMS\r\n";
                return endJump;
            }
        }


        private static string leadLine;
        public static string LeadLine
        {
            get
            {
                // leadLine = "LI-5000,0,0\r\nLI0,15000,0\r\nLI 20000,0,0\r\nLI0,-20000,0\r\nLI-15000,0,0\r\nLI0,5000,0\r\nBGS\r\n";
                leadLine = "LI0,10000,0\r\nLI400,0,0\r\nLI0,-10000,0\r\nLI400,0,0\r\nLI0,10000,0\r\nLI400,0,0\r\nLI0,-10000,0\r\nLI400,0,0\r\n LI0,10000,0\r\nLI400,0,0\r\nLI0,-10000,0\r\nLI400,0,0\r\n LI0,10000,0\r\nLI400,0,0\r\nLI0,-10000,0\r\nLI400,0,0\r\nBGS\r\n";
                return leadLine;
            }
        }

        private static string leadLineV;
        public static string LeadLineV
        {
            get
            {
                leadLineV = "VP 0,15000\r\nVP 20000,15000\r\nVP 20000,-5000\r\nVP 0,-5000\r\nVP 0,0\r\nBGS\r\n";
                return leadLineV;
            }
        }

    }

}
