using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AdvGraphics
{
    public class Point3D
    {
        private Point pt;
        private int z;

        public Point3D(int _x, int _y, int _z)
        {
            pt = new Point(_x, _y);
            z = _z;
        }

        public Point GetPoint()
        {
            return pt;
        }

        public int GetZ()
        {
            return z;
        }
    }
}
