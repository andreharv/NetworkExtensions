using CitiesHarmony.API;
using NetworkExtensions2.Patching;
using Transit.Framework.Modularity;

namespace Transit.Addon.Tools
{
    [Module("Transit.Addon.Mod", "NetworkExtensions.Mod")]
    public partial class ToolModule : ModuleBase
    {

        public override string Name
        {
            get { return "Tools"; }
        }

        // Hack For FileManager, deprecated 
        public override void OnEnabled()
        {
            HarmonyHelper.DoOnHarmonyReady(Patcher.PatchAll);
            
        }
        public override void OnDisabled()
        {
            HarmonyHelper.DoOnHarmonyReady(Patcher.UnpatchAll);
        }
        public override string AssetPath { get { return InternalAssetPath; } set { InternalAssetPath = value; } }
        internal static string InternalAssetPath { get; private set; }
    }
}