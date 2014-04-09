using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//this code adapted from java code taken from here: http://www.ahristov.com/tutorial/geometry-games/convex-hull.html


namespace AdvGraphics
{
    public class ConvexHull
    {
        public ConvexHull()
        {
        }

        public int pointLocation(BoundaryMove A, BoundaryMove B, BoundaryMove P)
        {
            int cp1 = (B.X - A.X) * (P.Y - A.Y) - (B.Y - A.Y) * (P.X - A.X);
            return (cp1 > 0) ? 1 : -1;
        }

        public int distance(BoundaryMove A, BoundaryMove B, BoundaryMove C)
        {
            int ABx = B.X - A.X;
            int ABy = B.Y - A.Y;
            int num = ABx * (A.Y - C.Y) - ABy * (A.X - C.X  );
            if (num < 0) num = -num;
            return num;
        } 

        public List<BoundaryMove> CalcQuickHull( List<BoundaryMove> boundaries )
        {
            if (boundaries.Count < 3)
                return boundaries;

            int minPoint = -1, maxPoint = -1;
            int minX = Int32.MaxValue;
            int maxX = Int32.MinValue;

            List<BoundaryMove> convexHull = new List<BoundaryMove>();

            for (int i = 0; i < boundaries.Count; i++)
            {
                if (boundaries[i].X < minX)
                {
                    minX = boundaries[i].X;
                    minPoint = i;
                }

                if (boundaries[i].X > maxX)
                {
                    maxX = boundaries[i].X;
                    maxPoint = i;
                }
            }

            BoundaryMove A = boundaries[minPoint];
            BoundaryMove B = boundaries[maxPoint];
            
            convexHull.Add(A);
            convexHull.Add(B);
            
            boundaries.Remove(A);
            boundaries.Remove(B);

            List<BoundaryMove> leftSet = new List<BoundaryMove>();
            List<BoundaryMove> rightSet = new List<BoundaryMove>();

            for (int i = 0; i < boundaries.Count; i++)
            {
                BoundaryMove p = boundaries[i];
                if (pointLocation(A, B, p) == -1)
                    leftSet.Add(p);
                else
                    rightSet.Add(p);
            }

            hullSet(A, B, rightSet, convexHull);
            hullSet(B, A, leftSet, convexHull);

            return convexHull;
        }

        public void hullSet(BoundaryMove A, BoundaryMove B, List<BoundaryMove> set, List<BoundaryMove> hull) 
        {
            if (set.Count == 0)
                return;

            int insertPosition = hull.IndexOf(B);
            
            if (set.Count == 1)
            {
                BoundaryMove p = set[0];
                set.Remove(p);
                hull.Insert(insertPosition,p);
                return;
            }

            int dist = Int32.MinValue;
            int furthestPoint = -1;
            
            for (int i = 0; i < set.Count; i++) 
            {
                BoundaryMove p = set[i];
                int dist2 = distance(A,B,p);
                if (dist2 > dist)
                {
                    dist = dist2;
                    furthestPoint = i;
                }
            }

            BoundaryMove P = set[furthestPoint];
            set.RemoveAt(furthestPoint);
            hull.Insert(insertPosition,P);
    
            // Determine who's to the left of AP
            List<BoundaryMove> leftSetAP = new List<BoundaryMove>();
            for (int i = 0; i < set.Count; i++) 
            {
                BoundaryMove M = set[i];
                if (pointLocation(A,P,M)==1) 
                    leftSetAP.Add(M);
            }
    
            // Determine who's to the left of PB
            List<BoundaryMove> leftSetPB = new List<BoundaryMove>();
            for (int i = 0; i < set.Count; i++)
            {
                BoundaryMove M = set[i];
                if (pointLocation(P,B,M)==1) 
                    leftSetPB.Add(M);
            }

            hullSet(A,P,leftSetAP,hull);
            hullSet(P,B,leftSetPB,hull);
        }
    }
}
