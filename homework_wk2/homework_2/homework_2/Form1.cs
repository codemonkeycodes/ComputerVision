using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AdvGraphics;

namespace homework_2
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
                    + Environment.NewLine + "Max: " + max.ToString();
            }
        }

        private void btnOutputImage_Click(object sender, EventArgs e)
        {
            //binarization 

            AdvColor[,] pixels = img.Binarization(mean, ColorPlane.eGreen);
            img.SaveToImage(pixels, "binarization.png");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Histogram histogram = img.BuildHistogram();

            //low bins to 0 out
            int low_bins = histogram.GetNumBinsWithItems(Convert.ToInt32(img.GetHeight()*img.GetWidth()*.10));
            AdvColor low_color = histogram.GetBinColor(low_bins);

            int high_bins = histogram.GetNumBinsWithItems(Convert.ToInt32(img.GetHeight() * img.GetWidth() * .90));

            AdvColor high_color = histogram.GetBinColor(high_bins);

            AdvColor[,] pixels = img.HistogramStretch( low_color, high_color, histogram, ColorPlane.eGreen );

            SortedDictionary<AdvColor, int> bins = histogram.GetHistogram();
            foreach (KeyValuePair<AdvColor, int> bin in bins)
                chart1.Series["Series1"].Points.AddXY(bin.Key.GetColor(ColorPlane.eGreen), bin.Value);

            RGBImage img2 = new RGBImage(pixels);

            img.SaveToImage( pixels, "histo_stretch.png");

            Histogram histogram2 = img2.BuildHistogram();

            SortedDictionary<AdvColor, int> bins2 = histogram2.GetHistogram();
            foreach (KeyValuePair<AdvColor, int> bin in bins2)
                chart2.Series["Series1"].Points.AddXY(bin.Key.GetColor(ColorPlane.eGreen), bin.Value);


        }

        private void button2_Click(object sender, EventArgs e)
        {
            double otsu_threshold = img.CalcOtsuThreshold();

            AdvColor[,] pixels = img.Binarization(otsu_threshold, ColorPlane.eGreen);
            img.SaveToImage(pixels, "otsu-binarization.png");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AdvColor[,] new_pixels = img.MirrorKernel(3);

            new_pixels = img.MedianFilter(new_pixels, 3);

            img.SaveToImage(new_pixels, "3x3_mirrored_median_img.png");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AdvColor[,] new_pixels = img.MirrorKernel(3);

            new_pixels = img.NoiseReductionThreshold(new_pixels, 3, 50);

            img.SaveToImage(new_pixels, "3x3_mirrored_50_img.png");
        }
    }
}
