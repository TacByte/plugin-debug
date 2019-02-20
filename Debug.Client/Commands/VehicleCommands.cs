using CitizenFX.Core;
using CitizenFX.Core.Native;
using NFive.SDK.Core.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace NFive.Debug.Client.Commands
{
	public class VehicleCommands
	{
		public static void Spawn(ILogger logger, List<string> args)
		{
			if (!args.Any())
			{
				logger.Warn("Vehicle command: Missing arguments");
				return;
			}

			switch (args[0].Trim().ToLower())
			{
				case "spawn":
					SpawnVehicle(logger, args[1].Trim());
					break;

				case "repair":
					RepairVehicle(logger);
					break;

				case "clean":
					CleanVehicle(logger);
					break;

				default:
					logger.Warn("Vehicle command: Unknown command");
					break;
			}
		}

		private static async void SpawnVehicle(ILogger logger, string modelName)
		{
			var model = new Model(API.GetHashKey(modelName));

			if (!model.IsValid)
			{
				logger.Warn("Vehicle Spawn command: Invalid model");
				return;
			}

			var player = Game.Player.Character;
			var vehicle = await World.CreateVehicle(model, player.Position, 0f);

			// Set fancy license plate name
			vehicle.Mods.LicensePlate = " N5 Dev ";

			// Set fancy license plate style
			vehicle.Mods.LicensePlateStyle = LicensePlateStyle.BlueOnWhite3;

			// Warp player in to driver seat
			player.SetIntoVehicle(vehicle, VehicleSeat.Driver);
		}

		private static void RepairVehicle(ILogger logger)
		{
			var vehicle = Game.Player.Character.CurrentVehicle;
			vehicle.EngineHealth = 1000;
			vehicle.IsEngineRunning = true;
			vehicle.Repair();
		}

		private static void CleanVehicle(ILogger logger)
		{
			var vehicle = Game.Player.Character.CurrentVehicle;
			vehicle.DirtLevel = 0f;
		}
	}
}
