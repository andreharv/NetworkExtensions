using ColossalFramework;

namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public class TAMPathFindFacadeManager : Singleton<TAMPathFindFacadeManager>
    {
        public TAMPathFindFacade[] Facades { get; set; }
    }
}
