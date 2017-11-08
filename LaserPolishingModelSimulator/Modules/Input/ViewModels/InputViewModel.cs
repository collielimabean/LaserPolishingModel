using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaserPolishingModelSimulator.Events;
using System.Windows.Input;
using Prism.Commands;
using LaserPolishingModel.Zygo;
using LaserPolishingModel.Parameter;
using System.Collections.ObjectModel;
using LaserPolishingModel.Util;

namespace LaserPolishingModelSimulator.Modules.Input.ViewModels
{
    class InputViewModel : BindableBase
    {
        const string DEFAULT_MATERIAL_FILE = @"materials.xml";
        const string DEFAULT_LASER_FILE = @"laser.xml";

        ZygoAsciiFile unpolishedSurface;
        string unpolishedSurfaceZygoInputFile;
        IEventAggregator eventAggregator;
        ObservableCollection<Material> materialCollection;
        ObservableCollection<Parameter> materialSettingParameters;
        ObservableCollection<Parameter> laserSettingParameters;
        bool hasLoaded;

        Laser laserSettings;
        Material selectedMaterialSettings;

        public InputViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            hasLoaded = false;
            unpolishedSurface = new ZygoAsciiFile();
            laserSettings = new Laser();
            materialCollection = new ObservableCollection<Material>();
            laserSettingParameters = new ObservableCollection<Parameter>();
        }

        public string UnpolishedSurfaceZygoFile
        {
            get { return unpolishedSurfaceZygoInputFile; }
            set { SetProperty(ref unpolishedSurfaceZygoInputFile, value); }
        }

        public Laser LaserSettings
        {
            get { return laserSettings; }
            set { laserSettings = value; }
        }

        public Material SelectedMaterial
        {
            get { return selectedMaterialSettings; }
            set
            {
                selectedMaterialSettings = value;

                // also update displayed list //
                MaterialSettingRawList = new ObservableCollection<Parameter>(selectedMaterialSettings.GetParameters());
            }
        }

        public ObservableCollection<Material> Materials
        {
            get { return materialCollection; }
            set { SetProperty(ref materialCollection, value); }
        }

        public ObservableCollection<Parameter> LaserSettingRawList
        {
            get { return laserSettingParameters; }
            set { SetProperty(ref laserSettingParameters, value); }
        }

        public ObservableCollection<Parameter> MaterialSettingRawList
        {
            get { return materialSettingParameters; }
            set { SetProperty(ref materialSettingParameters, value); }
        }
        
        public ICommand StartCommand
        {
            get { return new DelegateCommand(() => { StartSimulation(); }); }
        }

        public ICommand InputViewLoaded
        {
            get { return new DelegateCommand(() => { LoadMaterialAndLaserSettings(); }); }
        }

        void LoadMaterialAndLaserSettings()
        {
            if (hasLoaded)
                return;

            // TODO: load this from XML //
            Materials = new ObservableCollection<Material>
            {
                new Material("Material A")
                {
                    BoilingTemperature = new Parameter("Boiling Temperature", 0, "K"),
                    MeltingTemperature = new Parameter("Melting Temperature", 0, "K"),
                    Density = new Parameter("Density", 0, "kg/m^3"),
                    DynamicViscosity = new Parameter("Dynamic Viscosity", 0, "m^2/s"),
                    EnergyDensity = new Parameter("Energy Density", 0, "J/m^3-K"),
                    SurfaceAbsorptivity = new Parameter("Surface Absorptivity", 0, "m"),
                    SurfaceTensionCoefficient = new Parameter("Surface Tension Coefficient", 0, "-"),
                    ThermalConductivity = new Parameter("Thermal Conductivity", 0, "W/m-K")
                },
                new Material("Material B")
                {
                    BoilingTemperature = new Parameter("Boiling Temperature", 0, "K"),
                    MeltingTemperature = new Parameter("Melting Temperature", 0, "K"),
                    Density = new Parameter("Density", 0, "kg/m^3"),
                    DynamicViscosity = new Parameter("Dynamic Viscosity", 0, "m^2/s"),
                    EnergyDensity = new Parameter("Energy Density", 0, "J/m^3-K"),
                    SurfaceAbsorptivity = new Parameter("Surface Absorptivity", 0, "m"),
                    SurfaceTensionCoefficient = new Parameter("Surface Tension Coefficient", 0, "-"),
                    ThermalConductivity = new Parameter("Thermal Conductivity", 0, "W/m-K")
                }
            };

            // TODO: load this from XML //
            LaserSettings = new Laser()
            {
                AveragePower = new Parameter("Average Power", 0, "W"),
                BeamRadius = new Parameter("Beam Radius", 0, "μm"),
                PulseDuration = new Parameter("Pulse Duration", 0, "s"),
                PulseFrequency = new Parameter("Pulse Frequency", 0, "Hz"),
                DutyCycle = new Parameter("Duty Cycle", 0, "%")
            };

            LaserSettingRawList.AddRange(LaserSettings.GetParameters());
            hasLoaded = true;
        }

        void StartSimulation()
        {
            // XXX: raise warning to user //
            if (unpolishedSurfaceZygoInputFile == null)
                return;

            // TODO: exception handling
            unpolishedSurface.LoadFromFile(unpolishedSurfaceZygoInputFile);

            // verify numbers //
            var surface = new Surface(
                unpolishedSurface.PhaseData, 
                0,
                unpolishedSurface.CameraRes,
                0,
                unpolishedSurface.CameraRes
            );

            eventAggregator.GetEvent<LoadUnpolishedData>().Publish(surface);
        }
    }
}
