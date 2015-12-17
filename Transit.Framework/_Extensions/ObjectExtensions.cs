using System;

namespace Transit.Framework
{
    public static class ObjectExtensions
    {
        public static void Maybe<TInstance>(this TInstance instance, Action<TInstance> action)
        {
            if (instance != null)
            {
                action(instance);
            }
        }

        public static void Maybe<TInstance>(this object instance, Action<TInstance> action)
            where TInstance : class
        {
            Maybe<TInstance>(instance as TInstance, action);
        }

        public static TResult SelectOrDefault<TInstance, TResult>(this TInstance instance, Func<TInstance, TResult> selector)
        {
            return SelectOrDefault(instance, selector, default(TResult));
        }

        public static TResult SelectOrDefault<TInstance, TResult>(this TInstance instance, Func<TInstance, TResult> selector, TResult defaultValue)
        {
            return instance == null ? defaultValue : selector(instance);
        }

        public static T With<T>(this T target, Action<T> assignations)
        {
            assignations(target);
            return target;
        }

        public static bool IsNotNull(this object target)
        {
            return target != null;
        }
    }
}
