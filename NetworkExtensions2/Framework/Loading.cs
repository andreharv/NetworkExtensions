using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using ColossalFramework;
using UnityEngine;

namespace Transit.Framework
{
    public static class Loading
    {
        public static void QueueAction(IEnumerator ienum)
        {
            Singleton<LoadingManager>.instance.QueueLoadingAction(ienum);
        }

        public static void QueueAction(Action a)
        {
            Singleton<LoadingManager>.instance.QueueLoadingAction(a.AsEnumerator());
        }

        private static IEnumerator AsEnumerator(this Action a)
        {
            a.Invoke();
            yield break;
        }
    }
}
