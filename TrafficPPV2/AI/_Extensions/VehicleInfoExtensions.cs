
namespace CSL_Traffic
{
    public static class VehicleInfoExtensions
    {
        public static void ApplySpeedMultiplier(this VehicleInfo vehicle, CarSpeedData data)
        {
            vehicle.m_acceleration *= data.SpeedMultiplier;
            vehicle.m_maxSpeed *= data.SpeedMultiplier;
        }

        public static void RestoreVehicleSpeed(this VehicleInfo vehicle, CarSpeedData data)
        {
            vehicle.m_acceleration /= data.SpeedMultiplier;
            vehicle.m_maxSpeed /= data.SpeedMultiplier;
        }
    }
}
