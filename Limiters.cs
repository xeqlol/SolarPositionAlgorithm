namespace SPACalculator;

public static class Limiters
{
	public static double LimitDegrees(double degrees)
	{
		degrees /= 360.0;
		var limited = 360.0 * (degrees - Math.Floor(degrees));
		if (limited < 0) limited += 360.0;

		return limited;
	}

	public static double LimitDegrees180Pm(double degrees)
	{
		degrees /= 360.0;
		var limited = 360.0 * (degrees - Math.Floor(degrees));
		if (limited < -180.0) limited += 360.0;
		else if (limited > 180.0) limited -= 360.0;

		return limited;
	}

	public static double LimitDegrees180(double degrees)
	{
		degrees /= 180.0;
		var limited = 180.0 * (degrees - Math.Floor(degrees));
		if (limited < 0) limited += 180.0;

		return limited;
	}

	public static double LimitZeroToOne(double value)
	{
		var limited = value - Math.Floor(value);
		if (limited < 0) limited += 1.0;

		return limited;
	}

	public static double LimitMinutes(double minutes)
	{
		var limited = minutes;

		if (limited < -20.0) limited += 1440.0;
		else if (limited > 20.0) limited -= 1440.0;

		return limited;
	}

	public static double DayFracToLocalHr(double dayfrac, double timezone)
	{
		return 24.0 * Limiters.LimitZeroToOne(dayfrac + timezone / 24.0);
	}
}