using LaserPolishingModel.Util;
using LaserPolishingModelSimulator.Common.SurfacePlot3D;
using LaserPolishingModelSimulator.Events;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModelSimulator.Modules.OutputDisplay.ViewModels
{
    class OutputDisplayViewModel : BindableBase
    {
        SurfacePlotModel ripplesModel;
        IEventAggregator eventAggregator;

        public OutputDisplayViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            ripplesModel = new SurfacePlotModel();
            ripplesModel.ShowMiniCoordinates = true;

            this.eventAggregator.GetEvent<LoadRippleData>().Subscribe(HandleLoadUnpolishedData, true);
        }

        public SurfacePlotModel RipplesModel
        {
            get { return ripplesModel; }
            set { ripplesModel = value; }
        }

        void HandleLoadUnpolishedData(Surface surface)
        {
            var data = surface.ZData.ToArray();
            ripplesModel.PlotData(data);
        }
    }
}
