# SolarPositionAlgorithm
**NREL Solar Position Algorithm implemented in C#**. This algorithm calculates the solar zenith and azimuth angles in the period from the year -2000 to 6000, with uncertainties of +/- 0.0003 degrees based on the date, time, and location on Earth. (Reference: Reda, I.; Andreas, A., Solar Position Algorithm for Solar Radiation Applications, Solar Energy. Vol. 76(5), 2004; pp. 577-589). The software has not been tested on a variety of platforms and is not guaranteed to work on yours. It is provided here as a convenience.
Further information on this algorithm is available in the following NREL technical report: 

[Reda, I.; Andreas, A. (2003). Solar Position Algorithm for Solar Radiation Applications. 55 pp.; NREL Report No. TP-560-34302, Revised January 2008.](http://www.nrel.gov/docs/fy08osti/34302.pdf)

Original code in C/C++ and licence can be found [here](http://www.nrel.gov/midc/spa/).

Example of use:
```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPACalculator
{
    class Program
    {
        static void Main()
        {
            SPACalculus.SPAData spa = new SPACalculus.SPAData();

            spa.Year = 2003;
            spa.Month = 10;
            spa.Day = 17;
            spa.Hour = 12;
            spa.Minute = 30;
            spa.Second = 30;
            spa.Timezone = -7.0;
            spa.DeltaUt1 = 0;
            spa.DeltaT = 67;
            spa.Longitude = -105.1786;
            spa.Latitude = 39.742476;
            spa.Elevation = 1830.14;
            spa.Pressure = 820;
            spa.Temperature = 11;
            spa.Slope = 30;
            spa.AzmRotation = -10;
            spa.AtmosRefract = 0.5667;
            spa.Function = SPACalculus.CalculationMode.SPA_ALL;

            var result = SPACalculus.SPACalculate(ref spa);

            // Check for SPA errors
            if (result == 0)
            {
                Console.Write("Julian Day:    {0}\n", spa.Jd);
                Console.Write("L:             {0} degrees\n", spa.L);
                Console.Write("B:             {0} degrees\n", spa.B);
                Console.Write("R:             {0} AU\n", spa.R);
                Console.Write("H:             {0} degrees\n", spa.H);
                Console.Write("Delta Psi:     {0} degrees\n", spa.DelPsi);
                Console.Write("Delta Epsilon: {0} degrees\n", spa.DelEpsilon);
                Console.Write("Epsilon:       {0} degrees\n", spa.Epsilon);
                Console.Write("Zenith:        {0} degrees\n", spa.Zenith);
                Console.Write("Azimuth:       {0} degrees\n", spa.Azimuth);
                Console.Write("Incidence:     {0} degrees\n", spa.Incidence);

                var min = 60.0 * (spa.Sunrise - (int)(spa.Sunrise));
                var sec = 60.0 * (min - (int)min);
                Console.Write("Sunrise:       {0}:{1}:{2} Local Time\n", (int)(spa.Sunrise), (int)min, (int)sec);

                min = 60.0 * (spa.Sunset - (int)(spa.Sunset));
                sec = 60.0 * (min - (int)min);
                Console.Write("Sunset:        {0}:{1}:{2} Local Time\n", (int)(spa.Sunset), (int)min, (int)sec);

            }
            else Console.WriteLine("SPA Error Code: {0}", result);

            Console.ReadKey();
        }
    }
}

```
This code should output the following result:
Julian Day:    2452930.31284722
L:             24.0182616916794 degrees
B:             -0.000101121924800342 degrees
R:             0.996542297353971 AU
H:             11.1059020139134 degrees
Delta Psi:     -0.00399840430333278 degrees
Delta Epsilon: 0.00166656817724969 degrees
Epsilon:       23.4404645196175 degrees
Zenith:        50.1116220240297 degrees
Azimuth:       194.340240510192 degrees
Incidence:     25.1870002003532 degrees
Sunrise:       6:12:43 Local Time
Sunset:        17:20:19 Local Time
