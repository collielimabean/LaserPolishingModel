using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModelSimulator.Events
{
    public class Plot2DEventArgs
    {
        public string Name { get; set; }
        public double[] XVector { get; set; }
        public double[] YVector { get; set; }
    }
}
