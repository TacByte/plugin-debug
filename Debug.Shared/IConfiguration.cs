using JetBrains.Annotations;

namespace NFive.Debug.Shared
{
	[PublicAPI]
	public interface IConfiguration
	{
		string ActivateKey { get; set; }
	}
}
