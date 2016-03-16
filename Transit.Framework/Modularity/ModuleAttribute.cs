using System;
using System.Collections.Generic;

namespace Transit.Framework.Modularity
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ModuleAttribute : Attribute
    {
        private readonly HashSet<string> _associatedMods;

        public ModuleAttribute(Type mod)
        {
            _associatedMods = new HashSet<string>(new[] {mod.FullName}, StringComparer.InvariantCultureIgnoreCase);
        }

        public ModuleAttribute(params string[] associatedMods)
        {
            _associatedMods = new HashSet<string>(associatedMods, StringComparer.InvariantCultureIgnoreCase);
        }

        public bool IsAssociatedWith(Type type)
        {
            return _associatedMods.Contains(type.FullName) || _associatedMods.Contains(type.Namespace);
        }
    }
}
