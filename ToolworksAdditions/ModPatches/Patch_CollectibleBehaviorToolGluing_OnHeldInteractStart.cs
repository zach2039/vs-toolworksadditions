using HarmonyLib;
using Toolworks;
using Vintagestory.API.Common;

namespace ToolworksAdditions.ModPatches
{
    [HarmonyPatchCategory("ToolworksAdditions_CollectibleBehaviorToolGluing")]
	[HarmonyPatch(typeof(CollectibleBehaviorToolGluing), "OnHeldInteractStart")]
	public class Patch_CollectibleBehaviorToolGluing_OnHeldInteractStart {
		private static bool Prefix(CollectibleBehaviorToolGluing __instance, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling, ref EnumHandling handling)
		{
			if (blockSel == null)
            {
				// Prevent crash with Toolworks 1.8.1
				return false; // skip original method
			}

			return true; // continue with original method
		}
	}
}
