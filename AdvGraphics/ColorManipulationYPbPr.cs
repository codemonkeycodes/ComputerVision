using System;
using System.Drawing;

namespace AdvGraphics
{
    //class holding YPbPr array that can fade colors to gray-scale
    public class ColorManipulationYPbPr : ColorManipulation 
    {
        private const int MAX_STEPS = 1000;
        private ColorYPbPr[,] pixel_colors;

        public ColorManipulationYPbPr(int h, int w)
        {
            H = h;
            W = w;
            pixel_colors = new ColorYPbPr[H,W];
        }

        public void SetYPbPrFromRGB(int h, int w, int r, int g, int b)
        {
            Color clr = Color.FromArgb(255, r, g, b);
            ColorYPbPr color = new ColorYPbPr(clr);

            pixel_colors[h, w] = color;
        }

        private bool FadeYPbPr( ref ColorYPbPr[,] gray_pixels )
        {
            //slowly adjust the Pb & Pr values towards 0.0
            //linear shift each value by .200 until it's 0 or it jumps to the opposite sign and then clamp it at 0
            bool changed = false;
            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    ColorYPbPr shifted = gray_pixels[i, j];

                    if (shifted.Pb < 0.0)
                    {
                        changed = true;
                        shifted.Pb += .200;
                        if (shifted.Pb > 0)
                            shifted.Pb = 0;
                    }
                    else if (shifted.Pb > 0.0)
                    {
                        changed = true;
                        shifted.Pb -= .200;
                        if (shifted.Pb < 0)
                            shifted.Pb = 0;
                    }

                    if (shifted.Pr < 0.0)
                    {
                        changed = true;
                        shifted.Pr += .200;
                        if (shifted.Pr > 0)
                            shifted.Pr = 0;
                    }
                    else if (shifted.Pr > 0.0)
                    {
                        changed = true;
                        shifted.Pr -= .200;
                        if (shifted.Pr < 0)
                            shifted.Pr = 0;
                    }
                }
            }

            return changed;
        }

        private void FadeYPbPr(string postfix)
        {
            ColorYPbPr[,] gray_pixels = new ColorYPbPr[H, W];
            Array.Copy(pixel_colors, gray_pixels, H * W);

            for (int i = 0; i < MAX_STEPS; i++)
            {
                bool bChanged = FadeYPbPr(ref gray_pixels);
                if (!bChanged)
                    break;

                string file_name = String.Format("{0,4:D4}", i) + postfix;

                Color[,] rgb_gray = new Color[H,W];
                for (int j = 0; j < H; j++)
                    for (int k = 0; k < W; k++)
                        rgb_gray[j, k] = gray_pixels[j, k].ToRGB();
                
                SaveToImage(rgb_gray, file_name);
            }
        }

        public void Fade(string postfix)
        {
            FadeYPbPr(postfix);
        }
    }
}
