
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures
{
    public interface IExtendedLaneRestrictionManager
    {
        bool CanUseLane(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedVehicleType vehicleType);
    }
}
