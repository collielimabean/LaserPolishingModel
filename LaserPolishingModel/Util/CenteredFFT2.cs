using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.IntegralTransforms;

namespace LaserPolishingModel.Util
{
    public static class CenteredFourier
    {
        public struct CenteredFFT2Results
        {
            public Vector<double> FreqX { get; set; }
            public Vector<double> FreqY { get; set; }
            public Matrix<Complex> FFTX { get; set; }
            public Matrix<Complex> FFTY { get; set; }
        }

        public static CenteredFFT2Results CenteredFFT2(Matrix<double> zData, double fsx, double fsy)
        {
            var results = new CenteredFFT2Results();

            int n = zData.ColumnCount;
            int m = zData.RowCount;

            var freqX = (from number in Enumerable.Range(-n / 2, n) select number / (n / fsx)).ToArray();
            var freqY = (from number in Enumerable.Range(-m / 2, m) select number / (m / fsy)).ToArray();

            // compute fft2(Z) //
            var fft2_z = zData.ToComplex();
            Fourier.Forward2D(fft2_z, FourierOptions.Matlab);

            // FFT = fft2(Z)./(M*N) //
            // FFT = fftshift(FFT) //
            var fft = fft2_z.Clone() / (m * n);
            Console.WriteLine(fft);
            fft = fft.fftshift();
            Console.WriteLine(fft);

            // FFT2 = (1/(FsY*FsX))*fft2(Z) //
            // FFT2 = fftshift(FFT2) //
            var fft2 = fft2_z.Clone() * (1 / (fsx * fsy));
            fft2 = fft2.fftshift();

            results.FreqX = Vector<double>.Build.DenseOfArray(freqX);
            results.FreqY = Vector<double>.Build.DenseOfArray(freqY);
            results.FFTX = fft;
            results.FFTY = fft2;
            return results;
        }
    }
}
