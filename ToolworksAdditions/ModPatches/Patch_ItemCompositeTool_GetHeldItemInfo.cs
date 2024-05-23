using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Toolworks;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace ToolworksAdditions.ModPatches
{
	[HarmonyPatchCategory("ToolworksAdditions_ItemCompositeTool")]
	[HarmonyPatch(typeof(ItemCompositeTool), "GetHeldItemInfo")]
	class Patch_ItemCompositeTool_GetHeldItemInfo {
		/// <summary>
		/// Taken directly from SurvivalExpanded, more or less
		/// </summary>
		/// <param name="__instance"></param>
		/// <param name="__args"></param>
		static void Postfix(CollectibleObject __instance, object[] __args)
		{
			// Not accessible due to protection level; just assume we are always replacing text, for now
			//if (!SurvivalExpanded.ToolPowerAPI.ToolPowerPatches.replaceText)
			//{
			//	return;
			//}
			
			ItemSlot itemSlot = (ItemSlot)__args[0];
			StringBuilder seed = (StringBuilder)__args[1];
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (__instance.MiningSpeed != null && __instance.MiningSpeed.Count > 0)
			{
				string key = Lang.Get("Tool Tier: {0}", new object[]
				{
					__instance.ToolTier
				});
				string value = Lang.Get("survivalexpanded:tool-power", new object[]
				{
					__instance.ToolTier.ToString()
				});
				dictionary.Add(key, value);
			}
			ItemStack itemstack = itemSlot.Itemstack;
			if (__instance.GetAttackPower(itemstack) > 0.5f)
			{
				dictionary.Add(Lang.Get("Attack power: -{0} hp", new object[]
				{
					__instance.GetAttackPower(itemstack).ToString("0.#")
				}), Lang.Get("survivalexpanded:damage-power", new object[]
				{
					__instance.ToolTier
				}));
				dictionary.Add(Lang.Get("Attack tier: {0}", new object[]
				{
					__instance.ToolTier
				}), Lang.Get("survivalexpanded:attack-damage", new object[]
				{
					__instance.GetAttackPower(itemstack).ToString("0.#")
				}));
			}
			if (dictionary.Count > 0)
			{
				dictionary.Aggregate(seed, (StringBuilder old, KeyValuePair<string, string> upd) => old.Replace(upd.Key, upd.Value));
			}
		}
	}
}
