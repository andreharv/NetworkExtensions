using System.Linq;
using ColossalFramework;
using ColossalFramework.Math;
using System;
using System.Threading;
using UnityEngine;
using Transit.Framework.Light;
using Transit.Framework.Unsafe;

namespace CSL_Traffic
{
	/*
	 * The PathManager is needed to use the CustomPathFind class that is where the real magic happens.
	 * There's some work to do here as I have some old code that isn't used anymore.
	 */
    public partial class CustomPathManagerProxy : PathManager
    {
        protected override void Awake()
        {
            PathFind[] originalPathFinds = GetComponents<PathFind>();
            CustomPathManager.m_pathfinds = new CustomPathFind[originalPathFinds.Length];
            for (int i = 0; i < originalPathFinds.Length; i++)
            {
                Destroy(originalPathFinds[i]);
                CustomPathManager.m_pathfinds[i] = gameObject.AddComponent<CustomPathFind>();
            }
            typeof(PathManager).GetFieldByName("m_pathfinds").SetValue(this, CustomPathManager.m_pathfinds.OfType<PathFind>().ToArray());
        }

        // copy values from original to new path manager
        public void SetOriginalValues(PathManager originalPathManager)
        {
            // members of SimulationManagerBase
            this.m_simulationProfiler = originalPathManager.m_simulationProfiler;
            this.m_drawCallData = originalPathManager.m_drawCallData;
            this.m_properties = originalPathManager.m_properties;

            // members of PathManager
            this.m_pathUnitCount = originalPathManager.m_pathUnitCount;
            this.m_renderPathGizmo = originalPathManager.m_renderPathGizmo;
            this.m_pathUnits = originalPathManager.m_pathUnits;
            this.m_bufferLock = originalPathManager.m_bufferLock;
        }
    }
}
