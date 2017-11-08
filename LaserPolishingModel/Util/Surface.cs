using MathNet.Numerics.LinearAlgebra;
using System.Windows.Media.Media3D;

namespace LaserPolishingModel.Util
{
    public class Surface
    {
        public Surface(double[,] zData, double xMin, double xInc, double yMin, double yInc) : 
            this(Matrix<double>.Build.DenseOfArray(zData), xMin, xInc, yMin, yInc)
        {
        }

        public Surface(Matrix<double> zData, double xMin, double xInc, double yMin, double yInc)
        {
            int cols = zData.ColumnCount;
            int rows = zData.RowCount;
            int largest = cols > rows ? cols : rows;

            double[] yArray = new double[rows];
            double[] xArray = new double[cols];

            // optimization: use a single for loop to the max(n, m). the smaller
            // dimension drops out when i exceeds the dimension.
            for (int i = 0; i < largest; i++)
            {
                if (i < yArray.Length)
                    yArray[i] = yInc * i;
                if (i < xArray.Length)
                    xArray[i] = xInc * i;
            }

            Initialize(zData, xArray, yArray);
        }

        public Surface(Matrix<double> zData, double[] xVector, double[] yVector)
        {
            Initialize(zData, xVector, yVector);
        }

        public double[] XVector { get; set; }
        public double[] YVector { get; set; }
        public Matrix<double> ZData { get; set; }

        public void Transpose()
        {
            ZData = ZData.Transpose();

            var temp = XVector;
            XVector = YVector;
            YVector = temp;
        }

        public void FlipUpDown()
        {
            ZData = ZData.flipud();
        }

        public Point3D[,] GetPoints()
        {
            if (ZData == null)
                return null;

            int cols = ZData.ColumnCount;
            int rows = ZData.RowCount;

            var points = new Point3D[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    points[i, j] = new Point3D(XVector[j], YVector[i], ZData[i, j]);
                }
            }

            return points;
        }

        void Initialize(Matrix<double> zData, double[] xVector, double[] yVector)
        {
            ZData = zData;
            XVector = xVector;
            YVector = yVector;
        }
    }
}
