using System.Collections.Generic;
using System.Linq;
using TrafficManager;
using TrafficManager.Traffic;
using Transit.Addon.TM.Data;

namespace Transit.Addon.TM.DataSerialization
{
    public static class TMDataConverter
    {
        public static TMConfigurationV2 ConvertToV2(this Configuration dataV1)
        {
            var dataV2 = new TMConfigurationV2();

            dataV2.NodeTrafficLights        = dataV1.NodeTrafficLights;
            dataV2.NodeCrosswalk            = dataV1.NodeCrosswalk;
            dataV2.LaneFlags                = dataV1.LaneFlags;

            dataV2.LaneSpeedLimits          = dataV1.LaneSpeedLimits.Select(d => d.ConvertToV2()).ToList();
            dataV2.LaneAllowedVehicleTypes  = dataV1.LaneAllowedVehicleTypes.Select(d => d.ConvertToV2()).ToList();
            dataV2.TimedLights              = dataV1.TimedLights.Select(d => d.ConvertToV2()).ToList();
            dataV2.SegmentNodeConfs         = dataV1.SegmentNodeConfs.Select(d => d.ConvertToV2()).ToList();

            dataV2.PrioritySegments         = dataV1.PrioritySegments;
            dataV2.NodeDictionary           = dataV1.NodeDictionary;
            dataV2.ManualSegments           = dataV1.ManualSegments;

            dataV2.TimedNodes               = dataV1.TimedNodes;
            dataV2.TimedNodeGroups          = dataV1.TimedNodeGroups;
            dataV2.TimedNodeSteps           = dataV1.TimedNodeSteps;
            dataV2.TimedNodeStepSegments    = dataV1.TimedNodeStepSegments;

            return dataV2;
        }

        public static TMConfigurationV2.LaneSpeedLimit ConvertToV2(this Configuration.LaneSpeedLimit dataV1)
        {
            if (dataV1 == null)
            {
                return null;
            }

            var dataV2 = new TMConfigurationV2.LaneSpeedLimit();
            dataV2.laneId = dataV1.laneId;
            dataV2.speedLimit = dataV1.speedLimit;

            return dataV2;
        }

        public static TMConfigurationV2.LaneVehicleTypes ConvertToV2(this Configuration.LaneVehicleTypes dataV1)
        {
            if (dataV1 == null)
            {
                return null;
            }

            var dataV2 = new TMConfigurationV2.LaneVehicleTypes();
            dataV2.laneId = dataV1.laneId;
            dataV2.vehicleTypes = dataV1.vehicleTypes.ConvertToV2();

            return dataV2;
        }

        public static TMConfigurationV2.TimedTrafficLights ConvertToV2(this Configuration.TimedTrafficLights dataV1)
        {
            if (dataV1 == null)
            {
                return null;
            }

            var dataV2 = new TMConfigurationV2.TimedTrafficLights();
            dataV2.nodeId = dataV1.nodeId;
            dataV2.nodeGroup = dataV1.nodeGroup;
            dataV2.started = dataV1.started;
            dataV2.timedSteps = dataV1.timedSteps.Select(d => d.ConvertToV2()).ToList();

            return dataV2;
        }

        public static TMConfigurationV2.TimedTrafficLightsStep ConvertToV2(this Configuration.TimedTrafficLightsStep dataV1)
        {
            if (dataV1 == null)
            {
                return null;
            }

            var dataV2 = new TMConfigurationV2.TimedTrafficLightsStep();
            dataV2.minTime = dataV1.minTime;
            dataV2.maxTime = dataV1.maxTime;
            dataV2.segmentLights = dataV1
                .segmentLights
                .AsEnumerable()
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ConvertToV2());

            return dataV2;
        }

        public static TMConfigurationV2.CustomSegmentLights ConvertToV2(this Configuration.CustomSegmentLights dataV1)
        {
            if (dataV1 == null)
            {
                return null;
            }

            var dataV2 = new TMConfigurationV2.CustomSegmentLights();
            dataV2.nodeId = dataV1.nodeId;
            dataV2.segmentId = dataV1.segmentId;
            dataV2.customLights = dataV1.customLights.AsEnumerable().ToDictionary(kvp => kvp.Key.ConvertToV2(), kvp => kvp.Value.ConvertToV2());
            dataV2.pedestrianLightState = dataV1.pedestrianLightState;
            dataV2.manualPedestrianMode = dataV1.manualPedestrianMode;

            return dataV2;
        }

        public static TMVehicleType ConvertToV2(this ExtVehicleType dataV1)
        {
            return (TMVehicleType) (int) dataV1;
        }

        public static TMConfigurationV2.CustomSegmentLight ConvertToV2(this Configuration.CustomSegmentLight dataV1)
        {
            if (dataV1 == null)
            {
                return null;
            }

            var dataV2 = new TMConfigurationV2.CustomSegmentLight();
            dataV2.nodeId = dataV1.nodeId;
            dataV2.segmentId = dataV1.segmentId;
            dataV2.currentMode = dataV1.currentMode;
            dataV2.leftLight = dataV1.leftLight;
            dataV2.mainLight = dataV1.mainLight;
            dataV2.rightLight = dataV1.rightLight;

            return dataV2;
        }

        public static TMConfigurationV2.SegmentNodeConf ConvertToV2(this Configuration.SegmentNodeConf dataV1)
        {
            if (dataV1 == null)
            {
                return null;
            }

            var dataV2 = new TMConfigurationV2.SegmentNodeConf();
            dataV2.segmentId = dataV1.segmentId;
            dataV2.startNodeFlags = dataV1.startNodeFlags.ConvertToV2();
            dataV2.endNodeFlags = dataV1.endNodeFlags.ConvertToV2();

            return dataV2;
        }

        public static TMConfigurationV2.SegmentNodeFlags ConvertToV2(this Configuration.SegmentNodeFlags dataV1)
        {
            if (dataV1 == null)
            {
                return null;
            }

            var dataV2 = new TMConfigurationV2.SegmentNodeFlags();
            dataV2.uturnAllowed = dataV1.uturnAllowed;
            dataV2.straightLaneChangingAllowed = dataV1.straightLaneChangingAllowed;
            dataV2.enterWhenBlockedAllowed = dataV1.enterWhenBlockedAllowed;

            return dataV2;
        }
    }
}