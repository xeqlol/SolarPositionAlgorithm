namespace SPACalculator.Calculators;

public static class TopocentricCalc
{
	public static double TopoAzAngleAstro(double hPrime, double latitude, double deltaPrime)
	{
		var hPrimeRad = DegRadCalc.DegToRad(hPrime);
		var latRad = DegRadCalc.DegToRad(latitude);
		var deltaPrimeRad = DegRadCalc.DegToRad(deltaPrime);

		var y = Math.Cos(hPrimeRad) * Math.Sin(latRad) -
				Math.Tan(deltaPrimeRad) * Math.Cos(latRad);
		var o = Math.Atan2(Math.Sin(hPrimeRad), y);

		return Limiters.LimitDegrees(DegRadCalc.RadToDeg(o));
	}

	public static double TopoAzAngle(double azimuthAstro)
	{
		return Limiters.LimitDegrees(azimuthAstro + 180.0);
	}

	public static double TopoElAngle(double latitude, double deltaPrime, double hPrime)
	{
		var latRad = DegRadCalc.DegToRad(latitude);
		var deltaPrimeRad = DegRadCalc.DegToRad(deltaPrime);
		var hPrimeRad = DegRadCalc.DegToRad(hPrime);

		var sins = Math.Sin(latRad) * Math.Sin(deltaPrimeRad);
		var coses = Math.Cos(latRad) * Math.Cos(deltaPrimeRad);
		var x = sins + coses * Math.Cos(hPrimeRad);

		return DegRadCalc.RadToDeg(Math.Asin(x));
	}
}