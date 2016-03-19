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

        public TPPLaneDataV2 ConvertToV2()
        {
            return new TPPLaneDataV2()
            {
                m_laneId = m_laneId,
                m_nodeId = m_nodeId,
                m_laneConnections = m_laneConnections.ToArray(),
                m_unitTypes = m_vehicleTypes.ConvertToUnitType(),
                m_speed = m_speed,
            };
        }
    }
}
