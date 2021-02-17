using System;
using System.Linq;
using UnityEngine;

namespace Transit.Framework
{
    public static class AppDomainExtensions
    {
        public static Type GetTypeFromName(this AppDomain appDomain, string name)
        {
            return appDomain
                .GetAssemblies()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetExportedTypes();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("TFW: GetTypeFromName looking into assembly " + a.FullName);
                        Debug.Log("TFW: " + ex.Message);
                        Debug.Log("TFW: " + ex.ToString());
                        return new Type[] { };
                    }
                })
                .FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}