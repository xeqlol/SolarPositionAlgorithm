namespace SPACalculator.Calculators;

public static class GreenwichCalc
{
	public static double GreenwichMeanSiderealTime(double jd, double jc)
	{
		return Limiters.LimitDegrees(280.46061837 + 360.98564736629 * (jd - 2451545.0) +
									 jc * jc * (0.000387933 - jc / 38710000.0));
	}

	public static double GreenwichSiderealTime(double nu0, double deltaPsi, double epsilon)
	{
		return nu0 + deltaPsi * Math.Cos(DegRadCalc.DegToRad(epsilon));
	}
}