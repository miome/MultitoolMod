using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace MultiToolMod
{
    class ModConfig
    {
        public SButton ToolButton { get; set; } = SButton.Q;
        public SButton InfoButton { get; set; } = SButton.N;
    }
}