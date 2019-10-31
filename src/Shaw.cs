using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landis.Extension.ShawDamm
{
    public partial class ShawDamm
    {
        #region fields
        private Dictionary<OutputFile, StreamWriter> _outputWriters { get; set; }
        private Dictionary<InputFile, StreamReader> _inputReaders { get; set; }
        private StreamReader[] _canopyReaders { get; set; }
        private StreamReader _residueReader { get; set; }

        private ShawDammDailyWeatherRecord[] _weatherData;

        private int _weatherDataIndex;

        private Constn _constn;
        private Slparm _slparm;
        private Clayrs _clayrs;
        private Measur _measur;
        private Windv _windv;
        private Radcan _radcan;
        private Radres _radres;
        private Timewt _timewt;
        private Matrix _matrix;
        private Residu _residu;
        private Spheat _spheat;
        private Writeit _writeit;
        private Canlwr _canlwr;

        private GoshawSave _goshawSave;
        private DayinpSave _dayinpSave;
        private Day2hrSave _day2hrSave;
        private CanopySave _canopySave;
        private CanlayzcSave _canlayzcSave;
        private AtstabSave _atstabSave;
        private LeaftSave _leaftSave;
        private CantkSave _cantkSave;
        private EbsnowSave _ebsnowSave;
        private EbresSave _ebresSave;
        private EbsoilSave _ebsoilSave;
        private SoiltkSave _soiltkSave;
        private WbalncSave _wbalncSave;
        private EnergySave _energySave;
        private FrostSave _frostSave;
        private OutputSave _outputSave;
        private SnomltSave _snomltSave;
        private SumdtSave _sumdtSave;
        private WbsoilSave _wbsoilSave;
        private RainslSave _rainslSave;

        private int _nr;
        private int _nsp;
        private int _ns;
        private int _nc;
        private static double _toler;
        private int[] _level = new int[7];
        private int _mzcinp;
        private int _mpltgro;
        private int _nrchang;
        private static int _ivlcbc = 1;
        private static int _itmpbc;
        private double _tsavg;
        private int _nplant;
        private double[] _plthgt = new double[9];
        private double[] _pltwgt = new double[9];
        private double[] _pltlai = new double[9];
        private double[] _rootdp = new double[9];
        private double[] _dchar = new double[9];
        private double[] _tccrit = new double[9];
        private double[] _rstom0 = new double[9];
        private double[] _rstexp = new double[9];
        private double[] _pleaf0 = new double[9];
        private double[] _rleaf0 = new double[9];
        private double[] _rroot0 = new double[9];
        private double[] _canalb = new double[9];
        private static double _canma;
        private static double _canmb;
        private double _wcmax;
        private double[] _pintrcp = new double[9];
        private double[] _xangle = new double[9];
        private double[] _clumpng = new double[9];
        private int[] _itype = new int[9];
        private double[] _zc = new double[12];
        private double[] _wcandt = new double[11];
        private static int _istomate;
        private double[][] _stomate = JaggedArray<double>(9, 11);
        private double[] _zsp = new double[101];
        private double[] _dzsp = new double[101];
        private double[] _rhosp = new double[101];
        private double[] _tspdt = new double[101];
        private double[] _dlwdt = new double[101];
        private int[] _icespt = new int[101];
        private static int _isnotmp;
        private static double _snotmp;
        private static double _zmspcm;
        private double[] _gmcdt = new double[11];
        private double _gmcmax;
        private double _zrthik;
        private double _rload;
        private double _cover;
        private double _albres;
        private double _rescof;
        private double _restkb;
        private double[] _zs = new double[51];
        private double[][] _saltdt = JaggedArray<double>(11, 51);
        private double _albdry;
        private double _albexp;
        private double[] _dgrade = new double[11];
        private double[] _sltdif = new double[11];
        private double[] _asalt = new double[51];
        private double[] _disper = new double[51];
        private double _zmsrf;
        private double _zhsrf;
        private double _zersrf;
        private double _zmsp;
        private double _zhsp;
        private double _height;
        private double _pondmx;
        private int _nhrpdt;
        private int _jstart;
        private int _jend;
        private int _yrstar;
        private int _hrstar;
        private int _yrend;
        private double _wdt;
        private double _alatud;
        private double _slope;
        private double _aspect;
        private double _hrnoon;

        private double[] _vlcdt;
        private int _outputInital;

        private int _inital;

        // formerly local variables in CalculateSoilResults
        private double[] _tsdt;
        private double[] _vicdt = new double[51];
        private double[] _matdt = new double[51];
        private double[][] _concdt = JaggedArray<double>(11, 51);
        private int[] _icesdt = new int[51];

        private double[] _sunhor = new double[25];
        private double[] _tmpday = new double[25];
        private double[] _winday = new double[25];
        private double[] _humday = new double[25];
        private double[] _precip = new double[25];
        private double[] _snoden = new double[25];
        private double[] _soitmp = new double[25];
        private double[] _vlcday = new double[25];
        private double[][] _soilxt = JaggedArray<double>(25, 51); // needed to switch the order of indices in order to pass a double[] of length 51
        private double[] _tcdt = new double[12];
        private double[][] _tlcdt = JaggedArray<double>(9, 11);
        private double[] _vapcdt = new double[12];
        private double[] _pcandt = new double[9];
        private double[] _wlag = new double[12];
        private double[] _zr = new double[11];
        private double[] _rhor = new double[11];
        private double[] _trdt = new double[11];
        private double[] _vaprdt = new double[11];

        private double _clouds;
        private double _declin;
        private double _hafday;

        private double _store;
        private double _snowex;
        private double _dirres;
        private double _pond;

        #endregion

        #region constructor

        public ShawDamm()
        {
            _constn = new Constn();
            _slparm = new Slparm();
            _clayrs = new Clayrs();
            _measur = new Measur();
            _windv = new Windv();
            _radcan = new Radcan();
            _radres = new Radres();
            _timewt = new Timewt();
            _matrix = new Matrix();
            _residu = new Residu();
            _spheat = new Spheat();
            _writeit = new Writeit();
            _canlwr = new Canlwr();

            _goshawSave = new GoshawSave();
            _dayinpSave = new DayinpSave();
            _day2hrSave = new Day2hrSave();
            _canopySave = new CanopySave();
            _canlayzcSave = new CanlayzcSave();
            _atstabSave = new AtstabSave();
            _leaftSave = new LeaftSave();
            _cantkSave = new CantkSave();
            _ebsnowSave = new EbsnowSave();
            _ebresSave = new EbresSave();
            _ebsoilSave = new EbsoilSave();
            _soiltkSave = new SoiltkSave();
            _wbalncSave = new WbalncSave();
            _energySave = new EnergySave();
            _frostSave = new FrostSave();
            _outputSave = new OutputSave();
            _snomltSave = new SnomltSave();
            _sumdtSave = new SumdtSave();
            _wbsoilSave = new WbsoilSave();
            _rainslSave = new RainslSave();
        }

        #endregion

        #region properties

        //public Dictionary<OutputFile, StreamWriter> OutputWriters { get { return _outputWriters; } set { _outputWriters = value; } }
        //public Dictionary<InputFile, StreamReader> InputReaders { get { return _inputReaders; } set { _inputReaders = value; } }

        private static string _wd;
        private static int _iversion = 3;
        private static int _mtstep = 1;
        private static int _iflagsi = 1;
        private static int _inph2o = 0;
        private static int _mwatrxt;
        private static int[] _lvlout;

        //public int Ivlcbc => _ivlcbc;
        //public int Itmpbc => _itmpbc;

        public string SoilTemperatureFile;
        public string SoilMoistureFile;
        public string WeatherFile;

        public string Name { get; set; }

        public static SimpleFileParser InputFileParser { get; private set; }
        
        public static bool HasGlobalSetup { get; private set; }

        #endregion

        public static bool ReadInputFile(string inputFilePath, out string errorMessage)
        {
            errorMessage = string.Empty;
            _wd = Path.GetDirectoryName(inputFilePath);


            InputFileParser = new SimpleFileParser(inputFilePath, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return false;


            bool isMissing;
            string sval;
            int ival;
            double dval;

            // ModelSelectionFlags
            if (!InputFileParser.TryParse("StomatalResistance", out _istomate, out errorMessage, 0, true, 1, true))
                return false;

            if (!InputFileParser.TryParse("BoundaryTemperatureCondition", out _itmpbc, out errorMessage, 0, true, 1, true))
                return false;

            if (!InputFileParser.TryParse("EquationForWaterReleaseCurve", out Slparm.Iwrc, out errorMessage, 0, true, 1, true))
                return false;

            if (!InputFileParser.TryParse("FlagSourceSink", out _mwatrxt, out errorMessage, 0, true, 1, true))
                return false;


            if (!InputFileParser.TryParse("ErrorTolerance", out _toler, out errorMessage, 1e-6))
                return false;

            // PlantParameters
            if (!InputFileParser.TryParse("CoefficientWaterPotentialDeadPlants", out _canma, out errorMessage))
                return false;

            if (!InputFileParser.TryParse("ExponentWaterPotentialDeadPlants", out _canmb, out errorMessage))
                return false;

            // SnowParameters
            if (!InputFileParser.TryParse("SnowBasedOn", out sval, out errorMessage))
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

            if (!InputFileParser.TryParse("MaxTempSnowfall", out _snotmp, out errorMessage))
                return false;

            if (!InputFileParser.TryParse("WindProfileParameterSnow", out _zmspcm, out errorMessage, 0, true))
                return false;



            // read logging output flags.  If the logging name exists, set the corresponding _lvlout[] index to 24 for 24 hr output.
            _lvlout = new int[21];

            var logNames = Enum.GetNames(typeof(OutputFile));
            var logLvlouts = Enum.GetValues(typeof(OutputFile)).Cast<int>().ToList();
            for (var i = 0; i < logNames.Length; ++i)
            {
                bool containsToken;
                if (!InputFileParser.TryContainsToken(logNames[i], out containsToken, out errorMessage))
                    return false;

                if (containsToken)
                    _lvlout[logLvlouts[i]] = 24;
            }

            return true;
        }

        public bool Initialize(string name)
        {
            Name = name;
            if (!InputSiteFile(_wd, _iversion, ref _nc, ref _nsp, ref _nr, ref _ns,
               ref _toler, _level, _iflagsi, ref _mzcinp, ref _mpltgro, ref _nrchang,
               ref _ivlcbc, ref _itmpbc, ref _tsavg, ref _nplant, _plthgt, _pltwgt, _pltlai, _rootdp,
               _dchar, _tccrit, _rstom0, _rstexp, _pleaf0, _rleaf0, _rroot0, _canalb,
               ref _canma, ref _canmb, ref _wcmax, _pintrcp, _xangle, _clumpng, _itype, _zc, _wcandt,
               ref _istomate, _stomate, _zsp, _dzsp, _rhosp, _tspdt, _dlwdt, _icespt, ref _isnotmp,
               ref _snotmp, _gmcdt, ref _gmcmax, ref _zrthik, ref _rload, ref _cover, ref _albres, ref _rescof,
               ref _restkb, _zs, _saltdt,
               ref _albdry, ref _albexp, _dgrade, _sltdif, _asalt, _disper, ref _zmsrf, ref _zhsrf,
               ref _zersrf, ref _zmsp, ref _zhsp, ref _height, ref _pondmx, ref _jstart, ref _yrstar, ref _hrstar,
               ref _jend, ref _yrend, ref _nhrpdt, ref _wdt, ref _alatud, ref _slope, ref _aspect, ref _hrnoon))
            {
                Abort();
                return false;
            }

            return true;
        }

        public void SetInitialSoilMoisture(double[] soilMoistureInit)
        {
            _vlcdt = new double[_ns + 1];
            Array.Copy(soilMoistureInit, 0, _vlcdt, 1, _ns);
        }

        public List<double> GetDepths()
        {
            var depths = new List<double>(_ns);
            for (var i = 1; i <= _ns; ++i)
                depths.Add(_zs[i]);

            return depths;
        }

        /// <summary>Calculates the soil results. 
        /// 'soilTempBcFinal' is used when ITMPBC = 0 and GIPL is used to estimate the soil temperature boundary condition.
        /// 'averageAnnualSoilTemperature' is used when ITMPBC = 1 and GIPL is not used.  TSAVG is set to this value.
        /// Assumes that IVLCBC = 1, so that only the initial soil moisture profile is needed.
        /// </summary>
        /// <param name="weatherData">The weather data.</param>
        /// <param name="soilTemperatureInit">The soil temperature initialize.</param>
        /// <param name="soilTempBcFinal">The soil temporary bc final.</param>
        /// <param name="averageAnnualSoilTemperature">The average annual soil temperature.</param>
        /// <param name="soilMoistureInit">The soil moisture initialize.</param>
        /// <returns></returns>
        public ShawDammResults CalculateSoilResults(int year, int julian, ShawDammDailyWeatherRecord[] weatherData, List<double[]> dailySoilTemperature, double averageAnnualSoilTemperature)
        {
            _weatherData = weatherData;
            _weatherDataIndex = -1;

            // set _tsavg as this is passed to Shaw, not read from the site file
            _tsavg = averageAnnualSoilTemperature;

            //// SET FLAG INDICATING FIRST TIME THROUGH SUBROUTINES FOR INITIALIZATION
            //var _inital = 0;

            if (_inital == 0)
            {
                // copy soil temperature parameter for the first day (0-based) into Shaw's initial soil temperature profile (1-based)
                _tsdt = new double[_ns + 1];
                Array.Copy(dailySoilTemperature.First(), 0, _tsdt, 1, _ns);
            }

            var results = new ShawDammResults(_ns, _zs);

            //var _vicdt = new double[51];
            //var _matdt = new double[51];
            //var _concdt = JaggedArray<double>(11, 51);
            //var _icesdt = new int[51];

            if (_inital == 0)
            {
                // don't read initial soil temperature and moisture as these have been passed to this routineS
                if (!SetInitialConditions(_ns, _inph2o, _tsdt, _vlcdt, _vicdt, _matdt, _concdt, _icesdt, _saltdt, _jstart, _yrstar, _hrstar))
                {
                    Abort();
                    return results;
                }
            }

            //var _sunhor = new double[25];
            //var _tmpday = new double[25];
            //var _winday = new double[25];
            //var _humday = new double[25];
            //var _precip = new double[25];
            //var _snoden = new double[25];
            //var _soitmp = new double[25];
            //var _vlcday = new double[25];
            //var _soilxt = JaggedArray<double>(25, 51); // needed to switch the order of indices in order to pass a double[] of length 51
            //var _tcdt = new double[12];
            //var _tlcdt = JaggedArray<double>(9, 11);
            //var _vapcdt = new double[12];
            //var _pcandt = new double[9];
            //var _wlag = new double[12];
            //var _zr = new double[11];
            //var _rhor = new double[11];
            //var _trdt = new double[11];
            //var _vaprdt = new double[11];

            //var julian = _jstart;
            //var hour = _hrstar;
            //var year = _yrstar;

            var hour = 24;

            var maxjul = 365;
            if (year % 4 == 0) maxjul = 366;    // instead, this should call a globally-accessible method to determine leap years, since % 4 isn't always right, e.g. 1900 was not, but 2000 was.

            // **** DEFINE INITIAL BOUNDARY CONDITIONS FOR THE DAY

            var ihour = hour;
            //  CANNOT SET ZERO ARRAY ELEMENT
            if (ihour == 0) ihour = 24;

            var vlcdayBcInit = 0.0;
            if (_inph2o != 1)
            {
                //         INPUT SOIL MOISTURE IS WATER CONTENT
                vlcdayBcInit = _vlcday[ihour] = _vlcdt[_ns] + _vicdt[_ns] * Constn.Rhoi / Constn.Rhol;
            }
            else
            {
                //         INPUT SOIL MOISTURE IS MATRIC POTENTIAL
                _vlcday[ihour] = _matdt[_ns];
            }
            _soitmp[ihour] = _tsdt[_ns];

            if (hour == 24)
            {
                //         STARTING AT END OF DAY -- RESET DAY SO WE START AT BEGINNING
                //         OF NEXT DAY AND IT IS NOT REST AT BEGINNING OF SIMULATION
                hour = hour - 24;
                julian = julian + 1;
                //if (julian > maxjul)
                //{
                //    julian = julian - maxjul;
                //    year = year + 1;
                //    maxjul = 365;
                //    if (year % 4 == 0) maxjul = 366;
                //}
            }

            var dayCounter = 1;
            var totalDays = weatherData.Length - 1;

            if (!Dayinp(julian, year, maxjul, hour, _nhrpdt, _mtstep, _iversion, _iflagsi,
                _inital, _itmpbc, _ivlcbc, _lvlout[2], _mpltgro, _nrchang,
                _mwatrxt, _ns, _alatud, _hrnoon, _sunhor, _tmpday, _winday, _humday, _precip, _snoden, _soitmp,
                _vlcday, _soilxt, ref _nplant, _plthgt, _dchar, _clumpng, _pltwgt, _pltlai, _rootdp, ref _zrthik, ref _rload,
                ref _cover, ref _albres, ref _rescof, ref _restkb, totalDays, dayCounter, dailySoilTemperature, vlcdayBcInit))
            {
                Abort();
                return results;
            }

            //var _clouds = 0.0;
            //var _declin = 0.0;
            //var _hafday = 0.0;
            Cloudy(ref _clouds, ref _alatud, ref _declin, ref _hafday, _sunhor, ref julian, ref _nhrpdt);

            // **** BEGIN SIMULATION
            if (_lvlout[20] != 0)
            {
                //write(6, 525);
                Console.WriteLine();
                Console.WriteLine("                    Day   Hour   Year   Min Steps  Max Steps");
            }

            //var _store = 0.0;
            //var _snowex = 0.0;
            //var _dirres = 0.0;
            //var _pond = 0.0;

            var ishiter = 1;
            var ishitermax = -1;

            while (true)
            {
                // JM: add soil profiles to results
                results.AddOneBasedProfiles(_tsdt, _vlcdt);

                // **** START OF A NEW HOUR - UPDATE VALUES ASSUMED CONSTANT OVER THE HOUR
                var dt = _nhrpdt * 3600.0;
                hour = hour + _nhrpdt;

                if (hour > 24)
                {
                    //         START A NEW DAY
                    hour = hour - 24;
                    julian = julian + 1;
                    if (julian > maxjul)
                    {
                        julian = julian - maxjul;
                        year = year + 1;
                        maxjul = 365;
                        if (year % 4 == 0) maxjul = 366;
                    }

                    if (++dayCounter > totalDays)
                    //if (year == _yrend && julian > _jend || year > _yrend)
                    {
                        //Console.WriteLine();
                        //Console.WriteLine(" Normal completion; Press Enter to end");
                        //Console.ReadLine();
                        break;
                    }

                    var int0 = 0;
                    var int1 = 1;
                    if (!Dayinp(julian, year, maxjul, int0, _nhrpdt, _mtstep, _iversion, _iflagsi, int1,
                        _itmpbc, _ivlcbc, _lvlout[2], _mpltgro, _nrchang,
                        _mwatrxt, _ns, _alatud, _hrnoon, _sunhor, _tmpday, _winday, _humday, _precip, _snoden, _soitmp,
                        _vlcday, _soilxt, ref _nplant, _plthgt, _dchar, _clumpng, _pltwgt, _pltlai, _rootdp, ref _zrthik, ref _rload,
                        ref _cover, ref _albres, ref _rescof, ref _restkb, totalDays, dayCounter, dailySoilTemperature, vlcdayBcInit))
                    {
                        Abort();
                        return results;
                    }

                    Cloudy(ref _clouds, ref _alatud, ref _declin, ref _hafday, _sunhor, ref julian, ref _nhrpdt);
                }

                if (julian == _level[2] && hour == _level[3])
                {
                    //         CHANGE LEVEL OF OUTPUT
                    var lvl = _level[1];
                    _level[1] = _level[4];
                    _level[2] = _level[5];
                    _level[3] = _level[6];
                    _level[4] = lvl;
                    _level[5] = julian;
                    _level[6] = hour;
                }


                //_outputWriters[OutputFile.General].WriteLine($"CALL GOSHAW, ISHITER = {ishiter}");


                if (!Goshaw(ref julian, ref hour, ref year, ref _nhrpdt, ref _wdt, ref dt, ref _inital, ref _nc, ref _nsp, ref _nr, ref _ns, ref _toler, _level, ref _mzcinp,
                    ref _nrchang, _inph2o, _mwatrxt, _lvlout, ref _ivlcbc, ref _itmpbc, ref _tsavg, ref _nplant, _plthgt, _pltwgt, _pltlai, _rootdp,
                     _dchar, _tccrit, _rstom0, _rstexp, _pleaf0, _rleaf0, _rroot0, _pcandt, _canalb, ref _canma, ref _canmb, ref _wcmax,
                    _pintrcp, _xangle, _clumpng, _itype, ref _istomate, _stomate, _zc, _tcdt, _tlcdt, _vapcdt, _wcandt, _zsp, _dzsp,
                    _rhosp, _tspdt, _dlwdt, _icespt, _wlag, ref _store, ref _snowex, ref _isnotmp, ref _snotmp, _zr, _rhor, _trdt, _vaprdt,
                    _gmcdt, ref _gmcmax, ref _zrthik, ref _rload, ref _cover, ref _albres, ref _rescof, ref _restkb, ref _dirres, _zs, _tsdt, _vlcdt, _vicdt,
                    _matdt, _concdt, _icesdt, _saltdt, ref _albdry, ref _albexp, _dgrade, _sltdif, _asalt, _disper, ref _zmsrf, ref _zhsrf, ref _zersrf,
                    ref _zmsp, ref _zhsp, ref _height, ref _pond, ref _pondmx, ref _alatud, ref _slope, ref _aspect, ref _hrnoon, ref _clouds, ref _declin, ref _hafday, ref _sunhor[hour],
                    ref _tmpday[hour], ref _winday[hour], ref _humday[hour], ref _precip[hour], ref _snoden[hour], ref _soitmp[hour], ref _vlcday[hour], _soilxt[hour], ishiter, results))
                {
                    Abort();
                    return results;
                }


                if (ishiter == ishitermax)
                {
                    if (_lvlout[1] > 0)
                        _outputWriters[OutputFile.EntireProfile].WriteLine($"STOPPED AFTER ISHAWITER = {ishiter}");
                    break;
                }
                ++ishiter;

                //if (julian == 292)
                //{
                //    _outputWriters[OutputFile.General].WriteLine($"STOPPED AT JULIAN = {julian}");
                //    break;
                //}

                _inital = 1;
                _outputInital = 1;

                //if (WeatherDataEOS)
                //{
                //    Console.WriteLine();
                //    Console.WriteLine(" Normal completion; Press Enter to end");
                //    Console.ReadLine();
                //    break;
                //}
            }

            // done with simulation

            Cleanup();

            results.MakeProfileAveragesOverDays();
            results.Success = true;
            return results;
        }

        private bool Goshaw(ref int julian, ref int hour, ref int year, ref int nhrpdt, ref double wwdt, ref double dtime, ref int inital, ref int nc, ref int nsp,
            ref int nr, ref int ns, ref double toler, int[] level, ref int mzcinp, ref int nrchang, int inph2o, int mwatrxt, int[] lvlout, ref int ivlcbc,
            ref int itmpbc, ref double tsavg, ref int nplant, double[] plthgt, double[] pltwgt, double[] pltlai, double[] rootdp, double[] dchar, double[] tccrit,
            double[] rstom0, double[] rstexp, double[] pleaf0, double[] rleaf0, double[] rroot0, double[] pcandt, double[] canalb, ref double canma, ref double canmb,
            ref double wcmax, double[] pintrcp, double[] xangle, double[] clumpng, int[] itype, ref int istomate, double[][] stomate, double[] zc, double[] tcdt,
            double[][] tlcdt, double[] vapcdt, double[] wcandt, double[] zsp, double[] dzsp, double[] rhosp, double[] tspdt, double[] dlwdt, int[] icespt, double[] wlag,
            ref double store, ref double snowex, ref int isnotmp, ref double snotmp, double[] zr, double[] rhor, double[] trdt, double[] vaprdt, double[] gmcdt,
            ref double gmcmax, ref double zrthik, ref double rload, ref double cover, ref double albres, ref double rescof, ref double restkb, ref double dirres,
            double[] zs, double[] tsdt, double[] vlcdt, double[] vicdt, double[] matdt, double[][] concdt, int[] icesdt, double[][] saltdt, ref double albdry,
            ref double albexp, double[] dgrade, double[] sltdif, double[] asalt, double[] disper, ref double zmsrf, ref double zhsrf, ref double zersrf, ref double zmsp,
            ref double zhsp, ref double height, ref double pond, ref double pondmx, ref double alatud, ref double slope, ref double aspect, ref double hrnoon,
            ref double clouds, ref double declin, ref double hafday, ref double sunhor, ref double tmpday, ref double winday, ref double humday, ref double precip,
            ref double snoden, ref double soitmp, ref double vlcday, double[] soilxt, int ishiter, ShawDammResults results)
        {
            var lwcan = JaggedArray<double>(10, 11);
            var lwsnow = 0.0;
            var lwres = new double[11];
            var lwsoil = 0.0;
            var lwdown = new double[12];
            var lwup = new double[12];
            var swcan = JaggedArray<double>(10, 11);
            var swsnow = new double[101];
            var swres = new double[11];
            var swdown = new double[12];
            var swup = new double[12];
            var ts = new double[51];
            var mat = new double[51];
            var conc = JaggedArray<double>(11, 51);
            var vlc = new double[51];
            var vic = new double[51];
            var qsl = new double[51];
            var qsv = new double[51];
            var xtract = new double[51];
            var flolat = new double[51];
            var us = new double[51];
            var ss = new double[51];
            var tk = new double[51];
            var cs = new double[51];
            var salt = JaggedArray<double>(11, 51);
            var sink = JaggedArray<double>(11, 51);
            var rootxt = new double[51];
            var totflo = new double[51];
            var totlat = new double[51];
            var bts = new double[51];
            var bmat = new double[51];
            var bconc = JaggedArray<double>(11, 51);
            var bvlc = new double[51];
            var bvic = new double[51];
            var bsalt = JaggedArray<double>(11, 51);
            var tr = new double[11];
            var vapr = new double[11];
            var gmc = new double[11];
            var ur = new double[11];
            var sr = new double[11];
            var qvr = new double[11];
            var btr = new double[11];
            var bvapr = new double[11];
            var bgmc = new double[11];
            var tc = new double[12];
            var tlc = JaggedArray<double>(9, 11);
            var vapc = new double[12];
            var wcan = new double[11];
            var pcan = new double[9];
            var trnsp = new double[10];
            var uc = new double[11];
            var sc = new double[11];
            var qvc = new double[11];
            var btc = new double[12];
            var btlc = JaggedArray<double>(9, 11);
            var bvapc = new double[12];
            var bwcan = new double[11];
            var bpcan = new double[9];
            var tsp = new double[101];
            var dlw = new double[101];
            var usp = new double[101];
            var ssp = new double[101];
            var qvsp = new double[101];
            var tqvsp = new double[101];
            var btsp = new double[101];
            var bdlw = new double[101];
            var delta = new double[151];
            var delnrg = new double[151];
            var delwtr = new double[151];
            var hrstrt = 0;
            var htordr = new int[9];
            var ibices = new int[51];
            var ices = new int[51];
            var icesp = new int[101];

            var generalOut = _outputWriters[OutputFile.EntireProfile];
            var dummy = 0.0;

            var runoff = 0.0;
            var evap1 = 0.0;
            var etsum = 0.0;
            var nclst = 0;
            var windsub = 0.0;
            var tempsub = 0.0;
            var tsurface = 0.0;
            var ncmax = 0;
            var swsoil = 0.0;
            var maxtry = 0;
            var seep = 0.0;
            var tswsno = 0.0;
            var tlwsno = 0.0;
            var tswcan = 0.0;
            var tlwcan = 0.0;
            var tswres = 0.0;
            var tlwres = 0.0;
            var tswsoi = 0.0;
            var tlwsoi = 0.0;
            var tswdwn = 0.0;
            var tlwdwn = 0.0;
            var tswup = 0.0;
            var tlwup = 0.0;
            var thflux = 0.0;
            var tgflux = 0.0;
            var tseep = 0.0;
            var topsno = 0.0;
            var vapsp = 0.0;
            var vapspt = 0.0;

            var ntimes = 0;
            var ndt = 0;
            var maxndt = 0;
            var maxdbl = 0;
            var ta = 0.0;
            var tadt = 0.0;
            var hum = 0.0;
            var humdt = 0.0;
            var wind = 0.0;
            var vapa = 0.0;
            var vapadt = 0.0;
            var tsns = 0.0;
            var vlcns = 0.0;
            var dampng = 0.0;
            var dzs = 0.0;
            var c1ddt = 0.0;
            var vlc1 = 0.0;
            var availa = 0.0;
            var iter = 0;
            var itrslt = 0;
            var n = 0;
            var satvdt = 0.0;
            var materl = 0;
            var hflux = 0.0;
            var vflux = 0.0;
            var tmp = 0.0;
            var tmpdt = 0.0;
            var tlconc = 0.0;
            var tlcndt = 0.0;
            var totpot = 0.0;
            var totpdt = 0.0;
            var gflux = 0.0;
            var ieflag = 0;
            var iwflag = 0;
            var ice = 0;
            var chkmat = 0.0;
            var dldm = 0.0;
            var rain = 0.0;
            var nsplst = 0;
            var satv = 0.0;
            var vap = 0.0;
            var vapdt = 0.0;


            // line 285
            if (inital == 0)
            {
                //        FIRST TIME INTO SUBROUTINE FOR CURRENT PROFILE -- INITIALIZE
                //        POORLY DEFINED STATE VARIABLES
                //
                //        DEFINE MATRIC POTENTIAL OR WATER CONTENT OF SOIL AND DETERMINE
                //        WHETHER SOIL IS FROZEN
                for (var i = 1; i <= ns; ++i)
                {
                    if (inph2o != 1)
                    {
                        //              INPUT SOIL MOISTURE IS WATER CONTENT
                        if (vlcdt[i] <= _slparm.Soilwrc[i][4])
                        {
                            //                  INVALID INPUT SOIL WATER CONTENT
                            Console.WriteLine(" *** Input initial soil water content is less ***");
                            Console.WriteLine(" *** than or equal to residual water content. ***");
                            if (_lvlout[1] > 0)
                            {
                                generalOut.WriteLine(" *** Input initial soil water content is less ***");
                                generalOut.WriteLine(" *** than or equal to residual water content. ***");
                            }
                            return false;
                        }
                        Matvl1(i, ref matdt[i], ref vlcdt[i], ref dummy);
                    }
                    else
                    {
                        //              INPUT SOIL MOISTURE IS MATRIC POTENTIAL
                        Matvl2(i, ref matdt[i], ref vlcdt[i], ref dummy);
                        vicdt[i] = 0.0;
                        icesdt[i] = 0;
                    }
                    if (tsdt[i] <= 0.0)
                    {
                        if (!Frozen(ref i, vlcdt, vicdt, matdt, concdt, tsdt, saltdt, icesdt))
                            return false;
                    }
                label10:;
                }
                //
                //        INITIALIZE PONDING AND SNOWPACK LAG AND STORAGE
                pond = 0.0;
                store = 0.0;
                snowex = 0.0;
                for (var i = 1; i <= 11; ++i)
                {
                    wlag[i] = 0.0;
                label15:;
                }
                //
                if (zrthik > 0.0)
                {
                    //           RESIDUE LAYER PRESENT AT BEGINNING OF SIMULATION
                    Residue(ref nr, ref nrchang, ref inital, zr, rhor, trdt, vaprdt, gmcdt, ref rload, ref zrthik, ref cover, ref dirres, ref tmpday, ref humday, ref _slparm.Nsalt, tsdt, matdt, concdt);
                }
                else
                {
                    nr = 0;
                }
                //
                if (nplant != 0)
                {
                    //           DEFINE LAYERING OF CANOPY AND ROOT DISTRIBUTION
                    Canopy(ref nplant, ref nc, ref ncmax, ref nsp, 0, ref mzcinp, itype, htordr, 0, plthgt, pltwgt, pltlai, rleaf0, zsp, zc, tc, tcdt, tlc, tlcdt, vapc, vapcdt, wcan, wcandt, ref wcmax, pcandt, ref canma, ref canmb, ref tmpday, ref humday);
                    //           DETERMINE ROOT DISTRIBUTION (IF NOT DEFINED BY USER)
                    if (mzcinp <= 0 && nc > 0) Rtdist(ref nplant, itype, ref ns, zs, rootdp, rroot0);
                }
                else
                {
                    nc = 0;
                    ncmax = 0;
                }
                //
                hrstrt = hour - nhrpdt;
                //        INITIALIZE THE WATER BALANCE SUMMARY
                if (lvlout[11] != 0) Wbalnc(ref nplant, ref nc, ref nsp, ref nr, ref ns, ref lvlout[11], ref julian, ref hrstrt, ref year, itype, _outputInital, zc, wcan, wcandt, pcan, pcandt, vapc, vapcdt, rhosp,
                    dzsp, dlwdt, wlag, ref store, zr, gmc, gmcdt, vapr, vaprdt, rhor, zs, vlc, vlcdt, vic, vicdt, totflo, ref precip, ref runoff, ref pond, ref evap1, ref _goshawSave.Melt, ref etsum, results);
                //
                //        PRINT OUT INITIAL CONDITIONS
                Output(nplant, nc, ncmax, nsp, nr, ns, lvlout, 0, inph2o, julian, hrstrt, year, _outputInital, zc, tcdt, tlcdt, vapcdt, wcandt, rootxt, rhosp, zsp, tspdt, dlwdt, zr, trdt, vaprdt, gmcdt, zs, tsdt, vlcdt, vicdt, matdt, totflo, totlat, concdt, saltdt, tmpday, humday, vapcdt[1], winday, nclst, windsub, tempsub, tsurface, swdown, swup, lwdown, lwup, evap1, _goshawSave.Melt);

                //XOUT>    VICDT,MATDT,TOTFLO,CONCDT,SALTDT,
                //XOUT>    TMPDAY,HUMDAY,VAPCDT(1),WINDAY)
                //
                //        PRINT OUT SNOW TEMPERATURE PROFILE
                if (lvlout[9] != 0) Snowtemp(ref nsp, ref lvlout[9], ref julian, ref hrstrt, ref year, _outputInital, zsp, tspdt);
                //
                //        PRINT OUT INITIAL FROST AND SNOW DEPTH
                if (lvlout[15] != 0) Frost(ref nsp, ref ns, ref lvlout[15], ref julian, ref hrstrt, ref year, _outputInital, zsp, rhosp, dzsp, dlwdt, wlag, ref store, zs, vlcdt, vicdt, icesdt);
                //
            }
            else
            {
                if (nplant > 0)
                {
                    //         DEFINE LAYERING OF CANOPY AND ROOT DISTRIBUTION
                    Canopy(ref nplant, ref nc, ref ncmax, ref nsp, 0, ref mzcinp, itype, htordr, 1, plthgt, pltwgt, pltlai, rleaf0, zsp, zc, tc, tcdt, tlc, tlcdt, vapc, vapcdt, wcan, wcandt, ref wcmax, pcandt, ref canma, ref canmb, ref tmpday, ref humday);
                    //         DETERMINE ROOT DISTRIBUTION (IF NOT DEFINED BY USER)
                    if (mzcinp <= 0 && nc > 0) Rtdist(ref nplant, itype, ref ns, zs, rootdp, rroot0);
                }
                else
                {
                    nc = 0;
                    ncmax = 0;
                }
                //
                if (nrchang > 0)
                {
                    if (zrthik > 0.0)
                    {
                        //             UPDATE RESIDUE LAYER PARAMETERS
                        Residue(ref nr, ref nrchang, ref inital, zr, rhor, trdt, vaprdt, gmcdt, ref rload, ref zrthik, ref cover, ref dirres, ref tmpday, ref humday, ref _slparm.Nsalt, tsdt, matdt, concdt);
                    }
                    else
                    {
                        //             NO RESIDUE LAYER
                        nr = 0;
                    }
                }
                //-----------------------------------------------------------------------
                if (level[1] >= 1)
                {
                    //        PRINT CONDITIONS AT BEGINNING OF TIME STEP FOR DEBUGGING
                    Output(nplant, nc, ncmax, nsp, nr, ns, lvlout, 1, inph2o, julian, hour - nhrpdt, year, 1, zc, tcdt, tlcdt, vapcdt, wcandt, rootxt, rhosp, zsp, tspdt, dlwdt, zr, trdt, vaprdt, gmcdt, zs, tsdt, vlcdt, vicdt, matdt, totflo, totlat, concdt, saltdt, tmpday, humday, vapcdt[1], winday, nclst, windsub, tempsub, tsurface, swdown, swup, lwdown, lwup, evap1, _goshawSave.Melt);
                    //XOUT>   MATDT,TOTFLO,TOTLAT,CONCDT,SALTDT,
                    //XOUT>   TMPDAY,HUMDAY,VAPCDT(1),WINDAY)
                }
                //-----------------------------------------------------------------------
            }

            // line 420

            _timewt.Wdt = wwdt;
            _timewt.Wt = 1.0 - _timewt.Wdt;
            _timewt.Dt = dtime;
            ntimes = 1;
            ndt = 1;
            maxndt = nhrpdt * 80;
            maxdbl = maxndt;
            //
            //
            //
            //**** UPDATE BOUNDARY CONDITIONS FOR NEXT TIME STEP
            ta = tmpday;
            tadt = tmpday;
            hum = humday;
            humdt = humday;
            wind = winday;
            Vslope(ref dummy, ref satv, ref tmpday);
            vapa = hum * satv;
            vapadt = vapa;
            //
            //
            //**** SET LOWER BOUNDARY CONDITIONS
            if (itmpbc <= 0)
            {
                //        LOWER TEMPERATURE BOUNDARY SPECIFIED FROM INPUT FILE
                tsdt[ns] = soitmp;
            }
            else if (itmpbc == 1)
            {
                //....    ESTIMATE TEMP AT LOWER BOUNDARY BY METHOD OF HIROTA ET AL 2002,
                //        JGR 107, NO. d24, 4767, doi:10.1029/2001JD001280, 2002
                Soiltk(ref ns, tk, vlcdt, vicdt);
                Soilht(ref ns, cs, vlcdt, vicdt, tsdt, matdt, concdt);
                if (tsdt[ns] <= 0.0)
                {
                    //           ADJUST HEAT CAPACITY FOR SOIL FREEZING - INCLUDE LATENT
                    //           HEAT OF FUSION OVER THE NEXT 1.0C INTERVAL
                    tsns = tsdt[ns];
                    vlcns = vlcdt[ns];
                    if (tsdt[ns - 1] > tsdt[ns])
                    {
                        tsdt[ns] = tsdt[ns] + 1.0;
                    }
                    else
                    {
                        tsdt[ns] = tsdt[ns] - 1.0;
                    }
                    if (!Frozen(ref ns, vlcdt, vicdt, matdt, concdt, tsdt, saltdt, icesdt))
                        return false;
                    cs[ns] = cs[ns] + Math.Abs(Constn.Rhol * Constn.Lf * (vlcns - vlcdt[ns]));
                    //           RESET BOTTOM TEMPERATURE
                    tsdt[ns] = tsns;
                    if (!Frozen(ref ns, vlcdt, vicdt, matdt, concdt, tsdt, saltdt, icesdt))
                        return false;
                }
                //        CALCULATE DAMPING DEPTH, DELTA Z, AND C1(DELTA)/DT
                dampng = Math.Sqrt(2.0 * tk[ns] / cs[ns] / 1.99238e-07);
                dzs = zs[ns] - zs[ns - 1];
                c1ddt = (1.0 + 2.0 * dzs / dampng) / _timewt.Dt;
                tsdt[ns] = tsdt[ns] + (-2.0 * tk[ns] / cs[ns] / dampng / dzs / c1ddt) * (tsdt[ns] - tsdt[ns - 1]) - 1.99238e-07 * (tsdt[ns] - tsavg) / c1ddt;
                //        CALCULATE WEIGHTING COEFFICIENT BASED ON DAILY TIME STEP
                //cc      AA=(-0.00082+0.00983957*DAMPNG/(ZS(NS)-ZS(NS-1)))
                //cc  >       *(ZS(NS)/DAMPNG)**(-0.381266)
                //cc      IF (AA .LT. 0.0) AA=0.0
                //        ADJUST FOR ACTUAL TIME STEP
                //cc      AA=AA*NHRPDT/24
                //cc      TSDT(NS)=(1.0-AA)*TSDT(NS) + AA*TSDT(NS-1)
            }
            else
            {
                //....    ZERO FLUX AT BOTTOM BOUNDARY;
                //        SET TEMPERATURE FOR LAST NODE EQUAL TO SECOND TO LAST NODE
                tsdt[ns] = tsdt[ns - 1];
            }
            //
            if (ivlcbc > 0)
            {
                //....    UNIT GRADIENT IS ASSUMED FOR WATER FLUX AT BOTTOM BOUNDARY;
                //        SET MATRIC POTENTIAL FOR LAST NODE EQUAL TO SECOND TO LAST NODE
                //        IF FREEZING FRONT IS IN NS-1, DO NOT CHANGE POTENTIAL AT BOTTOM
                if (vicdt[ns - 1] <= 0.0 || vicdt[ns] > 0.0) matdt[ns] = matdt[ns - 1];
                vlc1 = vlcdt[ns] + vicdt[ns] * Constn.Rhoi / Constn.Rhol;
                vicdt[ns] = 0.0;
                Matvl2(ns, ref matdt[ns], ref vlcdt[ns], ref dummy);
                if (tsdt[ns] < 0.0) vicdt[ns] = (vlc1 - vlcdt[ns]) * Constn.Rhol / Constn.Rhoi;
                if (vicdt[ns] < 0.0) vicdt[ns] = 0.0;
            }
            else
            {
                //        INPUT WATER CONTENT SPECIFIED FOR WATER FLUX AT BOTTOM BOUNDARY
                if (inph2o != 1)
                {
                    //           INPUT SOIL MOISTURE IS WATER CONTENT
                    vlcdt[ns] = vlcday;
                    if (vlcdt[ns] <= _slparm.Soilwrc[ns][4])
                    {
                        //               INVALID INPUT SOIL WATER CONTENT
                        Console.WriteLine();
                        Console.WriteLine(" *** Input soil water content at lower boundary   ***");
                        Console.WriteLine(" *** less than or equal to residual water content.***");
                        if (_lvlout[1] > 0)
                        {
                            generalOut.WriteLine();
                            generalOut.WriteLine(" *** Input soil water content at lower boundary   ***");
                            generalOut.WriteLine(" *** less than or equal to residual water content.***");
                        }

                        return false;
                    }
                    Matvl1(ns, ref matdt[ns], ref vlcdt[ns], ref dummy);
                }
                else
                {
                    //           INPUT SOIL MOISTURE IS MATRIC POTENTIAL
                    matdt[ns] = vlcday;
                    Matvl2(ns, ref matdt[ns], ref vlcdt[ns], ref dummy);
                }
                vicdt[ns] = 0.0;
            }
            icesdt[ns] = 0;
            for (var j = 1; j <= _slparm.Nsalt; ++j)
            {
                concdt[j][ns] = saltdt[j][ns] / (_slparm.Saltkq[j][ns] + vlcdt[ns] * Constn.Rhol / _slparm.Rhob[ns]);
            label105:;
            }
            if (tsdt[ns] <= 0.0)
            {
                if (!Frozen(ref ns, vlcdt, vicdt, matdt, concdt, tsdt, saltdt, icesdt))
                    return false;
            }
            //
            //
            if (mwatrxt == 1)
            {
                //****    SOIL SINK TERM INPUT FOR EACH LAYER
                //        CHECK IF SOIL LAYERS CAN SATIFY SINK TERM
                availa = vlcdt[1] * (zs[2] - zs[1]) / 2;
                if (soilxt[1] * _timewt.Dt > availa) soilxt[1] = 0.0;
                availa = vlcdt[ns] * (zs[ns] - zs[ns - 1]) / 2;
                if (soilxt[ns] * _timewt.Dt > availa) soilxt[ns] = 0.0;
                for (var i = 2; i <= ns - 1; ++i)
                {
                    availa = vlcdt[i] * (zs[i + 1] - zs[i - 1]) / 2;
                    if (soilxt[i] * _timewt.Dt > availa) soilxt[i] = 0.0;
                    label1105:;
                }
            }
            else
            {
                //        SET SOIL SINK TERM TO ZERO
                for (var i = 1; i <= ns; ++i)
                {
                    soilxt[i] = 0.0;
                label1106:;
                }
            }

            // line 549

            //**** DEFINE THE ORDER IN WHICH THE MATERIALS ARE LAYERED
            Systm(ref nc, ref nplant, ref nsp, htordr, zc, zsp, ref zmsrf, ref zhsrf, ref zersrf, ref zmsp, ref zhsp, ref height, ref sunhor, ref tmpday, tccrit);
            //
            //
            //**** CALCULATE THE SHORT-WAVE ENERGY BALANCE
            Swrbal(ref nplant, ref nc, ref nsp, ref nr, xangle, clumpng, swcan, swsnow, swres, ref swsoil, swdown, swup, ref sunhor, canalb, zsp, dzsp, rhosp, zr, rhor, ref albres, ref dirres, ref albdry, ref albexp, ref vlcdt[1], ref alatud, ref slope, ref aspect, ref hrnoon, ref hafday, ref declin, ref hour, ref nhrpdt);
            //
            //
            //**** SAVE VALUES AT THE BEGINNING OF THE TIME STEP
            Update(ref ns, ref nr, ref nsp, ref nc, ref nplant, ref _slparm.Nsalt, ibices, icesdt, bts, tsdt, bmat, matdt, bconc, concdt, bvlc, vlcdt, bvic, vicdt, bsalt, saltdt, btr, trdt, bvapr, vaprdt, bgmc, gmcdt, btc, tcdt, btlc, tlcdt, bvapc, vapcdt, bwcan, wcandt, bpcan, pcandt, btsp, tspdt, bdlw, dlwdt, icesp, icespt);
        //
        //**** UPDATE PARAMETERS FOR NEXT TIME STEP
        label110:;
            Update(ref ns, ref nr, ref nsp, ref nc, ref nplant, ref _slparm.Nsalt, ices, icesdt, ts, tsdt, mat, matdt, conc, concdt, vlc, vlcdt, vic, vicdt, salt, saltdt, tr, trdt, vapr, vaprdt, gmc, gmcdt, tc, tcdt, tlc, tlcdt, vapc, vapcdt, wcan, wcandt, pcan, pcandt, tsp, tspdt, dlw, dlwdt, icesp, icespt);
        //
        //CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC

        //**** START ITERATIVE PROCEDURE TO SOLVE ENERGY AND MOISTURE BALANCE
        //
        label120:;
            iter = 0;
            itrslt = 0;
        label200:;
            iter = iter + 1;
            if (_timewt.Wdt <= 1.0)
            {
                //        IF SATURATED OR EXTREMELY DRY CONDITIONS EXIST,
                //        SPECIFY FULLY IMPLICIT SOLUTION
                for (var i = ns - 1; i >= 1; --i)
                {
                    if (matdt[i] > _slparm.Soilwrc[i][1] || vlcdt[i] - _slparm.Soilwrc[i][4] < 0.001)
                    {
                        _timewt.Wdt = 1.0;
                        _timewt.Wt = 0.0;
                        goto label101;
                    }
                label100:;
                }
            label101:;
            }
            //
            //**** DETERMINE LONG-WAVE RADIATION BALANCE FOR EACH NODE
            Lwrbal(ref nc, ref nsp, ref nr, ref nplant, ref ta, ref tadt, tlc, tlcdt, tsp, tspdt, tr, trdt, ts, tsdt, ref vapa, ref vapadt, ref clouds, lwcan, ref lwsnow, lwres, ref lwsoil, lwdown, lwup);
            //
            //**** CALCULATE LONG-WAVE RAD. CONTRIBUTION TO ENERGY BALANCE MATRIX
            Lwrmat(ref nc, ref nsp, ref nr, tc, tsp, tr, ts, icespt);
            //
            //**** SUM THE SOURCE-SINK TERMS FOR EACH NODE
            Source(ref nc, ref nsp, ref nr, ref ns, ref _slparm.Nsalt, ref nplant, uc, sc, usp, ssp, ur, sr, us, ss, sink, swcan, swsnow, swres, ref swsoil, lwcan, ref lwsnow, lwres, ref lwsoil, soilxt);
            //
            n = 1;

            //**** DETERMINE THE BOUNDARY CONDITION FOR THE SURFACE MATERIAL
            materl = 2;
            //
            if (nc > 0)
            {
                //        CANOPY IS THE SURFACE MATERIAL
                var int0 = 0;
                Atstab(ref nplant, ref nc, ref nsp, ref nr, ref ta, ref tadt, ref tcdt[1], ref tcdt[1], ref vapa, ref vapadt, ref vapcdt[1], ref vapcdt[1], ref wind, ref height, ref hflux, ref vflux, zc, zr, rhor, ref int0, ref iter);
                //        GO THROUGH CALCULATION OF THE VAPOR DENSITY AT THE SOIL SURFACE
                goto label205;
            }
            //
            if (nsp > 0)
            {
                //        SNOW IS THE SURFACE MATERIAL
                Vslope(ref dummy, ref satv, ref tsp[1]);
                Vslope(ref dummy, ref satvdt, ref tspdt[1]);
                //        CALCULATE THE SATURATED VAPOR DENSITY OVER ICE
                tmp = tsp[1] + 273.16;
                tmpdt = tspdt[1] + 273.16;
                vap = satv * Math.Exp(0.018 / (Constn.Ugas * tmp) * Constn.Lf * tsp[1] / tmp);
                vapdt = satvdt * Math.Exp(0.018 / (Constn.Ugas * tmpdt) * Constn.Lf * tspdt[1] / tmpdt);
                Atstab(ref nplant, ref nc, ref nsp, ref nr, ref ta, ref tadt, ref tsp[1], ref tspdt[1], ref vapa, ref vapadt, ref vap, ref vapdt, ref wind, ref height, ref hflux, ref vflux, zc, zr, rhor, ref icespt[1], ref iter);
                //        GO THROUGH CALCULATION OF THE VAPOR DENSITY AT THE SOIL SURFACE
                goto label205;
            }
            //
            if (nr > 0)
            {
                //        RESIDUE IS THE SURFACE MATERIAL
                var int0 = 0;
                Atstab(ref nplant, ref nc, ref nsp, ref nr, ref ta, ref tadt, ref tr[1], ref trdt[1], ref vapa, ref vapadt, ref vapr[1], ref vaprdt[1], ref wind, ref height, ref hflux, ref vflux, zc, zr, rhor, ref int0, ref iter);
            }
        //
        //     SOIL IS THE SURFACE MATERIAL
        //     ---- FIRST CALCULATE THE TOTAL WATER POTENTIAL, THEN THE HUMIDITY
        //          MAY BE DETERMINED.  CALL VSLOPE TO OBTAIN THE SATURATED
        //          VAPOR DENSITY.  USING HUMIDITY AND SATURATED VAPOR DENSITY,
        //          THE VAPOR PRESSURE AT THE SOIL SURFACE MAY BE CALCULATED.
        label205:;
            tlconc = 0.0;
            tlcndt = 0.0;
            for (var j = 1; j <= _slparm.Nsalt; ++j)
            {
                tlconc = tlconc + conc[j][1];
                tlcndt = tlcndt + concdt[j][1];
            label210:;
            }
            totpot = mat[1] - tlconc * Constn.Ugas * (ts[1] + 273.16) / Constn.G;
            totpdt = matdt[1] - tlcndt * Constn.Ugas * (tsdt[1] + 273.16) / Constn.G;
            Vslope(ref dummy, ref satv, ref ts[1]);
            Vslope(ref dummy, ref satvdt, ref tsdt[1]);
            vap = satv * Math.Exp(.018 * Constn.G / Constn.Ugas / (ts[1] + 273.16) * totpot);
            vapdt = satvdt * Math.Exp(.018 * Constn.G / Constn.Ugas / (tsdt[1] + 273.16) * totpdt);
            //
            if (nc == 0 && nr == 0 && nsp == 0) Atstab(ref nplant, ref nc, ref nsp, ref nr, ref ta, ref tadt, ref ts[1], ref tsdt[1], ref vapa, ref vapadt, ref vap, ref vapdt, ref wind, ref height, ref hflux, ref vflux, zc, zr, rhor, ref icesdt[1], ref iter);
            //
            //
            //**** DETERMINE ENERGY BALANCE FOR THE CANOPY LAYERS
            if (nc > 0)
            {
                //        DEFINE VAPOR DENSITY FOR THE LOWER BOUNDARY OF CANOPY
                if (nsp > 0)
                {
                    //           CANOPY IS OVERLYING SNOWPACK
                    Vslope(ref dummy, ref satv, ref tsp[1]);
                    Vslope(ref dummy, ref satvdt, ref tspdt[1]);
                    tmp = tsp[1] + 273.16;
                    tmpdt = tspdt[1] + 273.16;
                    vapc[nc + 1] = satv * Math.Exp(0.018 / (Constn.Ugas * tmp) * Constn.Lf * tsp[1] / tmp);
                    vapcdt[nc + 1] = satvdt * Math.Exp(0.018 / (Constn.Ugas * tmpdt) * Constn.Lf * tspdt[1] / tmpdt);
                    tc[nc + 1] = tsp[1];
                    tcdt[nc + 1] = tspdt[1];
                }
                else
                {
                    if (nr > 0)
                    {
                        //              CANOPY IS OVERLYING RESIDUE
                        vapc[nc + 1] = vapr[1];
                        vapcdt[nc + 1] = vaprdt[1];
                        tc[nc + 1] = tr[1];
                        tcdt[nc + 1] = trdt[1];
                    }
                    else
                    {
                        //              CANOPY IS OVERLYING BARE SOIL
                        vapc[nc + 1] = vap;
                        vapcdt[nc + 1] = vapdt;
                        tc[nc + 1] = ts[1];
                        tcdt[nc + 1] = tsdt[1];
                    }
                }
                Ebcan(ref n, ref nplant, ref nc, ref nsp, ref nr, ref ns, zc, tc, tcdt, tlc, tlcdt, vapc, vapcdt, wcan, wcandt, pcan, pcandt, mat, matdt, swcan, lwcan, swdown, ref canma, ref canmb, dchar, rstom0, rstexp, pleaf0, rleaf0, itype, ref istomate, stomate, ref iter);
                materl = materl + 1;
            }
            //
            //**** DETERMINE ENERGY BALANCE FOR THE SNOW LAYERS
            if (nsp > 0)
            {
                //        DEFINE VAPOR DENSITY AND TEMPERATURE FOR LOWER BOUNDARY OF
                //        SNOWPACK
                if (nr > 0)
                {
                    //           SNOW IS OVERLYING RESIDUE
                    vapsp = vapr[1];
                    vapspt = vaprdt[1];
                    tsp[nsp + 1] = tr[1];
                    tspdt[nsp + 1] = trdt[1];
                }
                else
                {
                    //           SNOW IS OVERLYING BARE SOIL
                    vapsp = vap;
                    vapspt = vapdt;
                    tsp[nsp + 1] = ts[1];
                    tspdt[nsp + 1] = tsdt[1];
                }
                Ebsnow(ref n, ref nsp, ref nr, icespt, tsp, tspdt, dlw, dlwdt, rhosp, zsp, dzsp, qvsp, ref vapsp, ref vapspt, ssp, ref iter);
                materl = materl + 1;
            }

            //
            //**** DETERMINE ENERGY BALANCE FOR THE RESIDUE LAYERS
            if (nr > 0)
            {
                vapr[nr + 1] = vap;
                vaprdt[nr + 1] = vapdt;
                tr[nr + 1] = ts[1];
                trdt[nr + 1] = tsdt[1];
                Ebres(ref n, ref nr, ref nsp, zr, tr, trdt, vapr, vaprdt, gmc, gmcdt, ref gmcmax, rhor, ref rescof, ref restkb, sr, ur, qvr, rhosp, ref iter);
                materl = materl + 1;
            }
            //
            //**** SOLVE FOR ENERGY BALANCE OF SOIL
            Ebsoil(ref n, ref ns, zs, ts, tsdt, mat, matdt, conc, concdt, vlc, vlcdt, vic, vicdt, ices, icesdt, qsl, qsv, ss, ref gflux, ref iter);
            //
            //-----------------------------------------------------------------------
            //if (Math.Abs(_matrix.B1[4]) > 50000.0)
            //    generalOut.WriteLine(" B1[4] exceeds limits");

            if (level[1] >= 2 && _lvlout[1] > 0)
            {
                generalOut.WriteLine($"HFLUX (W/M2), VFLUX (KG/S): {hflux} {vflux}");
                generalOut.WriteLine(" VAPOR FLUXES (KG/S)");

                for (var j = 1; j <= ns - 1; ++j)
                {
                    generalOut.Write($"{qsv[j],15:G6}");
                    if (j % 5 == 0 || j == ns - 1) generalOut.WriteLine();
                }
                generalOut.WriteLine();

                generalOut.WriteLine(" LIQUID FLUXES (M/S)");

                for (var j = 1; j <= ns - 1; ++j)
                {
                    generalOut.Write($"{qsl[j],15:G6}");
                    if (j % 5 == 0 || j == ns - 1) generalOut.WriteLine();
                }
                generalOut.WriteLine();

                generalOut.WriteLine(" JACOBIAN MATRIX");
                for (var k = 1; k <= n; ++k)
                    generalOut.WriteLine($"{_matrix.A1[k],18:G10} {_matrix.B1[k],18:G10} {_matrix.C1[k],18:G10} {_matrix.D1[k],18:G10}");
                //generalOut.WriteLine($"{_matrix.A1[k],13:G6} {_matrix.B1[k],13:G6} {_matrix.C1[k],13:G6} {_matrix.D1[k],13:G6}");

                //if (ishiter == 348)
                //    return true;

                generalOut.WriteLine();
                generalOut.WriteLine($" VALUES FOR ENERGY BALANCE ITERATION {iter}");
            }
            //-----------------------------------------------------------------------

            // line 761
            //**** SOLVE THE ENERGY BALANCE MATRIX
            Tdma(n, _matrix.A1, _matrix.B1, _matrix.C1, _matrix.D1, delta);
            //
            //**** SORT OUT THE SOLUTION OF THE MATRIX INTO THE PROPER MATERIALS
            materl = 2;
            n = 1;
            ieflag = 0;
            iwflag = 0;
            //
            if (nc > 0)
            {
                //**** CANOPY LAYERS
                for (var i = 1; i <= nc; ++i)
                {
                    if (Math.Abs(delta[n]) > 25.0 && ndt < maxndt)
                    {
                        //           TOO LARGE OF A CHANGE -- CONVERGENCE WILL NOT LIKELY BE
                        //           BE MET WITH CURRENT TIME STEP.  CUT TIME STEP IN HALF
                        ieflag = 1;
                        iter = 11;
                        goto label350;
                    }
                    //        RELAX THE TOLERANCE FOR TEMPERATURE IN THE CANOPY TO 0.1 C
                    //        IF (ABS(DELTA(N)) .GT. TOLER) IEFLAG = IEFLAG+1
                    if (Math.Abs(delta[n]) > 0.1) ieflag = ieflag + 1;
                    if (iter > 3 && delnrg[n] * delta[n] < 0)
                    {
                        //           DELTA IS JUMPING BETWEEN NEG. AND POS. -- CUT DELTA IN HALF
                        //           TO SUPPRESS TENDENCY TO JUMP BACK AND FORTH AROUND SOLUTION
                        delta[n] = delta[n] / 2.0;
                    }
                    delnrg[n] = delta[n];
                    tcdt[i] = tcdt[i] - delta[n];
                    if (level[1] >= 2 && _lvlout[1] > 0)
                        generalOut.WriteLine($"{delta[n],18:G10} {tcdt[i],18:G10} {vapcdt[i],18:G10} {wcandt[i],18:G10}");
                    n = n + 1;
                label220:;
                }
                materl = materl + 1;
            }

            if (nsp > 0)
            {
                //**** SNOW PACK LAYERS
                for (var i = 1; i <= nsp; ++i)
                {
                    if (Math.Abs(delta[n]) > 25.0 && ndt < maxndt)
                    {
                        //           TOO LARGE OF A CHANGE -- CONVERGENCE WILL NOT LIKELY BE
                        //           BE MET WITH CURRENT TIME STEP.  CUT TIME STEP IN HALF
                        ieflag = 1;
                        iter = 11;
                        goto label350;
                    }
                    if (icespt[i] == 0)
                    {
                        //           NO LIQUID WATER IN CURRENT LAYER AT END OF TIME STEP
                        if (Math.Abs(delta[n]) > toler) ieflag = ieflag + 1;
                        if (iter > 3 && delnrg[n] * delta[n] < 0)
                        {
                            //              DELTA IS JUMPING BETWEEN NEG. AND POS. -- CUT DELTA IN HALF
                            //              TO SUPPRESS TENDENCY TO JUMP BACK AND FORTH AROUND SOLUTION
                            delta[n] = delta[n] / 2.0;
                        }
                        delnrg[n] = delta[n];
                        tspdt[i] = tspdt[i] - delta[n];
                        //           CHECK IF LAYER HAS BEGUN MELTING
                        if (tspdt[i] > 0)
                        {
                            //              LAYER HAS GONE ABOVE 0 C - ADJUST WATER CONTENT IN LAYER
                            icespt[i] = 1;
                            //CCC           DLWDT(I) = RHOI*CI*DZSP(I)*TSPDT(I)/(RHOL*LF)
                            tspdt[i] = 0.0;
                        }
                        //
                    }
                    else
                    {
                        //           LAYER CONTAINS LIQUID WATER
                        //           CONVERT TOLERANCE FOR TEMPERATURE TO LIQUID EQUIVALENT
                        //           (DELTA LIQUID FRACTION) => 0.001*(DELTA TEMP.)
                        if (Math.Abs(delta[n]) / dzsp[i] > 0.001 * toler)
                        {
                            //              IF DELTA < E-07*DLWDT, PRECISION OF COMPUTER IS EXCEEDED,
                            //              AND  ADDING DELTA TO DLWDT WILL NOT CHANGE DLWDT
                            if (Math.Abs(delta[n]) > dlwdt[i] * 10e-07) ieflag = ieflag + 1;
                        }
                        if (iter > 3 && delnrg[n] * delta[n] < 0)
                        {
                            //              DELTA IS JUMPING BETWEEN NEG. AND POS. -- CUT DELTA IN HALF
                            //              TO SUPPRESS TENDENCY TO JUMP BACK AND FORTH AROUND SOLUTION
                            delta[n] = delta[n] / 2.0;
                        }
                        delnrg[n] = delta[n];
                        dlwdt[i] = dlwdt[i] - delta[n];
                        //           CHECK IF ALL THE LIQUID HAS FROZEN
                        if (dlwdt[i] < 0.0)
                        {
                            //              LAYER HAS FROZEN COMPLETELY - ADJUST TEMPERATURE
                            icespt[i] = 0;
                            //CCC           TSPDT(I) = RHOL*LF*DLWDT(I)/(RHOI*CI)
                            dlwdt[i] = 0.0;
                        }
                    }
                    if (level[1] >= 2 && _lvlout[1] > 0)
                        generalOut.WriteLine($"{delta[n],18:G10} {tspdt[i],18:G10} {dlwdt[i],18:G10} {dzsp[i],18:G10}");
                    n = n + 1;
                label230:;
                }
                materl = materl + 1;
            }


            if (nr > 0)
            {
                //**** RESIDUE LAYERS
                for (var i = 1; i <= nr; ++i)
                {
                    if (Math.Abs(delta[n]) > 25.0 && ndt < maxndt)
                    {
                        //           TOO LARGE OF A CHANGE -- CONVERGENCE WILL NOT LIKELY BE
                        //           BE MET WITH CURRENT TIME STEP.  CUT TIME STEP IN HALF
                        ieflag = 1;
                        iter = 11;
                        goto label350;
                    }
                    if (Math.Abs(delta[n]) > toler) ieflag = ieflag + 1;
                    if (iter > 3 && delnrg[n] * delta[n] < 0)
                    {
                        //           DELTA IS JUMPING BETWEEN NEG. AND POS. -- CUT DELTA IN HALF
                        //           TO SUPPRESS TENDENCY TO JUMP BACK AND FORTH AROUND SOLUTION
                        delta[n] = delta[n] / 2.0;
                    }
                    delnrg[n] = delta[n];
                    trdt[i] = trdt[i] - delta[n];
                    if (level[1] >= 2 && _lvlout[1] > 0)
                        generalOut.WriteLine($"{delta[n],18:G10} {trdt[i],18:G10} {vaprdt[i],18:G10} {gmcdt[i],18:G10}");
                    n = n + 1;
                label240:;
                }
                materl = materl + 1;
            }

            //**** SOIL LAYERS
            for (var i = 1; i <= ns - 1; ++i)
            {
                if (Math.Abs(delta[n]) > 25.0 && ndt < maxndt)
                {
                    //           TOO LARGE OF A CHANGE -- CONVERGENCE WILL NOT LIKELY BE
                    //           BE MET WITH CURRENT TIME STEP.  CUT TIME STEP IN HALF
                    ieflag = 1;
                    iter = 11;
                    goto label350;
                }
                if (Math.Abs(delta[n]) > toler) ieflag = ieflag + 1;
                if (iter > 3 && delnrg[n] * delta[n] < 0)
                {
                    //           DELTA IS JUMPING BETWEEN NEG. AND POS. -- CUT DELTA IN HALF
                    //           TO SUPPRESS TENDENCY TO JUMP BACK AND FORTH AROUND SOLUTION
                    delta[n] = delta[n] / 2.0;
                }
                delnrg[n] = delta[n];
                tsdt[i] = tsdt[i] - delta[n];
                //
                //        CHECK IF LAYER IS BELOW 0 C
                if (tsdt[i] <= 0.0)
                {
                    //           ICE MAY BE PRESENT - CALL FROZEN TO DETERMINE IF ICE PRESENT
                    ice = icesdt[i];
                    if (!Frozen(ref i, vlcdt, vicdt, matdt, concdt, tsdt, saltdt, icesdt))
                        return false;
                    //           CHECK IF LAYER HAS CROSSED THE FREEZING POINT AND ADJUST THE
                    //           TEMPERATURE FOR LATENT HEAT IF SO
                    if (ice == 0 && icesdt[i] == 1)
                    {
                        if (!Adjust(ref i, vicdt, vlcdt, matdt, concdt, tsdt, saltdt, icesdt))
                            return false;
                    }
                    //
                }
                else
                {
                    //           NO ICE IS PRESENT
                    if (icesdt[i] == 1)
                    {
                        //              CONVERT ANY REMAINING ICE TO WATER
                        vlcdt[i] = vlcdt[i] + vicdt[i] * Constn.Rhoi / Constn.Rhol;
                        if (vlcdt[i] > _slparm.Soilwrc[i][2]) vlcdt[i] = _slparm.Soilwrc[i][2];
                        vicdt[i] = 0.0;
                        icesdt[i] = 0;
                        //              IF WATER CONTENT AT RESIDUAL WATER CONTENT, DO NOT
                        //              REDEFINE MATRIC POTENTIAL
                        if (vlcdt[i] > _slparm.Soilwrc[i][4]) Matvl1(i, ref matdt[i], ref vlcdt[i], ref dummy);
                    }
                }
                //
                if (level[1] >= 2 && _lvlout[1] > 0)
                {
                    generalOut.WriteLine($"{delta[n],18:G10} {tsdt[i],18:G10} {vlcdt[i],18:G10} {vicdt[i],18:G10} {matdt[i],18:G10}");
                    generalOut.WriteLine($"{concdt[1][i],18:G10} {icesdt[i],18:G10}");
                }
                n = n + 1;
            label250:;
            }

            if (nc > 0)
            {
                //        DEFINE TEMPERATURE OF BOTTOM BOUNDARY OF CANOPY
                if (nsp > 0)
                {
                    //           SNOW IS UNDERLYING CANOPY
                    tcdt[nc + 1] = tspdt[1];
                }
                else
                {
                    if (nr > 0)
                    {
                        //              RESIDUE IS UNDERLYING CANOPY
                        tcdt[nc + 1] = trdt[1];
                    }
                    else
                    {
                        //              SOIL IS UNDERLYING CANOPY
                        tcdt[nc + 1] = tsdt[1];
                    }
                }
            }
            //
            if (nsp > 0)
            {
                //        DEFINE TEMPERATURE OF BOTTOM BOUNDARY OF SNOWPACK
                if (nr > 0)
                {
                    //           SNOW IS OVERLYING RESIDUE
                    tspdt[nsp + 1] = trdt[1];
                }
                else
                {
                    //           SNOW IS OVERLYING BARE SOIL
                    tspdt[nsp + 1] = tsdt[1];
                }
            }
            //
            //     DEFINE TEMPERATURE OF BOTTOM BOUNDARY OF RESIDUE
            if (nr > 0) trdt[nr + 1] = tsdt[1];
            //
            if (itmpbc == 2)
            {
                //....    ZERO HEAT FLUX AT BOTTOM BOUNDARY;
                //        SET TEMPERATURE FOR LAST NODE EQUAL TO SECOND TO LAST NODE
                tsdt[ns] = tsdt[ns - 1];
            }
            //
            //-----------------------------------------------------------------------
            if (level[1] >= 1 && _lvlout[1] > 0)
            {
                generalOut.WriteLine($" ENERGY BALANCE AT ITER = {iter} {julian} {hour} {ntimes} {ndt}");
                Output(nplant, nc, ncmax, nsp, nr, ns, lvlout, 1, inph2o, julian, hour, year, 1, zc, tcdt, tlcdt, vapcdt, wcandt, rootxt, rhosp, zsp, tspdt, dlwdt, zr, trdt, vaprdt, gmcdt, zs, tsdt, vlcdt, vicdt, matdt, totflo, totlat, concdt, saltdt, ta, hum, vapa, wind, nclst, windsub, tempsub, tsurface, swdown, swup, lwdown, lwup, evap1, _goshawSave.Melt);
                //XOUT>    MATDT,TOTFLO,TOTLAT,CONCDT,SALTDT,TA,HUM,VAPA,WIND)
            }
            //-----------------------------------------------------------------------
            //
            //CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC

            // line 978

            //**** BEGIN CALCULATIONS FOR THE MOISTURE BALANCE OF THE SYSTEM
            n = 1;
            //
            //**** SOLVE WATER BALANCE FOR CANOPY LAYERS
            if (nc > 0)
            {
                Wbcan(ref n, ref nplant, ref nc, ref nsp, ref nr, ref ns, zc, tc, tcdt, tlc, tlcdt, vapc, vapcdt, wcan, wcandt, pcan, pcandt, qvc, trnsp, mat, matdt, xtract, swcan, lwcan, swdown, ref canma, ref canmb, dchar, rstom0, rstexp, pleaf0, rleaf0, itype, ref istomate, stomate, ref icesdt[1], ref iter);
            }
            else
            {
                //        SET ROOT EXTRACTION AND PLANT TRANSPIRATION TO ZERO
                for (var i = 1; i <= ns; ++i)
                {
                    xtract[i] = 0.0;
                label300:;
                }
                //        TRNSP(NPLANT+1) IS TOTAL TRANSPIRATION FOR ALL PLANTS
                for (var i = 1; i <= nplant + 1; ++i)
                {
                    trnsp[i] = 0.0;
                label302:;
                }
            }
            //
            //**** SET BOUNDARY CONDITIONS IF THERE IS A SNOWPACK PRESENT
            //     (SNOWPACK IS NOT PART OF THE WATER BALANCE MATRIX - THE WATER
            //     BALANCE FOR THE SNOWPACK IS DONE AT THE END OF THE HOUR)
            if (nsp > 0.0)
            {
                _matrix.A2[n] = 0.0;
                Snowbc(ref n, ref nsp, ref nr, zsp, qvsp, ref vapspt, tsdt, tspdt, ref icesdt[1]);
            }
            //
            //**** SOLVE FOR WATER BALANCE OF THE RESIDUE LAYERS
            if (nr > 0) Wbres(ref n, ref nr, ref nsp, zr, tr, trdt, vapr, vaprdt, gmc, gmcdt, rhor, ref restkb, qvr, rhosp, ref icesdt[1], ref iter);
            //
            //**** SOLVE FOR THE WATER BALANCE OF SOIL LAYERS
            Wbsoil(ref n, ref ns, zs, ts, tsdt, mat, matdt, vlc, vlcdt, vic, vicdt, conc, concdt, icesdt, qsl, qsv, xtract, ref seep, flolat, us, ref slope, ref iter);
            //
            //-----------------------------------------------------------------------
            if (level[1] >= 2 && _lvlout[1] > 0)
            {
                generalOut.WriteLine($"HFLUX (W/M2), VFLUX (KG/S): {hflux} {vflux}");
                generalOut.WriteLine(" VAPOR FLUXES (KG/S)");
                //generalOut.WriteLine(string.Concat(Enumerable.Range(1, ns - 1).Select(j => $"{qsv[j],15:G6}")));
                for (var j = 1; j <= ns - 1; ++j)
                {
                    generalOut.Write($"{qsv[j],15:G6}");
                    if (j % 5 == 0 || j == ns - 1) generalOut.WriteLine();
                }
                generalOut.WriteLine();

                generalOut.WriteLine(" LIQUID FLUXES (M/S)");
                //generalOut.WriteLine(string.Concat(Enumerable.Range(1, ns - 1).Select(j => $"{qsl[j],15:G6}")));
                for (var j = 1; j <= ns - 1; ++j)
                {
                    generalOut.Write($"{qsl[j],15:G6}");
                    if (j % 5 == 0 || j == ns - 1) generalOut.WriteLine();
                }
                generalOut.WriteLine();

                generalOut.WriteLine(" JACOBIAN MATRIX");
                for (var k = 1; k <= n; ++k)
                    generalOut.WriteLine($"{_matrix.A2[k],18:G10} {_matrix.B2[k],18:G10} {_matrix.C2[k],18:G10} {_matrix.D2[k],18:G10}");

                //if (ishiter == 348)
                //    return true;

                generalOut.WriteLine("");
                generalOut.WriteLine($" VALUES FOR WATER BALANCE ITERATION {iter}");
            }
            //-----------------------------------------------------------------------
            //
            //**** SOLVE THE WATER BALANCE MATRIX
            Tdma(n, _matrix.A2, _matrix.B2, _matrix.C2, _matrix.D2, delta);
            //
            //**** SORT OUT THE SOLUTION OF THE MATRIX INTO THE PROPER MATERIALS
            materl = 2;
            n = 1;
            iwflag = 0;
            //
            if (nc > 0)
            {
                //**** CANOPY LAYERS
                for (var i = 1; i <= nc; ++i)
                {
                    //        RELAX THE TOLERANCE FOR VAPOR IN THE CANOPY TO 1% OF VAPOR
                    //        IF (ABS(DELTA(N)/VAPCDT(I)) .GT. TOLER) IWFLAG = IWFLAG+1
                    if (Math.Abs(delta[n] / vapcdt[i]) > 0.01) iwflag = iwflag + 1;
                    if (iter > 3 && delwtr[n] * delta[n] < 0)
                    {
                        //           DELTA IS JUMPING BETWEEN NEG. AND POS. -- CUT DELTA IN HALF
                        //           TO SUPPRESS TENDENCY TO JUMP BACK AND FORTH AROUND SOLUTION
                        delta[n] = delta[n] / 2.0;
                    }
                    delwtr[n] = delta[n];
                    vapcdt[i] = vapcdt[i] - delta[n];
                    if (level[1] >= 2 && _lvlout[1] > 0)
                        generalOut.WriteLine($"{delta[n],18:G10} {vapcdt[i],18:G10} {tcdt[i],18:G10} {wcandt[i],18:G10}");
                    n = n + 1;
                label310:;
                }
                materl = materl + 1;
            }
            //
            if (nsp > 0)
            {
                //**** SNOW PACK LAYERS
                //     SNOW IS NOT PART OF WATER BALANCE SOLUTION
                materl = materl + 1;
            }
            //
            if (nr > 0)
            {
                //**** RESIDUE LAYERS
                for (var i = 1; i <= nr; ++i)
                {
                    if (Math.Abs(delta[n] / vaprdt[i]) > toler) iwflag = iwflag + 1;
                    if (iter > 3 && delwtr[n] * delta[n] < 0)
                    {
                        //           DELTA IS JUMPING BETWEEN NEG. AND POS. -- CUT DELTA IN HALF
                        //           TO SUPPRESS TENDENCY TO JUMP BACK AND FORTH AROUND SOLUTION
                        delta[n] = delta[n] / 2.0;
                    }
                    delwtr[n] = delta[n];
                    vaprdt[i] = vaprdt[i] - delta[n];
                    if (level[1] >= 2 && _lvlout[1] > 0)
                        generalOut.WriteLine($"{delta[n],18:G10} {vaprdt[i],18:G10} {trdt[i],18:G10} {gmcdt[i],18:G10}");
                    n = n + 1;
                label320:;
                }
                materl = materl + 1;
            }

            //**** SOIL LAYERS
            for (var i = 1; i <= ns - 1; ++i)
            {
                if (icesdt[i] == 1)
                {
                    if (Math.Abs(delta[n]) > 1.0 && ndt < maxndt)
                    {
                        //              TOO LARGE OF A CHANGE -- CONVERGENCE WILL NOT LIKELY BE
                        //              BE MET WITH CURRENT TIME STEP.  CUT TIME STEP IN HALF
                        iwflag = 1;
                        iter = 11;
                        goto label350;
                    }
                    if (Math.Abs(delta[n]) > toler) iwflag = iwflag + 1;
                    if (iter > 3 && delwtr[n] * delta[n] < 0)
                    {
                        //              DELTA IS JUMPING BETWEEN NEG. AND POS.--CUT DELTA IN HALF
                        //              TO SUPPRESS TENDENCY TO JUMP AROUND SOLUTION
                        delta[n] = delta[n] / 2.0;
                    }
                    delwtr[n] = delta[n];
                    vicdt[i] = vicdt[i] - delta[n];
                    if (vicdt[i] < 0.0)
                    {
                        //              CHANGE IN ICE IS GREATER THAN ICE CONTENT -- ADJUST WATER
                        //              CONTENT FOR THE DIFFERENCE, IF NOT GREATER THAN
                        //              HALFWAY TO RESIDUAL WATER CONTENT
                        if ((vlcdt[i] + vicdt[i] * Constn.Rhoi / Constn.Rhol) > (vlcdt[i] + _slparm.Soilwrc[i][4]) / 2.0)
                        {
                            vlcdt[i] = vlcdt[i] + vicdt[i] * Constn.Rhoi / Constn.Rhol;
                        }
                        else
                        {
                            vlcdt[i] = (vlcdt[i] + _slparm.Soilwrc[i][4]) / 2.0;
                        }
                        //              IF WATER CONTENT AT RESIDUAL WATER CONTENT, DO NOT
                        //              REDEFINE MATRIC POTENTIAL
                        if (vlcdt[i] > _slparm.Soilwrc[i][4]) Matvl1(i, ref matdt[i], ref vlcdt[i], ref dummy);
                        vicdt[i] = 0.0;
                        icesdt[i] = 0;
                    }
                }
                else
                {
                    //
                    //           SAVE VALUE TO COMPARE RELATIVE CHANGE IN MATRIC POTENTIAL;
                    //           IF MATRIC POTENTIAL IS NEAR ZERO, SET CHECK VALUE TO 1.0
                    chkmat = matdt[i];
                    if (Math.Abs(chkmat) < 1.0) chkmat = 1.0;
                    //
                    if (Math.Abs(delta[n] / chkmat) > 100.0 && ndt < maxndt)
                    {
                        //              TOO LARGE OF A CHANGE -- CONVERGENCE WILL NOT LIKELY BE
                        //              BE MET WITH CURRENT TIME STEP.  CUT TIME STEP IN HALF
                        iwflag = 1;
                        iter = 11;
                        goto label350;
                    }
                    if (Math.Abs(delta[n] / chkmat) > toler) iwflag = iwflag + 1;
                    if (iter > 3 && delwtr[n] * delta[n] < 0)
                    {
                        //              DELTA IS JUMPING BETWEEN NEG. AND POS.--CUT DELTA IN HALF
                        //              TO SUPPRESS TENDENCY TO JUMP AROUND SOLUTION
                        delta[n] = delta[n] / 2.0;
                    }
                    delwtr[n] = delta[n];
                    matdt[i] = matdt[i] - delta[n];
                }
                dldm = 0.0;
                Matvl2(i, ref matdt[i], ref vlcdt[i], ref dldm);
                if (level[1] >= 2 && _lvlout[1] > 0)
                    generalOut.WriteLine($"{delta[n],18:G10} {matdt[i],18:G10} {vlcdt[i],18:G10} {vicdt[i],18:G10}");
                n = n + 1;
            label330:;
            }
            //     LIMIT WATER POTENTIAL AT SURFACE TO FREE WATER CONDITION
            //     TO ALLOW SEEPAGE TO OCCUR
            if (matdt[1] > 0.0)
            {
                //       CHANGE ANY SUBSEQUENT SATURATED NODES BY SAME AMOUNT
                for (var i = 2; i <= ns; ++i)
                {
                    if (matdt[i] > 0.0)
                    {
                        matdt[i] = matdt[i] - matdt[1];
                        Matvl2(i, ref matdt[i], ref vlcdt[i], ref dummy);
                    }
                    else
                    {
                        //           DONE WITH SATURATED NODES
                        goto label340;
                    }
                label335:;
                }
            label340:;
                matdt[1] = 0.0;
                Matvl2(1, ref matdt[1], ref vlcdt[1], ref dummy);
            }
            //
            //
            //-----------------------------------------------------------------------

            if (level[1] >= 1 && _lvlout[1] > 0)
            {
                generalOut.WriteLine($" WATER BALANCE AT ITERWB = {iter} {julian} {hour} {ntimes} {ndt}");
                Output(nplant, nc, ncmax, nsp, nr, ns, lvlout, 1, inph2o, julian, hour, year, 1, zc, tcdt, tlcdt, vapcdt, wcandt, rootxt, rhosp, zsp, tspdt, dlwdt, zr, trdt, vaprdt, gmcdt, zs, tsdt, vlcdt, vicdt, matdt, totflo, totlat, concdt, saltdt, ta, hum, vapa, wind, nclst, windsub, tempsub, tsurface, swdown, swup, lwdown, lwup, evap1, _goshawSave.Melt);
                //XOUT>    MATDT,TOTFLO,TOTLAT,CONCDT,SALTDT,TA,HUM,VAPA,WIND)
                generalOut.WriteLine($" ITERATION = {iter} ENERGY FLAGS = {ieflag} WATER FLAGS = {iwflag}");
            }
        //-----------------------------------------------------------------------

        label350:;
            if (iwflag > 0 || ieflag > 0)
            {
                //        CONVERGENCE HAS NOT BEEN MET - IF ITERATIONS ARE UNDER 10, GO
                //        BACK AND START NEXT ITERATION
                if (iter <= 10) goto label200;
                //
                //        HAVING PROBLEMS REACHING CONVERGENCE WITH CURRENT TIME STEP
                if (ndt < maxndt)
                {
                    //           CUT TIME STEP IN HALF AND TRY AGAIN
                    ndt = ndt * 2;
                    ntimes = ntimes * 2 - 1;
                    _timewt.Dt = dtime / ndt;
                    //           REDEFINE END-OF-TIME-STEP VALUES
                    Backup(ref ns, ref nr, ref nsp, ref nc, ref nplant, ref _slparm.Nsalt, ices, icesdt, ts, tsdt, mat, matdt, conc, concdt, vlc, vlcdt, vic, vicdt, salt, saltdt, tr, trdt, vapr, vaprdt, gmc, gmcdt, tc, tcdt, tlc, tlcdt, vapc, vapcdt, wcan, wcandt, pcan, pcandt, tsp, tspdt, dlw, dlwdt, icesp, icespt);
                    //           GO BACK AND CALCULATE BOUNDARY CONDITIONS FOR HALF TIME-STEP
                    goto label120;
                }
                //
                //        NDT > MAXNDT  --> INDICATE CONVERGENCE PROBLEMS AND CONTINUE
                if (lvlout[20] != 0) Console.WriteLine($"+ Converg. Prob. : {julian,4:D}   {hour,4:D}   {year,4:D}     {ntimes,4:D}      {ndt,4:D}");

                if (_lvlout[1] > 0)
                {
                    generalOut.WriteLine($" CONVERGENCE PROBLEMS AT : {julian} {hour} {year} {ntimes}");

                    if (ieflag > 0) generalOut.WriteLine(" ENERGY BALANCE WILL NOT CONVERGE");
                    if (iwflag > 0) generalOut.WriteLine(" WATER BALANCE WILL NOT CONVERGE");
                }
                iter = 0;

                if (level[1] == 1 || level[1] == 2)
                {
                    Console.WriteLine(" ***  Debugging limit reached ***");
                    return false;
                }
            }
            //
            //CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC

            //**** CALCULATE SOLUTE CONCENTRATIONS AT THE END OF THE TIME STEP
            if (_slparm.Nsalt <= 0) goto label400;
            Solute(ref ns, zs, conc, concdt, salt, saltdt, vlc, vlcdt, ts, tsdt, qsl, xtract, sink, dgrade, sltdif, asalt, disper);
            //
            //-----------------------------------------------------------------------
            if (level[1] >= 1 && _lvlout[1] > 0)
            {
                for (var i = 1; i <= ns; ++i)
                {
                    generalOut.WriteLine($"{hour,5:D}{zs[i],5:F2}{tsdt[i],8:F3}{vlcdt[i],6:F3}{vicdt[i],6:F3}{matdt[i],8:F2}{concdt[1][i],14:E4}{ta,8:F3}{tadt,8:F3}{saltdt[1][i],14:E4}");
                }
            }
            //-----------------------------------------------------------------------

            if (iter > 1)
            {
                //        IF IT TOOK MORE THAN ONE ITERATION FOR THE ENERGY AND WATER
                //        BALANCE, END-OF-TIME-STEP VALUES HAVE CHANGED ENOUGH THAT THE
                //        SOLUTE CONDITIONS ASSUMED WERE NOT ACCURATE.  GO BACK THROUGH
                //        ENERGY AND WATER BALANCE AGAIN
                itrslt = itrslt + 1;
                if (itrslt <= 4)
                {
                    iter = 0;
                    goto label200;
                }
                //
                //        CHANGES IN SOLUTE BALANCE ARE CAUSING SUCCESSIVE SOLUTIONS TO
                //        THE WATER-ENERGY BALANCE FOR THIS TIME STEP TO DIFFER TOO MUCH
                if (ndt < maxndt)
                {
                    //           CUT TIME STEP IN HALF AND TRY AGAIN
                    ndt = ndt * 2;
                    ntimes = ntimes * 2 - 1;
                    _timewt.Dt = dtime / ndt;
                    //           REDEFINE END-OF-TIME-STEP VALUES
                    Backup(ref ns, ref nr, ref nsp, ref nc, ref nplant, ref _slparm.Nsalt, ices, icesdt, ts, tsdt, mat, matdt, conc, concdt, vlc, vlcdt, vic, vicdt, salt, saltdt, tr, trdt, vapr, vaprdt, gmc, gmcdt, tc, tcdt, tlc, tlcdt, vapc, vapcdt, wcan, wcandt, pcan, pcandt, tsp, tspdt, dlw, dlwdt, icesp, icespt);
                    //           GO BACK AND CALCULATE BOUNDARY CONDITIONS FOR HALF TIME-STEP
                    goto label120;
                }
                //
                //        NDT > MAXNDT  ---> INDICATE CONVERGENCE PROBLEMS AND CONTINUE
                if (lvlout[20] != 0) Console.WriteLine($"+ Converg. Prob. : {julian,4:D}   {hour,4:D}   {year,4:D}     {ntimes,4:D}      {ndt,4:D}");

                if (_lvlout[1] > 0)
                {
                    generalOut.WriteLine($" CONVERGENCE PROBLEMS AT : {julian} {hour} {year} {ntimes}");
                    generalOut.WriteLine(" SOLUTES CAUSING CONVERGENCE PROBLEMS");
                }

                if (level[1] == 1 || level[1] == 2)
                {
                    Console.WriteLine(" ***  Debugging limit reached ***");
                    return false;
                }
            }
            itrslt = 0;
        label400:;

            // line 1277
            //**** END OF ITERATION FOR TIME STEP ***********************************
            //
            if (_goshawSave.Minstp == 0)
            {
                _goshawSave.Maxstp = ndt;
                _goshawSave.Minstp = ndt;
            }
            if (_goshawSave.Maxstp < ndt) _goshawSave.Maxstp = ndt;
            if (_goshawSave.Minstp > ndt) _goshawSave.Minstp = ndt;
            //
            //     SUM THE NECESSARY FLUXES OCCURRING OVER THE TIME STEP
            Sumdt(ref nc, ref nplant, ref nsp, ref nr, ref ns, ref hflux, ref vflux, ref gflux, lwcan, ref lwsnow, lwres, ref lwsoil, lwdown, lwup, swcan, swsnow, swres, ref swsoil, swdown, swup, qvc, qvr, qvsp, qsl, qsv, trnsp, xtract, flolat, ref seep, ref tswsno, ref tlwsno, ref tswcan, ref tlwcan, ref tswres, ref tlwres, ref tswsoi, ref tlwsoi, ref tswdwn, ref tlwdwn, ref tswup, ref tlwup, ref thflux, ref tgflux, ref evap1, ref etsum, ref tseep, rootxt, totflo, totlat, ref ntimes, ref topsno, tqvsp);
            //
            if (ntimes != ndt)
            {
                //        NOT REACHED END OF TIME STEP -- GO BACK THROUGH SUB-TIME-STEP
                if (ntimes % 2 == 0 && (ieflag + iwflag) == 0)
                {
                    //           CHECK IF IT'S WORTH IT TO TRY TO DOUBLE THE TIME STEP
                    if (ndt <= maxdbl)
                    {
                        if (ndt < maxdbl) maxtry = 0;
                        if (ndt <= 8) maxtry = maxtry + 2;
                        maxtry = maxtry + 1;
                        maxdbl = ndt;
                    }
                    if (ndt > maxdbl || maxtry <= 3 || ndt >= maxndt)
                    {
                        //              ATTEMPT TO INCREASE LENGTH OF SUB-TIME-STEP
                        ndt = ndt / 2;
                        ntimes = ntimes / 2;
                        _timewt.Dt = dtime / ndt;
                    }
                }
                ntimes = ntimes + 1;
                goto label110;
            }
            else
            {
                //        END OF TIME STEPS - INFILTRATE RAIN, DETERMINE OUTFLOW FROM
                //        SNOW,AND CALCULATE WATER BALANCE.  SET TIME STEP TO ONE HOUR
                //        FOR PRECIP CALULATIONS
                _timewt.Dt = dtime;
                rain = precip;
                tadt = tmpday;
                humdt = humday;
                ta = tmpday;
                hum = humday;
                nsplst = nsp;
                //xOUT    define surface temperature for output energy flux at snow,
                //        residue or soil surface before it is changed by PRECP
                if (nsp > 0)
                {
                    tsurface = tspdt[1];
                }
                else if (nr > 0)
                {
                    tsurface = trdt[1];
                }
                else
                {
                    tsurface = tsdt[1];
                }
                //xOUT

                Precp(ref nplant, ref nc, ref nsp, ref nr, ref ns, ref ta, ref tadt, ref hum, ref humdt, zc, pintrcp, xangle, clumpng, itype, wcandt, ref wcmax, pcandt, zsp, dzsp, tspdt, bdlw, dlwdt, rhosp, tqvsp, ref topsno, wlag, ref store, ref snowex, zr, trdt, gmcdt, ref gmcmax, rhor, zs, tsdt, vlcdt, vicdt, matdt, totflo, saltdt, concdt, icespt, icesdt, ref rain, ref pond, ref runoff, ref evap1, ref _goshawSave.Melt, ref pondmx, ref isnotmp, ref snotmp, ref snoden, ref dirres, ref slope);

                // JM: calculate snow heat capacity [J/m3/K] and thermal conductivity [W/m/K]
                // JM: also calculate snow density [kg/m3]
                var snowC = 0.0;
                var snowDensity = 0.0;
                var snowTk = 0.0;
                var snowThickness = 0.0;
                if (nsp > 0)
                {
                    var tk1 = new double[51];
                    Snowtk(ref nsp, tk1, rhosp);
                    tk1[nsp + 1] = tk1[nsp];

                    var nsp1 = nsp + 1;
                    var con = new double[101];
                    Conduc(nsp1, zsp, tk1, con);

                    var csp = new double[101];
                    Snowht(ref nsp, csp, tspdt, rhosp);

                    // as per Gerald's recommendations, calculate the heat capacity of the snow as csp weighted by the thickness of each snow layer, dzsp: 
                    //  sum(CSP(i) * DZSP(i)) / snowThickness.
                    //  calculate snowDensity as sum(RHOSP(i) * DZSP(i)) / snowThickness.
                    //  calculate TK as the total snowpack thickness divided by the harmonic mean of CON:  snowThickness / sum(1./CON(i)).
                    //  Gerald indicates that the snowThickness is zsp(nsp + 1).

                    snowThickness = zsp[nsp + 1];
                    var cSum = 0.0;
                    var densitySum = 0.0;
                    var conHarmonicMean = 0.0;

                    for (var i = 1; i <= nsp; ++i)
                    {
                        cSum += csp[i] * dzsp[i];
                        densitySum += rhosp[i] * dzsp[i];
                        conHarmonicMean += 1.0 / con[i];
                    }

                    snowC = cSum / snowThickness;
                    snowDensity = densitySum / snowThickness;
                    snowTk = snowThickness / conHarmonicMean;
                }

                results.DailySnowHeatCapacity.Add(snowC);
                results.DailySnowThermalConductivity.Add(snowTk);
                results.DailySnowDensity.Add(snowDensity);
                results.DailySnowThickness.Add(snowThickness);

                //
                //        ADD SEEPAGE TO RUNOFF
                runoff = runoff + tseep;
                //
                //        ADJUST DEPTHS OF CANOPY LAYERS FOR ANY CHANGE IN SNOWPACK
                //xOUT
                nclst = nc;
                if (nclst == 0)
                {
                    //          save condition above substrate for output of energy flues at
                    //          snow, residue or snow surface
                    windsub = winday;
                    tempsub = tmpday;
                }
                else
                {
                    windsub = _windv.Windc[nclst];
                    tempsub = tcdt[nclst];
                }
                //xOUT
                if (nsp > 0 || nsplst > 0)
                {
                    if (nplant != 0) Canopy(ref nplant, ref nc, ref ncmax, ref nsp, nsplst, ref mzcinp, itype, htordr, 1, plthgt, pltwgt, pltlai, rleaf0, zsp, zc, tc, tcdt, tlc, tlcdt, bvapc, vapcdt, bwcan, wcandt, ref wcmax, pcandt, ref canma, ref canmb, ref tmpday, ref humday);
                }
                //
                //        PRINT OUT SNOW TEMPERATURE PROFILE
                if (lvlout[9] != 0) Snowtemp(ref nsp, ref lvlout[9], ref julian, ref hour, ref year, 1, zsp, tspdt);
                //
                //        PRINT OUT WATER BALANCE FOR THIS HOUR

                // JM: call Wbalnc regardless of lvlout[11] to calculate values, even if they don't get logged
                Wbalnc(ref nplant, ref nc, ref nsp, ref nr, ref ns, ref lvlout[11], ref julian, ref hour, ref year, itype, 1, zc, bwcan, wcandt, bpcan, pcandt, bvapc, vapcdt, rhosp, dzsp, dlwdt, wlag, ref store, zr, bgmc, gmcdt, bvapr, vaprdt, rhor, zs, bvlc, vlcdt, bvic, vicdt, totflo, ref precip, ref runoff, ref pond, ref evap1, ref _goshawSave.Melt, ref etsum, results);
                //
                //        PRINT OUT ENERGY BALANCE FOR THIS HOUR
                if (lvlout[10] != 0) Energy(ref nsplst, ref lvlout[10], ref julian, ref hour, ref year, _outputInital, ref dtime, ref tswsno, ref tlwsno, ref tswcan, ref tlwcan, ref tswres, ref tlwres, ref tswsoi, ref tlwsoi, ref tswdwn, ref tlwdwn, ref tswup, ref tlwup, ref thflux, ref tgflux, ref evap1);
                //
                //        PRINT OUT FROST AND SNOW DEPTH
                if (lvlout[15] != 0) Frost(ref nsp, ref ns, ref lvlout[15], ref julian, ref hour, ref year, 1, zsp, rhosp, dzsp, dlwdt, wlag, ref store, zs, vlcdt, vicdt, icesdt);
                //
                //        PRINT OUTPUT FOR THIS HOUR
                Output(nplant, nc, ncmax, nsp, nr, ns, lvlout, 0, inph2o, julian, hour, year, 1, zc, tcdt, tlcdt, vapcdt, wcandt, rootxt, rhosp, zsp, tspdt, dlwdt, zr, trdt, vaprdt, gmcdt, zs, tsdt, vlcdt, vicdt, matdt, totflo, totlat, concdt, saltdt, ta, hum, vapa, wind, nclst, windsub, tempsub, tsurface, swdown, swup, lwdown, lwup, evap1, _goshawSave.Melt);
                //XOUT>    MATDT,TOTFLO,TOTLAT,CONCDT,SALTDT,TA,HUM,VAPA,WIND)
                //
                //        PRINT OUTPUT TO SCREEN
                if (lvlout[20] != 0)
                {
                    _goshawSave.Nprint = _goshawSave.Nprint + 1;
                    if (_goshawSave.Nprint % lvlout[20] == 0)
                    {
                        if (_lvlout[2] > 0)
                            generalOut.WriteLine($"+ Completed :      {julian,4:D}   {hour,4:D}   {year,4:D}     {_goshawSave.Minstp,4:D}      {_goshawSave.Maxstp,4:D}");
                        Console.WriteLine($"+ Completed :      {julian,4:D}   {hour,4:D}   {year,4:D}     {_goshawSave.Minstp,4:D}      {_goshawSave.Maxstp,4:D}");
                        _goshawSave.Nprint = 0;
                        _goshawSave.Maxstp = 0;
                        _goshawSave.Minstp = 0;
                    }
                }
            }

            return true;
        }

        private void Cleanup()
        {
            // flush any output streams
            if (_outputWriters != null)
                foreach (var w in _outputWriters.Values)
                    w?.Flush();

            //// close any input and output streams
            //if (_inputReaders != null)
            //    foreach (var r in _inputReaders.Values)
            //        r?.Close();

            //if (_outputWriters != null)
            //    foreach (var w in _outputWriters.Values)
            //        w?.Close();

            //if (_canopyReaders != null)
            //    foreach (var r in _canopyReaders)
            //        r?.Close();

            //_residueReader?.Close();
        }

        private void Abort()
        {
            Cleanup();

            Console.WriteLine();
            Console.WriteLine(" ===> Simulation aborted; Press Enter to end");
            Console.ReadLine();
        }

        // line 3590
        private void Cloudy(ref double clouds, ref double alatud, ref double declin, ref double hafday, double[] sunhor, ref int julian, ref int nhrpdt)
        {
            //
            //     THIS SUBROUTINE TOTALS THE SOLAR RADIATION FOR THE DAY.  IT THEN
            //     ESTIMATES THE CLOUD COVER, WHICH WILL BE USED IN THE CALCULATION
            //     FOR THE EMMISSIVITY OF THE ATMOSPHERE.  (DECLINATION AND HALF-DAY
            //     LENGTH FOR THE CURRENT JULIAN DAY ARE ALSO CALCULATED.)
            //
            //
            var totsun = 0.0;
            for (var i = nhrpdt; i <= 24; ++i)
            {
                if (sunhor[i] < 0.0) sunhor[i] = 0.0;
                totsun = totsun + sunhor[i];
            label10:;
            }

            declin = 0.4102 * Math.Sin(2 * 3.14159 * (julian - 80) / 365.0);
            var coshaf = -Math.Tan(alatud) * Math.Tan(declin);
            if (Math.Abs(coshaf) >= 1.0)
            {
                if (coshaf >= 1.0)
                {
                    //            SUN DOES NOT COME UP ON THIS DAY (WINTER IN ARCTIC CIRCLE)
                    hafday = 0.0;
                }
                else
                {
                    //            SUN DOES NOT SET ON THIS DAY (SUMMER IN THE ARCTIC CIRCLE)
                    hafday = 3.14159;
                }
            }
            else
            {
                hafday = Math.Acos(coshaf);
            }

            var sunmax = 24.0 * Swrcoe.Solcon * (hafday * Math.Sin(alatud) * Math.Sin(declin) + Math.Cos(alatud) * Math.Cos(declin) * Math.Sin(hafday)) / 3.14159;
            if (sunmax > 0.0)
            {
                var ttotal = totsun / sunmax;
                //         USE CLOUD EQUATION BASED FLERCHINGER & YU (2007), AG & FOREST MET
                clouds = 1.333 - 1.666 * ttotal;
                if (clouds > 1.0) clouds = 1.0;
                if (clouds < 0.0) clouds = 0.0;
            }
            else
            {
                //         MAXIMUM SOLAR IS ZERO; ATMOSPHERE TRANSMISSIVITY IS UNDEFINED
                //         SET CLOUD COVER TO 0.5 FOR LACK OF ANYTHING BETTER
                clouds = 0.5;
            }
        }

        // line 3640
        private void Canopy(ref int nplant, ref int nc, ref int ncmax, ref int nsp, int nsplst, ref int mzcinp, int[] itype, int[] htordr,
            int inital, double[] plthgt, double[] pltwgt, double[] pltlai, double[] rleaf0, double[] zsp, double[] zc, double[] tc, double[] tcdt,
            double[][] tlc, double[][] tlcdt, double[] vapc, double[] vapcdt, double[] wcan, double[] wcandt, ref double wcmax, double[] pcandt,
            ref double canma, ref double canmb, ref double ta, ref double humid)
        {
            //
            //     THIS SUBROUTINE DETERMINES WHICH LAYERS OF THE CANOPY ARE COVERED
            //     WITH SNOW
            //
            //***********************************************************************

            //     SAVE CANOPY PROPERTIES IF CONSTANT (ABS(MZCINP)>0)
            var plantw = new double[9];
            var tmp = new double[9];

            var dummy = 0.0;
            var satv = 0.0;
            var mzc = 0;
            var snodepth = 0.0;
            var ncchk = 0;
            var plthmax = 0.0;
            var zmid1 = 0.0;
            var zmid2 = 0.0;
            var nc1 = 0;
            var nclast = 0;
            var factor = 0.0;
            var dz = 0.0;

            if (inital == 0)
            {
                if (mzcinp <= 0) nc = 1;
                nclast = 1;
                //        INITIALIZE TEMPERATURE AND VAPOR DENSITY OF TOP LAYER
                //        (REMAINDER WILL BE SET SUBSEQUENTLY)
                Vslope(ref dummy, ref satv, ref ta);
                tcdt[1] = ta;
                vapcdt[1] = humid * satv;
                for (var j = 1; j <= nplant; ++j)
                {
                    tlcdt[j][1] = tcdt[1];
                    pcandt[j] = 0.0;
                }
                if (wcandt[1] <= 0.0)
                {
                    //           SET WATER CONTENT TO EQUILIBRIUM WITH HUMIDITY
                    Canhum(2, ref humid, ref dummy, ref wcandt[1], ref tcdt[1], ref canma, ref canmb);
                }
                if (Math.Abs(mzcinp) > 0)
                {
                    //           LAYERING OF CANOPY NODES SPECIFIED BY USER -- SAVE SPACING
                    mzc = Math.Abs(mzcinp);
                    for (var i = 1; i <= mzc; ++i)
                    {
                        _canopySave.Zzc[i] = zc[i];
                        if (i == 1)
                        {
                            _canopySave.Dzcan[i] = zc[2] - zc[1];
                            if (nc > 1) _canopySave.Dzcan[i] = _canopySave.Dzcan[i] / 2.0;
                        }
                        else if (i == nc)
                        {
                            _canopySave.Dzcan[i] = zc[i + 1] - zc[i] + (zc[i] - zc[i - 1]) / 2.0;
                        }
                        else
                        {
                            _canopySave.Dzcan[i] = (zc[i + 1] - zc[i - 1]) / 2.0;
                        }
                        if (mzcinp > 0)
                        {
                            for (var j = 1; j <= nplant; ++j)
                            {
                                _canopySave.Ccanlai[j][i] = _clayrs.Canlai[j][i];
                                _canopySave.Ddrycan[j][i] = _clayrs.Drycan[j][i];
                            }
                        }
                    }
                    _canopySave.Zzc[mzc + 1] = zc[mzc + 1];
                }
            }
            else
            {
                nclast = nc;
            }

            //
            //     CHECK IF CANOPY HAS EMERGED AND NOT COVERED WITH SNOW
            snodepth = 0.0;
            if (nsp > 0) snodepth = zsp[nsp + 1];
            ncchk = 0;
            plthmax = 0.0;
            for (var j = 1; j <= nplant; ++j)
            {
                //       INITIALIZE TOTAL LAI FOR NO SNOW; WILL LATER ADJUST FOR SNOW
                _clayrs.Totlai[j] = pltlai[j];
                if (pltlai[j] > 0.0)
                {
                    //         FIND MAXIMUM PLANT HEIGHT
                    if (plthgt[j] > plthmax)
                    {
                        plthmax = plthgt[j];
                        if (nsp > 0)
                        {
                            //             DETERMINE IF PLANT IS COVERED WITH SNOW
                            if (plthgt[j] > snodepth) ncchk = 1;
                        }
                        else
                        {
                            ncchk = 1;
                        }
                    }
                }
            }
            if (ncchk == 0)
            {
                //        CANOPY HAS NOT EMERGED OR IS COVERED WITH SNOW - EXIT OUT
                nc = 0;
                goto label80;
            }
            //
            //     CANOPY IS PRESENT
            if (nclast == 0)
            {
                //        NO CANOPY LAST TIME STEP
                //        DEFINE STATE VARIABLES FOR TOP OF CANOPY
                nc = 1;
                tc[1] = ta;
                tcdt[1] = ta;
                for (var j = 1; j <= nplant; ++j)
                {
                    tlcdt[j][1] = tcdt[1];
                }
                Vslope(ref dummy, ref satv, ref ta);
                vapc[1] = humid * satv;
                vapcdt[1] = humid * satv;
                if (nsplst > 0)
                {
                    //           SNOW MELTED AND EXPOSED TOP OF CANOPY--SET MAX WATER CONTENT
                    wcan[1] = wcmax;
                    wcandt[1] = wcmax;
                }
                else
                {
                    //           SET WATER CONTENT TO EQUILIBRIUM WITH HUMIDITY
                    Canhum(2, ref humid, ref dummy, ref wcandt[1], ref tcdt[1], ref canma, ref canmb);
                    wcan[1] = wcandt[1];
                }
            }
            //
            //     Initialize temporary variable used for sorting,
            for (var j = 1; j <= nplant; ++j)
            {
                tmp[j] = plthgt[j];
                if (plthgt[j] * pltlai[j] <= 0.0) tmp[j] = 0.0;
            }
            //**  Arrange the plants by height - in increasing order
            for (var htindx = 1; htindx <= nplant; ++htindx)
            {
                htordr[htindx] = 1;
                for (var j = 1; j <= nplant; ++j)
                {
                    if (tmp[htordr[htindx]] > tmp[j]) htordr[htindx] = j;
                }
                tmp[htordr[htindx]] = 9999.0;
            }
            //
            if (mzcinp <= 0)
            {
                //       ALLOW MODEL TO DEFINE LAYERING AND NODES WITHIN CANOPY
                for (var j = 1; j <= nplant; ++j)
                {
                    if (nsp > 0)
                    {
                        _clayrs.Plantz[j] = plthgt[j] - snodepth;
                        if (_clayrs.Plantz[j] <= 0.0)
                        {
                            _clayrs.Plantz[j] = 0.0;
                            plantw[j] = 0.0;
                            _clayrs.Totlai[j] = 0.0;
                        }
                        else
                        {
                            plantw[j] = pltwgt[j] * _clayrs.Plantz[j] / plthgt[j];
                            _clayrs.Totlai[j] = pltlai[j] * _clayrs.Plantz[j] / plthgt[j];
                        }
                    }
                    else
                    {
                        //           NO SNOW ON GROUND - SET PLANT DIMENSIONS FOR TIME STEP
                        _clayrs.Plantz[j] = plthgt[j];
                        plantw[j] = pltwgt[j];
                        _clayrs.Totlai[j] = pltlai[j];
                    }
                label10:;
                }
                //
                if (mzcinp == 0)
                {
                    //         DETERMINE LAYERING OF CANOPY ACCOUNTING FOR SNOW
                    Canlay(ref nc, ref ncmax, ref nplant, itype, htordr, zc, wcandt, tcdt, tlcdt, vapcdt, _clayrs.Plantz, plthgt, plantw, _clayrs.Totlai, pltlai, rleaf0, _clayrs.Drycan, _clayrs.Canlai, _clayrs.Rleaf);
                }
                else
                {
                    //         SET NUMBER NODES FOR USER-SPECIFIED NODE SPACING IN CANOPY
                    //         FIND HOW MANY CANOPY LAYERS ARE PRESENT AND HOW MANY ARE
                    //         COVERED WITH SNOW
                    Canlayzc(ref nc, ref ncmax, ref nplant, ref mzcinp, inital, itype, zc, _canopySave.Zzc, wcandt, tcdt, tlcdt, vapcdt, _clayrs.Plantz, plantw, _clayrs.Totlai, rleaf0, _clayrs.Drycan, _clayrs.Canlai, _clayrs.Rleaf, ref plthmax, ref snodepth);
                }
            }
            else
            {
                //       USER-DEFINED OF ALL PARAMETERS WITHIN CANOPY LAYERS
                //       SET MAX NUMBER OF NODES; NCMAX ALWAYS EQUALS MZCINP IF MZCINP>0
                ncmax = mzcinp;
                if (nsp > 0)
                {
                    //         CANOPY LAYERING WAS INPUT - DO NOT ALLOW MOVING OF NODES
                    //         DETERMINE WHICH CANOPY NODES ARE ABOVE SNOW
                    for (var i = mzcinp; i >= 1; --i)
                    {
                        if ((_canopySave.Zzc[mzcinp + 1] - _canopySave.Zzc[i]) > snodepth)
                        {
                            //              CANOPY LAYER IS ABOVE SNOWPACK
                            if ((_canopySave.Zzc[mzcinp + 1] - _canopySave.Zzc[i + 1]) <= snodepth)
                            {
                                //                SNOW WITHIN CURRENT CANOPY LAYER
                                nc = i;
                                zc[nc + 1] = _canopySave.Zzc[mzcinp + 1] - snodepth;
                                if (nc == 1)
                                {
                                    //                   ONLY ONE CANOPY LAYER
                                    zc[nc] = 0.0;
                                    dz = zc[nc + 1];
                                }
                                else
                                {
                                    //                  DETERMINE LOCATION OF BOTTOM CANOPY NODE;
                                    //                  FIND MID-POINTS TO NODE ABOVE AND BELOW CURRENT NODE
                                    zmid1 = (_canopySave.Zzc[nc] + _canopySave.Zzc[nc - 1]) / 2.0;
                                    zmid2 = (_canopySave.Zzc[nc + 1] + _canopySave.Zzc[nc]) / 2.0;
                                    if (zc[nc + 1] > zmid2)
                                    {
                                        zc[nc] = _canopySave.Zzc[nc];
                                    }
                                    else
                                    {
                                        zc[nc] = _canopySave.Zzc[nc] - (_canopySave.Zzc[nc] - zmid1) * (zmid2 - zc[nc + 1]) / (zmid2 - _canopySave.Zzc[nc]);
                                    }
                                    dz = zc[nc + 1] - _canopySave.Zzc[nc] + (_canopySave.Zzc[nc] - _canopySave.Zzc[nc - 1]) / 2.0;
                                }
                                //                SET PARAMETERS FOR BOTTOM CANOPY LAYER
                                factor = dz / _canopySave.Dzcan[nc];
                                if (factor > 1.0) factor = 1.0;
                            }
                            else
                            {
                                //                CANOPY LAYER IS ABOVE SNOW; SET PARAMETERS
                                factor = 1.0;
                                zc[i] = _canopySave.Zzc[i];
                            }
                            for (var j = 1; j <= nplant; ++j)
                            {
                                _clayrs.Canlai[j][nc] = _canopySave.Ccanlai[j][nc] * factor;
                                _clayrs.Drycan[j][nc] = _canopySave.Ddrycan[j][nc] * factor;
                            }
                        }
                    label30:;
                    }
                    //         RECOMPUTE OVERALL PLANT CHARACTERISTICS
                    for (var j = 1; j <= nplant; ++j)
                    {
                        _clayrs.Plantz[j] = plthgt[j] - snodepth;
                        if (_clayrs.Plantz[j] < 0.0) _clayrs.Plantz[j] = 0.0;
                        _clayrs.Totlai[j] = 0.0;
                        plantw[j] = 0.0;
                        for (var i = 1; i <= nc; ++i)
                        {
                            _clayrs.Totlai[j] = _clayrs.Totlai[j] + _clayrs.Canlai[j][i];
                            plantw[j] = plantw[j] + _clayrs.Drycan[j][i];
                        }
                    }
                }
                else
                {
                    //         NO SNOW ON GROUND
                    //ccc      IF (NSPLST .GT. 0) THEN >> does not work if INITAL = 0
                    //         RESET NUMBER OF NODES
                    nc = mzcinp;
                    //         RESET CONSTANT PLANT CHARACTERISTICS
                    for (var j = 1; j <= nplant; ++j)
                    {
                        _clayrs.Plantz[j] = plthgt[j];
                        plantw[j] = pltwgt[j];
                        _clayrs.Totlai[j] = pltlai[j];
                        for (var i = 1; i <= nc; ++i)
                        {
                            _clayrs.Canlai[j][nc] = _canopySave.Ccanlai[j][nc];
                            _clayrs.Drycan[j][nc] = _canopySave.Ddrycan[j][nc];
                        }
                    }
                    for (var i = 1; i <= nc + 1; ++i)
                    {
                        zc[i] = _canopySave.Zzc[i];
                    }
                    //ccc      END IF
                }
                //
                if (nc > nclast)
                {
                    //         SNOW HAS MELTED AND EXPOSED ADDITIONAL CANOPY LAYERS --
                    //         DEFINE STATE VARIABLES OF NEWLY EXPOSED LAYERS
                    nc1 = nclast;
                    if (nclast == 0) nc1 = 1;
                    for (var i = nc1 + 1; i <= nc; ++i)
                    {
                        tcdt[i] = tcdt[i - 1];
                        vapcdt[i] = vapcdt[i - 1];
                        for (var j = 1; j <= nplant; ++j)
                        {
                            tlcdt[j][i] = tlcdt[j][i - 1];
                        }
                    }
                }
                //
            }
            //
            if (nc > nclast)
            {
                //        SET WATER CONTENT, TEMPERATURE AND VAPOR OF NEW LAYERS
                //        FOR BEGINNING OF TIME STEP
                nc1 = nclast;
                if (nclast == 0) nc1 = 1;
                for (var i = nc1 + 1; i <= nc; ++i)
                {
                    if (nsplst > 0)
                    {
                        //              SNOW HAS MELTED EXPOSING LAYERS - WATER CONTENT = MAXIMUM
                        wcandt[i] = wcmax;
                    }
                    else
                    {
                        wcandt[i] = wcandt[i - 1];
                    }
                    wcan[i] = wcandt[i];
                    tc[i] = tcdt[i];
                    vapc[i] = vapcdt[i];
                    for (var j = 1; j <= nplant; ++j)
                    {
                        tlc[j][i] = tlcdt[j][i];
                    }
                label60:;
                }
            }
        //
        label80:;
            for (var j = 1; j <= nplant; ++j)
            {
                if (pltlai[j] <= 0.0 || _clayrs.Totlai[j] <= 0.0) pcandt[j] = 0.0;
            }
        }

        // line 3926
        private void Canlay(ref int nc, ref int ncmax, ref int nplant, int[] itype, int[] htordr, double[] zc, double[] wcandt,
            double[] tcdt, double[][] tlcdt, double[] vapcdt, double[] plantz, double[] plantw, double[] plthgt, double[] totlai,
            double[] pltlai, double[] rleaf0, double[][] drycan, double[][] canlai, double[][] rleaf)
        {
            //
            //      This subroutine splits the canopy into layers.  Each layer
            //      will have the same total leaf area index, but the actual layer
            //      dimension will vary.  The different plant variables are
            //      dimensioned by plant type and layer, and their values are
            //      apportioned according to their representation in the layer.
            //
            //***********************************************************************
            //

            //     -- Local variables:
            //     A         Leaf area index per layer.
            //     DZMAX     Maximum theoretical layer thickness for current values.
            //     HTINDX    Index for ordered plant height - in increasing order.
            //     LAILYR    Current accumulated LAI for current layer.
            //     LAISUM    The delta LAI/unit distance for current calculations.
            //     LAYER     Current layer number, used for loop control.
            //     LYRTOP    Height of top of current layer.
            //     LYRTOT    Current accumulated layer thickness for current layer.
            //     TMPLAI    Sum of total leaf area index for all plants.

            var laisum = 0.0;
            var lailyr = 0.0;
            var lyrtop = 0.0;
            var lyrtot = 0.0;
            var dzmax = 0.0;
            var a = 0.0;
            var dz = new double[11];
            var factor = 0.0;
            var tmplai = 0.0;
            var htindx = 0;
            var layer = 0;
            var oldnc = nc;

            var tmxlai = 0.0;
            var nzeros = 0;

            //     -- Initialize sum of total leaf area index for all plants,
            //        and sum of delta LAI per unit distance.
            for (var j = 1; j <= nplant; ++j)
            {
                if (plthgt[j] * pltlai[j] != 0.0)
                {
                    tmplai = tmplai + totlai[j];
                    //           SUM MAXIMUM LAI (LAI IF THERE IS NO SNOW)
                    tmxlai = tmxlai + pltlai[j];
                    laisum = laisum + totlai[j] / plantz[j];
                }
                else
                {
                    nzeros = nzeros + 1;
                    totlai[j] = 0.0;
                }
            label10:;
            }
            nc = (int)(tmplai / 0.50 + 0.5);   // Fortran uses 'nint' here, which rounds to the nearest integer.  Note: similar Fortran code in Residue truncates used 'int'.
            //     -- Set upper limit for number of canopy layers.
            if (nc == 0)
            {
                nc = 1;
            }
            else
            {
                if (nc > 10) nc = 10;
            }
            //     Set maximum number of nodes (nodes without snow cover)
            ncmax = (int)(tmxlai / 0.50 + 0.5);   // Fortran uses 'nint' here, which rounds to the nearest integer.  Note: similar Fortran code in Residue truncates used 'int'.
            if (ncmax == 0)
            {
                ncmax = 1;
            }
            else
            {
                if (ncmax > 10) ncmax = 10;
            }
            //
            htindx = nzeros + 1;
            //
            a = tmplai / nc;
            layer = nc;
            zc[nc + 1] = plantz[htordr[nplant]];
        //**   --Main loop   ***g
        //     --Calculate theoretical maximum layer based on current LAISUM.
        label100:;
            if (layer != 1)
            {
                dzmax = (a - lailyr) / laisum;
            }
            else
            {
                dzmax = plantz[htordr[nplant]] - lyrtop;
            }
            //
            if (lyrtop + dzmax <= plantz[htordr[htindx]] || layer == 1)
            {
                //**   -- Top of layer is below or equal to next tallest plant.
                lyrtop = lyrtop + dzmax;
                dz[layer] = lyrtot + dzmax;
                //**      --Apportioning routine start.
                for (var j = 1; j <= nplant; ++j)
                {
                    if (plantz[j] >= lyrtop)
                    {
                        //               -- Plant fully contained in layer, full apportioning.
                        factor = dz[layer] / plantz[j];
                    }
                    else if (plantz[j] > lyrtop - dz[layer])
                    {
                        //          -- Plant partially contained in layer, partial apportioning.
                        factor = (plantz[j] - (lyrtop - dz[layer])) / plantz[j];
                    }
                    else
                    {
                        //          -- Plant is not contained in layer, set values to zero.
                        factor = 0.0;
                    }
                    canlai[j][layer] = totlai[j] * factor;
                    drycan[j][layer] = plantw[j] * factor;
                    if (itype[j] != 0)
                    {
                        //          -- If plant is not dead.
                        if (factor * totlai[j] != 0)
                        {
                            //                -- Plant is within layer.
                            rleaf[j][layer] = rleaf0[j] * (canlai[j][layer] / totlai[j]);
                        }
                        else
                        {
                            //                -- Plant is outside of layer.
                            rleaf[j][layer] = 0.0;
                        }
                    }
                    else
                    {
                        //             -- Plant is dead.
                        rleaf[j][layer] = 0.0;
                    }
                //**        -- End apportioning routine.
                label110:;
                }
                //          --Update variables if more layers were added.
                if (oldnc < layer)
                {
                    tcdt[layer] = tcdt[oldnc];
                    vapcdt[layer] = vapcdt[oldnc];
                    wcandt[layer] = wcandt[oldnc];
                    for (var j = 1; j <= nplant; ++j)
                    {
                        tlcdt[j][layer] = tlcdt[j][oldnc];
                    label120:;
                    }
                }
                //          --Calc midpoint of the layer
                if (layer == 1)
                {
                    zc[1] = 0.0;
                }
                else
                {
                    zc[layer] = plantz[htordr[nplant]] - (lyrtop - dz[layer] / 2.0);
                }
                //          Check if done.  If so, drop out of loop
                if (layer == 1)
                {
                    if (Math.Abs(plantz[htordr[nplant]] - lyrtop) > 0.0001)
                    {
                        Console.WriteLine("ERROR MATCHING TOP OF CANOPY");
                        Console.WriteLine($"  LYRTOP =  {lyrtop}");
                    }
                    goto label190;
                }
                //**        -- If top of layer equaled top of the next tallest plant
                if (lyrtop >= plantz[htordr[htindx]] - .0001)
                {
                    laisum = laisum - totlai[htordr[htindx]] / plantz[htordr[htindx]];
                    htindx = htindx + 1;
                }
                lyrtot = 0.0;
                lailyr = 0.0;
                layer = layer - 1;
            }
            else
            {
                //**   -- Top of layer extends beyond top of plant being considered.
                //          -- Update calcs based on limitation of top of next plant.
                lyrtot = lyrtot + plantz[htordr[htindx]] - lyrtop;
                lailyr = lailyr + (a - lailyr) * ((plantz[htordr[htindx]] - lyrtop) / dzmax);
                laisum = laisum - totlai[htordr[htindx]] / plantz[htordr[htindx]];
                lyrtop = plantz[htordr[htindx]];
                htindx = htindx + 1;
            }
            goto label100;
        //     -- End of loop
        label190:;
        }

        // line 4105
        private void Canlayzc(ref int nc, ref int ncmax, ref int nplant, ref int mzcinp, int inital, int[] itype, double[] zc,
            double[] zzc, double[] wcandt, double[] tcdt, double[][] tlcdt, double[] vapcdt, double[] plantz, double[] plantw, double[] totlai,
            double[] rleaf0, double[][] drycan, double[][] canlai, double[][] rleaf, ref double plthmax, ref double snodepth)
        {
            //
            //     This subroutine places canopy nodes at the heights specified
            //     by the user, and decides how many nodes are present and now many
            //     are covered with snow.  It then apportions the LAI according to
            //     the thickness of each canopy layer.
            //
            //***********************************************************************
            //
            var lyrtop = 0.0;
            var lyrbtm = 0.0;
            var oldnc = 0;
            var ncsnow = 0;
            var nmiss = 0;
            var delta = 0.0;
            var pltzmax = 0.0;
            var delsno = 0.0;
            var dz = 0.0;
            var factor = 0.0;

            if (inital == 0)
            {
                _canlayzcSave.Mzc = Math.Abs(mzcinp);
                //        SET UP HEIGHT OF THE BOTTOM OF EACH LAYER
                for (var i = 1; i <= _canlayzcSave.Mzc; ++i)
                {
                    _canlayzcSave.Zmid[i] = zzc[_canlayzcSave.Mzc + 1] - (zzc[i + 1] + zzc[i]) / 2.0;
                }
                _canlayzcSave.Zmid[_canlayzcSave.Mzc] = 0.0;
            }
            //
            oldnc = nc;
            nc = _canlayzcSave.Mzc;
            ncsnow = _canlayzcSave.Mzc - 1;
            for (var i = 1; i <= _canlayzcSave.Mzc; ++i)
            {
                if (snodepth < zzc[_canlayzcSave.Mzc + 1] - zzc[i]) ncsnow = _canlayzcSave.Mzc - i;
                if (plthmax < _canlayzcSave.Zmid[i]) nc = _canlayzcSave.Mzc - i;
            }
            //     NMISS IS NUMBER OF NODES ABOVE PLANT HEIGHT THAT ARE NOT USED
            nmiss = _canlayzcSave.Mzc - nc;
            ncmax = nc;
            nc = nc - ncsnow;
            //
            //     COMPUTE OFFSET OF MAXIMUM PLANT HEIGHT TO ITS NODE
            delta = plthmax - (zzc[_canlayzcSave.Mzc + 1] - zzc[nmiss + 1]);
            //     COMPUTE HEIGHT OF TALLEST PLANT ABOVE SNOW
            pltzmax = plthmax - snodepth;
            //
            //     SET UP NODES AND APPORTION PLANT PROPERTIES TO LAYERS
            lyrtop = pltzmax;
            zc[1] = 0.0;
            for (var i = 1; i <= nc; ++i)
            {
                zc[i + 1] = zzc[nmiss + i + 1] - zzc[nmiss + 1] + delta;
                if (i == nc)
                {
                    if (nc == 1)
                    {
                        dz = pltzmax;
                    }
                    else
                    {
                        dz = zc[nc + 1] - zc[nc] + (zc[nc] - zc[nc - 1]) / 2.0;
                        if (dz > lyrtop)
                        {
                            //             ADJUST NODE NC PROPORTIONALLY UPWARD FOR SNOW DEPTH;
                            //             AS SNOW DEPTH APPROACH NC, NODE NC WILL BE
                            //             MOVED TO APPROACH MID-POINT OF NC-1 AND NC
                            dz = lyrtop;
                            if (snodepth > _canlayzcSave.Zmid[nc])
                            {
                                //               DO NOT ADJUST UPWARD UNTIL SNOW IS ABOVE MIDPOINT
                                //               BETWEEN CURRENT NODE AND THE NEXT LOWEST NODE.
                                delsno = snodepth - _canlayzcSave.Zmid[nc];
                                zc[nc] = zc[nc] - (delsno / (zc[nc + 1] - zc[nc])) * (zc[nc] - (zc[nc] + zc[nc - 1]) / 2.0);
                            }
                        }
                    }
                    //         FORCE NC+1 TO MAX PLANT HEIGHT ADJUSTED FOR SNOW
                    zc[nc + 1] = pltzmax;
                }
                else if (i == 1)
                {
                    dz = (zc[i + 1] - zc[i]) / 2.0;
                }
                else
                {
                    dz = (zc[i + 1] - zc[i - 1]) / 2.0;
                }
                lyrbtm = lyrtop - dz;
                if (lyrbtm < 0.0) lyrbtm = 0.0;
                //**     --Apportioning routine start.
                for (var j = 1; j <= nplant; ++j)
                {
                    if (plantz[j] >= lyrtop)
                    {
                        //              -- Plant fully contained in layer, full apportioning.
                        factor = dz / plantz[j];
                    }
                    else if (plantz[j] > lyrbtm)
                    {
                        //         -- Plant partially contained in layer, partial apportioning.
                        factor = (plantz[j] - lyrbtm) / plantz[j];
                    }
                    else
                    {
                        //         -- Plant is not contained in layer, set values to zero.
                        factor = 0.0;
                    }
                    canlai[j][i] = totlai[j] * factor;
                    drycan[j][i] = plantw[j] * factor;
                    if (itype[j] != 0)
                    {
                        //         -- If plant is not dead.
                        if (factor * totlai[j] != 0)
                        {
                            //               -- Plant is within layer.
                            rleaf[j][i] = rleaf0[j] * (canlai[j][i] / totlai[j]);
                        }
                        else
                        {
                            //               -- Plant is outside of layer.
                            rleaf[j][i] = 0.0;
                        }
                    }
                    else
                    {
                        //            -- Plant is dead.
                        rleaf[j][i] = 0.0;
                    }
                //**       -- End apportioning routine.
                label24:;
                }
                lyrtop = lyrbtm;
                //       --Update variables if more layers were added.
                if (oldnc < i)
                {
                    tcdt[i] = tcdt[oldnc];
                    vapcdt[i] = vapcdt[oldnc];
                    wcandt[i] = wcandt[oldnc];
                    for (var j = 1; j <= nplant; ++j)
                    {
                        tlcdt[j][i] = tlcdt[j][oldnc];
                    }
                }
            label25:;
            }
        }

        // line 4230
        private void Rtdist(ref int nplant, int[] itype, ref int ns, double[] zs, double[] rootdp, double[] rroot0)
        {
            //
            //     THIS PROGRAM CALCULATES THE ROOT RESISTANCES AND FRACTION OF ROOTS
            //     WITHIN EACH SOIL LAYER GIVEN THE MAXIMUM ROOTING DEPTH.  A
            //     TRIANGULAR ROOTING DENSITY IS ASSUMED HAVING A MAXIMUM DENSITY AT
            //     A DEPTH OF ZMXDEN AND ZERO DENSITY AT THE SURFACE AND THE MAXIMUM
            //     ROOTING DEPTH.  ZMXDEN IS ASSUMED TO BE A CONSTANT
            //     FRACTION (RMXDEN) OF THE MAXIMUM ROOTING DEPTH.
            //     (CURRENTLY RMXDEN IS SET IS DATA STATEMENT)
            //***********************************************************************

            const double rmxden = 0.1;
            var area2 = 0.0;
            var area1 = 0.0;
            var zmid2 = 0.0;
            var zmid1 = 0.0;
            var zmxden = 0.0;
            for (var j = 1; j <= nplant; ++j)
            {
                _clayrs.Totrot[j] = 0.0;
                if (itype[j] != 0 && _clayrs.Totlai[j] != 0.0)
                {
                    //         TRANSPIRING PLANT -- CALCULATE FRACTION OF ROOTS IN EACH
                    //         SOIL LAYER; START BY COMPUTING DEPTH OF MAXIMUM ROOT DENSITY
                    zmxden = rmxden * rootdp[j];
                    //
                    zmid1 = 0.0;
                    for (var i = 1; i <= ns; ++i)
                    {
                        //           CALCULATE MID-POINT BETWEEN THIS AND NEXT NODE, I.E. THE
                        //           LOWER BOUNDARY OF THIS LAYER
                        if (i < ns)
                        {
                            zmid2 = (zs[i + 1] + zs[i]) / 2.0;
                        }
                        else
                        {
                            zmid2 = zs[ns];
                        }
                        //
                        if (zmid2 < zmxden)
                        {
                            //             BOTTOM OF LAYER IS LESS THAN DEPTH OF MAXIMUM DENSITY
                            _clayrs.Rootdn[j][i] = (zmid2 - zmid1) * (zmid2 + zmid1) / zmxden / rootdp[j];
                        }
                        else
                        {
                            //             BOTTOM OF LAYER IS BEYOND DEPTH OF MAXIMUM DENSITY
                            if (zmid2 < rootdp[j])
                            {
                                //               BOTTOM OF LAYER IS WITHIN ROOTING DEPTH
                                if (zmid1 < zmxden)
                                {
                                    //                 LAYER STRATTLES DEPTH OF MAXIMUM DENSITY
                                    area1 = (zmxden - zmid1) * (zmxden + zmid1) / zmxden;
                                    area2 = (zmid2 - zmxden) * (2.0 - (zmid2 - zmxden) / (rootdp[j] - zmxden));
                                    _clayrs.Rootdn[j][i] = (area1 + area2) / rootdp[j];
                                }
                                else
                                {
                                    //                 LAYER IS BEYOND DEPTH OF MAXIMUM ROOTING DENSITY BUT
                                    //                 IS FULLY WITHIN THE ROOTING DEPTH
                                    _clayrs.Rootdn[j][i] = (zmid2 - zmid1) * (2.0 * rootdp[j] - zmid2 - zmid1) / (rootdp[j] - zmxden) / rootdp[j];
                                }
                            }
                            else
                            {
                                //               BOTTOM OF LAYER IS BEYOND ROOTING DEPTH
                                if (zmid1 < rootdp[j])
                                {
                                    //                 TOP OF LAYER IS STILL WITHIN ROOTING DEPTH; THE
                                    //                 REMAINING FRACTION OF ROOTS ARE WITHIN THIS LAYER
                                    _clayrs.Rootdn[j][i] = 1.0 - _clayrs.Totrot[j];
                                }
                                else
                                {
                                    //                 LAYER IS BEYOND THE ROOTING DEPTH -- NO ROOTS
                                    _clayrs.Rootdn[j][i] = 0.0;
                                }
                            }
                        }
                        //           SUM THE TOTAL FRACTION OF ROOTS
                        _clayrs.Totrot[j] = _clayrs.Totrot[j] + _clayrs.Rootdn[j][i];
                        zmid1 = zmid2;
                    label10:;
                    }
                    //         CALCULATE EFFECTIVE ROOT CONDUCTANCE FOR EACH SOIL LAYER
                    for (var i = 1; i <= ns; ++i)
                    {
                        _clayrs.Rroot[j][i] = rroot0[j] * _clayrs.Rootdn[j][i] / _clayrs.Totrot[j];
                    label20:;
                    }
                }
            label30:;
            }
        }

        // line 4314
        private void Residue(ref int nr, ref int nrchang, ref int inital, double[] zr, double[] rhor, double[] trdt, double[] vaprdt, double[] gmcdt, ref double rload, ref double zrthik, ref double cover, ref double dirres, ref double tmpday, ref double humday, ref int nsalt, double[] tsdt, double[] matdt, double[][] concdt)
        {
            //
            //     THIS SUBROUTINE INITIALIZES THE RESIDUE LAYER AND ADJUSTS THE
            //     VARIABLES AS THE RESIDUE CHANGES OVER THE SIMULATION BASED ON
            //     USER INPUT
            //
            //***********************************************************************

            var tlconc = 0.0;
            var totpot = 0.0;
            var satv = 0.0;
            var hum = 0.0;
            var resden = 0.0;
            var nrnew = 0;
            var nnew = 0;
            var dummy = 0.0;
            var vap = 0.0;

            if (inital == 0 || nr == 0)
            {
                if (nr == 0) nr = 1;
                //        SET INITIAL VAPOR DENSITY AND TEMPERATURE OF RESIDUE EQUAL
                //        TO SOIL SURFACE NODE.
                tlconc = 0.0;
                for (var j = 1; j <= nsalt; ++j)
                {
                    tlconc = tlconc + concdt[j][1];
                label10:;
                }
                totpot = matdt[1] - tlconc * Constn.Ugas * (tsdt[1] + 273.16) / Constn.G;
                Vslope(ref dummy, ref satv, ref tsdt[1]);
                vap = satv * Math.Exp(.018 * Constn.G / Constn.Ugas / (tsdt[1] + 273.16) * totpot);
                //        IF SOIL IT TOO TOO DRY, RESET VAPOR DENSITY TO 1% REL. HUM.
                if (vap < 0.01 * satv) vap = 0.01 * satv;
                zr[1] = 0.0;
                trdt[1] = tsdt[1];
                vaprdt[1] = vap;
                //        INITIALIZE RESIDUE WATER CONTENT DEPENDING ON INPUT
                if (gmcdt[1] <= 0.0)
                {
                    //           USER INPUT REQUIRES ESTIMATE OF RESIDUE WATER CONTENT
                    //           ASSUME IT IS IN EQUILIBRIUM WITH SOIL SURFACE
                    hum = vap / satv;
                    Reshum(2, ref hum, ref dummy, ref gmcdt[1], ref trdt[1]);
                }
            }
            //
            //     COMPUTE LAYER PARAMETERS
            resden = rload / zrthik;
            rhor[1] = resden;
            if (cover < 0.999)
            {
                dirres = -Math.Log(1.0 - cover) / rload / 10.0;
            }
            else
            {
                //        AVOID NUMERICAL OVERLOAD
                dirres = 6.9 / rload / 10.0;
            }

            if (nrchang == 0 && nr > 1)
            {
                //         DO NOT REDEFINE NUMBER OF RESIDUE LAYERS
                nrnew = nr;
            }
            else
            {
                //         COMPUTE NUMBER OF RESIDUE LAYERS: MAXIMUM LAYER THICKNESS IS 2.5CM
                nrnew = (int)(zrthik / 0.050);      // The 'int' command in FORTRAN and C# both truncate the value.  Note: similar statements in Canlay use 'nint' which rounds.
                if (nrnew == 0) nrnew = 1;
                if (nrnew > 10) nrnew = 10;
            }

            if (inital == 0)
            {
                //        FORCE INITIALIZATION OF ALL RESIDUE LAYERS
                nnew = nrnew;
            }
            else
            {
                nnew = nrnew - nr;
                if (nnew > 0)
                {
                    //           NEW RESIDUE - MOVE OLD LAYERS DOWN
                    for (var i = 1; i <= nr; ++i)
                    {
                        zr[nnew + i] = zrthik * (nnew + i - 1) / nrnew;
                        gmcdt[nnew + i] = gmcdt[i];
                        vaprdt[nnew + i] = vaprdt[i];
                        trdt[nnew + i] = trdt[i];
                        rhor[nnew + i] = resden;
                    label20:;
                    }
                    //           ASSUME ADDED RESIDUE IS IN EQUILIBRIUM WITH AIR
                    Vslope(ref dummy, ref satv, ref tmpday);
                    vap = humday * satv;
                    //           IF AIR IT TOO TOO DRY, RESET VAPOR DENSITY TO 1% REL. HUM.
                    if (humday < 0.01) vap = 0.01 * satv;
                    hum = satv / vap;
                    Reshum(2, ref hum, ref dummy, ref gmcdt[1], ref tmpday);
                    trdt[1] = tmpday;
                    vaprdt[1] = vap;
                }
            }
            //
            //     INITIALIZE CONDITIONS AND PARAMETERS FOR NEW LAYERS
            for (var i = 1; i <= nnew; ++i)
            {
                zr[i] = zrthik * (i - 1) / nrnew;
                gmcdt[i] = gmcdt[1];
                rhor[i] = resden;
                vaprdt[i] = vap;
                trdt[i] = trdt[1];
            label25:;
            }

            zr[nrnew + 1] = zrthik;
            nr = nrnew;
        }

        // line 4415
        private void Systm(ref int nc, ref int nplant, ref int nsp, int[] htordr, double[] zc, double[] zsp, ref double zmsrf, ref double zhsrf, ref double zersrf, ref double zmsp, ref double zhsp, ref double height, ref double solr, ref double ta, double[] tccrit)
        {
            //
            //     THIS SUBROUTINE DEFINES THE ROUGHNESS PARAMETERS FOR THE SYSTEM,
            //     AND DETERMINES WHETHER THE CANOPY WILL TRANSPIRE
            //
            //***********************************************************************
            //

            _windv.Zm = zmsrf;
            _windv.Zh = zhsrf;
            _windv.Zero = zersrf;

            var sumlai = 0.0;
            var zmmax = 0.0;
            var zeromax = 0.0;
            var j = 0;
            var zerofull = 0.0;
            var zmfull = 0.0;
            var zeroj = 0.0;
            var zmj = 0.0;
            var v = 0.0;

            //
            if (nc > 0)
            {
                if (nsp > 0)
                {
                    _windv.Zmsub = zmsp;
                    _windv.Zhsub = zhsp;
                    _windv.Zersub = 0.0;
                }
                else
                {
                    _windv.Zmsub = zmsrf;
                    _windv.Zhsub = zhsrf;
                    _windv.Zersub = zersrf;
                }
                //        COMPUTE LAI AND REDUCE ROUGHNESS PARAMETERS FOR SPARSE CANOPIES
                //        BASED ON WORK OF ZENG AND WANG (2007; J HYDROMET 8:730-737)
                //        (ASSUMES LAI = 2.0 IS A FULL CANOPY)
                //
                //        START WITH TALLEST PLANT - STOP WHEN ALL PLANTS ARE
                //        CONSIDERED OR WHEN IT GETS BELOW COMPUTED ZERO DISPLACEMENT.
                //        ZERO AND ZM WILL BE PLANT COMBINATION THAT GIVES MAXIMUM ZERO
                //        THIS WILL COMPENSATE FOR A SPARSE TREE CANOPY OVER A DENSE
                //        UNDERSTORY IF ZERO IS BELOW THE UNDERSTORY
                sumlai = 0.0;
                zmmax = 0.0;
                zeromax = 0.0;
                for (var k = nplant; k >= 1; --k)
                {
                    j = htordr[k];
                    if (_clayrs.Plantz[j] <= zeromax) goto label15;
                    sumlai = sumlai + _clayrs.Totlai[j];
                    zerofull = 0.77 * _clayrs.Plantz[j];
                    zmfull = 0.13 * _clayrs.Plantz[j];
                    if (sumlai < 2.0)
                    {
                        v = (1.0 - Math.Exp(-sumlai)) / (1.0 - Math.Exp(-2.0));
                        zeroj = v * zerofull + (1.0 - v) * _windv.Zersub;
                        //xxxx         ZMJ = EXP(V*ALOG(ZMFULL) + (1.0-V)*ALOG(ZMSUB))
                        zmj = v * zmfull + (1.0 - v) * _windv.Zmsub;
                    }
                    else
                    {
                        zeroj = zerofull;
                        zmj = zmfull;
                    }
                    if (zeroj > zeromax)
                    {
                        zeromax = zeroj;
                        zmmax = zmj;
                    }
                    if (sumlai >= 2.0) goto label15;
                    label10:;
                }
            label15:;
                _windv.Zero = zeromax;
                _windv.Zm = zmmax;
                _windv.Zh = 0.2 * _windv.Zm;
                //
                //        NO TRANPIRATION AT NIGHT
                if (solr < 10.0)
                {
                    //Solr    IF (SOLR .LT. 100.0) THEN
                    for (j = 1; j <= nplant; ++j)
                    {
                        _clayrs.Ievap[j] = 0;
                    label20:;
                    }
                }
                else
                {
                    for (j = 1; j <= nplant; ++j)
                    {
                        _clayrs.Ievap[j] = 1;
                        //              CHECK IF TEMPERATURE IS TOO COLD FOR TRANSPIRATION
                        if (ta <= tccrit[j]) _clayrs.Ievap[j] = 0;
                        label30:;
                    }
                }
            }
            else
            {
                //
                if (nsp > 0)
                {
                    //           SNOW IS SURFACE MATERIAL -- DEFINE ROUGHNESS ELEMENTS AND
                    //           ZERO PLANE OF DISPLACEMENT
                    _windv.Zm = zmsp;
                    _windv.Zh = zhsp;
                    _windv.Zero = zsp[nsp + 1];
                    //           DO NOT LET DISPLACEMENT PLANE BE ABOVE HEIGHT OF INSTRUMENTS
                    if (_windv.Zero + _windv.Zm > height) _windv.Zero = height / 2.0;
                }
            }
            //
        }

        // line 4516
        /// <summary>
        /// sets swcan, swsnow?, swres, swsoil, swdown, swup
        /// </summary>
        private void Swrbal(ref int nplant, ref int nc, ref int nsp, ref int nr, double[] xangle, double[] clumpng, double[][] swcan, double[] swsnow, double[] swres, ref double swsoil, double[] swdown, double[] swup, ref double sunhor, double[] canalb, double[] zsp, double[] dzsp, double[] rhosp, double[] zr, double[] rhor, ref double albres, ref double dirres, ref double albdry, ref double albexp, ref double vlc, ref double alatud, ref double slope, ref double aspect, ref double hrnoon, ref double hafday, ref double declin, ref int hour, ref int nhrpdt)
        {
            //
            //     THIS SUBROUTINE DETERMINES THE SHORT-WAVE RADIATION BALANCE FOR
            //     FOR EACH NODE ABOVE THE GROUND SURFACE.
            //
            //***********************************************************************
            //

            //**** SEPARATE THE TOTAL SOLAR RADIATION INTO DIRECT AND DIFFUSE
            var direct = 0.0;
            var diffus = 0.0;
            var sunslp = 0.0;
            var altitu = 0.0;
            var albsno = 0.0;
            var albsnodir = 0.0;
            var albsoi = 0.0;
            var albnxt = 0.0;
            var albnxtdir = 0.0;
            var reflect = 0.0;

            Solar(ref direct, ref diffus, ref sunslp, ref altitu, ref sunhor, ref alatud, ref slope, ref aspect, ref hrnoon, ref hafday, ref declin, ref hour, ref nhrpdt);
            swdown[1] = direct + diffus;
            if ((direct + diffus) <= 0.0) goto label60;
            //
            //**** DETERMINE THE SOIL AND SNOW ALBEDO
            albsoi = albdry * Math.Exp(-albexp * vlc);
            if (nsp > 0)
            {
                if (nr > 0)
                {
                    albnxt = albres;
                    albnxtdir = albres;
                }
                else
                {
                    albnxt = albsoi;
                    albnxtdir = albsoi;
                }
                Snoalb(ref nsp, ref albsno, ref albsnodir, zsp, rhosp, ref sunslp, ref albnxt);
            }
            //
            //
            //**** DETERMINE THE SOLAR RADIATION BALANCE FOR EACH MATERIAL, STARTING
            //     FROM THE SURFACE MATERIAL AND WORKING DOWNWARD.
            //
            //**** SOLAR RADIATION BALANCE OF THE CANOPY
            if (nc > 0)
            {
                if (nsp == 0 && nr > 0)
                {
                    //           RESIDUE LIES BENEATH CANOPY - DETERMINE SOLAR RADIATION
                    //           BALANCE THROUGH CANOPY AND RESIDUE TOGETHER
                    Swrcan(ref nplant, ref nc, ref nsp, ref nr, xangle, clumpng, swcan, swres, ref direct, ref diffus, swdown, swup, ref sunslp, ref altitu, canalb, zr, rhor, ref albres, ref dirres, ref albsoi, ref albsoi);
                }
                else
                {
                    //           CALCULATE RADIATION THROUGH CANOPY DOWN TO SNOW OR SOIL
                    if (nsp > 0)
                    {
                        albnxt = albsno;
                        albnxtdir = albsnodir;
                    }
                    else
                    {
                        albnxt = albsoi;
                        albnxtdir = albsoi;
                    }
                    var int0 = 0;
                    Swrcan(ref nplant, ref nc, ref nsp, ref int0, xangle, clumpng, swcan, swres, ref direct, ref diffus, swdown, swup, ref sunslp, ref altitu, canalb, zr, rhor, ref albres, ref dirres, ref albnxt, ref albnxtdir);
                    //
                }
            }
            //
            //**** SOLAR RADIATION BALANCE OF THE SNOWPACK
            if (nsp > 0)
            {
                Swrsno(ref nsp, swsnow, dzsp, rhosp, ref direct, ref diffus, ref reflect, ref albsno, ref albsnodir);
                //        SET UPWARD SOLAR FROM SNOW
                swup[nc + 1] = reflect;
            }
            //
            //
            //**** SOLAR RADIATION BALANCE OF THE RESIDUE IF NO CANOPY PRESENT OR IF
            //     RESIDUE IS COVERED BY SNOW
            if (nr > 0)
            {
                if (nsp > 0 || nc == 0)
                {
                    var int0 = 0;
                    Swrcan(ref nplant, ref int0, ref nsp, ref nr, xangle, clumpng, swcan, swres, ref direct, ref diffus, swdown, swup, ref sunslp, ref altitu, canalb, zr, rhor, ref albres, ref dirres, ref albsoi, ref albsoi);
                }
            }
            //
            //**** SOLAR RADIATION BALANCE FOR SOIL SURFACE
            swsoil = (direct + diffus) * (1.0 - albsoi);
            //     SET UPWARD SOLAR ABOVE SYSTEM IF FROM SOIL
            if (nc == 0 && nsp == 0 && nr == 0) swup[1] = (direct + diffus) * albsoi;
            //
            return;
        //
        //**** NO SOLAR RADIATION -- SET ABSORBED SOLAR RADIATION TO ZERO
        //
        //     CANOPY NODES
        label60:;
            for (var i = 1; i <= nc; ++i)
            {
                for (var j = 1; j <= nplant + 1; ++j)
                {
                    swcan[j][i] = 0.0;
                label65:;
                }
                swdown[i] = 0.0;
                swup[i] = 0.0;
            label70:;
            }
            swdown[nc + 1] = 0.0;
            swup[nc + 1] = 0.0;
            //
            //     DEFINE THE DIFFUSE RADIATION TRANSMISSION FOR LONG-WAVE BALANCE

            var double0 = 0.0;
            if (nc > 0) Transc(ref nplant, ref nc, xangle, clumpng, _radcan.Tdircc, _radcan.Tdiffc, _radcan.Difkl, _radcan.Dirkl, _radcan.Fbdu, _radcan.Fddu, ref double0, ref double0);
            //
            //     SNOWPACK NODES
            for (var i = 1; i <= nsp; ++i)
            {
                swsnow[i] = 0.0;
            label80:;
            }
            //
            //     RESIDUE NODES
            for (var i = 1; i <= nr; ++i)
            {
                swres[i] = 0.0;
            label90:;
            }
            //     DEFINE THE DIFFUSE RADIATION TRANSMISSION FOR LONG-WAVE BALANCE
            var double758 = 0.785;
            if (nr > 0) Transr(ref nr, _radres.Tdirec, _radres.Tdiffu, zr, rhor, ref double758, ref dirres);
            //
            //     SOIL SURFACE
            swsoil = 0.0;
        }

        // line 4646
        /// <summary>
        /// sets direct, diffus, sunslp, altitu
        /// </summary>
        private void Solar(ref double direct, ref double diffus, ref double sunslp, ref double altitu, ref double sunhor, ref double alatud, ref double slope, ref double aspect, ref double hrnoon, ref double hafday, ref double declin, ref int hour, ref int nhrpdt)
        {
            //
            //     THIS SUBROUTINE SEPARATES THE TOTAL RADIATION MEASURED ON THE
            //     HORIZONTAL INTO THE DIRECT AND DIFFUSE ON THE LOCAL SLOPE.
            //
            //***********************************************************************

            var sinazm = 0.0;
            var cosazm = 0.0;
            var sumalt = 0.0;
            var cosalt = 0.0;
            var sunmax = 0.0;
            var sunris = 0.0;
            var sunset = 0.0;
            var hrwest = 0.0;
            var thour = 0.0;
            var hrangl = 0.0;
            var sinalt = 0.0;
            var azm = 0.0;
            var sun = 0.0;
            var azmuth = 0.0;
            var ttotal = 0.0;
            var tdiffu = 0.0;
            var dirhor = 0.0;

            //**** CHECK IF SUN HAS RISEN YET (OR IF IT HAS ALREADY SET)
            if (sunhor <= 0.0)
            {
                direct = 0.0;
                diffus = 0.0;
                return;
            }
            sunris = hrnoon - hafday / 0.261799;
            sunset = hrnoon + hafday / 0.261799;
            //
            //**** CALCULATE HOUR ANGLE AT WHICH THE SUN WILL BE DUE EAST/WEST IN
            //     ORDER TO ADJUST AZIMUTH ANGLE FOR SOUTHERN AZIMUTHS
            //     -- SIN(AZIMUTH) TELLS YOU ONLY THE EAST/WEST DIRECTION - NOT
            //     WHETHER THE SUN IS NORTH/SOUTH.

            if (Math.Abs(declin) >= Math.Abs(alatud))
            {
                //        LATITUDE IS WITHIN THE TROPICS (EQUATION WON'T WORK)
                hrwest = 3.14159;
            }
            else
            {
                hrwest = Math.Acos(Math.Tan(declin) / Math.Tan(alatud));
            }
            //
            //**** SUM UP VALUES AND FIND AVERAGE SUN POSITION FOR TIME STEP
            for (var ihr = hour - nhrpdt; ihr <= hour; ++ihr)
            {
                thour = ihr;
                //
                //****    DETERMINE THE GEOMETRY OF THE SUN'S RAYS AT CURRENT TIME
                hrangl = 0.261799 * (ihr - hrnoon);
                if (thour > sunris && thour < sunset)
                {
                    //           SUN IS ABOVE HORIZON -- CALCULATE ITS ALTITUDE ABOVE THE
                    //           HORIZON (ALTITU) AND ANGLE FROM DUE NORTH (AZMUTH)
                    sinalt = Math.Sin(alatud) * Math.Sin(declin) + Math.Cos(alatud) * Math.Cos(declin) * Math.Cos(hrangl);
                    altitu = Math.Asin(sinalt);
                    azm = Math.Asin(-Math.Cos(declin) * Math.Sin(hrangl) / Math.Cos(altitu));
                    //           CORRECT AZIMUTH FOR SOUTHERN ANGLES
                    if (alatud - declin > 0.0)
                    {
                        //              NORTHERN LATITUDES   (HRANGL=0.0 AT NOON)
                        if (Math.Abs(hrangl) < hrwest) azm = 3.14159 - azm;
                    }
                    else
                    {
                        //              SOUTHERN LATITUDES
                        if (Math.Abs(hrangl) >= hrwest) azm = 3.14159 - azm;
                    }
                    //           SUM CONDITIONS TO GET AVERAGE ALTITUDE AND AZMUTH
                    //           (OBTAIN AVERAGE BY SUMMING VECTOR COMPONENTS)
                    sun = Swrcoe.Solcon * sinalt;
                    sumalt = sumalt + sun * sinalt;
                    cosalt = cosalt + sun * Math.Cos(altitu);
                    sinazm = sinazm + sun * Math.Sin(azm);
                    cosazm = cosazm + sun * Math.Cos(azm);
                    sunmax = sunmax + sun;
                }
            //
            label10:;
            }
            //
            //**** DETERMINE AVERAGE SOLAR RADIATION, AVERAGE ALTITUDE AND AZIMUTH OF
            //     THE SUN AND ANGLE ON LOCAL SLOPE
            if (sunmax == 0)
            {
                altitu = 0.0;
                sunslp = 0.0;
            }
            else
            {
                altitu = Math.Atan(sumalt / cosalt);
                azmuth = Math.Atan2(sinazm, cosazm);
                sunmax = sunmax / (nhrpdt + 1);
                sunslp = Math.Asin(Math.Sin(altitu) * Math.Cos(slope) + Math.Cos(altitu) * Math.Sin(slope) * Math.Cos(azmuth - aspect));
            }
            //
            //**** SEPARATE THE SOLAR RADIATION INTO DIRECT AND DIFFUSE COMPONENTS
            if (altitu <= 0.0)
            {
                //     SUN IS BELOW THE HORIZON - ALL RADIATION MUST BE DIFFUSE
                diffus = sunhor;
                direct = 0.0;
                return;
            }
            ttotal = sunhor / sunmax;
            //     LIMIT TOTAL TRANSMISSIVITY TO MAXIMUM (DIFATM) WHICH WILL
            //     CAUSE TDIFFU TO BE 0.0
            if (ttotal > Swrcoe.Difatm) ttotal = Swrcoe.Difatm;
            tdiffu = ttotal * (1.0 - Math.Exp(0.6 * (1.0 - Swrcoe.Difatm / ttotal) / (Swrcoe.Difatm - 0.4)));
            diffus = tdiffu * sunmax;
            dirhor = sunhor - diffus;
            //
            //**** NOW CALCULATE THE DIRECT SOLAR RADIATION ON THE LOCAL SLOPE
            if (sunslp <= 0.0)
            {
                //        SUN HAS NOT RISEN ON THE LOCAL SLOPE -- NO DIRECT RADIATION
                direct = 0.0;
            }
            else
            {
                direct = dirhor * Math.Sin(sunslp) / Math.Sin(altitu);
                //        IF THE SUN'S ALTITUDE IS NEAR ZERO, THE CALCULATED DIRECT
                //        RADIATION ON THE SLOPING SURFACE MAY BE UNREALISTICALLY LARGE --
                //        LIMIT DIRECT TO 5*DIRHOR (THIS IS APPROXIMATELY THE CASE WHEN
                //        THE SUN IS 10 DEGREES ABOVE THE HORIZON AND THE SLOPING SURFACE
                //        IS PERPENDICULAR TO THE SUN`S RAYS
                if (direct > 5.0 * dirhor) direct = 5.0 * dirhor;
            }
        }

        // line 4763
        /// <summary>
        /// sets albsno, albsnodir
        /// </summary>
        private void Snoalb(ref int nsp, ref double albsno, ref double albsnodir, double[] zsp, double[] rhosp, ref double sunslp, ref double albnxt)
        {
            //
            //     THIS SUBROUTINE COMPUTES THE ALBEDO, THE EXTINCTION COEFFICIENT
            //     AND THE AMOUNT OF SOLAR RADIATION ABSORBED BY EACH LAYER
            //
            //***********************************************************************
            //
            //     ASSUME 58% OF SOLAR RADIATION IS IN VISIBLE SPECTRAL
            const double vis = 0.58;

            var spgrav = 0.0;
            var grain = 0.0;
            var abs = 0.0;
            var w = 0.0;
            var exc = 0.0;
            var y = 0.0;
            var dzir = 0.0;
            var dzv = 0.0;

            //     DETERMINE THE ALBEDO OF THE SNOW
            spgrav = rhosp[1] / Constn.Rhol;
            grain = Spprop.G1 + Spprop.G2 * spgrav * spgrav + Spprop.G3 * Math.Pow(spgrav, 4);
            albsno = 1.0 - 0.206 * Spprop.Extsp * Math.Sqrt(grain);
            if (albsno < 0.35) albsno = 0.35;
            //
            if (zsp[nsp + 1] <= 0.04)
            {
                //        SNOWPACK IS LESS THAN 4.0 CM -- ALBEDO IS AFFECTED BY
                //        UNDERLYING MATERIAL
                abs = 1.0 - albnxt;
                w = 2.0 * (1.0 - albsno) / (1.0 + albsno);
                //        EXC = EXTINCTION COEFFICIENT --> CONVERT FROM 1/CM TO 1/M
                exc = Spprop.Extsp * spgrav * Math.Sqrt(1.0 / grain) * 100.0;
                y = Math.Exp(-exc * zsp[nsp + 1]) * (abs + w * (abs / 2.0 - 1.0)) / (w * (abs / 2.0 - 1.0) * Math.Cosh(exc * zsp[nsp + 1]) - abs * Math.Sinh(exc * zsp[nsp + 1]));
                albsno = (1.0 - w * (1.0 - y) / 2) / (1.0 + w * (1.0 - y) / 2);
            }
            //
            if (zsp[nsp + 1] < Swrcoe.Snocof)
            {
                //        SNOCOF IS MINIMUM DEPTH OF SNOW FOR COMPLETE GROUND COVER --
                //        SNOW IS NOT COMPLETELY COVERED WITH SNOW
                albsno = albnxt + (albsno - albnxt) * Math.Pow((zsp[nsp + 1] / Swrcoe.Snocof), Swrcoe.Snoexp);
            }
            //
            spgrav = rhosp[1] / Constn.Rhol;
            grain = Spprop.G1 + Spprop.G2 * spgrav * spgrav + Spprop.G3 * Math.Pow(spgrav, 4);
            //     COMPUTE ALBEDO FOR DIRECT RADIATION
            if (sunslp > 0.0)
            {
                dzv = (1.375e-3 * Math.Sqrt(grain * 1000.0)) * (1.0 - Math.Sin(sunslp));
                dzir = (2.0e-3 * Math.Sqrt(grain * 1000.0) + 0.1) * (1.0 - Math.Sin(sunslp));
            }
            else
            {
                //        SUN IS NOT ABOVE THE HORIZON
                dzv = 0.0;
                dzir = 0.0;
            }
            albsnodir = albsno + dzv * vis + dzir * (1.0 - vis);
        }

        // line 4821
        private void Swrsno(ref int nsp, double[] swsnow, double[] dzsp, double[] rhosp, ref double direct, ref double diffus, ref double reflect, ref double albsno, ref double albsnodir)
        {
            //
            //     THIS SUBROUTINE COMPUTES THE AMOUNT OF OF SOLAR RADIATION
            //     ABSORBED BY EACH LAYER WITHIN THE SNOWPACK
            //
            //***********************************************************************
            //
            //

            //     COMPUTE THE TOTAL ABSORBED FOR THE ENTIRE SNOWPACK
            var total = direct * (1.0 - albsnodir) + diffus * (1.0 - albsno);
            reflect = direct + diffus - total;
            //
            //     CALCULATE EXTINCTION COEFF. AND RADIATION ABSORBED BY EACH LAYER
            for (var i = 1; i <= nsp; ++i)
            {
                var spgrav = rhosp[i] / Constn.Rhol;
                var grain = Spprop.G1 + Spprop.G2 * spgrav * spgrav + Spprop.G3 * Math.Pow(spgrav, 4);
                //        EXC = EXTINCTION COEFFICIENT --> CONVERT FROM 1/CM TO 1/M
                var exc = Spprop.Extsp * spgrav * Math.Sqrt(1.0 / grain) * 100.0;
                var percab = 1.0 - Math.Exp(-exc * dzsp[i]);
                swsnow[i] = total * percab;
                total = total - swsnow[i];
            label10:;
            }
            //
            //     DEFINE THE DIFFUSE RADIATION OUT THE BOTTOM OF THE SNOWPACK
            //     (THIS WILL BE ABSORBED BY EITHER THE RESIDUE OR THE SOIL.)
            diffus = total;
            direct = 0.0;
        }

        // line 4861
        private void Swrcan(ref int nplant, ref int nc, ref int nsp, ref int nr, double[] xangle, double[] clumpng, double[][] swcan, double[] swres, ref double direct, ref double diffus, double[] swdown, double[] swup, ref double sunslp, ref double altitu, double[] canalb, double[] zr, double[] rhor, ref double albres, ref double dirres, ref double albnxt, ref double albnxtdir)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE AMOUNT OF SOLAR RADIATION ABSORBED
            //     BY EACH CANOPY AND RESIDUE NODE
            //
            //***********************************************************************
            //
            var diralb = new double[11];
            var difalb = new double[11];
            var dirtrn = new double[11];
            var diftrn = new double[11];
            var fbrefu = new double[11];
            var fdrefu = new double[11];
            var fbtrnu = new double[11];
            var fdtrnu = new double[11];
            var fbrefd = new double[11];
            var fdrefd = new double[11];
            var fbtrnd = new double[11];
            var fdtrnd = new double[11];
            var a = new double[43];
            var b = new double[43];
            var c = new double[43];
            var d = new double[43];
            var dir = new double[22];
            var down = new double[22];
            var up = new double[22];

            var cantrn = Enumerable.Repeat(0.2, 9).ToArray();

            var nr1 = 0;
            var n = 0;
            var nlayr = 0;
            var totdir = 0.0;
            var totdif = 0.0;
            var fract1 = 0.0;
            var fract2 = 0.0;
            var absorb = 0.0;
            var k = 0;

            //**** OBTAIN TRANSMISSIVITY TO DIRECT AND DIFFUSE RADIATION THROUGH
            //     CANOPY AND/OR RESIDUE DEPENDING ON LAYERING OF MATERIALS
            if (nc > 0)
            {
                //        GET TRANSMISSION COEFFICIENTS THROUGH LAYERS
                Transc(ref nplant, ref nc, xangle, clumpng, _radcan.Tdircc, _radcan.Tdiffc, _radcan.Difkl, _radcan.Dirkl, _radcan.Fbdu, _radcan.Fddu, ref sunslp, ref altitu);
                //        CALCULATE WEIGHTED ALBEDO, TRANSMISSION AND SCATTERING FOR
                //        DIRECT AND DIFFUSE RADIATION OR EACH OF THE CANOPY LAYERS.
                //        THE FRACTION OF DOWNWARD RADIATION TRANSMITTED THROUGH THE
                //        LEAVES AND SCATTERED DOWNWARD EQUALS DOWNWARD REFLECTED UPWARD.
                //        UPWARD SCATTERED REFLECTED AND TRANSMITTED (FBREFU AND FBTRNU)
                //        MUST BE WEIGHTED BASED ON ALBEDO AND TRANSMISSION RESPECTIVELY.
                for (var i = 1; i <= nc; ++i)
                {
                    diralb[i] = 0.0;
                    difalb[i] = 0.0;
                    dirtrn[i] = 0.0;
                    diftrn[i] = 0.0;
                    fbrefu[i] = 0.0;
                    fdrefu[i] = 0.0;
                    fbtrnu[i] = 0.0;
                    fdtrnu[i] = 0.0;
                    for (var j = 1; j <= nplant; ++j)
                    {
                        diralb[i] = diralb[i] + canalb[j] * _radcan.Dirkl[j][i];
                        difalb[i] = difalb[i] + canalb[j] * _radcan.Difkl[j][i];
                        dirtrn[i] = dirtrn[i] + cantrn[j] * _radcan.Dirkl[j][i];
                        diftrn[i] = diftrn[i] + cantrn[j] * _radcan.Difkl[j][i];
                        fbrefu[i] = fbrefu[i] + _radcan.Fbdu[j] * canalb[j] * _radcan.Dirkl[j][i];
                        fdrefu[i] = fdrefu[i] + _radcan.Fddu[j] * canalb[j] * _radcan.Difkl[j][i];
                        //              FOR A SINGLE PLANT SPECIES, FRACTION TRANSMITTED UPWARD
                        //              IS EQUAL TO 1 - REFLECTED UPWARD (BUT NOT NECESSARILY FOR
                        //              CANOPY LAYER BECAUSE THEY ARE WEIGHTED BY LAI, K, ETC.)
                        fbtrnu[i] = fbtrnu[i] + (1.0 - _radcan.Fbdu[j]) * cantrn[j] * _radcan.Dirkl[j][i];
                        fdtrnu[i] = fdtrnu[i] + (1.0 - _radcan.Fddu[j]) * cantrn[j] * _radcan.Difkl[j][i];
                    label10:;
                    }
                    if (diralb[i] > 0.0)
                    {
                        fbrefu[i] = fbrefu[i] / diralb[i];
                        fdrefu[i] = fdrefu[i] / difalb[i];
                    }
                    if (dirtrn[i] > 0.0)
                    {
                        fbtrnu[i] = fbtrnu[i] / dirtrn[i];
                        fdtrnu[i] = fdtrnu[i] / diftrn[i];
                    }
                    //           FRACTION SCATTERED DOWNWARD IS 1 - SCATTERED UPWARD
                    fbrefd[i] = 1.0 - fbrefu[i];
                    fdrefd[i] = 1.0 - fdrefu[i];
                    fbtrnd[i] = 1.0 - fbtrnu[i];
                    fdtrnd[i] = 1.0 - fdtrnu[i];
                    diralb[i] = diralb[i] / _radcan.Dirkl[nplant + 1][i];
                    difalb[i] = difalb[i] / _radcan.Difkl[nplant + 1][i];
                    dirtrn[i] = dirtrn[i] / _radcan.Dirkl[nplant + 1][i];
                    diftrn[i] = diftrn[i] / _radcan.Difkl[nplant + 1][i];
                label15:;
                }
            }
            //
            if (nr > 0) Transr(ref nr, _radres.Tdirec, _radres.Tdiffu, zr, rhor, ref sunslp, ref dirres);
            nr1 = nr;
            if (nr == 1)
            {
                //        RADIATION EXCHANGE IS NOT ACCURATE WITH ONLY ONE RESIDUE LAYER.
                //        SPLIT RESIDUE LAYER IN TWO AND REDEFINE TRANSMISSION COEFF.
                //        FOR TWO RESIDUE LAYERS.
                nr1 = 2;
                _radres.Tdirec[1] = Math.Sqrt(_radres.Tdirec[1]);
                _radres.Tdiffu[1] = Math.Sqrt(_radres.Tdiffu[1]);
                _radres.Tdirec[2] = _radres.Tdirec[1];
                _radres.Tdiffu[2] = _radres.Tdiffu[1];
            }
            //
            //**** RADIATION EXCHANGE WITHIN THE CANOPY AND RESIDUE
            //     INITIALIZE MATRIX ELEMENTS
            n = 1;
            a[1] = 0.0;
            b[1] = 1.0;
            c[1] = 0.0;
            d[1] = diffus;
            dir[1] = direct;
            //
            for (var i = 1; i <= nc; ++i)
            {
                n = n + 1;
                //
                //        COMPUTE COEFFICIENTS FOR UPWELLING RADIATION ABOVE LAYER
                a[n] = -(1.0 - _radcan.Tdiffc[i]) * (fdrefu[i] * difalb[i] + fdtrnu[i] * diftrn[i]);
                b[n] = 1.0;
                c[n] = -_radcan.Tdiffc[i] - (1.0 - _radcan.Tdiffc[i]) * (fdrefd[i] * difalb[i] + fdtrnd[i] * diftrn[i]);
                d[n] = (1.0 - _radcan.Tdircc[i]) * (fbrefu[i] * diralb[i] + fbtrnu[i] * dirtrn[i]) * dir[i];
                //        COMPUTE COEFFICIENTS FOR DOWNWELLING RADIATION BELOW LAYER
                n = n + 1;
                a[n] = c[n - 1];
                b[n] = 1.0;
                c[n] = a[n - 1];
                d[n] = (1.0 - _radcan.Tdircc[i]) * (fbrefd[i] * diralb[i] + fbtrnd[i] * dirtrn[i]) * dir[i];
                //        COMPUTE DIRECT RADIATION PASSING THROUGH THIS LAYER
                dir[i + 1] = dir[i] * _radcan.Tdircc[i];
            label20:;
            }
            //
            for (var i = 1; i <= nr1; ++i)
            {
                n = n + 1;
                //        COMPUTE COEFFICIENTS FOR UPWELLING RADIATION ABOVE LAYER
                //        HORIZONTAL LEAF ORIENTATION IS ASSUMED FOR SCATTERING.
                //        THEREFORE ONLY BACKSCATTERING; NO FORWARD SCATTERING
                a[n] = -albres * (1.0 - _radres.Tdiffu[i]);
                b[n] = 1.0;
                c[n] = -_radres.Tdiffu[i];
                d[n] = albres * (1.0 - _radres.Tdirec[i]) * dir[nc + i];
                //        COMPUTE COEFFICIENTS FOR DOWNWELLING RADIATION BELOW LAYER
                n = n + 1;
                a[n] = c[n - 1];
                b[n] = 1.0;
                c[n] = a[n - 1];
                d[n] = 0.0;
                //        COMPUTE DIRECT RADIATION PASSING THROUGH THIS LAYER
                dir[nc + i + 1] = dir[nc + i] * _radres.Tdirec[i];
            label30:;
            }
            //
            //     COMPUTE MATRIX ELEMENTS FOR LOWER SURFACE BOUNDARY
            n = n + 1;
            b[n] = 1.0;
            c[n] = 0.0;
            a[n] = -albnxt;
            d[n] = albnxtdir * dir[nc + nr1 + 1];
            //
            //     CALL MATRIX SOLVER
            nlayr = nc + nr1;
            Solvrad(ref nlayr, a, b, c, d, down, up);
            //
            //**** DETERMINE THE AMOUNT OF RADIATION ABSORBED AT EACH CANOPY NODE BY
            //     WEIGHTING ACCORDING TO ABSORBTION, LAI, AND ATTENUATION FACTOR
            for (var i = 1; i <= nc; ++i)
            {
                totdir = 0.0;
                totdif = 0.0;
                for (var j = 1; j <= nplant; ++j)
                {
                    totdir = totdir + (1.0 - canalb[j] - cantrn[j]) * _radcan.Dirkl[j][i];
                    totdif = totdif + (1.0 - canalb[j] - cantrn[j]) * _radcan.Difkl[j][i];
                label63:;
                }
                swcan[nplant + 1][i] = 0.0;
                for (var j = 1; j <= nplant; ++j)
                {
                    fract1 = (1.0 - canalb[j] - cantrn[j]) * _radcan.Dirkl[j][i] / totdir;
                    fract2 = (1.0 - canalb[j] - cantrn[j]) * _radcan.Difkl[j][i] / totdif;
                    swcan[j][i] = fract1 * (1.0 - diralb[i] - dirtrn[i]) * (1.0 - _radcan.Tdircc[i]) * dir[i] + fract2 * (1.0 - difalb[i] - diftrn[i]) * (1.0 - _radcan.Tdiffc[i]) * (down[i] + up[i + 1]);
                    swcan[nplant + 1][i] = swcan[nplant + 1][i] + swcan[j][i];
                label64:;
                }
                //        SAVE FLUXES BETWEEN NODES
                swdown[i] = dir[i] + down[i];
                swup[i] = up[i];
            label65:;
            }
            if (nc > 0)
            {
                swdown[nc + 1] = dir[nc + 1] + down[nc + 1];
                swup[nc + 1] = up[nc + 1];
            }
            else
            {
                //        UPWARD SOLAR AT SURFACE COMES FROM RESIDUE
                if (nsp == 0) swup[1] = up[1];
            }
            //**** DETERMINE THE AMOUNT OF RADIATION ABSORBED AT EACH RESIDUE NODE
            absorb = 0.0;
            for (var i = 1; i <= nr1; ++i)
            {
                k = i + nc;
                swres[i] = dir[k] + down[k] + up[k + 1] - dir[k + 1] - down[k + 1] - up[k];
                absorb = absorb + swres[i];
            label70:;
            }
            if (nr == 1)
            {
                swres[1] = swres[1] + swres[2];
                swres[2] = 0.0;
            }
            //
            //**** IF SNOW LAYER LIES ABOVE THE RESIDUE, ALL RADIATION LEAVING THE
            //     THE RESIDUE IS ASSUMED TO BE REFLECTED BACK AND ABSORBED.
            //     SPLIT UP THIS RADIATION PROPORTIONATELY BETWEEN THE LAYERS
            if (nr > 0 && nsp > 0)
            {
                if (absorb > 0.0)
                {
                    for (var i = 1; i <= nr; ++i)
                    {
                        swres[i] = swres[i] + up[1] * swres[i] / absorb;
                    label80:;
                    }
                }
            }
            //
            //**** DEFINE THE AMOUNT OF RADIATION AVAILABLE AT THE SOIL SURFACE
            direct = dir[nc + nr1 + 1];
            diffus = down[nc + nr1 + 1];
        }

        // line 5073
        private void Transc(ref int nplant, ref int nc, double[] xangle, double[] clumpng, double[] tdircc, double[] tdiffc, double[][] difkl, double[][] dirkl, double[] fbdu, double[] fddu, ref double sunslp, ref double altitu)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE DIRECT AND DIFFUSE RADIATION
            //     TRANSMISSION COEFFICIENTS AND THE SUM OF THE ATTENUATION FACTOR
            //     TIMES LEAF AREA INDEX FOR EACH CANOPY LAYER. (THIS SUM IS USED IN
            //     CALCULATING AN EFFECTIVE ALBEDO FOR THE CANOPY LAYER AND IN
            //     PROPORTIONING ABSORBED RADIATION INTO EACH OF THE PLANT SPECIES.
            //
            //***********************************************************************
            //
            //
            var dircan = new double[9];
            var difcan = new double[9];

            var aa = 0.65;
            var bb = 1.9;
            var cc = 1.46;
            var dd = 0.585;
            var ee = 0.569;
            var ff = 1.09;
            var gg = 1.585;

            var zen = 0.0;
            var difinf = 0.0;

            for (var j = 1; j <= nplant; ++j)
            {
                //        COMUPUTE ATTENUATION FACTOR BASED ON SOLAR AND LEAF ANGLE
                if (sunslp > 0.0)
                {
                    //           SUN IS ABOVE HORIZON
                    if (xangle[j] <= 0.0)
                    {
                        //              LEAF ANGLE INCLINATION IS VERTICLE -- USE SOLAR ALTITUDE
                        //              INSTEAD OF ANGLE ON LOCAL SLOPE AND USE SIMPLIFIED
                        //              EXPRESSION FOR K
                        if (altitu >= 1.57)
                        {
                            dircan[j] = 0.0;
                        }
                        else
                        {
                            dircan[j] = 1.0 / (1.5708 * Math.Tan(altitu));
                        }
                    }
                    else
                    {
                        //              LEAF ORIENTATION OTHER THAN VERTICLE -- USE ANGLE ON
                        //              LOCAL SLOPE -- COMPUTE ZENITH ANGLE
                        zen = 1.571 - sunslp;
                        dircan[j] = Math.Pow((xangle[j] * xangle[j] + Math.Pow((Math.Tan(zen)), 2)), 0.5) / (xangle[j] + 1.774 * Math.Pow((xangle[j] + 1.182), (-0.733)));
                    }
                    //           AVOID UNREALISTICALLY LARGE Kd AT VERY LOW SUN ANGLES
                    if (dircan[j] > 10.0) dircan[j] = 10.0;
                }
                else
                {
                    //           SUN IS BELOW HORIZON - JUST SET DIRCAN(J) TO 1.0
                    dircan[j] = 1.0;
                }
                //        COMPUTE Kd AT INFINITE LEAF AREA
                if (xangle[j] <= 1.0)
                {
                    difinf = Math.Atan(xangle[j]) / 1.5708;
                }
                else
                {
                    difinf = Math.Pow(xangle[j], cc) / (Math.Pow(xangle[j], cc) + 1.0);
                }
                difcan[j] = (difinf * clumpng[j] * Math.Pow(_clayrs.Totlai[j], aa) + bb) / (clumpng[j] * Math.Pow(_clayrs.Totlai[j], aa) + bb);
                //        DEFINE SCATTERING COEFF. FOR INDIVIDUAL PLANT TYPES COMPUTED
                //        FROM FLERCHINGER & YU (2007, AG & FOREST MET).  FBDU IS
                //        FRACTION OF DOWNWARD REFLECTED BEAM RADIATION SCATTERED
                //        UPWARD. FDDU IS THE SAME FOR DIFFUSE RADIATION.
                fbdu[j] = 0.5 + 0.5 * Math.Pow((Math.Atan(xangle[j]) / 1.5708), dd) * Math.Pow(xangle[j], (Math.Pow((Math.Cos(altitu)), (ee + ff * xangle[j])))) * Math.Sin(altitu);
                fddu[j] = 0.5 + 0.5 * Math.Pow((Math.Atan(xangle[j]) / 1.5708), gg);
            label5:;
            }
            //
            //**** DETERMINE THE TRANSMISSIVITY TO DIRECT AND DIFFUSE RADIATION
            //     BETWEEN CANOPY NODES
            for (var i = 1; i <= nc; ++i)
            {
                dirkl[nplant + 1][i] = 0.0;
                difkl[nplant + 1][i] = 0.0;
                //        CALCULATE PRODUCT OF ATTENUATION COEFFICIENT AND LAI
                for (var j = 1; j <= nplant; ++j)
                {
                    dirkl[j][i] = clumpng[j] * _clayrs.Canlai[j][i] * dircan[j];
                    difkl[j][i] = clumpng[j] * _clayrs.Canlai[j][i] * difcan[j];
                    dirkl[nplant + 1][i] = dirkl[nplant + 1][i] + dirkl[j][i];
                    difkl[nplant + 1][i] = difkl[nplant + 1][i] + difkl[j][i];
                label10:;
                }
                //        TRANSMISSIVITY OF LAYER IS EQUAL TO EXPONENT OF THE SUM OF
                //        ATTENUATION FACTOR TIMES LEAF AREA INDEX
                if (sunslp <= 0.0)
                {
                    //           SUN IS ON OR BELOW HORIZON OF LOCAL SLOPE -
                    //           SET DIRECT TRANSMISSIVITY TO ZERO.
                    tdircc[i] = 0.0;
                }
                else
                {
                    tdircc[i] = Math.Exp(-dirkl[nplant + 1][i]);
                }
                tdiffc[i] = Math.Exp(-difkl[nplant + 1][i]);
            label20:;
            }
        }

        // line 5166
        private void Transr(ref int nr, double[] tdirec, double[] tdiffu, double[] zr, double[] rhor, ref double angle, ref double dirres)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE DIRECT AND DIFFUSE TRANSMISSION
            //     COEFFICIENTS FOR THE RESIDUE LAYERS, GIVEN THE ANGLE BETWEEN THE
            //     SURFACE AND THE LINE OF DIRECT APPROACH (SUN-SLOPE ANGLE)
            //
            //     TRANSMISSION COEFFICIENTS ARE BASED ON THE TONNES/HA OF RESIDUE
            //***********************************************************************
            //

            var w = 0.0;

            //**** DETERMINE THE TRANSMISSIVITY TO DIRECT AND DIFFUSE RADIATION
            //     BETWEEN RESIDUE NODES BASED ON FRACTION OF SURFACE COVERED
            //     CALCULATED FROM THE WEIGHT OF RESIDUE (TON/HA)
            for (var i = 1; i <= nr; ++i)
            {
                //        DETERMINE THE WEIGHT OF THE RESIDUE AT EACH NODE
                if (i == 1)
                {
                    w = rhor[i] * (zr[2] - zr[1]) * 10.0;
                    if (nr != 1) w = w / 2.0;
                }
                else
                {
                    if (i == nr)
                    {
                        w = rhor[i] * (zr[nr + 1] - zr[nr] + (zr[nr] - zr[nr - 1]) / 2) * 5.0;
                    }
                    else
                    {
                        w = rhor[i] * (zr[i + 1] - zr[i - 1]) * 5.0;
                    }
                }
                //        NOW DETERMINE THE DIRECT AND DIFFUSE TRANMISSIVITIES
                if (angle <= 0.0)
                {
                    //           SUN IS NOT ABOVE HORIZON OF LOCAL SLOPE -- SET DIRECT
                    //           TRANSMISSIVITY TO ZERO
                    tdirec[i] = 0.0;
                }
                else
                {
                    tdirec[i] = Math.Exp(-dirres * w) * Math.Sin(angle);
                }
                tdiffu[i] = Swrcoe.Difres * Math.Exp(-dirres * w);
            label10:;
            }
        }

        // line 5209
        private void Solvrad(ref int nlayr, double[] a, double[] b, double[] c, double[] d, double[] rd, double[] ru)
        {
            //
            //     THIS SUBROUTINE SOLVES THE MATRIX OF EQUATIONS FOR RADIATION
            //     TRANSFER WITHIN THE CANOPY.  THE FORM OF THE MATRIX IS:
            //
            //     THIS SUBROUTINE SOLVES A TRI-DIAGONAL MATRIX OF THE FORM :
            //
            //              | B1  C1   0   0  .  .  .   0 | |RD1|   | D1|
            //              | A2  B2   0  C2  .  .  .   0 | |RU1|   | D2|
            //              | A3   0  B3  C3  .  .  .   0 | |RD2| = | D3|
            //              |  .   .   .   .     .  .   . | | . |   |  .|
            //              |  0   . AI1 BI1    0 CI1   0 | |RU |   |DI1|
            //              |  0   . AI+1  0 BI+1 CI+1  0 | |RD | = |DI2|
            //              |  .   .   .   .     .  .   . | | . |   |  .|
            //              |  0   0   0   0  .  . AN  BN | |RUN|   | DN|
            //
            //
            //     RD(I) IS THE DOWN FLUX ABOVE LAYER I
            //     RU(I) IS THE UPWARD FLUX ABOVE LAYER I
            //     (C1 IS NORMALLY ZERO, BUT IT DOES NOT HAVE TO BE)
            //
            //***********************************************************************

            var k = 0;
            var i = 0;
            var quot = 0.0;

            for (var n = 1; n <= nlayr; ++n)
            {
                //        ELIMINATE EACH COEFFICIENT BELOW DIAGONAL IN COLUMN K
                k = 2 * n - 1;
                i = k + 1;
                //        DETERMINE MULTIPLICATION FACTOR TO ELIMATE A(I)
                quot = a[i] / b[k];
                //        ADJUST ELEMENTS IN ROW (EQUATION) I
                b[i] = b[i] - quot * c[k];
                d[i] = d[i] - quot * d[k];
                a[i] = 0.0;
                //
                //        DETERMINE MULTIPLICATION FACTOR TO ELIMATE A(I), BUT ZERO
                //        ELEMENT IN ARRAY NOW TEMPORARILY ASSIGNED TO A(I)
                i = k + 2;
                quot = a[i] / b[k];
                //        ADJUST ELEMENTS IN ROW (EQUATION) I
                a[i] = -quot * c[k];
                d[i] = d[i] - quot * d[k];
                //
                k = k + 1;
                //        DETERMINE MULTIPLICATION FACTOR TO ELIMATE A(I)
                quot = a[i] / b[k];
                //        ADJUST ELEMENTS IN ROW (EQUATION) I
                c[i] = c[i] - quot * c[k];
                d[i] = d[i] - quot * d[k];
                a[i] = 0.0;
            //
            label40:;
            }
            //
            k = 2 * (nlayr + 1) - 1;
            i = k + 1;
            quot = a[i] / b[k];
            b[i] = b[i] - quot * c[k];
            d[i] = d[i] - quot * d[k];
            a[i] = 0.0;
            //
            //     COMPUTE UPWELLING RADIATION FROM SNOW OR SOIL
            ru[nlayr + 1] = d[2 * (nlayr + 1)] / b[2 * (nlayr + 1)];
            //
            //     Remainder of back-substitution
            for (var n = nlayr; n >= 1; --n)
            {
                i = 2 * n + 1;
                //        COMPUTE DOWNWELLING RADIATION BELOW LAYER N
                rd[n + 1] = (d[i] - c[i] * ru[n + 1]) / b[i];
                i = 2 * n;
                //        COMPUTE UPWELLING RADIATION ABOVE LAYER N
                ru[n] = (d[i] - c[i] * ru[n + 1]) / b[i];
            label60:;
            }
            rd[1] = d[1] / b[1];
        }

        // line 5288
        private void Update(ref int ns, ref int nr, ref int nsp, ref int nc, ref int nplant, ref int nsalt, int[] ices, int[] icesdt, double[] ts, double[] tsdt, double[] mat, double[] matdt, double[][] conc, double[][] concdt, double[] vlc, double[] vlcdt, double[] vic, double[] vicdt, double[][] salt, double[][] saltdt, double[] tr, double[] trdt, double[] vapr, double[] vaprdt, double[] gmc, double[] gmcdt, double[] tc, double[] tcdt, double[][] tlc, double[][] tlcdt, double[] vapc, double[] vapcdt, double[] wcan, double[] wcandt, double[] pcan, double[] pcandt, double[] tsp, double[] tspdt, double[] dlw, double[] dlwdt, int[] icesp, int[] icespt)
        {
            //
            //     THIS SUBROUTINE UPDATES THE BEGINNING OF TIME STEP VALUES FOR THE
            //     NEW TIME STEP.
            //
            //***********************************************************************

            //     SOIL PROPERTIES
            for (var i = 1; i <= ns; ++i)
            {
                ts[i] = tsdt[i];
                mat[i] = matdt[i];
                vlc[i] = vlcdt[i];
                vic[i] = vicdt[i];
                ices[i] = icesdt[i];
                for (var j = 1; j <= nsalt; ++j)
                {
                    salt[j][i] = saltdt[j][i];
                    conc[j][i] = concdt[j][i];
                label10:;
                }
            label15:;
            }
            //
            //     RESIDUE PROPERTIES
            for (var i = 1; i <= nr; ++i)
            {
                tr[i] = trdt[i];
                vapr[i] = vaprdt[i];
                gmc[i] = gmcdt[i];
            label20:;
            }
            //
            //     SNOWPACK PROPERTIES
            for (var i = 1; i <= nsp; ++i)
            {
                icesp[i] = icespt[i];
                tsp[i] = tspdt[i];
                dlw[i] = dlwdt[i];
            label30:;
            }
            //
            //     CANOPY PROPERTIES
            for (var i = 1; i <= nc; ++i)
            {
                tc[i] = tcdt[i];
                vapc[i] = vapcdt[i];
                wcan[i] = wcandt[i];
                for (var j = 1; j <= nplant; ++j)
                {
                    tlc[j][i] = tlcdt[j][i];
                label35:;
                }
            label40:;
            }
            for (var j = 1; j <= nplant; ++j)
            {
                pcan[j] = pcandt[j];
            label45:;
            }
        }

        // line 5351
        private void Lwrbal(ref int nc, ref int nsp, ref int nr, ref int nplant, ref double ta, ref double tadt, double[][] tlc, double[][] tlcdt, double[] tsp, double[] tspdt, double[] tr, double[] trdt, double[] ts, double[] tsdt, ref double vapa, ref double vapadt, ref double clouds, double[][] lwcan, ref double lwsnow, double[] lwres, ref double lwsoil, double[] lwdown, double[] lwup)
        {
            //
            //     THIS SUBROUTINE CALLS SUBROUTINES TO SET UP THE LONG-WAVE
            //     RADIATION BALANCE FOR SYSTEM.
            //
            //***********************************************************************
            //

            //**** DETERMINE THE LONGWAVE RADIATION FLUX ABOVE EACH NODE.  START WITH
            //     ATMOSPHERE AND WORK DOWNWARDS.
            //
            var above = 0.0;
            var tk = 0.0;
            var emitsfc = 0.0;

            Lwratm(ref above, ref clouds, ref ta, ref tadt, ref vapa, ref vapadt);
            lwdown[1] = above;
            //
            if (nsp > 0)
            {
                //        SURFACE IS SNOW
                tk = tsp[1] + 273.16;
                emitsfc = Lwrcof.Emitsp * Lwrcof.Stefan * (Math.Pow(tk, 4) + 4.0 * _timewt.Wdt * (Math.Pow(tk, 3)) * (tspdt[1] - tsp[1]));
            }
            else
            {
                //        SURFACE IS SOIL
                tk = ts[1] + 273.16;
                emitsfc = Lwrcof.Emits * Lwrcof.Stefan * ((Math.Pow(tk, 4)) + 4.0 * _timewt.Wdt * (Math.Pow(tk, 3)) * (tsdt[1] - ts[1]));
            }
            //
            if (nc > 0)
            {
                for (var i = 1; i <= nc; ++i)
                {
                    for (var j = 1; j <= nplant; ++j)
                    {
                        //             save temperatures used to compute canopy lwr balance to be
                        //             used in LEAFT
                        _canlwr.Tlclwr[j][i] = tlcdt[j][i];
                    label20:;
                    }
                label25:;
                }
                if (nsp == 0 && nr > 0)
                {
                    //           RESIDUE LIES BENEATH CANOPY - DETERMINE LONGWAVE RADIATION
                    //           BALANCE THROUGH CANOPY AND RESIDUE TOGETHER
                    Lwrcan(ref nplant, ref nc, ref nsp, ref nr, tlc, tlcdt, tr, trdt, ref above, ref emitsfc, lwcan, lwres, lwdown, lwup);
                }
                else
                {
                    //           CALCULATE RADIATION THROUGH CANOPY DOWN TO SNOW OR SOIL
                    var int0 = 0;
                    Lwrcan(ref nplant, ref nc, ref nsp, ref int0, tlc, tlcdt, tr, trdt, ref above, ref emitsfc, lwcan, lwres, lwdown, lwup);
                }
            }
            //
            if (nsp > 0)
            {
                //        RADIATION BALANCE FOR SNOW PACK
                lwsnow = Lwrcof.Emitsp * above - emitsfc;
                //        NO LWR BENEATH THE SNOW
                lwsoil = 0.0;
                for (var i = 1; i <= nr; ++i)
                {
                    lwres[i] = 0.0;
                label30:;
                }
                //        SET UPWARD LONG-WAVE FROM SNOW
                lwup[nc + 1] = (1.0 - Lwrcof.Emitsp) * above + emitsfc;
            }
            else
            {
                //
                //        RADIATION BALANCE OF RESIDUE AND SOIL
                if (nr > 0)
                {
                    //           CHECK IF ALREADY COMPUTED WITH CANOPY RADIATION BALANCE
                    if (nc == 0)
                    {
                        //              RADIATION BALANCE FOR THE RESIDUE LAYERS WITH NO CANOPY
                        var int0 = 0;
                        Lwrcan(ref nplant, ref int0, ref nsp, ref nr, tlc, tlcdt, tr, trdt, ref above, ref emitsfc, lwcan, lwres, lwdown, lwup);
                    }
                }
                //        RADIATION BALANCE OF SOIL
                lwsoil = Lwrcof.Emits * above - emitsfc;
                //        SET UPWARD LONG-WAVE ABOVE SYSTEM IF FROM SOIL
                if (nc == 0 && nr == 0) lwup[1] = (1.0 - Lwrcof.Emits) * above + emitsfc;
            }
        }

        // line 5438
        private void Lwrcan(ref int nplant, ref int nc, ref int nsp, ref int nr, double[][] tlc, double[][] tlcdt, double[] tr, double[] trdt, ref double above, ref double emitsfc, double[][] lwcan, double[] lwres, double[] lwdown, double[] lwup)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE AMOUNT OF LONGWAVE RADIATION
            //     ABSORBED BY EACH CANOPY AND RESIDUE NODE
            //
            //
            //***********************************************************************
            var a = new double[43];
            var b = new double[43];
            var c = new double[43];
            var d = new double[43];
            var rd = new double[22];
            var ru = new double[22];
            var ddu = new double[11];

            var n = 0;
            var tk = 0.0;
            var emit = 0.0;
            var fractn = 0.0;
            var nlayr = 0;
            var absorb = 0.0;

            //     INITIALIZE MATRIX ELEMENTS
            n = 1;
            a[1] = 0.0;
            b[1] = 1.0;
            c[1] = 0.0;
            d[1] = above;
            //
            for (var i = 1; i <= nc; ++i)
            {
                n = n + 1;
                d[n] = 0.0;
                ddu[i] = 0.0;
                //        COMPUTE RADIATION EMITTED BY EACH PLANT WITH LAYER AND TOTAL
                //        EMITTED BY ENTIRE LAYER
                for (var j = 1; j <= nplant; ++j)
                {
                    tk = tlc[j][i] + 273.16;
                    emit = Lwrcof.Emitc * Lwrcof.Stefan * (Math.Pow(tk, 4) + 4.0 * (Math.Pow(tk, 3)) * (tlcdt[j][i] - tlc[j][i]));
                    //           COMPUTE TOTAL RADIATION EMITTED FROM BOTH SIDES OF LAYER
                    //           BASED ON FRACTION OF TRANSMISSIVITY OF LAYER FOR PLANT
                    fractn = (1.0 - _radcan.Tdiffc[i]) * _radcan.Difkl[j][i] / _radcan.Difkl[nplant + 1][i];
                    lwcan[j][i] = -2.0 * fractn * emit;
                    d[n] = d[n] + fractn * emit;
                    //           WEIGHT THE FRACTION OF UPWARD SCATTERED RADIATION
                    ddu[i] = ddu[i] + _radcan.Fddu[j] * _radcan.Difkl[j][i];
                label10:;
                }
                ddu[i] = ddu[i] / _radcan.Difkl[nplant + 1][i];
                //
                //        COMPUTE COEFFICIENTS FOR UPWELLING RADIATION ABOVE LAYER
                a[n] = -ddu[i] * (1.0 - Lwrcof.Emitc) * (1.0 - _radcan.Tdiffc[i]);
                b[n] = 1.0;
                c[n] = -(_radcan.Tdiffc[i] + (1.0 - ddu[i]) * (1.0 - Lwrcof.Emitc) * (1.0 - _radcan.Tdiffc[i]));
                //        COMPUTE COEFFICIENTS FOR DOWNWELLING RADIATION BELOW LAYER
                n = n + 1;
                a[n] = c[n - 1];
                b[n] = 1.0;
                c[n] = a[n - 1];
                d[n] = d[n - 1];
            label20:;
            }
            //
            for (var i = 1; i <= nr; ++i)
            {
                n = n + 1;
                tk = tr[i] + 273.16;
                emit = (1.0 - _radres.Tdiffu[i]) * Lwrcof.Emitr * Lwrcof.Stefan * (Math.Pow(tk, 4) + 4.0 * _timewt.Wdt * (Math.Pow(tk, 3)) * (trdt[i] - tr[i]));
                lwres[i] = -2.0 * emit;
                //        COMPUTE COEFFICIENTS FOR UPWELLING RADIATION ABOVE LAYER
                //        HORIZONTAL LEAF ORIENTATION IS ASSUMED FOR SCATTERING.
                //        THEREFORE ONLY BACKSCATTERING; NO FORWARD SCATTERING
                a[n] = -(1.0 - Lwrcof.Emitr) * (1.0 - _radres.Tdiffu[i]);
                b[n] = 1.0;
                c[n] = -_radres.Tdiffu[i];
                d[n] = emit;
                //        COMPUTE COEFFICIENTS FOR DOWNWELLING RADIATION BELOW LAYER
                n = n + 1;
                a[n] = c[n - 1];
                b[n] = 1.0;
                c[n] = a[n - 1];
                d[n] = d[n - 1];
            label30:;
            }
            //
            //     COMPUTE MATRIX ELEMENTS FOR LOWER SURFACE BOUNDARY
            n = n + 1;
            b[n] = 1.0;
            c[n] = 0.0;
            if (nsp > 0)
            {
                a[n] = -(1.0 - Lwrcof.Emitsp);
            }
            else
            {
                a[n] = -(1.0 - Lwrcof.Emits);
            }
            d[n] = emitsfc;
            //
            //     CALL MATRIX SOLVER
            nlayr = nc + nr;
            Solvrad(ref nlayr, a, b, c, d, rd, ru);
            //
            n = 0;
            for (var i = 1; i <= nc; ++i)
            {
                n = n + 1;
                lwcan[nplant + 1][i] = 0.0;
                //        COMPUTE THE TOTAL RADIATION ABSORBED BY CANOPY LAYER
                absorb = (1.0 - _radcan.Tdiffc[i]) * Lwrcof.Emitc * (rd[n] + ru[n + 1]);
                for (var j = 1; j <= nplant; ++j)
                {
                    //           ADD THE FRACTION OF TOTAL RADIATION ABSORBED BY EACH PLANT
                    //           TO THAT EMITTED BY EACH PLANT
                    lwcan[j][i] = lwcan[j][i] + absorb * _radcan.Difkl[j][i] / _radcan.Difkl[nplant + 1][i];
                    lwcan[nplant + 1][i] = lwcan[nplant + 1][i] + lwcan[j][i];
                label35:;
                }
                //        SAVE FLUXES BETWEEN NODES
                lwdown[i] = rd[i];
                lwup[i] = ru[i];
            label40:;
            }
            if (nc > 0)
            {
                lwdown[nc + 1] = rd[nc + 1];
                lwup[nc + 1] = ru[nc + 1];
            }
            else
            {
                //        UPWARD LONG-WAVE AT SURFACE COMES FROM RESIDUE
                if (nsp == 0) lwup[1] = ru[1];
            }
            //
            for (var i = 1; i <= nr; ++i)
            {
                n = n + 1;
                lwres[i] = lwres[i] + (1.0 - _radres.Tdiffu[i]) * Lwrcof.Emitr * (rd[n] + ru[n + 1]);
            label50:;
            }
            //
            above = rd[n + 1];
        }

        // line 5565
        private void Lwratm(ref double above, ref double clouds, ref double ta, ref double tadt, ref double vapa, ref double vapadt)
        {
            //
            //     THIS SUBROUTINE DETERMINES THE LONG-WAVE RADIATIVE FLUX FROM THE
            //     ATMOSPHERE.
            //
            //***********************************************************************

            //**** DETERMINE THE EMISSIVITY OF CLEAR-SKY ATMOSPHERE BASED ON
            //     DILLEY & O'BRIEN (1998), QJR MET SOC.
            var avgtmp = _timewt.Wt * ta + _timewt.Wdt * tadt;
            var avgvap = _timewt.Wt * vapa + _timewt.Wdt * vapadt;
            //     CONVERT TO VAPOR PRESSURE (kPa)
            avgvap = 0.4619 * avgvap * (avgtmp + 273.16);
            var clear = 59.38 + 113.7 * Math.Pow(((avgtmp + 273.16) / 273.16), 6) + 96.96 * Math.Sqrt((465.0 * avgvap / (avgtmp + 273.16)) / 2.5);
            var black = Lwrcof.Stefan * Math.Pow((avgtmp + 273.16), 4);
            //     BACK-CALCULATE EMISSIVITY FROM CLEAR-SKY LONG-WAVE RADIATION
            var emitat = clear / black;
            //
            //**** ADJUST CLEAR-SKY EMISSIVITY FOR FRACION OF CLOUD COVER
            //     USE EQUATION BY UNSWORTH & MONTEITH (1975) QJR MET. SOC.
            //     (GIVEN IN CAMPBELL, 1985, SOIL PHYSICS WITH BASIC)
            emitat = (1.0 - 0.84 * clouds) * emitat + 0.84 * clouds;
            //
            //**** NOW CALCULATE THE LONG-WAVE RADIATIVE FLUX
            above = emitat * black;
        }

        // line 5599
        /// <summary>
        ///   <para>Sets: A1, B1, C1, D1 over ranges for nc+nsp+nr.</para>
        ///   <para>Not used: tc.</para>
        /// </summary>
        private void Lwrmat(ref int nc, ref int nsp, ref int nr, double[] tc, double[] tsp, double[] tr, double[] ts, int[] icespt)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE CONTRIBUTION TO THE ENERGY BALANCE
            //     MATRIX BY LONG-WAVE RADIATION.
            //
            //***********************************************************************

            var emitnc = 0.0;
            var tsp3 = 0.0;
            var tr3 = 0.0;
            var ts3 = 0.0;

            var n = 1;
            var materl = 2;

            _matrix.B1[n] = 0.0;
            //
            //     RADIATION BALANCE MATRIX COEFFICIENTS FOR CANOPY NODES
            if (nc > 0)
            {
                emitnc = (1.0 - _radcan.Tdiffc[1]) * Lwrcof.Emitc;
                //        TC3=STEFAN*4.0*WDT*(TC(1)+273.16)**3
                //        B1(N) =-2.0*EMITNC*TC3
                //        C1(N) = EMITNC
                //        A1(N+1) = EMITNC*TC3
                _matrix.B1[n] = 0.0;
                _matrix.C1[n] = 0.0;
                _matrix.A1[n + 1] = 0.0;
                n = n + 1;
                //
                for (var i = 2; i <= nc; ++i)
                {
                    emitnc = (1.0 - _radcan.Tdiffc[i]) * Lwrcof.Emitc;
                    //           TC3=STEFAN*4.0*WDT*(TC(I)+273.16)**3
                    //           C1(N-1)=C1(N-1)*EMITNC*TC3
                    //           A1(N) = A1(N)*EMITNC
                    //           B1(N) = -2.0*EMITNC*TC3
                    //           C1(N)= EMITNC
                    //           A1(N+1) = EMITNC*TC3
                    _matrix.C1[n - 1] = 0.0;
                    _matrix.A1[n] = 0.0;
                    _matrix.B1[n] = 0.0;
                    _matrix.C1[n] = 0.0;
                    _matrix.A1[n + 1] = 0.0;
                    n = n + 1;
                label10:;
                }
                materl = materl + 1;
            }
            //
            //     RADIATION BALANCE MATRIX COEFFICIENTS FOR SNOW PACK
            if (nsp > 0)
            {
                if (icespt[1] > 0)
                {
                    //           TEMP IS KNOWN - ENERGY BALANCE BASED ON LIQUID CONTENT
                    if (materl > 2)
                    {
                        _matrix.A1[n] = _matrix.A1[n] * Lwrcof.Emitsp;
                        _matrix.C1[n - 1] = 0.0;
                    }
                    _matrix.B1[n] = 0.0;
                }
                else
                {
                    //           ENERGY BALANCE BASED ON TEMPERATURE
                    tsp3 = Lwrcof.Stefan * 4.0 * _timewt.Wdt * Math.Pow((tsp[1] + 273.16), 3);
                    if (materl > 2)
                    {
                        //              ADJUST COEFFICIENTS FOR MATERIAL ABOVE SNOWPACK
                        _matrix.A1[n] = _matrix.A1[n] * Lwrcof.Emitsp;
                        _matrix.C1[n - 1] = _matrix.C1[n - 1] * Lwrcof.Emitsp * tsp3;
                    }
                    _matrix.B1[n] = -Lwrcof.Emitsp * tsp3;
                }
                n = n + nsp;
                materl = materl + 1;
                //        INITIALIZE COEFFICIENTS FOR BOTTOM NODE AND UNDERLYING MATERIAL
                if (nsp > 1) _matrix.B1[n - 1] = 0.0;
                _matrix.C1[n - 1] = 0.0;
                for (var i = 1; i <= nr + 1; ++i)
                {
                    //           SNOW OVERLYING RESIDUE AND SOIL - INITIALIZE MATRIX COEFF.
                    _matrix.A1[n] = 0.0;
                    _matrix.B1[n] = 0.0;
                    _matrix.C1[n] = 0.0;
                    n = n + 1;
                label5:;
                }
                goto label50;
            }
            //
            //     RADIATION BALANCE MATRIX COEFFICIENTS FOR RESIDUE NODES
            //
            if (nr > 0)
            {
                emitnc = (1 - _radres.Tdiffu[1]) * Lwrcof.Emitr;
                tr3 = Lwrcof.Stefan * 4.0 * _timewt.Wdt * Math.Pow((tr[1] + 273.16), 3);
                if (materl > 2)
                {
                    //           ADJUST COEFFICIENTS FOR MATERIAL ABOVE RESIDUE
                    _matrix.A1[n] = _matrix.A1[n] * emitnc;
                    _matrix.C1[n - 1] = _matrix.C1[n - 1] * emitnc * tr3;
                }
                _matrix.B1[n] = -2.0 * emitnc * tr3;
                _matrix.C1[n] = emitnc;
                _matrix.A1[n + 1] = emitnc * tr3;
                n = n + 1;
                //
                for (var i = 2; i <= nr; ++i)
                {
                    emitnc = (1 - _radres.Tdiffu[i]) * Lwrcof.Emitr;
                    tr3 = Lwrcof.Stefan * 4.0 * _timewt.Wdt * Math.Pow((tr[i] + 273.16), 3);
                    _matrix.C1[n - 1] = _matrix.C1[n - 1] * emitnc * tr3;
                    _matrix.A1[n] = _matrix.A1[n] * emitnc;
                    _matrix.B1[n] = -2.0 * emitnc * tr3;
                    _matrix.C1[n] = emitnc;
                    _matrix.A1[n + 1] = emitnc * tr3;
                    n = n + 1;
                label20:;
                }
                materl = materl + 1;
            }
            //
            //     RADIATION BALANCE MATRIX COEFFICIENTS FOR SOIL SURFACE
            ts3 = Lwrcof.Stefan * 4.0 * _timewt.Wdt * Math.Pow((ts[1] + 273.16), 3);
            if (materl > 2)
            {
                //        ADJUST COEFFICIENTS FOR MATERIAL ABOVE SOIL
                _matrix.A1[n] = _matrix.A1[n] * Lwrcof.Emits;
                _matrix.C1[n - 1] = _matrix.C1[n - 1] * Lwrcof.Emits * ts3;
            }
            _matrix.B1[n] = -Lwrcof.Emits * ts3;
        label50:;
        }

        // line 5725
        private void Source(ref int nc, ref int nsp, ref int nr, ref int ns, ref int nsalt, ref int nplant, double[] uc, double[] sc, double[] usp, double[] ssp, double[] ur, double[] sr, double[] us, double[] ss, double[][] sink, double[][] swcan, double[] swsnow, double[] swres, ref double swsoil, double[][] lwcan, ref double lwsnow, double[] lwres, ref double lwsoil, double[] soilxt)
        {
            //
            //     THIS SUBROUTINE SUMS UP THE SOURCE-SINK TERMS FOR EACH NODE.
            //
            //***********************************************************************

            //**** CANOPY NODES
            for (var i = 1; i <= nc; ++i)
            {
                uc[i] = 0.0;
                sc[i] = swcan[nplant + 1][i] + lwcan[nplant + 1][i];
            label10:;
            }
            //
            //**** SNOWPACK NODES
            if (nsp > 0)
            {
                for (var i = 1; i <= nsp; ++i)
                {
                    usp[i] = 0.0;
                    ssp[i] = swsnow[i];
                label20:;
                }
                ssp[1] = ssp[1] + lwsnow;
            }
            //
            //**** RESIDUE NODES
            for (var i = 1; i <= nr; ++i)
            {
                ur[i] = 0.0;
                sr[i] = swres[i] + lwres[i];
            label30:;
            }
            //**** SOIL NODES
            for (var i = 1; i <= ns; ++i)
            {
                us[i] = -soilxt[i];
                ss[i] = 0.0;
                for (var j = 1; j <= nsalt; ++j)
                {
                    sink[j][i] = 0.0;
                label40:;
                }
            label50:;
            }
            ss[1] = ss[1] + swsoil + lwsoil;
        }

        // line 5771
        private void Atstab(ref int nplant, ref int nc, ref int nsp, ref int nr, ref double ta, ref double tadt, ref double t, ref double tdt, ref double vapa, ref double vapadt, ref double vap, ref double vapdt, ref double wind, ref double height, ref double hflux, ref double vflux, double[] zc, double[] zr, double[] rhor, ref int ice, ref int iter)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE ATMOSPHERIC STABILITY, THE TRANSFER
            //     COEFFICIENTS FOR THE BOTH HEAT AND VAPOR FROM THE SURFACE,
            //     DEFINES THE BOUNDARY CONDITIONS FOR THE MATRIX SOLUTIONS, AND
            //     CALCULATES THE WINDSPEED PROFILES IN THE RESIDUE AND CANOPY
            //
            //***********************************************************************
            //xOUT
            //
            //xx   SAVE TMPAIR,VAPAIR,ZMLOG,ZHLOG,ZCLOG,PSIM,PSIH

            var tmpsfc = 0.0;
            var vapsfc = 0.0;
            var hmin = 0.0;
            var con = 0.0;
            var rhzc = 0.0;
            var rv = 0.0;
            var dv = 0.0;
            var vmin = 0.0;
            var conv = 0.0;
            var windrh = 0.0;
            var sumlai = 0.0;
            var awind = 0.0;
            var suml = 0.0;
            var windexp = 0.0;
            var hnode1 = 0.0;
            var hnode2 = 0.0;
            var zmcan1 = 0.0;
            var zmcan2 = 0.0;
            var wstar = 0.0;
            var windlog = 0.0;
            var s = 0.0;
            var dummy = 0.0;

            if (iter <= 2)
            {
                if (iter <= 1)
                {
                    _atstabSave.Tmpair = _timewt.Wt * ta + _timewt.Wdt * tadt;
                    _atstabSave.Vapair = _timewt.Wt * vapa + _timewt.Wdt * vapadt;
                    _atstabSave.Zmlog = Math.Log((height + _windv.Zm - _windv.Zero) / _windv.Zm);
                    _windv.Zhlog = Math.Log((height + _windv.Zh - _windv.Zero) / _windv.Zh);
                    if (nc > 0) _atstabSave.Zclog = Math.Log((zc[nc + 1] - _windv.Zero + _windv.Zh) / _windv.Zh);
                }
                //****    DEFINE INITIAL ASSUMPTIONS FOR CURRENT TIME STEP
                hflux = 0.0;
                _atstabSave.Psim = 0.0;
                _atstabSave.Psih = 0.0;
            }
            //
            tmpsfc = _timewt.Wt * t + _timewt.Wdt * tdt;
            vapsfc = _timewt.Wt * vap + _timewt.Wdt * vapdt;
            //
            if (wind <= 0.0)
            {
                //        FLUXES WILL BE SET TO DIFFUSIVITY VALUES BELOW
                hflux = 0.0;
                vflux = 0.0;
                _windv.Stable = 0.0;
                _windv.Ustar = 0.0;
            }
            else
            {
                Stab(ref iter, ref nc, ref zc[nc + 1], ref _atstabSave.Tmpair, ref tmpsfc, ref wind, ref height, ref _atstabSave.Zmlog, ref _windv.Zhlog, ref _atstabSave.Zclog, ref hflux, ref _windv.Ustar, ref _windv.Stable, ref _atstabSave.Psim, ref _atstabSave.Psih, ref _writeit.Rh, ref _windv.Zero);
            }
            //
            //**** COMPARE FLUX WITH MINIMUM FLUX AS CALCULATED FROM THERMAL
            //     CONDUCTIVITY AND VAPOR DIFFUSIVITY OF STILL AIR -- HEAT FLUX FIRST
            if (nc > 0)
            {
                hmin = Constn.Tka * (_atstabSave.Tmpair - tmpsfc) / (height - zc[nc + 1]);
            }
            else
            {
                hmin = Constn.Tka * (_atstabSave.Tmpair - tmpsfc) / (height - _windv.Zero);
            }

            if (Math.Abs(hmin) <= Math.Abs(hflux))
            {
                //        WIND IS SUFFICIENT THAT HEAT FLUX IS ENHANCED
                if (nc > 0)
                {
                    //           RH IS TO ZERO PLANE DISPLACEMENT - ADJUST TO TOP OF CANOPY
                    rhzc = _writeit.Rh * (1.0 - (_atstabSave.Zclog / _windv.Zhlog));
                    con = Constn.Rhoa * Constn.Ca / rhzc;
                }
                else
                {
                    con = Constn.Rhoa * Constn.Ca / _writeit.Rh;
                }
            }
            else
            {
                //        WIND IS SUFFICIENTLY LOW TO BE CONSIDERED AS "STILL AIR"
                hflux = hmin;
                if (nc > 0)
                {
                    con = Constn.Tka / (height - zc[nc + 1]);
                }
                else
                {
                    con = Constn.Tka / (height - _windv.Zero);
                }
            }
            _windv.Conrh = 1.0 / _writeit.Rh;
            //xout
            //     writeit
            _writeit.Hflux1 = hflux;
            //
            //**** NOW COMPARE VAPOR FLUXES
            //**** CALCULATE VAPOR FLUX
            rv = _writeit.Rh;
            if (nc > 0) rv = rhzc;
            vflux = (_atstabSave.Vapair - vapsfc) / rv;
            dv = Constn.Vdiff * (Math.Pow(((_atstabSave.Tmpair + 273.16) / 273.16), 2)) * (Constn.P0 / _constn.Presur);
            if (nc == 0)
            {
                vmin = dv * (_atstabSave.Vapair - vapsfc) / (height - _windv.Zero);
            }
            else
            {
                vmin = dv * (_atstabSave.Vapair - vapsfc) / (height - zc[nc + 1]);
            }
            if (Math.Abs(vmin) <= Math.Abs(vflux))
            {
                //        WIND IS SUFFICIENT THAT VAPOR FLUX IS ENHANCED
                conv = 1.0 / rv;
            }
            else
            {
                //        WIND IS SUFFICIENTLY LOW TO BE CONSIDERED AS "STILL AIR"
                vflux = vmin;
                conv = dv;
            }
            //
            //**** DEFINE MATRIX COEFFICIENTS FOR SURFACE MATERIAL PRESENT
            //
            if (nc > 0)
            {
                //****    SURFACE MATERIAL IS CANOPY
                _matrix.B1[1] = _matrix.B1[1] - con;
                _matrix.D1[1] = hflux;
                _matrix.B2[1] = -conv;
                _matrix.D2[1] = vflux;
                _windv.Windc[1] = _windv.Ustar * Math.Log((zc[nc + 1] + _windv.Zm - _windv.Zero) / _windv.Zm) / Constn.Vonkrm;
                //        CALCULATE WINDSPEED AT THE CANOPY NODES ASSUMING AN
                //        EXPONENTIAL DECREASE IN WINDSPEED WITH DEPTH
                //        AWIND = 0  FOR SPARSE CANOPY: AWIND >= 4 FOR DENSE CANOPY
                sumlai = 0.0;
                for (var j = 1; j <= nplant; ++j)
                {
                    sumlai = sumlai + _clayrs.Totlai[j];
                label40:;
                }
                //        USE EXTINCTION COEFF FROM NIKOLOV & ZELLER (2003) ENVIRON.POLL.
                awind = 2.879 * (1 - Math.Exp(-sumlai));
                //xxxx    AWIND = sqrt(SUMLAI)
                for (var i = 2; i <= nc + 1; ++i)
                {
                    //           COMPUTE WIND BASED ON EXPONENTIAL DECREASE WITH LAI
                    suml = 0.0;
                    for (var j = 1; j <= nplant; ++j)
                    {
                        suml = suml + _clayrs.Canlai[j][i - 1];
                    label42:;
                    }
                    //           u(L) = uh*EXP(-AWIND*L/Ltot)  L = Total LAI above layer
                    //           ==>  u(L+dL) = u(L)*exp(AWIND*dL/Ltot)
                    windexp = _windv.Windc[i - 1] * Math.Exp(-awind * suml / sumlai);
                    if (i < nc + 1)
                    {
                        //             COMPUTE WIND BASED ON LOGARITHMIC WIND PROFILE
                        hnode1 = zc[nc + 1] - zc[i - 1];
                        hnode2 = zc[nc + 1] - zc[i];
                        zmcan1 = Math.Log((hnode1 + _windv.Zmsub - _windv.Zersub) / _windv.Zmsub);
                        zmcan2 = Math.Log((hnode2 + _windv.Zmsub - _windv.Zersub) / _windv.Zmsub);
                        wstar = _windv.Windc[i - 1] * Constn.Vonkrm / zmcan1;
                        windlog = wstar * zmcan2 / Constn.Vonkrm;
                        //             WIND AT NODE IS MINIMUM OF THE TWO APPROACHES
                        //             (THIS RESOLVES PROBLEM WITH LITTLE OR NO LAI IN A LAYER)
                        _windv.Windc[i] = Math.Min(windlog, windexp);
                    }
                    else
                    {
                        //             WIND AT TOP OF RESIDUE CANNOT BE ZERO
                        _windv.Windc[i] = windexp;
                    }
                label45:;
                }
                if (nr > 0 && nsp == 0)
                {
                    //           RESIDUE LIES BENEATH THE CANOPY -- CALCULATE WINDRH
                    windrh = _windv.Windc[nc + 1];
                }
                else
                {
                    //           NO RESIDUE LIES BENEATH CANOPY -- PERHAPS IT IS SNOW-COVERED
                    windrh = 0.0;
                }
            }
            else
            {
                //
                if (nsp > 0)
                {
                    //****    SURFACE MATERIAL IS SNOW
                    Vslope(ref s, ref dummy, ref tdt);
                    if (ice == 0)
                    {
                        //           SNOW IS NOT MELTING - ENERGY BALANCE BASED ON TEMPERATURE
                        _matrix.B1[1] = _matrix.B1[1] - _timewt.Wdt * con - _timewt.Wdt * Constn.Ls * s * conv;
                    }
                    else
                    {
                        //           SNOW IS MELTING - ENERGY BALANCE BASED ON LIQUID WATER
                        _matrix.B1[1] = _matrix.B1[1];
                    }
                    _matrix.D1[1] = hflux + Constn.Ls * vflux;
                    _matrix.B2[1] = 0.0;
                    _matrix.D2[1] = 0.0;
                    //        IF RESIDUE LIES BENEATH THE SNOW, WINDRH = 0.0
                    windrh = 0.0;
                }
                else
                {
                    //
                    if (nr > 0)
                    {
                        //****    SURFACE MATERIAL IS RESIDUE
                        _matrix.B1[1] = _matrix.B1[1] - _timewt.Wdt * con;
                        _matrix.D1[1] = hflux;
                        _matrix.B2[1] = -_timewt.Wdt * conv;
                        _matrix.D2[1] = vflux;
                        //        CALCULATE WINDSPEED AT THE TOP OF THE RESIDUE LAYER
                        windrh = _windv.Ustar * Math.Log((zr[nr + 1] + _windv.Zm - _windv.Zero) / _windv.Zm) / Constn.Vonkrm;
                    }
                    else
                    {
                        //
                        //**** SURFACE MATERIAL IS BARE SOIL
                        Vslope(ref s, ref dummy, ref tdt);
                        _matrix.D1[1] = hflux + Constn.Lv * vflux;
                        _matrix.D2[1] = vflux / Constn.Rhol;
                        //        CHECK IF ICE IS PRESENT -- IF SO, THE WATER BALANCE IS WRITTEN
                        //        IN TERMS OF ICE CONTENT.  THE ENERGY BALANCE IS WRITTEN WITH
                        //        LIQUID WATER AS THE REFERENCE STATE, THEREFORE LATENT HEAT OF
                        //        SUBLIMATION IS NEVER USED -- ONLY HEAT OF VAPORIZATION.
                        if (ice == 0)
                        {
                            //           NO ICE IS PRESENT AT SOIL SURFACE
                            _matrix.B1[1] = _matrix.B1[1] - _timewt.Wdt * con - _timewt.Wdt * Constn.Lv * s * conv;
                            _matrix.B2[1] = -_timewt.Wdt * vapdt * conv * 0.018 * Constn.G / (Constn.Ugas * (tdt + 273.16)) / Constn.Rhol;
                        }
                        else
                        {
                            //           ICE IS PRESENT AT SOIL SURFACE
                            _matrix.B1[1] = _matrix.B1[1];
                            _matrix.B2[1] = 0.0;
                        }
                        //
                    }
                }
            }
            //
            if (nr > 0)
            {
                //        CALCULATE THE WINDSPEED AT THE MID-POINT OF EACH RESIDUE
                //        LAYER ASSUMING AN EXPONENTIAL DECREASE WITH DEPTH
                //        AWIND = 0  FOR SPARSE RESIDUE: AWIND >= 4 FOR DENSE RESIDUE
                awind = 0.1 * (rhor[1] - 20.0);
                if (awind < 0.0) awind = 0.0;
                _windv.Windr[1] = windrh * Math.Exp(-awind * (zr[1] + zr[2]) / 4 / zr[nr + 1]);
                for (var i = 2; i <= nr + 1; ++i)
                {
                    _windv.Windr[i] = windrh * Math.Exp(-awind * zr[i] / zr[nr + 1]);
                label65:;
                }
            }
        }

        // line 5999
        private void Ebcan(ref int n, ref int nplant, ref int nc, ref int nsp, ref int nr, ref int ns, double[] zc, double[] tc, double[] tcdt, double[][] tlc, double[][] tlcdt,
            double[] vapc, double[] vapcdt, double[] wcan, double[] wcandt, double[] pcan, double[] pcandt, double[] mat, double[] matdt, double[][] swcan, double[][] lwcan, double[] swdown,
            ref double canma, ref double canmb, double[] dchar, double[] rstom0, double[] rstexp, double[] pleaf0, double[] rleaf0, int[] itype, ref int istomate, double[][] stomate, ref int iter)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE JACOBIAN MATRIX COEFFICIENTS FOR
            //     THE CANOPY PORTION OF THE NEWTON-RAPHSON SOLUTION OF THE ENERGY
            //     BALANCE
            //
            //***********************************************************************
            //xOUT
            //
            var con = new double[11];
            var condt = new double[11];
            var trnsp = new double[10];
            var xtract = new double[51];
            var heatc = new double[11];
            var etlyr = new double[11];
            var detlyr = new double[11];
            var dheatc = new double[11];
            var dtldtc = new double[11];

            var j = 0;
            var conv = 0.0;
            var qvcan = 0.0;
            var slpnc = 0.0;
            var slpnc1 = 0.0;
            var humnc = 0.0;
            var humnc1 = 0.0;
            var satv = 0.0;

            //**** CALCULATE LEAF TEMPERATURE AND HEAT TRANSFER FROM CANOPY
            Leaft(ref nplant, ref nc, ref ns, ref iter, itype, tc, tcdt, tlc, tlcdt, vapc, vapcdt, wcan, wcandt, pcan, pcandt, mat, matdt, trnsp, xtract, swcan, lwcan, swdown, heatc, etlyr, detlyr, ref canma, ref canmb, dchar, rstom0, rstexp, pleaf0, rleaf0, ref istomate, stomate, dheatc, dtldtc);
            //
            //**** DETERMINE THE EDDY CONDUCTANCE TERM BETWEEN NODES
            Cantk(ref iter, ref nc, con, tcdt, zc);
            //
            //xOUT
            //XXX  if (nc .lt. 3) then
            _writeit.Contk = con[nc];
            //XXX   else
            //XXX    contk = con(3)
            //XOUT end if
            //
            //**** DETERMINE THE MATRIX COEFFICIENTS FOR THE TOP LAYER
            //
            _matrix.A1[n + 1] = _matrix.A1[n + 1] + con[1];
            _matrix.B1[n] = _matrix.B1[n] - (con[1] + dheatc[1] * (1.0 - dtldtc[1])) - (zc[2] - zc[1]) / (2.0 * _timewt.Dt) * Constn.Rhoa * Constn.Ca;
            if (nc == 1)
            {
                _matrix.C1[n] = _matrix.C1[n] + _timewt.Wdt * con[1];
                _matrix.D1[n] = _matrix.D1[n] - con[1] * (_timewt.Wt * (tcdt[1] - tc[2]) + _timewt.Wdt * (tcdt[1] - tcdt[2])) + heatc[1] - (zc[2] - zc[1]) / _timewt.Dt * Constn.Rhoa * Constn.Ca * (tcdt[1] - tc[1]);
            }
            else
            {
                _matrix.C1[n] = _matrix.C1[n] + con[1];
                _matrix.D1[n] = _matrix.D1[n] - con[1] * (tcdt[1] - tcdt[2]) + heatc[1] - (zc[2] - zc[1]) / (2.0 * _timewt.Dt) * Constn.Rhoa * Constn.Ca * (tcdt[1] - tc[1]);
            }
            //
            //**** DETERMINE THE COEFFICIENTS FOR THE REST OF THE LAYERS
            for (var i = n + 1; i <= n + nc - 1; ++i)
            {
                j = i - n + 1;
                _matrix.A1[i + 1] = _matrix.A1[i + 1] + con[j];
                _matrix.B1[i] = _matrix.B1[i] - (con[j - 1] + con[j] + dheatc[j] * (1.0 - dtldtc[j])) - (zc[j + 1] - zc[j - 1]) / (2.0 * _timewt.Dt) * Constn.Rhoa * Constn.Ca;
                if (j == nc)
                {
                    _matrix.C1[i] = _matrix.C1[i] + _timewt.Wdt * con[j];
                    _matrix.D1[i] = con[j - 1] * (tcdt[j - 1] - tcdt[j]) - con[j] * (_timewt.Wdt * (tcdt[j] - tcdt[j + 1]) + _timewt.Wt * (tcdt[j] - tc[j + 1])) - (zc[j + 1] - zc[j] + (zc[j] - zc[j - 1]) / 2) / _timewt.Dt * Constn.Rhoa * Constn.Ca * (tcdt[j] - tc[j]) + heatc[j];
                }
                else
                {
                    _matrix.C1[i] = _matrix.C1[i] + con[j];
                    _matrix.D1[i] = con[j - 1] * (tcdt[j - 1] - tcdt[j]) - con[j] * (tcdt[j] - tcdt[j + 1]) - (zc[j + 1] - zc[j - 1]) / (2.0 * _timewt.Dt) * Constn.Rhoa * Constn.Ca * (tcdt[j] - tc[j]) + heatc[j];
                }
            label10:;
            }
            n = n + nc;
            //
            //**** DETERMINE THE COEFFICIENTS FOR THE TOP LAYER OF THE NEXT MATERIAL
            if (nsp > 0 || nr == 0)
            {
                //        NEXT MATERIAL IS SNOW OR SOIL -- NEED TO CONSIDER LATENT HEAT
                //        TRANSFER TO THE NEXT NODE.
                //        DETERMINE THE CONVECTIVE VAPOR TRANSPORT FROM EDDY CONDUCTANCE
                conv = con[nc] / Constn.Rhoa / Constn.Ca;
                //        CALCULATE THE VAPOR TRANSFER BETWEEN NODES
                qvcan = conv * (_timewt.Wdt * (vapcdt[nc] - vapcdt[nc + 1]) + _timewt.Wt * (vapcdt[nc] - vapc[nc + 1]));
                //        CALCULATE HUMIDITY AND SLOPE OF SAT. VAPOR CURVE
                Vslope(ref slpnc, ref satv, ref tc[nc]);
                humnc = vapcdt[nc];
                Vslope(ref slpnc1, ref satv, ref tc[nc + 1]);
                humnc1 = vapcdt[nc + 1] / satv;
            }
            //
            if (nsp > 0)
            {
                //****    NEXT MATERIAL IS SNOW
                _matrix.A1[n] = _matrix.A1[n] + conv * Constn.Ls * slpnc * humnc;
                _matrix.B1[n] = _matrix.B1[n] - _timewt.Wdt * (con[nc] + conv * Constn.Ls * slpnc1 * humnc1);
                _matrix.D1[n] = con[nc] * (_timewt.Wt * (tcdt[nc] - tc[nc + 1]) + _timewt.Wdt * (tcdt[nc] - tcdt[nc + 1])) + Constn.Ls * qvcan;
            }
            else
            {
                //
                if (nr > 0)
                {
                    //****    NEXT MATERIAL IS RESIDUE
                    _matrix.B1[n] = _matrix.B1[n] - _timewt.Wdt * con[nc];
                    _matrix.D1[n] = con[nc] * (_timewt.Wt * (tcdt[nc] - tc[nc + 1]) + _timewt.Wdt * (tcdt[nc] - tcdt[nc + 1]));
                }
                else
                {
                    //
                    //**** NEXT MATERIAL IS SOIL
                    _matrix.A1[n] = _matrix.A1[n] + conv * Constn.Lv * slpnc * humnc;
                    _matrix.B1[n] = _matrix.B1[n] - _timewt.Wdt * (con[nc] + conv * Constn.Lv * slpnc1 * humnc1);
                    _matrix.D1[n] = con[nc] * (_timewt.Wt * (tcdt[nc] - tc[nc + 1]) + _timewt.Wdt * (tcdt[nc] - tcdt[nc + 1])) + Constn.Lv * qvcan;
                    //
                }
            }
            //xxOUT
            _writeit.Hnc = con[nc] * (_timewt.Wt * (tcdt[nc] - tcdt[nc + 1]) + _timewt.Wdt * (tcdt[nc] - tcdt[nc + 1]));
        }

        // line 6129
        private void Leaft(ref int nplant, ref int nc, ref int ns, ref int iter, int[] itype, double[] tc, double[] tcdt, double[][] tlc, double[][] tlcdt, double[] vapc, double[] vapcdt, double[] wcan, double[] wcandt, double[] pcan, double[] pcandt, double[] mat, double[] matdt, double[] trnsp, double[] xtract, double[][] swcan, double[][] lwcan, double[] swdown, double[] heatc, double[] etlyr, double[] detlyr, ref double canma, ref double canmb, double[] dchar, double[] rstom0, double[] rstexp, double[] pleaf0, double[] rleaf0, ref int istomate, double[][] stomate, double[] dheatc, double[] dtldtc)
        {
            //
            //     THIS SUBROUTINE COMPUTES LEAF TEMPERATURE OF EACH CANOPY TYPE AND
            //     THE TOTAL HEAT AND WATER TRANSFERRED FROM CANOPY TO SURROUNDING
            //     AIR SPACE IN EACH CANOPY LAYER
            //
            //***********************************************************************
            //
            //xOUT
            //
            var avgtmp = new double[11];
            var avgvap = new double[11];
            var avgdef = new double[11];
            var rstom = new double[11];
            var pleaf = new double[11];
            var pevap = new double[11];
            var avgmat = new double[51];
            var humid = new double[11];
            var vtslop = new double[11];
            var f1 = new double[11];
            var df1dp = new double[11];
            var df1dt = new double[11];
            var df1dx = new double[11];
            var dp = new double[11];
            var aa1 = new double[11];
            var f2 = new double[11];
            var df2dp = new double[11];
            var df2dt = new double[11];
            var df2dx = new double[11];
            var dtlc = new double[11];
            var df3dp = new double[11];
            var a1lwr = JaggedArray<double>(9, 11);
            var cc1 = new double[11];

            var etmax = 0.0;
            var delmax = 0.0;
            var d2 = 0.0;
            var d1 = 0.0;
            var det = 0.0;
            var ff2 = 0.0;
            var ff1 = 0.0;
            var b2 = 0.0;
            var a2 = 0.0;
            var humcan = 0.0;
            var b1 = 0.0;
            var a1 = 0.0;
            var dx = 0.0;
            var cc3 = 0.0;
            var f3 = 0.0;
            var df3dx = 0.0;
            var iflag = 0;
            var iter1 = 0;
            var compar = 0.0;
            var aneg = 0.0;
            var delta = 0.0;
            var deriv = 0.0;
            var error = 0.0;
            var rslog = 0.0;
            //var pleaf1 = 0.0;
            var resist = 0.0;
            var soimat = 0.0;
            var iroot = 0;
            var srroot = 0.0;
            var rsoil = 0.0;
            var iter0 = 0;
            var rvavg = 0.0;
            var rhavg = 0.0;
            var min = 0;
            var max = 0;
            var vapdef = 0.0;
            var fractn = 0.0;
            var sumev = 0.0;
            var sumet = 0.0;
            var term = 0.0;
            var dummy = 0.0;
            var satv = 0.0;
            var rstom1 = 0.0;
            var drsdplf = 0.0;
            var b1lv = 0.0;
            var wchum = 0.0;
            var hwslop = 0.0;
            var hum = 0.0;

            //     INITIALIZE ROOT EXTRACTION AND COMPUTE AVERAGE MATRIC POTENTIAL
            for (var i = 1; i <= ns; ++i)
            {
                xtract[i] = 0.0;
                avgmat[i] = _timewt.Wt * mat[i] + _timewt.Wdt * matdt[i];
                //        LIMIT THE WATER POTENTIALS THAT THE PLANT SEES
                if (avgmat[i] > 0.0) avgmat[i] = 0.0;
                label5:;
            }
            //
            //     INITIALIZE THE TOTAL TRANSP. FROM THE ENTIRE TRANSPIRING CANOPY
            trnsp[nplant + 1] = 0.0;
            //
            //     INITIALIZE CANOPY TEMP, VAPOR DENSITY, AND HEAT AND WATER FLUXES.
            //     HEATC(I) AND ETLYR ARE HEAT AND WATER FLUXES; DHEATC AND DETLYR
            //     ARE DERIVATIVES OF FLUX TERMS.
            for (var i = 1; i <= nc; ++i)
            {
                avgtmp[i] = tcdt[i];
                avgvap[i] = vapcdt[i];
                if (istomate == 2)
                {
                    //           COMPUTE VAPOR PRESSURE DEFICIT OF AIR (NOT LEAF TEMPERATURE)
                    Vslope(ref dummy, ref satv, ref avgtmp[i]);
                    avgdef[i] = satv - avgvap[i];
                }
                heatc[i] = 0.0;
                dheatc[i] = 0.0;
                etlyr[i] = 0.0;
                detlyr[i] = 0.0;
                dtldtc[i] = 0.0;
                //        INITIALIZE RESISTANCE TO TRANSPORT FROM CANOPY LEAVES
                //        AND LONG-WAVE EMITTANCE COEFFICIENT (FOR BOTH SIDES OF LEAVES)
                for (var j = 1; j <= nplant; ++j)
                {
                    a1lwr[j][i] = 8.0 * (1.0 - _radcan.Tdiffc[i]) * _radcan.Difkl[j][i] / _radcan.Difkl[nplant + 1][i] * Lwrcof.Emitc * Lwrcof.Stefan * (Math.Pow((_canlwr.Tlclwr[j][i] + 273.16), 3));
                    if (iter == 1)
                    {
                        //             RH & RV NOT NECESSARY IF ALREADY CALCULATED THIS TIME STEP
                        //             (AIR TEMPERATURE AT BEGINNING OF TIME STEP IS SUFFICIENT)
                        term = Math.Sqrt(dchar[j] / _windv.Windc[i]) * _constn.Presur / Constn.Ugas / (tcdt[i] + 273.16);
                        _leaftSave.Rhcan[j][i] = 7.40 * term;
                        _leaftSave.Rvcan[j][i] = 6.80 * term;
                        //             SET INIT SO THAT INITIAL ESTIMATE OF PXYLEM IS COMPUTED
                        _leaftSave.Init[j] = 1;
                    }
                label8:;
                }
            label10:;
            }
            //
            for (var j = 1; j <= nplant; ++j)
            {
                sumet = 0.0;
                sumev = 0.0;
                fractn = 1.0;
                //
                if (_clayrs.Totlai[j] == 0.0)
                {
                    //           NO LEAF AREA FOR THIS PLANT(PERHAPS DORMANT OR SNOW-COVERED)
                    trnsp[j] = 0.0;
                    goto label60;
                }
                //
                if (itype[j] != 0)
                {
                    //
                    //********   TRANSPIRING PLANT - CHECK IF CONDITIONS ARE SUCH THAT PLANT
                    //           WILL TRANSPIRE
                    pcandt[j] = 0.0;
                    if (_clayrs.Ievap[j] == 0 || pcan[j] > 0.0)
                    {
                        //*****         PLANT ISN'T TRANSPIRING - PERHAPS NO SUNLIGHT, TOO COLD,
                        //              OR INTERCEPTED PRECIP AVAILABLE ON PLANT LEAVES
                        //              "FRACTN" IS FRACTION OF TIME STEP PLANTS WILL TRANSPIRE
                        fractn = 0.0;
                        //              CALCULATE LEAF TEMPERATURE
                        for (var i = 1; i <= nc; ++i)
                        {
                            //                 CHECK IF PLANT HAS ANY LEAF AREA IN LAYER
                            if (_clayrs.Canlai[j][i] <= 0.0) goto label12;
                            rstom[i] = 0.0;
                            _leaftSave.Etcan[j][i] = 0.0;
                            if (pcan[j] > 0.0)
                            {
                                //***                 INTERCEPTED PRECIP AVAILABLE FOR EVAPORATION
                                humid[i] = 1.0;
                                //xxxx                CALL VSLOPE (VTSLOP(I),SATV,AVGTMP(I))
                                Vslope(ref vtslop[i], ref satv, ref _canlwr.Tlclwr[j][i]);
                                vapdef = _clayrs.Canlai[j][i] * (satv - avgvap[i]);
                                tlcdt[j][i] = _canlwr.Tlclwr[j][i] + (swcan[j][i] + lwcan[j][i] - Constn.Lv * vapdef / _leaftSave.Rvcan[j][i] - Constn.Rhoa * Constn.Ca * _clayrs.Canlai[j][i] * (_canlwr.Tlclwr[j][i] - avgtmp[i]) / _leaftSave.Rhcan[j][i] - _clayrs.Drycan[j][i] * Constn.Cr * (_canlwr.Tlclwr[j][i] - tlc[j][i]) / _timewt.Dt) / (Constn.Rhoa * Constn.Ca * _clayrs.Canlai[j][i] / _leaftSave.Rhcan[j][i] + a1lwr[j][i] + _clayrs.Drycan[j][i] * Constn.Cr / _timewt.Dt + Constn.Lv * _clayrs.Canlai[j][i] * vtslop[i] / _leaftSave.Rvcan[j][i]);
                                //                    DETERMINE AMOUNT OF INTERCEPTED PRECIP THAT
                                //                    EVAPORATED FROM PLANT SURFACES
                                pevap[i] = (_clayrs.Canlai[j][i] * vtslop[i] * (tlcdt[j][i] - _canlwr.Tlclwr[j][i]) + vapdef) / _leaftSave.Rvcan[j][i];
                                sumev = sumev + pevap[i];
                            }
                            else
                            {
                                //***                 NO WATER AVAILABLE FOR EVAPORATION
                                humid[i] = 0.0;
                                vtslop[i] = 0.0;
                                pevap[i] = 0.0;
                                sumev = 0.0;
                                tlcdt[j][i] = _canlwr.Tlclwr[j][i] + (swcan[j][i] + lwcan[j][i] - Constn.Rhoa * Constn.Ca * _clayrs.Canlai[j][i] * (_canlwr.Tlclwr[j][i] - avgtmp[i]) / _leaftSave.Rhcan[j][i] - _clayrs.Drycan[j][i] * Constn.Cr * (_canlwr.Tlclwr[j][i] - tlc[j][i]) / _timewt.Dt) / (Constn.Rhoa * Constn.Ca * _clayrs.Canlai[j][i] / _leaftSave.Rhcan[j][i] + a1lwr[j][i] + _clayrs.Drycan[j][i] * Constn.Cr / _timewt.Dt);
                            }
                        label12:;
                        }
                        if (pcan[j] > 0.0)
                        {
                            //*****            CALCULATE WATER ON PLANTS AT END OF TIME STEP
                            pcandt[j] = pcan[j] - _timewt.Dt * sumev / Constn.Rhol;
                            if (pcandt[j] < 0.0)
                            {
                                //                    NO WATER REMAINING ON PLANTS - COMPUTE FRACTION OF
                                //                    TIME STEP PLANTS WILL BE TRANSPIRING AND ADJUST
                                //                    AMOUNT EVAPORATED
                                pcandt[j] = 0.0;
                                fractn = 1 - (Constn.Rhol * pcan[j] / _timewt.Dt) / sumev;
                                sumev = (1.0 - fractn) * sumev;
                                for (var i = 1; i <= nc; ++i)
                                {
                                    pevap[i] = pevap[i] * (1.0 - fractn);
                                label13:;
                                }
                            }
                        }
                    }
                    //
                    if (pcandt[j] <= 0.0 && _clayrs.Ievap[j] != 0)
                    {
                        //********      PLANT IS TRANSPIRING
                        //
                        //              FIND THE EXTREMES OF MATRIC POTENTIAL SEEN BY ROOTS
                        //              TAKING INTO ACCOUNT ROOT RESISTANCE FOR EACH LAYER
                        max = 0;
                        for (var i = 1; i <= ns; ++i)
                        {
                            if (_clayrs.Rootdn[j][i] > 0.0)
                            {
                                if (max == 0)
                                {
                                    max = i;
                                    min = i;
                                }
                                else
                                {
                                    if (avgmat[i] < avgmat[min]) min = i;
                                    if (avgmat[i] / _clayrs.Rroot[j][i] > avgmat[max] / _clayrs.Rroot[j][max]) max = i;
                                }
                            }
                        label14:;
                        }
                        //
                        //              DETERMINE LEAF POTENTIAL AND TEMP
                        vapdef = 0.0;
                        rhavg = 0.0;
                        rvavg = 0.0;
                        //              INITIZE VARIABLES FOR CANOPY
                        for (var i = 1; i <= nc; ++i)
                        {
                            //                 CHECK IF PLANT HAS ANY LEAF AREA IN THIS LAYER
                            if (_clayrs.Canlai[j][i] > 0.0)
                            {
                                if (fractn > 0.999) pevap[i] = 0.0;
                                //XXXX                TLC(J,I) = AVGTMP(I)
                                humid[i] = 1.0;
                                //                    CALCULATE AVERAGE CONDITIONS IN CANOPY FOR AN
                                //                    INTIIAL APPROXIMATION TO PLEAF AND PXYLEM
                                Vslope(ref vtslop[i], ref satv, ref tlcdt[j][i]);
                                vapdef = vapdef + _clayrs.Canlai[j][i] * (satv - avgvap[i]);
                                rhavg = rhavg + _clayrs.Canlai[j][i] / _leaftSave.Rhcan[j][i];
                                rvavg = rvavg + _clayrs.Canlai[j][i] / _leaftSave.Rvcan[j][i];
                            }
                        label15:;
                        }
                        vapdef = vapdef / _clayrs.Totlai[j];
                        if (vapdef < 0.0) vapdef = 0.0;
                        rhavg = _clayrs.Totlai[j] / rhavg;
                        rvavg = _clayrs.Totlai[j] / rvavg;
                        //
                        //*****         BEGIN ITERATION TO FIND INITIAL ESTIMATE FOR PXYLEM
                        iter0 = 0;
                    //              CALCULATE INITIAL GUESS FOR PXYLEM IF THIS IS FIRST TIME
                    //              FOR THIS TIME STEP
                    label18:;
                        if (_leaftSave.Init[j] == 1) _leaftSave.Pxylem[j] = 2.0 * avgmat[max];
                        //***           COMPUTE SUM OF ROOT CONDUCTANCE TIMES MATRIC POTENTIAL
                        label19:;
                        rsoil = 0.0;
                        srroot = 0.0;
                        iroot = 0;
                        for (var i = 1; i <= ns; ++i)
                        {
                            //                 DO NOT CONSIDER IF NO PLANT J ROOTS IN SOIL LAYER
                            if (_clayrs.Rootdn[j][i] > 0.0)
                            {
                                //                    DO NOT INCLUDE SOIL LAYERS DRYER THAN PLANT XYLEM
                                if (avgmat[i] >= _leaftSave.Pxylem[j])
                                {
                                    rsoil = rsoil + _clayrs.Rroot[j][i] * avgmat[i];
                                    srroot = srroot + _clayrs.Rroot[j][i];
                                }
                                else
                                {
                                    iroot = 1;
                                }
                            }
                        label20:;
                        }
                        if (_leaftSave.Init[j] > 1)
                        {
                            //                 USE VALUES FOR PXYLEM AND ETCAN FROM PREVIOUS
                            //                 CALCULATIONS FOR THIS TIME STEP IF AVAILABLE
                            //                 (CALCULATE SUMET AND GO DIRECTLY TO ITERATIVE SCHEME)
                            sumet = rsoil - _leaftSave.Pxylem[j] * srroot;
                            if (sumet * srroot <= 0.0)
                            {
                                //                    PREVIOUS VALUE OF PXYLEM WILL NOT WORK FOR UPDATED
                                //                    END-OF-TIME-STEP CONDITIONS
                                _leaftSave.Init[j] = 1;
                                goto label18;
                            }
                            goto label24;
                        }
                        //***           CALC. EFFECTIVE MATRIC POT. AND TOTAL RESISTANCE OF PLANT
                        if (srroot == 0.0)
                        {
                            rsoil = _clayrs.Rroot[j][max] * avgmat[max];
                            srroot = _clayrs.Rroot[j][max];
                        }
                        soimat = rsoil / srroot;
                        resist = 1 / srroot + 1 / rleaf0[j];
                        if (iter0 == 0)
                        {
                            //                 ESTIMATE STARTING POINT FOR ITERATION
                            _leaftSave.Pleaf1 = _leaftSave.Pxylem[j];
                            if (_leaftSave.Pleaf1 / pleaf0[j] > 40.0)
                            {
                                //                    LIKELIHOOD OF ARITHMETIC OVERFLOW -- PROCEED WITH
                                //                    CAUTION BY TAKING LOGARITHM
                                rslog = Math.Log10(rstom0[j]) + rstexp[j] * Math.Log10(_leaftSave.Pleaf1 / pleaf0[j]);
                                if (rslog > 20)
                                {
                                    //                       LOG OF STOMATAL RESISTANCE EXTREMELY LARGE --
                                    //                       TRANSP IS ESSENTIALLY ZERO - CALC LEAF TEMP
                                    sumet = 0.0;
                                    for (var i = 1; i <= nc; ++i)
                                    {
                                        if (_clayrs.Canlai[j][i] <= 0.0) goto label21;
                                        humid[i] = 0.0;
                                        rstom[i] = 1.0e20;
                                        _leaftSave.Etcan[j][i] = 0.0;
                                        tlcdt[j][i] = _canlwr.Tlclwr[j][i] + (swcan[j][i] + lwcan[j][i] - Constn.Rhoa * Constn.Ca * _clayrs.Canlai[j][i] * (_canlwr.Tlclwr[j][i] - avgtmp[i]) / _leaftSave.Rhcan[j][i] - _clayrs.Drycan[j][i] * Constn.Cr * (_canlwr.Tlclwr[j][i] - tlc[j][i]) / _timewt.Dt) / (Constn.Rhoa * Constn.Ca * _clayrs.Canlai[j][i] / _leaftSave.Rhcan[j][i] + a1lwr[j][i] + _clayrs.Drycan[j][i] * Constn.Cr / _timewt.Dt);
                                    label21:;
                                    }
                                    goto label48;
                                }
                            }
                            Stomates(ref j, ref rstom1, ref drsdplf, ref _leaftSave.Pleaf1, ref swdown[1], ref avgtmp[1], ref avgdef[1], rstom0, pleaf0, rstexp, ref istomate, stomate);
                            sumet = _clayrs.Totlai[j] * vapdef / (rstom1 + rvavg);
                            _leaftSave.Pleaf1 = soimat - sumet * resist / 2.0;
                        }
                    //***           UPDATE STOMATAL RESISTANCE AND TRANSPIRATION
                    label22:;
                        Stomates(ref j, ref rstom1, ref drsdplf, ref _leaftSave.Pleaf1, ref swdown[1], ref avgtmp[1], ref avgdef[1], rstom0, pleaf0, rstexp, ref istomate, stomate);
                        sumet = _clayrs.Totlai[j] * vapdef / (rstom1 + rvavg);
                        //              CALCULATE ERROR IN ET ESTIMATE, DERIVATIVE WITH RESPECT
                        //              TO LEAF POTENTIAL, AND NEW APPROX. TO LEAF POTENTIAL
                        error = (soimat - _leaftSave.Pleaf1) / resist - sumet;
                        deriv = -1.0 / resist + sumet * drsdplf / (rstom1 + rvavg);
                        delta = error / deriv;
                        //***           DEPENDING ON MAGNITUDE OF RSTEXP, A DRASTIC POINT OF
                        //              INFLECTION OCCURS IN THE ERROR FUNCTION AT PLEAF0.0 IF
                        //              UPDATED PLEAF1 CROSSES THIS POINT, CUT DELTA IN HALF
                        aneg = (_leaftSave.Pleaf1 - pleaf0[j]) * (_leaftSave.Pleaf1 - delta - pleaf0[j]);
                        if (aneg < 0.0)
                        {
                            _leaftSave.Pleaf1 = _leaftSave.Pleaf1 - delta / 2.0;
                        }
                        else
                        {
                            _leaftSave.Pleaf1 = _leaftSave.Pleaf1 - delta;
                        }
                        //              CALCULATE UPDATED ET AND XYLEM POTENTIAL
                        sumet = (soimat - _leaftSave.Pleaf1) / resist;
                        _leaftSave.Pxylem[j] = (rsoil - sumet) / srroot;
                        if (Math.Abs(_leaftSave.Pleaf1) < 1.0)
                        {
                            //                 AVOID DIVISION BY ZERO
                            compar = 1.0;
                        }
                        else
                        {
                            compar = _leaftSave.Pleaf1;
                        }
                        if (Math.Abs(delta / compar) > 0.01 && iter0 <= 20)
                        {
                            //***              PLEAF AND PXYLEM NOT CLOSE ENOUGH
                            iter0 = iter0 + 1;
                            //                 IF PXYLEM > MINIMUM SOIL POTENTIAL, RECALCULATE
                            //                 RSOIL, SRROOT AND PXYLEM TO EXCLUDE DRY LAYERS
                            if (_leaftSave.Pxylem[j] > avgmat[min] || iroot > 0) goto label19;
                            goto label22;
                        }
                        //*****         ESTIMATE TRANSP (ETCAN) WITHIN EACH LAYER TO BEGIN ITER
                        for (var i = 1; i <= nc; ++i)
                        {
                            _leaftSave.Etcan[j][i] = _clayrs.Canlai[j][i] * sumet / _clayrs.Totlai[j];
                        label23:;
                        }
                        _leaftSave.Init[j] = 2;
                    //
                    //*****         BEGIN ITERATION TO FIND LEAF TEMPERATURE, LEAF POTENTIAL
                    //              AND TRANSPIRATION FROM EACH CANOPY LAYER FOR PLANT
                    label24:;
                        iter1 = 0;
                        for (var i = 1; i <= nc; ++i)
                        {
                            //                 INTIAL ESTIMATE OF LEAF POTENTIAL IN EACH LAYER
                            //                 AND DEFINE NEWTON-RAPHSON COEFF. THAT ARE CONSTANT
                            if (_clayrs.Canlai[j][i] > 0.0)
                            {
                                pleaf[i] = _leaftSave.Pxylem[j] - _leaftSave.Etcan[j][i] / _clayrs.Rleaf[j][i];
                                df1dx[i] = -_clayrs.Rleaf[j][i];
                                df2dt[i] = -_clayrs.Canlai[j][i] * Constn.Rhoa * Constn.Ca / _leaftSave.Rhcan[j][i] - a1lwr[j][i] - _clayrs.Drycan[j][i] * Constn.Cr / _timewt.Dt;
                            }
                        label25:;
                        }
                    //
                    label26:;
                        iflag = 0;
                        sumet = 0.0;
                        df3dx = 0.0;
                        //              SET UP COEFFICIENTS FOR NEWTON RAPHSON SOLUTION
                        for (var i = 1; i <= nc; ++i)
                        {
                            if (_clayrs.Canlai[j][i] > 0.0)
                            {
                                Vslope(ref vtslop[i], ref satv, ref tlcdt[j][i]);
                                Stomates(ref j, ref rstom[i], ref drsdplf, ref pleaf[i], ref swdown[i], ref avgtmp[i], ref avgdef[i], rstom0, pleaf0, rstexp, ref istomate, stomate);
                                f1[i] = _clayrs.Canlai[j][i] * (satv - avgvap[i]) / (rstom[i] + _leaftSave.Rvcan[j][i]);
                                if (f1[i] < 0.0)
                                {
                                    //***                 NO TRANSPIRATION
                                    vtslop[i] = 0.0;
                                    _leaftSave.Etcan[j][i] = 0.0;
                                    df1dp[i] = _clayrs.Rleaf[j][i];
                                    df1dt[i] = 0.0;
                                    df2dp[i] = 0.0;
                                    df2dx[i] = 0.0;
                                    df3dp[i] = 0.0;
                                    //                    FORCE LEAF POTENTIAL EQUAL TO PXYLEM POTENTIAL,
                                    //                    I.E. FORCE TRANSPIRATION IN LAYER TO ZERO
                                    f1[i] = 0.0;
                                    pleaf[i] = _leaftSave.Pxylem[j];
                                }
                                else
                                {
                                    //***                 CALCULATE TRANPIRATION IN EACH LAYER AND SET UP
                                    //***                 MATRIX FOR NEWTON-RAPHSON APPROX. OF UPDATED XYLEM
                                    //                    POTENTIAL, LEAF TEMPERATURE AND LEAF POTENTIAL
                                    _leaftSave.Etcan[j][i] = _clayrs.Rleaf[j][i] * (_leaftSave.Pxylem[j] - pleaf[i]);
                                    sumet = sumet + _leaftSave.Etcan[j][i];
                                    df1dp[i] = _clayrs.Rleaf[j][i] - f1[i] * drsdplf / (rstom[i] + _leaftSave.Rvcan[j][i]);
                                    df1dt[i] = _clayrs.Canlai[j][i] * vtslop[i] / (rstom[i] + _leaftSave.Rvcan[j][i]);
                                    df2dp[i] = Constn.Lv * _clayrs.Rleaf[j][i];
                                    df2dx[i] = -df2dp[i];
                                    df3dp[i] = _clayrs.Rleaf[j][i];
                                    df3dx = df3dx - _clayrs.Rleaf[j][i];
                                    f1[i] = f1[i] - _leaftSave.Etcan[j][i];
                                }
                                f2[i] = swcan[j][i] + lwcan[j][i] - Constn.Lv * _leaftSave.Etcan[j][i] - a1lwr[j][i] * (tlcdt[j][i] - _canlwr.Tlclwr[j][i]) - _clayrs.Canlai[j][i] * Constn.Rhoa * Constn.Ca * (tlcdt[j][i] - avgtmp[i]) / _leaftSave.Rhcan[j][i] - _clayrs.Drycan[j][i] * Constn.Cr * (tlcdt[j][i] - tlc[j][i]) / _timewt.Dt;
                            }
                        label30:;
                        }
                        f3 = rsoil - _leaftSave.Pxylem[j] * srroot - sumet;
                        df3dx = df3dx - srroot;
                        cc3 = df3dx;
                        //
                        if (sumet <= 0.0)
                        {
                            //                 SET TRANPIRATION TO ZERO AND CALCULATE LEAF TEMP
                            sumet = 0.0;
                            for (var i = 1; i <= nc; ++i)
                            {
                                if (_clayrs.Canlai[j][i] > 0.0)
                                {
                                    humid[i] = 0.0;
                                    rstom[i] = 1.0e20;
                                    _leaftSave.Etcan[j][i] = 0.0;
                                    tlcdt[j][i] = _canlwr.Tlclwr[j][i] + (swcan[j][i] + lwcan[j][i] - Constn.Rhoa * Constn.Ca * _clayrs.Canlai[j][i] * (_canlwr.Tlclwr[j][i] - avgtmp[i]) / _leaftSave.Rhcan[j][i] - _clayrs.Drycan[j][i] * Constn.Cr * (_canlwr.Tlclwr[j][i] - tlc[j][i]) / _timewt.Dt) / (-df2dt[i]);
                                    _leaftSave.Init[j] = 1;
                                }
                            label31:;
                            }
                            goto label48;
                        }
                        //
                        //              SOLVE MATRIX FOR CHANGE IN PXYLEM(J) AND TLCDT(J,I)
                        for (var i = 1; i <= nc; ++i)
                        {
                            if (_clayrs.Canlai[j][i] > 0.0)
                            {
                                aa1[i] = df1dp[i] - (df1dt[i] / df2dt[i]) * df2dp[i];
                                cc1[i] = df1dx[i] - (df1dt[i] / df2dt[i]) * df2dx[i];
                                f1[i] = f1[i] - (df1dt[i] / df2dt[i]) * f1[i];
                                cc3 = cc3 - (df3dp[i] / aa1[i]) * cc1[i];
                                f3 = f3 - (df3dp[i] / aa1[i]) * f1[i];
                            }
                        label32:;
                        }
                        dx = f3 / cc3;
                        _leaftSave.Pxylem[j] = _leaftSave.Pxylem[j] - dx;
                        // for j = 2, dx, f1, and f2 are off
                        //
                        //              SOLVE MATRIX FOR CHANGE IN PLEAF(I) AND TLCDT(J,I)
                        for (var i = 1; i <= nc; ++i)
                        {
                            if (_clayrs.Canlai[j][i] > 0.0)
                            {
                                dp[i] = (f1[i] - cc1[i] * dx) / aa1[i];
                                dtlc[i] = (f2[i] - df2dx[i] * dx - df2dp[i] * dp[i]) / df2dt[i];
                                //                 ADJUST LEAF TEMP & POTENTIAL
                                pleaf[i] = pleaf[i] - dp[i];
                                //                 Add small amount to PXYLEM when checking to avoid
                                //                 error related to inability of COMPAQ compiler to
                                //                 correctly assess the non-equality.  Otherwise it
                                //                 sometimes thinks that PLEAF(I) is > PXYLEM when they
                                //                 are in fact equal.
                                if (pleaf[i] > _leaftSave.Pxylem[j] + 1.0e-5) pleaf[i] = (pleaf[i] + dp[i] + _leaftSave.Pxylem[j]) / 2.0;
                                tlcdt[j][i] = tlcdt[j][i] - dtlc[i];
                                //                 CHECK IF TEMPERATURE CHANGE IS WITHIN 0.01 C
                                if (Math.Abs(dtlc[i]) > 0.01) iflag = iflag + 1;
                            }
                        label33:;
                        }
                        //
                        //*****         CHECK IF TOLERANCES HAVE BEEN MET
                        if (iflag > 0)
                        {
                            iter1 = iter1 + 1;
                            if (iter1 < 20) goto label26;
                        }
                        //
                        //*****         SOLUTION HAS BEEN FOUND FOR LEAF TEMP AND TRANPIRATION
                        //              FIND FINAL XYLEM POTENTIAL FOR USE IN ROOT EXTRACTION
                        if (srroot * sumet != 0.0)
                        {
                            _leaftSave.Pxylem[j] = (rsoil - sumet) / srroot;
                            if (_leaftSave.Pxylem[j] > avgmat[min] || iroot > 0)
                            {
                                rsoil = 0.0;
                                srroot = 0.0;
                                for (var i = 1; i <= ns; ++i)
                                {
                                    if (_clayrs.Rootdn[j][i] > 0.0)
                                    {
                                        //                       DON'T INCLUDE SOIL LAYERS DRYER THAN PLANT XYLEM
                                        if (avgmat[i] >= _leaftSave.Pxylem[j])
                                        {
                                            rsoil = rsoil + _clayrs.Rroot[j][i] * avgmat[i];
                                            srroot = srroot + _clayrs.Rroot[j][i];
                                        }
                                    }
                                label34:;
                                }
                                //                    RECALCULATE WATER POTENTIAL IN XYLEM
                                _leaftSave.Pxylem[j] = (rsoil - sumet) / srroot;
                            }
                            //
                            //                 CALCULATE ROOT EXTRACTION FROM EACH SOIL LAYER
                            for (var i = 1; i <= ns; ++i)
                            {
                                if (avgmat[i] > _leaftSave.Pxylem[j]) xtract[i] = xtract[i] + fractn * _clayrs.Totrot[j] * _clayrs.Rroot[j][i] * (avgmat[i] - _leaftSave.Pxylem[j]);
                                label35:;
                            }
                        }
                        else
                        {
                            //                 SOIL TOO DRY FOR PLANTS TO EXTACT WATER
                            sumet = 0.0;
                        }
                        //
                        //              SUM TRANSPIRATION FROM ENTIRE TRANSPIRING CANOPY
                        trnsp[nplant + 1] = trnsp[nplant + 1] + fractn * sumet;
                    }
                    //
                }
                else
                {
                    //********   DEAD PLANT MATERIAL -- COMPUTE WATER CONTENT AND TEMPERATURE
                    for (var i = 1; i <= nc; ++i)
                    {
                        if (_clayrs.Canlai[j][i] <= 0.0) goto label45;
                        //
                        pevap[i] = 0.0;
                        rstom[i] = 0.0;
                        b1lv = _clayrs.Drycan[j][i] / _timewt.Dt;
                        a1 = -a1lwr[j][i] - _clayrs.Canlai[j][i] * Constn.Rhoa * Constn.Ca / _leaftSave.Rhcan[j][i] - _clayrs.Drycan[j][i] * (Constn.Cr + wcan[i] * Constn.Cl) / _timewt.Dt;
                        b1 = Constn.Lv * b1lv;
                        //              CALCULATE WATER CONTENT BASED ON VAPOR DENSITY OF AIR
                        Vslope(ref dummy, ref satv, ref tlcdt[j][i]);
                        humcan = avgvap[i] / satv;
                        Canhum(2, ref humcan, ref dummy, ref wchum, ref tcdt[i], ref canma, ref canmb);
                        //
                        if (_leaftSave.Init[j] == 1)
                        {
                            //                 INITIALIZE VARIABLES FOR THIS TIME STEP
                            _leaftSave.Etcan[j][i] = -b1lv * (wcandt[i] - wcan[i]);
                            if (_leaftSave.Etcan[j][i] == 0.0)
                            {
                                //XXX                 TLC(J,I) = AVGTMP(I)
                                //                    COMPUTE HUMIDITY IN PLANT MATERIAL
                                Canhum(1, ref humid[i], ref dummy, ref wcandt[i], ref tcdt[i], ref canma, ref canmb);
                                Vslope(ref dummy, ref satv, ref tlcdt[j][i]);
                                _leaftSave.Etcan[j][i] = _clayrs.Canlai[j][i] * (humid[i] * satv - avgvap[i]) / _leaftSave.Rvcan[j][i];
                                wcandt[i] = wcan[i] - _leaftSave.Etcan[j][i] / b1lv;
                                //                    CHECK IF WATER CONTENT IS REASONABLE --
                                if ((wcandt[i] - wchum) * (wcan[i] - wchum) < 0.0)
                                {
                                    //                       WATER CONTENT WENT BEYOND EQUILIBRUIM WITH AIR
                                    _leaftSave.Etcan[j][i] = -b1lv * (wchum - wcan[i]);
                                    humid[i] = (avgvap[i] + _leaftSave.Etcan[j][i] * _leaftSave.Rvcan[j][i] / _clayrs.Canlai[j][i]) / satv;
                                    Canhum(2, ref humid[i], ref dummy, ref wcandt[i], ref tcdt[i], ref canma, ref canmb);
                                    _leaftSave.Etcan[j][i] = -b1lv * (wcandt[i] - wcan[i]);
                                }
                            }
                            tlcdt[j][i] = -(swcan[j][i] + lwcan[j][i] - Constn.Lv * _leaftSave.Etcan[j][i] + Constn.Rhoa * Constn.Ca * _clayrs.Canlai[j][i] * avgtmp[i] / _leaftSave.Rhcan[j][i] + _clayrs.Drycan[j][i] * (Constn.Cr + wcan[i] * Constn.Cl) * tlc[j][i] / _timewt.Dt) / a1;
                        }
                        //
                        Vslope(ref vtslop[i], ref satv, ref tlcdt[j][i]);
                        //
                        //*****         BEGIN ITERATIONS TO FIND LEAF TEMP AND WATER CONTENT
                        iter1 = 0;
                    label40:;
                        _leaftSave.Etcan[j][i] = -b1lv * (wcandt[i] - wcan[i]);
                        //              COMPUTE HUMIDITY IN PLANT MATERIAL AT END OF TIME STEP
                        Canhum(1, ref humid[i], ref hwslop, ref wcandt[i], ref tcdt[i], ref canma, ref canmb);
                        //***           SET UP AND SOLVE 2X2 MATRIC FOR NEWTON-RAPHSON APPROX.
                        //              FOR LEAF TEMP AND WATER CONTENT
                        a2 = _clayrs.Canlai[j][i] * humid[i] * vtslop[i] / _leaftSave.Rvcan[j][i];
                        b2 = _clayrs.Canlai[j][i] * satv * hwslop / _leaftSave.Rvcan[j][i] + b1lv;
                        ff1 = swcan[j][i] + lwcan[j][i] - Constn.Lv * _leaftSave.Etcan[j][i] - a1lwr[j][i] * (tlcdt[j][i] - _canlwr.Tlclwr[j][i]) - _clayrs.Canlai[j][i] * Constn.Rhoa * Constn.Ca * (tlcdt[j][i] - avgtmp[i]) / _leaftSave.Rhcan[j][i] - _clayrs.Drycan[j][i] * (Constn.Cr + wcan[i] * Constn.Cl) * (tlcdt[j][i] - tlc[j][i]) / _timewt.Dt;
                        ff2 = _clayrs.Canlai[j][i] * (humid[i] * satv - avgvap[i]) / _leaftSave.Rvcan[j][i] - _leaftSave.Etcan[j][i];
                        det = a1 * b2 - a2 * b1;
                        d1 = (ff1 * b2 - ff2 * b1) / det;
                        d2 = (a1 * ff2 - a2 * ff1) / det;
                        //
                        //***           UPDATE VALUES
                        tlcdt[j][i] = tlcdt[j][i] - d1;
                        wcandt[i] = wcandt[i] - d2;
                        //              CHECK IF WATER CONTENT IS REASONABLE --
                        Vslope(ref vtslop[i], ref satv, ref tlcdt[j][i]);
                        humcan = avgvap[i] / satv;
                        Canhum(2, ref humcan, ref dummy, ref wchum, ref tcdt[i], ref canma, ref canmb);
                        delmax = wcan[i] - wchum;
                        delta = wcandt[i] - wchum;
                        //CCCC          IF(DELTA*DELMAX.LT.0.0 .OR.ABS(DELMAX).LT.ABS(DELTA))THEN
                        if (delta * delmax < 0.0)
                        {
                            //                 WATER CONTENT WENT BEYOND EQUILIBRUIM WITH HUMIDITY
                            _leaftSave.Etcan[j][i] = -b1lv * (wchum - wcan[i]);
                            Canhum(1, ref hum, ref dummy, ref wcan[i], ref tcdt[i], ref canma, ref canmb);
                            etmax = _clayrs.Canlai[j][i] * (hum * satv - avgvap[i]) / _leaftSave.Rvcan[j][i];
                            if (Math.Abs(_leaftSave.Etcan[j][i]) > Math.Abs(etmax)) _leaftSave.Etcan[j][i] = etmax;
                            humid[i] = (avgvap[i] + _leaftSave.Etcan[j][i] * _leaftSave.Rvcan[j][i] / _clayrs.Canlai[j][i]) / satv;
                            Canhum(2, ref humid[i], ref dummy, ref wcandt[i], ref tcdt[i], ref canma, ref canmb);
                        }
                        if (Math.Abs(d1) > 0.01)
                        {
                            iter1 = iter1 + 1;
                            if (iter1 < 10) goto label40;
                        }
                        //              CALCULATE EVAPORATION FROM THIS LAYER
                        _leaftSave.Etcan[j][i] = -b1lv * (wcandt[i] - wcan[i]);
                        sumet = sumet + _leaftSave.Etcan[j][i];
                    label45:;
                    }
                    _leaftSave.Init[j] = 2;
                }
            //
            //********STORE EVAPORATION/TRANSPIRATION FROM PLANT SPECIES
            label48:;
                trnsp[j] = fractn * sumet + sumev;
                //
                //        SUM HEAT AND WATER TRANSFER IN EACH LAYER FROM ALL CANOPY TYPES
                //
                for (var i = 1; i <= nc; ++i)
                {
                    if (_clayrs.Canlai[j][i] <= 0.0) goto label50;
                    heatc[i] = heatc[i] + _clayrs.Canlai[j][i] * Constn.Rhoa * Constn.Ca * (tlcdt[j][i] - avgtmp[i]) / _leaftSave.Rhcan[j][i];
                    etlyr[i] = etlyr[i] + fractn * _leaftSave.Etcan[j][i] + pevap[i];
                    dheatc[i] = dheatc[i] + _clayrs.Canlai[j][i] * Constn.Rhoa * Constn.Ca / _leaftSave.Rhcan[j][i];
                    detlyr[i] = detlyr[i] + _clayrs.Canlai[j][i] / (rstom[i] + _leaftSave.Rvcan[j][i]);
                    dtldtc[i] = dtldtc[i] + Constn.Lv * humid[i] * vtslop[i] * _clayrs.Canlai[j][i] / (rstom[i] + _leaftSave.Rvcan[j][i]);
                label50:;
                }
            label60:;
            }

            //xOUT
            for (var i = 1; i <= nc; ++i)
            {
                _writeit.Tleaf[nplant + 1][i] = 0.0;
                for (var j = 1; j <= nplant; ++j)
                {
                    _writeit.Tleaf[j][i] = tlcdt[j][i];
                    if (_clayrs.Canlai[j][i] <= 0.0) _writeit.Tleaf[j][i] = 0.0;
                    _writeit.Tleaf[nplant + 1][i] = _writeit.Tleaf[nplant + 1][i] + tlcdt[j][i] * _radcan.Difkl[j][i] / _radcan.Difkl[nplant + 1][i];
                }
            }
            //xOUT
            //
            //     CALCULATE CHANGE OF LEAF TEMP WITH RESPECT TO CANOPY TEMP
            for (var i = 1; i <= nc; ++i)
            {
                dtldtc[i] = dheatc[i] / (dheatc[i] + dtldtc[i]);
            label70:;
            }
        }

        // line 6741
        private void Stomates(ref int j, ref double rstom, ref double drsdplf, ref double pleaf, ref double swdown, ref double tc, ref double vapdef, double[] rstom0, double[] pleaf0, double[] rstexp, ref int istomate, double[][] stomate)
        {
            //
            //     THIS SUBROUTINES COMPUTES THE STOMATAL RESISTANCE AND DERIVATIVE
            //     OF STOMALAL RESISANCE WITH RESPECT TO LEAF POTENTIAL (DRSDPLF)
            //     INFLUENCING FACTORS INCLUDE LEAF WATER POTENTIAL, SOLAR RADIATION,
            //     AIR TEMPERATURE, AND VAPOR PRESSURE DEFICIT.
            //
            //***********************************************************************

            var fstom = 0.0;
            var fvpd = 0.0;
            var vdefkpa = 0.0;
            var ftemp = 0.0;
            var fsolar = 0.0;
            if (istomate == 2)
            {
                //        USE STEWART-JARVIS EQUATIONS FOR SOLAR,TEMPERATURE AND VPD
                //        -- SOLAR RADIATION INFLUENCE
                if (swdown + stomate[j][1] <= 0.0)
                {
                    //           NO INFLUENCE OF SOLAR ON STOMATAL CONDUCTANCE
                    fsolar = 1.0;
                }
                else
                {
                    //           REDUCE STOMATAL CONDUCTANCE BASED ON SOLAR
                    if (swdown < 1000.0)
                    {
                        fsolar = swdown * (1000.0 + stomate[j][1]) / (1000.0 * (swdown + stomate[j][1]));
                    }
                    else
                    {
                        //              DO NOT ALLOW FACTOR TO GO ABOVE 1.0
                        fsolar = 1.0;
                    }
                }
                if (tc > stomate[j][2] && tc < stomate[j][3])
                {
                    ftemp = (tc - stomate[j][2]) * Math.Pow((stomate[j][3] - tc), stomate[j][10]) / (stomate[j][4] - stomate[j][2]) / Math.Pow((stomate[j][3] - stomate[j][4]), stomate[j][10]);
                }
                else
                {
                    //           OUTSIDE TRANPIRATION RANGE - SHUT DOWN STOMATES
                    ftemp = 0.0001;
                }
                //        CONVERT VPD (OF AIR, NOT LEAF TEMP) FROM KG/M3 TO kPA
                vdefkpa = Constn.Ugas * vapdef * (tc + 273.16) / 0.018 / 1000.0;
                if (vdefkpa < 0.0) vdefkpa = 0.0;
                fvpd = stomate[j][5] + (1.0 - stomate[j][5]) * Math.Pow(stomate[j][6], vdefkpa);
                fstom = fsolar * ftemp * fvpd;
            }
            else
            {
                fstom = 1.0;
            }
            //
            //     COMPUTE STOMATAL RESISTANCE
            rstom = rstom0[j] * (1.0 + Math.Pow((pleaf / pleaf0[j]), rstexp[j])) / fstom;
            //     COMPUTE DERIVIATIVE OF Rs WITH RESPECT TO LEAF WATER POTENTIAL
            drsdplf = rstom0[j] * rstexp[j] * Math.Pow((pleaf / pleaf0[j]), (rstexp[j] - 1.0)) / pleaf0[j] / fstom;
        }

        // line 6800
        private void Canhum(int ncalc, ref double hum, ref double dhdw, ref double wcan, ref double tc, ref double canma, ref double canmb)
        {
            //
            //     THIS SUBROUTINE DEFINES THE RELATION BETWEEN THE HUMIDITY AND THE
            //     MOISTURE CONTENT OF THE DEAD CANOPY MATERIAL USING THE CONVERSION
            //     BETWEEN WATER POTENTIAL AND HUMIDITY.  DEPENDING ON THE VALUE OF
            //     NCALC, THE SUBROUTINE WILL EITHER CALCULATE HUMIDITY AND THE
            //     DERIVATIVE OF HUMIDTY WITH RESPECT TO WATER CONTENT FOR A GIVEN
            //     WATER CONTENT, OR AN EQUILIBRIUM WATER CONTENT GIVEN THE HUMIDITY.
            //     (NCALC = 1: CALCULATE HUMIDITY; OTHERWISE CALCULATE WATER CONTENT)

            if (ncalc == 1)
            {
                //         CALCULATE HUMIDITY AND SLOPE BASED ON WATER CONTENT
                if (wcan <= 0.0)
                {
                    //            EQUATIONS CANNOT HANDLE CASE WHERE WCAN=0.0 -- SET HUM=0.0
                    hum = 0.0;
                    dhdw = 0.0;
                }
                else
                {
                    // 
                    hum = Math.Exp(0.018 * Constn.G / (Constn.Ugas * (tc + 273.16)) * canma * Math.Pow(wcan, (-canmb)));
                    dhdw = -0.018 * Constn.G * canma * canmb * hum * Math.Pow(wcan, (-canmb - 1.0)) / (Constn.Ugas * (tc + 273.16));
                }
            }
            else
            {
                //         CALCULATE WATER CONTENT BASED ON HUMIDITY
                if (hum < 0.999)
                {
                    if (hum <= 0.0)
                    {
                        wcan = 0.0;
                    }
                    else
                    {
                        wcan = Math.Pow((Math.Log(hum) * Constn.Ugas * (tc + 273.16) / 0.018 / Constn.G / canma), (-1.0 / canmb));
                    }
                }
                else
                {
                    wcan = Math.Pow((Math.Log(0.999) * Constn.Ugas * (tc + 273.16) / 0.018 / Constn.G / canma), (-1.0 / canmb));
                }
            }
        }

        // line 6845
        /// <summary>
        /// sets con
        /// </summary>
        /// <param name="iter"></param>
        /// <param name="nc"></param>
        /// <param name="con"></param>
        /// <param name="tc"></param>
        /// <param name="zc"></param>
        private void Cantk(ref int iter, ref int nc, double[] con, double[] tc, double[] zc)
        {
            //
            //     THIS SUBROUTINE COMPUTES THE EDDY CONDUCTANCE COEFFICIENT THROUGH
            //     THE CANOPY
            //
            //***********************************************************************
            //

            var richrd = 0.0;
            var ustar1 = 0.0;
            var stabl = 0.0;
            var phiw = 0.0;
            var phih = 0.0;
            var tl2 = 0.0;
            var sigma2 = 0.0;
            var hceff = 0.0;
            var btm = 0.0;
            var sigma = 0.0;
            var tl = 0.0;
            var tkcan = 0.0;
            var ri = 0.0;
            var top = 0.0;
            var tl1 = 0.0;
            var sigma1 = 0.0;
            var resist = 0.0;

            richrd = _windv.Stable;
            ustar1 = _windv.Ustar;
            if (richrd > 1.0) richrd = 1.0;
            if (richrd < -2.0) richrd = -2.0;
            stabl = richrd;
            if (stabl >= 0)
            {
                //        STABLE CONDITIONS
                phiw = 1.25 * (1.0 + 0.2 * stabl);
                phih = 1.0 + 5.0 * stabl;
            }
            else
            {
                //        UNSTABLE CONDITIONS
                phiw = 1.25 * Math.Pow((1.0 - 3.0 * stabl), 0.33);
                phih = 1.0 / Math.Sqrt(1 - 16.0 * stabl);
            }
            //     COMPUTE EFF. CANOPY HEIGHT BASED ON ZERO PLANE DISPLACEMENT
            hceff = _windv.Zero / 0.77;
            btm = zc[nc + 1];
            if (btm + _windv.Zhsub > 2.3 * hceff)
            {
                //        ABOVE INERTIAL SUBLAYER -> FORCE TO k(z+zH-d)u*
                tl2 = Constn.Vonkrm * (btm + _windv.Zh - _windv.Zero);
                sigma2 = 1.0;
            }
            else if (btm + _windv.Zhsub > 1.5 * hceff)
            {
                //        INTERPOLATE SIGMA FROM 1.1 AT Z/H=1.5 TO 1.25 AT Z/H=2.3
                sigma2 = 1.1 + 0.15 * ((btm + _windv.Zhsub) / hceff - 1.5) / 0.8;
                tl2 = 0.4 * hceff;
            }
            else
            {
                //        INTERPOLATE SIGMA FROM 0.664 AT Z/H=0.8 TO 1.1 AT Z/H=1.5
                sigma2 = 0.664 + 0.436 * ((btm + _windv.Zhsub) / hceff - 0.8) / 0.7;
                tl2 = 0.4 * hceff;
            }
            //
            for (var i = 1; i <= nc; ++i)
            {
                //        USE GRADIENT RICHARDSON'S NUMBER
                ri = Constn.G * (tc[i] - tc[i + 1]) * (zc[i + 1] - zc[i]) / (tc[i] + 273.16) / Math.Pow((_windv.Windc[i] - _windv.Windc[i + 1]), 2);
                if (ri <= 0.0)
                {
                    stabl = ri;
                }
                else
                {
                    if (ri > 0.175) ri = 0.175;
                    stabl = ri / (1.0 - 5.0 * ri);
                }
                if (stabl > 1.0) stabl = 1.0;
                if (stabl < -2.0) stabl = -2.0;
                //
                if (stabl >= 0)
                {
                    phiw = 1.0 + 0.2 * stabl;
                    phih = 1.0 + 5.0 * stabl;
                }
                else
                {
                    //           UNSTABLE CONDITIONS
                    phiw = Math.Pow((1.0 - 3.0 * stabl), 0.33);
                    phih = 1.0 / Math.Sqrt(1 - 16.0 * stabl);
                }
                if (i == nc)
                {
                    //           USE GROUND USTAR AT SURFACE
                    if (iter <= 1)
                    {
                        _cantkSave.Zmlog = Math.Log((zc[nc + 1] - zc[nc] + _windv.Zmsub - _windv.Zersub) / _windv.Zmsub);
                    }
                    ustar1 = _windv.Windc[nc] * Constn.Vonkrm / _cantkSave.Zmlog;
                }
                //
                top = btm;
                tl1 = tl2;
                sigma1 = sigma2;
                btm = zc[nc + 1] - zc[i + 1];
                //        SUM RESISTANCES FOR CURRENT LAYER
                resist = 0.0;
                //
                if (btm + _windv.Zhsub > 2.3 * hceff)
                {
                    //          ABOVE INERTIAL SUBLAYER -> FORCE TO k(z+zH-d)u*
                    tl2 = Constn.Vonkrm * (btm + _windv.Zh - _windv.Zero);
                    sigma2 = 1.0;
                    sigma = (sigma1 + sigma2) * ustar1 * phiw / 2.0;
                    tl = ((tl1 + tl2) / ustar1 / phih / Math.Pow(phiw, 2)) / 2.0;
                    tkcan = sigma * sigma * tl;
                    resist = resist + (top - btm) / tkcan;
                    goto label10;
                }
                else
                {
                    //          COMPUTE CONDUCTANCE VALUES AT 2.3*HCEFF
                    btm = 2.3 * hceff - _windv.Zhsub;
                    tl2 = Constn.Vonkrm * (btm + _windv.Zh - _windv.Zero);
                    sigma2 = 1.0;
                    tkcan = (tl1 + tl2) * ustar1 / phih / 2.0;
                    resist = resist + (top - btm) / tkcan;
                    top = btm;
                    btm = zc[nc + 1] - zc[i + 1];
                    tl1 = 0.4 * hceff;
                    sigma1 = 1.25;
                }
                if (btm + _windv.Zhsub > 1.5 * hceff)
                {
                    //          INTERPOLATE SIGMA FROM 1.1 AT Z/H=1.5 TO 1.25 AT Z/H=2.3
                    sigma2 = 1.1 + 0.15 * ((btm + _windv.Zhsub) / hceff - 1.5) / 0.8;
                    tl2 = 0.4 * hceff;
                    sigma = (sigma1 + sigma2) * ustar1 * phiw / 2.0;
                    tl = ((tl1 + tl2) / ustar1 / phih / Math.Pow(phiw, 2)) / 2.0;
                    tkcan = sigma * sigma * tl;
                    resist = resist + (top - btm) / tkcan;
                    goto label10;
                }
                else
                {
                    //          COMPUTE CONDUCTANCE VALUES AT 1.5*HCEFF
                    btm = 1.5 * hceff - _windv.Zhsub;
                    sigma2 = 1.1;
                    tl2 = 0.4 * hceff;
                    sigma = ((sigma1 + sigma2) * ustar1 * phiw) / 2.0;
                    tl = ((tl1 + tl2) / ustar1 / phih / Math.Pow(phiw, 2)) / 2.0;
                    tkcan = sigma * sigma * tl;
                    resist = resist + (top - btm) / tkcan;
                    top = btm;
                    btm = zc[nc + 1] - zc[i + 1];
                    tl1 = tl2;
                    sigma1 = sigma2;
                }
                if (btm + _windv.Zhsub > 0.8 * hceff)
                {
                    //          INTERPOLATE SIGMA FROM 0.664 AT Z/H=0.8 TO 1.1 AT Z/H=1.5
                    sigma2 = 0.664 + 0.436 * ((btm + _windv.Zhsub) / hceff - 0.8) / 0.7;
                    tl2 = 0.4 * hceff;
                    sigma = ((sigma1 + sigma2) * ustar1 * phiw) / 2.0;
                    tl = ((tl1 + tl2) / ustar1 / phih / Math.Pow(phiw, 2)) / 2.0;
                    tkcan = sigma * sigma * tl;
                    resist = resist + (top - btm) / tkcan;
                    goto label10;
                }
                else
                {
                    //          COMPUTE CONDUCTANCE AT 0.8HCEFF
                    btm = 0.8 * hceff - _windv.Zhsub;
                    sigma2 = 0.664;
                    tl2 = 0.4 * hceff;
                    sigma = ((sigma1 + sigma2) * ustar1 * phiw) / 2.0;
                    tl = ((tl1 + tl2) / ustar1 / phih / Math.Pow(phiw, 2)) / 2.0;
                    tkcan = sigma * sigma * tl;
                    resist = resist + (top - btm) / tkcan;
                    top = btm;
                    btm = zc[nc + 1] - zc[i + 1];
                    tl1 = tl2;
                    sigma1 = sigma2;
                }
                if (btm + _windv.Zhsub > 0.25 * hceff)
                {
                    //          COMPUTE CONDUCTANCE VALUES BETWEEN 0.8HCEFF AND 0.25HCEFF
                    sigma2 = 0.2 * Math.Exp(1.5 * (btm + _windv.Zhsub) / hceff);
                    tl2 = 0.4 * hceff;
                    sigma = (sigma1 + sigma2) * ustar1 * phiw / 2.0;
                    tl = ((tl1 + tl2) / ustar1 / phih / Math.Pow(phiw, 2)) / 2.0;
                    tkcan = sigma * sigma * tl;
                    resist = resist + (top - btm) / tkcan;
                    goto label10;
                }
                else
                {
                    //          COMPUTE CONDUCTANCE AT 0.25HCEFF
                    btm = 0.25 * hceff - _windv.Zhsub;
                    sigma2 = 0.29;
                    tl2 = 0.4 * hceff;
                    sigma = ((sigma1 + sigma2) * ustar1 * phiw) / 2.0;
                    tl = ((tl1 + tl2) / ustar1 / phih / Math.Pow(phiw, 2)) / 2.0;
                    tkcan = sigma * sigma * tl;
                    resist = resist + (top - btm) / tkcan;
                    top = btm;
                    btm = zc[nc + 1] - zc[i + 1];
                    tl1 = tl2;
                    sigma1 = sigma2;
                }
                //        COMPUTE CONDUCTANCE VALUES BELOW 0.25*HCEFF
                sigma2 = 0.2 * Math.Exp(1.5 * (btm + _windv.Zhsub) / hceff);
                tl2 = 1.6 * (btm + _windv.Zhsub);
                sigma = (sigma1 + sigma2) * ustar1 * phiw / 2.0;
                tl = ((tl1 + tl2) / ustar1 / phih / Math.Pow(phiw, 2)) / 2.0;
                tkcan = sigma * sigma * tl;
                resist = resist + (top - btm) / tkcan;
            //        COMPUTE OVERALL CONDUCTANCE OF LAYER
            label10:;
                tkcan = Constn.Rhoa * Constn.Ca / resist;
                if (tkcan < Constn.Tka / (zc[i + 1] - zc[i])) tkcan = Constn.Tka / (zc[i + 1] - zc[i]);
                con[i] = tkcan;
                btm = zc[nc + 1] - zc[i + 1];
            label20:;
            }
        }

        // line 7040
        private void Stab(ref int iter, ref int nc, ref double zcan, ref double tmpair, ref double tmpsfc, ref double wind, ref double height, ref double zmlog, ref double zhlog, ref double zclog, ref double hflux, ref double ustar, ref double stable, ref double psim, ref double psih, ref double rh, ref double zero)
        {
            //
            //***********************************************************************

            const double htoler = 0.001;

            var hflux1 = 0.0;
            var zref = 0.0;
            var hflux2 = 0.0;
            var rhzc = 0.0;
            var error = 0.0;

            //**** DEFINE INITIAL ASSUMPTIONS AND CONSTANTS FOR CURRENT ITERATION
            ustar = wind * Constn.Vonkrm / (zmlog + psim);
            hflux1 = hflux;
            zref = zero;
            if (nc > 0) zref = zcan;
            //
            var iter1 = 0;
        //
        //**** START ITERATIVE PROCESS TO OBTAIN HEAT FLUX
        label10:;
            iter1 = iter1 + 1;
            stable = Constn.Vonkrm * (height - zero) * Constn.G * hflux1 / (Constn.Rhoa * Constn.Ca * (tmpair + 273.16) * Math.Pow(ustar, 3));
            //
            if (stable >= 0.0)
            {
                //****    ATMOSPHERE IS STABLE
                //        IF STABILITY IS GREATER THAN ZMLOG/9.4, COMPUTED HFLUX WILL
                //        ACTUALLY DECREASE WITH INCREASING TEMPERATURE DIFFERENTIAL,
                //        CAUSING NUMERICAL INSTABILITY AND CONVERGENCE PROBLEMS -- AVOID
                //        THIS SITUATION BY LIMITING STABLE TO ZMLOG/9.4
                if (stable > zmlog / 9.4) stable = zmlog / 9.4;
                psih = 4.7 * stable;
                psim = psih;
            }
            else
            {
                //****    ATMOSPHERE IS UNSTABLE
                psih = -2.0 * Math.Log((1 + Math.Pow((1 - 16.0 * stable), 0.5)) / 2.0);
                psim = 0.6 * psih;
                //        IF ATMOSPHERE IS SUFFICIENTLY UNSTABLE, EQUATIONS MAY RESULT
                // 
                //        IN FRICTION VELOCITY LESS THEN ZERO 
                if (psim / zmlog < -0.50)
                {
                    psim = -0.50 * zmlog;
                    psih = psim / 0.6;
                    stable = -(Math.Pow((2.0 * Math.Exp(-psih / 2.0) - 1), 2) - 1) / 16.0;
                }
            }
            ustar = wind * Constn.Vonkrm / (zmlog + psim);
            rh = (zhlog + psih) / (ustar * Constn.Vonkrm);
            if (nc > 0)
            {
                //        RH IS TO ZERO PLANE DISPLACEMENT - ADJUST TO TOP OF CANOPY
                rhzc = rh * (1.0 - (zclog / zhlog));
                hflux2 = Constn.Rhoa * Constn.Ca * (tmpair - tmpsfc) / rhzc;
            }
            else
            {
                hflux2 = Constn.Rhoa * Constn.Ca * (tmpair - tmpsfc) / rh;
            }
            error = Math.Abs(hflux2 - hflux1);
            hflux1 = hflux2;
            if (Math.Abs(hflux1) < Math.Abs(Constn.Tka * (tmpair - tmpsfc) / (height - zref))) goto label15;
            //        WIND IS SUFFICIENTLY SMALL, OR ATMOSPHERE IS SUFFICIENTLY
            //        STABLE THAT TURBULENCE DOES NOT INCREASE FLUX -- NO NEED TO
            //        ITERATE FURTHER
            //
            //**** CHECK IF TOLERANCE HAS BEEN MET
            if (iter1 > 40)
            {
                //        CONVERGENCE NOT MET, BUT PROBABLY CLOSE ENOUGH
                goto label15;
            }
            if (error > htoler) goto label10;
            //
            label15:;
            hflux = hflux1;
        }

        // line 7113
        private void Ebsnow(ref int n, ref int nsp, ref int nr, int[] icespt, double[] tsp, double[] tspdt, double[] dlw, double[] dlwdt, double[] rhosp, double[] zsp, double[] dzsp, 
            double[] qvsp, ref double vapsp, ref double vapspt, double[] ssp, ref int iter)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE JACOBIAN MATRIX COEFFICIENTS FOR
            //     THE SNOW PORTION OF THE NEWTON-RAPHSON SOLUTION OF THE ENERGY
            //     BALANCE
            //
            //***********************************************************************
            var qvspdt = new double[101];
            var tk = new double[101];
            var conv = new double[101];
            var csp = new double[101];
            var cspdt = new double[101];
            var slope = new double[101];

            var nsp1 = 0;
            var j = 0;

            //**** DETERMINE THE VAPOR FLUX BETWEEN NODES
            if (iter == 1) Qvsnow(ref nsp, _ebsnowSave.Qvspt, conv, slope, tsp, zsp, ref vapsp);
            Qvsnow(ref nsp, qvspdt, conv, slope, tspdt, zsp, ref vapspt);
            //     IF UNDERLYING MATERIAL IS RESIDUE, VAPOR DENSITY IS NOT A FUNCTION
            //     OF TEMPERATURE, I.E.  SLOPE(NSP+1) = 0.0
            if (nr > 0) slope[nsp + 1] = 0.0;
            //
            //**** OBTAIN THE AVERAGE VAPOR FLUX OVER THE TIME STEP
            Weight(nsp, qvsp, _ebsnowSave.Qvspt, qvspdt);
            //
            if (iter == 1)
            {
                //        (DENSITY AND THERMAL CONDUCTIVITY ARE CONSTANT OVER TIME STEP)
                //****    DETERMINE THE THERMAL CONDUCTIVITY OF EACH NODE
                Snowtk(ref nsp, tk, rhosp);
                tk[nsp + 1] = tk[nsp];
                //
                //****    DETERMINE THE CONDUCTANCE TERM BETWEEN NODES
                nsp1 = nsp + 1;
                Conduc(nsp1, zsp, tk, _ebsnowSave.Con);
            }

            //
            //**** CALCULATE THE SPECIFIC HEAT OF EACH NODE

            if (iter == 1) Snowht(ref nsp, _ebsnowSave.Cspt, tsp, rhosp);
            Snowht(ref nsp, cspdt, tspdt, rhosp);

            //
            //**** OBTAIN THE AVERAGE SPECIFIC HEAT OVER THE TIME STEP
            Weight(nsp, csp, _ebsnowSave.Cspt, cspdt);
            //
            //
            //**** DETERMINE THE COEFFICIENTS FOR THE SURFACE LAYER
            _matrix.D1[n] = _matrix.D1[n] - _ebsnowSave.Con[1] * (_timewt.Wt * (tsp[1] - tsp[2]) + _timewt.Wdt * (tspdt[1] - tspdt[2])) + ssp[1] - csp[1] * (tspdt[1] - tsp[1]) * dzsp[1] / _timewt.Dt - Constn.Rhol * Constn.Lf * (dlwdt[1] - dlw[1]) / _timewt.Dt - Constn.Ls * qvsp[1];
            //
            if (icespt[1] == 0)
            {
                //        LAYER IS NOT MELTING - ENERGY BUDGET BASED ON TEMPERATURE
                _matrix.A1[n + 1] = _timewt.Wdt * (_ebsnowSave.Con[1] + conv[1] * Constn.Ls * slope[1]);
                _matrix.B1[n] = _matrix.B1[n] - _timewt.Wdt * (_ebsnowSave.Con[1] + conv[1] * Constn.Ls * slope[1]) - dzsp[1] * csp[1] / _timewt.Dt;
            }
            else
            {
                //
                //        LAYER IS MELTING - ENERGY BUDGET BASED ON WATER CONTENT
                _matrix.A1[n + 1] = 0.0;
                _matrix.B1[n] = _matrix.B1[n] - Constn.Rhol * Constn.Lf / _timewt.Dt;
                //        IF SNOW IS NOT FIRST MATERIAL, SET DERIV. FOR LAST NODE TO 0.0
                if (n > 1) _matrix.C1[n - 1] = 0.0;
            }
            //
            //**** DETERMINE THE COEFFICIENTS FOR THE REMAINDER OF THE SNOWPACK
            for (var i = n + 1; i <= n + nsp - 1; ++i)
            {
                j = i - n + 1;
                _matrix.D1[i] = _ebsnowSave.Con[j - 1] * (_timewt.Wt * (tsp[j - 1] - tsp[j]) + _timewt.Wdt * (tspdt[j - 1] - tspdt[j])) - _ebsnowSave.Con[j] * (_timewt.Wt * (tsp[j] - tsp[j + 1]) + _timewt.Wdt * (tspdt[j] - tspdt[j + 1])) + ssp[j] - csp[j] * (tspdt[j] - tsp[j]) * dzsp[j] / _timewt.Dt - Constn.Rhol * Constn.Lf * (dlwdt[j] - dlw[j]) / _timewt.Dt - Constn.Ls * (qvsp[j] - qvsp[j - 1]);
                //
                if (icespt[j] == 0)
                {
                    //           LAYER IS NOT MELTING - ENERGY BUDGET BASED ON TEMPERATURE
                    _matrix.A1[i + 1] = _timewt.Wdt * (_ebsnowSave.Con[j] + conv[j] * Constn.Ls * slope[j]);
                    _matrix.B1[i] = -_timewt.Wdt * (_ebsnowSave.Con[j - 1] + _ebsnowSave.Con[j]) - _timewt.Wdt * Constn.Ls * slope[j] * (conv[j - 1] + conv[j]) - dzsp[j] * csp[j] / _timewt.Dt;
                    _matrix.C1[i - 1] = _timewt.Wdt * (_ebsnowSave.Con[j - 1] + conv[j - 1] * Constn.Ls * slope[j]);
                }
                else
                {
                    //
                    //           LAYER IS MELTING - ENERGY BUDGET BASED ON WATER CONTENT
                    _matrix.A1[i + 1] = 0.0;
                    _matrix.B1[i] = -Constn.Rhol * Constn.Lf / _timewt.Dt;
                    _matrix.C1[i - 1] = 0.0;
                }
            label20:;
            }
            //
            //
            //**** DETERMINE THE BOUNDARY CONDITIONS FOR TOP LAYER OF NEXT MATERIAL
            n = n + nsp;
            //
            if (nr > 0)
            {
                //        SNOW OVERLYING RESIDUE
                //        CHECK IF LAST SNOW NODE IS MELTING - IF SO, ENERGY BALANCE
                //        IS BASED ON WATER CONTENT, NOT TEMPERATURE AND A1(N)=0.0
                if (icespt[nsp] == 0) _matrix.A1[n] = _timewt.Wdt * _ebsnowSave.Con[nsp];
                _matrix.B1[n] = -_timewt.Wdt * _ebsnowSave.Con[nsp];
                _matrix.C1[n - 1] = _matrix.C1[n - 1] + _timewt.Wdt * _ebsnowSave.Con[nsp];
                _matrix.D1[n] = _ebsnowSave.Con[nsp] * (_timewt.Wt * (tsp[nsp] - tsp[nsp + 1]) + _timewt.Wdt * (tspdt[nsp] - tspdt[nsp + 1]));
                //
            }
            else
            {
                //        SNOW IS LYING ON BARE SOIL - INCLUDE LATENT HEAT TRANSFER
                //        AND VAPOR FLUX DEPENDENCE ON TEMPERATURE OF SOIL SURFACE
                //        CHECK IF LAST SNOW NODE IS MELTING - IF SO, ENERGY BALANCE
                //        IS BASED ON WATER CONTENT, NOT TEMPERATURE AND A1(N)=0.0
                if (icespt[nsp] > 0) _matrix.A1[n] = _timewt.Wdt * (_ebsnowSave.Con[nsp] + conv[nsp] * Constn.Lv * slope[nsp]);
                _matrix.B1[n] = -_timewt.Wdt * (_ebsnowSave.Con[nsp] + conv[nsp] * Constn.Lv * slope[nsp + 1]);
                _matrix.C1[n - 1] = _timewt.Wdt * (_ebsnowSave.Con[nsp] + conv[nsp] * Constn.Ls * slope[nsp + 1]);
                _matrix.D1[n] = _ebsnowSave.Con[nsp] * (_timewt.Wt * (tsp[nsp] - tsp[nsp + 1]) + _timewt.Wdt * (tspdt[nsp] - tspdt[nsp + 1])) + Constn.Lv * qvsp[nsp];
            }
        }

        // line 7241
        private void Qvsnow(ref int nsp, double[] qvsp, double[] conv, double[] slope, double[] tsp, double[] zsp, ref double vapsp)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE VAPOR DIFFUSION IN THE SNOWPACK
            //
            //***********************************************************************
            var vapsno = new double[101];
            var vapice = new double[101];

            //**** DETERMINE THE VAPOR DIFFUSIVITY, DENSITY AND SLOPE OF EACH NODE
            var satv = 0.0;
            for (var i = 1; i <= nsp; ++i)
            {
                vapsno[i] = Spprop.Vdifsp * (Constn.P0 / _constn.Presur) * Math.Pow((1.0 + tsp[i] / 273.16), Spprop.Vapspx);
                Vslope(ref slope[i], ref satv, ref tsp[i]);
                //        CALCULATE THE SATURATED VAPOR DENSITY OVER ICE
                var tmp = tsp[i] + 273.16;
                var humid = Math.Exp(0.018 / (Constn.Ugas * tmp) * Constn.Lf * tsp[i] / tmp);
                vapice[i] = humid * satv;
                slope[i] = humid * slope[i];
            label10:;
            }
            vapice[nsp + 1] = vapsp;
            vapsno[nsp + 1] = vapsno[nsp];
            Vslope(ref slope[nsp + 1], ref satv, ref tsp[nsp + 1]);
            slope[nsp + 1] = slope[nsp + 1] * vapsp / satv;
            //
            //**** DETERMINE THE CONDUCTANCE TERM BETWEEN NODES
            var nsp1 = nsp + 1;
            Conduc(nsp1, zsp, vapsno, conv);
            //
            //**** CALCULATE THE VAPOR FLUX BETWEEN EACH NODE
            for (var i = 1; i <= nsp; ++i)
            {
                qvsp[i] = conv[i] * (vapice[i] - vapice[i + 1]);
            label20:;
            }
        }

        // line 7282
        private void Snowtk(ref int nsp, double[] tk, double[] rhosp)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE THERMAL CONDUCTIVITY OF THE
            //     SNOWPACK LAYERS.  THE EQUATION USED IS OF THE FORM:
            //
            //           K = A + B*(RHOSP/RHOL)**C
            //
            //     WHERE:    A = TKSPA      B = TKSPB     C = TKSPEX
            //
            //***********************************************************************

            for (var i = 1; i <= nsp; ++i)
            {
                tk[i] = Spprop.Tkspa + Spprop.Tkspb * Math.Pow((rhosp[i] / Constn.Rhol), Spprop.Tkspex);
            label10:;
            }
        }

        // line 7307
        private void Snowht(ref int nsp, double[] csp, double[] tsp, double[] rhosp)
        {
            //
            //     THIS SUBOUTINE CALCULATES THE VOLUMETRIC SPECIFIC HEAT FOR EACH
            //     RESIDUE NODE, INCLUDING LATENT HEAT EFFECTS.
            //
            //***********************************************************************

            for (var i = 1; i <= nsp; ++i)
            {
                //        CALCULATE THE SPECIFIC HEAT OF THE SNOW
                var spheat = 92.96 + 7.37 * (tsp[i] + 273.16);
                //        INCLUDE THE LATENT HEAT TERM
                var s = 0.0; var dummy = 0.0;
                Vslope(ref s, ref dummy, ref tsp[i]);
                //        CALCULATE THE VOLUMETRIC SPECIFIC HEAT INCLUDING LATENT HEAT
                csp[i] = rhosp[i] * spheat + (1.0 - rhosp[i] / Constn.Rhoi) * Constn.Ls * s;
            }
        }

        // line 7332
        private void Ebres(ref int n, ref int nr, ref int nsp, double[] zr, double[] tr, double[] trdt, double[] vapr, double[] vaprdt, double[] gmc, double[] gmcdt, ref double gmcmax, double[] rhor, ref double rescof, ref double restkb, double[] sr, double[] ur, double[] qvr, double[] rhosp, ref int iter)
        {
            //
            //     THIS SUBOUTINE CALCULATES THE NEWTON-RAPHSON COEFFICIENTS FOR THE
            //     ENERGY BALANCE OF THE RESIDUE LAYERS.
            //
            //***********************************************************************
            //
            var cres = new double[11];
            var cresdt = new double[11];
            var tkres = new double[11];
            var convec = new double[11];
            var vapcon = new double[11];
            var con = new double[11];
            var conv = new double[11];

            var nr1 = 0;
            var j = 0;

            //**** DETERMINE THE EVAPORATION FROM RESIDUE ELEMENTS AT EACH NODE
            Resvap(ref nr, _residu.Evap, _residu.Evapk, zr, tr, trdt, vapr, vaprdt, gmc, gmcdt, ref gmcmax, rhor, ref rescof, ur);
            //
            //**** DETERMINE THE HEAT TRANSFER COEFFICIENT FOR EACH NODE
            Restk(ref nr, ref nsp, tkres, convec, tr, trdt, gmc, gmcdt, rhor, ref restkb, rhosp);
            nr1 = nr + 1;
            tkres[nr1] = tkres[nr];
            //
            //**** DETERMINE THE CONDUCTANCE TERM FOR HEAT TRANSPORT BETWEEN NODES
            Conduc(nr1, zr, tkres, con);
            //
            //**** DETERMINE THE VAPOR TRANSPORT FROM THE THERMAL CONVECTION
            Resvk(ref nr, ref nsp, tr, trdt, convec, vapcon);
            vapcon[nr1] = vapcon[nr];
            //
            //**** DETERMINE THE CONDUCTANCE TERM FOR CONVECTIVE VAPOR TRANSPORT
            Conduc(nr1, zr, vapcon, conv);
            //
            //**** DETERMINE THE VAPOR FLUX BETWEEN RESIDUE NODES
            Qvres(ref nr, qvr, conv, vapr, vaprdt);
            //
            //**** DETERMINE THE VOLUMETRIC HEAT CAPACITY OF EACH NODE
            if (iter == 1) Resht(ref nr, _ebresSave.Crest, gmc, rhor);
            Resht(ref nr, cresdt, gmcdt, rhor);
            //
            //**** CALCULATE THE AVERAGE HEAT CAPACITY OVER THE TIME STEP
            Weight(nr, cres, _ebresSave.Crest, cresdt);
            //
            //
            //**** DETERMINE THE MATRIX COEFFICIENTS FOR THE TOP LAYER
            //
            _matrix.A1[n + 1] = _matrix.A1[n + 1] + _timewt.Wdt * con[1];
            _matrix.C1[n] = _matrix.C1[n] + _timewt.Wdt * con[1];
            if (nr > 1)
            {
                _matrix.B1[n] = _matrix.B1[n] - _timewt.Wdt * con[1] - (zr[2] - zr[1]) / (2 * _timewt.Dt) * cres[1];
                _matrix.D1[n] = _matrix.D1[n] - con[1] * (_timewt.Wt * (tr[1] - tr[2]) + _timewt.Wdt * (trdt[1] - trdt[2])) + Constn.Lv * _residu.Evap[1] + sr[1] - (zr[2] - zr[1]) / (2 * _timewt.Dt) * cres[1] * (trdt[1] - tr[1]);
            }
            else
            {
                _matrix.B1[n] = _matrix.B1[n] - _timewt.Wdt * con[1] - (zr[2] - zr[1]) / _timewt.Dt * cres[1];
                _matrix.D1[n] = _matrix.D1[n] - con[1] * (_timewt.Wt * (tr[1] - tr[2]) + _timewt.Wdt * (trdt[1] - trdt[2])) + Constn.Lv * _residu.Evap[1] + sr[1] - (zr[2] - zr[1]) / _timewt.Dt * cres[1] * (trdt[1] - tr[1]);
            }
            //
            //**** DETERMINE THE COEFFICIENTS FOR THE REST OF THE LAYERS
            for (var i = n + 1; i <= n + nr - 1; ++i)
            {
                j = i - n + 1;
                _matrix.A1[i + 1] = _matrix.A1[i + 1] + _timewt.Wdt * con[j];
                _matrix.C1[i] = _matrix.C1[i] + _timewt.Wdt * con[j];
                if (j != nr)
                {
                    _matrix.B1[i] = _matrix.B1[i] - _timewt.Wdt * (con[j - 1] + con[j]) - (zr[j + 1] - zr[j - 1]) / (2 * _timewt.Dt) * cres[j];
                    _matrix.D1[i] = con[j - 1] * (_timewt.Wdt * (trdt[j - 1] - trdt[j]) + _timewt.Wt * (tr[j - 1] - tr[j])) - con[j] * (_timewt.Wdt * (trdt[j] - trdt[j + 1]) + _timewt.Wt * (tr[j] - tr[j + 1])) + Constn.Lv * _residu.Evap[j] + sr[j] - (zr[j + 1] - zr[j - 1]) / (2 * _timewt.Dt) * cres[j] * (trdt[j] - tr[j]);
                }
                else
                {
                    _matrix.B1[i] = _matrix.B1[i] - _timewt.Wdt * (con[j - 1] + con[j]) - (zr[j + 1] - zr[j] + (zr[j] - zr[j - 1]) / 2) / _timewt.Dt * cres[j];
                    _matrix.D1[i] = con[j - 1] * (_timewt.Wdt * (trdt[j - 1] - trdt[j]) + _timewt.Wt * (tr[j - 1] - tr[j])) - con[j] * (_timewt.Wdt * (trdt[j] - trdt[j + 1]) + _timewt.Wt * (tr[j] - tr[j + 1])) + Constn.Lv * _residu.Evap[j] + sr[j] - (zr[j + 1] - zr[j] + (zr[j] - zr[j - 1]) / 2) / _timewt.Dt * cres[j] * (trdt[j] - tr[j]);
                }
            label10:;
            }
            //
            //**** DETERMINE THE COEFFICIENTS FOR THE SOIL SURFACE
            //
            n = n + nr;
            _matrix.B1[n] = _matrix.B1[n] - _timewt.Wdt * con[nr];
            _matrix.D1[n] = con[nr] * (_timewt.Wdt * (trdt[nr] - trdt[nr + 1]) + _timewt.Wt * (tr[nr] - tr[nr + 1])) + Constn.Lv * qvr[nr];
        }

        // line 7434
        private void Resvap(ref int nr, double[] evap, double[] evapk, double[] zr, double[] tr, double[] trdt, double[] vapr, double[] vaprdt, double[] gmc, double[] gmcdt, ref double gmcmax, double[] rhor, ref double rescof, double[] ur)
        {
            //
            //     THIS SUBROUTINE IS TO DETERMINE THE EVAPORATION WITHIN THE
            //     RESIDUE LAYERS ASSUMING THE AVERAGE MOISTURE CONTENT OF THE
            //     RESIDUE IS THE MOISTURE CONTENT AT THE BEGINNING OF THE TIME STEP
            //
            //***********************************************************************
            //

            var humt = 0.0;
            var dummy = 0.0;
            var humdt = 0.0;
            var dhdw = 0.0;
            var satv = 0.0;
            var satvdt = 0.0;
            var dz = 0.0;

            for (var i = 1; i <= nr; ++i)
            {
                //
                //****    DETERMINE THE RELATIVE HUMIDITY IN THE RESIDUE
                Reshum(1, ref humt, ref dummy, ref gmc[i], ref tr[i]);
                Reshum(1, ref humdt, ref dhdw, ref gmcdt[i], ref trdt[i]);
                //
                //****    DETERMINE THE SATURATED VAPOR DENSITY OF THE RESIDUE
                Vslope(ref dummy, ref satv, ref tr[i]);
                Vslope(ref dummy, ref satvdt, ref trdt[i]);
                //
                if (i == 1)
                {
                    dz = zr[2] - zr[1];
                    if (nr != 1) dz = dz / 2.0;
                }
                else
                {
                    if (i == nr)
                    {
                        dz = zr[i + 1] - zr[i] + (zr[i] - zr[i - 1]) / 2.0;
                    }
                    else
                    {
                        dz = (zr[i + 1] - zr[i - 1]) / 2.0;
                    }
                }
                //
                //****    DEFINE EVAPK(I) -- FOR NOW, IT WILL SIMPLY BE SET EQUAL TO
                //        THE INVERSE OF RESCOF, ASSUMING THAT RESCOF IS THE VAPOR
                //        RESISTANCE.  LATER WORK MAY REQUIRE THAT THE VAPOR TRANSPORT
                //        RESISTANCE BE CALCULATED OR OBTAINED FROM A SUBROUTINE.
                evapk[i] = 1.0 / rescof;
                //
                evap[i] = evapk[i] * (_timewt.Wdt * (vaprdt[i] - humdt * satv) + _timewt.Wt * (vapr[i] - humt * satv));
                gmcdt[i] = gmc[i] + evap[i] * _timewt.Dt / dz / rhor[i] + ur[i];
                if (gmcdt[i] < 0.0)
                {
                    gmcdt[i] = 0.0;
                    evap[i] = dz * rhor[i] * (gmcdt[i] - gmc[i] - ur[i]) / _timewt.Dt;
                }
                if (gmcdt[i] > gmcmax)
                {
                    gmcdt[i] = gmcmax;
                    evap[i] = dz * rhor[i] * (gmcdt[i] - gmc[i] - ur[i]) / _timewt.Dt;
                }
            //
            label20:;
            }
        }

        // line 7492
        private void Reshum(int ncalc, ref double hum, ref double dhdw, ref double gmc, ref double tr)
        {
            //
            //     THIS SUBROUTINE DEFINES THE RELATION BETWEEN THE HUMIDITY AND THE
            //     MOISTURE CONTENT OF THE RESIDUE USING THE CONVERSION
            //     BETWEEN WATER POTENTIAL AND HUMIDITY.  DEPENDING ON THE VALUE OF
            //     NCALC, THE SUBROUTINE WILL EITHER CALCULATE HUMIDITY AND THE
            //     DERIVATIVE OF HUMIDTY WITH RESPECT TO WATER CONTENT FOR A GIVEN
            //     WATER CONTENT, OR AN EQUILIBRIUM WATER CONTENT GIVEN THE HUMIDITY.
            //     (NCALC = 1: CALCULATE HUMIDITY; OTHERWISE CALCULATE WATER CONTENT)

            // 
            // **** THIS SUBROUTINE USES FOLLOWING RELATION:
            //                 TENSION = A*W**B
            //                 H=EXP(M*TENSION/(UGAS*TEMP))
            //      THEREFORE  H=EXP(M*A*W**B/(UGAS*TEMP))
            // 
            if (ncalc == 1)
            {
                //         CALCULATE HUMIDITY AND SLOPE BASED ON WATER CONTENT
                if (gmc <= 0.0)
                {
                    //            EQUATIONS CANNOT HANDLE CASE WHERE GMC=0.0 -- SET HUM=0.0
                    hum = 0.0;
                    dhdw = 0.0;
                }
                else
                {
                    hum = Math.Exp(0.018 * Constn.G / (Constn.Ugas * (tr + 273.16)) * Rsparm.Resma * Math.Pow(gmc, (-Rsparm.Resmb)));
                    dhdw = -0.018 * Constn.G * Rsparm.Resma * Rsparm.Resmb * hum * Math.Pow(gmc, (-Rsparm.Resmb - 1.0)) / (Constn.Ugas * (tr + 273.16));
                }
            }
            else
            {
                //         CALCULATE WATER CONTENT BASED ON HUMIDITY
                if (hum > 0.999)
                {
                    gmc = Math.Pow((Math.Log(0.999) * Constn.Ugas * (tr + 273.16) / 0.018 / Constn.G / Rsparm.Resma), (-1.0 / Rsparm.Resmb));
                }
                else
                {
                    gmc = Math.Pow((Math.Log(hum) * Constn.Ugas * (tr + 273.16) / 0.018 / Constn.G / Rsparm.Resma), (-1.0 / Rsparm.Resmb));
                }
            }
        }

        // line 7537
        private void Restk(ref int nr, ref int nsp, double[] tkres, double[] convec, double[] tr, double[] trdt, double[] gmc, double[] gmcdt, double[] rhor, ref double restkb, double[] rhosp)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE THERMAL CONDUCTANCE TERM BETWEEN
            //     RESIDUE NODES USING THE CALCULATED WINDSPEED AT A NODE FOR THE
            //     THE CONVECTIVE TRANSPORT TERM, AND THE THERMAL CONDUCTIVITIES OF
            //     OF THE RESIDUE AND WATER THE THERMAL CONDUCTIVITY TERM.
            //     CONVECTIVE AND CONDUCTIVE TERMS ARE THEN WEIGHTED ACCORDING TO
            //     THE VOLUMETRIC FRACTIONS OF AIR, WATER AND RESIDUE.
            //
            //***********************************************************************
            //
            var tksp = new double[101];

            const double resden = 170.0;

            if (nsp > 0)
            {
                //        ASSUME THE SNOW FILTERS DOWN INTO THE RESIDUE - THEREFORE
                //        REDEFINE THE THERMAL CONVECTION AS A CONDUCTION THRU SNOW
                Snowtk(ref nsp, tksp, rhosp);
            }
            //
            for (var i = 1; i <= nr; ++i)
            {
                //
                //        AVERAGE THE TEMP AND WATER CONTENT OVER TIME
                var avgtmp = _timewt.Wdt * trdt[i] + _timewt.Wt * tr[i];
                var avggmc = _timewt.Wdt * gmcdt[i] + _timewt.Wt * gmc[i];
                //
                //        CALCULATE THE VOLUME FRACTION OF EACH MATERIAL
                var resvol = rhor[i] / resden;
                if (resvol > 1.0) resvol = 1.0;
                var water = avggmc * rhor[i] / Constn.Rhol;
                var air = 1.0 - resvol - water;
                if (air < 0.0) air = 0.0;
                //
                //        IF SNOW IS PRESENT - DO NOT CALCULATE CONVECTION
                if (nsp > 0)
                {
                    convec[i] = tksp[nsp];
                }
                else
                {
                    //
                    //           CALCULATE THE CONVECTIVE HEAT TRANSFER COEFFICIENT
                    convec[i] = Constn.Tka * (1.0 + Rsparm.Restka * avgtmp) * (1.0 + restkb * _windv.Windr[i]);
                }
                //
                //        ADJUST CONVECTIVE TRANSPORT FOR AIR POROSITY
                //        (IN THE CASE OF SNOW, AIR POROSITY IS THE FRACTION OF SNOW)
                convec[i] = convec[i] * air;
                //
                //        CALCULATE THERMAL CONDUCTIVITY THROUGH THE RESIDUE AND WATER
                var tk = water * Constn.Tkl + resvol * Constn.Tkr;
                //
                //        CALCULATE THE EFFECTIVE HEAT TRANSFER COEFFICIENT
                tkres[i] = convec[i] + tk;
            label10:;
            }
        }

        // line 7604
        private void Resvk(ref int nr, ref int nsp, double[] tr, double[] trdt, double[] convec, double[] vapcon)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE VAPOR CONDUCTANCE TERM (CONV)
            //     BETWEEN RESIDUE NODES USING THE CALCULATED THERMAL CONDUCTANCE.
            //
            //***********************************************************************

            for (var i = 1; i <= nr; ++i)
            {
                vapcon[i] = convec[i] / Constn.Rhoa / Constn.Ca;
                //
                if (nsp > 0)
                {
                    //           SNOW FILTERS DOWN THRU RESIDUE - VAPOR DIFFUSIVITY IS THAT
                    //           THROUGH SNOW
                    var avgtmp = _timewt.Wt * tr[i] + _timewt.Wdt * trdt[i];
                    vapcon[i] = Spprop.Vdifsp * (Constn.P0 / _constn.Presur) * Math.Pow((1.0 + avgtmp / 273.16), Spprop.Vapspx);
                }
            label10:;
            }
        }

        // line 7633
        private void Qvres(ref int nr, double[] qvr, double[] conv, double[] vapr, double[] vaprdt)
        {
            //
            //     THIS SUBOUTINE CALCULATES VAPOR FLUX BETWEEN RESIDUE NODES.
            //
            //***********************************************************************

            for (var i = 1; i <= nr; ++i)
            {
                qvr[i] = conv[i] * (_timewt.Wt * (vapr[i] - vapr[i + 1]) + _timewt.Wdt * (vaprdt[i] - vaprdt[i + 1]));
            label10:;
            }
        }

        // line 7650
        private void Resht(ref int nr, double[] cres, double[] gmc, double[] rhor)
        {
            //
            //     THIS SUBOUTINE CALCULATES THE VOLUMETRIC SPECIFIC HEAT FOR EACH
            //     RESIDUE NODE.
            //
            //***********************************************************************
            //

            for (var i = 1; i <= nr; ++i)
            {
                cres[i] = rhor[i] * (Constn.Cr + gmc[i] * Constn.Cl);
            label10:;
            }
        }

        // line 7671
        /// <summary>
        ///   <para>Sets: qsv, dconvp.</para>
        /// </summary>
        private void Qvsoil(ref int ns, double[] qsv, double[] zs, double[] ts, double[] mat, double[] vlc, double[] vic, double[][] conc, double[] dconvp)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE VAPOR FLUX BETWEEN SOIL NODES DUE
            //     TO GRADIENTS IN POTENTIAL AND TEMPERATURE GRADIENTS. (FLUX DUE TO
            //     TEMPERATURE GRADIENTS ARE MULTIPLIED BY AN ENHANCEMENT FACTOR.
            //
            //***********************************************************************
            var humid = new double[51];
            var dvt = new double[51];
            var dvp = new double[51];
            var convt = new double[51];
            var convp = new double[51];

            var vac = 0.0;
            var dv = 0.0;
            var en = 0.0;
            var s = 0.0;
            var satvap = 0.0;
            var totpot = 0.0;
            var tlconc = 0.0;

            for (var i = 1; i <= ns; ++i)
            {
                vac = 1.0 - _slparm.Rhob[i] * ((1.0 - _slparm.Om[i]) / Constn.Rhom + _slparm.Om[i] / Constn.Rhoom) - vlc[i] - vic[i];
                if (mat[i] < _slparm.Soilwrc[i][1] && vac > 0.0)
                {
                    dv = Constn.Vdiff * (Math.Pow(((ts[i] + 273.16) / 273.16), 2)) * (Constn.P0 / _constn.Presur);
                    dv = dv * _slparm.Vapcof[i] * (Math.Pow(vac, _slparm.Vapexp[i]));
                    Enhanc(ref i, ref en, vlc);
                    Vslope(ref s, ref satvap, ref ts[i]);
                    //
                    //****       DETERMINE THE HUMIDITY FROM THE TOTAL WATER POTENTIAL
                    tlconc = 0.0;
                    for (var j = 1; j <= _slparm.Nsalt; ++j)
                    {
                        tlconc = tlconc + conc[j][i];
                    label10:;
                    }
                    totpot = mat[i] - tlconc * Constn.Ugas * (ts[i] + 273.16) / Constn.G;
                    humid[i] = Math.Exp(.018 * Constn.G / Constn.Ugas / (ts[i] + 273.16) * totpot);
                    dvt[i] = dv * en * humid[i] * s;
                    dvp[i] = dv * satvap;
                }
                else
                {
                    //           NO AIR POROSITY OR SATURATED --> NO VAPOR DIFFUSION
                    dvt[i] = 0.0;
                    dvp[i] = 0.0;
                    humid[i] = 1.0;
                }
            //
            label20:;
            }
            //
            Conduc(ns, zs, dvt, convt);
            Conduc(ns, zs, dvp, convp);
            //
            for (var i = 1; i <= ns - 1; ++i)
            {
                qsv[i] = convt[i] * (ts[i] - ts[i + 1]) + convp[i] * (humid[i] - humid[i + 1]);
                //xxxx    DCONVP(I)=HUMID(I)*0.018*G/UGAS/(TS(I)+273.16)*CONVP(I)
                dconvp[i] = .018 * Constn.G / Constn.Ugas / (ts[i] + 273.16) * convp[i];
            label30:;
            }
        }

        // line 7727
        /// <summary>
        ///   <para>Sets: en.</para>
        /// </summary>
        private void Enhanc(ref int i, ref double en, double[] vlc)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE ENHANCEMENT COEFFICIENT FOR VAPOR
            //     TRANSPORT DUE TO THERMAL GRADIENTS FOR NODE I
            //
            //***********************************************************************
            //

            var en1 = 9.5;
            var en2 = 3.0;
            var en4 = 1.0;
            var en5 = 4.0;
            double en3;
            if (_slparm.Clay[i] <= 0.02)
            {
                //        EN3 BECOMES INFINITELY LARGE WHEN CLAY IS ZERO
                //        (SMALLEST CLAY CONTENT IN DATA FROM CASS ET. AL. WAS 2%)
                en3 = _slparm.Soilwrc[i][2] * (1 + 2.6 / Math.Sqrt(0.02));
            }
            else
            {
                en3 = _slparm.Soilwrc[i][2] * (1 + 2.6 / Math.Sqrt(_slparm.Clay[i]));
            }
            var expon = -Math.Pow((en3 * vlc[i] / _slparm.Soilwrc[i][2]), en5);
            //
            //     EXP(-50.0) APPROXIMATELY = 0.0, BUT UNDERFLOW MAY RESULT IF YOU TRY
            //     TO USE LARGE NEGATIVE NUMBERS  --->  CHECK IF  < -50.0
            if (expon <= -50.0)
            {
                expon = 0.0;
            }
            else
            {
                expon = Math.Exp(expon);
            }
            //
            //**** CALCULATE ENHANCEMENT FACTOR
            en = en1 + en2 * vlc[i] / _slparm.Soilwrc[i][2] - (en1 - en4) * expon;
        }

        // line 7766
        /// <summary>
        ///   <para>Sets: qsl, qsv, gflux. Sets A1[n+1..n+ns-1]. Sets B1[n..n+ns-2], C1[n..n+ns-2], D1[n..n+ns-2] </para>
        ///   <para>Modifies: n -> n + ns - 2.</para>
        /// </summary>
        private void Ebsoil(ref int n, ref int ns, double[] zs, double[] ts, double[] tsdt, double[] mat, double[] matdt, double[][] conc, double[][] concdt,
            double[] vlc, double[] vlcdt, double[] vic, double[] vicdt, int[] ices, int[] icesdt, double[] qsl, double[] qsv, double[] ss, ref double gflux, ref int iter)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE COEFFICIENTS FOR THE SOIL IN THE
            //     NEWTON-RAPHSON PROCEDURE OF THE ENERGY BALANCE
            //
            //***********************************************************************

            var qsldt = new double[51];
            var tk = new double[51];
            var tkdt = new double[51];
            var con = new double[51];
            var csdt = new double[51];
            var hkt = new double[51];
            var hkdt = new double[51];
            var conh = new double[51];
            var qsvdt = new double[51];
            var dconvp = new double[51];
            var delta = new double[51];
            var epslon = new double[51];
            var conht = new double[51];

            var iflag = 0;
            var qmax = 0.0;
            var t = 0.0;
            var tlconc = 0.0;
            var tmpfrz = 0.0;
            var tdt = 0.0;
            var dldtdt = 0.0;
            var dldm = 0.0;
            var j = 0;
            var dldt = 0.0;
            var dummy = 0.0;

            //**** DETERMINE THE AVERAGE VAPOR FLUX BETWEEN SOIL NODES
            if (iter == 1) Qvsoil(ref ns, _ebsoilSave.Qsvt, zs, ts, mat, vlc, vic, conc, dconvp);
            Qvsoil(ref ns, qsvdt, zs, tsdt, matdt, vlcdt, vicdt, concdt, dconvp);
            Weight(ns - 1, qsv, _ebsoilSave.Qsvt, qsvdt);
            //
            //**** DETERMINE THE LIQUID MOISTURE FLUX BETWEEN NODES
            if (iter == 1)
            {
                //****    CALCULATE FLUX AT BEGINNING OF TIME STEP
                //****    DETERMINE THE HYDRAULIC CONDUCTIVITY OF EACH SOIL NODE
                Soilhk(ns, hkt, mat, vlc, vic);
                //****    DETERMINE THE CONDUCTANCE TERM BETWEEN SOIL NODES
                Conduc(ns, zs, hkt, conht);
                Qlsoil(ns, zs, _ebsoilSave.Qslt, conht, mat);
            }
            Soilhk(ns, hkdt, matdt, vlcdt, vicdt);
            Conduc(ns, zs, hkdt, conh);
            Qlsoil(ns, zs, qsldt, conh, matdt);
            //
            //**** OBTAIN THE AVERAGE MOISTURE FLUX OVER THE TIME STEP
            Weight(ns - 1, qsl, _ebsoilSave.Qslt, qsldt);
            //
            //     LIMIT NET WATER FLUX INTO NODES IF FLUX FILLS NODE BEYOND
            //     AVAILABLE POROSITY AS A RESULT DOWNWARD FLUX INTO LAYERS WITH
            //     LIMITED PERMEABILITY (PERHAPS DUE TO ICE)
            //     (START AT BOTTOM OF PROFILE AND WORK UP SO THAT ANY CHANGES IN THE
            //     FLUXES CAN WORK THEIR UP THROUGH THE PROFILE)
            iflag = 0;
            for (var i = ns - 1; i >= 2; --i)
            {
                if (vicdt[i] > 0.0)
                {
                    qmax = (_slparm.Soilwrc[i][2] - vlc[i] - vic[i]) * (zs[i + 1] - zs[i - 1]) / 2.0 / _timewt.Dt;
                    if (qmax < 0.0) qmax = 0.0;
                    if (qsl[i - 1] - qsl[i] > qmax)
                    {
                        iflag = 1;
                        if (qsl[i - 1] > 0.0)
                        {
                            conh[i - 1] = 0.0;
                            qsl[i - 1] = qsl[i] + qmax;
                            if (qsl[i - 1] < 0.0)
                            {
                                qsl[i - 1] = 0.0;
                                qsl[i] = qsl[i - 1] - qmax;
                                if (qsl[i] > 0.0) qsl[i] = 0.0;
                            }
                        }
                        else
                        {
                            conh[i] = 0.0;
                            qsl[i] = qsl[i - 1] - qmax;
                            if (qsl[i] > 0.0) qsl[i] = 0.0;
                        }
                    }
                }
                //        SET UP LIQUID FLUX COEFFICIENTS SO THAT HEAT IS CARRIED IN
                //        DIRECTION OF MOISTURE MOVEMENT.
                if (qsl[i] > 0.0)
                {
                    delta[i] = 1.0;
                    epslon[i] = 0.0;
                }
                else
                {
                    delta[i] = 0.0;
                    epslon[i] = -1.0;
                }
            label5:;
            }
            //
            if (qsl[1] > 0.0)
            {
                delta[1] = 1.0;
                epslon[1] = 0.0;
            }
            else
            {
                delta[1] = 0.0;
                epslon[1] = -1.0;
            }
            //
            if (vicdt[1] > 0.0)
            {
                qmax = (_slparm.Soilwrc[1][2] - vlc[1] - vic[1]) * (zs[2] - zs[1]) / 2.0 / _timewt.Dt;
                if (qmax < 0.0) qmax = 0.0;
                if (-qsl[1] > qmax)
                {
                    iflag = 1;
                    conh[1] = 0.0;
                    qsl[1] = -qmax;
                }
            }
            //
            if (iflag > 0)
            {
                for (var i = 2; i <= ns - 1; ++i)
                {
                    if (vicdt[i] > 0.0)
                    {
                        qmax = (_slparm.Soilwrc[i][2] - vlc[i] - vic[i]) * (zs[i + 1] - zs[i - 1]) / 2.0 / _timewt.Dt;
                        if (qmax < 0.0) qmax = 0.0;
                        if (qsl[i - 1] - qsl[i] > qmax)
                        {
                            if (qsl[i] < 0.0)
                            {
                                conh[i] = 0.0;
                                qsl[i] = qsl[i - 1] - qmax;
                                if (qsl[i] > 0.0)
                                {
                                    qsl[i] = 0.0;
                                    qsl[i - 1] = qsl[i] + qmax;
                                    if (qsl[i - 1] < 0.0) qsl[i - 1] = 0.0;
                                }
                            }
                            else
                            {
                                conh[i - 1] = 0.0;
                                qsl[i - 1] = qsl[i] + qmax;
                                if (qsl[i - 1] < 0.0) qsl[i - 1] = 0.0;
                            }
                        }
                    }
                label10:;
                }
            }
            //
            //**** CALCULATE THERMAL CONDUCTIVITY
            if (iter == 1) Soiltk(ref ns, _ebsoilSave.Tkt, vlc, vic);
            Soiltk(ref ns, tkdt, vlcdt, vicdt);
            //
            //**** OBTAIN AVERAGE CONDUCTIVITY OVER THE TIME STEP
            Weight(ns, tk, _ebsoilSave.Tkt, tkdt);
            //
            //**** CALCULATE CONDUCTANCE TERMS BETWEEN NODES
            Conduc(ns, zs, tk, con);
            //
            //**** CALCULATE THE EFFECTIVE SPECIFIC HEAT AT EACH NODE
            if (iter == 1) Soilht(ref ns, _ebsoilSave.Cst, vlc, vic, ts, mat, conc);
            Soilht(ref ns, csdt, vlcdt, vicdt, tsdt, matdt, concdt);
            //
            //**** OBTAIN AVERAGE SPECIFIC HEAT OVER THE TIME STEP
            Weight(ns, _spheat.Cs, _ebsoilSave.Cst, csdt);
            //
            //
            //**** DETERMINE THE COEFFICIENTS FOR THE SURFACE LAYER
            //
            _matrix.A1[n + 1] = _timewt.Wdt * (con[1] + delta[1] * (Constn.Rhol * Constn.Cl * qsl[1] + Constn.Cv * qsv[1]));
            _matrix.B1[n] = _matrix.B1[n] - _timewt.Wdt * (con[1] + epslon[1] * (Constn.Rhol * Constn.Cl * qsl[1] + Constn.Cv * qsv[1])) - (zs[2] - zs[1]) * csdt[1] / (2.0 * _timewt.Dt);
            _matrix.C1[n] = _timewt.Wdt * (con[1] + epslon[1] * (Constn.Rhol * Constn.Cl * qsl[1] + Constn.Cv * qsv[1]));
            _matrix.D1[n] = _matrix.D1[n] - (con[1] + epslon[1] * (Constn.Rhol * Constn.Cl * qsl[1] + Constn.Cv * qsv[1])) * (_timewt.Wt * (ts[1] - ts[2]) + _timewt.Wdt * (tsdt[1] - tsdt[2])) - Constn.Lv * qsv[1] + ss[1] - (zs[2] - zs[1]) / (2.0 * _timewt.Dt) * (_spheat.Cs[1] * (tsdt[1] - ts[1]) - Constn.Rhoi * Constn.Lf * (vicdt[1] - vic[1]));
            //
            //     COMPUTE SURFACE GROUND HEAT FLUX FOR ENERGY OUTPUT
            gflux = (con[1] + epslon[1] * (Constn.Rhol * Constn.Cl * qsl[1] + Constn.Cv * qsv[1])) * (_timewt.Wt * (ts[1] - ts[2]) + _timewt.Wdt * (tsdt[1] - tsdt[2])) + Constn.Lv * qsv[1] + (zs[2] - zs[1]) / (2.0 * _timewt.Dt) * (_spheat.Cs[1] * (tsdt[1] - ts[1]) - Constn.Rhoi * Constn.Lf * (vicdt[1] - vic[1]));
            //
            //**** CHECK IF ICE IS PRESENT AT THE END OF THE TIME STEP
            if (icesdt[1] != 1) goto label20;
            //
            //**** ICE IS PRESENT IN LAYER - - ADJUST COEFFICIENTS FOR LATENT HEAT
            //**** TRANSFER AND THE SLOPE OF THE WATER CONTENT-TEMPERATURE CURVE
            if (ices[1] == 1)
            {
                //        ICE IS PRESENT FOR THE ENTIRE TIME STEP
                t = 273.16 + ts[1];
            }
            else
            {
                //        ICE IS PRESENT ONLY AT THE END OF THE TIME STEP - DETERMINE
                //        THE TEMPERATURE AT WHICH THE SOIL WILL BEGIN TO FREEZE
                tlconc = 0.0;
                for (var k = 1; k <= _slparm.Nsalt; ++k)
                {
                    tlconc = tlconc + conc[k][1];
                label15:;
                }
                tmpfrz = 273.16 * Constn.Lf / Constn.G / (Constn.Lf / Constn.G - mat[1] + tlconc * Constn.Ugas * (ts[1] + 273.17) / Constn.G);
                t = tmpfrz;
            }
            tdt = 273.16 + tsdt[1];
            //     DETERMINE SLOPE OF LIQUID CONTENT-TEMPERATURE CURVE
            Fslope(1, ref dldtdt, ref dummy, ref tdt, matdt, concdt, vlcdt);
            Fslope(1, ref dldt, ref dummy, ref t, mat, conc, vlc);
            //     ENTER MATVLC FOR SLOPE OF LIQUID-MATRIC POTENTIAL CURVE
            Matvl3(1, ref matdt[1], ref vlcdt[1], ref dldm);
            _matrix.B1[n] = _matrix.B1[n] - (zs[2] - zs[1]) / (2.0 * _timewt.Dt) * 0.5 * Constn.Rhol * Constn.Lf * (dldt + dldtdt) - _timewt.Wdt * Constn.Rhol * Constn.Lf * conh[1] * dldtdt / dldm;
        //
        //**** DETERMINE THE MATRIX COEFFICIENTS FOR THE REST OF THE PROFILE
        label20:;
            for (var i = n + 1; i <= n + ns - 2; ++i)
            {
                j = i - n + 1;
                _matrix.A1[i + 1] = _timewt.Wdt * (con[j] + delta[j] * (Constn.Rhol * Constn.Cl * qsl[j] + Constn.Cv * qsv[j]));
                _matrix.B1[i] = -_timewt.Wdt * (con[j - 1] + con[j] + (delta[j - 1] * (Constn.Rhol * Constn.Cl * qsl[j - 1] + Constn.Cv * (qsv[j - 1])) + epslon[j] * (Constn.Rhol * Constn.Cl * qsl[j] + Constn.Cv * qsv[j]))) - (zs[j + 1] - zs[j - 1]) * csdt[j] / (2.0 * _timewt.Dt);
                _matrix.C1[i] = _timewt.Wdt * (con[j] + epslon[j] * (Constn.Rhol * Constn.Cl * qsl[j] + Constn.Cv * qsv[j]));
                _matrix.D1[i] = (con[j - 1] + delta[j - 1] * (Constn.Rhol * Constn.Cl * qsl[j - 1] + Constn.Cv * qsv[j - 1])) * (_timewt.Wt * (ts[j - 1] - ts[j]) + _timewt.Wdt * (tsdt[j - 1] - tsdt[j])) - (con[j] + epslon[j] * (Constn.Rhol * Constn.Cl * qsl[j] + Constn.Cv * qsv[j])) * (_timewt.Wt * (ts[j] - ts[j + 1]) + _timewt.Wdt * (tsdt[j] - tsdt[j + 1])) - Constn.Lv * (qsv[j] - qsv[j - 1]) + ss[j] - (zs[j + 1] - zs[j - 1]) / (2.0 * _timewt.Dt) * (_spheat.Cs[j] * (tsdt[j] - ts[j]) - Constn.Rhoi * Constn.Lf * (vicdt[j] - vic[j]));
                //
                //****    CHECK IF ICE IS PRESENT AT THE END OF THE TIME STEP
                if (icesdt[j] != 1) goto label30;
                //
                //****    ICE IS PRESENT IN LAYER - - ADJUST COEFFICIENTS FOR LATENT HEAT
                //****    TRANSFER AND THE SLOPE OF THE WATER CONTENT-TEMPERATURE CURVE
                if (ices[j] == 1)
                {
                    //           ICE IS PRESENT FOR THE ENTIRE TIME STEP
                    t = 273.16 + ts[j];
                }
                else
                {
                    //           ICE IS PRESENT ONLY AT THE END OF THE TIME STEP - DETERMINE
                    //           THE TEMPERATURE AT WHICH THE SOIL WILL BEGIN TO FREEZE
                    tlconc = 0.0;
                    for (var k = 1; k <= _slparm.Nsalt; ++k)
                    {
                        tlconc = tlconc + conc[k][j];
                    label25:;
                    }
                    tmpfrz = 273.16 * Constn.Lf / Constn.G / (Constn.Lf / Constn.G - mat[j] + tlconc * Constn.Ugas * (ts[j] + 273.16) / Constn.G);
                    t = tmpfrz;
                }
                tdt = 273.16 + tsdt[j];
                //        DETERMINE SLOPE OF LIQUID CONTENT-TEMPERATURE CURVE
                Fslope(j, ref dldtdt, ref dummy, ref tdt, matdt, concdt, vlcdt);
                Fslope(j, ref dldt, ref dummy, ref t, mat, conc, vlc);
                //        ENTER MATVLC FOR SLOPE OF LIQUID-MATRIC POTENTIAL CURVE
                Matvl3(j, ref matdt[j], ref vlcdt[j], ref dldm);
                _matrix.B1[i] = _matrix.B1[i] - (zs[j + 1] - zs[j - 1]) / (2.0 * _timewt.Dt) * 0.5 * Constn.Rhol * Constn.Lf * (dldt + dldtdt) - _timewt.Wdt * Constn.Rhol * Constn.Lf * (conh[j - 1] + conh[j]) * dldtdt / dldm;
            label30:;
            }
            n = n + ns - 2;
        }

        // line 8009
        /// <summary>
        ///   <para>Sets: tk.</para>
        /// </summary>
        private void Soiltk(ref int ns, double[] tk, double[] vlc, double[] vic)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE THERMAL CONDUCTIVITY OF THE SOIL
            //     LAYERS USING DE VRIES'S METHOD.
            //
            //***********************************************************************
            //

            //      WEIGHTING FACTOR FOR ICE IS NOW SET TO 1.0 TO CORRECT PROBLEMS 
            //      POINTED OUT BY KENNEDY & SHARRATT, SOIL SCI. 163(8):636-645.0
            //      DATA GAI, GAOM, GASA, GASI, GAC /0.33, 0.5, 0.144, 0.144, 0.125/

            var gaom = 0.5;
            var garo = 0.333;
            var gasa = 0.144;
            var gasi = 0.144;
            var gac = 0.125;

            if (_soiltkSave.Ifirst == 0)
            {
                //        FIRST TIME INTO SUBROUTINE -- CALCULATE WEIGHTING FACTORS FOR
                //        SOIL COMPONENTS WHEN SOIL IS COMPLETELY DRY AND WHEN WET
                _soiltkSave.Wfaird = 1.0;
                _soiltkSave.Wfrod = Devwf(Constn.Tka, Constn.Tkro, garo);
                _soiltkSave.Wfsad = Devwf(Constn.Tka, Constn.Tksa, gasa);
                _soiltkSave.Wfsid = Devwf(Constn.Tka, Constn.Tksi, gasi);
                _soiltkSave.Wfcld = Devwf(Constn.Tka, Constn.Tkcl, gac);
                _soiltkSave.Wfomd = Devwf(Constn.Tka, Constn.Tkom, gaom);
                _soiltkSave.Wficed = 1.0;
                //
                //        SET THERMAL CONDUCTIVITY OF MOIST AIR EQUAL TO THAT OF DRY AIR
                //        (VAPOR TRANSFER IS CALCULATED SEPARATELY AND INCLUDED IN HEAT
                //        TRANSFER THROUGH SOIL; THEREFORE IT SHOULD NOT BE COMPENSATED
                //        FOR IN COMPUTATION FOR THERMAL CONDUCTIVITY AS IS USUALLY DONE
                //        WHEN USING DEVRIES METHOD.)
                _soiltkSave.Tkma = Constn.Tka;
                _soiltkSave.Wfl = 1.0;
                _soiltkSave.Wfro = Devwf(Constn.Tkl, Constn.Tkro, garo);
                _soiltkSave.Wfsa = Devwf(Constn.Tkl, Constn.Tksa, gasa);
                _soiltkSave.Wfsi = Devwf(Constn.Tkl, Constn.Tksi, gasi);
                _soiltkSave.Wfcl = Devwf(Constn.Tkl, Constn.Tkcl, gac);
                _soiltkSave.Wfom = Devwf(Constn.Tkl, Constn.Tkom, gaom);
                _soiltkSave.Wfice = 1.0;
                _soiltkSave.Ifirst = 1;
            }
            //
            for (var i = 1; i <= ns; ++i)
            {
                //       CORRECTION OF TEXTURE FOR ORGANIC MATTER AND ROCK CONTENT
                var xsm = 1.0 - _slparm.Om[i] - _slparm.Rock[i];
                var xsand = xsm * _slparm.Sand[i];
                var xsilt = xsm * _slparm.Silt[i];
                var xclay = xsm * _slparm.Clay[i];
                //
                //       CONVERT WEIGHT FRACTION TO VOLUMETRIC FRACTION
                var vrock = _slparm.Rock[i] * _slparm.Rhob[i] / Constn.Rhom;
                var vsand = xsand * _slparm.Rhob[i] / Constn.Rhom;
                var vsilt = xsilt * _slparm.Rhob[i] / Constn.Rhom;
                var vclay = xclay * _slparm.Rhob[i] / Constn.Rhom;
                var vom = _slparm.Om[i] * _slparm.Rhob[i] / Constn.Rhoom;
                var poro = 1.0 - vrock - vsand - vsilt - vclay - vom;
                var vac = poro - vic[i] - vlc[i];
                if (vac < 0.0) vac = 0.0;
                //
                //       DETERMINE LIMIT OF DEVRIES METHOD FROM TEXTURE
                //       SAND ==> 0.05;  LOAM ==> 0.10;  CLAY ==> 0.15
                var vlmt = 0.10 + 0.2 * _slparm.Clay[i] - 0.1 * _slparm.Sand[i];
                if (vlmt < 0.05) vlmt = 0.05;
                if (vlmt > 0.15) vlmt = 0.15;
                //
                if (vlc[i] > vlmt)
                {
                    //         MOIST SOIL -- USE DEVRIES METHOD DIRECTLY
                    var gaair = 0.035 + 0.298 * (vlc[i] - vlmt) / (poro - vlmt);
                    var wfair = Devwf(Constn.Tkl, _soiltkSave.Tkma, gaair);
                    tk[i] = (_soiltkSave.Wfro * vrock * Constn.Tkro + _soiltkSave.Wfsa * vsand * Constn.Tksa + _soiltkSave.Wfl * vlc[i] * Constn.Tkl + _soiltkSave.Wfice * vic[i] * Constn.Tki + wfair * vac * _soiltkSave.Tkma + _soiltkSave.Wfsi * vsilt * Constn.Tksi + _soiltkSave.Wfcl * vclay * Constn.Tkcl + _soiltkSave.Wfom * vom * Constn.Tkom) / (_soiltkSave.Wfro * vrock + _soiltkSave.Wfsa * vsand + _soiltkSave.Wfl * vlc[i] + _soiltkSave.Wfice * vic[i] + wfair * vac + _soiltkSave.Wfsi * vsilt + _soiltkSave.Wfcl * vclay + _soiltkSave.Wfom * vom);
                }
                else
                {
                    //         INTERPOLATE THERMAL CONDUCTIVITY BETWEEN WATER CONTENT AT 0
                    //         AND LIMIT OF DEVRIES METHOD
                    var tk0 = 1.25 * (_soiltkSave.Wfrod * vrock * Constn.Tkro + _soiltkSave.Wfsad * vsand * Constn.Tksa + _soiltkSave.Wficed * vic[i] * Constn.Tki + _soiltkSave.Wfaird * (vac + vlc[i]) * Constn.Tka + _soiltkSave.Wfsid * vsilt * Constn.Tksi + _soiltkSave.Wfcld * vclay * Constn.Tkcl + _soiltkSave.Wfomd * vom * Constn.Tkom) / (_soiltkSave.Wfrod * vrock + _soiltkSave.Wfsad * vsand + _soiltkSave.Wficed * vic[i] + _soiltkSave.Wfaird * (vac + vlc[i]) + _soiltkSave.Wfsid * vsilt + _soiltkSave.Wfcld * vclay + _soiltkSave.Wfomd * vom);
                    //
                    //         SET AIR POROSITY TO LOWER LIMIT OF DEVRIES METHOD
                    //         (AS PER CORRECTION NOTED BY JEAN-SEBASTIEN GOSSELIN)
                    vac = poro - vic[i] - vlmt;
                    if (vac < 0.0) vac = 0.0;
                    var wfair = Devwf(Constn.Tkl, _soiltkSave.Tkma, 0.035);
                    var tklmt = (_soiltkSave.Wfro * vrock * Constn.Tkro + _soiltkSave.Wfsa * vsand * Constn.Tksa + _soiltkSave.Wfl * vlmt * Constn.Tkl + _soiltkSave.Wfice * vic[i] * Constn.Tki + wfair * vac * _soiltkSave.Tkma + _soiltkSave.Wfsi * vsilt * Constn.Tksi + _soiltkSave.Wfcl * vclay * Constn.Tkcl + _soiltkSave.Wfom * vom * Constn.Tkom) / (_soiltkSave.Wfro * vrock + _soiltkSave.Wfsa * vsand + _soiltkSave.Wfl * vlmt + _soiltkSave.Wfice * vic[i] + wfair * vac + _soiltkSave.Wfsi * vsilt + _soiltkSave.Wfcl * vclay + _soiltkSave.Wfom * vom);
                    //
                    tk[i] = tk0 + (tklmt - tk0) * vlc[i] / vlmt;
                }
            label10:;
            }
        }

        private double Devwf(double tk0, double tk1, double ga)
        {
            //      DEFINE FUNCTION FOR CALCULATING DEVRIES WEIGHTING FACTORS
            return 2.0 / 3.0 / (1.0 + (tk1 / tk0 - 1.0) * ga) + 1.0 / 3.0 / (1.0 + (tk1 / tk0 - 1.0) * (1.0 - 2.0 * ga));
        }

        // line 8124
        /// <summary>
        ///   <para>Sets: cs.</para>
        /// </summary>
        private void Soilht(ref int ns, double[] cs, double[] vlc, double[] vic, double[] ts, double[] mat, double[][] conc)
        {
            //     THIS SUBROUTINE CALCULATES THE SPECIFIC HEAT THE SOIL LAYERS
            //     -- THE LATENT HEAT OF VAPORIZATION IS INCLUDED.
            //
            //***********************************************************************
            //

            for (var i = 1; i <= ns; ++i)
            {
                //        CORRECTION OF MINERAL FRACTION FOR ORGANIC MATTER
                var xsm = 1.0 - _slparm.Om[i];
                //
                var vac = 1.0 - xsm * _slparm.Rhob[i] / Constn.Rhom - _slparm.Om[i] * _slparm.Rhob[i] / Constn.Rhoom - vlc[i] - vic[i];
                if (vac < 0.0) vac = 0.0;
                cs[i] = xsm * _slparm.Rhob[i] * Constn.Cm + _slparm.Om[i] * _slparm.Rhob[i] * Constn.Com + vlc[i] * Constn.Rhol * Constn.Cl + vic[i] * Constn.Rhoi * Constn.Ci + vac * Constn.Rhoa * Constn.Ca;
                //
                //****    INCLUDE THE LATENT HEAT OF VAPORIZATION IN THE HEAT CAPACITY
                //        IF LAYER IS NOT SATURATED
                if (mat[i] < _slparm.Soilwrc[i][1] && vac > 0.0)
                {
                    //           DETERMINE HUMIDITY OF LAYER FROM TOTAL WATER POTENTIAL
                    var tlconc = 0.0;
                    for (var j = 1; j <= _slparm.Nsalt; ++j)
                    {
                        tlconc = tlconc + conc[j][i];
                    label10:;
                    }
                    var totpot = mat[i] - tlconc * Constn.Ugas * ts[i] / Constn.G;
                    var humid = Math.Exp(.018 * Constn.G / Constn.Ugas / (ts[i] + 273.16) * totpot);
                    //           OBTAIN SLOPE OF VAPOR DENSITY CURVE
                    var s = 0.0;
                    var rhov = 0.0;
                    Vslope(ref s, ref rhov, ref ts[i]);
                    cs[i] = cs[i] + vac * Constn.Lv * humid * s;
                }
            label20:;
            }

        }

        // line 8168
        /// <summary>
        ///   <para>Sets: dldt.</para>
        ///   <para>Does not use: ptdldt.</para>
        /// </summary>
        private void Fslope(int i, ref double dldt, ref double ptdldt, ref double tmp, double[] mat, double[][] conc, double[] vlc)
        {
            //
            //     THIS SUBROUTINE DETERMINES THE SLOPE OF THE LIQUID CONTENT -
            //     TEMPERATURE CURVE (DLDT) AS WELL AS THE PARTIAL OF T*DLDT.
            //     (OSMOTIC EFFECTS ARE INCLUDED.)
            //     ---- TMP MUST BE IN DEGREES KELVIN
            //
            //***********************************************************************

            var cncgas = 0.0;
            var crt = 0.0;
            for (var j = 1; j <= _slparm.Nsalt; ++j)
            {
                var dkq = _slparm.Saltkq[j][i] + vlc[i] * Constn.Rhol / _slparm.Rhob[i];
                cncgas = cncgas + conc[j][i] * Constn.Ugas;
                crt = crt + conc[j][i] * Constn.Ugas * tmp * Constn.Rhol / _slparm.Rhob[i] / dkq;
            label10:;
            }
            var dldm = 0.0;
            Matvl3(i, ref mat[i], ref vlc[i], ref dldm);
            if (dldm > 0.0)
            {
                dldt = (Constn.Lf / tmp + cncgas) / (Constn.G / dldm + crt);
            }
            else
            {
                //        NO CHANGE IN WATER CONTENT WITH MATRIC POTENTIAL OR TEMPERATURE
                //        LAYER IS PROBABLY SATURATED
                dldt = 0.0;
            }
            //
            //**** THIS PART OF THE SUBROUTINE CALCULATES THE PARTIAL OF T*DLDT
            //
            var crkq = 0.0;
            for (var j = 1; j <= _slparm.Nsalt; ++j)
            {
                var dkq = _slparm.Saltkq[j][i] + vlc[i] * Constn.Rhol / _slparm.Rhob[i];
                crkq = crkq + conc[j][i] * Constn.Ugas / dkq * (1.0 - 2.0 * tmp * (Constn.Rhol / _slparm.Rhob[i]) * dldt / dkq);
            label20:;
            }
            //XXXX PTDLDT=-LF/ ((-B(I)*G*MAT(I)/VLC(I) + CRT)**2)
            //XXXX>      * (B(I)*G*MAT(I)*(B(I)+1)*DLDT/(VLC(I)**2) + CRKQ)
        }

        // line 8216
        private bool Frozen(ref int i, double[] vlc, double[] vic, double[] mat, double[][] conc, double[] ts, double[][] salt, int[] ices)
        {
            //
            //     THIS SUBROUTINE DETERMINES THE LIQUID AND ICE WATER CONTENT OF THE
            //     SOIL FOR TMP < 0 C  AND DETERMINES IF ANY ICE MAY BE PRESENT.
            //     IT THEN UPDATES THE MATRIC POTENTIAL AND SOLUTE CONCENTRATION.
            //
            //***********************************************************************
            var mat1 = 0.0;
            var generalOut = _outputWriters[OutputFile.EntireProfile];
            var dummy = 0.0;

            //     ITERATE TO FIND THE MAXIMUM WATER CONTENT FROM THE TOTAL, MATRIC
            //     AND OSMOTIC POTENTIALS:
            //               TOTAL - MATRIC - OSMOTIC = 0 => F
            //     MATRIC AND OSMOTIC POTENTIALS ARE FUNCTIONS OF WATER CONTENT.
            //     SOLVE BY NEWTON-RAPHSON ITERATION  =>  D(VLC) = -F/(DF/D(VLC)
            //

            var iconv = 0;
            var tmp = 0.0;
            var totpot = 0.0;
            var vlc1 = 0.0;
            var iter = 0;
            var osmpot = 0.0;
            var deriv = 0.0;
            var term = 0.0;
            var dldm = 0.0;
            var error = 0.0;
            var delta = 0.0;
            var vlc2 = 0.0;

        label5:;
            tmp = ts[i] + 273.16;
            totpot = Constn.Lf * ts[i] / tmp / Constn.G;
            Matvl2(i, ref totpot, ref vlc1, ref dummy);

            //
            //**** BEGIN ITERATIONS
            iter = 0;
        //     CALCULATE OSMOTIC POTENTIAL AND DERIVATIVE WITH RESPECT TO LIQUID
        label10:;
            osmpot = 0.0;
            deriv = 0.0;
            for (var j = 1; j <= _slparm.Nsalt; ++j)
            {
                term = salt[j][i] * Constn.Ugas * tmp / (_slparm.Saltkq[j][i] + vlc1 * Constn.Rhol / _slparm.Rhob[i]);
                osmpot = osmpot - term;
                deriv = deriv + term * Constn.Rhol / _slparm.Rhob[i] / (_slparm.Saltkq[j][i] + vlc1 * Constn.Rhol / _slparm.Rhob[i]);
            label20:;
            }
            osmpot = osmpot / Constn.G;
            deriv = deriv / Constn.G;

            //
            //     CALCULATE MATRIC POTENTIAL AND DERIVATIVE WITH RESPECT TO LIQUID
            if (vlc1 <= _slparm.Soilwrc[i][4]) vlc1 = vlc1 + 10.0e-7;
            Matvl1(i, ref mat1, ref vlc1, ref dummy);
            Matvl3(i, ref mat1, ref vlc1, ref dldm);
            if (dldm == 0.0)
            {
                Matvl3(i, ref totpot, ref vlc1, ref dldm);
            }

            //
            //     DERTERMINE ERROR IN WATER POTENTIAL AND ADJUST WATER CONTENT
            error = totpot - osmpot - mat1;
            delta = error / (deriv + 1 / dldm);
            vlc2 = vlc1 + delta;
            if (vlc2 > _slparm.Soilwrc[i][2]) vlc2 = _slparm.Soilwrc[i][2];

            //     DO NOT GO MORE THAN HALF WAY TO RESIDUAL AT EACH ITERATION
            if (vlc2 <= (vlc1 + _slparm.Soilwrc[i][4]) / 2.0) vlc2 = (vlc1 + _slparm.Soilwrc[i][4]) / 2.0;
            delta = vlc2 - vlc1;
            vlc1 = vlc2;
            if (Math.Abs(delta) < .00001) goto label30;
            iter = iter + 1;
            if (iconv > 0 && _lvlout[1] > 0) // write(21, *)iter,osmpot,mat1,vlc1,deriv,1 / dldm,error;
                generalOut.WriteLine($"{iter} {osmpot} {mat1} {vlc1} {deriv} {1.0 / dldm} {error}");
            if (iter > 30)
            {
                iconv = iconv + 1;
                // write(21,100)
                if (_lvlout[1] > 0)
                {
                    generalOut.WriteLine();
                    generalOut.WriteLine();
                    generalOut.WriteLine("*****  PROGRAM WAS STOPPED IN SUBROUTINE FROZEN DUE TO CONVERGENCE PROBLEMS *****");

                    // write(21,*)i,ts(i),vlc(i),vic(i),totpot
                    generalOut.WriteLine($"{i} {ts[i]} {vlc[i]} {vic[i]} {totpot}");
                }
                if (iconv == 1) goto label5;
                Console.WriteLine("*****  PROGRAM WAS STOPPED IN SUBROUTINE FROZEN DUE TO CONVERGENCE PROBLEMS *****");
                return false;
            }
            goto label10;
        //
        //**** IF ACTUAL LIQUID PLUS ICE CONTENT IS LESS THAN THAT CALCULATED
        //     FROM ABOVE, THERE IS NO ICE PRESENT IN THE LAYER.  (THE TOTAL
        //     POTENTIAL IS SUFFICIENTLY NEGATIVE THAT THE WATER WILL NOT
        //     FREEZE.)
        label30:;
            if ((vlc[i] + vic[i] * Constn.Rhoi / Constn.Rhol) < vlc2)
            {
                //*       NO ICE IS PRESENT AND LAYER IS UNSATURATED
                ices[i] = 0;
                vlc[i] = vlc[i] + vic[i] * Constn.Rhoi / Constn.Rhol;
                vic[i] = 0.0;
                if (vlc[i] > _slparm.Soilwrc[i][4])
                {
                    //           COMPUTE MATRIC POTENTIAL FROM LIQUID WATER CONTENT
                    Matvl1(i, ref mat[i], ref vlc[i], ref dummy);
                }
                else
                {
                    //           WATER CONTENT AT RESIDUAL WATER CONTENT - MATRIC POTENTIAL
                    //           CANNOT BE CALCULATED FROM WATER CONTENT AND IS NOT A
                    //           FUNCTION OF TEMPERATURE -- DO NOT RECOMPUTE
                }
            }
            else
            {
                //*       ICE MAY BE PRESENT; CONVERT ANY WATER PRESENT ABOVE THE MAXIMUM
                //        LIQUID WATER CONTENT TO ICE
                vic[i] = vic[i] + (vlc[i] - vlc2) * Constn.Rhol / Constn.Rhoi;
                vlc[i] = vlc2;
                if (vic[i] > 0.0)
                {
                    //           ICE IS PRESENT --> VLC AND MAT ARE FUNCTION OF TEMP
                    ices[i] = 1;
                    if (vlc1 > _slparm.Soilwrc[i][4])
                    {
                        //              COMPUTE MATRIC POTENTIAL FROM LIQUID WATER CONTENT
                        Matvl1(i, ref mat[i], ref vlc[i], ref dummy);
                    }
                    else
                    {
                        //              WATER CONTENT AT RESIDUAL WATER CONTENT - COMPUTE MATRIC
                        //              POTENTIAL DIRECTLY FROM TEMPERATURE AND OSMOTIC POTENTIAL
                        mat[i] = totpot - osmpot;
                    }
                }
                else
                {
                    //           NO ICE IS PRESENT AND WATER CONTENT IS AT SATURATION
                    vic[i] = 0.0;
                    ices[i] = 0;
                }
            }
            //     REDEFINE SOLUTE CONCENTRATIONS
            for (var j = 1; j <= _slparm.Nsalt; ++j)
            {
                conc[j][i] = salt[j][i] / (_slparm.Saltkq[j][i] + vlc[i] * Constn.Rhol / _slparm.Rhob[i]);
            label40:;
            }

            return true;
        }

        // line 8343
        private bool Adjust(ref int i, double[] vicdt, double[] vlcdt, double[] matdt, double[][] concdt, double[] tsdt, double[][] saltdt, int[] icesdt)
        {
            //
            //     THIS SUBROUTINE ADJUSTS THE SOIL LAYER TO ACCOUNT FOR THE LATENT
            //     HEAT OF FUSION ON THE FIRST ITERATION THAT THE LAYER CROSSES THE
            //     FREEZING POINT.
            //
            //***********************************************************************

            //**** INITIALLY ASSUME THAT ALL WATER IS LIQUID
            vlcdt[i] = vlcdt[i] + (Constn.Rhoi / Constn.Rhol) * vicdt[i];
            var dummy = 0.0;
            Matvl1(i, ref matdt[i], ref vlcdt[i], ref dummy);
            for (var j = 1; j <= _slparm.Nsalt; ++j)
            {
                concdt[j][i] = saltdt[j][i] / (_slparm.Saltkq[j][i] + vlcdt[i] * Constn.Rhol / _slparm.Rhob[i]);
            label5:;
            }
            vicdt[i] = 0.0;
            //
            //**** CALCULATE THE FREEZING POINT TEMPERATURE ACCORDING TO TOTAL WATER
            //     POTENTIAL AT THE END OF THE TIME STEP
            var tlconc = 0.0;
            for (var j = 1; j <= _slparm.Nsalt; ++j)
            {
                tlconc = tlconc + concdt[j][i];
            label10:;
            }
            var totpot = matdt[i] - tlconc * Constn.Ugas * 273.16 / Constn.G;
            var tmpfrz = 273.16 * totpot / (Constn.Lf / Constn.G - totpot);
            var tmpf = tmpfrz + 273.16;
            //
            //**** CALCULATE THE ENERGY AVAILABLE TO FREEZE WATER
            var energy = _spheat.Cs[i] * (tmpfrz - tsdt[i]);
            //
            //**** CALCULATE THE EFFECTIVE HEAT CAPACITY INCLUDING LATENT HEAT TERM
            var dldt = 0.0;
            Fslope(i, ref dldt, ref dummy, ref tmpf, matdt, concdt, vlcdt);
            var effcs = _spheat.Cs[i] + Constn.Rhol * Constn.Lf * dldt;
            //
            //**** CALCULATE THE TEMPERATURE AT THE END OF THE TIME STEP
            tsdt[i] = tmpfrz - energy / effcs;
            //
            //**** CALL SUBROUTINE FROZEN TO DETERMINE THE LIQUID, ICE, AND SOLUTES
            return Frozen(ref i, vlcdt, vicdt, matdt, concdt, tsdt, saltdt, icesdt);
        }

        // line 8398
        private void Wbcan(ref int n, ref int nplant, ref int nc, ref int nsp, ref int nr, ref int ns, double[] zc, double[] tc, double[] tcdt, double[][] tlc, double[][] tlcdt, double[] vapc, double[] vapcdt, double[] wcan, double[] wcandt, double[] pcan, double[] pcandt, double[] qvc, double[] trnsp, double[] mat, double[] matdt, double[] xtract, double[][] swcan, double[][] lwcan, double[] swdown, ref double canma, ref double canmb, double[] dchar, double[] rstom0, double[] rstexp, double[] pleaf0, double[] rleaf0, int[] itype, ref int istomate, double[][] stomate, ref int icesdt, ref int iter)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE JACOBIAN MATRIX COEFFICIENTS FOR
            //     THE CANOPY PORTION OF THE NEWTON-RAPHSON SOLUTION OF THE ENERGY
            //     BALANCE
            //
            //***********************************************************************
            //xOUT
            //
            var con = new double[11];
            var condt = new double[11];
            var heatc = new double[11];
            var etlyr = new double[11];
            var detlyr = new double[11];
            var dheatc = new double[11];
            var dtldtc = new double[11];

            //**** CALCULATE TRANSPIRATION FROM CANOPY
            Leaft(ref nplant, ref nc, ref ns, ref iter, itype, tc, tcdt, tlc, tlcdt, vapc, vapcdt, wcan, wcandt, pcan, pcandt, mat, matdt, trnsp, xtract, swcan, lwcan, swdown, heatc, etlyr, detlyr, ref canma, ref canmb, dchar, rstom0, rstexp, pleaf0, rleaf0, ref istomate, stomate, dheatc, dtldtc);
            //
            //**** DETERMINE THE EDDY CONDUCTANCE TERM BETWEEN NODES
            Cantk(ref iter, ref nc, con, tcdt, zc);
            //
            //
            //**** CONVERT THERMAL EDDY CONDUCTANCE TO VAPOR CONDUCTANCE
            //     AND CALCULATE LIQUID FLUX BETWEEN LAYERS
            for (var i = 1; i <= nc - 1; ++i)
            {
                con[i] = con[i] / Constn.Rhoa / Constn.Ca;
                qvc[i] = con[i] * (vapcdt[i] - vapcdt[i + 1]);
            label5:;
            }
            con[nc] = con[nc] / Constn.Rhoa / Constn.Ca;
            qvc[nc] = con[nc] * (_timewt.Wdt * (vapcdt[nc] - vapcdt[nc + 1]) + _timewt.Wt * (vapcdt[nc] - vapc[nc + 1]));
            //
            //**** DETERMINE THE MATRIX COEFFICIENTS FOR THE TOP LAYER
            //
            _matrix.A2[n + 1] = con[1];
            _matrix.B2[n] = _matrix.B2[n] - (con[1] + detlyr[1]) - (zc[2] - zc[1]) / 2 / _timewt.Dt;
            if (nc == 1)
            {
                _matrix.C2[n] = _timewt.Wdt * con[1];
                _matrix.D2[n] = _matrix.D2[n] - qvc[1] + etlyr[1] - (zc[2] - zc[1]) * (vapcdt[1] - vapc[1]) / _timewt.Dt;
            }
            else
            {
                _matrix.C2[n] = con[1];
                _matrix.D2[n] = _matrix.D2[n] - qvc[1] + etlyr[1] - (zc[2] - zc[1]) / 2 * (vapcdt[1] - vapc[1]) / _timewt.Dt;
            }
            //
            //**** DETERMINE THE COEFFICIENTS FOR THE REST OF THE LAYERS
            for (var i = n + 1; i <= n + nc - 1; ++i)
            {
                var j = i - n + 1;
                _matrix.A2[i + 1] = con[j];
                _matrix.B2[i] = -(con[j - 1] + con[j] + detlyr[j]) - (zc[j + 1] - zc[j - 1]) / 2 / _timewt.Dt;
                if (j == nc)
                {
                    _matrix.C2[i] = _timewt.Wdt * con[j];
                    _matrix.D2[i] = qvc[j - 1] - qvc[j] + etlyr[j] - (zc[j + 1] - zc[j] + (zc[j] - zc[j - 1]) / 2.0) / _timewt.Dt * (vapcdt[j] - vapc[j]);
                }
                else
                {
                    _matrix.C2[i] = con[j];
                    _matrix.D2[i] = qvc[j - 1] - qvc[j] + etlyr[j] - (zc[j + 1] - zc[j - 1]) / 2.0 / _timewt.Dt * (vapcdt[j] - vapc[j]);
                }
            label10:;
            }
            n = n + nc;
            //
            //**** DETERMINE THE COEFFICIENTS FOR THE TOP LAYER OF THE NEXT MATERIAL
            //
            if (nsp > 0)
            {
                //****    NEXT MATERIAL IS SNOW -- NO WATER BALANCE MATRIX FOR SNOW
                _matrix.A2[n] = 0.0;
                _matrix.B2[n] = 0.0;
                _matrix.C2[n - 1] = 0.0;
                _matrix.D2[n] = 0.0;
            }
            else
            {
                //
                if (nr > 0)
                {
                    //****    NEXT MATERIAL IS RESIDUE
                    _matrix.B2[n] = -_timewt.Wdt * con[nc];
                    _matrix.D2[n] = qvc[nc];
                }
                else
                {
                    //
                    //**** NEXT MATERIAL IS SOIL -- WATER BALANCE BASED ON WATER CONTENT AND
                    //     MATRIC POTENTIAL
                    _matrix.A2[n] = _matrix.A2[n] / Constn.Rhol;
                    if (icesdt == 0)
                    {
                        //       NO ICE PRESENT AT SOIL SURFACE - WATER BALANCE FOR MATRIC POT.
                        _matrix.B2[n] = -_timewt.Wdt * con[nc] * vapcdt[nc + 1] * 0.018 * Constn.G / (Constn.Ugas * (tcdt[nc + 1] + 273.16) * Constn.Rhol);
                        _matrix.C2[n - 1] = _matrix.C2[n - 1] * vapcdt[nc + 1] * 0.018 * Constn.G / (Constn.Ugas * (tcdt[nc + 1] + 273.16));
                    }
                    else
                    {
                        //       ICE IS PRESENT AT SOIL SURFACE - WATER BALANCE FOR ICE CONTENT
                        _matrix.B2[n] = 0.0;
                        _matrix.C2[n - 1] = 0.0;
                    }
                    _matrix.D2[n] = qvc[nc] / Constn.Rhol;
                    //
                }
            }
            //XOUT
            if (nsp > 0)
            {
                _writeit.Xlenc = Constn.Ls * qvc[nc];
            }
            else
            {
                _writeit.Xlenc = Constn.Lv * qvc[nc];
            }
        }

        // line 8527
        private void Snowbc(ref int n, ref int nsp, ref int nr, double[] zsp, double[] qvsp, ref double vapspt, double[] tsdt, double[] tspdt, ref int icesdt)
        {
            //
            //     THIS SUBROUTINE SETS UP THE UPPER BOUNDARY CONDITION FOR THE WATER
            //     BALANCE OF THE RESIDUE-SOIL SYSTEM WHEN THERE IS A SNOWPACK
            //***********************************************************************
            //

            //**** DETERMINE THE VAPOR DIFFUSIVITY OF BOTTOM SNOWPACK NODE
            var vdif = Spprop.Vdifsp * (Constn.P0 / _constn.Presur) * Math.Pow((1.0 + tspdt[nsp] / 273.16), Spprop.Vapspx);
            var con = vdif / (zsp[nsp + 1] - zsp[nsp]);
            //
            //**** DEFINE COEFFICIENTS
            if (nr > 0)
            {
                //        CALCULATE BOUNDARY COEFFICIENTS FOR SURFACE RESIDUE NODE
                _matrix.B2[n] = -_timewt.Wdt * con;
                _matrix.D2[n] = qvsp[nsp];
            }
            else
            {
                //        CALCULATE BOUNDARY COEFFICIENTS FOR SURFACE SOIL NODE
                if (icesdt == 0)
                {
                    //           NO ICE PRESENT AT SOIL SURFACE - MATRIC POTENTIAL UNKNOWN
                    _matrix.B2[n] = -_timewt.Wdt * con * vapspt * 0.018 * Constn.G / (Constn.Ugas * (tsdt[1] + 273.16)) / Constn.Rhol;
                }
                else
                {
                    //           ICE PRESENT AT SOIL SURFACE - ICE CONTENT UNKNOWN
                    _matrix.B2[n] = 0.0;
                }
                _matrix.D2[n] = qvsp[nsp] / Constn.Rhol;
            }
        }

        // line 8569
        private void Wbres(ref int n, ref int nr, ref int nsp, double[] zr, double[] tr, double[] trdt, double[] vapr, double[] vaprdt, double[] gmc, double[] gmcdt, double[] rhor, ref double restkb, double[] qvr, double[] rhosp, ref int icesdt, ref int iter)
        {
            //
            //     THIS SUBOUTINE CALCULATES THE NEWTON-RAPHSON COEFFICIENTS FOR THE
            //     WATER BALANCE OF THE RESIDUE LAYERS.
            //
            //***********************************************************************
            //
            var conv = new double[11];
            var vapcon = new double[11];
            var convec = new double[11];
            var tkres = new double[11];

            //**** DETERMINE THE HEAT TRANSFER COEFFICIENT FOR EACH NODE
            Restk(ref nr, ref nsp, tkres, convec, tr, trdt, gmc, gmcdt, rhor, ref restkb, rhosp);
            //
            //**** DETERMINE THE VAPOR TRANSPORT FROM THE THERMAL CONVECTION
            Resvk(ref nr, ref nsp, tr, trdt, convec, vapcon);
            var nr1 = nr + 1;
            vapcon[nr1] = vapcon[nr];
            //
            //**** DETERMINE THE CONDUCTANCE TERM FOR CONVECTIVE VAPOR TRANSPORT
            Conduc(nr1, zr, vapcon, conv);
            //
            //**** DETERMINE THE VAPOR FLUX BETWEEN RESIDUE NODES
            Qvres(ref nr, qvr, conv, vapr, vaprdt);
            //
            //**** DETERMINE THE COEFFICIENTS FOR THE SURFACE LAYER
            _matrix.C2[n] = _timewt.Wdt * conv[1];
            if (nr > 1)
            {
                _matrix.B2[n] = _matrix.B2[n] - _timewt.Wdt * (conv[1] + _residu.Evapk[1]) - (zr[2] - zr[1]) / (2 * _timewt.Dt);
                _matrix.D2[n] = _matrix.D2[n] - qvr[1] - _residu.Evap[1] - (zr[2] - zr[1]) / (2 * _timewt.Dt) * (vaprdt[1] - vapr[1]);
            }
            else
            {
                _matrix.B2[n] = _matrix.B2[n] - _timewt.Wdt * (conv[1] + _residu.Evapk[1]) - (zr[2] - zr[1]) / _timewt.Dt;
                _matrix.D2[n] = _matrix.D2[n] - qvr[1] - _residu.Evap[1] - (zr[2] - zr[1]) / _timewt.Dt * (vaprdt[1] - vapr[1]);
            }
            //
            //**** DETERMINE THE COEFFICIENTS FOR THE REST OF THE RESIDUE LAYERS
            for (var i = n + 1; i <= n + nr - 1; ++i)
            {
                var j = i - n + 1;
                _matrix.A2[i] = _timewt.Wdt * conv[j - 1];
                _matrix.C2[i] = _timewt.Wdt * conv[j];
                if (j != nr)
                {
                    _matrix.B2[i] = -_timewt.Wdt * (conv[j - 1] + conv[j] + _residu.Evapk[j]) - (zr[j + 1] - zr[j - 1]) / 2.0 / _timewt.Dt;
                    _matrix.D2[i] = qvr[j - 1] - qvr[j] - _residu.Evap[j] - (zr[j + 1] - zr[j - 1]) / (2 * _timewt.Dt) * (vaprdt[j] - vapr[j]);
                }
                else
                {
                    _matrix.B2[i] = -_timewt.Wdt * (conv[j - 1] + conv[j] + _residu.Evapk[j]) - (zr[j + 1] - zr[j] + (zr[j] - zr[j - 1]) / 2) / _timewt.Dt;
                    _matrix.D2[i] = qvr[j - 1] - qvr[j] - _residu.Evap[j] - (zr[j + 1] - zr[j] + (zr[j] - zr[j - 1]) / 2) / _timewt.Dt * (vaprdt[j] - vapr[j]);
                }
            label10:;
            }
            n = n + nr;
            //
            //**** DETERMINE THE COEFFICIENTS SOIL SURFACE
            _matrix.A2[n] = _timewt.Wdt * conv[nr] / Constn.Rhol;
            if (icesdt == 0)
            {
                //        NO ICE IS PRESENT AT SOIL SURFACE
                _matrix.B2[n] = -_timewt.Wdt * conv[nr] * vaprdt[nr + 1] * 0.018 * Constn.G / Constn.Ugas / (trdt[nr + 1] + 273.16) / Constn.Rhol;
                _matrix.C2[n - 1] = _matrix.C2[n - 1] * vaprdt[nr + 1] * .018 * Constn.G / Constn.Ugas / (trdt[nr + 1] + 273.16);
            }
            else
            {
                //        ICE IS PRESENT AT SOIL SURFACE - WATER BALANCE FOR ICE CONTENT
                _matrix.B2[n] = 0.0;
                _matrix.C2[n - 1] = 0.0;
            }
            _matrix.D2[n] = qvr[nr] / Constn.Rhol;
        }

        // line 8653
        private void Wbsoil(ref int n, ref int ns, double[] zs, double[] ts, double[] tsdt, double[] mat, double[] matdt, double[] vlc, double[] vlcdt, double[] vic, double[] vicdt, double[][] conc, double[][] concdt, int[] icesdt, double[] qsl, double[] qsv, double[] xtract, ref double seep, double[] flolat, double[] us, ref double slope, ref int iter)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE COEFFICIENTS FOR THE SOIL IN THE
            //     NEWTON-RAPHSON PROCEDURE OF THE ENERGY BALANCE
            //
            //***********************************************************************
            //
            var qsldt = new double[51];
            var hkt = new double[51];
            var hkdt = new double[51];
            var con = new double[51];
            var cont = new double[51];
            var qsvdt = new double[51];
            var dconvp = new double[51];
            var node = new int[51];

            var iflag = 0;
            var qmax = 0.0;
            var isat = 0;
            var flomax = 0.0;
            var dldm = 0.0;
            var j = 0;
            var jtop = 0;
            var i = 0;
            var itop = 0;
            var close = 0.0;
            var closvlc = 0.0;

            //     INITIALIZE A2(1) IF SOIL IS FIRST MATERIAL
            if (n == 1) _matrix.A2[n] = 0.0;
            //
            //**** DETERMINE THE AVERAGE VAPOR FLUX BETWEEN SOIL NODES
            if (iter == 1) Qvsoil(ref ns, _wbsoilSave.Qsvt, zs, ts, mat, vlc, vic, conc, dconvp);
            Qvsoil(ref ns, qsvdt, zs, tsdt, matdt, vlcdt, vicdt, concdt, dconvp);
            Weight(ns - 1, qsv, _wbsoilSave.Qsvt, qsvdt);
            //
            //**** DETERMINE THE LIQUID MOISTURE FLUX BETWEEN NODES
            if (iter == 1)
            {
                //****    CALCULATE FLUX AT BEGINNING OF TIME STEP
                //****    DETERMINE THE HYDRAULIC CONDUCTIVITY OF EACH SOIL NODE
                Soilhk(ns, hkt, mat, vlc, vic);
                //****    DETERMINE THE CONDUCTANCE TERM BETWEEN SOIL NODES
                Conduc(ns, zs, hkt, cont);
                Qlsoil(ns, zs, _wbsoilSave.Qslt, cont, mat);
            }
            Soilhk(ns, hkdt, matdt, vlcdt, vicdt);
            Conduc(ns, zs, hkdt, con);
            Qlsoil(ns, zs, qsldt, con, matdt);
            //
            //**** OBTAIN THE AVERAGE MOISTURE FLUX OVER THE TIME STEP
            Weight(ns - 1, qsl, _wbsoilSave.Qslt, qsldt);
            //
            //     LIMIT NET WATER FLUX INTO NODES IF FLUX FILLS NODE BEYOND
            //     AVAILABLE POROSITY AS A RESULT DOWNWARD FLUX INTO LAYERS WITH
            //     LIMITED PERMEABILITY (PERHAPS DUE TO ICE)
            //     (START AT BOTTOM OF PROFILE AND WORK UP SO THAT ANY CHANGES IN THE
            //     FLUXES CAN WORK THEIR UP THROUGH THE PROFILE)
            iflag = 0;
            for (i = ns - 1; i >= 2; --i)
            {
                if (vicdt[i] > 0.0000)
                {
                    qmax = (_slparm.Soilwrc[i][2] - vlc[i] - vic[i]) * (zs[i + 1] - zs[i - 1]) / 2.0 / _timewt.Dt;
                    if (qmax < 0.0) qmax = 0.0;
                    if (qsl[i - 1] - qsl[i] > qmax)
                    {
                        iflag = 1;
                        if (qsl[i - 1] > 0.0)
                        {
                            con[i - 1] = 0.0;
                            qsl[i - 1] = qsl[i] + qmax;
                            if (qsl[i - 1] < 0.0)
                            {
                                qsl[i - 1] = 0.0;
                                qsl[i] = qsl[i - 1] - qmax;
                                if (qsl[i] > 0.0) qsl[i] = 0.0;
                            }
                        }
                        else
                        {
                            con[i] = 0.0;
                            qsl[i] = qsl[i - 1] - qmax;
                            if (qsl[i] > 0.0) qsl[i] = 0.0;
                        }
                    }
                }
            label5:;
            }
            if (vicdt[1] > 0.0000)
            {
                qmax = (_slparm.Soilwrc[1][2] - vlc[1] - vic[1]) * (zs[2] - zs[1]) / 2.0 / _timewt.Dt;
                if (qmax < 0.0) qmax = 0.0;
                if (-qsl[1] > qmax)
                {
                    iflag = 1;
                    con[1] = 0.0;
                    qsl[1] = -qmax;
                }
            }
            //
            if (iflag > 0)
            {
                for (i = 2; i <= ns - 1; ++i)
                {
                    if (vicdt[i] > 0.0000)
                    {
                        qmax = (_slparm.Soilwrc[i][2] - vlc[i] - vic[i]) * (zs[i + 1] - zs[i - 1]) / 2.0 / _timewt.Dt;
                        if (qmax < 0.0) qmax = 0.0;
                        if (qsl[i - 1] - qsl[i] > qmax)
                        {
                            if (qsl[i] < 0.0)
                            {
                                con[i] = 0.0;
                                qsl[i] = qsl[i - 1] - qmax;
                                if (qsl[i] > 0.0)
                                {
                                    qsl[i] = 0.0;
                                    qsl[i - 1] = qsl[i] + qmax;
                                    if (qsl[i - 1] < 0.0) qsl[i - 1] = 0.0;
                                }
                            }
                            else
                            {
                                con[i - 1] = 0.0;
                                qsl[i - 1] = qsl[i] + qmax;
                                if (qsl[i - 1] < 0.0) qsl[i - 1] = 0.0;
                            }
                        }
                    }
                label10:;
                }
            }
            //
            //     COUNT NUMBER OF SATURATED NODS
            isat = 0;
            //
            //**** DETERMINE THE COEFFICIENTS FOR THE SURFACE LAYER
            if (matdt[1] >= 0.0)
            {
                //        ALLOW FOR LATERAL FLOW EXITING PROFILE
                flolat[1] = _slparm.Satklat[1] * Math.Sin(slope) * (zs[2] - zs[1]) / 2.0;
                //        LIMIT LATERAL FLOW TO NET INFLUX SO THAT LAYER DOES NOT
                //        DESATURATE DUE TO LATERAL FLOW ALONE
                flomax = _matrix.D2[n] - qsl[1] - qsv[1] / Constn.Rhol + us[1] - xtract[1] / Constn.Rhol;
                if (flomax < 0.0) flomax = 0.0;
                if (flolat[1] > flomax) flolat[1] = flomax;
            }
            else
            {
                flolat[1] = 0.0;
            }
            _matrix.D2[n] = _matrix.D2[n] - qsl[1] - qsv[1] / Constn.Rhol + us[1] - xtract[1] / Constn.Rhol - flolat[1] - (zs[2] - zs[1]) * (vlcdt[1] - vlc[1] + Constn.Rhoi / Constn.Rhol * (vicdt[1] - vic[1])) / 2.0 / _timewt.Dt;
            if (icesdt[1] == 1)
            {
                //        ICE IS PRESENT -- CALCULATE COEFFICIENTS FOR ICE CONTENT
                _matrix.B2[n] = -(zs[2] - zs[1]) / (2.0 * _timewt.Dt) * Constn.Rhoi / Constn.Rhol;
                _matrix.A2[n + 1] = 0.0;
                seep = 0.0;
            }
            else
            {
                //        NO ICE IS PRESENT -- CALCULATE COEFFICIENTS FOR LIQUID BALANCE
                if (matdt[1] < 0.0 || _matrix.D2[n] < 0.0)
                {
                    //           NO SEEPAGE
                    Matvl3(1, ref matdt[1], ref vlcdt[1], ref dldm);
                    //XXXX       IF (MATDT(1) .GT. SoilWRC(1,1) .or. dldm .eq. 0.0) THEN
                    if (matdt[1] > _slparm.Soilwrc[1][1] || matdt[1] >= 0.0)
                    {
                        //              NODE IS SATURATED - CHECK IF BOUNDARY IS POORLY DEFINED
                        if (-_matrix.B2[n] < con[1] * 1.0e-6 || con[1] <= 0.0)
                        {
                            //                 SOIL WATER FLOW OVERWHELMS SURFACE BOUNDARY CONDITION
                            isat = isat + 1;
                            node[isat] = 1;
                        }
                    }
                    _matrix.B2[n] = _matrix.B2[n] - _timewt.Wdt * (con[1] + dconvp[1] / Constn.Rhol) - (zs[2] - zs[1]) / (2.0 * _timewt.Dt) * dldm;
                    _matrix.A2[n + 1] = _timewt.Wdt * (con[1] + dconvp[1] / Constn.Rhol);
                    seep = 0.0;
                }
                else
                {
                    //           ALLOW FOR SEEPAGE FROM SURFACE NODE - BOUNDARY IS PROPERLY
                    //           DEFINED (DO NOT INCLUDE AS A SATURATED NODE)
                    _matrix.B2[n] = 1.0;
                    _matrix.A2[n + 1] = _timewt.Wdt * con[1];
                    //           NET FLOW INTO NODE IS EQUAL TO SEEPAGE FROM SURFACE
                    seep = _matrix.D2[n];
                    _matrix.D2[n] = 0.0;
                }
            }
            //
            //**** DETERMINE THE COEFFICIENTS FOR THE REST OF THE PROFILE
            for (i = n + 1; i <= n + ns - 2; ++i)
            {
                j = i - n + 1;
                if (icesdt[j] == 1)
                {
                    //           ICE IS PRESENT -- CALCULATE COEFFICIENTS FOR ICE CONTENT
                    _matrix.B2[i] = -(zs[j + 1] - zs[j - 1]) / (2.0 * _timewt.Dt) * Constn.Rhoi / Constn.Rhol;
                    _matrix.C2[i - 1] = 0.0;
                    _matrix.A2[i + 1] = 0.0;
                    flolat[j] = 0.0;
                }
                else
                {
                    //           NO ICE IS PRESENT -- CALCULATE COEFF. FOR LIQUID BALANCE
                    Matvl3(j, ref matdt[j], ref vlcdt[j], ref dldm);
                    //XXXX       IF (MATDT(J) .GT. SoilWRC(J,1) .or. dldm .eq. 0.0) THEN
                    if (matdt[j] > _slparm.Soilwrc[j][1] || matdt[j] >= 0.0)
                    {
                        //              NODE IS SATURATED
                        isat = isat + 1;
                        node[isat] = j;
                        //              COMPUTE LATERAL FLOW EXITING PROFILE
                        flolat[j] = _slparm.Satklat[j] * Math.Sin(slope) * (zs[j + 1] - zs[j - 1]) / 2.0;
                        //              LIMIT LATERAL FLOW SO LAYER DOES NOT DESATURATE
                        flomax = qsl[j - 1] - qsl[j] + (qsv[j - 1] - qsv[j]) / Constn.Rhol + us[j] - xtract[j] / Constn.Rhol - (zs[j + 1] - zs[j - 1]) / (2.0 * _timewt.Dt) * (vlcdt[j] - vlc[j] + Constn.Rhoi / Constn.Rhol * (vicdt[j] - vic[j]));
                        if (flomax < 0.0) flomax = 0.0;
                        if (flolat[j] > flomax) flolat[j] = flomax;
                    }
                    else
                    {
                        flolat[j] = 0.0;
                    }
                    _matrix.B2[i] = -_timewt.Wdt * (con[j - 1] + con[j] + (dconvp[j - 1] + dconvp[j]) / Constn.Rhol) - (zs[j + 1] - zs[j - 1]) / (2.0 * _timewt.Dt) * dldm;
                    _matrix.C2[i - 1] = _timewt.Wdt * (con[j - 1] + dconvp[j - 1] / Constn.Rhol);
                    _matrix.A2[i + 1] = _timewt.Wdt * (con[j] + dconvp[j] / Constn.Rhol);
                }
                _matrix.D2[i] = qsl[j - 1] - qsl[j] + (qsv[j - 1] - qsv[j]) / Constn.Rhol + us[j] - xtract[j] / Constn.Rhol - flolat[j] - (zs[j + 1] - zs[j - 1]) / (2.0 * _timewt.Dt) * (vlcdt[j] - vlc[j] + Constn.Rhoi / Constn.Rhol * (vicdt[j] - vic[j]));
            label20:;
            }
            //
            //     ADJUST MATRIX IF SEEPAGE OCCURS
            if (seep > 0.0)
            {
                _matrix.A2[n] = 0.0;
                _matrix.C2[n] = 0.0;
            }
            //
            if (isat > 0)
            {
                //        SATURATED NODES ARE PRESENT - CHECK FOR MATRIX SINGULARITY
                jtop = 0;
                node[isat + 1] = ns;
                for (j = 1; j <= isat; ++j)
                {
                    i = node[j] + n - 1;
                    //           CHECK FOR MATRIX DISCONTINUITY AND SAVE TOP NODE
                    if (node[j] == 1)
                    {
                        jtop = node[j];
                    }
                    else
                    {
                        //xxxx          IF (CON(NODE(J)-1).LE.0.0) JTOP=NODE(J)
                        //              CHECK IF CONDUCTIVITY OF NODE ABOVE IS SUFFICIENTLY SMALL
                        //              TO BE CONSIDERED ZERO
                        if (con[node[j] - 1] <= -_matrix.B2[i] * 1.0e-7) jtop = node[j];
                        //              THIS IS TO CHECK IF THE SPECIFIC STORGAGE OF THE
                        //              UNSATURATED NODE ABOVE THE SATURATED ZONE IS NEGLIBLE
                        if (-con[node[j] - 1] - _matrix.B2[i - 1] <= -_matrix.B2[i] * 1.0e-7 && jtop == 0) jtop = node[j];
                    }
                    if (jtop != 0)
                    {
                        //xxxx          IF (CON(NODE(J)).LE.0.0) THEN
                        if (con[node[j]] <= -_matrix.B2[i] * 1.0e-7)
                        {
                            //                SATURATED NODES SURROUNDED BY ZERO CONDUCTIVITY;
                            //                SOLUTION IS UNDEFINED - RESET B2(I) OF TOP SATURATED
                            //                LAYER AS IF IT IS JUST UNDER AIR ENTRY POTENTIAL
                            itop = jtop + n - 1;
                            if (Slparm.Iwrc == 1 || Slparm.Iwrc == 2)
                            {
                                close = 1.0001 * _slparm.Soilwrc[jtop][1];
                                Matvl3(jtop, ref close, ref vlcdt[jtop], ref dldm);
                            }
                            else if (Slparm.Iwrc == 3)
                            {
                                closvlc = vlcdt[jtop] - 0.001;
                                Matvl1(jtop, ref close, ref closvlc, ref dldm);
                                Matvl3(jtop, ref close, ref closvlc, ref dldm);
                            }
                            if (jtop == 1)
                            {
                                _matrix.B2[itop] = _matrix.B2[itop] - (zs[jtop + 1] - zs[jtop]) / (2.0 * _timewt.Dt) * dldm;
                            }
                            else
                            {
                                _matrix.B2[itop] = _matrix.B2[itop] - (zs[jtop + 1] - zs[jtop - 1]) / (2.0 * _timewt.Dt) * dldm;
                            }
                            jtop = 0;
                        }
                    }
                    //           IF NEXT NODE IS NOT SATURATED, RESET JTOP
                    if (node[j] + 1 != node[j + 1]) jtop = 0;
                    label30:;
                }
            }
            //
            n = n + ns - 2;
        }

        // line 8914
        /// <summary>
        ///   <para>Sets: hk.</para>
        ///   <para>Does not use: vlc.</para>
        /// </summary>
        private void Soilhk(int ns, double[] hk, double[] mat, double[] vlc, double[] vic)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE HYDRAULIC CONDUCTIVITY OF EACH SOIL
            //     NODE
            //
            //***********************************************************************

            //     Calculation of hydraulic conductivity from potential,
            for (var i = 1; i <= ns; ++i)
            {
                if (Slparm.Iwrc == 1)
                {
                    //           This is Campbell model
                    if (_slparm.Soilwrc[i][1] > mat[i])
                    {
                        hk[i] = _slparm.Satk[i] * Math.Pow((_slparm.Soilwrc[i][1] / mat[i]), (2.0 + 3.0 / _slparm.Soilwrc[i][3]));
                    }
                    else
                    {
                        hk[i] = _slparm.Satk[i];
                    }
                }
                else if (Slparm.Iwrc == 2)
                {
                    //           This is Brooks-Corey model
                    if (_slparm.Soilwrc[i][1] > mat[i])
                    {
                        hk[i] = _slparm.Satk[i] * Math.Pow((_slparm.Soilwrc[i][1] / mat[i]), (_slparm.Soilwrc[i][3] * (_slparm.Soilwrc[i][5] + 2.0) + 2.0));
                    }
                    else
                    {
                        hk[i] = _slparm.Satk[i];
                    }
                }
                else if (Slparm.Iwrc == 3)
                {
                    //           This is Van Genuchten-Mualem model
                    if (_slparm.Soilwrc[i][1] > mat[i])
                    {
                        var alfahn = Math.Pow((Math.Abs(_slparm.Soilwrc[i][6] * mat[i])), _slparm.Soilwrc[i][3]);
                        hk[i] = _slparm.Satk[i] * Math.Pow((1.0 - Math.Pow((1.0 - 1.0 / (1.0 + alfahn)), _slparm.Soilwrc[i][7])), 2) / Math.Pow((1.0 + alfahn), (_slparm.Soilwrc[i][5] * _slparm.Soilwrc[i][7]));
                        //              HK(I)=SATK(I)*(1.0-AlfaHN**SoilWRC(I, 7)
                        //    >               *(1+AlfaHN)**(-SoilWRC(I,7)))**2
                        //    >               /(1.0+AlfaHN)**(SoilWRC(I,5)*SoilWRC(I, 7))
                    }
                    else
                    {
                        hk[i] = _slparm.Satk[i];
                    }
                }
                //
                if (vic[i] > 0.0)
                {
                    //           LIMIT CONDUCTIVITY IF ICE CONTENT IS TOO LARGE (BASED ON
                    //           RESULTS FROM BLOOMSBURG AND WANG, 1969
                    //           (CALCULATE POROSITY ADJUSTING SPEC. DENSITY FOR ORG. MATTER
                    var poros = 1.0 - _slparm.Rhob[i] * ((1.0 - _slparm.Om[i]) / Constn.Rhom + _slparm.Om[i] / Constn.Rhoom);
                    if (poros - vic[i] < 0.13)
                    {
                        hk[i] = 0.0;
                    }
                    else
                    {
                        //              LINEARLY REDUCE CONDUCTIVITY TO ZERO AT
                        //              AN AVAILABE POROSITY OF 0.13
                        var fract = (poros - vic[i] - 0.13) / (poros - 0.13);
                        hk[i] = fract * hk[i];
                    }
                }
            label10:;
            }
        }

        // line 8983
        /// <summary>
        ///   <para>Sets: qsl.</para>
        /// </summary>
        private void Qlsoil(int ns, double[] zs, double[] qsl, double[] con, double[] mat)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE LIQUID MOISTURE FLUX BETWEEN
            //     SOIL NODES
            //
            //***********************************************************************

            for (var i = 1; i <= ns - 1; ++i)
            {
                qsl[i] = con[i] * (mat[i] - mat[i + 1] + zs[i + 1] - zs[i]);
            label10:;
            }
        }

        // line 8999
        private void Backup(ref int ns, ref int nr, ref int nsp, ref int nc, ref int nplant, ref int nsalt, int[] ices, int[] icesdt, double[] ts, double[] tsdt, double[] mat, double[] matdt, double[][] conc, double[][] concdt, double[] vlc, double[] vlcdt, double[] vic, double[] vicdt, double[][] salt, double[][] saltdt, double[] tr, double[] trdt, double[] vapr, double[] vaprdt, double[] gmc, double[] gmcdt, double[] tc, double[] tcdt, double[][] tlc, double[][] tlcdt, double[] vapc, double[] vapcdt, double[] wcan, double[] wcandt, double[] pcan, double[] pcandt, double[] tsp, double[] tspdt, double[] dlw, double[] dlwdt, int[] icesp, int[] icespt)
        {
            //
            //     THIS SUBROUTINE SETS END OF TIME STEP VALUES BACK TO THOSE FOR
            //     THE BEGINNING OF THE TIME STEP.
            //
            //***********************************************************************

            //     SOIL PROPERTIES
            for (var i = 1; i <= ns; ++i)
            {
                tsdt[i] = ts[i];
                matdt[i] = mat[i];
                vlcdt[i] = vlc[i];
                vicdt[i] = vic[i];
                icesdt[i] = ices[i];
                for (var j = 1; j <= _slparm.Nsalt; ++j)
                {
                    saltdt[j][i] = salt[j][i];
                    concdt[j][i] = conc[j][i];
                label10:;
                }
            label15:;
            }
            //
            //     RESIDUE PROPERTIES
            for (var i = 1; i <= nr; ++i)
            {
                trdt[i] = tr[i];
                vaprdt[i] = vapr[i];
                gmcdt[i] = gmc[i];
            label20:;
            }
            //
            //     SNOWPACK PROPERTIES
            for (var i = 1; i <= nsp; ++i)
            {
                icespt[i] = icesp[i];
                tspdt[i] = tsp[i];
                dlwdt[i] = dlw[i];
            label30:;
            }
            //
            //     CANOPY PROPERTIES
            for (var i = 1; i <= nc; ++i)
            {
                tcdt[i] = tc[i];
                vapcdt[i] = vapc[i];
                wcandt[i] = wcan[i];
                for (var j = 1; j <= nplant; ++j)
                {
                    tlcdt[j][i] = tlc[j][i];
                label35:;
                }
            label40:;
            }
            if (nc > 0)
            {
                for (var j = 1; j <= nplant; ++j)
                {
                    pcandt[j] = pcan[j];
                label45:;
                }
            }
        }

        // line 9063
        private void Solute(ref int ns, double[] zs, double[][] conc, double[][] concdt, double[][] salt, double[][] saltdt, double[] vlc, double[] vlcdt, double[] ts, double[] tsdt, double[] qsl, double[] xtract, double[][] sink, double[] dgrade, double[] sltdif, double[] asalt, double[] disper)
        {
            //
            //     THIS SUBROUTINE SOLVES THE SOLUTE BALANCE OF THE SOIL FOR EACH
            //     TYPE OF SOLUTE.  THE SOLUTION FOR THE SOLUTE BALANCE IS A LINEAR
            //     SET OF EQUATIONS, AND CAN THEREFORE BE SOLVED DIRECTLY.
            //     (NO NEWTON-RAPHSON ITERATION REQUIRED)
            //
            //***********************************************************************
            //
            var diff = new double[51];
            var difft = new double[51];
            var diffdt = new double[51];
            var con = new double[51];
            var a3 = new double[51];
            var b3 = new double[51];
            var c3 = new double[51];
            var d3 = new double[51];
            var delta = new double[51];
            var epslon = new double[51];
            var concnw = new double[51];

            var aa = 0.0;
            var bb = 0.0;
            var cc = 0.0;

            //**** SET UP LIQUID FLUX COEFFICIENTS SO THAT SOLUTES ARE CARRIED IN
            //**** DIRECTION OF MOISTURE MOVEMENT.
            for (var i = 1; i <= ns - 1; ++i)
            {
                if (qsl[i] > 0.0)
                {
                    delta[i] = 1.0;
                    epslon[i] = 0.0;
                }
                else
                {
                    delta[i] = 0.0;
                    epslon[i] = 1.0;
                }
            label10:;
            }
            //
            //**** START LOOP FOR EACH TYPE OF SOLUTE
            for (var j = 1; j <= _slparm.Nsalt; ++j)
            {
                //        CALCULATE DIFFUSION COEFFICIENT FOR EACH LAYER
                Saltk1(ns, j, difft, zs, vlc, ts, qsl, sltdif, asalt, disper);
                Saltk1(ns, j, diffdt, zs, vlcdt, tsdt, qsl, sltdif, asalt, disper);
                //
                //****    DETERMINE AVERAGE DIFFUSIVITY OVER TIME STEP, AND CONDUCTANCE
                //****    TERM BETWEEN NODES
                Weight(ns, diff, difft, diffdt);
                Conduc(ns, zs, diff, con);
                Saltk2(ns, j, con, zs, vlc, vlcdt, qsl, sltdif, asalt, disper);
                //
                //****    DETERMINE THE COEFFICIENTS FOR THE SURFACE LAYER
                bb = Constn.Rhol * (con[1] + qsl[1] * delta[1]) + xtract[1];
                cc = Constn.Rhol * (con[1] - qsl[1] * epslon[1]);
                b3[1] = -_timewt.Wdt * bb - _slparm.Rhob[1] * (zs[2] - zs[1]) / (2.0 * _timewt.Dt) * (_slparm.Saltkq[j][1] + vlcdt[1] * Constn.Rhol / _slparm.Rhob[1]);
                c3[1] = _timewt.Wdt * cc;
                d3[1] = -_timewt.Wt * cc * conc[j][2] + (_timewt.Wt * bb - _slparm.Rhob[1] * (zs[2] - zs[1]) / (2.0 * _timewt.Dt) * (_slparm.Saltkq[j][1] + vlc[1] * Constn.Rhol / _slparm.Rhob[1])) * conc[j][1] + sink[j][1];
                //
                //****    DETERMINE THE COEFFICIENTS FOR THE REST OF THE PROFILE
                for (var i = 2; i <= ns - 1; ++i)
                {
                    aa = Constn.Rhol * (con[i - 1] + qsl[i - 1] * delta[i - 1]);
                    bb = Constn.Rhol * (con[i - 1] - qsl[i - 1] * epslon[i - 1] + con[i] + qsl[i] * delta[i]) + xtract[i];
                    cc = Constn.Rhol * (con[i] - qsl[i] * epslon[i]);
                    a3[i] = _timewt.Wdt * aa;
                    b3[i] = -_timewt.Wdt * bb - _slparm.Rhob[i] * (zs[i + 1] - zs[i - 1]) / (2.0 * _timewt.Dt) * (_slparm.Saltkq[j][i] + vlcdt[i] * Constn.Rhol / _slparm.Rhob[i]);
                    c3[i] = _timewt.Wdt * cc;
                    d3[i] = -_timewt.Wt * aa * conc[j][i - 1] - _timewt.Wt * cc * conc[j][i + 1] + sink[j][i] + (_timewt.Wt * bb - _slparm.Rhob[i] * (zs[i + 1] - zs[i - 1]) / (2.0 * _timewt.Dt) * (_slparm.Saltkq[j][i] + vlc[i] * Constn.Rhol / _slparm.Rhob[i])) * conc[j][i];
                //
                label30:;
                }
                //
                //****    ADJUST D3(NS-1) FOR KNOWN BOUNDARY CONDITION AT NODE NS
                //        D3(NS-1)=D3(NS-1) - C3(NS)*CONCDT(J,NS)
                b3[ns - 1] = b3[ns - 1] + c3[ns - 1];
                //
                //****    SOLVE SET OF EQUATIONS FOR CONCENTRATION AT END OF TIME STEP
                Tdma(ns - 1, a3, b3, c3, d3, concnw);
                concnw[ns] = concnw[ns - 1];
                //
                //****    UPDATE SOLUTE CONCENTRATIONS AND TOTAL SALTS FOR EACH NODE
                for (var i = 1; i <= ns; ++i)
                {
                    concdt[j][i] = concnw[i];
                    saltdt[j][i] = (_slparm.Saltkq[j][i] + vlcdt[i] * Constn.Rhol / _slparm.Rhob[i]) * concdt[j][i];
                    //           ADJUST TOTAL SALTS AND SOLUTE CONCENTRATION FOR DEGRADATION
                    saltdt[j][i] = saltdt[j][i] * Math.Exp(-dgrade[j] * _timewt.Dt / 86400.0);
                    concdt[j][i] = saltdt[j][i] / (_slparm.Saltkq[j][i] + vlcdt[i] * Constn.Rhol / _slparm.Rhob[i]);
                label40:;
                }
            label50:;
            }
        }

        // line 9183
        private void Saltk1(int ns, int j, double[] diff, double[] zs, double[] vlc, double[] ts, double[] qsl, double[] sltdif, double[] asalt, double[] disper)
        {
            //
            //     THIS SUBROUTINE IS DIVIDED INTO TWO PARTS.  THE FIRST DETERMINES
            //     THE MOLECULAR DIFFUSION OF SOLUTES AT EACH OF THE SOIL NODES.
            //     AFTER WEIGHTING THE MOLECULAR DIFFUSION OVER TIME AND CALCULATING
            //     THE CONDUCTANCE DUE TO MOLECULAR DIFFUSION BETWEEN NODES IN THE
            //     MAIN PORTION OF THE PROGRAM, THE SECOND PORTION OF THE SUBROUTINE
            //     MAY BE CALLED TO ADD THE CONDUCTANCE BETWEEN NODES DUE TO
            //     HYDRODYNAMIC DISPERSION (WHICH IS A FUNCTION OF LIQUID FLUX
            //     BETWEEN NODES).
            //
            //***********************************************************************
            //

            //****
            //     THIS PORTION OF THE SUBROUTINE CALCULATES THE MOLECULAR DIFFUSION
            //     OF THE SOLUTES AT THE SOIL NODES.  (SINCE LIQUID FLUX IS
            //     DETERMINED FOR BETWEEN NODES, THE HYDRODYNAMIC DISPERSION CANNOT
            //     BE ADDED ON UNTIL THE MOLECULAR DIFFUSION IS DETERMINED FOR
            //     BETWEEN NODES IN THE "CONDUC" SUBROUTINE)
            for (var i = 1; i <= ns; ++i)
            {
                diff[i] = sltdif[j] * asalt[i] * Math.Pow(vlc[i], 3) * ((ts[i] + 273.16) / 273.16);
            label10:;
            }
        }

        // line 9193
        private void Saltk2(int ns, int j, double[] con, double[] zs, double[] vlc, double[] vlcdt, double[] qsl, double[] sltdif, double[] asalt, double[] disper)
        {
            //
            //     THIS SUBROUTINE IS DIVIDED INTO TWO PARTS.  THE FIRST DETERMINES
            //     THE MOLECULAR DIFFUSION OF SOLUTES AT EACH OF THE SOIL NODES.
            //     AFTER WEIGHTING THE MOLECULAR DIFFUSION OVER TIME AND CALCULATING
            //     THE CONDUCTANCE DUE TO MOLECULAR DIFFUSION BETWEEN NODES IN THE
            //     MAIN PORTION OF THE PROGRAM, THE SECOND PORTION OF THE SUBROUTINE
            //     MAY BE CALLED TO ADD THE CONDUCTANCE BETWEEN NODES DUE TO
            //     HYDRODYNAMIC DISPERSION (WHICH IS A FUNCTION OF LIQUID FLUX
            //     BETWEEN NODES).
            //
            //***********************************************************************
            //

            //****
            //     THIS PORTION OF THE SUBROUTINE ADDS THE CONDUCTANCE DUE TO
            //     HYDRODYNAMIC DISPERSION TO THE CONDUCTANCE DUE TO MOLECULAR
            //     DIFFUSION (BETWEEN NODES).
            for (var i = 1; i <= ns - 1; ++i)
            {
                var avgvlc = _timewt.Wt * (vlc[i] + vlc[i + 1]) / 2.0 + _timewt.Wdt * (vlcdt[i] + vlcdt[i + 1]) / 2.0;
                con[i] = con[i] + disper[i] * Math.Abs(qsl[i]) / avgvlc / (zs[i + 1] - zs[i]);
            label20:;
            }
        }

        // line 9202
        private void Sumdt(ref int nc, ref int nplant, ref int nsp, ref int nr, ref int ns, ref double hflux, ref double vflux, ref double gflux, double[][] lwcan, ref double lwsnow, double[] lwres, ref double lwsoil, double[] lwdown, double[] lwup, double[][] swcan, double[] swsnow, double[] swres, ref double swsoil, double[] swdown, double[] swup, double[] qvc, double[] qvr, double[] qvsp, double[] qsl, double[] qsv, double[] trnsp, double[] xtract, double[] flolat, ref double seep, ref double tswsno, ref double tlwsno, ref double tswcan, ref double tlwcan, ref double tswres, ref double tlwres, ref double tswsoi, ref double tlwsoi, ref double tswdwn, ref double tlwdwn, ref double tswup, ref double tlwup, ref double thflux, ref double tgflux, ref double evap1, ref double etsum, ref double tseep, double[] rootxt, double[] totflo, double[] totlat, ref int ntimes, ref double topsno, double[] tqvsp)
        {
            //
            //     THIS SUBROUTINE SUMS THE NECESSARY FLUXES EACH TIME STEP FOR USE
            //     IN THE WATER AND ENERGY BALANCE SUMMARIES AND IN THE SNOWPACK
            //     ADJUSTMENTS DUE FOR VAPOR TRANSFER
            //
            //***********************************************************************
            //
            //
            //

            if (ntimes == 1)
            {
                tswcan = 0.0;
                tlwcan = 0.0;
                tswsno = 0.0;
                tlwsno = 0.0;
                tswres = 0.0;
                tlwres = 0.0;
                tswsoi = 0.0;
                tlwsoi = 0.0;
                tswdwn = 0.0;
                tlwdwn = 0.0;
                tswup = 0.0;
                tlwup = 0.0;
                thflux = 0.0;
                tgflux = 0.0;
                evap1 = 0.0;
                etsum = 0.0;
                tseep = 0.0;
                topsno = 0.0;
                for (var j = 1; j <= nplant; ++j)
                {
                    _sumdtSave.Transp[j] = 0.0;
                label6:;
                }
                for (var i = 1; i <= nsp; ++i)
                {
                    tqvsp[i] = 0.0;
                label8:;
                }
                for (var i = 1; i <= ns; ++i)
                {
                    totflo[i] = 0.0;
                    totlat[i] = 0.0;
                    rootxt[i] = 0.0;
                label10:;
                }
            }
            //
            //     SUM UPWARD AND DOWNWARD RADIATION ABOVE PROFILE (IN KILOJOULES)
            tswdwn = tswdwn + swdown[1] * _timewt.Dt / 1000.0;
            tlwdwn = tlwdwn + lwdown[1] * _timewt.Dt / 1000.0;
            tswup = tswup + swup[1] * _timewt.Dt / 1000.0;
            tlwup = tlwup + lwup[1] * _timewt.Dt / 1000.0;
            //
            //     SUM RADIATION ABSORBED BY THE CANOPY (IN KILOJOULES)
            for (var i = 1; i <= nc; ++i)
            {
                tswcan = tswcan + swcan[nplant + 1][i] * _timewt.Dt / 1000.0;
                tlwcan = tlwcan + lwcan[nplant + 1][i] * _timewt.Dt / 1000.0;
            label20:;
            }
            //
            //     SUM RADIATION ABSORBED BY THE SNOWPACK (IN KILOJOULES)
            for (var i = 1; i <= nsp; ++i)
            {
                tswsno = tswsno + swsnow[i] * _timewt.Dt / 1000.0;
            label30:;
            }
            if (nsp > 0) tlwsno = tlwsno + lwsnow * _timewt.Dt / 1000.0;
            //
            //     SUM RADIATION ABSORBED BY THE RESIDUE (IN KILOJOULES)
            for (var i = 1; i <= nr; ++i)
            {
                tswres = tswres + swres[i] * _timewt.Dt / 1000.0;
                tlwres = tlwres + lwres[i] * _timewt.Dt / 1000.0;
            label40:;
            }
            //
            //     SUM RADIATION ABSORBED BY THE SOIL (IN KILOJOULES)
            tswsoi = tswsoi + swsoil * _timewt.Dt / 1000.0;
            tlwsoi = tlwsoi + lwsoil * _timewt.Dt / 1000.0;
            //
            //     SUM SENSIBLE HEAT FLUX AND SOIL HEAT FLUX
            thflux = thflux + hflux * _timewt.Dt / 1000.0;
            tgflux = tgflux + gflux * _timewt.Dt / 1000.0;
            //
            //     SUM THE EVAPORATION, TRANSPIRATION FROM EACH PLANT SPECIES
            evap1 = evap1 + vflux / Constn.Rhol * _timewt.Dt;
            for (var j = 1; j <= nplant; ++j)
            {
                _sumdtSave.Transp[j] = _sumdtSave.Transp[j] + trnsp[j] * _timewt.Dt;
            label50:;
            }
            etsum = etsum + trnsp[nplant + 1] / Constn.Rhol * _timewt.Dt;
            //
            //     SUM THE VAPOR FLUX OCCURRING IN, ABOVE AND BELOW THE SNOWPACK
            if (nsp > 0)
            {
                if (nc > 0)
                {
                    //           VAPOR FLUX ABOVE SNOWPACK IS FROM BOTTOM OF CANOPY
                    topsno = topsno + qvc[nc] * _timewt.Dt;
                }
                else
                {
                    //           VAPOR FLUX ABOVE SNOWPACK IS FROM ATMOSPHERE
                    topsno = topsno + vflux * _timewt.Dt;
                }
                for (var i = 1; i <= nsp; ++i)
                {
                    tqvsp[i] = tqvsp[i] + qvsp[i] * _timewt.Dt;
                label60:;
                }
                //        TQVSP(NSP) IS THE VAPOR LEAVING BOTTOM OF SNOWPACK
            }
            //
            //     SUM THE ROOT EXTRACTION FROM EACH SOIL LAYER
            if (nplant > 0)
            {
                for (var i = 1; i <= ns; ++i)
                {
                    rootxt[i] = rootxt[i] + xtract[i] * _timewt.Dt;
                label70:;
                }
            }
            //
            //     SUM THE TOTAL WATER FLUX BETWEEN SOIL NODES
            for (var i = 1; i <= ns - 1; ++i)
            {
                totflo[i] = totflo[i] + (qsl[i] + qsv[i] / Constn.Rhol) * _timewt.Dt;
            label80:;
            }
            //
            //     SUM THE TOTAL LATERAL FLUX EXITING PROFILE FROM EACH SOIL LAYER
            for (var i = 1; i <= ns - 1; ++i)
            {
                totlat[i] = totlat[i] + flolat[i] * _timewt.Dt;
            label90:;
            }
            //
            //     SUM THE SEEPAGE FROM THE SURFACE NODE
            tseep = seep * _timewt.Dt;
        }

        // line 9340
        private void Precp(ref int nplant, ref int nc, ref int nsp, ref int nr, ref int ns, ref double ta, ref double tadt, ref double hum, ref double humdt, double[] zc,
            double[] pintrcp, double[] xangle, double[] clumpng, int[] itype, double[] wcandt, ref double wcmax, double[] pcandt, double[] zsp, double[] dzsp, double[] tspdt,
            double[] dlw, double[] dlwdt, double[] rhosp, double[] tqvsp, ref double topsno, double[] wlag, ref double store, ref double snowex, double[] zr, double[] trdt,
            double[] gmcdt, ref double gmcmax, double[] rhor, double[] zs, double[] tsdt, double[] vlcdt, double[] vicdt, double[] matdt, double[] totflo, double[][] saltdt,
            double[][] concdt, int[] icespt, int[] icesdt, ref double rain, ref double pond, ref double runoff, ref double evap1, ref double melt, ref double pondmx, ref int isnotmp,
            ref double snotmp, ref double snoden, ref double dirres, ref double slope)
        {
            //
            //     THIS SUBROUTINE DETERMINES WHERE THE PRECIPITATION AND SNOWMELT
            //     SHOULD GO
            //
            //***********************************************************************
            //
            var scout = 0.0;
            var snow = 0.0;
            var we = 0.0;

            var avgta = _timewt.Wt * ta + _timewt.Wdt * tadt;
            var avghum = _timewt.Wt * hum + _timewt.Wdt * humdt;
            var train = avgta;
            //
            //     RAINFALL INTERCEPTION BY THE CANOPY
            if (rain > 0.0 && nc > 0) Rainc(ref nplant, ref nc, pintrcp, xangle, clumpng, itype, ref rain, pcandt, wcandt, ref wcmax, ref slope);
            melt = 0.0;
            scout = 0.0;
            //
            if (rain > 0.0)
            {
                //        RAIN TEMPERATURE IS ASSUMED TO BE EQUAL TO WET BULB TEMPERATURE
                Wtbulb(ref train, ref avgta, ref avghum);
                double thrshold;
                if (isnotmp == 1)
                {
                    //           USE AIR TEMPERATURE AS SNOW THRESHOLD
                    thrshold = avgta;
                }
                else
                {
                    //           USE WET BULB TEMPERATURE AS SNOW THRESHOLD
                    thrshold = train;
                }
                if (thrshold <= snotmp || snoden > 0.0)
                {
                    //           PRECIPITATION IS ASSUMED TO BE SNOW
                    //           (ADD ANY SNOW EXCESS REMAINING FROM ELIMINATING A SHALLOW
                    //           SNOWCOVER)
                    snow = rain + snowex;
                    pond = pond - snowex;
                    snowex = 0.0;
                    rain = 0.0;
                    if (train > 0.0) train = 0.0;
                    Nwsnow(ref nsp, ref nc, ref nr, ref ns, ref nplant, icespt, zsp, dzsp, rhosp, tspdt, dlw, dlwdt, ref train, ref snow, ref snoden, tqvsp, ref melt, zc, pcandt, zr, trdt, gmcdt, rhor, zs, vlcdt, vicdt, tsdt, matdt, concdt);
                    if (nsp <= 0)
                    {
                        //              SNOW DID NOT STICK (MELT IS NOT INTERCEPTED BY CANOPY)
                        train = 0.0;
                    }
                }
                else
                {
                    //           PRECIP IS ASSUMED TO BE RAIN - DO NOT ALLOW RAIN TEMP < 0.0
                    if (train < 0.0) train = 0.0;
                }
            }
            //
            //     RAINFALL ABSORPTION BY SNOWPACK AND CALCULATION OF SNOWMELT
            if (nsp > 0)
            {
                Wbsnow(ref nsp, icespt, zsp, dzsp, rhosp, tspdt, dlw, dlwdt, ref rain, ref train, ref topsno, ref evap1, tqvsp, wlag, ref store, ref scout);
                rain = scout;
                train = 0.0;
            }
            //
            //     DO NOT INFILTRATE PONDED WATER OR SNOW EXCESS (SNOWEX) IF
            //     TEMPERATURE IS BELOW FREEZING
            if ((rain + pond + melt) > 0.0 && train >= 0.0)
            {
                //
                //        ADD MELT (FROM SNOW NOT STICKING) BACK INTO RAIN
                rain = rain + melt;
                //
                //        RAINFALL INTERCEPTION BY RESIDUE LAYERS
                if (nr > 0) Rainrs(ref nr, ref train, ref rain, zr, trdt, gmcdt, rhor, ref gmcmax, ref dirres, ref slope);
                //
                if (rain > 0.0)
                {
                    //           ADD ANY EXCESS FROM ELIMINATING SHALLOW SNOWPACK BACK
                    //           INTO RAIN
                    rain = rain + snowex;
                    pond = pond - snowex;
                    snowex = 0.0;
                }
                rain = rain + pond;
                pond = 0.0;
                //
                //        RAINFALL INFILTRATION INTO THE SOIL
                Rainsl(ref ns, ref train, ref rain, zs, tsdt, vlcdt, vicdt, icesdt, matdt, totflo, saltdt, concdt, ref pond, ref pondmx);
                //
                if (snowex > 0.0)
                {
                    //           DO NOT ALLOW EXCESS WATER FROM ELIMINATING SHALLOW SNOWPACK
                    //           TO RUNOFF - PUT THIS WATER BACK INTO PONDING AND SNOW EXCESS
                    //           (THIS MAY ALLOW PONDING DEPTH TO BECOME GREATER THAN PONDMX)
                    pond = pond + rain;
                    rain = 0.0;
                    if (pond < snowex) snowex = pond;
                }
            }
            //
            if (nsp > 0)
            {
                //
                //     CALCULATE WATER EQUIVALENT - IF SUFFICIENTLY SMALL, ASSUME ENTIRE
                //     SNOWPACK IS GONE
                we = 0.0;
                for (var i = 1; i <= nsp; ++i)
                {
                    we = we + dzsp[i] * rhosp[i] / Constn.Rhol + dlwdt[i];
                label20:;
                }
                if (we < 0.0005)
                {
                    //        WATER EQUIVALENT IS SUFFICIENTLY SMALL-ASSUME SNOWPACK IS GONE
                    scout = we;
                    nsp = 0;
                    //        ADD WATER CURRENTLY BEING LAGGED TO SNOW COVER OUTFLOW
                    Snomlt(nsp, icespt, dzsp, rhosp, tspdt, dlwdt, wlag, ref store, ref scout);
                    //
                    if (_lvlout[1] > 0)
                    {
                        _outputWriters[OutputFile.EntireProfile].WriteLine();
                        _outputWriters[OutputFile.EntireProfile].WriteLine($"SNOWCOVER IS ASSUMED GONE: WATER-EQUIVALENT OF ICE AND TOTAL WATER = {we} {scout} METERS");
                    }
                    //
                    //        SET SNOW WATER EXCESS (SNOWEX) TO THE TOTAL WATER EQUIV.
                    //        REMAINING IN THE SNOW COVER - THIS WILL BE ADDED TO
                    //        PRECIPITATION OR ALLOWED TO INFITRATE AT A LATER TIME STEP
                    snowex = snowex + scout;
                    //        INCLUDE SNOW EXCESS IN AMOUNT PONDED SO IT CAN BE INCLUDED IN
                    //        WATER BALANCE
                    pond = pond + scout;
                    //
                }
                else
                {
                    //        SNOWPACK STILL PRESENT -- DEFINE TEMPERATURE FOR BOTTOM BOUNDARY
                    if (nr > 0)
                    {
                        //           BOTTOM TEMPERATURE BOUNDARY IS TOP OF RESIDUE
                        tspdt[nsp + 1] = trdt[1];
                    }
                    else
                    {
                        //           BOTTOM TEMPERATURE BOUNDARY IS SOIL SURFACE
                        tspdt[nsp + 1] = tsdt[1];
                    }
                }
                //
            }
            //
            runoff = rain;
            melt = melt + scout;
        }

        // line 9500
        private void Wtbulb(ref double tw, ref double ta, ref double hum)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE WETBULB TEMPERATURE (TW IN CELCIUS)
            //     FROM THE AIR TEMPERATURE AND RELATIVE HUMIDITY
            //     ---- PROCEDURE FOR FINDING WETBULB TEMPERATURE WAS ADOPTED FROM
            //          THE ANDERSON SNOWMELT MODEL.
            //
            //***********************************************************************

            //     CONVERT ATMOSPHERIC PRESSURE FROM PASCALS TO MBARS
            var pa = 0.01 * _constn.Presur;
            //
            //     CALCULATE VAPOR PRESSURE AND SATURATED VAPOR PRESSURE IN MBARS
            var eas = 2.7489e8 * Math.Exp(-4278.63 / (ta + 242.792));
            var ea = hum * eas;
            //
            var delta = (4278.63 / ((ta + 242.792) * (ta + 242.792))) * eas;
            for (var i = 1; i <= 3; ++i)
            {
                tw = delta * ta + 6.6e-4 * pa * ta + 7.59e-7 * pa * ta * ta + ea - eas;
                tw = tw / (delta + 6.6e-4 * pa + 7.59e-7 * pa * ta);
                var tav = (ta + tw) * 0.5;
                var eav = 2.7489e8 * Math.Exp(-4278.63 / (tav + 242.792));
                delta = (4278.63 / ((tav + 242.792) * (tav + 242.792))) * eav;
            label10:;
            }
        }

        // line 9534
        private void Nwsnow(ref int nsp, ref int nc, ref int nr, ref int ns, ref int nplant, int[] icespt, double[] zsp, double[] dzsp, double[] rhosp, double[] tspdt, double[] dlw, double[] dlwdt, ref double twbulb, ref double snow, ref double snoden, double[] tqvsp, ref double melt, double[] zc, double[] pcandt, double[] zr, double[] trdt, double[] gmcdt, double[] rhor, double[] zs, double[] vlcdt, double[] vicdt, double[] tsdt, double[] matdt, double[][] concdt)
        {
            //
            //     THIS SUBROUTINES ADDS SNOW LAYERS FOR NEWLY FALLEN SNOW
            //
            //***********************************************************************
            //
            var cres = new double[11];
            var cs = new double[51];

            var densty = 0.0;
            var depth = 0.0;
            var total = 0.0;
            var change = 0.0;
            var nwlayr = 0;
            var dns = 0.0;
            var wei = 0.0;
            var wens = 0.0;
            var wel = 0.0;
            var freeze = 0.0;
            var extra = 0.0;
            var down = 0.0;
            var tdepth = 0.0;

            //     PRECIPITATION IS SNOW
            if (snoden > 0.0)
            {
                //        DENSITY OF NEW SNOW IS KNOWN -- CONVERT TO KG/M3
                densty = snoden * Constn.Rhol;
            }
            else
            {
                //        COMPUTE DENSITY OF THE NEW SNOW BASED ON WETBULB TEMPERATURE
                if (twbulb <= -15.0)
                {
                    densty = 50.0;
                }
                else
                {
                    densty = 50.0 + 1.7 * (Math.Pow((twbulb + 15.0), 1.5));
                }
            }
            depth = snow * Constn.Rhol / densty;
            //
            if (nc > 0)
            {
                //        ADJUST CANOPY INTERCEPTION FOR DEPTH OF CANOPY COVERED BY SNOW
                //        (NO CONSIDERATION ABOUT WHERE LAI IS WITHIN THE CANOPY)
                total = 0.0;
                for (var j = 1; j <= nplant; ++j)
                {
                    change = pcandt[j] * depth / zc[nc + 1];
                    if (change > pcandt[j]) change = pcandt[j];
                    pcandt[j] = pcandt[j] - change;
                    total = total + change;
                label3:;
                }
                snow = snow + total;
                densty = snow * Constn.Rhol / depth;
            }
            //
            melt = 0.0;
            if (nsp == 0)
            {
                //        SNOWCOVER DOES NOT EXIST -- MELT SOME SNOW TO GET UNDERLYING
                //        MATERIAL TO 0 DEGREES
                if (nr > 0)
                {
                    //           SNOW IS FALLING ON RESIDUE
                    Resht(ref nr, cres, gmcdt, rhor);
                    if (trdt[1] <= 0.0)
                    {
                        //              NO MELTING OF SNOW IS NEEDED TO GET RESIDUE TO 0 DEGREES
                        melt = 0.0;
                    }
                    else
                    {
                        melt = cres[1] * zr[2] / 2.0 * trdt[1] / (Constn.Rhol * Constn.Lf - Constn.Rhol * Constn.Ci * twbulb);
                        snow = snow - melt;
                        depth = depth - melt * Constn.Rhol / densty;
                        if (snow <= 0.0)
                        {
                            //                 ALL SNOW THAT FELL IS MELTED - ADJUST RESIDUE FOR
                            //                 ENERGY REQUIRED TO MELT SNOW (MELTWATER ENERGY WILL
                            //                 BE CONSIDERED LATER IN SUBROUTINE RAINRS)
                            melt = melt + snow;
                            trdt[1] = trdt[1] - (Constn.Rhol * Constn.Lf - Constn.Rhol * Constn.Ci * twbulb) * melt / (cres[1] * zr[2] / 2.0);
                            return;
                        }
                        trdt[1] = 0.0;
                    }
                    //
                }
                else
                {
                    //           SNOW IS FALLING ON BARE SOIL
                    if (tsdt[1] <= 0.0)
                    {
                        //              NO MELTING OF SNOW IS NEEDED TO GET SOIL TO 0 DEGREES
                        melt = 0.0;
                    }
                    else
                    {
                        Soilht(ref ns, cs, vlcdt, vicdt, tsdt, matdt, concdt);
                        melt = cs[1] * zs[2] / 2.0 * tsdt[1] / (Constn.Rhol * Constn.Lf - Constn.Rhol * Constn.Ci * twbulb);
                        snow = snow - melt;
                        depth = depth - melt * Constn.Rhol / densty;
                        if (snow <= 0.0)
                        {
                            //                 ALL SNOW THAT FELL IS MELTED - ADJUST SOIL TEMP FOR
                            //                 ENERGY REQUIRED TO MELT SNOW (MELTWATER ENERGY WILL
                            //                 BE CONSIDERED LATER IN SUBROUTINE RAINSL)
                            melt = melt + snow;
                            tsdt[1] = tsdt[1] - (Constn.Rhol * Constn.Lf - Constn.Rhol * Constn.Ci * twbulb) * melt / (cs[1] * zs[2] / 2.0);
                            return;
                        }
                        tsdt[1] = 0.0;
                    }
                }
                //
            }
            else
            {
                //        NEW SNOW IS FALLING ON OLD SNOW - CHECK IF NEW SNOW CAUSES THE
                //        THE SURFACE LAYER TO EXCEED THE SPECIFIED DEPTH LIMIT.
                if ((dzsp[1] + depth) <= (1.55 * Spwatr.Thick))
                {
                    //           INCORPORATE ALL OF NEW SNOW INTO OLD SURFACE LAYER.
                    dns = depth;
                    depth = 0.0;
                    nwlayr = 0;
                }
                else
                {
                    //           DETERMINE HOW MUCH SHOULD BE COMBINED WITH OLD SURFACE LAYER
                    if (dzsp[1] > Spwatr.Thick)
                    {
                        //              LAYER IS ALREADY GREATER THAN DESIRED THICKNESS - DO NOT
                        //              ADD NEW SNOW TO OLD SURFACE LAYER
                        dns = 0.0;
                        //              GO SET UP NEW LAYERS FOR THE NEW SNOW
                        goto label5;
                    }
                    dns = Spwatr.Thick - dzsp[1];
                    depth = depth - dns;
                }
                //
                //        INCORPORATE NEW SNOW INTO OLD SURFACE LAYER - DEFINE PROPERTIES
                wei = rhosp[1] * dzsp[1] / Constn.Rhol;
                wens = densty * dns / Constn.Rhol;
                wel = wei + wens;
                dzsp[1] = dzsp[1] + dns;
                rhosp[1] = Constn.Rhol * wel / dzsp[1];
                tspdt[1] = (wens * twbulb + wei * tspdt[1]) / wel;
                if (icespt[1] == 1)
                {
                    //           LIQUID-WATER AND SUBFREEZING TEMP MAY EXIST IN NEW LAYER
                    freeze = -(tspdt[1] * Constn.Ci * wel) / (Constn.Lf * Constn.Rhol);
                    if (dlwdt[1] <= freeze)
                    {
                        tspdt[1] = tspdt[1] + (dlwdt[1] * Constn.Lf * Constn.Rhol) / (Constn.Ci * wel);
                        wel = wel + dlwdt[1];
                        rhosp[1] = Constn.Rhol * wel / dzsp[1];
                        dlwdt[1] = 0.0;
                        icespt[1] = 0;
                    }
                    else
                    {
                        dlwdt[1] = dlwdt[1] - freeze;
                        wel = wel + freeze;
                        rhosp[1] = Constn.Rhol * wel / dzsp[1];
                        tspdt[1] = 0.0;
                    }
                }
            }
        //
        //
        //     COMPUTE THE NUMBER OF LAYERS WITH ENTIRELY NEW SNOW
        label5:;
            nwlayr = (int)(depth / Spwatr.Thick);   // Fortran code is simply a division: DEPTH / THICK, which should be a truncation
            extra = depth - nwlayr * Spwatr.Thick;
            if (extra > 0.55 * Spwatr.Thick) nwlayr = nwlayr + 1;
            if (extra > 0.0 && nwlayr == 0) nwlayr = 1;
            //
            if (nsp > 0)
            {
                down = depth + dns;
                if (nwlayr == 0)
                {
                    //           ADJUST DEPTHS OF THE LAYERS AND RETURN
                    zsp[1] = 0.0;
                    for (var i = 2; i <= nsp + 1; ++i)
                    {
                        zsp[i] = zsp[i] + down;
                    label10:;
                    }
                    return;
                }
                else
                {
                    //           MOVE LAYERS DOWN TO MAKE ROOM FOR NEW SNOW LAYERS
                    zsp[nsp + nwlayr + 1] = zsp[nsp + 1] + down;
                    tspdt[nsp + nwlayr + 1] = tspdt[nsp + 1];
                    for (var i = nsp; i >= 1; --i)
                    {
                        rhosp[nwlayr + i] = rhosp[i];
                        tspdt[nwlayr + i] = tspdt[i];
                        dlw[nwlayr + i] = dlw[i];
                        dlwdt[nwlayr + i] = dlwdt[i];
                        icespt[nwlayr + i] = icespt[i];
                        zsp[nwlayr + i] = zsp[i] + down;
                        dzsp[nwlayr + i] = dzsp[i];
                        tqvsp[nwlayr + i] = tqvsp[i];
                    label15:;
                    }
                    //           ADJUST DEPTH OF OLD SURFACE LAYER TO BE IN MIDDLE OF LAYER
                    zsp[nwlayr + 1] = zsp[nwlayr + 1] + dzsp[nwlayr + 1] / 2.0;
                }
            }
            //
            //     LEFT-OVER SNOW GOES IN TOP LAYER
            extra = depth - (nwlayr - 1) * Spwatr.Thick;
            zsp[1] = 0.0;
            dzsp[1] = extra;
            rhosp[1] = densty;
            tspdt[1] = twbulb;
            dlw[1] = 0.0;
            dlwdt[1] = 0.0;
            icespt[1] = 0;
            tqvsp[1] = 0.0;
            tdepth = dzsp[1];
            //
            //     FULL SIZE NEW SNOW LAYERS.
            for (var j = 2; j <= nwlayr; ++j)
            {
                dzsp[j] = Spwatr.Thick;
                zsp[j] = tdepth + dzsp[j] / 2.0;
                tdepth = tdepth + dzsp[j];
                rhosp[j] = densty;
                tspdt[j] = twbulb;
                dlw[j] = 0.0;
                dlwdt[j] = 0.0;
                icespt[j] = 0;
                tqvsp[j] = 0.0;
            label20:;
            }
            //
            if (nsp == 0)
            {
                //        DEFINE DEPTH OF BOTTOM BOUNDARY OF SNOWPACK
                zsp[nwlayr + 1] = tdepth;
                //        NEW SNOW COVER - DEFINE TEMP OF BOTTOM BOUNDARY OF SNOWPACK
                //        THIS NODE IS SHARED BY THE MATERIAL BELOW
                if (nr > 0)
                {
                    //           UNDERLYING MATERIAL IS RESIDUE
                    tspdt[nwlayr + 1] = trdt[1];
                }
                else
                {
                    //           UNDERLYING MATERIAL IS SOIL
                    tspdt[nwlayr + 1] = tsdt[1];
                }
            }
            //
            nsp = nsp + nwlayr;
        }

        // line 9760
        private void Wbsnow(ref int nsp, int[] icespt, double[] zsp, double[] dzsp, double[] rhosp, double[] tspdt, double[] dlw, double[] dlwdt, ref double rain, ref double train, ref double topsno, ref double evap1, double[] tqvsp, double[] wlag, ref double store, ref double scout)
        {
            //
            //     THIS SUBROUTINE PERFORMS A WATER BALANCE OF THE SNOWPACK BY
            //     ADSUSTING THE DENSITY FOR VAPOR FLUX AND ANY MELT WHICH OCCURRED
            //     OVER THE TIME STEP.  CHECKS ARE MADE TO SEE IF ANY LAYERS HAVE
            //     DISAPPEARED DUE TO MELT, OR IF ANY LAYERS ARE OUTSIDE THE
            //     ACCEPTABLE THICKNESS RANGE.
            //
            //***********************************************************************
            //

            var change = 0.0;
            var nl = 0;
            var above = 0.0;
            var excess = 0.0;
            var wel = 0.0;
            var freeze = 0.0;
            var we = 0.0;
            var tdepth = 0.0;
            var next = 0;
            var l = 0;
            var i = 0;
            var zz = 0.0;
            var dz1 = 0.0;
            var ll = 0;
            var wei = 0.0;
            var wenl = 0.0;

            scout = 0.0;
            //
            //     ADJUST THE DENSITY FOR VAPOR FLUX
            //     (THICKNESS OF LAYER IS ADJUSTED FOR SURFACE LAYER)
            //
            change = (topsno - tqvsp[1]) / rhosp[1];
            dzsp[1] = dzsp[1] + change;
            if (dzsp[1] <= 0.0 && nsp == 1)
            {
                //        SNOWPACK IS LOST TO SUBLIMATION -- ADJUST TOPSNO AND EVAP1 FOR
                //        THE WATER BALANCE TO CLOSE AND RETURN TO CALLING SUBROUTINE
                topsno = topsno - dzsp[1] * rhosp[1];
                evap1 = evap1 - dzsp[1] * rhosp[1] / Constn.Rhol;
                nsp = 0;
                return;
            }
            for (i = 2; i <= nsp; ++i)
            {
                change = (tqvsp[i - 1] - tqvsp[i]) / dzsp[i];
                rhosp[i] = rhosp[i] + change;
            label10:;
            }
            //
            //     ADJUST LAYERS FOR MELT AND RAINFALL
            //        ABOVE = THE RAIN OR THE MELTWATER FROM ABOVE LAYERS
            //        EXCESS= THIS IS THE MELTWATER THAT WOULD HAVE BEEN PRODUCED
            //                BY ABOVE LAYERS HAD THERE BEEN ENOUGH ICE IN THE LAYER
            //                FOR ALL THE ENERGY ABSORBED.  (FOR THE FIRST LAYER,
            //                THIS TERM INCLUDES THE ENERGY TRANSFERRED BY RAIN.)
            nl = nsp;
            above = rain;
            excess = rain * (Constn.Cl * train + Constn.Lf) / Constn.Lf;
            //
            for (i = 1; i <= nsp; ++i)
            {
                if (icespt[i] == 0)
                {
                    //
                    //        TEMPERATURE IS UNKNOWN--SOME EXCESS LIQUID-WATER FROM
                    //        THE ABOVE LAYER MUST BE FROZEN.
                    //
                    if (excess != 0.0)
                    {
                        //           COMPUTE AMOUNT TO BE FROZE IN ORDER TO RAISE THE TEMPERATURE
                        //           TO ZERO DEGREES CELSIUS. (USE SPECIFIC HEAT OF ICE, CI)
                        wel = rhosp[i] * dzsp[i] / Constn.Rhol;
                        freeze = -tspdt[i] * wel * Constn.Ci / Constn.Lf;
                        if (excess > freeze)
                        {
                            //              EXCESS EXCEEDS REFREEZE.
                            tspdt[i] = 0.0;
                            wel = wel + freeze;
                            rhosp[i] = Constn.Rhol * wel / dzsp[i];
                            excess = excess - freeze;
                            above = above - freeze;
                            icespt[i] = 1;
                        }
                        else
                        {
                            //              EXCESS IS ALL FROZEN IN THIS LAYER.
                            tspdt[i] = tspdt[i] + (excess * Constn.Lf * Constn.Rhol) / (Constn.Ci * rhosp[i] * dzsp[i]);
                            wel = wel + excess;
                            rhosp[i] = Constn.Rhol * wel / dzsp[i];
                            above = above - excess;
                            excess = 0.0;
                        }
                    }
                }
                //
                dlwdt[i] = dlwdt[i] + excess;
                change = dlwdt[i] - dlw[i] - above;
                //
                if (Math.Abs(change) < 0.0000001)
                {
                    //        THIS IS BEYOND PRECISION OF COMPUTER -- ASSUME ZERO
                    above = 0.0;
                    excess = 0.0;
                }
                else
                {
                    if (change <= 0.0)
                    {
                        //           SOME LIQUID-WATER IS FROZEN. ADD TO ICE CONTENT OF LAYER.
                        wel = rhosp[i] * dzsp[i] / Constn.Rhol;
                        wel = wel - change;
                        rhosp[i] = Constn.Rhol * wel / dzsp[i];
                        above = 0.0;
                        excess = 0.0;
                    }
                    else
                    {
                        //           MELT HAS OCCURRED,SUBTRACT FROM ICE CONTENT
                        wel = rhosp[i] * dzsp[i] / Constn.Rhol;
                        //           IF MELT EXCEEDS ICE CONTENT OF LAYER--LAYER IS GONE.
                        if (change >= wel)
                        {
                            //              LAYER IS GONE
                            //              LIQUID-WATER IS ADDED TO THE LAYER BELOW (EXCESS).  THE
                            //              ICE CONTENT PLUS DLW OF THE LAYER CANNOT BE TAKEN FROM THE
                            //              NEXT LAYER, BUT IS STILL ADDED TO THE LIQUID-WATER
                            //              CONTENT OF THE NEXT LAYER (ABOVE).
                            nl = nl - 1;
                            above = above + wel + dlw[i];
                            excess = dlwdt[i];
                            dzsp[i] = 0.0;
                        }
                        else
                        {
                            //              LAYER REMAINS
                            wel = wel - change;
                            dzsp[i] = Constn.Rhol * wel / rhosp[i];
                            above = 0.0;
                            excess = 0.0;
                        }
                    }
                }
            //
            label20:;
            }
            //
            //***********************************************************************
            //     CHECK TO SEE IF ENTIRE SNOW COVER IS GONE.
            if (nl <= 0)
            {
                nsp = 0;
                we = 0.0;
                tdepth = 0.0;
                scout = above;
                Snomlt(nsp, icespt, dzsp, rhosp, tspdt, dlwdt, wlag, ref store, ref scout);
                return;
            }
            //
            //     ELIMINATE LAYERS WHICH ARE GONE.
            if (nl != nsp)
            {
                for (i = 1; i <= nsp; ++i)
                {
                label23:;
                    if (dzsp[i] > 0.0) goto label30;
                    //           LAYER GONE. MOVE OTHER LAYERS UP.
                    next = i + 1;
                    for (var j = next; j <= nsp; ++j)
                    {
                        l = j - 1;
                        dzsp[l] = dzsp[j];
                        rhosp[l] = rhosp[j];
                        tspdt[l] = tspdt[j];
                        dlwdt[l] = dlwdt[j];
                        icespt[l] = icespt[j];
                    label25:;
                    }
                    nsp = nsp - 1;
                    if (nsp == nl) goto label35;
                    goto label23;
                label30:;
                }
            label35:;
                dlwdt[nsp] = dlwdt[nsp] + above;
            }
            //
            //***********************************************************************
            //     CHECK THICKNESS OF EACH LAYER. IF NOT WITHIN SPECIFIED LIMITS,
            //     DIVIDE (IF TOO LARGE) OR ADD TO AN ADJACENT LAYER (IF TOO SMALL).
            tdepth = 0.0;
            i = 1;
            if (nsp == 1) goto label80;
            //
            label40:;
            zz = tdepth + dzsp[i] * 0.5;
            //     COMPUTED DESIRED THICKNESS FOR THE LAYER.
            dz1 = Spwatr.Thick;
            if (zz > 0.30) dz1 = Spwatr.Cthick * (zz - 0.30) + Spwatr.Thick;
            //     CHECK ACTUAL THICKNESS AGAINST DESIRED THICKNESS.
            if (dzsp[i] > 1.55 * dz1) goto label50;
            if (dzsp[i] < 0.55 * dz1 && nsp > 1) goto label60;
            //     THICKNESS IS WITHIN SPECIFIED LIMITS.
            tdepth = tdepth + dzsp[i];
            goto label70;
        //***********************************************************************
        //     THICKNESS IS GREATER THAN SPECIFIED LIMTS.
        //          SUB-DIVIDE LAYER,THUS CREATING A NEW LAYER.
        //          PROPERTIES ARE THE SAME FOR BOTH LAYERS.
        //          MOVE OTHER LAYERS DOWN.
        label50:;
            next = i + 1;
            if (next <= nsp)
            {
                for (var j = next; j <= nsp; ++j)
                {
                    l = nsp - j + next;
                    ll = l + 1;
                    dzsp[ll] = dzsp[l];
                    rhosp[ll] = rhosp[l];
                    tspdt[ll] = tspdt[l];
                    dlwdt[ll] = dlwdt[l];
                    icespt[ll] = icespt[l];
                label55:;
                }
            }
            tdepth = tdepth + dzsp[i];
            dzsp[next] = dzsp[i] * 0.5;
            rhosp[next] = rhosp[i];
            tspdt[next] = tspdt[i];
            dlwdt[next] = dlwdt[i] * 0.5;
            icespt[next] = icespt[i];
            dzsp[i] = dzsp[i] * 0.5;
            dlwdt[i] = dlwdt[i] * 0.5;
            i = i + 1;
            nsp = nsp + 1;
            goto label70;
        //***********************************************************************
        //     THICKNESS IS SMALLER THAN SPECIFIED LIMITS.
        //          ADD TO THE SMALLEST ADJACENT LAYER, THUS
        //          LOSING A LAYER. PROPERIES OF THE NEW LAYER
        //          ARE THE WEIGHTED AVERAGE OF THE TWO FORMER
        //          LAYERS. MOVE OTHER LAYERS UP.
        label60:;
            if (i == 1 || i == nsp)
            {
                if (i == 1)
                {
                    nl = 2;
                }
                else
                {
                    nl = nsp - 1;
                }
            }
            else
            {
                nl = i - 1;
                if (dzsp[i + 1] < dzsp[i - 1]) nl = i + 1;
            }
            //
            //     NL IS THE SMALLEST ADJACENT LAYER - ADD LAYER I TO LAYER NL
            wei = rhosp[i] * dzsp[i] / Constn.Rhol;
            wenl = rhosp[nl] * dzsp[nl] / Constn.Rhol;
            wel = wei + wenl;
            dzsp[nl] = dzsp[nl] + dzsp[i];
            rhosp[nl] = Constn.Rhol * wel / dzsp[nl];
            dlwdt[nl] = dlwdt[nl] + dlwdt[i];
            tspdt[nl] = (wenl * tspdt[nl] + wei * tspdt[i]) / wel;
            //
            if (icespt[i] != icespt[nl])
            {
                //        UNKNOWNS ARE DIFFERENT. COMPUTE THE UNKNOWN FOR THE NEW LAYER.
                freeze = -(tspdt[nl] * Constn.Ci * rhosp[nl] * dzsp[nl]) / (Constn.Lf * Constn.Rhol);
                if (dlwdt[nl] <= freeze)
                {
                    //           TEMPERATURE IS UNKNOWN
                    tspdt[nl] = tspdt[nl] + (dlwdt[nl] * Constn.Lf * Constn.Rhol) / (Constn.Ci * rhosp[nl] * dzsp[nl]);
                    wel = wel + dlwdt[nl];
                    rhosp[nl] = Constn.Rhol * wel / dzsp[nl];
                    icespt[nl] = 0;
                }
                else
                {
                    //           LIQUID-WATER IS UNKNOWN.
                    dlwdt[nl] = dlwdt[nl] - freeze;
                    wel = wel + freeze;
                    rhosp[nl] = Constn.Rhol * wel / dzsp[nl];
                    icespt[nl] = 1;
                }
            }
            //
            if (icespt[nl] == 1) tspdt[nl] = 0.0;
            if (icespt[nl] == 0) dlwdt[nl] = 0.0;
            //     MOVE OTHER LAYERS UP.
            if (nl < i) tdepth = tdepth + dzsp[i];
            next = i + 1;
            //
            if (next <= nsp)
            {
                for (var j = next; j <= nsp; ++j)
                {
                    l = j - 1;
                    dzsp[l] = dzsp[j];
                    rhosp[l] = rhosp[j];
                    tspdt[l] = tspdt[j];
                    dlwdt[l] = dlwdt[j];
                    icespt[l] = icespt[j];
                label65:;
                }
            }
            //
            i = i - 1;
            nsp = nsp - 1;
        //
        //***********************************************************************
        //     CHECK FOR LAST LAYER.
        label70:;
            if (i < nsp)
            {
                //        START NEXT LAYER
                i = i + 1;
                goto label40;
            }
        //
        //***********************************************************************
        //     ADJUST THE DENSITY FOR METAMORPHISM AND COMPUTE ANY SNOMELT
        label80:;
            Meta(nsp, tspdt, rhosp, dzsp, dlwdt);
            Snomlt(nsp, icespt, dzsp, rhosp, tspdt, dlwdt, wlag, ref store, ref scout);
            //
            //***********************************************************************
            //     CALCULATE THE DEPTH FOR EACH NODE
            zsp[1] = 0.0;
            tdepth = dzsp[1];
            we = dzsp[1] * rhosp[1] / Constn.Rhol + dlwdt[1];
            for (var ii = 2; ii <= nsp; ++ii)
            {
                zsp[ii] = tdepth + dzsp[ii] / 2.0;
                tdepth = tdepth + dzsp[ii];
                we = we + dzsp[ii] * rhosp[ii] / Constn.Rhol + dlwdt[ii];
            label85:;
            }
            zsp[nsp + 1] = tdepth;
        }

        // line 10055
        /// <summary>
        /// sets rhosp, dzsp.
        /// </summary>
        /// <param name="nsp"></param>
        /// <param name="tspdt"></param>
        /// <param name="rhosp"></param>
        /// <param name="dzsp"></param>
        /// <param name="dlwdt"></param>
        private void Meta(int nsp, double[] tspdt, double[] rhosp, double[] dzsp, double[] dlwdt)
        {
            //
            //     COMPUTES THE CHANGE IN DENSITY OF THE SNOW COVER CAUSED BY
            //     DESTRUCTIVE (EQUI-TEMPERATURE) METAMORPHISM, COMPACTION, AND THE
            //     PRESENCE OF LIQUID-WATER.
            //***********************************************************************

            if (Metasp.Cmet1 <= 0.0 || Metasp.Cmet3 <= 0.0)
            {
                if (Metasp.Cmet5 <= 0.0) return;
            }
            //
            //     WEIGHT IS THE WATER-EQUIVALENT (CM) ABOVE THE LAYER.
            var weight = 0.0;
            //
            for (var i = 1; i <= nsp; ++i)
            {
                //        IF DENSITY IS THAT OF ICE, DO NOT INCREASE IT ANY MORE
                if (rhosp[i] >= Constn.Rhoi) goto label10;
                var wel = rhosp[i] * dzsp[i] / Constn.Rhol;
                //
                //****    DESTRUCTIVE METAMORPHISM TERM.
                //
                var term = Math.Exp(Metasp.Cmet4 * tspdt[i]);
                double t1;
                if (rhosp[i] <= Metasp.Snomax)
                {
                    t1 = term * Metasp.Cmet3;
                }
                else
                {
                    t1 = term * Metasp.Cmet3 * Math.Exp(-46.0 * (rhosp[i] - Metasp.Snomax) / Constn.Rhol);
                }
                //
                //****    COMPACTION TERM.
                //
                term = Math.Exp(0.08 * tspdt[i]);
                var t2 = weight * Metasp.Cmet1 * Math.Exp(-Metasp.Cmet2 * rhosp[i] / Constn.Rhol) * term;
                //
                //***     LIQUID-WATER TERM.
                //
                if (dlwdt[i] > 0.0) t1 = Metasp.Cmet5 * t1;
                //
                //****    DENSIFICATION OF THE LAYER.  (WATER-EQUIVALENT STAYS THE SAME.)
                //
                rhosp[i] = rhosp[i] * (1.0 + _timewt.Dt * (t1 + t2) / 3600.0);
                dzsp[i] = Constn.Rhol * wel / rhosp[i];
                weight = weight + (wel + dlwdt[i]) * 100.0;
            label10:;
            }
        }

        // line 10110
        private void Snomlt(int nsp, int[] icespt, double[] dzsp, double[] rhosp, double[] tspdt, double[] dlwdt, double[] wlag, ref double store, ref double scout)
        {
            //
            //     THIS SUBROUTINE DETERMINES THE SNOW COVER OUTFLOW DURING EACH
            //     PERIOD BASED ON THE CHANGE IN LIQUID-WATER DURING THE PERIOD AND
            //     THE PHYSICAL CHARTERISTICS OF THE SNOW COVER.
            //***********************************************************************
            //

            var bottom = 0.0;
            var excess = 0.0;
            var wel = 0.0;
            var plw = 0.0;
            var wmax = 0.0;
            var we = 0.0;
            var tdepth = 0.0;
            var freeze = 0.0;
            var w = 0.0;
            var outhr = 0.0;
            var excshr = 0.0;
            var idt = 0;
            var dt8 = 0.0;
            var hours = 0.0;
            var dense = 0.0;
            var ni = 0;
            var fn = 0.0;
            var flmax = 0.0;
            var fj = 0.0;
            var flag = 0.0;
            var k = 0.0;    // k appears to be a real, not an int, below
            var por = 0.0;
            var winc = 0.0;
            var r = 0.0;

            if (_snomltSave.Ifirst == 0)
            {
                //        INITIALIZE VARIABLES.
                //        1.0 STORE IS THE AMOUNT OF LIQUID-WATER(THAT HAS ALREADY BEEN
                //           LAGGED) THAT IS IN STORAGE IN THE SNOW-COVER.
                //        2.0 WLAG() IS THE AMOUNT OF LIQUID-WATER IN THE PROCESS OF
                //           BEING LAGGED.  NLAG IS THE NUMBER OF ARRAY ELEMENTS USED.
                var jdt = Spwatr.Clag1 + 0.01;
                _snomltSave.Nlag = (int)(jdt + 2);
                if (_snomltSave.Nlag > 11) _snomltSave.Nlag = 11;
                _snomltSave.Ifirst = 1;
            }
            //
            if (nsp <= 0)
            {
                //        SNOW COVER HAS JUST DISAPPEARED
                for (var j = 1; j <= _snomltSave.Nlag; ++j)
                {
                    scout = scout + wlag[j];
                    wlag[j] = 0.0;
                label10:;
                }
                scout = scout + store;
                store = 0.0;
                return;
            }
            //
            //     DETERMINE THE EXCESS LIQUID-WATER (IN EXCESS OF LIQUID-WATER
            //     HOLDING CAPACITY) GENERATED DURING THIS TIME PERIOD.
            excess = 0.0;
            //
            //     EXCESS WATER IN BOTTOM LAYER IS NOT TO BE ROUTED.
            if (nsp == 1) goto label12;
            bottom = 0.0;
            if (icespt[nsp] == 1)
            {
                wel = rhosp[nsp] * dzsp[nsp] / Constn.Rhol;
                plw = (Spwatr.Plwmax - Spwatr.Plwhc) * (Spwatr.Plwden - rhosp[nsp]) / Spwatr.Plwden + Spwatr.Plwhc;
                if (rhosp[nsp] >= Spwatr.Plwden) plw = Spwatr.Plwhc;
                wmax = plw * wel;
                if (dlwdt[nsp] > wmax) bottom = dlwdt[nsp] - wmax;
            }
        //
        label12:;
            we = 0.0;
            tdepth = 0.0;
            for (var i = 1; i <= nsp; ++i)
            {
                wel = rhosp[i] * dzsp[i] / Constn.Rhol;
                if (icespt[i] == 0)
                {
                    //           LAYER IS BELOW ZERO DEGREES CELSIUS.
                    if (excess == 0.0) goto label15;
                    //           FREEZE SOME OF THE LIQUID-WATER.
                    freeze = -tspdt[i] * Constn.Ci * wel / Constn.Lf;
                    if (excess <= freeze)
                    {
                        //              EXCESS IS ALL FROZEN IN THIS LAYER.
                        tspdt[i] = tspdt[i] + (excess * Constn.Lf * Constn.Rhol) / (Constn.Ci * wel * Constn.Rhol);
                        wel = wel + excess;
                        excess = 0.0;
                        rhosp[i] = Constn.Rhol * wel / dzsp[i];
                        goto label15;
                    }
                    else
                    {
                        //              EXCESS EXCEEDS REFREEZE.
                        tspdt[i] = 0.0;
                        wel = wel + freeze;
                        rhosp[i] = Constn.Rhol * wel / dzsp[i];
                        excess = excess - freeze;
                        icespt[i] = 1;
                    }
                }
                plw = (Spwatr.Plwmax - Spwatr.Plwhc) * (Spwatr.Plwden - rhosp[i]) / Spwatr.Plwden + Spwatr.Plwhc;
                if (rhosp[i] >= Spwatr.Plwden) plw = Spwatr.Plwhc;
                wmax = plw * wel;
                w = dlwdt[i] + excess;
                if (w <= wmax)
                {
                    //           LIQUID-WATER HOLDING CAPACITY IS NOT SATISFIED.
                    dlwdt[i] = w;
                    excess = 0.0;
                }
                else
                {
                    //           LIQUID-WATER HOLDING CAPACITY IS EXCEEDED.
                    dlwdt[i] = wmax;
                    excess = w - wmax;
                }
            label15:;
                we = we + wel;
                tdepth = tdepth + dzsp[i];
            label20:;
            }
            //
            if (nsp == 1)
            {
                //        WATER NOT LAGGED IF ONLY ONE NODE IN THE SNOWPACK
                scout = excess;
                for (var j = 1; j <= _snomltSave.Nlag; ++j)
                {
                    scout = scout + wlag[j];
                    wlag[j] = 0.0;
                label50:;
                }
                scout = scout + store;
                store = 0.0;
                return;
            }
            //
            excess = excess - bottom;
            if (icespt[nsp] == 0)
            {
                //        DO NOT ALLOW ANY WATER LAGGED OR STORED TO LEAVE SNOWPACK
                //        (EXCESS MUST BE EQUAL 0.0)
                outhr = 0.0;
                scout = 0.0;
                return;
            }
            //
            //     ROUTE EXCESS WATER THROUGH THE SNOW COVER.
            //     EMPIRICAL LAG AND ATTENUATION EQUATIONS - ONE HOUR TIME STEP USED.
            excshr = excess * 3600.0 / _timewt.Dt;
            idt = (int)(_timewt.Dt / 3600.0);   // Fortran code uses an automatic type conversion to int, which truncates
            dt8 = _timewt.Dt;
            if (dt8 % 3600.0 > 0.00001) idt = idt + 1;
            if (idt == 0) idt = 1;
            hours = 1.0;
            //
            dense = we / tdepth;
            scout = 0.0;
            for (var ihr = 1; ihr <= idt; ++ihr)
            {
                //        IF THIS IS THE LAST TIME INCREMENT, ACCOUNT FOR ANY REMAINING
                //        FRACTION OF AN HOUR IN THE TIME STEP
                if (ihr == idt) hours = _timewt.Dt / 3600 - (idt - 1);
                excess = excshr * hours;
                //
                outhr = 0.0;
                //        LAG-FUNCTION OF DEPTH,DENSITY,AND EXCESS WATER.
                if (excshr >= 0.00001)
                {
                    ni = (int)((Math.Pow((excshr * 10000.0), 0.3)) + 0.5);
                    if (ni < 1) ni = 1;
                    fn = ni;
                    flmax = Spwatr.Clag1 * (1.0 - Math.Exp(-0.25 * tdepth / dense));
                    for (var j = 1; j <= ni; ++j)
                    {
                        fj = j;
                        flag = flmax / (Spwatr.Clag2 * excshr * 100.0 * (fj - 0.5) / fn + 1.0);
                        k = flag + 1.0;
                        var kk = (int)k;
                        por = kk - flag;
                        winc = 1.0 / fn;
                        wlag[kk] = wlag[kk] + excess * winc * por;
                        wlag[kk + 1] = wlag[kk + 1] + excess * winc * (1.0 - por);
                    label25:;
                    }
                }
                else
                {
                    wlag[1] = wlag[1] + excess;
                }
                //
                //        ATTENUATION-FUNCTION OF DENSITY AND PREVIOUS OUTFLOW.
                if ((store + wlag[1]) != 0.0)
                {
                    r = 1.0 / (Spwatr.Clag3 * Math.Exp(-Spwatr.Clag4 * wlag[1] * dense / tdepth) + 1.0);
                    outhr = (store + wlag[1]) * r;
                    store = store + (wlag[1] - outhr) * hours;
                    scout = scout + outhr * hours;
                    if (store <= 0.00001)
                    {
                        outhr = outhr + store;
                        scout = scout + store;
                        store = 0.0;
                    }
                }
                ni = _snomltSave.Nlag - 1;
                for (var j = 1; j <= ni; ++j)
                {
                    wlag[j] = wlag[j] * (1.0 - hours) + wlag[j + 1] * hours;
                    if (wlag[j + 1] <= 0.000001)
                    {
                        //              TIME STEPS OF LESS THAN 1 HOUR RESULT IN CONTINUALLY
                        //              TAKING FRACTIONS OF THE LAGGED DEPTH; DEPTH IS TOO
                        //              SMALL -- PASS THE ENTIRE DEPTH ONTO THE NEXT LAG
                        wlag[j] = wlag[j] + wlag[j + 1] + wlag[j + 1] * (1.0 - hours);
                        wlag[j + 1] = 0.0;
                    }
                label30:;
                }
                wlag[_snomltSave.Nlag] = 0.0;
            label35:;
            }
            //
            scout = scout + bottom;
        }

        // line 10301
        private void Rainc(ref int nplant, ref int nc, double[] pintrcp, double[] xangle, double[] clumpng, int[] itype, ref double rain, double[] pcandt, double[] wcandt, ref double wcmax, ref double slope)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE RAINFALL DEPTH INTERCEPTED BY THE
            //     CANOPY LAYERS AND ADJUST THE WATER CONTENT OF THE DEAD PLANT
            //
            //***********************************************************************
            //
            var tdircc = new double[11];
            var tdiffc = new double[11];
            var dirkl = JaggedArray<double>(10, 11);
            var difkl = JaggedArray<double>(10, 11);
            var fbdu = new double[9];
            var fddu = new double[9];
            var maxint = 0.0;

            //     USE THE TRANSMITTANCE TO DIRECT RADIATION TO CALCULATE THE
            //     FRACTION OF RAIN INTERCEPTED BY THE CANOPY
            var angle = 1.5708 - slope;
            var double157 = 1.57;
            Transc(ref nplant, ref nc, xangle, clumpng, tdircc, tdiffc, difkl, dirkl, fbdu, fddu, ref angle, ref double157);
            //
            //**** CALCULATE RAINFALL INTERCEPTED BY THE ENTIRE CANOPY
            var trans = 1.0;
            for (var i = 1; i <= nc; ++i)
            {
                trans = trans * tdircc[i];
            label5:;
            }
            var rinter = rain * (1.0 - trans);
            //
            var sumlai = 0.0;
            for (var j = 1; j <= nplant; ++j)
            {
                sumlai = sumlai + _clayrs.Totlai[j];
            label10:;
            }
            //
            var total = 0.0;
            //**** DETERMINE HOW MUCH RAINFALL INTERCEPTED BY EACH PLANT SPECIES
            for (var j = 1; j <= nplant; ++j)
            {
                if (itype[j] != 0)
                {
                    //****       PRECIP INTERCEPTED BY LIVING PLANTS
                    var slai = 0.0;
                    for (var i = 1; i <= nc; ++i)
                    {
                        slai = slai + _clayrs.Canlai[j][i];
                    label20:;
                    }
                    //
                    //           CALCULATE PRECIP INTERCEPTED BY ALL LAYERS
                    var rint = rinter * slai / sumlai;
                    //           ADD TO PRECIP REMAINING ON PLANT LEAVES FROM BEFORE
                    pcandt[j] = pcandt[j] + rint;
                    //           ESTIMATE HOW MUCH PRECIP CAN BE HELD ON PLANTS
                    maxint = pintrcp[j] * slai;
                    //           CHECK IF PRECIP INTERCEPTED EXCEEDS AMOUNT THAT CAN BE HELD
                    if (pcandt[j] > maxint)
                    {
                        rint = maxint - (pcandt[j] - rint);
                        pcandt[j] = maxint;
                    }
                    total = total + rint;
                }
                else
                {
                    //****       DEAD PLANT MATERIAL
                    var excess = 0.0;
                    for (var i = 1; i <= nc; ++i)
                    {
                        //           CHECK IF PLANT EXISTS IN LAYER
                        if (_clayrs.Canlai[j][i] > 0.0)
                        {
                            //              PRECIP INTERCEPTED BY LAYER
                            var rintly = rinter * _clayrs.Canlai[j][i] / sumlai + excess;
                            wcandt[i] = wcandt[i] + rintly * Constn.Rhol / _clayrs.Drycan[j][i];
                            excess = 0.0;
                            //              CHECK IF THE LAYER CAN HOLD THIS MUCH WATER
                            if (wcandt[i] > wcmax)
                            {
                                excess = (wcandt[i] - wcmax) * _clayrs.Drycan[j][i] / Constn.Rhol;
                                rintly = rintly - excess;
                                wcandt[i] = wcmax;
                            }
                            total = total + rintly;
                        }
                    label30:;
                    }
                }
            label40:;
            }
            //
            //     CALCULATE THE AMOUNT OF PRECIP THAT MAKES IT THROUGH CANOPY
            rain = rain - total;
        }

        // line 10391
        private void Rainrs(ref int nr, ref double train, ref double rain, double[] zr, double[] trdt, double[] gmcdt, double[] rhor, ref double gmcmax, ref double dirres, ref double slope)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE RAINFALL DEPTH INTERCEPTED BY THE
            //     RESIDUE LAYERS, AND ADJUSTS THE TEMPERATURE TO ACCOUNT FOR THE
            //     HEAT (OR LACK THEREOF) INTRODUCED BY THE RAINFALL
            //***********************************************************************
            //
            var tdirec = new double[11];
            var tdiffu = new double[11];
            var cres = new double[11];

            //     USE THE TRANSMITTANCE TO DIRECT RADIATION TO CALCULATE THE
            //     FRACTION OF RAIN INTERCEPTED BY THE RESIDUE
            var angle = 1.5708 - slope;
            Transr(ref nr, tdirec, tdiffu, zr, rhor, ref angle, ref dirres);
            //
            //     CALCULATE THE SPECIFIC HEAT OF THE RESIDUE BEFORE RAIN IS ADDED
            Resht(ref nr, cres, gmcdt, rhor);
            //
            for (var i = 1; i <= nr; ++i)
            {
                double dz;
                if (i == 1)
                {
                    dz = zr[2] - zr[1];
                    if (nr != 1) dz = dz / 2.0;
                }
                else
                {
                    if (i == nr)
                    {
                        dz = zr[i + 1] - zr[i] + (zr[i] - zr[i - 1]) / 2.0;
                    }
                    else
                    {
                        dz = (zr[i + 1] - zr[i - 1]) / 2.0;
                    }
                }
                //
                //
                //****    CALCULATE RAINFALL INTERCEPTED BY THE NODE
                var rinter = rain * (1.0 - tdirec[i]);
                gmcdt[i] = gmcdt[i] + rinter * Constn.Rhol / (dz * rhor[1]);
                rain = rain - rinter;
                //        CHECK IF THE NODE CAN HOLD THIS MUCH WATER
                if (gmcdt[i] > gmcmax)
                {
                    var excess = rhor[i] * (gmcdt[i] - gmcmax) * dz / Constn.Rhol;
                    rinter = rinter - excess;
                    rain = rain + excess;
                    gmcdt[i] = gmcmax;
                }
                //
                //****    CALCULATE THE TEMPERATURE OF THE NODE
                var rainht = rinter * Constn.Rhol * Constn.Cl;
                var resiht = cres[i] * dz;
                trdt[i] = trdt[i] + rainht / (rainht + resiht) * (train - trdt[i]);
            //
            label10:;
            }
        }

        // line 10450
        private void Rainsl(ref int ns, ref double train, ref double rain, double[] zs, double[] tsdt, double[] vlcdt, double[] vicdt, int[] icesdt, double[] matdt, double[] totflo, double[][] saltdt, double[][] concdt, ref double pond, ref double pondmx)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE INFILTRATION INTO THE SOIL AS WELL
            //     AS THE FINAL SOIL TEMPERATURE AND SOLUTE CONCENTRATION RESULTING
            //     FROM THE HEAT AND SALTS CARRIED BY THE WATER.
            //     ---- CURRENTLY THE SUBROUTINE WILL NOT ALLOW THE WATER CONTENT TO
            //          EXCEED 90% OF THE SATURATED VALUE (PSAT=0.9)
            //***********************************************************************
            //
            var satkmx = new double[51];
            var cs = new double[51];
            var matric = new double[51];
            var infmax = 0.0;
            var infil = 0.0;
            var psat = new double[51];
            var vlc9 = new double[51];
            var mat9 = new double[51];
            var satk9 = new double[51];

            const double psatk = 0.9;

            // JM **  I added PSAT as a SAVE since PSAT should be initialized to 0.9 only at compilation, but it is changed later, and Fortran may be treating it as local.

            var vlcmax = 0.0;
            var dtime = 0.0;
            var zstar = 0.0;
            var sumz = 0.0;
            var sumzk = 0.0;
            var satinf = 0.0;
            var i = 0;
            var potinf = 0.0;
            var dtstar = 0.0;
            var time = 0.0;
            var diminf = 0.0;
            var sumz2 = 0.0;
            var dz = 0.0;
            var sloss = 0.0;
            var cleach = 0.0;
            var skqvlc = 0.0;
            var sltnew = 0.0;
            var sheat = 0.0;
            var rainht = 0.0;
            var oldvlc = 0.0;
            var tlconc = 0.0;
            var totpot = 0.0;
            var tmpfrz = 0.0;
            var avheat = 0.0;
            var rqheat = 0.0;
            var tsoil = 0.0;
            var oldvic = 0.0;
            var vicmax = 0.0;

            var dummy = 0.0;

            //     DETERMINE THE MAXIMUM CONDUCTIVITY FOR EACH NODE -- THIS WILL BE
            //     DEFINED BY THE AIR ENTRY POTENTIAL UNLESS THE SOIL CONTAINS ICE
            for (i = 1; i <= ns; ++i)
            {
                if (icesdt[i] >= 1)
                {
                    //           SATURATED CONDUCTIVITY IS LIMITED BY THE PRESENCE OF ICE
                    vlcmax = _slparm.Soilwrc[i][2] - vicdt[i];
                    if (vlcdt[i] > vlcmax) vlcmax = vlcdt[i];
                    Matvl1(i, ref matric[i], ref vlcmax, ref dummy);
                    vlc9[i] = psatk * vlcmax;
                }
                else
                {
                    //           SET MATRIC POTENTIAL FOR COMPUTING SATURATED CONDUCTIVITY
                    //           AND INITIALIZE 90% (OR PSATK) OF MAXIMUM WATER CONENT
                    matric[i] = _slparm.Soilwrc[i][1];
                    vlc9[i] = psatk * _slparm.Soilwrc[i][2];
                }
                Matvl1(i, ref mat9[i], ref vlc9[i], ref dummy);
            label10:;
            }
            Soilhk(ns, satkmx, matric, vlcdt, vicdt);
            //
            //     INITIALIZE INFILTRATION PARAMETERS
            dtime = _timewt.Dt;
            infil = 0.0;
            zstar = 0.0;
            sumz = 0.0;
            sumzk = 0.0;
            _rainslSave.Psat[1] = psatk;

            //
            i = 1;
            //     CHECK IF CONDUCTIVITY OF SURFACE IS ZERO (POSSIBLY DUE TO ICE)
            if (satkmx[1] <= 0) goto label40;
            //     CHECK IF WETTING FRONT HAS REACHED SATURATED CONDITIONS
            if (matdt[1] >= 0.0) goto label40;
            //
            //     CALCULATE THE MAXIMUM INFILTRATION THAT THE FIRST LAYER CAN HOLD
            infmax = (_rainslSave.Psat[1] * _slparm.Soilwrc[1][2] - vlcdt[1] - vicdt[1]) * (zs[2] - zs[1]) / 2.0;
            //
            //     IF CURRENT LAYER IS ALREADY SATURATED -- GO TO NEXT LAYER
            if (infmax <= 0.0) goto label30;
            //
            //     CALCULATE THE POTENTIAL INFILTRATION FOR THE LAYER
            label20:;
            if (zstar <= 1.0)
            {
                //        ASSUMPTION OF SATURATED FLOW BEHIND WETTING FRONT IS VALID
                dtstar = satkmx[i] * dtime / (_rainslSave.Psat[i] * _slparm.Soilwrc[i][2] - vlcdt[i] - vicdt[i]) / (-matdt[i] + sumz);
                potinf = (dtstar - 2 * zstar + Math.Sqrt(Math.Pow((dtstar - 2 * zstar), 2) + 8 * dtstar)) / 2;
                potinf = potinf * (_rainslSave.Psat[i] * _slparm.Soilwrc[i][2] - vlcdt[i] - vicdt[i]) * (-matdt[i] + sumz);
            }
            else
            {
                //        INFILTRATION FLOW HAS BECOME UNSATURATED -- USE FLOW THROUGH
                //        SATURATED MEDIUM
                potinf = satinf * dtime;
            }
            //
            if (infmax >= (rain - infil))
            {
                //        LAYER CAN HOLD REMAINDER OF WATER - CHECK IF ALL CAN INFILTRATE
                //
                if (potinf >= (rain - infil))
                {
                    //           LAYER CAN INFILTRATE REMAINDER OF RAIN
                    infil = rain;
                }
                else
                {
                    //           PROFILE CANNOT INFILTRATE ALL OF THE RAIN
                    infil = infil + potinf;
                }
                //
                //        INFILTRATION CALCULATION IS COMPLETE - GO TO ENERGY CALCULATION
                goto label40;
            }
            //
            //     LAYER CANNOT HOLD REMAINDER OF RAIN - CHECK IF IT CAN HOLD POTINF
            //
            if (potinf < infmax)
            {
                //        PROFILE CANNOT INFILTRATE REMAINDER OF THE RAIN
                infil = infil + potinf;
                //        INFILTRATION CALCULATION COMPLETE - GO TO ENERGY CALCULATION
                goto label40;
            }
            //
            //     POTENTIAL INFILTRATION AND RAIN ARE SUFFICIENT TO FILL LAYER
            //      --- CALCULATE TIME REQUIRED TO FILL THE LAYER AND ADJUST TIME
            if (zstar <= 1.0)
            {
                //        ASSUMPTION OF SATURATED FLOW BEHIND WETTING FRONT IS VALID
                diminf = infmax / (_rainslSave.Psat[i] * _slparm.Soilwrc[i][2] - vlcdt[i] - vicdt[i]) / (-matdt[i] + sumz);
                if (diminf > 0.01)
                {
                    dtstar = (zstar - 1) * Math.Log(1.0 + diminf) + diminf;
                }
                else
                {
                    //           IF DIMINF IS SMALL, USE ALTERNATE METHOD OF CALCULATING LOG
                    //           TERM BECAUSE SERIOUS ERRORS MAY RESULT WHEN TAKING THE LOG
                    //           OF A NUMBER VERY CLOSE TO 1.0, I.E. (1.0 + 10^-5)
                    dtstar = (zstar - 1) * 2 * diminf / (2 + diminf) + diminf;
                }
                time = dtstar * (_rainslSave.Psat[i] * _slparm.Soilwrc[i][2] - vlcdt[i] - vicdt[i]) * (-matdt[i] + sumz) / satkmx[i];
            }
            else
            {
                //        INFILTRATION FLOW HAS BECOME UNSATURATED -- USE FLOW THROUGH
                //        SATURATED MEDIUM
                time = infmax / satinf;
            }
            //
            dtime = dtime - time;
            infil = infil + infmax;
            if (dtime <= 0.0) goto label40;
            //
            //     UPDATE PARAMETERS FOR NEXT LAYER
            label30:;
            i = i + 1;
            //     CHECK IF HYDRAULIC CONDUCTIVITY IS ZERO (POSSIBLY DUE TO ICE)
            if (satkmx[i] <= 0) goto label40;
            //     CHECK IF WETTING FRONT HAS REACHED SATURATED CONDITIONS
            if (matdt[i] > sumz) goto label40;
            //
            sumz2 = (zs[i] + zs[i - 1]) / 2.0;
            dz = sumz2 - sumz;
            sumz = sumz2;
            sumzk = sumzk + dz / satkmx[i - 1];
            //
            if (i < ns)
            {
                if (zstar <= 1.0)
                {
                    //           SATURATED FLOW STILL EXISTS -- SETUP ZSTAR FOR NEXT LAYER
                    zstar = satkmx[i] * sumzk / (-matdt[i] + sumz);
                    _rainslSave.Psat[i] = psatk;
                    if (zstar > 1.0)
                    {
                        //              HYDRAULIC CONDUCTIVITY OF THIS LAYER IS SUFFICIENTLY HIGH
                        //              THAT SATURATED CONDITIONS NO LONGER EXIST -- CALCULATE
                        //              INFILTRATION USING SATURATED RELATIONSHIPS BEHIND
                        //              CURRENT LAYER ASSUMING GRAVITY FLOW.
                        satinf = (-matdt[i] + sumz) / sumzk;
                        //              COMPUTE CONDUCTIVITY OF ALL LAYERS AT PSATK (CURRENTLY
                        //              SET AT 0.90) OF MAXIMUM WATER CONENT FOR INTERPOLATION
                        Soilhk(ns, satk9, mat9, vlc9, vicdt);
                        //              DETERMINE WATER CONTENT OF LAYER SUCH THAT CONDUCTIVITY
                        //              MATCHES FLOW INTO LAYER - USE LOGARITHMIC INTERPOLATION
                        //              BETWEEN KNOWN POINTS OF VLC AND CONDUCTIVITY
                        _rainslSave.Psat[i] = Math.Pow(10.0, (Math.Log10(psatk) * Math.Log10(satinf / satkmx[i]) / Math.Log10(satk9[i] / satkmx[i])));
                        if (_rainslSave.Psat[i] > psatk) _rainslSave.Psat[i] = psatk;
                        //              DEFINE MAXIMUM SATURATED CONDUCTIVITY FOR CURRENT LAYER
                        satkmx[i] = sumz / sumzk;
                    }
                }
                else
                {
                    //           SATURATED CONDITIONS NO LONGER EXIST AT WETTING FRONT;
                    //           COMPARE FLUX TO SATURATED CONDUCTIVITY OF CURRENT LAYER
                    satinf = (-matdt[i] + sumz) / sumzk;
                    if (satkmx[i] < satinf) satinf = satkmx[i];
                    //           DETERMINE WATER CONTENT OF LAYER SUCH THAT CONDUCTIVITY
                    //           MATCHES FLOW INTO LAYER - USE LOGARITHMIC INTERPOLATION
                    //           BETWEEN KNOWN POINTS OF VLC AND CONDUCTIVITY
                    _rainslSave.Psat[i] = Math.Pow(10.0, (Math.Log10(psatk) * Math.Log10(satinf / satkmx[i]) / Math.Log10(satk9[i] / satkmx[i])));
                    if (_rainslSave.Psat[i] > psatk) _rainslSave.Psat[i] = psatk;
                    //           DEFINE MAXIMUM SATURATED CONDUCTIVITY FOR CURRENT LAYER
                    satkmx[i] = sumz / sumzk;
                }
                //        CALCULATE THE MAXIMUM INFILTRATION THAT LAYER CAN HOLD
                infmax = (_rainslSave.Psat[i] * _slparm.Soilwrc[i][2] - vlcdt[i] - vicdt[i]) * (zs[i + 1] - zs[i - 1]) / 2.0;
                //        IF LAYER CANNOT HOLD ADDITIONAL WATER, GO TO NEXT LAYER
                if (infmax <= 0.0) goto label30;
                goto label20;
            }
            else
            {
                //        INFILTRATION HAS REACHED BOTTOM OF PROFILE
                if (zstar <= 1.0)
                {
                    //           COMPUTE FLOW THROUGH SATURATED PROFILE
                    infil = infil + dtime * sumz / sumzk;
                }
                else
                {
                    //           COMPARE FLUX TO SATURATED CONDUCIVITY OF CURRENT LAYER
                    if (satkmx[i] < satinf) satinf = satkmx[i];
                    infil = infil + dtime * satinf;
                }
                if (infil > rain) infil = rain;
            }
        //
        //-----------------------------------------------------------------------
        //     INFILTRATION CALCULATION COMPLETE -- CALCULATE WATER TO BE PONDED,
        //     THEN DETERMINE TEMPERATURE, MOISTURE CONTENTS AND SOLUTES.
        //
        label40:;
            pond = rain - infil;
            rain = 0.0;
            if (pond > pondmx)
            {
                //        SET RAIN EQUAL TO THE EXCESS RAIN, I.E. THE RUNOFF
                rain = pond - pondmx;
                pond = pondmx;
            }
            //
            if (infil <= 0.0)
            {
                //        UPDATE OF WATER CONTENTS, TEMPERATURES, AND SOLUTES COMPLETE
                infmax = 0.0;
                goto label110;
            }
            //
            //     CALCULATE THE SPECIFIC HEAT OF THE LAYERS PRIOR TO ADDING WATER
            Soilht(ref ns, cs, vlcdt, vicdt, tsdt, matdt, concdt);
            //
            //     DEFINE CONDITIONS FOR SURFACE NODE
            //     SLOSS = SALTS LOST OR LEACHED FROM NODES ABOVE
            i = 1;
            sloss = 0.0;
            dz = (zs[2] - zs[1]) / 2.0;
        //
        //     START ENERGY AND SOLUTE CALCULATIONS FOR EACH NODE
        //
        //     CALCULATE THE WATER INFILTRATED INTO THE CURRENT NODE
        label50:;
            infmax = (_rainslSave.Psat[i] * _slparm.Soilwrc[i][2] - vlcdt[i] - vicdt[i]) * dz;
            if (infmax < 0.0) infmax = 0.0;
            if (infmax >= infil) infmax = infil;
            //
            //     ADJUST THE SOLUTE CONCENTRATION IN NODE TO ACCOUNT FOR LEACHING
            for (var j = 1; j <= _slparm.Nsalt; ++j)
            {
                //        CALCULATE SOLUTE CONCENTRATION OF WATER COMING INTO NODE
                //        AND ADJUST SALT CONCENTRATION IN NODE FOR THE WATER
                //        REQUIRED TO GET TO MAXIMUM WATER CONTENT
                if (infil <= 0.0)
                {
                    cleach = 0.0;
                }
                else
                {
                    cleach = sloss / Constn.Rhol / infil;
                }
                saltdt[j][i] = saltdt[j][i] + cleach * Constn.Rhol * infmax / _slparm.Rhob[i];
                //        CALCULATE SALT PRESENT IN NODE AFTER LEACHING
                skqvlc = _slparm.Saltkq[j][i] + (vlcdt[i] + infmax / dz) * Constn.Rhol / _slparm.Rhob[i];
                sltnew = (cleach * Constn.Rhol * (infil - infmax) / dz + saltdt[j][i] * (_slparm.Rhob[i] - _timewt.Wt * Constn.Rhol * (infil - infmax) / dz / skqvlc)) / (_slparm.Rhob[i] + _timewt.Wdt * Constn.Rhol * (infil - infmax) / dz / skqvlc);
                if (sltnew < 0.0) sltnew = 0.0;
                sloss = (saltdt[j][i] - sltnew) * _slparm.Rhob[i] * dz;
                saltdt[j][i] = sltnew;
            label60:;
            }
            //
            //     CALCULATE THE HEAT CAPACITY OF THE RAIN AND OF THE SOIL
            //     --RAIN HEAT CAPACITY IS BASED ON TOTAL WATER ABSORBED AND PASSING
            //     THROUGH CURRENT NODE (TEMP WATER LEAVING NODE IS SOIL TEMP)
            sheat = cs[i] * dz;
            rainht = infil * Constn.Rhol * Constn.Cl;
            //
            if (icesdt[i] <= 0)
            {
                //        LAYER IS NOT FROZEN -- CALCULATE THE TEMPERATURE DIRECTLY AND
                //        ADJUST MOISTURE CONTENT, MATRIC POTENTIAL, AND SOLUTES
                tsdt[i] = tsdt[i] + rainht / (rainht + sheat) * (train - tsdt[i]);
                vlcdt[i] = vlcdt[i] + infmax / dz;
                //        DO NOT ALLOW ADJUSTMENT OF SATURATED POTENTIALS
                if (matdt[i] < _slparm.Soilwrc[i][1]) Matvl1(i, ref matdt[i], ref vlcdt[i], ref dummy);
                for (var j = 1; j <= _slparm.Nsalt; ++j)
                {
                    concdt[j][i] = saltdt[j][i] / (_slparm.Saltkq[j][i] + vlcdt[i] * Constn.Rhol / _slparm.Rhob[i]);
                label70:;
                }
                //
            }
            else
            {
                //        LAYER CONTAINS ICE -- MUST ACCOUNT FOR LATENT HEAT OF FUSION.
                //        CALCULATE TEMPERATURE AT WHICH LAYER WILL BE COMPLETELY THAWED
                oldvlc = vlcdt[i];
                vlcdt[i] = vlcdt[i] + vicdt[i] * Constn.Rhoi / Constn.Rhol + infmax / dz;
                Matvl1(i, ref matdt[i], ref vlcdt[i], ref dummy);
                tlconc = 0.0;
                for (var j = 1; j <= _slparm.Nsalt; ++j)
                {
                    concdt[j][i] = saltdt[j][i] / (_slparm.Saltkq[j][i] + vlcdt[i] * Constn.Rhol / _slparm.Rhob[i]);
                    tlconc = tlconc + concdt[j][i];
                label80:;
                }
                totpot = matdt[i] - tlconc * Constn.Ugas * 273.16 / Constn.G;
                tmpfrz = 273.16 * totpot / (Constn.Lf / Constn.G - totpot);
                //
                //        CHECK IF HEAT OF RAIN IS SUFFICIENT TO MELT ALL OF THE ICE
                avheat = rainht * (train - tmpfrz);
                rqheat = Constn.Rhoi * Constn.Lf * vicdt[i] * dz + sheat * (tmpfrz - tsdt[i]);
                if (avheat >= rqheat)
                {
                    //           AVAILABLE HEAT IS MORE THAN REQUIRED TO MELT THE ICE.
                    //           CALCULATE TEMPERATURE OF SOIL
                    tsdt[i] = tmpfrz + (avheat - rqheat) / (sheat + rainht);
                    vicdt[i] = 0.0;
                    //           DONE WITH THIS LAYER -- GO TO NEXT LAYER
                    goto label110;
                }
                //
                //        LAYER REMAINS FROZEN - ASSUME TEMPERATURE IS AT FREEZING POINT
                //        AND CALCULATE THE ICE CONTENT FROM AN ENERGY BALANCE:
                //        (LATENT HEAT)*(DELTA ICE) =
                //                        RAINHT*(TRAIN-T(NEW)) + SOILHT(T(NEW) - TSDT)
                tsoil = tsdt[i];
                oldvic = vicdt[i];
                vicmax = vicdt[i] + infmax / dz * (Constn.Rhol / Constn.Rhoi);
                vicdt[i] = oldvic - (rainht * (train - tmpfrz) - sheat * (tmpfrz - tsoil)) / (Constn.Rhoi * Constn.Lf * dz);
                if (vicdt[i] > vicmax) vicdt[i] = vicmax;
                vlcdt[i] = oldvlc + (vicmax - vicdt[i]) * Constn.Rhoi / Constn.Rhol;
                //
                //        DETERMINE MATRIC, SOLUTES AND TEMP FOR THIS WATER CONTENT
                Matvl1(i, ref matdt[i], ref vlcdt[i], ref dummy);
                tlconc = 0.0;
                for (var j = 1; j <= _slparm.Nsalt; ++j)
                {
                    concdt[j][i] = saltdt[j][i] / (_slparm.Saltkq[j][i] + vlcdt[i] * Constn.Rhol / _slparm.Rhob[i]);
                    tlconc = tlconc + concdt[j][i];
                label90:;
                }
                totpot = matdt[i] - tlconc * Constn.Ugas * 273.16 / Constn.G;
                tsdt[i] = 273.16 * totpot / (Constn.Lf / Constn.G - totpot);
                //
                //        CALCULATE ICE CONTENT USING UPDATED TEMPERATURE
                vicdt[i] = oldvic - (rainht * (train - tsdt[i]) - sheat * (tsdt[i] - tsoil)) / (Constn.Rhoi * Constn.Lf * dz);
                if (vicdt[i] > vicmax) vicdt[i] = vicmax;
                vlcdt[i] = oldvlc + (vicmax - vicdt[i]) * Constn.Rhoi / Constn.Rhol;
                //
                //        DETERMINE MATRIC, SOLUTES AND TEMP FOR THIS WATER CONTENT
                Matvl1(i, ref matdt[i], ref vlcdt[i], ref dummy);
                tlconc = 0.0;
                for (var j = 1; j <= _slparm.Nsalt; ++j)
                {
                    concdt[j][i] = saltdt[j][i] / (_slparm.Saltkq[j][i] + vlcdt[i] * Constn.Rhol / _slparm.Rhob[i]);
                    tlconc = tlconc + concdt[j][i];
                label100:;
                }
                totpot = matdt[i] - tlconc * Constn.Ugas * 273.16 / Constn.G;
                tsdt[i] = 273.16 * (Constn.Lf / Constn.G / (Constn.Lf / Constn.G - totpot) - 1.0);
            }
        //
        //     CALCULATE CONDITIONS OF NEXT LAYER
        label110:;
            i = i + 1;
            infil = infil - infmax;
            //     ADD INFILTRATION BETWEEN NODES TO TOTAL FLOW BETWEEN NODES
            totflo[i - 1] = totflo[i - 1] + infil;
            if (i < ns)
            {
                dz = (zs[i + 1] - zs[i - 1]) / 2.0;
                train = tsdt[i - 1];
                if (infil > 0.0) goto label50;
            }
        }

        // line 10827
        private void Wbalnc(ref int nplant, ref int nc, ref int nsp, ref int nr, ref int ns, ref int lvlout, ref int julian, ref int hour, ref int year,
            int[] itype, int inital, double[] zc, double[] wcan, double[] wcandt, double[] pcan, double[] pcandt, double[] vapc, double[] vapcdt,
            double[] rhosp, double[] dzsp, double[] dlwdt, double[] wlag, ref double store, double[] zr, double[] gmc, double[] gmcdt, double[] vapr,
            double[] vaprdt, double[] rhor, double[] zs, double[] vlc, double[] vlcdt, double[] vic, double[] vicdt, double[] totflo, ref double precip,
            ref double runoff, ref double pond, ref double evap1, ref double melt, ref double etsum, ShawDammResults results)
        {
            //
            //     THIS SUBROUTINE SUMS THE EVAPORATION AND DEEP PERCOLATION AT THE
            //     END OF EACH HOUR, THEN PRINTS THE SUMMARY AT THE DESIRED OUTPUT
            //     INTERVAL
            //
            //***********************************************************************
            //

            var dz = 0.0;
            var swedt = 0.0;
            var tpcan = 0.0;
            var dpond = 0.0;
            var error = 0.0;

            var waterBalanceOut = _outputWriters[OutputFile.WaterBalance];

            if (inital == 0)
            {
                // write(31,100)
                waterBalanceOut.WriteLine();
                waterBalanceOut.WriteLine("                                       SUMMARY OF WATER BALANCE");
                waterBalanceOut.WriteLine();
                waterBalanceOut.WriteLine("                                                              CHANGE IN STORAGE");
                waterBalanceOut.WriteLine("                               PRECIP          PLANT   ------------------------------  DEEP                     CUM.");
                waterBalanceOut.WriteLine(" DAY HR  YR   PRECIP  SNOWMELT INTRCP    ET    TRANSP  CANOPY   SNOW  RESIDUE   SOIL   PERC   RUNOFF   PONDED    ET    ERROR");
                waterBalanceOut.WriteLine("                 MM      MM      MM      MM      MM      MM      MM      MM      MM      MM      MM      MM      MM      MM");

                //        CALCULATE THE SNOW WATER EQUIVALENT FOR THE INITIAL CONDITIONS
                _wbalncSave.Swe = 0.0;
                for (var i = 1; i <= nsp; ++i)
                {
                    _wbalncSave.Swe = _wbalncSave.Swe + rhosp[i] * dzsp[i] / Constn.Rhol + dlwdt[i];
                label5:;
                }
                return;
            }
            //
            //
            //     END OF THE HOUR -- DETERMINE THE CHANGE IN STORAGE
            //
            //     CHANGE IN STORAGE OF CANOPY
            for (var i = 1; i <= nc; ++i)
            {
                for (var j = 1; j <= nplant; ++j)
                {
                    //           CALCULATE CHANGE IN WATER CONTENT OF DEAD PLANT MATERIAL
                    if (itype[j] == 0) _wbalncSave.Dcan = _wbalncSave.Dcan + (wcandt[i] - wcan[i]) * _clayrs.Drycan[j][i] * 1000.0 / Constn.Rhol;
                    label10:;
                }
                //        INCLUDE CHANGE IN VAPOR DENSITY OF AIR SPACE
                if (i == 1)
                {
                    dz = (zc[2] - zc[1]) / 2.0;
                }
                else
                {
                    dz = (zc[i + 1] - zc[i - 1]) / 2.0;
                }
                _wbalncSave.Dcan = _wbalncSave.Dcan + dz * (vapcdt[i] - vapc[i]) * 1000.0 / Constn.Rhol;
            label15:;
            }
            //
            //     CHANGE IN SNOWPACK
            swedt = 0.0;
            if (nsp > 0)
            {
                for (var i = 1; i <= nsp; ++i)
                {
                    swedt = swedt + rhosp[i] * dzsp[i] / Constn.Rhol + dlwdt[i];
                label20:;
                }
                //        WATER IN PROCESS OF BEING LAGGED THROUGH THE SNOWPACK
                swedt = swedt + store;
                for (var i = 1; i <= 11; ++i)
                {
                    swedt = swedt + wlag[i];
                label25:;
                }
            }
            //     CALCULATE CHANGE IN WATER CONTENT OF SNOWPACK IN MILLIMETERS
            _wbalncSave.Dsnow = _wbalncSave.Dsnow + (swedt - _wbalncSave.Swe) * 1000.0;
            _wbalncSave.Swe = swedt;
            //
            //     CHANGE IN STORAGE OF RESIDUE
            for (var i = 1; i <= nr; ++i)
            {
                if (i == 1 || i == nr)
                {
                    if (nr == 1)
                    {
                        dz = zr[nr + 1];
                    }
                    else
                    {
                        if (i == 1) dz = (zr[2] - zr[1]) / 2.0;
                        if (i == nr) dz = zr[nr + 1] - zr[nr] + (zr[nr] - zr[nr - 1]) / 2.0;
                    }
                }
                else
                {
                    dz = (zr[i + 1] - zr[i - 1]) / 2.0;
                }
                _wbalncSave.Dres = _wbalncSave.Dres + dz * ((gmcdt[i] - gmc[i]) * rhor[i] + (vaprdt[i] - vapr[i])) * 1000.0 / Constn.Rhol;
            label30:;
            }
            //
            //     CHANGE IN STORAGE FOR THE SOIL
            for (var i = 1; i <= ns - 1; ++i)
            {
                if (i == 1)
                {
                    dz = (zs[2] - zs[1]) / 2.0;
                }
                else
                {
                    dz = (zs[i + 1] - zs[i - 1]) / 2.0;
                }
                _wbalncSave.Dsoil = _wbalncSave.Dsoil + dz * 1000 * (vlcdt[i] - vlc[i] + (vicdt[i] - vic[i]) * Constn.Rhoi / Constn.Rhol);
            label40:;
            }
            //
            //     COMPUTE THE CHANGE IN PRECIP INTERCEPTED ON PLANT LEAVES
            tpcan = 0.0;
            for (var j = 1; j <= nplant; ++j)
            {
                if (itype[j] != 0) tpcan = tpcan + pcandt[j];
                if (itype[j] != 0) _wbalncSave.Dpcan = _wbalncSave.Dpcan + pcandt[j] - pcan[j];
                label50:;
            }
            //
            _wbalncSave.Rain = _wbalncSave.Rain + precip;
            _wbalncSave.Trunof = _wbalncSave.Trunof + runoff;
            _wbalncSave.Tetsum = _wbalncSave.Tetsum + etsum;
            _wbalncSave.Tevap = _wbalncSave.Tevap + evap1;
            _wbalncSave.Tmelt = _wbalncSave.Tmelt + melt;
            _wbalncSave.Tperc = _wbalncSave.Tperc + totflo[ns - 1];
            //
            //
            //     RETURN IF HOURLY OUTPUT IS NOT REQUIRED AND IT IS NOT END OF DAY
            if (hour % lvlout != 0) return;
            //
            //     CONVERT TO MILLIMETERS
            tpcan = tpcan * 1000.0;
            _wbalncSave.Dpcan = _wbalncSave.Dpcan * 1000.0;
            dpond = pond * 1000.0 - _wbalncSave.Pond2;
            _wbalncSave.Pond2 = pond * 1000.0;
            _wbalncSave.Rain = _wbalncSave.Rain * 1000.0;
            _wbalncSave.Trunof = _wbalncSave.Trunof * 1000.0;
            _wbalncSave.Tetsum = _wbalncSave.Tetsum * 1000.0;
            _wbalncSave.Tevap = _wbalncSave.Tevap * 1000.0;
            _wbalncSave.Tmelt = _wbalncSave.Tmelt * 1000.0;
            _wbalncSave.Tperc = _wbalncSave.Tperc * 1000.0;
            _wbalncSave.Cumvap = _wbalncSave.Cumvap + _wbalncSave.Tevap;
            error = _wbalncSave.Rain + _wbalncSave.Tevap - _wbalncSave.Tperc - _wbalncSave.Trunof - dpond - _wbalncSave.Dpcan - _wbalncSave.Dcan - _wbalncSave.Dsnow - _wbalncSave.Dres - _wbalncSave.Dsoil;

            // JM: add water balance values to results
            results.DailyEvapotranspiration.Add(-_wbalncSave.Tevap);
            results.DailyDeepPercolation.Add(_wbalncSave.Tperc);
            results.DailyRunoff.Add(_wbalncSave.Trunof);

            if (lvlout != 0)
                waterBalanceOut.WriteLine($" {julian,3:D}{hour,3:D}{year,5:D} {_wbalncSave.Rain,7:F2} {_wbalncSave.Tmelt,7:F2} {tpcan,7:F2} {-_wbalncSave.Tevap,7:F2} {_wbalncSave.Tetsum,7:F2} " +
                                      $"{_wbalncSave.Dcan,7:F2} {_wbalncSave.Dsnow,7:F2} {_wbalncSave.Dres,7:F2} {_wbalncSave.Dsoil,7:F2} {_wbalncSave.Tperc,7:F2} {_wbalncSave.Trunof,7:F2} {_wbalncSave.Pond2,7:F2} " +
                                      $"{-_wbalncSave.Cumvap,7:F1} {error,7:F2}");
            //
            _wbalncSave.Rain = 0.0;
            _wbalncSave.Dpcan = 0.0;
            _wbalncSave.Trunof = 0.0;
            _wbalncSave.Tetsum = 0.0;
            _wbalncSave.Tevap = 0.0;
            _wbalncSave.Tmelt = 0.0;
            _wbalncSave.Tperc = 0.0;
            _wbalncSave.Dcan = 0.0;
            _wbalncSave.Dsnow = 0.0;
            _wbalncSave.Dres = 0.0;
            _wbalncSave.Dsoil = 0.0;
        }

        // line 10962
        private void Energy(ref int nsp, ref int lvlout, ref int julian, ref int hour, ref int year, int inital, ref double dtime, ref double tswsno, ref double tlwsno, ref double tswcan, ref double tlwcan, ref double tswres, ref double tlwres, ref double tswsoi, ref double tlwsoi, ref double tswdwn, ref double tlwdwn, ref double tswup, ref double tlwup, ref double thflux, ref double tgflux, ref double evap1)
        {
            //
            //     THIS SUBROUTINE SUMS THE SOLAR AND LONG-WAVE RADIATION AND THE
            //     TURBULENT CONVECTION AT THE SURFACE, AND PRINT THE SUMMARY OF THE
            //     ENERGY TRANSFER AT THE SURFACE AT THE DESIRED OUTPUT INTERVAL.
            //
            //***********************************************************************
            //

            // JM ** : made SWDOWN,LWDOWN,SWUP,LWUP into SAVEs.

            if (inital == 0)
            {
                _outputWriters[OutputFile.SurfaceEnergyBalance].WriteLine();
                _outputWriters[OutputFile.SurfaceEnergyBalance].WriteLine("                                                               SUMMARY OF SURFACE ENERGY BALANCE");
                _outputWriters[OutputFile.SurfaceEnergyBalance].WriteLine();
                _outputWriters[OutputFile.SurfaceEnergyBalance].WriteLine();
                _outputWriters[OutputFile.SurfaceEnergyBalance].WriteLine("              INCOMING                NET SOLAR RADIATION BY MATERIAL                               NET LONG-WAVE RADIATION BY MATERIAL");
                _outputWriters[OutputFile.SurfaceEnergyBalance].WriteLine($"               SOLAR   REFLECTED {new string('-', 42)}  INCOMING OUTGOING   {new string('-', 41)}");
                _outputWriters[OutputFile.SurfaceEnergyBalance].WriteLine(" DAY HR  YR  ON SLOPE   SOLAR   CANOPY     SNOW   RESIDUE    SOIL    TOTAL  LONGWAVE LONGWAVE   CANOPY    SNOW   RESIDUE    SOIL    TOTAL  SENSIBLE  LATENT    SOIL");
                _outputWriters[OutputFile.SurfaceEnergyBalance].WriteLine($"            {string.Concat(Enumerable.Repeat("     W/M2", 17))}");
            }

            _energySave.Sswcan = _energySave.Sswcan + tswcan;
            _energySave.Slwcan = _energySave.Slwcan + tlwcan;
            _energySave.Sswsno = _energySave.Sswsno + tswsno;
            _energySave.Slwsno = _energySave.Slwsno + tlwsno;
            _energySave.Sswres = _energySave.Sswres + tswres;
            _energySave.Slwres = _energySave.Slwres + tlwres;
            _energySave.Sswsoi = _energySave.Sswsoi + tswsoi;
            _energySave.Slwsoi = _energySave.Slwsoi + tlwsoi;
            _energySave.Swdown = _energySave.Swdown + tswdwn;
            _energySave.Lwdown = _energySave.Lwdown + tlwdwn;
            _energySave.Swup = _energySave.Swup + tswup;
            _energySave.Lwup = _energySave.Lwup + tlwup;
            _energySave.Shflux = _energySave.Shflux + thflux;
            _energySave.Sgflux = _energySave.Sgflux + tgflux;
            _energySave.Stime = _energySave.Stime + dtime;
            //     LATENT HEAT
            if (nsp > 0)
            {
                //        USE LATENT HEAT OF SUBLIMATION
                _energySave.Slatnt = _energySave.Slatnt + Constn.Ls * evap1 * Constn.Rhol / 1000.0;
            }
            else
            {
                _energySave.Slatnt = _energySave.Slatnt + Constn.Lv * evap1 * Constn.Rhol / 1000.0;
            }
            //     RETURN IF HOURLY OUTPUT IS NOT REQUIRED AND IT IS NOT END OF DAY
            if (hour % lvlout != 0) return;
            //
            var totswr = _energySave.Sswcan + _energySave.Sswsno + _energySave.Sswres + _energySave.Sswsoi;
            var totlwr = _energySave.Slwcan + _energySave.Slwsno + _energySave.Slwres + _energySave.Slwsoi;
            //
            //     PRINT OUT FLUXES - CONVERT FROM KJ/M2 TO W/M2
            var cvrt = 1000.0 / _energySave.Stime;
            _outputWriters[OutputFile.SurfaceEnergyBalance].WriteLine($"{julian,4:D}{hour,3:D}{year,5:D} { _energySave.Swdown * cvrt,8:F1} { _energySave.Swup * cvrt,8:F1} {_energySave.Sswcan * cvrt,8:F1} {_energySave.Sswsno * cvrt,8:F1} {_energySave.Sswres * cvrt,8:F1} {_energySave.Sswsoi * cvrt,8:F1} {totswr * cvrt,8:F1} { _energySave.Lwdown * cvrt,8:F1} { _energySave.Lwup * cvrt,8:F1} {_energySave.Slwcan * cvrt,8:F1} {_energySave.Slwsno * cvrt,8:F1} {_energySave.Slwres * cvrt,8:F1} {_energySave.Slwsoi * cvrt,8:F1} {totlwr * cvrt,8:F1} {_energySave.Shflux * cvrt,8:F1} {_energySave.Slatnt * cvrt,8:F1} {_energySave.Sgflux * cvrt,8:F1}");
            //
            _energySave.Sswcan = 0.0;
            _energySave.Sswsno = 0.0;
            _energySave.Sswres = 0.0;
            _energySave.Sswsoi = 0.0;
            _energySave.Slwcan = 0.0;
            _energySave.Slwsno = 0.0;
            _energySave.Slwres = 0.0;
            _energySave.Slwsoi = 0.0;
            _energySave.Swdown = 0.0;
            _energySave.Lwdown = 0.0;
            _energySave.Swup = 0.0;
            _energySave.Lwup = 0.0;
            _energySave.Shflux = 0.0;
            _energySave.Sgflux = 0.0;
            _energySave.Slatnt = 0.0;
            _energySave.Stime = 0.0;
        }

        // line 11562
        private void Snowtemp(ref int nsp, ref int lvlout, ref int julian, ref int hour, ref int year, int inital, double[] zsp, double[] tspdt)
        {
            //
            //     This subroutine outputs the snow temperature at the soil surface
            //     and interpolates the snow temperature at 10 cm increments to the
            //     snow surface.
            //
            //***********************************************************************
            var height = new double[101];
            var temp10 = new double[101];

            var j = 0;
            var i = 0;
            var high = 0.0;

            if (inital == 0)
            {
                _outputWriters[OutputFile.SnowTemperature].WriteLine("INTERPOLATED SNOW TEMPERATURES (C) AT 10-CM INCREMENTS FROM THE SOIL TO THE SNOW SURFACE");
                _outputWriters[OutputFile.SnowTemperature].WriteLine("  DY HR   YR  n DEPTH     0cm  10cm  20cm  30cm . . . etc to snow surface");
            }
            //
            if (hour % lvlout != 0 || nsp == 0) return;
            //
            //     Reverse depths to measure up from surface
            for (i = 1; i <= nsp + 1; ++i)
            {
                height[i] = zsp[nsp + 1] - zsp[i];
            }
            //
            //     Set temperature at bottom of snow pack
            temp10[1] = tspdt[nsp + 1];
            if (temp10[1] > 0.0) temp10[1] = 0.0;
            //
            //     Parse out temperature at 10-cm increments
            j = 0;
            i = nsp + 1;
        label10:;
            high = j * 0.1;
        //     Locate depth to interpolate
        label20:;
            if (height[i] <= high && height[i - 1] >= high)
            {
                j = j + 1;
                temp10[j] = tspdt[i] + (high - height[i]) * (tspdt[i - 1] - tspdt[i]) / (height[i - 1] - height[i]);
                goto label10;
            }
            else
            {
                i = i - 1;
                if (i > 1) goto label20;
            }
            //
            j = j + 1;
            temp10[j] = tspdt[1];
            _outputWriters[OutputFile.SnowTemperature].WriteLine($"{julian,4:D}{hour,3:D}{year,5:D}{j,3:D}{zsp[nsp + 1] * 100.0,6:F1}  {string.Concat(Enumerable.Range(1, j).Select(k => $"{temp10[k],6:F1}"))}");

        }

        // line 11641
        private void Frost(ref int nsp, ref int ns, ref int lvlout, ref int julian, ref int hour, ref int year, int inital, double[] zsp,
            double[] rhosp, double[] dzsp, double[] dlwdt, double[] wlag, ref double store, double[] zs, double[] vlcdt, double[] vicdt, int[] icesdt)
        {
            //
            //     THIS SUBROUTINE INTERPOLATES BETWEEN NODES TO DETERMINE THE FROST
            //     DEPTH, THEN PRINT THE FROST DEPTH AND SNOW DEPTH AT THE DESIRED
            //     OUTPUT INTERVAL
            //
            //***********************************************************************
            //
            //
            //

            int nthaw;
            int nfrost;
            int nf;
            double fractn;
            int nt;
            double swe;
            double snow;

            if (inital == 0)
            {
                _outputWriters[OutputFile.SnowFrostDepth].WriteLine();
                _outputWriters[OutputFile.SnowFrostDepth].WriteLine("                     FROST AND SNOW DEPTH");
                _outputWriters[OutputFile.SnowFrostDepth].WriteLine();
                _outputWriters[OutputFile.SnowFrostDepth].WriteLine();
                _outputWriters[OutputFile.SnowFrostDepth].WriteLine(" DAY HR  YR     THAW   FROST    SNOW     SWE      ICE CONTENT OF EACH NODE (M3/M3)");
                _outputWriters[OutputFile.SnowFrostDepth].WriteLine($"                 CM      CM      CM       MM      {string.Concat(Enumerable.Range(1, ns).Select(i => $"{zs[i],6:F2}"))}");

                _frostSave.Last = 0;
                _frostSave.Lday = 0;
                _frostSave.Lhour = 0;
                _frostSave.Lyr = 0;
                _frostSave.Zero = 0.0;
                _frostSave.Fdepth = 0.0;
                _frostSave.Tdepth = 0.0;
                goto label5;
            }
            //
            if (hour % lvlout != 0) return;
            //
            //     FIND LAYERS OF MAXIMUM THAW AND FROST
            label5:;
            nthaw = 0;
            nfrost = 0;
            for (var i = ns; i >= 1; --i)
            {
                if (icesdt[i] == 1)
                {
                    if (nfrost == 0) nfrost = i;
                }
                else
                {
                    if (nfrost > 0 && nthaw == 0)
                    {
                        nthaw = i + 1;
                        goto label15;
                    }
                }
            label10:;
            }
        //
        label15:;
            if (nfrost == 0 && _frostSave.Last == 0 && nsp <= 0)
            {
                //        SAVE TIME ON WHICH FROST WAS LAST CHECKED AND RETURN
                _frostSave.Lday = julian;
                _frostSave.Lhour = hour;
                _frostSave.Lyr = year;
                return;
            }
            //
            //     IF LAST = 0, SNOW OR FROST WAS NOT PRESENT LAST TIME STEP BUT IS
            //     PRESENT THIS TIME STEP --> PRINT OUT ZEROES FOR LAST TIME STEP
            if (_frostSave.Last == 0 && _frostSave.Lday != 0) // write(35, 110)_frostSave.Lday,_frostSave.Lhour,_frostSave.Lyr,(_frostSave.Zero,i = 1,ns + 4);
                _outputWriters[OutputFile.SnowFrostDepth].WriteLine($"{_frostSave.Lday,4:D}{_frostSave.Lhour,3:D}{_frostSave.Lyr,5:D}{string.Concat(Enumerable.Repeat($"{_frostSave.Zero,8:F1}", 4))}      {string.Concat(Enumerable.Repeat($"{_frostSave.Zero,6:F3}", ns))}");

            _frostSave.Lday = julian;
            _frostSave.Lhour = hour;
            _frostSave.Lyr = year;
            //
            if (nfrost > 0)
            {
                //        CALCULATE DEPTH OF FROST
                _frostSave.Last = 1;
                nf = nfrost;
                fractn = vicdt[nf] / (vlcdt[nf] + vicdt[nf]);
                if (nf == 1 || nf == ns)
                {
                    if (nf == 1)
                    {
                        _frostSave.Fdepth = fractn * (zs[2] - zs[1]) / 2.0;
                    }
                    else
                    {
                        _frostSave.Fdepth = (zs[ns] + zs[ns - 1]) / 2.0 + fractn * (zs[ns] - zs[ns - 1]) / 2.0;
                    }
                }
                else
                {
                    _frostSave.Fdepth = (zs[nf] + zs[nf - 1]) / 2.0 + fractn * (zs[nf + 1] - zs[nf - 1]) / 2.0;
                }
                //
                _frostSave.Tdepth = 0.0;
                if (nthaw > 0)
                {
                    //           CALCULATE DEPTH OF THAW
                    nt = nthaw;
                    fractn = vlcdt[nt] / (vlcdt[nt] + vicdt[nt]);
                    if (nt == 1)
                    {
                        _frostSave.Tdepth = fractn * (zs[2] - zs[1]) / 2.0;
                    }
                    else
                    {
                        if (nt == nfrost)
                        {
                            _frostSave.Tdepth = (zs[nt] + zs[nt - 1]) / 2.0 + fractn * (_frostSave.Fdepth - (zs[nt] + zs[nt - 1]) / 2.0);
                        }
                        else
                        {
                            _frostSave.Tdepth = (zs[nt] + zs[nt - 1]) / 2.0 + fractn * (zs[nt + 1] - zs[nt - 1]) / 2.0;
                        }
                    }
                }
                //
            }
            else
            {
                //        NO FROST EXISTS IN THE PROFILE - IF FROST EXISTED AT THE LAST
                //        OUTPUT, INDICATE THAT FROST HAS LEFT
                if (_frostSave.Last > 0)
                {
                    //           FROST EXISTED IN PROFILE AT LAST OUTPUT -- ENSURE THAT
                    //           FROST DEPTH AND THAW DEPTH LINES COME TOGETHER
                    if (_frostSave.Tdepth == 0.0)
                    {
                        //              GROUND WAS THAWING FROM THE BOTTOM UP
                        _frostSave.Fdepth = 0.0;
                    }
                    else
                    {
                        //              THAW AND FROST DEPTH COME TOGETHER SOMEWHERE BETWEEN THE
                        //              DEPTHS AT LAST OUTPUT - USUALLY 2/3 BETWEEN
                        _frostSave.Fdepth = (2.0 * _frostSave.Fdepth + _frostSave.Tdepth) / 3.0;
                        _frostSave.Tdepth = _frostSave.Fdepth;
                    }
                }
                else
                {
                    //           NO FROST IN PROFILE AT LAST OUTPUT
                    _frostSave.Fdepth = 0.0;
                    _frostSave.Tdepth = 0.0;
                }
                _frostSave.Last = 0;
            }
            //
            swe = 0.0;
            if (nsp <= 0)
            {
                snow = 0.0;
            }
            else
            {
                //        SAVE SNOW DEPTH AND CALCULATE SNOW WATER EQUIVALENT
                snow = zsp[nsp + 1];
                for (var i = 1; i <= nsp; ++i)
                {
                    swe = swe + rhosp[i] * dzsp[i] / Constn.Rhol + dlwdt[i];
                label20:;
                }
                //        WATER IN PROCESS OF BEING LAGGED THROUGH THE SNOWPACK
                swe = swe + store;
                for (var i = 1; i <= 11; ++i)
                {
                    swe = swe + wlag[i];
                label25:;
                }
                _frostSave.Last = 1;
            }
            //
            //     PRINT FROST, THAW AND SNOW DEPTH IN CENTIMETERS AND SWE IN MM
            // write(35,110)julian,hour,year,tdepth*100,fdepth*100,snow*100,swe*1000.0,(vicdt(i),i=1,ns)
            _outputWriters[OutputFile.SnowFrostDepth].WriteLine($"{julian,4:D}{hour,3:D}{year,5:D}{_frostSave.Tdepth * 100,8:F1}{_frostSave.Fdepth * 100,8:F1}{snow * 100,8:F1}{swe * 1000.0,8:F1}      {string.Concat(Enumerable.Range(1, ns).Select(i => $"{vicdt[i],6:F3}"))}");
            if (_frostSave.Last == 0)
                _outputWriters[OutputFile.SnowFrostDepth].WriteLine();
        }

        // line 11770
        private void Tdma(int n, double[] a, double[] b, double[] c, double[] d, double[] x)
        {
            //
            //     THIS SUBROUTINE SOLVES A TRI-DIAGONAL MATRIX OF THE FORM :
            //
            //              | B1  C1   0   0  .  .   . | |X1|   |D1|
            //              | A2  B2  C2   0  .  .   . | |X2|   |D2|
            //              |  0  A3  B3  C3  .  .   . | |X3| = |D3|
            //              |  .   .   .   .     .   . | | .|   | .|
            //              |  0   0   0   0  . AN  BN | |XN|   |DN|
            //
            //***********************************************************************

            for (var i = 2; i <= n; ++i)
            {
                c[i - 1] = c[i - 1] / b[i - 1];
                d[i - 1] = d[i - 1] / b[i - 1];
                b[i] = b[i] - a[i] * c[i - 1];
                d[i] = d[i] - a[i] * d[i - 1];
            label10:;
            }
            x[n] = d[n] / b[n];
            for (var i = n - 1; i >= 1; --i)
            {
                x[i] = d[i] - c[i] * x[i + 1];
            label20:;
            }
        }

        // line 11798
        /// <summary>
        ///   <para>Sets: avg.</para>
        /// </summary>
        private void Weight(int n, double[] avg, double[] begin, double[] end)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE WEIGHTED AVERAGE OF VARIABLES AT
            //     THE BEGINNING AND END OF THE TIME STEP
            //
            //        WT = WEIGHTING FOR VALUES AT THE BEGINNING OF THE TIME STEP
            //        WDT = WEIGHTING FOR VALUES AT THE END OF THE TIME STEP
            //
            //***********************************************************************

            for (var i = 1; i <= n; ++i)
            {
                avg[i] = _timewt.Wt * begin[i] + _timewt.Wdt * end[i];
            label10:;
            }
        }

        // line 11818
        /// <summary>
        ///   <para>Sets: con.</para>
        /// </summary>
        private void Conduc(int n, double[] z, double[] rk, double[] con)
        {
            //
            //     THIS SUBROUTINE CALCULATES THE CONDUCTANCE TERM BETWEEN TWO NODES
            //     USING THE GEOMETRIC MEAN AND THE SPACE INCREMENT
            //
            //                                                     K(I)
            //        K(I) = ( K(I)*K(I+1) )**1/2 =>  CON(I) = -------------
            //                                                 Z(I+1) - Z(I)
            //
            //***********************************************************************

            for (var i = 1; i <= n - 1; ++i)
            {
                con[i] = Math.Sqrt(rk[i] * rk[i + 1]) / (z[i + 1] - z[i]);
            label10:;
            }

        }

        // line 11838
        /// <summary>
        ///   <para>Sets: s, satv.</para>
        /// </summary>
        private void Vslope(ref double s, ref double satv, ref double t)
        {
            //      THIS SUBROUTINE CALCULATES THE SATURATED VAPOR DENSITY AND THE
            //      SLOPE OF THE VAPOR DENSITY CURVE  (SATV IS IN KG/M**3) USING A
            //      SEVENTH-ORDER POLYNOMIAL (ACCURATE RANGE IS -50 C TO +50 C).
            //      J. OF APPLIED METEOROLOGY 32(7):1294-1300 (1993)

            // todo: yikes!  this should be 273.15, not 273.16.
            var tmp = t + 273.16;
            satv = (6.1104546 + t * (0.4442351 + t * (0.014302099 + t * (2.6454708e-4 + t * (3.0357098e-6 + t * (2.0972268e-8 + t * (6.0487594e-11 - 1.469687e-13 * t))))))) * 100.0 / (Constn.Ugas * tmp / .018);
            if (t > 45.0)
            {
                s = (0.4442351 + t * (0.028604198 + t * (7.9364124e-4 + t * (1.2142839e-5 + t * (1.0486134e-7 + t * (3.6292556e-10 - 1.0287809e-12 * t)))))) * 100.0 / (Constn.Ugas * tmp / .018) - satv / tmp;
            }
            else
            {
                s = .0000165 + 4944.43 * satv / (tmp * tmp);
            }
        }

        // line 11867
        /// <summary>
        ///   <para>Sets: mat.</para>
        ///   <para>Does not use: dldm.</para>
        /// </summary>
        private void Matvl1(int i, ref double mat, ref double vlc, ref double dldm)
        {
            //
            //     THIS SUBROUTINE DEFINES THE RELATION BETWEEN THE VOLUMETRIC
            //     LIQUID CONTENT AND THE MATRIC POTENTIAL. THE SUBROUTINE IS DIVIDED
            //     INTO THREE PART, AND THE OUTPUT DEPENDS ON WHICH PART IS CALLED
            //           MATVL1 : THE MATRIC POTENTIAL IS CALCULATED FROM MOISTURE
            //           MATVL2 : THE MOISTURE CONTENT IS CALCULATED FROM MATRIC
            //           MATVL3 : THE DERIVATIVE OF MOISTURE CONTENT WITH RESPECT
            //                    TO MATRIC POTENTIAL IS CALCULTED.
            //                I = NODE NUMBER
            //

            //      DETERMINE THE MATRIC POTENTIAL FROM THE MOISTURE CONTENT
            if (Slparm.Iwrc == 1)
            {
                //        This is Campbell model
                if (vlc < _slparm.Soilwrc[i][2])
                {
                    mat = _slparm.Soilwrc[i][1] * Math.Pow((vlc / _slparm.Soilwrc[i][2]), (-_slparm.Soilwrc[i][3]));
                }
                else
                {
                    mat = _slparm.Soilwrc[i][1];
                }
            }
            else if (Slparm.Iwrc == 2)
            {
                //        This is Brooks-Corey model
                if (vlc < _slparm.Soilwrc[i][2])
                {
                    mat = _slparm.Soilwrc[i][1] * Math.Pow(((vlc - _slparm.Soilwrc[i][4]) / (_slparm.Soilwrc[i][2] - _slparm.Soilwrc[i][4])), (-1.0 / _slparm.Soilwrc[i][3]));
                }
                else
                {
                    mat = _slparm.Soilwrc[i][1];
                }
            }
            else if (Slparm.Iwrc == 3)
            {
                //        This is Van Genuchten model
                var se = (vlc - _slparm.Soilwrc[i][4]) / (_slparm.Soilwrc[i][2] - _slparm.Soilwrc[i][4]);
                if (se >= 1.0)
                {
                    //           PRECISION PROBLEMS CAN RESULT IN NUMERIC ERRORS WHEN Se=1.0
                    mat = 0.0;
                }
                else
                {
                    mat = -1.0 / Math.Abs(_slparm.Soilwrc[i][6]) * Math.Pow((Math.Pow(se, (-1.0 / _slparm.Soilwrc[i][7])) - 1.0), (1.0 / _slparm.Soilwrc[i][3]));
                }
            }
        }

        /// <summary>
        ///   <para>Sets: vlc.</para>
        ///   <para>Does not use: dldm.</para>
        /// </summary>
        private void Matvl2(int i, ref double mat, ref double vlc, ref double dldm)
        {
            //
            //     THIS SUBROUTINE DEFINES THE RELATION BETWEEN THE VOLUMETRIC
            //     LIQUID CONTENT AND THE MATRIC POTENTIAL. THE SUBROUTINE IS DIVIDED
            //     INTO THREE PART, AND THE OUTPUT DEPENDS ON WHICH PART IS CALLED
            //           MATVL1 : THE MATRIC POTENTIAL IS CALCULATED FROM MOISTURE
            //           MATVL2 : THE MOISTURE CONTENT IS CALCULATED FROM MATRIC
            //           MATVL3 : THE DERIVATIVE OF MOISTURE CONTENT WITH RESPECT
            //                    TO MATRIC POTENTIAL IS CALCULTED.
            //                I = NODE NUMBER
            //

            //      DETERMINE THE MOISTURE CONTENT FROM THE MATRIC POTENTIAL
            if (Slparm.Iwrc == 1)
            {
                //        This is Campbell model
                if (_slparm.Soilwrc[i][1] > mat)
                {
                    vlc = _slparm.Soilwrc[i][2] * Math.Pow((mat / _slparm.Soilwrc[i][1]), (-1.0 / _slparm.Soilwrc[i][3]));
                }
                else
                {
                    vlc = _slparm.Soilwrc[i][2];
                }
            }
            else if (Slparm.Iwrc == 2)
            {
                //        This is Brooks-Corey model
                if (_slparm.Soilwrc[i][1] > mat)
                {
                    vlc = _slparm.Soilwrc[i][4] + (_slparm.Soilwrc[i][2] - _slparm.Soilwrc[i][4]) * Math.Pow((Math.Abs(mat / _slparm.Soilwrc[i][1])), (-_slparm.Soilwrc[i][3]));
                }
                else
                {
                    vlc = _slparm.Soilwrc[i][2];
                }
            }
            else if (Slparm.Iwrc == 3)
            {
                //        This is Van Genuchten model
                if (mat < _slparm.Soilwrc[i][1])
                {
                    var alfah = Math.Pow((Math.Abs(_slparm.Soilwrc[i][6] * mat)), _slparm.Soilwrc[i][3]);
                    vlc = _slparm.Soilwrc[i][4] + (_slparm.Soilwrc[i][2] - _slparm.Soilwrc[i][4]) / Math.Pow((1.0 + alfah), _slparm.Soilwrc[i][7]);
                }
                else
                {
                    vlc = _slparm.Soilwrc[i][2];
                }
            }
        }

        /// <summary>
        ///   <para>Sets: dldm.</para>
        /// </summary>
        private void Matvl3(int i, ref double mat, ref double vlc, ref double dldm)
        {
            //
            //     THIS SUBROUTINE DEFINES THE RELATION BETWEEN THE VOLUMETRIC
            //     LIQUID CONTENT AND THE MATRIC POTENTIAL. THE SUBROUTINE IS DIVIDED
            //     INTO THREE PART, AND THE OUTPUT DEPENDS ON WHICH PART IS CALLED
            //           MATVL1 : THE MATRIC POTENTIAL IS CALCULATED FROM MOISTURE
            //           MATVL2 : THE MOISTURE CONTENT IS CALCULATED FROM MATRIC
            //           MATVL3 : THE DERIVATIVE OF MOISTURE CONTENT WITH RESPECT
            //                    TO MATRIC POTENTIAL IS CALCULTED.
            //                I = NODE NUMBER
            //

            //      DETERMINE THE DERIVATIVE OF THE MOISTURE CONTENT WITH RESPECT
            //      TO MATRIC POTENTIAL
            if (Slparm.Iwrc == 1)
            {
                //        This is Campbell model
                if (mat > _slparm.Soilwrc[i][1])
                {
                    //         LAYER IS SATURATED
                    dldm = 0.0;
                }
                else
                {
                    //         UNSATURATED CONDITIONS
                    dldm = -vlc / _slparm.Soilwrc[i][3] / mat;
                }
            }
            else if (Slparm.Iwrc == 2)
            {
                //        This is Brooks-Corey model
                if (mat > _slparm.Soilwrc[i][1])
                {
                    //         LAYER IS SATURATED
                    dldm = 0.0;
                }
                else
                {
                    //         UNSATURATED CONDITIONS
                    dldm = -_slparm.Soilwrc[i][3] * (vlc - _slparm.Soilwrc[i][4]) / mat;
                }
            }
            else if (Slparm.Iwrc == 3)
            {
                //        This is Van Genuchten model
                if (mat >= _slparm.Soilwrc[i][1])
                {
                    //         LAYER IS SATURATED
                    dldm = 0.0;
                }
                else
                {
                    //         UNSATURATED CONDITIONS
                    var alphahn = Math.Pow((Math.Abs(_slparm.Soilwrc[i][6] * mat)), _slparm.Soilwrc[i][3]);
                    dldm = -(vlc - _slparm.Soilwrc[i][4]) * _slparm.Soilwrc[i][7] * _slparm.Soilwrc[i][3] * alphahn / mat / (1.0 + alphahn);
                }
            }
        }
    }
}
