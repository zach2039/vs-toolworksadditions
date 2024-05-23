using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace ToolworksAdditions.ModCompat
{
    public static class DataTweaksCompat
    {
        internal static void EnsureAttributesNotNull(CollectibleObject obj)
        {
            obj.Attributes ??= new JsonObject(new JObject());
        }

        /// <summary>
        /// Mimics Dana's PatchScytheAttributes method from DanaTweaks, but for Toolworks tools with CollectibleBehaviorScythe behavior
        /// </summary>
        /// <param name="item"></param>
        /// <param name="newPrefixes"></param>
        /// <param name="newSuffixes"></param>
        private static void PatchScytheAttributes(Item item, List<string> newPrefixes, List<string> newSuffixes)
		{
			if (item == null || !item.HasBehavior<Toolworks.CollectibleBehaviorScythe>())
			{
				return;
			}

			EnsureAttributesNotNull(item);

			List<string> codePrefixes = item?.GetBehavior<Toolworks.CollectibleBehaviorScythe>()?.properties?.codePrefixes?.ToList<string>();
			List<string> disallowedSuffixes = item?.GetBehavior<Toolworks.CollectibleBehaviorScythe>()?.properties?.codePrefixes?.ToList<string>();

			if (codePrefixes?.Any() == true)
			{
				codePrefixes.AddRange(newPrefixes.Except(codePrefixes));
				item.GetBehavior<Toolworks.CollectibleBehaviorScythe>().properties.codePrefixes = codePrefixes.ToArray();
			}
			if (disallowedSuffixes?.Any() == true)
			{
				disallowedSuffixes.AddRange(newSuffixes.Except(disallowedSuffixes));
				item.GetBehavior<Toolworks.CollectibleBehaviorScythe>().properties.disallowedSuffixes = disallowedSuffixes.ToArray();
			}
		}

		/// <summary>
		/// Mimics Dana's Scythe change code from DanaTweaks
		/// </summary>
		/// <param name="sapi"></param>
		internal static void ApplyDanaTweaksScytheMoreChanges(ICoreServerAPI sapi)
		{
			List<string> scytheMorePrefixes = new List<string>();

			foreach (Block block in sapi.World.Blocks)
			{
				if (block?.Code == null)
				{
					continue;
				}

				if (DanaTweaks.Core.ConfigServer.ScytheMore.Enabled && block.BlockMaterial == EnumBlockMaterial.Plant && !DanaTweaks.Core.ConfigServer.ScytheMore.DisallowedParts.Any(x => block.Code.ToString().Contains(x)))
				{
					if (!scytheMorePrefixes.Contains(block.Code.FirstCodePart()))
					{
						scytheMorePrefixes.Add(block.Code.FirstCodePart());
					}
				}
			}

			foreach (Item item in sapi.World.Items)
			{
				if (DanaTweaks.Core.ConfigServer.ScytheMore.Enabled && item.HasBehavior<Toolworks.CollectibleBehaviorScythe>())
				{
					PatchScytheAttributes(item, scytheMorePrefixes, DanaTweaks.Core.ConfigServer.ScytheMore.DisallowedSuffixes);
				}
			}
		}
    }
}