using System;
using System.Collections.Generic;
using System.Linq;

namespace Transit.Framework
{
    public static class CollectionExtensions
    {
        public static void Trim<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            foreach (var item in collection.ToArray())
            {
                if (predicate(item))
                {
                    collection.Remove(item);
                }
            }
        }

        public static void RemoveIfAny<T>(this ICollection<T> collection, T item)
        {
            if (collection.Contains(item))
            {
                collection.Remove(item);
            }
        }
    }
}
