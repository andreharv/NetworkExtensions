using System;

namespace Transit.Addon.TM.Data
{
    public partial class TMConfiguration
    {
        [Serializable]
        public class LaneVehicleTypes
        {
            public uint laneId;
            public TMVehicleType vehicleTypes;

            public LaneVehicleTypes(uint laneId, TMVehicleType vehicleTypes)
            {
                this.laneId = laneId;
                this.vehicleTypes = vehicleTypes;
            }
        }
    }
}
