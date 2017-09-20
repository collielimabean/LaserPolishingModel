using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModelSimulator.Modules.Input.ViewModels
{
    class InputViewModel : BindableBase
    {
        string zygoInputFile;

        public string ZygoFile
        {
            get { return zygoInputFile; }
            set { SetProperty(ref zygoInputFile, value); }
        }
    }
}
