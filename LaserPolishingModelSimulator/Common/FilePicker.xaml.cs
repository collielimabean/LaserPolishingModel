using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LaserPolishingModelSimulator.Common
{
    /// <summary>
    /// Interaction logic for FilePicker.xaml
    /// </summary>
    public partial class FilePicker : UserControl
    {
        public static readonly DependencyProperty FiltersProperty =
            DependencyProperty.Register("Filters", typeof(string), typeof(FilePicker));

        public static readonly DependencyProperty DefaultExtProperty =
            DependencyProperty.Register("DefaultExt", typeof(string), typeof(FilePicker));

        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(string), typeof(FilePicker));

        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.Register("Enabled", typeof(bool), typeof(FilePicker));

        public FilePicker()
        {
            InitializeComponent();
            root.DataContext = this;
        }

        public string Filters
        {
            get { return GetValue(FiltersProperty) as string; }
            set { SetValue(FiltersProperty, value); }
        }

        public string DefaultExt
        {
            get { return GetValue(DefaultExtProperty) as string; }
            set { SetValue(DefaultExtProperty, value); }
        }

        public string SelectedFile
        {
            get { return GetValue(SelectedFileProperty) as string; }
            set { SetValue(SelectedFileProperty, value); }
        }

        public bool Enabled
        {
            get { return (bool)GetValue(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
        }

        void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = DefaultExt;
            ofd.Filter = Filters;

            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                SelectedFile = ofd.FileName;
            }
        }
    }
}
