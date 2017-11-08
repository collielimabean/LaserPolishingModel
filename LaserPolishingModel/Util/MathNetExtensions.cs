using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModel.Util
{
    public static class MathNetExtensions
    {
        public static Matrix<T> flipud<T>(this Matrix<T> mat) where T : struct, IEquatable<T>, IFormattable
        {
            var mat_copy = mat.Clone();

            for (int i = 0; i < mat_copy.ColumnCount; i++)
            {
                // flip column //
                var col = mat_copy.Column(i);

                for (int j = 0; j < col.Count / 2; j++)
                {
                    var temp = col[j];
                    col[j] = col[col.Count - j - 1];
                    col[col.Count - j - 1] = temp;
                }

                mat_copy.SetColumn(i, col);
            }

            return mat_copy;
        }

        public static Vector<T> circshift<T>(this Vector<T> vec, int pos) where T : struct, IEquatable<T>, IFormattable
        {
            // negative index is shifting left instead of right
            // which should be equivalent to length - pos
            if (pos < 0)
                pos = vec.Count - pos;

            if (pos == 0)
                return vec;

            // if shift is larger than length
            // just bring it down into the range [0, vec.Count)
            if (pos >= vec.Count)
                pos %= vec.Count;

            T[] raw = vec.AsArray();

            Array.Reverse(raw);
            Array.Reverse(raw, 0, pos);
            Array.Reverse(raw, pos, raw.Length - pos);
            return vec;
        }

        public static Matrix<T> circshift<T>(this Matrix<T> mat, int pos) where T : struct, IEquatable<T>, IFormattable
        {
            return circshift(mat, new Tuple<int, int>(1, 0));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mat"></param>
        /// <param name="pos">First item is how many rows to shift downwards. Second is how many columns to shift right.</param>
        /// <returns></returns>
        public static Matrix<T> circshift<T>(this Matrix<T> mat, Tuple<int, int> pos) where T : struct, IEquatable<T>, IFormattable
        {
            var mat_clone = mat.Clone();
            int col_shift = pos.Item1; // if we want to shift rows, in reality, we're circular shifting the columns downwards
            int row_shift = pos.Item2; // likewise, shifting columns is circular shifting rows

            for (int i = 0; i < mat.ColumnCount; i++)
                mat_clone.SetColumn(i, mat_clone.Column(i).circshift(col_shift));

            for (int i = 0; i < mat.RowCount; i++)
                mat_clone.SetRow(i, mat_clone.Row(i).circshift(row_shift));

            return mat_clone;
        }

        // needs proper test cases
        public static Matrix<T> fftshift<T>(this Matrix<T> mat) where T : struct, IEquatable<T>, IFormattable
        {
            return mat.circshift(new Tuple<int, int>(mat.RowCount / 2, mat.ColumnCount / 2));
        }
        
        public static Matrix<T> ifftshift<T>(this Matrix<T> mat) where T : struct, IEquatable<T>, IFormattable
        {
            int x_shift = (int) Math.Ceiling(mat.RowCount / 2.0);
            int y_shift = (int) Math.Ceiling(mat.ColumnCount / 2.0);

            return mat.circshift(new Tuple<int, int>(x_shift, y_shift));
        }
    }
}
