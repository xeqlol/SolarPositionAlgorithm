namespace SPACalculator.Models;

public class OutputModel
{
	/// <summary>
	///     topocentric azimuth angle (eastward from north) [for navigators and solar radiation]
	/// </summary>
	public double Azimuth;

	/// <summary>
	///     topocentric azimuth angle (westward from south) [for astronomers]
	/// </summary>
	public double AzimuthAstro;

	/// <summary>
	///     surface incidence angle [degrees]
	/// </summary>
	public double Incidence;

	/// <summary>
	///     local sunrise time (+/- 30 seconds) [fractional hour]
	/// </summary>
	public double Sunrise;

	/// <summary>
	///     local sunset time (+/- 30 seconds) [fractional hour]
	/// </summary>
	public double Sunset;

	/// <summary>
	///     local sun transit time (or solar noon) [fractional hour]
	/// </summary>
	public double Suntransit;

	/// <summary>
	///     topocentric zenith angle [degrees]
	/// </summary>
	public double Zenith;
}