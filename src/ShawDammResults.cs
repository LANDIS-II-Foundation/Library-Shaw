using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landis.Extension.ShawDamm
{
    public class ShawDammResults
    {
        private int _ns;

        public ShawDammResults(int ns, double[] zs)
        {
            _ns = ns;
            DailySoilTemperatureProfiles = new List<double[]>();
            DailySoilLiquidWaterProfiles = new List<double[]>();
            DailySoilTotalWaterProfiles = new List<double[]>();

            DailySnowHeatCapacity = new List<double>();
            DailySnowThermalConductivity = new List<double>();
            DailySnowThickness = new List<double>();
            DailySnowDensity = new List<double>();

            DailyEvapotranspiration = new List<double>();
            DailyDeepPercolation = new List<double>();
            DailyRunoff = new List<double>();

            // copy zs (1-based) into ShawDepths (0-based)
            ShawDepths = new double[ns];
            Array.Copy(zs, 1, ShawDepths, 0, ns);
        }

        public bool Success { get; set; }

        public List<double[]> DailySoilTemperatureProfiles { get; }     // may not be needed
        public List<double[]> DailySoilLiquidWaterProfiles { get; }
        public List<double[]> DailySoilTotalWaterProfiles { get; }

        public List<double> DailySnowHeatCapacity { get; }              // may not be needed
        public List<double> DailySnowThermalConductivity { get; }       // may not be needed
        public List<double> DailySnowThickness { get; }
        public List<double> DailySnowDensity { get; }

        public List<double> DailyEvapotranspiration { get; }    // [mm]
        public List<double> DailyDeepPercolation { get; }       // [mm]
        public List<double> DailyRunoff { get; }                // [mm]

        public double MonthEvapotranspirationInCm { get; set; }  // [cm]
        public double MonthDeepPercolationInCm { get; set; }    // [cm]
        public double MonthRunoffInCm { get; set; }             // [cm]

        public double[] ShawDepths { get; set; }

        public List<double> MonthSoilLiquidWaterProfile { get; private set; }    // average across the month
        public List<double> MonthSoilTotalWaterProfile { get; private set; }    // average across the month

        public void AddOneBasedProfiles(double[] tsdt, double[] vlcdt, double[] vicdt, double rhoi, double rhol)
        {
            var t = new double[_ns];
            Array.Copy(tsdt, 1, t, 0, _ns);
            DailySoilTemperatureProfiles.Add(t);

            var vl = new double[_ns];
            Array.Copy(vlcdt, 1, vl, 0, _ns);
            var vi = new double[_ns];
            Array.Copy(vicdt, 1, vi, 0, _ns);

            var vtotal = new double[_ns];
            for (var i = 0; i < _ns; ++i)
                vtotal[i] = vlcdt[i] + vicdt[i] * rhoi / rhol;

            DailySoilLiquidWaterProfiles.Add(vl);
            DailySoilTotalWaterProfiles.Add(vtotal);
        }

        public void MakeMonthSummaries()
        {
            MonthSoilLiquidWaterProfile = AverageProfileOverDays(DailySoilLiquidWaterProfiles);
            MonthSoilTotalWaterProfile = AverageProfileOverDays(DailySoilTotalWaterProfiles);
            MonthEvapotranspirationInCm = DailyEvapotranspiration.Sum() / 10.0;      // convert to cm
            MonthDeepPercolationInCm = DailyDeepPercolation.Sum() / 10.0;           // convert to cm
            MonthRunoffInCm = DailyRunoff.Sum() / 10.0;                             // convert to cm
        }

        private List<double> AverageProfileOverDays(List<double[]> dailyProfiles)
        {
            var days = dailyProfiles.Count;
            var depths = dailyProfiles.First().Length;
            var averageProfile = new List<double>(depths);

            for (var j = 0; j < depths; ++j)
                averageProfile.Add(Enumerable.Range(0, days).Average(i => dailyProfiles[i][j]));

            return averageProfile;
        }
    }
}
