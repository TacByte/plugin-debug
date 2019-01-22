using CitizenFX.Core.Native;
using NFive.SDK.Core.Diagnostics;
using System;
using System.Collections.Generic;

namespace NFive.Debug.Client.Commands
{
	public static class IplCommands
	{
		public static void Load(ILogger logger, IEnumerable<string> args)
		{
			foreach (var arg in args)
			{
				try
				{
					logger.Debug($"Loading IPL \"{arg}\"...");

					API.RequestIpl(arg);
				}
				catch (Exception ex)
				{
					logger.Error(ex, $"Error loading IPL \"{arg}\"");
				}
			}
		}

		public static void Unload(ILogger logger, IEnumerable<string> args)
		{
			foreach (var arg in args)
			{
				try
				{
					logger.Debug($"Unloading IPL \"{arg}\"...");

					API.RemoveIpl(arg);
				}
				catch (Exception ex)
				{
					logger.Error(ex, $"Error unloading IPL \"{arg}\"");
				}
			}
		}
	}
}
