using System;
using System.Collections.Generic;

namespace Transit.Addon.TM.Data.Legacy
{
    [Serializable]
    public class TPPLaneDataV1
    {
        public uint m_laneId;
        public ushort m_nodeId;
        public List<uint> m_laneConnections = new List<uint>();
        public TPPVehicleType m_vehicleTypes = TPPVehicleType.All;
        public float m_speed = 1f;

        public TAMLaneRoute ConvertToRoute()
        {
            return new TAMLaneRoute()
            {
                LaneId = m_laneId,
                NodeId = m_nodeId,
                Connections = m_laneConnections.ToArray(),
            };
        }

        public TAMLaneSpeedLimit ConvertToSpeedLimit()
        {
            ushort? speedInKmH = (ushort) Math.Round(m_speed * 50, -1, MidpointRounding.AwayFromZero);

            if (speedInKmH == 0 || speedInKmH > 130)
            {
                speedInKmH = null;
            }

            return new TAMLaneSpeedLimit()
            {
                LaneId = m_laneId,
                SpeedLimit = speedInKmH,
            };
        }

        public TAMLaneRestriction ConvertToRestriction()
        {
            return new TAMLaneRestriction()
            {
                LaneId = m_laneId,
                UnitTypes = m_vehicleTypes.ConvertToUnitType(),
            };
        }
    }
}
