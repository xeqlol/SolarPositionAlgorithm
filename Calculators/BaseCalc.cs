using SPACalculator.Enums;

namespace SPACalculator.Calculators;

public static class BaseCalc
{
	public static double ThirdOrderPolynomial(double a, double b, double c, double d, double x)
	{
		return ((a * x + b) * x + c) * x + d;
	}

	public static void NutationLongitudeAndObliquity(double jce, double[] x, out double delPsi,
		out double delEpsilon)
	{
		double sumPsi = 0, sumEpsilon = 0;

		for (var i = 0; i < Consts.YCount; i++)
		{
			var xyTermSum = DegRadCalc.DegToRad(XYTermSummation(i, x));

			sumPsi += (Consts.PETerms[i][(int)Term3.TermPsiA] + jce *
				Consts.PETerms[i][(int)Term3.TermPsiB]) * Math.Sin(xyTermSum);

			sumEpsilon += (Consts.PETerms[i][(int)Term3.TermEpsC] + jce *
				Consts.PETerms[i][(int)Term3.TermEpsD]) * Math.Cos(xyTermSum);
		}

		delPsi = sumPsi / 36000000.0;
		delEpsilon = sumEpsilon / 36000000.0;
	}

	public static double AberrationCorrection(double r)
	{
		return -20.4898 / (3600.0 * r);
	}

	public static double ApparentSunLongitude(double theta, double deltaPsi, double deltaTau)
	{
		return theta + deltaPsi + deltaTau;
	}

	private static double XYTermSummation(int i, double[] x = null)
	{
		x ??= new double[(int)Term2.TermXCount];
		double sum = 0;

		for (var j = 0; j < (int)Term2.TermYCount; j++)
			sum += x[j] * Consts.YTerms[i][j];

		return sum;
	}
}