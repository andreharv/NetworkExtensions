using System;
using Transit.Addon.ToolsV2.Data;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.ToolsV2.PathFinding.ExtendedFeatures
{
    public class TPPRoadSpeedManager : IRoadSpeedManager
    {
        public float GetLaneSpeedLimit(ref NetSegment segment, NetInfo.Lane laneInfo, ExtendedVehicleType vehicleType)
        {
            if ((vehicleType & TPPSupported.UNITS) == 0)
            {
                return laneInfo.m_speedLimit;
            }

            var laneId = NetManager.instance.GetLaneId(laneInfo, segment);

            if (laneId == null)
            {
                throw new Exception("TAM: LaneId has not been found");
            }

            return TPPLaneDataManager.GetLane(laneId.Value).m_speed;
        }
    }
}
