using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AdvGraphics
{
    class BresenhamLine
    {
        private int H;
        private int W;

        public BresenhamLine( int h, int w )
        {
            H = h;
            W = w;
        }

        public void DrawBresenhamLine( int[,] pixel_data, int startH, int startW, int endH, int endW, Color color )
        {
            DrawBresenhamLine(pixel_data, startH, startW, endH, endW, color, color);
        }

        public void DrawBresenhamLine( int[,] pixel_data, int startH, int startW, int endH, int endW, Color color, Color end_color )
        {
            int actual_startH = startH;
            int actual_startW = startW;

            int dx = Math.Abs(endW - startW);
            int dy = Math.Abs(endH - startH);

            int sx, sy;

            if (startW < endW) sx = 1; else sx = -1;
            if (startH < endH) sy = 1; else sy = -1;

            int err = dx - dy;

            while (true)
            {
                if (startH < 0 || startW < 0 || startH >= H || startW >= W)
                    break;

                double percent_startH = ((endH - actual_startH) - (startH - actual_startH)) / Convert.ToDouble(endH - actual_startH);
                double percent_startW = ((endW - actual_startW) - (startW - actual_startW)) / Convert.ToDouble(endW - actual_startW);

                double percent = 0.0;
                if ((endH - actual_startH) > (endW - actual_startW))
                    percent = percent_startH;
                else
                    percent = percent_startW;

                int colorR = Convert.ToInt32(((percent * color.R) + ((1 - percent) * end_color.R)));
                int colorG = Convert.ToInt32(((percent * color.G) + ((1 - percent) * end_color.G)));
                int colorB = Convert.ToInt32(((percent * color.B) + ((1 - percent) * end_color.B)));
                int colorA = Convert.ToInt32(((percent * color.A) + ((1 - percent) * end_color.A)));

                Color drawn_color = Color.FromArgb(colorA, colorR, colorG, colorB);

                pixel_data[startH, startW] = drawn_color.ToArgb();

                if (startW == endW && startH == endH)
                    break;

                int e2 = 2 * err;

                if (e2 > -dy)
                {
                    err = err - dy;
                    startW = startW + sx;
                }

                if (e2 < dx)
                {
                    err = err + dx;
                    startH = startH + sy;
                }
            }
        }
    }
}
