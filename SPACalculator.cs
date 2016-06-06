using System;

namespace SPACalculator
{
    public static class SPACalculator
    {
        public struct SPAData
        {
            //----------------------INPUT VALUES------------------------

            public int Year; // 4-digit year,      valid range: -2000 to 6000, error code: 1
            public int Month; // 2-digit month,         valid range: 1 to  12,  error code: 2
            public int Day; // 2-digit day,           valid range: 1 to  31,  error code: 3
            public int Hour; // Observer local hour,   valid range: 0 to  24,  error code: 4
            public int Minute; // Observer local minute, valid range: 0 to  59,  error code: 5
            public double Second; // Observer local second, valid range: 0 to <60,  error code: 6

            public double DeltaUt1; // Fractional second difference between UTC and UT which is used
            // to adjust UTC for earth's irregular rotation rate and is derived
            // from observation only and is reported in this bulletin:
            // http://maia.usno.navy.mil/ser7/ser7.dat,
            // where delta_ut1 = DUT1
            // valid range: -1 to 1 second (exclusive), error code 17

            public double DeltaT; // Difference between earth rotation time and terrestrial time
            // It is derived from observation only and is reported in this
            // bulletin: http://maia.usno.navy.mil/ser7/ser7.dat,
            // where delta_t = 32.184 + (TAI-UTC) - DUT1
            // valid range: -8000 to 8000 seconds, error code: 7

            public double Timezone; // Observer time zone (negative west of Greenwich)
            // valid range: -18   to   18 hours,   error code: 8

            public double Longitude; // Observer longitude (negative west of Greenwich)
            // valid range: -180  to  180 degrees, error code: 9

            public double Latitude; // Observer latitude (negative south of equator)
            // valid range: -90   to   90 degrees, error code: 10

            public double Elevation; // Observer elevation [meters]
            // valid range: -6500000 or higher meters,    error code: 11

            public double Pressure; // Annual average local pressure [millibars]
            // valid range:    0 to 5000 millibars,       error code: 12

            public double Temperature; // Annual average local temperature [degrees Celsius]
            // valid range: -273 to 6000 degrees Celsius, error code; 13

            public double Slope; // Surface slope (measured from the horizontal plane)
            // valid range: -360 to 360 degrees, error code: 14

            public double AzmRotation; // Surface azimuth rotation (measured from south to projection of
            //     surface normal on horizontal plane, negative east)
            // valid range: -360 to 360 degrees, error code: 15

            public double AtmosRefract; // Atmospheric refraction at sunrise and sunset (0.5667 deg is typical)
            // valid range: -5   to   5 degrees, error code: 16

            public CalculationMode Function; // Switch to choose functions for desired output (from enumeration)

            //-----------------Intermediate OUTPUT VALUES--------------------

            public double Jd; //Julian day
            public double Jc; //Julian century

            public double Jde; //Julian ephemeris day
            public double Jce; //Julian ephemeris century
            public double Jme; //Julian ephemeris millennium

            public double L; //earth heliocentric longitude [degrees]
            public double B; //earth heliocentric latitude [degrees]
            public double R; //earth radius vector [Astronomical Units, AU]

            public double Theta; //geocentric longitude [degrees]
            public double Beta; //geocentric latitude [degrees]

            public double X0; //mean elongation (moon-sun) [degrees]
            public double X1; //mean anomaly (sun) [degrees]
            public double X2; //mean anomaly (moon) [degrees]
            public double X3; //argument latitude (moon) [degrees]
            public double X4; //ascending longitude (moon) [degrees]

            public double DelPsi; //nutation longitude [degrees]
            public double DelEpsilon; //nutation obliquity [degrees]
            public double Epsilon0; //ecliptic mean obliquity [arc seconds]
            public double Epsilon; //ecliptic true obliquity  [degrees]

            public double DelTau; //aberration correction [degrees]
            public double Lamda; //apparent sun longitude [degrees]
            public double Nu0; //Greenwich mean sidereal time [degrees]
            public double Nu; //Greenwich sidereal time [degrees]

            public double Alpha; //geocentric sun right ascension [degrees]
            public double Delta; //geocentric sun declination [degrees]

            public double H; //observer hour angle [degrees]
            public double Xi; //sun equatorial horizontal parallax [degrees]
            public double DelAlpha; //sun right ascension parallax [degrees]
            public double DeltaPrime; //topocentric sun declination [degrees]
            public double AlphaPrime; //topocentric sun right ascension [degrees]
            public double HPrime; //topocentric local hour angle [degrees]

            public double E0; //topocentric elevation angle (uncorrected) [degrees]
            public double DelE; //atmospheric refraction correction [degrees]
            public double E; //topocentric elevation angle (corrected) [degrees]

            public double Eot; //equation of time [minutes]
            public double Srha; //sunrise hour angle [degrees]
            public double Ssha; //sunset hour angle [degrees]
            public double Sta; //sun transit altitude [degrees]

            //---------------------Final OUTPUT VALUES------------------------

            public double Zenith; //topocentric zenith angle [degrees]
            public double AzimuthAstro; //topocentric azimuth angle (westward from south) [for astronomers]

            public double Azimuth;
            //topocentric azimuth angle (eastward from north) [for navigators and solar radiation]

            public double Incidence; //surface incidence angle [degrees]

            public double Suntransit; //local sun transit time (or solar noon) [fractional hour]
            public double Sunrise; //local sunrise time (+/- 30 seconds) [fractional hour]
            public double Sunset; //local sunset time (+/- 30 seconds) [fractional hour]

        }

        public enum CalculationMode
        {
            SPA_ZA, //calculate zenith and azimuth
            SPA_ZA_INC, //calculate zenith, azimuth, and incidence
            SPA_ZA_RTS, //calculate zenith, azimuth, and sun rise/transit/set values
            SPA_ALL, //calculate all SPA output values
        }

        static double Pi = 3.1415926535897932384626433832795028841971;
        static double SunRadius = 0.26667;

        static int L_COUNT = 6;
        static int B_COUNT = 2;
        static int R_COUNT = 5;
        static int Y_COUNT = 63;

        static int L_MAX_SUBCOUNT = 64;
        static int B_MAX_SUBCOUNT = 5;
        static int R_MAX_SUBCOUNT = 40;

        enum TERM1
        {
            TERM_A = 0,
            TERM_B = 1,
            TERM_C = 2,
            TERM_COUNT = 3
        };

        enum TERM2
        {
            TERM_X0 = 0,
            TERM_X1 = 1,
            TERM_X2 = 2,
            TERM_X3 = 3,
            TERM_X4 = 4,
            TERM_X_COUNT = 5,
            TERM_Y_COUNT = 5
        };

        enum TERM3
        {
            TERM_PSI_A = 0,
            TERM_PSI_B = 1,
            TERM_EPS_C = 2,
            TERM_EPS_D = 3,
            TERM_PE_COUNT = 4
        };

        enum TERM4
        {
            JD_MINUS = 0,
            JD_ZERO = 1,
            JD_PLUS = 2,
            JD_COUNT = 3
        };

        enum TERM5
        {
            SUN_TRANSIT = 0,
            SUN_RISE = 1,
            SUN_SET = 2,
            SUN_COUNT = 3
        };

        static int[] l_subcount = { 64, 34, 20, 7, 3, 1 };
        static int[] b_subcount = { 5, 2 };
        static int[] r_subcount = { 40, 10, 6, 2, 1 };

        static double[][][] L_TERMS = new double[][][]
        {
            new double[][]
            {
                new double[] {175347046.0, 0, 0},
                new double[] {3341656.0, 4.6692568, 6283.07585},
                new double[] {34894.0, 4.6261, 12566.1517},
                new double[] {3497.0, 2.7441, 5753.3849},
                new double[] {3418.0, 2.8289, 3.5231},
                new double[] {3136.0, 3.6277, 77713.7715},
                new double[] {2676.0, 4.4181, 7860.4194},
                new double[] {2343.0, 6.1352, 3930.2097},
                new double[] {1324.0, 0.7425, 11506.7698},
                new double[] {1273.0, 2.0371, 529.691},
                new double[] {1199.0, 1.1096, 1577.3435},
                new double[] {990, 5.233, 5884.927},
                new double[] {902, 2.045, 26.298},
                new double[] {857, 3.508, 398.149},
                new double[] {780, 1.179, 5223.694},
                new double[] {753, 2.533, 5507.553},
                new double[] {505, 4.583, 18849.228},
                new double[] {492, 4.205, 775.523},
                new double[] {357, 2.92, 0.067},
                new double[] {317, 5.849, 11790.629},
                new double[] {284, 1.899, 796.298},
                new double[] {271, 0.315, 10977.079},
                new double[] {243, 0.345, 5486.778},
                new double[] {206, 4.806, 2544.314},
                new double[] {205, 1.869, 5573.143},
                new double[] {202, 2.458, 6069.777},
                new double[] {156, 0.833, 213.299},
                new double[] {132, 3.411, 2942.463},
                new double[] {126, 1.083, 20.775},
                new double[] {115, 0.645, 0.98},
                new double[] {103, 0.636, 4694.003},
                new double[] {102, 0.976, 15720.839},
                new double[] {102, 4.267, 7.114},
                new double[] {99, 6.21, 2146.17},
                new double[] {98, 0.68, 155.42},
                new double[] {86, 5.98, 161000.69},
                new double[] {85, 1.3, 6275.96},
                new double[] {85, 3.67, 71430.7},
                new double[] {80, 1.81, 17260.15},
                new double[] {79, 3.04, 12036.46},
                new double[] {75, 1.76, 5088.63},
                new double[] {74, 3.5, 3154.69},
                new double[] {74, 4.68, 801.82},
                new double[] {70, 0.83, 9437.76},
                new double[] {62, 3.98, 8827.39},
                new double[] {61, 1.82, 7084.9},
                new double[] {57, 2.78, 6286.6},
                new double[] {56, 4.39, 14143.5},
                new double[] {56, 3.47, 6279.55},
                new double[] {52, 0.19, 12139.55},
                new double[] {52, 1.33, 1748.02},
                new double[] {51, 0.28, 5856.48},
                new double[] {49, 0.49, 1194.45},
                new double[] {41, 5.37, 8429.24},
                new double[] {41, 2.4, 19651.05},
                new double[] {39, 6.17, 10447.39},
                new double[] {37, 6.04, 10213.29},
                new double[] {37, 2.57, 1059.38},
                new double[] {36, 1.71, 2352.87},
                new double[] {36, 1.78, 6812.77},
                new double[] {33, 0.59, 17789.85},
                new double[] {30, 0.44, 83996.85},
                new double[] {30, 2.74, 1349.87},
                new double[] {25, 3.16, 4690.48}
            },
            new double[][]
            {
                new double[] {628331966747.0, 0, 0},
                new double[] {206059.0, 2.678235, 6283.07585},
                new double[] {4303.0, 2.6351, 12566.1517},
                new double[] {425.0, 1.59, 3.523},
                new double[] {119.0, 5.796, 26.298},
                new double[] {109.0, 2.966, 1577.344},
                new double[] {93, 2.59, 18849.23},
                new double[] {72, 1.14, 529.69},
                new double[] {68, 1.87, 398.15},
                new double[] {67, 4.41, 5507.55},
                new double[] {59, 2.89, 5223.69},
                new double[] {56, 2.17, 155.42},
                new double[] {45, 0.4, 796.3},
                new double[] {36, 0.47, 775.52},
                new double[] {29, 2.65, 7.11},
                new double[] {21, 5.34, 0.98},
                new double[] {19, 1.85, 5486.78},
                new double[] {19, 4.97, 213.3},
                new double[] {17, 2.99, 6275.96},
                new double[] {16, 0.03, 2544.31},
                new double[] {16, 1.43, 2146.17},
                new double[] {15, 1.21, 10977.08},
                new double[] {12, 2.83, 1748.02},
                new double[] {12, 3.26, 5088.63},
                new double[] {12, 5.27, 1194.45},
                new double[] {12, 2.08, 4694},
                new double[] {11, 0.77, 553.57},
                new double[] {10, 1.3, 6286.6},
                new double[] {10, 4.24, 1349.87},
                new double[] {9, 2.7, 242.73},
                new double[] {9, 5.64, 951.72},
                new double[] {8, 5.3, 2352.87},
                new double[] {6, 2.65, 9437.76},
                new double[] {6, 4.67, 4690.48}
            },
            new double[][]
            {
                new double[] {52919.0, 0, 0},
                new double[] {8720.0, 1.0721, 6283.0758},
                new double[] {309.0, 0.867, 12566.152},
                new double[] {27, 0.05, 3.52},
                new double[] {16, 5.19, 26.3},
                new double[] {16, 3.68, 155.42},
                new double[] {10, 0.76, 18849.23},
                new double[] {9, 2.06, 77713.77},
                new double[] {7, 0.83, 775.52},
                new double[] {5, 4.66, 1577.34},
                new double[] {4, 1.03, 7.11},
                new double[] {4, 3.44, 5573.14},
                new double[] {3, 5.14, 796.3},
                new double[] {3, 6.05, 5507.55},
                new double[] {3, 1.19, 242.73},
                new double[] {3, 6.12, 529.69},
                new double[] {3, 0.31, 398.15},
                new double[] {3, 2.28, 553.57},
                new double[] {2, 4.38, 5223.69},
                new double[] {2, 3.75, 0.98}
            },
            new double[][]
            {
                new double[] {289.0, 5.844, 6283.076},
                new double[] {35, 0, 0},
                new double[] {17, 5.49, 12566.15},
                new double[] {3, 5.2, 155.42},
                new double[] {1, 4.72, 3.52},
                new double[] {1, 5.3, 18849.23},
                new double[] {1, 5.97, 242.73}
            },
            new double[][]
            {
                new double[] {114.0, 3.142, 0},
                new double[] {8, 4.13, 6283.08},
                new double[] {1, 3.84, 12566.15}
            },
            new double[][]
            {
                new double[] {1, 3.14, 0}
            }
        };

        static double[][][] B_TERMS = new double[][][]
        {
            new double[][]
            {
                new double[] {280.0, 3.199, 84334.662},
                new double[] {102.0, 5.422, 5507.553},
                new double[] {80, 3.88, 5223.69},
                new double[] {44, 3.7, 2352.87},
                new double[] {32, 4, 1577.34}
            },
            new double[][]
            {
                new double[] {9, 3.9, 5507.55},
                new double[] {6, 1.73, 5223.69}
            }
        };

        static double[][][] R_TERMS = new double[][][]
        {
            new double[][]
            {
                new double[] {100013989.0, 0, 0},
                new double[] {1670700.0, 3.0984635, 6283.07585},
                new double[] {13956.0, 3.05525, 12566.1517},
                new double[] {3084.0, 5.1985, 77713.7715},
                new double[] {1628.0, 1.1739, 5753.3849},
                new double[] {1576.0, 2.8469, 7860.4194},
                new double[] {925.0, 5.453, 11506.77},
                new double[] {542.0, 4.564, 3930.21},
                new double[] {472.0, 3.661, 5884.927},
                new double[] {346.0, 0.964, 5507.553},
                new double[] {329.0, 5.9, 5223.694},
                new double[] {307.0, 0.299, 5573.143},
                new double[] {243.0, 4.273, 11790.629},
                new double[] {212.0, 5.847, 1577.344},
                new double[] {186.0, 5.022, 10977.079},
                new double[] {175.0, 3.012, 18849.228},
                new double[] {110.0, 5.055, 5486.778},
                new double[] {98, 0.89, 6069.78},
                new double[] {86, 5.69, 15720.84},
                new double[] {86, 1.27, 161000.69},
                new double[] {65, 0.27, 17260.15},
                new double[] {63, 0.92, 529.69},
                new double[] {57, 2.01, 83996.85},
                new double[] {56, 5.24, 71430.7},
                new double[] {49, 3.25, 2544.31},
                new double[] {47, 2.58, 775.52},
                new double[] {45, 5.54, 9437.76},
                new double[] {43, 6.01, 6275.96},
                new double[] {39, 5.36, 4694},
                new double[] {38, 2.39, 8827.39},
                new double[] {37, 0.83, 19651.05},
                new double[] {37, 4.9, 12139.55},
                new double[] {36, 1.67, 12036.46},
                new double[] {35, 1.84, 2942.46},
                new double[] {33, 0.24, 7084.9},
                new double[] {32, 0.18, 5088.63},
                new double[] {32, 1.78, 398.15},
                new double[] {28, 1.21, 6286.6},
                new double[] {28, 1.9, 6279.55},
                new double[] {26, 4.59, 10447.39}
            },
            new double[][]
            {
                new double[] {103019.0, 1.10749, 6283.07585},
                new double[] {1721.0, 1.0644, 12566.1517},
                new double[] {702.0, 3.142, 0},
                new double[] {32, 1.02, 18849.23},
                new double[] {31, 2.84, 5507.55},
                new double[] {25, 1.32, 5223.69},
                new double[] {18, 1.42, 1577.34},
                new double[] {10, 5.91, 10977.08},
                new double[] {9, 1.42, 6275.96},
                new double[] {9, 0.27, 5486.78}
            },
            new double[][]
            {
                new double[] {4359.0, 5.7846, 6283.0758},
                new double[] {124.0, 5.579, 12566.152},
                new double[] {12, 3.14, 0},
                new double[] {9, 3.63, 77713.77},
                new double[] {6, 1.87, 5573.14},
                new double[] {3, 5.47, 18849.23}
            },
            new double[][]
            {
                new double[] {145.0, 4.273, 6283.076},
                new double[] {7, 3.92, 12566.15}
            },
            new double[][]
            {
                new double[] {4, 2.56, 6283.08}
            }
        };

        static int[][] Y_TERMS = new int[][]
        {
            new int[] {0, 0, 0, 0, 1},
            new int[] {-2, 0, 0, 2, 2},
            new int[] {0, 0, 0, 2, 2},
            new int[] {0, 0, 0, 0, 2},
            new int[] {0, 1, 0, 0, 0},
            new int[] {0, 0, 1, 0, 0},
            new int[] {-2, 1, 0, 2, 2},
            new int[] {0, 0, 0, 2, 1},
            new int[] {0, 0, 1, 2, 2},
            new int[] {-2, -1, 0, 2, 2},
            new int[] {-2, 0, 1, 0, 0},
            new int[] {-2, 0, 0, 2, 1},
            new int[] {0, 0, -1, 2, 2},
            new int[] {2, 0, 0, 0, 0},
            new int[] {0, 0, 1, 0, 1},
            new int[] {2, 0, -1, 2, 2},
            new int[] {0, 0, -1, 0, 1},
            new int[] {0, 0, 1, 2, 1},
            new int[] {-2, 0, 2, 0, 0},
            new int[] {0, 0, -2, 2, 1},
            new int[] {2, 0, 0, 2, 2},
            new int[] {0, 0, 2, 2, 2},
            new int[] {0, 0, 2, 0, 0},
            new int[] {-2, 0, 1, 2, 2},
            new int[] {0, 0, 0, 2, 0},
            new int[] {-2, 0, 0, 2, 0},
            new int[] {0, 0, -1, 2, 1},
            new int[] {0, 2, 0, 0, 0},
            new int[] {2, 0, -1, 0, 1},
            new int[] {-2, 2, 0, 2, 2},
            new int[] {0, 1, 0, 0, 1},
            new int[] {-2, 0, 1, 0, 1},
            new int[] {0, -1, 0, 0, 1},
            new int[] {0, 0, 2, -2, 0},
            new int[] {2, 0, -1, 2, 1},
            new int[] {2, 0, 1, 2, 2},
            new int[] {0, 1, 0, 2, 2},
            new int[] {-2, 1, 1, 0, 0},
            new int[] {0, -1, 0, 2, 2},
            new int[] {2, 0, 0, 2, 1},
            new int[] {2, 0, 1, 0, 0},
            new int[] {-2, 0, 2, 2, 2},
            new int[] {-2, 0, 1, 2, 1},
            new int[] {2, 0, -2, 0, 1},
            new int[] {2, 0, 0, 0, 1},
            new int[] {0, -1, 1, 0, 0},
            new int[] {-2, -1, 0, 2, 1},
            new int[] {-2, 0, 0, 0, 1},
            new int[] {0, 0, 2, 2, 1},
            new int[] {-2, 0, 2, 0, 1},
            new int[] {-2, 1, 0, 2, 1},
            new int[] {0, 0, 1, -2, 0},
            new int[] {-1, 0, 1, 0, 0},
            new int[] {-2, 1, 0, 0, 0},
            new int[] {1, 0, 0, 0, 0},
            new int[] {0, 0, 1, 2, 0},
            new int[] {0, 0, -2, 2, 2},
            new int[] {-1, -1, 1, 0, 0},
            new int[] {0, 1, 1, 0, 0},
            new int[] {0, -1, 1, 2, 2},
            new int[] {2, -1, -1, 2, 2},
            new int[] {0, 0, 3, 2, 2},
            new int[] {2, -1, 0, 2, 2},
        };

        static double[][] PE_TERMS = new double[][]
        {
            new double[] {-171996, -174.2, 92025, 8.9},
            new double[] {-13187, -1.6, 5736, -3.1},
            new double[] {-2274, -0.2, 977, -0.5},
            new double[] {2062, 0.2, -895, 0.5},
            new double[] {1426, -3.4, 54, -0.1},
            new double[] {712, 0.1, -7, 0},
            new double[] {-517, 1.2, 224, -0.6},
            new double[] {-386, -0.4, 200, 0},
            new double[] {-301, 0, 129, -0.1},
            new double[] {217, -0.5, -95, 0.3},
            new double[] {-158, 0, 0, 0},
            new double[] {129, 0.1, -70, 0},
            new double[] {123, 0, -53, 0},
            new double[] {63, 0, 0, 0},
            new double[] {63, 0.1, -33, 0},
            new double[] {-59, 0, 26, 0},
            new double[] {-58, -0.1, 32, 0},
            new double[] {-51, 0, 27, 0},
            new double[] {48, 0, 0, 0},
            new double[] {46, 0, -24, 0},
            new double[] {-38, 0, 16, 0},
            new double[] {-31, 0, 13, 0},
            new double[] {29, 0, 0, 0},
            new double[] {29, 0, -12, 0},
            new double[] {26, 0, 0, 0},
            new double[] {-22, 0, 0, 0},
            new double[] {21, 0, -10, 0},
            new double[] {17, -0.1, 0, 0},
            new double[] {16, 0, -8, 0},
            new double[] {-16, 0.1, 7, 0},
            new double[] {-15, 0, 9, 0},
            new double[] {-13, 0, 7, 0},
            new double[] {-12, 0, 6, 0},
            new double[] {11, 0, 0, 0},
            new double[] {-10, 0, 5, 0},
            new double[] {-8, 0, 3, 0},
            new double[] {7, 0, -3, 0},
            new double[] {-7, 0, 0, 0},
            new double[] {-7, 0, 3, 0},
            new double[] {-7, 0, 3, 0},
            new double[] {6, 0, 0, 0},
            new double[] {6, 0, -3, 0},
            new double[] {6, 0, -3, 0},
            new double[] {-6, 0, 3, 0},
            new double[] {-6, 0, 3, 0},
            new double[] {5, 0, 0, 0},
            new double[] {-5, 0, 3, 0},
            new double[] {-5, 0, 3, 0},
            new double[] {-5, 0, 3, 0},
            new double[] {4, 0, 0, 0},
            new double[] {4, 0, 0, 0},
            new double[] {4, 0, 0, 0},
            new double[] {-4, 0, 0, 0},
            new double[] {-4, 0, 0, 0},
            new double[] {-4, 0, 0, 0},
            new double[] {3, 0, 0, 0},
            new double[] {-3, 0, 0, 0},
            new double[] {-3, 0, 0, 0},
            new double[] {-3, 0, 0, 0},
            new double[] {-3, 0, 0, 0},
            new double[] {-3, 0, 0, 0},
            new double[] {-3, 0, 0, 0},
            new double[] {-3, 0, 0, 0},
        };

        static double RadToDeg(double radians)
        {
            return (180.0 / Pi) * radians;
        }

        static double DegToRad(double degrees)
        {
            return (Pi / 180.0) * degrees;
        }

        static int Integer(double value)
        {
            return (int)value;
        }

        static double LimitDegrees(double degrees)
        {
            degrees /= 360.0;
            var limited = 360.0 * (degrees - Math.Floor(degrees));
            if (limited < 0) limited += 360.0;

            return limited;
        }

        static double LimitDegrees180pm(double degrees)
        {
            degrees /= 360.0;
            var limited = 360.0 * (degrees - Math.Floor(degrees));
            if (limited < -180.0) limited += 360.0;
            else if (limited > 180.0) limited -= 360.0;

            return limited;
        }

        static double LimitDegrees180(double degrees)
        {
            degrees /= 180.0;
            var limited = 180.0 * (degrees - Math.Floor(degrees));
            if (limited < 0) limited += 180.0;

            return limited;
        }

        static double LimitZeroToOne(double value)
        {
            var limited = value - Math.Floor(value);
            if (limited < 0) limited += 1.0;

            return limited;
        }

        static double LimitMinutes(double minutes)
        {
            double limited = minutes;

            if (limited < -20.0) limited += 1440.0;
            else if (limited > 20.0) limited -= 1440.0;

            return limited;
        }

        static double DayFracToLocalHr(double dayfrac, double timezone)
        {
            return 24.0 * LimitZeroToOne(dayfrac + timezone / 24.0);
        }

        static double ThirdOrderPolynomial(double a, double b, double c, double d, double x)
        {
            return ((a * x + b) * x + c) * x + d;
        }

        public static int ValidateInputs(ref SPAData spa)
        {
            if ((spa.Year < -2000) || (spa.Year > 6000)) return 1;
            if ((spa.Month < 1) || (spa.Month > 12)) return 2;
            if ((spa.Day < 1) || (spa.Day > 31)) return 3;
            if ((spa.Hour < 0) || (spa.Hour > 24)) return 4;
            if ((spa.Minute < 0) || (spa.Minute > 59)) return 5;
            if ((spa.Second < 0) || (spa.Second >= 60)) return 6;
            if ((spa.Pressure < 0) || (spa.Pressure > 5000)) return 12;
            if ((spa.Temperature <= -273) || (spa.Temperature > 6000)) return 13;
            if ((spa.DeltaUt1 <= -1) || (spa.DeltaUt1 >= 1)) return 17;
            if ((spa.Hour == 24) && (spa.Minute > 0)) return 5;
            if ((spa.Hour == 24) && (spa.Second > 0)) return 6;

            if (Math.Abs(spa.DeltaT) > 8000) return 7;
            if (Math.Abs(spa.Timezone) > 18) return 8;
            if (Math.Abs(spa.Longitude) > 180) return 9;
            if (Math.Abs(spa.Latitude) > 90) return 10;
            if (Math.Abs(spa.AtmosRefract) > 5) return 16;
            if (spa.Elevation < -6500000) return 11;

            if ((spa.Function == CalculationMode.SPA_ZA_INC) || (spa.Function == CalculationMode.SPA_ALL))
            {
                if (Math.Abs(spa.Slope) > 360) return 14;
                if (Math.Abs(spa.AzmRotation) > 360) return 15;
            }

            return 0;
        }

        static double JulianDay(int year, int month, int day, int hour, int minute, double second, double dut1,
            double tz)
        {
            var dayDecimal = day + (hour - tz + (minute + (second + dut1) / 60.0) / 60.0) / 24.0;

            if (month < 3)
            {
                month += 12;
                year--;
            }

            var julianDay = Integer(365.25 * (year + 4716.0)) + Integer(30.6001 * (month + 1)) + dayDecimal - 1524.5;

            if (julianDay > 2299160.0)
            {
                double a = Integer(year / 100);
                julianDay += (2 - a + Integer(a / 4));
            }

            return julianDay;
        }

        static double JulianCentury(double jd)
        {
            return (jd - 2451545.0) / 36525.0;
        }

        static double JulianEphemerisDay(double jd, double deltaT)
        {
            return jd + deltaT / 86400.0;
        }

        static double JulianEphemerisCentury(double jde)
        {
            return (jde - 2451545.0) / 36525.0;
        }

        static double JulianEphemerisMillennium(double jce)
        {
            return (jce / 10.0);
        }

        static double EarthPeriodicTermSummation(double[][] terms, int count, double jme)
        {

            int i;
            double sum = 0;

            for (i = 0; i < count; i++)
                sum += terms[i][(int)TERM1.TERM_A] *
                       Math.Cos(terms[i][(int)TERM1.TERM_B] + terms[i][(int)TERM1.TERM_C] * jme);

            return sum;
        }

        static double EarthValues(double[] termSum, int count, double jme)
        {
            int i;
            double sum = 0;

            for (i = 0; i < count; i++)
                sum += termSum[i] * Math.Pow(jme, i);

            sum /= 1.0e8;

            return sum;
        }

        static double EarthHeliocentricLongitude(double jme)
        {
            double[] sum = new double[L_COUNT];
            int i;

            for (i = 0; i < L_COUNT; i++)
                sum[i] = EarthPeriodicTermSummation(L_TERMS[i], l_subcount[i], jme);

            return LimitDegrees(RadToDeg(EarthValues(sum, L_COUNT, jme)));

        }

        static double EarthHeliocentricLatitude(double jme)
        {
            double[] sum = new double[L_COUNT];
            int i;

            for (i = 0; i < B_COUNT; i++)
                sum[i] = EarthPeriodicTermSummation(B_TERMS[i], b_subcount[i], jme);

            return RadToDeg(EarthValues(sum, B_COUNT, jme));

        }

        static double EarthRadiusVector(double jme)
        {
            double[] sum = new double[R_COUNT];
            int i;

            for (i = 0; i < R_COUNT; i++)
                sum[i] = EarthPeriodicTermSummation(R_TERMS[i], r_subcount[i], jme);

            return EarthValues(sum, R_COUNT, jme);

        }

        static double GeocentricLongitude(double l)
        {
            double theta = l + 180.0;

            if (theta >= 360.0) theta -= 360.0;

            return theta;
        }

        static double GeocentricLatitude(double b)
        {
            return -b;
        }

        static double MeanElongationMoonSun(double jce)
        {
            return ThirdOrderPolynomial(1.0 / 189474.0, -0.0019142, 445267.11148, 297.85036, jce);
        }

        static double MeanAnomalySun(double jce)
        {
            return ThirdOrderPolynomial(-1.0 / 300000.0, -0.0001603, 35999.05034, 357.52772, jce);
        }

        static double MeanAnomalyMoon(double jce)
        {
            return ThirdOrderPolynomial(1.0 / 56250.0, 0.0086972, 477198.867398, 134.96298, jce);
        }

        static double ArgumentLatitudeMoon(double jce)
        {
            return ThirdOrderPolynomial(1.0 / 327270.0, -0.0036825, 483202.017538, 93.27191, jce);
        }

        static double AscendingLongitudeMoon(double jce)
        {
            return ThirdOrderPolynomial(1.0 / 450000.0, 0.0020708, -1934.136261, 125.04452, jce);
        }

        static double XYTermSummation(int i, double[] x = null)
        {
            x = x ?? new double[(int)TERM2.TERM_X_COUNT];
            int j;
            double sum = 0;

            for (j = 0; j < (int)TERM2.TERM_Y_COUNT; j++)
                sum += x[j] * Y_TERMS[i][j];

            return sum;
        }

        static void NutationLongitudeAndObliquity(double jce, double[] x, ref double delPsi,
            ref double delEpsilon)
        {
            int i;
            double sumPsi = 0, sumEpsilon = 0;

            for (i = 0; i < Y_COUNT; i++)
            {
                var xyTermSum = DegToRad(XYTermSummation(i, x));
                sumPsi += (PE_TERMS[i][(int)TERM3.TERM_PSI_A] + jce * PE_TERMS[i][(int)TERM3.TERM_PSI_B]) *
                           Math.Sin(xyTermSum);
                sumEpsilon += (PE_TERMS[i][(int)TERM3.TERM_EPS_C] + jce * PE_TERMS[i][(int)TERM3.TERM_EPS_D]) *
                               Math.Cos(xyTermSum);
            }

            delPsi = sumPsi / 36000000.0;
            delEpsilon = sumEpsilon / 36000000.0;
        }

        static double EclipticMeanObliquity(double jme)
        {
            double u = jme / 10.0;

            return 84381.448 + u * (-4680.93 + u * (-1.55 + u * (1999.25 + u * (-51.38 + u * (-249.67 + u * (-39.05 + u * (7.12 + u * (27.87 + u * (5.79 + u * 2.45)))))))));
        }

        static double EclipticTrueObliquity(double deltaEpsilon, double epsilon0)
        {
            return deltaEpsilon + epsilon0 / 3600.0;
        }

        static double AberrationCorrection(double r)
        {
            return -20.4898 / (3600.0 * r);
        }

        static double ApparentSunLongitude(double theta, double deltaPsi, double deltaTau)
        {
            return theta + deltaPsi + deltaTau;
        }

        static double GreenwichMeanSiderealTime(double jd, double jc)
        {
            return LimitDegrees(280.46061837 + 360.98564736629 * (jd - 2451545.0) +
                                 jc * jc * (0.000387933 - jc / 38710000.0));
        }

        static double GreenwichSiderealTime(double nu0, double deltaPsi, double epsilon)
        {
            return nu0 + deltaPsi * Math.Cos(DegToRad(epsilon));
        }

        static double GeocentricRightAscension(double lamda, double epsilon, double beta)
        {
            double lamdaRad = DegToRad(lamda);
            double epsilonRad = DegToRad(epsilon);

            return LimitDegrees(RadToDeg(Math.Atan2(Math.Sin(lamdaRad) * Math.Cos(epsilonRad) -
                                                    Math.Tan(DegToRad(beta)) * Math.Sin(epsilonRad), Math.Cos(lamdaRad))));
        }

        static double GeocentricDeclination(double beta, double epsilon, double lamda)
        {
            double betaRad = DegToRad(beta);
            double epsilonRad = DegToRad(epsilon);

            return RadToDeg(Math.Asin(Math.Sin(betaRad) * Math.Cos(epsilonRad) +
                                     Math.Cos(betaRad) * Math.Sin(epsilonRad) * Math.Sin(DegToRad(lamda))));
        }

        static double ObserverHourAngle(double nu, double longitude, double alphaDeg)
        {
            return LimitDegrees(nu + longitude - alphaDeg);
        }

        static double SunEquatorialHorizontalParallax(double r)
        {
            return 8.794 / (3600.0 * r);
        }

        static void RightAscensionParallaxAndTopocentricDec(double latitude, double elevation,
            double xi, double h, double delta, ref double deltaAlpha, ref double deltaPrime)
        {
            double latRad = DegToRad(latitude);
            double xiRad = DegToRad(xi);
            double hRad = DegToRad(h);
            double deltaRad = DegToRad(delta);
            double u = Math.Atan(0.99664719 * Math.Tan(latRad));
            double y = 0.99664719 * Math.Sin(u) + elevation * Math.Sin(latRad) / 6378140.0;
            double x = Math.Cos(u) + elevation * Math.Cos(latRad) / 6378140.0;

            var deltaAlphaRad = Math.Atan2(-x * Math.Sin(xiRad) * Math.Sin(hRad),
                Math.Cos(deltaRad) - x * Math.Sin(xiRad) * Math.Cos(hRad));

            deltaPrime = RadToDeg(Math.Atan2((Math.Sin(deltaRad) - y * Math.Sin(xiRad)) * Math.Cos(deltaAlphaRad),
                Math.Cos(deltaRad) - x * Math.Sin(xiRad) * Math.Cos(hRad)));

            deltaAlpha = RadToDeg(deltaAlphaRad);
        }

        static double TopocentricRightAscension(double alphaDeg, double deltaAlpha)
        {
            return alphaDeg + deltaAlpha;
        }

        static double TopocentricLocalHourAngle(double h, double deltaAlpha)
        {
            return h - deltaAlpha;
        }

        static double TopocentricElevationAngle(double latitude, double deltaPrime, double hPrime)
        {
            double latRad = DegToRad(latitude);
            double deltaPrimeRad = DegToRad(deltaPrime);

            return RadToDeg(Math.Asin(Math.Sin(latRad) * Math.Sin(deltaPrimeRad) +
                                     Math.Cos(latRad) * Math.Cos(deltaPrimeRad) * Math.Cos(DegToRad(hPrime))));
        }

        static double AtmosphericRefractionCorrection(double pressure, double temperature,
            double atmosRefract, double e0)
        {
            double delE = 0;

            if (e0 >= -1 * (SunRadius + atmosRefract))
                delE = (pressure / 1010.0) * (283.0 / (273.0 + temperature)) *
                        1.02 / (60.0 * Math.Tan(DegToRad(e0 + 10.3 / (e0 + 5.11))));

            return delE;
        }

        static double TopocentricElevationAngleCorrected(double e0, double deltaE)
        {
            return e0 + deltaE;
        }

        static double TopocentricZenithAngle(double e)
        {
            return 90.0 - e;
        }

        static double TopocentricAzimuthAngleAstro(double hPrime, double latitude, double deltaPrime)
        {
            double hPrimeRad = DegToRad(hPrime);
            double latRad = DegToRad(latitude);

            return LimitDegrees(RadToDeg(Math.Atan2(Math.Sin(hPrimeRad), Math.Cos(hPrimeRad) * Math.Sin(latRad) - Math.Tan(DegToRad(deltaPrime)) * Math.Cos(latRad))));
        }

        static double TopocentricAzimuthAngle(double azimuthAstro)
        {
            return LimitDegrees(azimuthAstro + 180.0);
        }

        static double SurfaceIncidenceAngle(double zenith, double azimuthAstro, double azmRotation,
            double slope)
        {
            double zenithRad = DegToRad(zenith);
            double slopeRad = DegToRad(slope);

            return RadToDeg(Math.Acos(Math.Cos(zenithRad) * Math.Cos(slopeRad) + Math.Sin(slopeRad) * Math.Sin(zenithRad) * Math.Cos(DegToRad(azimuthAstro - azmRotation))));
        }

        static double SunMeanLongitude(double jme)
        {
            return LimitDegrees(280.4664567 + jme * (360007.6982779 + jme * (0.03032028 + jme * (1 / 49931.0 + jme * (-1 / 15300.0 + jme * (-1 / 2000000.0))))));
        }

        static double EquationOfTime(double m, double alpha, double delPsi, double epsilon)
        {
            return LimitMinutes(4.0 * (m - 0.0057183 - alpha + delPsi * Math.Cos(DegToRad(epsilon))));
        }

        static double ApproxSunTransitTime(double alphaZero, double longitude, double nu)
        {
            return (alphaZero - longitude - nu) / 360.0;
        }

        static double SunHourAngleAtRiseSet(double latitude, double deltaZero, double h0Prime)
        {
            double h0 = -99999;
            double latitudeRad = DegToRad(latitude);
            double deltaZeroRad = DegToRad(deltaZero);
            double argument = (Math.Sin(DegToRad(h0Prime)) - Math.Sin(latitudeRad) * Math.Sin(deltaZeroRad)) / (Math.Cos(latitudeRad) * Math.Cos(deltaZeroRad));

            if (Math.Abs(argument) <= 1) h0 = LimitDegrees180(RadToDeg(Math.Acos(argument)));

            return h0;
        }

        static void ApproxSunRiseAndSet(ref double[] mRts, double h0)
        {
            double h0Dfrac = h0 / 360.0;

            mRts[(int)TERM5.SUN_RISE] = LimitZeroToOne(mRts[(int)TERM5.SUN_TRANSIT] - h0Dfrac);
            mRts[(int)TERM5.SUN_SET] = LimitZeroToOne(mRts[(int)TERM5.SUN_TRANSIT] + h0Dfrac);
            mRts[(int)TERM5.SUN_TRANSIT] = LimitZeroToOne(mRts[(int)TERM5.SUN_TRANSIT]);
        }

        static double RtsAlphaDeltaPrime(ref double[] ad, double n)
        {
            double a = ad[(int)TERM4.JD_ZERO] - ad[(int)TERM4.JD_MINUS];
            double b = ad[(int)TERM4.JD_PLUS] - ad[(int)TERM4.JD_ZERO];

            if (Math.Abs(a) >= 2.0) a = LimitZeroToOne(a);
            if (Math.Abs(b) >= 2.0) b = LimitZeroToOne(b);

            return ad[(int)TERM4.JD_ZERO] + n * (a + b + (b - a) * n) / 2.0;
        }

        static double RtsSunAltitude(double latitude, double deltaPrime, double hPrime)
        {
            double latitudeRad = DegToRad(latitude);
            double deltaPrimeRad = DegToRad(deltaPrime);

            return RadToDeg(Math.Asin(Math.Sin(latitudeRad) * Math.Sin(deltaPrimeRad) +
                                     Math.Cos(latitudeRad) * Math.Cos(deltaPrimeRad) * Math.Cos(DegToRad(hPrime))));
        }

        static double SunRiseAndSet(ref double[] mRts, ref double[] hRts, ref double[] deltaPrime, double latitude,
            ref double[] hPrime, double h0Prime, int sun)
        {
            return mRts[sun] + (hRts[sun] - h0Prime) /
                   (360.0 * Math.Cos(DegToRad(deltaPrime[sun])) * Math.Cos(DegToRad(latitude)) *
                    Math.Sin(DegToRad(hPrime[sun])));
        }

        static void CalculateGeocentricSunRightAscensionAndDeclination(ref SPAData spa)
        {
            double[] x = new double[(int)TERM2.TERM_X_COUNT];

            spa.Jc = JulianCentury(spa.Jd);

            spa.Jde = JulianEphemerisDay(spa.Jd, spa.DeltaT);
            spa.Jce = JulianEphemerisCentury(spa.Jde);
            spa.Jme = JulianEphemerisMillennium(spa.Jce);

            spa.L = EarthHeliocentricLongitude(spa.Jme);
            spa.B = EarthHeliocentricLatitude(spa.Jme);
            spa.R = EarthRadiusVector(spa.Jme);

            spa.Theta = GeocentricLongitude(spa.L);
            spa.Beta = GeocentricLatitude(spa.B);

            x[(int)TERM2.TERM_X0] = spa.X0 = MeanElongationMoonSun(spa.Jce);
            x[(int)TERM2.TERM_X1] = spa.X1 = MeanAnomalySun(spa.Jce);
            x[(int)TERM2.TERM_X2] = spa.X2 = MeanAnomalyMoon(spa.Jce);
            x[(int)TERM2.TERM_X3] = spa.X3 = ArgumentLatitudeMoon(spa.Jce);
            x[(int)TERM2.TERM_X4] = spa.X4 = AscendingLongitudeMoon(spa.Jce);

            NutationLongitudeAndObliquity(spa.Jce, x, ref spa.DelPsi, ref spa.DelEpsilon);

            spa.Epsilon0 = EclipticMeanObliquity(spa.Jme);
            spa.Epsilon = EclipticTrueObliquity(spa.DelEpsilon, spa.Epsilon0);

            spa.DelTau = AberrationCorrection(spa.R);
            spa.Lamda = ApparentSunLongitude(spa.Theta, spa.DelPsi, spa.DelTau);
            spa.Nu0 = GreenwichMeanSiderealTime(spa.Jd, spa.Jc);
            spa.Nu = GreenwichSiderealTime(spa.Nu0, spa.DelPsi, spa.Epsilon);

            spa.Alpha = GeocentricRightAscension(spa.Lamda, spa.Epsilon, spa.Beta);
            spa.Delta = GeocentricDeclination(spa.Beta, spa.Epsilon, spa.Lamda);
        }

        static void CalculateEOTAndSunRiseTransitSet(ref SPAData spa)
        {
            double[] alpha = new double[(int)TERM4.JD_COUNT], delta = new double[(int)TERM4.JD_COUNT];
            double[] mRts = new double[(int)TERM5.SUN_COUNT],
                nuRts = new double[(int)TERM5.SUN_COUNT],
                hRts = new double[(int)TERM5.SUN_COUNT];
            double[] alphaPrime = new double[(int)TERM5.SUN_COUNT],
                deltaPrime = new double[(int)TERM5.SUN_COUNT],
                hPrime = new double[(int)TERM5.SUN_COUNT];
            double h0Prime = -1 * (SunRadius + spa.AtmosRefract);
            int i;

            SPAData sunRts = spa;

            var m = SunMeanLongitude(spa.Jme);
            spa.Eot = EquationOfTime(m, spa.Alpha, spa.DelPsi, spa.Epsilon);

            sunRts.Hour = sunRts.Minute = 0;
            sunRts.Second = 0;
            sunRts.DeltaUt1 = sunRts.Timezone = 0.0;

            sunRts.Jd = JulianDay(sunRts.Year, sunRts.Month, sunRts.Day, sunRts.Hour,
                sunRts.Minute, sunRts.Second, sunRts.DeltaUt1, sunRts.Timezone);

            CalculateGeocentricSunRightAscensionAndDeclination(ref sunRts);
            var nu = sunRts.Nu;

            sunRts.DeltaT = 0;
            sunRts.Jd--;

            for (i = 0; i < (int)TERM4.JD_COUNT; i++)
            {
                CalculateGeocentricSunRightAscensionAndDeclination(ref sunRts);
                alpha[i] = sunRts.Alpha;
                delta[i] = sunRts.Delta;
                sunRts.Jd++;
            }

            mRts[(int)TERM5.SUN_TRANSIT] = ApproxSunTransitTime(alpha[(int)TERM4.JD_ZERO], spa.Longitude, nu);
            var h0 = SunHourAngleAtRiseSet(spa.Latitude, delta[(int)TERM4.JD_ZERO], h0Prime);

            if (h0 >= 0)
            {
                ApproxSunRiseAndSet(ref mRts, h0);

                for (i = 0; i < (int)TERM5.SUN_COUNT; i++)
                {

                    nuRts[i] = nu + 360.985647 * mRts[i];

                    var n = mRts[i] + spa.DeltaT / 86400.0;
                    alphaPrime[i] = RtsAlphaDeltaPrime(ref alpha, n);
                    deltaPrime[i] = RtsAlphaDeltaPrime(ref delta, n);

                    hPrime[i] = LimitDegrees180pm(nuRts[i] + spa.Longitude - alphaPrime[i]);

                    hRts[i] = RtsSunAltitude(spa.Latitude, deltaPrime[i], hPrime[i]);
                }

                spa.Srha = hPrime[(int)TERM5.SUN_RISE];
                spa.Ssha = hPrime[(int)TERM5.SUN_SET];
                spa.Sta = hRts[(int)TERM5.SUN_TRANSIT];

                spa.Suntransit =
                    DayFracToLocalHr(mRts[(int)TERM5.SUN_TRANSIT] - hPrime[(int)TERM5.SUN_TRANSIT] / 360.0,
                        spa.Timezone);

                spa.Sunrise = DayFracToLocalHr(SunRiseAndSet(ref mRts, ref hRts, ref deltaPrime,
                    spa.Latitude, ref hPrime, h0Prime, (int)TERM5.SUN_RISE), spa.Timezone);

                spa.Sunset = DayFracToLocalHr(SunRiseAndSet(ref mRts, ref hRts, ref deltaPrime,
                    spa.Latitude, ref hPrime, h0Prime, (int)TERM5.SUN_SET), spa.Timezone);

            }
            else spa.Srha = spa.Ssha = spa.Sta = spa.Suntransit = spa.Sunrise = spa.Sunset = -99999;
        }

        public static int SPACalculate(ref SPAData spa)
        {
            var result = ValidateInputs(ref spa);

            if (result == 0)
            {
                spa.Jd = JulianDay(spa.Year, spa.Month, spa.Day, spa.Hour,
                    spa.Minute, spa.Second, spa.DeltaUt1, spa.Timezone);

                CalculateGeocentricSunRightAscensionAndDeclination(ref spa);

                spa.H = ObserverHourAngle(spa.Nu, spa.Longitude, spa.Alpha);
                spa.Xi = SunEquatorialHorizontalParallax(spa.R);

                RightAscensionParallaxAndTopocentricDec(spa.Latitude, spa.Elevation, spa.Xi,
                    spa.H, spa.Delta, ref spa.DelAlpha, ref spa.DeltaPrime);

                spa.AlphaPrime = TopocentricRightAscension(spa.Alpha, spa.DelAlpha);
                spa.HPrime = TopocentricLocalHourAngle(spa.H, spa.DelAlpha);

                spa.E0 = TopocentricElevationAngle(spa.Latitude, spa.DeltaPrime, spa.HPrime);
                spa.DelE = AtmosphericRefractionCorrection(spa.Pressure, spa.Temperature,
                    spa.AtmosRefract, spa.E0);
                spa.E = TopocentricElevationAngleCorrected(spa.E0, spa.DelE);

                spa.Zenith = TopocentricZenithAngle(spa.E);
                spa.AzimuthAstro = TopocentricAzimuthAngleAstro(spa.HPrime, spa.Latitude,
                    spa.DeltaPrime);
                spa.Azimuth = TopocentricAzimuthAngle(spa.AzimuthAstro);

                if ((spa.Function == CalculationMode.SPA_ZA_INC) || (spa.Function == CalculationMode.SPA_ALL))
                    spa.Incidence = SurfaceIncidenceAngle(spa.Zenith, spa.AzimuthAstro,
                        spa.AzmRotation, spa.Slope);

                if ((spa.Function == CalculationMode.SPA_ZA_RTS) || (spa.Function == CalculationMode.SPA_ALL))
                    CalculateEOTAndSunRiseTransitSet(ref spa);
            }

            return result;
        }
    }
}
