using System;
using ColossalFramework.UI;
using ICities;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.DataSerialization;
using Transit.Addon.TM.UI;
using UnityEngine;

namespace Transit.Addon.TM {

	public class OptionManager : MonoBehaviour {
		private static UIDropDown simAccuracyDropdown = null;
		private static UIDropDown laneChangingRandomizationDropdown = null;
		private static UIDropDown recklessDriversDropdown = null;
		private static UICheckBox relaxedBussesToggle = null;
		private static UICheckBox allRelaxedToggle = null;
		private static UICheckBox prioritySignsOverlayToggle = null;
		private static UICheckBox timedLightsOverlayToggle = null;
		private static UICheckBox speedLimitsOverlayToggle = null;
		private static UICheckBox vehicleRestrictionsOverlayToggle = null;
		private static UICheckBox nodesOverlayToggle = null;
		private static UICheckBox allowEnterBlockedJunctionsToggle = null;
		private static UICheckBox allowUTurnsToggle = null;
		private static UICheckBox allowLaneChangesWhileGoingStraightToggle = null;
		private static UICheckBox enableDespawningToggle = null;

		private static UICheckBox strongerRoadConditionEffectsToggle = null;
		private static UICheckBox advancedAIToggle = null;
		private static UICheckBox dynamicPathRecalculationToggle = null;
		private static UICheckBox highwayRulesToggle = null;
		private static UICheckBox showLanesToggle = null;
		private static UIButton forgetTrafficLightsBtn = null;
#if DEBUG
		private static UICheckBox disableSomething5Toggle = null;
		private static UICheckBox disableSomething4Toggle = null;
		private static UICheckBox disableSomething3Toggle = null;
		private static UICheckBox disableSomething2Toggle = null;
		private static UICheckBox disableSomething1Toggle = null;
		private static UITextField pathCostMultiplicatorField = null;
		private static UITextField pathCostMultiplicator2Field = null;
		private static UITextField someValueField = null;
		private static UITextField someValue2Field = null;
		private static UITextField someValue3Field = null;
		private static UITextField someValue4Field = null;
#endif

		private static UIHelperBase mainGroup = null;
		private static UIHelperBase aiGroup = null;
		private static UIHelperBase overlayGroup = null;
		private static UIHelperBase maintenanceGroup = null;

		public static void makeSettings(UIHelperBase helper) {
			mainGroup = helper.AddGroup(Translation.GetString("TMPE_Title"));
			simAccuracyDropdown = mainGroup.AddDropdown(Translation.GetString("Simulation_accuracy") + ":", new string[] { Translation.GetString("Very_high"), Translation.GetString("High"), Translation.GetString("Medium"), Translation.GetString("Low"), Translation.GetString("Very_Low") }, TMDataManager.Options.simAccuracy, onSimAccuracyChanged) as UIDropDown;
			recklessDriversDropdown = mainGroup.AddDropdown(Translation.GetString("Reckless_driving") + ":", new string[] { Translation.GetString("Path_Of_Evil_(10_%)"), Translation.GetString("Rush_Hour_(5_%)"), Translation.GetString("Minor_Complaints_(2_%)"), Translation.GetString("Holy_City_(0_%)") }, TMDataManager.Options.recklessDrivers, onRecklessDriversChanged) as UIDropDown;
			relaxedBussesToggle = mainGroup.AddCheckbox(Translation.GetString("Busses_may_ignore_lane_arrows"), TMDataManager.Options.relaxedBusses, onRelaxedBussesChanged) as UICheckBox;
#if DEBUG
			allRelaxedToggle = mainGroup.AddCheckbox(Translation.GetString("All_vehicles_may_ignore_lane_arrows"), TMDataManager.Options.allRelaxed, onAllRelaxedChanged) as UICheckBox;
#endif
			allowEnterBlockedJunctionsToggle = mainGroup.AddCheckbox(Translation.GetString("Vehicles_may_enter_blocked_junctions"), TMDataManager.Options.allowEnterBlockedJunctions, onAllowEnterBlockedJunctionsChanged) as UICheckBox;
			allowUTurnsToggle = mainGroup.AddCheckbox(Translation.GetString("Vehicles_may_do_u-turns_at_junctions") + " (BETA feature)", TMDataManager.Options.allowUTurns, onAllowUTurnsChanged) as UICheckBox;
			allowLaneChangesWhileGoingStraightToggle = mainGroup.AddCheckbox(Translation.GetString("Vehicles_going_straight_may_change_lanes_at_junctions"), TMDataManager.Options.allowLaneChangesWhileGoingStraight, onAllowLaneChangesWhileGoingStraightChanged) as UICheckBox;
			strongerRoadConditionEffectsToggle = mainGroup.AddCheckbox(Translation.GetString("Road_condition_has_a_bigger_impact_on_vehicle_speed"), TMDataManager.Options.strongerRoadConditionEffects, onStrongerRoadConditionEffectsChanged) as UICheckBox;
			enableDespawningToggle = mainGroup.AddCheckbox(Translation.GetString("Enable_despawning"), TMDataManager.Options.enableDespawning, onEnableDespawningChanged) as UICheckBox;
			aiGroup = helper.AddGroup("Advanced Vehicle AI");
			advancedAIToggle = aiGroup.AddCheckbox(Translation.GetString("Enable_Advanced_Vehicle_AI"), TMDataManager.Options.advancedAI, onAdvancedAIChanged) as UICheckBox;
#if DEBUG
			dynamicPathRecalculationToggle = aiGroup.AddCheckbox(Translation.GetString("Enable_dynamic_path_calculation"), TMDataManager.Options.dynamicPathRecalculation, onDynamicPathRecalculationChanged) as UICheckBox;
#endif
			highwayRulesToggle = aiGroup.AddCheckbox(Translation.GetString("Enable_highway_specific_lane_merging/splitting_rules")+" (BETA feature)", TMDataManager.Options.highwayRules, onHighwayRulesChanged) as UICheckBox;
			laneChangingRandomizationDropdown = aiGroup.AddDropdown(Translation.GetString("Drivers_want_to_change_lanes_(only_applied_if_Advanced_AI_is_enabled):"), new string[] { Translation.GetString("Very_often") + " (50 %)", Translation.GetString("Often") + " (25 %)", Translation.GetString("Sometimes") + " (10 %)", Translation.GetString("Rarely") + " (5 %)", Translation.GetString("Very_rarely") + " (2.5 %)", Translation.GetString("Only_if_necessary") }, TMDataManager.Options.laneChangingRandomization, onLaneChangingRandomizationChanged) as UIDropDown;
			overlayGroup = helper.AddGroup(Translation.GetString("Persistently_visible_overlays"));
			prioritySignsOverlayToggle = overlayGroup.AddCheckbox(Translation.GetString("Priority_signs"), TMDataManager.Options.prioritySignsOverlay, onPrioritySignsOverlayChanged) as UICheckBox;
			timedLightsOverlayToggle = overlayGroup.AddCheckbox(Translation.GetString("Timed_traffic_lights"), TMDataManager.Options.timedLightsOverlay, onTimedLightsOverlayChanged) as UICheckBox;
			speedLimitsOverlayToggle = overlayGroup.AddCheckbox(Translation.GetString("Speed_limits"), TMDataManager.Options.speedLimitsOverlay, onSpeedLimitsOverlayChanged) as UICheckBox;
			vehicleRestrictionsOverlayToggle = overlayGroup.AddCheckbox(Translation.GetString("Vehicle_restrictions"), TMDataManager.Options.vehicleRestrictionsOverlay, onVehicleRestrictionsOverlayChanged) as UICheckBox;
			nodesOverlayToggle = overlayGroup.AddCheckbox(Translation.GetString("Nodes_and_segments"), TMDataManager.Options.nodesOverlay, onNodesOverlayChanged) as UICheckBox;
			showLanesToggle = overlayGroup.AddCheckbox(Translation.GetString("Lanes"), TMDataManager.Options.showLanes, onShowLanesChanged) as UICheckBox;
			maintenanceGroup = helper.AddGroup(Translation.GetString("Maintenance"));
			forgetTrafficLightsBtn = maintenanceGroup.AddButton(Translation.GetString("Forget_toggled_traffic_lights"), onClickForgetToggledLights) as UIButton;
#if DEBUG
			disableSomething1Toggle = maintenanceGroup.AddCheckbox("Enable path-finding debugging", TMDataManager.Options.disableSomething1, onDisableSomething1Changed) as UICheckBox;
			disableSomething2Toggle = maintenanceGroup.AddCheckbox("Disable something #2", TMDataManager.Options.disableSomething2, onDisableSomething2Changed) as UICheckBox;
			disableSomething3Toggle = maintenanceGroup.AddCheckbox("Disable something #3", TMDataManager.Options.disableSomething3, onDisableSomething3Changed) as UICheckBox;
			disableSomething4Toggle = maintenanceGroup.AddCheckbox("Disable something #4", TMDataManager.Options.disableSomething4, onDisableSomething4Changed) as UICheckBox;
			disableSomething5Toggle = maintenanceGroup.AddCheckbox("Disable something #5", TMDataManager.Options.disableSomething5, onDisableSomething5Changed) as UICheckBox;
			pathCostMultiplicatorField = maintenanceGroup.AddTextfield("Pathcost multiplicator (mult)", String.Format("{0:0.##}", TMDataManager.Options.pathCostMultiplicator), onPathCostMultiplicatorChanged) as UITextField;
			pathCostMultiplicator2Field = maintenanceGroup.AddTextfield("Pathcost multiplicator (div)", String.Format("{0:0.##}", TMDataManager.Options.pathCostMultiplicator2), onPathCostMultiplicator2Changed) as UITextField;
			someValueField = maintenanceGroup.AddTextfield("Some value #1", String.Format("{0:0.##}", TMDataManager.Options.someValue), onSomeValueChanged) as UITextField;
			someValue2Field = maintenanceGroup.AddTextfield("Some value #2", String.Format("{0:0.##}", TMDataManager.Options.someValue2), onSomeValue2Changed) as UITextField;
			someValue3Field = maintenanceGroup.AddTextfield("Some value #3", String.Format("{0:0.##}", TMDataManager.Options.someValue3), onSomeValue3Changed) as UITextField;
			someValue4Field = maintenanceGroup.AddTextfield("Some value #4", String.Format("{0:0.##}", TMDataManager.Options.someValue4), onSomeValue4Changed) as UITextField;
#endif
		}

		private static bool checkGameLoaded() {
			if (!TMDataSerializer.StateLoading && !ToolModuleV2.IsGameLoaded()) {
				UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Nope!", Translation.GetString("Settings_are_defined_for_each_savegame_separately") + ". https://www.viathinksoft.de/tmpe/#options", false);
				return false;
			}
			return true;
		}

		private static void onPrioritySignsOverlayChanged(bool newPrioritySignsOverlay) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"prioritySignsOverlay changed to {newPrioritySignsOverlay}");
			TMDataManager.Options.prioritySignsOverlay = newPrioritySignsOverlay;
		}

		private static void onTimedLightsOverlayChanged(bool newTimedLightsOverlay) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"timedLightsOverlay changed to {newTimedLightsOverlay}");
			TMDataManager.Options.timedLightsOverlay = newTimedLightsOverlay;
		}

		private static void onSpeedLimitsOverlayChanged(bool newSpeedLimitsOverlay) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"speedLimitsOverlay changed to {newSpeedLimitsOverlay}");
			TMDataManager.Options.speedLimitsOverlay = newSpeedLimitsOverlay;
		}

		private static void onVehicleRestrictionsOverlayChanged(bool newVehicleRestrictionsOverlay) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"vehicleRestrictionsOverlay changed to {newVehicleRestrictionsOverlay}");
			TMDataManager.Options.vehicleRestrictionsOverlay = newVehicleRestrictionsOverlay;
		}

		private static void onDisableSomething1Changed(bool newDisableSomething) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"disableSomething1 changed to {newDisableSomething}");
			TMDataManager.Options.disableSomething1 = newDisableSomething;
		}

		private static void onDisableSomething2Changed(bool newDisableSomething) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"disableSomething2 changed to {newDisableSomething}");
			TMDataManager.Options.disableSomething2 = newDisableSomething;
		}

		private static void onDisableSomething3Changed(bool newDisableSomething) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"disableSomething3 changed to {newDisableSomething}");
			TMDataManager.Options.disableSomething3 = newDisableSomething;
		}

		private static void onDisableSomething4Changed(bool newDisableSomething) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"disableSomething4 changed to {newDisableSomething}");
			TMDataManager.Options.disableSomething4 = newDisableSomething;
		}

		private static void onDisableSomething5Changed(bool newDisableSomething) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"disableSomething5 changed to {newDisableSomething}");
			TMDataManager.Options.disableSomething5 = newDisableSomething;
		}

		private static void onSimAccuracyChanged(int newAccuracy) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"Simulation accuracy changed to {newAccuracy}");
			TMDataManager.Options.simAccuracy = newAccuracy;
		}

		private static void onLaneChangingRandomizationChanged(int newLaneChangingRandomization) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"Lane changing frequency changed to {newLaneChangingRandomization}");
			TMDataManager.Options.laneChangingRandomization = newLaneChangingRandomization;
		}

		private static void onRecklessDriversChanged(int newRecklessDrivers) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"Reckless driver amount changed to {newRecklessDrivers}");
			TMDataManager.Options.recklessDrivers = newRecklessDrivers;
		}

		private static void onRelaxedBussesChanged(bool newRelaxedBusses) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"Relaxed busses changed to {newRelaxedBusses}");
			TMDataManager.Options.relaxedBusses = newRelaxedBusses;
		}

		private static void onAllRelaxedChanged(bool newAllRelaxed) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"All relaxed changed to {newAllRelaxed}");
			TMDataManager.Options.allRelaxed = newAllRelaxed;
		}

		private static void onAdvancedAIChanged(bool newAdvancedAI) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"advancedAI changed to {newAdvancedAI}");
				setAdvancedAI(newAdvancedAI);
		}

		private static void onHighwayRulesChanged(bool newHighwayRules) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"Highway rules changed to {newHighwayRules}");
			TMDataManager.Options.highwayRules = newHighwayRules;
			Flags.clearHighwayLaneArrows();
			Flags.applyAllFlags();
			if (newHighwayRules)
				setAdvancedAI(true);
		}

		private static void onDynamicPathRecalculationChanged(bool value) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"dynamicPathRecalculation changed to {value}");
			TMDataManager.Options.dynamicPathRecalculation = value;
			if (value)
				setAdvancedAI(true);
		}

		private static void onAllowEnterBlockedJunctionsChanged(bool newMayEnterBlockedJunctions) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"allowEnterBlockedJunctions changed to {newMayEnterBlockedJunctions}");
			TMDataManager.Options.allowEnterBlockedJunctions = newMayEnterBlockedJunctions;
		}

		private static void onAllowUTurnsChanged(bool newValue) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"allowUTurns changed to {newValue}");
			TMDataManager.Options.allowUTurns = newValue;
		}

		private static void onAllowLaneChangesWhileGoingStraightChanged(bool newValue) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"allowLaneChangesWhileGoingStraight changed to {newValue}");
			TMDataManager.Options.allowLaneChangesWhileGoingStraight = newValue;
		}

		private static void onStrongerRoadConditionEffectsChanged(bool newStrongerRoadConditionEffects) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"strongerRoadConditionEffects changed to {newStrongerRoadConditionEffects}");
			TMDataManager.Options.strongerRoadConditionEffects = newStrongerRoadConditionEffects;
		}

		private static void onEnableDespawningChanged(bool value) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"enableDespawning changed to {value}");
			TMDataManager.Options.enableDespawning = value;
		}

		private static void onNodesOverlayChanged(bool newNodesOverlay) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"Nodes overlay changed to {newNodesOverlay}");
			TMDataManager.Options.nodesOverlay = newNodesOverlay;
		}

		private static void onShowLanesChanged(bool newShowLanes) {
			if (!checkGameLoaded())
				return;

			Log._Debug($"Show lanes changed to {newShowLanes}");
			TMDataManager.Options.showLanes = newShowLanes;
		}

		private static void onPathCostMultiplicatorChanged(string newPathCostMultiplicatorStr) {
			if (!checkGameLoaded())
				return;

			try {
				float newPathCostMultiplicator = Single.Parse(newPathCostMultiplicatorStr);
				TMDataManager.Options.pathCostMultiplicator = newPathCostMultiplicator;
			} catch (Exception e) {
				Log.Warning($"An invalid value was inserted: '{newPathCostMultiplicatorStr}'. Error: {e.ToString()}");
                //UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Invalid value", "An invalid value was inserted.", false);
			}
		}

		private static void onPathCostMultiplicator2Changed(string newPathCostMultiplicatorStr) {
			if (!checkGameLoaded())
				return;

			try {
				float newPathCostMultiplicator = Single.Parse(newPathCostMultiplicatorStr);
				TMDataManager.Options.pathCostMultiplicator2 = newPathCostMultiplicator;
			} catch (Exception e) {
				Log.Warning($"An invalid value was inserted: '{newPathCostMultiplicatorStr}'. Error: {e.ToString()}");
				//UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Invalid value", "An invalid value was inserted.", false);
			}
		}

		private static void onSomeValueChanged(string newSomeValueStr) {
			if (!checkGameLoaded())
				return;

			try {
				float newSomeValue = Single.Parse(newSomeValueStr);
				TMDataManager.Options.someValue = newSomeValue;
			} catch (Exception e) {
				Log.Warning($"An invalid value was inserted: '{newSomeValueStr}'. Error: {e.ToString()}");
				//UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Invalid value", "An invalid value was inserted.", false);
			}
		}

		private static void onSomeValue2Changed(string newSomeValueStr) {
			if (!checkGameLoaded())
				return;

			try {
				float newSomeValue = Single.Parse(newSomeValueStr);
				TMDataManager.Options.someValue2 = newSomeValue;
			} catch (Exception e) {
				Log.Warning($"An invalid value was inserted: '{newSomeValueStr}'. Error: {e.ToString()}");
				//UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Invalid value", "An invalid value was inserted.", false);
			}
		}

		private static void onSomeValue3Changed(string newSomeValueStr) {
			if (!checkGameLoaded())
				return;

			try {
				float newSomeValue = Single.Parse(newSomeValueStr);
				TMDataManager.Options.someValue3 = newSomeValue;
			} catch (Exception e) {
				Log.Warning($"An invalid value was inserted: '{newSomeValueStr}'. Error: {e.ToString()}");
				//UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Invalid value", "An invalid value was inserted.", false);
			}
		}

		private static void onSomeValue4Changed(string newSomeValueStr) {
			if (!checkGameLoaded())
				return;

			try {
				float newSomeValue = Single.Parse(newSomeValueStr);
				TMDataManager.Options.someValue4 = newSomeValue;
			} catch (Exception e) {
				Log.Warning($"An invalid value was inserted: '{newSomeValueStr}'. Error: {e.ToString()}");
				//UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Invalid value", "An invalid value was inserted.", false);
			}
		}

		private static void onClickForgetToggledLights() {
			if (!checkGameLoaded())
				return;

			Flags.resetTrafficLights(false);
		}

		public static void setSimAccuracy(int newAccuracy) {
			TMDataManager.Options.simAccuracy = newAccuracy;
			if (simAccuracyDropdown != null)
				simAccuracyDropdown.selectedIndex = newAccuracy;
		}

		public static void setLaneChangingRandomization(int newLaneChangingRandomization) {
			TMDataManager.Options.laneChangingRandomization = newLaneChangingRandomization;
			if (laneChangingRandomizationDropdown != null)
				laneChangingRandomizationDropdown.selectedIndex = newLaneChangingRandomization;
		}

		public static void setRecklessDrivers(int newRecklessDrivers) {
			TMDataManager.Options.recklessDrivers = newRecklessDrivers;
			if (recklessDriversDropdown != null)
				recklessDriversDropdown.selectedIndex = newRecklessDrivers;
		}

#if DEBUG
		public static void setPathCostMultiplicator(float newPathCostMultiplicator) {
			TMDataManager.Options.pathCostMultiplicator = newPathCostMultiplicator;
			if (pathCostMultiplicatorField != null)
				pathCostMultiplicatorField.text = newPathCostMultiplicator.ToString();
		}
#endif

		public static void setRelaxedBusses(bool newRelaxedBusses) {
			TMDataManager.Options.relaxedBusses = newRelaxedBusses;
			if (relaxedBussesToggle != null)
				relaxedBussesToggle.isChecked = newRelaxedBusses;
		}

		public static void setAllRelaxed(bool newAllRelaxed) {
			TMDataManager.Options.allRelaxed = newAllRelaxed;
			if (allRelaxedToggle != null)
				allRelaxedToggle.isChecked = newAllRelaxed;
		}

		public static void setHighwayRules(bool newHighwayRules) {
			TMDataManager.Options.highwayRules = newHighwayRules;
			if (highwayRulesToggle != null)
				highwayRulesToggle.isChecked = newHighwayRules;
		}

		public static void setShowLanes(bool newShowLanes) {
			TMDataManager.Options.showLanes = newShowLanes;
			if (showLanesToggle != null)
				showLanesToggle.isChecked = newShowLanes;
		}

		public static void setAdvancedAI(bool newAdvancedAI) {
			TMDataManager.Options.advancedAI = newAdvancedAI;

			if (advancedAIToggle != null)
				advancedAIToggle.isChecked = newAdvancedAI;

			if (!newAdvancedAI) {
				setDynamicPathRecalculation(false);
				setHighwayRules(false);
			}
		}

		public static void setDynamicPathRecalculation(bool value) {
			TMDataManager.Options.dynamicPathRecalculation = value;

			if (dynamicPathRecalculationToggle != null)
				dynamicPathRecalculationToggle.isChecked = value;
		}

		public static void setMayEnterBlockedJunctions(bool newMayEnterBlockedJunctions) {
			TMDataManager.Options.allowEnterBlockedJunctions = newMayEnterBlockedJunctions;
			if (allowEnterBlockedJunctionsToggle != null)
				allowEnterBlockedJunctionsToggle.isChecked = newMayEnterBlockedJunctions;
		}

		public static void setStrongerRoadConditionEffects(bool newStrongerRoadConditionEffects) {
			TMDataManager.Options.strongerRoadConditionEffects = newStrongerRoadConditionEffects;
			if (strongerRoadConditionEffectsToggle != null)
				strongerRoadConditionEffectsToggle.isChecked = newStrongerRoadConditionEffects;
		}

		public static void setEnableDespawning(bool value) {
			TMDataManager.Options.enableDespawning = value;

			if (enableDespawningToggle != null)
				enableDespawningToggle.isChecked = value;
		}

		public static void setAllowUTurns(bool value) {
			TMDataManager.Options.allowUTurns = value;
			if (allowUTurnsToggle != null)
				allowUTurnsToggle.isChecked = value;
		}

		public static void setAllowLaneChangesWhileGoingStraight(bool value) {
			TMDataManager.Options.allowLaneChangesWhileGoingStraight = value;
			if (allowLaneChangesWhileGoingStraightToggle != null)
				allowLaneChangesWhileGoingStraightToggle.isChecked = value;
		}

		public static void setPrioritySignsOverlay(bool newPrioritySignsOverlay) {
			TMDataManager.Options.prioritySignsOverlay = newPrioritySignsOverlay;
			if (prioritySignsOverlayToggle != null)
				prioritySignsOverlayToggle.isChecked = newPrioritySignsOverlay;
		}

		public static void setTimedLightsOverlay(bool newTimedLightsOverlay) {
			TMDataManager.Options.timedLightsOverlay = newTimedLightsOverlay;
			if (timedLightsOverlayToggle != null)
				timedLightsOverlayToggle.isChecked = newTimedLightsOverlay;
		}

		public static void setSpeedLimitsOverlay(bool newSpeedLimitsOverlay) {
			TMDataManager.Options.speedLimitsOverlay = newSpeedLimitsOverlay;
			if (speedLimitsOverlayToggle != null)
				speedLimitsOverlayToggle.isChecked = newSpeedLimitsOverlay;
		}

		public static void setVehicleRestrictionsOverlay(bool newVehicleRestrictionsOverlay) {
			TMDataManager.Options.vehicleRestrictionsOverlay = newVehicleRestrictionsOverlay;
			if (vehicleRestrictionsOverlayToggle != null)
				vehicleRestrictionsOverlayToggle.isChecked = newVehicleRestrictionsOverlay;
		}

		public static void setNodesOverlay(bool newNodesOverlay) {
			TMDataManager.Options.nodesOverlay = newNodesOverlay;
			if (nodesOverlayToggle != null)
				nodesOverlayToggle.isChecked = newNodesOverlay;
		}
	}
}
