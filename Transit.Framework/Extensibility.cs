using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Transit.Framework
{
    public static class Extensibility
    {
        public static IEnumerable<Type> GetImplementations<T>()
        {
            var lookupType = typeof(T);

            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (Exception)
                    {
                        //Debug.Log("TFW: GetImplementations looking into assembly " + a.FullName);
                        //Debug.Log("TFW: " + ex.Message);
                        //Debug.Log("TFW: " + ex.ToString());
                        return new Type[] { };
                    }
                })
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => lookupType.IsAssignableFrom(t));
        }
    }
}
