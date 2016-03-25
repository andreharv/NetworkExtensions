using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
		
        public static object GetFieldValue(this object target, string fieldName, bool crashOnNotFound = true)
        {
            return GetFieldValue<object>(target, fieldName, crashOnNotFound);
        }

        public static T GetFieldValue<T>(this object target, string fieldName, bool crashOnNotFound = true) 
        {
            var field = target.GetType().GetFieldRecursive(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
            {
                if (crashOnNotFound)
                {
                    throw new Exception(string.Format("TFW: Field \"{0}\" not found on object {1}", fieldName, target.GetType()));
                }
                else
                {
                    return default(T);
                }
            }

            return (T)field.GetValue(target);
        }

        public static void SetFieldValue(this object target, string fieldName, object value, bool crashOnNotFound = true)
        {
            var field = target.GetType().GetFieldRecursive(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
            {
                if (crashOnNotFound)
                {
                    throw new Exception(string.Format("TFW: Field \"{0}\" not found on object {1}", fieldName, target.GetType()));
                }
                else
                {
                    return;
                }
            }

            field.SetValue(target, value);
        }
    }
}
