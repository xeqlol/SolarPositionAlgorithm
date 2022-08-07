using SPACalculator.Enums;
using SPACalculator.Models;

namespace SPACalculator.Calculators;

public static class RTSCalc
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

		var m = SunMeanLongitude(spa.MidOut.JulianTime.JEMillennium);
		spa.MidOut.Eot = EquationOfTime(m, spa.MidOut.SunMidOut.Alpha,
			spa.MidOut.DelPsi, spa.MidOut.Epsilon);

		sunRts.Time.Hour = sunRts.Time.Minute = 0;
		sunRts.Time.Second = 0;
		sunRts.TimeDeltas.DeltaUt1 = sunRts.Time.Timezone = 0.0;

		sunRts.MidOut.JulianTime.JDay = JulianCalc.JulianDay(sunRts.Time.Year, sunRts.Time.Month,
			sunRts.Time.Day,
			sunRts.Time.Hour,
			sunRts.Time.Minute, sunRts.Time.Second, sunRts.TimeDeltas.DeltaUt1, sunRts.Time.Timezone);

		CalculateGeocentricSunRightAscensionAndDeclination(ref sunRts);
		var nu = sunRts.MidOut.Nu;

		sunRts.TimeDeltas.DeltaT = 0;
		sunRts.MidOut.JulianTime.JDay--;

		for (i = 0; i < (int)Term4.JDCount; i++)
		{
			CalculateGeocentricSunRightAscensionAndDeclination(ref sunRts);
			alpha[i] = sunRts.MidOut.SunMidOut.Alpha;
			delta[i] = sunRts.MidOut.SunMidOut.Delta;
			sunRts.MidOut.JulianTime.JDay++;
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

			spa.MidOut.SunMidOut.SunriseHourAngle = hPrime[(int)Term5.SunRise];
			spa.MidOut.SunMidOut.SunsetHourAngle = hPrime[(int)Term5.SunSet];
			spa.MidOut.SunMidOut.SunTransitAltitude = hRts[(int)Term5.SunTransit];

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
			spa.MidOut.SunMidOut.SunriseHourAngle =
				spa.MidOut.SunMidOut.SunsetHourAngle =
					spa.MidOut.SunMidOut.SunTransitAltitude =
						spa.Output.Suntransit = spa.Output.Sunrise = spa.Output.Sunset = -99999;
		}
	}

	public static void CalculateGeocentricSunRightAscensionAndDeclination(ref DataModel spa)
	{
		var x = new double[(int)Term2.TermXCount];

		spa.MidOut.JulianTime.JCentury =
			JulianCalc.JulianCentury(spa.MidOut.JulianTime.JDay);

		spa.MidOut.JulianTime.JEDay =
			JulianCalc.JulianEphemerisDay(spa.MidOut.JulianTime.JDay, spa.TimeDeltas.DeltaT);
		spa.MidOut.JulianTime.JECentury =
			JulianCalc.JulianEphemerisCentury(spa.MidOut.JulianTime.JEDay);
		spa.MidOut.JulianTime.JEMillennium =
			JulianCalc.JulianEphemerisMillennium(spa.MidOut.JulianTime.JECentury);

		spa.MidOut.EarthMidOut.HeliocentericLongitude =
			EarthCalc.EarthHeliocentricLongitude(spa.MidOut.JulianTime.JEMillennium);
		spa.MidOut.EarthMidOut.HeliocentericLatitude =
			EarthCalc.EarthHeliocentricLatitude(spa.MidOut.JulianTime.JEMillennium);
		spa.MidOut.EarthMidOut.RadiusVector =
			EarthCalc.EarthRadiusVector(spa.MidOut.JulianTime.JEMillennium);

		spa.MidOut.Theta =
			GeocentricCalc.GeoLongitude(spa.MidOut.EarthMidOut.HeliocentericLongitude);
		spa.MidOut.Beta = -spa.MidOut.EarthMidOut.HeliocentericLatitude;

		x[(int)Term2.TermX0] =
			spa.MidOut.SunMidOut.X0 =
				MoonSunCalc.MeanElongationMoonSun(spa.MidOut.JulianTime.JECentury);
		x[(int)Term2.TermX1] = spa.MidOut.SunMidOut.X1 =
			MoonSunCalc.MeanAnomalySun(spa.MidOut.JulianTime.JECentury);
		x[(int)Term2.TermX2] = spa.MidOut.MoonMidOut.X2 =
			MoonSunCalc.MeanAnomalyMoon(spa.MidOut.JulianTime.JECentury);
		x[(int)Term2.TermX3] =
			spa.MidOut.MoonMidOut.X3 =
				MoonSunCalc.ArgumentLatitudeMoon(spa.MidOut.JulianTime.JECentury);
		x[(int)Term2.TermX4] =
			spa.MidOut.MoonMidOut.X4 =
				MoonSunCalc.AscendingLongitudeMoon(spa.MidOut.JulianTime.JECentury);

		BaseCalc.NutationLongitudeAndObliquity
		(spa.MidOut.JulianTime.JECentury,
			x,
			out var delPsi,
			out var delEpsilon);
		spa.MidOut.DelPsi = delPsi;
		spa.MidOut.DelEpsilon = delEpsilon;

		spa.MidOut.Epsilon0 =
			EclipticCalc.EclipticMeanObliquity(spa.MidOut.JulianTime.JEMillennium);
		spa.MidOut.Epsilon =
			EclipticCalc.EclipticTrueObliquity(spa.MidOut.DelEpsilon, spa.MidOut.Epsilon0);

		spa.MidOut.DelTau =
			BaseCalc.AberrationCorrection(spa.MidOut.EarthMidOut.RadiusVector);
		spa.MidOut.SunMidOut.Lamda = BaseCalc.ApparentSunLongitude(
			spa.MidOut.Theta,
			spa.MidOut.DelPsi, spa.MidOut.DelTau);
		spa.MidOut.Nu0 =
			GreenwichCalc.GreenwichMeanSiderealTime(spa.MidOut.JulianTime.JDay,
				spa.MidOut.JulianTime.JCentury);
		spa.MidOut.Nu = GreenwichCalc.GreenwichSiderealTime(spa.MidOut.Nu0,
			spa.MidOut.DelPsi, spa.MidOut.Epsilon);

		spa.MidOut.SunMidOut.Alpha = GeocentricCalc.GeoRightAscension(
			spa.MidOut.SunMidOut.Lamda,
			spa.MidOut.Epsilon, spa.MidOut.Beta);
		spa.MidOut.SunMidOut.Delta = GeocentricCalc.GeoDeclination(
			spa.MidOut.Beta,
			spa.MidOut.Epsilon, spa.MidOut.SunMidOut.Lamda);
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
											delPsi * Math.Cos(DegRadCalc.DegToRad(epsilon))));
	}

	private static double ApproxSunTransitTime(double alphaZero, double longitude, double nu)
	{
		return (alphaZero - longitude - nu) / 360.0;
	}

	private static double SunHourAngleAtRiseSet(double latitude, double deltaZero, double h0Prime)
	{
		double h0 = -99999;
		var latitudeRad = DegRadCalc.DegToRad(latitude);
		var deltaZeroRad = DegRadCalc.DegToRad(deltaZero);
		var argument = (Math.Sin(DegRadCalc.DegToRad(h0Prime)) -
						Math.Sin(latitudeRad) * Math.Sin(deltaZeroRad)) /
					   (Math.Cos(latitudeRad) * Math.Cos(deltaZeroRad));

		if (Math.Abs(argument) <= 1) h0 = Limiters.LimitDegrees180(DegRadCalc.RadToDeg(Math.Acos(argument)));

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
		var latitudeRad = DegRadCalc.DegToRad(latitude);
		var deltaPrimeRad = DegRadCalc.DegToRad(deltaPrime);

		return DegRadCalc.RadToDeg(Math.Asin(Math.Sin(latitudeRad) * Math.Sin(deltaPrimeRad) +
											 Math.Cos(latitudeRad) * Math.Cos(deltaPrimeRad) *
											 Math.Cos(DegRadCalc.DegToRad(hPrime))));
	}

	private static double SunRiseAndSet(ref double[] mRts, ref double[] hRts, ref double[] deltaPrime,
		double latitude,
		ref double[] hPrime, double h0Prime, int sun)
	{
		return mRts[sun] + (hRts[sun] - h0Prime) /
			(360.0 * Math.Cos(DegRadCalc.DegToRad(deltaPrime[sun])) *
			 Math.Cos(DegRadCalc.DegToRad(latitude)) *
			 Math.Sin(DegRadCalc.DegToRad(hPrime[sun])));
	}
}