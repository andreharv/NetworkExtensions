using Transit.Framework.Modularity;

namespace Transit.Addon.PathFinding
{
    [Module(typeof(Mod))]
    public partial class PathfindingModule : ModuleBase
    {
        public override string Name
        {
            get { return "Pathfinding"; }
        }
    }
}
