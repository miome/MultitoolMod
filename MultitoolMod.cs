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
            if( e.Button == this.Config.InfoButton) {
                int x = (int)e.Cursor.AbsolutePixels.X;
                int y = (int)e.Cursor.AbsolutePixels.Y;
                int xtile = (int)x / Game1.tileSize;
                int ytile = (int)y / Game1.tileSize;
                GameLocation location = Game1.player.currentLocation;
                Vector2 tileVec = new Vector2(xtile, ytile);
                IDictionary<String, System.Object> properties = multitool.Get_Properties(x, y);
                string formattedProperties = multitool.Format_Properties(properties);
                this.Monitor.Log($"{formattedProperties}");
                Game1.addHUDMessage(new HUDMessage(formattedProperties));
            }
            else if (e.Button == this.Config.ToolButton)
            {
                int powerupLevel = 3; //(int)((Game1.toolHold + 20f) / 600f) + 1;
                int x = (int)e.Cursor.AbsolutePixels.X;
                int y = (int)e.Cursor.AbsolutePixels.Y;
                int xtile = (int)x / Game1.tileSize;
                int ytile = (int)y / Game1.tileSize;
                GameLocation location = Game1.player.currentLocation;
                Vector2 tileVec = new Vector2(xtile, ytile);
                multitool.DoFunction(Game1.currentLocation, x, y, powerupLevel, Game1.player);
                //string tool = this.multitool.whichTool(Game1.currentLocation, x, y, powerupLevel, Game1.player);
                //this.Monitor.Log($"whichTool called with ({Game1.currentLocation}, {x}, {y}, {powerupLevel}, {Game1.player}) returned {tool}");
                // bool canActOn = location.checkAction(new Location(xtile, ytile), Game1.viewport, Game1.player);
                //IDictionary<string, System.Object> tileProperties = this.Get_Properties(x, y);
                //string message = this.Format_Properties(tileProperties);
                //this.Monitor.Log($"{message}");

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

    }
}
