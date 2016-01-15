using Transit.Addon.Core.PathFinding;

namespace Transit.Addon.TrafficTools.Core.PathFinding
{
    public class ControledPathFinder : DefaultPathFinder
    {
        protected override float GetLaneSpeedLimit(NetInfo.Lane lane, uint laneId)
        {
            return LanesManager.GetLaneSpeed(laneId);
        }

        protected override bool CanLanesConnect(uint laneId1, uint laneId2)
        {
            return LanesManager.CheckLaneConnection(laneId1, laneId2);
            //&& 
            //LanesManager.CanUseLane(this.m_vehicleType, num2) && 
            //LanesManager.CanUseLane(this.m_vehicleType, item.m_laneID)
        }
    }
}
