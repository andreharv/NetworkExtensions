using Transit.Framework.ExtensionPoints.AI;
using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;

namespace Transit.Framework.Hooks.AI
{
    public partial class RoadAIHook : RoadAI
    {
        [RedirectFrom(typeof(RoadAI), (ulong)PrerequisiteType.AI)]
        public override float GetLengthSnap()
        {
            if (RoadSnappingModeManager.HasCustomSnapping(this.m_info.name))
            {
                return RoadSnappingModeManager
                    .GetCustomSnapping(this.m_info.name)
                    .GetLengthSnap();
            }
            else
            {
                return (!this.m_enableZoning) ? 0f : 8f;
            }
        }
    }
}