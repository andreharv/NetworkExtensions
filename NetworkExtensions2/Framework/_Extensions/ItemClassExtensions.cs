using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Transit.Framework
{
    public static class ItemClassExtensions
    {
        private static readonly IDictionary<string, ItemClass> s_newClasses = new Dictionary<string, ItemClass>(StringComparer.InvariantCultureIgnoreCase); 

        public static ItemClass Clone(this ItemClass itemClass, string newName)
        {
            if (!s_newClasses.ContainsKey(newName))
            {
                var newClass = ScriptableObject.CreateInstance<ItemClass>();
                newClass.m_layer = itemClass.m_layer;
                newClass.m_level = itemClass.m_level;
                newClass.m_service = itemClass.m_service;
                newClass.m_subService = itemClass.m_subService;
                newClass.hideFlags = itemClass.hideFlags;
                newClass.name = newName;

                s_newClasses[newName] = newClass;
            }

            return s_newClasses[newName];
        }
    }
}
