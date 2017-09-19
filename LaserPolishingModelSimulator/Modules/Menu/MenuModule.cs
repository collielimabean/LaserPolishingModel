using LaserPolishingModelSimulator.Common;
using LaserPolishingModelSimulator.Modules.Menu.Views;
using Prism.Modularity;
using Prism.Regions;

namespace LaserPolishingModelSimulator.Modules.Menu
{
    public class MenuModule : IModule
    {
        RegionManager regionManager;

        public MenuModule(RegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            regionManager.RegisterViewWithRegion(Regions.MENU_REGION, typeof(MenuView));
        }
    }
}
