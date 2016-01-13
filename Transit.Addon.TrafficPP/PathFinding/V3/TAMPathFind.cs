using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using ColossalFramework;
using ColossalFramework.Math;
using Transit.Framework.Unsafe;
using UnityEngine;

namespace Transit.Addon.TrafficPP.PathFinding.V3
{
    public class TAMPathFind : PathFind
    {
        static Dictionary<int, CustomPathFind> s_pathFinds = new Dictionary<int, CustomPathFind>();

        [RedirectFrom(typeof(PathFind))]
        private void PathFindImplementation(uint unit, ref PathUnit data)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            CustomPathFind pathFind;
            if (!s_pathFinds.TryGetValue(threadId, out pathFind))
            {
                pathFind = new CustomPathFind();
                s_pathFinds.Add(threadId, pathFind);
            }

            pathFind.PathFindImplementation(unit, ref data);
        }
    }
}
