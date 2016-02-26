using JetBrains.Annotations;
using Transit.Framework;

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
