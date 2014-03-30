using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AdvGraphics
{
    public class Histogram
    {
        private SortedDictionary<AdvColor, int> histogram;

        public Histogram(AdvColor[,] pixels, ColorPlane plane)
        {
            histogram = new SortedDictionary<AdvColor, int>();

            for (int i = 0; i < pixels.GetLength(0); i++)
                for (int j = 0; j < pixels.GetLength(1); j++)
                {
                    AdvColor val = new AdvColor(pixels[i, j], plane);
                    if (histogram.ContainsKey(val))
                        histogram[val] = histogram[val] + 1;
                    else
                        histogram[val] = 1;
                }
        }

        public int GetNumBins()
        {
            return histogram.Keys.Count();
        }

        public SortedDictionary<AdvColor, int> GetHistogram()
        {
            return histogram;
        }

        public AdvColor GetBinColor( int bin )
        {
            int i = 0;
            foreach (KeyValuePair<AdvColor, int> data in histogram)
            {
                if (i == bin)
                    return data.Key;

                i++;
            }

            return null;
        }

        public int GetBinCount(int bin)
        {
            int i = 0;
            foreach( KeyValuePair<AdvColor, int> data in histogram )
            {
                if (i == bin)
                    return data.Value;

                i++;
            }

            return 0;
        }

        public int GetBinContent(AdvColor bin)
        {
            return histogram[bin];
        }

        public int GetNumBinsWithItems(int count)
        {
            int bins = 0;
            int total = 0;
            int key = 0;
            while (total < count )
            {
                if (key > 255)
                    return -1;

                AdvColor color = new AdvColor(Color.FromArgb(255, key, key, key), ColorPlane.eGreen);

                if (histogram.ContainsKey(color))
                {
                    total += histogram[color];
                    if (total >= count)
                        return bins;

                    bins++;
                }

                key++;
            }

            return bins;
        }
    }
}
