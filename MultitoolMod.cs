using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.TerrainFeatures;
using StardewValley.Locations;
using StardewValley;
using MultitoolMod.Framework;
using SObject = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;



namespace MultiToolMod
{
    /// <summary>The mod entry point.</summary>
    public class MultitoolMod : Mod
    {
        private ModConfig Config;
        private Multitool multitool;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
            InputEvents.ButtonReleased += this.InputEvents_ButtonReleased;
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.multitool = new Multitool();
        }

        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            // doFunction = performs tool action at coordinates, no user animation
            if (e.Button == this.Config.ToolButton)
            {
                //int powerupLevel = 3; //(int)((Game1.toolHold + 20f) / 600f) + 1;
                int x = (int)e.Cursor.AbsolutePixels.X;
                int y = (int)e.Cursor.AbsolutePixels.Y;
                int xtile = (int)x / Game1.tileSize;
                int ytile = (int)y / Game1.tileSize;
                GameLocation location = Game1.player.currentLocation;
                Vector2 tileVec = new Vector2(xtile, ytile);
                //string tool = this.multitool.whichTool(Game1.currentLocation, x, y, powerupLevel, Game1.player);
                //this.Monitor.Log($"whichTool called with ({Game1.currentLocation}, {x}, {y}, {powerupLevel}, {Game1.player}) returned {tool}");
                // bool canActOn = location.checkAction(new Location(xtile, ytile), Game1.viewport, Game1.player);
                IDictionary<string, System.Object> tileProperties = this.Get_Properties(x, y);
                string message = this.Format_Properties(tileProperties);
                this.Monitor.Log($"{message}");

                //HUDMessage show_properties = new HUDMessage($"{this.Format_Properties(tileProperties)}");
                //show_properties.fadeIn = false;
                //Game1.addHUDMessage(show_properties);
                //IClickableMenu.drawHoverText (b, message, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, overrideX, overrideY, 1f, null);
                /* 
                TODO: HUDMessage to small for text!
                Moo - Today at 6:59 AM
                Game1.chatBox.addInfoMessage ?
                Moo - Today at 7:03 AM
                Or you could maybe do something like spriteBatch.DrawString in 
                OnPostRenderHudEvent or something, to draw text directly on the 
                screen, if you want a constant display of cursor info rather 
                than something that appears when you press a key.
                */


                /*
                location.terrainFeatures.TryGetValue(tileVec, out TerrainFeature tileFeature);
                location.objects.TryGetValue(tileVec, out SObject tileObj);
                this.Monitor.Log($"tf= {tileFeature} to= {tileObj}");
     

                this.Monitor.Log($"Better use {tool}");
                Game1.addHUDMessage(new HUDMessage($"tf:{tileFeature}/to:{tileObj}/cl:{clump}/{tool}"));

                //this.Monitor.Log($"Calling {Game1.player.CurrentTool}.beginUsing with args ({Game1.currentLocation}, {x}, {y}, {Game1.player}) ");
                //Game1.player.CurrentTool.beginUsing(Game1.currentLocation, x, y, Game1.player);


                //this.Monitor.Log($"Calling {Game1.player.CurrentTool}.DoFunction with Args ({Game1.currentLocation}, {x}, {y}, {powerupLevel}, {Game1.player} )");
                //Game1.player.CurrentTool.DoFunction(Game1.currentLocation, x, y, powerupLevel, Game1.player);
                */
            }
        }


        public string Format_Properties(IDictionary<string, System.Object> properties)
        {
            string formatted_output = "";
            foreach (KeyValuePair<string, System.Object> kvp in properties)
            {
                formatted_output += $"{kvp.Key} = {kvp.Value}, ";// System.Environment.NewLine;
            }
            return formatted_output;

        }
        public IDictionary<string, System.Object> Get_Properties(int x, int y)
        {
            int xtile = (int)x / Game1.tileSize;
            int ytile = (int)y / Game1.tileSize;
            GameLocation location = Game1.player.currentLocation;
            Vector2 tileVec = new Vector2(xtile, ytile);

            //This will be the return value for the function
            IDictionary<string, System.Object> properties = new Dictionary<string, System.Object>();

            location.terrainFeatures.TryGetValue(tileVec, out TerrainFeature tileFeature);
            location.objects.TryGetValue(tileVec, out SObject tileObj);
            ResourceClump clump = this.GetResourceClumpCoveringTile(location, tileVec);

            properties.Add("bool_isResourceClump", (System.Object)clump != null);
            if ((bool)properties["bool_isResourceClump"])
            {
                properties.Add("ResourceClump_clump", (System.Object)clump);
                switch (clump.parentSheetIndex)
                {
                    case ResourceClump.boulderIndex:
                        properties.Add("string_useTool", (System.Object)"pickaxe");
                        properties.Add("bool_isBoulder", (System.Object)true);
                        break;
                    case ResourceClump.hollowLogIndex:
                        properties.Add("string_useTool", (System.Object)"axe");
                        properties.Add("bool_isHollowLog", (System.Object)true);
                        break;
                    case ResourceClump.meteoriteIndex:
                        properties.Add("string_useTool", (System.Object)"pickaxe");
                        properties.Add("bool_isMeteorite", (System.Object)true);
                        break;
                    case ResourceClump.stumpIndex:
                        properties.Add("string_useTool", (System.Object)"axe");
                        properties.Add("bool_Stump", (System.Object)true);
                        break;
                    case ResourceClump.mineRock1Index:
                    case ResourceClump.mineRock2Index:
                    case ResourceClump.mineRock3Index:
                    case ResourceClump.mineRock4Index:
                        properties.Add("string_useTool", (System.Object)"pickaxe");
                        properties.Add("bool_isMineRock", (System.Object)true);
                        break;
                }
            }
            else if (tileFeature == null && tileObj == null)
            {
                properties.Add("string_useTool", (System.Object)"hoe");
                properties.Add("bool_isDirt", (System.Object)true);
            }
            else if (tileFeature == null && tileObj != null)
            {
                properties.Add("object_tileObj", (System.Object)tileObj);
                properties.Add("string_tileObjName", (System.Object)tileObj.Name);
                if (tileObj.Name == "Twig")
                {
                    properties.Add("string_useTool", (System.Object)"axe");
                    properties.Add("bool_isTwig", (System.Object)true);
                }
                else if (tileObj.Name.ToLower().Contains("weed"))
                {
                    properties.Add("string_useTool", (System.Object)"melee");
                    properties.Add("bool_isWeed", (System.Object)true);
                }
                else if (tileObj.Name == "Stone")
                {
                    properties.Add("string_useTool", (System.Object)"pickaxe");
                    properties.Add("bool_isStone", (System.Object)true);
                }
                else
                {
                    // Pick up farm machines with pickaxe. Probably need to be more specific!
                    properties.Add("string_useTool", (System.Object)"pickaxe");
                }
            }
            else if (tileObj == null && tileFeature != null)
            {
                properties.Add("object_terrainFeature", (System.Object)tileFeature);
                properties.Add("string_terrainFeatureName", (System.Object)tileFeature.ToString());
                if (tileFeature is HoeDirt dirt)
                {
                    properties.Add("bool_isDirt", (System.Object)true);
                    properties.Add("bool_isTilledDirt", (System.Object)true);
                    if (dirt.crop != null)
                    {
                        if (dirt.crop.dead.Get())
                        {
                            properties.Add("bool_hasDeadCrop", (System.Object)true);
                            properties.Add("bool_hasLiveLCrop", (System.Object)false);
                            properties.Add("string_useTool", (System.Object)"melee");
                        }
                        else
                        {
                            properties.Add("bool_hasLiveLCrop", (System.Object)true);
                            int harvestablePhase = dirt.crop.phaseDays.Count - 1;
                            bool canHarvestNow = (dirt.crop.currentPhase.Value >= harvestablePhase)
                                && (!dirt.crop.fullyGrown.Value || dirt.crop.dayOfCurrentPhase.Value <= 0);
                            properties.Add("bool_fullyGrownCrop", (System.Object)canHarvestNow);
                            if (canHarvestNow)
                            {
                                if (dirt.crop.harvestMethod.Value == Crop.sickleHarvest)
                                {
                                    properties.Add("string_useTool", (System.Object)"melee");
                                }
                                else
                                {
                                    properties.Add("string_useTool", (System.Object)"grab");
                                }
                            }
                            else
                            {
                                properties.Add("string_useTool", (System.Object)"wateringcan");
                            }

                        }
                    }
                    else
                    {
                        properties.Add("bool_hasDeadCrop", (System.Object)false);
                        properties.Add("bool_hasLiveLCrop", (System.Object)false);
                        properties.Add("string_useTool", (System.Object)"wateringcan");
                    }
                }
                else if (tileFeature is Tree tree)
                {
                    //TODO: use hoe on tree seeds
                    properties.Add("string_useTool", (System.Object)"axe");
                    properties.Add("bool_hasTree", (System.Object)true);
                }
                else if (tileFeature is Grass grass)
                {
                    properties.Add("string_useTool", (System.Object)"melee");
                    properties.Add("bool_hasGrass", (System.Object)true);
                }

            }
            else
            { // tileFeature and tileObj are both not null
                properties.Add("string_useTool", (System.Object)"pickaxe");
            }
            return properties;
        }
        private void InputEvents_ButtonReleased(object sender, EventArgsInput e)
        {
            if (!Context.IsWorldReady)
                return;
            if (e.Button == this.Config.ToolButton)
            {
                //int powerupLevel = 3; //(int)((Game1.toolHold + 20f) / 600f) + 1;
                int x = (int)e.Cursor.AbsolutePixels.X;
                int y = (int)e.Cursor.AbsolutePixels.Y;
                //this.Monitor.Log($"Calling {Game1.player.CurrentTool}.DoFunction with Args ({Game1.currentLocation}, {x}, {y}, {powerupLevel}, {Game1.player} )");
                //Game1.player.CurrentTool.DoFunction(Game1.currentLocation, x,
                //                                y, powerupLevel, Game1.player);
                //this.Monitor.Log($"Calling {Game1.player.CurrentTool}.onRelease with args ({Game1.currentLocation}, {x}, {y}, {Game1.player}) ");
                //Game1.player.CurrentTool.onRelease(Game1.currentLocation, x, y, Game1.player);
                //Game1.player.CurrentTool.
            }
        }


        ///The functions below are taking directly from Tractor Mod by PathosChild
        /// https://github.com/Pathoschild/StardewMods/tree/develop/TractorMod

        /// <summary>Get resource clumps in a given location.</summary>
        /// <param name="location">The location to search.</param>
        protected IEnumerable<ResourceClump> GetResourceClumps(GameLocation location)
        {
            if (location is Farm farm)
                return farm.resourceClumps;
            if (location is Woods woods)
                return woods.stumps;
            if (location is MineShaft mineshaft)
                return mineshaft.resourceClumps;
            return new ResourceClump[0];
        }
        /// <summary>Get the resource clump which covers a given tile, if any.</summary>
        /// <param name="location">The location to check.</param>
        /// <param name="tile">The tile to check.</param>
        protected ResourceClump GetResourceClumpCoveringTile(GameLocation location, Vector2 tile)
        {
            Rectangle tileArea = this.GetAbsoluteTileArea(tile);
            foreach (ResourceClump clump in this.GetResourceClumps(location))
            {
                if (clump.getBoundingBox(clump.tile.Value).Intersects(tileArea))
                    return clump;
            }

            return null;
        }

        /// <summary>Get a rectangle representing the tile area in absolute pixels from the map origin.</summary>
        /// <param name="tile">The tile position.</param>
        protected Rectangle GetAbsoluteTileArea(Vector2 tile)
        {
            Vector2 pos = tile * Game1.tileSize;
            return new Rectangle((int)pos.X, (int)pos.Y, Game1.tileSize, Game1.tileSize);
        }
    }
}
