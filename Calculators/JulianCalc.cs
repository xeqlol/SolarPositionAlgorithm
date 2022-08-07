namespace SPACalculator.Calculators;

public static class JulianCalc
{
	public static double JulianDay(int year, int month, int day, int hour, int minute,
		double second, double dut1, double tz)
	{
		if (month < 3)
		{
			month += 12;
			year--;
		}

		var dayDecimal = day + (hour - tz + (minute + (second + dut1) / 60.0) / 60.0) / 24.0;

		var julianDay = (int)(365.25 * (year + 4716.0)) + (int)(30.6001 * (month + 1)) + dayDecimal - 1524.5;

		if (!(julianDay > 2299160.0)) return julianDay;

		// ReSharper disable once RedundantCast
		var a = year / 100.0;
		julianDay += 2 - (int)a + (int)(a / 4);

		return julianDay;
	}

	public static double JulianCentury(double jd)
	{
		return (jd - 2451545.0) / 36525.0;
	}

	public static double JulianEphemerisDay(double jd, double deltaT)
	{
		return jd + deltaT / 86400.0;
	}

	public static double JulianEphemerisCentury(double jde)
	{
		return (jde - 2451545.0) / 36525.0;
	}

	public static double JulianEphemerisMillennium(double jce)
	{
		return jce / 10.0;
	}
}