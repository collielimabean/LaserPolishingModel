﻿using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaserPolishingModelSimulator.Events;
using LaserPolishingModelSimulator.Common.SurfacePlot3D;
using Prism.Events;

namespace LaserPolishingModelSimulator.Modules.InputDisplay.ViewModels
{
    class InputDisplayViewModel : BindableBase
    {
        SurfacePlotModel unpolishedModel;
        IEventAggregator eventAggregator;

        public InputDisplayViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            unpolishedModel = new SurfacePlotModel();
            unpolishedModel.ShowMiniCoordinates = true;

            this.eventAggregator.GetEvent<LoadUnpolishedData>().Subscribe(HandleLoadUnpolishedData, true);
        }

        public SurfacePlotModel UnpolishedSurface
        {
            get { return unpolishedModel; }
            set { unpolishedModel = value; }
        }

        void HandleLoadUnpolishedData(double[,] data)
        {
            UnpolishedSurface.PlotData(data);
        }
    }
}
