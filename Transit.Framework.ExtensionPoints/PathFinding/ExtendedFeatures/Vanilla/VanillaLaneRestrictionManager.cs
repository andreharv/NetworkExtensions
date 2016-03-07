
namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Vanilla
{
    public class VanillaLaneRestrictionManager : IExtendedLaneRestrictionManager
    {
        public bool CanUseLane(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo, Network.ExtendedVehicleType vehicleType)
        {
            return true;
        }
    }
}
