using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AdvGraphics
{
    public class GraphicDisplay
    {
        private int[,] pixel_data;
        private int W;
        private int H;
        private List<Point> points;

        private void Init( int _H, int _W )
        {
            H = _H;
            W = _W;
            pixel_data = new int[H, W];

            points = new List<Point>();

            for (int i = 0; i < H; i++)
                for (int j = 0; j < W; j++)
                    pixel_data[i, j] = -1;
        }

        public GraphicDisplay( int _H, int _W, Point p0, Point p1, Point p2, Point p3 )
        {
            Init(_H, _W);

            points.Add(p0);
            points.Add(p1);
            points.Add(p2);
            points.Add(p3);
        }

        public GraphicDisplay(int _H, int _W, Point p0, Point p1, Point p2 )
        {
            Init(_H, _W);

            points.Add(p0);
            points.Add(p1);
            points.Add(p2);
        }

        public void DrawPointsToImage()
        {
            DrawPoints();
        }

        private void FillArea( bool blend, Color color, PictureBox pictbox )
        {
            //find maxH, minH, maxW, minW
            int maxH = -1, minH = 99999999, maxW = -1, minW = 99999999;
            foreach( Point p in points )
            {
                if (p.X > maxW)
                    maxW = p.X;
                if (p.X < minW)
                    minW = p.X;

                if (p.Y > maxH)
                    maxH = p.Y;
                if (p.Y < minH)
                    minH = p.Y;
            }

            //walk to find the first point
            for( int i = minH; i < maxH+1; i++ )
            {
                for(int j = minW; j < maxW+1; j++)
                {
                    if( pixel_data[i,j] != -1 )
                    {
                        for( int jj = maxW; jj > j; jj--)
                        {
                            if( pixel_data[i,jj] != -1 )
                            {
                                if (blend)
                                {
                                    DrawBresenhamLine(i, j, i, jj, Color.FromArgb(pixel_data[i, j]),
                                                      Color.FromArgb(pixel_data[i, jj]), pictbox);
                                    break;   
                                }
                                else
                                {
                                    DrawBresenhamLine(i, j, i, jj, color,
                                                      color, pictbox);
                                    break;
                                }
                            }
                        }

                        break;
                    }
                    
                }
            }
        }

        public void DoFill(bool blend, Color color, PictureBox pictbox)
        {
            FillArea(blend, color, pictbox);
        }

        public void DrawLine(Color point1, Color point2, Color point3, PictureBox pictbox, bool blend )
        {
            DrawBresenhamLine(points[0].Y, points[0].X, points[1].Y, points[1].X, point1, point2, pictbox);
            DrawBresenhamLine(points[1].Y, points[1].X, points[2].Y, points[2].X, point2, point3, pictbox);
            DrawBresenhamLine(points[0].Y, points[0].X, points[2].Y, points[2].X, point1, point3, pictbox);

            FillArea(blend, point1, pictbox);
        }

        public void DrawLine(Color point1, Color point2, Color point3, PictureBox pictbox )
        {
            DrawBresenhamLine( points[0].Y, points[0].X, points[1].Y, points[1].X, point1, point2, pictbox );
            DrawBresenhamLine(points[1].Y, points[1].X, points[2].Y, points[2].X, point2, point3, pictbox);
            DrawBresenhamLine(points[0].Y, points[0].X, points[2].Y, points[2].X, point1, point3, pictbox);
        }

        private void DrawPoints()
        {
            //draw and X for each "point"
            foreach ( Point point in points )
            {
                pixel_data[point.Y, point.X] = Color.Yellow.ToArgb();
                pixel_data[point.Y - 1, point.X] = Color.Yellow.ToArgb();
                pixel_data[point.Y, point.X-1] = Color.Yellow.ToArgb();
                pixel_data[point.Y+1, point.X] = Color.Yellow.ToArgb();
                pixel_data[point.Y, point.X+1] = Color.Yellow.ToArgb();

                
                pixel_data[point.Y+1, point.X+1] = Color.Yellow.ToArgb();
                pixel_data[point.Y-1, point.X-1] = Color.Yellow.ToArgb();
                pixel_data[point.Y-1, point.X+1] = Color.Yellow.ToArgb();
                pixel_data[point.Y+1, point.X-1] = Color.Yellow.ToArgb();

                
                pixel_data[point.Y + 2, point.X + 2] = Color.Yellow.ToArgb();
                pixel_data[point.Y - 2, point.X - 2] = Color.Yellow.ToArgb();
                pixel_data[point.Y - 2, point.X + 2] = Color.Yellow.ToArgb();
                pixel_data[point.Y + 2, point.X - 2] = Color.Yellow.ToArgb();

                pixel_data[point.Y + 3, point.X + 3] = Color.Yellow.ToArgb();
                pixel_data[point.Y - 3, point.X - 3] = Color.Yellow.ToArgb();
                pixel_data[point.Y - 3, point.X + 3] = Color.Yellow.ToArgb();
                pixel_data[point.Y + 3, point.X - 3] = Color.Yellow.ToArgb();
            }
        }

        public void Clear()
        {
            pixel_data = new int[H,W];
        }

        //bresenhamline
        public void DrawBresenhamLine(int startH, int startW, int endH, int endW, Color color, Color end_color, PictureBox pictureBox )
        {
            int actual_startH = startH;
            int actual_startW = startW;

            int dx = Math.Abs(endW - startW);
            int dy = Math.Abs(endH - startH);

            int sx, sy;

            if (startW < endW) sx = 1; else sx = -1;
            if (startH < endH) sy = 1; else sy = -1;

            int err = dx - dy;

            Graphics g = null;
            if (pictureBox != null)
                g = pictureBox.CreateGraphics();

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

                Color drawn_color = Color.FromArgb(colorR, colorG, colorB);

                if (g != null)
                    g.DrawEllipse(new Pen(drawn_color), (float)startW, (float)startH, 1, 1);

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

        //bresenhamline
        public void DrawBresenhamLine( int startH, int startW, int endH, int endW, Color color, ref PictureBox pictureBox )
        {
            DrawBresenhamLine(startH, startW, endH, endW, color, color, pictureBox);
        }

        //save to bitmap
        public void OutputBitmap( byte[] byte_array, String file_name )
        {
            //create bitmap & copy byte array to bitmap data
            Bitmap bitmap = new Bitmap(W, H, PixelFormat.Format32bppRgb);

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, W, H),
                                    ImageLockMode.WriteOnly,
                                    bitmap.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            Marshal.Copy(byte_array, 0, ptr, byte_array.Length);
            bitmap.UnlockBits(bmpData);

            bitmap.Save(file_name);
        }

        //covert 2D array representing which pixels are "on" 1D byte array of BGR data
        public byte[] ConvertArrayToPixel()
        {
            byte [] single_array = new byte[H*W*4];
            int ii = 0;
            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    //line will be white
                    //circle will be cyan
                    //BGRA not RGBA or even ARGB
                    Color color;
                    if (pixel_data[i, j] == -1)
                    {
                        color = Color.Black;
                    }
                    else
                    {
                        color = Color.FromArgb(pixel_data[i, j]);
                    }

                    if (color != Color.Black)
                    {
                        //white
                        single_array[ii++] = color.B;
                        single_array[ii++] = color.G;
                        single_array[ii++] = color.R;
                        single_array[ii++] = 0;
                    }
                    else
                    {
                        //white
                        single_array[ii++] = 0;
                        single_array[ii++] = 0;
                        single_array[ii++] = 0;
                        single_array[ii++] = 0;
                    }
                }
            }

            return single_array;
        }

        public static GraphicDisplay operator +(GraphicDisplay g1, GraphicDisplay g2)
        {
            int max_h = (g1.H > g2.H ? g1.H : g2.H);
            int max_w = (g1.W > g2.W ? g1.W : g2.W);

            GraphicDisplay g_new = new GraphicDisplay(max_h, max_w, g1.points[0], g1.points[1], g1.points[2], g1.points[3]);

            foreach( Point point in g2.points )
                g_new.points.Add(point);

            for (int i = 0; i < max_h; i++)
            {
                for (int j = 0; j < max_w; j++)
                {
                    if (i < g1.H && j < g1.W)
                        g_new.pixel_data[i, j] = g1.pixel_data[i, j];

                    //2 overlaps 1...assume drawing in Z order from bottom to top
                    if (i < g2.H && j < g2.W)
                        g_new.pixel_data[i, j] = g1.pixel_data[i, j];
                }
            }

            return g_new;
        }
    }
}
