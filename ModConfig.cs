using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace MultitoolMod
{
    class ModConfig
    {
        public KeybindList ToolButton { get; set; } = KeybindList.ForSingle(SButton.Q);
        public KeybindList InfoButton { get; set; } = KeybindList.ForSingle(SButton.N);
        public KeybindList CleanButton { get; set; } = KeybindList.ForSingle(SButton.C);
    }
}
