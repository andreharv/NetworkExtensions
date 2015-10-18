using System;

namespace Transit.Framework.Modularity
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ModuleAttribute : Attribute
    {
        public Type Mod { get; set; }
    }
}
