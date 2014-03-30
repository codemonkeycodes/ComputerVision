using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AdvGraphics
{
    //class to fade RGB to grayscale
    public class ColorManipulationRGB : ColorManipulation
    {
        private const int MAX_STEPS = 1000;
        private Color[,] pixel_colors;

        public ColorManipulationRGB( int width, int height )
        {
            pixel_colors = new Color[height,width];
            H = height;
            W = width;
        }

        public void SetRGBColor( int hpos, int wpos, Color clr )
        {
            pixel_colors[hpos, wpos] = clr;
        }

        private bool FadeRGB( ref Color[,] gray_pixels )
        {
            bool changed = false;
            for( int i = 0; i < H; i++ )
            {
                for(int j = 0; j < W; j++ )
                {
                    Color original = pixel_colors[i, j];
                    Color shifted = gray_pixels[i, j];
                    int gray = Convert.ToInt32((original.R * .299) + (original.G * .587) + (original.B * .114));

                    int r = shifted.R, g = shifted.G, b = shifted.B;

                    if (gray < shifted.R)
                    {
                        changed = true;
                        r--;
                    }
                    else if (gray > shifted.R)
                    {
                        changed = true;
                        r++;
                    }

                    if (gray < shifted.G)
                    {
                        changed = true;
                        g--;
                    }
                    else if (gray > shifted.G)
                    {
                        changed = true;
                        g++;
                    }
                    if (gray < shifted.B)
                    {
                        changed = true;
                        b--;
                    }
                    else if (gray > shifted.B)
                    {
                        changed = true;
                        b++;
                    }

                    if (changed)
                    {
                        Color new_clr = Color.FromArgb(255, r, g, b);
                        gray_pixels[i, j] = new_clr;
                    }
                }
            }

            return changed;
        }

        private void FadeRGB(string file_postfix)
        {
            Color[,] gray_pixels = new Color[H,W];
            Array.Copy(pixel_colors, gray_pixels, H*W);

            bool bChanged = false;
            for (int i = 0; i < MAX_STEPS; i++)
            {
                bChanged = FadeRGB(ref gray_pixels);
                if (!bChanged)
                    break;

                string file_name = String.Format("{0,4:D4}", i) + file_postfix;

                SaveToImage(gray_pixels, file_name);
            }
        }

        public void Fade( string prefix )
        {
            FadeRGB(prefix);
        }
    }
}
