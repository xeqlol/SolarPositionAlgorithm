namespace SPACalculator.Models.IntermediateModels;

public class IntermediateOutputModel
{
	/// <summary>
	///     geocentric latitude [degrees]
	/// </summary>
	public double Beta;

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
	///     Greenwich sidereal time [degrees]
	/// </summary>
	public double Nu;

	/// <summary>
	///     Greenwich mean sidereal time [degrees]
	/// </summary>
	public double Nu0;

	/// <summary>
	///     geocentric longitude [degrees]
	/// </summary>
	public double Theta;

	public JulianTimeModel JulianTime { get; } = new();
	public EarthIntermediateModel EarthMidOut { get; } = new();
	public SunItermediateModel SunMidOut { get; } = new();
	public MoonIntermediateModel MoonMidOut { get; } = new();
}