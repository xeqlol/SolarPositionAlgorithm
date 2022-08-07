namespace SPACalculator.Models;

public class TimeDeltasModel
{
	/// <summary>
	///     Difference between earth rotation time and terrestrial time
	///     It is derived from observation only and is reported in this
	///     bulletin: http://maia.usno.navy.mil/ser7/ser7.dat,
	///     where delta_t = 32.184 + (TAI-UTC) - DUT1
	///     valid range: -8000 to 8000 seconds, error code: 7
	/// </summary>
	public double DeltaT;

	/// <summary>
	///     Fractional second difference between UTC and UT which is used
	///     to adjust UTC for earth's irregular rotation rate and is derived
	///     from observation only and is reported in this bulletin:
	///     http://maia.usno.navy.mil/ser7/ser7.dat,
	///     where delta_ut1 = DUT1
	///     valid range: -1 to 1 second (exclusive), error code 17
	/// </summary>
	public double DeltaUt1;
}