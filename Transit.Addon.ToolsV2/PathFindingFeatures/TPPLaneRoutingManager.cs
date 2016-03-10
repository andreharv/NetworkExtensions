using Transit.Addon.ToolsV2.Data;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.ToolsV2.PathFindingFeatures
{
    public class TPPLaneRoutingManager : ILaneRoutingManager
    {
        public bool CanLanesConnect(ushort nodeId, uint originLaneId, uint destinationLaneId, ExtendedUnitType vehicleType)
        {
            if ((vehicleType & TPPSupported.UNITS) == 0)
            {
                return true;
            }


            var originLaneInfo = NetManager.instance.GetLaneInfo(originLaneId);
            if (originLaneInfo == null)
            {
                return true;
            }

            if ((originLaneInfo.m_vehicleType & TPPSupported.VEHICLETYPES) == 0)
            {
                return true;
            }


            var destinationLane = NetManager.instance.GetLaneInfo(destinationLaneId);
            if (destinationLane == null)
            {
                return true;
            }

            if ((destinationLane.m_vehicleType & TPPSupported.VEHICLETYPES) == 0)
            {
                return true;
            }


            return TPPLaneDataManager
                .GetLane(originLaneId)
                .ConnectsTo(destinationLaneId);
        }
    }
}
