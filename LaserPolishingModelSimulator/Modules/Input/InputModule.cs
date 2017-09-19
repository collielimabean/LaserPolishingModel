using LaserPolishingModelSimulator.Common;
using LaserPolishingModelSimulator.Modules.Input.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModelSimulator.Modules.Input
{
    class InputModule : IModule
    {
        RegionManager regionManager;

        public InputModule(RegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            regionManager.RegisterViewWithRegion(Regions.INPUT_REGION, typeof(InputView));
        }
    }
}
