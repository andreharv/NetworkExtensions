using System;
using Transit.Addon.ToolsV2.Data;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.ToolsV2.PathFindingFeatures
{
    public class TPPRoadSpeedManager : IRoadSpeedManager
    {
        public float GetLaneSpeedLimit(ref NetSegment segment, NetInfo.Lane laneInfo, ExtendedUnitType unitType)
        {
            if ((unitType & TPPSupported.UNITS) == 0)
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
