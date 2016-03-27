namespace Transit.Addon.RealisticSpeed
{
    public static class RealisticSpeedManager
    {
        public static void Activate()
        {
            SetRealisticSpeed(true);
        }

        public static void Deactivate()
        {
            SetRealisticSpeed(false);
        }

        private static void SetRealisticSpeed(bool activating)
        {
            for (uint i = 0; i < PrefabCollection<CitizenInfo>.LoadedCount(); i++)
            {
                CitizenInfo cit = PrefabCollection<CitizenInfo>.GetLoaded(i);
                float m_walkSpeedMultiplier = 0.5f;

                if (!activating)
                {
                    m_walkSpeedMultiplier = 1f / m_walkSpeedMultiplier;
                }

                cit.m_walkSpeed *= m_walkSpeedMultiplier;
            }

            for (uint i = 0; i < PrefabCollection<VehicleInfo>.LoadedCount(); i++)
            {
                VehicleInfo vehicle = PrefabCollection<VehicleInfo>.GetLoaded(i);
                float accelerationMultiplier;
                float maxSpeedMultiplier;

                var name = vehicle.name.ToLowerInvariant();

                if (name.Contains("bus") ||
                    name.Contains("truck") ||
                    name.Contains("tractor") ||
                    name.Contains("trailer") ||
                    name.Contains("van"))
                {
                    accelerationMultiplier = 0.25f;
                    maxSpeedMultiplier = 0.5f;
                }
                else
                {
                    accelerationMultiplier = 0.5f;
                    maxSpeedMultiplier = 0.5f;
                }

                if (!activating)
                {
                    accelerationMultiplier = 1f / accelerationMultiplier;
                    maxSpeedMultiplier = 1f / maxSpeedMultiplier;
                }

                vehicle.m_acceleration *= accelerationMultiplier;
                vehicle.m_maxSpeed *= maxSpeedMultiplier;
            }
        }
    }
}