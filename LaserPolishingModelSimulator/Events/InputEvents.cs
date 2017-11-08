using LaserPolishingModel.Util;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LaserPolishingModelSimulator.Events
{

    public class LoadUnpolishedData : PubSubEvent<Surface> { }
}
