using System.Collections.Generic;
using System.Linq;
using Transit.Framework.Interfaces;

namespace Transit.Framework
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> TrimNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable
                .Where(element => element != null)
                .ToArray();
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T other)
        {
            return enumerable.Except(new[] { other });
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<T> enumerable, T other)
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

        public static IEnumerable<T> WhereMeetRequirements<T>(this IEnumerable<T> enumerable)
        {
            return enumerable
                .Where(element =>
                {
                    if (element is IDLCRequired)
                    {
                        var requiredDLC = ((IDLCRequired) element).RequiredDLC;
                        if (requiredDLC == SteamHelper.DLC.None)
                        {
                            return true;
                        }

                        return SteamHelper.IsDLCOwned(requiredDLC);
                    }

                    return true;
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
