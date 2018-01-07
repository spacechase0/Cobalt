using CustomElementHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cobalt
{
    public class CobaltSprinklerObject : StardewValley.Object, ISaveElement
    {
        public const int INDEX = 901;
        public const string NAME = "Cobalt Sprinkler";
        public const string DESCRIPTION = "Waters the 48 adjacent tiles every morning.";
        public const int PRICE = 2000;
        public const string TYPE = "Crafting";
        public const int CATEGORY = CraftingCategory;
        public const int EDIBILITY = -300;

        private Texture2D icon = Mod.instance.Helper.Content.Load<Texture2D>("cobalt-sprinkler.png");

        public CobaltSprinklerObject()
        {
            parentSheetIndex = INDEX;
            name = NAME;
            price = PRICE;
            type = TYPE;
            category = CATEGORY;
            edibility = EDIBILITY;
            canBeSetDown = true;
            canBeGrabbed = true; // ?
        }

        public object getReplacement()
        {
            return new StardewValley.Object(tileLocation, 645);
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            var ret = new Dictionary<string, string>();
            return ret;
        }

        public void rebuild(Dictionary<string, string> saveData, object replacement)
        {
            parentSheetIndex = INDEX;
            name = NAME;
            price = PRICE;
            type = TYPE;
            category = CATEGORY;
            edibility = EDIBILITY;
            canBeSetDown = true;
            canBeGrabbed = true; // ?
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            var v = base.getBoundingBox(tileLocation);
            v.Width = v.Height = Game1.tileSize;
            return v;
        }

        public override void DayUpdate(GameLocation location)
        {
            this.health = 10;
            if (!Game1.isRaining || !location.isOutdoors)
            {
                    for (int index1 = (int)this.tileLocation.X - 3; (double)index1 <= (double)this.tileLocation.X + 3.0; ++index1)
                    {
                        for (int index2 = (int)this.tileLocation.Y - 3; (double)index2 <= (double)this.tileLocation.Y + 3.0; ++index2)
                        {
                            Vector2 key = new Vector2((float)index1, (float)index2);
                            if (location.terrainFeatures.ContainsKey(key) && location.terrainFeatures[key] is HoeDirt)
                                (location.terrainFeatures[key] as HoeDirt).state = 1;
                        }
                    }
                    location.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.animations, new Microsoft.Xna.Framework.Rectangle(0, 2176, Game1.tileSize * 5, Game1.tileSize * 5), 60f, 4, 100, this.tileLocation * (float)Game1.tileSize + new Vector2((float)(-Game1.tileSize * 3 + Game1.tileSize), (float)(-Game1.tileSize * 2)), false, false)
                    {
                        color = Color.White * 0.4f,
                        delayBeforeAnimationStart = Game1.random.Next(1000),
                        id = (float)((double)this.tileLocation.X * 4000.0 + (double)this.tileLocation.Y)
                    });
                }
            }
        }
}
