using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Addon.TM.Traffic;
using Transit.Addon.TM.TrafficLight;
using Transit.Framework;

namespace Transit.Addon.TM.Data {
	public static partial class TMDataManager {
		private static void ApplyConfiguration(TMConfigurationV2 configuration) {
			Log.Info("Loading TM:PE State from Config");
			if (configuration == null) {
				Log.Warning("Configuration NULL, Couldn't load save data. Possibly a new game?");
				return;
			}

			NetManager netManager = Singleton<NetManager>.instance;

			// Load segment flags
			if (configuration.SegmentConfs != null) {
				Log.Info($"Loading segment data. {configuration.SegmentConfs.Count} elements");
				foreach (TMConfigurationV2.SegmentConf segmentConf in configuration.SegmentConfs) {
					if ((netManager.m_segments.m_buffer[segmentConf.segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
						continue;

					if (segmentConf.startNodeConf != null) {
						ushort startNodeId = netManager.m_segments.m_buffer[segmentConf.segmentId].m_startNode;
						if (startNodeId != 0u && (netManager.m_nodes.m_buffer[startNodeId].m_flags & NetNode.Flags.Created) != NetNode.Flags.None) {
							TMConfigurationV2.SegmentEndConf segAtStartNodeConf = segmentConf.startNodeConf;

							if (segAtStartNodeConf.flags != null) {
								// load segment end flags at start node
								Flags.SetSegmentEndFlags(segmentConf.segmentId, true, segAtStartNodeConf.flags);
							}

							if (segAtStartNodeConf.priorityType != null) {
								// load priority sign configuration
								TrafficPriority.AddPrioritySegment(startNodeId, segmentConf.segmentId, (SegmentEnd.PriorityType)segAtStartNodeConf.priorityType);
							}
						}
					}

					if (segmentConf.endNodeConf != null) {
						ushort endNodeId = netManager.m_segments.m_buffer[segmentConf.segmentId].m_endNode;
						if (endNodeId != 0u && (netManager.m_nodes.m_buffer[endNodeId].m_flags & NetNode.Flags.Created) != NetNode.Flags.None) {
							TMConfigurationV2.SegmentEndConf segAtEndNodeConf = segmentConf.endNodeConf;

							if (segAtEndNodeConf.flags != null) {
								// load segment end flags at end node
								Flags.SetSegmentEndFlags(segmentConf.segmentId, false, segAtEndNodeConf.flags);
							}

							if (segAtEndNodeConf.priorityType != null) {
								// load priority sign configuration
								TrafficPriority.AddPrioritySegment(endNodeId, segmentConf.segmentId, (SegmentEnd.PriorityType)segAtEndNodeConf.priorityType);
							}
						}
					}
				}
			} else {
				Log.Warning("Segment data structure undefined!");
			}

			// Load lane configuration
			if (configuration.LaneConfs != null) {
				Log.Info($"Loading lane configuration data. {configuration.LaneConfs.Count} elements");
				foreach (TMConfigurationV2.LaneConf laneConf in configuration.LaneConfs) {
					if (((NetLane.Flags)netManager.m_lanes.m_buffer[laneConf.laneId].m_flags & NetLane.Flags.Created) == NetLane.Flags.None)
						continue;

					if (laneConf.directions != null) {
                        // load lane arrows
                        TAMLaneRoutingManager.instance.LoadLaneDirection(laneConf.laneId, (TAMLaneDirection)(laneConf.directions));
					}
				}
			} else {
				Log.Warning("Lane configuration structure undefined!");
			}

			// Load node configuration
			if (configuration.NodeConfs != null) {
				Log.Info($"Loading {configuration.NodeConfs.Count()} timed traffic lights (new method)");

				foreach (TMConfigurationV2.NodeConf nodeConf in configuration.NodeConfs) {
					if ((netManager.m_nodes.m_buffer[nodeConf.nodeId].m_flags & NetNode.Flags.Created) == NetNode.Flags.None)
						continue;
					if ((Singleton<NetManager>.instance.m_nodes.m_buffer[nodeConf.nodeId].Info.m_class.m_service != ItemClass.Service.Road &&
							Singleton<NetManager>.instance.m_nodes.m_buffer[nodeConf.nodeId].Info.m_class.m_service != ItemClass.Service.PublicTransport))
						continue;

					if (nodeConf.hasTrafficLight != null) {
						// Load toggled traffic light
						Flags.setNodeTrafficLight(nodeConf.nodeId, (bool)nodeConf.hasTrafficLight);
					}

					if (nodeConf.timedLights != null) {
						// Load timed traffic light
						TMConfigurationV2.TimedTrafficLights cnfTimedLights = nodeConf.timedLights;
						Flags.setNodeTrafficLight(nodeConf.nodeId, true);
						Log._Debug($"Adding Timed Node at node {nodeConf.nodeId}");

						TrafficLightSimulation sim = TrafficLightSimulation.AddNodeToSimulation(nodeConf.nodeId);
						sim.SetupTimedTrafficLight(cnfTimedLights.nodeGroup);
						var timedNode = sim.TimedLight;

						int j = 0;
						foreach (TMConfigurationV2.TimedTrafficLightsStep cnfTimedStep in cnfTimedLights.timedSteps) {
							Log._Debug($"Loading timed step {j} at node {nodeConf.nodeId}");
							TimedTrafficLightsStep step = timedNode.AddStep(cnfTimedStep.minTime, cnfTimedStep.maxTime, cnfTimedStep.waitFlowBalance);

							foreach (KeyValuePair<ushort, TMConfigurationV2.CustomSegmentLights> e in cnfTimedStep.segmentLights) {
								ushort segmentId = e.Key;
								TMConfigurationV2.CustomSegmentLights cnfLights = e.Value;

								Log._Debug($"Loading timed step {j}, segment {segmentId} at node {nodeConf.nodeId}");
								CustomSegmentLights lights = null;
								if (!step.segmentLights.TryGetValue(segmentId, out lights)) {
									Log._Debug($"No segment lights found at timed step {j} for segment {e.Key}, node {nodeConf.nodeId}");
									continue;
								}

								Log._Debug($"Loading pedestrian light @ seg. {segmentId}, step {j}: {cnfLights.pedestrianLightState} {cnfLights.manualPedestrianMode}");

								lights.ManualPedestrianMode = cnfLights.manualPedestrianMode;
								lights.PedestrianLightState = cnfLights.pedestrianLightState;

								foreach (KeyValuePair<TMVehicleType, TMConfigurationV2.CustomSegmentLight> e2 in cnfLights.customLights) {
									TMVehicleType vehicleType = e2.Key;
									TMConfigurationV2.CustomSegmentLight cnfLight = e2.Value;

									Log._Debug($"Loading timed step {j}, segment {segmentId}, vehicleType {vehicleType} at node {nodeConf.nodeId}");
									CustomSegmentLight light = null;
									if (!lights.CustomLights.TryGetValue(vehicleType, out light)) {
										Log._Debug($"No segment light found for timed step {j}, segment {segmentId}, vehicleType {vehicleType} at node {nodeConf.nodeId}");
										continue;
									}

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
				}
			} else {
				Log.Warning("Node configuration structure undefined!");
			}
		}

		public static TMConfigurationV2 CreateConfiguration() {
			var configuration = new TMConfigurationV2();
			NetManager netManager = Singleton<NetManager>.instance;

			Dictionary<ushort, TMConfigurationV2.NodeConf> nodeConfs = new Dictionary<ushort, TMConfigurationV2.NodeConf>();
			Dictionary<ushort, TMConfigurationV2.SegmentConf> segmentConfs = new Dictionary<ushort, TMConfigurationV2.SegmentConf>();
			Dictionary<uint, TMConfigurationV2.LaneConf> laneConfs = new Dictionary<uint, TMConfigurationV2.LaneConf>();

			// Assemble segment configuration
			for (ushort segmentId = 0; segmentId < NetManager.MAX_SEGMENT_COUNT; segmentId++) {
				if (TrafficPriority.TrafficSegments[segmentId] != null) {
					// save priority signs

					TrafficSegment trafficSegment = TrafficPriority.TrafficSegments[segmentId];
					ushort startNodeId = netManager.m_segments.m_buffer[segmentId].m_startNode;
					ushort endNodeId = netManager.m_segments.m_buffer[segmentId].m_endNode;

					SegmentEnd startNodeSegEnd = null;
					if (startNodeId > 0) {
						if (trafficSegment.Node1 == startNodeId && trafficSegment.Instance1.Type != SegmentEnd.PriorityType.None)
							startNodeSegEnd = trafficSegment.Instance1;
						else if (trafficSegment.Node2 == startNodeId && trafficSegment.Instance2.Type != SegmentEnd.PriorityType.None)
							startNodeSegEnd = trafficSegment.Instance2;
					}

					SegmentEnd endNodeSegEnd = null;
					if (endNodeId > 0) {
						if (trafficSegment.Node1 == endNodeId && trafficSegment.Instance1.Type != SegmentEnd.PriorityType.None)
							endNodeSegEnd = trafficSegment.Instance1;
						else if (trafficSegment.Node2 == endNodeId && trafficSegment.Instance2.Type != SegmentEnd.PriorityType.None)
							endNodeSegEnd = trafficSegment.Instance2;
					}

					if (startNodeSegEnd != null) {
						Log.Info($"Saving Priority Segment of type: {startNodeSegEnd.Type} @ node {startNodeId}, seg. {segmentId}");

						if (!segmentConfs.ContainsKey(segmentId))
							segmentConfs[segmentId] = new TMConfigurationV2.SegmentConf(segmentId);
						TMConfigurationV2.SegmentConf segmentConf = segmentConfs[segmentId];
						if (segmentConf.startNodeConf == null)
							segmentConf.startNodeConf = new TMConfigurationV2.SegmentEndConf();

						segmentConf.startNodeConf.priorityType = startNodeSegEnd.Type;
					}

					if (endNodeSegEnd != null) {
						Log.Info($"Saving Priority Segment of type: {endNodeSegEnd.Type} @ node {endNodeId}, seg. {segmentId}");

						if (!segmentConfs.ContainsKey(segmentId))
							segmentConfs[segmentId] = new TMConfigurationV2.SegmentConf(segmentId);
						TMConfigurationV2.SegmentConf segmentConf = segmentConfs[segmentId];
						if (segmentConf.endNodeConf == null)
							segmentConf.endNodeConf = new TMConfigurationV2.SegmentEndConf();

						segmentConf.endNodeConf.priorityType = endNodeSegEnd.Type;
					}
				}

				// save segment-node flags
				TMConfigurationV2.SegmentEndFlags startNodeFlags = Flags.GetSegmentEndFlags(segmentId, true);
				if (startNodeFlags != null) {
					if (!segmentConfs.ContainsKey(segmentId))
						segmentConfs[segmentId] = new TMConfigurationV2.SegmentConf(segmentId);
					TMConfigurationV2.SegmentConf segmentConf = segmentConfs[segmentId];
					if (segmentConf.startNodeConf == null)
						segmentConf.startNodeConf = new TMConfigurationV2.SegmentEndConf();

					segmentConf.startNodeConf.flags = startNodeFlags;
				}

				TMConfigurationV2.SegmentEndFlags endNodeFlags = Flags.GetSegmentEndFlags(segmentId, false);
				if (endNodeFlags != null) {
					if (!segmentConfs.ContainsKey(segmentId))
						segmentConfs[segmentId] = new TMConfigurationV2.SegmentConf(segmentId);
					TMConfigurationV2.SegmentConf segmentConf = segmentConfs[segmentId];
					if (segmentConf.endNodeConf == null)
						segmentConf.endNodeConf = new TMConfigurationV2.SegmentEndConf();

					segmentConf.endNodeConf.flags = endNodeFlags;
				}
			}

			// Assemble node configuration
			for (ushort nodeId = 0; nodeId < NetManager.MAX_NODE_COUNT; nodeId++) {
				bool? hasTrafficLight = Flags.isNodeTrafficLight(nodeId);
				if (Flags.mayHaveTrafficLight(nodeId) && hasTrafficLight != null) {
					// save toggled traffic light

					if (!nodeConfs.ContainsKey(nodeId))
						nodeConfs[nodeId] = new TMConfigurationV2.NodeConf(nodeId);

					nodeConfs[nodeId].hasTrafficLight = (bool)hasTrafficLight;
				}

				TrafficLightSimulation sim = TrafficLightSimulation.GetNodeSimulation(nodeId);
				if (sim != null && sim.IsTimedLight()) {
					// save timed traffic light

					Log._Debug($"Saving timed light at node {nodeId}.");

					if (!nodeConfs.ContainsKey(nodeId))
						nodeConfs[nodeId] = new TMConfigurationV2.NodeConf(nodeId);
					TMConfigurationV2.NodeConf nodeConf = nodeConfs[nodeId];

					var timedNode = sim.TimedLight;
					timedNode.handleNewSegments();

					TMConfigurationV2.TimedTrafficLights cnfTimedLights = new TMConfigurationV2.TimedTrafficLights();
					nodeConf.timedLights = cnfTimedLights;

					cnfTimedLights.nodeGroup = timedNode.NodeGroup;
					cnfTimedLights.started = timedNode.IsStarted();
					cnfTimedLights.timedSteps = new List<TMConfigurationV2.TimedTrafficLightsStep>();

					for (var j = 0; j < timedNode.NumSteps(); j++) {
						Log._Debug($"Saving timed light step {j} at node {nodeId}.");
						TimedTrafficLightsStep timedStep = timedNode.Steps[j];
						TMConfigurationV2.TimedTrafficLightsStep cnfTimedStep = new TMConfigurationV2.TimedTrafficLightsStep();
						cnfTimedLights.timedSteps.Add(cnfTimedStep);

						cnfTimedStep.minTime = timedStep.minTime;
						cnfTimedStep.maxTime = timedStep.maxTime;
						cnfTimedStep.waitFlowBalance = timedStep.waitFlowBalance;
						cnfTimedStep.segmentLights = new Dictionary<ushort, TMConfigurationV2.CustomSegmentLights>();
						foreach (KeyValuePair<ushort, CustomSegmentLights> e in timedStep.segmentLights) {
							Log._Debug($"Saving timed light step {j}, segment {e.Key} at node {nodeId}.");

							CustomSegmentLights segLights = e.Value;
							TMConfigurationV2.CustomSegmentLights cnfSegLights = new TMConfigurationV2.CustomSegmentLights();
							cnfTimedStep.segmentLights.Add(e.Key, cnfSegLights);

							cnfSegLights.customLights = new Dictionary<TMVehicleType, TMConfigurationV2.CustomSegmentLight>();
							cnfSegLights.pedestrianLightState = segLights.PedestrianLightState;
							cnfSegLights.manualPedestrianMode = segLights.ManualPedestrianMode;

							Log._Debug($"Saving pedestrian light @ seg. {e.Key}, step {j}: {cnfSegLights.pedestrianLightState} {cnfSegLights.manualPedestrianMode}");

							foreach (KeyValuePair<TMVehicleType, CustomSegmentLight> e2 in segLights.CustomLights) {
								Log._Debug($"Saving timed light step {j}, segment {e.Key}, vehicleType {e2.Key} at node {nodeId}.");

								CustomSegmentLight segLight = e2.Value;
								TMConfigurationV2.CustomSegmentLight cnfSegLight = new TMConfigurationV2.CustomSegmentLight();
								cnfSegLights.customLights.Add(e2.Key, cnfSegLight);

								cnfSegLight.currentMode = (int)segLight.CurrentMode;
								cnfSegLight.leftLight = segLight.LightLeft;
								cnfSegLight.mainLight = segLight.LightMain;
								cnfSegLight.rightLight = segLight.LightRight;
							}
						}
					}
				}
			}

			// Assemble lane configuration
			for (uint laneId = 0; laneId < NetManager.MAX_LANE_COUNT; laneId++) {
				TAMLaneDirection? laneArrows = TAMLaneRoutingManager.instance.GetLaneDirection(laneId);
				if (laneArrows != null) {
					// save lane arrows

					Log._Debug($"Saving lane data for lane {laneId}, setting to {laneArrows.ToString()}");
					if (!laneConfs.ContainsKey(laneId))
						laneConfs[laneId] = new TMConfigurationV2.LaneConf(laneId);

					laneConfs[laneId].directions = laneArrows;
				}
			}

			configuration.NodeConfs = new List<TMConfigurationV2.NodeConf>(nodeConfs.Values);
			configuration.SegmentConfs = new List<TMConfigurationV2.SegmentConf>(segmentConfs.Values);
			configuration.LaneConfs = new List<TMConfigurationV2.LaneConf>(laneConfs.Values);

			return configuration;
		}
	}
}
