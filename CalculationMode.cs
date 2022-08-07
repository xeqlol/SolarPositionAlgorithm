namespace SPACalculator;

public enum CalculationMode
{
	ZA = 0, //calculate zenith and azimuth
	ZAInc = 1, //calculate zenith, azimuth, and incidence
	ZARts = 2, //calculate zenith, azimuth, and sun rise/transit/set values
	All = 3 //calculate all SPA output values
}