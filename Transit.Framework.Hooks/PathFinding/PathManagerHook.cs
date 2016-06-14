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
            Log.Info("TFW: PathManager hook installed");

            this.m_simulationProfiler = new ThreadProfiler();
            typeof(PathManager)
                .GetFieldByName("m_sampleName")
                .SetValue(this, base.gameObject.name);

            this.m_pathUnits = new Array32<PathUnit>(262144u);
            this.m_bufferLock = new object();
            uint num;
            this.m_pathUnits.CreateItem(out num);
            int num2 = Mathf.Clamp(SystemInfo.processorCount / 2, 1, 4);
            var facades = new TAMPathFindFacade[num2];
            for (int i = 0; i < num2; i++)
            {
                facades[i] = base.gameObject.AddComponent<TAMPathFindFacade>();
            }

            TAMPathFindFacadeManager.instance.Facades = facades;
            typeof(PathManager)
                .GetFieldByName("m_pathfinds")
                .SetValue(this, facades.OfType<PathFind>().ToArray());
        }

        [RedirectFrom(typeof(PathManager), (ulong)PrerequisiteType.PathFinding)]
        public new bool CreatePath(out uint unit, ref Randomizer randomizer, uint buildIndex, PathUnit.Position startPosA, PathUnit.Position startPosB, PathUnit.Position endPosA, PathUnit.Position endPosB, PathUnit.Position vehiclePosition, NetInfo.LaneType laneTypes, VehicleInfo.VehicleType vehicleTypes, float maxLength, bool isHeavyVehicle, bool ignoreBlocked, bool stablePath, bool skipQueue, bool randomParking)
        {
            return this.CreatePath(
                ExtendedUnitType.Unknown, 
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
                skipQueue,
                randomParking);
        }

        [RedirectFrom(typeof(PathManager), (ulong)PrerequisiteType.PathFinding)]
        public new void WaitForAllPaths()
        {
            if (TAMPathFindFacadeManager.instance.Facades == null)
            {
                throw new Exception("PathManagerHook is not installed correctly");
            }

            for (int i = 0; i < TAMPathFindFacadeManager.instance.Facades.Length; i++)
            {
                TAMPathFindFacadeManager.instance.Facades[i].WaitForAllPaths();
            }
        }
    }
}
