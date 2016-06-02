using System.Linq;
using Transit.Framework;
using Transit.Framework.Redirection;
using UnityEngine;

namespace CSL_Traffic
{
    public class PathManagerHook : PathManager
    {
        [RedirectFrom(typeof(PathManager))]
        protected override void Awake()
        {
            Logger.LogInfo("PathManager hook installed");

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
    }
}
