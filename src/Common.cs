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
        private static T[][] JaggedArray<T>(int rowCount, int colCount)
        {
            return Enumerable.Range(0, rowCount).Select(x => new T[colCount]).ToArray();
        }

        private class Constn
        {
            public const double Lf = 335000.0;
            public const double Lv = 2500000.0;
            public const double Ls = 2835000.0;
            public const double G = 9.81;
            public const double Ugas = 8.3143;
            public const double Rhol = 1000.0;
            public const double Rhoi = 920.0;
            public const double Rhom = 2650.0;
            public const double Rhoom = 1300.0;
            public const double Rhoa = 1.25;
            public const double Cl = 4200.0;
            public const double Ci = 2100.0;
            public const double Cm = 900.0;
            public const double Com = 1920.0;
            public const double Ca = 1006.0;
            public const double Cv = 1860.0;
            public const double Cr = 1900.0;
            public const double Vonkrm = 0.4;
            public const double Vdiff = 0.0000212;
            public double Presur;
            public const double P0 = 101300.0;
            public const double Tkl = 0.57;
            public const double Tki = 2.2;
            public const double Tka = 0.025;
            public const double Tkr = 0.05;
            public const double Tkro = 8.8;
            public const double Tksa = 8.8;
            public const double Tksi = 2.92;
            public const double Tkcl = 2.92;
            public const double Tkom = 0.25;
        }

        private class Swrcoe
        {
            public const double Solcon = 1360.0;
            public const double Difatm = 0.76;
            public const double Difres = 0.667;
            public const double Snocof = 0.0;
            public const double Snoexp = 1.0;
        }

        private class Lwrcof
        {
            public const double Stefan = 5.6697E-08;
            public const double Ematm1 = 0.261;
            public const double Ematm2 = 0.000777;
            public const double Emitc = 0.95;
            public const double Emitr = 0.95;
            public const double Emitsp = 0.9;
            public const double Emits = 0.95;
        }

        private class Slparm
        {
            public double[] Rhob = new double[51];
            public double[] Satk = new double[51];
            public double[] Satklat = new double[51];
            public double[][] Soilwrc = JaggedArray<double>(51, 11);
            public int Nsalt;
            public double[][] Saltkq = JaggedArray<double>(11, 51);
            public double[] Vapcof = Enumerable.Repeat(0.66, 51).ToArray();
            public double[] Vapexp = Enumerable.Repeat(1.0, 51).ToArray();
            public double[] Rock = new double[51];
            public double[] Sand = new double[51];
            public double[] Silt = new double[51];
            public double[] Clay = new double[51];
            public double[] Om = new double[51];
            public static int Iwrc;
        }

        private class Rsparm
        {
            public const double Resma = -53.72;
            public const double Resmb = 1.32;
            public const double Resmc = 0.0;
            public const double Restka = 0.007;
        }

        private class Spprop
        {
            public const double G1 = 0.16;
            public const double G2 = 0.0;
            public const double G3 = 110.0;
            public const double Extsp = 1.77;
            public const double Tkspa = 0.021;
            public const double Tkspb = 2.51;
            public const double Tkspex = 2.0;
            public const double Vdifsp = 0.00009;
            public const double Vapspx = 14.0;
        }

        private class Spwatr
        {
            public const double Clag1 = 10.0;
            public const double Clag2 = 1.0;
            public const double Clag3 = 5.0;
            public const double Clag4 = 450.0;
            public const double Plwmax = 0.1;
            public const double Plwden = 200.0;
            public const double Plwhc = 0.03;
            public const double Thick = 0.025;
            public const double Cthick = 0.10;
        }

        private class Metasp
        {
            public const double Cmet1 = 0.01;
            public const double Cmet2 = 21.0;
            public const double Cmet3 = 0.01;
            public const double Cmet4 = 0.04;
            public const double Cmet5 = 2.0;
            public const double Snomax = 150.0;
        }

        private class Clayrs
        {
            public double[] Plantz = new double[9];
            public double[][] Drycan = JaggedArray<double>(9, 11);
            public double[][] Canlai = JaggedArray<double>(9, 11);
            public double[] Totlai = new double[9];
            public int[] Ievap = new int[9];
            public double[][] Rleaf = JaggedArray<double>(9, 11);
            public double[][] Rroot = JaggedArray<double>(9, 51);
            public double[][] Rootdn = JaggedArray<double>(9, 51);
            public double[] Totrot = new double[9];
        }

        private class Measur
        {
            public int Measdy;
            public int Meashr;
            public double[] Vlcmes = new double[51];
            public double[] Tsmeas = new double[51];
        }

        private class Windv
        {
            public double Zh;
            public double Zm;
            public double Zero;
            public double Ustar;
            public double Stable;
            public double Conrh;
            public double Conrv;
            public double Zhlog;
            public double Zmsub;
            public double Zhsub;
            public double Zersub;
            public double[] Windc = new double[12];
            public double[] Windr = new double[11];
        }

        private class Radcan
        {
            public double[] Tdircc = new double[11];
            public double[] Tdiffc = new double[11];
            public double[][] Dirkl = JaggedArray<double>(10, 11);
            public double[][] Difkl = JaggedArray<double>(10, 11);
            public double[] Fbdu = new double[9];
            public double[] Fddu = new double[9];
        }

        private class Radres
        {
            public double[] Tdirec = new double[11];
            public double[] Tdiffu = new double[11];
        }

        private class Timewt
        {
            public double Wt;
            public double Wdt;
            public double Dt;
        }

        private class Matrix
        {
            public double[] A1 = new double[151];
            public double[] B1 = new double[151];
            public double[] C1 = new double[151];
            public double[] D1 = new double[151];
            public double[] A2 = new double[151];
            public double[] B2 = new double[151];
            public double[] C2 = new double[151];
            public double[] D2 = new double[151];
        }

        private class Residu
        {
            public double[] Evap = new double[11];
            public double[] Evapk = new double[11];
        }

        private class Spheat
        {
            public double[] Cs = new double[51];
        }

        private class Writeit
        {
            public double Hflux1;
            public double Rh;
            public double Hnc;
            public double Xlenc;
            public double Contk;
            public double[][] Tleaf = JaggedArray<double>(9, 11);
        }

        private class Canlwr
        {
            public double[][] Tlclwr = JaggedArray<double>(9, 11);
        }
    }
}
