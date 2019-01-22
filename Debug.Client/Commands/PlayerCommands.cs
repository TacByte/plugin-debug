using CitizenFX.Core;
using NFive.Debug.Client.Extensions;
using NFive.SDK.Core.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace NFive.Debug.Client.Commands
{
	public static class PlayerCommands
	{
		public static void Invincible(ILogger logger, List<string> args)
		{
			if (!args.Any())
			{
				Game.Player.IsInvincible = !Game.Player.IsInvincible;
			}
			else
			{
				Game.Player.IsInvincible = args[0].IsTruthy();
			}

			logger.Debug($"You are now{(Game.Player.IsInvincible ? string.Empty : " not")} invincible");
		}
	}
}
