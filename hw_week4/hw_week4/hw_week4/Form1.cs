using System;
using System.Windows.Forms;
using AdvGraphics;

namespace hw_week4
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

                AdvColor[,] new_pixels = img.MedianFilter(img.MirrorByKernel(3,3,img.HistogramStretch(.1, .9, ColorPlane.eGreen)), 3);

                //RGBImage blurred = new RGBImage(img.GaussianBlur2D(15, 15, 1.5));
                RGBImage blurred = new RGBImage(img.GaussianBlur1D(1, 15, 1.5,img.GaussianBlur1D(15, 1, 1.5, new_pixels )));

                blurred.CalcSobel("sobel_mag.png", "sobel_dir.png");

                RGBImage mag_image = new RGBImage("sobel_mag.png", ColorPlane.eGreen);
                RGBImage dir_image = new RGBImage("sobel_dir.png", ColorPlane.eGreen);

                double threshold = mag_image.CalcOtsuThreshold();

                AdvColor[,] mag_image_prime = mag_image.Binarization(threshold, ColorPlane.eGreen);
                AdvColor[,] dir_image_prime = dir_image.Binzarization(255, ColorPlane.eGreen, mag_image_prime );

                mag_image.SaveToImage(mag_image_prime,"mag_image_prime.png");
                dir_image.SaveToImage(dir_image_prime, "dir_image_prime.png");

                RGBImage new_mag_image = new RGBImage(mag_image_prime);
                RGBImage new_dir_image = new RGBImage(dir_image_prime);

                mean = new_mag_image.Mean(ColorPlane.eGreen);

                textBox1.Text = "Mean: " + mean.ToString() + Environment.NewLine + "Mean - Dir: " + new_dir_image.Mean(ColorPlane.eGreen).ToString();
            }
        }
    }
}
