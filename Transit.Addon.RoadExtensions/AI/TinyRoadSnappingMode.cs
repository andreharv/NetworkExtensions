using Transit.Framework.ExtensionPoints.AI;
using Transit.Framework.ExtensionPoints.AI.Networks;

namespace Transit.Addon.RoadExtensions.AI
{
    public class TinyRoadSnappingMode : IRoadSnappingMode
    {
        public float GetLengthSnap()
        {
            return 4f;
        }
    }
}