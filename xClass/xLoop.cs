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

namespace Nuwa.xClass
{
    using Points = List<xPoint2>;

    public class xLoopNode
    {
        /// <summary>
        /// 成员变量
        /// 线段的头点head和尾点tail,以及线段的Id  (head) ---> (tail)
        /// </summary>
        private xPoint2 pt;
        private int headPtId;
        private int tailPtId;
        public int id;
        public bool isLoopHead;
        public bool isVisited;

        /// <summary>
        /// 属性:  xLoopNode线段的XYZ，Id指的是head点的XYZ.,Id
        /// </summary>
        public double X
        {
            get { return this.pt.X; }
            set { this.pt.X = value; }
        }

        public double Y
        {
            get { return this.pt.Y; }
            set { this.pt.Y = value; }
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
        /// <summary>
        /// xLoopNode构造函数，初始化所包含的一个点
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
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

    }
    public class xLoop
    {
        /// <summary>
        /// 成员:  包含loop所有点的List 以及,Loop的Id，
        /// 以及判断这个Loop是最外面的Loop（True）还是里面的Loop（False）的参数
        /// </summary>
        private Points points;
        public bool isOuterLoop;
        private int  id;
        private int indexOfTop = 0, indexOfBottom = 0, indexOfLeft = 0, indexOfRight = 0;
        private double maxX = double.MinValue, maxY = double.MinValue, minX = double.MaxValue, minY = double.MaxValue;
        
        /// <summary>
        /// Points(List)的中xPoint2D点的数量
        /// </summary>
        public int NumberOfPoints
        {
            get { return this.points.Count; }
        }
        /// <summary>
        /// 返回Points(List)的位置为i的xPoint点，注意i不能大于xPoint点总数。
        /// </summary>
        public xPoint2 GetPointAt(int i)
        {
            return this.points[i];
        }

        /// <summary>
        /// 属性:  Loop的Id
        /// </summary>
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// 构造函数，所有属性初始化为0
        /// </summary>
        public xLoop()
        {
            id = 0;
            isOuterLoop = true;
            points = new List<xPoint2>();
        }
        /// <summary>
        /// 在Points的List中增加一个点
        /// </summary>
        public void AddPoint (xPoint2  p)
        {
            points.Add(p);
        }
        /// <summary>
        /// 在Points(List)中增加一个坐标为XYZ的xPoint2D点
        /// </summary>
        public void AddPoint (double  x, double  y)
        {
            xPoint2  p = new xPoint2 (x, y);
            points.Add(p);
        }

        public void AddPoints(xPoint2[] ps)
        {
            points.AddRange(ps);
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

        /// <summary>
        /// 检查xy组成的点是否在loop内
        /// </summary>
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


    }
}
