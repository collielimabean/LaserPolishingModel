using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModel.Util
{
    public static class LeastSquaresMeanPlane
    {
        public struct LsmPlaneResult
        {
            /// <summary>
            /// "f" in the Matlab code
            /// </summary>
            public Matrix<double> LeastSquaresMeanPlane { get; set; }

            /// <summary>
            /// "eta" in the Matlab code
            /// </summary>
            public Matrix<double> AdjustedSurfacePlane { get; set; }
        }

        public static LsmPlaneResult lsmplane(Surface surface)
        {
            var x_bar = surface.XVector.Average();
            var y_bar = surface.YVector.Average();
            var z_bar = (from column in surface.ZData.EnumerateColumns() select column.Average()).Average();

            double sumB1, sumB2, sumC1, sumC2;
            sumB1 = sumB2 = sumC1 = sumC2 = 0;

            for (int i  = 0; i < surface.XVector.Count(); i++)
            {
                for (int j = 0; j < surface.YVector.Count(); j++)
                {
                    sumB1 += surface.XVector[i] * (surface.ZData[i, j] - z_bar);
                    sumB2 += surface.XVector[i] * (surface.XVector[i] - x_bar);

                    sumC1 += surface.YVector[j] * (surface.ZData[i, j] - z_bar);
                    sumC2 += surface.YVector[j] * (surface.YVector[j] - y_bar);
                }
            }

            var b = sumB1 / sumB2;
            var c = sumC1 / sumC2;
            var a = z_bar - (b * x_bar) - (c * y_bar);

            var f_enumerable = from index in Enumerable.Range(0, surface.XVector.Count())
                               select a + b * surface.XVector[index] + c * Vector<double>.Build.DenseOfArray(surface.YVector);


            var f = Matrix<double>.Build.DenseOfColumnVectors(f_enumerable);
            var eta = surface.ZData - f;

            return new LsmPlaneResult()
            {
                LeastSquaresMeanPlane = f,
                AdjustedSurfacePlane = eta
            };
        }
    }
}
