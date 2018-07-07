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
// TODO: Create class X_attachment for each tool X which subclasses X. Override 
//       doFunction to use x,y from cursor location

namespace MultitoolMod.Framework
{
    public class Multitool : Tool
    {
        
        public Axe axe;
        public Pickaxe pickaxe;
        public MeleeWeapon scythe;
        public WateringCan wateringCan;

        public IDictionary<string, Tool> blades;
        public IDictionary<string, Tool> attachedTools;

        public Multitool()
        {
            
            this.axe = new Axe();
            this.pickaxe = new Pickaxe();
            this.scythe = new MeleeWeapon(47);
            this.wateringCan = new WateringCan();
            attachedTools["axe"] = this.axe;
            attachedTools["pickaxe"] = this.pickaxe;
            attachedTools["scythe"] = this.scythe;
            attachedTools["wateringcan"] = this.wateringCan;

            blades["pickaxe"] = new PickaxeBlade();



        }
        public override Item getOne()
        {
            return new Multitool { };
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
            Tool tool = attachedTools[this.whichTool(location,x,y,power,who)];

        }
        
        public string whichTool(GameLocation location, int x, int y, int power, Farmer who){
            int xtile = (int)x / Game1.tileSize;
            int ytile = (int)y / Game1.tileSize;
            string retval = "";

            return retval;
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
