using ColossalFramework.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Transit.Framework
{
    public static class Tools
    {
        public static void Compare<T>(T unityObj, T otherUnityObj)
             where T : Object
        {
            Debug.Log(string.Format("TFW: ----->  Comparing {0} with {1}", unityObj.name, otherUnityObj.name));

            var fields = typeof(T).GetAllFieldsFromType();

            foreach (var f in fields)
            {
                var newValue = f.GetValue(unityObj);
                var oldValue = f.GetValue(otherUnityObj);

                if (!Equals(newValue, oldValue))
                {
                    Debug.Log(string.Format("Value {0} not equal (N-O) ({1},{2})", f.Name, newValue, oldValue));
                }
            }
        }

        public static void ListMembers<T>(this T unityObj)
            where T : Object
        {
            Debug.Log(string.Format("TFW: ----->  Listing {0}", unityObj.name));

            var fields = typeof(T).GetAllFieldsFromType();

            foreach (var f in fields)
            {
                var value = f.GetValue(unityObj);
                Debug.Log(string.Format("Member name \"{0}\" value is \"{1}\"", f.Name, value));
            }
        }
    }
}
