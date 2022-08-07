namespace SPACalculator.Calculators;

public static class EclipticCalc
{
	public static double EclipticMeanObliquity(double jme)
	{
		var u = jme / 10.0;

		return 84381.448 + u * (-4680.93 + u * (-1.55 + u * (1999.25 +
															 u * (-51.38 + u * (-249.67 +
																 u * (-39.05 + u * (7.12 +
																	 u * (27.87 + u * (5.79 + u * 2.45)))))))));
	}

	public static double EclipticTrueObliquity(double deltaEpsilon, double epsilon0)
	{
		return deltaEpsilon + epsilon0 / 3600.0;
	}
}