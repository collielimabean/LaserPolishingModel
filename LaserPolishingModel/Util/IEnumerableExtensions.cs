using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModel.Util
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<int> Range(int start, int end, int step = 1)
        {
            if (step == 0)
                throw new ArgumentException();

            IEnumerable<int> RangeIterator(int _start, int _stop, int _step)
            {
                int x = start;

                do
                {
                    yield return x;
                    x += _step;
                    if (_step < 0 && x <= _stop || 0 < _step && _stop <= x)
                        break;
                }
                while (true);
            }

            return RangeIterator(start, end, step);
        }

        public static IEnumerable<int> Range(int end)
        {
            return Range(0, end);
        }

        public static IEnumerable<double> Range(double start, double end, double step)
        {
            if (step == 0)
                throw new ArgumentException();

            IEnumerable<double> RangeIterator(double _start, double _stop, double _step)
            {
                double x = start;

                do
                {
                    yield return x;
                    x += _step;
                    if (_step < 0 && x <= _stop || 0 < _step && _stop <= x)
                        break;
                }
                while (true);
            }

            return RangeIterator(start, end, step);
        }

        public static IEnumerable<double> Range(double start, double end, int num_elements)
        {
            if (num_elements < 0)
                throw new ArgumentException();

            IEnumerable<double> RangeIterator(double _start, double _stop, int _num_elements)
            {
                double step = (_stop - _start) / _num_elements;

                for (int i = 0; i < _num_elements; i++)
                    yield return _start + (i * step);
            }

            return RangeIterator(start, end, num_elements);
        }
    }
}
