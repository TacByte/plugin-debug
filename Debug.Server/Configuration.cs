using NFive.Debug.Shared;
using NFive.SDK.Core.Controllers;

namespace NFive.Debug.Server
{
	public class Configuration : ControllerConfiguration, IConfiguration
	{
		public string ActivateKey { get; set; } = "ReplayStartStopRecordingSecondary"; // Default to F2
	}
}
