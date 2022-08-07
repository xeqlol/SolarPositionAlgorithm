namespace SPACalculator.Models;

public class IntermediateOutputModel
{
	/// <summary>
	///     geocentric sun right ascension [degrees]
	/// </summary>
	public double Alpha;

	/// <summary>
	///     topocentric sun right ascension [degrees]
	/// </summary>
	public double AlphaPrime;

	/// <summary>
	///     earth heliocentric latitude [degrees]
	/// </summary>
	public double B;

	/// <summary>
	///     geocentric latitude [degrees]
	/// </summary>
	public double Beta;

	/// <summary>
	///     sun right ascension parallax [degrees]
	/// </summary>
	public double DelAlpha;

	/// <summary>
	///     atmospheric refraction correction [degrees]
	/// </summary>
	public double DelE;

	/// <summary>
	///     nutation obliquity [degrees]
	/// </summary>
	public double DelEpsilon;

	/// <summary>
	///     nutation longitude [degrees]
	/// </summary>
	public double DelPsi;

	/// <summary>
	///     geocentric sun declination [degrees]
	/// </summary>
	public double Delta;

	/// <summary>
	///     topocentric sun declination [degrees]
	/// </summary>
	public double DeltaPrime;

	/// <summary>
	///     aberration correction [degrees]
	/// </summary>
	public double DelTau;

	/// <summary>
	///     topocentric elevation angle (corrected) [degrees]
	/// </summary>
	public double E;

	/// <summary>
	///     topocentric elevation angle (uncorrected) [degrees]
	/// </summary>
	public double E0;

	/// <summary>
	///     equation of time [minutes]
	/// </summary>
	public double Eot;

	/// <summary>
	///     ecliptic true obliquity  [degrees]
	/// </summary>
	public double Epsilon;

	/// <summary>
	///     ecliptic mean obliquity [arc seconds]
	/// </summary>
	public double Epsilon0;

	/// <summary>
	///     observer hour angle [degrees]
	/// </summary>
	public double H;

	/// <summary>
	///     topocentric local hour angle [degrees]
	/// </summary>
	public double HPrime;

	/// <summary>
	///     Julian century
	/// </summary>
	public double Jc;

	/// <summary>
	///     Julian ephemeris century
	/// </summary>
	public double Jce;

	/// <summary>
	///     Julian day
	/// </summary>
	public double Jd;

	/// <summary>
	///     Julian ephemeris day
	/// </summary>
	public double Jde;

	/// <summary>
	///     Julian ephemeris millennium
	/// </summary>
	public double Jme;

	/// <summary>
	///     earth heliocentric longitude [degrees]
	/// </summary>
	public double L;

	/// <summary>
	///     apparent sun longitude [degrees]
	/// </summary>
	public double Lamda;

	/// <summary>
	///     Greenwich sidereal time [degrees]
	/// </summary>
	public double Nu;

	/// <summary>
	///     Greenwich mean sidereal time [degrees]
	/// </summary>
	public double Nu0;

	/// <summary>
	///     earth radius vector [Astronomical Units, AU]
	/// </summary>
	public double R;

	/// <summary>
	///     sunrise hour angle [degrees]
	/// </summary>
	public double Srha;

	/// <summary>
	///     sunset hour angle [degrees]
	/// </summary>
	public double Ssha;

	/// <summary>
	///     sun transit altitude [degrees]
	/// </summary>
	public double Sta;

	/// <summary>
	///     geocentric longitude [degrees]
	/// </summary>
	public double Theta;

	/// <summary>
	///     mean elongation (moon-sun) [degrees]
	/// </summary>
	public double X0;

	/// <summary>
	///     mean anomaly (sun) [degrees]
	/// </summary>
	public double X1;

	/// <summary>
	///     mean anomaly (moon) [degrees]
	/// </summary>
	public double X2;

	/// <summary>
	///     argument latitude (moon) [degrees]
	/// </summary>
	public double X3;

	/// <summary>
	///     ascending longitude (moon) [degrees]
	/// </summary>
	public double X4;

	/// <summary>
	///     sun equatorial horizontal parallax [degrees]
	/// </summary>
	public double Xi;
}