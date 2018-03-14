using LaserPolishingModelSimulator.Modules.Input;
using LaserPolishingModelSimulator.Modules.InputDisplay;
using LaserPolishingModelSimulator.Modules.Menu;
using LaserPolishingModelSimulator.Modules.OutputDisplay;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;

namespace LaserPolishingModelSimulator
{
    class Bootstrapper : UnityBootstrapper
    {
        // initialize modules here //
        static readonly Type[] moduleLoadList =
        {
            typeof(MenuModule),
            typeof(InputModule),
            typeof(InputDisplayModule),
            typeof(OutputDisplayModule)
        };

        protected override DependencyObject CreateShell()
        {
            return Container.TryResolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var catalog = ModuleCatalog as ModuleCatalog;
            if (catalog == null) // XXX: handle gracefully
                throw new Exception("Failed to obtain ModuleCatalog!");

            foreach (var module in moduleLoadList)
                catalog.AddModule(module);
        }
    }
}
