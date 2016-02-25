
namespace Transit.Framework.Network
{
    public class ExtendedNetInfoLane : NetInfo.Lane
    {
        public ExtendedVehicleType AllowedVehicleTypes { get; private set; }

        public ExtendedNetInfoLane(NetInfo.Lane lane, ExtendedVehicleType vehicleTypes)
        {
            this.AllowedVehicleTypes = vehicleTypes;
            this.ShallowCloneFrom(lane);
        }
    }
}
