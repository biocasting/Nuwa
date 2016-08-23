using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuwa.xClass
{

    public class xPolynode
    {
        public List<Node> nodes;

        public int NumberOfNodes
        {
            get { return this.nodes.Count; }
        }

        public Node GetNodeAt(int i)
        {
            return this.nodes[i];
        }

        public void SetNode(int i, Node node)
        {
            this.nodes[i] = node;
        }

        public xPolynode()
        {
            this.nodes = new List<Node>();
        }

        public void AddNode(Node node)
        {
            this.nodes.Add(node);
        }

        public void AddRange(Node[] nodes)
        {
            this.nodes.AddRange(nodes);
        }

        public Node[] ToArray()
        {
            return this.nodes.ToArray();
        }

        public void Reverse()
        {
            this.nodes.Reverse();
        }

    }

    public class Node
    {
        public double X;
        public double Y;
        public Node Above;
        public Node Below;
        private Node pair;

        public Node Pair
        {
            get { return pair; }
            set
            {
                if (value == null)
                {
                    if (pair != null)
                    {
                        pair.pair = null;
                        pair = null;
                    }
                }
                else
                {
                    if (value.pair != null)
                    {
                        value.pair.pair = null;
                    }
                    this.pair = value;
                    value.pair = this;
                }
            }
        }

        public bool Visited;

        public Node(double x, double y)
        {
            X = x; Y = y; Visited = false;
            Below = null; pair = null;
        }

    }



}