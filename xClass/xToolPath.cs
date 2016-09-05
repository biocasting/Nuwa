using System;
using System.Collections;
using System.Collections.Generic;
using ClipperLib;

namespace Nuwa.xClass
{
    using Polygon = List<IntPoint>;
    using Polygons = List<List<IntPoint>>;
    using Loops = List<xLoop>;
    using ToolPaths = List<xToolPath>;
    using PolyNodes = List<xPolynode>;

    public class xToolPath
    {

        #region 成员
        public Loops Borders;
        public Loops Paths;

        // 生成toolpath的辅助变量
        private Polygons subjects = new Polygons();
        private Polygons solution = new Polygons();
        public int Material;
        private double scale1 = 1000; 
        #endregion

        #region 属性

        #endregion

        #region 构造函数

        public xToolPath()
        {
            Paths = new Loops();
            Borders = new Loops();
            Material = 0;
        }

        #endregion

        #region Clipper
        private xLoop PolygonToLoop(Polygon pg, double scale)
        {
            xLoop result =new xLoop();
            for (int i = 0; i < pg.Count; i++)
            {
                xPoint2 pt = new xPoint2( (double)pg[i].X / scale, (double)pg[i].Y / scale) ;
                result.AddPoint(pt);
            }
            xPoint2 pt0 = new xPoint2((double)pg[0].X / scale, (double)pg[0].Y / scale);
            result.AddPoint(pt0);
            return result;
        }

        private Polygon  LoopToPolygon( xLoop lp, double scale)
        {
            Polygon result = new Polygon();
            for (int i = 0; i < lp.NumberOfPoints; i++)
            {
                IntPoint pt = new IntPoint(Convert.ToInt64( lp.GetPointAt(i).X * scale), Convert.ToInt64( lp.GetPointAt(i).Y * scale));
                result.Add(pt);
            }
            IntPoint pt0 = new IntPoint(Convert.ToInt64(lp.GetPointAt(0).X * scale), Convert.ToInt64(lp.GetPointAt(0).Y * scale));
            result.Add(pt0);
            return result;
        }

        public Loops GetLoopsbyOffset(double offset)
        {
            MakeCorrectWindings();
            foreach (xLoop pl in Borders)
            {
                Polygon pg = LoopToPolygon(pl, scale1);
                if (pg.Count > 2)
                    subjects.Add(pg);
            }

            Loops result = new Loops();
            if (offset != 0)
            {
                ClipperOffset co = new ClipperOffset();
                co.AddPaths(subjects, JoinType.jtRound, EndType.etClosedPolygon);
                co.Execute(ref solution, (double)offset * scale1);
            }

            foreach (Polygon pg in solution)
            {
                xLoop pts = PolygonToLoop(pg, scale1);
                if (pts.NumberOfPoints > 2)
                    result.Add(pts);
            }
            return result;
        }

        public void SetSubject()
        {
            //MakeCorrectWindings();
            foreach (xLoop pl in Borders)
            {
                Polygon pg = LoopToPolygon(pl, scale1);
                if (pg.Count > 2)
                    subjects.Add(pg);
                //pts = null;
            }
        }

        public Loops GetLoops(double offset)
        {
            Loops result = new Loops();
            if (offset != 0)
            {
                ClipperOffset co = new ClipperOffset();
                co.AddPaths(subjects, JoinType.jtRound, EndType.etClosedPolygon);
                co.Execute(ref solution, (double)offset * scale1*-1);
            }

            foreach (Polygon pg in solution)
            {
                xLoop pts = PolygonToLoop(pg, scale1);
                if (pts.NumberOfPoints > 2)
                    result.Add(pts);
            }
            return result;
        }

        #endregion

        public void Rearange(int option)
        {
            foreach (xLoop line in this.Borders)
            {
                line.InitIndexPoints();
                line.Rearrange(option);
            }
        }

        public ToolPaths GetTooPathsbyOffset(double offset)
        {
            xBoundry boundry = new xBoundry();
            boundry.AddLoops(this.GetLoopsbyOffset(-1 * offset));
            boundry.MarkOuterLoops(); ;
            ToolPaths tps = new ToolPaths();
            xToolPath tp = null;
            xLoop border = null;
            for (int i = 0; i < boundry.NumberOfLoops; i++) // 遍历各个Loop
            {
                xLoop loop = boundry.GetLoopAt(i);
                if (loop.isOuterLoop)
                {
                    tp = new xToolPath();
                    tps.Add(tp);
                    border = new xLoop();
                    tp.Borders.Add(border);
                }
                else
                {
                    border = new xLoop();
                    tp.Borders.Add(border);
                }

                for (int j = 0; j < loop.NumberOfPoints; j++) // 遍历Loop中各个点
                {
                    border.AddPoint(loop.GetPointAt(j));
                } // end for i
            }   // end for j
            return tps;
        }

        #region RasterPaths

        public Loops RasterPath(double offset, xVector2D xDirection)//return polylines
        {
            List<xPolynode> list = RasterFill(offset, xDirection);    // Polylinelist 是xLoop  ， currentList是 Polynode  Final list 是 List<Polyline>  sublist是 Polynode  List /RasterFill 是 List < Polynode >

            //make pairs

            xPolynode nodeBag = new xPolynode();
            for (int i = 0; i < list.Count; i++)
            {
                xPolynode subList = list[i];
                for (int j = 0; j < subList.NumberOfNodes / 2; j++)
                {
                    subList.GetNodeAt(2 * j).Pair = subList.GetNodeAt(2 * j + 1);
                }
                nodeBag.AddRange(subList.ToArray());
            }

            //pull out solutions
            List<xPolynode> returnList = new List<xPolynode>();
            List<xPolynode> unOptimized = new List<xPolynode>();
            for (int i = 0; i < nodeBag.NumberOfNodes; i++)
            {
                if (nodeBag.GetNodeAt(i).Visited)
                    continue;
                xPolynode onePolyline = new xPolynode();
                Node startN = nodeBag.GetNodeAt(i);
                //search up, then seach down
                Node upN = startN;
                while (upN.Above != null)
                {
                    if (upN.Above.Visited) break;
                    onePolyline.AddNode(upN.Above);
                    onePolyline.AddNode(upN.Above.Pair);
                    upN.Above.Visited = true;
                    upN.Above.Pair.Visited = true;
                    upN = upN.Above.Pair;
                }
                onePolyline.Reverse();
                Node downN = startN;
                onePolyline.AddNode(downN);
                onePolyline.AddNode(downN.Pair);
                downN.Visited = true;
                downN.Pair.Visited = true;
                downN = downN.Pair;
                while (downN.Below != null)
                {
                    if (downN.Below.Visited) break;
                    onePolyline.AddNode(downN.Below);
                    onePolyline.AddNode(downN.Below.Pair);
                    downN.Below.Visited = true;
                    downN.Below.Pair.Visited = true;
                    downN = downN.Below.Pair;
                }
                //optimize paths
                Node start = onePolyline.GetNodeAt(0);
                Node end = onePolyline.GetNodeAt(onePolyline.NumberOfNodes - 1);
                bool connected = false;
                for (int j = 0; j < unOptimized.Count; j++)
                {
                    xPolynode subList = unOptimized[j];
                    Node testStart = subList.GetNodeAt(0);
                    Node testEnd = subList.GetNodeAt(subList.NumberOfNodes - 1);
                    if (start.Below == testEnd.Pair)
                    {
                        //should test if subList is rearrangeable here
                        //rearrange subList
                        for (int k = 0; k < subList.NumberOfNodes / 2; k++)
                        {
                            Node temp = subList.GetNodeAt(2 * k);
                            subList.SetNode(2 * k, subList.GetNodeAt(2 * k + 1));
                            subList.SetNode(2 * k + 1, temp);
                        }
                        //subList+onepolyline
                        subList.AddRange(onePolyline.ToArray());
                        unOptimized.Remove(subList);

                        connected = true;
                        break;
                    }
                    else if (start.Pair.Below == testEnd.Pair)
                    {
                        //rearrange both subList
                        for (int k = 0; k < subList.NumberOfNodes / 2; k++)
                        {
                            Node temp = subList.GetNodeAt(2 * k);
                            subList.SetNode(2 * k, subList.GetNodeAt(2 * k + 1));
                            subList.SetNode(2 * k + 1, temp);
                        }
                        for (int k = 0; k < onePolyline.NumberOfNodes / 2; k++)
                        {
                            Node temp = onePolyline.GetNodeAt(2 * k);
                            onePolyline.SetNode(2 * k, onePolyline.GetNodeAt(2 * k + 1));
                            onePolyline.SetNode(2 * k + 1, temp);
                        }
                        //subList+polyline
                        subList.AddRange(onePolyline.ToArray());
                        unOptimized.Remove(subList);
                        connected = true;
                        break;
                    }

                    else if (testStart.Below == end.Pair)
                    {
                        //rearrange onePolyline
                        for (int k = 0; k < onePolyline.NumberOfNodes / 2; k++)
                        {
                            Node temp = onePolyline.GetNodeAt(2 * k);
                            onePolyline.SetNode(2 * k, onePolyline.GetNodeAt(2 * k + 1));
                            onePolyline.SetNode(2 * k + 1, temp);
                        }
                        onePolyline.AddRange(subList.ToArray());
                        subList = onePolyline;
                        unOptimized.Remove(subList);
                        connected = true;
                        break;
                        //onePolyline+subList
                    }
                    else if (testStart.Pair.Below == end.Pair)
                    {
                        //rearrange both onePolyline and subList
                        for (int k = 0; k < subList.NumberOfNodes / 2; k++)
                        {
                            Node temp = subList.GetNodeAt(2 * k);
                            subList.SetNode(2 * k, subList.GetNodeAt(2 * k + 1));
                            subList.SetNode(2 * k + 1, temp);
                        }
                        for (int k = 0; k < onePolyline.NumberOfNodes / 2; k++)
                        {
                            Node temp = onePolyline.GetNodeAt(2 * k);
                            onePolyline.SetNode(2 * k, onePolyline.GetNodeAt(2 * k + 1));
                            onePolyline.SetNode(2 * k + 1, temp);
                        }
                        //onePolyline+subList
                        onePolyline.AddRange(subList.ToArray());
                        subList = onePolyline;
                        unOptimized.Remove(subList);
                        connected = true;
                        break;
                    }
                }
                if (!connected)
                {
                    returnList.Add(onePolyline);
                    unOptimized.Add(onePolyline);
                }
            }
            Loops finalList = new Loops();
            for (int i = 0; i < returnList.Count; i++)
            {
                xLoop polylineList = new xLoop();
                xPolynode currentList = returnList[i];
                for (int j = 0; j < currentList.NumberOfNodes; j++)
                {
                    polylineList.AddPoint(new xPoint2(currentList.GetNodeAt(j).X, currentList.GetNodeAt(j).Y)); // Polylinelist 是Polyline  ， currentList是 List<Node>  Final list 是 List<Polyline>
                }
                if (polylineList.NumberOfPoints >= 2)
                    finalList.Add(polylineList);
            }
            return finalList;
        }

        // Polylinelist 是Polyline, currentList是 Polynode, Final list 是 List<Polyline>, sublist是 Polynode  List,RasterFill 是 List < Polynode >
        public PolyNodes RasterFill(double offset, xVector2D xDirection)
        {
            xToolPath poly = new xToolPath();
            xTransform tran = new xTransform(new xPoint2(0.0, 0.0), xDirection);
            for (int i = 0; i < Borders.Count; i++)
            {
                xLoop sublist = Borders[i];
                xLoop looplist = new xLoop();
                for (int j = 0; j < sublist.NumberOfPoints; j++)
                {
                    looplist.AddPoint(tran.To(sublist.GetPointAt(j)));
                }
                poly.Add(looplist.ToArray());
            }

            PolyNodes list = poly.RasterFill(offset);
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].NumberOfNodes; j++)
                {
                    Node n = list[i].GetNodeAt(j);
                    xPoint2 p = tran.Back(new xPoint2(n.X, n.Y));
                    n.X = p.X; n.Y = p.Y;
                }
            }
            return list;
        }   


        public PolyNodes RasterFill(double offset)
        {
            //从顶部创建Nodes
            xPolygon sp = new xPolygon(this.Borders[0].ToArray());
            double minY = sp.LowerCorner.Y, maxY = sp.UpperCorner.Y;
            PolyNodes chainList = new PolyNodes();
            // 计算总共扫描线数量n，产生n个Polynode 的列表
            int totalLines = (int)((maxY - minY) / offset);
            for (int i = 0; i < totalLines; i++)
                chainList.Add(new xPolynode());
            //从顶部到底部, 扫描外周 polygon， 不包括顶点

            // 在外loop中搜索
            #region search outer loop

            bool downSearching = true;
            double y0 = maxY;
            int level = -1;
            Node lastNode = null;

            // 搜索每一个点
            for (int count = 0; count < sp.Points.Length; count++)
            {
                int i = (count + sp.IndexOfTop + 1) % sp.Points.Length;
                xPoint2 currentP = sp.Points[i];
                xPoint2 previousP = sp.Points[(i - 1 + sp.Points.Length) % sp.Points.Length];
                // ------------------------------向上搜索-------------------------------------------------
                if (currentP.Y > previousP.Y)//向上搜索
                {
                    if (downSearching)
                    {
                        y0 -= offset;
                        level++;
                        lastNode = null;
                        downSearching = false;
                    }
                    //排除最后一个点
                    int solutions = (int)((currentP.Y - y0) / offset);
                    if (solutions <= 0)
                    {
                        continue;
                    }
                    //有至少一个结果
                    double factor = (currentP.X - previousP.X) / (currentP.Y - previousP.Y);
                    for (int j = 0; j < solutions; j++)
                    {
                        if (level == 0)//不算最顶上的点
                        {
                            solutions--;
                            break;
                        }
                        level--;
                        double y = (j + 1) * offset + y0;
                        double x = (y - previousP.Y) * factor + previousP.X;
                        Node n = new Node(x, y);
                        n.Below = lastNode;
                        if (lastNode != null)
                        {
                            lastNode.Above = n;
                        }
                        lastNode = n;
                        chainList[level].AddNode(n);
                    }
                    y0 += solutions * offset;
                }
                // ------------------------------向下搜索-------------------------------------------------
                else if (currentP.Y < previousP.Y)//向下搜索
                {
                    if (!downSearching)
                    {
                        y0 += offset;
                        level--;
                        lastNode = null;
                        downSearching = true;
                    }
                    int solutions = (int)((y0 - currentP.Y) / offset);
                    if (solutions <= 0)
                    {
                        continue;
                    }
                    //有至少一个结果
                    double factor = (currentP.X - previousP.X) / (currentP.Y - previousP.Y);
                    for (int j = 0; j < solutions; j++)
                    {
                        level++;
                        double y = y0 - (j + 1) * offset;
                        double x = (y - previousP.Y) * factor + previousP.X;
                        Node n = new Node(x, y);
                        n.Above = lastNode;
                        if (lastNode != null)
                        {
                            lastNode.Below = n;
                        }
                        lastNode = n;
                        chainList[level].AddNode(n);
                    }
                    y0 -= solutions * offset;
                }
                else
                {
                    //ignore
                }
            }
            #endregion

            #region search inner loops

            for (int index = 1; index < Borders.Count; index++)
            {
                xPolygon sp0 = new xPolygon(Borders[index].ToArray());
                downSearching = true;
                int ii = (int)((maxY - sp0.UpperCorner.Y) / offset);
                y0 = maxY - ii * offset;
                level = -1 + ii;
                lastNode = null;

                for (int count = 0; count < sp0.Points.Length; count++)
                {
                    int i = (count + sp0.IndexOfTop + 1) % sp0.Points.Length;
                    xPoint2 currentP = sp0.Points[i];
                    xPoint2 previousP = sp0.Points[(i - 1 + sp0.Points.Length) % sp0.Points.Length];
                    if (currentP.Y > previousP.Y)//Searching up
                    {
                        if (downSearching)
                        {
                            y0 -= offset;
                            level++;
                            lastNode = null;
                            downSearching = false;
                        }
                        //exclude last point
                        int solutions = (int)((currentP.Y - y0) / offset);
                        if (solutions <= 0)
                        {
                            continue;
                        }
                        double factor = (currentP.X - previousP.X) / (currentP.Y - previousP.Y);
                        for (int j = 0; j < solutions; j++)
                        {
                            if (level == ii)//do not count the top point
                            {
                                solutions--;
                                break;
                            }
                            level--;
                            double y = (j + 1) * offset + y0;
                            double x = (y - previousP.Y) * factor + previousP.X;
                            Node n = new Node(x, y);
                            n.Below = lastNode;
                            if (lastNode != null)
                            {
                                lastNode.Above = n;
                            }
                            lastNode = n;
                            chainList[level].AddNode(n);
                        }
                        y0 += solutions * offset;
                    }
                    else if (currentP.Y < previousP.Y)//searching down
                    {
                        if (!downSearching)
                        {
                            y0 += offset;
                            level--;
                            lastNode = null;
                            downSearching = true;
                        }
                        int solutions = (int)((y0 - currentP.Y) / offset);
                        if (solutions <= 0)
                        {
                            continue;
                        }
                        //has at least one solution
                        double factor = (currentP.X - previousP.X) / (currentP.Y - previousP.Y);
                        for (int j = 0; j < solutions; j++)
                        {
                            level++;
                            double y = y0 - (j + 1) * offset;
                            double x = (y - previousP.Y) * factor + previousP.X;
                            Node n = new Node(x, y);
                            n.Above = lastNode;
                            if (lastNode != null)
                            {
                                lastNode.Below = n;
                            }
                            lastNode = n;
                            chainList[level].AddNode(n);
                        }
                        y0 -= solutions * offset;
                    }
                    else
                    {
                        //ignore
                    }
                }
            }
            #endregion

            for (int i = 0; i < chainList.Count; i++)
            {
                // 按照X大小排列
                chainList[i].nodes.Sort((Comparison<Node>)delegate(Node a, Node b) { return a.X > b.X ? 1 : a.X < b.X ? -1 : 0; });
            }
            return chainList;
        }

        # endregion

        # region 类简单方法

        public void Add(xPoint2[] points)
        {
            xPolygon sp = new xPolygon(points);
            xLoop polyline = new xLoop();
            if (Borders.Count == 0)//first polygon, CW, outerline
            {
                if (sp.Winding == Winding.CW)
                {
                    polyline.AddPoints(sp.Points);
                }
                else
                {
                    polyline.AddPoints(sp.Inverse().Points);
                }
            }
            else
            {
                if (sp.Winding == Winding.CCW)
                {
                    polyline.AddPoints(sp.Points);
                }
                else
                {
                    polyline.AddPoints(sp.Inverse().Points);
                }
            }
            this.Borders.Add(polyline);
        }

        public void MakeCorrectWindings()
        {
            if (this.Borders.Count > 0)
            {
                for (int i = 0; i < this.Borders.Count; i++)
                {
                    xPolygon spn = new xPolygon(this.Borders[i].ToArray());
                    if (i == 0)
                    {
                        if (spn.Winding == Winding.CCW)
                            Borders[i].Reverse();
                    }
                    else
                    {
                        if (spn.Winding == Winding.CW)
                            Borders[i].Reverse();
                    }
                }
            }
        }

        # endregion

    }//end class


}
