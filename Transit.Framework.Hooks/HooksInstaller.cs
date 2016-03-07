using Transit.Framework.ExtensionPoints.UI;
using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;

namespace Transit.Framework.Hooks
{
    public class HooksInstaller : IModPrerequisite
    {
        public void Install()
        {
            Redirector.PerformRedirections();
        }

        public void Uninstall()
        {
            Redirector.RevertRedirections();

            // TODO: do this every time the mod is "Released" instead of "Disabled"
            GameMenuManager.Reset();
        }
    }
}
