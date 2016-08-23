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
}
