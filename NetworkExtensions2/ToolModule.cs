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

        public override string AssetPath { get { return InternalAssetPath; } set { InternalAssetPath = value; } }
        internal static string InternalAssetPath { get; private set; }
    }
}