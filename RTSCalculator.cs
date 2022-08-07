using SPACalculator.Enums;
using SPACalculator.Models;

namespace SPACalculator;

public static class RTSCalculator
{
	public static void CalculateEOTAndSunRiseTransitSet(ref DataModel spa)
	{
		double[] alpha = new double[(int)Term4.JDCount], delta = new double[(int)Term4.JDCount];
		double[] mRts = new double[(int)Term5.SunCount],
			nuRts = new double[(int)Term5.SunCount],
			hRts = new double[(int)Term5.SunCount];
		double[] alphaPrime = new double[(int)Term5.SunCount],
			deltaPrime = new double[(int)Term5.SunCount],
			hPrime = new double[(int)Term5.SunCount];
		var h0Prime = -1 * (Consts.SunRadius + spa.Enviroment.AtmosRefract);
		int i;

		var sunRts = spa;

		var m = SunMeanLongitude(spa.IntermediateOutput.JulianTimeModel.JEMillennium);
		spa.IntermediateOutput.Eot = EquationOfTime(m, spa.IntermediateOutput.SunItermediateModel.Alpha,
			spa.IntermediateOutput.DelPsi, spa.IntermediateOutput.Epsilon);

		sunRts.Time.Hour = sunRts.Time.Minute = 0;
		sunRts.Time.Second = 0;
		sunRts.TimeDeltas.DeltaUt1 = sunRts.Time.Timezone = 0.0;

		sunRts.IntermediateOutput.JulianTimeModel.JDay = JulianCalculator.JulianDay(sunRts.Time.Year, sunRts.Time.Month,
			sunRts.Time.Day,
			sunRts.Time.Hour,
			sunRts.Time.Minute, sunRts.Time.Second, sunRts.TimeDeltas.DeltaUt1, sunRts.Time.Timezone);

		CalculateGeocentricSunRightAscensionAndDeclination(ref sunRts);
		var nu = sunRts.IntermediateOutput.Nu;

		sunRts.TimeDeltas.DeltaT = 0;
		sunRts.IntermediateOutput.JulianTimeModel.JDay--;

		for (i = 0; i < (int)Term4.JDCount; i++)
		{
			CalculateGeocentricSunRightAscensionAndDeclination(ref sunRts);
			alpha[i] = sunRts.IntermediateOutput.SunItermediateModel.Alpha;
			delta[i] = sunRts.IntermediateOutput.SunItermediateModel.Delta;
			sunRts.IntermediateOutput.JulianTimeModel.JDay++;
		}

		mRts[(int)Term5.SunTransit] =
			ApproxSunTransitTime(alpha[(int)Term4.JDZero], spa.Enviroment.Longitude, nu);
		var h0 = SunHourAngleAtRiseSet(spa.Enviroment.Latitude, delta[(int)Term4.JDZero], h0Prime);

		if (h0 >= 0)
		{
			ApproxSunRiseAndSet(ref mRts, h0);

			for (i = 0; i < (int)Term5.SunCount; i++)
			{
				nuRts[i] = nu + 360.985647 * mRts[i];

				var n = mRts[i] + spa.TimeDeltas.DeltaT / 86400.0;
				alphaPrime[i] = RtsAlphaDeltaPrime(ref alpha, n);
				deltaPrime[i] = RtsAlphaDeltaPrime(ref delta, n);

				hPrime[i] = Limiters.LimitDegrees180Pm(nuRts[i] + spa.Enviroment.Longitude - alphaPrime[i]);

				hRts[i] = RtsSunAltitude(spa.Enviroment.Latitude, deltaPrime[i], hPrime[i]);
			}

			spa.IntermediateOutput.SunItermediateModel.SunriseHourAngle = hPrime[(int)Term5.SunRise];
			spa.IntermediateOutput.SunItermediateModel.SunsetHourAngle = hPrime[(int)Term5.SunSet];
			spa.IntermediateOutput.SunItermediateModel.SunTransitAltitude = hRts[(int)Term5.SunTransit];

			spa.Output.Suntransit = Limiters.DayFracToLocalHr(
				mRts[(int)Term5.SunTransit] - hPrime[(int)Term5.SunTransit] / 360.0,
				spa.Time.Timezone);

			spa.Output.Sunrise = Limiters.DayFracToLocalHr(SunRiseAndSet(ref mRts, ref hRts,
				ref deltaPrime,
				spa.Enviroment.Latitude, ref hPrime, h0Prime, (int)Term5.SunRise), spa.Time.Timezone);

			spa.Output.Sunset = Limiters.DayFracToLocalHr(SunRiseAndSet(ref mRts, ref hRts,
				ref deltaPrime,
				spa.Enviroment.Latitude, ref hPrime, h0Prime, (int)Term5.SunSet), spa.Time.Timezone);
		}
		else
		{
			spa.IntermediateOutput.SunItermediateModel.SunriseHourAngle =
				spa.IntermediateOutput.SunItermediateModel.SunsetHourAngle =
					spa.IntermediateOutput.SunItermediateModel.SunTransitAltitude =
						spa.Output.Suntransit = spa.Output.Sunrise = spa.Output.Sunset = -99999;
		}
	}

	public static void CalculateGeocentricSunRightAscensionAndDeclination(ref DataModel spa)
	{
		var x = new double[(int)Term2.TermXCount];

		spa.IntermediateOutput.JulianTimeModel.JCentury =
			JulianCalculator.JulianCentury(spa.IntermediateOutput.JulianTimeModel.JDay);

		spa.IntermediateOutput.JulianTimeModel.JEDay =
			JulianCalculator.JulianEphemerisDay(spa.IntermediateOutput.JulianTimeModel.JDay, spa.TimeDeltas.DeltaT);
		spa.IntermediateOutput.JulianTimeModel.JECentury =
			JulianCalculator.JulianEphemerisCentury(spa.IntermediateOutput.JulianTimeModel.JEDay);
		spa.IntermediateOutput.JulianTimeModel.JEMillennium =
			JulianCalculator.JulianEphemerisMillennium(spa.IntermediateOutput.JulianTimeModel.JECentury);

		spa.IntermediateOutput.EarthIntermediateModel.HeliocentericLongitude =
			SPACalculator.EarthHeliocentricLongitude(spa.IntermediateOutput.JulianTimeModel.JEMillennium);
		spa.IntermediateOutput.EarthIntermediateModel.HeliocentericLatitude =
			BaseCalculator.EarthHeliocentricLatitude(spa.IntermediateOutput.JulianTimeModel.JEMillennium);
		spa.IntermediateOutput.EarthIntermediateModel.RadiusVector =
			BaseCalculator.EarthRadiusVector(spa.IntermediateOutput.JulianTimeModel.JEMillennium);

		spa.IntermediateOutput.Theta =
			BaseCalculator.GeocentricLongitude(spa.IntermediateOutput.EarthIntermediateModel.HeliocentericLongitude);
		spa.IntermediateOutput.Beta =
			BaseCalculator.GeocentricLatitude(spa.IntermediateOutput.EarthIntermediateModel.HeliocentericLatitude);

		x[(int)Term2.TermX0] =
			spa.IntermediateOutput.SunItermediateModel.X0 =
				BaseCalculator.MeanElongationMoonSun(spa.IntermediateOutput.JulianTimeModel.JECentury);
		x[(int)Term2.TermX1] = spa.IntermediateOutput.SunItermediateModel.X1 =
			BaseCalculator.MeanAnomalySun(spa.IntermediateOutput.JulianTimeModel.JECentury);
		x[(int)Term2.TermX2] = spa.IntermediateOutput.MoonIntermediateModel.X2 =
			BaseCalculator.MeanAnomalyMoon(spa.IntermediateOutput.JulianTimeModel.JECentury);
		x[(int)Term2.TermX3] =
			spa.IntermediateOutput.MoonIntermediateModel.X3 =
				BaseCalculator.ArgumentLatitudeMoon(spa.IntermediateOutput.JulianTimeModel.JECentury);
		x[(int)Term2.TermX4] =
			spa.IntermediateOutput.MoonIntermediateModel.X4 =
				BaseCalculator.AscendingLongitudeMoon(spa.IntermediateOutput.JulianTimeModel.JECentury);

		BaseCalculator.NutationLongitudeAndObliquity(spa.IntermediateOutput.JulianTimeModel.JECentury, x,
			ref spa.IntermediateOutput.DelPsi,
			ref spa.IntermediateOutput.DelEpsilon);

		spa.IntermediateOutput.Epsilon0 =
			BaseCalculator.EclipticMeanObliquity(spa.IntermediateOutput.JulianTimeModel.JEMillennium);
		spa.IntermediateOutput.Epsilon =
			BaseCalculator.EclipticTrueObliquity(spa.IntermediateOutput.DelEpsilon, spa.IntermediateOutput.Epsilon0);

		spa.IntermediateOutput.DelTau =
			BaseCalculator.AberrationCorrection(spa.IntermediateOutput.EarthIntermediateModel.RadiusVector);
		spa.IntermediateOutput.SunItermediateModel.Lamda = BaseCalculator.ApparentSunLongitude(
			spa.IntermediateOutput.Theta,
			spa.IntermediateOutput.DelPsi, spa.IntermediateOutput.DelTau);
		spa.IntermediateOutput.Nu0 =
			BaseCalculator.GreenwichMeanSiderealTime(spa.IntermediateOutput.JulianTimeModel.JDay,
				spa.IntermediateOutput.JulianTimeModel.JCentury);
		spa.IntermediateOutput.Nu = BaseCalculator.GreenwichSiderealTime(spa.IntermediateOutput.Nu0,
			spa.IntermediateOutput.DelPsi, spa.IntermediateOutput.Epsilon);

		spa.IntermediateOutput.SunItermediateModel.Alpha = BaseCalculator.GeocentricRightAscension(
			spa.IntermediateOutput.SunItermediateModel.Lamda,
			spa.IntermediateOutput.Epsilon, spa.IntermediateOutput.Beta);
		spa.IntermediateOutput.SunItermediateModel.Delta = BaseCalculator.GeocentricDeclination(
			spa.IntermediateOutput.Beta,
			spa.IntermediateOutput.Epsilon, spa.IntermediateOutput.SunItermediateModel.Lamda);
	}

	private static double SunMeanLongitude(double jme)
	{
		return Limiters.LimitDegrees(280.4664567 + jme * (360007.6982779 +
		                                                  jme * (0.03032028 + jme * (1 / 49931.0 +
			                                                  jme * (-1 / 15300.0 +
			                                                         jme * (-1 / 2000000.0))))));
	}

	private static double EquationOfTime(double m, double alpha, double delPsi, double epsilon)
	{
		return Limiters.LimitMinutes(4.0 * (m - 0.0057183 - alpha +
		                                    delPsi * Math.Cos(DegRadCalculator.DegToRad(epsilon))));
	}

	private static double ApproxSunTransitTime(double alphaZero, double longitude, double nu)
	{
		return (alphaZero - longitude - nu) / 360.0;
	}

	private static double SunHourAngleAtRiseSet(double latitude, double deltaZero, double h0Prime)
	{
		double h0 = -99999;
		var latitudeRad = DegRadCalculator.DegToRad(latitude);
		var deltaZeroRad = DegRadCalculator.DegToRad(deltaZero);
		var argument = (Math.Sin(DegRadCalculator.DegToRad(h0Prime)) -
		                Math.Sin(latitudeRad) * Math.Sin(deltaZeroRad)) /
		               (Math.Cos(latitudeRad) * Math.Cos(deltaZeroRad));

		if (Math.Abs(argument) <= 1) h0 = Limiters.LimitDegrees180(DegRadCalculator.RadToDeg(Math.Acos(argument)));

		return h0;
	}

	private static void ApproxSunRiseAndSet(ref double[] mRts, double h0)
	{
		var h0Dfrac = h0 / 360.0;

		mRts[(int)Term5.SunRise] = Limiters.LimitZeroToOne(mRts[(int)Term5.SunTransit] - h0Dfrac);
		mRts[(int)Term5.SunSet] = Limiters.LimitZeroToOne(mRts[(int)Term5.SunTransit] + h0Dfrac);
		mRts[(int)Term5.SunTransit] = Limiters.LimitZeroToOne(mRts[(int)Term5.SunTransit]);
	}

	private static double RtsAlphaDeltaPrime(ref double[] ad, double n)
	{
		var a = ad[(int)Term4.JDZero] - ad[(int)Term4.JDMinus];
		var b = ad[(int)Term4.JDPlus] - ad[(int)Term4.JDZero];

		if (Math.Abs(a) >= 2.0) a = Limiters.LimitZeroToOne(a);
		if (Math.Abs(b) >= 2.0) b = Limiters.LimitZeroToOne(b);

		return ad[(int)Term4.JDZero] + n * (a + b + (b - a) * n) / 2.0;
	}

	private static double RtsSunAltitude(double latitude, double deltaPrime, double hPrime)
	{
		var latitudeRad = DegRadCalculator.DegToRad(latitude);
		var deltaPrimeRad = DegRadCalculator.DegToRad(deltaPrime);

		return DegRadCalculator.RadToDeg(Math.Asin(Math.Sin(latitudeRad) * Math.Sin(deltaPrimeRad) +
		                                           Math.Cos(latitudeRad) * Math.Cos(deltaPrimeRad) *
		                                           Math.Cos(DegRadCalculator.DegToRad(hPrime))));
	}

	private static double SunRiseAndSet(ref double[] mRts, ref double[] hRts, ref double[] deltaPrime,
		double latitude,
		ref double[] hPrime, double h0Prime, int sun)
	{
		return mRts[sun] + (hRts[sun] - h0Prime) /
			(360.0 * Math.Cos(DegRadCalculator.DegToRad(deltaPrime[sun])) *
			 Math.Cos(DegRadCalculator.DegToRad(latitude)) *
			 Math.Sin(DegRadCalculator.DegToRad(hPrime[sun])));
	}
}