using System;
using System.Linq;
using System.Threading;
using ColossalFramework.Math;
using Transit.Framework.ExtensionPoints.PathFinding;
using Transit.Framework.Network;
using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;
using UnityEngine;

namespace Transit.Framework.Hooks.PathFinding
{
    public class PathManagerHook : PathManager
    {
        [RedirectFrom(typeof(PathManager), (ulong)PrerequisiteType.PathFinding)]
        protected override void Awake()
        {
            Debug.Log("TFW: PathManager hook installed");

            this.m_simulationProfiler = new ThreadProfiler();
            typeof(PathManager)
                .GetFieldByName("m_sampleName")
                .SetValue(this, base.gameObject.name);

            this.m_pathUnits = new Array32<PathUnit>(262144u);
            this.m_bufferLock = new object();
            uint num;
            this.m_pathUnits.CreateItem(out num);
            int num2 = Mathf.Clamp(SystemInfo.processorCount / 2, 1, 4);
            var facades = new ExtendedPathFindFacade[num2];
            for (int i = 0; i < num2; i++)
            {
                facades[i] = base.gameObject.AddComponent<ExtendedPathFindFacade>();
            }

            ExtendedPathManager.PathFindFacades = facades;
            typeof(PathManager)
                .GetFieldByName("m_pathfinds")
                .SetValue(this, facades.OfType<PathFind>().ToArray());
        }

        [RedirectFrom(typeof(PathManager), (ulong)PrerequisiteType.PathFinding)]
        public new bool CreatePath(out uint unit, ref Randomizer randomizer, uint buildIndex, PathUnit.Position startPos, PathUnit.Position endPos, NetInfo.LaneType laneTypes, VehicleInfo.VehicleType vehicleTypes, float maxLength)
        {
            PathUnit.Position position = default(PathUnit.Position);
            return this.CreatePath(out unit, ref randomizer, buildIndex, startPos, position, endPos, position, position, laneTypes, vehicleTypes, maxLength, false, false, false, false);
        }

        [RedirectFrom(typeof(PathManager), (ulong)PrerequisiteType.PathFinding)]
        public new bool CreatePath(out uint unit, ref Randomizer randomizer, uint buildIndex, PathUnit.Position startPosA, PathUnit.Position startPosB, PathUnit.Position endPosA, PathUnit.Position endPosB, NetInfo.LaneType laneTypes, VehicleInfo.VehicleType vehicleTypes, float maxLength)
        {
            return this.CreatePath(out unit, ref randomizer, buildIndex, startPosA, startPosB, endPosA, endPosB, default(PathUnit.Position), laneTypes, vehicleTypes, maxLength, false, false, false, false);
        }

        [RedirectFrom(typeof(PathManager), (ulong)PrerequisiteType.PathFinding)]
        public new bool CreatePath(out uint unit, ref Randomizer randomizer, uint buildIndex, PathUnit.Position startPosA, PathUnit.Position startPosB, PathUnit.Position endPosA, PathUnit.Position endPosB, NetInfo.LaneType laneTypes, VehicleInfo.VehicleType vehicleTypes, float maxLength, bool isHeavyVehicle, bool ignoreBlocked, bool stablePath, bool skipQueue)
        {
            return this.CreatePath(out unit, ref randomizer, buildIndex, startPosA, startPosB, endPosA, endPosB, default(PathUnit.Position), laneTypes, vehicleTypes, maxLength, isHeavyVehicle, ignoreBlocked, stablePath, skipQueue);
        }

        [RedirectFrom(typeof(PathManager), (ulong)PrerequisiteType.PathFinding)]
        public new bool CreatePath(out uint unit, ref Randomizer randomizer, uint buildIndex, PathUnit.Position startPosA, PathUnit.Position startPosB, PathUnit.Position endPosA, PathUnit.Position endPosB, PathUnit.Position vehiclePosition, NetInfo.LaneType laneTypes, VehicleInfo.VehicleType vehicleTypes, float maxLength, bool isHeavyVehicle, bool ignoreBlocked, bool stablePath, bool skipQueue)
        {
            return this.CreatePath(
                ExtendedVehicleType.Unknown, 
                out unit,
                ref randomizer,
                buildIndex,
                startPosA,
                startPosB,
                endPosA,
                endPosB,
                vehiclePosition,
                laneTypes,
                vehicleTypes,
                maxLength,
                isHeavyVehicle,
                ignoreBlocked,
                stablePath,
                skipQueue);
        }

        [RedirectFrom(typeof(PathManager), (ulong)PrerequisiteType.PathFinding)]
        public new void WaitForAllPaths()
        {
            if (ExtendedPathManager.PathFindFacades == null)
            {
                throw new Exception("ExtendedPathManager is not installed correctly");
            }

            for (int i = 0; i < ExtendedPathManager.PathFindFacades.Length; i++)
            {
                ExtendedPathManager.PathFindFacades[i].WaitForAllPaths();
            }
        }
    }
}
