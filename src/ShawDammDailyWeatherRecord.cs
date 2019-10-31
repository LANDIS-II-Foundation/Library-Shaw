using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landis.Extension.ShawDamm
{
    public class ShawDammDailyWeatherRecord
    {
        //public int Day { get; set; }
        //public int Year { get; set; }
        public double Tmax { get; set; }        // C
        public double Tmin { get; set; }        // C
        public double Tdew { get; set; }        // C
        public double Wind { get; set; }        // m/s           
        public double Precip { get; set; }      // mm
        public double Solar { get; set; }       // W/m2
    }
}
