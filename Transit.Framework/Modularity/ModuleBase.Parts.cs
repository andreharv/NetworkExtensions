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

                                if (part is IActivable)
                                {
                                    var activable = (IActivablePart)part;

                                    activable.IsEnabled = IsPartActivatedOnLoad(activable);
                                }
                                return part;
                            })
                        .OrderBy(m =>
                            {
                                if (m is IOrderable)
                                {
                                    return ((IOrderable) m).Order;
                                }
                                else
                                {
                                    return int.MaxValue;
                                }
                            })
                        .ToArray();
                }

                return _parts;
            }
        }

        protected virtual bool IsPartActivatedOnLoad(IActivablePart activatable)
        {
            return true;
        }
    }
}
