using SPACalculator.Enums;

namespace SPACalculator.Calculators;

public static class EarthCalc
{
	public static double EarthHeliocentricLatitude(double jme)
	{
		var sum = new double[Consts.LCount];
		int i;

		for (i = 0; i < Consts.BCount; i++)
			sum[i] = EarthPeriodicTermSummation(Consts.BTerms[i], Consts.BSubcount[i], jme);

		return DegRadCalc.RadToDeg(EarthValues(sum, Consts.BCount, jme));
	}

	public static double EarthRadiusVector(double jme)
	{
		var sum = new double[Consts.RCount];
		int i;

		for (i = 0; i < Consts.RCount; i++)
			sum[i] = EarthPeriodicTermSummation(Consts.RTerms[i], Consts.RSubcount[i], jme);

		return EarthValues(sum, Consts.RCount, jme);
	}

	private static double EarthPeriodicTermSummation(double[][] terms, int count, double jme)
	{
		int i;
		double sum = 0;

		for (i = 0; i < count; i++)
			sum += terms[i][(int)Term1.TermA] *
			       Math.Cos(terms[i][(int)Term1.TermB] + terms[i][(int)Term1.TermC] * jme);

		return sum;
	}

	private static double EarthValues(double[] termSum, int count, double jme)
	{
		int i;
		double sum = 0;

		for (i = 0; i < count; i++)
			sum += termSum[i] * Math.Pow(jme, i);

		sum /= 1.0e8;

		return sum;
	}

	public static double EarthHeliocentricLongitude(double jme)
	{
		var sum = new double[Consts.LCount];
		int i;

		for (i = 0; i < Consts.LCount; i++)
			sum[i] = EarthPeriodicTermSummation(Consts.LTerms[i], Consts.LSubcount[i], jme);

		return Limiters.LimitDegrees(DegRadCalc.RadToDeg(EarthValues(sum, Consts.LCount, jme)));
	}
}