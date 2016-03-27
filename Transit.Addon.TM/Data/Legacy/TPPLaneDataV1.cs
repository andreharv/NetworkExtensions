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
    }
}
