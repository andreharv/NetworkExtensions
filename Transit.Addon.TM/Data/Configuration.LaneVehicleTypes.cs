using System;

namespace TrafficManager
{
    public partial class Configuration
    {
        [Serializable]
        public class LaneVehicleTypes
        {
            public uint laneId;
            public Traffic.ExtVehicleType vehicleTypes;

            public LaneVehicleTypes(uint laneId, Traffic.ExtVehicleType vehicleTypes)
            {
                this.laneId = laneId;
                this.vehicleTypes = vehicleTypes;
            }
        }
    }
}
