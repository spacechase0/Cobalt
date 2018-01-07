using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using Harmony;
using System.Reflection;
using StardewValley;

namespace Cobalt
{
    public class Mod : StardewModdingAPI.Mod
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
