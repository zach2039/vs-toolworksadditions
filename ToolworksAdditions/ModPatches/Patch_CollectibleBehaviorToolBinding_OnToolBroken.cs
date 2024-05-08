using System;
using HarmonyLib;
using Toolworks;
using Vintagestory.API.Common;

namespace ToolworksAdditions.ModPatches
{
    [HarmonyPatchCategory("ToolworksAdditions_CollectibleBehaviorToolBinding")]
	[HarmonyPatch(typeof(CollectibleBehaviorToolBinding), "OnToolBroken")]
	public class Patch_CollectibleBehaviorToolBinding_OnToolBroken {
		private static bool Prefix(CollectibleBehaviorToolBinding __instance, ItemStack partStack, ref bool breakPart)
		{
            ItemStack glueStack = __instance.GetGlue(partStack);
            if (glueStack == null)
			{
                // no glue, so proceed with dropping binding item
				return false; // skip original method
			}

            // If glued, reduce item durability and max durability by glue multipler so binding comes out of tool
            double multiplier = glueStack.ItemAttributes["glueProperties"]["multiplier"].AsDouble(0.0);

            // Remove glue from part, otherwise GetMaxDurability will use multiplier
            partStack.Attributes.SetItemstack("glue", null);

            int originalDurability = Math.Max(0, (int)Math.Round(partStack.Attributes.GetDecimal("durability", 0.0d) / multiplier));
            int originalMaxDurability = __instance.GetMaxDurability(partStack);

            if (originalMaxDurability == 0)
            {
                breakPart = true;
                return false; // skip original method
            }

            partStack.Attributes.SetInt("durability", originalDurability);

            return false; // skip original method
		}
	}
}
