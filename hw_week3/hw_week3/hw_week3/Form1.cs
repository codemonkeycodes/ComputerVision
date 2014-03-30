using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AdvGraphics;

namespace hw_week3
{
    public partial class Form1 : Form
    {
        private RGBImage img;
        private double mean = 255;
        private double std_dev = 0.0;
        private int max = 0;
        private int min = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = new RGBImage(openFileDialog1.FileName, ColorPlane.eGreen);
                mean = img.Mean(ColorPlane.eGreen);
                max = img.Max(ColorPlane.eGreen);
                min = img.Min(ColorPlane.eGreen);
                std_dev = img.StdDev(ColorPlane.eGreen);

                textBox1.Text = "Mean: " + mean.ToString() + Environment.NewLine + "Std. Dev.: " + std_dev.ToString() + Environment.NewLine + "Min: " + min.ToString()
                    + Environment.NewLine + "Max: " + max.ToString() + Environment.NewLine;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdvColor[,] new_pixels = img.GaussianBlur2D(15, 15, 1.6);

            img.SaveToImage( new_pixels, "2DGaussianBlur.png");

            RGBImage img_2D = new RGBImage("2DGaussianBlur.png", ColorPlane.eGreen);

            double mean = img_2D.Mean(ColorPlane.eGreen);

            textBox1.Text += "2D Gaussian Image Mean: " + mean + System.Environment.NewLine;
        }

        private void btnOutputImage_Click(object sender, EventArgs e)
        {
            AdvColor[,] new_pixels = img.GaussianBlur1D(1, 15, 1.6, null);
            AdvColor[,] newer_pixels = img.GaussianBlur1D(15, 1, 1.6, new_pixels);

            img.SaveToImage(newer_pixels, "1DGaussianBlur.png");

            RGBImage img_1D = new RGBImage("1DGaussianBlur.png", ColorPlane.eGreen);
            double mean = img_1D.Mean(ColorPlane.eGreen);

            textBox1.Text += "1D Gaussian Image Mean: " + mean + System.Environment.NewLine;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            img.CalcSobel("mag_image.png", "dir_image.png");

            RGBImage mag = new RGBImage("mag_image.png", ColorPlane.eGreen);
            RGBImage dir = new RGBImage("mag_image.png", ColorPlane.eGreen);

            double mean_mag = mag.Mean(ColorPlane.eGreen);
            double mean_dir = dir.Mean(ColorPlane.eGreen);

            textBox1.Text += "Mag Image Mean: " + mean_mag + System.Environment.NewLine;
            textBox1.Text += "Dir Image Mean: " + mean_dir + System.Environment.NewLine;

        }
    }
}
