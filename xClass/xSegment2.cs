using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuwa.xClass
{
    public class xSegment2
    {
        private xPoint2 startPoint;
        private xPoint2 endPoint;
        //private int composition = 0;
        private double radius; 
       public xPoint2 arcCenterPoint;
        private double startAngle;
        private double endAngle;
        private double speed = 1000;   // mm/min
        private SegmentType type = SegmentType.Line;
        public List<xPoint2> arcPoints;
        public const int NumberOfArcPoints = 50;
        private int id = 0;


        public xPoint2 StartPoint
        {
            get{ return this.startPoint;}
            set{ this.startPoint = value; }
        }

        public xPoint2 EndPoint
        {
            get { return this.endPoint; }
            set { this.endPoint = value; }
        }

        public int Radius
        {
            get { return (int)(this.radius * 1000); }
        }

        public int ID
        {
            get {return this.id;}
            set { this.id = value; }
        }

        //public xPoint2 ArcCenterPoint
        //{
        //    get { return this.arcCenterPoint; }
        //    set { this.arcCenterPoint = value; }
        //}

        //public double StartAngle
        //{
        //    get{ return angle.X;}
        //    set{ angle.X = value;}
        //}

        //public double EndAngle
        //{
        //    get{ return angle.Y;}
        //    set{ angle.Y = value;}
        //}

        public double Speed
        {
            get { return this.speed; }
            set { this.speed = value; }
        }

        public SegmentType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public xSegment2()
        {
            this.startPoint = new xPoint2(0,0);
            this.endPoint = new xPoint2(0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public xSegment2(xPoint2 start, xPoint2 end)
        {
            this.startPoint = start;
            this.endPoint = end;
            this.type = SegmentType.Line;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public xSegment2(xPoint2 start, xPoint2 end, float speed)
        {
            this.startPoint = start;
            this.endPoint = end;
            this.speed = speed;
            this.type = SegmentType.Line;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="center">中心点</param>
        /// <param name="startangle">开始角度</param>
        /// <param name="endangle">结束角度</param>
        public xSegment2(xPoint2 start, double radius, double startangle,double endangle)
        {

            this.startPoint = start;
            this.radius = radius;
            this.startAngle = startangle;
            this.endAngle = endangle;
            this.type = SegmentType.Arc;
            this.arcPoints = new List<xPoint2>();
            double cx = startPoint.X - COS(this.startAngle) * this.radius;
            double cy= startPoint.Y - SIN(this.startAngle) * this.radius;
            this.arcCenterPoint = new xPoint2(cx, cy);
            double ex = startPoint.X + (COS(endAngle)-COS(this.startAngle) )*this.radius;
            double ey= startPoint.Y + (SIN(endAngle) - SIN(this.startAngle) ) * this.radius;
            this.endPoint = new xPoint2(ex, ey);
        }
        public string LI_LINE
        {
            get {

                if (endPoint.XL == startPoint.XL && endPoint.YL == startPoint.Y)
                    return "";
                else
                return "LI " + (endPoint.XL - startPoint.XL).ToString() + ", " + (endPoint.YL - startPoint.YL).ToString() + ",0" + Environment.NewLine; 
            }
        }

        public string CR_LINE
        {
            get { return "CR " + Radius.ToString() + ", " + startAngle.ToString() + "," + endAngle.ToString() + Environment.NewLine; }
        }
        public string VP_LINE(xPoint2 VP_StartPoint)
        {
            return "VP " + (endPoint.XL - VP_StartPoint.XL).ToString() + ", " + (endPoint.YL - VP_StartPoint.YL).ToString() + Environment.NewLine; 
        }
        public xPoint2 arcPoint(double sita)
        {
            double x = arcCenterPoint.X + Math.Cos((this.startAngle + sita) / 180.0 * Math.PI) * this.radius;
            double y = arcCenterPoint.Y + Math.Sin((this.startAngle + sita) / 180.0 * Math.PI) * this.radius;
            return new xPoint2(x, y);
        }
        public double SIN(double angle)
        {
            return Math.Sin(angle / 180.0 * Math.PI);
        }
        public double COS(double angle)
        {
            return Math.Cos(angle / 180.0 * Math.PI);
        }
        public List<xPoint2> GetArcPoints()
        {
            double delta = (endAngle - startAngle) / NumberOfArcPoints;
            for (int i = 0; i < NumberOfArcPoints+1; i++)
            {
                this.arcPoints.Add(arcPoint(delta * i));
            }
            return this.arcPoints;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void Swap()
        {
            double x = this.startPoint.X;
            double y = this.startPoint.Y;
            this.startPoint.X = this.endPoint.X;
            this.startPoint.Y = this.endPoint.Y;
            this.endPoint.X = x;
            this.endPoint.Y = y;
        }


        /// <summary>
        ///  方法： 输出此点坐标的文字
        /// </summary>
        public override string ToString()
        {
            return this.startPoint.X.ToString("f3") + "," + this.startPoint.Y.ToString("f3") + ";" + this.endPoint.X.ToString("f3") + "," + this.endPoint.Y.ToString("f3");
        }

    }
}
