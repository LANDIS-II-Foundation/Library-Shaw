using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Landis.Extension.ShawDamm
{
    public partial class ShawDamm
    {
        private static Regex _whiteSpaceRegex = new Regex(@"\s+");
        private static Regex _repeatInputRegex = new Regex(@"^(?'count'\d+)\*(?'value'.*)$");

        private enum InputFile
        {
            SiteDescriptionData = 11,
            WeatherData = 12,
            MoistureProfiles = 13,
            TemperatureProfiles = 14,
            WaterExtraction = 15,
        }

        // the enum values are the indices of the corresponding _lvlout[] array.
        private enum OutputFile
        {
            EntireProfile = 1,
            ComparisonSimulatedMeasured = 2,
            SoilTemperature = 3,
            SoilTotalWaterContent = 4,
            SoilLiquidWaterContent = 5,
            SoilMatricPotential = 6,
            PlantCanopyTemperature = 7,
            PlantCanopyHumidity = 8,
            SnowTemperature = 9,
            SurfaceEnergyBalance = 10,
            WaterBalance = 11,
            WaterFlowNodes = 12,
            WaterExtractedPlants = 13,
            LateralFlow = 14,
            SnowFrostDepth = 15,
            TotalSaltConc = 16,
            SoilSolutionConc = 17,
            //ExtraOutput1 = 38,
            //ExtraOutput2 = 39,
        }

        // hardcoded file names with indices that match those of _lvlout[]
        private static string[] _outputFileNames =
        {
            "",
            "profile.out",
            "comparison.out",
            "soiltemp.out",
            "soilwater.out",
            "soilliquid.out",
            "soilmatric.out",
            "planttemp.out",
            "planthumidity.out",
            "snowtemp.out",
            "energy.out",
            "water.out",
            "flow.out",
            "extract.out",
            "lateral.out",
            "snow.out",
            "salt.out",
            "solutes.out",
        };

        // line 1449
        //private bool IoFiles(string inputFileName, out string wd, out int iversion, out int mtstep, out int iflagsi, out int inph2O, out int mwatrxt, out int[] lvlout)
        //{
        //    wd = string.Empty;
        //    iversion = mtstep = iflagsi = inph2O = 0;
        //    mwatrxt = 0;
        //    lvlout = new int[21];
        //    _outputWriters = new Dictionary<OutputFile, StreamWriter>(); // dictionary of stream writers for various kinds of output
        //    _inputReaders = new Dictionary<InputFile, StreamReader>();  // dictionary of input data

        //    List<string> t;

        //    //Console.WriteLine(">>>>>>> Simultaneous Heat And Water (SHAW) Model <<<<<<<");
        //    //Console.WriteLine("                    Version 3.0.1");
        //    //Console.WriteLine("Enter the file path containing the list of input/output files:");

        //    //var inputFileName = Console.ReadLine() ?? string.Empty;
        //    StreamReader inputFileReader;

        //    if (!OpenStreamReader(inputFileName, out inputFileReader))
        //    {
        //        return false;
        //    }

        //    wd = Path.GetDirectoryName(inputFileName);

        //    // READ FIRST LINE WITH ALPHAMERIC TO SEE IF IT IS AN OLDER VERSION
        //    var ioutput = 2;
        //    var s = inputFileReader.ReadLine();
        //    for (var i = 0; i < s.Length; ++i)
        //    {
        //        // FIND 'SHAW' TEXT
        //        if (s[i] == 's' || s[i] == 'S')
        //        {
        //            // START OF SHAW VERSION -- FIND VERSION
        //            var js = 0;
        //            for (var j = i + 1; j < s.Length; ++j)
        //            {
        //                if (s[j] == '2')
        //                {
        //                    iversion = 2;
        //                    if (s.Substring(j + 1).Contains("8"))   // OUTPUT FILE SELECTION FOLLOWS VERSION 3
        //                        ioutput = 3;
        //                    js = j;
        //                    break;
        //                }

        //                if (s[j] == '3')
        //                {
        //                    iversion = 3;
        //                    ioutput = 3;
        //                    js = j;
        //                    break;
        //                }
        //            }

        //            // CHECK IF THIS IS AN SI VERSION FOR STRICTLY SI UNITS
        //            if (s.Substring(js + 1).IndexOf("SI", StringComparison.OrdinalIgnoreCase) >= 0)
        //                iflagsi = 1;

        //            // get next line
        //            s = inputFileReader.ReadLine();
        //            break;
        //        }

        //        if (s[i] == '0' || s[i] == '1' || s[i] == '2')
        //        {
        //            // NO 'SHAW' TEXT - THIS IS FIRST LINE OF VERSION 2.x
        //            iversion = 2;
        //            iflagsi = 0;
        //            break;
        //        }
        //    }

        //    if (iversion == 0)
        //    {
        //        Console.WriteLine("COULD NOT DETERMINE THE INPUT FORMAT VERSION");
        //        Console.WriteLine("FROM FILE CONTAINING LIST OF INPUT/OUTPUT FILES");
        //        Console.WriteLine($" ===> Check the input file:{inputFileName}");
        //        return false;
        //    }

        //    // line 1569

        //    // SPECIFY DATA FORMAT OF WEATHER DATA (HOURLY OR DAILY), WHETHER
        //    // THERE WILL BE ANY INPUT FOR A SINK TERM IN THE SOIL

        //    // parse the first line
        //    t = ParseLine(s);
        //    if (iversion == 2)
        //    {
        //        mtstep = int.Parse(t[0]);
        //        _inph2o = int.Parse(t[1]);
        //        mwatrxt = int.Parse(t[2]);
        //    }
        //    else
        //    {
        //        mtstep = int.Parse(t[0]);
        //        iflagsi = int.Parse(t[1]);
        //        _inph2o = int.Parse(t[2]);
        //        mwatrxt = int.Parse(t[3]);
        //    }

        //    // the next four lines specify input files. open stream readers for these.
        //    StreamReader streamReader;
        //    if (!OpenStreamReader(Path.Combine(wd, inputFileReader.ReadLine()), out streamReader))
        //        return false;
        //    _inputReaders[InputFile.SiteDescriptionData] = streamReader;

        //    if (!OpenStreamReader(Path.Combine(wd, inputFileReader.ReadLine()), out streamReader))
        //        return false;
        //    _inputReaders[InputFile.WeatherData] = streamReader;

        //    if (!OpenStreamReader(Path.Combine(wd, inputFileReader.ReadLine()), out streamReader))
        //        return false;
        //    _inputReaders[InputFile.MoistureProfiles] = streamReader;

        //    if (!OpenStreamReader(Path.Combine(wd, inputFileReader.ReadLine()), out streamReader))
        //        return false;
        //    _inputReaders[InputFile.TemperatureProfiles] = streamReader;

        //    // OPEN WATER EXTRACTION FILE IF SPECIFIED
        //    if (mwatrxt > 0)
        //    {
        //        if (!OpenStreamReader(Path.Combine(wd, inputFileReader.ReadLine()), out streamReader))
        //            return false;
        //        _inputReaders[InputFile.WaterExtraction] = streamReader;
        //    }

        //    // **** ALLOW USER TO SPECIFY WHICH OUTPUT FILES ARE DESIRED
        //    var outLvls = ParseLine(inputFileReader.ReadLine()).Select(x => int.Parse(x)).ToList();
        //    if (ioutput == 2)
        //    {
        //        var k = 0;
        //        for (var i = 1; i <= 4; ++i)
        //            lvlout[i] = outLvls[k++];

        //        lvlout[6] = outLvls[k++];

        //        for (var i = 10; i <= 13; ++i)
        //            lvlout[i] = outLvls[k++];

        //        for (var i = 15; i <= 17; ++i)
        //            lvlout[i] = outLvls[k++];

        //        lvlout[20] = outLvls[k++];
        //    }
        //    else
        //    {
        //        var k = 0;
        //        lvlout[1] = outLvls[k++];

        //        for (var i = 3; i <= 17; ++i)
        //            lvlout[i] = outLvls[k++];

        //        lvlout[2] = outLvls[k++];

        //        for (var i = 18; i <= 20; ++i)
        //            lvlout[i] = outLvls[k++];
        //    }

        //    // get output file StreamWriters
        //    var outputFileNames = new Dictionary<OutputFile, string>();
        //    string ofile;

        //    // 1629

        //    // GENERAL OUTPUT FILE
        //    outputFileNames[OutputFile.General] = inputFileReader.ReadLine();

        //    // PROFILES FILE
        //    if (ioutput == 2)
        //    {
        //        ofile = inputFileReader.ReadLine();
        //        if (lvlout[2] > 0)
        //            outputFileNames[OutputFile.Properties] = ofile;
        //    }

        //    // SOIL TEMPERATURE FILE
        //    ofile = inputFileReader.ReadLine();
        //    if (lvlout[3] > 0)
        //        outputFileNames[OutputFile.SoilTemperature] = ofile;

        //    // SOIL TOTAL WATER CONTENT FILE
        //    ofile = inputFileReader.ReadLine();
        //    if (lvlout[4] > 0)
        //        outputFileNames[OutputFile.SoilTotalWaterContent] = ofile;

        //    // SOIL LIQUID WATER CONTENT FILE
        //    ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
        //    if (lvlout[5] > 0)
        //        outputFileNames[OutputFile.SoilLiquidWaterContent] = ofile;

        //    // SOIL MATRIC POTENTIAL FILE
        //    ofile = inputFileReader.ReadLine();
        //    if (lvlout[6] > 0)
        //        outputFileNames[OutputFile.SoilMatricPotential] = ofile;

        //    // CANOPY AIR AND LEAF TEMPERATURE
        //    ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
        //    if (lvlout[7] > 0)
        //        outputFileNames[OutputFile.CanopyAirAndLeafTemperature] = ofile;

        //    // CANOPY VAPOR PRESSURE OR RH
        //    ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
        //    if (Math.Abs(lvlout[8]) > 0)
        //        outputFileNames[OutputFile.CanopyVaporPressureOrRh] = ofile;

        //    // SNOW TEMPERATURE BY DEPTH
        //    ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
        //    if (lvlout[9] > 0)
        //        outputFileNames[OutputFile.SnowTemperatureByDepth] = ofile;

        //    // SURFACE ENERGY BALANCE
        //    ofile = inputFileReader.ReadLine();
        //    if (lvlout[10] > 0)
        //        outputFileNames[OutputFile.SurfaceEnergyBalance] = ofile;

        //    // WATER BALANCE SUMMARY
        //    ofile = inputFileReader.ReadLine();
        //    if (lvlout[11] > 0)
        //        outputFileNames[OutputFile.WaterBalanceSummary] = ofile;

        //    // WATER FLUX BETWEEN SOIL NODES
        //    ofile = inputFileReader.ReadLine();
        //    if (lvlout[12] > 0)
        //        outputFileNames[OutputFile.WaterFluxBetweenSoilNodes] = ofile;

        //    // ROOT EXTRACTION
        //    ofile = inputFileReader.ReadLine();
        //    if (lvlout[13] > 0)
        //        outputFileNames[OutputFile.RootExtraction] = ofile;

        //    // LATERAL FLUX
        //    ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
        //    if (lvlout[14] > 0)
        //        outputFileNames[OutputFile.LateralFlux] = ofile;

        //    // FROST DEPTH AND ICE CONTENT
        //    ofile = inputFileReader.ReadLine();
        //    if (lvlout[15] > 0)
        //        outputFileNames[OutputFile.FrostDepthAndIceContent] = ofile;

        //    // SALT CONCENTRATION
        //    ofile = inputFileReader.ReadLine();
        //    if (lvlout[16] > 0)
        //        outputFileNames[OutputFile.SaltConcentration] = ofile;

        //    // SOLUTE CONCENTRATION
        //    ofile = inputFileReader.ReadLine();
        //    if (lvlout[17] > 0)
        //        outputFileNames[OutputFile.SoluteConcentration] = ofile;

        //    // PROFILES FILE
        //    if (ioutput != 2)
        //    {
        //        // IF VERSION 2.X, THIS FILE NAME HAS ALREADY BEEN READ
        //        ofile = inputFileReader.ReadLine();
        //        if (lvlout[2] > 0)
        //            outputFileNames[OutputFile.Properties] = ofile;
        //    }

        //    // EXTRA OUTPUT 1
        //    ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
        //    if (lvlout[18] > 0)
        //        outputFileNames[OutputFile.ExtraOutput1] = ofile;

        //    // EXTRA OUTPUT 2
        //    ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
        //    if (lvlout[19] > 0)
        //        outputFileNames[OutputFile.ExtraOutput2] = ofile;

        //    // if output files already exist, query user to overwrite
        //    var wd1 = wd;
        //    var existingFiles = outputFileNames.Values.Where(x => File.Exists(Path.Combine(wd1, x))).ToList();
        //    if (existingFiles.Any())
        //    {
        //        Console.WriteLine("The following output files already exist:");
        //        existingFiles.ForEach(x => Console.WriteLine(x));
        //        Console.WriteLine();
        //        Console.WriteLine("DO YOU WISH TO WRITE OVER THESE FILES? (Y/N):");
        //        var a = Console.ReadLine() ?? string.Empty;

        //        if (!a.Equals("y", StringComparison.OrdinalIgnoreCase))
        //        {
        //            Console.WriteLine("Rename above files or change output filenames in");
        //            Console.WriteLine("the file containing the list of input/output files.");
        //            return false;
        //        }
        //    }

        //    // get stream writers for the output files
        //    foreach (var kvp in outputFileNames)
        //        _outputWriters[kvp.Key] = new StreamWriter(Path.Combine(wd, kvp.Value));

        //    Console.WriteLine();
        //    Console.WriteLine();
        //    Console.WriteLine("Simulation in progress . . .");
        //    Console.WriteLine();
        //    return true;
        //}

        public static bool GlobalInitialization(string inputFilePath, out string errorMessage)
        {
            errorMessage = string.Empty;
            _wd = Path.GetDirectoryName(inputFilePath);

            var inputFileParser = new SimpleFileParser(inputFilePath, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return false;

            bool isMissing;
            string sval;
            int ival;
            double dval;

            // ModelSelectionFlags
            if (!inputFileParser.TryParse("StomatalResistance", out _istomate, out errorMessage, 1, 2))
                return false;

            if (!inputFileParser.TryParse("BoundaryTemperatureCondition", out _itmpbc, out errorMessage, 0, 1))
                return false;

            if (!inputFileParser.TryParse("EquationForWaterReleaseCurve", out Slparm.Iwrc, out errorMessage, 1, 4))
                return false;

            if (!inputFileParser.TryParse("ErrorTolerance", out _toler, out errorMessage, 1e-6))
                return false;

            // PlantParameters
            if (!inputFileParser.TryParse("CoefficientWaterPotentialDeadPlants", out _canma, out errorMessage))
                return false;

            if (!inputFileParser.TryParse("ExponentWaterPotentialDeadPlants", out _canmb, out errorMessage))
                return false;

            // SnowParameters
            if (!inputFileParser.TryParse("SnowBasedOn", out sval, out errorMessage))
                return false;

            if (sval.Equals("AirTemp", StringComparison.OrdinalIgnoreCase))
                _isnotmp = 1;
            else if (sval.Equals("WetBulbTemp", StringComparison.OrdinalIgnoreCase))
                _isnotmp = 2;
            else
            {
                errorMessage = "Cannot parse 'SnowBasedOn' as either 'AirTemp' or 'WetBulbTemp'";
                return false;
            }

            if (!inputFileParser.TryParse("MaxTempSnowfall", out _snotmp, out errorMessage, SimpleFileParser.AbsZero))
                return false;

            if (!inputFileParser.TryParse("WindProfileParameterSnow", out _zmspcm, out errorMessage, 0))
                return false;

            if (!inputFileParser.TryParse("LitterAlbedo", out _albres, out errorMessage, 0.0, 1.0))
                return false;

            if (!inputFileParser.TryParse("VaporTransferResistanceFromLitter", out _rescof, out errorMessage))
                return false;

            if (!inputFileParser.TryParse("WindCoefficientThermalConvection", out _restkb, out errorMessage))
                return false;

            if (!inputFileParser.TryParse("AlbedoDrySoil", out _albdry, out errorMessage, 0.0, 1.0))
                return false;

            if (!inputFileParser.TryParse("ExponentMoistSoil", out _albexp, out errorMessage))
                return false;

            if (!inputFileParser.TryParse("WindSurfaceRoughness", out _zmcm, out errorMessage, 0.0))
                return false;

            if (!inputFileParser.TryParse("HeightInstrumentation", out _height, out errorMessage, 0.0))
                return false;

            if (!inputFileParser.TryParse("MaximumPondingDepth", out _pondcm, out errorMessage, 0.0))
                return false;

            // read logging output flags.  If the logging name exists, set the corresponding _lvlout[] index to 24 for 24 hr output.
            _lvlout = new int[21];

            var logNames = Enum.GetNames(typeof(OutputFile));
            var logLvlouts = Enum.GetValues(typeof(OutputFile)).Cast<int>().ToList();
            for (var i = 0; i < logNames.Length; ++i)
            {
                //string fname;
                //if (!InputFileParser.TryParseOptional(logNames[i], out fname, out errorMessage, out isMissing))
                //    return false;

                //if (!isMissing)
                //{
                //    _outputWriters[OutputFile.ComparisonSimulatedMeasured] = new StreamWriter(Path.Combine(_wd, fname));
                //    _lvlout[logLvlouts[i]] = 24;
                //}

                bool containsToken;
                if (!inputFileParser.TryContainsToken(logNames[i], out containsToken, out errorMessage))
                    return false;

                if (containsToken)
                    _lvlout[logLvlouts[i]] = 24;
            }

            return true;
        }

        // not used: todo: delete
        public bool GlobalInitialization(string inputFileName)
        {
            HasGlobalSetup = true;

            //_mwatrxt = 0;
            //_lvlout = new int[21];
            _outputWriters = new Dictionary<ShawDamm.OutputFile, StreamWriter>(); // dictionary of stream writers for various kinds of output
            _inputReaders = new Dictionary<ShawDamm.InputFile, StreamReader>();  // dictionary of input data

            SoilMoistureFile = SoilTemperatureFile = null;

            List<string> t;

            //Console.WriteLine(">>>>>>> Simultaneous Heat And Water (SHAW) Model <<<<<<<");
            //Console.WriteLine("                    Version 3.0.1");
            //Console.WriteLine("Enter the file path containing the list of input/output files:");

            //var inputFileName = Console.ReadLine() ?? string.Empty;
            StreamReader inputFileReader;

            if (!OpenStreamReader(inputFileName, out inputFileReader))
            {
                return false;
            }

            _wd = Path.GetDirectoryName(inputFileName);

            // READ FIRST LINE WITH ALPHAMERIC TO SEE IF IT IS AN OLDER VERSION
            var ioutput = 2;
            var s = inputFileReader.ReadLine();
            for (var i = 0; i < s.Length; ++i)
            {
                // FIND 'SHAW' TEXT
                if (s[i] == 's' || s[i] == 'S')
                {
                    // START OF SHAW VERSION -- FIND VERSION
                    var js = 0;
                    for (var j = i + 1; j < s.Length; ++j)
                    {
                        if (s[j] == '2')
                        {
                            _iversion = 2;
                            if (s.Substring(j + 1).Contains("8"))   // OUTPUT FILE SELECTION FOLLOWS VERSION 3
                                ioutput = 3;
                            js = j;
                            break;
                        }

                        if (s[j] == '3')
                        {
                            _iversion = 3;
                            ioutput = 3;
                            js = j;
                            break;
                        }
                    }

                    // CHECK IF THIS IS AN SI VERSION FOR STRICTLY SI UNITS
                    if (s.Substring(js + 1).IndexOf("SI", StringComparison.OrdinalIgnoreCase) >= 0)
                        _iflagsi = 1;

                    // get next line
                    s = inputFileReader.ReadLine();
                    break;
                }

                if (s[i] == '0' || s[i] == '1' || s[i] == '2')
                {
                    // NO 'SHAW' TEXT - THIS IS FIRST LINE OF VERSION 2.x
                    _iversion = 2;
                    _iflagsi = 0;
                    break;
                }
            }

            if (_iversion == 0)
            {
                Console.WriteLine("COULD NOT DETERMINE THE INPUT FORMAT VERSION");
                Console.WriteLine("FROM FILE CONTAINING LIST OF INPUT/OUTPUT FILES");
                Console.WriteLine($" ===> Check the input file:{inputFileName}");
                return false;
            }

            // line 1569

            // SPECIFY DATA FORMAT OF WEATHER DATA (HOURLY OR DAILY), WHETHER
            // THERE WILL BE ANY INPUT FOR A SINK TERM IN THE SOIL

            // parse the first line
            t = ParseLine(s);
            if (_iversion == 2)
            {
                _mtstep = int.Parse(t[0]);
                _inph2o = int.Parse(t[1]);
                _mwatrxt = int.Parse(t[2]);
            }
            else
            {
                //_mtstep = int.Parse(t[0]);
                //_iflagsi = int.Parse(t[1]);
                //_inph2o = int.Parse(t[2]);
                //_mwatrxt = int.Parse(t[3]);
            }

            // the next four lines specify input files. open stream readers for these.
            StreamReader streamReader;

            if (!OpenStreamReader(Path.Combine(_wd, inputFileReader.ReadLine()), out streamReader))
                return false;
            _inputReaders[ShawDamm.InputFile.SiteDescriptionData] = streamReader;

            WeatherFile = Path.Combine(_wd, inputFileReader.ReadLine());
            //if (!OpenStreamReader(WeatherFile, out streamReader))
            //    return false;
            //_inputReaders[ShawDamm.InputFile.WeatherData] = streamReader;

            SoilMoistureFile = Path.Combine(_wd, inputFileReader.ReadLine());
            //if (!OpenStreamReader(SoilMoistureFile, out streamReader))
            //    return false;
            //_inputReaders[ShawDamm.InputFile.MoistureProfiles] = streamReader;

            SoilTemperatureFile = Path.Combine(_wd, inputFileReader.ReadLine());
            //if (!OpenStreamReader(SoilTemperatureFile, out streamReader))
            //    return false;
            //_inputReaders[ShawDamm.InputFile.TemperatureProfiles] = streamReader;

            // OPEN WATER EXTRACTION FILE IF SPECIFIED
            if (_mwatrxt > 0)
            {
                if (!OpenStreamReader(Path.Combine(_wd, inputFileReader.ReadLine()), out streamReader))
                    return false;
                _inputReaders[ShawDamm.InputFile.WaterExtraction] = streamReader;
            }

            // **** ALLOW USER TO SPECIFY WHICH OUTPUT FILES ARE DESIRED
            var outLvls = ParseLine(inputFileReader.ReadLine()).Select(x => int.Parse(x)).ToList();
            //if (ioutput == 2)
            //{
            //    var k = 0;
            //    for (var i = 1; i <= 4; ++i)
            //        _lvlout[i] = outLvls[k++];

            //    _lvlout[6] = outLvls[k++];

            //    for (var i = 10; i <= 13; ++i)
            //        _lvlout[i] = outLvls[k++];

            //    for (var i = 15; i <= 17; ++i)
            //        _lvlout[i] = outLvls[k++];

            //    _lvlout[20] = outLvls[k++];
            //}
            //else
            //{
            //    var k = 0;
            //    _lvlout[1] = outLvls[k++];

            //    for (var i = 3; i <= 17; ++i)
            //        _lvlout[i] = outLvls[k++];

            //    _lvlout[2] = outLvls[k++];

            //    for (var i = 18; i <= 20; ++i)
            //        _lvlout[i] = outLvls[k++];
            //}

            // get output file StreamWriters
            var outputFileNames = new Dictionary<ShawDamm.OutputFile, string>();
            string ofile;

            // 1629

            // GENERAL OUTPUT FILE
            outputFileNames[ShawDamm.OutputFile.EntireProfile] = inputFileReader.ReadLine();

            // PROFILES FILE
            if (ioutput == 2)
            {
                ofile = inputFileReader.ReadLine();
                if (_lvlout[2] > 0)
                    outputFileNames[ShawDamm.OutputFile.ComparisonSimulatedMeasured] = ofile;
            }

            // SOIL TEMPERATURE FILE
            ofile = inputFileReader.ReadLine();
            if (_lvlout[3] > 0)
                outputFileNames[ShawDamm.OutputFile.SoilTemperature] = ofile;

            // SOIL TOTAL WATER CONTENT FILE
            ofile = inputFileReader.ReadLine();
            if (_lvlout[4] > 0)
                outputFileNames[ShawDamm.OutputFile.SoilTotalWaterContent] = ofile;

            // SOIL LIQUID WATER CONTENT FILE
            ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
            if (_lvlout[5] > 0)
                outputFileNames[ShawDamm.OutputFile.SoilLiquidWaterContent] = ofile;

            // SOIL MATRIC POTENTIAL FILE
            ofile = inputFileReader.ReadLine();
            if (_lvlout[6] > 0)
                outputFileNames[ShawDamm.OutputFile.SoilMatricPotential] = ofile;

            // CANOPY AIR AND LEAF TEMPERATURE
            ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
            if (_lvlout[7] > 0)
                outputFileNames[ShawDamm.OutputFile.PlantCanopyTemperature] = ofile;

            // CANOPY VAPOR PRESSURE OR RH
            ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
            if (Math.Abs(_lvlout[8]) > 0)
                outputFileNames[ShawDamm.OutputFile.PlantCanopyHumidity] = ofile;

            // SNOW TEMPERATURE BY DEPTH
            ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
            if (_lvlout[9] > 0)
                outputFileNames[ShawDamm.OutputFile.SnowTemperature] = ofile;

            // SURFACE ENERGY BALANCE
            ofile = inputFileReader.ReadLine();
            if (_lvlout[10] > 0)
                outputFileNames[ShawDamm.OutputFile.SurfaceEnergyBalance] = ofile;

            // WATER BALANCE SUMMARY
            ofile = inputFileReader.ReadLine();
            if (_lvlout[11] > 0)
                outputFileNames[ShawDamm.OutputFile.WaterBalance] = ofile;

            // WATER FLUX BETWEEN SOIL NODES
            ofile = inputFileReader.ReadLine();
            if (_lvlout[12] > 0)
                outputFileNames[ShawDamm.OutputFile.WaterFlowNodes] = ofile;

            // ROOT EXTRACTION
            ofile = inputFileReader.ReadLine();
            if (_lvlout[13] > 0)
                outputFileNames[ShawDamm.OutputFile.WaterExtractedPlants] = ofile;

            // LATERAL FLUX
            ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
            if (_lvlout[14] > 0)
                outputFileNames[ShawDamm.OutputFile.LateralFlow] = ofile;

            // FROST DEPTH AND ICE CONTENT
            ofile = inputFileReader.ReadLine();
            if (_lvlout[15] > 0)
                outputFileNames[ShawDamm.OutputFile.SnowFrostDepth] = ofile;

            // SALT CONCENTRATION
            ofile = inputFileReader.ReadLine();
            if (_lvlout[16] > 0)
                outputFileNames[ShawDamm.OutputFile.TotalSaltConc] = ofile;

            // SOLUTE CONCENTRATION
            ofile = inputFileReader.ReadLine();
            if (_lvlout[17] > 0)
                outputFileNames[ShawDamm.OutputFile.SoilSolutionConc] = ofile;

            // PROFILES FILE
            if (ioutput != 2)
            {
                // IF VERSION 2.X, THIS FILE NAME HAS ALREADY BEEN READ
                ofile = inputFileReader.ReadLine();
                if (_lvlout[2] > 0)
                    outputFileNames[ShawDamm.OutputFile.ComparisonSimulatedMeasured] = ofile;
            }

            //// EXTRA OUTPUT 1
            //ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
            //if (lvlout[18] > 0)
            //    outputFileNames[ShawDamm.OutputFile.ExtraOutput1] = ofile;

            //// EXTRA OUTPUT 2
            //ofile = ioutput != 2 ? inputFileReader.ReadLine() : string.Empty;
            //if (lvlout[19] > 0)
            //    outputFileNames[ShawDamm.OutputFile.ExtraOutput2] = ofile;

            //// if output files already exist, query user to overwrite
            //var wd1 = _wd;
            //var existingFiles = outputFileNames.Values.Where(x => File.Exists(Path.Combine(wd1, x))).ToList();
            //if (existingFiles.Any())
            //{
            //    Console.WriteLine("The following output files already exist:");
            //    existingFiles.ForEach(x => Console.WriteLine(x));
            //    Console.WriteLine();
            //    Console.WriteLine("DO YOU WISH TO WRITE OVER THESE FILES? (Y/N):");
            //    var a = Console.ReadLine() ?? string.Empty;

            //    if (!a.Equals("y", StringComparison.OrdinalIgnoreCase))
            //    {
            //        Console.WriteLine("Rename above files or change output filenames in");
            //        Console.WriteLine("the file containing the list of input/output files.");
            //        return false;
            //    }
            //}

            //foreach (var kvp in outputFileNames)
            //    _outputWriters[kvp.Key] = new StreamWriter(Path.Combine(_wd, kvp.Value));

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Simulation in progress . . .");
            Console.WriteLine();
            return true;
        }

        private bool InputSiteFile(string wd, int iversion, ref int nc, int iflagsi,
            ref int mzcinp, ref int mpltgro, ref int nrchang, ref double wcmax, double[] zc, double[] wcandt,
            double[] zsp, double[] dzsp, double[] rhosp, double[] tspdt, double[] dlwdt, int[] icespt,
            double[] gmcdt, ref double gmcmax, double[] zs,
            double[][] saltdt, double[] dgrade,
            double[] sltdif, double[] asalt, double[] disper, ref double zmsrf, ref double zhsrf, ref double zersrf, ref double zmsp, ref double zhsp,
            ref double pondmx, ref double wdt, ref double alatud, ref double slope,
            ref double aspect, double altdeg, double altmin, double slp, double aspec, double elev, 
            Dictionary<string, string> thuFileData, Dictionary<string, Dictionary<string, string>> plantFileData, Dictionary<string, Dictionary<string, string>> soilFileData, out string errorMessage)
        {
            errorMessage = string.Empty;
            //
            //     THIS SUBROUTINE IS USED TO INPUT ALL GENERAL INFORMATION AND
            //     INITIAL CONDITIONS.
            //
            //***********************************************************************
            //
            //
            //
            //
            var heights = new double[11];

            var cf = 0.0;
            var textot = 0.0;
            var satkl = 0.0;
            var nwrc = 0;
            var tdepth = 0.0;
            var zhspcm = 0.0;
            var doi = 0.0;
            var ninfil = 0;
            var doj = 0.0;
            var zhcm = 0.0;
            var mcanflg = 0;
            var wcan = 0.0;
            var haflif = 0.0;
            var satcon = 0.0;
            var dummy = 0.0;

            //StreamReader streamReader;

            //if (!OpenStreamReader(Path.Combine(_wd, siteFileName), out streamReader))
            //    return false;
            //_inputReaders[ShawDamm.InputFile.SiteDescriptionData] = streamReader;

            List<string> t = null;
            //var generalOut = _outputWriters[OutputFile.EntireProfile];

            // READ (11,100) TITLE
            //var title = ReadNextLine(InputFile.SiteDescriptionData).Trim();

            // line B
            // READ (11,*) JSTART,HRSTAR,YRSTAR,JEND,YREND
            //t = ParseNextLine(InputFile.SiteDescriptionData);
            /*jstart = int.Parse(t[0]); hrstar = int.Parse(t[1]); yrstar = int.Parse(t[2]); jend = int.Parse(t[3]); yrend = int.Parse(t[4]);*/

            // line C
            // READ (11,*) ALTDEG,ALTMIN,SLP,ASPEC,HRNOON,ELEV
            //t = ParseNextLine(InputFile.SiteDescriptionData);
            //var altdeg = double.Parse(t[0]); var altmin = double.Parse(t[1]); var slp = double.Parse(t[2]); var aspec = double.Parse(t[3]); hrnoon = double.Parse(t[4]); var elev = double.Parse(t[5]);

            // line D
            // READ (11,*) NPLANT,NSP,NR,NS,NSALT,TOLER,NHRPDT,(LEVEL(I), I=1,6)
            //t = ParseNextLine(InputFile.SiteDescriptionData);
            // nplant = int.Parse(t[0]); _nsp = int.Parse(t[1]); nr = int.Parse(t[2]); */ _ns = int.Parse(t[3]); // _slparm.Nsalt = int.Parse(t[4]); toler = double.Parse(t[5]);  nhrpdt = int.Parse(t[6]);

            _nsp = 0;
            _slparm.Nsalt = 0;

            if (!SimpleFileParser.TryParseInput("NumberLitterNodes", thuFileData["NumberLitterNodes"], out _nr, out errorMessage, 0))
                return false;

            //for (var i = 1; i <= 6; ++i)
            //    level[i] = int.Parse(t[6 + i]);

            //if (hrstar == 0)
            //{
            //    //        CONVERT TO 24 HOURS FOR EASE IN FINDING INITIAL CONDITIONS
            //    hrstar = 24;
            //    jstart = jstart - 1;
            //}
            _constn.Presur = 101300.0 * Math.Exp(-elev / 8278.0);

            //generalOut.WriteLine(title);
            //generalOut.WriteLine();
            //generalOut.WriteLine($"     SIMULATION BEGINS ON DAY{_jstart,4:D}, HOUR{_hrstar,3:D}, OF {_yrstar,4:D}");
            //generalOut.WriteLine($"     SIMULATION ENDS   ON DAY{_jend,4:D}, HOUR 24  OF {_yrend,4:D}");

            // ISSUE STATEMENT FOR INPUT VERSION AND WEATHER FILE FORMAT
            Console.WriteLine();
            if (_lvlout[1] > 0)
                _outputWriters[OutputFile.EntireProfile].WriteLine();

            if (iversion == 2)
            {
                Console.WriteLine("     Input format follows version 2.x");
                if (_lvlout[1] > 0)
                    _outputWriters[OutputFile.EntireProfile].WriteLine("     INPUT FORMAT FOLLOWS VERSION 2.x");
            }
            else
            {
                Console.WriteLine("     Input format follows version 3.x");
                if (_lvlout[1] > 0)
                    _outputWriters[OutputFile.EntireProfile].WriteLine("     INPUT FORMAT FOLLOWS VERSION 3.x");
            }
            if (iflagsi == 0)
            {
                Console.WriteLine("     with weather file in mixed English/SI units");
                if (_lvlout[1] > 0)
                    _outputWriters[OutputFile.EntireProfile].WriteLine("     WITH WEATHER FILE IN MIXED ENGLISH/SI UNITS");
            }
            else
            {
                Console.WriteLine("     with weather file in metric units only");
                if (_lvlout[1] > 0)
                    _outputWriters[OutputFile.EntireProfile].WriteLine("     WITH WEATHER FILE IN METRIC UNITS ONLY");
            }

            if (_lvlout[1] > 0)
            {
                var generalOut = _outputWriters[OutputFile.EntireProfile];
                generalOut.WriteLine();
                generalOut.WriteLine();
                generalOut.WriteLine(" GENERAL SITE DESCRIPTION");
                generalOut.WriteLine();
                generalOut.WriteLine($"     LATITUDE  : {altdeg,5:F1} DEGREES {altmin,5:F2} MINUTES");
                generalOut.WriteLine($"     SLOPE     : {slp,5:F2} %");
                generalOut.WriteLine($"     ASPECT    : {aspec,5:F1} DEGREES FROM NORTH");
                generalOut.WriteLine($"     SOLAR NOON: {_hrnoon,5:F2} HOURS");
                generalOut.WriteLine($"     ELEVATION : {elev,5:F} M");
                generalOut.WriteLine($"     PRESSURE  : {_constn.Presur / 1000.0,5:F1} KPA");
            }

            if (altdeg < 0.0 && altmin > 0.0) altmin = -altmin;
            alatud = (altdeg + altmin / 60.0) * 3.14159 / 180.0;
            slope = Math.Atan(slp / 100.0);
            aspect = aspec * 3.14159 / 180.0;
            if (_nhrpdt == 1)
            {
                wdt = 0.6;
            }
            else
            {
                wdt = 1.0;
            }
            //

            //      CHECK IF NHRPDT IS VALID
            if (24 % _nhrpdt != 0)
            {
                Console.WriteLine();
                Console.WriteLine(" ************************************************");
                Console.WriteLine(" PARAMETER FOR THE NUMBER OF HOURS PER TIME STEP");
                Console.WriteLine(" (NHRPDT; LINE D OF INPUT FILE) IS NOT VALID. ");
                Console.WriteLine(" NHRPDT MUST BE EVENLY DIVISIBLE INTO 24 HOURS.");
                Console.WriteLine(" ************************************************");
                if (_lvlout[1] > 0)
                {
                    var generalOut = _outputWriters[OutputFile.EntireProfile];
                    generalOut.WriteLine();
                    generalOut.WriteLine();
                    generalOut.WriteLine(" ************************************************");
                    generalOut.WriteLine(" THE PARAMETER FOR THE NUMBER OF HOURS PER TIME");
                    generalOut.WriteLine(" (NHRPDT; LINE D OF INPUT FILE) IS NOT VALID. ");
                    generalOut.WriteLine(" NHRPDT MUST BE EVENLY DIVISIBLE INTO 24 HOURS.");
                    generalOut.WriteLine(" ************************************************");
                }
                return false;
            }

            //      CHECK IF HRSTAR IS COMPATIBLE WITH NHRPDT
            if (_hrstar % _nhrpdt != 0)
            {
                Console.WriteLine();
                Console.WriteLine(" **************************************************");
                Console.WriteLine(" THE PARAMETER FOR THE NUMBER OF HOURS PER TIME");
                Console.WriteLine(" STEP (NHRPDT; LINE D OF INPUT FILE) IS NOT");
                Console.WriteLine(" COMPATIBLE WITH THE BEGINNING HOUR FOR THE");
                Console.WriteLine(" SIMULATION (HRSTAR; LINE B OF INPUT FILE). ");
                Console.WriteLine(" HRSTAR MUST BE ZERO OR EVENLY DIVISIBLE BY NHRPDT.");
                Console.WriteLine(" **************************************************");
                if (_lvlout[1] > 0)
                {
                    var generalOut = _outputWriters[OutputFile.EntireProfile];
                    generalOut.WriteLine();
                    generalOut.WriteLine();
                    generalOut.WriteLine(" **************************************************");
                    generalOut.WriteLine(" THE PARAMETER FOR THE NUMBER OF HOURS PER TIME");
                    generalOut.WriteLine(" STEP (NHRPDT; LINE D OF INPUT FILE) IS NOT");
                    generalOut.WriteLine(" COMPATIBLE WITH THE BEGINNING HOUR FOR THE");
                    generalOut.WriteLine(" SIMULATION (HRSTAR; LINE B OF INPUT FILE).");
                    generalOut.WriteLine(" HRSTAR MUST BE ZERO OR EVENLY DIVISIBLE BY NHRPDT.");
                    generalOut.WriteLine(" **************************************************");
                }
                return false;
            }

            // **** INPUT AERODYNAMIC ATMOSPHERIC AND SURFACE PROPERTIES
            // READ (11,*) ZMCM,HEIGHT,PONDCM
            //t = ParseNextLine(InputFile.SiteDescriptionData);
            //var zmcm = double.Parse(t[0]); height = double.Parse(t[1]); var pondcm = double.Parse(t[2]);

            zmsrf = _zmcm / 100.0;
            zhsrf = 0.2 * zmsrf;
            zhcm = zhsrf * 100.0;
            pondmx = _pondcm / 100.0;
            zersrf = 0.0;

            if (_lvlout[1] > 0)
            {
                var generalOut = _outputWriters[OutputFile.EntireProfile];
                generalOut.WriteLine();
                generalOut.WriteLine();
                generalOut.WriteLine(" WIND PROFILE AND SURFACE PARAMETERS");
                generalOut.WriteLine();
                generalOut.WriteLine($"     ZM :{_zmcm,5:F2} CM FOR SOIL OR RESIDUE SURFACE ");
                generalOut.WriteLine($"     ZH :{zhcm,5:F2} CM FOR SOIL OR RESIDUE SURFACE ");
                generalOut.WriteLine($"     ZERO PLANE DISPLACEMENT :{zersrf,5:F2} M");
                generalOut.WriteLine($"     HEIGHT OF INSTRUMENTATION :{_height,5:F1} M");
                generalOut.WriteLine($"     MAXIMUM DEPTH OF PONDING :{_pondcm,4:F1} CM");
                generalOut.WriteLine();
                generalOut.WriteLine();
                generalOut.WriteLine(" ATMOSPHERIC CONDITIONS");
                generalOut.WriteLine();
                generalOut.WriteLine($"     MAXIMUM CLEAR-SKY TRANSMISSIVITY :{Swrcoe.Difatm,5:F2}");
                generalOut.WriteLine($"     CLEAR-SKY LONG-WAVE EMISSIVITY PARAMETERS :{Lwrcof.Ematm1,6:F3}{Lwrcof.Ematm2,15:E3}");
            }

            // set plant inputs
            var jj = 1;
            while (true)
            {
                var key = $"VegetationType{jj}";
                if (!thuFileData.TryGetValue(key, out var vegType) || string.IsNullOrEmpty(vegType))
                    break;

                if (!plantFileData.TryGetValue(vegType, out var plantData))
                {
                    errorMessage = $"Plant data for {key} '{vegType}' not found";
                    return false;
                }

                if (!SimpleFileParser.TryParseInput($"{vegType} : LeafOnDay", plantData["LeafOnDay"], out _pltLeafOn[jj], out errorMessage, 1, 366))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : LeafOffDay", plantData["LeafOffDay"], out _pltLeafOff[jj], out errorMessage, 1, 366))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : DryBiomass", plantData["DryBiomass"], out _pltwgt[jj], out errorMessage, 0.0))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : LAI", plantData["LAI"], out _pltlaiOn[jj], out errorMessage, 0.0))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : Height", plantData["Height"], out _plthgt[jj], out errorMessage, 0.0))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : RootingDepth", plantData["RootingDepth"], out _rootdp[jj], out errorMessage, 0.0))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : CharacteristicDimension", plantData["CharacteristicDimension"], out _dchar[jj], out errorMessage, 0.0))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : ClumpingParameter", plantData["ClumpingParameter"], out _clumpng[jj], out errorMessage, 0.0, 1.0, false))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : PlantType", plantData["PlantType"], out _itype[jj], out errorMessage, 0, 1))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : MaxPrecipInterceptPerLAI", plantData["MaxPrecipInterceptPerLAI"], out _pintrcp[jj], out errorMessage, 0.0))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : LeafAngleVertical", plantData["LeafAngleVertical"], out _xangle[jj], out errorMessage, 0.0, 5.0))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : Albedo", plantData["Albedo"], out _canalb[jj], out errorMessage, 0.0, 1.0))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : MinTranspirationTemp", plantData["MinTranspirationTemp"], out _tccrit[jj], out errorMessage, SimpleFileParser.AbsZero))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : MinStomatalResistance", plantData["MinStomatalResistance"], out _rstom0[jj], out errorMessage, 0.0))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : StomatalResistanceExponent", plantData["StomatalResistanceExponent"], out _rstexp[jj], out errorMessage))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : CriticalLeafWaterPotential", plantData["CriticalLeafWaterPotential"], out _pleaf0[jj], out errorMessage))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : LeafResistance", plantData["LeafResistance"], out _rleaf0[jj], out errorMessage, 0.0))
                    return false;

                if (!SimpleFileParser.TryParseInput($"{vegType} : RootResistance", plantData["RootResistance"], out _rroot0[jj], out errorMessage, 0.0))
                    return false;

                if (_istomate == 2)
                {
                    //           Option for Jarvis-Stewart stomatal resistance model
                    if (!SimpleFileParser.TryParseInput($"{vegType} : SolarRadiationInfluence", plantData["SolarRadiationInfluence"], out _stomate[jj][1], out errorMessage, 0.0))
                        return false;

                    if (!SimpleFileParser.TryParseInput($"{vegType} : TranspirationLowerLimit", plantData["TranspirationLowerLimit"], out _stomate[jj][2], out errorMessage))
                        return false;

                    if (!SimpleFileParser.TryParseInput($"{vegType} : TranspirationUpperLimit", plantData["TranspirationUpperLimit"], out _stomate[jj][3], out errorMessage, SimpleFileParser.AbsZero))
                        return false;

                    if (!SimpleFileParser.TryParseInput($"{vegType} : TranspirationOptimum", plantData["TranspirationOptimum"], out _stomate[jj][4], out errorMessage, _stomate[jj][2], _stomate[jj][3]))
                        return false;

                    if (!SimpleFileParser.TryParseInput($"{vegType} : VPDMaximumReduction", plantData["VPDMaximumReduction"], out _stomate[jj][5], out errorMessage, 0.0, 1.0, false))
                        return false;

                    if (!SimpleFileParser.TryParseInput($"{vegType} : StomatalConductanceCoefficient", plantData["StomatalConductanceCoefficient"], out _stomate[jj][6], out errorMessage, 0.0, 1.0, false))
                        return false;
                }

                ++jj;
            }
            _nplant = jj - 1;

            if (_nplant > 0)
            {
                if (iversion == 2)
                {
                    //// READ (11,*) MCANFLG,CANMA,CANMB,WCAN
                    //t = ParseNextLine(InputFile.SiteDescriptionData);
                    //mcanflg = int.Parse(t[0]); canma = double.Parse(t[1]); canmb = double.Parse(t[2]); wcan = double.Parse(t[3]);
                    //istomate = 1;
                }
                else
                {
                    // READ (11,*) MCANFLG,ISTOMATE,CANMA,CANMB,WCAN
                    //t = ParseNextLine(InputFile.SiteDescriptionData);
                    // mcanflg = int.Parse(t[0]);  istomate = int.Parse(t[1]); canma = double.Parse(t[2]); canmb = double.Parse(t[3]);  wcan = double.Parse(t[4]);
                }
                //
                //        DETERMINE MAXIMUM WATER CONTENT OF STANDING DEAD (AT 99.9% RH)
                var double999 = 0.999;
                var double0 = 0.0;
                Canhum(2, ref double999, ref dummy, ref wcmax, ref double0, ref _canma, ref _canmb);
                if (wcan > wcmax) wcan = wcmax;
                //        SET WCANTDT ARRAY TO THE INPUT DEAD PLANT WATER CONTENT
                for (var i = 1; i < wcandt.Length; ++i)
                    wcandt[i] = wcan;
                //
                if (mcanflg == 0 || mcanflg == 2)
                {
                    //           NO INPUT FILE FOR PLANT GROWTH
                    mpltgro = 0;
                }
                else
                {
                    //           USER INPUTS FILE FOR PLANT GROWTH
                    mpltgro = 1;
                }
                if (mcanflg >= 2)
                {
                    //           SET FLAG FOR USER INPUT OF NODE SPACING
                    //           INITIALIZE MZCINP; WILL LATER SET TO NUMBER OF CANOPY NODES
                    if (mcanflg == 2)
                    {
                        mzcinp = 1;
                    }
                    else
                    {
                        //             NEGATIVE VALUE FOR MZCINP WILL INDICATE THAT THE MODEL
                        //             WILL NEED TO ASSIGN PROPERTIES (LAI, ETC) TO LAYERS
                        mzcinp = -1;
                    }
                }
                else
                {
                    //           MODEL WILL GENERATE SPACING OF NODES WITHIN THE CANOPY
                    mzcinp = 0;
                }

                for (var j = 1; j <= _nplant; ++j)
                {
                    if (iversion == 2)
                    {
                        // READ (11,*) ITYPE(J),XANGLE(J),CANALB(J),TCCRIT(J),RSTOM0(J),RSTEXP(J),PLEAF0(J),RLEAF0(J),RROOT0(J)
                        t = ParseNextLine(InputFile.SiteDescriptionData);
                        _itype[j] = int.Parse(t[0]); _xangle[j] = double.Parse(t[1]); _canalb[j] = double.Parse(t[2]); _tccrit[j] = double.Parse(t[3]); _rstom0[j] = double.Parse(t[4]); _rstexp[j] = double.Parse(t[5]); _pleaf0[j] = double.Parse(t[6]); _rleaf0[j] = double.Parse(t[7]); _rroot0[j] = double.Parse(t[8]);
                        //            SET MAX RAINFALL INTERCEPTION TO 1 MM PER LAI
                        _pintrcp[j] = 0.001;
                    }
                    else
                    {
                        // READ (11,*) ITYPE(J),PINTRCP(J),XANGLE(J),CANALB(J),TCCRIT(J),RSTOM0(J),RSTEXP(J),PLEAF0(J),RLEAF0(J),RROOT0(J)
                        //t = ParseNextLine(InputFile.SiteDescriptionData);
                        //itype[j] = int.Parse(t[0]); pintrcp[j] = double.Parse(t[1]); xangle[j] = double.Parse(t[2]); canalb[j] = double.Parse(t[3]); tccrit[j] = double.Parse(t[4]); rstom0[j] = double.Parse(t[5]); rstexp[j] = double.Parse(t[6]); pleaf0[j] = double.Parse(t[7]); rleaf0[j] = double.Parse(t[8]); rroot0[j] = double.Parse(t[9]);
                        _pintrcp[j] = _pintrcp[j] / 1000.0;
                    }
                label1010:;
                }

                if (_lvlout[1] > 0)
                {
                    var generalOut = _outputWriters[OutputFile.EntireProfile];
                    generalOut.WriteLine();
                    generalOut.WriteLine();
                    generalOut.WriteLine(" CANOPY PARAMETERS");
                    generalOut.WriteLine();
                    generalOut.WriteLine($"     EMISSIVITY OF PLANT MATERIAL :{Lwrcof.Emitc,5:F2}");
                    generalOut.WriteLine($"     MOISTURE PARAMETERS FOR ANY DEAD PLANT MATERIAL  :{_canma,7:F2}{_canmb,5:F2}");
                    generalOut.WriteLine($"     INITIAL MOISTURE CONTENT FOR DEAD PLANT MATERIAL :{wcandt[1],5:F2} KG/KG");
                    generalOut.WriteLine($"     MAXIMUM MOISTURE CONTENT FOR DEAD PLANT MATERIAL :{wcmax,5:F2} KG/KG");
                    generalOut.WriteLine();
                    generalOut.WriteLine($"                                           {string.Concat(Enumerable.Range(1, _nplant).Select(j => $"  PLANT #{j}"))}");

                    generalOut.WriteLine($"     PLANT TYPE (0=DEAD,1=TRANSPIRING)    :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_itype[j],10:D}"))}");
                    generalOut.WriteLine($"     MAX PRECIP INTERCEPTION PER LAI (MM) :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_pintrcp[j] * 1000.0,10:F2}"))}");
                    generalOut.WriteLine($"     LEAF ANGLE (0=VERT,1=RANDOM,INF=HORZ):{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_xangle[j],10:F2}"))}");
                    generalOut.WriteLine($"     ALBEDO                               :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_canalb[j],10:F2}"))}");
                    generalOut.WriteLine($"     MIN. TRANSPIRATION TEMPERATURE (C)   :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_tccrit[j],10:F2}"))}");
                    generalOut.WriteLine($"     MINIMUM STOMATAL RESISTANCE (S/M)    :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_rstom0[j],10:F2}"))}");
                    generalOut.WriteLine($"     STOMATAL RESISTANCE EXPONENT         :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_rstexp[j],10:F2}"))}");
                    generalOut.WriteLine($"     CRITICAL LEAF WATER POTENTIAL (M)    :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_pleaf0[j],10:F2}"))}");
                    generalOut.WriteLine($"     LEAF RESISTANCE (KG/M2-S)            :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_rleaf0[j],10:E2}"))}");
                    generalOut.WriteLine($"     ROOT RESISTANCE (KG/M2-S)            :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_rroot0[j],10:E2}"))}");
                }

                if (_istomate == 2)
                {
                    //           Option for Jarvis-Stewart stomatal resistance model
                    for (var j = 1; j <= _nplant; ++j)
                    {
                        // read(11, *)(stomate(j, k), k = 1, 7);
                        //t = ParseNextLine(InputFile.SiteDescriptionData);
                        //for (var k = 1; k <= 7; ++k)
                        //    stomate[j][k] = double.Parse(t[k - 1]);

                        //             COMPUTE EXPONENT FOR TEMPERATURE MODEL
                        _stomate[j][10] = (_stomate[j][3] - _stomate[j][4]) / (_stomate[j][4] - _stomate[j][2]);
                    }

                    if (_lvlout[1] > 0)
                    {
                        var generalOut = _outputWriters[OutputFile.EntireProfile];
                        generalOut.WriteLine("     STEWART-JARVIS STOMATAL PARAMETERS:");
                        generalOut.WriteLine($"        SOLAR PARAMETER                   :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_stomate[j][1],10:F1}"))}");
                        generalOut.WriteLine($"        LOW TEMPERATURE LIMIT FOR TRANSP. :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_stomate[j][2],10:F1}"))}");
                        generalOut.WriteLine($"        HIGH TEMPERATURE LIMIT FOR TRANSP.:{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_stomate[j][3],10:F1}"))}");
                        generalOut.WriteLine($"        OPTIMAL TEMPERATURE FOR TRANSP.   :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_stomate[j][4],10:F1}"))}");
                        generalOut.WriteLine($"        MAX REDUCTION FOR VPD             :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_stomate[j][5],10:F3}"))}");
                        generalOut.WriteLine($"        VAPOR PRESSURE DEFICIT PARAMETER  :{string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_stomate[j][6],10:F3}"))}");
                    }
                }

                //         CONVERT RLEAF0 AND RROOT0 TO FROM RESISTANCE TO CONDUCTANCE
                //         IF PLANT IS TRANSPIRING
                for (var j = 1; j <= _nplant; ++j)
                {
                    if (_itype[j] > 0)
                    {
                        _rleaf0[j] = 1.0 / _rleaf0[j];
                        _rroot0[j] = 1.0 / _rroot0[j];
                    }
                }

                if (mpltgro > 0)
                {
                    //            OPEN FILES FOR PLANT GROWTH
                    if (_lvlout[1] > 0)
                        _outputWriters[OutputFile.EntireProfile].WriteLine("     INPUT FILES FOR PLANT GROWTH:");
                    _canopyReaders = new StreamReader[_nplant + 1];
                    var fileReadErrors = new List<string>();
                    for (var j = 1; j <= _nplant; ++j)
                    {
                        var file = ReadNextLine(InputFile.SiteDescriptionData);
                        if (_lvlout[1] > 0)
                            _outputWriters[OutputFile.EntireProfile].WriteLine($"          PLANT #{j}: {file}");

                        StreamReader r;
                        if (OpenStreamReader(Path.Combine(wd, file), out r))
                            _canopyReaders[j] = r;
                        else
                            fileReadErrors.Add(file);
                    }

                    if (fileReadErrors.Any())
                    {
                        Console.WriteLine();
                        Console.WriteLine($" THE FOLLOWING INPUT FILE(S) CANNOT BE OPENED: {string.Join(", ", fileReadErrors)}");
                        Console.WriteLine();
                        Console.WriteLine("Check that filenames and paths for the plant growth");
                        Console.WriteLine("files listed in the site characteristics file are correct.");
                        return false;
                    }

                    if (Math.Abs(mzcinp) > 0)
                    {
                        //               USER INPUT OF NODE SPACING WITHIN THE CANOPY
                        // read(11,*)nc
                        nc = int.Parse(ReadNextLine(InputFile.SiteDescriptionData));

                        //               SET MZCINP TO NUMBER OF CANOPY NODES (PRESERVE SIGN)
                        mzcinp = mzcinp * nc;
                        if (mpltgro == 1)
                        {
                            //                 ENTER HEIGHTS ABOVE GROUND FOR DESIRED NODES
                            //read(11, *)(heights[i], i = 1, nc);
                            t = ParseNextLine(InputFile.SiteDescriptionData);
                            for (var k = 1; k <= nc; ++k)
                                heights[k] = double.Parse(t[k - 1]);

                            //                 CONVERT HEIGHTS TO DEPTH FROM TOP OF CANOPY (NODES
                            //                 ABOVE MAX PLANT HEIGHT ARE OK BUT WILL NOT BE USED)
                            for (var i = 1; i <= nc; ++i)
                            {
                                zc[i] = heights[nc] - heights[nc - i + 1];
                            }
                            zc[nc + 1] = heights[nc];
                        }
                    }
                }
                else if (mzcinp == 0)
                {
                    //            READ IN PLANT PARAMETERS (ASSUMED TO BE CONSTANT FOR
                    //            ENTIRE SIMULATION)
                    for (var j = 1; j <= _nplant; ++j)
                    {
                        if (iversion == 2)
                        {
                            // read(11,*)plthgt(j),dchar(j),pltwgt(j),pltlai(j),rootdp(j)
                            t = ParseNextLine(InputFile.SiteDescriptionData);
                            _plthgt[j] = double.Parse(t[0]); _dchar[j] = double.Parse(t[1]); _pltwgt[j] = double.Parse(t[2]); _pltlai[j] = double.Parse(t[3]); _rootdp[j] = double.Parse(t[4]);
                            _clumpng[j] = 1.0;
                        }
                        else
                        {
                            // read(11,*)plthgt(j),dchar(j),clumpng(j),pltwgt(j),pltlai(j),rootdp(j)
                            //t = ParseNextLine(InputFile.SiteDescriptionData);
                            //plthgt[j] = double.Parse(t[0]); dchar[j] = double.Parse(t[1]); clumpng[j] = double.Parse(t[2]); pltwgt[j] = double.Parse(t[3]); pltlai[j] = double.Parse(t[4]); rootdp[j] = double.Parse(t[5]);
                        }
                        //               CONVERT LEAF DIMENSION FROM CM TO METERS
                        _dchar[j] = _dchar[j] / 100.0;
                    }
                }
                else // line 2212
                {
                    if (iversion == 2)
                    {
                        //               USER INPUT OF ROOT DISTRIBUTION - THIS WAS MOVED DOWN IN
                        //               SUBSEQUENT VERSIONS OF MODEL
                        for (var j = 1; j <= _nplant; ++j)
                        {
                            //read(11, *)(rootdn(j, i), i = 1, ns);
                            t = ParseNextLine(InputFile.SiteDescriptionData);
                            for (var k = 1; k <= _ns; ++k)
                                _clayrs.Rootdn[j][k] = double.Parse(t[k - 1]);
                        }
                    }
                    //            USER INPUT OF NODE SPACING WITHIN THE CANOPY
                    // read(11,*)nc
                    t = ParseNextLine(InputFile.SiteDescriptionData);
                    nc = int.Parse(t[0]);
                    //            SET MZCINP TO NUMBER OF DESIRED CANOPY NODES (PRESERVE SIGN)
                    mzcinp = mzcinp * nc;
                    //            USER INPUT OF ROOT DISTRIBUTION AND NODE SPACING WITHIN
                    //            THE CANOPY (ASSUMED TO BE CONSTANT FOR ENTIRE SIMULATION)

                    if (_lvlout[1] > 0)
                    {
                        var generalOut = _outputWriters[OutputFile.EntireProfile];
                        generalOut.WriteLine();
                        generalOut.WriteLine($"             H2O IN     {string.Concat(Enumerable.Range(1, _nplant).Select(j => $"---------plantspecies#{j}------- "))}");
                        generalOut.WriteLine($"     DEPTH  DEAD MATL  {string.Concat(Enumerable.Repeat("  CHAR.DIM. CLUMPING BIOMASS  LAI", _nplant))}))");
                        generalOut.WriteLine($"      (M)     (KG/KG)  {string.Concat(Enumerable.Repeat("     (CM)    FACTOR  (KG/M2)     ", _nplant))}))");
                    }

                    if (iversion == 2)
                    {
                        //read(11, *)(zc[i], (dchar[j], drycan(j, i), canlai(j, i), j = 1, nplant),i = 1,nc);
                        t = ParseNextLine(InputFile.SiteDescriptionData);
                        var k = 0;
                        for (var i = 1; i <= nc; ++i)
                        {
                            zc[i] = double.Parse(t[k++]);
                            for (var j = 1; j <= _nplant; ++j)
                            {
                                _dchar[j] = double.Parse(t[k++]);
                                _clayrs.Drycan[j][i] = double.Parse(t[k++]);
                                _clayrs.Canlai[j][i] = double.Parse(t[k++]);
                            }
                        }
                        for (var i = 1; i < _clumpng.Length; ++i)
                            _clumpng[i] = 1.0;
                    }
                    else
                    {
                        //read(11, *)(zc[i], (dchar[j], clumpng[j], drycan(j, i), canlai(j, i), j = 1, nplant),i = 1,nc);
                        t = ParseNextLine(InputFile.SiteDescriptionData);
                        var k = 0;
                        for (var i = 1; i <= nc; ++i)
                        {
                            zc[i] = double.Parse(t[k++]);
                            for (var j = 1; j <= _nplant; ++j)
                            {
                                _dchar[j] = double.Parse(t[k++]);
                                _clumpng[j] = double.Parse(t[k++]);
                                _clayrs.Drycan[j][i] = double.Parse(t[k++]);
                                _clayrs.Canlai[j][i] = double.Parse(t[k++]);
                            }
                        }
                    }

                    for (var i = 1; i <= nc; ++i)
                    {
                        //WRITE(21, 187)ZC(I),WCANDT(I),()(dchar[j], clumpng[j], _clayrs.Drycan(j, i), _clayrs.Canlai(j, i), j = 1, nplant);
                        if (_lvlout[1] > 0)
                            _outputWriters[OutputFile.EntireProfile].WriteLine($"     {zc[i],5:F2}{wcandt[i],9:F2}    {string.Concat(Enumerable.Range(1, _nplant).Select(j => $"{_dchar[j],8:F2}{_clumpng[j],9:F2}{_clayrs.Drycan[j][i],9:F2}{_clayrs.Canlai[j][i],7:F2}"))}");
                    }

                    zc[nc + 1] = double.Parse(ReadNextLine(InputFile.SiteDescriptionData));
                    if (_lvlout[1] > 0)
                    {
                        var generalOut = _outputWriters[OutputFile.EntireProfile];
                        generalOut.WriteLine($"     {zc[nc + 1],5:F2}  ==>  BOTTOM DEPTH OF CANOPY");

                        generalOut.WriteLine();
                        generalOut.WriteLine("     ROOT DENSITY PROFILE:");
                        generalOut.WriteLine($"     SOIL LAYER            :{string.Concat(Enumerable.Range(1, _ns).Select(i => $"{i,5:D}"))}");
                    }

                    for (var j = 1; j <= _nplant; ++j)
                    {
                        if (iversion > 2)
                        {
                            //read(11, *)(_clayrs.Rootdn[j][i], i = 1, ns);
                            t = ParseNextLine(InputFile.SiteDescriptionData);
                            for (var i = 1; i <= _ns; ++i)
                                _clayrs.Rootdn[j][i] = double.Parse(t[i - 1]);

                            //WRITE(21, 181)J,(ROOTDN(J, I),I = 1,NS)();
                            if (_lvlout[1] > 0)
                                _outputWriters[OutputFile.EntireProfile].WriteLine($"     PLANT #{j} ROOT FRACTION: {string.Concat(Enumerable.Range(1, _ns).Select(i => $"{_clayrs.Rootdn[j][i],5:F2}"))}");
                        }
                    }

                    //           INITIALIZE PLANT HEIGHT, WEIGHT, LAI, AND ROOTS
                    for (var j = 1; j <= _nplant; ++j)
                    {
                        _plthgt[j] = 0.0;
                        _pltwgt[j] = 0.0;
                        _pltlai[j] = 0.0;
                        _clayrs.Totrot[j] = 0.0;
                        //              CONVERT LEAF DIMENSION FROM CM TO METERS
                        _dchar[j] = _dchar[j] / 100.0;
                        for (var i = nc; i >= 1; --i)
                        {
                            if (_clayrs.Canlai[j][i] > 0.0) _plthgt[j] = zc[nc + 1] - zc[i];
                            _pltwgt[j] = _pltwgt[j] + _clayrs.Drycan[j][i];
                            _pltlai[j] = _pltlai[j] + _clayrs.Canlai[j][i];
                        label1025:;
                        }
                        _clayrs.Totlai[j] = _pltlai[j];
                        if (_itype[j] != 0)
                        {
                            //                 TRANSPIRING PLANT -- CALCULATE TOTAL ROOT DENSITY
                            //                 (FRACTION OF ROOTS IN SOIL COLUMN)
                            for (var i = 1; i <= _ns; ++i)
                            {
                                _clayrs.Totrot[j] = _clayrs.Totrot[j] + _clayrs.Rootdn[j][i];
                            label1030:;
                            }
                            //                 CALC. EFFECTIVE LEAF CONDUCT. FOR EACH CANOPY LAYER
                            for (var i = 1; i <= nc; ++i)
                            {
                                _clayrs.Rleaf[j][i] = _rleaf0[j] * _clayrs.Canlai[j][i] / _clayrs.Totlai[j];
                            label1040:;
                            }
                            //                 CALC. EFFECTIVE ROOT CONDUC. FOR EACH SOIL LAYER
                            for (var i = 1; i <= _ns; ++i)
                            {
                                _clayrs.Rroot[j][i] = _rroot0[j] * _clayrs.Rootdn[j][i] / _clayrs.Totrot[j];
                            label1050:;
                            }
                        }
                    label1060:;
                    }
                }
            }

            // line 2286
            // **** INPUT PROPERTIES FOR SNOWPACK
            //      NSP = 0  ---> SNOW IS NOT PRESENT, BUT IT MAY SNOW
            //double zmspcm;
            //t = ParseNextLine(InputFile.SiteDescriptionData);
            if (iversion == 2)
            {
                //// READ (11,*) SNOTMP,ZMSPCM
                //snotmp = double.Parse(t[0]); //_zmspcm = double.Parse(t[1]);
                //isnotmp = 1;
            }
            else
            {
                // READ (11,*) ISNOTMP,SNOTMP,ZMSPCM
                // isnotmp = int.Parse(t[0]); snotmp = double.Parse(t[1]); _zmspcm = double.Parse(t[2]);
            }

            zmsp = _zmspcm / 100.0;
            zhsp = 0.2 * zmsp;
            zhspcm = zhsp * 100.0;

            var osnotmp = _isnotmp == 1 ? " C AIR TEMPERATURE      " : " C WET BULB TEMPERATURE ";

            if (_lvlout[1] > 0)
            {
                var generalOut = _outputWriters[OutputFile.EntireProfile];
                generalOut.WriteLine();
                generalOut.WriteLine();
                generalOut.WriteLine(" SNOW PARAMETERS");
                generalOut.WriteLine();
                generalOut.WriteLine($"     MAXIMUM TEMPERATURE FOR SNOWFALL :{_snotmp,5:F2}{osnotmp}");
                generalOut.WriteLine($"     WIND-PROFILE PARAMETERS FOR SNOW: ZM ={_zmspcm,5:F2} CM    ZH ={zhspcm,5:F2} CM");
                generalOut.WriteLine($"     GRAIN-SIZE DIAMETER PARAMETERS :{Spprop.G1,5:F2}{Spprop.G2,6:F2}{Spprop.G3,8:F1}");
                generalOut.WriteLine($"     SOLAR RADIATION EXTINCTION COEFFICIENT PARAMETER :{Spprop.Extsp,6:F2}");
                generalOut.WriteLine($"     EMISSIVITY OF SNOWPACK :{Lwrcof.Emitsp,5:F2}");
                generalOut.WriteLine($"     COEFFICIENT AND EXPONENT FOR ALBEDO :{Swrcoe.Snocof,5:F2}{Swrcoe.Snoexp,7:F2}");
                generalOut.WriteLine($"     COEFFICIENTS AND EXPONENT FOR THEMAL COND.:{Spprop.Tkspa,6:F3}{Spprop.Tkspb,7:F2}{Spprop.Tkspex,7:F1}");
                generalOut.WriteLine($"     VAPOR DIFFUSIVITY AND TEMP-DEPENDENCE EXPONENT :{Spprop.Vdifsp,8:F5} M**2{Spprop.Vapspx,7:F1}");
                generalOut.WriteLine($"     LAG COEFFICIENTS FOR SNOWCOVER OUTFLOW :{Spwatr.Clag1,5:F1}{Spwatr.Clag2,6:F1}{Spwatr.Clag3,6:F1}{Spwatr.Clag4,8:F1}");
                generalOut.WriteLine($"     MAX AND MIN WATER HOLDING CAPACITY AND WHC DENSITY:{Spwatr.Plwmax,5:F2} M/M{Spwatr.Plwhc,7:F3} M/M{Spwatr.Plwden,8:F1} KG/M**3");
                generalOut.WriteLine($"     COMPACTION AND SETTLING PARAMETERS :{Metasp.Cmet1,5:F2}{Metasp.Cmet2,7:F1}{Metasp.Cmet3,7:F2}{Metasp.Cmet4,7:F2}{Metasp.Cmet5,7:F1}{Metasp.Snomax,8:F1}");
            }

            if (_nsp > 0)
            {
                //        SNOWPACK IS PRESENT AT THE START OF THE SIMULATION
                for (var i = 1; i <= _nsp; ++i)
                {
                    // READ (11,*) DZSP(I),TSPDT(I),DLWDT(I),RHOSP(I)
                    t = ParseNextLine(InputFile.SiteDescriptionData);
                    dzsp[i] = double.Parse(t[0]); tspdt[i] = double.Parse(t[1]); dlwdt[i] = double.Parse(t[2]); rhosp[i] = double.Parse(t[3]);
                    icespt[i] = 0;
                    if (dlwdt[i] > 0.0) icespt[i] = 1;
                    label5:;
                }
                zsp[1] = 0.0;
                tdepth = dzsp[1];
                for (var i = 2; i <= _nsp; ++i)
                {
                    zsp[i] = tdepth + dzsp[i] / 2.0;
                    tdepth = tdepth + dzsp[i];
                label10:;
                }
                zsp[_nsp + 1] = tdepth;
            }

            // **** INPUT PROPERTIES FOR RESIDUE LAYERS
            if (_nr > 0)
            {
                //         DETERMINE MAXIMUM WATER CONTENT OF RESIDUE (AT 99.9% RH)
                var hum999 = 0.999;
                var double0 = 0.0;
                Reshum(2, ref hum999, ref dummy, ref gmcmax, ref double0);

                if (iversion == 2)
                {
                    //// read(11,*)cover,albres,rload,zrthik,gmcdt(1),rescof
                    //t = ParseNextLine(InputFile.SiteDescriptionData);
                    //cover = double.Parse(t[0]); albres = double.Parse(t[1]); rload = double.Parse(t[2]); zrthik = double.Parse(t[3]); gmcdt[1] = double.Parse(t[4]); rescof = double.Parse(t[5]);

                    //nrchang = 0;
                    //restkb = 4.0;
                }
                else
                {
                    // read(11,*)nrchang,gmcdt(1)
                    //t = ParseNextLine(InputFile.SiteDescriptionData);
                    //nrchang = int.Parse(t[0]); gmcdt[1] = double.Parse(t[1]);
                    nrchang = 0; gmcdt[1] = 0.0;

                    if (nrchang == 0)
                    {
                        //              RESIDUE DOES NOT CHANGE DURING SIMULATION
                        // read(11,*)zrthik,rload,cover,albres,rescof,restkb
                        //t = ParseNextLine(InputFile.SiteDescriptionData);
                        //zrthik = double.Parse(t[0]); rload = double.Parse(t[1]); cover = double.Parse(t[2]); // albres = double.Parse(t[3]); rescof = double.Parse(t[4]); restkb = double.Parse(t[5]);

                        if (!SimpleFileParser.TryParseInput("LitterThickness", thuFileData["LitterThickness"], out _zrthik, out errorMessage, 0.0))
                            return false;

                        if (!SimpleFileParser.TryParseInput("LitterDryWeight", thuFileData["LitterDryWeight"], out _rload, out errorMessage, 0.0))
                            return false;

                        if (!SimpleFileParser.TryParseInput("FractionGroundCoveredLitter", thuFileData["FractionGroundCoveredLitter"], out _cover, out errorMessage, 0.0, 1.0))
                            return false;
                    }
                }

                if (nrchang == 0)
                {
                    // write(21,125)zrthik,rload,cover,albres,rescof,restkb,restka,emitr,resma,resmb,gmcmax
                    if (_lvlout[1] > 0)
                    {
                        var generalOut = _outputWriters[OutputFile.EntireProfile];
                        generalOut.WriteLine();
                        generalOut.WriteLine();
                        generalOut.WriteLine(" RESIDUE PARAMETERS");
                        generalOut.WriteLine();
                        generalOut.WriteLine($"     THICKNESS OF RESIDUE LAYER :{_zrthik,5:F2} CM");
                        generalOut.WriteLine($"     RESIDUE LOADING :{_rload,7:F0} KG/HA");
                        generalOut.WriteLine($"     FRACTION OF GROUND COVERED BY RESIDUE :{_cover,5:F2}");
                        generalOut.WriteLine($"     ALBEDO OF RESIDUE :{_albres,5:F2}");
                        generalOut.WriteLine($"     VAPOR TRANSFER RESISTANCE FROM RESIDUE :{_rescof,7:F0}");
                        generalOut.WriteLine($"     WIND COEFFICIENT FOR THERMAL CONVECTION :{_restkb,6:F2}");
                        generalOut.WriteLine($"     TEMPERATURE COEFF. FOR THERMAL CONVECTION :{Rsparm.Restka,6:F3}");
                        generalOut.WriteLine($"     EMISSIVITY OF RESIDUE :{Lwrcof.Emitr,5:F2}");
                        generalOut.WriteLine($"     MOISTURE PARAMETERS FOR RESIDUE :{Rsparm.Resma,7:F2}{Rsparm.Resmb,5:F2}");
                        generalOut.WriteLine($"     MAXIMUM MOISTURE CONTENT FOR RESIDUE :{gmcmax,5:F2} KG/KG");
                    }

                    //           CONVERT RLOAD FROM KG/HA TO KG/M2
                    _rload = _rload / 10000.0;
                    //           CONVERT ZRTHIK FROM CM TO M
                    _zrthik = _zrthik / 100.0;
                }
                else
                {
                    //           OPEN FILE FOR CHANGING RESIDUE PROPERTIES
                    // read(11,100)ifile
                    var ifile = ReadNextLine(InputFile.SiteDescriptionData);
                    StreamReader r;
                    if (!OpenStreamReader(Path.Combine(wd, ifile), out r))
                    {
                        //              RESIDUE INPUT FILE COULD NOT BE OPENED
                        Console.WriteLine();
                        Console.WriteLine($" THE FOLLOWING INPUT FILE CANNOT BE OPENED: {ifile}");
                        Console.WriteLine();
                        Console.WriteLine(" Check that filename and path for the residue ");
                        Console.WriteLine(" file listed in the site characteristics file is correct:''");
                        return false;
                    }

                    _residueReader = r;

                    if (_lvlout[1] > 0)
                    {
                        var generalOut = _outputWriters[OutputFile.EntireProfile];
                        generalOut.WriteLine();
                        generalOut.WriteLine();
                        generalOut.WriteLine(" RESIDUE PARAMETERS");
                        generalOut.WriteLine();
                        generalOut.WriteLine($"     TEMPERATURE COEFF. FOR THERMAL CONVECTION :{Rsparm.Restka,6:F3}");
                        generalOut.WriteLine($"     EMISSIVITY OF RESIDUE :{Lwrcof.Emitr,5:F2}");
                        generalOut.WriteLine($"     MOISTURE PARAMETERS FOR RESIDUE :{Rsparm.Resma,7:F2}{Rsparm.Resmb,5:F2}");
                        generalOut.WriteLine($"     MAXIMUM MOISTURE CONTENT FOR RESIDUE :{gmcmax,5:F2} KG/KG");
                        generalOut.WriteLine($"     INPUT FILE FOR RESIDUE STATUS : {ifile}");
                    }
                }

                if (gmcdt[1] > gmcmax) gmcdt[1] = gmcmax;
            }
            else
            {
                //         SET RESIDUE TO NEVER CHANGE
                nrchang = 0;
                _zrthik = 0.0;
            }

            //**** INPUT PROPERTIES AND INITIAL CONDITIONS FOR EACH TYPE OF SOLUTE
            if (_slparm.Nsalt > 0 && _lvlout[1] > 0)
            {
                var generalOut = _outputWriters[OutputFile.EntireProfile];
                generalOut.WriteLine();
                generalOut.WriteLine();
                generalOut.WriteLine(" SOLUTE PROPERTIES");
            }

            for (var i = 1; i <= _slparm.Nsalt; ++i)
            {
                t = ParseNextLine(InputFile.SiteDescriptionData);
                sltdif[i] = double.Parse(t[0]); haflif = double.Parse(t[1]);

                // read(11,*)(saltkq(i,j),j=1,ns)
                t = ParseNextLine(InputFile.SiteDescriptionData);
                for (var j = 1; j <= _ns; ++j)
                    _slparm.Saltkq[i][j] = double.Parse(t[j - 1]);

                // read(11,*)(saltdt(i,j),j=1,ns)
                t = ParseNextLine(InputFile.SiteDescriptionData);
                for (var j = 1; j <= _ns; ++j)
                    saltdt[i][j] = double.Parse(t[j - 1]);

                // write(21,140)i,sltdif(i),haflif
                if (_lvlout[1] > 0)
                {
                    var generalOut = _outputWriters[OutputFile.EntireProfile];
                    generalOut.WriteLine();
                    generalOut.WriteLine($"     SALT SPECIES #{i}:");
                    generalOut.WriteLine($"     DIFFUSIVITY  = {sltdif[i],10:E3} M**2/S");
                    generalOut.WriteLine($"     HALF-LIFE ={haflif,6:F1} DAYS (ZERO INDICATES NO DEGRADATION)");
                    generalOut.WriteLine("     SOIL MATRIX-SOIL SOLUTION PARTITIONING COEEF (Kd) FOR EACH SOIL NODE:");

                    // write(21,145)(saltkq(i,j),j=1,ns)
                    if (_ns > 15)
                    {
                        generalOut.WriteLine($"     {string.Concat(Enumerable.Range(1, 15).Select(j => $"{_slparm.Saltkq[i][j],7:F2}"))}");
                        generalOut.WriteLine($"     {string.Concat(Enumerable.Range(16, _ns).Select(j => $"{_slparm.Saltkq[i][j],7:F2}"))}");
                    }
                    else
                    {
                        generalOut.WriteLine($"     {string.Concat(Enumerable.Range(1, _ns).Select(j => $"{_slparm.Saltkq[i][j],7:F2}"))}");
                    }
                }

                //        CALCULATE EXPONENT FOR DEGRADATION BASED ON HALF LIFE
                if (haflif == 0.0)
                {
                    //           INDICATES NO DEGRADATION OF SOLUTE SPECIES
                    dgrade[i] = 0.0;
                }
                else
                {
                    dgrade[i] = 0.693147 / haflif;
                }
            label30:;
            }

            // line 2407

            // **** INPUT PROPERTIES FOR SOIL LAYERS
            //t = ParseNextLine(InputFile.SiteDescriptionData);
            if (iversion == 2)
            {
                //// read(11,*)ivlcbc,itmpbc,albdry,albexp
                //ivlcbc = int.Parse(t[0]); itmpbc = int.Parse(t[1]); albdry = double.Parse(t[2]); albexp = double.Parse(t[3]);

                //Slparm.Iwrc = 1;

                //if (itmpbc == 1)
                //{
                //    Console.WriteLine(" Enter the average soil temperature at depth: ");
                //    if (!double.TryParse(Console.ReadLine(), out tsavg))
                //    {
                //        Console.WriteLine();
                //        Console.WriteLine(" Invalid input");
                //        return false;
                //    }
                //}
            }
            else
            {
                // read(11,*)ivlcbc,itmpbc,albdry,albexp,iwrc
                //ivlcbc = int.Parse(t[0]); itmpbc = int.Parse(t[1]);  albdry = double.Parse(t[2]); albexp = double.Parse(t[3]); // Slparm.Iwrc = int.Parse(t[4]);

                //         READ AVERAGE ANNUAL SOIL TEMPERATURE IF LOWER SOIL TEMPERATURE
                //         IS ESTIMATED BY THE MODEL

                //// John McNabb: don't read tsavg from the site file, rather it is passed to Shaw. simply skip over the line in the site file.
                //if (_itmpbc == 1)
                //    // read(11, *)tsavg;
                //    double.Parse(ReadNextLine(InputFile.SiteDescriptionData));
                //tsavg = double.Parse(ReadNextLine(InputFile.SiteDescriptionData));
            }

            // write(21,150)
            if (_lvlout[1] > 0)
            {
                var generalOut = _outputWriters[OutputFile.EntireProfile];
                generalOut.WriteLine();
                generalOut.WriteLine();
                generalOut.WriteLine(" SOIL PROPERTIES");

                if (_ivlcbc <= 0)
                {
                    // write(21,151)
                    generalOut.WriteLine();
                    generalOut.WriteLine("     INPUT WATER CONTENT SPECIFIED FOR LOWER BOUNDARY OF WATER FLUX");
                }
                else
                {
                    // write(21,152)
                    generalOut.WriteLine();
                    generalOut.WriteLine("     UNIT GRADIENT SPECIFIED FOR LOWER BOUNDARY OF WATER FLUX");
                }
                if (_itmpbc <= 0)
                {
                    // write(21,153)
                    generalOut.WriteLine("     INPUT SOIL TEMPERATURE SPECIFIED FOR LOWER BOUNDARY");
                }
                else if (_itmpbc == 1)
                {
                    // write(21,154)tsavg
                    generalOut.WriteLine("     SOIL TEMPERATURE AT LOWER BOUNDARY ESTIMATED BY MODEL");
                    generalOut.WriteLine($"          ASSUMING AVERAGE ANNUAL SOIL TEMPERATURE OF{_tsavg,5:F1}C");
                }
                else
                {
                    // write(21,1154)
                    generalOut.WriteLine("     ZERO HEAT FLUX ASSUMED AT LOWER BOUNDARY");
                }

                // write(21,155)albdry,albexp,emits
                generalOut.WriteLine();
                generalOut.WriteLine($"     ALBEDO OF DRY SOIL AND MOISTURE-DEPENDENCE EXPONENT :{_albdry,5:F2}{_albexp,7:F2}");
                generalOut.WriteLine($"     EMISSIVITY OF SOIL:{Lwrcof.Emits,5:F2}");

                if (_slparm.Nsalt == 0)
                {
                    if (Slparm.Iwrc == 1)
                    {
                        //write(21, 1156);
                        generalOut.WriteLine();
                        generalOut.WriteLine("     DEPTH   SAND   SILT   CLAY   ROCK    OM    DENSITY   K-SAT  Ks-LATRL  AIR ENTRY    SAT     B-VALUE");
                        generalOut.WriteLine("     (m)    (% wt) (% wt) (% wt) (% wt) (% wt)  (kg/m3)   (cm/hr)  (cm/hr)     (m)    (cm3/cm3)    (-)");
                    }
                    if (Slparm.Iwrc == 2)
                    {
                        //write(21, 2156);
                        generalOut.WriteLine();
                        generalOut.WriteLine("     DEPTH   SAND   SILT   CLAY   ROCK    OM    DENSITY   K-SAT  Ks-LATRL  AIR ENTRY    SAT     Lambda   ResidSat   L-value");
                        generalOut.WriteLine("     (m)    (% wt) (% wt) (% wt) (% wt) (% wt)  (kg/m3)   (cm/hr)  (cm/hr)     (m)    (cm3/cm3)    (-)    (cm3/cm3)    (-)");
                    }
                    if (Slparm.Iwrc == 3)
                    {
                        //write(21, 3156);
                        generalOut.WriteLine();
                        generalOut.WriteLine("     DEPTH   SAND   SILT   CLAY   ROCK    OM    DENSITY   K-SAT  Ks-LATRL  AIR ENTRY    SAT     n-value  ResidSat   L-value   alpha");
                        generalOut.WriteLine("     (m)    (% wt) (% wt) (% wt) (% wt) (% wt)  (kg/m3)   (cm/hr)  (cm/hr)     (m)    (cm3/cm3)    (-)    (cm3/cm3)    (-)     (-)");
                    }
                    if (Slparm.Iwrc == 4)
                    {
                        //write(21, 4156);
                        generalOut.WriteLine();
                        generalOut.WriteLine("     DEPTH   SAND   SILT   CLAY   ROCK    OM    DENSITY   K-SAT  Ks-LATRL  AIR ENTRY    SAT     B-VALUE     A-VALUE");
                        generalOut.WriteLine("     (m)    (% wt) (% wt) (% wt) (% wt) (% wt)  (kg/m3)   (cm/hr)  (cm/hr)     (m)    (cm3/cm3)    (-)         (m)");
                    }
                }
                else
                {
                    if (Slparm.Iwrc == 1)
                    {
                        //write(21, 1157);
                        generalOut.WriteLine();
                        generalOut.WriteLine("     DEPTH   SAND   SILT   CLAY   ROCK    OM    DENSITY   K-SAT  Ks-LATRL  AIR ENTRY    SAT     B-VALUE   SALT   PARAMS");
                        generalOut.WriteLine("     (m)    (% wt) (% wt) (% wt) (% wt) (% wt)  (kg/m3)   (cm/hr)  (cm/hr)     (m)    (cm3/cm3)    (-)     -------------");
                    }
                    if (Slparm.Iwrc == 2)
                    {
                        //write(21, 2157);
                        generalOut.WriteLine();
                        generalOut.WriteLine("     DEPTH   SAND   SILT   CLAY   ROCK    OM    DENSITY   K-SAT  Ks-LATRL  AIR ENTRY    SAT     Lambda   ResidSat   L-valueSALT   PARAMS");
                        generalOut.WriteLine("     (m)    (% wt) (% wt) (% wt) (% wt) (% wt)  (kg/m3)   (cm/hr)  (cm/hr)     (m)    (cm3/cm3)    (-)    (cm3/cm3)    (-)     -------------");
                    }
                    if (Slparm.Iwrc == 3)
                    {
                        //write(21, 3157);
                        generalOut.WriteLine();
                        generalOut.WriteLine("     DEPTH   SAND   SILT   CLAY   ROCK    OM    DENSITY   K-SAT  Ks-LATRL  AIR ENTRY    SAT     n-value  ResidSat   L-valuealpha   SALT   PARAMS");
                        generalOut.WriteLine("     (m)    (% wt) (% wt) (% wt) (% wt) (% wt)  (kg/m3)   (cm/hr)  (cm/hr)     (m)    (cm3/cm3)    (-)    (cm3/cm3)    (-)     (-)     -------------");
                    }
                    if (Slparm.Iwrc == 4)
                    {
                        //write(21, 4157);
                        generalOut.WriteLine();
                        generalOut.WriteLine("     DEPTH   SAND   SILT   CLAY   ROCK    OM    DENSITY   K-SAT  Ks-LATRL  AIR ENTRY    SAT     B-VALUE     A-VALUE   SALTPARAMS");
                        generalOut.WriteLine("     (m)    (% wt) (% wt) (% wt) (% wt) (% wt)  (kg/m3)   (cm/hr)  (cm/hr)     (m)    (cm3/cm3)    (-)         (m)     -------------");
                    }
                }
            }

            if (Slparm.Iwrc == 1)
            {
                nwrc = 3;
            }
            else if (Slparm.Iwrc == 2)
            {
                nwrc = 5;
            }
            else if (Slparm.Iwrc == 3)
            {
                nwrc = 6;
            }
            else if (Slparm.Iwrc == 4)
            {
                nwrc = 4;
            }

            // set soil inputs
            jj = 1;
            _ns = 0;

            //if (!SimpleFileParser.TryParseInput("SurfaceWaterContent", thuFileData["SurfaceWaterContent"], out double previousWaterContentAtMaxDepth, out errorMessage, 0.0))
            //    return false;

            var previousMaxDepth = 0.0;
            while (true)
            {
                var key = $"ShawSoilType{jj}";
                if (!thuFileData.TryGetValue(key, out var soilType) || string.IsNullOrEmpty(soilType))
                    break;

                if (!soilFileData.TryGetValue(soilType, out var soilData))
                {
                    errorMessage = $"Soil data for {key} '{soilType}' not found";
                    return false;
                }


                if (!SimpleFileParser.TryParseInput($"MaxDepth{jj}", thuFileData[$"MaxDepth{jj}"], out double maxDepth, out errorMessage, 0.0))
                    return false;

                if (!SimpleFileParser.TryParseInput($"Nodes{jj}", thuFileData[$"Nodes{jj}"], out int nodeCount, out errorMessage, 0))
                    return false;

                //if (!SimpleFileParser.TryParseInput($"WaterContentAtMaxDepth{jj}", thuFileData[$"WaterContentAtMaxDepth{jj}"], out double waterContentAtMaxDepth, out errorMessage, 0.0))
                //    return false;

                if (!SimpleFileParser.TryParseInput($"InitWaterContent{jj}", thuFileData[$"InitWaterContent{jj}"], out double initWaterContent, out errorMessage, 0.0))
                    return false;

                var step = (maxDepth - previousMaxDepth) / nodeCount;
                //var waterStep = (waterContentAtMaxDepth - previousWaterContentAtMaxDepth) / nodeCount;

                // if this is the first layer, then start with a node at zero depth
                for (var i = (jj == 1 ? 0 : 1); i <= nodeCount; ++i)
                {
                    ++_ns;
                    zs[_ns] = previousMaxDepth + step * i;
                    _vlcdt[_ns] = initWaterContent;
                    //_vlcdt[_ns] = previousWaterContentAtMaxDepth + waterStep * i;

                    if (!SimpleFileParser.TryParseInput("Sand", soilData["Sand"], out _slparm.Sand[_ns], out errorMessage, 0.0, 100.0))
                        return false;

                    if (!SimpleFileParser.TryParseInput("Silt", soilData["Silt"], out _slparm.Silt[_ns], out errorMessage, 0.0, 100.0))
                        return false;

                    if (!SimpleFileParser.TryParseInput("Clay", soilData["Clay"], out _slparm.Clay[_ns], out errorMessage, 0.0, 100.0))
                        return false;

                    if (!SimpleFileParser.TryParseInput("Rock", soilData["Rock"], out _slparm.Rock[_ns], out errorMessage, 0.0, 100.0))
                        return false;

                    if (!SimpleFileParser.TryParseInput("Organic", soilData["Organic"], out _slparm.Om[_ns], out errorMessage, 0.0, 100.0))
                        return false;

                    if (!SimpleFileParser.TryParseInput("BulkDensity", soilData["BulkDensity"], out _slparm.Rhob[_ns], out errorMessage, 0.0))
                        return false;

                    if (!SimpleFileParser.TryParseInput("Ksat", soilData["Ksat"], out satcon, out errorMessage, 0.0))
                        return false;

                    if (!SimpleFileParser.TryParseInput("KsatLateral", soilData["KsatLateral"], out satkl, out errorMessage, 0.0))
                        return false;

                    //         CONVERT CONDUCTIVITY FROM CM/HR TO M/SEC
                    _slparm.Satk[_ns] = satcon / 360000.0;
                    _slparm.Satklat[_ns] = satkl / 360000.0;

                    if (!SimpleFileParser.TryParseInput("AirEntry", soilData["AirEntry"], out _slparm.Soilwrc[_ns][1], out errorMessage))
                        return false;

                    if (!SimpleFileParser.TryParseInput("ThetaSat", soilData["ThetaSat"], out _slparm.Soilwrc[_ns][2], out errorMessage))
                        return false;

                    if (Slparm.Iwrc == 1)
                        if (!SimpleFileParser.TryParseInput("Campbells", soilData["Campbells"], out _slparm.Soilwrc[_ns][3], out errorMessage))
                            return false;

                    if (Slparm.Iwrc == 2)
                        if (!SimpleFileParser.TryParseInput("BrooksCoreyPoreSize", soilData["BrooksCoreyPoreSize"], out _slparm.Soilwrc[_ns][3], out errorMessage))
                            return false;

                    if (Slparm.Iwrc == 2 || Slparm.Iwrc == 3)
                        if (!SimpleFileParser.TryParseInput("ThetaResidual", soilData["ThetaResidual"], out _slparm.Soilwrc[_ns][4], out errorMessage))
                            return false;

                    if (Slparm.Iwrc == 2)
                        if (!SimpleFileParser.TryParseInput("BrooksCoreyPoreConnectivity", soilData["BrooksCoreyPoreConnectivity"], out _slparm.Soilwrc[_ns][5], out errorMessage))
                            return false;

                    if (Slparm.Iwrc == 3)
                        if (!SimpleFileParser.TryParseInput("nVanGenuchten", soilData["nVanGenuchten"], out _slparm.Soilwrc[_ns][3], out errorMessage))
                            return false;

                    if (Slparm.Iwrc == 3)
                        if (!SimpleFileParser.TryParseInput("vanGenuchtenPoreConnectivity", soilData["vanGenuchtenPoreConnectivity"], out _slparm.Soilwrc[_ns][5], out errorMessage))
                            return false;

                    if (Slparm.Iwrc == 3)
                        if (!SimpleFileParser.TryParseInput("alphaVanGenuchten", soilData["alphaVanGenuchten"], out _slparm.Soilwrc[_ns][6], out errorMessage))
                            return false;

                }

                previousMaxDepth = maxDepth;
                //previousWaterContentAtMaxDepth = initWaterContent;

                ++jj;
            }

            for (var i = 1; i <= _ns; ++i)
            {
                //t = ParseNextLine(InputFile.SiteDescriptionData);
                if (iversion == 2)
                {
                    if (_slparm.Nsalt == 0)
                    {
                        // read(11,*)zs(i),soilwrc(i,3),soilwrc(i,1),satcon,rhob(i),soilwrc(i,2),sand(i),silt(i),clay(i),om(i)
                        zs[i] = double.Parse(t[0]); _slparm.Soilwrc[i][3] = double.Parse(t[1]); _slparm.Soilwrc[i][1] = double.Parse(t[2]); satcon = double.Parse(t[3]); _slparm.Rhob[i] = double.Parse(t[4]); _slparm.Soilwrc[i][2] = double.Parse(t[5]); _slparm.Sand[i] = double.Parse(t[6]); _slparm.Silt[i] = double.Parse(t[7]); _slparm.Clay[i] = double.Parse(t[8]); _slparm.Om[i] = double.Parse(t[9]);
                    }
                    else
                    {
                        // read(11,*)zs(i),soilwrc(i,3),soilwrc(i,1),satcon,rhob(i),soilwrc(i,2),sand(i),silt(i),clay(i),om(i),asalt(i),disper(i)
                        zs[i] = double.Parse(t[0]); _slparm.Soilwrc[i][3] = double.Parse(t[1]); _slparm.Soilwrc[i][1] = double.Parse(t[2]); satcon = double.Parse(t[3]); _slparm.Rhob[i] = double.Parse(t[4]); _slparm.Soilwrc[i][2] = double.Parse(t[5]); _slparm.Sand[i] = double.Parse(t[6]); _slparm.Silt[i] = double.Parse(t[7]); _slparm.Clay[i] = double.Parse(t[8]); _slparm.Om[i] = double.Parse(t[9]); asalt[i] = double.Parse(t[10]); disper[i] = double.Parse(t[11]);
                    }
                    _slparm.Rock[i] = 0.0;
                    satkl = 0.0;
                }
                else
                {
                    if (_slparm.Nsalt == 0)
                    {
                        // read(11,*)zs(i),sand(i),silt(i),clay(i),rock(i),om(i),rhob(i),satcon,satkl,(soilwrc(i,j),j=1,nwrc)
                        //zs[i] = double.Parse(t[0]); _slparm.Sand[i] = double.Parse(t[1]); _slparm.Silt[i] = double.Parse(t[2]); _slparm.Clay[i] = double.Parse(t[3]); _slparm.Rock[i] = double.Parse(t[4]); _slparm.Om[i] = double.Parse(t[5]); _slparm.Rhob[i] = double.Parse(t[6]); satcon = double.Parse(t[7]); satkl = double.Parse(t[8]);
                        //for (var j = 1; j <= nwrc; ++j)
                        //    _slparm.Soilwrc[i][j] = double.Parse(t[8 + j]);
                    }
                    else
                    {
                        // read(11,*)zs(i),sand(i),silt(i),clay(i),rock(i),om(i),rhob(i),satcon,satkl,(soilwrc(i,j),j=1,nwrc),asalt(i),disper(i)
                        zs[i] = double.Parse(t[0]); _slparm.Sand[i] = double.Parse(t[1]); _slparm.Silt[i] = double.Parse(t[2]); _slparm.Clay[i] = double.Parse(t[3]); _slparm.Rock[i] = double.Parse(t[4]); _slparm.Om[i] = double.Parse(t[5]); _slparm.Rhob[i] = double.Parse(t[6]); satcon = double.Parse(t[7]); satkl = double.Parse(t[8]);
                        for (var j = 1; j <= nwrc; ++j)
                            _slparm.Soilwrc[i][j] = double.Parse(t[8 + j]);
                        asalt[i] = double.Parse(t[8 + nwrc + 1]); disper[i] = double.Parse(t[8 + nwrc + 2]);
                    }
                    if (Slparm.Iwrc == 3) _slparm.Soilwrc[i][7] = 1.0 - 1.0 / _slparm.Soilwrc[i][3];
                }

                //         Residual saturation is zero for Campbell and Saxton equations
                if (Slparm.Iwrc == 1 || Slparm.Iwrc == 4) _slparm.Soilwrc[i][4] = 0.0;

                //         ASSURE THAT TEXTURE SUMS TO 100% AND CONVERT TO FRACTION
                textot = _slparm.Sand[i] + _slparm.Silt[i] + _slparm.Clay[i];
                cf = 1.0 / textot;
                _slparm.Sand[i] = _slparm.Sand[i] * cf;
                _slparm.Silt[i] = _slparm.Silt[i] * cf;
                _slparm.Clay[i] = _slparm.Clay[i] * cf;
                _slparm.Rock[i] = _slparm.Rock[i] / 100.0;
                _slparm.Om[i] = _slparm.Om[i] / 100.0;

                //         CHECK IF SATURATION VALUE IS NOT TOO HIGH FOR DENSITY
                //         (ADJUST SPECIFIC DENSITY FOR ORGANIC MATTER)
                if (_slparm.Soilwrc[i][2] > 1.0 - _slparm.Rhob[i] * ((1.0 - _slparm.Om[i]) / Constn.Rhom + _slparm.Om[i] / Constn.Rhoom))
                    _slparm.Soilwrc[i][2] = 1.0 - _slparm.Rhob[i] * ((1.0 - _slparm.Om[i]) / Constn.Rhom + _slparm.Om[i] / Constn.Rhoom);

                ////         CONVERT CONDUCTIVITY FROM CM/HR TO M/SEC
                //_slparm.Satk[i] = satcon / 360000.0;
                //_slparm.Satklat[i] = satkl / 360000.0;

                if (_lvlout[1] > 0)
                {
                    var generalOut = _outputWriters[OutputFile.EntireProfile];
                    if (_slparm.Nsalt == 0)
                    {
                        if (Slparm.Iwrc == 1)
                        {
                            // write(21,1158)zs(i),sand(i)*100.0,silt(i)*100.0,clay(i)*100.0,rock(i)*100.0,om(i)*100.0,rhob(i),satcon,satkl,(soilwrc(i,j),j=1,nwrc)
                            generalOut.WriteLine($"    {zs[i],6:F3}{_slparm.Sand[i] * 100.0,7:F1}{_slparm.Silt[i] * 100.0,7:F1}{_slparm.Clay[i] * 100.0,7:F1}{_slparm.Rock[i] * 100.0,7:F1}{_slparm.Om[i] * 100.0,7:F1}{_slparm.Rhob[i],9:F0} {_slparm.Satk[i] * 360000.0,9:F3}{_slparm.Satklat[i] * 360000.0,9:F3}{_slparm.Soilwrc[i][1],9:F2}{_slparm.Soilwrc[i][2],11:F3}{_slparm.Soilwrc[i][3],10:F3}");
                        }
                        else if (Slparm.Iwrc == 2)
                        {
                            // write(21,2158)zs(i),sand(i)*100.0,silt(i)*100.0,clay(i)*100.0,rock(i)*100.0,om(i)*100.0,rhob(i),satcon,satkl,(soilwrc(i,j),j=1,nwrc)
                            generalOut.WriteLine($"    {zs[i],6:F3}{_slparm.Sand[i] * 100.0,7:F1}{_slparm.Silt[i] * 100.0,7:F1}{_slparm.Clay[i] * 100.0,7:F1}{_slparm.Rock[i] * 100.0,7:F1}{_slparm.Om[i] * 100.0,7:F1}{_slparm.Rhob[i],9:F0} {_slparm.Satk[i] * 360000.0,9:F3}{_slparm.Satklat[i] * 360000.0,9:F3}{_slparm.Soilwrc[i][1],9:F2}{_slparm.Soilwrc[i][2],11:F3}{_slparm.Soilwrc[i][3],10:F3}{_slparm.Soilwrc[i][4],10:F3}{_slparm.Soilwrc[i][5],10:F3}");
                        }
                        else if (Slparm.Iwrc == 3)
                        {
                            // write(21,3158)zs(i),sand(i)*100.0,silt(i)*100.0,clay(i)*100.0,rock(i)*100.0,om(i)*100.0,rhob(i),satcon,satkl,(soilwrc(i,j),j=1,nwrc)
                            generalOut.WriteLine($"    {zs[i],6:F3}{_slparm.Sand[i] * 100.0,7:F1}{_slparm.Silt[i] * 100.0,7:F1}{_slparm.Clay[i] * 100.0,7:F1}{_slparm.Rock[i] * 100.0,7:F1}{_slparm.Om[i] * 100.0,7:F1}{_slparm.Rhob[i],9:F0} {_slparm.Satk[i] * 360000.0,9:F3}{_slparm.Satklat[i] * 360000.0,9:F3}{_slparm.Soilwrc[i][1],9:F2}{_slparm.Soilwrc[i][2],11:F3}{_slparm.Soilwrc[i][3],10:F3}{_slparm.Soilwrc[i][4],10:F3}{_slparm.Soilwrc[i][5],9:F3}{_slparm.Soilwrc[i][6],9:F3}");
                        }
                        else
                        {
                            // write(21,4158)zs(i),sand(i)*100.0,silt(i)*100.0,clay(i)*100.0,rock(i)*100.0,om(i)*100.0,rhob(i),satcon,satkl,(soilwrc(i,j),j=1,nwrc)
                            generalOut.WriteLine($"    {zs[i],6:F3}{_slparm.Sand[i] * 100.0,7:F1}{_slparm.Silt[i] * 100.0,7:F1}{_slparm.Clay[i] * 100.0,7:F1}{_slparm.Rock[i] * 100.0,7:F1}{_slparm.Om[i] * 100.0,7:F1}{_slparm.Rhob[i],9:F0} {_slparm.Satk[i] * 360000.0,9:F3}{_slparm.Satklat[i] * 360000.0,9:F3}{_slparm.Soilwrc[i][1],9:F2}{_slparm.Soilwrc[i][2],11:F3}{_slparm.Soilwrc[i][3],10:F3}{_slparm.Soilwrc[i][4],10:F3}");
                        }
                    }
                    else
                    {
                        if (Slparm.Iwrc == 1)
                        {
                            // write(21,1158)zs(i),sand(i)*100.0,silt(i)*100.0,clay(i)*100.0,rock(i)*100.0,om(i)*100.0,rhob(i),satcon,satkl,(soilwrc(i,j),j=1,nwrc),asalt(i),disper(i)
                            generalOut.WriteLine($"    {zs[i],6:F3}{_slparm.Sand[i] * 100.0,7:F1}{_slparm.Silt[i] * 100.0,7:F1}{_slparm.Clay[i] * 100.0,7:F1}{_slparm.Rock[i] * 100.0,7:F1}{_slparm.Om[i] * 100.0,7:F1}{_slparm.Rhob[i],9:F0} {_slparm.Satk[i] * 360000.0,9:F3}{_slparm.Satklat[i] * 360000.0,9:F3}{_slparm.Soilwrc[i][1],9:F2}{_slparm.Soilwrc[i][2],11:F3}{_slparm.Soilwrc[i][3],10:F3}    {asalt[i],6:F1}{disper[i],8:F3}");
                        }
                        else if (Slparm.Iwrc == 2)
                        {
                            // write(21,2158)zs(i),sand(i)*100.0,silt(i)*100.0,clay(i)*100.0,rock(i)*100.0,om(i)*100.0,rhob(i),satcon,satkl,(soilwrc(i,j),j=1,nwrc),asalt(i),disper(i)
                            generalOut.WriteLine($"    {zs[i],6:F3}{_slparm.Sand[i] * 100.0,7:F1}{_slparm.Silt[i] * 100.0,7:F1}{_slparm.Clay[i] * 100.0,7:F1}{_slparm.Rock[i] * 100.0,7:F1}{_slparm.Om[i] * 100.0,7:F1}{_slparm.Rhob[i],9:F0} {_slparm.Satk[i] * 360000.0,9:F3}{_slparm.Satklat[i] * 360000.0,9:F3}{_slparm.Soilwrc[i][1],9:F2}{_slparm.Soilwrc[i][2],11:F3}{_slparm.Soilwrc[i][3],10:F3}{_slparm.Soilwrc[i][4],10:F3}{_slparm.Soilwrc[i][5],10:F3}    {asalt[i],6:F1}{disper[i],8:F3}");
                        }
                        else if (Slparm.Iwrc == 3)
                        {
                            // write(21,3158)zs(i),sand(i)*100.0,silt(i)*100.0,clay(i)*100.0,rock(i)*100.0,om(i)*100.0,rhob(i),satcon,satkl,(soilwrc(i,j),j=1,nwrc),asalt(i),disper(i)
                            generalOut.WriteLine($"    {zs[i],6:F3}{_slparm.Sand[i] * 100.0,7:F1}{_slparm.Silt[i] * 100.0,7:F1}{_slparm.Clay[i] * 100.0,7:F1}{_slparm.Rock[i] * 100.0,7:F1}{_slparm.Om[i] * 100.0,7:F1}{_slparm.Rhob[i],9:F0} {_slparm.Satk[i] * 360000.0,9:F3}{_slparm.Satklat[i] * 360000.0,9:F3}{_slparm.Soilwrc[i][1],9:F2}{_slparm.Soilwrc[i][2],11:F3}{_slparm.Soilwrc[i][3],10:F3}{_slparm.Soilwrc[i][4],10:F3}{_slparm.Soilwrc[i][5],9:F2}{_slparm.Soilwrc[i][6],9:F3}   {asalt[i],6:F1}{disper[i],8:F3}");
                        }
                        else
                        {
                            // write(21,4158)zs(i),sand(i)*100.0,silt(i)*100.0,clay(i)*100.0,rock(i)*100.0,om(i)*100.0,rhob(i),satcon,satkl,(soilwrc(i,j),j=1,nwrc),asalt(i),disper(i)
                            generalOut.WriteLine($"    {zs[i],6:F3}{_slparm.Sand[i] * 100.0,7:F1}{_slparm.Silt[i] * 100.0,7:F1}{_slparm.Clay[i] * 100.0,7:F1}{_slparm.Rock[i] * 100.0,7:F1}{_slparm.Om[i] * 100.0,7:F1}{_slparm.Rhob[i],9:F0} {_slparm.Satk[i] * 360000.0,9:F3}{_slparm.Satklat[i] * 360000.0,9:F3}{_slparm.Soilwrc[i][1],9:F2}{_slparm.Soilwrc[i][2],11:F3}{_slparm.Soilwrc[i][3],10:F3}{_slparm.Soilwrc[i][4],10:F3}    {asalt[i],6:F1}{disper[i],8:F6}");
                        }
                    }
                }
            }   // line 2575

            if (_lvlout[1] > 0)
            {
                var generalOut = _outputWriters[OutputFile.EntireProfile];
                generalOut.WriteLine();
                generalOut.WriteLine();
                generalOut.WriteLine(" SIMULATION BEGINS");
                generalOut.WriteLine();
            }

            return true;
        }

        private bool SetInitialConditions(int ns, int inph2o, double[] tsdt, double[] vlcdt, double[] vicdt, double[] matdt, double[][] concdt, int[] icesdt, double[][] saltdt,
            int jstart, int yrstar, int hrstar)
        {
            //
            //     THIS SUBROUTINE IS USED TO INPUT ALL GENERAL INFORMATION AND
            //     INITIAL CONDITIONS.
            //
            //***********************************************************************
            //
            //
            //
            //
            var heights = new double[11];

            var cf = 0.0;
            var textot = 0.0;
            var satkl = 0.0;
            var nwrc = 0;
            var tdepth = 0.0;
            var zhspcm = 0.0;
            var doi = 0.0;
            var ninfil = 0;
            var doj = 0.0;
            var zhcm = 0.0;
            var mcanflg = 0;
            var wcan = 0.0;
            var haflif = 0.0;
            var satcon = 0.0;
            var dummy = 0.0;

            List<string> t;
            //var generalOut = _outputWriters[OutputFile.EntireProfile];

            // JM: no longer read initial conditions for soil layers.
            //  these have been passed to Shaw

            //// **** INPUT INITIAL CONDITIONS OF TEMP AND MOISTURE FOR SOIL LAYERS
            //while (true)
            //{
            //    if (_inputReaders[InputFile.TemperatureProfiles].EndOfStream)
            //    {
            //        Console.WriteLine("CANNOT FIND SOIL TEMPERATURE DATA FOR INITIAL HOUR");
            //        generalOut.WriteLine("CANNOT FIND SOIL TEMPERATURE DATA FOR INITIAL HOUR");
            //        return false;
            //    }

            //    // READ (14,*,END=90) JDAY,JHR,JYR,(TSDT(I), I=1,NS)
            //    t = ParseNextLine(InputFile.TemperatureProfiles);
            //    var jday = int.Parse(t[0]); var jhr = int.Parse(t[1]); var jyr = int.Parse(t[2]);
            //    for (var i = 1; i <= ns; ++i)
            //        tsdt[i] = double.Parse(t[2 + i]);

            //    if (jday != jstart || jhr != hrstar || jyr != yrstar)
            //    {
            //        if (hrstar == 24 && jhr == 0)
            //        {
            //            if (jstart != jday - 1 || yrstar != jyr) continue;
            //        }
            //        else
            //        {
            //            continue;
            //        }
            //    }

            //    break;
            //}

            //while (true)
            //{
            //    if (_inputReaders[InputFile.MoistureProfiles].EndOfStream)
            //    {
            //        Console.WriteLine("CANNOT FIND SOIL MOISTURE DATA FOR INITIAL HOUR");
            //        generalOut.WriteLine("CANNOT FIND SOIL MOISTURE DATA FOR INITIAL HOUR");
            //        return false;
            //    }

            //    // READ (13,*,END=95) JDAY,JHR,JYR,(VLCDT(I), I=1,NS)
            //    t = ParseNextLine(InputFile.MoistureProfiles);
            //    var jday = int.Parse(t[0]); var jhr = int.Parse(t[1]); var jyr = int.Parse(t[2]);
            //    for (var i = 1; i <= ns; ++i)
            //        vlcdt[i] = double.Parse(t[2 + i]);

            //    if (jday != jstart || jhr != hrstar || jyr != yrstar)
            //    {
            //        if (hrstar == 24 && jhr == 0)
            //        {
            //            if (jstart != jday - 1 || yrstar != jyr) continue;
            //        }
            //        else
            //        {
            //            continue;
            //        }
            //    }

            //    break;
            //}

            //      SAVE DAY AND HOUR OF LAST TEMPERATURE AND MOISTURE MEASUREMENTS
            _measur.Measdy = jstart;
            _measur.Meashr = hrstar;

            for (var i = 1; i <= ns; ++i)
            {
                //         DEFINE MATRIC POTENTIAL OR WATER CONTENT OF SOIL AND DETERMINE
                //         WHETHER SOIL IS FROZEN
                if (inph2o != 1)
                {
                    //            INPUT SOIL MOISTURE IS WATER CONTENT
                    if (vlcdt[i] > _slparm.Soilwrc[i][2]) vlcdt[i] = _slparm.Soilwrc[i][2];
                    if (vlcdt[i] <= _slparm.Soilwrc[i][4])
                    {
                        //               INVALID INPUT SOIL WATER CONTENT
                        Console.WriteLine(" *** Input initial soil water content is less ***");
                        Console.WriteLine(" *** than or equal to residual water content. ***");
                        _outputWriters[OutputFile.EntireProfile].WriteLine(" *** Input initial soil water content is less ***");
                        _outputWriters[OutputFile.EntireProfile].WriteLine(" *** than or equal to residual water content. ***");
                        return false;
                    }
                    Matvl1(i, ref matdt[i], ref vlcdt[i], ref dummy);
                    //            SAVE INITIAL CONDITIONS FOR SUBROUTINE OUTPUT
                    _measur.Tsmeas[i] = tsdt[i];
                    _measur.Vlcmes[i] = vlcdt[i];
                }
                else
                {
                    //            INPUT SOIL MOISTURE IS MATRIC POTENTIAL
                    matdt[i] = vlcdt[i];
                    Matvl2(i, ref matdt[i], ref vlcdt[i], ref dummy);
                    //            SAVE INITIAL CONDITIONS FOR SUBROUTINE OUTPUT
                    _measur.Tsmeas[i] = tsdt[i];
                    _measur.Vlcmes[i] = matdt[i];
                }

                vicdt[i] = 0.0;
                icesdt[i] = 0;

                //  === (do not adjust if frozen - this is done on entry into GOSHAW
                //  === and problems result if it is done twice when INPH2O = 1
                //  === IF(TSDT(I).LE. 0.0) CALL FROZEN (I, VLCDT, VICDT, MATDT,
                // === > CONCDT, TSDT, SALTDT, ICESDT)

                //         DEFINE SOLUTE CONCENTRATION
                for (var j = 1; j <= _slparm.Nsalt; ++j)
                {
                    concdt[j][i] = saltdt[j][i] / (_slparm.Saltkq[j][i] + vlcdt[i] * Constn.Rhol / _slparm.Rhob[i]);
                }
            }   // line 2575

            return true;
        }

        // line 2753
        /// <summary>Day input.</summary>
        /// <returns></returns>
        private bool Dayinp(int julian, int year, int maxjul, int inhour, int nhrpdt, int mtstep, int iversion, int iflagsi, int inital,
            int itmpbc, int ivlcbc, int lvlout2, int mpltgro, int nrchang, int mwatrxt, int ns,
            double alatud, double hrnoon, double[] sunhor, double[] tmpday, double[] winday, double[] humday, double[] precip, double[] snoden, double[] soitmp,
            double[] vlcday, double[][] soilxt, ref int nplant, double[] plthgt, double[] dchar, double[] clumpng, double[] pltwgt, double[] pltlai, double[] rootdp,
            ref double zrthik, ref double rload, ref double cover, ref double albres, ref double rescof, ref double restkb,
            int totalDays, int dayCounter, List<double[]> dailySoilTemperature, double vlcdayBcInit)
        {
            //
            //     THIS SUBROUTINE READS THE TIME VARYING DATA REQUIRED TO RUN THE
            //     PROGRAM, AND STORES IT IN ARRAYS WITH VALUES FOR THE END OF EACH
            //     HOUR.  DATA SUCH AS SOIL TEMPERATURE AND MOISTURE CONTENT AT
            //     DEPTH, WHICH ARE SELDOM AVAILABLE AT EVERY HOUR, ARE
            //     INTERPOLATED TO GET HOURLY VALUES.
            //
            //***********************************************************************

            const double thresh = 0.5;  // THRESHHOLD WINDSPEED IS ASSUMED TO BE 0.5 M/S

            List<string> t;
            //var generalOut = _outputWriters[OutputFile.EntireProfile];

            var ratio = 0.0;
            var daylst = 0.0;
            var jyr1 = 0;
            var jhr1 = 0;
            var jdy1 = 0;
            var ltmpmx = 0;
            var nstep1 = 0;
            var j = 0;
            var lasthr = 0;
            var days = 0.0;
            var lvlcmx = 0;
            var nstep2 = 0;
            var inhr = 0;
            var isnow = 0;
            var jyr = 0;
            var jd = 0;
            var jh = 0;


            // JM: assign pltlai based on julian and LeafOn/LeafOffDay
            var halfWindow = 15.0; // leaf turn on window.
            for (var i = 1; i <= _nplant; ++i)
            {
                if (julian < _pltLeafOn[i] - halfWindow)
                    pltlai[i] = 0.0;
                else if (julian <= _pltLeafOn[i] + halfWindow)
                    pltlai[i] = 0.5 * ((julian - _pltLeafOn[i]) / halfWindow + 1.0) * _pltlaiOn[i];
                else if (julian <= _pltLeafOff[i] - halfWindow)
                    pltlai[i] = _pltlaiOn[i];
                else if (julian <= _pltLeafOff[i] + halfWindow)
                    pltlai[i] = 0.5 * (1.0 - (julian - _pltLeafOff[i]) / halfWindow) * _pltlaiOn[i];
                else
                    pltlai[i] = 0.0;
            }

            //**** READ WEATHER DATA
            //
            //     CHECK FLAG (MTSTEP) FOR TYPE OF WEATHER DATA:  0 = HOURLY;
            //     1 = DAILY;   2 ==> DATA MATCHES NHRPDT
            if (mtstep == 1)
            {
                //       DAILY WEATHER INPUT
                if (!Day2Hr(julian, year, inital, ref sunhor, ref tmpday, ref winday, ref humday, ref precip, ref snoden, alatud, hrnoon))
                    return false;

                //       CONVERT WIND RUN (MILES) TO MPH IF INPUT ARE ENGLISH UNITS
                if (iflagsi == 0)
                {
                    for (var jj = 0; jj < winday.Length; ++jj)
                        winday[jj] = winday[jj] / 24.0;
                }
                _dayinpSave.Nstart = 1;
                _dayinpSave.Nstep = 1;
            }

        // JM: only support daily weather input
        /*
        else
        {
            //       SUB-DAILY INPUT (PROBABLY HOURLY)
            if (inital == 0)
            {
                //         FIRST TIME INTO SUBROUTINE - FIND CORRECT PLACE IN DATA SET
                //         WHICH IS HOUR NSTEP OF CURRENT DAY
                _dayinpSave.Nstep = 1;
                if (mtstep == 2) _dayinpSave.Nstep = nhrpdt;
                label10:;
                if (WeatherDataEOS)
                {
                    Console.WriteLine("CANNOT FIND WEATHER DATA FOR BEGINNING DAY");
                    generalOut.WriteLine("CANNOT FIND WEATHER DATA FOR BEGINNING DAY");
                    return false;
                }

                // READ(12,*,END=100) JD,JH,JYR,TMPDAY(NSTEP),WINDAY(NSTEP),HUMDAY(NSTEP),PRECIP(NSTEP),SNODEN(NSTEP),SUNHOR(NSTEP)
                t = ParseNextLine(InputFile.WeatherData);
                jd = int.Parse(t[0]); jh = int.Parse(t[1]); jyr = int.Parse(t[2]); tmpday[_dayinpSave.Nstep] = double.Parse(t[3]); winday[_dayinpSave.Nstep] = double.Parse(t[4]); humday[_dayinpSave.Nstep] = double.Parse(t[5]); precip[_dayinpSave.Nstep] = double.Parse(t[6]); snoden[_dayinpSave.Nstep] = double.Parse(t[7]); sunhor[_dayinpSave.Nstep] = double.Parse(t[8]);

                if (jh == 0 && _dayinpSave.Nstep == 24)
                {
                    //            OBSCURE CASE OF 24-HOUR (BUT NOT DAILY) WEATHER FILE;
                    //            WILL NEVER FIND HOUR 24 IF INPUT ASSUMES HOUR 0 AT MIDNIGHT
                    jh = 24;
                    jd = jd - 1;
                    if (jd == 0)
                    {
                        jyr = jyr - 1;
                        jd = 365;
                        if (jyr % 4 == 0) jd = 366;
                    }
                }
                if (jd != julian || jyr != year || jh != _dayinpSave.Nstep) goto label10;
                _dayinpSave.Nstart = 2 * _dayinpSave.Nstep;
            }
            else
            {
                //        SUB-DAILY INPUT (PROBABLY HOURLY); SET STARTING HOUR TO READ
                _dayinpSave.Nstart = _dayinpSave.Nstep;
            }
            //
            for (var i = _dayinpSave.Nstart; i <= 24; i += _dayinpSave.Nstep)
            {
                // READ (12,*) JD,JH,JYR,TMPDAY(I),WINDAY(I),HUMDAY(I),PRECIP(I),SNODEN(I),SUNHOR(I)
                t = ParseNextLine(InputFile.WeatherData);
                jd = int.Parse(t[0]); jh = int.Parse(t[1]); jyr = int.Parse(t[2]); tmpday[i] = double.Parse(t[3]); winday[i] = double.Parse(t[4]); humday[i] = double.Parse(t[5]); precip[i] = double.Parse(t[6]); snoden[i] = double.Parse(t[7]); sunhor[i] = double.Parse(t[8]);
            label20:;
            }
            //
            //       MAKE SURE IT IS AT THE RIGHT POINT IN THE DATA FILE
            if (jd != (julian + 1) || jh != 0)
            {
                //         CHECK FOR EXCEPTIONS -> END OF YEAR OR CALLING HOUR 0, HOUR 24
                if (jd == julian && jh == 24) goto label25;
                if (julian == maxjul && jd == 1 && jh == 0) goto label25;
                //         NOT AT THE CORRECT POINT IN THE DATA FILE
                Console.WriteLine(" ENCOUNTERED PROBLEMS READING HOURLY WEATHER DATA");
                Console.WriteLine($" FOR JULIAN DAY {julian} IN SUBROUTINE DAYINP");
                generalOut.WriteLine(" ENCOUNTERED PROBLEMS READING HOURLY WEATHER DATA");
                generalOut.WriteLine($" FOR JULIAN DAY {julian} IN SUBROUTINE DAYINP");
                return false;
            }
        }
        */
        //
        label25:;
            for (var i = _dayinpSave.Nstep; i <= 24; i += _dayinpSave.Nstep)
            {
                //        CONVERT HUMIDITY FROM % TO DECIMAL; PRECIP FROM INCHES TO
                //        METERS; AND WIND FROM MILES PER HOUR TO M/S
                humday[i] = humday[i] / 100.0;
                if (iflagsi == 0)
                {
                    //           ENGLISH UNITS FOR PRECIP AND WIND - CONVERT TO SI
                    precip[i] = precip[i] * .0254;
                    winday[i] = winday[i] * 0.447;
                }
                else
                {
                    //           SI UNITS FOR WEATHER INPUT - CONVERT PRECIP FROM MM TO M
                    precip[i] = precip[i] / 1000.0;
                }
                //        SET WIND TO MINIMUM THRESHHOLD VALUE TO AVOID ZERO WINDSPEED
                if (winday[i] < thresh) winday[i] = thresh;
                label27:;
            }

            //     AVERAGE HOURLY WEATHER DATA IF TIME STEPS ARE LARGER THAN 1 HOUR
            if (nhrpdt > 1 && _dayinpSave.Nstep == 1)
            {
                for (var i = nhrpdt; i <= 24; i += nhrpdt)
                {
                    isnow = 0;
                    tmpday[i] = tmpday[i] / nhrpdt;
                    winday[i] = winday[i] / nhrpdt;
                    humday[i] = humday[i] / nhrpdt;
                    if (snoden[i] * precip[i] > 0)
                    {
                        isnow = 1;
                    }
                    else
                    {
                        snoden[i] = 1.0;
                    }
                    snoden[i] = snoden[i] * precip[i];
                    sunhor[i] = sunhor[i] / nhrpdt;
                    //           SUM UP ALL HOURS WITHIN TIME STEP
                    for (j = i - nhrpdt + 1; j <= i - 1; ++j)
                    {
                        tmpday[i] = tmpday[i] + tmpday[j] / nhrpdt;
                        winday[i] = winday[i] + winday[j] / nhrpdt;
                        humday[i] = humday[i] + humday[j] / nhrpdt;
                        precip[i] = precip[i] + precip[j];
                        if (snoden[j] * precip[j] > 0)
                        {
                            isnow = 1;
                        }
                        else
                        {
                            snoden[j] = 1.0;
                        }
                        snoden[i] = snoden[i] + snoden[j] * precip[j];
                        sunhor[i] = sunhor[i] + sunhor[j] / nhrpdt;
                    label30:;
                    }
                    if (isnow == 0 || precip[i] < 0.0001)
                    {
                        snoden[i] = 0.0;
                    }
                    else
                    {
                        snoden[i] = snoden[i] / precip[i];
                    }
                label35:;
                }
            }

            //inhr = inhour;
            //nstep2 = 1;

            //**** READ MOISTURE CONTENT DATA FOR THE LOWER BOUNDARY CONDITION
            //
            //     SKIP BOUNDARY CONDITIONS IF NOT NECESSARY
            if (ivlcbc == 0 || lvlout2 > 0)
            {
                Console.WriteLine(" PROBLEM ENCOUNTERED IN SUBROUTINE DAYINP:");
                Console.WriteLine(" IVLCBC equals 0 or LVLOUT[2] > 0.  Must have IVLCBC equal to 1.");
                _outputWriters[OutputFile.EntireProfile].WriteLine(" PROBLEM ENCOUNTERED IN SUBROUTINE DAYINP:");
                _outputWriters[OutputFile.EntireProfile].WriteLine(" IVLCBC equals 0 or LVLOUT[2] > 0.  Must have IVLCBC equal to 1.");
                return false;

                /*
                if (inital == 0)
                {
                    _dayinpSave.Lvlcdy = julian;
                    _dayinpSave.Lvlchr = inhour;
                    _dayinpSave.Lvlcyr = year;
                    if (inhour == 0)
                    {
                        //           CANNOT USE ARRAY ELEMENT ZERO; HAS BEEN STORED IN 24
                        _dayinpSave.Vlclst = vlcday[24];
                    }
                    else
                    {
                        _dayinpSave.Vlclst = vlcday[inhour];
                    }
                }
                else
                {
                    _dayinpSave.Vlclst = vlcday[24];
                }
                inhr = inhour;
                nstep2 = 1;
            label60:;
                if (_dayinpSave.Lvlcdy == julian && _dayinpSave.Lvlchr == inhr && _dayinpSave.Lvlcyr == year)
                {
                    //        DAY AND HOUR ARE SAME AS THE LAST DAY AND HOUR READ FOR
                    //        MOISTURE DATA - READ MOISTURE DATA FOR NEXT SAMPLING DATE;
                    //        BUT FIRST SAVE THE DATA FOR THE LAST SAMPLING DATE READ SO
                    //        SUBROUTINE 'OUTPUT' CAN PRINT IT OUT WITH PREDICTED VALUES
                    if (inital != 0)
                    {
                        //           IF INITAL =0, VLCMES ALREADY SET IN 'INPUT'
                        _measur.Meashr = _dayinpSave.Lvlchr;
                        _measur.Measdy = _dayinpSave.Lvlcdy;
                        for (var i = 1; i <= ns; ++i)
                        {
                            _measur.Vlcmes[i] = _dayinpSave.Vlc1[i];
                        label65:;
                        }
                    }

                    // read(13,*)lvlcdy,lvlchr,lvlcyr,(vlc1(i),i=1,ns)
                    t = ParseNextLine(InputFile.MoistureProfiles);
                    _dayinpSave.Lvlcdy = int.Parse(t[0]); _dayinpSave.Lvlchr = int.Parse(t[1]); _dayinpSave.Lvlcyr = int.Parse(t[2]);
                    for (var i = 1; i <= ns; ++i)
                        _dayinpSave.Vlc1[i] = double.Parse(t[2 + i]);

                    if (_dayinpSave.Lvlchr == 24)
                    {
                        //           ALGORITHM ASSUMES HOUR 24 IS HOUR 0 -- ADJUST DAY AND HOUR
                        _dayinpSave.Lvlchr = 0;
                        _dayinpSave.Lvlcdy = _dayinpSave.Lvlcdy + 1;
                        lvlcmx = 365;
                        if (_dayinpSave.Lvlcyr % 4 == 0) lvlcmx = 366;
                        if (_dayinpSave.Lvlcdy > lvlcmx)
                        {
                            _dayinpSave.Lvlcdy = _dayinpSave.Lvlcdy - lvlcmx;
                            _dayinpSave.Lvlcyr = _dayinpSave.Lvlcyr + 1;
                        }
                    }
                    if (_dayinpSave.Lvlcdy == julian + 1 && _dayinpSave.Lvlchr == 0)
                    {
                        //           THIS IS THE END OF CURRENT DAY - SAVE FOR OUTPUT
                        _measur.Meashr = _dayinpSave.Lvlchr;
                        _measur.Measdy = _dayinpSave.Lvlcdy;
                        for (var i = 1; i <= ns; ++i)
                        {
                            _measur.Vlcmes[i] = _dayinpSave.Vlc1[i];
                        label66:;
                        }
                    }
                    _dayinpSave.Vlc = _dayinpSave.Vlc1[ns];
                }
                days = (_dayinpSave.Lvlcyr - year) * maxjul + _dayinpSave.Lvlcdy - julian;
                lasthr = _dayinpSave.Lvlchr;
                if (days > 0)
                {
                    lasthr = 24;
                    //        ALLOW FOR STEPPING MORE THAN 1 HOUR IF NO DATA ON THIS DAY
                    if (inhr == inhour) nstep2 = nhrpdt;
                }
                else
                {
                    if (days < 0 || (days == 0 && lasthr < inhr))
                    {
                        Console.WriteLine(" PROBLEM ENCOUNTERED IN SUBROUTINE DAYINP:");
                        Console.WriteLine(" SOIL WATER CONTENT DATA IS NOT CHRONOLOGICAL");
                        Console.WriteLine($" AT JULIAN DAY {_dayinpSave.Lvlcdy,4:D} HOUR {_dayinpSave.Lvlchr,4:D} YEAR{_dayinpSave.Lvlcyr,6:D}");
                        generalOut.WriteLine(" PROBLEM ENCOUNTERED IN SUBROUTINE DAYINP:");
                        generalOut.WriteLine(" SOIL WATER CONTENT DATA IS NOT CHRONOLOGICAL");
                        generalOut.WriteLine($" AT JULIAN DAY {_dayinpSave.Lvlcdy,4:D} HOUR {_dayinpSave.Lvlchr,4:D} YEAR{_dayinpSave.Lvlcyr,6:D}");
                        return false;
                    }
                }
                //     INTERPOLATE FOR EACH HOUR OF DAY BETWEEN THE CURRENT HOUR AND
                //     THE LAST SAMPLING DATE READ.
                j = lasthr;
                for (var i = inhr + nstep2; i <= lasthr; i += nstep2)
                {
                    j = i;
                    vlcday[i] = _dayinpSave.Vlclst + (i - inhr) * (_dayinpSave.Vlc - _dayinpSave.Vlclst) / (days * 24 + _dayinpSave.Lvlchr - inhr);
                label70:;
                }
                inhr = j;
                _dayinpSave.Vlclst = vlcday[inhr];
                if (lasthr != 24)
                {
                    //        HAVE NOT REACH END OF CURRENT DAY - GO BACK AND READ DATA AGAIN
                    goto label60;
                }
                //     AVERAGE HOURLY VALUES IF TIME STEPS ARE LARGER THAN 1 HOUR AND
                //     DATA WERE GIVEN DURING MIDDLE OF CURRENT DAY
                if (nhrpdt > 1 && nstep2 == 1)
                {
                    for (var i = inhour + nhrpdt; i <= 24; i += nhrpdt)
                    {
                        vlcday[i] = vlcday[i] / nhrpdt;
                        for (j = i - nhrpdt + 1; j <= i - 1; ++j)
                        {
                            vlcday[i] = vlcday[i] + vlcday[j] / nhrpdt;
                        label74:;
                        }
                    label75:;
                    }
                }
                */
            }

            //**** READ TEMPERATURE DATA FOR LOWER BOUNDARY CONDITION
            //
            //     SKIP BOUNDARY CONDITIONS IF NOT NECESSARY
            if (itmpbc == 0 || lvlout2 > 0)
            {
                // JM: linearly interpolate lower boundary conditions based on days and dayCounter
                //soitmp[24] = soilTemperatureBcInit + (soilTemperatureFinal - soilTemperatureBcInit) * dayCounter / totalDays;

                // use the bottom of the soil temperature profile for this day as the lower boundary condition
                soitmp[24] = dailySoilTemperature[dayCounter - 1].Last();

                /*
                if (inital == 0)
                {
                    _dayinpSave.Ltmpdy = julian;
                    _dayinpSave.Ltmphr = inhour;
                    _dayinpSave.Ltmpyr = year;
                    if (inhour == 0)
                    {
                        //           CANNOT USE ARRAY ELEMENT ZERO; HAS BEEN STORED IN 24
                        _dayinpSave.Tmplst = soitmp[24];
                    }
                    else
                    {
                        _dayinpSave.Tmplst = soitmp[inhour];
                    }
                }
                else
                {
                    _dayinpSave.Tmplst = soitmp[24];
                }
                inhr = inhour;
                nstep1 = 1;
            label40:;
                if (_dayinpSave.Ltmpdy == julian && _dayinpSave.Ltmphr == inhr && _dayinpSave.Ltmpyr == year)
                {
                    //        DAY AND HOUR ARE SAME AS THE LAST DAY AND HOUR READ FOR
                    //        TEMPERATURE DATA - READ TEMPERATUE DATA FOR NEXT SAMPLING DATE;
                    //        BUT FIRST SAVE THE LAST SAMPLING DATE SO IT CAN PRINTED
                    //        OUT ALONG WITH THE MOISTURE DATA IN SUBROUTINE 'OUTPUT'
                    if (_dayinpSave.Ltmpdy == _measur.Measdy && _dayinpSave.Ltmphr == _measur.Meashr)
                    {
                        //           SAVE TEMPERATURE DATA FOR PROFILE OUTPUT FILE (THIS CATCHES
                        //            INPUT PROFILE IF NEXT OBSERVATION COMES AFTER CURRENT DAY)
                        for (var i = 1; i <= ns; ++i)
                        {
                            _measur.Tsmeas[i] = _dayinpSave.Tmp1[i];
                        label45:;
                        }
                    }

                    // READ (14,*) LTMPDY,LTMPHR,LTMPYR,(TMP1(I), I=1,NS)
                    t = ParseNextLine(InputFile.TemperatureProfiles);
                    _dayinpSave.Ltmpdy = int.Parse(t[0]); _dayinpSave.Ltmphr = int.Parse(t[1]); _dayinpSave.Ltmpyr = int.Parse(t[2]);
                    for (var i = 1; i <= ns; ++i)
                        _dayinpSave.Tmp1[i] = double.Parse(t[2 + i]);
                    if (_dayinpSave.Ltmphr == 24)
                    {
                        //           ALGORITHM ASSUMES HOUR 24 IS HOUR 0 -- ADJUST DAY AND HOUR
                        _dayinpSave.Ltmphr = 0;
                        _dayinpSave.Ltmpdy = _dayinpSave.Ltmpdy + 1;
                        ltmpmx = 365;
                        if (_dayinpSave.Ltmpyr % 4 == 0) ltmpmx = 366;
                        if (_dayinpSave.Ltmpdy > ltmpmx)
                        {
                            _dayinpSave.Ltmpdy = _dayinpSave.Ltmpdy - ltmpmx;
                            _dayinpSave.Ltmpyr = _dayinpSave.Ltmpyr + 1;
                        }
                    }
                    if (_dayinpSave.Ltmpdy == _measur.Measdy && _dayinpSave.Ltmphr == _measur.Meashr)
                    {
                        //           SAVE TEMPERATURE DATA FOR PROFILE OUTPUT FILE (THIS CATCHES
                        //           INPUT PROFILE IF IT COMES AT END OF CURRENT DAY)
                        for (var i = 1; i <= ns; ++i)
                        {
                            _measur.Tsmeas[i] = _dayinpSave.Tmp1[i];
                        label46:;
                        }
                    }
                    _dayinpSave.Tmp = _dayinpSave.Tmp1[ns];
                }
                days = (_dayinpSave.Ltmpyr - year) * maxjul + _dayinpSave.Ltmpdy - julian;
                lasthr = _dayinpSave.Ltmphr;
                if (days > 0)
                {
                    lasthr = 24;
                    //        ALLOW FOR STEPPING MORE THAN 1 HOUR IF NO DATA ON THIS DAY
                    if (inhr == inhour) nstep1 = nhrpdt;
                }
                else
                {
                    if (days < 0 || (days == 0 && lasthr < inhr))
                    {
                        Console.WriteLine(" PROBLEM ENCOUNTERED IN SUBROUTINE DAYINP:");
                        Console.WriteLine(" SOIL TEMPERATURE DATA IS NOT CHRONOLOGICAL");
                        Console.WriteLine($" AT JULIAN DAY {_dayinpSave.Ltmpdy,4:D} HOUR {_dayinpSave.Ltmphr,4:D} YEAR{_dayinpSave.Ltmpyr,6:D}");
                        generalOut.WriteLine(" PROBLEM ENCOUNTERED IN SUBROUTINE DAYINP:");
                        generalOut.WriteLine(" SOIL TEMPERATURE DATA IS NOT CHRONOLOGICAL");
                        generalOut.WriteLine($" AT JULIAN DAY {_dayinpSave.Ltmpdy,4:D} HOUR {_dayinpSave.Ltmphr,4:D} YEAR{_dayinpSave.Ltmpyr,6:D}");
                        return false;
                    }
                }
                //     INTERPOLATE FOR EACH HOUR OF DAY BETWEEN THE CURRENT HOUR AND
                //     THE LAST SAMPLING DATE READ.
                j = lasthr;
                for (var i = inhr + nstep1; i <= lasthr; i += nstep1)
                {
                    j = i;
                    soitmp[i] = _dayinpSave.Tmplst + (i - inhr) * (_dayinpSave.Tmp - _dayinpSave.Tmplst) / (days * 24 + _dayinpSave.Ltmphr - inhr);
                label50:;
                }
                inhr = j;
                _dayinpSave.Tmplst = soitmp[inhr];
                if (lasthr != 24)
                {
                    //        HAVE NOT REACH END OF CURRENT DAY - GO BACK AND READ DATA AGAIN
                    goto label40;
                }
                //     AVERAGE HOURLY VALUES IF TIME STEPS ARE LARGER THAN 1 HOUR AND
                //     DATA WERE GIVEN DURING MIDDLE OF CURRENT DAY
                if (nhrpdt > 1 && nstep1 == 1)
                {
                    for (var i = inhour + nhrpdt; i <= 24; i += nhrpdt)
                    {
                        soitmp[i] = soitmp[i] / nhrpdt;
                        for (j = i - nhrpdt + 1; j <= i - 1; ++j)
                        {
                            soitmp[i] = soitmp[i] + soitmp[j] / nhrpdt;
                        label54:;
                        }
                    label55:;
                    }
                }
                */
            }

            //***  READ SOIL SINK DATA
            if (mwatrxt > 0)
            {
                if (inital == 0)
                {
                    //        FIRST TIME INTO SUBROUTINE - FIND CORRECT PLACE IN DATA SET
                    _dayinpSave.Lwtrdy = julian;
                    _dayinpSave.Lwtrhr = inhour;
                    _dayinpSave.Lwtryr = year;
                    _dayinpSave.Lwtrmx = maxjul;
                    jdy1 = julian;
                    jhr1 = inhour;
                    jyr1 = year;
                label80:;

                    // READ (15,*,END=102) JDY,JHR,JYR,(FLUX(J),J=1,NS)
                    t = ParseNextLine(InputFile.WaterExtraction);
                    var jdy = int.Parse(t[0]); var jhr = int.Parse(t[1]); jyr = int.Parse(t[2]);
                    for (j = 1; j <= ns; ++j)
                        _dayinpSave.Flux[j] = double.Parse(t[2 + j]);

                    days = (jyr - _dayinpSave.Lwtryr) * _dayinpSave.Lwtrmx + jdy - _dayinpSave.Lwtrdy + (jhr - _dayinpSave.Lwtrhr) / 24.0;
                    if (days <= 0)
                    {
                        jyr1 = jyr;
                        jdy1 = jdy;
                        jhr1 = jhr;
                        goto label80;
                    }
                    days = (jyr - jyr1) * _dayinpSave.Lwtrmx + jdy - jdy1 + (jhr - jhr1) / 24.0;
                    for (j = 1; j <= ns; ++j)
                    {
                        _dayinpSave.Flux[j] = _dayinpSave.Flux[j] / days / 24.0 / 3600.0;
                    label82:;
                    }
                    _dayinpSave.Lwtrdy = jdy;
                    _dayinpSave.Lwtrhr = jhr;
                    _dayinpSave.Lwtryr = jyr;
                }
                for (var i = inhour + nhrpdt; i <= 24; i += nhrpdt)
                {
                label84:;
                    days = (year - _dayinpSave.Lwtryr) * _dayinpSave.Lwtrmx + julian - _dayinpSave.Lwtrdy + (i - _dayinpSave.Lwtrhr) / 24.0;
                    if (days > 0.0)
                    {
                        //           TIME IS NOW PAST LAST LINE OF DATA READ -- READ DATA AGAIN
                        // READ (15,*) JDY,JHR,JYR,(FLUX(J),J=1,NS)
                        t = ParseNextLine(InputFile.WaterExtraction);
                        var jdy = int.Parse(t[0]); var jhr = int.Parse(t[1]); jyr = int.Parse(t[2]);
                        for (j = 1; j <= ns; ++j)
                            _dayinpSave.Flux[j] = double.Parse(t[2 + j]);

                        days = (jyr - _dayinpSave.Lwtryr) * _dayinpSave.Lwtrmx + jdy - _dayinpSave.Lwtrdy + (jhr - _dayinpSave.Lwtrhr) / 24.0;
                        for (j = 1; j <= ns; ++j)
                        {
                            _dayinpSave.Flux[j] = _dayinpSave.Flux[j] / days / 24.0 / 3600.0;
                        label85:;
                        }
                        _dayinpSave.Lwtrdy = jdy;
                        _dayinpSave.Lwtrhr = jhr;
                        _dayinpSave.Lwtryr = jyr;
                        _dayinpSave.Lwtrmx = 365;
                        if (_dayinpSave.Lwtryr % 4 == 0) _dayinpSave.Lwtrmx = 366;
                        goto label84;
                    }
                    for (j = 1; j <= ns; ++j)
                    {
                        soilxt[j][i] = _dayinpSave.Flux[j];
                    label88:;
                    }
                label90:;
                }
            }

            //***  READ CANOPY INFORMATION
            //
            if (nplant > 0 && mpltgro == 1)
            {
                if (inital == 0)
                {
                    //        FIRST TIME INTO SUBROUTINE - FIND CORRECT PLACE IN DATA SET
                    for (j = 1; j <= nplant; ++j)
                    {
                        _dayinpSave.Iflagc[j] = -1;
                        daylst = 0.0;
                    label210:;
                        if (_canopyReaders[j].EndOfStream)
                        {
                            Console.WriteLine($" INITIAL CONDITIONS FOR PLANT #{j,2:D} CANNOT BE INTERPOLATED FROM PLANT GROWTH FILE");
                            if (_lvlout[1] > 0)
                                _outputWriters[OutputFile.EntireProfile].WriteLine($" INITIAL CONDITIONS FOR PLANT #{j,2:D} CANNOT BE INTERPOLATED FROM PLANT GROWTH FILE");
                            return false;
                        }

                        t = ParseNextLine(_canopyReaders[j]);
                        if (iversion == 2)
                        {
                            _dayinpSave.Lcandy[j] = int.Parse(t[0]); _dayinpSave.Lcanyr[j] = int.Parse(t[1]); _dayinpSave.Zclst[j] = double.Parse(t[2]); _dayinpSave.Dchlst[j] = double.Parse(t[3]); _dayinpSave.Wlst[j] = double.Parse(t[4]); _dayinpSave.Tlalst[j] = double.Parse(t[5]); _dayinpSave.Rdplst[j] = double.Parse(t[6]);
                            _dayinpSave.Clmplst[j] = 1.0;
                        }
                        else
                        {
                            _dayinpSave.Lcandy[j] = int.Parse(t[0]); _dayinpSave.Lcanyr[j] = int.Parse(t[1]); _dayinpSave.Zclst[j] = double.Parse(t[2]); _dayinpSave.Dchlst[j] = double.Parse(t[3]); _dayinpSave.Clmplst[j] = double.Parse(t[4]); _dayinpSave.Wlst[j] = double.Parse(t[5]); _dayinpSave.Tlalst[j] = double.Parse(t[6]); _dayinpSave.Rdplst[j] = double.Parse(t[7]);
                        }
                        //           CONVERT LEAF DIMENSION FROM CM TO METERS
                        _dayinpSave.Dchlst[j] = _dayinpSave.Dchlst[j] / 100.0;
                        days = (_dayinpSave.Lcanyr[j] - year) * maxjul + _dayinpSave.Lcandy[j] - julian;
                        if (days <= 0)
                        {
                            //              SAVE LAST VALUES READ IN
                            daylst = -days;
                            plthgt[j] = _dayinpSave.Zclst[j];
                            dchar[j] = _dayinpSave.Dchlst[j];
                            clumpng[j] = _dayinpSave.Clmplst[j];
                            pltwgt[j] = _dayinpSave.Wlst[j];
                            pltlai[j] = _dayinpSave.Tlalst[j];
                            rootdp[j] = _dayinpSave.Rdplst[j];
                            _dayinpSave.Iflagc[j] = 0;
                            goto label210;
                        }
                        else
                        {
                            //              INTERPOLATE VEGETATION DATA FOR STARTING DAY
                            //              (CHECK IF DATA START ON OR BEFORE STARTING DAY
                            if (_dayinpSave.Iflagc[j] < 0)
                            {
                                Console.WriteLine($" INITIAL CONDITIONS FOR PLANT #{j,2:D} CANNOT BE INTERPOLATED FROM PLANT GROWTH FILE");
                                if (_lvlout[1] > 0)
                                    _outputWriters[OutputFile.EntireProfile].WriteLine($" INITIAL CONDITIONS FOR PLANT #{j,2:D} CANNOT BE INTERPOLATED FROM PLANT GROWTH FILE");
                                return false;
                            }
                            _dayinpSave.Iflagc[j] = 0;
                            ratio = daylst / (days + daylst);
                            plthgt[j] = plthgt[j] + (_dayinpSave.Zclst[j] - plthgt[j]) * ratio;
                            dchar[j] = dchar[j] + (_dayinpSave.Dchlst[j] - dchar[j]) * ratio;
                            clumpng[j] = clumpng[j] + (_dayinpSave.Clmplst[j] - clumpng[j]) * ratio;
                            pltwgt[j] = pltwgt[j] + (_dayinpSave.Wlst[j] - pltwgt[j]) * ratio;
                            pltlai[j] = pltlai[j] + (_dayinpSave.Tlalst[j] - pltlai[j]) * ratio;
                            rootdp[j] = rootdp[j] + (_dayinpSave.Rdplst[j] - rootdp[j]) * ratio;
                        }
                    label212:;
                    }
                }
                else
                {
                    //
                    for (j = 1; j <= nplant; ++j)
                    {
                        //           CHECK IF WE HAVE REACHED END OF VEGETATION DATA FILE
                        if (_dayinpSave.Iflagc[j] == 1) goto label215;
                        //
                        //           INTERPOLATE BETWEEN CURRENT DAY AND NEXT DAY W/ PLANT DATA -
                        //           CALCULATE # OF DAYS TO NEXT DAY WITH PLANT DATA
                        days = (_dayinpSave.Lcanyr[j] - year) * maxjul + _dayinpSave.Lcandy[j] - julian;
                        //
                        //           CHECK TIME COMPARED TO DATA -- INCLUDE COMPARISON OF YEAR
                        //           FOR SITUATION WHEN INPUT DATA INCLUDE DAYS 366 AND DAY 1 AT
                        //           THE END OF A LEAP YEAR.  (MAXJUL PERTAINS TO YEAR RATHER
                        //           THAN LCANYR)
                        if (days < 0.0 || _dayinpSave.Lcanyr[j] < year)
                        {
                            //               TIME IS NOW PAST LAST LINE OF DATA READ - READ DATA AGAIN
                            if (_canopyReaders[j].EndOfStream)
                            {
                                // END OF VEGETATION DATA FILE -- ISSUE WARNING AND SET FLAG
                                Console.WriteLine($" *** LAST DATA FOR GROWTH OF PLANT #{j,1:D} WAS ON DAY{julian,4:D} OF YEAR {year,4:D} ***'");
                                _outputWriters[OutputFile.EntireProfile].WriteLine($" *** LAST DATA FOR GROWTH OF PLANT #{j,1:D} WAS ON DAY{julian,4:D} OF YEAR {year,4:D} ***'");
                                _dayinpSave.Iflagc[j] = 1;
                                continue;
                            }

                            t = ParseNextLine(_canopyReaders[j]);
                            if (iversion == 2)
                            {
                                _dayinpSave.Lcandy[j] = int.Parse(t[0]); _dayinpSave.Lcanyr[j] = int.Parse(t[1]); _dayinpSave.Zclst[j] = double.Parse(t[2]); _dayinpSave.Dchlst[j] = double.Parse(t[3]); _dayinpSave.Wlst[j] = double.Parse(t[4]); _dayinpSave.Tlalst[j] = double.Parse(t[5]); _dayinpSave.Rdplst[j] = double.Parse(t[6]);
                                clumpng[j] = 1.0;
                            }
                            else
                            {
                                _dayinpSave.Lcandy[j] = int.Parse(t[0]); _dayinpSave.Lcanyr[j] = int.Parse(t[1]); _dayinpSave.Zclst[j] = double.Parse(t[2]); _dayinpSave.Dchlst[j] = double.Parse(t[3]); _dayinpSave.Clmplst[j] = double.Parse(t[4]); _dayinpSave.Wlst[j] = double.Parse(t[5]); _dayinpSave.Tlalst[j] = double.Parse(t[6]); _dayinpSave.Rdplst[j] = double.Parse(t[7]);
                            }
                            //              CONVERT LEAF DIMENSION FROM CM TO METERS
                            _dayinpSave.Dchlst[j] = _dayinpSave.Dchlst[j] / 100.0;
                            days = (_dayinpSave.Lcanyr[j] - year) * maxjul + _dayinpSave.Lcandy[j] - julian;
                        }
                        plthgt[j] = plthgt[j] + (_dayinpSave.Zclst[j] - plthgt[j]) / (days + 1);
                        dchar[j] = dchar[j] + (_dayinpSave.Dchlst[j] - dchar[j]) / (days + 1);
                        clumpng[j] = clumpng[j] + (_dayinpSave.Clmplst[j] - clumpng[j]) / (days + 1);
                        pltwgt[j] = pltwgt[j] + (_dayinpSave.Wlst[j] - pltwgt[j]) / (days + 1);
                        pltlai[j] = pltlai[j] + (_dayinpSave.Tlalst[j] - pltlai[j]) / (days + 1);
                        rootdp[j] = rootdp[j] + (_dayinpSave.Rdplst[j] - rootdp[j]) / (days + 1);
                    label215:;
                    }
                }
            }

            //***  READ RESIDUE INFORMATION
            //
            if (nrchang > 0)
            {
                if (inital == 0)
                {
                    //        FIRST TIME INTO SUBROUTINE - FIND CORRECT PLACE IN DATA SET
                    _dayinpSave.Iflagr = -1;
                    daylst = 0.0;
                label310:;
                    if (_residueReader.EndOfStream)
                    {
                        Console.WriteLine(" INITIAL CONDITIONS FOR RESIDUE COVER CANNOT BE INTERPOLATED FROM RESIDUE FILE");
                        if (_lvlout[1] > 0)
                            _outputWriters[OutputFile.EntireProfile].WriteLine(" INITIAL CONDITIONS FOR RESIDUE COVER CANNOT BE INTERPOLATED FROM RESIDUE FILE");
                        return false;
                    }
                    t = ParseNextLine(_residueReader);
                    _dayinpSave.Lresdy = double.Parse(t[0]); _dayinpSave.Lresyr = double.Parse(t[1]); _dayinpSave.Zrlst = double.Parse(t[2]); _dayinpSave.Rllst = double.Parse(t[3]); _dayinpSave.Covlst = double.Parse(t[4]); _dayinpSave.Alblst = double.Parse(t[5]); _dayinpSave.Resclst = double.Parse(t[6]); _dayinpSave.Restlst = double.Parse(t[7]);

                    //        CONVERT RLOAD FROM KG/HA TO KG/M2
                    _dayinpSave.Rllst = _dayinpSave.Rllst / 10000.0;
                    //        CONVERT ZRTHIK FROM CM TO M
                    _dayinpSave.Zrlst = _dayinpSave.Zrlst / 100.0;
                    days = (_dayinpSave.Lresyr - year) * maxjul + _dayinpSave.Lresdy - julian;
                    if (days <= 0)
                    {
                        //           SAVE LAST VALUES READ IN
                        daylst = -days;
                        zrthik = _dayinpSave.Zrlst;
                        rload = _dayinpSave.Rllst;
                        cover = _dayinpSave.Covlst;
                        albres = _dayinpSave.Alblst;
                        rescof = _dayinpSave.Resclst;
                        restkb = _dayinpSave.Restlst;
                        _dayinpSave.Iflagr = 0;
                        goto label310;
                    }
                    else
                    {
                        //           INTERPOLATE VEGETATION DATA FOR STARTING DAY
                        //           (CHECK IF DATA START ON OR BEFORE STARTING DAY
                        if (_dayinpSave.Iflagr < 0)
                        {
                            Console.WriteLine(" INITIAL CONDITIONS FOR RESIDUE COVER CANNOT BE INTERPOLATED FROM RESIDUE FILE");
                            if (_lvlout[1] > 0)
                                _outputWriters[OutputFile.EntireProfile].WriteLine(" INITIAL CONDITIONS FOR RESIDUE COVER CANNOT BE INTERPOLATED FROM RESIDUE FILE");
                            return false;
                        }
                        _dayinpSave.Iflagr = 0;
                        ratio = daylst / (days + daylst);
                        zrthik = zrthik + (_dayinpSave.Zrlst - zrthik) * ratio;
                        rload = rload + (_dayinpSave.Rllst - rload) * ratio;
                        cover = cover + (_dayinpSave.Covlst - cover) * ratio;
                        albres = albres + (_dayinpSave.Alblst - albres) * ratio;
                        rescof = rescof + (_dayinpSave.Resclst - rescof) * ratio;
                        restkb = restkb + (_dayinpSave.Restlst - restkb) * ratio;
                    }
                }
                else
                {
                    //
                    //        CHECK IF WE HAVE REACHED END OF RESIDUE DATA FILE
                    if (_dayinpSave.Iflagr == 1) goto label315;
                    //
                    //        INTERPOLATE BETWEEN CURRENT DAY AND NEXT DAY W/ RESIDUE DATA
                    //        CALCULATE # OF DAYS TO NEXT DAY WITH RESIDUE DATA
                    days = (_dayinpSave.Lresyr - year) * maxjul + _dayinpSave.Lresdy - julian;
                    //
                    //        CHECK TIME COMPARED TO DATA -- INCLUDE COMPARISON OF YEAR
                    //        FOR SITUATION WHEN INPUT DATA INCLUDE DAYS 366 AND DAY 1 AT
                    //        THE END OF A LEAP YEAR.  (MAXJUL PERTAINS TO YEAR RATHER
                    //        THAN LCANYR)
                    if (days < 0.0 || _dayinpSave.Lresyr < year)
                    {
                        //            TIME IS NOW PAST LAST LINE OF DATA READ - READ DATA AGAIN
                        if (_residueReader.EndOfStream)
                        {
                            Console.WriteLine($" *** LAST DATA FOR RESIDUE COVER WAS ON DAY{julian,4:D} OF YEAR {year,4:D} ***'");
                            if (_lvlout[1] > 0)
                                _outputWriters[OutputFile.EntireProfile].WriteLine($" *** LAST DATA FOR RESIDUE COVER WAS ON DAY{julian,4:D} OF YEAR {year,4:D} ***'");
                            _dayinpSave.Iflagr = 1;
                            return true;
                        }
                        t = ParseNextLine(_residueReader);
                        _dayinpSave.Lresdy = double.Parse(t[0]); _dayinpSave.Lresyr = double.Parse(t[1]); _dayinpSave.Zrlst = double.Parse(t[2]); _dayinpSave.Rllst = double.Parse(t[3]); _dayinpSave.Covlst = double.Parse(t[4]); _dayinpSave.Alblst = double.Parse(t[5]); _dayinpSave.Resclst = double.Parse(t[6]); _dayinpSave.Restlst = double.Parse(t[7]);
                        _dayinpSave.Rllst = _dayinpSave.Rllst / 10000.0;
                        //           CONVERT ZRTHIK FROM CM TO M
                        _dayinpSave.Zrlst = _dayinpSave.Zrlst / 100.0;
                        days = (_dayinpSave.Lresyr - year) * maxjul + _dayinpSave.Lresdy - julian;
                    }
                    zrthik = zrthik + (_dayinpSave.Zrlst - zrthik) / (days + 1);
                    rload = rload + (_dayinpSave.Rllst - rload) / (days + 1);
                    cover = cover + (_dayinpSave.Covlst - cover) / (days + 1);
                    albres = albres + (_dayinpSave.Alblst - albres) / (days + 1);
                    rescof = rescof + (_dayinpSave.Resclst - rescof) / (days + 1);
                    restkb = restkb + (_dayinpSave.Restlst - restkb) / (days + 1);
                label315:;
                }
            }

            return true;
        }

        // line 3349
        /// <summary>
        /// Converts daily input to hourly.
        /// </summary>
        /// <returns></returns>
        private bool Day2Hr(int julian, int year, int inital, ref double[] sunhor, ref double[] tmpday, ref double[] winday, ref double[] humday,
                    ref double[] precip, ref double[] snoden, double alatud, double hrnoon)
        {
            //
            //      THIS SUBROUTINE SEPARATES DAILY VALUES FOR MAXIMUM-MINIMUM AIR
            //      TEMPERATURE (C), DEW-POINT TEMPERATURE (C), WIND RUN (MILES),
            //      PRECIPITATION (INCHES) AND SOLAR RADIATION (W/M2) INTO HOURLY
            //      VALUES OF TEMPERATURE, WINDSPEED, RELATIVE HUMIDITY, PRECIPITATION
            //      AND SOLAR RADIATION.
            //
            //      THE INPUT FILE IS READ WITH OPEN FORMAT, AND DATA FOR EACH DAY IN
            //      THE INPUT FILE MUST BE IN THE FOLLOWING ORDER:
            //
            //      DAY, YEAR, MAX TEMP, MIN TEMP, DEW-POINT, WIND, PRECIP, SOLAR
            //
            //      For example:
            //
            //       65  87   12.3    4.6    2.6   109.0   0.09    38.9
            //       66  87   13.2    1.6    0.6    77.6   0.00   171.1
            //       67  87   15.8    1.4    1.4   125.0   0.06   168.8
            //       68  87   12.6    5.0    3.0   119.2   0.08   110.4
            //

            // todo: argh!!! more multiple declarations of constants
            const double pi = 3.14159;
            const double dtmin = -0.8;
            const double dtdsk = 1.7;
            const double wndnoon = 1.25;
            const double dtwind = 1.0;

            //List<string> t;
            ShawDammDailyWeatherRecord rec;
            //var generalOut = _outputWriters[OutputFile.EntireProfile];

            // read each line of the weather data
            if (inital == 0)
            {
                //_day2hrSave.Tmax = -999.0;
                //_day2hrSave.Tmin = 999.0;

                //         SAVE MAX AND MIN TEMPERATURE FROM PREVIOUS DAY TO CALCULATE
                //         PREVIOUS DAY'S DUSK TEMP AND INTERPOLATE MORNING TEMPERATURES
                //while (!WeatherDataEOS)
                //{
                //    _day2hrSave.Tmax1 = _day2hrSave.Tmax;
                //    _day2hrSave.Tmin1 = _day2hrSave.Tmin;
                // read(12,*,end=50)jday,jyr,tmax,tmin,tdew,wind,prec,solar
                //t = ParseNextLine(InputFile.WeatherData);
                //t = NextWeatherDatum;
                //_day2hrSave.Jday = int.Parse(t[0]); _day2hrSave.Jyr = int.Parse(t[1]); _day2hrSave.Tmax = double.Parse(t[2]); _day2hrSave.Tmin = double.Parse(t[3]); _day2hrSave.Tdew = double.Parse(t[4]); _day2hrSave.Wind = double.Parse(t[5]); _day2hrSave.Prec = double.Parse(t[6]); _day2hrSave.Solar = double.Parse(t[7]);
                rec = NextWeatherDatum;
                //_day2hrSave.Jday = rec.Day; _day2hrSave.Jyr = rec.Year;
                _day2hrSave.Tmax = rec.Tmax; _day2hrSave.Tmin = rec.Tmin; _day2hrSave.Tdew = rec.Tdew; _day2hrSave.Wind = rec.Wind; _day2hrSave.Prec = rec.Precip; _day2hrSave.Solar = rec.Solar;

                _day2hrSave.Tmax1 = _day2hrSave.Tmax;
                _day2hrSave.Tmin1 = _day2hrSave.Tmin;

                //    if (_day2hrSave.Jday == julian && _day2hrSave.Jyr == year)
                //        break;
                //}

                //if (WeatherDataEOS)
                //{
                //    Console.WriteLine(" ERROR READING DAILY WEATHER DATA IN SUBROUTINE DAY2HR:");
                //    Console.WriteLine(" YEAR AND/OR DAY DO NOT MATCH");
                //    generalOut.WriteLine(" ERROR READING DAILY WEATHER DATA IN SUBROUTINE DAY2HR:");
                //    generalOut.WriteLine(" YEAR AND/OR DAY DO NOT MATCH");
                //    return false;
                //}

                ////         CHECK IF TMAX1 IS SET TO PREVIOUS DAY'S MAX TEMP (IF START TIME
                ////         IS FIRST LINE OF DATA IN FILE, SET TMAX1 TO CURRENT DAY'S TEMP)
                //if (_day2hrSave.Tmax1 < -998.0) _day2hrSave.Tmax1 = _day2hrSave.Tmax;
                //if (_day2hrSave.Tmin1 > 998.0) _day2hrSave.Tmin1 = _day2hrSave.Tmin;
            }

            // line 3394
            // READ (12,*) JULAN3,YEAR3,TMAX3,TMIN3,TDEW3,WIND3,PREC3,SOLAR3
            //t = ParseNextLine(InputFile.WeatherData);
            //t = NextWeatherDatum;
            //var julan3 = int.Parse(t[0]); var year3 = int.Parse(t[1]); var tmax3 = double.Parse(t[2]); var tmin3 = double.Parse(t[3]); var tdew3 = double.Parse(t[4]); var wind3 = double.Parse(t[5]); var prec3 = double.Parse(t[6]); var solar3 = double.Parse(t[7]);
            rec = NextWeatherDatum;
            //var julan3 = rec.Day; var year3 = rec.Year;
            var tmax3 = rec.Tmax; var tmin3 = rec.Tmin; var tdew3 = rec.Tdew; var wind3 = rec.Wind; var prec3 = rec.Precip; var solar3 = rec.Solar;

            ////      MAKE SURE IT IS AT THE RIGHT POINT IN THE DATA FILE
            //if (_day2hrSave.Jday != julian || _day2hrSave.Jyr != year)
            //{
            //    //         NOT AT THE CORRECT POINT IN THE DATA FILE
            //    Console.WriteLine(" ENCOUNTERED PROBLEMS READING DAILY WEATHER DATA");
            //    Console.WriteLine($" AT JULIAN DAY {julian} IN SUBROUTINE DAY2HR");
            //    generalOut.WriteLine(" ENCOUNTERED PROBLEMS READING DAILY WEATHER DATA");
            //    generalOut.WriteLine($" AT JULIAN DAY {julian} IN SUBROUTINE DAY2HR");
            //    return false;
            //}

            // **** CALCULATE SUN'S DECLINATION ANGLE (DECLIN), HALF-DAY ANGLE
            //      (HAFDAY), MAXIMUM SOLAR RADIATION AT 100% TRANMISSIVITY (SUNMAX),
            //      TOTAL TRANSMISSIVITY TO RADIATION (TTOTAL), AND HOUR OF SUNRISE
            //      AND SUNSET
            var declin = 0.4102 * Math.Sin(2.0 * pi * (julian - 80) / 365.0);
            var coshaf = -Math.Tan(alatud) * Math.Tan(declin);
            double hafday;
            if (Math.Abs(coshaf) >= 1.0)
            {
                //  coshaf >= 1.0          SUN DOES NOT COME UP ON THIS DAY (WINTER IN ARCTIC CIRCLE)
                //  else                   SUN DOES NOT SET ON THIS DAY (SUMMER IN THE ARCTIC CIRCLE)
                hafday = coshaf >= 1.0 ? 0.0 : pi;
            }
            else
            {
                hafday = Math.Acos(coshaf);
            }

            var sunmax = Swrcoe.Solcon * (hafday * Math.Sin(alatud) * Math.Sin(declin) + Math.Cos(alatud) * Math.Cos(declin) * Math.Sin(hafday)) / pi;

            //         IF MAXIMUM SOLAR IS ZERO; ATMOSPHERE TRANSMISSIVITY IS UNDEFINED
            //         SET TTOTAL TO 1.0; ANY MEASURED SOLAR WILL BE DISTRIBUTED EVENLY
            var ttotal = sunmax > 0.0 ? _day2hrSave.Solar / sunmax : 1.0;
            var sunris = hrnoon - hafday / 0.261799;
            var sunset = hrnoon + hafday / 0.261799;
            var daylen = sunset - sunris;

            for (var hour = 1; hour <= 24; ++hour)
            {
                // **** SET HOUR TO MID-POINT OF THE HOUR, OR IF THE SUN RISES OR SETS IN
                //      THIS HOUR, SET HOUR TO MID-POINT OF THE HOUR AND TIME AT WHICH IT
                //      RISES OR SETS.
                var halfhr = hour - 0.5;
                if (daylen > 0.0)
                {
                    if (hour > sunris && hour - 1.0 < sunris) halfhr = (sunris + hour) / 2.0;
                    if (hour > sunset && hour - 1.0 < sunset) halfhr = sunset - (sunset - (hour - 1)) / 2.0;
                    //
                    // ****   DETERMINE THE GEOMETRY OF THE SUN ANGLE AT THE HOUR MID=POINT
                    var hrangl = 0.261799 * (halfhr - hrnoon);
                    var altitu = Math.Asin(Math.Sin(alatud) * Math.Sin(declin) + Math.Cos(alatud) * Math.Cos(declin) * Math.Cos(hrangl));
                    if (altitu > 0.0)
                    {
                        sunmax = Swrcoe.Solcon * Math.Sin(altitu);
                        //           ADJUST TRANSMISS. FOR TIME OF DAY: 1.1 AT NOON; 0.624 AT ENDS
                        var factor = 0.624 + 0.476 * Math.Cos((halfhr - hrnoon) * pi / daylen);
                        sunhor[hour] = ttotal * factor * sunmax;
                    }
                    else
                    {
                        sunhor[hour] = 0.0;
                    }
                }
                else
                {
                    //        SUN DOESN'T RISE - WINTER IN ARCTIC; DISPERSE SOLAR EVENLY
                    sunhor[hour] = _day2hrSave.Solar;
                }
            }

            //      FORCE THE AVERAGE ESTIMATED HOURLY SOLAR TO THE OBSERVED AVERAGE
            var sumswr = sunhor.Sum() / 24.0;
            if (sumswr > 0.0)
                for (var jj = 0; jj < 24; ++jj)
                    sunhor[jj] = _day2hrSave.Solar * sunhor[jj] / sumswr;

            // line 3474
            var timmin = sunris + dtmin;
            var timmax = (sunset + hrnoon) / 2.0;
            var day = timmax - timmin;
            var anight = timmin + (24.0 - sunset - dtdsk);
            var tmpdsk1 = 0.5 * (_day2hrSave.Tmax1 + _day2hrSave.Tmin1 + (_day2hrSave.Tmax1 - _day2hrSave.Tmin1) * Math.Cos(pi / day * (sunset + dtdsk - timmax)));
            var tmpdsk = 0.5 * (_day2hrSave.Tmax + _day2hrSave.Tmin + (_day2hrSave.Tmax - _day2hrSave.Tmin) * Math.Cos(pi / day * (sunset + dtdsk - timmax)));

            var s = 0.0;
            var vapor = 0.0;
            Vslope(ref s, ref vapor, ref _day2hrSave.Tdew);

            // **** INTERPOLATE TEMPERATURE BETWEEN MAXIMUM AND MINIMUM TEMPERATURE
            //      DEPENDING ON TIME OF DAY
            for (var hour = 1; hour <= 24; ++hour)
            {
                if (hour < timmin)
                {
                    //            LINEAR INTERPOLATION FROM DUSK TO TIMMIN
                    tmpday[hour] = tmpdsk1 - (tmpdsk1 - _day2hrSave.Tmin) * (24.0 + hour - (sunset + dtdsk)) / anight;
                }
                else if (hour > timmax)
                {
                    if (hour < sunset + dtdsk)
                    {
                        //              PARTIAL COSINE CURVE
                        tmpday[hour] = 0.5 * (_day2hrSave.Tmax + _day2hrSave.Tmin + (_day2hrSave.Tmax - _day2hrSave.Tmin) * Math.Cos(pi / day * (hour - timmax)));
                    }
                    else
                    {
                        //              LINEAR INTERPOLATION FROM DUSK TO TIMMIN
                        tmpday[hour] = tmpdsk - (tmpdsk - tmin3) * (hour - (sunset + dtdsk)) / anight;
                    }
                }
                else
                {
                    tmpday[hour] = 0.5 * (_day2hrSave.Tmax + _day2hrSave.Tmin + (_day2hrSave.Tmax - _day2hrSave.Tmin) * Math.Cos(pi / day * (hour - timmax)));
                }

                //
                // ****    CALCULATE HUMIDITY
                var satv = 0.0;
                Vslope(ref s, ref satv, ref tmpday[hour]);
                humday[hour] = 100.0 * vapor / satv;
                if (humday[hour] > 100.0) humday[hour] = 100.0;

                //
                // ***     COMPUTE NIGHTTIME WIND ADJUSTMENT TO ENSURE AVERAGE WINDSPEED
                //         OVER ENTIRE DAY REMAINS SAME
                var wndnght = (24.0 - wndnoon * 2.0 * daylen / pi) / (24.0 - 2.0 * daylen / pi);

                //         COMPUTE WIND DEPENDING AT TIME OF DAY (USING WIND RUN)
                if (hour < sunris + dtwind || hour >= sunset + dtwind)
                {
                    winday[hour] = wndnght * _day2hrSave.Wind;
                }
                else
                {
                    var dyangl = (hour - (sunris + dtwind)) * pi / daylen;
                    winday[hour] = _day2hrSave.Wind * (wndnght + (wndnoon - wndnght) * Math.Sin(dyangl));
                }
            }

            //      SET PRECIP ARRAY TO ZERO
            for (var jj = 0; jj < precip.Length; ++jj) precip[jj] = 0.0;
            // The next line is a bugfix from Gerald in his Shaw302.for code:
            for (var jj = 0; jj < snoden.Length; ++jj) snoden[jj] = 0.0;

            //      COMPUTE TIME OF MEAN TEMPERATURE: MIDWAY BETWEEN TMAX AND TMIN AND
            //      PUT PRECIP AT TIME OF MEAN TEMPERATURE
            var imean = Math.Round((timmin + hrnoon + 1.8) / 2.0);
            precip[Convert.ToInt32(imean)] = _day2hrSave.Prec;

            _day2hrSave.Tmin1 = _day2hrSave.Tmin;
            _day2hrSave.Tmax1 = _day2hrSave.Tmax;
            _day2hrSave.Tmax = tmax3;
            _day2hrSave.Tmin = tmin3;
            _day2hrSave.Tdew = tdew3;
            _day2hrSave.Wind = wind3;
            _day2hrSave.Prec = prec3;
            _day2hrSave.Solar = solar3;
            //_day2hrSave.Jday = julan3;
            //_day2hrSave.Jyr = year3;

            return true;
        }

        // line 11057
        private void Output(int nplant, int nc, int ncmax, int nsp, int nr, int ns, int[] lvlout, int notall, int inph2o, int julian, int hour, int year, int outputInital,
            double[] zc, double[] tcdt, double[][] tlcdt, double[] vapcdt, double[] wcandt, double[] rootxt, double[] rhosp, double[] zsp, double[] tspdt, double[] dlwdt, double[] zr,
            double[] trdt, double[] vaprdt, double[] gmcdt, double[] zs, double[] tsdt, double[] vlcdt, double[] vicdt, double[] matdt, double[] totflo, double[] totlat, double[][] concdt,
            double[][] saltdt, double ta, double hum, double vapa, double wind, int nclst, double windsub, double tempsub, double tsurface, double[] swdown, double[] swup, double[] lwdown,
            double[] lwup, double evap1, double melt)
        {
            //      THIS SUBROUTINE PRINTS THE TEMPERATURE AND MOISTURE PROFILES AT
            //      DESIRED INTERVALS.

            var humid = new double[11];
            //var generalOut = _outputWriters[OutputFile.EntireProfile];
            var dummy = 0.0;

            //      IF THIS IS THE FIRST TIME INTO SUBROUTINE, INITIALIZE SUMMATIONS
            //      FOR AVG TEMPERATURE, MOISTURE & SALTS, WATER FLOW BETWEEN SOIL
            //      LAYERS, AND TOTAL ROOT EXTRACTION
            if (outputInital == 0)
            {
                _outputSave.Nout = 1;
                for (var i = 1; i <= ns; ++i)
                {
                    _outputSave.Avgtmp[i] = 0.0;
                    _outputSave.Avgmat[i] = 0.0;
                    _outputSave.Avgvlc[i] = 0.0;
                    _outputSave.Avgvwc[i] = 0.0;
                    totflo[i] = 0.0;
                    _outputSave.Sumflo[i] = 0.0;
                    rootxt[i] = 0.0;
                    _outputSave.Sumrxt[i] = 0.0;
                    totlat[i] = 0.0;
                    _outputSave.Sumlat[i] = 0.0;

                    for (var j = 1; j <= _slparm.Nsalt; ++j)
                    {
                        _outputSave.Avgslt[j][i] = 0.0;
                        _outputSave.Avgcon[j][i] = 0.0;
                    }
                }

                _outputSave.Navtmp = 0;
                _outputSave.Navmat = 0;
                _outputSave.Navvlc = 0;
                _outputSave.Navvwc = 0;
                _outputSave.Navslt = 0;
                _outputSave.Navcon = 0;

                if (lvlout[2] > 0)
                {
                    _outputWriters[OutputFile.ComparisonSimulatedMeasured].WriteLine(" COMPARISON OF PREDICTED AND MEASURED PROFILES");
                    _outputWriters[OutputFile.ComparisonSimulatedMeasured].WriteLine();
                    _outputWriters[OutputFile.ComparisonSimulatedMeasured].WriteLine("  N  DY HR  YR   DEPTH     MOISTURE      TEMPERATURE    SOLUTES");
                    _outputWriters[OutputFile.ComparisonSimulatedMeasured].WriteLine("                          MEAS  PRED     MEAS  PRED    PREDICTED");
                }

                if (lvlout[3] > 0)
                {
                    _outputWriters[OutputFile.SoilTemperature].WriteLine("AVERAGE PREDICTED SOIL TEMPERATURES FOR EACH NODE (C)");
                    _outputWriters[OutputFile.SoilTemperature].WriteLine($"  DY HR  YR {string.Concat(Enumerable.Range(1, ns).Select(j => $"{zs[j],6:F2}"))}");
                }

                if (lvlout[4] > 0)
                {
                    _outputWriters[OutputFile.SoilTotalWaterContent].WriteLine("AVERAGE PREDICTED TOTAL (LIQUID + ICE) SOIL WATER CONTENT FOR EACH NODE (M3/M3)");
                    _outputWriters[OutputFile.SoilTotalWaterContent].WriteLine($"  DY HR  YR {string.Concat(Enumerable.Range(1, ns).Select(j => $"{zs[j],6:F2}"))}");
                }

                if (lvlout[5] > 0)
                {
                    _outputWriters[OutputFile.SoilLiquidWaterContent].WriteLine("AVERAGE PREDICTED LIQUID WATER CONTENT FOR EACH NODE (M3/M3)");
                    _outputWriters[OutputFile.SoilLiquidWaterContent].WriteLine($"  DY HR  YR {string.Concat(Enumerable.Range(1, ns).Select(j => $"{zs[j],6:F2}"))}");
                }

                if (lvlout[6] > 0)
                {
                    _outputWriters[OutputFile.SoilMatricPotential].WriteLine("AVERAGE PREDICTED MATRIC POTENTIAL FOR EACH NODE (METERS)");
                    _outputWriters[OutputFile.SoilMatricPotential].WriteLine($"  DY HR  YR {string.Concat(Enumerable.Range(1, ns).Select(j => $"{zs[j],10:F2}"))}");
                }

                if (lvlout[7] > 0)
                {
                    //           CANOPY AIR AND LEAF TEMPERATURES
                    _outputSave.Prtzero7 = 99.9;
                    var nprt = 1;
                    if (lvlout[7] == 1) nprt = 0;

                    _outputWriters[OutputFile.PlantCanopyTemperature].WriteLine($"CANOPY AIR AND LEAF TEMPERATURES FOR THE BOTTOM{lvlout[7],3:D} CANOPY LAYERS (C); ABSENT LAYERS ARE DESIGNATED BY{_outputSave.Prtzero7,5:F1}");
                    _outputWriters[OutputFile.PlantCanopyTemperature].Write($"  DY HR  YR  NC  ABOVE   TOP{string.Concat(Enumerable.Repeat("------", lvlout[7] - 2))}{string.Concat(Enumerable.Repeat("BOTTOM", nprt))}");
                    for (var j = 1; j <= nplant; ++j)
                        _outputWriters[OutputFile.PlantCanopyTemperature].Write($"  LEAF{string.Concat(Enumerable.Repeat("------", lvlout[7] - 2))}{string.Concat(Enumerable.Repeat("BOTTOM", nprt))}");
                    _outputWriters[OutputFile.PlantCanopyTemperature].WriteLine();
                }

                if (lvlout[8] < 0)
                {
                    //           CANOPY RELATIVE HUMIDITY
                    _outputSave.Prtzero8 = 999.9;
                    var nprt = 1;
                    if (Math.Abs(lvlout[8]) == 1) nprt = 0;

                    _outputWriters[OutputFile.PlantCanopyHumidity].WriteLine($"CANOPY RELATIVE HUMIDITY FOR THE BOTTOM{Math.Abs(lvlout[8]),3:D} CANOPY LAYERS (%); ABSENT LAYERS ARE DESIGNATED BY{_outputSave.Prtzero8,6:F1}");
                    _outputWriters[OutputFile.PlantCanopyHumidity].WriteLine($"  DY HR  YR  NC  ABOVE   TOP{string.Concat(Enumerable.Repeat("------", lvlout[7] - 2))}{string.Concat(Enumerable.Repeat("BOTTOM", nprt))}");
                }

                if (lvlout[8] > 0)
                {
                    //           CANOPY VAPOR PRESSURE
                    _outputSave.Prtzero8 = 9.999;
                    var nprt = 1;
                    if (Math.Abs(lvlout[8]) == 1) nprt = 0;

                    _outputWriters[OutputFile.PlantCanopyHumidity].WriteLine($"CANOPY VAPOR PRESSURE FOR THE BOTTOM{Math.Abs(lvlout[8]),3:D} CANOPY LAYERS (kPA); ABSENT LAYERS ARE DESIGNATED BY{_outputSave.Prtzero8,6:F3}");
                    _outputWriters[OutputFile.PlantCanopyHumidity].WriteLine($"  DY HR  YR  NC  ABOVE   TOP{string.Concat(Enumerable.Repeat("------", lvlout[7] - 2))}{string.Concat(Enumerable.Repeat("BOTTOM", nprt))}");
                }

                if (lvlout[12] > 0)
                {
                    _outputWriters[OutputFile.WaterFlowNodes].WriteLine("TOTAL WATER FLUX BETWEEN SOIL NODES (MM; DOWNWARD POSITIVE)");
                    _outputWriters[OutputFile.WaterFlowNodes].WriteLine($"  DY HR  YR {string.Concat(Enumerable.Range(1, ns).Select(j => $"{zs[j],3:F1}"))}");
                }

                if (lvlout[13] > 0)
                {
                    _outputWriters[OutputFile.WaterExtractedPlants].WriteLine("WATER EXTRACTED BY ROOTS FROM EACH LAYER (M)");
                    _outputWriters[OutputFile.WaterExtractedPlants].WriteLine($"  DY HR  YR {string.Concat(Enumerable.Range(1, ns).Select(j => $"{zs[j],10:F2}"))}");
                }

                if (lvlout[14] > 0)
                {
                    _outputWriters[OutputFile.LateralFlow].WriteLine("LATERAL FLOW EXITING FROM SOIL EACH LAYER (M); NEGATIVE INDICATES LOSS FROM PROFILE");
                    _outputWriters[OutputFile.LateralFlow].WriteLine($"  DY HR  YR {string.Concat(Enumerable.Range(1, ns).Select(j => $"{zs[j],10:F2}"))}");
                }

                if (_slparm.Nsalt > 0)
                {
                    //            WRITE HEADERS FOR SOLUTE OUTPUT FILES
                    if (lvlout[16] > 0)
                    {
                        _outputWriters[OutputFile.TotalSaltConc].WriteLine("AVERAGE PREDICTED SALT CONCENTRATION FOR EACH NODE (MOLES/KG OF SOIL)");
                        _outputWriters[OutputFile.TotalSaltConc].WriteLine($"  DY HR  YR   #{string.Concat(Enumerable.Range(1, ns).Select(j => $"{zs[j],10:F2}"))}");
                    }
                    if (lvlout[17] > 0)
                    {
                        _outputWriters[OutputFile.SoilSolutionConc].WriteLine("AVERAGE PREDICTED SOLUTE CONCENTRATION FOR EACH NODE (MOLES/LITER OF SOIL WATER)");
                        _outputWriters[OutputFile.SoilSolutionConc].WriteLine($"  DY HR  YR   #{string.Concat(Enumerable.Range(1, ns).Select(j => $"{zs[j],10:F2}"))}");
                    }
                }

                //if (lvlout[18] > 0)
                //{
                //    _outputWriters[OutputFile.ExtraOutput1].WriteLine(" DAY HR YEAR  nc nsp albcan albsrf   d_e  zm_e wind-nc  T-nc  T-srf SWDwn-nc SWup-nc  LWDwn-nc LWup-nc  H-nc   LE-nc  Melt(mm)");
                //    _outputWriters[OutputFile.ExtraOutput1].WriteLine("                                     (m)   (m)   m/s    (C)    (C)   (W/m2)   (W/m2)   (W/m2)   (W/m2)  (W/m2) (W/m2)  (mm)");
                //}

                //if (lvlout[19] > 0)
                //{
                //    //           CANOPY WIND SPEED
                //    _outputSave.Prtzero19 = 99.9;
                //    var nprt = 1;
                //    if (lvlout[19] == 1) nprt = 0;
                //    _outputWriters[OutputFile.ExtraOutput2].WriteLine($"CANOPY WIND SPEED FOR THE BOTTOM{lvlout[19],3:D} CANOPY LAYERS (m/s); ABSENT LAYERS ARE DESIGNATED BY{_outputSave.Prtzero19,6:F1}");
                //    _outputWriters[OutputFile.ExtraOutput2].WriteLine($"  DY HR  YR  NC  ABOVE   TOP{string.Concat(Enumerable.Repeat("------", lvlout[7] - 2))}{string.Concat(Enumerable.Repeat("BOTTOM", nprt))}");
                //}
            }   // line 11106

            //      IF NOTALL = 1, THIS INDICATES THAT NOT EVERYTHING IS TO PRINTED --
            //      ONLY THE FULL PROFILE TO THE GENERAL OUTPUT FILE

            if (notall != 1)
            {
                if ((hour == _measur.Meashr && julian == _measur.Measdy && lvlout[2] > 0) || (hour == 24 && _measur.Meashr == 0 && julian + 1 == _measur.Measdy && lvlout[2] > 0))
                {
                    //         PRINT OUT COMPARISON OF MEASURED VS PREDICTED PROFILES
                    for (var i = 1; i <= ns; ++i)
                    {
                        var totvlc = vlcdt[i] + vicdt[i] * Constn.Rhoi / Constn.Rhol;
                        if (inph2o == 1)
                        {
                            //               INPUT SOIL MOISTURE IS MATRIC POTENTIAL - CONVERT
                            var soimat = _measur.Vlcmes[i];
                            Matvl2(i, ref soimat, ref _measur.Vlcmes[i], ref dummy);
                        }
                        if (_slparm.Nsalt > 0)
                        {
                            _outputWriters[OutputFile.ComparisonSimulatedMeasured].WriteLine($" {_outputSave.Nout,2:D}{julian,4:D}{hour,3:D}{year,5:D}{zs[i],6:F2}   {_measur.Vlcmes[i],6:F3}{totvlc,6:F3}   {_measur.Tsmeas[i],6:F1}{tsdt[i],6:F1}   {string.Concat(Enumerable.Range(1, _slparm.Nsalt).Select(j => $"{saltdt[j][i],10:E3}"))}");
                        }
                        else
                        {
                            _outputWriters[OutputFile.ComparisonSimulatedMeasured].WriteLine($" {_outputSave.Nout,2:D}{julian,4:D}{hour,3:D}{year,5:D}{zs[i],6:F2}   {_measur.Vlcmes[i],6:F3}{totvlc,6:F3}   {_measur.Tsmeas[i],6:F1}{tsdt[i],6:F1}   ");
                        }
                    }
                    _outputSave.Nout = _outputSave.Nout + 1;
                    _outputWriters[OutputFile.ComparisonSimulatedMeasured].WriteLine();
                }

                for (var i = 1; i <= ns; ++i)
                {
                    _outputSave.Avgtmp[i] = _outputSave.Avgtmp[i] + tsdt[i];
                    _outputSave.Avgmat[i] = _outputSave.Avgmat[i] + matdt[i];
                    _outputSave.Avgvlc[i] = _outputSave.Avgvlc[i] + vlcdt[i];
                    _outputSave.Avgvwc[i] = _outputSave.Avgvwc[i] + vlcdt[i] + vicdt[i] * Constn.Rhoi / Constn.Rhol;
                    _outputSave.Sumflo[i] = _outputSave.Sumflo[i] + totflo[i];
                    _outputSave.Sumrxt[i] = _outputSave.Sumrxt[i] + rootxt[i];
                    _outputSave.Sumlat[i] = _outputSave.Sumlat[i] + totlat[i];
                    for (var j = 1; j <= _slparm.Nsalt; ++j)
                    {
                        _outputSave.Avgslt[j][i] = _outputSave.Avgslt[j][i] + saltdt[j][i];
                        _outputSave.Avgcon[j][i] = _outputSave.Avgcon[j][i] + concdt[j][i];
                    }
                }
                _outputSave.Navtmp = _outputSave.Navtmp + 1;
                _outputSave.Navmat = _outputSave.Navmat + 1;
                _outputSave.Navvlc = _outputSave.Navvlc + 1;
                _outputSave.Navvwc = _outputSave.Navvwc + 1;
                _outputSave.Navslt = _outputSave.Navslt + 1;
                _outputSave.Navcon = _outputSave.Navcon + 1;

                //      PRINT OUT SOIL TEMPERATURES IF AT DESIRED INTERVAL
                if (lvlout[3] > 0)
                {
                    if (hour % lvlout[3] == 0 || outputInital == 0)
                    {
                        _outputWriters[OutputFile.SoilTemperature].WriteLine($" {julian,3:D}{hour,3:D}{year,5:D}{string.Concat(Enumerable.Range(1, ns).Select(i => $"{_outputSave.Avgtmp[i] / _outputSave.Navtmp,6:F1}"))}");

                        for (var i = 1; i <= ns; ++i)
                        {
                            _outputSave.Avgtmp[i] = 0.0;
                        }
                        _outputSave.Navtmp = 0;
                    }
                }

                //      PRINT OUT SOIL TOTAL WATER CONTENT IF AT DESIRED INTERVAL
                if (lvlout[4] > 0)
                {
                    if (hour % lvlout[4] == 0 || outputInital == 0)
                    {
                        _outputWriters[OutputFile.SoilTotalWaterContent].WriteLine($" {julian,3:D}{hour,3:D}{year,5:D}{string.Concat(Enumerable.Range(1, ns).Select(i => $"{_outputSave.Avgvwc[i] / _outputSave.Navvwc,6:F3}"))}");

                        for (var i = 1; i <= ns; ++i)
                        {
                            _outputSave.Avgvwc[i] = 0.0;
                        }
                        _outputSave.Navvwc = 0;
                    }
                }

                //      PRINT OUT SOIL LIQUID WATER CONTENT IF AT DESIRED INTERVAL
                if (lvlout[5] > 0)
                {
                    if (hour % lvlout[5] == 0 || outputInital == 0)
                    {
                        _outputWriters[OutputFile.SoilLiquidWaterContent].WriteLine($" {julian,3:D}{hour,3:D}{year,5:D}{string.Concat(Enumerable.Range(1, ns).Select(i => $"{_outputSave.Avgvlc[i] / _outputSave.Navvlc,6:F3}"))}");

                        for (var i = 1; i <= ns; ++i)
                        {
                            _outputSave.Avgvlc[i] = 0.0;
                        }
                        _outputSave.Navvlc = 0;
                    }
                }

                //      PRINT OUT SOIL MATRIC POTENTIAL IF AT DESIRED INTERVAL
                if (lvlout[6] > 0)
                {
                    if (hour % lvlout[6] == 0 || outputInital == 0)
                    {
                        _outputWriters[OutputFile.SoilMatricPotential].WriteLine($"{julian,4:D}{hour,3:D}{year,5:D}{string.Concat(Enumerable.Range(1, ns).Select(i => $"{_outputSave.Avgmat[i] / _outputSave.Navmat,10:F1}"))}");

                        for (var i = 1; i <= ns; ++i)
                        {
                            _outputSave.Avgmat[i] = 0.0;
                        }
                        _outputSave.Navmat = 0;
                    }
                }

                //      PRINT OUT CANOPY AIR AND LEAF TEMPERATURE (EVERY TIME STEP)
                if (lvlout[7] > 0 && nplant > 0)
                {
                    if (nplant > 1)
                    {
                        //          SET LEAF TEMPERATURE TO MISSING IF NO LAI IN LAYER
                        for (var j = 1; j <= nplant; ++j)
                        {
                            for (var i = 1; i <= nc; ++i)
                            {
                                if (_clayrs.Canlai[j][i] <= 0.0) tlcdt[j][i] = _outputSave.Prtzero7;
                            }
                        }
                    }
                    var nzbtm = ncmax - nc;
                    if (nzbtm > lvlout[7]) nzbtm = lvlout[7];

                    var nprt = nc;
                    if (nprt + nzbtm > lvlout[7]) nprt = lvlout[7] - nzbtm;
                    var nztop = lvlout[7] - nprt - nzbtm;

                    _outputWriters[OutputFile.PlantCanopyTemperature].Write($" {julian,3:D}{hour,3:D}{year,5:D}{nc,3:D} {ta,6:F1}{string.Concat(Enumerable.Range(1, nztop).Select(i => $"{_outputSave.Prtzero7,6:F1}"))}{string.Concat(Enumerable.Range(1, nprt).Select(i => $"{tcdt[i],6:F1}"))}{string.Concat(Enumerable.Range(1, nzbtm).Select(i => $"{_outputSave.Prtzero7,6:F1}"))}");
                    for (var j = 1; j <= nplant; ++j)
                        _outputWriters[OutputFile.PlantCanopyTemperature].Write($"{string.Concat(Enumerable.Range(1, nztop).Select(i => $"{_outputSave.Prtzero7,6:F1}"))}{string.Concat(Enumerable.Range(1, nprt).Select(i => $"{tlcdt[j][i],6:F1}"))}{string.Concat(Enumerable.Range(1, nzbtm).Select(i => $"{_outputSave.Prtzero7,6:F1}"))}");

                    _outputWriters[OutputFile.PlantCanopyTemperature].WriteLine();
                }

                // line 11220

                //      PRINT OUT CANOPY VAPOR PRESSURE OR RH (EVERY TIME STEP)
                if (Math.Abs(lvlout[8]) > 0 && nplant > 0)
                {
                    var nzbtm = ncmax - nc;
                    if (nzbtm > Math.Abs(lvlout[8])) nzbtm = Math.Abs(lvlout[8]);

                    var nprt = nc;
                    if (nprt + nzbtm > Math.Abs(lvlout[8])) nprt = Math.Abs(lvlout[8]) - nzbtm;

                    var nztop = Math.Abs(lvlout[8]) - nprt - nzbtm;

                    if (lvlout[8] < 0)
                    {
                        //           PRINT OUT RELATIVE HUMIDITY
                        for (var i = 1; i <= nc; ++i)
                        {
                            var s = 0.0;
                            var satv = 0.0;
                            Vslope(ref s, ref satv, ref tcdt[i]);
                            humid[i] = 100.0 * vapcdt[i] / satv;
                        }
                        _outputWriters[OutputFile.PlantCanopyHumidity].WriteLine($" {julian,3:D}{hour,3:D}{year,5:D}{nc,3:D} {100.0 * hum,6:F1}{string.Concat(Enumerable.Range(1, nztop).Select(i => $"{_outputSave.Prtzero8,6:F1}"))}{string.Concat(Enumerable.Range(1, nprt).Select(i => $"{humid[i],6:F1}"))}{string.Concat(Enumerable.Range(1, nzbtm).Select(i => $"{_outputSave.Prtzero8,6:F1}"))}");
                    }
                    else
                    {
                        //           PRINT OUT VAPOR PRESSURE IN kPA
                        for (var i = 1; i <= nc; ++i)
                        {
                            humid[i] = 0.4619 * vapcdt[i] * (tcdt[i] + 273.16);
                        }
                        _outputWriters[OutputFile.PlantCanopyHumidity].WriteLine($" {julian,3:D}{hour,3:D}{year,5:D}{nc,3:D} {0.4619 * vapa * (ta + 273.16),6:F3}{string.Concat(Enumerable.Range(1, nztop).Select(i => $"{_outputSave.Prtzero8,6:F3}"))}{string.Concat(Enumerable.Range(1, nprt).Select(i => $"{humid[i],6:F3}"))}{string.Concat(Enumerable.Range(1, nzbtm).Select(i => $"{_outputSave.Prtzero8,6:F3}"))}");
                    }
                }

                //      PRINT OUT WATER FLOW BETWEEN SOIL NODES IF AT DESIRED INTERVAL
                if (lvlout[12] > 0)
                {
                    if (hour % lvlout[12] == 0 && outputInital != 0)
                    {
                        _outputWriters[OutputFile.WaterFlowNodes].WriteLine($" {julian,3:D}{hour,3:D}{year,5:D}{string.Concat(Enumerable.Range(1, ns - 1).Select(i => $"{_outputSave.Sumflo[i] * 1000.0,6:F2}"))}");

                        for (var i = 1; i <= ns; ++i)
                        {
                            _outputSave.Sumflo[i] = 0.0;
                        }
                    }
                }

                //      PRINT OUT WATER EXTRACTED BY ROOTS FROM EACH NODE
                if (lvlout[13] > 0 && nplant > 0)
                {
                    if (hour % lvlout[13] == 0 || outputInital == 0)
                    {
                        _outputWriters[OutputFile.WaterExtractedPlants].WriteLine($"{julian,4:D}{hour,3:D}{year,5:D}{string.Concat(Enumerable.Range(1, ns).Select(i => $"{_outputSave.Sumrxt[i] / Constn.Rhol,10:E2}"))}");

                        for (var i = 1; i <= ns; ++i)
                        {
                            _outputSave.Sumrxt[i] = 0.0;
                        }
                    }
                }

                //      PRINT OUT LATERAL FLOW EXITING PROFILE FROM EACH SOIL LAYER
                if (lvlout[14] > 0)
                {
                    if (hour % lvlout[14] == 0 || outputInital == 0)
                    {
                        _outputWriters[OutputFile.LateralFlow].WriteLine($"{julian,4:D}{hour,3:D}{year,5:D}{string.Concat(Enumerable.Range(1, ns).Select(i => $"{-_outputSave.Sumlat[i],10:E2}"))}");

                        for (var i = 1; i <= ns; ++i)
                        {
                            _outputSave.Sumlat[i] = 0.0;
                        }
                    }
                }

                //      PRINT OUT SALT CONCENTRATION IF AT DESIRED INTERVAL
                if (lvlout[16] > 0)
                {
                    if (hour % lvlout[16] == 0 || outputInital == 0)
                    {
                        for (var j = 1; j <= _slparm.Nsalt; ++j)
                        {
                            _outputWriters[OutputFile.TotalSaltConc].WriteLine($" {julian,3:D}{hour,3:D}{year,5:D}{j,3:D}{string.Concat(Enumerable.Range(1, ns).Select(i => $"{_outputSave.Avgslt[j][i] / _outputSave.Navslt,10:E3}"))}");

                            for (var i = 1; i <= ns; ++i)
                            {
                                _outputSave.Avgslt[j][i] = 0.0;
                            }
                        }
                        _outputSave.Navslt = 0;
                    }
                }

                //      PRINT OUT SOLUTE CONCENTRATION IF AT DESIRED INTERVAL
                if (lvlout[17] > 0)
                {
                    if (hour % lvlout[17] == 0 || outputInital == 0)
                    {
                        for (var j = 1; j <= _slparm.Nsalt; ++j)
                        {
                            _outputWriters[OutputFile.SoilSolutionConc].WriteLine($" {julian,3:D}{hour,3:D}{year,5:D}{j,3:D}{string.Concat(Enumerable.Range(1, ns).Select(i => $"{_outputSave.Avgcon[j][i] / _outputSave.Navcon,10:E3}"))}");

                            for (var i = 1; i <= ns; ++i)
                            {
                                _outputSave.Avgcon[j][i] = 0.0;
                            }
                        }
                        _outputSave.Navcon = 0;
                    }
                }

                ////      PRINT OUT FILE FOR SPECIAL STUDIES
                //if (lvlout[18] > 0)
                //{
                //    if (hour % lvlout[18] == 0 && inital != 0)
                //    {
                //        if (nc == 0)
                //        {
                //            _writeit.Hnc = _writeit.Hflux1;
                //            // xOUT         should use DT instead of 3600 in case not hourly timesteps
                //            if (nsp > 0)
                //            {
                //                _writeit.Xlenc = Constn.Ls * evap1 * Constn.Rhol / 3600.0;
                //            }
                //            else
                //            {
                //                _writeit.Xlenc = Constn.Lv * evap1 * Constn.Rhol / 3600.0;
                //            }
                //        }

                //        double albtop, albsrf;
                //        if (swdown[1] > 0.0)
                //        {
                //            albtop = swup[1] / swdown[1];
                //            albsrf = swup[nc + 1] / swdown[nclst + 1];
                //        }
                //        else
                //        {
                //            albtop = 0.0;
                //            albsrf = 0.0;
                //        }
                //        _outputWriters[OutputFile.ExtraOutput1].WriteLine($" {julian,3:D}{hour,3:D}{year,5:D}{nclst,4:D}{nsp,4:D}{albtop,7:F3}{albsrf,7:F3}{_windv.Zero,6:F2}{_windv.Zm,6:F2}{windsub,7:F2}{tempsub,7:F2}{tsurface,7:F2}{swdown[nclst + 1],9:F1}{swup[nclst + 1],9:F1}{lwdown[nclst + 1],9:F1}{lwup[nclst + 1],9:F1}{_writeit.Hnc,8:F1}{_writeit.Xlenc,8:F1}{melt * 1000.0,6:F2}");
                //    }
                //}

                ////      PRINT OUT FILE FOR SPECIAL STUDIES - CANOPY WIND SPEED
                //if (lvlout[19] > 0 && nplant > 0 && inital != 0)
                //{
                //    // xout
                //    var nzbtm = ncmax - nc;
                //    if (nzbtm > lvlout[19]) nzbtm = lvlout[19];

                //    var nprt = nc;
                //    if (nprt + nzbtm > lvlout[19]) nprt = lvlout[19] - nzbtm;

                //    var nztop = lvlout[19] - nprt - nzbtm;
                //    _outputWriters[OutputFile.ExtraOutput2].WriteLine($" {julian,3:D}{hour,3:D}{year,5:D}{nc,3:D} {wind,6:F1}{string.Concat(Enumerable.Range(1, nztop).Select(i => $"{_outputSave.Prtzero19,6:F1}"))}{string.Concat(Enumerable.Range(1, nprt).Select(i => $"{_windv.Windc[i],6:F1}"))}{string.Concat(Enumerable.Range(1, nzbtm).Select(i => $"{_outputSave.Prtzero19,6:F1}"))}");
                //}

                if (outputInital != 0 && (lvlout[1] == 0 || hour % lvlout[1] != 0))
                    return;
            }

            if (_lvlout[1] == 0)
                return;

            // line 11350 (70)
            var generalOut = _outputWriters[OutputFile.EntireProfile];
            generalOut.WriteLine();

            //      CANOPY LAYERS
            if (nc > 0)
            {
                generalOut.WriteLine("CANOPY LAYERS :");
                generalOut.WriteLine($" DAY HR  YR  DEPTH   TEMP   VAPOR  MOISTURE {string.Concat(Enumerable.Range(1, nplant).Select(j => $"LAI#{j,2:D1}"))}");
                generalOut.WriteLine("              (M)     (C)  (KG/M3)  (KG/KG)");

                for (var i = 1; i <= nc; ++i)
                {
                    generalOut.WriteLine($"{julian,4:D}{hour,3:D}{year,5:D}{zc[i],6:F3}{tcdt[i],7:F3}{vapcdt[i],10:F6}{wcandt[i],7:F3} {string.Concat(Enumerable.Range(1, nplant).Select(j => $"{_clayrs.Canlai[j][i],10:F6}"))}");
                }
                generalOut.WriteLine($"{julian,4:D}{hour,3:D}{year,5:D}{zc[nc + 1],6:F3} ");
            }

            //      SNOWPACK LAYERS
            if (nsp > 0)
            {
                generalOut.WriteLine("SNOW LAYERS :");
                generalOut.WriteLine(" DAY HR  YR  DEPTH  TEMP   LIQUID  DENSITY");
                generalOut.WriteLine("              (M)    (C)     (M)   (KG/M3)");

                for (var i = 1; i <= nsp; ++i)
                {
                    generalOut.WriteLine($"{julian,4:D}{hour,3:D}{year,5:D}{zsp[i],6:F3}{tspdt[i],7:F3}{dlwdt[i],8:F4}{rhosp[i],8:F2}");
                }
                generalOut.WriteLine($"{julian,4:D}{hour,3:D}{year,5:D}{zsp[nsp + 1],6:F3}");
            }

            //      RESIDUE LAYERS
            if (nr > 0)
            {
                generalOut.WriteLine("RESIDUE LAYERS :");
                generalOut.WriteLine(" DAY HR  YR  DEPTH  TEMP   VAPOR  MOISTURE");
                generalOut.WriteLine("              (M)     (C)  (KG/M3)  (KG/KG)");

                for (var i = 1; i <= nr; ++i)
                {
                    generalOut.WriteLine($"{julian,4:D}{hour,3:D}{year,5:D}{zr[i],6:F3}{trdt[i],7:F3}{vaprdt[i],10:F6}{gmcdt[i],7:F3} ");
                }
                generalOut.WriteLine($"{julian,4:D}{hour,3:D}{year,5:D}{zr[nr + 1],6:F3} ");
            }

            //      SOIL LAYER
            generalOut.WriteLine("SOIL LAYERS :");

            if (_slparm.Nsalt == 0)
            {
                generalOut.WriteLine(" DAY HR  YR  DEPTH   TEMP WATER  ICE   MATRIC");
                generalOut.WriteLine("              (M)     (C) (M/M) (M/M)    (M)");
                for (var i = 1; i <= ns; ++i)
                {
                    generalOut.WriteLine($"{julian,4:D}{hour,3:D}{year,5:D}{zs[i],6:F3}{tsdt[i],7:F3}{vlcdt[i],6:F3}{vicdt[i],6:F3}{matdt[i],8:F2}");
                }
            }
            else
            {
                generalOut.WriteLine(" DAY HR  YR  DEPTH   TEMP WATER  ICE   MATRIC    SOLUTE #1   TOTAL SALT #1");
                generalOut.WriteLine("              (M)     (C) (M/M) (M/M)    (M)      (MOLE/L)     (MOLES/KG)");
                for (var i = 1; i <= ns; ++i)
                {
                    generalOut.WriteLine($"{julian,4:D}{hour,3:D}{year,5:D}{zs[i],6:F3}{tsdt[i],7:F3}{vlcdt[i],6:F3}{vicdt[i],6:F3}{matdt[i],8:F2}{concdt[1][i],14:E4}{saltdt[1][i],14:E4}");
                }
            }
        }

        private bool WeatherDataEOS => _weatherDataIndex == _weatherData.Length - 1;

        private ShawDammDailyWeatherRecord NextWeatherDatum => WeatherDataEOS ? _weatherData[_weatherDataIndex] : _weatherData[++_weatherDataIndex];

        private string ReadNextLine(InputFile inputFile) => ReadNextLine(_inputReaders[inputFile]);

        private string ReadNextLine(StreamReader reader)
        {
            var s = string.Empty;

            while (!reader.EndOfStream)
            {
                s = reader.ReadLine().Trim();
                if (!string.IsNullOrEmpty(s))
                    break;
            }

            return s;
        }

        private List<string> ParseNextLine(InputFile inputFile) => ParseLine(ReadNextLine(inputFile));

        private List<string> ParseNextLine(StreamReader reader) => ParseLine(ReadNextLine(reader));

        private static List<string> ParseLine(string s)
        {
            // I'm not entirely sure whether FORTRAN allows the repeat format, i.e. count*value, anywhere within whitespace-delimited text, but let's assume it does.
            var atoms = _whiteSpaceRegex.Split(s.Trim()).ToList();

            var t1 = new List<string>(atoms.Count);
            Match m;
            foreach (var a in atoms)
            {
                if ((m = _repeatInputRegex.Match(a)).Success)
                {
                    t1.AddRange(Enumerable.Repeat(m.Groups["value"].Value, int.Parse(m.Groups["count"].Value)));
                }
                else
                {
                    t1.Add(a);
                }
            }
            return t1;
        }

        private static bool OpenStreamReader(string file, out StreamReader reader)
        {
            try
            {
                reader = new StreamReader(file);
                return true;
            }
            catch (Exception)
            {
                reader = null;
                Console.WriteLine($"Cannot open file {file}.");
                return false;
            }
        }
    }
}