namespace SPACalculator.Calculators;

public static class MoonSunCalc
{
	public static double MeanElongationMoonSun(double jce)
	{
		return BaseCalc.ThirdOrderPolynomial(1.0 / 189474.0, -0.0019142,
			445267.11148, 297.85036, jce);
	}

	public static double MeanAnomalySun(double jce)
	{
		return BaseCalc.ThirdOrderPolynomial(-1.0 / 300000.0, -0.0001603,
			35999.05034, 357.52772, jce);
	}

	public static double MeanAnomalyMoon(double jce)
	{
		return BaseCalc.ThirdOrderPolynomial(1.0 / 56250.0, 0.0086972,
			477198.867398, 134.96298, jce);
	}

	public static double ArgumentLatitudeMoon(double jce)
	{
		return BaseCalc.ThirdOrderPolynomial(1.0 / 327270.0, -0.0036825,
			483202.017538, 93.27191, jce);
	}

	public static double AscendingLongitudeMoon(double jce)
	{
		return BaseCalc.ThirdOrderPolynomial(1.0 / 450000.0, 0.0020708,
			-1934.136261, 125.04452, jce);
	}

	public static double SunEquatorialHorizontalParallax(double r)
	{
		return 8.794 / (3600.0 * r);
	}
}