using System;

namespace Transit.Addon.TM.Data {
	public partial class TMConfigurationV2 {
		[Serializable]
		public class Options {
			public int simAccuracy = 1;
			public int laneChangingRandomization = 2;
			public int recklessDrivers = 3;
			public bool relaxedBusses = true;
			public bool allRelaxed = false;
			public bool prioritySignsOverlay = true;
			public bool timedLightsOverlay = true;
			public bool speedLimitsOverlay = true;
			public bool vehicleRestrictionsOverlay = true;
			public bool nodesOverlay = false;
			public bool allowEnterBlockedJunctions = false;
			public bool allowUTurns = false;
			public bool allowLaneChangesWhileGoingStraight = false;
			public bool advancedAI = false;
			public bool dynamicPathRecalculation = false;
			public bool highwayRules = false;
			public bool showLanes = false;
			public bool strongerRoadConditionEffects = false;
			public bool enableDespawning = true;

			public float pathCostMultiplicator = 1f; // debug value
			public float pathCostMultiplicator2 = 0f; // debug value
			public bool disableSomething1 = false; // debug switch
			public bool disableSomething2 = false; // debug switch
			public bool disableSomething3 = false; // debug switch
			public bool disableSomething4 = false; // debug switch
			public bool disableSomething5 = false; // debug switch
			public float someValue = 5f; // debug value
			public float someValue2 = 4f; // debug value
			public float someValue3 = 2f; // debug value
			public float someValue4 = 5f; // debug value

			internal int GetLaneChangingRandomizationTargetValue() {
				int ret = 100;
				switch (laneChangingRandomization) {
					case 0:
						ret = 2;
						break;
					case 1:
						ret = 4;
						break;
					case 2:
						ret = 10;
						break;
					case 3:
						ret = 20;
						break;
					case 4:
						ret = 50;
						break;
				}
				return ret;
			}

			internal float GetLaneChangingProbability() {
				switch (laneChangingRandomization) {
					case 0:
						return 0.5f;
					case 1:
						return 0.25f;
					case 2:
						return 0.1f;
					case 3:
						return 0.05f;
					case 4:
						return 0.01f;
				}
				return 0.01f;
			}

			internal int GetRecklessDriverModulo() {
				switch (recklessDrivers) {
					case 0:
						return 10;
					case 1:
						return 20;
					case 2:
						return 50;
					case 3:
						return 10000;
				}
				return 10000;
			}
		}
	}
}
