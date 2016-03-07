
namespace Transit.Framework.ExtensionPoints.PathFinding
{
    public interface IPathFindingImplementation
    {
        void ProcessPathUnit(uint unit, ref PathUnit data);
    }
}
