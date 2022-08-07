namespace SPACalculator.Calculators;

public static class DegRadCalc
{
	private const double RadToDegFactor = 180 / Consts.Pi;
	private const double DegToRadFactor = Consts.Pi / 180;

	public static double RadToDeg(double radians) => RadToDegFactor * radians;
	public static double DegToRad(double degrees) => DegToRadFactor * degrees;
}