using Transit.Framework.ExtensionPoints.PathFinding.Contracts;
using Transit.Framework.ExtensionPoints.PathFindingFeatures;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public static class PathFindExtensions
    {
        public static TAMPathFindFeatureManager GetFeatures(this IPathFind pathFind)
        {
            return TAMPathFindFeatureManager.instance;
        }
    }
}
