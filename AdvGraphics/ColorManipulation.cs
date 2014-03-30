using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AdvGraphics
{
    public class ColorManipulation
    {
        public int H;
        public int W;

        public ColorManipulation()
        {
            H = 0;
            W = 0;
        }

        //covert 2D array representing which pixels are "on" 1D byte array of BGR data
        public byte[] ConvertArrayToPixel(Color[,] pixels)
        {
            byte[] single_array = new byte[W * H * 4];
            int ii = 0;
            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    //line will be white
                    //circle will be cyan
                    //BGRA not RGBA or even ARGB
                    single_array[ii++] = pixels[i, j].B;
                    single_array[ii++] = pixels[i, j].G;
                    single_array[ii++] = pixels[i, j].R;
                    single_array[ii++] = pixels[i, j].A;

                }
            }

            return single_array;
        }

        public byte[] ConvertArrayToPixel( int[,] pixels )
        {
            byte[] single_array = new byte[W * H * 4];
            int ii = 0;
            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    Color bg = Color.Black;
                    Color clr = Color.FromArgb(pixels[i, j]);
                    if (clr.Name == "0")
                        clr = Color.Black;

                    if (clr.ToArgb() == Color.Black.ToArgb())
                    {
                        //line will be white
                        //circle will be cyan
                        //BGRA not RGBA or even ARGB
                        single_array[ii++] = clr.B;
                        single_array[ii++] = clr.G;
                        single_array[ii++] = clr.R;
                        single_array[ii++] = clr.A;
                    }
                    else
                    {
                        //assume background color is fully opaque
                        int A1 = clr.A;

                        Byte r = clr.R;
                        Byte g = clr.G;
                        Byte b = clr.B;

                        double percent = (A1 / 255.0);
                        if (percent < 1)
                        {
                            b = Convert.ToByte((clr.B * percent) + (bg.B * (1 - percent)));
                            g = Convert.ToByte((clr.G * percent) + (bg.G * (1 - percent)));
                            r = Convert.ToByte((clr.R * percent) + (bg.R * (1 - percent)));
                        }

                        single_array[ii++] = b;
                        single_array[ii++] = g;
                        single_array[ii++] = r;
                        single_array[ii++] = 255;
                    }
                }
            }

            return single_array;
        }

        public void SaveToImage( int[,] pixels, string file_name )
        {
            Bitmap new_bmp = new Bitmap(W, H, PixelFormat.Format32bppArgb);

            BitmapData bmpData = new_bmp.LockBits(new Rectangle(0, 0, W, H),
                                ImageLockMode.WriteOnly,
                                new_bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            byte[] bytes = ConvertArrayToPixel(pixels);

            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            new_bmp.UnlockBits(bmpData);

            new_bmp.Save(file_name);
        }

        //save image by taking array of colors, converting to byte array, copying it to the bmp data, then saving the image
        public void SaveToImage( Color[,] gray_pixels, string file_name )
        {
            Bitmap new_bmp = new Bitmap( W, H, PixelFormat.Format32bppArgb );

                BitmapData bmpData = new_bmp.LockBits(new Rectangle(0, 0, W, H),
                                    ImageLockMode.WriteOnly,
                                    new_bmp.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                byte [] bytes = ConvertArrayToPixel(gray_pixels);

                Marshal.Copy(bytes, 0, ptr, bytes.Length);
                new_bmp.UnlockBits(bmpData);

                new_bmp.Save(file_name);
        }
    }
}
