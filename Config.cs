using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuwa
{
    public class Config
    {
        public static double LineHeight = 0.4;
        public static double FirstLayerHeight
        {
            get { return LineHeight * 0.75; }
        }

        public static double GearRatio
        {
            get { return 0.065 * LineSpacing - 0.006; }
        }
        public static double LineSpacing = 0.4;
        public static int FillPattern = 3; //填充方式
        public static double EGrearRatio = 0.02;  //电子齿轮比
        public static double FristOffset = 0.4;
        public static double Offset = 0.4;
        public static double BuildSpeed = 10;  //10mm/s
        public static double JogSpeed = 0.1;
    }
}
