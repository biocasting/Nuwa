/*
* ==============================================================================
* 文件名称: xLoop.cs
* 文件描述:  关于Loop（一个封闭的点集）的类文件
* 版本: 1.0
* 生产时间: $time$
* 编译器: Visual Studio 2013
* 作者: 谢宝军
* 公司: 百廷三维
* ==============================================================================
*/
using System;
using System.Collections.Generic;
using ClipperLib;

namespace Nuwa.xClass
{
    using Points = List<xPoint2>;
    using Polygon = List<IntPoint>;
    public class xLoopNode
    {
        #region 成员
        private xPoint2 pt;
        private int headPtId;
        private int tailPtId;
        public int id;
        public bool isLoopHead;
        public bool isVisited;
        #endregion

        #region 属性
        public double X
        {
            get { return this.pt.X; }
            set { this.pt.X = value; }
        }
        public long XL
        {
            get { return this.pt.XL; }
            set { this.pt.XL = value; }
        }
        public double Y
        {
            get { return this.pt.Y; }
            set { this.pt.Y = value; }
        }
        public long YL
        {
            get { return this.pt.YL; }
            set { this.pt.YL = value; }
        }

        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public int HeadPtId
        {
            get { return this.headPtId; }
            set { this.headPtId = value; }
        }

        public int TailPtId
        {
            get { return this.tailPtId; }
            set { this.tailPtId = value; }
        }

        #endregion

        #region 构造函数
        public xLoopNode()
        {
            this.pt = new xPoint2(0.0, 0.0);
        }

        public xLoopNode(double[] p)
        {
            this.pt = new xPoint2(p[0], p[1]);
        }

        public xLoopNode(xPoint2 p)
        {
            this.pt = p;
        }

        #endregion
    }
    public class xLoop
    {
        #region 成员
        private Points points;
        private Polygon polygon = new Polygon();
        public bool isOuterLoop;
        private int  id;

        //最大最小值
        private int indexOfTop = 0, indexOfBottom = 0, indexOfLeft = 0, indexOfRight = 0;
        private double maxX = double.MinValue, maxY = double.MinValue, minX = double.MaxValue, minY = double.MaxValue;
        #endregion

        #region 属性
        public int NumberOfPoints
        {
            get { return this.points.Count; }
        }

        public int NumberOfIntPoints
        {
            get { return this.polygon.Count; }
        }

        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public Polygon Polygon
        {
            get { ClosePolygon(); return this.polygon; }
            set { this.polygon = value; }
        }

        public void ClosePolygon()
        {
            if (this.NumberOfIntPoints < 2)
                return;
            if (this.polygon[0] != this.polygon[this.NumberOfIntPoints - 1])
                this.polygon.Add(new IntPoint(this.polygon[0].X, this.polygon[0].Y));
        }


        #endregion

        #region 构造函数
        public xLoop()
        {
            id = 0;
            isOuterLoop = true;
            points = new List<xPoint2>();
        }
        #endregion

        public void AddPoint (xPoint2  point)
        {
            this.points.Add(point);
            IntPoint pt = new IntPoint(point.XL, point.YL);
            this.polygon.Add(pt);
        }

        public void AddPoints(xPoint2[] points)
        {
            this.points.AddRange(points);
        }

        public xPoint2 GetPointAt(int i)
        {
            return this.points[i];
        }
       
        public xPoint2[] ToArray()
        {
            return this.points.ToArray();
        }

        public void Reverse()
        {
            this.points.Reverse();
        }

        public double MaxY
        {
            get
            {
                InitIndexPoints();
                return this.maxY;
            }
        }

        public void InitIndexPoints()
        {
            for (int i = 0; i < points.Count; i++)
            {
                xPoint2 p = this.GetPointAt(i);
                if (p.X < minX)
                {
                    this.minX = p.X;
                    this.indexOfLeft = i;
                }
                if (p.X > maxX)
                {
                    this.maxX = p.X;
                    this.indexOfRight = i;
                }
                if (p.Y < minY)
                {
                    this.minY = p.Y;
                    this.indexOfBottom = i;
                }
                if (p.Y > maxY)
                {
                    this.maxY = p.Y;
                    this.indexOfTop = i;
                }
            }

        }  // 找到Polygon的最大最小X,Y值， 以及对应的点位置

        public void Rearrange(int option)
        {
            Points pts = new Points();
            int splitIndex = 0;
            switch (option)
            {
                case 1:
                    splitIndex = this.indexOfTop;
                    break;
                case 2:
                    splitIndex = this.indexOfBottom;
                    break;
                case 3:
                    splitIndex = this.indexOfLeft;
                    break;
                case 4:
                    splitIndex = this.indexOfRight;
                    break;
            }

            if (splitIndex == 0)
                return;
            for (int i = splitIndex; i < this.points.Count; i++)
                pts.Add(this.GetPointAt(i));
            for (int i = 0; i < splitIndex; i++)
                pts.Add(this.GetPointAt(i));
            this.points.Clear();
            this.points = pts;
        }

        public int  ptInLoop(double  x, double  y)
        {
            int  i;
            int  j;
            int  o;
            bool c = false;

            for (i = 0, j = points.Count - 1; i < points.Count; j = i++)
            {
                if ((((points[i].Y <= y) && (y < points[j].Y) || ((points[j].Y <= y) && (y < points[i].Y))) && (x < (points[j].X- points[i].X) * (y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X)))
                    c = !c;
            }
            if (c) { o = 1; }   else { o = 0; }
            return  o;
        }

       public void FillWithPolygon()
        {
           foreach (IntPoint intPoint in this.polygon)
           {
               xPoint2 point = new xPoint2(intPoint.X,intPoint.Y);
               this.points.Add(point);
           }
        }
    }
}
