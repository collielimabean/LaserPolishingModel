using LaserPolishingModelSimulator.Common;
using LaserPolishingModelSimulator.Modules.InputDisplay.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModelSimulator.Modules.InputDisplay
{
    class InputDisplayModule : IModule
    {
        RegionManager regionManager;

        public InputDisplayModule(RegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            regionManager.RegisterViewWithRegion(Regions.INPUT_DISPLAY_REGION, typeof(InputDisplayView));
        }
    }
}
