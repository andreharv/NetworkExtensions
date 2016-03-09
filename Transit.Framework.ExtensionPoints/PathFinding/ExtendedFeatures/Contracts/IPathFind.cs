using System.Threading;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts
{
    public interface IPathFind
    {
        ExtendedPathFindFacade Facade { get; set; }

        void OnAwake();

        void OnDestroy();

        Thread PathFindThread { get; }

        bool CalculatePath(ExtendedVehicleType vehicleType, uint unit, bool skipQueue);

        void WaitForAllPaths();
    }
}
