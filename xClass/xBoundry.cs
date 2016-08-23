using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuwa.xClass
{
    using Loops = List<xLoop>;
    using Points = List<xPoint2>;
    using Nodes = List<xLoopNode>;
    using ToolPaths = List<xToolPath>;

    public class xBoundry
    {
        private Nodes nodes;
        private xLoopNode prevNode;
        private Loops loops;
        private Points featurepoints;

         public xBoundry()
        {
            this.nodes = new List<xLoopNode>();
            this.prevNode = new xLoopNode();
            this.loops = new List<xLoop>();       
            this.featurepoints = new List<xPoint2>();
        }

         public int NumberOfNodes
        {
            get { return this.nodes.Count; }
        }// Nodes中所包含的node数量

         public int NumberOfLoops
         {
             get { return this.loops.Count; }
         }// Loops中所包含的loop数量

        private xPoint2 GetFeaturePointAt(int i)
         {
               return this.GetLoopAt(i).GetPointAt(0);
         }

         #region Node
         public xLoopNode GetNodeAt(int i)
        {
             return this.nodes[i];
         }

        public void AddNode(xLoopNode node)
        {
            this.nodes.Add(node);
        }

        private bool FindUnvisitedNode(xLoopNode node)
        {
            return  !node.isVisited;
        }

        private bool FindNextNode(xLoopNode node)
        {
            if (this.prevNode.isVisited)
                return  false;
            else if (this.prevNode.TailPtId == node.HeadPtId)
                return  true;
            else
                return  false;
        }

        public void SortNodes()
        {
            // Initialize parameteres
            bool IsNewLoop = true;
            int  IdCount = 0;

            // Sor the cell by x
            nodes.Sort((Comparison<xLoopNode>)delegate(xLoopNode a, xLoopNode b) { return a.X> b.X? 1 : a.X== b.X ? 0 : -1; });

            for (; IdCount < this.NumberOfNodes; )
            {
                if (IsNewLoop)
                {
                    xLoopNode node = this.nodes.Find(FindUnvisitedNode);
                    node.id = IdCount++;
                    node.isLoopHead = true;
                    node.isVisited = true;
                    IsNewLoop = false;
                    prevNode.TailPtId = node.TailPtId;
                }
                xLoopNode node2 = nodes.Find(FindNextNode);
                if (node2.isVisited)
                {
                    IsNewLoop = true;
                }
                else
                {
                    node2.id = IdCount++;
                    node2.isVisited = true;
                    prevNode.TailPtId = node2.TailPtId;
                }
            }
            //Finally line up
            nodes.Sort((Comparison<xLoopNode>)delegate(xLoopNode a, xLoopNode b) { return  a.id > b.id ? 1 : a.id == b.id ? 0 : -1; });
        }

        #endregion

        public void AddLoop(xLoop loop)
        {
                this.loops.Add(loop);
        }

        public void AddLoops(Loops loops)
        {
            foreach (xLoop lp in loops)
            {
                this.loops.Add(lp);
            }

        }

        public xLoop GetLoopAt(int i)
        {
            return this.loops[i];
        }// 返回Polyloop中位置为i的loop，注意i不能大于loop总数。

        public void MarkOuterLoops()
        {
            int count = this.NumberOfLoops;
            if (count< 2)
                return;
            for (int i = 1; i < this.NumberOfLoops; i++)
            {
                int nCheck = 0;
                xLoop CurrentLoop = this.GetLoopAt(i);
                xLoop prevLoop;
                for (int j = 0; j < i; j++)
                {
                    prevLoop = this.GetLoopAt(j);
                    xPoint2 fp = GetFeaturePointAt(i);
                    int nIsIn = prevLoop.ptInLoop(fp.X, fp.Y);
                    nCheck += nIsIn;
                }
                int bIsInnerLoop = (int)nCheck % 2;
                if (bIsInnerLoop != 0)
                {
                    CurrentLoop.isOuterLoop = false;
                }
                else
                {
                    CurrentLoop.isOuterLoop = true;
                }
            }

            for (int i = 1; i < count; i++)
            {
                xLoop CurrentLoop = this.GetLoopAt(i);
                xLoop prevLoop;
                for (int j = 0; j < i; j++)
                {
                    prevLoop = this.GetLoopAt(j);
                    xPoint2 fp = GetFeaturePointAt(i);
                    int nIsIn = prevLoop.ptInLoop(fp.X, fp.Y);
                    if (nIsIn != 0)
                    {
                        if (prevLoop.isOuterLoop)
                        {
                            if (CurrentLoop.isOuterLoop)
                            {
                                CurrentLoop.Id = prevLoop.Id + 100;
                            }
                            else
                            {
                                CurrentLoop.Id = prevLoop.Id + 1;
                            }
                        }
                    }
                    else
                    {
                        if (prevLoop.isOuterLoop)
                        {
                            if (CurrentLoop.isOuterLoop)
                            {
                                CurrentLoop.Id = prevLoop.Id + 100;
                            }
                        }
                        else
                        {
                            if (prevLoop.Id == CurrentLoop.Id)
                            {
                                CurrentLoop.Id++;
                            }
                        }
                    }
                }
            }
            loops.Sort((Comparison<xLoop>)delegate(xLoop a, xLoop b) { return a.Id > b.Id ? 1 : a.Id == b.Id ? 0 : -1; });
        }//  判断特征点是否在loop中，如果是，则这个loop是内环。

        public void FillLoopsFromNodes()
        {
            SortNodes();
            xLoop thisLoop = new xLoop();
            for (int i = 0; i < this.NumberOfNodes; i++)
            {
                xLoopNode node = GetNodeAt(i);
                if (node.isLoopHead)
                {
                    xLoop loop = new xLoop();
                    AddLoop(loop);
                    thisLoop = loop;
                    thisLoop.AddPoint(node.X, node.Y);
                }
                else
                {
                    thisLoop.AddPoint(node.X, node.Y);
                }
            }

        }

        public ToolPaths GetToolPathsWithBorders( int material)
        {
            ToolPaths tps = new ToolPaths();
            xToolPath toolpath = null;
            xLoop border = null;
            for (int i = 0; i < this.NumberOfLoops; i++) // 遍历各个Loop
            {
                xLoop loop = this.GetLoopAt(i);
                if (loop.isOuterLoop)
                {
                    toolpath = new xToolPath();
                    toolpath.Material = material;
                    tps.Add(toolpath);
                    border = new xLoop();
                    toolpath.Borders.Add(border);
                }
                else
                {
                    border = new xLoop();
                    toolpath.Borders.Add(border);
                }

                for (int j = 0; j < loop.NumberOfPoints; j++) // 遍历Loop中各个点
                {
                    border.AddPoint(loop.GetPointAt(j));
                } // end for i
            }// end for j

            return tps;
        }




    }   // end xBoundry class
} // end namespace
