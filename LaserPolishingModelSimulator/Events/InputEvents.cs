using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModelSimulator.Events
{
    public class LoadUnpolishedData : PubSubEvent<double[,]> { }
}
