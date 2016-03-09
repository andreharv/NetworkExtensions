
namespace Transit.Framework.Network
{
    public class ExtendedNetInfoLane : NetInfo.Lane
    {
        public ExtendedUnitType AllowedVehicleTypes { get; private set; }

        public ExtendedNetInfoLane(ExtendedUnitType vehicleTypes)
        {
            this.AllowedVehicleTypes = vehicleTypes;
        }

        public ExtendedNetInfoLane(NetInfo.Lane lane, ExtendedUnitType vehicleTypes)
        {
            this.AllowedVehicleTypes = vehicleTypes;
            this.ShallowCloneFrom(lane);
        }
    }
}
