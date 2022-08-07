namespace SPACalculator.Calculators;

public static class GeocentricCalc
{
	public static double GeoRightAscension(double lamda, double epsilon, double beta)
	{
		var lamdaRad = DegRadCalc.DegToRad(lamda);
		var epsilonRad = DegRadCalc.DegToRad(epsilon);
		var betaRad = DegRadCalc.DegToRad(beta);

		var y = Math.Sin(lamdaRad) * Math.Cos(epsilonRad) -
				Math.Tan(betaRad) * Math.Sin(epsilonRad);
		var o = Math.Atan2(y, Math.Cos(lamdaRad));

		return Limiters.LimitDegrees(DegRadCalc.RadToDeg(o));
	}

	public static double GeoDeclination(double beta, double epsilon, double lamda)
	{
		var betaRad = DegRadCalc.DegToRad(beta);
		var epsilonRad = DegRadCalc.DegToRad(epsilon);
		var lamdaRad = DegRadCalc.DegToRad(lamda);

		var d = Math.Sin(betaRad) * Math.Cos(epsilonRad) +
				Math.Cos(betaRad) * Math.Sin(epsilonRad) * Math.Sin(lamdaRad);
		var o = Math.Asin(d);

		return DegRadCalc.RadToDeg(o);
	}

	public static double GeoLongitude(double l)
	{
		var theta = l + 180.0;

		if (theta >= 360.0) theta -= 360.0;

		return theta;
	}
}