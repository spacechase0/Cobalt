using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System.Collections.Generic;

namespace Cobalt
{
    class CobaltInjector : IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("TileSheets\\tools");
        }

        public void Edit<T>(IAssetData asset)
        {
            if ( asset.AssetNameEquals( "TileSheets\\tools" ) )
                asset.AsImage().PatchImage(Mod.instance.Helper.Content.Load<Texture2D>("cobalt-tools.png"), null, null, PatchMode.Overlay);
        }
    }

    [HarmonyPatch(typeof(Utility), "priceForToolUpgradeLevel")]
    static class CobaltPriceHook
    {
        public static void Postfix(int level, ref int __result)
        {
            if (level == 5)
                __result = 100000;
        }
    }

    [HarmonyPatch(typeof(Utility), "getBlacksmithUpgradeStock")]
    static class CobaltUpgradeStockHook
    {
        public static void Postfix( StardewValley.Farmer who, Dictionary<Item, int[]> __result )
        {
            Tool tool = who.getToolFromName("Axe");
            if (tool != null && tool.upgradeLevel == 4 )
            {
                var newTool = new Axe();
                newTool.UpgradeLevel = 5;
                __result.Add(newTool, new int[] { 100000, 1, CobaltBarItem.INDEX });
            }

            tool = who.getToolFromName("Watering Can");
            if (tool != null && tool.upgradeLevel == 4)
            {
                var newTool = new WateringCan();
                newTool.UpgradeLevel = 5;
                __result.Add(newTool, new int[] { 100000, 1, CobaltBarItem.INDEX });
            }

            tool = who.getToolFromName("Pickaxe");
            if (tool != null && tool.upgradeLevel == 4)
            {
                var newTool = new Pickaxe();
                newTool.UpgradeLevel = 5;
                __result.Add(newTool, new int[] { 100000, 1, CobaltBarItem.INDEX });
            }

            tool = who.getToolFromName("Hoe");
            if (tool != null && tool.upgradeLevel == 4)
            {
                var newTool = new Hoe();
                newTool.UpgradeLevel = 5;
                __result.Add(newTool, new int[] { 100000, 1, CobaltBarItem.INDEX });
            }
        }
    }

    [HarmonyPatch(typeof(Tree), "performToolAction")]
    static class CobaltTreeFix
    {
        public static bool Prefix( Tree __instance, Tool t, int explosion )
        {
            if (t is Axe && t.upgradeLevel == 5)
            {
                if (__instance.tapped && explosion <= 0)
                    return true;
                if (__instance.growthStage >= 5)
                    __instance.health -= 10;
                else if (__instance.growthStage >= 3)
                    __instance.health -= 15;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(FruitTree), "performToolAction")]
    static class CobaltFruitTreeFix
    {
        public static bool Prefix(Tree __instance, Tool t, int explosion)
        {
            if (t is Axe && t.upgradeLevel == 5)
            {
                if (__instance.tapped && explosion <= 0)
                    return true;
                if (__instance.growthStage >= 4)
                    __instance.health -= 10f;
                else if (__instance.growthStage >= 3)
                    __instance.health -= 15f;
            }
            return true;
        }
    }
    
    [HarmonyPatch(typeof(StardewValley.Object), "performToolAction")]
    static class CobaltObjectFix
    {
        public static bool Prefix(StardewValley.Object __instance, Tool t)
        {
            if (t is Pickaxe && t.upgradeLevel == 5)
            {
                if (__instance.name.Equals("Stone") && t.GetType() == typeof(Pickaxe))
                    if (__instance.parentSheetIndex == 12 && t.upgradeLevel == 1 || (__instance.parentSheetIndex == 12 || __instance.parentSheetIndex == 14) && t.upgradeLevel == 0)
                    { }
                    else
                        __instance.minutesUntilReady -= 6;
            }
            return true;
        }
    }
    
    [HarmonyPatch(typeof(Quartz), "performToolAction")]
    static class CobaltQuartzFix
    {
        private static bool wasCobalt = false;

        public static bool Prefix(Quartz __instance, Tool t)
        {
            if (t is Pickaxe && t.upgradeLevel == 5)
            {
                wasCobalt = true;
                t.upgradeLevel = 4;
            }
            return true;
        }
        public static void Postfix(Quartz __instance, Tool t)
        {
            if ( wasCobalt )
            {
                t.upgradeLevel = 5;
                wasCobalt = false;
            }
        }
    }
    
    /* Triggers the lovely harmony problems.
    [HarmonyPatch(typeof(WateringCan), "DoFunction")]
    static class CobaltWateringCanFix
    {
        public static void Postfix(WateringCan __instance)
        {
            if ( __instance.waterCanMax == __instance.WaterLeft && __instance.upgradeLevel == 5 )
            {
                __instance.waterCanMax = __instance.WaterLeft = 1;
            }
        }
    }*/

    [HarmonyPatch(typeof(Tool), "get_DisplayName")]
    static class CobaltDisplayNameHook
    {
        public static bool Prefix( Tool __instance, ref string __result )
        {
            if ( __instance.upgradeLevel == 5 )
            {
                __result = __instance.Name;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Tool), "get_Name")]
    static class CobaltNameHook
    {
        public static bool Prefix(Tool __instance, ref string __result)
        {
            if (__instance.upgradeLevel == 5)
            {
                __result = "Cobalt " + __instance.name;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Tool), "tilesAffected")]
    static class Tier5TilesHook
    {
        public static void Postfix(Tool __instance, List<Vector2> __result, Vector2 tileLocation, int power, StardewValley.Farmer who)
        {
            if ( power >= 6)
            {
                __result.Clear();
                switch ( who.facingDirection )
                {
                    case 0:
                        for (int i = 0; i < 7 * 7; ++i)
                        {
                            int ix = i % 7 - 3;
                            int iy = i / 7 - 6;
                            __result.Add(tileLocation + new Vector2(ix, iy));
                        }
                        break;
                    case 1:
                        for (int i = 0; i < 7 * 7; ++i)
                        {
                            int ix = i % 7;
                            int iy = i / 7 - 3;
                            __result.Add(tileLocation + new Vector2(ix, iy));
                        }
                        break;
                    case 2:
                        for (int i = 0; i < 7 * 7; ++i)
                        {
                            int ix = i % 7 - 3;
                            int iy = i / 7;
                            __result.Add(tileLocation + new Vector2(ix, iy));
                        }
                        break;
                    case 3:
                        for (int i = 0; i < 7 * 7; ++i)
                        {
                            int ix = i % 7 - 6;
                            int iy = i / 7 - 3;
                            __result.Add(tileLocation + new Vector2(ix, iy));
                        }
                        break;
                }
            }
        }
    }
}
