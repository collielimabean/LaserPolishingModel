using Prism.Events;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModelSimulator.Modules.OutputDisplay
{
    class OutputDisplayModule : IModule
    {
        RegionManager regionManager;

        public OutputDisplayModule(RegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
