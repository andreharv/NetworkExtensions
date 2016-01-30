using ICities;
using Transit.Framework.Unsafe;

namespace Transit.Framework.UI
{
    public class UILoading : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            Redirector.PerformRedirections();
        }

        public override void OnReleased()
        {
            Redirector.RevertRedirections();
            GameToolbar.Reset();
        }
    }
}
