using System;

namespace Nuwa.xClass
{

    public class xPoint3
    {
        #region 成员

        //  点的X,Y坐标
        private double x,y,z;
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
        /// 属性：返回Z坐标的值
        /// </summary>
        public double Z
        {
            get { return this.z; }
            set { this.z = value; }
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


        public long ZL
        {
            get { return Convert.ToInt64(this.z*PRECISION + 0.5d) ; }
            set { this.z = Convert.ToDouble(value / PRECISION); }
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
        public xPoint3(double x, double y,double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
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
        public xPoint3(int x, int y, int z)
        {
            this.x = Convert.ToDouble(x); this.y = Convert.ToDouble(y); this.z = Convert.ToDouble(z);
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
        public double DistanceTo(xPoint3 other)
        {
            return Math.Sqrt( DistanceToSq(other));
        }

        /// <summary>
        ///  方法： 到其他点的距离的平方
        /// </summary>
        public double DistanceToSq(xPoint3 other)
        {
            return (other.x - this.x) * (other.x - this.x) + (other.y - this.y) * (other.y - this.y) + (other.z - this.z) * (other.z - this.z);
        }

        /// <summary>
        ///  方法： 判断与其他点是否相同
        /// </summary>
        public bool Equals(xPoint3 other)
        {
            return this.x == other.X && this.y == other.Y && this.z == other.Z;
        }

        /// <summary>
        ///  方法： 得到这个点的拷贝，而非引用
        /// </summary>
        public xPoint3 Clone()
        {
            return new xPoint3(this.x, this.y, this.z);
        }


        public void SetValue( xPoint3  other)
        {
            this.x = other.X; this.y =other.Y; this.z = other.Z; 
        }



        /// <summary>
        ///  方法： 输出此点坐标的文字
        /// </summary>
        public override string ToString()
        {
            return "[" + this.x.ToString() + "," + this.y.ToString() +  "," + this.z.ToString()+"]";
        }

    }




}
