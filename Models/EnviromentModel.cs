namespace SPACalculator.Models;

public class EnviromentModel
{
	/// <summary>
	///     Atmospheric refraction at sunrise and sunset (0.5667 deg is typical),
	///     valid range: -5 to 5 degrees,
	///     error code:16
	/// </summary>
	public double AtmosRefract;

	/// <summary>
	///     Surface azimuth rotation
	///     (measured from south to projection ofsurface normal on horizontal plane, negative east) ,
	///     valid range: -360 to 360 degrees,
	///     error code: 15
	/// </summary>
	public double AzmRotation;

	/// <summary>
	///     Observer elevation [meters],
	///     valid range: -6500000 or higher meters,
	///     error code: 11
	/// </summary>
	public double Elevation;

	/// <summary>
	///     Observer latitude (negative south of equator),
	///     valid range: -90   to   90 degrees,
	///     error code: 10
	/// </summary>
	public double Latitude;

	/// <summary>
	///     Observer longitude (negative west of Greenwich),
	///     valid range: -180  to  180 degrees,
	///     error code: 9
	/// </summary>
	public double Longitude;

	/// <summary>
	///     Annual average local pressure [millibars],
	///     valid range:    0 to 5000 millibars,
	///     error code: 12
	/// </summary>
	public double Pressure;

	/// <summary>
	///     Surface slope (measured from the horizontal plane),
	///     valid range: -360 to 360 degrees,
	///     error code: 14
	/// </summary>
	public double Slope;

	/// <summary>
	///     Annual average local temperature [degrees Celsius],
	///     valid range: -273 to 6000 degrees Celsius,
	///     error code; 13
	/// </summary>
	public double Temperature;
}