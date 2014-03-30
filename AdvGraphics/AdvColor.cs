using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AdvGraphics
{
    public class AdvColor : IComparable
    {
        private int alpha;
        private int red;
        private int blue;
        private int green;
        private ColorPlane plane;

        public AdvColor( int _red, int _green, int _blue, ColorPlane _plane )
        {
            red = _red;
            blue = _blue;
            green = _green;
            alpha = 255;
            plane = _plane;
        }

        public AdvColor(AdvColor color, ColorPlane color_plane )
        {
            alpha = color.A;
            red = color.R;
            blue = color.B;
            green = color.G;
            plane = color_plane;
        }

        public AdvColor(Color color, ColorPlane color_plane)
        {
            red = color.R;
            alpha = color.A;
            blue = color.B;
            green = color.G;

            plane = color_plane;
        }

        public int B
        {
            get { return blue; }
        }

        public int R
        {
            get { return red; }
        }

        public int G
        {
            get { return green; }
        }

        public int A
        {
            get { return alpha; }
        }

        public int GetColor( ColorPlane plane )
        {
            switch (plane)
            {
                case ColorPlane.eBlue:
                    return B;
                case ColorPlane.eRed:
                    return R;
                case ColorPlane.eGreen:
                    return G;
                case ColorPlane.eAll:
                    return A;
            }

            return 0;
        }

        public int CompareTo(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 1;
            if (Object.ReferenceEquals(this, obj))
                return 0;
            AdvColor color = (AdvColor) obj;
            int obj_val;
            int in_val;
            switch( color.plane )
            {
                case ColorPlane.eBlue:
                    obj_val = color.blue;
                    in_val = blue;
                    break;
                case ColorPlane.eGreen:
                    obj_val = color.green;
                    in_val = green;
                    break;
                default:
                    obj_val = color.red;
                    in_val = red;
                    break;
            }

            if (obj_val > in_val)
                return -1;

            if (obj_val == in_val)
                return 0;

            if (obj_val < in_val)
                return 1;

            return 0;
        }

    }
}
