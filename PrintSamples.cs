using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToothFairy.xClass;

namespace ToothFairy
{

    public struct iPoint2D
    {
        public int X;
        public int Y;

        public iPoint2D(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static iPoint2D operator -(iPoint2D p1, iPoint2D p2)
        {
            return new iPoint2D(p1.X - p2.X, p1.Y - p2.Y); ;
        }
    }

    public struct iLine2D
    {
        public iPoint2D P1;
        public iPoint2D P2;

        public iLine2D(iPoint2D p1, iPoint2D p2)
        {
            this.P1 = p1;
            this.P2 = p2;
        }

        public string ToLiString()
        {
            iPoint2D dp = this.P1 - this.P2;
            if (dp.X == 0 && dp.Y == 0)
                return "";
            else
                return "LI " + dp.X.ToString() + "," + dp.Y.ToString() + ",0" + System.Environment.NewLine;
        }

        public static string ToLiStringUpZ(int z)
        {
            return "LI 0,0," + z.ToString() + System.Environment.NewLine + "REM ***********New Layer*********************" + System.Environment.NewLine;
        }

    }

    class Sample
    {
        public List<iPoint2D> points;
        public iPoint2D startPoint;
        private int length;
        private int width;
        private int lineSpacing;
        private int lineHeight;
        private int lengthLineNum;
        private int widthLineNum;
        private int lenWid;
        private int lenWidLineNum;

        public Sample()
        {
            this.points = new List<iPoint2D>();
            startPoint = new iPoint2D(0, 0);
            this.length = 0;
            this.width = 0;
            this.lenWid = 0;
            this.lineSpacing = 0;
            this.lineHeight = 0;
            this.lengthLineNum = 0;
            this.widthLineNum = 0;
            this.lenWidLineNum = 0;
        }

        public void AddPoint(iPoint2D pt)
        {
            this.points.Add(pt);
        }

        public iPoint2D GetPointAt(int i)
        {
            return this.points[i];
        }

        public void Clear()
        {
            this.points.Clear();
        }

        public int NumberOfPoints
        {
            get { return this.points.Count; }
        }

        public static bool IsOdd(int n)
        {
            return Convert.ToBoolean(n % 2);
        }

        public iPoint2D GetFirstPt()
        {
            if (this.NumberOfPoints > 0)
                return this.points[0];
            else
                return new iPoint2D(0, 0);
        }

        public int Length
        {
            get { return this.length; }
        }

        public int Width
        {
            get { return this.width; }
        }

        public iPoint2D GetLastPt()
        {
            if (this.NumberOfPoints > 0)
                return this.points[this.NumberOfPoints - 1];
            else
                return new iPoint2D(0, 0);
        }

        public void Boundry(iPoint2D pt_start, int l, int w)
        {
            iPoint2D P1 = new iPoint2D(0, 0);
            iPoint2D P2 = new iPoint2D(0, w);
            iPoint2D P3 = new iPoint2D(l, w);
            iPoint2D P4 = new iPoint2D(l, 0);
            if (pt_start.X == 0 && pt_start.Y == 0)
            {
                this.AddPoint(P1);this.AddPoint(P2); this.AddPoint(P3); this.AddPoint(P4); 
            }
            else if (pt_start.X > 0 && pt_start.Y == 0)
            {
                this.AddPoint(P4);this.AddPoint(P1); this.AddPoint(P2); this.AddPoint(P3); 
            }
            else if (pt_start.X == 0 && pt_start.Y > 0)
            {
                this.AddPoint(P2);this.AddPoint(P3); this.AddPoint(P4); this.AddPoint(P1); 
            }
            else
            {
                this.AddPoint(P3);this.AddPoint(P4); this.AddPoint(P1); this.AddPoint(P2); 
            }
        }


        public int[] bounds
        {
            get
            {
                if (this.NumberOfPoints > 0)
                {
                    int maxX = int.MinValue;
                    int maxY = int.MinValue;
                    int minX = int.MaxValue;
                    int minY = int.MaxValue;
                    for (int i = 0; i < this.NumberOfPoints; i++)
                    {
                        iPoint2D poly = this.points[i];
                        maxX = maxX > poly.X ? maxX : poly.X;
                        maxY = maxY > poly.Y ? maxY : poly.Y;
                        minX = minX < poly.X ? minX : poly.X;
                        minY = minY < poly.Y ? minY : poly.Y;
                    }
                    return new int[4] { minX, maxX, minY, maxY };

                }
                return new int[4] { 0, 0, 0, 0 };
            }
        }


        public void SetDimesion(int length, int width, int linespacing, int lineheight)
        {
            this.lineSpacing = linespacing;
            this.lineHeight = lineheight;
            this.lengthLineNum = (int)(length / linespacing);

            this.length = this.lengthLineNum * linespacing;
            this.widthLineNum = (int)(width / linespacing);
            this.width = this.widthLineNum * linespacing;
            this.lenWidLineNum = (int)((length > width ? width : length) / linespacing);
            this.lenWid = this.lenWidLineNum * linespacing;
        }

        //circleNum


        public void FillRaster(iPoint2D start, bool axis)
        {
            iPoint2D p1, p2, p3, p4;
            int dx = 1, dy = 1;
            if (start.X == 0)
                dx = 1;
            else
                dx = -1;

            if (start.Y == 0)
                dy = 1;
            else
                dy = -1;
            startPoint.X = start.X; startPoint.Y = start.Y;

            AddPoint(start);

            if (axis)
            {
                for (int i = 0; i < this.lengthLineNum / 2; i++)
                {
                    p1.X = start.X;
                    p1.Y = start.Y + dy * this.width;
                    AddPoint(p1);

                    p2.X = p1.X + dx * this.lineSpacing;
                    p2.Y = p1.Y;
                    AddPoint(p2);

                    p3.X = p2.X;
                    p3.Y = p2.Y - dy * this.width;
                    AddPoint(p3);

                    p4.X = p3.X + dx * this.lineSpacing;
                    p4.Y = p3.Y;
                    AddPoint(p4);

                    start.X = p4.X; start.Y = p4.Y;
                }

                if (IsOdd(this.lengthLineNum))
                {
                    p1.X = start.X;
                    p1.Y = start.Y + dy * this.width;
                    AddPoint(p1);

                    p2.X = p1.X + dx * this.lineSpacing;
                    p2.Y = p1.Y;
                    AddPoint(p2);

                    p3.X = p2.X;
                    p3.Y = p2.Y - dy * this.width;
                    AddPoint(p3);
                }
                else
                {
                    p1.X = start.X;
                    p1.Y = start.Y + dy * this.width;
                    AddPoint(p1);
                }
            }
            else
            {

                for (int i = 0; i < this.widthLineNum / 2; i++)
                {
                    p1.X = start.X + dx * this.length;
                    p1.Y = start.Y;
                    AddPoint(p1);

                    p2.X = p1.X;
                    p2.Y = p1.Y + dy * this.lineSpacing;
                    AddPoint(p2);

                    p3.X = p2.X - dx * this.length;
                    p3.Y = p2.Y;
                    AddPoint(p3);

                    p4.X = p3.X;
                    p4.Y = p3.Y + dy * this.lineSpacing;
                    AddPoint(p4);

                    start.X = p4.X; start.Y = p4.Y;
                }
                if (IsOdd(this.widthLineNum))
                {
                    p1.X = start.X + dx * this.length;
                    p1.Y = start.Y;
                    AddPoint(p1);

                    p2.X = p1.X;
                    p2.Y = p1.Y + dy * this.lineSpacing;
                    AddPoint(p2);

                    p3.X = p2.X - dx * this.length;
                    p3.Y = p2.Y;
                    AddPoint(p3);
                }
                else
                {
                    p1.X = start.X + dx * this.length;
                    p1.Y = start.Y;
                    AddPoint(p1);

                }

            }

        }// if direction +1 -->递增 ， axis + --》 X方向

        public void FillSpiral(iPoint2D start, bool axis)
        {
            iPoint2D p1, p2, p3, p4;
            startPoint.X = start.X; startPoint.Y = start.Y;
            AddPoint(start);

            if (axis)
            {
                start.Y = start.Y - this.lineSpacing;
                for (int i = 0; i < this.lenWidLineNum / 2; i++)
                {
                    p1.X = start.X;
                    p1.Y = start.Y + this.width - (2 * i) * this.lineSpacing;
                    AddPoint(p1);

                    p2.X = p1.X + this.length - (2 * i) * this.lineSpacing;
                    p2.Y = p1.Y;
                    AddPoint(p2);

                    p3.X = p2.X;
                    p3.Y = p2.Y - this.width + (2 * i + 1) * this.lineSpacing;
                    AddPoint(p3);

                    p4.X = p3.X - this.length + (2 * i + 1) * this.lineSpacing;
                    p4.Y = p3.Y;
                    AddPoint(p4);

                    start.X = p4.X; start.Y = p4.Y;
                }

                if (IsOdd(this.lenWidLineNum))
                {
                    p1.X = start.X;
                    p1.Y = start.Y + this.width - this.lenWidLineNum * this.lineSpacing;
                    AddPoint(p1);

                    p2.X = p1.X + this.length - this.lenWidLineNum * this.lineSpacing;
                    p2.Y = p1.Y;
                    AddPoint(p2);

                    p3.X = p2.X;
                    p3.Y = p2.Y - this.width + (this.lenWidLineNum + 1) * this.lineSpacing;
                    AddPoint(p3);

                    p4.X = p3.X - this.length + (this.lenWidLineNum + 1) * this.lineSpacing;
                    p4.Y = p3.Y;
                    AddPoint(p4);
                }

            }
            else
            {
                start.X = start.X - this.lineSpacing;
                for (int i = 0; i < this.lenWidLineNum / 2; i++)
                {
                    p1.X = start.X + this.length - (2 * i) * this.lineSpacing;
                    p1.Y = start.Y;
                    AddPoint(p1);

                    p2.X = p1.X;
                    p2.Y = p1.Y + this.width - (2 * i) * this.lineSpacing;
                    AddPoint(p2);

                    p3.X = p2.X - this.length + (2 * i + 1) * this.lineSpacing;
                    p3.Y = p2.Y;
                    AddPoint(p3);

                    p4.X = p3.X;
                    p4.Y = p3.Y - this.width + (2 * i + 1) * this.lineSpacing;
                    AddPoint(p4);

                    start.X = p4.X; start.Y = p4.Y;
                }

                if (IsOdd(this.lenWidLineNum))
                {
                    p1.X = start.X + this.length - this.lenWidLineNum * this.lineSpacing;
                    p1.Y = start.Y;
                    AddPoint(p1);

                    p2.X = p1.X;
                    p2.Y = p1.Y + this.width - this.lenWidLineNum * this.lineSpacing;
                    AddPoint(p2);

                    p3.X = p2.X - this.length + (this.lenWidLineNum + 1) * this.lineSpacing;
                    p3.Y = p2.Y;
                    AddPoint(p3);

                    p4.X = p3.X;
                    p4.Y = p3.Y - this.width + (this.lenWidLineNum + 1) * this.lineSpacing;
                    AddPoint(p4);
                }

            }

        }// if direction +1 -->递增 ， axis + --》 X方向

        public void FillSpiralWall(iPoint2D start, bool axis, int count)
        {
            iPoint2D p1, p2, p3, p4;
            startPoint.X = start.X; startPoint.Y = start.Y;
            AddPoint(start);

            if (axis)
            {
                start.Y = start.Y - this.lineSpacing;
                for (int i = 0; i < count; i++)
                {
                    p1.X = start.X;
                    p1.Y = start.Y + this.width - (2 * i) * this.lineSpacing;
                    AddPoint(p1);

                    p2.X = p1.X + this.length - (2 * i) * this.lineSpacing;
                    p2.Y = p1.Y;
                    AddPoint(p2);

                    p3.X = p2.X;
                    p3.Y = p2.Y - this.width + (2 * i + 1) * this.lineSpacing;
                    AddPoint(p3);

                    p4.X = p3.X - this.length + (2 * i + 1) * this.lineSpacing;
                    p4.Y = p3.Y;
                    AddPoint(p4);

                    start.X = p4.X; start.Y = p4.Y;
                }
            }
            else
            {
                start.X = start.X - this.lineSpacing;
                for (int i = 0; i < count; i++)
                {
                    p1.X = start.X + this.length - (2 * i) * this.lineSpacing;
                    p1.Y = start.Y;
                    AddPoint(p1);

                    p2.X = p1.X;
                    p2.Y = p1.Y + this.width - (2 * i) * this.lineSpacing;
                    AddPoint(p2);

                    p3.X = p2.X - this.length + (2 * i + 1) * this.lineSpacing;
                    p3.Y = p2.Y;
                    AddPoint(p3);

                    p4.X = p3.X;
                    p4.Y = p3.Y - this.width + (2 * i + 1) * this.lineSpacing;
                    AddPoint(p4);

                    start.X = p4.X; start.Y = p4.Y;
                }


            }

        }// if direction +1 -->递增 ， axis + --》 X方向

        public string GetLIStringRaster()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < this.NumberOfPoints; i++)
            {
                iLine2D line = new iLine2D(GetPointAt(i), GetPointAt(i - 1));
                sb.Append(line.ToLiString());
            }
            sb.Append(iLine2D.ToLiStringUpZ(this.lineHeight));
            return sb.ToString();
        }

        public string GetLIStringSpiral(bool spiral_in)
        {
            StringBuilder sb = new StringBuilder();
            if (spiral_in)
            {
                for (int i = 1; i < this.NumberOfPoints; i++)
                {
                    iLine2D line = new iLine2D(GetPointAt(i), GetPointAt(i - 1));
                    sb.Append(line.ToLiString());
                }
                sb.Append(iLine2D.ToLiStringUpZ(this.lineHeight));
            }
            else
            {
                for (int i = this.NumberOfPoints - 1; i > 0; i--)
                {
                    iLine2D line = new iLine2D(GetPointAt(i - 1), GetPointAt(i));
                    sb.Append(line.ToLiString());
                }
                sb.Append(iLine2D.ToLiStringUpZ(this.lineHeight));
            }
            return sb.ToString();
        }




    }

}
