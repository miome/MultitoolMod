using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Tools;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;
using xTile.ObjectModel;
using StardewValley.TerrainFeatures;
using StardewValley.Locations;
using SObject = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

// TODO: Look at stardewvalley bash script, figure out command arguments and
// env variables for mono, see if can launch from debugger.


namespace MultitoolMod.Framework
{
    public class Multitool : Tool
    {

        public Axe axe;
        public Pickaxe pickaxe;
        public MeleeWeapon scythe;
        public WateringCan wateringCan;
        public Hoe hoe;

        public IDictionary<string, Tool> blades;
        public IDictionary<string, Tool> attachedTools;

        public Multitool()
        {
            this.attachedTools = new Dictionary<string, Tool>();
            this.axe = new Axe();
            this.pickaxe = new Pickaxe();
            this.scythe = new MeleeWeapon(47);
            this.scythe = (MeleeWeapon)this.scythe.getOne();
            this.scythe.Category = -99;
            this.wateringCan = new WateringCan();
            this.hoe = new Hoe();
            attachedTools["axe"] = this.axe;
            attachedTools["pickaxe"] = this.pickaxe;
            attachedTools["melee"] = this.scythe;
            attachedTools["wateringcan"] = this.wateringCan;
            attachedTools["hoe"] = this.hoe;
        }
        public override Item getOne()
        {
            return new Multitool();
        }
        protected override string loadDisplayName()
        {
            return Game1.content.LoadString("A tool for all trades");
        }

        protected override string loadDescription()
        {
            return Game1.content.LoadString("A tool for all trades");
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            this.Refresh_Tools();
            Tool tool;
            IDictionary<string, object> properties = this.Get_Properties(x, y);
            string toolName;
            try
            {
                toolName = (string)properties["string_useTool"];
                tool = this.attachedTools[toolName];
            }
            catch (KeyNotFoundException)
            {
                //Game1.addHUDMessage(new HUDMessage($"No appropriate tool found, trying checkAction"));
                int xtile = (int)x / Game1.tileSize;
                int ytile = (int)y / Game1.tileSize;
                location.checkAction(new Location(xtile, ytile), Game1.viewport, Game1.player);
                return;
            }

            //Game1.addHUDMessage(new HUDMessage($"{toolName}/{tool} selected."));
            if (toolName == "melee")
            {
                //this.scythe.DoDamage(Game1.currentLocation, x, y, Game1.player.facingDirection, power, who);
                //TODO this doesn't land at the right location
                this.scythe.DoDamage(Game1.currentLocation, Game1.tileSize, Game1.tileSize, 0, power, who);
                //this.scythe.DoFunction(location, x, y, power, who);
                return;
            }
            else
            {
                tool.DoFunction(location, x, y, power, who);
                return;
            }
        }

        public void Refresh_Tools()
        {
            foreach (Item item in Game1.player.Items)
            {
                //Game1.addHUDMessage(new HUDMessage($"refresh found: {item.Name}"));
                if (item is Tool)
                {
                    if (item is Pickaxe p)
                    {
                        this.pickaxe = p;
                        //Game1.addHUDMessage(new HUDMessage($"refresh pick: {item.Name}"));
                    }
                    else if (item is Axe a)
                    {
                        this.axe = a;
                        //Game1.addHUDMessage(new HUDMessage($"refresh axe: {item.Name}"));
                    }
                    else if (item is WateringCan w)
                    {
                        this.wateringCan = w;
                        //Game1.addHUDMessage(new HUDMessage($"refresh wc: {item.Name}"));
                    }
                    else if (item is Hoe h)
                    {
                        this.hoe = h;
                        //Game1.addHUDMessage(new HUDMessage($"refresh hoe: {item.Name}"));
                    }
                    else if (((Tool)item).Name == "Scythe")
                    {
                        this.scythe = (MeleeWeapon)item;
                        //Game1.addHUDMessage(new HUDMessage($"refresh scythe: {item.Name}"));
                    }
                }
            }
        }


        public IDictionary<string, System.Object> Get_Properties(int x, int y)
        {
            //TODO: Identify worms(artifact spots)
            //TODO: Initialize properties with all false values
            int xtile = (int)x / Game1.tileSize;
            int ytile = (int)y / Game1.tileSize;
            GameLocation location = Game1.player.currentLocation;
            Vector2 tileVec = new Vector2(xtile, ytile);

            //This will be the return value for the function
            IDictionary<string, System.Object> properties = new Dictionary<string, System.Object>();
            properties.Add("bool_canPlant", (System.Object)false);

            location.terrainFeatures.TryGetValue(tileVec, out TerrainFeature tileFeature);
            location.objects.TryGetValue(tileVec, out SObject tileObj);
            ResourceClump clump = this.GetResourceClumpCoveringTile(location, tileVec);

            properties.Add("bool_isResourceClump", (System.Object)clump != null);
            if ((bool)properties["bool_isResourceClump"])
            {
                properties.Add("ResourceClump_clump", (System.Object)clump);
                switch (clump.parentSheetIndex)
                {
                    //TODO: Add large crops
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
                    properties.Add("bool_isDirt", (System.Object)false);
                }
                else if (tileObj.Name == "Stone")
                {
                    properties.Add("string_useTool", (System.Object)"pickaxe");
                    properties.Add("bool_isStone", (System.Object)true);
                }
                else if (tileObj is StardewValley.Objects.IndoorPot pot)
                {
                    properties = Get_HoeDirtProperties(pot.hoeDirt, properties);
                }
            }
            else if (tileObj == null && tileFeature != null)
            {
                properties.Add("object_terrainFeature", (System.Object)tileFeature);
                properties.Add("string_terrainFeatureName", (System.Object)tileFeature.ToString());
                properties.Add("bool_isWeed", (System.Object)false);
                if (tileFeature is HoeDirt dirt)
                {
                    properties.Add("bool_isDirt", (System.Object)true);
                    properties.Add("bool_isTilledDirt", (System.Object)true);
                    properties = Get_HoeDirtProperties(dirt, properties);

                }
                else
                {
                    properties.Add("bool_hasDeadCrop", (System.Object)false);
                    properties.Add("bool_hasLiveLCrop", (System.Object)false);

                    if (tileFeature is Tree tree)
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
            }
            return properties;
        }




        public IDictionary<string, System.Object> Get_HoeDirtProperties(HoeDirt dirt, IDictionary<string, System.Object> properties)
        {
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
                    // bool canHarvestNow = (dirt.crop.currentPhase.Value >= harvestablePhase)
                    //&& (!dirt.crop.fullyGrown.Value || dirt.crop.dayOfCurrentPhase.Value <= 0);
                    bool canHarvestNow = dirt.readyForHarvest();
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
                    else if (dirt.needsWatering())
                    {
                        properties["bool_needsWater"]=(System.Object)true;
                        properties.Add("string_useTool", (System.Object)"wateringcan");
                    }


                }
            }
            else
            {
                properties.Add("bool_canPlant", (System.Object)false);
            }

            return properties;
        }
        public string Format_Properties(IDictionary<string, System.Object> properties)
        {
            string formatted_output = "";
            foreach (KeyValuePair<string, System.Object> kvp in properties)
            {
                formatted_output += $"{kvp.Key} = {kvp.Value}, " + System.Environment.NewLine;
            }
            return formatted_output;

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
