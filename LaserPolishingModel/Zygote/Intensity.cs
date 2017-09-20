using LaserPolishingModel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModel.Zygote
{
    class Intensity
    {
        public Intensity(int x, int y, int height, int width, int buckets, int range)
        {
            Origin = new Coordinate<int>(x, y);
            Height = height;
            Width = width;
            Buckets = buckets;
            Range = range;
        }

        public Coordinate<int> Origin
        {
            get; set;
        }

        public int Height
        {
            get; set;
        }

        public int Width
        {
            get; set;
        }

        public int Buckets
        {
            get; set;
        }

        public int Range
        {
            get; set;
        }
    }
}
