using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Install;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        private static IEnumerable<IModulePart> s_parts;
        public static IEnumerable<IModulePart> Parts
        {
            get
            {
                if (s_parts == null)
                {
                    var partType = typeof(IModulePart);

                    s_parts = typeof(RoadsInstaller)
                        .Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract && !t.IsInterface)
                        .Where(partType.IsAssignableFrom)
                        .Select(t =>
                        {
                            var part = (IModulePart)Activator.CreateInstance(t);

                            if (part is IActivablePart)
                            {
                                var activable = (IActivablePart) part;

                                activable.IsEnabled = Options.Instance.IsPartEnabled(activable);
                            }
                            return part;
                        })
                        .ToArray();
                }

                return s_parts;
            }
        }

        public static IEnumerable<IActivablePart> ActivableParts
        {
            get
            {
                return Parts
                    .OfType<IActivablePart>()
                    .OrderBy(p => p.Order)
                    .ToArray();
            }
        }

        public static IEnumerable<INetInfoBuilder> NetInfoBuilders
        {
            get
            {
                return ActivableParts
                    .Where(p => p.IsEnabled)
                    .OfType<INetInfoBuilder>()
                    .ToArray();
            }
        }

        public static IEnumerable<INetInfoModifier> NetInfoModifiers
        {
            get
            {
                return ActivableParts
                    .Where(p => p.IsEnabled)
                    .OfType<INetInfoModifier>()
                    .ToArray();
            }
        }

        public static IEnumerable<ICompatibilityPart> CompatibilityParts
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
