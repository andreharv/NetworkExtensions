using ColossalFramework;
using ColossalFramework.Math;
using System;
using UnityEngine;
using Transit.Framework.Light;

namespace CSL_Traffic
{
    static class CustomCarAI
    {
        public static SpeedData[] sm_speedData = new SpeedData[VehicleManager.MAX_VEHICLE_COUNT];
        public struct SpeedData
        {
            public uint currentPath;
            public float speedMultiplier;

            public void SetRandomSpeedMultiplier(float rangeMin = 0.75f, float rangeMax = 1.25f)
            {
                speedMultiplier = UnityEngine.Random.Range(rangeMin, rangeMax);
            }

            public void ApplySpeedMultiplier(VehicleInfo vehicle)
            {
                vehicle.m_acceleration *= speedMultiplier;
                //vehicle.m_braking *= speedMultiplier;
                //vehicle.m_turning *= speedMultiplier;
                vehicle.m_maxSpeed *= speedMultiplier;
            }

            public void RestoreVehicleSpeed(VehicleInfo vehicle)
            {
                vehicle.m_acceleration /= speedMultiplier;
                //vehicle.m_braking /= speedMultiplier;
                //vehicle.m_turning /= speedMultiplier;
                vehicle.m_maxSpeed /= speedMultiplier;
            }
        }
    }
}
