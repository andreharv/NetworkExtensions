using System.Threading;
using Transit.Framework.Network;

namespace Transit.Framework.ExtensionPoints.PathFinding.ExtendedFeatures.Contracts
{
    public interface IExtendedPathFind
    {
        ExtendedPathFindFacade Facade { get; set; }

        void OnAwake();

        void OnDestroy();

        Thread PathFindThread { get; }

        bool CalculatePath(uint unit, bool skipQueue, ExtendedVehicleType vehicleType);

        void WaitForAllPaths();
    }
}
