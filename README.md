Toolworks Additions
=================

**An unofficial Toolworks addon. Requires [Toolworks](https://mods.vintagestory.at/show/mod/10324)**

Overview
--------

This mod includes patches and additional functionality to Crupette's Toolworks. 

Changes include:

 * Add any color leather as a binding material, of durability 475.
 
 * Make handles ground storable against a wall.
 
 * Make tool heads and parts use vanilla's global tool durability modifier.
 
 * Fix crash when right-clicking with glue pot in air.
 
 * Fix resin glue recipe.
 
 * Fix compatibility issue with ExpandedFoods that prevented resin glue from being crafted.
 
 * Fix compatibility issue with XSkills that prevented toolheads from being placed.

 * Fix compatibility issue with ProspectTogether that prevented prospecting results from being recorded properly.

 * Add additional compatibility with AncientTools, StillNecessaries, PrimitiveSurvival, and TailorsDelight. Credits to ErisLuna!

 * Add any metal nails and strips as a binding material, of varying durabilities. Credits to ErisLuna!

 * Fix compatibility issue with MorePiles that prevented sticks and bones from being placed if vanilla storage behavior was disabled.
 

Config Settings (`VintageStoryData/ModConfig/ToolworksAdditions.json`)
--------

 * `PatchToolworksCollectibleBehaviorToolGluingOnHeldInteractStart`: Enables or disables harmony patch that fixes crash with using gluing items on nothing/air; defaults to `true`.
 
 * `PatchToolworksCollectibleBehaviorProspectingPrintProbeResults`: Enables or disables harmony patch that adds compatibility with ProspectTogether; defaults to `true`.
 
 * `ApplyToolDurabilityConfigToToolHeads`: Enables or disables vanilla's tool durability modifier for tool heads; defaults to `true`.
 
 * `ApplyToolDurabilityConfigToToolParts`: Enables or disables vanilla's tool durability modifier for tool parts; defaults to `true`.


Future Plans
--------

 * More Wildcraft Trees compatibility, such as more tool handles of suitable wood.


Known Issues
--------

 * Resin glue pot will not display item description properly, listing contents as "unknown" instead. This seems to be a vanilla bug related to cooking recipes that use `dirtyPot: true` and item descriptions on `dirtyPotOutput`.

 * `[Error] [toolworks] Exception: Could not load file or assembly 'butchering...` is an error related to Toolworks trying to load Butchering's DLL no matter if enabled or not; I am unsure if I can fix this or if Toolworks will need to be updated.


Extras
--------

 * All credit for Toolworks goes to Crupette. Crupette has given me permission to publish this addon mod.
