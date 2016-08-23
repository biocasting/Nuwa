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

    /// <summary>
    /// xPolygons的列表, 第一个是outer boundary,  xToolPaths -- xPolygon -- xPoint2D
    /// </summary>
    public class xToolPath
    {

        #region 成员
        public Loops Borders;
        public Loops Paths;
        private Polygons subjects = new Polygons();
        private Polygons solution = new Polygons();
        public int Material;
        private double scale1 = 10000; 
        #endregion

        #region 属性

        public xPolygon Polygon
        {
            get
            {
                return SimplePolygon();
            }
        }

        private xPolygon SimplePolygon()
        {
            MakeCorrectWindings();
            if (Borders.Count == 0)
                throw new Exception("No polygon found");
            else if (Borders.Count == 1)
                return new xPolygon(Borders[0].ToArray());
            else
            {
                this.Borders.Sort((Comparison<xLoop>)delegate(xLoop a, xLoop b) { return a.MaxY < b.MaxY ? 1 : a.MaxY == b.MaxY ? 0 : -1; });
                xPolygon returnPoly = new xPolygon(Borders[0].ToArray());
                for (int i = 1; i < this.Borders.Count; i++)
                {
                    xPolygon spoly = CombinePolygons(returnPoly, new xPolygon(Borders[i].ToArray()));
                    returnPoly = spoly;
                }
                return returnPoly;
            }
        }

        public int NumberOfPolylines
        {
            get { return this.Borders.Count; }

        }

        public int TotalVertices
        {
            get
            {
                int vt = 0;
                for (int i = 0; i < Borders.Count; i++)
                    vt += (Borders[i]).NumberOfPoints;
                return vt;
            }
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public xToolPath()
        {
            Paths = new Loops();
            Borders = new Loops();
            Material = 0;
        }

        public void Rearange(int option)
        {
            foreach (xLoop line in this.Borders)
            {
                line.InitIndexPoints();
                line.Rearrange(option);
            }
        }

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

        public  Loops GetLoopsbyOffset(double offset)
        {
            //MakeCorrectWindings();
            foreach (xLoop pl in Borders)
            {
                Polygon pg= LoopToPolygon(pl, scale1);
                if (pg.Count > 2)
                    subjects.Add(pg);
                //pts = null;
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
                    //pts = null;
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
                //pts = null;
            }
            return result;
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

        public List<xPolynode> RasterFill(double offset, xVector2D xDirection)
        {
            xToolPath poly = new xToolPath();
            xTransform tran = new xTransform(new xPoint2(0.0, 0.0), xDirection);
            for (int i = 0; i < Borders.Count; i++)
            {
                xLoop sublist = Borders[i];
                ArrayList looplist = new ArrayList();
                for (int j = 0; j < sublist.NumberOfPoints; j++)
                {
                    looplist.Add(tran.To(sublist.GetPointAt(j)));
                }
                poly.Add(looplist);
            }

            List<xPolynode> list = poly.RasterFill(offset);
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
        }   // Polylinelist 是Polyline, currentList是 Polynode, Final list 是 List<Polyline>, sublist是 Polynode  List,RasterFill 是 List < Polynode >

        public List<xPolynode> RasterFill(double offset)
        {
            //从顶部创建Nodes
            xPolygon sp = new xPolygon(this.Borders[0].ToArray());
            double minY = sp.LowerCorner.Y, maxY = sp.UpperCorner.Y;
            List<xPolynode> chainList = new List<xPolynode>();
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

        # region ContourPaths

        public Loops ContourPath(double firstDepth, double offset)
        {
            return this.Polygon.ContourPath(firstDepth, offset);
        }

        # endregion

        # region 类简单方法

        private void updateBound()
        {
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;
            for (int i = 0; i < Borders.Count; i++)
            {
                xLoop list = Borders[i];
                for (int j = 0; j < list.NumberOfPoints; j++)
                {
                    xPoint2 p = list.GetPointAt(j);
                    minX = p.X > minX ? minX : p.X;
                    minY = p.Y > minY ? minY : p.Y;
                    maxX = p.X > maxX ? p.X : maxX;
                    maxY = p.Y > maxY ? p.Y : maxY;
                }
            }
        }

        public void Add(params xPolygon[] polys)
        {
            for (int i = 0; i < polys.Length; i++)
            {
                xLoop polyline = new xLoop();
                if (Borders.Count == 0)//first polygon, CW, outerline
                {
                    if (polys[i].Winding == Winding.CW)
                    {
                        polyline.AddPoints(polys[i].Points);
                    }
                    else
                    {
                        polyline.AddPoints(polys[i].Inverse().Points);
                    }
                }
                else
                {
                    if (polys[i].Winding == Winding.CCW)
                    {
                        polyline.AddPoints(polys[i].Points);
                    }
                    else
                    {
                        polyline.AddPoints(polys[i].Inverse().Points);
                    }
                }
                Borders.Add(polyline);
            }
        }

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

        public void Add(ArrayList points) //arrayList of points
        {
            Add((xPoint2[])points.ToArray(typeof(xPoint2)));
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

        private xPolygon CombinePolygons(xPolygon outerLoop, xPolygon innerLoop)
        {
            //从内环的顶点发一条射线，看看是否与外环相交，相交的话，返回外环交点的index
            
            xPoint2 innerTopPoint = innerLoop.Points[innerLoop.IndexOfTop]; // 顶点
            List<int> UpInterscetion = new List<int>();

            //向上发一条放射性,离放射性最近的点，可以认为是交点
            for (int i = 0; i < outerLoop.NumberOfPoints; i++)
            {
                xPoint2 p0 = outerLoop.Points[i];
                xPoint2 p1 = outerLoop.Points[(i + 1) % outerLoop.NumberOfPoints];
                if (p0.Y > innerTopPoint.Y || p1.Y > innerTopPoint.Y)
                {
                    if (!((innerTopPoint.X < p0.X && innerTopPoint.X < p1.X) || (innerTopPoint.X > p0.X && innerTopPoint.X > p1.X)))
                    {
                        UpInterscetion.Add(i);
                    }
                }
            }
            // 向下发一条直线
            //for (int i = 0; i < outerLoop.NumberOfPoints; i++)
            //{
            //    xPoint2D p0 = outerLoop.Points[i];
            //    xPoint2D p1 = outerLoop.Points[(i + 1) % outerLoop.NumberOfPoints];
            //    if (p0.Y < innerTopPoint.Y || p1.Y < innerTopPoint.Y)
            //    {
            //        if (!((innerTopPoint.X < p0.X && innerTopPoint.X < p1.X) || (innerTopPoint.X > p0.X && innerTopPoint.X > p1.X)))
            //        {
            //            candidateIndices.Add(i);
            //        }
            //    }
            //}

            //不交，则有问题。 
            if (UpInterscetion.Count == 0)
                throw new Exception("Algorithm wrong on making a complex polygon simple");
            Console.WriteLine("交点数目 {0}  1: {1}   点: X{2}，Y{3} ", UpInterscetion.Count, UpInterscetion[0], outerLoop.Points[UpInterscetion[0]].X, outerLoop.Points[UpInterscetion[0]].Y);

            // 获得第一个交点，
            int intersectionIndex = UpInterscetion[0];
            bool addsplit = false;
            xPoint2 OuterStartPoint = outerLoop.Points[intersectionIndex]; // 放射性左面的点
            xPoint2 OuterEndPoint = outerLoop.Points[(intersectionIndex + 1) % outerLoop.NumberOfPoints]; // 放射性右面的点
            double MiddleX= innerTopPoint.X;
            double MiddleY = (innerTopPoint.X - OuterStartPoint.X) / (OuterEndPoint.X - OuterStartPoint.X) * (OuterEndPoint.Y - OuterStartPoint.Y) + OuterStartPoint.Y;
            xPoint2 outerMiddlePoint = new xPoint2(MiddleX, MiddleY);// 两个点的中点

            #region  获取交点的Y值

            double y;
            // 如果两点X相同，证明平行于Y轴，可能在边线上
            if (OuterStartPoint.X == OuterEndPoint.X)
            {
                // Y选下面那个点
                if (OuterStartPoint.Y > OuterEndPoint.Y)
                    y = OuterEndPoint.Y;
                else
                    y = OuterStartPoint.Y;
            }
            else
            {
                // Y为交点。
                y = (innerTopPoint.X - OuterStartPoint.X) * (OuterEndPoint.Y - OuterStartPoint.Y) / (OuterEndPoint.X - OuterStartPoint.X) + OuterStartPoint.Y;
            }

            // 获得其他交点的Y坐标，与前面比较，获得最小的Y值
            for (int i = 1; i < UpInterscetion.Count; i++)
            {
                int index = UpInterscetion[i];
                int indexN = (index + 1) % outerLoop.NumberOfPoints;

                xPoint2 OuterStartPoint1 = outerLoop.Points[index];
                xPoint2 OuterEndPoint1 = outerLoop.Points[indexN];
                double y1;
                if (OuterStartPoint1.X == OuterEndPoint1.X)
                {
                    if (OuterStartPoint1.Y > OuterEndPoint1.Y) y1 = OuterEndPoint.Y;
                    else y1 = OuterStartPoint1.Y;
                }
                else
                {
                    y1 = (innerTopPoint.X - OuterStartPoint.X) * (OuterEndPoint1.Y - OuterStartPoint.Y) / (OuterEndPoint1.X - OuterStartPoint1.X) + OuterStartPoint1.Y;
                }
                if (y1 < y)
                {
                    intersectionIndex = UpInterscetion[i];
                    y = y1;
                }
            }
            # endregion

            //get solution alread here, three cases: solution on start end of the line, on end of the line, or intersection
            List<xPoint2> list = new List<xPoint2>();
            // 交点在起点
            if (OuterStartPoint.X == innerTopPoint.X && OuterStartPoint.Y == y)
            {
                //落在起点，不要更改
            }
            // 交点在终点
            else if (OuterEndPoint.X == innerTopPoint.X && OuterEndPoint.Y == y)
            {
                intersectionIndex = (intersectionIndex + 1) % outerLoop.NumberOfPoints;
            }
            // 交点在交叉点
            else
            {
                intersectionIndex = (intersectionIndex + 1) % outerLoop.NumberOfPoints;
                addsplit = true;
            }

            // 将所有的点串起来  外环交点中点-->外环终点-->外环起点-->外环交叉点-->外环交点中点--->内环顶点--->内环终点--->内环起点--->内环顶点-
            if (addsplit)
                list.Add(outerMiddlePoint);
            for (int i = intersectionIndex; i < outerLoop.NumberOfPoints; i++)
            {
                list.Add(outerLoop.Points[i]);
            }
            for (int i = 0; i < intersectionIndex; i++)
            {
                list.Add(outerLoop.Points[i]);
            }
            if (addsplit)
                list.Add(outerMiddlePoint);
            for (int i = innerLoop.IndexOfTop; i < innerLoop.NumberOfPoints; i++)
            {
                list.Add(innerLoop.Points[i]);
            }
            for (int i = 0; i <= innerLoop.IndexOfTop; i++)
            {
                list.Add(innerLoop.Points[i]);
            }

            Console.Write("Split 1 : X {0} Y {1} \n", outerMiddlePoint.X, outerMiddlePoint.Y);
            Console.Write("Solution : X {0} Y {1} \n", outerLoop.Points[intersectionIndex].X, outerLoop.Points[intersectionIndex].Y);

            return new xPolygon(list.ToArray());
        }

        # endregion

    }//end class

    ////Top-down search for intersections, start from 
    //public class NodeComparer : IComparer
    //{
    //    public int Compare(Node a, Node b)
    //    {
    //        if (a.X > b.X)
    //            return 1;
    //        else if (a.X < b.X)
    //            return -1;
    //        else
    //        {
    //            return 0;
    //        }
    //    }
    //}

}
