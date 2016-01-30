using Transit.Framework.Prerequisites;
using Transit.Framework.Unsafe;

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
        }
    }
}
