
namespace Transit.Addon.Core.Extenders.PathFinding
{
    public interface IPathFindingImplementation
    {
        void ProcessPathUnit(uint unit, ref PathUnit data);
    }
}
