using System.Collections.Generic;
using System.Linq;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        public IEnumerable<IActivablePart> ActivableParts
        {
            get
            {
                return Parts
                    .OfType<IActivablePart>()
                    .ToArray();
            }
        }

        public IEnumerable<INetInfoBuilder> NetInfoBuilders
        {
            get
            {
                return ActivableParts
                    .Where(p => p.IsEnabled)
                    .OfType<INetInfoBuilder>()
                    .ToArray();
            }
        }

        public IEnumerable<INetInfoModifier> NetInfoModifiers
        {
            get
            {
                return ActivableParts
                    .Where(p => p.IsEnabled)
                    .OfType<INetInfoModifier>()
                    .ToArray();
            }
        }

        public IEnumerable<ICompatibilityPart> CompatibilityParts
        {
            get
            {
                return Parts
                    .OfType<ICompatibilityPart>()
                    .ToArray();
            }
        }
    }
}
