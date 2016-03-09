using ICities;
using TrafficManager.State;
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
		}

		public override void OnDisabled() {
			Log._Debug("TrafficManagerMod Disabled");
		}

		public override void OnSettingsUI(UIHelperBase helper) {
			Options.makeSettings(helper);
		}
	}
}
