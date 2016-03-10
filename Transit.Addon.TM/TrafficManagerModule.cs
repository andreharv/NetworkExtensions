using ICities;
using TrafficManager.Custom.PathFinding;
using TrafficManager.State;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Implementations;
using Transit.Framework.Modularity;
using UnityEngine;

namespace TrafficManager
{
    [Module("Transit.Addon.Mod")]
    public partial class TrafficManagerModule : ModuleBase
    {
		public override string Name => "Traffic Manager";

		public override void OnEnabled() {
			Log._Debug("TrafficManagerMod Enabled");

            ExtendedPathManager.instance.DefinePathFinding<CustomPathFind>();
        }

		public override void OnDisabled() {
			Log._Debug("TrafficManagerMod Disabled");

            ExtendedPathManager.instance.ResetPathFinding<CustomPathFind>();
        }

		public override void OnSettingsUI(UIHelperBase helper) {
			Options.makeSettings(helper);
		}
	}
}
