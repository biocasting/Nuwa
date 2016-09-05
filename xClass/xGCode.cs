using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuwa.xClass
{
    public class xGCode
    {

        public static List<xSegment3> Path = new List<xSegment3>();
        public static Double preSpeed = 0;
        public static xPoint3 prePoint = new xPoint3(0, 0, 0);

        public xGCode()
        {

        }
        
        public static void Parse(string line)
        {
            if (line == null || line == "")
                return;
            xSegment3 sm = null;
            string[] segments = line.Split(' '); // 将每行分解成不同字符串
            foreach (string s in segments)   // 对每个字符串进行解释
            {
                if (s.Contains(";"))
                    break;
                if (s == "G0" || s == "G00")
                { 
                    sm = new xSegment3(SegmentType.Line);
                    sm.EndPoint.SetValue(prePoint);
                    sm.Speed = preSpeed;
                }     
                else if (s == "G1" || s == "G01")
                {
                    sm = new xSegment3(SegmentType.Line);
                    sm.EndPoint.SetValue(prePoint);
                }
                else if (s == "G2" || s == "G02")
                {
                    sm = new xSegment3(SegmentType.Arc);
                    sm.EndPoint.SetValue(prePoint);
                }
               else if (s.Contains("F"))
                {
                    sm.Speed= Convert.ToDouble(s.Substring(1));
                }
                else if (s.Contains("X"))
                {
                    if (sm != null)
                    {
                        sm.EndPoint.X = Convert.ToDouble(s.Substring(1));
                    }
                }
                else if (s.Contains("Y"))
                {
                     if (sm != null)
                    {
                        sm.EndPoint.Y = Convert.ToDouble(s.Substring(1));
                    }
                }
                else if (s.Contains("Z"))
                {
                     if (sm != null)
                    {
                        sm.EndPoint.Z = Convert.ToDouble(s.Substring(1));
                    }
                }
                //else if (s.Contains("E"))
            }


            if (sm != null)
            {
                sm.StartPoint.SetValue(prePoint);
                prePoint.SetValue(sm.EndPoint);
                preSpeed = sm.Speed;
                xGCode.Path.Add(sm);
                //Console.WriteLine("line {0}  ; start {1} ; end {2}", line, sm.StartPoint.ToString(), sm.EndPoint.ToString()  );
            }
        }

    }
}
