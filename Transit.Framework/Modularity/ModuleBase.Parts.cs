using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework.Interfaces;

namespace Transit.Framework.Modularity
{
    public abstract partial class ModuleBase : IModule
    {
        private IEnumerable<IModulePart> _parts;
        public IEnumerable<IModulePart> Parts
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
                        .Select(t =>
                            {
                                var part = (IModulePart)Activator.CreateInstance(t);

                                if (part is IActivablePart)
                                {
                                    var activablePart = (IActivablePart)part;

                                    activablePart.IsEnabled = IsPartActivatedOnLoad(activablePart);
                                }
                                return part;
                            })
                        .OrderOrderables()
                        .ToArray();
                }

                return _parts;
            }
        }

        protected virtual bool IsPartActivatedOnLoad(IActivablePart part)
        {
            return true;
        }
    }
}
