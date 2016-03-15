using Transit.Addon.ToolsV2.Data;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.ToolsV2.PathFindingFeatures
{
    public class TPPRoadRestrictionManager : IRoadRestrictionManager
    {
        public bool CanUseLane(ushort segmentId, byte laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedUnitType unitType)
        {
            if ((unitType & TPPSupported.UNITS) == 0)
            {
                return true;
            }

            if (laneInfo == null)
            {
                return true;
            }

            if ((laneInfo.m_vehicleType & TPPSupported.VEHICLETYPES) == 0)
            {
                return true;
            }

            TPPLaneDataV2 lane = TPPLaneDataManager.GetLane(laneId, false);
            if (lane == null)
                return true;

            return (lane.m_unitTypes & unitType) != ExtendedUnitType.None;
        }
    }
}
