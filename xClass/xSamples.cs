using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuwa.xClass;

namespace Nuwa.xClass
{
    using SampleLayer = List<xSegment2>;
    using SamplePath = List<List<xSegment2>>;
    public class xSample
    {
        public static SamplePath Path = new SamplePath();
        public static List<double> ZList = new List<double>();
        public static xPoint2 LastPoint = new xPoint2(0, 0); // 用于记忆图形最后一个点，然后赋予矢量第一个点。
        public static xPoint2 Origin = new xPoint2(10, 10); // 形状的起点
        public static string LiString = "";
        public static string Li_ZUp = "LI 0,0," + Convert.ToInt32((Config.LineSpacing)*1000).ToString() + Environment.NewLine;
        public static string Li_ZUpFirstLayer = "LI 0,0," + Convert.ToInt32((Config.FirstLayerHeight) * 1000).ToString() + Environment.NewLine;
        public static string VE_END = "VE\r\nBGS\r\nAMS"+ Environment.NewLine;
        public static string PR_ZUp = "PR 0,0," + Convert.ToInt32((Config.LineSpacing) * 1000).ToString() + "\r\nBGC\r\nAMC" + Environment.NewLine;
        public static string PR_ZUpFirstLayer = "PR 0,0," + Convert.ToInt32((Config.FirstLayerHeight) * 1000).ToString() + "\r\nBGC\r\nAMC" + Environment.NewLine;
        public xSample()
        {
        }
        public static void AddPoint(int index,xPoint2 point)
        {
            xSegment2 line = new xSegment2( LastPoint.Clone(), point.Clone());
            SampleLayer layer = Path[index];
            layer.Add(line);
            LastPoint.Copy(point);
        }
        public static void AddArc(int index, double r, double startangle, double endangle)
        {
            xSegment2 line = new xSegment2(LastPoint.Clone(), r, startangle, endangle);
            SampleLayer layer = Path[index];
            layer.Add(line);
            LastPoint.Copy(line.EndPoint);
        }
        public static bool IsOdd(int n)
        {
            return Convert.ToBoolean(n % 2);
        }
        public static xPoint2 GetLastPt(int index)
        {
            SampleLayer layer = Path[index];
            if (layer.Count > 0)
                return layer[layer.Count - 1].EndPoint;
            else
                return null ;
        }

        // 单层路径生成

        // 此函数生成长L，宽W的长方形，此长方形左下角为（0，0）。如果Start为0，0点，则
        public static void Boundry(xPoint2 start, double L, double W, int index)
        {
            double lineSpacing = Config.LineSpacing;
            int lengthLineNum = (int)(L / Config.LineSpacing);
            double length = lengthLineNum * Config.LineSpacing;
            int widthLineNum = (int)(W / Config.LineSpacing);
            double width = widthLineNum * Config.LineSpacing;

            xPoint2 P1 = new xPoint2(Origin.X, Origin.Y);
            xPoint2 P2 = new xPoint2(Origin.X, width + Origin.Y);
            xPoint2 P3 = new xPoint2(length + Origin.X, width + Origin.X);
            xPoint2 P4 = new xPoint2(length + Origin.X, Origin.Y);

            if (start.XL == Origin.XL && start.YL == Origin.YL)
            {
                AddPoint(index, P1); AddPoint(index, P2); AddPoint(index, P3); AddPoint(index, P4); AddPoint(index, P1);
            }
            else if (start.XL > Origin.XL && start.YL == Origin.XL)
            {
                AddPoint(index, P4); AddPoint(index, P1); AddPoint(index, P2); AddPoint(index, P3); AddPoint(index, P4);
            }
            else if (start.XL == Origin.XL && start.Y > Origin.YL)
            {
                AddPoint(index, P2); AddPoint(index, P3); AddPoint(index, P4); AddPoint(index, P1); AddPoint(index, P2);
            }
            else
            {
                AddPoint(index, P3); AddPoint(index, P4); AddPoint(index, P1); AddPoint(index, P2); AddPoint(index, P3);
            }
        }
        public static void FillRasterSquare(xPoint2 start, bool axis, double L, double W, int index)
        {
            xPoint2 p1 = new xPoint2(0,0);
            xPoint2 p2 = new xPoint2(0, 0);
            xPoint2 p3 = new xPoint2(0, 0);
            xPoint2 p4 = new xPoint2(0, 0);

            double lineSpacing = Config.LineSpacing;
            int lengthLineNum = (int)( L / Config.LineSpacing);
            double length = lengthLineNum * Config.LineSpacing;
            int widthLineNum = (int)( W / Config.LineSpacing);
            double width = widthLineNum * Config.LineSpacing;
            
            int dx = 1, dy = 1;
            if (start.XL == Origin.XL)
                dx = 1;
            else
                dx = -1;

            if (start.YL == Origin.YL)
                dy = 1;
            else
                dy = -1;

            if (axis)
            {
                for (int i = 0; i < lengthLineNum / 2; i++)
                {
                    p1.X = start.X;
                    p1.Y = start.Y + dy * width;
                    AddPoint(index,p1);

                    p2.X = p1.X + dx * lineSpacing;
                    p2.Y = p1.Y;
                    AddPoint(index,p2);

                    p3.X = p2.X;
                    p3.Y = p2.Y - dy * width;
                    AddPoint(index,p3);

                    p4.X = p3.X + dx * lineSpacing;
                    p4.Y = p3.Y;
                    AddPoint(index,p4);

                    start.X = p4.X; start.Y = p4.Y;
                }

                if (IsOdd(lengthLineNum))
                {
                    p1.X = start.X;
                    p1.Y = start.Y + dy * width;
                    AddPoint(index,p1);

                    p2.X = p1.X + dx * lineSpacing;
                    p2.Y = p1.Y;
                    AddPoint(index,p2);

                    p3.X = p2.X;
                    p3.Y = p2.Y - dy * width;
                    AddPoint(index,p3);
                }
                else
                {
                    p1.X = start.X;
                    p1.Y = start.Y + dy * width;
                    AddPoint(index,p1);
                }
            }
            else
            {

                for (int i = 0; i < widthLineNum / 2; i++)
                {
                    p1.X = start.X + dx * length;
                    p1.Y = start.Y;
                    AddPoint(index,p1);

                    p2.X = p1.X;
                    p2.Y = p1.Y + dy * lineSpacing;
                    AddPoint(index,p2);

                    p3.X = p2.X - dx * length;
                    p3.Y = p2.Y;
                    AddPoint(index,p3);

                    p4.X = p3.X;
                    p4.Y = p3.Y + dy * lineSpacing;
                    AddPoint(index,p4);

                    start.X = p4.X; start.Y = p4.Y;
                }
                if (IsOdd(widthLineNum))
                {
                    p1.X = start.X + dx * length;
                    p1.Y = start.Y;
                    AddPoint(index,p1);

                    p2.X = p1.X;
                    p2.Y = p1.Y + dy * lineSpacing;
                    AddPoint(index,p2);

                    p3.X = p2.X - dx * length;
                    p3.Y = p2.Y;
                    AddPoint(index,p3);
                }
                else
                {
                    p1.X = start.X + dx * length;
                    p1.Y = start.Y;
                    AddPoint(index,p1);
                }
            }

        }// if direction +1 -->递增 ， axis + --》 X方向
        public static void FillContourSquare(xPoint2 start, bool axis, double L, double W, int index)
        {
            xPoint2 p1 = new xPoint2(0, 0);
            xPoint2 p2 = new xPoint2(0, 0);
            xPoint2 p3 = new xPoint2(0, 0);
            xPoint2 p4 = new xPoint2(0, 0);

            double lineSpacing = Config.LineSpacing;
            int lengthLineNum = (int)(L / Config.LineSpacing);
            double length = lengthLineNum * Config.LineSpacing;
            int widthLineNum = (int)(W / Config.LineSpacing);
            double width = widthLineNum * Config.LineSpacing;
            int lenWidLineNum = (int)((L > W ? W : L) / Config.LineSpacing);
            double lenWid = lenWidLineNum * Config.LineSpacing;

            if (axis)
            {
                start.Y = start.Y - lineSpacing;
                for (int i = 0; i < lenWidLineNum / 2; i++)
                {
                    p1.X = start.X;
                    p1.Y = start.Y + width - (2 * i) * lineSpacing;
                    AddPoint(index,p1);

                    p2.X = p1.X + length - (2 * i) * lineSpacing;
                    p2.Y = p1.Y;
                    AddPoint(index,p2);

                    p3.X = p2.X;
                    p3.Y = p2.Y - width + (2 * i + 1) * lineSpacing;
                    AddPoint(index,p3);

                    p4.X = p3.X - length + (2 * i + 1) * lineSpacing;
                    p4.Y = p3.Y;
                    AddPoint(index,p4);

                    start.X = p4.X; start.Y = p4.Y;
                }

                if (IsOdd(lenWidLineNum))
                {
                    p1.X = start.X;
                    p1.Y = start.Y + width - lenWidLineNum * lineSpacing;
                    AddPoint(index,p1);

                    p2.X = p1.X + length - lenWidLineNum * lineSpacing;
                    p2.Y = p1.Y;
                    AddPoint(index,p2);

                    p3.X = p2.X;
                    p3.Y = p2.Y - width + (lenWidLineNum + 1) * lineSpacing;
                    AddPoint(index,p3);

                    p4.X = p3.X - length + (lenWidLineNum + 1) * lineSpacing;
                    p4.Y = p3.Y;
                    AddPoint(index,p4);
                }

            }
            else
            {
                start.X = start.X - lineSpacing;
                for (int i = 0; i < lenWidLineNum / 2; i++)
                {
                    p1.X = start.X + length - (2 * i) * lineSpacing;
                    p1.Y = start.Y;
                    AddPoint(index,p1);

                    p2.X = p1.X;
                    p2.Y = p1.Y + width - (2 * i) * lineSpacing;
                    AddPoint(index,p2);

                    p3.X = p2.X - length + (2 * i + 1) * lineSpacing;
                    p3.Y = p2.Y;
                    AddPoint(index,p3);

                    p4.X = p3.X;
                    p4.Y = p3.Y - width + (2 * i + 1) * lineSpacing;
                    AddPoint(index,p4);

                    start.X = p4.X; start.Y = p4.Y;
                }

                if (IsOdd(lenWidLineNum))
                {
                    p1.X = start.X + length - lenWidLineNum * lineSpacing;
                    p1.Y = start.Y;
                    AddPoint(index,p1);

                    p2.X = p1.X;
                    p2.Y = p1.Y + width - lenWidLineNum * lineSpacing;
                    AddPoint(index,p2);

                    p3.X = p2.X - length + (lenWidLineNum + 1) * lineSpacing;
                    p3.Y = p2.Y;
                    AddPoint(index,p3);

                    p4.X = p3.X;
                    p4.Y = p3.Y - width + (lenWidLineNum + 1) * lineSpacing;
                    AddPoint(index,p4);
                }

            }

        }// if direction +1 -->递增 ， axis + --》 X方向
        public static void FillContourWallSquare(xPoint2 start, bool axis, double L, double W, double T, int index)
        {
            xPoint2 p1 = new xPoint2(0, 0);
            xPoint2 p2 = new xPoint2(0, 0);
            xPoint2 p3 = new xPoint2(0, 0);
            xPoint2 p4 = new xPoint2(0, 0);
            double lineSpacing = Config.LineSpacing;
            int lengthLineNum = (int)(L / Config.LineSpacing);
            double length = lengthLineNum * Config.LineSpacing;
            int widthLineNum = (int)(W / Config.LineSpacing);
            double width = widthLineNum * Config.LineSpacing;
            int count = (int)(T / Config.LineSpacing);

            //LastPoint.X = start.X; LastPoint.Y = start.Y;
            AddPoint(index, start);

            if (axis)
            {
                start.Y = start.Y - lineSpacing;
                for (int i = 0; i < count; i++)
                {
                    p1.X = start.X;
                    p1.Y = start.Y + width - (2 * i) * lineSpacing;
                    AddPoint(index, p1);

                    p2.X = p1.X + length - (2 * i) * lineSpacing;
                    p2.Y = p1.Y;
                    AddPoint(index, p2);

                    p3.X = p2.X;
                    p3.Y = p2.Y - width + (2 * i + 1) * lineSpacing;
                    AddPoint(index, p3);

                    p4.X = p3.X - length + (2 * i + 1) * lineSpacing;
                    p4.Y = p3.Y;
                    AddPoint(index, p4);

                    start.X = p4.X; start.Y = p4.Y;
                }
            }
            else
            {
                start.X = start.X - lineSpacing;
                for (int i = 0; i < count; i++)
                {
                    p1.X = start.X + length - (2 * i) * lineSpacing;
                    p1.Y = start.Y;
                    AddPoint(index, p1);

                    p2.X = p1.X;
                    p2.Y = p1.Y + width - (2 * i) * lineSpacing;
                    AddPoint(index, p2);

                    p3.X = p2.X - length + (2 * i + 1) * lineSpacing;
                    p3.Y = p2.Y;
                    AddPoint(index, p3);

                    p4.X = p3.X;
                    p4.Y = p3.Y - width + (2 * i + 1) * lineSpacing;
                    AddPoint(index, p4);

                    start.X = p4.X; start.Y = p4.Y;
                }


            }
        }
        public static void FillContourCircle(xPoint2 start, bool sin, double D1, double D2, int index)  // D1 外圆  D2 内圆   H1  总高 H2 底部
        {
            xPoint2 pt1 = new xPoint2(0, 0);
            xPoint2 pt2 = new xPoint2(0, 0);

            int numberOfCircles = (int)((D1 - D2) * 0.5 / Config.LineSpacing);
            double actualTubeOutDiameter = D2 + Config.LineSpacing * (numberOfCircles - 1) * 2;
            double actualTubeInDiameter = D2;
            double IR = actualTubeInDiameter / 2.0;
            double OR = actualTubeOutDiameter / 2.0;

            //if (index == 0)
            //    AddPoint(index, new xPoint2(IR, 0));

            if (sin)
            {
                for (int j = 0; j < numberOfCircles; j++)
                {
                    double r = IR + Config.LineSpacing * j;
                    double p = r + Config.LineSpacing +Origin.X;
                    AddArc(index, r, 0, 360);
                    if (j < numberOfCircles - 1)
                        AddPoint(index, new xPoint2(p, Origin.Y));
                }
            }
            else
            {
                for (int j = 0; j < numberOfCircles; j++)
                {
                    double r = OR - Config.LineSpacing * j;
                    double p = r - Config.LineSpacing + Origin.X;
                    AddArc(index, r, 0, 360);
                    if (j < numberOfCircles - 1)
                        AddPoint(index, new xPoint2(p, Origin.Y));
                }

            }
        }

        // 堆积成最终形状
        public static void CylinderContourFill(double D1, double D2, double H)  // D1 外圆  D2 内圆   H1  总高 H2 底部
        {
            int numberOfLayers = (int)(H / Config.LineHeight);
            double actualTubeHeight = numberOfLayers * Config.LineHeight;
            xPoint2 vpStart = new xPoint2(0, 0);
            for (int i = 0; i < numberOfLayers; i++)
            {
                if (i ==0)
                    LiString += PR_ZUpFirstLayer;
                else
                    LiString += PR_ZUp;
                LiString += "REM Layer" + i.ToString() + Environment.NewLine;
                xPoint2 pt_start = new xPoint2(0, 0);
                SampleLayer layer = new SampleLayer();
                Path.Add(layer);
                FillContourCircle(pt_start, (i % 2 == 0), D1, D2, i);
                 
                for (int j = 0; j < layer.Count; j++)
                {
                    xSegment2 line = layer[j];
                    if (line.Type == SegmentType.Line)
                        LiString += line.VP_LINE(vpStart);
                    else
                        LiString += line.CR_LINE;
                    if (j == 0)
                        LiString += "BGS" + System.Environment.NewLine; 
                }
                vpStart.Copy(GetLastPt(i));
                LiString += VE_END;

            }
        }
        public static void RoundCrucibleContourFill(double D1, double D2, double H1, double H2)  // D1 外圆  D2 内圆   H1  总高 H2 底部
        {
            int numberOfLayers = (int)(H1 / Config.LineHeight);
            double actualTubeHeight = numberOfLayers * Config.LineHeight;
            xPoint2 vpStart = new xPoint2(0, 0);
            int numberOfBottomLayers = (int)(H2 / Config.LineHeight);
            double actualTubeBottomHeight = numberOfBottomLayers * Config.LineHeight;

            xPoint2 pt_start = new xPoint2(0, 0);
            for (int i = 0; i < numberOfBottomLayers; i++)
            {
                if (i == 0)
                    LiString += PR_ZUpFirstLayer;
                else
                    LiString += PR_ZUp;
                LiString += "REM Layer" + i.ToString() + Environment.NewLine;
                SampleLayer layer = new SampleLayer();
                Path.Add(layer);
                FillContourCircle(pt_start, (i % 2 == 0), D1, Config.LineSpacing*2.0, i);
                for (int j = 0; j < layer.Count; j++)
                {
                    xSegment2 line = layer[j];
                    if (line.Type == SegmentType.Line)
                        LiString += line.VP_LINE(vpStart);
                    else
                    { 
                        LiString += line.CR_LINE;
                    }
                    if (j == 0)
                        LiString += "BGS" + System.Environment.NewLine; 
                }
                vpStart.Copy(GetLastPt(i));
                LiString += VE_END;

            }

            pt_start.Copy(vpStart);  //
            for (int i = numberOfBottomLayers; i < numberOfLayers; i++)
            {
                LiString += PR_ZUp;
                LiString += "REM Layer" + i.ToString() + Environment.NewLine;
                SampleLayer layer = new SampleLayer();
                Path.Add(layer);
                FillContourCircle(pt_start, (i % 2 == 0), D1, D2, i);
                if (i == numberOfBottomLayers)
                    layer.RemoveAt(0);    
                for (int j = 0; j < layer.Count; j++)
                {

                        xSegment2 line = layer[j];
                    if (line.Type == SegmentType.Line)
                        LiString += line.VP_LINE(vpStart);
                    else
                    {
                        LiString += line.CR_LINE;
                        Console.WriteLine("CP X{0} Y{1}", line.arcCenterPoint.XL, line.arcCenterPoint.YL);
                    }
                }
                vpStart.Copy(GetLastPt(i));
                LiString += VE_END;


            }
            //int numberOfLayers = (int)(H1 / Config.LineHeight);
            //double actualTubeHeight = numberOfLayers * Config.LineHeight;

            //int numberOfCircles = (int)((D2 - D1) * 0.5 / Config.LineSpacing);
            //double actualTubeOutDiameter = D2 + Config.LineSpacing * numberOfCircles * 2;
            //double actualTubeInDiameter = D2;

            //int numbOfBottomLayers = (int)(H2 / Config.LineHeight);

            //int OR = (int)(actualTubeOutDiameter * 500);
            //int IR = (int)(actualTubeInDiameter * 500);
            //int ls = (int)(Config.LineSpacing * 1000);
            //int ls2 = (int)(Config.LineSpacing * 500);
            //int lh = (int)(Config.LineHeight * 1000);

            //LiString += "VP " + (OR + 1200).ToString() + ",0" + System.Environment.NewLine;
            //LiString += "CR " + (OR + 1200).ToString() + ",0,360" + System.Environment.NewLine;
            //LiString += "VP " + (OR + 1000).ToString() + ",0" + System.Environment.NewLine;
            //LiString += "CR " + (OR + 1000).ToString() + ",0,360" + System.Environment.NewLine;
            //LiString += "VP " + (IR + ls2).ToString() + ",0" + System.Environment.NewLine;
            //LiString += "BGS\r\nVE " + System.Environment.NewLine;
            //LiString += "AMS " + System.Environment.NewLine;

            //IR = 0;
            //numberOfCircles = (int)(D1 * 0.5 / Config.LineSpacing);
            //for (int i = 0; i < numbOfBottomLayers; i++)
            //{
            //    //LiString += Ini.StartPolyLine;
            //    if (i % 2 == 0)
            //    {
            //        for (int j = 0; j < numberOfCircles; j++)
            //        {
            //            int offset1 = IR + j * ls + ls2;
            //            int offset2 = (j + 1) * ls;
            //            LiString += "CR " + offset1.ToString() + ",0,360" + System.Environment.NewLine;
            //            if (j < numberOfCircles - 1)
            //                LiString += "VP " + offset2.ToString() + ",0" + System.Environment.NewLine;
            //            if (j == 0)
            //                LiString += "BGS" + System.Environment.NewLine; ;
            //        }
            //    }
            //    else
            //    {
            //        for (int j = 0; j < numberOfCircles; j++)
            //        {
            //            int offset1 = OR - j * ls - ls2;
            //            int offset2 = -(j + 1) * ls;
            //            LiString += "CR " + offset1.ToString() + ",0,360" + System.Environment.NewLine;
            //            if (j < numberOfCircles - 1)
            //                LiString += "VP " + offset2.ToString() + ",0" + System.Environment.NewLine;
            //            if (j == 0)
            //                LiString += "BGS" + System.Environment.NewLine; ;
            //        }
            //    }
            //    LiString += "VE\r\nAMS\r\n";
            //    LiString += "PR ,," + lh.ToString() + "\r\nBGC\r\nAMC\r\n";
            //}

            //IR = (int)(actualTubeInDiameter * 500);
            //numberOfCircles = (int)((D1 - D2) * 0.5 / Config.LineSpacing);

            //for (int i = numbOfBottomLayers; i < numberOfLayers; i++)
            //{
            //    //LiString += Ini.StartPolyLine;

            //    if (i % 2 == 0)
            //    {
            //        for (int j = 0; j < numberOfCircles; j++)
            //        {
            //            int offset1 = IR + j * ls + ls2;
            //            int offset2 = (j + 1) * ls;
            //            LiString += "CR " + offset1.ToString() + ",0,360" + System.Environment.NewLine;
            //            if (j < numberOfCircles - 1)
            //                LiString += "VP " + offset2.ToString() + ",0" + System.Environment.NewLine;
            //            if (j == 0)
            //                LiString += "BGS" + System.Environment.NewLine; ;
            //        }
            //    }
            //    else
            //    {
            //        for (int j = 0; j < numberOfCircles; j++)
            //        {
            //            int offset1 = OR - j * ls - ls2;
            //            int offset2 = -(j + 1) * ls;
            //            LiString += "CR " + offset1.ToString() + ",0,360" + System.Environment.NewLine;
            //            if (j < numberOfCircles - 1)
            //                LiString += "VP " + offset2.ToString() + ",0" + System.Environment.NewLine;
            //            if (j == 0)
            //                LiString += "BGS" + System.Environment.NewLine; ;
            //        }
            //    }
            //    LiString += "VE\r\nAMS\r\n";
            //    LiString += "PR ,," + lh.ToString() + "\r\nBGC\r\nAMC\r\n";
            //}

        }
        public static void CubeRasterFill(double L, double W, double H)
        {
            int numberOfLayers = (int)( H / Config.LineHeight );
           xPoint2 start = Origin.Clone();
            //LastPoint.X = 100; LastPoint.Y = 100;
            for (int i = 0; i < numberOfLayers; i++)
            {
                if (i==0)
                    LiString += Li_ZUpFirstLayer;
                else
                    LiString += Li_ZUp;
                LiString += "REM Layer" + i.ToString() + Environment.NewLine;
                SampleLayer layer = new SampleLayer();
                Path.Add(layer);
                Boundry(start, L, W,i);
                FillRasterSquare(start, (i % 2 == 0 ? true : false), L, W, i); //
                start.Copy(GetLastPt(i));
                for (int j = 0; j < layer.Count; j++ )
                {
                    xSegment2 line  = layer[j];
                    LiString += line.LI_LINE;
                    if ( i==0 &&j == 0)
                        LiString += "BGS" + System.Environment.NewLine;
                    Console.WriteLine("  {0}  ", line.ToString());
                }
            }
        }
        public static void CubeCoutourFill(double L, double W, double H)
        {
            int numberOfLayers = (int)(H / Config.LineHeight);

            xSample.Path.Clear();
            for (int i = 0; i < numberOfLayers; i++)
            {
                xPoint2 start = Origin.Clone(); ;
                Console.WriteLine("Layer {0}", i.ToString());
                if (i == 0)
                    LiString += Li_ZUp;
                else
                    LiString += Li_ZUpFirstLayer;
                LiString += "REM Layer" + i.ToString() + Environment.NewLine;
                SampleLayer layer = new SampleLayer();
                Path.Add(layer);
                Boundry(start, L, W, i);
                FillContourSquare(start,false, L, W, i); 
                if (i % 2 == 1)
                {
                    layer.Reverse();
                    layer.RemoveAt(layer.Count - 1);
                }
                else
                {
                    layer.RemoveAt(0);
                }
                for (int j = 0; j < layer.Count; j++)
                {
                    xSegment2 line = layer[j];
                    if (i % 2 == 1)
                    {
                        line.Swap();
                    }
                    LiString += line.LI_LINE;
                    Console.WriteLine("{0}", line.ToString());
                    if (i == 0 && j == 0)
                        LiString += "BGS" + System.Environment.NewLine; 
                }


            }
        }
        public static void SquareCrucibleCoutourFill(double L, double W, double T, double H1, double H2)
        {
            int numberOfLayers = (int)(H1 / Config.LineHeight);
            int numberofBottomLayers = (int)(H2 / Config.LineHeight);
            xPoint2 pt_start;
            xSample.Path.Clear();
            for (int i = 0; i < numberofBottomLayers; i++)
            {
                if (i == 0)
                    LiString += Li_ZUpFirstLayer;
                else
                    LiString += Li_ZUp;
                LiString += "REM Layer" + i.ToString() + Environment.NewLine;
                pt_start = new xPoint2(0, 0);
                SampleLayer layer = new SampleLayer();
                Path.Add(layer);
                Boundry(pt_start, L, W, i);
                FillContourSquare(pt_start, false, L, W, i);
                if (i % 2 == 1)
                {
                    layer.Reverse();
                    layer.RemoveAt(layer.Count - 1);
                }
                else
                {
                    layer.RemoveAt(0);
                }

                for (int j = 0; j < layer.Count; j++)
                {
                    if (i % 2 == 1)
                    {
                        layer[j].Swap();
                    }
                    LiString += layer[j].LI_LINE;
                    if (i == 0 && j == 0)
                        LiString += "BGS" + System.Environment.NewLine; 
                }

            }


            for (int i = numberofBottomLayers; i < numberOfLayers; i++)
            {
                LiString += Li_ZUp;
                LiString += "REM Layer" + i.ToString() + Environment.NewLine;
                pt_start = new xPoint2(0, 0);
                SampleLayer layer = new SampleLayer();
                Path.Add(layer);
                Boundry(pt_start, L, W, i);
                FillContourWallSquare(pt_start, false, L, W, T, i);
                if (i % 2 == 1)
                {
                    layer.Reverse();
                    layer.RemoveAt(layer.Count - 1);
                }
                else
                {
                    layer.RemoveAt(0);
                }
                for (int j = 0; j < layer.Count; j++)
                {

                    if (i % 2 == 1)
                    {
                        layer[j].Swap();
                    }
                    LiString += layer[j].LI_LINE;
                }

            }


        }

    }

}
