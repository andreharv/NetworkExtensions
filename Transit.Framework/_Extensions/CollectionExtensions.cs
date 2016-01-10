using System;
using System.Collections.Generic;
using System.Linq;

namespace Transit.Framework
{
    public static class CollectionExtensions
    {
        public static void Trim<T>(this ICollection<T> collection, Func<T, bool> predicate)
            where T : class
        {
            foreach (var item in collection.ToArray())
            {
                if (predicate(item))
                {
                    collection.Remove(item);
                }
            }
        }
    }
}
