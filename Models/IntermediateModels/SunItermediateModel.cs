namespace SPACalculator.Models.IntermediateModels;

public class SunItermediateModel
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
	///     sun right ascension parallax [degrees]
	/// </summary>
	public double DelAlpha;

	/// <summary>
	///     geocentric sun declination [degrees]
	/// </summary>
	public double Delta;

	/// <summary>
	///     topocentric sun declination [degrees]
	/// </summary>
	public double DeltaPrime;

	/// <summary>
	///     apparent sun longitude [degrees]
	/// </summary>
	public double Lamda;

	/// <summary>
	///     sunrise hour angle [degrees]
	/// </summary>
	public double SunriseHourAngle;

	/// <summary>
	///     sunset hour angle [degrees]
	/// </summary>
	public double SunsetHourAngle;

	/// <summary>
	///     sun transit altitude [degrees]
	/// </summary>
	public double SunTransitAltitude;

	/// <summary>
	///     mean anomaly (sun) [degrees]
	/// </summary>
	public double X1;

	/// <summary>
	///     sun equatorial horizontal parallax [degrees]
	/// </summary>
	public double Xi;

	/// <summary>
	///     mean elongation (moon-sun) [degrees]
	/// </summary>
	public double X0;
}