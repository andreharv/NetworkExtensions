using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.Traffic;
using Transit.Addon.TM.TrafficLight;

namespace Transit.Addon.TM.PathFindingFeatures {
	public static partial class TMDataManager {
		private static void ApplyConfiguration(TMConfigurationV2 configuration) {
			Log.Info("Loading State from Config");
			if (configuration == null) {
				Log.Warning("Configuration NULL, Couldn't load save data. Possibly a new game?");
				return;
			}

			// load priority segments
			if (configuration.PrioritySegments != null) {
				Log.Info($"Loading {configuration.PrioritySegments.Count()} priority segments");
				foreach (var segment in configuration.PrioritySegments) {
					if (segment.Length < 3)
						continue;
#if DEBUG
					bool debug = segment[0] == 13630;
#endif

					if ((SegmentEnd.PriorityType)segment[2] == SegmentEnd.PriorityType.None) {
#if DEBUG
						if (debug)
							Log._Debug($"Loading priority segment: Not adding 'None' priority segment: {segment[1]} @ node {segment[0]}");
#endif
						continue;
					}

					if ((Singleton<NetManager>.instance.m_nodes.m_buffer[segment[0]].m_flags & NetNode.Flags.Created) == NetNode.Flags.None) {
#if DEBUG
						if (debug)
							Log._Debug($"Loading priority segment: node {segment[0]} is invalid");
#endif
						continue;
					}
					if ((Singleton<NetManager>.instance.m_segments.m_buffer[segment[1]].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None) {
#if DEBUG
						if (debug)
							Log._Debug($"Loading priority segment: segment {segment[1]} @ node {segment[0]} is invalid");
#endif
						continue;
					}
					if (TrafficPriority.IsPrioritySegment((ushort)segment[0], (ushort)segment[1])) {
#if DEBUG
						if (debug)
							Log._Debug($"Loading priority segment: segment {segment[1]} @ node {segment[0]} is already a priority segment");
#endif
						TrafficPriority.GetPrioritySegment((ushort)segment[0], (ushort)segment[1]).Type = (SegmentEnd.PriorityType)segment[2];
						continue;
					}
#if DEBUG
					Log._Debug($"Adding Priority Segment of type: {segment[2].ToString()} to segment {segment[1]} @ node {segment[0]}");
#endif
					TrafficPriority.AddPrioritySegment((ushort)segment[0], (ushort)segment[1], (SegmentEnd.PriorityType)segment[2]);
				}
			} else {
				Log.Warning("Priority segments data structure undefined!");
			}

			// load vehicle restrictions (warning: has to be done before loading timed lights!)
			if (configuration.LaneAllowedVehicleTypes != null) {
				Log.Info($"Loading lane vehicle restriction data. {configuration.LaneAllowedVehicleTypes.Count} elements");
				foreach (TMConfigurationV2.LaneVehicleTypes laneVehicleTypes in configuration.LaneAllowedVehicleTypes) {
					TMVehicleType maskedType = laneVehicleTypes.vehicleTypes & VehicleRestrictionsManager.GetDefaultAllowedVehicleTypes(laneVehicleTypes.laneId);
					Log._Debug($"Loading lane vehicle restriction: lane {laneVehicleTypes.laneId} = {laneVehicleTypes.vehicleTypes}, masked = {maskedType}");
					Flags.setLaneAllowedVehicleTypes(laneVehicleTypes.laneId, maskedType);
				}
			} else {
				Log.Warning("Lane speed limit structure undefined!");
			}

			var timedStepCount = 0;
			var timedStepSegmentCount = 0;

			NetManager netManager = Singleton<NetManager>.instance;

			if (configuration.TimedLights != null) {
				Log.Info($"Loading {configuration.TimedLights.Count()} timed traffic lights (new method)");

				foreach (TMConfigurationV2.TimedTrafficLights cnfTimedLights in configuration.TimedLights) {
					if ((Singleton<NetManager>.instance.m_nodes.m_buffer[cnfTimedLights.nodeId].m_flags & NetNode.Flags.Created) == NetNode.Flags.None)
						continue;
					Flags.setNodeTrafficLight(cnfTimedLights.nodeId, true);

					Log._Debug($"Adding Timed Node at node {cnfTimedLights.nodeId}");

					TrafficLightSimulation sim = TrafficLightSimulation.AddNodeToSimulation(cnfTimedLights.nodeId);
					sim.SetupTimedTrafficLight(cnfTimedLights.nodeGroup);
					var timedNode = sim.TimedLight;

					int j = 0;
					foreach (TMConfigurationV2.TimedTrafficLightsStep cnfTimedStep in cnfTimedLights.timedSteps) {
						Log._Debug($"Loading timed step {j} at node {cnfTimedLights.nodeId}");
						TimedTrafficLightsStep step = timedNode.AddStep(cnfTimedStep.minTime, cnfTimedStep.maxTime, cnfTimedStep.waitFlowBalance);

						foreach (KeyValuePair<ushort, TMConfigurationV2.CustomSegmentLights> e in cnfTimedStep.segmentLights) {
							Log._Debug($"Loading timed step {j}, segment {e.Key} at node {cnfTimedLights.nodeId}");
							CustomSegmentLights lights = null;
							if (!step.segmentLights.TryGetValue(e.Key, out lights)) {
								Log._Debug($"No segment lights found at timed step {j} for segment {e.Key}, node {cnfTimedLights.nodeId}");
								continue;
							}
							TMConfigurationV2.CustomSegmentLights cnfLights = e.Value;

							Log._Debug($"Loading pedestrian light @ seg. {e.Key}, step {j}: {cnfLights.pedestrianLightState} {cnfLights.manualPedestrianMode}");

							lights.ManualPedestrianMode = cnfLights.manualPedestrianMode;
							lights.PedestrianLightState = cnfLights.pedestrianLightState;

							foreach (KeyValuePair<TMVehicleType, TMConfigurationV2.CustomSegmentLight> e2 in cnfLights.customLights) {
								Log._Debug($"Loading timed step {j}, segment {e.Key}, vehicleType {e2.Key} at node {cnfTimedLights.nodeId}");
								CustomSegmentLight light = null;
								if (!lights.CustomLights.TryGetValue(e2.Key, out light)) {
									Log._Debug($"No segment light found for timed step {j}, segment {e.Key}, vehicleType {e2.Key} at node {cnfTimedLights.nodeId}");
									continue;
								}
								TMConfigurationV2.CustomSegmentLight cnfLight = e2.Value;

								light.CurrentMode = (CustomSegmentLight.Mode)cnfLight.currentMode;
								light.LightLeft = cnfLight.leftLight;
								light.LightMain = cnfLight.mainLight;
								light.LightRight = cnfLight.rightLight;
							}
						}
						++j;
					}

					if (cnfTimedLights.started)
						timedNode.Start();
				}
			} else if (configuration.TimedNodes != null && configuration.TimedNodeGroups != null) {
				Log.Info($"Loading {configuration.TimedNodes.Count()} timed traffic lights (old method)");
				for (var i = 0; i < configuration.TimedNodes.Count; i++) {
					try {
						var nodeid = (ushort)configuration.TimedNodes[i][0];
						if ((Singleton<NetManager>.instance.m_nodes.m_buffer[nodeid].m_flags & NetNode.Flags.Created) == NetNode.Flags.None)
							continue;
						Flags.setNodeTrafficLight(nodeid, true);

						Log._Debug($"Adding Timed Node {i} at node {nodeid}");

						var nodeGroup = new List<ushort>();
						for (var j = 0; j < configuration.TimedNodeGroups[i].Length; j++) {
							nodeGroup.Add(configuration.TimedNodeGroups[i][j]);
						}

						TrafficLightSimulation sim = TrafficLightSimulation.AddNodeToSimulation(nodeid);
						sim.SetupTimedTrafficLight(nodeGroup);
						var timedNode = sim.TimedLight;

						timedNode.CurrentStep = configuration.TimedNodes[i][1];

						for (var j = 0; j < configuration.TimedNodes[i][2]; j++) {
							var cfgstep = configuration.TimedNodeSteps[timedStepCount];
							// old (pre 1.3.0):
							//   cfgstep[0]: time of step
							//   cfgstep[1]: number of segments
							// new (post 1.3.0):
							//   cfgstep[0]: min. time of step
							//   cfgstep[1]: max. time of step
							//   cfgstep[2]: number of segments

							int minTime = 1;
							int maxTime = 1;
							//int numSegments = 0;
							float waitFlowBalance = 1f;

							if (cfgstep.Length == 2) {
								minTime = cfgstep[0];
								maxTime = cfgstep[0];
								//numSegments = cfgstep[1];
							} else if (cfgstep.Length >= 3) {
								minTime = cfgstep[0];
								maxTime = cfgstep[1];
								//numSegments = cfgstep[2];
								if (cfgstep.Length == 4) {
									waitFlowBalance = Convert.ToSingle(cfgstep[3]) / 10f;
								}
								if (cfgstep.Length == 5) {
									waitFlowBalance = Convert.ToSingle(cfgstep[4]) / 1000f;
								}
							}

							Log._Debug($"Adding timed step to node {nodeid}: min/max: {minTime}/{maxTime}, waitFlowBalance: {waitFlowBalance}");

							timedNode.AddStep(minTime, maxTime, waitFlowBalance);
							var step = timedNode.Steps[j];

							for (var s = 0; s < 8; s++) {
								var segmentId = netManager.m_nodes.m_buffer[nodeid].GetSegment(s);
								if (segmentId <= 0)
									continue;

								bool tooFewSegments = (timedStepSegmentCount >= configuration.TimedNodeStepSegments.Count);

								var leftLightState = tooFewSegments ? RoadBaseAI.TrafficLightState.Red : (RoadBaseAI.TrafficLightState)configuration.TimedNodeStepSegments[timedStepSegmentCount][0];
								var mainLightState = tooFewSegments ? RoadBaseAI.TrafficLightState.Red : (RoadBaseAI.TrafficLightState)configuration.TimedNodeStepSegments[timedStepSegmentCount][1];
								var rightLightState = tooFewSegments ? RoadBaseAI.TrafficLightState.Red : (RoadBaseAI.TrafficLightState)configuration.TimedNodeStepSegments[timedStepSegmentCount][2];
								var pedLightState = tooFewSegments ? RoadBaseAI.TrafficLightState.Red : (RoadBaseAI.TrafficLightState)configuration.TimedNodeStepSegments[timedStepSegmentCount][3];
								CustomSegmentLight.Mode? mode = null;
								if (configuration.TimedNodeStepSegments[timedStepSegmentCount].Length >= 5) {
									mode = (CustomSegmentLight.Mode)configuration.TimedNodeStepSegments[timedStepSegmentCount][4];
								}

								foreach (KeyValuePair<TMVehicleType, CustomSegmentLight> e in step.segmentLights[segmentId].CustomLights) {
									//ManualSegmentLight segmentLight = new ManualSegmentLight(step.NodeId, step.segmentIds[k], mainLightState, leftLightState, rightLightState, pedLightState);
									e.Value.LightLeft = leftLightState;
									e.Value.LightMain = mainLightState;
									e.Value.LightRight = rightLightState;
									if (mode != null)
										e.Value.CurrentMode = (CustomSegmentLight.Mode)mode;
								}

								if (step.segmentLights[segmentId].PedestrianLightState != null)
									step.segmentLights[segmentId].PedestrianLightState = pedLightState;

								timedStepSegmentCount++;
							}
							timedStepCount++;
						}

						if (Convert.ToBoolean(configuration.TimedNodes[i][3])) {
							timedNode.Start();
						}
					} catch (Exception e) {
						// ignore, as it's probably corrupt save data. it'll be culled on next save
						Log.Warning("Error loading data from the TimedNodes: " + e.ToString());
					}
				}
			} else {
				Log.Warning("Timed traffic lights data structure undefined!");
			}

			var trafficLightDefs = configuration.NodeTrafficLights.Split(',');

			Log.Info($"Loading junction traffic light data");
			if (trafficLightDefs.Length <= 1) {
				// old method
				Log.Info($"Using old method to load traffic light data");

				var saveDataIndex = 0;
				for (var i = 0; i < Singleton<NetManager>.instance.m_nodes.m_buffer.Length; i++) {
					//Log.Message($"Adding NodeTrafficLights iteration: {i1}");
					try {
						if ((Singleton<NetManager>.instance.m_nodes.m_buffer[i].Info.m_class.m_service != ItemClass.Service.Road &&
							Singleton<NetManager>.instance.m_nodes.m_buffer[i].Info.m_class.m_service != ItemClass.Service.PublicTransport) ||
							(Singleton<NetManager>.instance.m_nodes.m_buffer[i].m_flags & NetNode.Flags.Created) == NetNode.Flags.None)
							continue;

						// prevent overflow
						if (configuration.NodeTrafficLights.Length > saveDataIndex) {
							var trafficLight = configuration.NodeTrafficLights[saveDataIndex];
#if DEBUG
							Log._Debug("Setting traffic light flag for node " + i + ": " + (trafficLight == '1'));
#endif
							Flags.setNodeTrafficLight((ushort)i, trafficLight == '1');
						}
						++saveDataIndex;
					} catch (Exception e) {
						// ignore as it's probably bad save data.
						Log.Warning("Error setting the NodeTrafficLights (old): " + e.Message);
					}
				}
			} else {
				// new method
				foreach (var split in trafficLightDefs.Select(def => def.Split(':')).Where(split => split.Length > 1)) {
					try {
						Log.Info($"Traffic light split data: {split[0]} , {split[1]}");
						var nodeId = Convert.ToUInt16(split[0]);
						uint flag = Convert.ToUInt16(split[1]);

						Flags.setNodeTrafficLight(nodeId, flag > 0);
					} catch (Exception e) {
						// ignore as it's probably bad save data.
						Log.Warning("Error setting the NodeTrafficLights (new): " + e.Message);
					}
				}
			}

			if (configuration.LaneFlags != null) {
				Log.Info($"Loading lane arrow data");
#if DEBUG
				Log._Debug($"LaneFlags: {configuration.LaneFlags}");
#endif
				var lanes = configuration.LaneFlags.Split(',');

				if (lanes.Length > 1) {
					foreach (var split in lanes.Select(lane => lane.Split(':')).Where(split => split.Length > 1)) {
						try {
							Log.Info($"Split Data: {split[0]} , {split[1]}");
							var laneId = Convert.ToUInt32(split[0]);
							uint flags = Convert.ToUInt32(split[1]);

							//make sure we don't cause any overflows because of bad save data.
							if (Singleton<NetManager>.instance.m_lanes.m_buffer.Length <= laneId)
								continue;

							if (flags > UInt16.MaxValue)
								continue;

							if ((Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags & (ushort)NetLane.Flags.Created) == 0 || Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_segment == 0)
								continue;

							Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags = FixLaneFlags(Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags);

							uint laneArrowFlags = flags & Flags.lfr;
							uint origFlags = (Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags & Flags.lfr);
#if DEBUG
							Log._Debug("Setting flags for lane " + laneId + " to " + flags + " (" + ((Flags.LaneArrows)(laneArrowFlags)).ToString() + ")");
							if ((origFlags | laneArrowFlags) == origFlags) { // only load if setting differs from default
								Log._Debug("Flags for lane " + laneId + " are original (" + ((NetLane.Flags)(origFlags)).ToString() + ")");
							}
#endif
							Flags.setLaneArrowFlags(laneId, (Flags.LaneArrows)(laneArrowFlags));
						} catch (Exception e) {
							Log.Error($"Error loading Lane Split data. Length: {split.Length} value: {split}\nError: {e.Message}");
						}
					}
				}
			} else {
				Log.Warning("Lane arrow data structure undefined!");
			}

			// load speed limits
			if (configuration.LaneSpeedLimits != null) {
				Log.Info($"Loading lane speed limit data. {configuration.LaneSpeedLimits.Count} elements");
				foreach (TMConfigurationV2.LaneSpeedLimit laneSpeedLimit in configuration.LaneSpeedLimits) {
					Log._Debug($"Loading lane speed limit: lane {laneSpeedLimit.laneId} = {laneSpeedLimit.speedLimit}");
					Flags.setLaneSpeedLimit(laneSpeedLimit.laneId, laneSpeedLimit.speedLimit);
				}
			} else {
				Log.Warning("Lane speed limit structure undefined!");
			}

			// Load segment-at-node flags
			if (configuration.SegmentNodeConfs != null) {
				Log.Info($"Loading segment-at-node data. {configuration.SegmentNodeConfs.Count} elements");
				foreach (TMConfigurationV2.SegmentNodeConf segNodeConf in configuration.SegmentNodeConfs) {
					if ((Singleton<NetManager>.instance.m_segments.m_buffer[segNodeConf.segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
						continue;
					Flags.setSegmentNodeFlags(segNodeConf.segmentId, true, segNodeConf.startNodeFlags);
					Flags.setSegmentNodeFlags(segNodeConf.segmentId, false, segNodeConf.endNodeFlags);
				}
			} else {
				Log.Warning("Segment-at-node structure undefined!");
			}
		}

		private static ushort FixLaneFlags(ushort flags) {
			ushort ret = 0;
			if ((flags & (ushort)NetLane.Flags.Created) != 0)
				ret |= (ushort)NetLane.Flags.Created;
			if ((flags & (ushort)NetLane.Flags.Deleted) != 0)
				ret |= (ushort)NetLane.Flags.Deleted;
			if ((flags & (ushort)NetLane.Flags.Inverted) != 0)
				ret |= (ushort)NetLane.Flags.Inverted;
			if ((flags & (ushort)NetLane.Flags.JoinedJunction) != 0)
				ret |= (ushort)NetLane.Flags.JoinedJunction;
			if ((flags & (ushort)NetLane.Flags.Forward) != 0)
				ret |= (ushort)NetLane.Flags.Forward;
			if ((flags & (ushort)NetLane.Flags.Left) != 0)
				ret |= (ushort)NetLane.Flags.Left;
			if ((flags & (ushort)NetLane.Flags.Right) != 0)
				ret |= (ushort)NetLane.Flags.Right;
			if ((flags & (ushort)NetLane.Flags.Stop) != 0)
				ret |= (ushort)NetLane.Flags.Stop;
			if ((flags & (ushort)NetLane.Flags.StartOneWayLeft) != 0)
				ret |= (ushort)NetLane.Flags.StartOneWayLeft;
			if ((flags & (ushort)NetLane.Flags.StartOneWayRight) != 0)
				ret |= (ushort)NetLane.Flags.StartOneWayRight;
			if ((flags & (ushort)NetLane.Flags.EndOneWayLeft) != 0)
				ret |= (ushort)NetLane.Flags.EndOneWayLeft;
			if ((flags & (ushort)NetLane.Flags.EndOneWayRight) != 0)
				ret |= (ushort)NetLane.Flags.EndOneWayRight;
			return ret;
		}
	}
}
