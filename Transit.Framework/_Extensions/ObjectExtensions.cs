using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Transit.Framework
{
    public static class ObjectExtensions
    {
        public static object GetFieldValue(this object target, string fieldName, bool crashOnNotFound = true)
        {
            return GetFieldValue<object>(target, fieldName, crashOnNotFound);
        }

        public static T GetFieldValue<T>(this object target, string fieldName, bool crashOnNotFound = true) 
            where T : class
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
            {
                if (crashOnNotFound)
                {
                    throw new Exception(string.Format("TFW: Field \"{0}\" not found", fieldName));
                }
                else
                {
                    return default(T);
                }
            }

            return field.GetValue(target) as T;
        }

        public static void SetFieldValue(this object target, string fieldName, object value, bool crashOnNotFound = true)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
            {
                if (crashOnNotFound)
                {
                    throw new Exception(string.Format("TFW: Field \"{0}\" not found", fieldName));
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
