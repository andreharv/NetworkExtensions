using System;
using System.Collections.Generic;
using System.Linq;

namespace Transit.Framework.Modularity
{
    public abstract partial class ModuleBase : IModule
    {
        private IEnumerable<IModulePart> _parts;
        public virtual IEnumerable<IModulePart> Parts
        {
            get
            {
                if (_parts == null)
                {
                    var partType = typeof(IModulePart);

                    _parts = GetType()
                        .Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract && !t.IsInterface)
                        .Where(partType.IsAssignableFrom)
                        .Select(t => (IModulePart)Activator.CreateInstance(t))
                        .WhereMeetRequirements()
                        .OrderOrderables()
                        .ToArray();
                }

                return _parts;
            }
        }
    }
}
