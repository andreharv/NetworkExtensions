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
                    _parts = GetType()
                        .Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract && !t.IsInterface)
                        .Where(typeof(IModulePart).IsAssignableFrom)
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
