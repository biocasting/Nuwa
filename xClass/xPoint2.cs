using System;

namespace Nuwa.xClass
{

    public class xPoint2
    {
        #region 成员

        //  点的X,Y坐标
        private double x,y;
        // 用于排序和拓扑关系Id
        int id = 0;
        //Double转换为Long的精确度
        private const double PRECISION = 1000; 

        #endregion 

        #region 成员
        /// <summary>
        ///  属性：返回X坐标的值
        /// </summary>
        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        /// <summary>
        /// 属性：返回Y坐标的值
        /// </summary>
        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }
        /// <summary>
        /// 返回X的long类型数值
        /// </summary>
        public long XL
        {
            get { return Convert.ToInt64(this.x * PRECISION + 0.5d); }
            set { this.y = Convert.ToDouble(value / PRECISION); }
        }

        public long YL
        {
            get { return Convert.ToInt64(this.y*PRECISION + 0.5d) ; }
            set { this.y = Convert.ToDouble(value / PRECISION); }
        }

        /// <summary>
        ///  属性：返回Id的值
        /// </summary>
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }


        #endregion

        /// <summary>
        ///  方法： 通过设置X,Y坐标的Double值得到点
        /// </summary>
        public xPoint2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        ///// <summary>
        /////   方法： 通过设置X,Y坐标的float值得到点
        ///// </summary>
        //public xPoint2(float x, float y)
        //{
        //    this.x = Convert.ToDouble(x); ; this.y = Convert.ToDouble(y);
        //}

        /// <summary>
        ///   方法： 通过设置X,Y坐标的int值得到点
        /// </summary>
        /// 
        public xPoint2(int x, int y)
        {
            this.x = Convert.ToDouble(x); this.y = Convert.ToDouble(y);
        }

        ///// <summary>
        /////   方法： 通过设置X,Y坐标的long值得到点
        ///// </summary>
        //public xPoint2(long x, long y)
        //{
        //    this.x = Convert.ToDouble(x); this.y = Convert.ToDouble(y);
        //}

        /// <summary>
        ///  方法： 到其他点的距离
        /// </summary>
        public double DistanceTo(xPoint2 other)
        {
            return Math.Sqrt((other.x - this.x) * (other.x - this.x) + (other.y - this.y) * (other.y - this.y));
        }

        /// <summary>
        ///  方法： 到其他点的距离的平方
        /// </summary>
        public double DistanceToSq(xPoint2 other)
        {
            return (other.x - this.x) * (other.x - this.x) + (other.y - this.y) * (other.y - this.y);
        }

        /// <summary>
        ///  方法： 判断与其他点是否相同
        /// </summary>
        public bool Equals(xPoint2 other)
        {
            return this.x == other.X && this.y == other.Y;
        }

        /// <summary>
        ///  方法： 得到这个点的拷贝，而非引用
        /// </summary>
        public xPoint2 Clone()
        {
            return new xPoint2(this.x, this.y);
        }

        /// <summary>
        ///  方法： 输出此点坐标的文字
        /// </summary>
        public override string ToString()
        {
            return "[" + this.x.ToString() + "," + this.y.ToString() + "]";
        }

    }




}
