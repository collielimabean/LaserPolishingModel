using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LaserPolishingModel.Util.MathNetExtensions;
using static LaserPolishingModel.Util.IEnumerableExtensions;

namespace LaserPolishingModel.Util
{
    public static class Ripples
    {
#warning Magic numbers and questionable variable names - refactor
        public static void GenerateRipples(Vector<double> xVec, Vector<double> yVec, double afs, double d, double spot, double step)
        {
            double r = d / 2;
            double r2 = spot / 2;
            double r1 = r - r2;
            double gridset = spot / 20; // why 20?

            // spot size //
            double Lx = 350;
            double Ly = 260;


            //  //
            double x0 = 0;
            double y0 = 0;
            double z0 = -r1 * afs;
            double z3 = -z0;

            var vec_builder = Vector<double>.Build;
            var v = vec_builder.DenseOfEnumerable(Range(-r, r, gridset));
            var mesh = meshgrid(v, v);
            var x = mesh.Item1;
            var y = mesh.Item2;

            var xSquared = x.PointwisePower(2);
            var ySquared = y.PointwisePower(2);

            // z = (afs * ((x.^ 2 + y.^ 2).^ 0.5) + z0).* (x.^ 2 + y.^ 2 < r1 ^ 2) + (-afs * ((x.^ 2 + y.^ 2).^ 0.5) + z3).* (x.^ 2 + y.^ 2 >= r1 ^ 2);
            var less_threshold = (xSquared + ySquared).Pointwise((cell, arg1) => cell < arg1 ? 1 : 0, r1 * r1);
            var geq_threshold = (xSquared + ySquared).Pointwise((cell, arg1) => cell >= arg1 ? 1 : 0, r1 * r1);

            var z = (afs * ((xSquared + ySquared).PointwiseSqrt()) + z0).PointwiseMultiply(less_threshold)
                 + (-afs * ((xSquared + ySquared).PointwiseSqrt()) + z3).PointwiseMultiply(geq_threshold);

            var roit = (x + y).Pointwise((cell, arg1) => cell < arg1 ? 1 : 0, Math.Pow((r + gridset * 0.5), 2));
            var z_temp = z.LogicalExtract(roit);

            int Nx = (int) Math.Floor((5 * r - d) / spot + 1); // No.of spots
            int Ny = (int) Math.Floor((5 * r - d) / step + 1); // No.of steps

            int NX = Nx - 1; // number of moves
            int NY = NX / 2;

            // meshgrid //
            var x_gridset = Vector<double>.Build.DenseOfEnumerable(Range(-r, 4 * r, gridset));
            var y_gridset = x_gridset.Clone();

            var result = meshgrid(x_gridset, y_gridset);
            var X = result.Item1;
            var Y = result.Item2;

            double X0 = 0;
            double Y0 = 0;
            var Roit = ((X - X0).PointwisePower(2) + (Y - Y0).PointwisePower(2))
                .Pointwise((double cell, double arg1) => { return cell < arg1 ? 1 : 0; }, Math.Pow((r + gridset * 0.5), 2));


            for (int i = 0; i < NY; i++)
            {

            }
        }
    }
}
