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

        #region 成员
        private List<xBoundry> boundries; 
        private List<xToolPath> toolPaths;
        private double layerThickness; 
        private double z; 
        private int id; 
        #endregion

        #region 属性
        public int NumberOfBoundries
        {
            get { return this.boundries.Count;  }
        }

        public int NumberOfToolPaths
        {
            get
            {
                if (this.toolPaths == null)
                    return 0;
                else
                    return this.toolPaths.Count;
            }
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

        #endregion

        #region 构建函数
        public xLayer()
        {
            this.boundries = new List<xBoundry>();
            //this.toolPaths = new List<xToolPath>();
            this.toolPaths = null;
            this.z = 0;
            this.layerThickness = 0.1;
            this.id = 0;
        }

        # endregion

        public void AddBoundry(xBoundry boundry)
        {
            this.boundries.Add(boundry);
        }
        public xBoundry GetBoundryAt(int i)
        {
            return this.boundries[i];
        }
        public xToolPath GetToolPathAt(int i)
        {
            return this.toolPaths[i];
        }
        public void CreateToolPathsWithBordersOnly()
        {
            for (int i = 0; i < this.NumberOfBoundries; i++)   // 遍历各个xToolPath
            {
                xBoundry boundry = this.GetBoundryAt(i);
                if (boundry == null)
                    continue;
                this.toolPaths = boundry.CreateToolPathsWithBordersOnly(i);
            }// end for i
        }
        public void FillToolPathsFromBorders(double offset, int fillPattern)
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
