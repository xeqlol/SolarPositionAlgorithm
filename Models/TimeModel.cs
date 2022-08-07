namespace SPACalculator.Models;

public class TimeModel
{
	/// <summary>
	///     2-digit day,
	///     valid range: 1 to  31,
	///     error code: 3
	/// </summary>
	public int Day;

	/// <summary>
	///     Observer local hour,
	///     valid range: 0 to  24,
	///     error code: 4
	/// </summary>
	public int Hour;

	/// <summary>
	///     Observer local minute,
	///     valid range: 0 to  59,
	///     error code: 5
	/// </summary>
	public int Minute;

	/// <summary>
	///     2-digit month,
	///     valid range: 1 to  12,
	///     error code: 2
	/// </summary>
	public int Month;

	/// <summary>
	///     Observer local second,
	///     valid range: 0 to 60,
	///     error code: 6
	/// </summary>
	public double Second;

	/// <summary>
	///     Observer time zone (negative west of Greenwich),
	///     valid range: -18   to   18 hours,
	///     error code: 8
	/// </summary>
	public double Timezone;

	/// <summary>
	///     4-digit year,
	///     valid range: -2000 to 6000,
	///     error code: 1
	/// </summary>
	public int Year;
}