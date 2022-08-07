using SPACalculator.Enums;
using SPACalculator.Models;

namespace SPACalculator;

public static class SPACalculator
{
	public static double EarthHeliocentricLongitude(double jme)
	{
		var sum = new double[Consts.LCount];
		int i;

		for (i = 0; i < Consts.LCount; i++)
			sum[i] = BaseCalculator.EarthPeriodicTermSummation(Consts.LTerms[i], Consts.LSubcount[i], jme);

		return Limiters.LimitDegrees(DegRadCalculator.RadToDeg(BaseCalculator.EarthValues(sum, Consts.LCount, jme)));
	}

	private static double ObserverHourAngle(double nu, double longitude, double alphaDeg)
	{
		return Limiters.LimitDegrees(nu + longitude - alphaDeg);
	}

	private static double SunEquatorialHorizontalParallax(double r)
	{
		return 8.794 / (3600.0 * r);
	}

	private static void RightAscensionParallaxAndTopocentricDec(double latitude, double elevation,
		double xi, double h, double delta, ref double deltaAlpha, ref double deltaPrime)
	{
		var latRad = DegRadCalculator.DegToRad(latitude);
		var xiRad = DegRadCalculator.DegToRad(xi);
		var hRad = DegRadCalculator.DegToRad(h);
		var deltaRad = DegRadCalculator.DegToRad(delta);
		var u = Math.Atan(0.99664719 * Math.Tan(latRad));
		var y = 0.99664719 * Math.Sin(u) + elevation * Math.Sin(latRad) / 6378140.0;
		var x = Math.Cos(u) + elevation * Math.Cos(latRad) / 6378140.0;

		var deltaAlphaRad = Math.Atan2(-x * Math.Sin(xiRad) * Math.Sin(hRad),
			Math.Cos(deltaRad) - x * Math.Sin(xiRad) * Math.Cos(hRad));

		deltaPrime = DegRadCalculator.RadToDeg(Math.Atan2((Math.Sin(deltaRad) - y * Math.Sin(xiRad)) * Math.Cos(deltaAlphaRad),
			Math.Cos(deltaRad) - x * Math.Sin(xiRad) * Math.Cos(hRad)));

		deltaAlpha = DegRadCalculator.RadToDeg(deltaAlphaRad);
	}

	private static double TopocentricRightAscension(double alphaDeg, double deltaAlpha)
	{
		return alphaDeg + deltaAlpha;
	}

	private static double TopocentricLocalHourAngle(double h, double deltaAlpha)
	{
		return h - deltaAlpha;
	}

	private static double TopocentricElevationAngle(double latitude, double deltaPrime, double hPrime)
	{
		var latRad = DegRadCalculator.DegToRad(latitude);
		var deltaPrimeRad = DegRadCalculator.DegToRad(deltaPrime);

		return DegRadCalculator.RadToDeg(Math.Asin(Math.Sin(latRad) * Math.Sin(deltaPrimeRad) +
		                                           Math.Cos(latRad) * Math.Cos(deltaPrimeRad) * Math.Cos(DegRadCalculator.DegToRad(hPrime))));
	}

	private static double AtmosphericRefractionCorrection(double pressure, double temperature,
		double atmosRefract, double e0)
	{
		double delE = 0;

		if (e0 >= -1 * (Consts.SunRadius + atmosRefract))
			delE = pressure / 1010.0 * (283.0 / (273.0 + temperature)) *
				1.02 / (60.0 * Math.Tan(DegRadCalculator.DegToRad(e0 + 10.3 / (e0 + 5.11))));

		return delE;
	}

	private static double TopocentricElevationAngleCorrected(double e0, double deltaE)
	{
		return e0 + deltaE;
	}

	private static double TopocentricZenithAngle(double e)
	{
		return 90.0 - e;
	}

	private static double TopocentricAzimuthAngleAstro(double hPrime, double latitude, double deltaPrime)
	{
		var hPrimeRad = DegRadCalculator.DegToRad(hPrime);
		var latRad = DegRadCalculator.DegToRad(latitude);

		return Limiters.LimitDegrees(DegRadCalculator.RadToDeg(Math.Atan2(Math.Sin(hPrimeRad),
			Math.Cos(hPrimeRad) * Math.Sin(latRad) - Math.Tan(DegRadCalculator.DegToRad(deltaPrime)) * Math.Cos(latRad))));
	}

	private static double TopocentricAzimuthAngle(double azimuthAstro)
	{
		return Limiters.LimitDegrees(azimuthAstro + 180.0);
	}

	private static double SurfaceIncidenceAngle(double zenith, double azimuthAstro, double azmRotation,
		double slope)
	{
		var zenithRad = DegRadCalculator.DegToRad(zenith);
		var slopeRad = DegRadCalculator.DegToRad(slope);

		return DegRadCalculator.RadToDeg(Math.Acos(Math.Cos(zenithRad) * Math.Cos(slopeRad) + Math.Sin(slopeRad) * Math.Sin(zenithRad) *
			Math.Cos(DegRadCalculator.DegToRad(azimuthAstro - azmRotation))));
	}

	public static int SPACalculate(ref DataModel spa)
	{
		var result = Validators.ValidateInputs(ref spa);

		if (result != 0) return result;

		spa.IntermediateOutput.JulianTimeModel.JDay = JulianCalculator.JulianDay(spa.Time.Year, spa.Time.Month, spa.Time.Day, spa.Time.Hour,
			spa.Time.Minute, spa.Time.Second, spa.TimeDeltas.DeltaUt1, spa.Time.Timezone);

		RTSCalculator.CalculateGeocentricSunRightAscensionAndDeclination(ref spa);

		spa.IntermediateOutput.H = ObserverHourAngle(spa.IntermediateOutput.Nu, spa.Enviroment.Longitude, spa.IntermediateOutput.Alpha);
		spa.IntermediateOutput.Xi = SunEquatorialHorizontalParallax(spa.IntermediateOutput.EarthIntermediateModel.R);

		RightAscensionParallaxAndTopocentricDec(spa.Enviroment.Latitude, spa.Enviroment.Elevation, spa.IntermediateOutput.Xi,
			spa.IntermediateOutput.H, spa.IntermediateOutput.Delta, ref spa.IntermediateOutput.DelAlpha, ref spa.IntermediateOutput.DeltaPrime);

		spa.IntermediateOutput.AlphaPrime = TopocentricRightAscension(spa.IntermediateOutput.Alpha, spa.IntermediateOutput.DelAlpha);
		spa.IntermediateOutput.HPrime = TopocentricLocalHourAngle(spa.IntermediateOutput.H, spa.IntermediateOutput.DelAlpha);

		spa.IntermediateOutput.E0 = TopocentricElevationAngle(spa.Enviroment.Latitude, spa.IntermediateOutput.DeltaPrime, spa.IntermediateOutput.HPrime);
		spa.IntermediateOutput.DelE = AtmosphericRefractionCorrection(spa.Enviroment.Pressure, spa.Enviroment.Temperature,
			spa.Enviroment.AtmosRefract, spa.IntermediateOutput.E0);
		spa.IntermediateOutput.E = TopocentricElevationAngleCorrected(spa.IntermediateOutput.E0, spa.IntermediateOutput.DelE);

		spa.Output.Zenith = TopocentricZenithAngle(spa.IntermediateOutput.E);
		spa.Output.AzimuthAstro = TopocentricAzimuthAngleAstro(spa.IntermediateOutput.HPrime, spa.Enviroment.Latitude,
			spa.IntermediateOutput.DeltaPrime);
		spa.Output.Azimuth = TopocentricAzimuthAngle(spa.Output.AzimuthAstro);

		if (spa.Mode == CalculationMode.ZAInc || spa.Mode == CalculationMode.All)
			spa.Output.Incidence = SurfaceIncidenceAngle(spa.Output.Zenith, spa.Output.AzimuthAstro,
				spa.Enviroment.AzmRotation, spa.Enviroment.Slope);

		if (spa.Mode == CalculationMode.ZARts || spa.Mode == CalculationMode.All) RTSCalculator.CalculateEOTAndSunRiseTransitSet(ref spa);

		return result;
	}
}