using ColossalFramework;
using ColossalFramework.Math;
using System;
using UnityEngine;

namespace CSL_Traffic
{
    public class CarSpeedData
    {
        private static readonly CarSpeedData[] sm_speedData = new CarSpeedData[VehicleManager.MAX_VEHICLE_COUNT];

        public static CarSpeedData Of(ushort vehicleID)
        {
            if (sm_speedData[vehicleID] == null)
            {
                sm_speedData[vehicleID] = new CarSpeedData();
            }
            return sm_speedData[vehicleID];
        }

        public uint CurrentPath { get; set; }
        public float SpeedMultiplier { get; set; }

        public void SetRandomSpeedMultiplier(float rangeMin = 0.75f, float rangeMax = 1.25f)
        {
            SpeedMultiplier = UnityEngine.Random.Range(rangeMin, rangeMax);
        }
    }
}
