namespace SPACalculator;

public struct SPAData
{
	//----------------------INPUT VALUES------------------------

	public int Year; // 4-digit year,      valid range: -2000 to 6000, error code: 1
	public int Month; // 2-digit month,         valid range: 1 to  12,  error code: 2
	public int Day; // 2-digit day,           valid range: 1 to  31,  error code: 3
	public int Hour; // Observer local hour,   valid range: 0 to  24,  error code: 4
	public int Minute; // Observer local minute, valid range: 0 to  59,  error code: 5
	public double Second; // Observer local second, valid range: 0 to <60,  error code: 6

	public double DeltaUt1; // Fractional second difference between UTC and UT which is used
	// to adjust UTC for earth's irregular rotation rate and is derived
	// from observation only and is reported in this bulletin:
	// http://maia.usno.navy.mil/ser7/ser7.dat,
	// where delta_ut1 = DUT1
	// valid range: -1 to 1 second (exclusive), error code 17

	public double DeltaT; // Difference between earth rotation time and terrestrial time
	// It is derived from observation only and is reported in this
	// bulletin: http://maia.usno.navy.mil/ser7/ser7.dat,
	// where delta_t = 32.184 + (TAI-UTC) - DUT1
	// valid range: -8000 to 8000 seconds, error code: 7

	public double Timezone; // Observer time zone (negative west of Greenwich)
	// valid range: -18   to   18 hours,   error code: 8

	public double Longitude; // Observer longitude (negative west of Greenwich)
	// valid range: -180  to  180 degrees, error code: 9

	public double Latitude; // Observer latitude (negative south of equator)
	// valid range: -90   to   90 degrees, error code: 10

	public double Elevation; // Observer elevation [meters]
	// valid range: -6500000 or higher meters,    error code: 11

	public double Pressure; // Annual average local pressure [millibars]
	// valid range:    0 to 5000 millibars,       error code: 12

	public double Temperature; // Annual average local temperature [degrees Celsius]
	// valid range: -273 to 6000 degrees Celsius, error code; 13

	public double Slope; // Surface slope (measured from the horizontal plane)
	// valid range: -360 to 360 degrees, error code: 14

	public double AzmRotation; // Surface azimuth rotation (measured from south to projection of
	//     surface normal on horizontal plane, negative east)
	// valid range: -360 to 360 degrees, error code: 15

	public double AtmosRefract; // Atmospheric refraction at sunrise and sunset (0.5667 deg is typical)
	// valid range: -5   to   5 degrees, error code: 16

	public CalculationMode Function; // Switch to choose functions for desired output (from enumeration)

	//-----------------Intermediate OUTPUT VALUES--------------------

	public double Jd; //Julian day
	public double Jc; //Julian century

	public double Jde; //Julian ephemeris day
	public double Jce; //Julian ephemeris century
	public double Jme; //Julian ephemeris millennium

	public double L; //earth heliocentric longitude [degrees]
	public double B; //earth heliocentric latitude [degrees]
	public double R; //earth radius vector [Astronomical Units, AU]

	public double Theta; //geocentric longitude [degrees]
	public double Beta; //geocentric latitude [degrees]

	public double X0; //mean elongation (moon-sun) [degrees]
	public double X1; //mean anomaly (sun) [degrees]
	public double X2; //mean anomaly (moon) [degrees]
	public double X3; //argument latitude (moon) [degrees]
	public double X4; //ascending longitude (moon) [degrees]

	public double DelPsi; //nutation longitude [degrees]
	public double DelEpsilon; //nutation obliquity [degrees]
	public double Epsilon0; //ecliptic mean obliquity [arc seconds]
	public double Epsilon; //ecliptic true obliquity  [degrees]

	public double DelTau; //aberration correction [degrees]
	public double Lamda; //apparent sun longitude [degrees]
	public double Nu0; //Greenwich mean sidereal time [degrees]
	public double Nu; //Greenwich sidereal time [degrees]

	public double Alpha; //geocentric sun right ascension [degrees]
	public double Delta; //geocentric sun declination [degrees]

	public double H; //observer hour angle [degrees]
	public double Xi; //sun equatorial horizontal parallax [degrees]
	public double DelAlpha; //sun right ascension parallax [degrees]
	public double DeltaPrime; //topocentric sun declination [degrees]
	public double AlphaPrime; //topocentric sun right ascension [degrees]
	public double HPrime; //topocentric local hour angle [degrees]

	public double E0; //topocentric elevation angle (uncorrected) [degrees]
	public double DelE; //atmospheric refraction correction [degrees]
	public double E; //topocentric elevation angle (corrected) [degrees]

	public double Eot; //equation of time [minutes]
	public double Srha; //sunrise hour angle [degrees]
	public double Ssha; //sunset hour angle [degrees]
	public double Sta; //sun transit altitude [degrees]

	//---------------------Final OUTPUT VALUES------------------------

	public double Zenith; //topocentric zenith angle [degrees]
	public double AzimuthAstro; //topocentric azimuth angle (westward from south) [for astronomers]

	public double Azimuth;
	//topocentric azimuth angle (eastward from north) [for navigators and solar radiation]

	public double Incidence; //surface incidence angle [degrees]

	public double Suntransit; //local sun transit time (or solar noon) [fractional hour]
	public double Sunrise; //local sunrise time (+/- 30 seconds) [fractional hour]
	public double Sunset; //local sunset time (+/- 30 seconds) [fractional hour]
}