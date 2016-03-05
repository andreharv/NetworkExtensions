using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;

namespace Transit.Framework.Hooks.PathFinding
{
    public class PathFindHook : PathFind
    {
        [RedirectFrom(typeof(PathFind), (ulong)PrerequisiteType.PathFinding)]
        private void Awake()
        {
            // Disabling the vanilla Awake method
        }

        [RedirectFrom(typeof(PathFind), (ulong)PrerequisiteType.PathFinding)]
        private void OnDestroy()
        {
            // Disabling the vanilla Awake method
        }
    }
}
