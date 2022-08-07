namespace SPACalculator.Enums;

public enum CalculationMode
{
	/// <summary>
	///     calculate zenith and azimuth
	/// </summary>
	ZA = 0,

	/// <summary>
	///     calculate zenith, azimuth, and incidence
	/// </summary>
	ZAInc = 1,

	/// <summary>
	///     calculate zenith, azimuth, and sun rise/transit/set values
	/// </summary>
	ZARts = 2,

	/// <summary>
	///     calculate all SPA output values
	/// </summary>
	All = 3
}