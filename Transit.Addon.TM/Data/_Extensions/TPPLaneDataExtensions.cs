using System;
using System.Collections.Generic;
using Transit.Addon.TM.Data.Legacy;

namespace Transit.Addon.TM.Data
{
    public static class TPPLaneDataV1Extensions
    {
        public static TAMLaneRoute ConvertToRoute(this TPPLaneDataV1 data)
        {
            return new TAMLaneRoute()
            {
                LaneId = data.m_laneId,
                NodeId = data.m_nodeId,
                Connections = data.m_laneConnections.ToArray(),
            };
        }

        public static TAMLaneSpeedLimit ConvertToSpeedLimit(this TPPLaneDataV1 data)
        {
            ushort? speedInKmH = (ushort) Math.Round(data.m_speed * 50, -1, MidpointRounding.AwayFromZero);

            if (speedInKmH == 0 || speedInKmH > 130)
            {
                speedInKmH = null;
            }

            return new TAMLaneSpeedLimit()
            {
                LaneId = data.m_laneId,
                SpeedLimit = speedInKmH,
            };
        }

        public static TAMLaneRestriction ConvertToRestriction(this TPPLaneDataV1 data)
        {
            return new TAMLaneRestriction()
            {
                LaneId = data.m_laneId,
                UnitTypes = data.m_vehicleTypes.ConvertToUnitType(),
            };
        }
    }
}
