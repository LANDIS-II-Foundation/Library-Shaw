using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Landis.Extension.ShawDamm
{
    public partial class ShawDamm
    {
        private class GoshawSave
        {
            public int Nprint;
            public int Maxstp;
            public int Minstp;
            public double Melt;
        }

        private class DayinpSave
        {
            public int Nstep;
            public int Nstart;
            public int Ltmpdy;
            public int Ltmphr;
            public int Ltmpyr;
            public double Tmplst;
            public double[] Tmp1 = new double[51];
            public double Tmp;
            public int Lvlcdy;
            public int Lvlchr;
            public int Lvlcyr;
            public double Vlclst;
            public double[] Vlc1 = new double[51];
            public double Vlc;
            public int Lwtrdy;
            public int Lwtrhr;
            public int Lwtryr;
            public int Lwtrmx;
            public double[] Flux = new double[51];
            public int[] Lcandy = new int[9];
            public int[] Lcanyr = new int[9];
            public double[] Zclst = new double[9];
            public double[] Dchlst = new double[9];
            public double[] Clmplst = new double[9];
            public double[] Wlst = new double[9];
            public double[] Tlalst = new double[9];
            public double[] Rdplst = new double[9];
            public int[] Iflagc = new int[9];
            public double Lresdy;
            public double Lresyr;
            public double Covlst;
            public double Alblst;
            public double Rllst;
            public double Zrlst;
            public double Resclst;
            public double Restlst;
            public int Iflagr;
        }

        private class Day2hrSave
        {
            public double Tmax1;
            public double Tmin1;
            public double Tmax;
            public double Tmin;
            public double Tdew;
            public double Wind;
            public double Prec;
            public double Solar;
            public int Jday;
            public int Jyr;
        }

        private class CanopySave
        {
            public double[] Zzc = new double[12];
            public double[] Dzcan = new double[11];
            public double[][] Ddrycan = JaggedArray<double>(9, 11);
            public double[][] Ccanlai = JaggedArray<double>(9, 11);
        }

        private class CanlayzcSave
        {
            public int Mzc;
            public double[] Zmid = new double[11];
        }

        private class AtstabSave
        {
            public double Tmpair;
            public double Vapair;
            public double Zmlog;
            public double Zclog;
            public double Psim;
            public double Psih;
        }

        private class LeaftSave
        {
            public int[] Init = new int[9];
            public double[] Pxylem = new double[9];
            public double[][] Rhcan = JaggedArray<double>(9, 11);
            public double[][] Rvcan = JaggedArray<double>(9, 11);
            public double[][] Etcan = JaggedArray<double>(9, 11);

            public double Pleaf1;
        }

        private class CantkSave
        {
            public double Zmlog;
        }

        private class EbsnowSave
        {
            public double[] Qvspt = new double[101];
            public double[] Con = new double[101];
            public double[] Cspt = new double[101];
        }

        private class EbresSave
        {
            public double[] Crest = new double[11];
        }

        private class EbsoilSave
        {
            public double[] Qsvt = new double[51];
            public double[] Qslt = new double[51];
            public double[] Tkt = new double[51];
            public double[] Cst = new double[51];
        }

        private class SoiltkSave
        {
            public int Ifirst;
            public double Wfaird;
            public double Wfrod;
            public double Wfsad;
            public double Wfsid;
            public double Wfcld;
            public double Wfomd;
            public double Wficed;
            public double Tkma;
            public double Wfl;
            public double Wfro;
            public double Wfsa;
            public double Wfsi;
            public double Wfcl;
            public double Wfom;
            public double Wfice;
        }

        private class WbalncSave
        {
            public double Rain;
            public double Dpcan;
            public double Dcan;
            public double Dsnow;
            public double Dres;
            public double Dsoil;
            public double Trunof;
            public double Pond2;
            public double Tperc;
            public double Tetsum;
            public double Tevap;
            public double Cumvap;
            public double Swe;
            public double Tmelt;
        }

        private class EnergySave
        {
            public double Sswsno;
            public double Slwsno;
            public double Sswcan;
            public double Slwcan;
            public double Sswres;
            public double Slwres;
            public double Sswsoi;
            public double Slwsoi;
            public double Shflux;
            public double Sgflux;
            public double Slatnt;
            public double Stime;
            public double Swdown;
            public double Lwdown;
            public double Swup;
            public double Lwup;
        }

        private class FrostSave
        {
            public int Last;
            public int Lday;
            public int Lhour;
            public int Lyr;
            public double Zero;
            public double Fdepth;
            public double Tdepth;
        }

        private class OutputSave
        {
            public double Nout;
            public double Navtmp;
            public double Navmat;
            public double Navvlc;
            public double Navvwc;
            public double Navslt;
            public double Navcon;
            public double[] Avgtmp = new double[51];
            public double[] Avgmat = new double[51];
            public double[] Avgvlc = new double[51];
            public double[] Avgvwc = new double[51];
            public double[] Sumflo = new double[51];
            public double[] Sumrxt = new double[51];
            public double[] Sumlat = new double[51];
            public double[][] Avgslt = JaggedArray<double>(11, 51);
            public double[][] Avgcon = JaggedArray<double>(11, 51);
            public double Prtzero7;
            public double Prtzero8;
            public double Prtzero19;
        }

        private class SnomltSave
        {
            public int Ifirst;
            public int Nlag;
        }

        private class SumdtSave
        {
            public double[] Transp = new double[9];
        }

        private class WbsoilSave
        {
            public double[] Qslt = new double[51];
            public double[] Qsvt = new double[51];
        }

        private class RainslSave
        {
            public double[] Psat = Enumerable.Repeat(0.9, 51).ToArray();
        }
    }
}
