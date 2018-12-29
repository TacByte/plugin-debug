using NFive.SDK.Core.Controllers;

namespace NFive.Debug.Shared
{
	public class Configuration : ControllerConfiguration
	{
		public string ActivateKey { get; set; } = "ReplayStartStopRecordingSecondary"; // Default to F2
	}
}
