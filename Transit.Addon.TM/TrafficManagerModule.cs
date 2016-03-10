using ICities;
using TrafficManager.Custom.PathFinding;
using TrafficManager.State;
using Transit.Framework.ExtensionPoints.PathFinding;
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

            TAMPathFindManager.instance.DefinePathFinding<ImprovedPathFind>();
        }

		public override void OnDisabled() {
			Log._Debug("TrafficManagerMod Disabled");

            TAMPathFindManager.instance.ResetPathFinding<ImprovedPathFind>();
        }

		public override void OnSettingsUI(UIHelperBase helper) {
			Options.makeSettings(helper);
		}
	}
}
