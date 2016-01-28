
namespace Transit.Framework.Extenders.PathFinding
{
    public interface IPathFindingImplementation
    {
        void ProcessPathUnit(uint unit, ref PathUnit data);
    }
}
