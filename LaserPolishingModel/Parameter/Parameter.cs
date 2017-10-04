namespace LaserPolishingModel.Parameter
{
    public class Parameter
    {
        public Parameter(
            string name, 
            double value = 0, 
            string unit = "",
            double min = double.NegativeInfinity, 
            double max = double.PositiveInfinity, 
            string comment = "")
        {
            Name = name;
            Value = value;
            Unit = unit;
            Min = min;
            Max = max;
            Comment = comment;
        }

        public string Name
        {
            get; set;
        }

        public double Value
        {
            get; set;
        }

        public double Min
        {
            get; set;
        }

        public double Max
        {
            get; set;
        }

        public string Unit
        {
            get; set;
        }

        public string Comment
        {
            get; set;
        }
    }
}
