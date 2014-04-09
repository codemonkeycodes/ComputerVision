using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AdvGraphics
{
    public enum Direction
    {
        eNorth,
        eEast,
        eSouth,
        eWest,
        eStartPoint
    }

    public class BoundaryMove
    {
        private Point pixel;
        private Direction dir;

        public int X
        {
            get { return pixel.X; }
        }

        public int Y
        {
            get { return pixel.Y; }
        }

        public Direction Dir
        {
            get { return dir; }
        }

        public String GetDirectionString()
        {
            switch( Dir )
            {
                case Direction.eNorth:
                    return "N";
                case Direction.eSouth:
                    return "S";
                case Direction.eEast:
                    return "E";
                case Direction.eWest:
                    return "W";
            }

            return "";
        }

        public BoundaryMove( int x, int y )
        {
            pixel = new Point(x, y);
            dir = Direction.eStartPoint;
        }

        public BoundaryMove( int x, int y, Direction d )
        {
            pixel = new Point(x, y);
            dir = d;
        }

        private bool IsValueOnRight(BoundaryMove pt, int val, AdvColor[,] pixels )
        {
            BoundaryMove pt2 = pt.MoveRight();
            return (pt2.X >= 0 && pt2.Y >= 0 && pixels[pt2.X, pt2.Y].G == val);
        }

        public BoundaryMove Move( int val, AdvColor[,] pixels )
        {

            //figure out the direction to move - left or right never back or forward - forward is handled in calling function

            BoundaryMove pt2 = MoveRight();
            if (IsValueOnRight(pt2, val, pixels) && pixels[pt2.X, pt2.Y].G != val )
                return pt2;

            pt2 = MoveLeft();
            return pt2;
        }

        public Point GetPointOnRight()
        {
            switch (dir)
            {
                case Direction.eStartPoint:
                case Direction.eNorth:
                    {
                        return new Point(X, Y+1);
                    }
                case Direction.eSouth:
                    {
                        return new Point(X, Y-1);
                    }
                case Direction.eEast:
                    {
                        return new Point(X+1, Y);
                    }
                case Direction.eWest:
                    {
                        return new Point(X-1, Y);
                    }
            }

            return new Point(-1, -1);
        }

        public BoundaryMove MoveForward()
        {
            //depending on the direction we are coming from - forward is relative
            switch( dir )
            {
                case Direction.eStartPoint:
                case Direction.eNorth:
                    {
                        return new BoundaryMove(X-1, Y, Direction.eNorth);   
                    }
                case Direction.eSouth:
                    {
                        return new BoundaryMove(X+1, Y, Direction.eSouth);
                    }
                case Direction.eEast:
                    {
                        return new BoundaryMove(X, Y+1, Direction.eEast);
                    }
                case Direction.eWest:
                    {
                        return new BoundaryMove(X, Y-1, Direction.eWest);
                    }
            }

            return new BoundaryMove(-1, -1);
        }

        public BoundaryMove MoveRight()
        {
            //depending on the direction we are coming from - right is relative
            switch (dir)
            {
                case Direction.eStartPoint:
                case Direction.eNorth:
                    {
                        return new BoundaryMove(X, Y+1,Direction.eEast);
                    }
                case Direction.eSouth:
                    {
                        return new BoundaryMove(X, Y-1,Direction.eWest);
                    }
                case Direction.eEast:
                    {
                        return new BoundaryMove(X+1, Y,Direction.eSouth);
                    }
                case Direction.eWest:
                    {
                        return new BoundaryMove(X-1, Y,Direction.eNorth);
                    }
            }

            return new BoundaryMove(-1, -1);
        }

        public BoundaryMove MoveLeft()
        {
            //depending on the direction we are coming from - left is relative
            switch (dir)
            {
                case Direction.eStartPoint:
                case Direction.eNorth:
                    {
                        return new BoundaryMove(X, Y-1, Direction.eWest);
                    }
                case Direction.eSouth:
                    {
                        return new BoundaryMove(X, Y+1, Direction.eEast);
                    }
                case Direction.eEast:
                    {
                        return new BoundaryMove(X-1, Y, Direction.eNorth);
                    }
                case Direction.eWest:
                    {
                        return new BoundaryMove(X+1, Y, Direction.eSouth);
                    }
            }

            return new BoundaryMove(-1, -1);
        }

        public BoundaryMove MoveBack()
        {
            //depending on the direction we are coming from - left is relative
            switch (dir)
            {
                case Direction.eNorth:
                    {
                        return new BoundaryMove(X+1, Y, Direction.eSouth);
                    }
                case Direction.eStartPoint:
                case Direction.eSouth:
                    {
                        return new BoundaryMove(X-1, Y, Direction.eNorth);
                    }
                case Direction.eEast:
                    {
                        return new BoundaryMove(X, Y-1, Direction.eWest);
                    }
                case Direction.eWest:
                    {
                        return new BoundaryMove(X, Y+1, Direction.eEast);
                    }
            }

            return new BoundaryMove(-1, -1);
        }

        //return if this veritical & horizontal component are equal to "obj"
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
                return false;

            BoundaryMove rhs = (BoundaryMove)obj;

            // Return true if the fields match:
            return (pixel.X == rhs.X) && (pixel.Y == rhs.Y);
        }

    }
}
