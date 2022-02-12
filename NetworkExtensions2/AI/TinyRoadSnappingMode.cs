using Transit.Framework.ExtensionPoints.AI;

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