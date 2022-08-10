using HarmonyLib;
using ICities;
using NetworkExtensions;
using NetworkExtensions2.Patching;
using Transit.Framework.Modularity;

namespace Transit.Addon.Tools
{
    [Module("Transit.Addon.Mod", "NetworkExtensions.Mod")]
    public partial class ToolModule : ModuleBase
    {
        private Harmony harmony => new Harmony("andreharv.CSL.NetworkExtensions2");
        public override string Name
        {
            get { return "Tools"; }
        }

        // Hack For FileManager, deprecated
        public override void OnCreated(ILoading loading)
        {
            if (!Mod.FoundZoningAdjuster)
                CreateZoneBlocksPatch.Apply(harmony);
            CheckBuildPositionPatch.Apply(harmony);
            GetLengthSnapPatch.Apply(harmony);
            GetCategoryOrderPatch.Apply(harmony);
            SpawnButtonEntryPatch.Apply(harmony);
        }
        public override void OnReleased()
        {
            harmony.UnpatchAll();
        }
        public override string AssetPath { get { return InternalAssetPath; } set { InternalAssetPath = value; } }
        internal static string InternalAssetPath { get; private set; }
    }
}