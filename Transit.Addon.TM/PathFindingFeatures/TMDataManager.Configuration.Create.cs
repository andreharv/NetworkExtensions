using ColossalFramework;
using System;
using System.Collections.Generic;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.Traffic;
using Transit.Addon.TM.TrafficLight;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public static partial class TMDataManager
    {
        public static TMConfigurationV2 CreateConfiguration()
        {
            var configuration = new TMConfigurationV2();

            if (TrafficPriority.PrioritySegments != null)
            {
                for (ushort i = 0; i < Singleton<NetManager>.instance.m_segments.m_size; i++)
                {
                    SavePrioritySegment(i, configuration);
                    SaveSegmentNodeFlags(i, configuration);
                }
            }

            for (ushort i = 0; i < Singleton<NetManager>.instance.m_nodes.m_size; i++)
            {
                /*if (TrafficLightSimulation.LightSimulationByNodeId != null) {
					SaveTrafficLightSimulation(i, configuration);
				}*/

                /*if (TrafficLightsManual.ManualSegments != null) {
					SaveManualTrafficLight(i, configuration);
				}*/

                TrafficLightSimulation sim = TrafficLightSimulation.GetNodeSimulation(i);
                if (sim != null && sim.IsTimedLight())
                {
                    SaveTimedTrafficLight(i, configuration);
                    // TODO save new traffic lights
                }

                SaveNodeLights(i, configuration);
            }

#if !TAM
			if (TrafficManagerModule.IsPathManagerCompatible) {
#endif
            for (uint i = 0; i < Singleton<NetManager>.instance.m_lanes.m_buffer.Length; i++)
            {
                SaveLaneData(i, configuration);
            }
#if !TAM
			}
#endif

            foreach (KeyValuePair<uint, ushort> e in Flags.getAllLaneSpeedLimits())
            {
                SaveLaneSpeedLimit(new TMConfigurationV2.LaneSpeedLimit(e.Key, e.Value), configuration);
            }

            foreach (KeyValuePair<uint, TMVehicleType> e in Flags.getAllLaneAllowedVehicleTypes())
            {
                SaveLaneAllowedVehicleTypes(new TMConfigurationV2.LaneVehicleTypes(e.Key, e.Value), configuration);
            }

            return configuration;
        }

        private static void SaveLaneData(uint i, TMConfigurationV2 configuration)
        {
            try
            {
                //NetLane.Flags flags = (NetLane.Flags)lane.m_flags;
                /*if ((flags & NetLane.Flags.LeftForwardRight) == NetLane.Flags.None) // only save lanes with explicit lane arrows
					return;*/
                var laneSegmentId = Singleton<NetManager>.instance.m_lanes.m_buffer[i].m_segment;
                if (laneSegmentId <= 0)
                    return;
                if ((Singleton<NetManager>.instance.m_lanes.m_buffer[i].m_flags & (ushort)NetLane.Flags.Created) == 0 || laneSegmentId == 0)
                    return;
                if ((Singleton<NetManager>.instance.m_segments.m_buffer[laneSegmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
                    return;

                //if (TrafficPriority.PrioritySegments.ContainsKey(laneSegmentId)) {
                Flags.LaneArrows? laneArrows = Flags.getLaneArrowFlags(i);
                if (laneArrows != null)
                {
                    uint laneArrowInt = (uint)laneArrows;
                    Log._Debug($"Saving lane data for lane {i}, segment {laneSegmentId}, setting to {laneArrows.ToString()} ({laneArrowInt})");
                    configuration.LaneFlags += $"{i}:{laneArrowInt},";
                }
                //}
            }
            catch (Exception e)
            {
                Log.Error($"Error saving NodeLaneData {e.Message}");
            }
        }

        private static void SaveNodeLights(int i, TMConfigurationV2 configuration)
        {
            try
            {
                if (!Flags.mayHaveTrafficLight((ushort)i))
                    return;

                bool? hasTrafficLight = Flags.isNodeTrafficLight((ushort)i);
                if (hasTrafficLight == null)
                    return;

                if ((bool)hasTrafficLight)
                {
                    Log.Info($"Saving that node {i} has a traffic light");
                }
                else {
                    Log.Info($"Saving that node {i} does not have a traffic light");
                }
                configuration.NodeTrafficLights += $"{i}:{Convert.ToUInt16((bool)hasTrafficLight)},";
                return;
            }
            catch (Exception e)
            {
                Log.Error($"Error Adding Node Lights and Crosswalks {e.Message}");
                return;
            }
        }

        private static void SaveTimedTrafficLight(ushort i, TMConfigurationV2 configuration)
        {
            try
            {
                TrafficLightSimulation sim = TrafficLightSimulation.GetNodeSimulation(i);
                if (sim == null || !sim.IsTimedLight())
                    return;

                Log._Debug($"Going to save timed light at node {i}.");

                var timedNode = sim.TimedLight;
                timedNode.handleNewSegments();

                TMConfigurationV2.TimedTrafficLights cnfTimedLights = new TMConfigurationV2.TimedTrafficLights();
                configuration.TimedLights.Add(cnfTimedLights);

                cnfTimedLights.nodeId = timedNode.NodeId;
                cnfTimedLights.nodeGroup = timedNode.NodeGroup;
                cnfTimedLights.started = timedNode.IsStarted();
                cnfTimedLights.timedSteps = new List<TMConfigurationV2.TimedTrafficLightsStep>();

                for (var j = 0; j < timedNode.NumSteps(); j++)
                {
                    Log._Debug($"Saving timed light step {j} at node {i}.");
                    TimedTrafficLightsStep timedStep = timedNode.Steps[j];
                    TMConfigurationV2.TimedTrafficLightsStep cnfTimedStep = new TMConfigurationV2.TimedTrafficLightsStep();
                    cnfTimedLights.timedSteps.Add(cnfTimedStep);

                    cnfTimedStep.minTime = timedStep.minTime;
                    cnfTimedStep.maxTime = timedStep.maxTime;
                    cnfTimedStep.waitFlowBalance = timedStep.waitFlowBalance;
                    cnfTimedStep.segmentLights = new Dictionary<ushort, TMConfigurationV2.CustomSegmentLights>();
                    foreach (KeyValuePair<ushort, CustomSegmentLights> e in timedStep.segmentLights)
                    {
                        Log._Debug($"Saving timed light step {j}, segment {e.Key} at node {i}.");

                        CustomSegmentLights segLights = e.Value;
                        TMConfigurationV2.CustomSegmentLights cnfSegLights = new TMConfigurationV2.CustomSegmentLights();
                        cnfTimedStep.segmentLights.Add(e.Key, cnfSegLights);

                        cnfSegLights.nodeId = segLights.NodeId;
                        cnfSegLights.segmentId = segLights.SegmentId;
                        cnfSegLights.customLights = new Dictionary<TMVehicleType, TMConfigurationV2.CustomSegmentLight>();
                        cnfSegLights.pedestrianLightState = segLights.PedestrianLightState;
                        cnfSegLights.manualPedestrianMode = segLights.ManualPedestrianMode;

                        Log._Debug($"Saving pedestrian light @ seg. {e.Key}, step {j}: {cnfSegLights.pedestrianLightState} {cnfSegLights.manualPedestrianMode}");

                        foreach (KeyValuePair<TMVehicleType, CustomSegmentLight> e2 in segLights.CustomLights)
                        {
                            Log._Debug($"Saving timed light step {j}, segment {e.Key}, vehicleType {e2.Key} at node {i}.");

                            CustomSegmentLight segLight = e2.Value;
                            TMConfigurationV2.CustomSegmentLight cnfSegLight = new TMConfigurationV2.CustomSegmentLight();
                            cnfSegLights.customLights.Add(e2.Key, cnfSegLight);

                            cnfSegLight.nodeId = segLight.NodeId;
                            cnfSegLight.segmentId = segLight.SegmentId;
                            cnfSegLight.currentMode = (int)segLight.CurrentMode;
                            cnfSegLight.leftLight = segLight.LightLeft;
                            cnfSegLight.mainLight = segLight.LightMain;
                            cnfSegLight.rightLight = segLight.LightRight;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error adding TimedTrafficLights to save {e.Message}");
            }
        }

        private static void SaveLaneSpeedLimit(TMConfigurationV2.LaneSpeedLimit laneSpeedLimit, TMConfigurationV2 configuration)
        {
            Log._Debug($"Saving speed limit of lane {laneSpeedLimit.laneId}: {laneSpeedLimit.speedLimit}");
            configuration.LaneSpeedLimits.Add(laneSpeedLimit);
        }

        private static void SaveLaneAllowedVehicleTypes(TMConfigurationV2.LaneVehicleTypes laneVehicleTypes, TMConfigurationV2 configuration)
        {
            Log._Debug($"Saving vehicle restrictions of lane {laneVehicleTypes.laneId}: {laneVehicleTypes.vehicleTypes}");
            configuration.LaneAllowedVehicleTypes.Add(laneVehicleTypes);
        }

        private static void SavePrioritySegment(ushort segmentId, TMConfigurationV2 configuration)
        {
            try
            {
                if (TrafficPriority.PrioritySegments[segmentId] == null)
                {
                    return;
                }

                if (TrafficPriority.PrioritySegments[segmentId].Node1 != 0 && TrafficPriority.PrioritySegments[segmentId].Instance1.Type != SegmentEnd.PriorityType.None)
                {
                    Log.Info($"Saving Priority Segment of type: {TrafficPriority.PrioritySegments[segmentId].Instance1.Type} @ node {TrafficPriority.PrioritySegments[segmentId].Node1}, seg. {segmentId}");
                    configuration.PrioritySegments.Add(new[]
                    {
                        TrafficPriority.PrioritySegments[segmentId].Node1, segmentId,
                        (int) TrafficPriority.PrioritySegments[segmentId].Instance1.Type
                    });
                }

                if (TrafficPriority.PrioritySegments[segmentId].Node2 == 0 || TrafficPriority.PrioritySegments[segmentId].Instance2.Type == SegmentEnd.PriorityType.None)
                    return;

                Log.Info($"Saving Priority Segment of type: {TrafficPriority.PrioritySegments[segmentId].Instance2.Type} @ node {TrafficPriority.PrioritySegments[segmentId].Node2}, seg. {segmentId}");
                configuration.PrioritySegments.Add(new[] {
                    TrafficPriority.PrioritySegments[segmentId].Node2, segmentId,
                    (int) TrafficPriority.PrioritySegments[segmentId].Instance2.Type
                });
            }
            catch (Exception e)
            {
                Log.Error($"Error adding Priority Segments to Save: {e.ToString()}");
            }
        }

        private static void SaveSegmentNodeFlags(ushort segmentId, TMConfigurationV2 configuration)
        {
            try
            {
                if ((Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
                    return;

                TMConfigurationV2.SegmentNodeFlags startNodeFlags = Flags.getSegmentNodeFlags(segmentId, true);
                TMConfigurationV2.SegmentNodeFlags endNodeFlags = Flags.getSegmentNodeFlags(segmentId, false);

                if (startNodeFlags == null && endNodeFlags == null)
                    return;

                TMConfigurationV2.SegmentNodeConf conf = new TMConfigurationV2.SegmentNodeConf(segmentId);

                conf.startNodeFlags = startNodeFlags;
                conf.endNodeFlags = endNodeFlags;

                Log.Info($"Saving segment-at-node flags for seg. {segmentId}");
                configuration.SegmentNodeConfs.Add(conf);
            }
            catch (Exception e)
            {
                Log.Error($"Error adding Priority Segments to Save: {e.ToString()}");
            }
        }
    }
}
