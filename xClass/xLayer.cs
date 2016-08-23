using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Nuwa.xClass
{

    using Loops = List<xLoop>;
    public class xLayer
    {
        /// <summary>
        /// 成员： xToolPath列表，上下角，位置和层厚
        /// </summary>       
        private List<xBoundry> boundries;
        private List<xToolPath> toolPaths;
        private double layerThickness;
        private double z;
        private int id;

        /// <summary>
        /// 属性： 所有成员的获得
        /// </summary>
        public int NumberOfBoundries
        {
            get { return this.boundries.Count;  }
        }

        public double LayerThickness
        {
            get { return this.layerThickness; }
            set { this.layerThickness = value; }
        }

        public double Z
        {
            get { return this.z; }
            set { this.z = value; }
        }

        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// 构建函数
        /// </summary>

        public xLayer()
        {
            this.boundries = new List<xBoundry>();
            //this.toolPaths = new List<xToolPath>();
            this.toolPaths = null;
            this.z = 0;
            this.layerThickness = 0.1;
            this.id = 0;
        }

        public xLayer(double z)
		{
            this.toolPaths = new List<xToolPath>();
            this.z = z;
            this.layerThickness = 0.1;
		}

        public xLayer(double z, double layerThickness)
		{
            this.toolPaths = new List<xToolPath>(); 
			this.z = z;
			if(layerThickness<0)
			{
				layerThickness=0;
			}
            this.layerThickness = layerThickness;
		}

        public void AddBoundry(xBoundry boundry)
        {
            this.boundries.Add(boundry);
        }

        public xBoundry GetBoundryAt(int i)
        {
            return this.boundries[i];
        }

        /// <summary>
        /// 增加一个xToolPath到xToolPath列表
        /// </summary>
        public void AddToolPath(xToolPath polygon)
        {
            this.toolPaths.Add(polygon);
        }

        /// <summary>
        /// 返回xToolPath列表位置为index的polygon
        /// </summary>
        public xToolPath GetToolPathAt(int i)
        {
            return this.toolPaths[i];
        }

        /// <summary>
        /// xToolPath列表中的xToolPath总数
        /// </summary>
        public int NumberOfToolPaths
        {
            get 
            {
                if (this.toolPaths==null)
                    return 0;
                else
                return this.toolPaths.Count; 
            }
        }

        public void GetToolPathsWithBorders()
        {
            for (int i = 0; i < this.NumberOfBoundries; i++)   // 遍历各个xToolPath
            {
                xBoundry boundry = this.GetBoundryAt(i);
                if (boundry == null)
                    continue;
                this.toolPaths = boundry.GetToolPathsWithBorders(i);
            }// end for i
        }


        public void FillToolPathsFromBorders(double offset, double firstOffset, int fillPattern)
        {
            //each material is this.assembly pen, from pen1 to pen 4
            for (int j = 0; j < NumberOfToolPaths; j++)
            {
                xToolPath toolpath =GetToolPathAt(j);
                switch (fillPattern)
                {
                    case 0: // 等值填充
                        toolpath.Paths.Clear();
                        toolpath.SetSubject();
                        for (int k = 1; ; k++)
                        {
                            Loops lps = toolpath.GetLoops(offset * k);
                            if (lps.Count == 0)
                                break;
                            else
                            {
                                foreach (xLoop loop in lps)
                                {
                                    toolpath.Paths.AddRange(lps);
                                }
                            }

                        }
                        break;

                    case 1: // 扫描填充
                        toolpath.Paths.Clear();
                        xVector2D v1 = new xVector2D(1, 0);
                        if (id % 2 == 0)
                        {
                            foreach (xToolPath tp in toolpath.GetTooPathsbyOffset(offset))
                            {
                                toolpath.Paths.AddRange(tp.RasterPath(offset, v1));
                            }
                            foreach (xLoop line in toolpath.Paths)
                            {
                                line.Reverse();
                            }
                        }
                        else
                        {
                            foreach (xToolPath tp in toolpath.GetTooPathsbyOffset(offset))
                            {
                                toolpath.Paths.AddRange(tp.RasterPath(offset, v1));
                            }
                        }
                        break;


                    case 2: // XY交叉扫描
                        toolpath.Paths.Clear();

                        if (id % 2 == 0)
                        {
                            xVector2D v2 = new xVector2D(0, 1);
                            foreach (xToolPath tp in toolpath.GetTooPathsbyOffset(offset))
                            {
                                toolpath.Paths.AddRange(tp.RasterPath(offset, v2));
                            }
                        }
                        else
                        {
                            xVector2D v2 = new xVector2D(1, 0);
                            foreach (xToolPath tp in toolpath.GetTooPathsbyOffset(offset))
                            {
                                toolpath.Paths.AddRange(tp.RasterPath(offset, v2));
                            }
                        }
                        break;


                    case 3: // 外轮廓内扫描
                        toolpath.Paths.Clear();

                        if (id % 2 == 0)
                        {
                            toolpath.Rearange(2);
                            toolpath.Paths.AddRange(toolpath.Borders);
                            xVector2D v2 = new xVector2D(0, 1);
                            foreach (xToolPath tp in toolpath.GetTooPathsbyOffset(offset))
                            {
                                toolpath.Paths.AddRange(tp.RasterPath(offset, v2));
                            }
                        }
                        else
                        {
                            toolpath.Rearange(1);
                            toolpath.Paths.AddRange(toolpath.Borders);
                            xVector2D v2 = new xVector2D(1, 0);
                            foreach (xToolPath tp in toolpath.GetTooPathsbyOffset(offset))
                            {
                                toolpath.Paths.AddRange(tp.RasterPath(offset, v2));
                            }
                        }
                        break;

                } //  end switch

            } // end for


        }


    } // End Class
} // End NameSpace
