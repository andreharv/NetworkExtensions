using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Transit.Framework
{
    public static class Extensibility
    {
        public static IEnumerable<Type> GetImplementations<T>()
        {
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetImplementations<T>());
        }

        public static IEnumerable<Type> GetImplementations<T>(this Assembly assembly)
        {
            var lookupType = typeof(T);

            IEnumerable<Type> allTypes;
            try
            {
                allTypes = assembly.GetTypes();
            }
            catch (Exception)
            {
                //Debug.Log("TFW: GetImplementations looking into assembly " + a.FullName);
                //Debug.Log("TFW: " + ex.Message);
                //Debug.Log("TFW: " + ex.ToString());
                allTypes = new Type[] { };
            }

            return allTypes
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => lookupType.IsAssignableFrom(t));
        }
    }
}
