using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModel.Parameter
{
    interface IParameterCollection
    {
        IEnumerable<Parameter> GetParameters();
    }
}
