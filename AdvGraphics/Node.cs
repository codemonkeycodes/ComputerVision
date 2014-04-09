using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvGraphics
{
    public class Node
    {
        private int m_V;        //vertical component
        private int m_H;        //horizontal component
        private bool m_visited;

        public Node(int v, int h)
        {
            m_H = h;
            m_V = v;
            m_visited = false;
            Parent = null;
            Cost = 0;
            GScore = 0.0;
            HScore = 0.0;
        }

        public Node( Node rhs )
        {
            m_H = rhs.H;
            m_V = rhs.V;
            m_visited = rhs.Vistied;
            Cost = rhs.Cost;
            GScore = rhs.GScore;
            HScore = rhs.HScore;
            Parent = rhs.Parent;
        }

        //get only property for FScore
        public double FScore { get { return GScore + HScore; } }

        //get & set property for Parent node
        public Node Parent { get; set; }

        //get & set GScore
        public double GScore { get; set; }

        //get & set HScore
        public double HScore { get; set; }

        //get & set Cost
        public int Cost { get; set; }

        //get horizontal component
        public int H
        {
            get { return m_H; }
        }

        //get veritical component
        public int V
        {
            get { return m_V; }
        }

        //get & set visitied state
        public bool Vistied
        {
            get { return m_visited; }
            set { m_visited = value; }
        }

        //return if this veritical & horizontal component are equal to "obj"
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
                return false;

            Node rhs = (Node) obj;

            // Return true if the fields match:
            return (m_H == rhs.H) && (m_V == rhs.V);
        }

        //output to string [ V, H ]
        public override String ToString()
        {
            return "[" + m_V + "," + m_H + "]";
        }

        public override int  GetHashCode()
        {
            return (V ^ H);
        }

        //determine if node has already been visited 

        private bool AlreadyVisited(Node n, Node t)
        {

            if (Equals(n, t))
                return true;


            while (n.Parent != null)
            {

                if (Equals(t, n.Parent))
                    return true;

                n = n.Parent;
            }

            return false;
        }



        //get 1 8-way connected node

        public Node GetConnectedNode(int i, Node n, int size_v, int size_h)
        {

            //generate of of the 8 connected node 
            int H = n.H;
            int V = n.V;

            int next_H = 0;
            int next_V = 0;

            int cost = 10;

            //pick one of the 1 way nodes in order as defined
            switch (i)
            {

                //diagonal up left
                case 0:
                    next_H = --H;
                    next_V = --V;
                    cost = 14;
                    break;
                //up

                case 1:
                    next_H = H;
                    next_V = --V;
                    break;

                //diagonal up right

                case 2:
                    next_H = ++H;
                    next_V = --V;
                    cost = 14;
                    break;

                //left

                case 3:
                    next_H = --H;
                    next_V = V;
                    break;

                //right
                case 4:
                    next_H = ++H;
                    next_V = V;
                    break;

                //digonal down left

                case 5:
                    next_H = --H;
                    next_V = ++V;
                    cost = 14;
                    break;

                //down
                case 6:
                    next_H = H;
                    next_V = ++V;
                    break;

                //diagonal down right 
                case 7:
                    next_H = ++H;
                    next_V = ++V;
                    cost = 14;
                    break;

            }

            //if the node is off the graph extents in H return null

            if (next_H < 0 || next_H >= size_h)
                return null;



            //if the node is off the graph extents in V return null

            if (next_V < 0 || next_V >= size_v)
                return null;



            //build the node assign the cost

            Node t = new Node(next_V, next_H);
            t.GScore += t.Cost = cost;



            return AlreadyVisited(n, t) ? null : t;

        }
    }
}
