using SPACalculator.Enums;

namespace SPACalculator;

public static class BaseCalculator
{
	private static double ThirdOrderPolynomial(double a, double b, double c, double d, double x)
	{
		return ((a * x + b) * x + c) * x + d;
	}

	public static void NutationLongitudeAndObliquity(double jce, double[] x, ref double delPsi,
		ref double delEpsilon)
	{
		int i;
		double sumPsi = 0, sumEpsilon = 0;

		for (i = 0; i < Consts.YCount; i++)
		{
			var xyTermSum = DegRadCalculator.DegToRad(XYTermSummation(i, x));
			sumPsi += (Consts.PETerms[i][(int)Term3.TermPsiA] + jce * Consts.PETerms[i][(int)Term3.TermPsiB]) *
			          Math.Sin(xyTermSum);
			sumEpsilon += (Consts.PETerms[i][(int)Term3.TermEpsC] + jce * Consts.PETerms[i][(int)Term3.TermEpsD]) *
			              Math.Cos(xyTermSum);
		}

		delPsi = sumPsi / 36000000.0;
		delEpsilon = sumEpsilon / 36000000.0;
	}

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

	public static double AberrationCorrection(double r)
	{
		return -20.4898 / (3600.0 * r);
	}

	public static double ApparentSunLongitude(double theta, double deltaPsi, double deltaTau)
	{
		return theta + deltaPsi + deltaTau;
	}

	public static double GreenwichMeanSiderealTime(double jd, double jc)
	{
		return Limiters.LimitDegrees(280.46061837 + 360.98564736629 * (jd - 2451545.0) +
		                             jc * jc * (0.000387933 - jc / 38710000.0));
	}

	public static double GreenwichSiderealTime(double nu0, double deltaPsi, double epsilon)
	{
		return nu0 + deltaPsi * Math.Cos(DegRadCalculator.DegToRad(epsilon));
	}

	public static double GeocentricRightAscension(double lamda, double epsilon, double beta)
	{
		var lamdaRad = DegRadCalculator.DegToRad(lamda);
		var epsilonRad = DegRadCalculator.DegToRad(epsilon);

		return Limiters.LimitDegrees(DegRadCalculator.RadToDeg(Math.Atan2(Math.Sin(lamdaRad) * Math.Cos(epsilonRad) -
		                                                                  Math.Tan(DegRadCalculator.DegToRad(beta)) *
		                                                                  Math.Sin(epsilonRad), Math.Cos(lamdaRad))));
	}

	public static double GeocentricDeclination(double beta, double epsilon, double lamda)
	{
		var betaRad = DegRadCalculator.DegToRad(beta);
		var epsilonRad = DegRadCalculator.DegToRad(epsilon);

		return DegRadCalculator.RadToDeg(Math.Asin(Math.Sin(betaRad) * Math.Cos(epsilonRad) +
		                                           Math.Cos(betaRad) * Math.Sin(epsilonRad) *
		                                           Math.Sin(DegRadCalculator.DegToRad(lamda))));
	}

	public static double EarthHeliocentricLatitude(double jme)
	{
		var sum = new double[Consts.LCount];
		int i;

		for (i = 0; i < Consts.BCount; i++)
			sum[i] = EarthPeriodicTermSummation(Consts.BTerms[i], Consts.BSubcount[i], jme);

		return DegRadCalculator.RadToDeg(EarthValues(sum, Consts.BCount, jme));
	}

	public static double EarthRadiusVector(double jme)
	{
		var sum = new double[Consts.RCount];
		int i;

		for (i = 0; i < Consts.RCount; i++)
			sum[i] = EarthPeriodicTermSummation(Consts.RTerms[i], Consts.RSubcount[i], jme);

		return EarthValues(sum, Consts.RCount, jme);
	}

	public static double GeocentricLongitude(double l)
	{
		var theta = l + 180.0;

		if (theta >= 360.0) theta -= 360.0;

		return theta;
	}

	public static double GeocentricLatitude(double b)
	{
		return -b;
	}

	public static double MeanElongationMoonSun(double jce)
	{
		return ThirdOrderPolynomial(1.0 / 189474.0, -0.0019142, 445267.11148, 297.85036, jce);
	}

	public static double MeanAnomalySun(double jce)
	{
		return ThirdOrderPolynomial(-1.0 / 300000.0, -0.0001603, 35999.05034, 357.52772, jce);
	}

	public static double MeanAnomalyMoon(double jce)
	{
		return ThirdOrderPolynomial(1.0 / 56250.0, 0.0086972, 477198.867398, 134.96298, jce);
	}

	public static double ArgumentLatitudeMoon(double jce)
	{
		return ThirdOrderPolynomial(1.0 / 327270.0, -0.0036825, 483202.017538, 93.27191, jce);
	}

	public static double AscendingLongitudeMoon(double jce)
	{
		return ThirdOrderPolynomial(1.0 / 450000.0, 0.0020708, -1934.136261, 125.04452, jce);
	}

	private static double XYTermSummation(int i, double[] x = null)
	{
		x ??= new double[(int)Term2.TermXCount];
		int j;
		double sum = 0;

		for (j = 0; j < (int)Term2.TermYCount; j++)
			sum += x[j] * Consts.YTerms[i][j];

		return sum;
	}

	public static double EarthPeriodicTermSummation(double[][] terms, int count, double jme)
	{
		int i;
		double sum = 0;

		for (i = 0; i < count; i++)
			sum += terms[i][(int)Term1.TermA] *
			       Math.Cos(terms[i][(int)Term1.TermB] + terms[i][(int)Term1.TermC] * jme);

		return sum;
	}

	public static double EarthValues(double[] termSum, int count, double jme)
	{
		int i;
		double sum = 0;

		for (i = 0; i < count; i++)
			sum += termSum[i] * Math.Pow(jme, i);

		sum /= 1.0e8;

		return sum;
	}
}