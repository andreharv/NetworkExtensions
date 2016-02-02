using System;
using System.Reflection;
using JetBrains.Annotations;
using Transit.Framework.ExtensionPoints.AI;
using Transit.Addon.RoadExtensions.AI;
using Transit.Framework;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.Alley2L;
using Transit.Addon.RoadExtensions.Roads.TinyRoads.OneWay1L;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        [UsedImplicitly]
        private class Initializer : Installer
        {
#if DEBUG
            private int _frameNb = 0;
#endif

            protected override bool ValidatePrerequisites()
            {
#if DEBUG
                if (_frameNb++ < 20)
                    // Giving some time for the UI to refresh **NB. Putting this constant higher than 100 causes wierd behavior**
                {
                    return false;
                }
#endif

                return true;
            }

            protected override void Install()
            {
                var version = GetType().Assembly.GetName().Version;
                Debug.Log(string.Format("REx: Version {0}", version));
            }
        }
    }
}
