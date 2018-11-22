using JetBrains.Annotations;
using NFive.Debug.Shared;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Server.Controllers;
using NFive.SDK.Server.Events;
using NFive.SDK.Server.Rpc;

namespace NFive.Debug.Server
{
	[PublicAPI]
	public class DebugController : ConfigurableController<Configuration>
	{
		public DebugController(ILogger logger, IEventManager events, IRpcHandler rpc, Configuration configuration) : base(logger, events, rpc, configuration)
		{
			this.Rpc.Event(DebugEvents.GetConfig).On(e => e.Reply((IConfiguration)this.Configuration));
		}
	}
}
