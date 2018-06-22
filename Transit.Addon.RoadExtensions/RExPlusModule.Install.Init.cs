using JetBrains.Annotations;
using Transit.Framework;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace TransitPlus.Addon.RoadExtensions
{
    public partial class RExPlusModule
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
                Debug.Log(string.Format("RExPlus Version {0}", version));
            }
        }
    }
}
