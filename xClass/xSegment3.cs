using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuwa.xClass
{
   public enum SegmentType
    {
        Line,
        Arc
    }
    public class xSegment3
    {
        private xPoint3 startPoint;
        private xPoint3 endPoint;
        private xPoint3 VP_StartPoint;
        //private int composition = 0;
        private xPoint3 arcCenterPoint;
        private xPoint2 angle;
        private double speed = 1000;   // mm/min
        private SegmentType type = SegmentType.Line;


        public xPoint3 StartPoint
        {
            get{ return this.startPoint;}
            set{ this.startPoint = value; }
        }

        public xPoint3 EndPoint
        {
            get { return this.endPoint; }
            set { this.endPoint = value; }
        }
        public xPoint3 ArcCenterPoint
        {
            get { return this.arcCenterPoint; }
            set { this.arcCenterPoint = value; }
        }

        public double StartAngle
        {
            get{ return angle.X;}
            set{ angle.X = value;}
        }

        public double EndAngle
        {
            get{ return angle.Y;}
            set{ angle.Y = value;}
        }

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

        public xSegment3(SegmentType Type)
        {
            this.startPoint = new xPoint3(0,0,0);
            this.endPoint = new xPoint3(0, 0, 0);
            if (this.type == SegmentType.Arc)
            {
                this.angle = new xPoint2(0,360);
                this.arcCenterPoint = new xPoint3(1,0,0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public xSegment3(xPoint3 start, xPoint3 end, float speed)
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
        public xSegment3(xPoint3 start, xPoint3 end, xPoint3 center, double startangle,double endangle, double speed)
        {
            this.startPoint = start;
            this.endPoint = end;
            this.arcCenterPoint = center;
            this.angle = new xPoint2(startangle, endangle);
            this.type = SegmentType.Arc;
        }

        public string LI_LINE
        {
            get { return "LI " + (endPoint.X - startPoint.X).ToString("f3") + ", " + (endPoint.Y - startPoint.Y).ToString("f3") + ", " + (endPoint.Z - startPoint.Z).ToString("f3"); }
        }

        public string VP_LINE
        {
            get { return "VP " + (endPoint.X - VP_StartPoint.X).ToString("f3") + ", " + (endPoint.Y - VP_StartPoint.Y).ToString("f3") + ", " + (endPoint.Z - VP_StartPoint.Z).ToString("f3"); }
        }

    }
}
