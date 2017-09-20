using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserPolishingModel.Zygote
{
    enum SystemType
    {
        None = 0,
        MarkIVxp = 1,
        Maxim3D = 2,
        MaximNT = 3,
        GPI_XP = 4,
        NewView = 5,
        MaximGP = 6,
        NewView_GP = 7,
        MarktoGPI_Conversion = 8
    }

    class SystemInformation
    {


        public DateTime SoftwareDate
        {
            get; set;
        }

        public SystemType SystemType
        {
            get; set;
        }

        public int SystemBoard
        {
            get; set;
        }

        public int SerialNumber
        {
            get; set;
        }

        public int InstrumentID
        {
            get; set;
        }
    }
}
