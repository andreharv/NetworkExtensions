using System.Threading;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.Contracts
{
    public interface IPathFind
    {
        TAMPathFindFacade Facade { get; set; }

        void OnAwake();

        void OnDestroy();

        Thread PathFindThread { get; }

        bool CalculatePath(ExtendedUnitType unitType, uint unit, bool skipQueue);

        void WaitForAllPaths();
    }
}
