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
            DailySoilMoistureProfiles = new List<double[]>();

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
        public List<double[]> DailySoilMoistureProfiles { get; }

        public List<double> DailySnowHeatCapacity { get; }              // may not be needed
        public List<double> DailySnowThermalConductivity { get; }       // may not be needed
        public List<double> DailySnowThickness { get; }
        public List<double> DailySnowDensity { get; }

        public List<double> DailyEvapotranspiration { get; }    // [mm]
        public List<double> DailyDeepPercolation { get; }       // [mm]
        public List<double> DailyRunoff { get; }                // [mm]

        public double[] ShawDepths { get; set; }

        public double[] AverageSoilMoistureProfile { get; private set; }    // average across the month

        public void AddOneBasedProfiles(double[] tsdt, double[] vlcdt)
        {
            var t = new double[_ns];
            Array.Copy(tsdt, 1, t, 0, _ns);
            DailySoilTemperatureProfiles.Add(t);

            var v = new double[_ns];
            Array.Copy(vlcdt, 1, v, 0, _ns);
            DailySoilMoistureProfiles.Add(v);
        }

        public void MakeProfileAveragesOverDays()
        {
            AverageSoilMoistureProfile = AverageProfileOverDays(DailySoilMoistureProfiles);
        }

        private double[] AverageProfileOverDays(List<double[]> dailyProfiles)
        {
            var days = dailyProfiles.Count;
            var depths = dailyProfiles.First().Length;
            var averageProfile = new double[depths];

            for (var j = 0; j < depths; ++j)
                averageProfile[j] = Enumerable.Range(0, days).Average(i => dailyProfiles[i][j]);

            return averageProfile;
        }
    }
}
