/// This file is part of wpf-surfaceplot3d.
/// 
/// wpf-surfaceplot3d is free software: you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
/// 
/// wpf-surfaceplot3d is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License
/// along with wpf-surfaceplot3d.  If not, see<http://www.gnu.org/licenses/>.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace LaserPolishingModelSimulator.Common.SurfacePlot3D
{
    /// <summary>
    /// Interaction logic for SurfacePlotView.xaml
    /// </summary>
    public partial class SurfacePlotView : UserControl
    {
        public SurfacePlotView()
        {
            InitializeComponent();
            DataContext = LayoutRoot.DataContext;
            hViewport.ZoomExtentsGesture = new KeyGesture(Key.Space);
        }

        public Point3D[,] DataPoints
        {
            get { return (Point3D[,])GetValue(DataPointsProperty); }
            set { SetValue(DataPointsProperty, value); }
        }

        public static readonly DependencyProperty DataPointsProperty = DependencyProperty.Register("DataPoints", typeof(Point3D[,]), typeof(SurfacePlotView), new FrameworkPropertyMetadata(SurfacePlotVisual3D.SamplePoints));
        
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SurfacePlotView), new FrameworkPropertyMetadata("Surface Plot Title"));

        public string XAxisLabel
        {
            get { return (string)GetValue(XAxisLabelProperty); }
            set { SetValue(XAxisLabelProperty, value); }
        }

        public static readonly DependencyProperty XAxisLabelProperty = DependencyProperty.Register("XAxisLabel", typeof(string), typeof(SurfacePlotView), new FrameworkPropertyMetadata("X Axis Label"));

        public string YAxisLabel
        {
            get { return (string)GetValue(YAxisLabelProperty); }
            set { SetValue(YAxisLabelProperty, value); }
        }

        public static readonly DependencyProperty YAxisLabelProperty = DependencyProperty.Register("YAxisLabel", typeof(string), typeof(SurfacePlotView), new FrameworkPropertyMetadata("Y Axis Label"));

        public string ZAxisLabel
        {
            get { return (string)GetValue(ZAxisLabelProperty); }
            set { SetValue(ZAxisLabelProperty, value); }
        }

        public static readonly DependencyProperty ZAxisLabelProperty = DependencyProperty.Register("ZAxisLabel", typeof(string), typeof(SurfacePlotView), new FrameworkPropertyMetadata("Z Axis Label"));

        public bool ShowSurfaceMesh
        {
            get { return (bool)GetValue(ShowSurfaceMeshProperty); }
            set { SetValue(ShowSurfaceMeshProperty, value); }
        }

        public static readonly DependencyProperty ShowSurfaceMeshProperty = DependencyProperty.Register("ShowSurfaceMesh", typeof(bool), typeof(SurfacePlotView), new FrameworkPropertyMetadata(true));

        public bool ShowContourLines
        {
            get { return (bool)GetValue(ShowContourLinesProperty); }
            set { SetValue(ShowContourLinesProperty, value); }
        }

        public static readonly DependencyProperty ShowContourLinesProperty = DependencyProperty.Register("ShowContourLines", typeof(bool), typeof(SurfacePlotView), new FrameworkPropertyMetadata(true));

        public bool ShowMiniCoordinates
        {
            get { return (bool)GetValue(ShowMiniCoordinatesProperty); }
            set { SetValue(ShowMiniCoordinatesProperty, value); }
        }

        public static readonly DependencyProperty ShowMiniCoordinatesProperty = DependencyProperty.Register("ShowMiniCoordinates", typeof(bool), typeof(SurfacePlotView), new FrameworkPropertyMetadata(true));
        
    }
}
