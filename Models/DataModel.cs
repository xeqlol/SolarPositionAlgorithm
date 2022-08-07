using SPACalculator.Enums;
using SPACalculator.Models.IntermediateModels;

namespace SPACalculator.Models;

public class DataModel
{
	//----------------------INPUT VALUES------------------------
	public CalculationMode Mode;
	public TimeModel Time { get; set; }
	public TimeDeltasModel TimeDeltas { get; set; }
	public EnviromentModel Enviroment { get; set; }

	//-----------------Intermediate OUTPUT VALUES--------------------
	public IntermediateOutputModel MidOut { get; set; }

	//---------------------Final OUTPUT VALUES------------------------
	public OutputModel Output { get; set; }
}