using LaserPolishingModel.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LaserPolishingModel.Zygote
{
    enum SoftwareType
    {
        Unknown = 0,
        MetroPro = 1,
        MetroBASIC = 2,
        d2dbug = 3
    }

    struct SoftwareVersion
    {
        int MajorVersion;
        int MinorVersion;
        int BugVersion;
    }

    enum DataSource
    {
        FromInstrument = 0,
        Generated = 1
    }

    class ZygoAsciiFile
    {
        #region Constants
        // I sincerely apologize for this horrendous regular expression.
        // The Zygo ASCII format just assumes you know where each part is located.
        // So it's not like a JSON file with key-value pairs so you can parse it.
        // You have two options: parse line by line with a massive if else chain for
        // each line, or you could do a regex and pull from matching groups.
        // As you can see, I chose the regex option. I chose to hide it in a region
        // block for good reason.
        const string ASCII_HEADER_REGEX =
@"Zygo ASCII Data File - Format \d+
(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+""(.*)""
(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)
(\d+)\s+(\d+)\s+(\d+)\s+(\d+)
""(.*)""
""(.*)""
""(.*)""
(\d+)\s+(\d+.\d+)\s+(\d+.\de[-]?\d+)\s+(\d+)\s+(\d+)\s(\d+)\s+(\d+.\d+e[-]?\d+)\s+(\d+)
(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+""(.*)""
(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+.\d+)\s+(\d+)\s+(\d+)
(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)
(\d+)\s+""(.*)""
(\d+)\s+(\d+)
""(.*)""
#";
        public const double UNKNOWN_CAMERA_RESOLUTION = 0;
        #endregion

        static readonly Regex COMPILED_HEADER_REGEX = new Regex(ASCII_HEADER_REGEX, RegexOptions.Multiline);

        public ZygoAsciiFile()
        {
        }

        // XXX: add async version
        public bool LoadFromFile(string filename)
        {
            throw new NotImplementedException();
        }

        #region Properties
        public SoftwareVersion Version
        {
            get; set;
        }

        public SoftwareType SoftwareType
        {
            get; private set;
        }

        public SystemInformation SystemInformation
        {
            get; private set;
        }

        public Intensity Intensity
        {
            get; private set;
        }

        public Phase Phase
        {
            get; private set;
        }

        public string Comment
        {
            get; private set;
        }

        public string PartSerNum
        {
            get; private set;
        }

        public string PartNum
        {
            get; private set;
        }

        public DataSource Source
        {
            get; private set;
        }

        public double InterferometricScaleFactor
        {
            get; private set;
        }

        public double InterferogramWavelength
        {
            get; private set;
        }

        public double NumericAperture
        {
            get; private set;
        }

        public double ObliquityFactor
        {
            get; private set;
        }

        public double Magnification
        {
            get; private set;
        }

        public double CameraResolution
        {
            get; private set;
        }

        public DateTime TimeStamp
        {
            get; private set;
        }

        public int CameraWidth
        {
            get; private set;
        }

        public int CameraHeight
        {
            get; private set;
        }
        #endregion
    }
}
