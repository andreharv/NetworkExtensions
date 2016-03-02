using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;

namespace Transit.Framework.Hooks
{
    public class HookInstaller : IPrerequisiteSetup
    {
        public void Install(PrerequisiteType type)
        {
            Redirector.PerformRedirections((ulong) type);
        }

        public void Uninstall(PrerequisiteType type)
        {
            Redirector.RevertRedirections((ulong) type);
        }
    }
}
