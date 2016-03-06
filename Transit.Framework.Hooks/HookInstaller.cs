using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;
using UnityEngine;

namespace Transit.Framework.Hooks
{
    public class HookInstaller : IPrerequisiteSetup
    {
        public void Install(PrerequisiteType type)
        {
            if (type == PrerequisiteType.None)
            {
                return;
            }

            Debug.Log("TFW: Installing requirements of type(s) " + type);

            Redirector.PerformRedirections((ulong) type);
        }

        public void Uninstall(PrerequisiteType type)
        {
            if (type == PrerequisiteType.None)
            {
                return;
            }

            Debug.Log("TFW: Uninstalling requirements of type(s) " + type);

            Redirector.RevertRedirections((ulong) type);
        }
    }
}
