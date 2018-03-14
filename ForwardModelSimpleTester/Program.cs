using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathNet.Numerics.LinearAlgebra;

using LaserPolishingModel.ForwardModel;
using LaserPolishingModel.Parameter;
using LaserPolishingModel.Util;
using static LaserPolishingModel.Util.MathNetExtensions;
using LaserPolishingModel.Zygo;
using static LaserPolishingModel.ForwardModel.ForwardModel;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;


namespace ForwardModelSimpleTester
{
    class Program
    {
        // Linear curve fitting emperical model from 2009 Tyler Perry Paper
        // Curve fitting for pulse durations and melt time relationship from Tyler (SS316)
        static readonly double[] PULSE_DURATION = { 0, 50, 100, 200, 300, 400, 500, 750, 1000 };
        static readonly double[] MAX_MELT_TIME = { 0, 102, 215, 406, 653, 868, 1096, 1615, 2090 };
        static Tuple<double, double> TerryMeltTimePolyFit = Fit.Line(PULSE_DURATION, MAX_MELT_TIME);


        static void Main(string[] args)
        {
            var unpolishedSurface = new ZygoAsciiFile();
            unpolishedSurface.LoadFromFile(@"C:\Users\William\Downloads\Sensitivity Edit - Brodan\1-15-15_MilledH_1_20x.asc");

            var phaseData = Matrix<double>.Build.DenseOfArray(unpolishedSurface.PhaseData);

            var surface = new Surface(
                phaseData * 1e6,
                0,
                unpolishedSurface.CameraRes * 1000,
                0,
                unpolishedSurface.CameraRes * 1000
            );

            surface.Transpose();

            var material = new Material("Tool Steel")
            {
                SurfaceAbsorptivity = new Parameter("", 0.38),
                Density = new Parameter("", 7783),
                SurfaceTensionCoefficient = new Parameter("", -0.49e-3),
                DynamicViscosity = new Parameter("", 5.5e-3),
                ThermalConductivity = new Parameter("", 28.5),
                EnergyDensity = new Parameter("", 6.05e6),
                MeltingTemperature = new Parameter("", 1727),
                BoilingTemperature = new Parameter("", 3134)
            };

            var laser = new Laser()
            {
                BeamRadius = new Parameter("", 15e-6),
                PulseDuration = new Parameter("", 3e-6),
                AveragePower = new Parameter("", 3),
                DutyCycle = new Parameter("", 0.25),
                PulseFrequency = new Parameter("", 20e3)
            };

            var assumption = MeltTimeAssumption.Default;

            // direct translation of matlab code //
            // not optimized or future proofed //
            // i.e. units are assumed //
            double alpha_td = material.ThermalConductivity.Value / material.EnergyDensity.Value;
            double T0 = 298; // K

            // melt time assumption //
            double melt_time;
            switch (assumption)
            {
                case MeltTimeAssumption.Nicholas:
                    melt_time = laser.PulseDuration.Value * 1e9 / laser.DutyCycle.Value;
                    break;
                case MeltTimeAssumption.Justin:
                    melt_time = laser.PulseDuration.Value * 1e9 * 12;
                    break;
                case MeltTimeAssumption.Brodan:
                    double cv_prime = material.Density.Value * (133 + 182.3 / (material.BoilingTemperature.Value - T0));
                    melt_time = Math.Pow((material.BoilingTemperature.Value - material.MeltingTemperature.Value), 2) *
                        Math.Pow(Math.PI, 3) * Math.Pow(laser.BeamRadius.Value, 4) * Math.Pow(cv_prime, 2) *
                        alpha_td / (16 * Math.Pow(material.SurfaceAbsorptivity.Value, 2) * Math.Pow(laser.PulseAveragePower.Value, 2)) * 1e9;
                    break;
                case MeltTimeAssumption.Default:
                default:
                    melt_time = TerryMeltTimePolyFit.Item1 + TerryMeltTimePolyFit.Item2 * (laser.PulseDuration.Value * 1e9);
                    break;
            }

            // NAD & IFS CALCULATION - Normalized Average Displacement and Introducted Feature Slope Calculation
            // From Chao Ma's 2014 Paper, used for estimating thermocapillary flow

            // [K] Maximum melt temperature in center of melt pool using 1D model
            double Tn = 4 * (material.SurfaceAbsorptivity.Value) * (laser.PulseAveragePower.Value /
                (Math.Pow(Math.PI, 1.5) * Math.Pow(laser.BeamRadius.Value, 2) * material.EnergyDensity.Value) * Math.Sqrt(laser.PulseDuration.Value / alpha_td));

            // dimensionless temperature
            double theta_m = (material.MeltingTemperature.Value - T0) / Tn;

            // [-] Normalized Average Displacement
            double ln = -18.51 * material.SurfaceTensionCoefficient.Value * material.SurfaceAbsorptivity.Value
                * laser.PulseAveragePower.Value * Math.Pow(laser.PulseDuration.Value, 2)
                / (material.DynamicViscosity.Value * material.EnergyDensity.Value * Math.Pow(laser.BeamRadius.Value, 4))
                * (1 - theta_m) * Math.Exp(-8.80 * theta_m);

            double afs = 0.001 * ln; // [-] Average Feature Slope - Actual experimental value for S7 Tool Steel (Nicholas)

            // CRITICAL FREQUENCY CALCULATION -Calculation of critical frequency for dampening of roughness features
            // From Tyler Perry's 2009 Paper, and Madu's ____ %%% TODO - Figure out specific paper it is from

            // Critical Frequency calculation

            double fcr = Math.Sqrt(material.Density.Value / (8 * Math.Pow(Math.PI, 2) * material.DynamicViscosity.Value * melt_time * 1e-9)) * 1e-3; // [1 / mm] Critical Frequency Calculation
            double fwall = 1 / ((2 * laser.BeamRadius.Value) * 1e3); // [1 / mm] 'Wall Frequency' - i.e. frequency of diameter of beam

            // If critical frequency is less than wall frequency, use wall frequency
            if (fcr < fwall)
                fcr = fwall;

            // TODO: knnimpute //

            // TODO: remove least squares mean plane //
            // since original code has transposed data, we create a new surface object //
            // this is potentially expensive. this will probably work for now //

            // transpose & flip - in matlab code. //
            // not sure why...? //
            surface.Transpose();
            surface.FlipUpDown();

            // run 2D FFT //
            double FsX = 1 / (surface.XVector[1] - surface.XVector[0]);
            double FsY = 1 / (surface.YVector[1] - surface.YVector[0]);
            var results = CenteredFourier.CenteredFFT2(surface.ZData, FsX, FsY);

            Console.WriteLine(results.FFTX);
            Console.WriteLine(results.FreqX);
            Console.WriteLine(results.FreqY);


            // CROSS SECTION OF THE FREQUENCY SPECTRUM - Takes center of FFT matrix
            // USES THE OUTLINE INSTEAD OF PROFILE, WANT TO LOOK INTO BEST METHOD
            int col_index = Array.IndexOf(results.FreqX.AsArray(), 0);
            var fft_unpo_y = results.FFTX.Column(col_index);

            int row_index = Array.IndexOf(results.FreqY.AsArray(), 0);
            var fft_unpo_x = results.FFTX.Row(row_index);

            // create capillary low pass filter //
            var filter_enumerable = new List<Vector<double>>();
            for (int i = 0; i < results.FreqY.Count; i++)
            {
                var t1 = results.FreqX / fcr;
                t1 = -t1.PointwisePower(2);
                t1 -= Math.Pow((results.FreqY[i] / fcr), 2);
                t1 = t1.PointwiseExp();
                filter_enumerable.Add(t1);
            }

            var filter = Matrix<Double>.Build.DenseOfColumnVectors(filter_enumerable).Transpose();

            // apply low pass filter on surface //
            var redcFFT = filter.ToComplex().PointwiseMultiply(results.FFTX);
            Console.WriteLine(redcFFT);
            redcFFT = redcFFT.ifftshift();
            Console.WriteLine(redcFFT);
            Fourier.Inverse2D(redcFFT, FourierOptions.Matlab);
            Console.WriteLine(redcFFT);
            redcFFT *= surface.XVector.Count() * surface.YVector.Count();
            Console.WriteLine(redcFFT);

            // begin thermocapillary
            


            Console.ReadKey();

        }
    }
}
