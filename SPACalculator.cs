namespace SPACalculator;

public static class SPACalculator
{
	private static double RadToDeg(double radians)
	{
		return 180.0 / Consts.Pi * radians;
	}

	private static double DegToRad(double degrees)
	{
		return Consts.Pi / 180.0 * degrees;
	}

	private static int Integer(double value)
	{
		return (int)value;
	}

	private static double LimitDegrees(double degrees)
	{
		degrees /= 360.0;
		var limited = 360.0 * (degrees - Math.Floor(degrees));
		if (limited < 0) limited += 360.0;

		return limited;
	}

	private static double LimitDegrees180Pm(double degrees)
	{
		degrees /= 360.0;
		var limited = 360.0 * (degrees - Math.Floor(degrees));
		if (limited < -180.0) limited += 360.0;
		else if (limited > 180.0) limited -= 360.0;

		return limited;
	}

	private static double LimitDegrees180(double degrees)
	{
		degrees /= 180.0;
		var limited = 180.0 * (degrees - Math.Floor(degrees));
		if (limited < 0) limited += 180.0;

		return limited;
	}

	private static double LimitZeroToOne(double value)
	{
		var limited = value - Math.Floor(value);
		if (limited < 0) limited += 1.0;

		return limited;
	}

	private static double LimitMinutes(double minutes)
	{
		var limited = minutes;

		if (limited < -20.0) limited += 1440.0;
		else if (limited > 20.0) limited -= 1440.0;

		return limited;
	}

	private static double DayFracToLocalHr(double dayfrac, double timezone)
	{
		return 24.0 * LimitZeroToOne(dayfrac + timezone / 24.0);
	}

	private static double ThirdOrderPolynomial(double a, double b, double c, double d, double x)
	{
		return ((a * x + b) * x + c) * x + d;
	}

	public static int ValidateInputs(ref SPAData spa)
	{
		if (spa.Year < -2000 || spa.Year > 6000) return 1;
		if (spa.Month < 1 || spa.Month > 12) return 2;
		if (spa.Day < 1 || spa.Day > 31) return 3;
		if (spa.Hour < 0 || spa.Hour > 24) return 4;
		if (spa.Minute < 0 || spa.Minute > 59) return 5;
		if (spa.Second < 0 || spa.Second >= 60) return 6;
		if (spa.Pressure < 0 || spa.Pressure > 5000) return 12;
		if (spa.Temperature <= -273 || spa.Temperature > 6000) return 13;
		if (spa.DeltaUt1 <= -1 || spa.DeltaUt1 >= 1) return 17;
		if (spa.Hour == 24 && spa.Minute > 0) return 5;
		if (spa.Hour == 24 && spa.Second > 0) return 6;

		if (Math.Abs(spa.DeltaT) > 8000) return 7;
		if (Math.Abs(spa.Timezone) > 18) return 8;
		if (Math.Abs(spa.Longitude) > 180) return 9;
		if (Math.Abs(spa.Latitude) > 90) return 10;
		if (Math.Abs(spa.AtmosRefract) > 5) return 16;
		if (spa.Elevation < -6500000) return 11;

		if (spa.Function == CalculationMode.ZAInc || spa.Function == CalculationMode.All)
		{
			if (Math.Abs(spa.Slope) > 360) return 14;
			if (Math.Abs(spa.AzmRotation) > 360) return 15;
		}

		return 0;
	}

	private static double JulianDay(int year, int month, int day, int hour, int minute, double second, double dut1,
		double tz)
	{
		var dayDecimal = day + (hour - tz + (minute + (second + dut1) / 60.0) / 60.0) / 24.0;

		if (month < 3)
		{
			month += 12;
			year--;
		}

		var julianDay = Integer(365.25 * (year + 4716.0)) + Integer(30.6001 * (month + 1)) + dayDecimal - 1524.5;

		if (julianDay > 2299160.0)
		{
			double a = Integer(year / 100);
			julianDay += 2 - a + Integer(a / 4);
		}

		return julianDay;
	}

	private static double JulianCentury(double jd)
	{
		return (jd - 2451545.0) / 36525.0;
	}

	private static double JulianEphemerisDay(double jd, double deltaT)
	{
		return jd + deltaT / 86400.0;
	}

	private static double JulianEphemerisCentury(double jde)
	{
		return (jde - 2451545.0) / 36525.0;
	}

	private static double JulianEphemerisMillennium(double jce)
	{
		return jce / 10.0;
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

	private static double EarthHeliocentricLongitude(double jme)
	{
		var sum = new double[Consts.LCount];
		int i;

		for (i = 0; i < Consts.LCount; i++)
			sum[i] = EarthPeriodicTermSummation(Consts.LTerms[i], Consts.LSubcount[i], jme);

		return LimitDegrees(RadToDeg(EarthValues(sum, Consts.LCount, jme)));
	}

	private static double EarthHeliocentricLatitude(double jme)
	{
		var sum = new double[Consts.LCount];
		int i;

		for (i = 0; i < Consts.BCount; i++)
			sum[i] = EarthPeriodicTermSummation(Consts.BTerms[i], Consts.BSubcount[i], jme);

		return RadToDeg(EarthValues(sum, Consts.BCount, jme));
	}

	private static double EarthRadiusVector(double jme)
	{
		var sum = new double[Consts.RCount];
		int i;

		for (i = 0; i < Consts.RCount; i++)
			sum[i] = EarthPeriodicTermSummation(Consts.RTerms[i], Consts.RSubcount[i], jme);

		return EarthValues(sum, Consts.RCount, jme);
	}

	private static double GeocentricLongitude(double l)
	{
		var theta = l + 180.0;

		if (theta >= 360.0) theta -= 360.0;

		return theta;
	}

	private static double GeocentricLatitude(double b)
	{
		return -b;
	}

	private static double MeanElongationMoonSun(double jce)
	{
		return ThirdOrderPolynomial(1.0 / 189474.0, -0.0019142, 445267.11148, 297.85036, jce);
	}

	private static double MeanAnomalySun(double jce)
	{
		return ThirdOrderPolynomial(-1.0 / 300000.0, -0.0001603, 35999.05034, 357.52772, jce);
	}

	private static double MeanAnomalyMoon(double jce)
	{
		return ThirdOrderPolynomial(1.0 / 56250.0, 0.0086972, 477198.867398, 134.96298, jce);
	}

	private static double ArgumentLatitudeMoon(double jce)
	{
		return ThirdOrderPolynomial(1.0 / 327270.0, -0.0036825, 483202.017538, 93.27191, jce);
	}

	private static double AscendingLongitudeMoon(double jce)
	{
		return ThirdOrderPolynomial(1.0 / 450000.0, 0.0020708, -1934.136261, 125.04452, jce);
	}

	private static double XYTermSummation(int i, double[] x = null)
	{
		x = x ?? new double[(int)Term2.TermXCount];
		int j;
		double sum = 0;

		for (j = 0; j < (int)Term2.TermYCount; j++)
			sum += x[j] * Consts.YTerms[i][j];

		return sum;
	}

	private static void NutationLongitudeAndObliquity(double jce, double[] x, ref double delPsi,
		ref double delEpsilon)
	{
		int i;
		double sumPsi = 0, sumEpsilon = 0;

		for (i = 0; i < Consts.YCount; i++)
		{
			var xyTermSum = DegToRad(XYTermSummation(i, x));
			sumPsi += (Consts.PETerms[i][(int)Term3.TermPsiA] + jce * Consts.PETerms[i][(int)Term3.TermPsiB]) *
			          Math.Sin(xyTermSum);
			sumEpsilon += (Consts.PETerms[i][(int)Term3.TermEpsC] + jce * Consts.PETerms[i][(int)Term3.TermEpsD]) *
			              Math.Cos(xyTermSum);
		}

		delPsi = sumPsi / 36000000.0;
		delEpsilon = sumEpsilon / 36000000.0;
	}

	private static double EclipticMeanObliquity(double jme)
	{
		var u = jme / 10.0;

		return 84381.448 + u * (-4680.93 + u * (-1.55 + u * (1999.25 +
		                                                     u * (-51.38 + u * (-249.67 +
			                                                     u * (-39.05 + u * (7.12 +
				                                                     u * (27.87 + u * (5.79 + u * 2.45)))))))));
	}

	private static double EclipticTrueObliquity(double deltaEpsilon, double epsilon0)
	{
		return deltaEpsilon + epsilon0 / 3600.0;
	}

	private static double AberrationCorrection(double r)
	{
		return -20.4898 / (3600.0 * r);
	}

	private static double ApparentSunLongitude(double theta, double deltaPsi, double deltaTau)
	{
		return theta + deltaPsi + deltaTau;
	}

	private static double GreenwichMeanSiderealTime(double jd, double jc)
	{
		return LimitDegrees(280.46061837 + 360.98564736629 * (jd - 2451545.0) +
		                    jc * jc * (0.000387933 - jc / 38710000.0));
	}

	private static double GreenwichSiderealTime(double nu0, double deltaPsi, double epsilon)
	{
		return nu0 + deltaPsi * Math.Cos(DegToRad(epsilon));
	}

	private static double GeocentricRightAscension(double lamda, double epsilon, double beta)
	{
		var lamdaRad = DegToRad(lamda);
		var epsilonRad = DegToRad(epsilon);

		return LimitDegrees(RadToDeg(Math.Atan2(Math.Sin(lamdaRad) * Math.Cos(epsilonRad) -
		                                        Math.Tan(DegToRad(beta)) * Math.Sin(epsilonRad), Math.Cos(lamdaRad))));
	}

	private static double GeocentricDeclination(double beta, double epsilon, double lamda)
	{
		var betaRad = DegToRad(beta);
		var epsilonRad = DegToRad(epsilon);

		return RadToDeg(Math.Asin(Math.Sin(betaRad) * Math.Cos(epsilonRad) +
		                          Math.Cos(betaRad) * Math.Sin(epsilonRad) * Math.Sin(DegToRad(lamda))));
	}

	private static double ObserverHourAngle(double nu, double longitude, double alphaDeg)
	{
		return LimitDegrees(nu + longitude - alphaDeg);
	}

	private static double SunEquatorialHorizontalParallax(double r)
	{
		return 8.794 / (3600.0 * r);
	}

	private static void RightAscensionParallaxAndTopocentricDec(double latitude, double elevation,
		double xi, double h, double delta, ref double deltaAlpha, ref double deltaPrime)
	{
		var latRad = DegToRad(latitude);
		var xiRad = DegToRad(xi);
		var hRad = DegToRad(h);
		var deltaRad = DegToRad(delta);
		var u = Math.Atan(0.99664719 * Math.Tan(latRad));
		var y = 0.99664719 * Math.Sin(u) + elevation * Math.Sin(latRad) / 6378140.0;
		var x = Math.Cos(u) + elevation * Math.Cos(latRad) / 6378140.0;

		var deltaAlphaRad = Math.Atan2(-x * Math.Sin(xiRad) * Math.Sin(hRad),
			Math.Cos(deltaRad) - x * Math.Sin(xiRad) * Math.Cos(hRad));

		deltaPrime = RadToDeg(Math.Atan2((Math.Sin(deltaRad) - y * Math.Sin(xiRad)) * Math.Cos(deltaAlphaRad),
			Math.Cos(deltaRad) - x * Math.Sin(xiRad) * Math.Cos(hRad)));

		deltaAlpha = RadToDeg(deltaAlphaRad);
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
		var latRad = DegToRad(latitude);
		var deltaPrimeRad = DegToRad(deltaPrime);

		return RadToDeg(Math.Asin(Math.Sin(latRad) * Math.Sin(deltaPrimeRad) +
		                          Math.Cos(latRad) * Math.Cos(deltaPrimeRad) * Math.Cos(DegToRad(hPrime))));
	}

	private static double AtmosphericRefractionCorrection(double pressure, double temperature,
		double atmosRefract, double e0)
	{
		double delE = 0;

		if (e0 >= -1 * (Consts.SunRadius + atmosRefract))
			delE = pressure / 1010.0 * (283.0 / (273.0 + temperature)) *
				1.02 / (60.0 * Math.Tan(DegToRad(e0 + 10.3 / (e0 + 5.11))));

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
		var hPrimeRad = DegToRad(hPrime);
		var latRad = DegToRad(latitude);

		return LimitDegrees(RadToDeg(Math.Atan2(Math.Sin(hPrimeRad),
			Math.Cos(hPrimeRad) * Math.Sin(latRad) - Math.Tan(DegToRad(deltaPrime)) * Math.Cos(latRad))));
	}

	private static double TopocentricAzimuthAngle(double azimuthAstro)
	{
		return LimitDegrees(azimuthAstro + 180.0);
	}

	private static double SurfaceIncidenceAngle(double zenith, double azimuthAstro, double azmRotation,
		double slope)
	{
		var zenithRad = DegToRad(zenith);
		var slopeRad = DegToRad(slope);

		return RadToDeg(Math.Acos(Math.Cos(zenithRad) * Math.Cos(slopeRad) + Math.Sin(slopeRad) * Math.Sin(zenithRad) *
			Math.Cos(DegToRad(azimuthAstro - azmRotation))));
	}

	private static double SunMeanLongitude(double jme)
	{
		return LimitDegrees(280.4664567 + jme * (360007.6982779 +
		                                         jme * (0.03032028 + jme * (1 / 49931.0 +
		                                                                    jme * (-1 / 15300.0 +
			                                                                    jme * (-1 / 2000000.0))))));
	}

	private static double EquationOfTime(double m, double alpha, double delPsi, double epsilon)
	{
		return LimitMinutes(4.0 * (m - 0.0057183 - alpha + delPsi * Math.Cos(DegToRad(epsilon))));
	}

	private static double ApproxSunTransitTime(double alphaZero, double longitude, double nu)
	{
		return (alphaZero - longitude - nu) / 360.0;
	}

	private static double SunHourAngleAtRiseSet(double latitude, double deltaZero, double h0Prime)
	{
		double h0 = -99999;
		var latitudeRad = DegToRad(latitude);
		var deltaZeroRad = DegToRad(deltaZero);
		var argument = (Math.Sin(DegToRad(h0Prime)) - Math.Sin(latitudeRad) * Math.Sin(deltaZeroRad)) /
		               (Math.Cos(latitudeRad) * Math.Cos(deltaZeroRad));

		if (Math.Abs(argument) <= 1) h0 = LimitDegrees180(RadToDeg(Math.Acos(argument)));

		return h0;
	}

	private static void ApproxSunRiseAndSet(ref double[] mRts, double h0)
	{
		var h0Dfrac = h0 / 360.0;

		mRts[(int)Term5.SunRise] = LimitZeroToOne(mRts[(int)Term5.SunTransit] - h0Dfrac);
		mRts[(int)Term5.SunSet] = LimitZeroToOne(mRts[(int)Term5.SunTransit] + h0Dfrac);
		mRts[(int)Term5.SunTransit] = LimitZeroToOne(mRts[(int)Term5.SunTransit]);
	}

	private static double RtsAlphaDeltaPrime(ref double[] ad, double n)
	{
		var a = ad[(int)Term4.JDZero] - ad[(int)Term4.JDMinus];
		var b = ad[(int)Term4.JDPlus] - ad[(int)Term4.JDZero];

		if (Math.Abs(a) >= 2.0) a = LimitZeroToOne(a);
		if (Math.Abs(b) >= 2.0) b = LimitZeroToOne(b);

		return ad[(int)Term4.JDZero] + n * (a + b + (b - a) * n) / 2.0;
	}

	private static double RtsSunAltitude(double latitude, double deltaPrime, double hPrime)
	{
		var latitudeRad = DegToRad(latitude);
		var deltaPrimeRad = DegToRad(deltaPrime);

		return RadToDeg(Math.Asin(Math.Sin(latitudeRad) * Math.Sin(deltaPrimeRad) +
		                          Math.Cos(latitudeRad) * Math.Cos(deltaPrimeRad) * Math.Cos(DegToRad(hPrime))));
	}

	private static double SunRiseAndSet(ref double[] mRts, ref double[] hRts, ref double[] deltaPrime, double latitude,
		ref double[] hPrime, double h0Prime, int sun)
	{
		return mRts[sun] + (hRts[sun] - h0Prime) /
			(360.0 * Math.Cos(DegToRad(deltaPrime[sun])) * Math.Cos(DegToRad(latitude)) *
			 Math.Sin(DegToRad(hPrime[sun])));
	}

	private static void CalculateGeocentricSunRightAscensionAndDeclination(ref SPAData spa)
	{
		var x = new double[(int)Term2.TermXCount];

		spa.Jc = JulianCentury(spa.Jd);

		spa.Jde = JulianEphemerisDay(spa.Jd, spa.DeltaT);
		spa.Jce = JulianEphemerisCentury(spa.Jde);
		spa.Jme = JulianEphemerisMillennium(spa.Jce);

		spa.L = EarthHeliocentricLongitude(spa.Jme);
		spa.B = EarthHeliocentricLatitude(spa.Jme);
		spa.R = EarthRadiusVector(spa.Jme);

		spa.Theta = GeocentricLongitude(spa.L);
		spa.Beta = GeocentricLatitude(spa.B);

		x[(int)Term2.TermX0] = spa.X0 = MeanElongationMoonSun(spa.Jce);
		x[(int)Term2.TermX1] = spa.X1 = MeanAnomalySun(spa.Jce);
		x[(int)Term2.TermX2] = spa.X2 = MeanAnomalyMoon(spa.Jce);
		x[(int)Term2.TermX3] = spa.X3 = ArgumentLatitudeMoon(spa.Jce);
		x[(int)Term2.TermX4] = spa.X4 = AscendingLongitudeMoon(spa.Jce);

		NutationLongitudeAndObliquity(spa.Jce, x, ref spa.DelPsi, ref spa.DelEpsilon);

		spa.Epsilon0 = EclipticMeanObliquity(spa.Jme);
		spa.Epsilon = EclipticTrueObliquity(spa.DelEpsilon, spa.Epsilon0);

		spa.DelTau = AberrationCorrection(spa.R);
		spa.Lamda = ApparentSunLongitude(spa.Theta, spa.DelPsi, spa.DelTau);
		spa.Nu0 = GreenwichMeanSiderealTime(spa.Jd, spa.Jc);
		spa.Nu = GreenwichSiderealTime(spa.Nu0, spa.DelPsi, spa.Epsilon);

		spa.Alpha = GeocentricRightAscension(spa.Lamda, spa.Epsilon, spa.Beta);
		spa.Delta = GeocentricDeclination(spa.Beta, spa.Epsilon, spa.Lamda);
	}

	private static void CalculateEOTAndSunRiseTransitSet(ref SPAData spa)
	{
		double[] alpha = new double[(int)Term4.JDCount], delta = new double[(int)Term4.JDCount];
		double[] mRts = new double[(int)Term5.SunCount],
			nuRts = new double[(int)Term5.SunCount],
			hRts = new double[(int)Term5.SunCount];
		double[] alphaPrime = new double[(int)Term5.SunCount],
			deltaPrime = new double[(int)Term5.SunCount],
			hPrime = new double[(int)Term5.SunCount];
		var h0Prime = -1 * (Consts.SunRadius + spa.AtmosRefract);
		int i;

		var sunRts = spa;

		var m = SunMeanLongitude(spa.Jme);
		spa.Eot = EquationOfTime(m, spa.Alpha, spa.DelPsi, spa.Epsilon);

		sunRts.Hour = sunRts.Minute = 0;
		sunRts.Second = 0;
		sunRts.DeltaUt1 = sunRts.Timezone = 0.0;

		sunRts.Jd = JulianDay(sunRts.Year, sunRts.Month, sunRts.Day, sunRts.Hour,
			sunRts.Minute, sunRts.Second, sunRts.DeltaUt1, sunRts.Timezone);

		CalculateGeocentricSunRightAscensionAndDeclination(ref sunRts);
		var nu = sunRts.Nu;

		sunRts.DeltaT = 0;
		sunRts.Jd--;

		for (i = 0; i < (int)Term4.JDCount; i++)
		{
			CalculateGeocentricSunRightAscensionAndDeclination(ref sunRts);
			alpha[i] = sunRts.Alpha;
			delta[i] = sunRts.Delta;
			sunRts.Jd++;
		}

		mRts[(int)Term5.SunTransit] = ApproxSunTransitTime(alpha[(int)Term4.JDZero], spa.Longitude, nu);
		var h0 = SunHourAngleAtRiseSet(spa.Latitude, delta[(int)Term4.JDZero], h0Prime);

		if (h0 >= 0)
		{
			ApproxSunRiseAndSet(ref mRts, h0);

			for (i = 0; i < (int)Term5.SunCount; i++)
			{
				nuRts[i] = nu + 360.985647 * mRts[i];

				var n = mRts[i] + spa.DeltaT / 86400.0;
				alphaPrime[i] = RtsAlphaDeltaPrime(ref alpha, n);
				deltaPrime[i] = RtsAlphaDeltaPrime(ref delta, n);

				hPrime[i] = LimitDegrees180Pm(nuRts[i] + spa.Longitude - alphaPrime[i]);

				hRts[i] = RtsSunAltitude(spa.Latitude, deltaPrime[i], hPrime[i]);
			}

			spa.Srha = hPrime[(int)Term5.SunRise];
			spa.Ssha = hPrime[(int)Term5.SunSet];
			spa.Sta = hRts[(int)Term5.SunTransit];

			spa.Suntransit =
				DayFracToLocalHr(mRts[(int)Term5.SunTransit] - hPrime[(int)Term5.SunTransit] / 360.0,
					spa.Timezone);

			spa.Sunrise = DayFracToLocalHr(SunRiseAndSet(ref mRts, ref hRts, ref deltaPrime,
				spa.Latitude, ref hPrime, h0Prime, (int)Term5.SunRise), spa.Timezone);

			spa.Sunset = DayFracToLocalHr(SunRiseAndSet(ref mRts, ref hRts, ref deltaPrime,
				spa.Latitude, ref hPrime, h0Prime, (int)Term5.SunSet), spa.Timezone);
		}
		else
		{
			spa.Srha = spa.Ssha = spa.Sta = spa.Suntransit = spa.Sunrise = spa.Sunset = -99999;
		}
	}

	public static int SPACalculate(ref SPAData spa)
	{
		var result = ValidateInputs(ref spa);

		if (result == 0)
		{
			spa.Jd = JulianDay(spa.Year, spa.Month, spa.Day, spa.Hour,
				spa.Minute, spa.Second, spa.DeltaUt1, spa.Timezone);

			CalculateGeocentricSunRightAscensionAndDeclination(ref spa);

			spa.H = ObserverHourAngle(spa.Nu, spa.Longitude, spa.Alpha);
			spa.Xi = SunEquatorialHorizontalParallax(spa.R);

			RightAscensionParallaxAndTopocentricDec(spa.Latitude, spa.Elevation, spa.Xi,
				spa.H, spa.Delta, ref spa.DelAlpha, ref spa.DeltaPrime);

			spa.AlphaPrime = TopocentricRightAscension(spa.Alpha, spa.DelAlpha);
			spa.HPrime = TopocentricLocalHourAngle(spa.H, spa.DelAlpha);

			spa.E0 = TopocentricElevationAngle(spa.Latitude, spa.DeltaPrime, spa.HPrime);
			spa.DelE = AtmosphericRefractionCorrection(spa.Pressure, spa.Temperature,
				spa.AtmosRefract, spa.E0);
			spa.E = TopocentricElevationAngleCorrected(spa.E0, spa.DelE);

			spa.Zenith = TopocentricZenithAngle(spa.E);
			spa.AzimuthAstro = TopocentricAzimuthAngleAstro(spa.HPrime, spa.Latitude,
				spa.DeltaPrime);
			spa.Azimuth = TopocentricAzimuthAngle(spa.AzimuthAstro);

			if (spa.Function == CalculationMode.ZAInc || spa.Function == CalculationMode.All)
				spa.Incidence = SurfaceIncidenceAngle(spa.Zenith, spa.AzimuthAstro,
					spa.AzmRotation, spa.Slope);

			if (spa.Function == CalculationMode.ZARts || spa.Function == CalculationMode.All)
				CalculateEOTAndSunRiseTransitSet(ref spa);
		}

		return result;
	}
}