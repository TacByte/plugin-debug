using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using JetBrains.Annotations;
using NFive.SDK.Client.Events;
using NFive.SDK.Client.Interface;
using NFive.SDK.Client.Rpc;
using NFive.SDK.Client.Services;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Models.Player;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using NFive.Debug.Shared;
using NFive.SDK.Client.Commands;

namespace NFive.Debug.Client
{
	[PublicAPI]
	public class DebugService : Service
	{
		public Control ActivateKey;
		public bool Enabled;
		public Entity Tracked;
		public readonly PlayerList Players = new PlayerList();

		public DebugService(ILogger logger, ITickManager ticks, IEventManager events, IRpcHandler rpc, ICommandManager commands, OverlayManager overlay, User user) : base(logger, ticks, events, rpc, commands, overlay, user) { }

		public override async Task Started()
		{
			var config = await this.Rpc.Event(DebugEvents.GetConfig).Request<Configuration>();
			this.ActivateKey = (Control)Enum.Parse(typeof(Control), config.ActivateKey, true);

			this.Logger.Debug($"Activate key set to {this.ActivateKey}");

			this.Ticks.Attach(Tick);
		}

		private async Task Tick()
		{
			if (Game.IsControlJustPressed(2, this.ActivateKey))
			{
				this.Enabled = !this.Enabled;

				Screen.ShowNotification($"Debug tools are now {(this.Enabled ? "~g~enabled" : "~r~disabled")}~s~.");
			}

			if (!this.Enabled) return;

			if (Game.IsControlPressed(2, Control.Aim)) // Right click
			{
				DrawCrosshair();

				this.Tracked = GetEntityInCrosshair();
			}

			if (this.Tracked != null)
			{
				HighlightObject(this.Tracked);
				DrawData(this.Tracked);
			}

			await Task.FromResult(0);
		}

		private void DrawCrosshair()
		{
			API.DrawRect(0.5f, 0.5f, 0.008333333f, 0.001851852f, 255, 0, 0, 255);
			API.DrawRect(0.5f, 0.5f, 0.001041666f, 0.014814814f, 255, 0, 0, 255);
		}

		private Entity GetEntityInCrosshair()
		{
			var raycast = World.Raycast(GameplayCamera.Position, GameplayCameraForwardVector(), 100f, IntersectOptions.Everything, Game.PlayerPed);

			if (!raycast.DitHit || !raycast.DitHitEntity || raycast.HitPosition == default(Vector3)) return null;
			return raycast.HitEntity;
		}

		private void HighlightObject(Entity entity)
		{
			var res = API.GetEntityCoords(entity.Handle, true);

			API.DrawLine(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, res.X, res.Y, res.Z, 0, 255, 0, 255);

			API.SetDrawOrigin(res.X, res.Y, res.Z, 0);

			API.RequestStreamedTextureDict("helicopterhud", false);

			API.DrawSprite("helicopterhud", "hud_corner", -0.02f / 1.777f, -0.02f, 0.008f, 0.008f, 0.0f, 255, 0, 0, 200);
			API.DrawSprite("helicopterhud", "hud_corner", 0.02f / 1.777f, -0.02f, 0.008f, 0.008f, 90.0f, 255, 0, 0, 200);
			API.DrawSprite("helicopterhud", "hud_corner", -0.02f / 1.777f, 0.02f, 0.008f, 0.008f, 270.0f, 255, 0, 0, 200);
			API.DrawSprite("helicopterhud", "hud_corner", 0.02f / 1.777f, 0.02f, 0.008f, 0.008f, 180.0f, 255, 0, 0, 200);

			API.ClearDrawOrigin();
		}

		private void DrawData(Entity entity)
		{
			var data = GetDataFor(entity);
			var lineHeight = 0.024f;
			var pos = new PointF(0.9f, 0.5f);
			var size = new SizeF(0.16f, data.Count * lineHeight + 0.04f);

			API.DrawRect(pos.X, pos.Y, size.Width, size.Height, 0, 0, 0, 127);

			pos.Y -= size.Height / 2;
			pos.X -= size.Width / 2;

			pos.Y += 0.02f;
			pos.X += 0.01f;

			foreach (var entry in data)
			{
				if (!string.IsNullOrEmpty(entry.Value)) DrawText($"{entry.Key}: {entry.Value}", new Vector2(pos.X, pos.Y));
				pos.Y += lineHeight;
			}
		}

		private Dictionary<string, string> GetDataFor(Entity entity)
		{
			var list = new Dictionary<string, string>();

			if (!entity.Exists())
			{
				this.Tracked = null;

				return list;
			}

			try
			{
				list["Model Name"] = GetModelName(entity.Model);
				list[""] = "";

				var player = this.Players.FirstOrDefault(p => p.Character == entity);
				if (player != null)
				{
					list["Player Name"] = player.Name;
					list["Server ID"] = $"{player.ServerId}";
					list["Is Talking"] = $"{(Function.Call<bool>(Hash.NETWORK_IS_PLAYER_TALKING, player) ? "~g~True" : "~r~False")}~s~";
					list["  "] = "";
					list["Health"] = $"{player.Character.Health} / {player.Character.MaxHealth}";
					list["Invincible"] = $"{(player.IsInvincible ? "~g~True" : "~r~False")}~s~";
				}
				else if (entity is Vehicle veh)
				{
					list["Engine Health"] = $"{veh.EngineHealth:N1} / 1,000.0";
					list["Body Health"] = $"{veh.BodyHealth:N1} / 1,000.0";
					list["Speed"] = $"{veh.Speed / 0.621371f:N3}MP/H";
					list["RPM"] = $"{veh.CurrentRPM:N3}";
					list["Current Gear"] = $"{veh.CurrentGear}";
					list["Acceleration"] = $"{veh.Acceleration:N3}";
				}
				else
				{
					list["Health"] = $"{entity.Health} / {entity.MaxHealth}";
				}

				list[" "] = "";
				list["Distance"] = $"{Math.Sqrt(Game.PlayerPed.Position.DistanceToSquared(entity.Position)):N3}m";
				list["Heading"] = $"{entity.Heading:N3}";

				var pos = entity.Position;
				var rot = entity.Rotation;
				var vel = entity.Velocity;
				list["Position"] = $"{pos.X:N5} {pos.Y:N5} {pos.Z:N5}";
				list["Rotation"] = $"{rot.X:N5} {rot.Y:N5} {rot.Z:N5}";
				list["Velocity"] = $"{vel.X:N5} {vel.Y:N5} {vel.Z:N5}";
			}
			catch (Exception ex)
			{
				this.Logger.Error(ex);
			}

			return list;
		}

		private void DrawText(string text, Vector2 pos, Color? color = null, float scale = 0.25f, bool shadow = false, float shadowOffset = 1f, Alignment alignment = Alignment.Left, Font font = Font.ChaletLondon)
		{
			var col = color ?? Color.FromArgb(255, 255, 255);

			Function.Call(Hash.SET_TEXT_FONT, font);
			Function.Call(Hash.SET_TEXT_PROPORTIONAL, 0);
			Function.Call(Hash.SET_TEXT_SCALE, scale, scale);
			if (shadow) Function.Call(Hash.SET_TEXT_DROPSHADOW, shadowOffset, 0, 0, 0, 255);
			Function.Call(Hash.SET_TEXT_COLOUR, col.R, col.G, col.B, col.A);
			Function.Call(Hash.SET_TEXT_EDGE, 1, 0, 0, 0, 255);
			Function.Call(Hash.SET_TEXT_JUSTIFICATION, alignment);
			Function.Call(Hash._SET_TEXT_ENTRY, "STRING");
			Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, text);
			Function.Call(Hash._DRAW_TEXT, pos.X, pos.Y);
		}

		private string GetModelName(Model model)
		{
			var name = string.Empty;

			if (model.IsVehicle)
			{
				name = Enum.GetName(typeof(VehicleHash), (VehicleHash)model.Hash) ?? string.Empty;
			}

			if (model.IsProp)
			{
				name = Enum.GetName(typeof(ObjectHash), (ObjectHash)model.Hash) ?? string.Empty;
			}

			// model.IsPed isn't implemented in FiveM
			if (string.IsNullOrEmpty(name))
			{
				name = Enum.GetName(typeof(PedHash), (PedHash)model.Hash) ?? string.Empty;
			}

			return string.IsNullOrEmpty(name) ? model.Hash.ToString() : name;
		}

		/// GameplayCamera.ForwardVector is stubbed out so this is necessary
		private static Vector3 GameplayCameraForwardVector()
		{
			var rotation = (float)(Math.PI / 180.0) * GameplayCamera.Rotation;

			return Vector3.Normalize(new Vector3((float)-Math.Sin(rotation.Z) * (float)Math.Abs(Math.Cos(rotation.X)), (float)Math.Cos(rotation.Z) * (float)Math.Abs(Math.Cos(rotation.X)), (float)Math.Sin(rotation.X)));
		}
	}
}
