using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

using ToolworksAdditions.ModNetwork;
using HarmonyLib;
using Vintagestory.GameContent;
using System.Reflection;
using System;

namespace ToolworksAdditions
{
	public class ToolworksAdditionsModSystem : ModSystem
	{
		private IServerNetworkChannel serverChannel;
		
		private ICoreAPI api;

		public Harmony harmony;

		public override void StartPre(ICoreAPI api)
		{
			string cfgFileName = "ToolworksAdditions.json";

			try 
			{
				ToolworksAdditionsConfig cfgFromDisk;
				if ((cfgFromDisk = api.LoadModConfig<ToolworksAdditionsConfig>(cfgFileName)) == null)
				{
					api.StoreModConfig(ToolworksAdditionsConfig.Loaded, cfgFileName);
				}
				else
				{
					ToolworksAdditionsConfig.Loaded = cfgFromDisk;
				}
			} 
			catch 
			{
				api.StoreModConfig(ToolworksAdditionsConfig.Loaded, cfgFileName);
			}

			base.StartPre(api);
		}

		public override void Start(ICoreAPI api)
		{
			this.api = api;

			if (!Harmony.HasAnyPatches(Mod.Info.ModID)) {
				harmony = new Harmony(Mod.Info.ModID);
				if (ToolworksAdditionsConfig.Loaded.PatchToolworksCollectibleBehaviorToolGluingOnHeldInteractStart)
				{
					harmony.PatchCategory("ToolworksAdditions_CollectibleBehaviorToolGluing");
					api.Logger.Notification("Applied ToolworksAdditions_CollectibleBehaviorToolGluing patch from Toolworks Additions!");
				}
			}

			base.Start(api);

			api.Logger.Notification("Loaded Toolworks Additions!");
		}

		private void ApplyToolDurabilityConfigToParts(ICoreServerAPI sapi)
		{
            if (typeof(SurvivalCoreSystem).GetField("config", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sapi.ModLoader.GetModSystem<SurvivalCoreSystem>()) is SurvivalConfig survivalConfig)
            {
                foreach (CollectibleObject obj in sapi.World.Collectibles)
                {
					if (obj.Attributes != null && (obj.Attributes.KeyExists("toolPartPropertiesByType") || obj.Attributes.KeyExists("toolHeadProperties")))
					{
						if (obj.Durability != 1) // Hopefully this is a decent check to make sure items have durability already
						{
							obj.Durability = (int)((float)obj.Durability * survivalConfig.ToolDurabilityModifier);
						}
					}
                }
            }
        }

		private void OnPlayerJoin(IServerPlayer player)
		{
			// Send connecting players config settings
			this.serverChannel.SendPacket(
				new SyncConfigClientPacket {
					PatchToolworksCollectibleBehaviorToolGluingOnHeldInteractStart = ToolworksAdditionsConfig.Loaded.PatchToolworksCollectibleBehaviorToolGluingOnHeldInteractStart
				}, player);
		}

		public override void StartServerSide(ICoreServerAPI sapi)
		{
			sapi.Event.PlayerJoin += this.OnPlayerJoin; 
			
			// Create server channel for config data sync
			this.serverChannel = sapi.Network.RegisterChannel(Mod.Info.ModID)
				.RegisterMessageType<SyncConfigClientPacket>()
				.SetMessageHandler<SyncConfigClientPacket>((player, packet) => {});

			// Apply tool durability modifier to parts, if enabled
			if (ToolworksAdditionsConfig.Loaded.ApplyToolDurabilityConfigToParts)
			{
				ApplyToolDurabilityConfigToParts(sapi);
			}
		}

		public override void StartClientSide(ICoreClientAPI capi)
		{
			// Sync config settings with clients
			capi.Network.RegisterChannel(Mod.Info.ModID)
				.RegisterMessageType<SyncConfigClientPacket>()
				.SetMessageHandler<SyncConfigClientPacket>(p => {
					this.Mod.Logger.Event("Received config settings from server");
					ToolworksAdditionsConfig.Loaded.ApplyToolDurabilityConfigToParts = p.ApplyToolDurabilityConfigToParts;
					ToolworksAdditionsConfig.Loaded.PatchToolworksCollectibleBehaviorToolGluingOnHeldInteractStart = p.PatchToolworksCollectibleBehaviorToolGluingOnHeldInteractStart;
				});
		}
		
		public override void Dispose()
		{
			if (this.api is ICoreServerAPI sapi)
			{
				sapi.Event.PlayerJoin -= this.OnPlayerJoin;
			}
		}
	}
}
