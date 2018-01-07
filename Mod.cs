using Harmony;
using StardewModdingAPI;
using StardewValley;
using System.Reflection;

namespace Cobalt
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;

        public override void Entry(IModHelper helper)
        {
            instance = this;

            Items.init();

            helper.Content.AssetEditors.Add(new ItemInjector());
            helper.Content.AssetEditors.Add(new CobaltInjector());
            Game1.ResetToolSpriteSheet();

            var harmony = HarmonyInstance.Create("spacechase0.Cobalt");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
