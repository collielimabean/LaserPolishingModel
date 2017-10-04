using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModel.Parameter
{
    public class Laser : IParameterCollection
    {
        public Parameter BeamRadius
        {
            get; set;
        }

        public Parameter PulseDuration
        {
            get; set;
        }

        public Parameter AveragePower
        {
            get; set;
        }

        public Parameter DutyCycle
        {
            get; set;
        }

        public Parameter PulseFrequency
        {
            get; set;
        }

        // computed from others //
        // TODO: make this more elegant - not hardcoded.... //
        public Parameter PulseAveragePower
        {
            get
            {
                return new Parameter(
                    "Pulse Average Power",
                    AveragePower.Value / (PulseFrequency.Value * PulseDuration.Value));
            }
        }

        public IEnumerable<Parameter> GetParameters()
        {
            return new List<Parameter>
            {
                BeamRadius,
                PulseDuration,
                AveragePower,
                DutyCycle,
                PulseFrequency
            };
        }
    }
}
