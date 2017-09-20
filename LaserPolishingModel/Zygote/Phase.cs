using LaserPolishingModel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModel.Zygote
{
    class Phase
    {
        public Phase(int x, int y, int width, int height)
        {
            Origin = new Coordinate<int>(x, y);
            Width = width;
            Height = height;
        }

        public Coordinate<int> Origin
        {
            get; set;
        }

        public int Width
        {
            get; set;
        }

        public int Height
        {
            get; set;
        }
    }
}
