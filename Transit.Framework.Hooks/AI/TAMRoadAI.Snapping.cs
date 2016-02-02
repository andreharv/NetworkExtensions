using Transit.Framework.ExtensionPoints.AI;
using Transit.Framework.Redirection;

namespace Transit.Framework.Hooks.AI
{
    public partial class TAMRoadAI : RoadAI
    {
        [RedirectFrom(typeof (RoadAI))]
        public override float GetLengthSnap()
        {
            if (RoadSnappingModeProvider.instance.HasCustomSnapping(this.m_info.name))
            {
                return RoadSnappingModeProvider
                    .instance
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