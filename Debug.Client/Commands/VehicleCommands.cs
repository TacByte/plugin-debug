using NFive.SDK.Core.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace NFive.Debug.Client.Commands
{
	public class VehicleCommands
	{
		public static void Spawn(ILogger logger, List<string> args)
		{

			if (!args.Any())
			{
				logger.Debug("Vehicle Commands: Missing arguments");
			}

			if (args[0].Equals("spawn"))
			{
				SpawnVehicle(logger, args[1]);
			}

			if (args[0].Equals("repair"))
			{
				RepairVehicle(logger);
			}

			if (args[0].Equals("clean"))
			{
				CleanVehicle(logger);
			}

		}

		private static async void SpawnVehicle(ILogger logger, string modelName)
		{
			var player = Game.Player.Character;
			var modelHash = API.GetHashKey(modelName);

			if (!API.IsModelValid((uint) modelHash))
			{
				logger.Debug("Vehicle Spawn Command: Invalid model");
			}
			else
			{
				var model = new Model(modelHash);
				var vehicle = await World.CreateVehicle(model, player.Position, 0f);

				// Set fancy license plate name
				vehicle.Mods.LicensePlate = " N5 Dev ";

				// Set fancy license plate style
				vehicle.Mods.LicensePlateStyle = LicensePlateStyle.BlueOnWhite3;

				// Warp player in to driver seat
				player.SetIntoVehicle(vehicle, VehicleSeat.Driver);
			}
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
