using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AdvGraphics
{
    public class Dither : ColorManipulation
    {
        private Color[,] gray_pixels;
        private const int threshhold = 126;

        private int max_val;
        private int min_val;

        public Dither(int h, int w, Color[,] pixels )
        {
            max_val = -999;
            min_val = 999;
            H = h;
            W = w;
            gray_pixels = new Color[H,W];
            Array.Copy(pixels, gray_pixels, H*W);
        }

        //clamp the values between 0 and 255
        private int ClampValue(double val)
        {
            if (val < 0)
                val = 0.0;
            if (val > 255)
                val = 255;

            return Convert.ToInt32(val);
        }

        //spread the error to surrounding pixels
        private void DitherSurrounding( int h, int w, int pixel, int error )
        {
            int i = h, j = w;
            double factor = 0;
            switch( pixel )
            {
                case 0:
                    j++;
                    factor = ((7*error)/16.0);
                    break;
                case 1:
                    i++;
                    j--;
                    factor = ((3 * error) / 16.0);
                    break;
                case 2:
                    i++;
                    factor = ((5 * error) / 16.0);
                    break;
                case 3:
                    i++;
                    j++;
                    factor = ((error) / 16.0);
                    break;
                default:
                    return;
            }

            if( i < 0 || i >= H )
                return;
            if( j < 0 || j >= W)
                return;

            Color orig = gray_pixels[i, j];
            int r = ClampValue(factor + orig.R);

            
            Color clr = Color.FromArgb(255, r, r, r);
            gray_pixels[i, j] = clr;

        }

        //calculate the pixel color to display, then calculate surrounding pixels
        private void DoDithering( int h, int w  )
        {
            int val = gray_pixels[h, w].R;  //only need 1 color since they should all be the same as this is a gray scale image
            int display_color = 0;
            int error = 0;

            if (val < threshhold)
                display_color = 0;
            else
                display_color = 255;

            if (val > max_val)
                max_val = val;

            if (val < min_val)
                min_val = val;

            error = val - display_color;

            gray_pixels[h, w] = Color.FromArgb(255, display_color, display_color, display_color);

            for (int i = 0; i < 4; i++)
                DitherSurrounding(h, w, i, error);
        }

        //walk pixels & save image after dithering entire image
        public void DitherImage( string dithered_name )
        {
            for (int i = 0; i < H; i++)
                for (int j = 0; j < W; j++)
                    DoDithering( i, j );

            SaveToImage(gray_pixels, dithered_name);

        }

    }
}
