using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AdvGraphics;


namespace week5
{
    public partial class Form1 : Form
    {
        private RGBImage img;
        private double mean = 255;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = new RGBImage(openFileDialog1.FileName, ColorPlane.eGreen);

                List<int> shapes = img.FindShapes();

                List<List<BoundaryMove>> shape_boundaries = new List<List<BoundaryMove>>();
                List<List<double>> moments = new List<List<double>>();
                List<List<BoundaryMove>> hull_bounadaries = new List<List<BoundaryMove>>();

                ConvexHull hull = new ConvexHull();
                for( int i = 0; i < shapes.Count; i++ )
                {
                    List<BoundaryMove> boundary = img.GetBoundaryPixels(shapes[i]);
                    shape_boundaries.Add( boundary );
                    moments.Add(img.CalcMoments(shapes[i]));
                    hull_bounadaries.Add(hull.CalcQuickHull(boundary));
                }


                for( int i = 0; i < shapes.Count; i++ )
                {
                    String filename = String.Format("boundary{0}.txt", i + 1);
                    StreamWriter writer = new StreamWriter(filename);

                    List<double> boundary_moments = moments[i];
                    for (int ii = 0; ii < boundary_moments.Count; ii++)
                    {
                        writer.Write(boundary_moments[ii]);
                        if( ii < boundary_moments.Count )
                            writer.Write(", ");
                    }

                    writer.Write(System.Environment.NewLine);

                    List<BoundaryMove> convexHull = hull_bounadaries[i];

                    for (int ii = 0; ii < convexHull.Count; ii++)
                    {
                        writer.WriteLine("" + convexHull[ii].X + ", " + convexHull[ii].Y);
                    }

                    List<BoundaryMove> boundary = shape_boundaries[i];
                    for (int ii = 0; ii < boundary.Count; ii++)
                    {
                        if( ii == 0 )
                            writer.Write( boundary[ii].X + ", " + boundary[ii].Y );
                        
                        writer.Write(boundary[ii].GetDirectionString() + " " );
                    }

                    writer.Flush();
                    writer.Close();
                }


               AdvColor[,] black_img = new AdvColor[256,256];
                for (int i = 0; i < black_img.GetLength(0); i++)
                    for (int j = 0; j < black_img.GetLength(1); j++ )
                        black_img[i,j] = new AdvColor(0,0,0, ColorPlane.eGreen);

                /*foreach (List<BoundaryMove> shape in shape_boundaries)
                {
                    img.OutlineShape(shape, black_img);
                }*/

                foreach (List<BoundaryMove> _hull in hull_bounadaries)
                {
                    img.OutlineHull(_hull, black_img);
                }

                img.SaveToImage(black_img, "TestConvexHullOutput.png");
            }
        }
    }
}
