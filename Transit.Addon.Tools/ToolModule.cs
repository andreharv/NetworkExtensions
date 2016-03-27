using Transit.Framework.Modularity;

namespace Transit.Addon.Tools
{
    [Module("Transit.Mod.TAM", "Transit.Mod.NetworkExtensions")]
    public partial class ToolModule : ModuleBase
    {
        public override string Name
        {
            get { return "Zoning Tools"; }
        }

        public override int Order
        {
            get { return 90; }
        }

        // Hack For FileManager, deprecated
        public override string AssetPath { get { return InternalAssetPath; } set { InternalAssetPath = value; } }
        internal static string InternalAssetPath { get; private set; }
    }
}
