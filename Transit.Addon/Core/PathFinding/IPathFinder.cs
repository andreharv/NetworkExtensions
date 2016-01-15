
using System;

namespace Transit.Addon.Core.PathFinding
{
    public interface IPathFinder
    {
        void ProcessPathUnit(uint unit, ref PathUnit data);
    }
}
