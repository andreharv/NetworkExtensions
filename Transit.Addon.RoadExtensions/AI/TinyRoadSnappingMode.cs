using Transit.Framework.Extenders.AI;

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