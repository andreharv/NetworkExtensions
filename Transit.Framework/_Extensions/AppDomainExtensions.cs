using System;
using System.Linq;

namespace Transit.Framework
{
    public static class AppDomainExtensions
    {
        public static Type GetTypeFromName(this AppDomain appDomain, string name)
        {
            return appDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}