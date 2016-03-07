using System;
using System.Collections.Generic;

namespace Transit.Addon.ToolsV3.LaneRouting.DataLegacy.TPP
{
    [Serializable]
    public class TPPLaneData
    {
        public const ushort CONTROL_BIT = 2048;

        public uint m_laneId;
        public ushort m_nodeId;
        public List<uint> m_laneConnections = new List<uint>();
        public TPPVehicleType m_vehicleTypes = TPPVehicleType.All;
        public float m_speed = 1f;            
    }
}
