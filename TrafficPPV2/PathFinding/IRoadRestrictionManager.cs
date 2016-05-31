using Transit.Framework.Network;

namespace CSL_Traffic.PathFinding
{
    public interface IRoadRestrictionManager
    {
        bool CanUseLane(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedVehicleType vehicleType);
    }
}
