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
			else
			{
				if (args[0].Equals("spawn"))
				{
					SpawnVehicle(logger, args[1]);
				}
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

				// Set Vehicle Entity as Mission Entity
				Function.Call(Hash.SET_ENTITY_AS_MISSION_ENTITY, vehicle, true, false);


				// Set vehicle as Owned by player
				Function.Call(Hash.SET_VEHICLE_HAS_BEEN_OWNED_BY_PLAYER, vehicle, true);


				// Set Vehicle does not have to be hotwired
				Function.Call(Hash.SET_VEHICLE_NEEDS_TO_BE_HOTWIRED, vehicle, false);


				// Delete model from memory
				Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, model);

				// Set fancy license plate
				Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT_INDEX, vehicle, LicensePlateStyle.BlueOnWhite3);
				Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT, vehicle, " N5 Dev ");

				// Warp player in to driver seat
				player.SetIntoVehicle(vehicle, VehicleSeat.Driver);
			}
		}
	}
}
