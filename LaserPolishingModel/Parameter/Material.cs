using System.Collections.Generic;

namespace LaserPolishingModel.Parameter
{
    public class Material : IParameterCollection
    {
        public Material(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Material name (e.g. tool steel)
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// stc
        /// </summary>
        public Parameter SurfaceTensionCoefficient
        {
            get; set;
        }

        /// <summary>
        /// mu
        /// </summary>
        public Parameter DynamicViscosity
        {
            get; set;
        }

        /// <summary>
        /// k
        /// </summary>
        public Parameter ThermalConductivity
        {
            get; set;
        }

        /// <summary>
        /// rho * c
        /// </summary>
        public Parameter EnergyDensity
        {
            get; set;
        }

        /// <summary>
        /// T_m
        /// </summary>
        public Parameter MeltingTemperature
        {
            get; set;
        }

        /// <summary>
        /// T_b
        /// </summary>
        public Parameter BoilingTemperature
        {
            get; set;
        }

        /// <summary>
        /// absp
        /// </summary>
        public Parameter SurfaceAbsorptivity
        {
            get; set;
        }

        /// <summary>
        /// rho
        /// </summary>
        public Parameter Density
        {
            get; set;
        }

        public IEnumerable<Parameter> GetParameters()
        {
            return new List<Parameter>
            {
                SurfaceTensionCoefficient,
                DynamicViscosity,
                ThermalConductivity,
                EnergyDensity,
                MeltingTemperature,
                BoilingTemperature,
                SurfaceAbsorptivity,
                Density
            };
        }
    }
}
