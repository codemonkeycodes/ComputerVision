using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace AdvGraphics
{
    public enum ColorPlane
    {
        eRed,
        eBlue,
        eGreen,
        eAll
    };

    public class RGBImage
    {
        private AdvColor[,] pixels;

        public RGBImage( String image, ColorPlane plane )
        {
            //load the pixels directly from the image
            Bitmap img = new Bitmap(image);

            pixels = new AdvColor[img.Height, img.Width];

            //walk image and load pixels
            for (int i = 0; i < img.Height; i++)
                for (int j = 0; j < img.Width; j++)
                {
                    AdvColor clr = new AdvColor(img.GetPixel(j, i), plane);
                    pixels[i, j] = clr;// new AdvColor(img.GetPixel(j, i), plane);
                }
        }

        public RGBImage(AdvColor[,] in_pixels)
        {
            pixels = new AdvColor[in_pixels.GetLength(0), in_pixels.GetLength(1)];

            Array.Copy(in_pixels, 0, pixels, 0, in_pixels.Length); 
        }

        public T ClampValue<T>(T val, T min, T max ) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
                return min;
            else if (val.CompareTo(max) > 0)
                return max;

            return val;
        }


        public double Mean( ColorPlane plane )
        {
            int H = pixels.GetLength(0);
            int W = pixels.GetLength(1);

            double mean = 0.0;

            for( int i = 0; i < H; i++ )
                for( int j = 0; j < W; j++ )
                    switch(plane)
                    {
                        case ColorPlane.eBlue:
                            mean += pixels[i, j].B;
                            break;
                        case ColorPlane.eRed:
                            mean += pixels[i, j].R;
                            break;
                        case ColorPlane.eGreen:
                            mean += pixels[i, j].G;
                            break;
                    }

            return (mean/pixels.Length);
        }

        //covert 2D array representing which pixels are "on" 1D byte array of BGR data
        public byte[] ConvertArrayToPixel( AdvColor[,] _pixels )
        {
            byte[] single_array = new byte[_pixels.GetLength(1) * _pixels.GetLength(0) * 4];
            int ii = 0;
            for (int i = 0; i < _pixels.GetLength(0); i++)
            {
                for (int j = 0; j < _pixels.GetLength(1); j++)
                {
                    //line will be white
                    //circle will be cyan
                    //BGRA not RGBA or even ARGB
                    single_array[ii++] = (byte)_pixels[i, j].B;
                    single_array[ii++] = (byte)_pixels[i, j].G;
                    single_array[ii++] = (byte)_pixels[i, j].R;
                    single_array[ii++] = (byte)_pixels[i, j].A;
                }
            }

            return single_array;
        }

        public void SaveToImage(AdvColor[,] _pixels, string file_name)
        {
            Bitmap new_bmp = new Bitmap(_pixels.GetLength(1), _pixels.GetLength(0), PixelFormat.Format32bppArgb);

            BitmapData bmpData = new_bmp.LockBits(new Rectangle(0, 0, _pixels.GetLength(0), _pixels.GetLength(1)),
                                ImageLockMode.WriteOnly,
                                new_bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            byte[] bytes = ConvertArrayToPixel(_pixels);

            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            new_bmp.UnlockBits(bmpData);

            new_bmp.Save(file_name);
        }

        public double Variance( double mean, ColorPlane plane )
        {
            int H = pixels.GetLength(0);
            int W = pixels.GetLength(1);
            double var = 0.0;

            for (int i = 0; i < H; i++)
                for (int j = 0; j < W; j++)
                    switch (plane)
                    {
                        case ColorPlane.eBlue:
                            var += Math.Pow(pixels[i, j].B-mean,2);
                            break;
                        case ColorPlane.eRed:
                            var += Math.Pow(pixels[i, j].R - mean, 2);
                            break;
                        case ColorPlane.eGreen:
                            var += Math.Pow(pixels[i, j].G - mean, 2);
                            break;
                    }

            return ( var / pixels.Length );
        }

        public double StdDev( ColorPlane plane )
        {
            double var = Variance(Mean(plane), plane);
            return Math.Sqrt(var);
        }

        public int Max( ColorPlane plane )
        {
            int max = 0;

            int H = pixels.GetLength(0);
            int W = pixels.GetLength(1);

            for (int i = 0; i < H; i++)
                for (int j = 0; j < W; j++)
                    switch (plane)
                    {
                        case ColorPlane.eBlue:
                            if (max < pixels[i, j].B)
                                max = pixels[i, j].B;
                            break;
                        case ColorPlane.eRed:
                            if (max < pixels[i, j].R)
                                max = pixels[i, j].R;
                            break;
                        case ColorPlane.eGreen:
                            if (max < pixels[i, j].G)
                                max = pixels[i, j].G;
                            break;
                    }

            return max;
        }

        public int Min(ColorPlane plane)
        {
            int min = 255;

            int H = pixels.GetLength(0);
            int W = pixels.GetLength(1);

            for (int i = 0; i < H; i++)
                for (int j = 0; j < W; j++)
                    switch (plane)
                    {
                        case ColorPlane.eBlue:
                            if (min > pixels[i, j].B)
                                min = pixels[i, j].B;
                            break;
                        case ColorPlane.eRed:
                            if (min > pixels[i, j].R)
                                min = pixels[i, j].R;
                            break;
                        case ColorPlane.eGreen:
                            if (min > pixels[i, j].G)
                                min = pixels[i, j].G;
                            break;
                    }

            return min;
        }

        public double CalcOtsuThreshold()
        {
            //get histogram - normalize it
            Histogram histogram = BuildHistogram();
            SortedDictionary<AdvColor, int> bins = histogram.GetHistogram();

            int threshold, optimal_thresh;  // k = the current threshold; kStar = optimal threshold
            int N1 = 0, N = 0;    // N1 = # points with intensity <=k; N = total number of points
            double BCV, BCVmax; // The current Between Class Variance and maximum BCV
            double num, denom;  // temporary bookeeping
            int Sk;  // The total intensity for all histogram points <=k
            int S = 0; // The total intensity of the image

            bool first = true;
            for (threshold = 0; threshold < 256; threshold++)
            {
                AdvColor color = new AdvColor(threshold, threshold, threshold, ColorPlane.eAll);
                if (bins.ContainsKey(color))
                {
                    if( first )
                    {
                        N1 = bins[color];
                        first = false;
                    }
                    S += threshold * bins[color];	// Total histogram intensity
                    N += bins[color];		// Total number of data points
                }
            }

            Sk = 0;
            BCVmax = 0;
            optimal_thresh = 0;

            // Look at each possible threshold value,
            // calculate the between-class variance, and decide if it's a max
            for (threshold = 1; threshold < 255; threshold++)
            {
                AdvColor color = new AdvColor(threshold, threshold, threshold, ColorPlane.eAll);
                if (bins.ContainsKey(color))
                {
                    Sk += threshold * bins[color];
                    N1 += bins[color];

                    // The float casting here is to avoid compiler warning about loss of precision and
                    // will prevent overflow in the case of large saturated images
                    denom = (double)(N1) * (N - N1); // Maximum value of denom is (N^2)/4 =  approx. 3E10

                    if (denom != 0.0)
                    {
                        // Float here is to avoid loss of precision when dividing
                        num = ((double)N1 / N) * S - Sk; 	// Maximum value of num =  255*N = approx 8E7
                        BCV = (num * num) / denom;
                    }
                    else
                        BCV = 0;

                    if (BCV >= BCVmax)
                    { // Assign the best threshold found so far
                        BCVmax = BCV;
                        optimal_thresh = threshold;
                    }
                }
            }

            return optimal_thresh;
        }

        public AdvColor[,] Binarization(double value, ColorPlane plane)
        {
            AdvColor[,] new_pixels = new AdvColor[pixels.GetLength(0), pixels.GetLength(1)];

            for( int i = 0; i < pixels.GetLength(0); i++ )
                for( int j = 0; j < pixels.GetLength(1); j++ )
                {
                    int val = pixels[i,j].GetColor(plane);
                    if( val > value )
                        val = 255;
                    else
                        val = 0;

                    new_pixels[i, j] = new AdvColor(val, val, val, plane);
                }

            return new_pixels;
        }

        public AdvColor[,] Binzarization(double value, ColorPlane plane, AdvColor[,] source )
        {
            AdvColor[,] new_pixels = new AdvColor[pixels.GetLength(0), pixels.GetLength(1)];

            for (int i = 0; i < source.GetLength(0); i++)
                for (int j = 0; j < source.GetLength(1); j++)
                {
                    int val = source[i, j].GetColor(plane);
                    if (val < value )
                        val = 0;
                    else
                        val = pixels[i, j].GetColor(plane);

                    new_pixels[i, j] = new AdvColor(val, val, val, plane);
                }

            return new_pixels;
        }

        public Color[,] Modify(double value)
        {
            Color[,] new_pixels = new Color[pixels.GetLength(0),pixels.GetLength(1)];

            for( int i = 0; i < pixels.GetLength(0); i++ )
                for (int j = 0; j < pixels.GetLength(1); j++)
                {
                    //assuming uniform R/G/B values
                    int val = pixels[i, j].G;
                    if (val + value > 255)
                        val = 255;
                    else if (val + value < 0)
                        val = 0;
                    else
                        val += (int)value;

                    new_pixels[i, j] = Color.FromArgb(255, val, val, val);
                }

            return new_pixels;
        }

        public Histogram BuildHistogram()
        {
            return new Histogram(pixels, ColorPlane.eGreen);
        }

        public int GetHeight()
        {
            return pixels.GetLength(0);
        }

        public int GetWidth()
        {
            return pixels.GetLength(1);
        }

        public AdvColor[,] HistogramStretch( AdvColor low, AdvColor high, Histogram histogram, ColorPlane plane )
        {
            int height = GetHeight();
            int width = GetWidth();
            AdvColor[,] new_pixels = new AdvColor[height,width];

            int low_color = low.GetColor(plane);
            int high_color = high.GetColor(plane);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int val = Convert.ToInt32(( pixels[i,j].GetColor(plane) - low_color) * ( 255.0 / (high_color - low_color)));
                    if (val < 0)
                        val = 0;
                    if (val > 255)
                        val = 255;
                    new_pixels[i, j] = new AdvColor(val, val, val, plane);
                }
            }

            return new_pixels;
        }

        public AdvColor[,] HistogramStretch(double low_percentage, double high_percentance, ColorPlane plane)
        {
            Histogram histogram = BuildHistogram();

            //low bins to 0 out
            int low_bins = histogram.GetNumBinsWithItems(Convert.ToInt32(GetHeight() * GetWidth() * low_percentage));
            AdvColor low_color = histogram.GetBinColor(low_bins);

            int high_bins = histogram.GetNumBinsWithItems(Convert.ToInt32(GetHeight() * GetWidth() * high_percentance));

            AdvColor high_color = histogram.GetBinColor(high_bins);

            return HistogramStretch(low_color, high_color, histogram, ColorPlane.eGreen);
        }

        //try 1 - cheese since it was just a 3x3 kernel meant 1 pixel corner
        //try 2 - mirror and then mirror an edge. - not quite right
        //try 3 - group solution from class - row major to column major for mirroring

        public AdvColor[,] MirrorByKernel(int kernel_size_r, int kernel_size_c, AdvColor[,] new_img)
        {
            int additions_r = (kernel_size_r / 2);
            int additions_c = (kernel_size_c / 2);

            //allocate the larger image
            AdvColor[,] new_pixels = new AdvColor[new_img.GetLength(0) + (additions_r * 2), new_img.GetLength(1) + (additions_c * 2)];

            //copy the bulk of the data first
            for (int i = 0; i < new_img.GetLength(0); i++)
            {
                for (int j = 0; j < new_img.GetLength(1); j++)
                {
                    int i_prime = i + additions_r;
                    int j_prime = j + additions_c;

                    new_pixels[i_prime, j_prime] = new_img[i, j];
                }
            }

            //right side
            for (int i = additions_r; i < new_pixels.GetLength(0) - additions_r; i++)
            {
                for (int j = 0; j < additions_c; j++)
                {
                    int j_prime = new_pixels.GetLength(1) - additions_c + j;
                    int j_sub_prime = new_pixels.GetLength(1) - additions_c - j - 1;
                    new_pixels[i, j_prime] = new AdvColor(new_pixels[i, j_sub_prime], ColorPlane.eAll);
                }
            }

            //left side
            for (int i = 0; i < pixels.GetLength(0); i++)
            {
                int i_prime = additions_r + i;
                for (int j = 0; j < additions_c; j++)
                {
                    int j_prime = additions_c - j - 1;
                    new_pixels[i_prime, j_prime] = new AdvColor(new_img[i, j], ColorPlane.eAll);
                }
            }

            //top
            for (int i = 0; i < additions_r; i++)
            {
                for (int j = 0; j < new_img.GetLength(1); j++)
                {
                    new_pixels[additions_r - i - 1, j + additions_c] = new AdvColor(new_img[i, j], ColorPlane.eGreen);
                }
            }

            //bottom
            for (int i = 0; i < additions_r ; i++)
            {
                for (int j = 0; j < new_img.GetLength(1); j++)
                {
                    int i_new = new_pixels.GetLength(0) - additions_r + i;
                    int i_old = new_img.GetLength(0) - i - 1;

                    new_pixels[i_new, j + additions_c] = new AdvColor(new_img[i_old, j], ColorPlane.eGreen);
                }
            }

            int half_width = additions_c;
            int half_height = additions_r;

            //top left corner
            if (additions_r == 1 && additions_c == 1)
                new_pixels[0, 0] = new AdvColor(new_img[0, 0], ColorPlane.eAll);
            else if( additions_r > 1 && additions_c > 1 )
            {
                for( int i = 0, oj = half_width-1; i < additions_r; ++i, --oj )
                {
                    for( int j = 0, oi = half_height-1; j < half_width; ++j, --oi )
                    {
                        new_pixels[i, j] = new AdvColor(pixels[oi, oj], ColorPlane.eGreen);
                    }
                }

                /*for (int i = 0; i < additions_r; i++)
                {
                    for (int j = 0; j < additions_c; j++)
                    {
                        new_pixels[additions_r - i - 1, additions_c - j - 1] = new AdvColor(new_pixels[i + additions_r, additions_c - j - 1], ColorPlane.eGreen);
                    }
                }*/

            }

            //top right corner
            if (additions_r == 1 && additions_c == 1)
                new_pixels[0, new_img.GetLength(1) + (additions_c * 2) - 1] = new AdvColor(new_img[0, pixels.GetLength(1) - 1], ColorPlane.eAll);
            else if( additions_r > 1 && additions_c > 1 )
            {
                //move UP original pixel - apply across new pixels
                int orig_start_point = pixels.GetLength(1) - additions_c;
                for (int i = 0, oj = new_pixels.GetLength(1) - additions_c; i < additions_c; ++i, oj++)
                {
                    for (int j = orig_start_point, oi = 0; j < pixels.GetLength(1); j++, oi++)
                    {
                        new_pixels[oi, oj] = new AdvColor(pixels[i,j], ColorPlane.eGreen);
                    }
                }
                /*
                for (int i = 0; i < additions_r; i++)
                {
                    for (int j = 0; j < additions_c; j++)
                    {
                        int i_prime = additions_r - i - 1;
                        int j_prime = new_pixels.GetLength(1) - additions_c + j;
                        int j_sub_prime = new_img.GetLength(1) + additions_c + j;
                        new_pixels[i_prime, j_prime] = new AdvColor(new_pixels[i + additions_r, j_sub_prime ], ColorPlane.eGreen);
                    }
                }*/
            }

            //bottom left corner
            if (additions_r == 1 && additions_c == 1)
                new_pixels[new_pixels.GetLength(0) - 1, 0] = new AdvColor(new_img[new_img.GetLength(0) - 1, 0], ColorPlane.eAll);
            else
            {
                //start bottom left pixel from source image, move right to number of added columns then up until achieved added rows
                //start at #added columns, new_pixels.GetLength(0) - half_height, move down the rows and back across the columns until achieve added columns
                for (int i = pixels.GetLength(0) - 1, oj = additions_c - 1; i > pixels.GetLength(0) - (additions_r+1); --i, --oj)
                {
                    for (int j = 0, oi = new_pixels.GetLength(0)-additions_r; j < additions_c; j++, oi++)
                    {
                        new_pixels[oi,oj] = new AdvColor(pixels[i,j], ColorPlane.eGreen);
                    }
                }

                /*
                for (int i = 0; i < additions_r; i++)
                {
                    for (int j = 0; j < additions_c; j++)
                    {
                        int i_new = new_pixels.GetLength(0) - additions_r + i;
                        int i_old = new_pixels.GetLength(0) - additions_r - i - 1;
                        new_pixels[i_new, additions_c - j - 1] = new AdvColor(new_pixels[i_old, additions_c - j - 1], ColorPlane.eGreen);
                    }
                }*/
            }

            //bottom right corner
            if (additions_r == 1 && additions_c == 1)
                new_pixels[new_pixels.GetLength(0) - 1, new_pixels.GetLength(1) - 1] = new AdvColor(new_img[new_img.GetLength(0) - 1, new_img.GetLength(1) - 1], ColorPlane.eAll);
            else
            {
                //for initial pixels start at pixels.getlengt(0) - additions_r, pixels.getlength(1)-additions_r
                //for new pixels start at new_pixels.getlength(0) - 1, new_pixels.getlenght(1)-1
                //initial pixels move across additions_c...then down until through additions_r
                //new pixels move UP the rows and then - 1 column
                for (int i = pixels.GetLength(0) - additions_r, oj = new_pixels.GetLength(1) - 1; i < pixels.GetLength(0); i++, --oj )
                {
                    for (int j = pixels.GetLength(1) - additions_r, oi = new_pixels.GetLength(0) - 1; j < pixels.GetLength(1); j++, oi--)
                    {
                        new_pixels[oi,oj] = new AdvColor(pixels[i,j], ColorPlane.eGreen);
                    }
                }

                /*for (int i = 0; i < additions_r; i++)
                {
                    for (int j = 0; j < additions_c; j++)
                    {
                        int i_new = new_pixels.GetLength(0) - additions_r + i;
                        int i_old = new_pixels.GetLength(0) - additions_r - i - 1;
                        int j_new = new_pixels.GetLength(1) - additions_c + j;
                        new_pixels[i_new, j_new] = new AdvColor(new_pixels[i_old, j_new], ColorPlane.eGreen);
                    }
                }*/
            }

            //make sure nothing was missed
            for( int i = 0; i < new_pixels.GetLength(0); i++)
                for( int j = 0; j < new_pixels.GetLength(1); j++)
                    if (new_pixels[i, j] == null)
                    {
                        int blah = 0;
                        blah++;
                    }

            return new_pixels;
        }

        public List<AdvColor> GetNeighborhood(AdvColor[,] new_pixels, int i, int j, int kernel_size)
        {
            int num_items = kernel_size * kernel_size;

            List<AdvColor> neighborhood = new List<AdvColor>();
            for (int rows = - (kernel_size/2); rows < kernel_size - (kernel_size/2); rows++)
            {
                for (int columns = -(kernel_size / 2); columns < kernel_size - (kernel_size / 2); columns++)
                    neighborhood.Add(new_pixels[rows+i, columns+j]);
            }

            return neighborhood;
        }

        public AdvColor[,] GetNeighborhood( AdvColor[,] new_pixels, int i, int j, int kernel_r, int kernel_c )
        {
            AdvColor[,] neighborhood = new AdvColor[kernel_r,kernel_c];
            for (int rows = -(kernel_r / 2), ii = 0; rows < kernel_r - (kernel_r / 2); rows++, ii++ )
            {
                for (int columns = -(kernel_c / 2), jj = 0; columns < kernel_c - (kernel_c / 2); columns++, jj++)
                {
                    neighborhood[ii, jj] = new AdvColor(new_pixels[rows + i, columns + j], ColorPlane.eGreen);
                }
            }

            return neighborhood;
        }

        //new_pixels is the enlarged photo - use for neighborhood, but write to smaller buffer of original size
        public AdvColor[,] MedianFilter(AdvColor[,] new_pixels, int kernel)
        {
            AdvColor[,] new_image = new AdvColor[pixels.GetLength(0), pixels.GetLength(1)];

            int additions = kernel / 2;

            for (int i = 0 + additions; i < pixels.GetLength(0) + additions; i++)
            {
                for (int j = 0 + additions; j < pixels.GetLength(1) + additions; j++)
                {
                    List<AdvColor> neighborhood = GetNeighborhood(new_pixels, i, j, kernel);
                    neighborhood.Sort();
                    new_image[i - additions, j - additions] = new AdvColor(neighborhood[neighborhood.Count/2], ColorPlane.eAll);
                }
            }

            return new_image;

        }

        //new_pixels is the enlarged photo - use for neighborhood, but write to smaller buffer of original size
        public AdvColor[,] NoiseReductionThreshold( AdvColor[,] new_pixels, int kernel, int threshold )
        {
            AdvColor[,] new_image = new AdvColor[pixels.GetLength(0), pixels.GetLength(1)];

            int additions = kernel / 2;

            for (int i = 0 + additions; i < pixels.GetLength(0) + additions; i++)
            {
                for (int j = 0 + additions; j < pixels.GetLength(1) + additions; j++)
                {
                    List<AdvColor> neighborhood = GetNeighborhood(new_pixels, i, j, kernel);
                    int sum = 0;
                    foreach (AdvColor color in neighborhood)
                        sum += color.GetColor(ColorPlane.eGreen);

                    sum /= neighborhood.Count;
                    AdvColor original_pixel = new AdvColor( pixels[i - additions, j - additions], ColorPlane.eGreen );

                    if( Math.Abs(original_pixel.GetColor(ColorPlane.eGreen) - sum) > threshold )
                        original_pixel = new AdvColor( sum, sum, sum, ColorPlane.eGreen );

                    new_image[i - additions, j - additions] = original_pixel;
                }
            }

            return new_image;
        }

        public double [,] CalcGaussianKernel( int rows, int columns, double sigma )
        {
            double[,] kernel_values = new double[rows, columns];

            double sum_total = 0;

            //calc kernel values
            for (int row = (-rows) / 2; row <= (rows) / 2; row++)
                for (int column = (-columns) / 2; column <= (columns) / 2; column++ )
                {
                    double exponent = Math.Pow(row, 2) + Math.Pow(column, 2);
                    exponent = (exponent / (2*Math.Pow(sigma,2)));

                    double base_num = 1 / ( 2 * Math.PI * Math.Pow(sigma,2));

                    double val = base_num*Math.Exp(-exponent);

                    int actual_row = row + (rows/2);
                    int actual_col = column + (columns / 2);
                    kernel_values[actual_row, actual_col] = val;
                    sum_total += val;
                }

            //normalize
            for (int row = 0; row < rows; row++)
                for (int col = 0; col < columns; col++)
                    kernel_values[row, col] = kernel_values[row, col] * (1.0 / sum_total);

          return kernel_values;

        }

        public AdvColor[,] ApplyKernel( AdvColor[,] img, double[,] kernel )
        {
            int kernel_r = kernel.GetLength(0);
            int kernel_c = kernel.GetLength(1);

            int image_r = img.GetLength(0);
            int image_c = img.GetLength(1);

            AdvColor[,] new_img = new AdvColor[image_r - (kernel_r)+1, image_c-(kernel_c)+1];

            for( int i = kernel_r/2; i < (image_r - kernel_r/2); i++ )
            {
                for( int j = kernel_c/2; j < image_c - kernel_c/2; j++ )
                {
                    AdvColor[,] neighborhood = GetNeighborhood(img, i, j, kernel_r, kernel_c);

                    double val = 0;
                    for (int ii = 0; ii < kernel_r; ii++)
                    {
                        for (int jj = 0; jj < kernel_c; jj++)
                            val += neighborhood[ii, jj].GetColor(ColorPlane.eGreen) * kernel[ii, jj];
                    }

                    int i_prime = i - (kernel_r/2);
                    int j_prime = j - (kernel_c / 2);

                    new_img[i_prime,j_prime] = new AdvColor((int)val, (int)val, (int)val, ColorPlane.eGreen);
                }
            }

            return new_img;

        }

        public AdvColor[,] GaussianBlur2D( int kernel_r, int kernel_c, double sigma )
        {
            double[,] kernel = CalcGaussianKernel(kernel_r, kernel_c, sigma);

            AdvColor[,] big_img = MirrorByKernel(kernel_r, kernel_c, pixels);

            AdvColor[,] modified_img = ApplyKernel(big_img, kernel);

            return modified_img;

        }

        public AdvColor[,] GaussianBlur1D(int kernel_r, int kernel_c, double sigma, AdvColor[,] img_pixels)
        {
            double[,] kernel = CalcGaussianKernel(kernel_r, kernel_c, sigma);

            AdvColor[,] big_img = MirrorByKernel(kernel_r, kernel_c, (img_pixels == null ? pixels : img_pixels));

            AdvColor[,] modified_img = ApplyKernel(big_img, kernel);

            return modified_img;

        }

        public void CalcSobel( String mag_image, String dir_image )
        {
            double[,] kernel1 = new double[,] {{-1, -2, -1}, {0, 0, 0}, {1, 2, 1}};
            double[,] kernel2 = new double[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };

            AdvColor[,] img = MirrorByKernel(3, 3, pixels);

            AdvColor[,] img1 = ApplyKernel(img, kernel1);
            AdvColor[,] img2 = ApplyKernel(img, kernel2);

            AdvColor[,] mag_img = new AdvColor[img1.GetLength(0), img1.GetLength(1)];
            AdvColor[,] dir_img = new AdvColor[img2.GetLength(0), img2.GetLength(1)];

            for (int i = 0; i < img1.GetLength(0); i++)
            {
                for (int j = 0; j < img1.GetLength(1); j++)
                {
                    double mag = Math.Sqrt((Math.Pow(img1[i, j].GetColor(ColorPlane.eGreen), 2) + Math.Pow(img2[i, j].GetColor(ColorPlane.eGreen), 2)));
                    double dir = Math.Atan2(img1[i, j].GetColor(ColorPlane.eGreen), img2[i, j].GetColor(ColorPlane.eGreen));

                    double mag_prime = mag;
                    double dir_prime = dir;
                    
                    mag_prime = ((mag * 255)/Math.Sqrt(Math.Pow(1024,2) + Math.Pow(1024,2)));
                    dir_prime = ((dir + Math.PI)/(2 * Math.PI)) * 255;

                    mag_prime = ClampValue(mag_prime, 0.0, 255.0);
                    dir_prime = ClampValue(dir_prime, 0.0, 255.0);

                    mag_img[i, j] = new AdvColor((int)mag_prime, (int)mag_prime, (int)mag_prime, ColorPlane.eGreen);
                    dir_img[i, j] = new AdvColor((int)dir_prime, (int)dir_prime, (int)dir_prime, ColorPlane.eGreen);
                }
            }

            SaveToImage(mag_img, mag_image);
            SaveToImage(dir_img, dir_image);

        }

        private bool IsValueOnRight( BoundaryMove pt, int val )
        {
            BoundaryMove pt2 = pt.MoveRight();
            return (pt2.X >= 0 && pt2.Y >= 0 && pixels[pt2.X, pt2.Y].G == val );
        }

        public List<BoundaryMove> WalkShape( int start_i, int start_j, int shape_val )
        {
            //found the first pixel running face first into it scanning by line.  Backup one pixel and set the direction to north so the pixel is on our right
            BoundaryMove pt = new BoundaryMove( start_i, --start_j, Direction.eNorth );

            List<BoundaryMove> points = new List<BoundaryMove>();
            points.Add(pt);

            BoundaryMove pt2 = pt.MoveForward();
            while( true )
            {
                if (Equals(pt2, pt))
                    break;

                points.Add(pt2);

                while( IsValueOnRight(pt2, shape_val) )
                {
                    BoundaryMove pt3 = pt2.MoveForward();
                    if (pixels[pt3.X, pt3.Y].G == shape_val)
                        break;
                    
                    pt2 = pt3;
                    points.Add(pt2);

                    if (Equals(pt2, pt))
                        break;
                }

                if (Equals(pt2, pt))
                    break;
                
                pt2 = pt2.Move(shape_val, pixels);
            }

            return points;
        }

        public List<BoundaryMove> GetBoundaryPixels( int shape_val )
        {
            //find the first pixel value matching shape_val
            int start_i = 0;
            int start_j = 0;
            bool found = false;
            for (start_i = 0; start_i < pixels.GetLength(0); start_i++)
            {
                for (start_j = 0; start_j < pixels.GetLength(1); start_j++)
                {
                    if (pixels[start_i, start_j].G == shape_val)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                    break;
            }

            //walk the shape
            return WalkShape(start_i, start_j, shape_val);
        }

        public List<int> FindShapes()
        {
            List<int> shape_values = new List<int>();

            for( int i = 0; i < pixels.GetLength(0); i++ )
                for( int j = 0; j < pixels.GetLength(1); j++ )
                {
                    int val = pixels[i, j].G;
                    if( val != 0 && !shape_values.Contains(val) )
                        shape_values.Add(val);
                }

            return shape_values;
        }

        public void OutlineHull(List<BoundaryMove> hull, AdvColor[,] new_pixels)
        {
            BresenhamLine line = new BresenhamLine(pixels.GetLength(1), pixels.GetLength(0));
            for( int i = 0; i < hull.Count; i++ )
            {
                BoundaryMove p1 = hull[i];
                BoundaryMove p2;
                if (i == hull.Count - 1)
                    p2 = hull[0];
                else
                    p2 = hull[i + 1];

                line.DrawBresenhamLine( new_pixels, p1.X, p1.Y, p2.X, p2.Y, Color.White );
                
            }
        }

        public void OutlineShape(List<BoundaryMove> shape, AdvColor[,] new_pixels)
        {
            foreach( BoundaryMove pixel in shape )
            {
                new_pixels[pixel.X,pixel.Y] = new AdvColor(255,255,255, ColorPlane.eGreen);
            }
        }

        public double CalcMoment( int p, int q, int shape_val )
        {
            double val = 0;
            for (int i = 0; i < pixels.GetLength(0); i++)
            {
                for (int j = 0; j < pixels.GetLength(1); j++)
                {
                    val += Math.Pow(i, p) * Math.Pow(j, q) * (pixels[i, j].G == shape_val ? 1 : 0);
                }
            }

            return val;
        }

        public List<double> CalcMoments( int shape_val )
        {
            List<double> moments = new List<double>();

            //mu00, mu10, mu01, mu11, mu20, mu02

            //m10, m00, m01,
            //xBar = m10/m00
            //yBar = m01/m00

            //calc m00
            double m00 = 0;

            m00 = CalcMoment(0, 0, shape_val);
            double m10 = CalcMoment(1, 0, shape_val);
            double m01 = CalcMoment(0, 1, shape_val);
            double m11 = CalcMoment(1, 1, shape_val);
            double m20 = CalcMoment(2, 0, shape_val);
            double m02 = CalcMoment(0, 2, shape_val);

            double xBar = (m10/m00);
            double yBar = (m01/m00);

            //mu00 = m00
            moments.Add(m00);
            moments.Add(0);
            moments.Add(0);
            moments.Add(m11 - (m00*xBar*yBar));
            moments.Add(m20 - (m10 * xBar));
            moments.Add(m02 - (m01 * yBar));

            return moments;
        }

        public void WriteToTextFile()
        {
            StreamWriter writer = new StreamWriter("debug.txt");
            for( int i = 0; i < pixels.GetLength(0); i++ )
            {
                String output = "";
                for (int j = 0; j < pixels.GetLength(1); j++)
                {
                    output += pixels[i, j].G + " \t";
                }
                writer.WriteLine(output);
            }

            writer.Flush();
            writer.Close();
        }
    }
}
