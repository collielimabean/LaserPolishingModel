using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LaserPolishingModel.Zygo
{
    enum SoftwareType
    {
        Unknown = 0,
        MetroPro = 1,
        MetroBASIC = 2,
        d2dbug = 3
    }

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

    struct SoftwareVersion
    {
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int BugVersion { get; set; }
    }

    enum DataSource
    {
        FromInstrument = 0,
        Generated = 1
    }

    public class ZygoAsciiFile
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
        const string HEADER_END_DELIMITER = "#";
        const string SOFTWARE_DATE_FORMAT = "ddd MMM dd H:mm:ss yyyy";
        const int INVALID_PHASE_DATA_THRESHOLD = 2147483640;
        const int INVALID_INTENSITY_DATA_THRESHOLD = 65535;


        const int SOFTWARE_TYPE_INDEX = 1;
        const int MAJOR_VERS_INDEX = 2;
        const int MINOR_VERS_INDEX = 3;
        const int BUG_VERS_INDEX = 4;
        const int SOFTWARE_DATE_INDEX = 5;
        const int INTENS_ORIGIN_X_INDEX = 6;
        const int INTENS_ORIGIN_Y_INDEX = 7;
        const int INTENS_WIDTH_INDEX = 8;
        const int INTENS_HEIGHT_INDEX = 9;
        const int N_BUCKETS_INDEX = 10;
        const int INTENS_RANGE_INDEX = 11;
        const int PHASE_ORIGIN_X_INDEX = 12;
        const int PHASE_ORIGIN_Y_INDEX = 13;
        const int PHASE_WIDTH_INDEX = 14;
        const int PHASE_HEIGHT_INDEX = 15;
        const int COMMENT_INDEX = 16;
        const int PART_SER_NUM_INDEX = 17;
        const int PART_NUM_INDEX = 18;
        const int SOURCE_INDEX = 19;
        const int INTF_SCALE_FACTOR_INDEX = 20;
        const int WAVELENGTH_IN_INDEX = 21;
        const int NUMERIC_APERTURE_INDEX = 22;
        const int OBLIQUITY_FACTOR_INDEX = 23;
        const int MAGNIFICATION_INDEX = 24;
        const int CAMERA_RES_INDEX = 25;
        const int TIME_STAMP_INDEX = 26;
        const int CAMERA_WIDTH_INDEX = 27;
        const int CAMERA_HEIGHT_INDEX = 28;
        const int SYSTEM_TYPE_INDEX = 29;
        const int SYSTEM_BOARD_INDEX = 30;
        const int SYSTEM_SERIAL_INDEX = 31;
        const int INSTRUMENT_ID_INDEX = 32;
        const int OBJECTIVE_NAME_INDEX = 33;
        const int ACQUIRE_MODE_INDEX = 34;
        const int INTENS_AVGS_INDEX = 35;
        const int PZT_CAL_INDEX = 36;
        const int PZT_GAIN_INDEX = 37;
        const int PZT_GAIN_TOLERANCE_INDEX = 38;
        const int AGC_INDEX = 39;
        const int TARGET_RANGE_INDEX = 40;
        const int LIGHT_LEVEL_INDEX = 41;
        const int MIN_MOD_INDEX = 42;
        const int MIN_MOD_PTS_INDEX = 43;
        const int PHASE_RES_INDEX = 44;
        const int PHASE_AVGS_INDEX = 45;
        const int MINIMUM_AREA_SIZE_INDEX = 46;
        const int DISCON_ACTION_INDEX = 47;
        const int DISCON_FILTER_INDEX = 48;
        const int CONNECTION_ORDER_INDEX = 49;
        const int REMOVE_TILT_BIAS_INDEX = 50;
        const int DATA_SIGN_INDEX = 51;
        const int CODE_V_TYPE_INDEX = 52;
        const int SUBTRACT_SYS_ERR_INDEX = 53;
        const int SYS_ERR_FILE_INDEX = 54;
        const int REFRACTIVE_INDEX_INDEX = 55;
        const int PART_THICKNESS_INDEX = 56;
        const int ZOOM_DESC_INDEX = 57;
        #endregion

        static readonly Regex COMPILED_HEADER_REGEX = new Regex(ASCII_HEADER_REGEX, RegexOptions.Multiline);

        static DateTime ConvertFromUnixTimestamp(int timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public ZygoAsciiFile()
        {
        }

        // XXX: add async version
        public void LoadFromFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException();

            using (StreamReader reader = new StreamReader(filename))
            {
                StringBuilder sb = new StringBuilder();

                // read header //
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    sb.AppendLine(line);

                    if (line.Trim() == HEADER_END_DELIMITER)
                        break;
                }

                // attempt to parse header //
                var match = COMPILED_HEADER_REGEX.Match(sb.ToString());
                if (!match.Success)
                    throw new FileFormatException();

                // grab header parts //
                // note: okay to use regular Parse() instead of TryParse, since header verified //
                #region Parsing
                SoftwareType = int.Parse(match.Groups[SOFTWARE_TYPE_INDEX].Value);
                MajorVers = int.Parse(match.Groups[MAJOR_VERS_INDEX].Value);
                MinorVers = int.Parse(match.Groups[MINOR_VERS_INDEX].Value);
                BugVers = int.Parse(match.Groups[BUG_VERS_INDEX].Value);
                SoftwareDate = DateTime.ParseExact(match.Groups[SOFTWARE_DATE_INDEX].Value.Trim(), SOFTWARE_DATE_FORMAT, null);
                IntensOriginX = int.Parse(match.Groups[INTENS_ORIGIN_X_INDEX].Value);
                IntensOriginY = int.Parse(match.Groups[INTENS_ORIGIN_Y_INDEX].Value);
                IntensWidth = int.Parse(match.Groups[INTENS_WIDTH_INDEX].Value);
                IntensHeight = int.Parse(match.Groups[INTENS_HEIGHT_INDEX].Value);
                NBuckets = int.Parse(match.Groups[N_BUCKETS_INDEX].Value);
                IntensRange = int.Parse(match.Groups[INTENS_RANGE_INDEX].Value);
                PhaseOriginX = int.Parse(match.Groups[PHASE_ORIGIN_X_INDEX].Value);
                PhaseOriginY = int.Parse(match.Groups[PHASE_ORIGIN_Y_INDEX].Value);
                PhaseWidth = int.Parse(match.Groups[PHASE_WIDTH_INDEX].Value);
                PhaseHeight = int.Parse(match.Groups[PHASE_HEIGHT_INDEX].Value);
                Comment = match.Groups[COMMENT_INDEX].Value;
                PartSerNum = match.Groups[PART_SER_NUM_INDEX].Value;
                PartNum = match.Groups[PART_NUM_INDEX].Value;
                Source = int.Parse(match.Groups[SOURCE_INDEX].Value);
                IntfScaleFactor = double.Parse(match.Groups[INTF_SCALE_FACTOR_INDEX].Value);
                WavelengthIn = double.Parse(match.Groups[WAVELENGTH_IN_INDEX].Value);
                NumericAperture = double.Parse(match.Groups[NUMERIC_APERTURE_INDEX].Value);
                ObliquityFactor = double.Parse(match.Groups[OBLIQUITY_FACTOR_INDEX].Value);
                Magnification = double.Parse(match.Groups[MAGNIFICATION_INDEX].Value);
                CameraRes = double.Parse(match.Groups[CAMERA_RES_INDEX].Value);
                TimeStamp = ConvertFromUnixTimestamp(int.Parse(match.Groups[TIME_STAMP_INDEX].Value));
                CameraWidth = int.Parse(match.Groups[CAMERA_WIDTH_INDEX].Value);
                CameraHeight = int.Parse(match.Groups[CAMERA_HEIGHT_INDEX].Value);
                SystemType = int.Parse(match.Groups[SYSTEM_TYPE_INDEX].Value);
                SystemBoard = int.Parse(match.Groups[SYSTEM_BOARD_INDEX].Value);
                SystemSerial = int.Parse(match.Groups[SYSTEM_SERIAL_INDEX].Value);
                InstrumentId = int.Parse(match.Groups[INSTRUMENT_ID_INDEX].Value);
                ObjectiveName = match.Groups[OBJECTIVE_NAME_INDEX].Value;
                AcquireMode = int.Parse(match.Groups[ACQUIRE_MODE_INDEX].Value);
                IntensAvgs = int.Parse(match.Groups[INTENS_AVGS_INDEX].Value);
                PZTCal = int.Parse(match.Groups[PZT_CAL_INDEX].Value);
                PZTGain = int.Parse(match.Groups[PZT_GAIN_INDEX].Value);
                PZTGainTolerance = int.Parse(match.Groups[PZT_GAIN_TOLERANCE_INDEX].Value);
                AGC = int.Parse(match.Groups[AGC_INDEX].Value);
                TargetRange = double.Parse(match.Groups[TARGET_RANGE_INDEX].Value);
                LightLevel = double.Parse(match.Groups[LIGHT_LEVEL_INDEX].Value);
                MinMod = int.Parse(match.Groups[MIN_MOD_INDEX].Value);
                MinModPts = int.Parse(match.Groups[MIN_MOD_PTS_INDEX].Value);
                PhaseRes = int.Parse(match.Groups[PHASE_RES_INDEX].Value);
                PhaseAvgs = int.Parse(match.Groups[PHASE_AVGS_INDEX].Value);
                MinimumAreaSize = int.Parse(match.Groups[MINIMUM_AREA_SIZE_INDEX].Value);
                DisconAction = int.Parse(match.Groups[DISCON_ACTION_INDEX].Value);
                DisconFilter = double.Parse(match.Groups[DISCON_FILTER_INDEX].Value);
                ConnectionOrder = int.Parse(match.Groups[CONNECTION_ORDER_INDEX].Value);
                RemoveTiltBias = int.Parse(match.Groups[REMOVE_TILT_BIAS_INDEX].Value);
                DataSign = int.Parse(match.Groups[DATA_SIGN_INDEX].Value);
                CodeVType = int.Parse(match.Groups[CODE_V_TYPE_INDEX].Value);
                SubtractSysErr = int.Parse(match.Groups[SUBTRACT_SYS_ERR_INDEX].Value);
                SysErrFile = match.Groups[SYS_ERR_FILE_INDEX].Value;
                RefractiveIndex = double.Parse(match.Groups[REFRACTIVE_INDEX_INDEX].Value);
                PartThickness = double.Parse(match.Groups[PART_THICKNESS_INDEX].Value);
                ZoomDesc = match.Groups[ZOOM_DESC_INDEX].Value;
                #endregion

                if (NBuckets != 1)
                    throw new Exception("> 1 bucket is not supported!");

                // convert some header metadata to scaled values, enum, etc... //
                PhaseRes = (PhaseRes == 1) ? 32768 : 4096; // XXX: convert to constants

                // read intensity data //
                IntensityData = ParseIntegerArray(reader, IntensHeight, IntensWidth, 65535);

                // read phase data //
                // conver to wave unit //
                PhaseData = ParseIntegerArray(reader, PhaseHeight, PhaseWidth, 2147483640, IntfScaleFactor * ObliquityFactor / PhaseRes);
            }
        }

        double[,] ParseIntegerArray(StreamReader streamReader, int height, int width, int invalid_threshold, double scale_factor = 1)
        {
            string line;
            int index = 0;
            int totalNumberPoints = height * width;
            double[,] data = new double[height, width];

            while ((line = streamReader.ReadLine().Trim()) != null)
            {
                if (line.Trim() == HEADER_END_DELIMITER)
                    break;

                var raw_elements = line.Split();
                foreach (var element in raw_elements)
                {
                    int val;

                    if (index >= totalNumberPoints)
                        throw new FileFormatException("Too many points!");

                    int row = index / width;
                    int col = index % width;

                    if (!int.TryParse(element, out val))
                        throw new FileFormatException("Point was not an integer!");

                    // XXX: FIX ME - either set NaN OR run >>> knnimpute <<<
                    if (val >= invalid_threshold)
                        val = -1;

                    data[row, col] = val * scale_factor;
                    index++;
                }
            }

            return data;
        }

        #region Properties
        public double[,] IntensityData
        {
            get; private set;
        }

        public double[,] PhaseData
        {
            get; private set;
        }

        public int SoftwareType
        {
            get; private set;
        }

        public int MajorVers
        {
            get; private set;
        }

        public int MinorVers
        {
            get; private set;
        }

        public int BugVers
        {
            get; private set;
        }

        public DateTime SoftwareDate
        {
            get; private set;
        }

        public int IntensOriginX
        {
            get; private set;
        }

        public int IntensOriginY
        {
            get; private set;
        }

        public int IntensWidth
        {
            get; private set;
        }

        public int IntensHeight
        {
            get; private set;
        }

        public int NBuckets
        {
            get; private set;
        }

        public int IntensRange
        {
            get; private set;
        }

        public int PhaseOriginX
        {
            get; private set;
        }

        public int PhaseOriginY
        {
            get; private set;
        }

        public int PhaseWidth
        {
            get; private set;
        }

        public int PhaseHeight
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

        public int Source
        {
            get; private set;
        }

        public double IntfScaleFactor
        {
            get; private set;
        }

        public double WavelengthIn
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

        public double CameraRes
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

        public int SystemType
        {
            get; private set;
        }

        public int SystemBoard
        {
            get; private set;
        }

        public int SystemSerial
        {
            get; private set;
        }

        public int InstrumentId
        {
            get; private set;
        }

        public string ObjectiveName
        {
            get; private set;
        }

        public int AcquireMode
        {
            get; private set;
        }

        public int IntensAvgs
        {
            get; private set;
        }

        public int PZTCal
        {
            get; private set;
        }

        public int PZTGain
        {
            get; private set;
        }

        public int PZTGainTolerance
        {
            get; private set;
        }

        public int AGC
        {
            get; private set;
        }

        public double TargetRange
        {
            get; private set;
        }

        public double LightLevel
        {
            get; private set;
        }

        public int MinMod
        {
            get; private set;
        }

        public int MinModPts
        {
            get; private set;
        }

        public int PhaseRes
        {
            get; private set;
        }

        public int PhaseAvgs
        {
            get; private set;
        }

        public int MinimumAreaSize
        {
            get; private set;
        }

        public int DisconAction
        {
            get; private set;
        }

        public double DisconFilter
        {
            get; private set;
        }

        public int ConnectionOrder
        {
            get; private set;
        }

        public int RemoveTiltBias
        {
            get; private set;
        }

        public int DataSign
        {
            get; private set;
        }

        public int CodeVType
        {
            get; private set;
        }

        public int SubtractSysErr
        {
            get; private set;
        }

        public string SysErrFile
        {
            get; private set;
        }

        public double RefractiveIndex
        {
            get; private set;
        }

        public double PartThickness
        {
            get; private set;
        }

        public string ZoomDesc
        {
            get; private set;
        }
        #endregion
    }
}
