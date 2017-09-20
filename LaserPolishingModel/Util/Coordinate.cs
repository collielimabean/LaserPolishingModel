using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModel.Util
{
    struct Coordinate<T>
    {
        T X;
        T Y;

        public Coordinate(T x, T y)
        {
            X = x;
            Y = y;
        }
    }
}
