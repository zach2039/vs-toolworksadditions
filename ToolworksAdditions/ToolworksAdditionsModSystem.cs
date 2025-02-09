﻿using System.Reflection;
using HarmonyLib;

using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

using ToolworksAdditions.ModNetwork;

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

				if (ToolworksAdditionsConfig.Loaded.PatchToolworksCollectibleBehaviorToolBindingOnToolBreak)
				{
					harmony.PatchCategory("ToolworksAdditions_CollectibleBehaviorToolBinding");
					api.Logger.Notification("Applied ToolworksAdditions_CollectibleBehaviorToolBinding patch from Toolworks Additions!");
				}

				// Mod compatibility with ProspectTogether, but only if that mod is present
				if (ToolworksAdditionsConfig.Loaded.PatchToolworksCollectibleBehaviorProspectingPrintProbeResults && api.ModLoader.IsModEnabled("prospecttogether"))
				{
					harmony.PatchCategory("ToolworksAdditions_CollectibleBehaviorProspecting");
					api.Logger.Notification("Applied ToolworksAdditions_CollectibleBehaviorProspecting compatibility patch for ProspectTogether from Toolworks Additions!");
				}

				// Mod compatibility with SurvivalExpanded, but only if that mod is present
				if (ToolworksAdditionsConfig.Loaded.PatchToolworksCollectibleBehaviorProspectingPrintProbeResults && api.ModLoader.IsModEnabled("survivalexpanded"))
				{
					harmony.PatchCategory("ToolworksAdditions_ItemCompositeTool");
					api.Logger.Notification("Applied ToolworksAdditions_ItemCompositeTool compatibility patch for SurvivalExpanded from Toolworks Additions!");
				}
			}

			base.Start(api);

			api.Logger.Notification("Loaded Toolworks Additions!");
		}

		public override void AssetsFinalize(ICoreAPI api)
		{
			if (api is ICoreServerAPI sapi)
			{
				if (api.ModLoader.IsModEnabled("danatweaks"))
				{
					ModCompat.CompatDanaTweaks.ApplyDanaTweaksScytheMoreChanges(sapi);

					api.Logger.Notification("Applied DanaTweaks ScytheMore compatibility changes from Toolworks Additions!");
				}
			}
		}

		private void ApplyToolDurabilityConfig(ICoreServerAPI sapi)
		{
			if (typeof(SurvivalCoreSystem).GetField("config", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sapi.ModLoader.GetModSystem<SurvivalCoreSystem>()) is SurvivalConfig survivalConfig)
			{
				foreach (CollectibleObject obj in sapi.World.Collectibles)
				{
					if (obj.Attributes != null && obj.Durability != 1) // Hopefully this is a decent check to make sure items have durability already
					{
						bool doModify = false;

						if (ToolworksAdditionsConfig.Loaded.ApplyToolDurabilityConfigToToolHeads && (obj.Attributes.KeyExists("toolHeadPropertiesByType") || obj.Attributes.KeyExists("toolHeadProperties")))
						{
							doModify = true;
						}
						else if (ToolworksAdditionsConfig.Loaded.ApplyToolDurabilityConfigToToolParts && (obj.Attributes.KeyExists("toolPartPropertiesByType") || obj.Attributes.KeyExists("toolPartProperties")))
						{
							doModify = true;
						}

						if (doModify)
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
					PatchToolworksCollectibleBehaviorToolGluingOnHeldInteractStart = ToolworksAdditionsConfig.Loaded.PatchToolworksCollectibleBehaviorToolGluingOnHeldInteractStart,
					PatchToolworksCollectibleBehaviorProspectingPrintProbeResults = ToolworksAdditionsConfig.Loaded.PatchToolworksCollectibleBehaviorProspectingPrintProbeResults,
					ApplyToolDurabilityConfigToToolHeads = ToolworksAdditionsConfig.Loaded.ApplyToolDurabilityConfigToToolHeads,
					ApplyToolDurabilityConfigToToolParts = ToolworksAdditionsConfig.Loaded.ApplyToolDurabilityConfigToToolParts
				}, player);
		}

		public override void StartServerSide(ICoreServerAPI sapi)
		{
			sapi.Event.PlayerJoin += this.OnPlayerJoin; 
			
			// Create server channel for config data sync
			this.serverChannel = sapi.Network.RegisterChannel(Mod.Info.ModID)
				.RegisterMessageType<SyncConfigClientPacket>()
				.SetMessageHandler<SyncConfigClientPacket>((player, packet) => {});

			// Apply tool durability modifier to parts and/or heads, if enabled
			if (ToolworksAdditionsConfig.Loaded.ApplyToolDurabilityConfigToToolParts || ToolworksAdditionsConfig.Loaded.ApplyToolDurabilityConfigToToolHeads)
			{
				ApplyToolDurabilityConfig(sapi);
			}
		}

		public override void StartClientSide(ICoreClientAPI capi)
		{
			// Sync config settings with clients
			capi.Network.RegisterChannel(Mod.Info.ModID)
				.RegisterMessageType<SyncConfigClientPacket>()
				.SetMessageHandler<SyncConfigClientPacket>(p => {
					this.Mod.Logger.Event("Received config settings from server");
					ToolworksAdditionsConfig.Loaded.PatchToolworksCollectibleBehaviorToolGluingOnHeldInteractStart = p.PatchToolworksCollectibleBehaviorToolGluingOnHeldInteractStart;
					ToolworksAdditionsConfig.Loaded.PatchToolworksCollectibleBehaviorProspectingPrintProbeResults = p.PatchToolworksCollectibleBehaviorProspectingPrintProbeResults;
					ToolworksAdditionsConfig.Loaded.ApplyToolDurabilityConfigToToolHeads = p.ApplyToolDurabilityConfigToToolHeads;
					ToolworksAdditionsConfig.Loaded.ApplyToolDurabilityConfigToToolParts = p.ApplyToolDurabilityConfigToToolParts;
					
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
