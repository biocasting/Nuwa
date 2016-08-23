/*
* ==============================================================================
* 文件名称: xPolygon.cs
* 文件描述:  关于polygon算法的类文件
* 版本: 1.0
* 生产时间: $time$
* 编译器: Visual Studio 2013
* 作者: 谢宝军
* 公司: 百廷三维
* ==============================================================================
*/

using System;
using System.Collections.Generic;
//using System.Collections;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuwa.xClass
{

    //*************************************************************xPolygon****************************************************************************************

    public class xPolygon
    {
        # region 成员
        public xPoint2[] Points;	//points for the polygon
        private xSite[] sites;
        private xBNode voronoiRoot;
        public Winding winding;//CCW for hole, CW for outer (true polygon)

        private int indexOfTop;//yMax
        private int indexOfBottom; //yMin
        private int indexOfLeft; //xMin
        private int indexOfRight; //xMax
        double maxX, maxY, minX, minY;

        #endregion

        #region 属性

        public int NumberOfPoints
        {
            get { return this.Points.Length;  }
        }

        public Winding Winding
        {
            get { return this.winding; }
        }

        public int IndexOfTop
        {
            get { return this.indexOfTop; }
        }

        public int IndexOfBottom
        {
            get { return this.indexOfBottom; }
        }

        public int IndexOfLeft
        {
            get { return this.indexOfLeft; }
        }

        public int IndexOfRight
        {
            get { return this.indexOfRight; }
        }

        public xBNode VoronoiRoot
        {
            get
            {
                if (voronoiRoot == null)
                {
                    voronoiRoot = Voronoi();
                }
                return voronoiRoot;
            }
        }

        public xSite[] xSites
        {
            get
            {
                if (sites == null)
                {
                    //create site array
                    List<xSite> siteArray = new List<xSite>();
                    int convexNum = 0;
                    for (int i = 0; i < Points.Length; i++)
                    {
                        xVector2D uv1, uv2;
                        xPoint2 pp = Points[(i - 1 + Points.Length) % Points.Length];	//previous point
                        xPoint2 pn = Points[(i + 1) % Points.Length];			//next point
                        xPoint2 pc = Points[i];						//current point
                        uv1 = new xVector2D(pp, pc); //not neccesary to be a unit vector
                        uv2 = new xVector2D(pc, pn);
                        if (uv1.Cross(uv2) < 0 && Winding == Winding.CCW
                            || uv1.Cross(uv2) > 0 && Winding == Winding.CW) //>180 case
                        {
                            siteArray.Add(new xSite(pc));
                            convexNum++;
                        }
                        siteArray.Add(new xSite(new xLine2D(pc, pn)));
                    }
                    sites = new xSite[siteArray.Count];
                    sites = siteArray.ToArray();
                    for (int i = 0; i < sites.Length; i++)
                    {
                        sites[i].Index = i;
                        sites[i].Previous = sites[(i - 1 + sites.Length) % sites.Length];
                        sites[i].Next = sites[(i + 1) % sites.Length];
                    }
                }
                return sites;
            }
        }

        public xPoint2 LowerCorner
        {
            get { return new xPoint2(minX, minY); }
        }

        public xPoint2 UpperCorner
        {
            get { return new xPoint2(maxX, maxY); }
        }

        #endregion

        public xPolygon(xPoint2[] inPoints)
        {
            this.Points = SmallLengthAngleFilter(inPoints);
            //if (Points.Length < 3)
            //    throw new Exception("A polygon has at leat 3 vertices");
            ////test self intersection
            //if(selfIntersecting(Points))
            //	   MessageBox.Show("Self-intersecting polygon");
            InitializeComponent();
        }

        private void InitializeComponent()
        {    
            voronoiRoot = null;
            sites = null;
            InitIndexPoints();
            InitWinding();
        }

        public void InitIndexPoints()
        {

            this.maxX = this.minX = Points[0].X;
            this.maxY = this.minY = Points[0].Y;

            this.indexOfTop = 0;
            this.indexOfBottom = 0;
            this.indexOfLeft = 0;
            this.indexOfRight = 0;
            for (int i = 0; i < Points.Length; i++)
            {
                if (Points[i].X < minX)
                {
                    this.minX = Points[i].X;
                    this.indexOfLeft = i;
                }
                if (Points[i].X > maxX)
                {
                    this.maxX = Points[i].X;
                    this.indexOfRight = i;
                }
                if (Points[i].Y < minY)
                {
                    this.minY = Points[i].Y;
                    this.indexOfBottom = i;
                }
                if (Points[i].Y > maxY)
                {
                    this.maxY = Points[i].Y;
                    this.indexOfTop = i;
                }
            }

        }  // 找到Polygon的最大最小X,Y值， 以及对应的点位置

        private void InitWinding()
        {
             xPoint2 pp = Points[(IndexOfTop - 1 + Points.Length) % Points.Length];  // indexofTop 前面的点
            xPoint2 pc = Points[IndexOfTop];
            xPoint2 pn = Points[(IndexOfTop + 1) % Points.Length];// indexofTop 后面的点
            xVector2D v1 = new xVector2D(pp, pc).Unit;
            xVector2D v2 = new xVector2D(pc, pn).Unit;
            // 如果积为正，则逆时针
            double windingCrossProduct = v1.Cross(v2);
            if (windingCrossProduct > 0)
                this.winding = Winding.CCW; //CCW
            else
                this.winding = Winding.CW;

        }

        public List<xLoop> ContourPath(double firstOffset, double offset)
        {
            if (offset == 0)
                throw new Exception("offset can't be zero");
            List<List<xContour>> returnList = new List<List<xContour>>();    // Returnlist: List<Polyline> ; firstLoops List<xContour>;
            List<xContour> firstLoops = this.ContoursAt(firstOffset);
            //if (firstLoops.Count == 0) 
            // return (List<Polyline>)returnList ;  // -----------------------------------------------
            returnList.Add(firstLoops);
            for (double d = firstOffset + offset; ; d += offset)
            {
                List<xContour> polyline = this.ContoursAt(d);
                if (polyline.Count == 0) break;
                else returnList.Add(polyline);
            }
            //distribute to each site
            for (int i = 0; i < returnList.Count; i++)
            {
                List<xContour> levelList = returnList[i];
                for (int j = 0; j < levelList.Count; j++)
                {
                    List<xContourNode> contourList = levelList[j].ContourNodes;
                    if (contourList.Count < 2)
                        continue;
                    xContour ct = new xContour(contourList);
                    for (int k = 0; k < contourList.Count; k++)
                    {
                        xContourNode cn = (xContourNode)contourList[k];
                        if (cn.S1.Contours.Count == 0)
                        {
                            cn.S1.Contours.Add(ct);
                        }
                        else
                        {
                            if (!(cn.S1.Contours[cn.S1.Contours.Count - 1]).Equals(ct))
                                cn.S1.Contours.Add(ct);
                        }
                        if (cn.S2.Contours.Count == 0)
                            cn.S2.Contours.Add(ct);
                        else
                        {
                            if (!(cn.S2.Contours[cn.S2.Contours.Count - 1]).Equals(ct))
                                cn.S2.Contours.Add(ct);
                        }
                    }
                }
            }

            List<xLoop> finalList = new List<xLoop>();
            //distribute to final solutions
            for (int i = 0; i < sites.Length; i++)
            {
                List<xContour> ctList = sites[i].Contours;
                for (int j = 0; j < ctList.Count; j++)
                {
                    xContour ct = ctList[j];
                    if (ct.Visited) continue;
                    ct.Visited = true;
                    xLoop polylineList = new xLoop();    // PolyLineList  :  List<Polyline>  ; 
                    for (int k = 0; k < ct.ContourNodes.Count; k++)
                    {
                        xContourNode cn1 = ct.ContourNodes[k];
                        xContourNode cn2 = ct.ContourNodes[(k + 1) % ct.ContourNodes.Count];
                        //find common site
                        xSite s;
                        if (cn1.S1 == cn2.S2 || cn1.S1 == cn2.S1)
                            s = cn1.S1;
                        else
                            s = cn1.S2;
                        if (s.Content is xLine2D)
                            polylineList.AddPoint(ct.ContourNodes[k].P);
                        else// insert more interpolated poitns.It is actuall an arc
                        {
                            //make sure error is less than 1/10 of offset
                            xPoint2 p0 = (xPoint2)s.Content;
                            xPoint2 p1 = cn1.P;
                            xPoint2 p2 = cn2.P;
                            double length = p0.DistanceTo(p1);
                            polylineList.AddPoint(p1);
                            if (length > 0.1 * offset)
                            {
                                double deltaAlpha = Math.Acos(1 - 0.1 * offset / length) * 2.0;
                                double beta = -(new xVector2D(p0, p1).AngleTo(new xVector2D(1, 0)));
                                double between;
                                if (this.Winding == Winding.CCW)
                                {
                                    between = new xVector2D(p0, p2).AngleTo(new xVector2D(p0, p1));
                                }
                                else
                                {
                                    between = new xVector2D(p0, p1).AngleTo(new xVector2D(p0, p2));
                                }
                                for (int m = 1; m < between / deltaAlpha; m++)
                                {
                                    double alpha;
                                    if (this.Winding == Winding.CCW)
                                        alpha = beta - (double)m * deltaAlpha;
                                    else
                                        alpha = beta + (double)m * deltaAlpha;
                                    double x = length * Math.Cos(alpha) + p0.X;
                                    double y = length * Math.Sin(alpha) + p0.Y;
                                    polylineList.AddPoint(new xPoint2(x, y));
                                }
                            }
                        }
                    }//end k				
                    if (polylineList.NumberOfPoints >= 2)
                    {
                        polylineList.AddPoint(polylineList.GetPointAt(0));//make it closed
                        finalList.Add(polylineList);
                    }
                }//endj
            }//end i
            return finalList;
        }

        public List<xLoop> ContourPathFirstoffset(double firstOffset)
        {
                 List<xContour> firstLoops = this.ContoursAt(firstOffset);
                List<xContour> levelList = firstLoops;
                for (int j = 0; j < levelList.Count; j++)
                {
                    List<xContourNode> contourList = levelList[j].ContourNodes;
                    if (contourList.Count < 2)
                        continue;
                    xContour ct = new xContour(contourList);
                    for (int k = 0; k < contourList.Count; k++)
                    {
                        xContourNode cn = (xContourNode)contourList[k];
                        if (cn.S1.Contours.Count == 0)
                        {
                            cn.S1.Contours.Add(ct);
                        }
                        else
                        {
                            if (!(cn.S1.Contours[cn.S1.Contours.Count - 1]).Equals(ct))
                                cn.S1.Contours.Add(ct);
                        }
                        if (cn.S2.Contours.Count == 0)
                            cn.S2.Contours.Add(ct);
                        else
                        {
                            if (!(cn.S2.Contours[cn.S2.Contours.Count - 1]).Equals(ct))
                                cn.S2.Contours.Add(ct);
                        }
                    }
                }

            List<xLoop> finalList = new List<xLoop>();
            //distribute to final solutions
            for (int i = 0; i < sites.Length; i++)
            {
                List<xContour> ctList = sites[i].Contours;
                for (int j = 0; j < ctList.Count; j++)
                {
                    xContour ct = ctList[j];
                    if (ct.Visited) continue;
                    ct.Visited = true;
                    xLoop polylineList = new xLoop();    // PolyLineList  :  List<Polyline>  ; 
                    for (int k = 0; k < ct.ContourNodes.Count; k++)
                    {
                        xContourNode cn1 = ct.ContourNodes[k];
                        xContourNode cn2 = ct.ContourNodes[(k + 1) % ct.ContourNodes.Count];
                        //find common site
                        xSite s;
                        if (cn1.S1 == cn2.S2 || cn1.S1 == cn2.S1)
                            s = cn1.S1;
                        else
                            s = cn1.S2;
                        if (s.Content is xLine2D)
                            polylineList.AddPoint(ct.ContourNodes[k].P);
                        else// insert more interpolated poitns.It is actuall an arc
                        {
                            //make sure error is less than 1/10 of offset
                            xPoint2 p0 = (xPoint2)s.Content;
                            xPoint2 p1 = cn1.P;
                            xPoint2 p2 = cn2.P;
                            double length = p0.DistanceTo(p1);
                            polylineList.AddPoint(p1);
                        }
                    }//end k				
                    if (polylineList.NumberOfPoints >= 2)
                    {
                        polylineList.AddPoint(polylineList.GetPointAt(0));//make it closed
                        finalList.Add(polylineList);
                        //System.Windows.Forms.MessageBox.Show("plyline" + j.ToString());
                    }
                }//endj
                
            }//end i
            //System.Windows.Forms.MessageBox.Show("finalList" + finalList.Count.ToString());
            return finalList;
        }

        //public List<xLoop> ContourPathOuter(double firstOffset, double offset, int outerCircileNumber)
        //{
        //    if (offset == 0)
        //        throw new Exception("offset can't be zero");

        //    List<List<xContour>> returnList = new List<List<xContour>>();    // Returnlist: List<Polyline> ; firstLoops List<xContour>;
        //    List<xContour> firstLoops = this.ContoursAt(firstOffset);
            
        //    //if (firstLoops.Count == 0) 
        //    // return (List<Polyline>)returnList ;  // -----------------------------------------------
        //    returnList.Add(firstLoops);
            
        //    for (double d = firstOffset + offset; ; d += offset)
        //    {
        //        List<xContour> polyline = this.ContoursAt(d);
        //        if (polyline.Count == 0) break;
        //        else returnList.Add(polyline);
        //    }
            
        //    //distribute to each site
        //    //for (int i = 0; i < returnList.Count; i++)
        //    int temp = (returnList.Count < outerCircileNumber) ? returnList.Count : outerCircileNumber;
        //    for (int i = 0; i < temp; i++)
        //    {
        //        List<xContour> levelList = returnList[i];
        //        for (int j = 0; j < levelList.Count; j++)
        //        {
        //            List<xContourNode> contourList = levelList[j].ContourNodes;
        //            if (contourList.Count < 2)
        //                continue;
        //            xContour ct = new xContour(contourList);
        //            for (int k = 0; k < contourList.Count; k++)
        //            {
        //                xContourNode cn = (xContourNode)contourList[k];
        //                if (cn.S1.Contours.Count == 0)
        //                {
        //                    cn.S1.Contours.Add(ct);
        //                }
        //                else
        //                {
        //                    if (!(cn.S1.Contours[cn.S1.Contours.Count - 1]).Equals(ct))
        //                        cn.S1.Contours.Add(ct);
        //                }
        //                if (cn.S2.Contours.Count == 0)
        //                    cn.S2.Contours.Add(ct);
        //                else
        //                {
        //                    if (!(cn.S2.Contours[cn.S2.Contours.Count - 1]).Equals(ct))
        //                        cn.S2.Contours.Add(ct);
        //                }
        //            }
        //        }
        //    }

        //    List<xLoop> finalList = new List<xLoop>();
        //    //distribute to final solutions
        //    for (int i = 0; i < sites.Length; i++)
        //    {
        //        List<xContour> ctList = sites[i].Contours;
        //        for (int j = 0; j < ctList.Count; j++)
        //        {
        //            xContour ct = ctList[j];
        //            if (ct.Visited) continue;
        //            ct.Visited = true;
        //            xLoop polylineList = new xLoop();    // PolyLineList  :  List<Polyline>  ; 
        //            for (int k = 0; k < ct.ContourNodes.Count; k++)
        //            {
        //                xContourNode cn1 = ct.ContourNodes[k];
        //                xContourNode cn2 = ct.ContourNodes[(k + 1) % ct.ContourNodes.Count];
        //                //find common site
        //                xSite s;
        //                if (cn1.S1 == cn2.S2 || cn1.S1 == cn2.S1)
        //                    s = cn1.S1;
        //                else
        //                    s = cn1.S2;
        //                if (s.Content is xLine2D)
        //                    polylineList.AddPoint(ct.ContourNodes[k].P);
        //                else// insert more interpolated poitns.It is actuall an arc
        //                {
        //                    //make sure error is less than 1/10 of offset
        //                    xPoint2D p0 = (xPoint2D)s.Content;
        //                    xPoint2D p1 = cn1.P;
        //                    xPoint2D p2 = cn2.P;
        //                    double length = p0.DistanceTo(p1);
        //                    polylineList.AddPoint(p1);
        //                    if (length > 0.1 * offset)
        //                    {
        //                        double deltaAlpha = Math.Acos(1 - 0.1 * offset / length) * 2.0;
        //                        double beta = -(new xVector2D(p0, p1).AngleTo(new xVector2D(1, 0)));
        //                        double between;
        //                        if (this.Winding == Winding.CCW)
        //                        {
        //                            between = new xVector2D(p0, p2).AngleTo(new xVector2D(p0, p1));
        //                        }
        //                        else
        //                        {
        //                            between = new xVector2D(p0, p1).AngleTo(new xVector2D(p0, p2));
        //                        }
        //                        for (int m = 1; m < between / deltaAlpha; m++)
        //                        {
        //                            double alpha;
        //                            if (this.Winding == Winding.CCW)
        //                                alpha = beta - (double)m * deltaAlpha;
        //                            else
        //                                alpha = beta + (double)m * deltaAlpha;
        //                            double x = length * Math.Cos(alpha) + p0.X;
        //                            double y = length * Math.Sin(alpha) + p0.Y;
        //                            polylineList.AddPoint(new xPoint2D(x, y));
        //                        }
        //                    }
        //                }
        //            }//end k				
        //            if (polylineList.NumberOfPoints >= 2)
        //            {
        //                polylineList.AddPoint(polylineList.GetPointAt(0));//make it closed
        //                finalList.Add(polylineList);
        //            }
        //        }//end j
        //    }//end i
        //    return finalList;
        //} // 外面数outerCircileNumber个圈

        public xPolygon Inverse()
        {
            int length = this.Points.Length;
            xPoint2[] pts = new xPoint2[length];
            for (int i = 0; i < length; i++)
                pts[i] = Points[length - i - 1].Clone();
            return new xPolygon(pts);
        }

        public Position PtPosition(xPoint2 p)
        {
            //make a very long line
            xLine2D line = new xLine2D(p, new xVector2D(100000000, 0));
            bool inside = false;
            for (int i = 0; i < Points.Length; i++)
            {
                xLine2D lineA = new xLine2D(Points[i], Points[(i + 1) % Points.Length]);
                LineRelationship relation = line.Relationship(lineA);
                if (relation == LineRelationship.Cross)
                    inside = !inside;
                else if (relation == LineRelationship.Coincident)
                {
                    //exclude "on" case
                    double Mx = Math.Abs(lineA.X2 - lineA.X1);
                    double My = Math.Abs(lineA.Y2 - lineA.Y1);
                    double dx1 = Math.Abs(p.X - lineA.X1);
                    double dx2 = Math.Abs(p.X - lineA.X2);
                    double dy1 = Math.Abs(p.Y - lineA.Y1);
                    double dy2 = Math.Abs(p.Y - lineA.Y2);
                    if (Mx == dx1 + dx2 && My == dy1 + dy2)
                        return Position.ON;
                    xPoint2 p1 = new xLine2D(Points[(i + 2) % Points.Length], Points[(i + 1) % Points.Length]).MidPoint();
                    xPoint2 p2 = new xLine2D(Points[i], Points[(i - 1 + Points.Length) % Points.Length]).MidPoint();
                    if (line.Relationship(new xLine2D(p1, p2)) == LineRelationship.Cross)
                        inside = !inside;
                }
                else if (relation == LineRelationship.TailEdge
                    || relation == LineRelationship.TailHead
                    || relation == LineRelationship.TailTail)
                    return Position.ON;
                else if (relation == LineRelationship.EdgeHead)//Edge-Tail, ignore
                {
                    //if next is not Coincident, then ignore
                    xLine2D lineNN = new xLine2D(Points[(i + 1) % Points.Length], Points[(i + 2) % Points.Length]);
                    LineRelationship relationNN = line.Relationship(lineNN);
                    if (relationNN != LineRelationship.Coincident)
                    {
                        if (line.PtPosition(lineA.P1) != line.PtPosition(lineNN.P2))
                            inside = !inside;
                    }
                }
            }
            if (inside)
                return Position.IN;
            else
                return Position.OUT;
        }

        public List<xContourNode> GetContourNodesAt(xBNode startNode, double depth)   // Contours----Contour--> BNode
        {
            List<xContourNode> list = new List<xContourNode>();
            if (startNode == null)
                return list;
            if (startNode.LeftChild != null)
            {
                list.AddRange(this.GetNodes(startNode, startNode.LeftChild, depth));
                list.AddRange(this.GetContourNodesAt(startNode.LeftChild, depth));
            }
            if (startNode.RightChild != null)
            {
                list.AddRange(this.GetNodes(startNode, startNode.RightChild, depth));
                list.AddRange(this.GetContourNodesAt(startNode.RightChild, depth));
            }
            return list;
        }

        public void DistributeContourNodes(List<xContourNode> contourNodes)
        {
            //clear site contournodes
            for (int i = 0; i < sites.Length; i++)
            {
                sites[i].ContourNodes.Clear();
            }
            for (int i = 0; i < contourNodes.Count; i++)
            {
                xContourNode cn = contourNodes[i];
                cn.S1.ContourNodes.Add(cn);
                cn.S2.ContourNodes.Add(cn);
            }
            //make contourNodes of a site ordered
            //make connections in each site
            for (int i = 0; i < sites.Length; i++)
            {
                xSite s = sites[i];
                if (s.Content is xLine2D)
                {
                    for (int j = 0; j < s.ContourNodes.Count; j++)
                    {
                        xContourNode cnode = s.ContourNodes[j];
                        xVector2D v = new xVector2D((xLine2D)s.Content).Unit;
                        cnode.Tag = v.Dot(new xVector2D(s.P1, cnode.P));
                    }
                }
                else//site is point
                {
                    for (int j = 0; j < s.ContourNodes.Count; j++)
                    {
                        xContourNode cnode = s.ContourNodes[j];

                        if (cnode.P.Equals((xPoint2)s.Content))
                        {
                            if (cnode.S1 == s.Previous || cnode.S2 == s.Previous)
                            {
                                cnode.Tag = double.MinValue;
                            }
                            else cnode.Tag = double.MaxValue;
                        }
                        else
                        {
                            xVector2D v = new xVector2D((xLine2D)s.Next.Content);
                            if (this.Winding == Winding.CW)
                                cnode.Tag = -new xVector2D(s.P1, cnode.P).AngleTo(v);
                            else
                                cnode.Tag = -v.AngleTo(new xVector2D(s.P1, cnode.P));
                        }
                    }
                }
                s.ContourNodes.Sort((Comparison<xContourNode>)delegate(xContourNode a, xContourNode b) { return a.Tag > b.Tag ? 1 : a.Tag == b.Tag ? 0 : -1; });
                //make conenction
                for (int j = 0; j < s.ContourNodes.Count / 2; j++)
                {
                    s.ContourNodes[j * 2].Next = s.ContourNodes[j * 2 + 1];
                    s.ContourNodes[j * 2 + 1].Previous = s.ContourNodes[j * 2];
                }
            }
        }

        public List<xContour> ContoursAt(double depth)
        {
            List<xContourNode> nodeList = GetContourNodesAt(VoronoiRoot, depth);
            DistributeContourNodes(nodeList);

            #region 如果 Badlist 多，则返回Badlist
            int count = 0;
            List<xContourNode> badList = new List<xContourNode>();
            for (double disturbance = 0; disturbance < depth / 10.0; disturbance += (depth + 0.000001) / 100)
            {
                count = 0;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    if (nodeList[i].Next == null || nodeList[i].Previous == null)
                    {
                        count++;
                        badList.Add(nodeList[i]);
                    }
                }
                if (count == 0)
                    break;
                else
                {
                    nodeList = GetContourNodesAt(VoronoiRoot, depth + disturbance);
                    DistributeContourNodes(nodeList);
                }
            }

            if (count > 0)
            {
                List<xContour> lst = new List<xContour>();
                lst.Add(new xContour(badList));
                return lst;
            }

            #endregion

            List<xContour> returnList = new List<xContour>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                xContourNode startNode = nodeList[i];
                if (startNode.Visited)
                    continue;
                List<xContourNode> list = new List<xContourNode>();
                returnList.Add(new xContour(list));
                list.Add(startNode);
                startNode.Visited = true;
                xContourNode next = startNode.Next;
                while (next != startNode)
                {
                    list.Add(next);
                    next.Visited = true;
                    next = next.Next;
                }
            }
            return returnList;
        }

        public List<xContourNode> GetNodes(xBNode parent, xBNode child, double depth)
        {
            //eith node1 is node2's parent or node2 is node1's parent
            List<xContourNode> list = new List<xContourNode>();
            if (parent.xPoint2D.Equals(child.xPoint2D))//no length bisector
                return list;
            xSite site1 = child.S1;
            xSite site2 = child.S2;
            if (site1.Content is xLine2D && site2.Content is xLine2D)
            {//LL
                double d1 = ((xLine2D)site1.Content).DistanceLn(parent.xPoint2D);
                double d2 = ((xLine2D)site2.Content).DistanceLn(child.xPoint2D);
                if (d1 == d2)	//parallel, leave it alone
                    return list;

                xPoint2 p1 = parent.xPoint2D;
                xPoint2 p2 = child.xPoint2D;
                if (d1 > d2)
                {
                    p1 = child.xPoint2D;
                    p2 = parent.xPoint2D;
                    double temp = d1; d1 = d2; d2 = temp;//swap d1, d2
                }
                if (depth >= d2 || depth < d1)//not in range
                    return list;
                double u = (depth - d1) / (d2 - d1);
                xPoint2 p = new xPoint2(p1.X + u * (p2.X - p1.X), p1.Y + u * (p2.Y - p1.Y));
                list.Add(new xContourNode(site1, site2, p));
                return list;
            }
            else if (site1.Content is xLine2D && site2.Content is xPoint2)
            {//LP
                xLine2D line = (xLine2D)site1.Content;
                xPoint2 p0 = site2.P1;

                xPoint2 p1 = parent.xPoint2D;
                xPoint2 p2 = child.xPoint2D;

                Position pos = line.PtPosition(p0);
                if (pos == Position.ON || pos == Position.ABOVE || pos == Position.BELOW)
                {
                    double dis = p1.DistanceTo(p2);
                    double u = depth / dis;
                    if (u < 1)
                    {
                        xPoint2 p;
                        if (p1.DistanceToSq(p0) < p2.DistanceToSq(p0))
                            p = new xPoint2(p1.X + u * (p2.X - p1.X), p1.Y + u * (p2.Y - p1.Y));
                        else
                            p = new xPoint2(p2.X + u * (p1.X - p2.X), p2.Y + u * (p1.Y - p2.Y));
                        list.Add(new xContourNode(site1, site2, p));
                    }
                    return list;
                }

                //transform and get the solution
                xPoint2 proj = line.Projection(p0);
                xPoint2 origin = new xPoint2((p0.X + proj.X) / 2, (p0.Y + proj.Y) / 2);
                xTransform tran = new xTransform(origin, new xVector2D(line));
                double a = proj.DistanceTo(p0) / 2;
                if (depth - a < 0)
                    return list;
                //y=X^2/4a
                double solution1 = 2 * Math.Sqrt(a * (depth - a));
                double solution2 = -2 * Math.Sqrt(a * (depth - a));

                //pick good solution
                xPoint2 ptran1 = tran.To(p1);
                xPoint2 ptran2 = tran.To(p2);

                double y = line.PtPosition(p2) == Position.LEFT ? depth - a : a - depth;
                if (!((solution1 > ptran1.X && solution1 > ptran2.X) || (solution1 < ptran1.X && solution1 < ptran2.X)))
                {//solution1 is good
                    list.Add(new xContourNode(site1, site2, tran.Back(new xPoint2(solution1, y))));
                }
                if (!((solution2 > ptran1.X && solution2 > ptran2.X) || (solution2 < ptran1.X && solution2 < ptran2.X)))
                {//solution2 is good
                    list.Add(new xContourNode(site1, site2, tran.Back(new xPoint2(solution2, y))));
                }
                return list;
            }
            else if (site1.Content is xPoint2 && site2.Content is xLine2D)
            {//PL
                xLine2D line = (xLine2D)site2.Content;
                xPoint2 p0 = site1.P1;

                xPoint2 p1 = parent.xPoint2D;
                xPoint2 p2 = child.xPoint2D;

                Position pos = line.PtPosition(p0);
                if (pos == Position.ON || pos == Position.ABOVE || pos == Position.BELOW)
                {
                    double dis = p1.DistanceTo(p2);
                    double u = depth / dis;
                    if (u < 1)
                    {
                        xPoint2 p;
                        if (p1.DistanceToSq(p0) < p2.DistanceToSq(p0))
                            p = new xPoint2(p1.X + u * (p2.X - p1.X), p1.Y + u * (p2.Y - p1.Y));
                        else
                            p = new xPoint2(p2.X + u * (p1.X - p2.X), p2.Y + u * (p1.Y - p2.Y));
                        list.Add(new xContourNode(site1, site2, p));
                    }
                    return list;
                }

                //transform and get the solution
                xPoint2 proj = line.Projection(p0);
                xPoint2 origin = new xPoint2((p0.X + proj.X) / 2, (p0.Y + proj.Y) / 2);
                xTransform tran = new xTransform(origin, new xVector2D(line));
                double a = proj.DistanceTo(p0) / 2;
                if (depth - a < 0)
                    return list;
                //y=X^2/4a
                double solution1 = 2 * Math.Sqrt(a * (depth - a));
                double solution2 = -2 * Math.Sqrt(a * (depth - a));

                //pick good solution
                xPoint2 ptran1 = tran.To(p1);
                xPoint2 ptran2 = tran.To(p2);

                double y = line.PtPosition(p2) == Position.LEFT ? depth - a : a - depth;
                if (!((solution1 > ptran1.X && solution1 > ptran2.X) || (solution1 < ptran1.X && solution1 < ptran2.X)))
                {//solution1 is good
                    list.Add(new xContourNode(site1, site2, tran.Back(new xPoint2(solution1, y))));
                }
                if (!((solution2 > ptran1.X && solution2 > ptran2.X) || (solution2 < ptran1.X && solution2 < ptran2.X)))
                {//solution2 is good
                    list.Add(new xContourNode(site1, site2, tran.Back(new xPoint2(solution2, y))));
                }
                return list;
            }
            else//PP
            {
                xPoint2 p0 = site1.P1;
                xPoint2 p1 = parent.xPoint2D;
                xPoint2 p2 = child.xPoint2D;
                double d1 = p1.DistanceTo(p0);
                double d2 = p2.DistanceTo(p0);
                if ((d1 > depth && d2 > depth) || (d1 < depth && d2 < depth))
                    return list;
                xPoint2 proj = new xLine2D(p1, p2).Projection(p0);
                double a0 = depth * depth - proj.DistanceToSq(p0);
                if (a0 < 0)
                    throw new Exception("In getNodes(xBNode parent, xBNode child, double depth)");
                if (a0 == 0)
                    return list; //ignore
                double l1 = Math.Sqrt(a0);
                double l2 = -l1;
                xVector2D v = new xVector2D(p1, p2).Unit;
                xPoint2 ps1 = new xPoint2(proj.X + l1 * v.X, proj.Y + l1 * v.Y);
                xPoint2 ps2 = new xPoint2(proj.X + l2 * v.X, proj.Y + l2 * v.Y);
                if (!(v.Dot(new xVector2D(p1, ps1)) < 0 || v.Dot(new xVector2D(p2, ps1)) > 0))
                {//ps1 is good
                    list.Add(new xContourNode(site1, site2, ps1));
                }
                if (!(v.Dot(new xVector2D(p1, ps2)) < 0 || v.Dot(new xVector2D(p2, ps2)) > 0))
                {//ps2 is good
                    list.Add(new xContourNode(site1, site2, ps2));
                }
                return list;
            }
        }

        private xPoint2[] SmallLengthAngleFilter(xPoint2[] p)
        {
            if (p.Length < 3) 
                return new xPoint2[0];

            #region find maximum dimension, scale
            double xMax = p[0].X;
            double xMin = p[0].X;
            double yMax = p[0].Y;
            double yMin = p[0].Y;
            for (int i = 0; i < p.Length; i++)
            {
                if (yMax < p[i].Y)
                {
                    yMax = p[i].Y;
                }
                if (xMax < p[i].X) xMax = p[i].X;
                if (xMin > p[i].X) xMin = p[i].X;
                if (yMin > p[i].Y) yMin = p[i].Y;
            }
            double maxDimension = (xMax - xMin) > (yMax - yMin) ? (xMax - xMin) : (yMax - yMin);
            double filterFactor = 0.0001;
            double minDeltaAngle = 0.1;//degree
            double minSin = Math.Sin(minDeltaAngle * Math.PI / 180.0);//degree
            #endregion

            #region filter points

            //small segment filter
            double minSegmentSq = (maxDimension * filterFactor) * (maxDimension * filterFactor);

            List<xPoint2> minSegFilter = new  List<xPoint2>();
            for (int i = 0; i < p.Length; i++)
            {
                xPoint2 pp, pc; //previous, current, next
                if (minSegFilter.Count > 0)
                    pp = minSegFilter[minSegFilter.Count - 1];
                else
                    pp = p[(i - 1 + p.Length) % p.Length];
                pc = p[i];
                //see if it belongs to small segment
                if (pp.DistanceToSq(pc) > minSegmentSq)
                {
                    minSegFilter.Add(pc);
                }
            }
            if (minSegFilter.Count < 3)
                return new xPoint2[0]; //not a valid polygon
            //small angle filter
            double minAngle = Math.Sin(minDeltaAngle * Math.PI / 180.0);

            List<xPoint2> filterA = new List<xPoint2>();
            for (int i = 0; i < minSegFilter.Count; i++)
            {
                xPoint2 pp, pc, pn; //previous, current, next
                if (filterA.Count > 0)
                    pp = filterA[filterA.Count - 1];
                else
                    pp = minSegFilter[(i - 1 + minSegFilter.Count) % minSegFilter.Count];
                pc =minSegFilter[i];
                pn = minSegFilter[(i + 1) % minSegFilter.Count];
                //see if it belongs to small segment
                xVector2D v1 = new xVector2D(pp, pc).Unit;
                xVector2D v2 = new xVector2D(pc, pn).Unit;
                if (Math.Abs(v1.Cross(v2)) > minSin)
                {
                    filterA.Add(pc);
                }
            }

            if (filterA.Count < 3)
                return new xPoint2[0];
            #endregion

            return filterA.ToArray();
        }

        public xBNode Voronoi()
        {
            Queue<xSite> Q = new Queue<xSite>(this.xSites);
            xBNode lastInterNode = null;
            while (Q.Count > 1)//leave last one
            {
                Q.Enqueue(Q.Dequeue());
                xSite s = Q.Dequeue();
                //create BTreeNode
                xBNode node = new xBNode(s.P2, s, s.Next);
                lastInterNode = GrowNode(node);
            }
            xSite lastxSite = Q.Dequeue();
            xBNode root = new xBNode(lastxSite.P2, lastxSite, lastxSite.Next);
            root.LeftChild = lastInterNode;
            return root;
        }

        private xBNode GrowNode(xBNode node)
        {
            //change this equal to direct object comparason; site generation caused not equal
            if (node.S1.P1 == node.S2.P2)
                return node;
            //Starting from incoming site's LN, get a point
            if (node.S1.LN == null && node.S2.RN == null) return null;
            if (node.S1.LN != null && node.S2.RN == null)
            {
                //find intersection point, using this point as new node, grow it.
                xBNode jc0 = trimxBNode(node, node.S1.LN, true);
                if (jc0 != null)
                {
                    jc0.LeftChild = node.S1.LN.Root;
                    jc0.RightChild = node;
                    return GrowNode(jc0);
                }
            }
            if (node.S1.LN == null && node.S2.RN != null)
            {
                //find intersection point, using this point as new node, grow it.
                xBNode jc1 = trimxBNode(node, node.S2.RN, false);
                if (jc1 != null)
                {
                    jc1.LeftChild = node;
                    jc1.RightChild = node.S2.RN.Root;
                    return GrowNode(jc1);
                }
            }
            if (node.S1.LN != null && node.S2.RN != null)
            {
                xBNode jc0 = trimxBNode(node, node.S1.LN, true);
                xBNode jc1 = trimxBNode(node, node.S2.RN, false);
                if (jc0 != null && jc1 == null)
                {
                    jc0.LeftChild = node.S1.LN.Root;
                    jc0.RightChild = node;
                    return GrowNode(jc0);
                }
                else if (jc1 != null && jc0 == null)
                {
                    jc1.LeftChild = node;
                    jc1.RightChild = node.S2.RN.Root;

                    return GrowNode(jc1);
                }
                else if (jc1 != null && jc0 != null)
                {
                    //compare the two
                    if (node.xPoint2D.DistanceToSq(jc0.xPoint2D) < node.xPoint2D.DistanceToSq(jc1.xPoint2D))
                    {
                        jc0.LeftChild = node.S1.LN.Root;
                        jc0.RightChild = node;

                        return GrowNode(jc0);
                    }
                    else//including ==
                    {
                        jc1.LeftChild = node;
                        jc1.RightChild = node.S2.RN.Root;

                        return GrowNode(jc1);
                    }
                }
                else//nothing
                {
                }
            }
            return null;
        }

        private xBNode trimxBNode(xBNode trimer, xBNode target, bool atLeft)
        {
            xBNode tester = target;
            while (true)
            {
                xPoint2 pc = xSite.Junction(tester.S1, tester.S2, atLeft ? trimer.S2 : trimer.S1, this);
                if (pc != null)//test if it is a closer one
                {
                    if (tester.Parent == null)
                    {
                        if (atLeft)
                        {
                            xBNode jc0 = new xBNode(pc, tester.S1, trimer.S2, tester.S2);

                            return jc0;
                        }
                        else
                        {
                            xBNode jc0 = new xBNode(pc, trimer.S1, tester.S2, tester.S1);

                            return jc0;
                        }
                    }
                    else
                    {
                        //DistanceTo test:
                        if (tester.xPoint2D.DistanceToSq(tester.Parent.xPoint2D) > tester.xPoint2D.DistanceToSq(pc))//equal will be dealt next round
                        {
                            xBNode newNode;
                            if (atLeft)
                            {
                                newNode = new xBNode(pc, tester.S1, trimer.S2, tester.S2);

                            }
                            else
                            {
                                newNode = new xBNode(pc, trimer.S1, tester.S2, tester.S1);

                            }
                            while (tester.Parent != null)//break useless connections at upper levels
                            {
                                tester = tester.Parent;
                                tester.LeftChild = null;
                                tester.RightChild = null;
                            }
                            return newNode;
                        }
                    }
                }
                if (tester.Parent == null)
                    return null;
                else
                    tester = tester.Parent;//continue testing next
            }
        }

    } // end of xPolygon Class

    //************************************************************xBNode*******************************************************************************************

    public class xBNode
    {
        # region 成员
        public bool Visited = false;//this-parent bisector visited
        private xBNode parent = null, leftChild = null, rightChild = null;
        private xPoint2 point = null;
        private xSite s1 = null, s2 = null, s0 = null;

        #endregion

        #region 属性
        public xSite S1
        {
            get { return s1; }
        }//incoming

        public xSite S2
        {
            get { return s2; }
        }//outgoing

        public xSite S0
        {
            get { return s0; }
        }//inner

        public xPoint2 xPoint2D//this is the location of the bisector node
        {
            get { return point; }
        }

        public xBNode LeftChild
        {
            get { return leftChild; }
            set
            {
                removeLeftChild();
                if (value != null)
                {
                    value.divorceParent();
                    value.parent = this;
                    leftChild = value;
                }
            }
        }

        public xBNode RightChild
        {
            get { return rightChild; }
            set
            {
                removeRightChild();
                if (value != null)
                {
                    value.divorceParent();
                    value.parent = this;
                    rightChild = value;
                }
            }
        }

        public xBNode Parent
        {
            get { return parent; }
        }

        public xBNode Brother
        {
            get
            {
                if (parent == null)
                    return null;
                else if (this == parent.LeftChild)
                    return parent.RightChild;
                else
                    return parent.LeftChild;
            }
        }

        public xBNode LeftSmallestGrandChild
        {
            get
            {
                xBNode start = leftChild;
                if (start == null)
                    return null;
                else
                {
                    while (start.leftChild != null)
                        start = start.leftChild;
                    return start;
                }
            }
        }

        public xBNode RightSmallestGrandChild
        {
            get
            {
                xBNode start = RightChild;
                if (start == null)
                    return null;
                else
                {
                    while (start.rightChild != null)
                        start = start.rightChild;
                    return start;
                }
            }
        }

        public xBNode Root
        {
            get
            {
                xBNode tester = this;
                while (tester.Parent != null)
                    tester = tester.Parent;
                return tester;
            }
        }

        # endregion

        public xBNode(xPoint2 location, xSite s1, xSite s2, xSite s0)
        {
            point = location;
            this.s1 = s1;
            this.s2 = s2;
            this.s0 = s0;
        }// s1: incoming site, s2: outgoing site, s0: inner site

        public xBNode(xPoint2 location, xSite s1, xSite s2)
        {
            point = location;
            this.s1 = s1;
            this.s2 = s2;
            this.s0 = null;
            s1.RN = this;
            //s1.Next.LN=this;
            s2.LN = this;
            if (s2 != s1.Next)
            {
                throw new Exception("xBNode constructor not implemented");
            }
        }// s1 is incoming site and s2 is outgoing site.This must be a leaf node

        private void removeLeftChild()
        {
            if (leftChild != null)
            {
                leftChild.parent = null;
                leftChild = null;
            }
        }

        private void removeRightChild()
        {
            if (rightChild != null)
            {
                rightChild.parent = null;
                rightChild = null;
            }
        }

        private void divorceParent()
        {
            if (parent != null)
            {
                if (this.parent.leftChild == this)
                {
                    this.parent.leftChild = null;
                }
                else
                {
                    this.parent.rightChild = null;
                }
                this.parent = null;
            }
        }

        //this function returns all contour nodes on an edge defined by this BNode and its parent:
        public void CreateContourNodes(double depth)
        {

        }
    }

    //************************************************************xContourNode*************************************************************************************

    public class xContourNode
    {
        public bool Visited = false;
        public xSite S1;
        public xSite S2;
        public xPoint2 P;
        public xContourNode Previous;
        public xContourNode Next;
        public double Tag;//for comparison
        public double depth;

        public xContourNode(xSite s1, xSite s2, xPoint2 p)
        {
            S1 = s1; S2 = s2; P = p;
        }
        public xContourNode(xSite s1, xSite s2, xPoint2 p, double depth)
        {
            S1 = s1; S2 = s2; P = p; this.depth = depth;
        }
    }

    //***********************************************************xContour*********************************************************************************************
    public class xContour
    {
        public bool Visited;
        private List<xContourNode> contourNodes;
        public List<xContourNode> ContourNodes
        {
            get { return this.contourNodes; }
        }
        public xContour(List<xContourNode> contournodes)
        {
            this.contourNodes = contournodes;
            Visited = false;
        }
    }

    //***********************************************************XSite****************************************************************************************************

    public class xSite
    {
        #region 成员与属性
        int index;
        xSite previous;
        xSite next;
        object content;

        public xBNode LN = null;//left BTreeNode
        public xBNode RN = null;//right BTeeNode
        private List<xBNode> cellNodes = null;

        public List<xContourNode> ContourNodes = new List<xContourNode>();

        private xBNode nextNode(xBNode previous, xBNode current)
        {
            if (current.Parent != null)
            {
                if (current.Parent != previous &&
                    (current.Parent.S0 == this || current.Parent.S1 == this || current.Parent.S2 == this))
                    return current.Parent;
            }
            if (current.LeftChild != null)
            {
                if (current.LeftChild != previous &&
                    (current.LeftChild.S0 == this || current.LeftChild.S1 == this || current.LeftChild.S2 == this))
                    return current.LeftChild;
            }
            if (current.RightChild != null)
            {
                if (current.RightChild != previous &&
                    (current.RightChild.S0 == this || current.RightChild.S1 == this || current.RightChild.S2 == this))
                    return current.RightChild;
            }
            return null;
        }

        public List<xBNode> CellNodes
        {
            get
            {
                if (cellNodes == null)
                {
                    cellNodes = new List<xBNode>();
                    //		if(LN!=null)
                    //		{
                    cellNodes.Add(LN);

                    xBNode current = LN;
                    xBNode previous = null;
                    //get next:
                    //    up or down?
                    //    not the current
                    //    
                    while (true)
                    {
                        xBNode next = nextNode(previous, current);
                        if (next == null)
                            break;
                        cellNodes.Add(next);
                        previous = current;
                        current = next;
                    }
                    //	}
                    /*			else if(RN!=null)
                                {
                                    cellNodes.Add(RN);
						
                                    xBNode current=RN;
                                    xBNode previous=null;
                                    //get next:
                                    //    up or down?
                                    //    not the current
                                    //    
                                    while(true)
                                    {
                                        xBNode next=nextNode(previous, current);
                                        if(next==null)
                                            break;
                                        cellNodes.Add(next);
                                        previous=current;
                                        current=next;
                                    }
                                }*/
                }
                return cellNodes;
            }
        }
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        public xSite Previous
        {
            get { return previous; }
            set { previous = value; }
        }
        public xSite Next
        {
            get { return next; }
            set { next = value; }
        }
        public object Content
        {
            get { return content; }
            set { content = value; }
        }

        public xPoint2 P1
        {
            get
            {
                if (content is xPoint2)
                    return (xPoint2)content;
                else
                    return ((xLine2D)content).P1;
            }
        }
        public xPoint2 P2
        {
            get
            {
                if (content is xPoint2)
                    return (xPoint2)content;
                else
                    return ((xLine2D)content).P2;
            }
        }

        public List<xContour> Contours = new List<xContour>();
        #endregion field and properties

        public xSite(xPoint2 pt)
        {
            content = pt;
        }

        public xSite(xLine2D ln)
        {
            content = ln;
        }

        public static xPoint2 Junction(xSite s1, xSite s2, xSite s3, xPolygon sp)
        {
            #region PPP, implementation
            if (s1.Content is xPoint2 && s2.Content is xPoint2 && s3.Content is xPoint2)
            {
                xVector2D v1 = new xVector2D((xPoint2)s1.Content, (xPoint2)s2.Content);
                xVector2D v2 = new xVector2D((xPoint2)s2.Content, (xPoint2)s3.Content);
                double cross = v1.Cross(v2);
                if ((cross > 0 && (sp.Winding == Winding.CW))
                    || (cross < 0 && (sp.Winding == Winding.CCW))
                    || (cross == 0))
                {	//no valid solution since windings are different or colinear
                    return null;
                }
                //calculate the point
                double a = 2 * (s2.P1.X - s1.P1.X);
                double b = 2 * (s2.P1.Y - s1.P1.Y);
                double c = 2 * (s3.P1.X - s1.P1.X);
                double d = 2 * (s3.P1.Y - s1.P1.Y);
                double e = s2.P1.X * s2.P1.X - s1.P1.X * s1.P1.X + s2.P1.Y * s2.P1.Y - s1.P1.Y * s1.P1.Y;
                double f = s3.P1.X * s3.P1.X - s1.P1.X * s1.P1.X + s3.P1.Y * s3.P1.Y - s1.P1.Y * s1.P1.Y;
                double[] solution = xSolver.Linear2(a, b, c, d, e, f);
                if (solution != null)
                {
                    xPoint2 pReturn = new xPoint2(solution[0], solution[1]);
                    if (assertValidSolution(pReturn, s1, s2, s3, sp))
                        return pReturn;
                    else
                        return null;
                }
            }
            #endregion
            #region PPL, implementation
            else if (s1.Content is xPoint2 && s2.Content is xPoint2 && s3.Content is xLine2D)
            {
                //exclude situatiosn:
                //1. dot product >=0 //wrong!!!!!!!!!
                //2. ptLindPosgion not conforms to winding
                //3. both on line
                xLine2D line = (xLine2D)s3.Content;
                xPoint2 p1 = (xPoint2)s1.Content;
                xPoint2 p2 = (xPoint2)s2.Content;
                Position pos1 = line.PtPosition(p1);
                Position pos2 = line.PtPosition(p2);
                if (sp.Winding == Winding.CCW &&
                    (pos1 == Position.RIGHT || pos2 == Position.RIGHT)) //wrong sided point
                    return null;
                if (sp.Winding == Winding.CW &&
                    (pos1 == Position.LEFT || pos2 == Position.LEFT)) //wrong sided point
                    return null;

                double x1 = s1.P1.X;
                double y1 = s1.P1.Y;
                double x2 = s2.P1.X;
                double y2 = s2.P1.Y;
                double x3 = s3.P1.X;
                double y3 = s3.P1.Y;
                double x4 = s3.P2.X;
                double y4 = s3.P2.Y;
                if (pos1 == Position.ON)
                {
                    if (pos2 == Position.ON || pos2 == Position.ABOVE || pos2 == Position.BELOW)
                        return null;
                    //find intersection
                    double a = 2 * (x2 - x1);
                    double b = 2 * (y2 - y1);
                    double c = x4 - x3;
                    double d = y4 - y3;
                    double e = x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1;
                    double f = (x4 - x3) * x1 + (y4 - y3) * y1;
                    double[] solution = xSolver.Linear2(a, b, c, d, e, f);
                    if (solution != null)
                    {
                        xPoint2 pReturn = new xPoint2(solution[0], solution[1]);
                        if (assertValidSolution(pReturn, s1, s2, s3, sp))
                            return pReturn;
                        else
                            return null;
                    }
                }
                if (pos2 == Position.ON)
                {
                    if (pos1 == Position.ON || pos1 == Position.ABOVE || pos1 == Position.BELOW)
                        return null;
                    //find intersection
                    double a = 2 * (x2 - x1);
                    double b = 2 * (y2 - y1);
                    double c = x4 - x3;
                    double d = y4 - y3;
                    double e = x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1;
                    double f = (x4 - x3) * x2 + (y4 - y3) * y2;
                    double[] solution = xSolver.Linear2(a, b, c, d, e, f);
                    if (solution != null)
                    {
                        xPoint2 pReturn = new xPoint2(solution[0], solution[1]);
                        if (assertValidSolution(pReturn, s1, s2, s3, sp))
                            return pReturn;
                        else
                            return null;
                    }
                }
                //in case one point is on the line

                xVector2D u;
                if (sp.Winding == Winding.CCW)
                    u = new xVector2D(s3.P1, s3.P2).Turn90().Unit;
                else
                    u = new xVector2D(s3.P1, s3.P2).Turn270().Unit;
                if (Math.Abs(x2 - x1) > Math.Abs(y2 - y1))// x=ay+b
                {
                    double a = (y1 - y2) / (x2 - x1);
                    double b = (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1) / (2 * (x2 - x1));
                    double c = (b - x3) * u.X - y3 * u.Y;
                    double a0 = (b - x1) * (b - x1) + y1 * y1 - c * c;
                    double a1 = 2.0 * (a * (b - x1) - y1 - (a * u.X + u.Y) * c);
                    double a2 = a * a + 1.0 - (a * u.X + u.Y) * (a * u.X + u.Y);

                    List<double> solution = xSolver.Polynomial(a0, a1, a2);
                    switch (solution.Count)
                    {
                        case 0:
                            return null;
                        case 1://p1-p2 is parallel to line
                            //see dot product
                            if ((p2.X - p1.X) * (line.X2 - line.X1) + (p2.Y - p1.Y) * (line.Y2 - line.Y1) > 0)//actually solution is infinite
                                return null;
                            double x = a * (double)solution[0] + b;
                            xPoint2 pReturn = new xPoint2(x, (double)solution[0]);

                            if (assertValidSolution(pReturn, s1, s2, s3, sp))
                                return pReturn;
                            else
                                return null;
                        case 2:
                            //see which one is close
                            xPoint2 ps1 = new xPoint2(a * (double)solution[0] + b, (double)solution[0]);
                            xPoint2 ps2 = new xPoint2(a * (double)solution[1] + b, (double)solution[1]);
                            xVector2D v0 = new xVector2D(p1, p2);
                            xVector2D v1 = new xVector2D(p1, ps1);
                            xVector2D v2 = new xVector2D(p1, ps2);
                            if (sp.Winding == Winding.CCW)
                            {
                                if (v0.Cross(v1) < v0.Cross(v2))
                                {
                                    if (assertValidSolution(ps2, s1, s2, s3, sp))
                                        return ps2;
                                    else
                                        return null;
                                }
                                else
                                {
                                    if (assertValidSolution(ps1, s1, s2, s3, sp))
                                        return ps1;
                                    else
                                        return null;
                                }
                            }
                            else//CW
                            {
                                if (v0.Cross(v1) < v0.Cross(v2))
                                {
                                    if (assertValidSolution(ps1, s1, s2, s3, sp))
                                        return ps1;
                                    else
                                        return null;
                                }
                                else
                                {
                                    if (assertValidSolution(ps2, s1, s2, s3, sp))
                                        return ps2;
                                    else
                                        return null;
                                }
                            }
                    }
                }
                else//y=ax+b
                {

                    double a = (x1 - x2) / (y2 - y1);
                    double b = (y2 * y2 - y1 * y1 + x2 * x2 - x1 * x1) / (2 * (y2 - y1));
                    double c = (b - y3) * u.Y - x3 * u.X;
                    double a0 = (b - y1) * (b - y1) + x1 * x1 - c * c;
                    double a1 = 2.0 * (a * (b - y1) - x1 - (a * u.Y + u.X) * c);
                    double a2 = a * a + 1.0 - (a * u.Y + u.X) * (a * u.Y + u.X);

                    List<double> solution = xSolver.Polynomial(a0, a1, a2);
                    switch (solution.Count)
                    {
                        case 0:
                            return null;
                        case 1:
                            //
                            if ((p2.X - p1.X) * (line.X2 - line.X1) + (p2.Y - p1.Y) * (line.Y2 - line.Y1) > 0)//actually solution is infinite
                                return null;
                            double y = a * (double)solution[0] + b;
                            xPoint2 pReturn = new xPoint2((double)solution[0], y);
                            if (assertValidSolution(pReturn, s1, s2, s3, sp))
                                return pReturn;
                            else
                                return null;
                        case 2:
                            //see which one is close
                            xPoint2 ps1 = new xPoint2((double)solution[0], a * (double)solution[0] + b);
                            xPoint2 ps2 = new xPoint2((double)solution[1], a * (double)solution[1] + b);
                            xVector2D v0 = new xVector2D(p1, p2);
                            xVector2D v1 = new xVector2D(p1, ps1);
                            xVector2D v2 = new xVector2D(p1, ps2);
                            if (sp.Winding == Winding.CCW)
                            {
                                if (v0.Cross(v1) < v0.Cross(v2))
                                {
                                    if (assertValidSolution(ps2, s1, s2, s3, sp))
                                        return ps2;
                                    else
                                        return null;
                                }
                                else
                                {
                                    if (assertValidSolution(ps1, s1, s2, s3, sp))
                                        return ps1;
                                    else
                                        return null;
                                }
                            }
                            else//CW
                            {
                                if (v0.Cross(v1) < v0.Cross(v2))
                                {
                                    if (assertValidSolution(ps1, s1, s2, s3, sp))
                                        return ps1;
                                    else
                                        return null;
                                }
                                else
                                {
                                    if (assertValidSolution(ps2, s1, s2, s3, sp))
                                        return ps2;
                                    else
                                        return null;
                                }
                            }
                        default:
                            break;
                    }
                }
            }

            #endregion
            #region PLP

            else if (s1.Content is xPoint2 && s2.Content is xLine2D && s3.Content is xPoint2)
            {
                return Junction(s3, s1, s2, sp);
            }

            #endregion*/
            #region PLL, implementation
            else if (s1.Content is xPoint2 && s2.Content is xLine2D && s3.Content is xLine2D)
            {
                xPoint2 p = (xPoint2)s1.Content;
                xLine2D l1 = (xLine2D)s2.Content;
                xLine2D l2 = (xLine2D)s3.Content;
                Position pos1 = l1.PtPosition(p);
                Position pos2 = l2.PtPosition(p);

                xVector2D v, u;
                if (sp.Winding == Winding.CCW)
                {
                    if (pos1 == Position.RIGHT || pos2 == Position.RIGHT)
                        return null;
                    v = new xVector2D(l1).Turn90().Unit;
                    u = new xVector2D(l2).Turn90().Unit;
                }
                else
                {
                    if (pos1 == Position.LEFT || pos2 == Position.LEFT)
                        return null;
                    v = new xVector2D(l1).Turn270().Unit;
                    u = new xVector2D(l2).Turn270().Unit;
                }


                if (pos1 == Position.ON)
                {
                    if (pos2 == Position.ON)
                    {
                        if (s1.Next == s2 && s3.Next == s1
                            || s1.Next == s3 && s2.Next == s1)//corner itself
                            return null;
                        else
                            return s1.P1;
                    }
                    else
                    {
                        //find intersection
                        double a = v.X - u.X;
                        double b = v.Y - u.Y;
                        double c = l1.X2 - l1.X1;
                        double d = l1.Y2 - l1.Y1;
                        double e = v.X * l1.X1 + v.Y * l1.Y1 - u.X * l2.X1 - u.Y * l2.Y1;
                        double f = p.X * (l1.X2 - l1.X1) + p.Y * (l1.Y2 - l1.Y1);
                        double[] solution = xSolver.Linear2(a, b, c, d, e, f);
                        if (solution == null) return null;
                        else
                        {
                            xPoint2 pReturn = new xPoint2(solution[0], solution[1]);
                            if (assertValidSolution(pReturn, s1, s2, s3, sp))
                                return pReturn;
                            else
                                return null;
                        }
                    }
                }
                else if (pos2 == Position.ON)
                {
                    //find intersection
                    double a = v.X - u.X;
                    double b = v.Y - u.Y;
                    double c = l2.X2 - l2.X1;
                    double d = l2.Y2 - l2.Y1;
                    double e = v.X * l1.X1 + v.Y * l1.Y1 - u.X * l2.X1 - u.Y * l2.Y1;
                    double f = p.X * (l2.X2 - l2.X1) + p.Y * (l2.Y2 - l2.Y1);
                    double[] solution = xSolver.Linear2(a, b, c, d, e, f);
                    if (solution == null) return null;
                    else
                    {
                        xPoint2 pReturn = new xPoint2(solution[0], solution[1]);
                        if (assertValidSolution(pReturn, s1, s2, s3, sp))
                            return pReturn;
                        else
                            return null;
                    }
                }

                if (Math.Abs(v.X - u.X) > Math.Abs(v.Y - u.Y))//x=ay+b
                {
                    double x1 = l1.P1.X;
                    double y1 = l1.P1.Y;
                    double x2 = l2.P1.X;
                    double y2 = l2.P1.Y;
                    double a = -(v.Y - u.Y) / (v.X - u.X);
                    double b = (x1 * v.X + y1 * v.Y - x2 * u.X - y2 * u.Y) / (v.X - u.X);
                    double c = (b - x2) * u.X - y2 * u.Y;

                    double a0 = c * c - (b - p.X) * (b - p.X) - p.Y * p.Y;
                    double a1 = 2.0 * (c * (a * u.X + u.Y) - a * (b - p.X) + p.Y);
                    double a2 = (a * u.X + u.Y) * (a * u.X + u.Y) - a * a - 1;
                    List<double> solution = xSolver.Polynomial(a0, a1, a2);
                    switch (solution.Count)
                    {
                        case 0:
                            return null;
                        case 1:
                            //
                            double x = a * (double)solution[0] + b;
                            xPoint2 pReturn = new xPoint2(x, (double)solution[0]);
                            if (assertValidSolution(pReturn, s1, s2, s3, sp))
                                return pReturn;
                            else
                                return null;
                        case 2:
                            //see which one is close
                            xPoint2 ps1 = new xPoint2(a * (double)solution[0] + b, (double)solution[0]);
                            xPoint2 ps2 = new xPoint2(a * (double)solution[1] + b, (double)solution[1]);
                            xVector2D v1 = new xVector2D(l1);
                            xVector2D v2 = new xVector2D(l2);
                            xVector2D vs12 = new xVector2D(ps1, ps2);
                            if (v1.Dot(vs12) > 0)//pick ps2
                            {
                                if (assertValidSolution(ps2, s1, s2, s3, sp))
                                    return ps2;
                                else
                                    return null;
                            }
                            else//pick ps1
                            {
                                if (assertValidSolution(ps1, s1, s2, s3, sp))
                                    return ps1;
                                else
                                    return null;
                            }
                    }
                }
                else//y=ax+b
                {
                    double x1 = l1.P1.X;
                    double y1 = l1.P1.Y;
                    double x2 = l2.P1.X;
                    double y2 = l2.P1.Y;
                    double a = -(v.X - u.X) / (v.Y - u.Y);
                    double b = (y1 * v.Y + x1 * v.X - y2 * u.Y - x2 * u.X) / (v.Y - u.Y);
                    double c = (b - y2) * u.Y - x2 * u.X;

                    double a0 = c * c - (b - p.Y) * (b - p.Y) - p.X * p.X;
                    double a1 = 2.0 * (c * (a * u.Y + u.X) - a * (b - p.Y) + p.X);
                    double a2 = (a * u.Y + u.X) * (a * u.Y + u.X) - a * a - 1;
                    List<double> solution = xSolver.Polynomial(a0, a1, a2);

                    switch (solution.Count)
                    {
                        case 0:
                            return null;
                        case 1:
                            //
                            double y = a * (double)solution[0] + b;
                            xPoint2 pReturn = new xPoint2((double)solution[0], y);
                            if (assertValidSolution(pReturn, s1, s2, s3, sp))
                                return pReturn;
                            else
                                return null;
                        case 2:
                            //see which one is close

                            xPoint2 ps1 = new xPoint2((double)solution[0], a * (double)solution[0] + b);
                            xPoint2 ps2 = new xPoint2((double)solution[1], a * (double)solution[1] + b);
                            xVector2D v1 = new xVector2D(l1);
                            xVector2D v2 = new xVector2D(l2);
                            xVector2D vs12 = new xVector2D(ps1, ps2);
                            if (v1.Dot(vs12) > 0)//pick ps2
                            {
                                if (assertValidSolution(ps2, s1, s2, s3, sp))
                                    return ps2;
                                else
                                    return null;
                            }
                            else//pick ps1
                            {
                                if (assertValidSolution(ps1, s1, s2, s3, sp))
                                    return ps1;
                                else
                                    return null;
                            }
                    }
                }
            }
            #endregion
            #region LPP
            else if (s1.Content is xLine2D && s2.Content is xPoint2 && s3.Content is xPoint2)
            {
                return Junction(s2, s3, s1, sp);//PPL
            }
            #endregion
            #region LPL
            else if (s1.Content is xLine2D && s2.Content is xPoint2 && s3.Content is xLine2D)
            {
                return Junction(s2, s3, s1, sp);
            }


            #endregion
            #region LLP
            else if (s1.Content is xLine2D && s2.Content is xLine2D && s3.Content is xPoint2)
            {
                return Junction(s3, s1, s2, sp);
            }

            #endregion
            #region LLL, implementation
            else if (s1.Content is xLine2D && s2.Content is xLine2D && s3.Content is xLine2D)
            {
                xVector2D u, v, w;
                if (sp.Winding == Winding.CCW)
                {
                    u = new xVector2D(s1.P1, s1.P2).Turn90().Unit;
                    v = new xVector2D(s2.P1, s2.P2).Turn90().Unit;
                    w = new xVector2D(s3.P1, s3.P2).Turn90().Unit;
                }
                else
                {
                    u = new xVector2D(s1.P1, s1.P2).Turn270().Unit;
                    v = new xVector2D(s2.P1, s2.P2).Turn270().Unit;
                    w = new xVector2D(s3.P1, s3.P2).Turn270().Unit;
                }
                double a = u.X - v.X;
                double b = u.Y - v.Y;
                double c = u.X - w.X;
                double d = u.Y - w.Y;
                double e = s1.P1.X * u.X + s1.P1.Y * u.Y - s2.P1.X * v.X - s2.P1.Y * v.Y;
                double f = s1.P1.X * u.X + s1.P1.Y * u.Y - s3.P1.X * w.X - s3.P1.Y * w.Y;
                double[] solution = xSolver.Linear2(a, b, c, d, e, f);
                if (solution != null)
                {
                    if ((solution[0] - s1.P1.X) * u.X + (solution[1] - s1.P1.Y) * u.Y < 0)//opposite solution 
                        return null;
                    xPoint2 pReturn = new xPoint2(solution[0], solution[1]);
                    if (assertValidSolution(pReturn, s1, s2, s3, sp))
                        return pReturn;
                }
            }
            #endregion

            else
            {//other case to be finished
                throw new Exception("Unhandled site-site-site operation");
            }
            return null;
        }

        private static bool assertValidSolution(xPoint2 p, xSite s1, xSite s2, xSite s3, xPolygon sp)
        {
            return isValidPoint(p, s1, sp) && isValidPoint(p, s2, sp) && isValidPoint(p, s3, sp);
        }

        private static bool isValidPoint(xPoint2 p, xSite s, xPolygon sp)
        {
            if (p.X < sp.LowerCorner.X || p.X > sp.UpperCorner.X || p.Y < sp.LowerCorner.Y || p.Y > sp.UpperCorner.Y)
                return false;
            if (s.Content is xLine2D)
            {
                //1. see if it is on right side
                xLine2D line = (xLine2D)s.Content;
                Position pos = line.PtPosition(p);
                if (sp.Winding == Winding.CCW && pos == Position.RIGHT
                    || sp.Winding == Winding.CW && pos == Position.LEFT)
                    return false;
                //2. see if it is with range, using dot product
                xVector2D vline = new xVector2D(line);
                xVector2D vp1p = new xVector2D(line.P1, p);
                xVector2D vp2p = new xVector2D(line.P2, p);
                double epsilon = line.Length > p.DistanceTo(line.P1) ? line.Length : p.DistanceTo(line.P1);
                epsilon *= 1e-6;
                if ((vline.Dot(vp1p) <= -epsilon) || vline.Dot(vp2p) >= epsilon)
                    return false;
                return true;
            }
            else//point site, its previous and next sites MUST be line sites
            {
                //return true;
                xPoint2 ps = (xPoint2)s.Content;
                if (ps.Equals(p))
                    return true;
                xVector2D v1 = new xVector2D((xLine2D)s.Previous.content);
                xVector2D v2 = new xVector2D((xLine2D)s.Next.Content);
                xVector2D vTest = new xVector2D(ps, p);
                double epsilon = 1e-6;
                if (sp.Winding == Winding.CCW)
                {
                    v1 = v1.Turn90();
                    v2 = v2.Turn90();
                    if (v1.Cross(vTest) <= epsilon && vTest.Cross(v2) <= epsilon)
                    {
                        return true;
                    }
                    else return false;
                }
                else
                {
                    v1 = v1.Turn270();
                    v2 = v2.Turn270();
                    if (v1.Cross(vTest) >= -epsilon && vTest.Cross(v2) >= -epsilon)
                    {
                        return true;
                    }
                    else return false;
                }
            }
        }

        public xContourNode GetxContourNodesAt(double depth)
        {
            if (depth < 0)
                throw new Exception("GetxContourNodesAt function exception");
            //return all leaf nodes
            if (depth == 0)
            {
                return new xContourNode(this.Previous, this, this.P1);
            }
            //search for one
            xBNode beginNode = (xBNode)CellNodes[0];
            for (int i = 1; i < CellNodes.Count; i++)
            {
                xContourNode test = createUpContourNode(beginNode, (xBNode)CellNodes[i], depth);
                if (test != null)
                    return test;
                beginNode = (xBNode)CellNodes[i];
            }
            return null;
        }

        private xContourNode createUpContourNode(xBNode node1, xBNode node2, double depth)
        {
            //eith node1 is node2's parent or node2 is node1's parent
            xSite site1, site2;
            if (node1.Parent == node2)
            {
                site1 = node1.S1;
                site2 = node1.S2;
            }
            else
            {
                site1 = node2.S1;
                site2 = node2.S2;
            }
            if (site1.Content is xLine2D && site2.Content is xLine2D)
            {//LL
                double d1 = ((xLine2D)this.Content).DistanceLn(node1.xPoint2D);
                double d2 = ((xLine2D)this.Content).DistanceLn(node2.xPoint2D);
                if (d2 <= depth)
                    return null;
                double u = (depth - d1) / (d2 - d1);
                xPoint2 p = new xPoint2(node1.xPoint2D.X + u * (node2.xPoint2D.X - node1.xPoint2D.X), node1.xPoint2D.Y + u * (node2.xPoint2D.Y - node1.xPoint2D.Y));
                return new xContourNode(site1, site2, p);
            }
            else if (site1.Content is xLine2D && site2.Content is xPoint2)
            {//LP
                xLine2D line = (xLine2D)site1.Content;
                xPoint2 p0 = site2.P1;
                xPoint2 p1 = node1.xPoint2D;
                xPoint2 p2 = node2.xPoint2D;
                double d2 = line.DistanceLn(p2);
                if (d2 <= depth)
                    return null;
                //debug
                double d1 = line.DistanceLn(p1);
                if (d2 < d1)
                    throw new Exception("Here is wrong");
                //end debuf
                //transform and get the solution
                Position pos = line.PtPosition(p0);
                if (pos == Position.ON || pos == Position.ABOVE || pos == Position.BELOW)
                {
                    double dis = p1.DistanceTo(p2);
                    double u = depth / dis;
                    xPoint2 p = new xPoint2(p1.X + u * (p2.X - p1.X), p1.Y + u * (p2.Y - p1.Y));
                    return new xContourNode(site1, site2, p);
                }
                xPoint2 proj = line.Projection(p0);
                xPoint2 origin = new xPoint2((p0.X + proj.X) / 2, (p0.Y + proj.Y) / 2);
                xTransform tran = new xTransform(origin, new xVector2D(line));
                double a = proj.DistanceTo(p0) / 2;
                //y=X^2/4a
                double solution1 = 2 * Math.Sqrt(a * (depth - a));
                double solution2 = -2 * Math.Sqrt(a * (depth - a));

                //pick good solution
                double x, y;
                xPoint2 ptran1 = tran.To(p1);
                xPoint2 ptran2 = tran.To(p2);
                if ((solution1 > ptran1.X && solution1 > ptran2.X) || (solution1 < ptran1.X && solution1 < ptran2.X))
                {//solution2 must be a valid solution
                    x = solution2;
                }
                else if ((solution2 > ptran1.X && solution2 > ptran2.X) || (solution2 < ptran1.X && solution2 < ptran2.X))
                {//solution 1 must be the valid solution
                    x = solution1;
                }
                else//pick whoever close to ptran1
                {
                    if ((solution1 - ptran1.X) < (solution2 - ptran1.X))
                        x = solution1;
                    else
                        x = solution2;
                }
                y = line.PtPosition(p2) == Position.LEFT ? depth - a : a - depth;
                return new xContourNode(site1, site2, tran.Back(new xPoint2(x, y)));
            }
            else if (site1.Content is xPoint2 && site2.Content is xLine2D)
            {//PL
                xLine2D line = (xLine2D)site2.Content;
                xPoint2 p0 = site1.P1;
                xPoint2 p1 = node1.xPoint2D;
                xPoint2 p2 = node2.xPoint2D;
                double d2 = line.DistanceLn(p2);
                if (d2 <= depth)
                    return null;
                //debug
                double d1 = line.DistanceLn(p1);
                if (d2 < d1)
                    throw new Exception("Here is wrong");
                //end debuf
                Position pos = line.PtPosition(p0);
                if (pos == Position.ON || pos == Position.ABOVE || pos == Position.BELOW)
                {
                    double dis = p1.DistanceTo(p2);
                    double u = depth / dis;
                    xPoint2 p = new xPoint2(p1.X + u * (p2.X - p1.X), p1.Y + u * (p2.Y - p1.Y));
                    return new xContourNode(site1, site2, p);
                }
                //transform and get the solution
                xPoint2 proj = line.Projection(p0);
                xPoint2 origin = new xPoint2((p0.X + proj.X) / 2, (p0.Y + proj.Y) / 2);
                xTransform tran = new xTransform(origin, new xVector2D(line));
                double a = proj.DistanceTo(p0) / 2;
                //y=X^2/4a
                double solution1 = 2 * Math.Sqrt(a * (depth - a));
                double solution2 = -2 * Math.Sqrt(a * (depth - a));

                //pick good solution
                double x, y;
                xPoint2 ptran1 = tran.To(p1);
                xPoint2 ptran2 = tran.To(p2);
                if ((solution1 > ptran1.X && solution1 > ptran2.X) || (solution1 < ptran1.X && solution1 < ptran2.X))
                {//solution2 must be a valid solution
                    x = solution2;
                }
                else if ((solution2 > ptran1.X && solution2 > ptran2.X) || (solution2 < ptran1.X && solution2 < ptran2.X))
                {//solution 1 must be the valid solution
                    x = solution1;
                }
                else//pick whoever close to ptran1
                {
                    if ((solution1 - ptran1.X) < (solution2 - ptran1.X))
                        x = solution1;
                    else
                        x = solution2;
                }
                y = line.PtPosition(p2) == Position.LEFT ? depth - a : a - depth;
                return new xContourNode(site1, site2, tran.Back(new xPoint2(x, y)));
            }
            else//PP
            {
                xPoint2 p0 = site1.P1;
                xPoint2 p1 = node1.xPoint2D;
                xPoint2 p2 = node2.xPoint2D;
                double d1 = p1.DistanceTo(p0);
                double d2 = p2.DistanceTo(p0);
                if ((d1 > depth && d2 > depth) || (d1 < depth && d2 < depth))
                    return null;
                xPoint2 proj = new xLine2D(p1, p2).Projection(p0);
                double a0 = depth * depth - proj.DistanceToSq(p0);
                if (a0 < 0)
                    throw new Exception("In createUpxContourNode funtion");
                if (a0 == 0)
                    return null; //ignore
                double l1 = Math.Sqrt(a0);
                double l2 = -Math.Sqrt(a0);
                xVector2D v = new xVector2D(p1, p2).Unit;
                xPoint2 ps1 = new xPoint2(proj.X + l1 * v.X, proj.Y + l1 * v.Y);
                xPoint2 ps2 = new xPoint2(proj.X + l2 * v.X, proj.Y + l2 * v.Y);
                //pick close one
                if (v.Dot(new xVector2D(p1, ps1)) < 0 || v.Dot(new xVector2D(p2, ps1)) > 0)
                {//must be ps2
                    return new xContourNode(site1, site2, ps2);
                }
                else if (v.Dot(new xVector2D(p1, ps2)) < 0 || v.Dot(new xVector2D(p2, ps2)) > 0)
                {//must be ps1
                    return new xContourNode(site1, site2, ps1);
                }
                else
                {//pick close one
                    if (ps1.DistanceToSq(p1) < ps2.DistanceToSq(p1))
                        return new xContourNode(site1, site2, ps1);
                    else
                        return new xContourNode(site1, site2, ps2);
                }
            }
        } //create a contourNode between node1 and node2, when searching up, assumption: node1 is closer to the current site

        private void DigitizeCell(double depth)
        {
            //for(int i=0; i<
        }// This function create xContourNodes of each xBNode in the cell
    }

    //************************************************************xSolver**************************************************************************************************

    public class xSolver
    {
        //a0 + a1*x + a2*x^2 + ... =0
        public static List<double> Polynomial(params double[] a)
        {
            List<double> solution = new List<double>();
            if (a.Length < 2)
                throw new Exception("A polynomial should have at least 2 coefficients");
            else if (a.Length == 2)
            {
                if (a[1] == 0)
                    throw new Exception("a1 is Zero for a0 + a1*x type of polynomial");
                solution.Add(-a[0] / a[1]);
            }
            else if (a.Length == 3)
            {
                if (a[1] != 0)
                {
                    if (Math.Abs(a[2] * a[0]) / (a[1] * a[1]) < 1e-6)//ill-conditioned
                        return Polynomial(a[0], a[1]);
                }
                double delta = a[1] * a[1] - 4.0 * a[2] * a[0];
                if (delta == 0)
                    solution.Add(-a[1] / 2.0 / a[2]);
                else if (delta > 0)
                {
                    double root1 = (-a[1] - Math.Sqrt(delta)) / 2.0 / a[2];
                    double root2 = (-a[1] + Math.Sqrt(delta)) / 2.0 / a[2];
                    solution.Add(root1);
                    solution.Add(root2);
                }
            }
            return solution;
        }


        /// <summary>
        /// |a b|x   |e|
        /// |   |  = | |
        /// |c d|y   |f|
        /// </summary>
        public static double[] Linear2(double a, double b, double c, double d, double e, double f)
        {
            double denominator = a * d - b * c;
            if (denominator == 0)
                return null;
            double[] solution = new double[2];
            solution[0] = (d * e - b * f) / denominator;
            solution[1] = (a * f - c * e) / denominator;
            return solution;
        }
    }

    //***********************************************************xTimer***************************************************************************************************

    enum TimerCommand
    {
        Reset,
        Start,
        Stop,
        Advance,
        GetAbsoluteTime,
        GetApplicationTime,
        GetElapsedTime
    };   // Enumeration for various cmds our timer can execute

    public class xTimer
    {

        [System.Security.SuppressUnmanagedCodeSecurity] // Stop stack walk security check to improve performance
        [DllImport("kernel32")]
        private static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

        [System.Security.SuppressUnmanagedCodeSecurity] // Stop stack walk security check to improve performance
        [DllImport("kernel32")]
        private static extern bool QueryPerformanceCounter(ref long PerformanceCount);

        [System.Security.SuppressUnmanagedCodeSecurity] // Stop stack walk security check to improve performance
        [DllImport("winmm.dll")]
        public static extern int timeGetTime();

        private static bool initialized = false;
        private static bool useQPF = false;
        private static bool stopped = true;
        private static long ticksPerSecond = 0;
        private static long timeStopped = 0;
        private static long timeElapsed = 0;
        private static long timeBase = 0;
        private static double doubleTimeElapsed = 0.0;
        private static double doubleTimeBase = 0.0;
        private static double doubleTimeStop = 0.0;

        private xTimer() { }

        public static void Start()
        {
            ExecTimerCommand(TimerCommand.Start);
        }

        public static void Stop()
        {
            ExecTimerCommand(TimerCommand.Stop);
        }

        public static void Reset()
        {
            ExecTimerCommand(TimerCommand.Reset);
        }

        public static void Advance()
        {
            ExecTimerCommand(TimerCommand.Advance);
        }

        /// <summary>
        /// Application time.
        /// </summary>
        public static double ApplicationTime
        {
            get { return ExecTimerCommand(TimerCommand.GetApplicationTime); }
        }

        public static double AbsoluteTime
        {
            get { return ExecTimerCommand(TimerCommand.GetAbsoluteTime); }
        }

        public static double ElapsedTime
        {
            get { return ExecTimerCommand(TimerCommand.GetElapsedTime); }
        }

        private static double ExecTimerCommand(TimerCommand cmd)
        {
            if (!initialized)
            {
                initialized = true;

                // Use QueryPerformanceFrequency() to get frequency of timer.  If QPF is
                // not supported, we will timeGetTime() which returns milliseconds.
                long qwTicksPerSec = 0;
                useQPF = QueryPerformanceFrequency(ref qwTicksPerSec);
                if (useQPF)
                    ticksPerSecond = qwTicksPerSec;
            }
            if (useQPF)
            {
                double time;
                double fElapsedTime;
                long qwTime = 0;

                // Get either the current time or the stop time, depending
                // on whether we're stopped and what cmd was sent
                if (timeStopped != 0 && cmd != TimerCommand.Start && cmd != TimerCommand.GetAbsoluteTime)
                    qwTime = timeStopped;
                else
                    QueryPerformanceCounter(ref qwTime);

                // Return the elapsed time
                if (cmd == TimerCommand.GetElapsedTime)
                {
                    fElapsedTime = (double)(qwTime - timeElapsed) / (double)ticksPerSecond;
                    timeElapsed = qwTime;
                    return fElapsedTime;
                }

                // Return the current time
                if (cmd == TimerCommand.GetApplicationTime)
                {
                    double fAppTime = (double)(qwTime - timeBase) / (double)ticksPerSecond;
                    return fAppTime;
                }

                // Reset the timer
                if (cmd == TimerCommand.Reset)
                {
                    timeBase = qwTime;
                    timeElapsed = qwTime;
                    timeStopped = 0;
                    stopped = false;
                    return 0.0d;
                }

                // Start the timer
                if (cmd == TimerCommand.Start)
                {
                    if (stopped)
                        timeBase += qwTime - timeStopped;
                    timeStopped = 0;
                    timeElapsed = qwTime;
                    stopped = false;
                    return 0.0d;
                }

                // Stop the timer
                if (cmd == TimerCommand.Stop)
                {
                    if (!stopped)
                    {
                        timeStopped = qwTime;
                        timeElapsed = qwTime;
                        stopped = true;
                    }
                    return 0.0d;
                }

                // Advance the timer by 1/10th second
                if (cmd == TimerCommand.Advance)
                {
                    timeStopped += ticksPerSecond / 10;
                    return 0.0d;
                }

                if (cmd == TimerCommand.GetAbsoluteTime)
                {
                    time = qwTime / (double)ticksPerSecond;
                    return time;
                }

                return -1.0d; // Invalid cmd specified
            }
            else
            {
                // Get the time using timeGetTime()

                double time;
                double fElapsedTime;

                // Get either the current time or the stop time, depending
                // on whether we're stopped and what cmd was sent
                if (doubleTimeStop != 0.0 && cmd != TimerCommand.Start && cmd != TimerCommand.GetAbsoluteTime)
                    time = doubleTimeStop;
                else
                    time = timeGetTime() * 0.001;

                // Return the elapsed time
                if (cmd == TimerCommand.GetElapsedTime)
                {
                    fElapsedTime = (double)(time - doubleTimeElapsed);
                    doubleTimeElapsed = time;
                    return fElapsedTime;
                }

                // Return the current time
                if (cmd == TimerCommand.GetApplicationTime)
                {
                    return (time - doubleTimeBase);
                }

                // Reset the timer
                if (cmd == TimerCommand.Reset)
                {
                    doubleTimeBase = time;
                    doubleTimeElapsed = time;
                    doubleTimeStop = 0;
                    stopped = false;
                    return 0.0d;
                }

                // Start the timer
                if (cmd == TimerCommand.Start)
                {
                    if (stopped)
                        doubleTimeBase += time - doubleTimeStop;
                    doubleTimeStop = 0.0f;
                    doubleTimeElapsed = time;
                    stopped = false;
                    return 0.0d;
                }

                // Stop the timer
                if (cmd == TimerCommand.Stop)
                {
                    if (!stopped)
                    {
                        doubleTimeStop = time;
                        doubleTimeElapsed = time;
                        stopped = true;
                    }
                    return 0.0d;
                }

                // Advance the timer by 1/10th second
                if (cmd == TimerCommand.Advance)
                {
                    doubleTimeStop += 0.1f;
                    return 0.0d;
                }

                if (cmd == TimerCommand.GetAbsoluteTime)
                {
                    return time;
                }

                return -1.0d; // Invalid cmd specified
            }
        }
    }    // Wrapper for system level timer functions

    //*************************************************************xVector***********************************************************************************************

    public class xVector
    {
        private int dimension;
        private double[] values;
        public int Dimension
        {
            get { return dimension; }
        }
        public double this[int index]
        {
            get { return values[index]; }
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(LengthSq);
            }
        }
        public double LengthSq
        {
            get
            {
                double norm = 0;
                foreach (double d in values)
                    norm += d * d;
                return norm;
            }
        }
        public xVector Unit
        {
            get
            {
                if (LengthSq == 0)
                    throw new Exception("0 vector can not be normalized");
                return this.Scale(1.0 / Length);
            }
        }
        public xVector(params double[] input)
        {
            //
            // TODO: Add constructor logic here
            //
            dimension = input.Length;
            if (dimension == 0)
                throw new Exception("0 dimension vector is not supported");
            this.values = (double[])input.Clone();
        }

        public xVector Add(xVector v)
        {
            if (v.Dimension != this.Dimension)
            {
                throw new Exception("Trying to add two vectors with different dimensions");
            }
            double[] d = new double[this.Dimension];
            for (int i = 0; i < this.Dimension; i++)
            {
                d[i] = v[i] + this[i];
            }
            return new xVector(d);
        }

        public xVector Subtract(xVector v)
        {
            if (v.Dimension != this.Dimension)
            {
                throw new Exception("Trying to add two vectors with different dimensions");
            }
            double[] d = new double[this.Dimension];
            for (int i = 0; i < this.Dimension; i++)
            {
                d[i] = this[i] - v[i];
            }
            return new xVector(d);
        }
        public xVector Scale(double scale)
        {
            double[] d = new double[this.Dimension];
            for (int i = 0; i < this.Dimension; i++)
            {
                d[i] = this[i] * scale;
            }
            return new xVector(d);
        }
        public double Dot(xVector v)
        {
            if (v.Dimension != this.Dimension)
            {
                throw new Exception("Trying to add two vectors with different dimensions");
            }
            double d = 0;
            for (int i = 0; i < this.Dimension; i++)
            {
                d += this[i] * v[i];
            }
            return d;
        }
        public double AngleTo(xVector v)//not degree, but rad, CCW is possitive
        {
            return Math.Acos(this.Dot(v) / (this.Length * v.Length));
        }
        public bool IsPerpendicular(xVector v)
        {
            return this.Dot(v) == 0;
        }
        public bool Equals(xVector v)
        {
            if (this.Dimension != v.Dimension)
                throw new Exception("Trying to compare two vectors with different dimensions");
            for (int i = 0; i < this.Dimension; i++)
            {
                if (this[i] != v[i])
                    return false;
            }
            return true;
        }
        public override string ToString()
        {
            string s = "[";
            for (int i = 0; i < Dimension; i++)
            {
                s += "\t" + this[i].ToString("e");
            }
            s += "\t]";
            return s;
        }
    }

    //****************************************************************xVector2D****************************************************************************************

    public enum Position
    {
        IN, OUT, ON, ABOVE, BELOW, LEFT, RIGHT
    }
    public enum Winding
    {
        CW, CCW
    }
    public enum LineRelationship
    {
        HeadHead, HeadTail, TailHead, TailTail, HeadEdge, TailEdge, EdgeHead, EdgeTail, Cross, Coincident, Parallel, None
    }
    public class xVector2D : xVector
    {
        public double X
        {
            get { return this[0]; }
        }
        public double Y
        {
            get { return this[1]; }
        }
        public double Theta //from 0 to 2*PI
        {
            get
            {
                return new xVector2D(1, 0).AngleTo(this);
            }
        }
        public new xVector2D Unit
        {
            get
            {
                if (LengthSq == 0)
                    throw new Exception("0 vector can not be normalized");
                return this.Scale(1.0 / Length);
            }
        }
        public xVector2D(params double[] d)
            : base(d)
        {
            if (d.Length != 2)
                throw new Exception("xVector2D has to have exact two parameters, one if for X, another is for Y");
        }
        // a vector from point p1 to point p2
        public xVector2D(xPoint2 p1, xPoint2 p2)
            : base(p2.X - p1.X, p2.Y - p1.Y)
        {
        }

        public xVector2D(xLine2D xLine2D)
            : base(xLine2D.P2.X - xLine2D.P1.X, xLine2D.P2.Y - xLine2D.P1.Y)
        {
        }

        /// <summary>
        /// The vector turns in CCW direction, original vector is intact
        /// </summary>
        public xVector2D Turn(double degree)
        {
            double alpha = degree * Math.PI / 180.0;
            return new xVector2D(X * Math.Cos(alpha) - Y * Math.Sin(alpha), X * Math.Sin(alpha) + Y * Math.Cos(alpha));
        }
        public xVector2D Turn90()
        {
            return new xVector2D(-Y, X);
        }
        public xVector2D Turn180()
        {
            return new xVector2D(-X, -Y);
        }
        public xVector2D Turn270()
        {
            return new xVector2D(Y, -X);
        }

        public xVector2D Add(xVector2D v1)
        {
            return new xVector2D(X + v1.X, Y + v1.Y);
        }
        public new xVector2D Scale(double scalar)
        {
            return new xVector2D(X * scalar, Y * scalar);
        }
        public xVector2D Subtract(xVector2D v1)
        {
            return new xVector2D(X - v1.X, Y - v1.Y);
        }

        public double Cross(xVector2D v1) //return value ONLY
        {
            return X * v1.Y - v1.X * Y;
        }
        public double AngleTo(xVector2D u)//not degree, but rad, CCW is possitive
        {
            if (this.Cross(u) >= 0)
                return Math.Acos(this.Dot(u) / (this.Length * u.Length));
            else
                return 2.0 * Math.PI - Math.Acos(this.Dot(u) / (this.Length * u.Length));
        }

        public bool IsParallel(xVector2D v)
        {
            return this.Cross(v) == 0;
        }
    }

    //***************************************************************xVerex***********************************************************************************************

    public class xVertex : xPoint2
    {
        public bool Traversed = false;
        public List<xVertex> InComing = new List<xVertex>(2);	//incoming Vertices
        public List<xVertex> OutGoing = new List<xVertex>(2);	//outgoing Vertices


        public xVertex(xPoint2 p) : base(p.X, p.Y) { }
        public xVertex(double x, double y) : base(x, y) { }

        public xVertex NextLeft(xVertex Previous)
        {
            //find left most one
            xVertex vertexNext = null;
            foreach (xVertex o in OutGoing)
            {
                if (vertexNext == null)
                    vertexNext = o;
                else
                {
                    xVertex vertexCurrent = (xVertex)o;
                    xVector2D u0 = new xVector2D((xPoint2)this, (xPoint2)Previous).Unit;
                    xVector2D u1 = new xVector2D((xPoint2)this, (xPoint2)vertexNext).Unit;
                    xVector2D u2 = new xVector2D((xPoint2)this, (xPoint2)vertexCurrent).Unit;
                    if (u0.AngleTo(u1) > u0.AngleTo(u2))
                        vertexNext = vertexCurrent;
                }
            }
            return vertexNext;
        }
        public xVertex NextRight(xVertex Previous)
        {
            //find left most one
            xVertex vertexNext = null;
            foreach (object o in OutGoing)
            {
                if (vertexNext == null)
                    vertexNext = (xVertex)o;
                else
                {
                    xVertex vertexCurrent = (xVertex)o;
                    xVector2D u0 = new xVector2D((xPoint2)this, (xPoint2)Previous).Unit;
                    xVector2D u1 = new xVector2D((xPoint2)this, (xPoint2)vertexNext).Unit;
                    xVector2D u2 = new xVector2D((xPoint2)this, (xPoint2)vertexCurrent).Unit;
                    if (u1.Equals(u0))
                    {
                    }
                    else if (u2.Equals(u0))
                    {
                        vertexNext = vertexCurrent;
                    }
                    else if (u0.AngleTo(u1) < u0.AngleTo(u2))
                    {
                        vertexNext = vertexCurrent;
                    }
                }
            }
            return vertexNext;
        }
    }   //A vertex is a xPoint2D that also has some neighbors

   //**************************************************************xTransform****************************************************************************************

        public class xTransform
        {
            xPoint2 origin;	//new origin
            xVector2D u;		//x axis, normalized
            public xPoint2 Origin { get { return origin; } }
            public xVector2D UnitVector { get { return u; } }
            public xTransform(xPoint2 newOrigin, xVector2D newXAxis)
            {
                this.origin = newOrigin;
                u = newXAxis.Unit;
            }
            public xPoint2 Back(xPoint2 p)
            {
                double x = u.X * p.X - u.Y * p.Y;
                double y = u.X * p.Y + u.Y * p.X;
                x += origin.X;
                y += origin.Y;
                return new xPoint2(x, y);
            }
            public xPoint2 To(xPoint2 p)
            {
                double x = p.X - origin.X;
                double y = p.Y - origin.Y;

                double t = u.X * x + u.Y * y;
                y = u.X * y - u.Y * x;
                x = t;
                return new xPoint2(x, y);
            }
            public bool Equals(xTransform another)
            {
                return this.UnitVector.Equals(another.UnitVector) && this.Origin.Equals(another.Origin);
            }
        }   //end xTransform definition

    //**************************************************************xLine2D**********************************************************************************************

        public class xLine2D
        {
            /// <summary>
            /// 成员
            /// </summary>
            private xPoint2 p1, p2;

            /// <summary>
            /// 属性
            /// </summary>
            public double X1
            {
                get { return p1.X; }
                set
                {
                    if (p1.Y == p2.Y && value == p2.X)
                        throw new Exception("Two identical points can not form a xLine2D");
                    p1.X = value;
                }
            }

            public double Y1
            {
                get { return p1.Y; }
                set
                {
                    if (p1.X == p2.X && value == p2.Y)
                        throw new Exception("Two identical points can not form a xLine2D");
                    p1.Y = value;
                }
            }

            public double X2
            {
                get { return p2.X; }
                set
                {
                    if (p1.Y == p2.Y && value == p1.X)
                        throw new Exception("Two identical points can not form a xLine2D");
                    p2.X = value;
                }
            }

            public double Y2
            {
                get { return p2.Y; }
                set
                {
                    if (p1.X == p2.X && value == p1.Y)
                        throw new Exception("Two identical points can not form a xLine2D");
                    p2.Y = value;
                }
            }

            public xPoint2 P1
            {
                get { return p1; }
                set
                {
                    if (value.Equals(p2))
                        throw new Exception("Two identical points can not form a xLine2D");
                    p1 = value.Clone();
                }
            }

            public xPoint2 P2
            {
                get { return p2; }
                set
                {
                    if (value.Equals(p1))
                        throw new Exception("Two identical points can not form a xLine2D");
                    p2 = value.Clone();
                }
            }

            public xVector2D UnitVector
            {
                get
                {
                    return (new xVector2D(X2 - X1, Y2 - Y1)).Unit;
                }
            }

            public double Length
            {
                get { return Math.Sqrt(LengthSq); }
            }

            public double LengthSq
            {
                get { return p1.DistanceToSq(p2); }
            }

            public xLine2D(double x1, double y1, double x2, double y2)
            {
                if (x1 == x2 && y1 == y2)
                    throw new Exception("Two identical points can not form a xLine2D");
                this.p1 = new xPoint2(x1, y1);
                this.p2 = new xPoint2(x2, y2);
            }

            public xLine2D(xPoint2 p1, xPoint2 p2)
            {
                if (p1.Equals(p2))
                    throw new Exception("Two identical points can not form a xLine2D");
                this.p1 = p1; this.p2 = p2;
            }

            public xLine2D(xPoint2 p1, xVector2D v)
            {
                if (v.LengthSq == 0)
                    throw new Exception("Two identical points can not form a xLine2D");
                this.p1 = p1.Clone();
                this.p2 = new xPoint2(p1.X + v.X, p1.Y + v.Y);
            }

            /// <summary>
            /// DistanceTo square between the point and xLine2D segment. (Not xLine2D)
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>		
            public double DistanceSegSq(xPoint2 p)
            {

                double edx, edy;
                double edp1, edp2;
                double enx, eny, elength;

                //check the dot products to see if should use edge DistanceTo
                //or point DistanceTo
                edx = (X2 - X1);
                edy = (Y2 - Y1);
                edp1 = edx * (p.X - X1) + edy * (p.Y - Y1);
                edp2 = -edx * (p.X - X2) - edy * (p.Y - Y2);

                //if edp1 is negative then use DistanceTo to X1,Y1
                if (edp1 < 0)
                {
                    return (p.X - X1) * (p.X - X1) + (p.Y - Y1) * (p.Y - Y1);
                }
                //if edp2 is negative then use DistanceTo to X2,Y2
                if (edp2 < 0)
                {
                    return (p.X - X2) * (p.X - X2) + (p.Y - Y2) * (p.Y - Y2);
                }

                    //if both are positive then use edge DistanceTo
                else
                {
                    //remember going for squared DistanceTo here
                    eny = X2 - X1;
                    enx = -(Y2 - Y1);
                    elength = enx * enx + eny * eny;
                    return ((enx * enx) * (p.X - X1) * (p.X - X1) + 2 * enx * eny * (p.X - X1) * (p.Y - Y1) + (eny * eny) * (p.Y - Y1) * (p.Y - Y1)) / elength;
                }
            }

            public double DistanceSeg(xPoint2 p)
            {
                return Math.Sqrt(DistanceSegSq(p));
            }

            /// <summary>
            /// DistanceTo between the point and xLine2D. (Not Segment)
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            public double DistanceLn(xPoint2 p)
            {
                return Math.Sqrt(DistanceLnSq(p));
            }

            /// <summary>
            /// DistanceTo square between the point and xLine2D. (Not segment)
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            public double DistanceLnSq(xPoint2 p)
            {
                double edx, edy;
                double enx, eny, elength;
                edx = (X2 - X1);
                edy = (Y2 - Y1);
                eny = X2 - X1;
                enx = -(Y2 - Y1);
                elength = enx * enx + eny * eny;
                return ((enx * enx) * (p.X - X1) * (p.X - X1) + 2 * enx * eny * (p.X - X1) * (p.Y - Y1) + (eny * eny) * (p.Y - Y1) * (p.Y - Y1)) / elength;
            }

            public xPoint2 Projection(xPoint2 p)
            {
                //use both cross product and dot product
                double a = X2 - X1;
                double b = Y2 - Y1;
                double c = b;
                double d = -a;
                double e = a * p.X + b * p.Y;
                double f = b * X1 - a * Y1;
                double det = a * d - b * c;

                double x = (e * d - b * f) / det;
                double y = (-c * e + a * f) / det;
                return new xPoint2(x, y);
            }

            public xPoint2 MidPoint()
            {
                return new xPoint2((X1 + X2) / 2.0, (Y1 + Y2) / 2.0);
            }

            public xPoint2 PointAt(double t) //parametric t
            {
                return new xPoint2(X1 + t * (X2 - X1), Y1 + t * (Y2 - Y1));
            }

            public bool Intersects(xLine2D another)//coincident considered intersect
            {
                Position pos1 = this.PtPosition(another.P1);
                Position pos2 = this.PtPosition(another.P2);
                Position pos3 = another.PtPosition(this.P1);
                Position pos4 = another.PtPosition(this.P2);
                if (pos1 == Position.ON || pos2 == Position.ON || pos3 == Position.ON || pos4 == Position.ON)
                    return true;
                if (
                    ((pos1 == Position.LEFT && pos2 == Position.RIGHT)
                        || (pos1 == Position.RIGHT && pos2 == Position.LEFT))
                    && ((pos3 == Position.LEFT && pos4 == Position.RIGHT)
                        || (pos3 == Position.RIGHT && pos4 == Position.LEFT))
                    ) return true;
                return false;
            }

            public xPoint2 Intersection(xLine2D line)// return intersection or intersection of extensions
            {
                double u, x, y, x3, x4, y3, y4;
                x3 = line.X1; x4 = line.X2; y3 = line.Y1; y4 = line.Y2;
                double sub = ((y4 - y3) * (X2 - X1) - (x4 - x3) * (Y2 - Y1));
                if (Math.Abs(sub) == 0)
                    return null;//parallel case
                u = ((x4 - x3) * (Y1 - y3) - (y4 - y3) * (X1 - x3)) / sub;
                x = X1 + u * (X2 - X1);
                y = Y1 + u * (Y2 - Y1);
                return new xPoint2(x, y);
            }

            public bool IsParallel(xLine2D another)
            {
                return new xVector2D(this).IsParallel(new xVector2D(another));
            }

            public LineRelationship Relationship(xLine2D line)
            {
                double ua, ub, x1, x2, y1, y2, x3, x4, y3, y4;
                x1 = this.X1; x2 = this.X2; y1 = this.Y1; y2 = this.Y2;
                x3 = line.X1; x4 = line.X2; y3 = line.Y1; y4 = line.Y2;

                double denominator = ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
                if (denominator == 0)
                {
                    if ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3) == 0)
                    {
                        //exclude HeadTail, TailHead, HeadHead and TailTail cases
                        //also exclude non-overlaping case, as  parallel
                        double dx1 = Math.Abs(x2 - x1);
                        double dx2 = Math.Abs(x4 - x3);
                        double dy1 = Math.Abs(y2 - y1);
                        double dy2 = Math.Abs(y4 - y3);
                        double Mx = Math.Abs(((x4 + x3) - (x2 + x1)) / 2.0);
                        double My = Math.Abs(((y4 + y3) - (y2 + y1)) / 2.0);
                        if (Mx > (dx1 + dx2) / 2.0 || My > (dy1 + dy2) / 2.0)
                            return LineRelationship.Parallel;
                        else if (Mx < (dx1 + dx2) / 2.0 || My < (dy1 + dy2) / 2.0)
                            return LineRelationship.Coincident;
                        else
                        {
                            if (p1.Equals(line.P1)) return LineRelationship.TailTail;
                            if (p1.Equals(line.P2)) return LineRelationship.TailHead;
                            if (p2.Equals(line.P1)) return LineRelationship.HeadTail;
                            if (p2.Equals(line.P2)) return LineRelationship.HeadHead;
                        }

                    }
                    else
                        return LineRelationship.Parallel; //parallel case
                }

                if (p1.Equals(line.P1)) return LineRelationship.TailTail;
                if (p1.Equals(line.P2)) return LineRelationship.TailHead;
                if (p2.Equals(line.P1)) return LineRelationship.HeadTail;
                if (p2.Equals(line.P2)) return LineRelationship.HeadHead;

                ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / denominator;
                ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / denominator;
                if (ua < 0 || ua > 1 || ub < 0 || ub > 1)
                    return LineRelationship.None;
                else if (ua != 0 && ua != 1 && ub != 0 && ub != 1)
                    return LineRelationship.Cross;
                else if (ua == 0 && ub == 0)
                {
                    //should be TailTail, but it is actually caused by math error
                    return LineRelationship.Parallel;
                }
                else if (ua == 0 & ub == 1)
                {
                    return LineRelationship.Parallel;
                    //return LineRelationship.TailHead;
                }
                else if (ua == 1 && ub == 0)
                {
                    return LineRelationship.Parallel;
                    //return LineRelationship.HeadTail;
                }
                else if (ua == 1 && ub == 1)
                {
                    return LineRelationship.Parallel;
                    //return LineRelationship.HeadHead;
                }
                else if (ua == 0)
                    return LineRelationship.TailEdge;
                else if (ua == 1)
                    return LineRelationship.HeadEdge;
                else if (ub == 0)
                    return LineRelationship.EdgeTail;
                else if (ub == 1)
                    return LineRelationship.EdgeHead;
                else
                {
                    //MessageBox.Show("A non-handled case");
                    return LineRelationship.None;
                }
            }

            public Position PtPosition(xPoint2 p)
            {
                double ux = X2 - X1;
                double uy = Y2 - Y1;
                double vx = p.X - X1;
                double vy = p.Y - Y1;
                double crossProduct = ux * vy - vx * uy;
                if (crossProduct > 0) return Position.LEFT;
                if (crossProduct < 0) return Position.RIGHT;
                if (Math.Abs(ux) > Math.Abs(uy))
                {
                    if ((X2 - p.X) * (X1 - p.X) <= 0) return Position.ON;
                    if (X2 > X1 && p.X > X2 || X2 < X1 && p.X < X2)
                        return Position.ABOVE;
                    else return Position.BELOW;
                }
                else
                {
                    if ((Y2 - p.Y) * (Y1 - p.Y) <= 0) return Position.ON;
                    if (Y2 > Y1 && p.Y > Y2 || Y2 < Y1 && p.Y < Y2)
                        return Position.ABOVE;
                    else return Position.BELOW;
                }
            }

            public xLine2D Clone()
            {
                return new xLine2D(p1, p2);
            }

            public override string ToString()
            {
                return p1.ToString() + "---" + p2.ToString();
            }

        }    //end xLine2D definition

}// endof NameSpace
