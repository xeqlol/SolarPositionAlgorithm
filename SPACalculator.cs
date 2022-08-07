using SPACalculator.Calculators;
using SPACalculator.Enums;
using SPACalculator.Models;

namespace SPACalculator;

public static class SPACalculator
{
	private static double ObserverHourAngle(double nu, double longitude, double alphaDeg)
	{
		return Limiters.LimitDegrees(nu + longitude - alphaDeg);
	}

	private static void RightAscensionParallaxAndTopocentricDec(double latitude, double elevation,
		double xi, double h, double delta, out double deltaAlpha, out double deltaPrime)
	{
		var latRad = DegRadCalc.DegToRad(latitude);
		var xiRad = DegRadCalc.DegToRad(xi);
		var hRad = DegRadCalc.DegToRad(h);
		var deltaRad = DegRadCalc.DegToRad(delta);
		var u = Math.Atan(0.99664719 * Math.Tan(latRad));
		var y = 0.99664719 * Math.Sin(u) + elevation * Math.Sin(latRad) / 6378140.0;
		var x = Math.Cos(u) + elevation * Math.Cos(latRad) / 6378140.0;

		var deltaAlphaRad = Math.Atan2(-x * Math.Sin(xiRad) * Math.Sin(hRad),
			Math.Cos(deltaRad) - x * Math.Sin(xiRad) * Math.Cos(hRad));

		deltaPrime = DegRadCalc.RadToDeg(Math.Atan2(
			(Math.Sin(deltaRad) - y * Math.Sin(xiRad)) * Math.Cos(deltaAlphaRad),
			Math.Cos(deltaRad) - x * Math.Sin(xiRad) * Math.Cos(hRad)));

		deltaAlpha = DegRadCalc.RadToDeg(deltaAlphaRad);
	}

	private static double AtmosphericRefractionCorrection(double pressure, double temperature,
		double atmosRefract, double e0)
	{
		double delE = 0;

		if (e0 >= -1 * (Consts.SunRadius + atmosRefract))
			delE = pressure / 1010.0 * (283.0 / (273.0 + temperature)) *
				1.02 / (60.0 * Math.Tan(DegRadCalc.DegToRad(e0 + 10.3 / (e0 + 5.11))));

		return delE;
	}

	private static double SurfaceIncidenceAngle(double zenith, double azimuthAstro, double azmRotation,
		double slope)
	{
		var zenithRad = DegRadCalc.DegToRad(zenith);
		var slopeRad = DegRadCalc.DegToRad(slope);

		return DegRadCalc.RadToDeg(Math.Acos(Math.Cos(zenithRad) * Math.Cos(slopeRad) + Math.Sin(slopeRad) *
			Math.Sin(zenithRad) *
			Math.Cos(DegRadCalc.DegToRad(azimuthAstro - azmRotation))));
	}

	public static int SPACalculate(ref DataModel spa)
	{
		var result = Validators.ValidateInputs(ref spa);

		if (result != 0) return result;

		spa.MidOut.JulianTime.JDay = JulianCalc.JulianDay(spa.Time.Year, spa.Time.Month, spa.Time.Day,
			spa.Time.Hour, spa.Time.Minute, spa.Time.Second, spa.TimeDeltas.DeltaUt1,
			spa.Time.Timezone);

		RTSCalc.CalculateGeocentricSunRightAscensionAndDeclination(ref spa);

		spa.MidOut.H = ObserverHourAngle(spa.MidOut.Nu, spa.Enviroment.Longitude, spa.MidOut.SunMidOut.Alpha);
		spa.MidOut.SunMidOut.Xi = MoonSunCalc.SunEquatorialHorizontalParallax(spa.MidOut.EarthMidOut.RadiusVector);

		RightAscensionParallaxAndTopocentricDec(spa.Enviroment.Latitude, spa.Enviroment.Elevation,
			spa.MidOut.SunMidOut.Xi, spa.MidOut.H,
			spa.MidOut.SunMidOut.Delta,
			out var delAlpha, out var deltaPrime);
		spa.MidOut.SunMidOut.DelAlpha = delAlpha;
		spa.MidOut.SunMidOut.DeltaPrime = deltaPrime;

		spa.MidOut.SunMidOut.AlphaPrime = spa.MidOut.SunMidOut.Alpha + spa.MidOut.SunMidOut.DelAlpha;
		spa.MidOut.HPrime = spa.MidOut.H - spa.MidOut.SunMidOut.DelAlpha;

		spa.MidOut.E0 = TopocentricCalc.TopoElAngle(spa.Enviroment.Latitude,
			spa.MidOut.SunMidOut.DeltaPrime, spa.MidOut.HPrime);
		spa.MidOut.DelE = AtmosphericRefractionCorrection(spa.Enviroment.Pressure,
			spa.Enviroment.Temperature, spa.Enviroment.AtmosRefract, spa.MidOut.E0);
		spa.MidOut.E = spa.MidOut.E0 + spa.MidOut.DelE;

		spa.Output.Zenith = 90 - spa.MidOut.E;
		spa.Output.AzimuthAstro = TopocentricCalc.TopoAzAngleAstro(spa.MidOut.HPrime, spa.Enviroment.Latitude,
			spa.MidOut.SunMidOut.DeltaPrime);
		spa.Output.Azimuth = TopocentricCalc.TopoAzAngle(spa.Output.AzimuthAstro);

		if (spa.Mode == CalculationMode.ZAInc || spa.Mode == CalculationMode.All)
			spa.Output.Incidence = SurfaceIncidenceAngle(spa.Output.Zenith, spa.Output.AzimuthAstro,
				spa.Enviroment.AzmRotation, spa.Enviroment.Slope);

		if (spa.Mode == CalculationMode.ZARts || spa.Mode == CalculationMode.All)
			RTSCalc.CalculateEOTAndSunRiseTransitSet(ref spa);

		return result;
	}
}