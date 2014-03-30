using System.Drawing;

namespace AdvGraphics
{
    class BresenhamCircle
    {
        private int maxH;
        private int maxW;

        public BresenhamCircle( int H, int W )
        {
            maxH = H;
            maxW = W;
        }

        //source: http://rosettacode.org/wiki/Bitmap/Midpoint_circle_algorithm#C.23 
        public void Draw(int centerH, int centerW, int radius, int[,] pixel_data, Color color )
        {
            int d = (5 - radius * 4) / 4;
            int x = 0;
            int y = radius;

            while (x <= y)
            {
                // ensure index is in range before setting (depends on your image implementation)
                // in this case we check if the pixel location is within the bounds of the image before setting the pixel
                if (centerW + x >= 0 && centerW + x <= maxW && centerH + y >= 0 && centerH + y <= maxH - 1) pixel_data[centerW + x, centerH + y] = color.ToArgb();
                if (centerW + x >= 0 && centerW + x <= maxW - 1 && centerH - y >= 0 && centerH - y <= maxH - 1) pixel_data[centerW + x, centerH - y] = color.ToArgb();
                if (centerW - x >= 0 && centerW - x <= maxW - 1 && centerH + y >= 0 && centerH + y <= maxH - 1) pixel_data[centerW - x, centerH + y] = color.ToArgb();
                if (centerW - x >= 0 && centerW - x <= maxW - 1 && centerH - y >= 0 && centerH - y <= maxH - 1) pixel_data[centerW - x, centerH - y] = color.ToArgb();
                if (centerW + y >= 0 && centerW + y <= maxW - 1 && centerH + x >= 0 && centerH + x <= maxH - 1) pixel_data[centerW + y, centerH + x] = color.ToArgb();
                if (centerW + y >= 0 && centerW + y <= maxW - 1 && centerH - x >= 0 && centerH - x <= maxH - 1) pixel_data[centerW + y, centerH - x] = color.ToArgb();
                if (centerW - y >= 0 && centerW - y <= maxW - 1 && centerH + x >= 0 && centerH + x <= maxH - 1) pixel_data[centerW - y, centerH + x] = color.ToArgb();
                if (centerW - y >= 0 && centerW - y <= maxW - 1 && centerH - x >= 0 && centerH - x <= maxH - 1) pixel_data[centerW - y, centerH - x] = color.ToArgb();
                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    d += 2 * (x - y) + 1;
                    y--;
                }
                x++;
            }
        }
    }
}
