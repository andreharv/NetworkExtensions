using System.Collections.Generic;
using System.Linq;
using Transit.Framework.Interfaces;

namespace Transit.Framework
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> TrimNull<T>(this IEnumerable<T> enumerable)
            where T: class
        {
            return enumerable
                .Where(element => element != null)
                .ToArray();
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T other)
            where T : class
        {
            return enumerable.Except(new[] { other });
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<T> enumerable, T other)
            where T : class
        {
            return enumerable.Union(new [] { other });
        }

        public static IEnumerable<T> WhereActivated<T>(this IEnumerable<T> enumerable, bool returnNonActivable = true)
        {
            return enumerable
                .Where(element =>
                {
                    if (element is IActivable)
                    {
                        return ((IActivable) element).IsEnabled;
                    }

                    return returnNonActivable;
                });
        }

        public static IEnumerable<T> OrderOrderables<T>(this IEnumerable<T> enumerable)
        {
            return enumerable
                .OrderBy(element =>
                {
                    if (element is IOrderable)
                    {
                        return ((IOrderable) element).Order;
                    }

                    return int.MaxValue;
                });
        }
    }
}
