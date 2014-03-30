using System;
using System.Drawing;

namespace AdvGraphics
{
    //class for YPbPr colors
    //can convert from RGB to YPbPr & from YPbPr to RGB
    public class ColorYPbPr
    {
        public double Y;
        public double Pb;
        public double Pr;

        private double[,] to_ypbpr = new double[,] { { .299, .587, .114 }, { -.168, -.331, .500 }, { .500, -.418, -.081 } };
        private double[,] to_rgb = new double[,] { { 1.00, 0.0, 1.402 }, { 1.00, -.344, -.714 }, { 1.00, 1.772, 0.0 } };

        public ColorYPbPr()
        {
            Y = Pb = Pr = 0;
        }

        public ColorYPbPr( double y, double pb, double pr )
        {
           Y = y;
           Pb = pb;
           Pr = pr;
        }

        //take RGB & convert to YPbPr array
        private double[] ConvertFromRGB( int r, int g, int b )
        {
            double [] ypbpr = new double[3];

            ypbpr[0] = ((to_ypbpr[0, 0] * r) + (to_ypbpr[0, 1] * g) + (to_ypbpr[0, 2] * b));
            ypbpr[1] = ((to_ypbpr[1, 0] * r) + (to_ypbpr[1, 1] * g) + (to_ypbpr[1, 2] * b));
            ypbpr[2] = ((to_ypbpr[2, 0] * r) + (to_ypbpr[2, 1] * g) + (to_ypbpr[2, 2] * b));

            return ypbpr;
        }

        //take C# Color class and convert to YPbPr
        public ColorYPbPr( Color color )
        {
            double [] ypbpr = ConvertFromRGB(color.R, color.G, color.B);
            Y = ypbpr[0];
            Pb = ypbpr[1];
            Pr = ypbpr[2];
        }

        //cap the values
        public int CapValue(int val)
        {
            if (val > 255)
                return 255;

            if (val < 0)
                return 0;

            return val;
        }

        //convert YPbPr to RGB
        public Color ToRGB()
        {
            int r = 0, g = 0, b = 0;

            r = Convert.ToInt32((to_rgb[0, 0] * Y) + (to_rgb[0, 1] * Pb) + (to_rgb[0, 2] * Pr));
            g = Convert.ToInt32((to_rgb[1, 0] * Y) + (to_rgb[1, 1] * Pb) + (to_rgb[1, 2] * Pb));
            b = Convert.ToInt32((to_rgb[2, 0] * Y) + (to_rgb[2, 1] * Pb) + (to_rgb[2, 2] * Pr));

            r = CapValue(r);
            g = CapValue(g);
            b = CapValue(b);


            Color clr = Color.FromArgb(255, r, g, b);

            return clr;
        }
    }
}
