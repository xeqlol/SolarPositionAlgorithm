using SPACalculator.Enums;
using SPACalculator.Models;

namespace SPACalculator;

public static class Validators
{
	public static int ValidateInputs(ref DataModel spa)
	{
		if (spa.Time.Year < -2000 || spa.Time.Year > 6000) return 1;
		if (spa.Time.Month < 1 || spa.Time.Month > 12) return 2;
		if (spa.Time.Day < 1 || spa.Time.Day > 31) return 3;
		if (spa.Time.Hour < 0 || spa.Time.Hour > 24) return 4;
		if (spa.Time.Minute < 0 || spa.Time.Minute > 59) return 5;
		if (spa.Time.Second < 0 || spa.Time.Second >= 60) return 6;
		if (spa.Enviroment.Pressure < 0 || spa.Enviroment.Pressure > 5000) return 12;
		if (spa.Enviroment.Temperature <= -273 || spa.Enviroment.Temperature > 6000) return 13;
		if (spa.TimeDeltas.DeltaUt1 <= -1 || spa.TimeDeltas.DeltaUt1 >= 1) return 17;
		if (spa.Time.Hour == 24 && spa.Time.Minute > 0) return 5;
		if (spa.Time.Hour == 24 && spa.Time.Second > 0) return 6;

		if (Math.Abs(spa.TimeDeltas.DeltaT) > 8000) return 7;
		if (Math.Abs(spa.Time.Timezone) > 18) return 8;
		if (Math.Abs(spa.Enviroment.Longitude) > 180) return 9;
		if (Math.Abs(spa.Enviroment.Latitude) > 90) return 10;
		if (Math.Abs(spa.Enviroment.AtmosRefract) > 5) return 16;
		if (spa.Enviroment.Elevation < -6500000) return 11;

		if (spa.Mode != CalculationMode.ZAInc && spa.Mode != CalculationMode.All) return 0;

		if (Math.Abs(spa.Enviroment.Slope) > 360) return 14;
		if (Math.Abs(spa.Enviroment.AzmRotation) > 360) return 15;

		return 0;
	}
}