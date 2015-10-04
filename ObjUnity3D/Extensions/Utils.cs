using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ObjUnity3D
{
    public static class Utils
    {
        public static bool HasKeys(Dictionary<string, object> lData, params string[] lKeys)
        {
            if (lKeys != null)
            {
                for (int i = 0; i < lKeys.Length; i++)
                {
                    if (!lData.ContainsKey(lKeys[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void ClearChildren(GameObject lGo, string lTarget)
        {
            if (lGo != null)
            {
                for (int i = lGo.transform.childCount - 1; i > -1; i--)
                {
                    Transform child = lGo.transform.GetChild(i);
                    if (child.name.Contains(lTarget))
                    {
                        child.parent = null;
                        UnityEngine.Object.Destroy(child.gameObject);
                    }
                }
            }
        }

        public static void ClearChildrenRegex(GameObject lGo, string lPattern)
        {
            if (lGo != null)
            {
                Regex regex = new Regex(lPattern);
                for (int i = lGo.transform.childCount - 1; i > -1; i--)
                {
                    Transform child = lGo.transform.GetChild(i);
                    if (regex.IsMatch(child.name))
                    {
                        child.parent = null;
                        UnityEngine.Object.Destroy(child.gameObject);
                    }
                }
            }
        }

        public static void VerifyObjects(string lMsg, params object[] lObjects)
        {
            for (int i = 0; i < lObjects.Length; i++)
            {
                if (lObjects[i] == null)
                {
                    Debug.LogError(lMsg);
                    return;
                }
            }
        }

        public static bool JSONCheck(string lText)
        {
            return !string.IsNullOrEmpty(lText) && lText[0] == '{';
        }

        public static Vector3 ParseVector3Json(string lJsonData)
        {
            string[] array = lJsonData.Replace("(", "").Replace(")", "").Replace(" ", "").Split(new char[]
			{
				','
			});
            Vector3 zero = Vector3.zero;
            if (!float.TryParse(array[0], out zero.x))
            {
                return Vector3.zero;
            }
            if (!float.TryParse(array[1], out zero.y))
            {
                return Vector3.zero;
            }
            if (!float.TryParse(array[2], out zero.z))
            {
                return Vector3.zero;
            }
            return zero;
        }

        public static Vector4 ParseVector4Json(string lJsonData)
        {
            string[] array = lJsonData.Replace("(", "").Replace(")", "").Replace(" ", "").Split(new char[]
			{
				','
			});
            Vector4 zero = Vector4.zero;
            if (!float.TryParse(array[0], out zero.x))
            {
                return Vector4.zero;
            }
            if (!float.TryParse(array[1], out zero.y))
            {
                return Vector4.zero;
            }
            if (!float.TryParse(array[2], out zero.z))
            {
                return Vector4.zero;
            }
            if (!float.TryParse(array[3], out zero.w))
            {
                return Vector4.zero;
            }
            return zero;
        }

        public static Vector2 ParseVector2String(string lData, char lSeperator = ' ')
        {
            string[] array = lData.Split(new char[]
			{
				lSeperator
			}, StringSplitOptions.RemoveEmptyEntries);
            float x = array[0].ParseInvariantFloat();
            float y = array[1].ParseInvariantFloat();
            return new Vector2(x, y);
        }

        public static Vector3 ParseVector3String(string lData, char lSeperator = ' ')
        {
            string[] array = lData.Split(new char[]
			{
				lSeperator
			}, StringSplitOptions.RemoveEmptyEntries);
            float x = array[0].ParseInvariantFloat();
            float y = array[1].ParseInvariantFloat();
            float z = array[2].ParseInvariantFloat();
            return new Vector3(x, y, z);
        }

        public static Vector4 ParseVector4String(string lData, char lSeperator = ' ')
        {
            string[] array = lData.Split(new char[]
			{
				lSeperator
			}, StringSplitOptions.RemoveEmptyEntries);
            float x = array[0].ParseInvariantFloat();
            float y = array[1].ParseInvariantFloat();
            float z = array[2].ParseInvariantFloat();
            float w = array[3].ParseInvariantFloat();
            return new Vector4(x, y, z, w);
        }

        public static Quaternion ParseQuaternion(string lJsonData)
        {
            string[] array = lJsonData.Replace("(", "").Replace(")", "").Replace(" ", "").Split(new char[]
			{
				','
			});
            Quaternion identity = Quaternion.identity;
            if (!float.TryParse(array[0], out identity.x))
            {
                return Quaternion.identity;
            }
            if (!float.TryParse(array[1], out identity.y))
            {
                return Quaternion.identity;
            }
            if (!float.TryParse(array[2], out identity.z))
            {
                return Quaternion.identity;
            }
            if (!float.TryParse(array[3], out identity.w))
            {
                return Quaternion.identity;
            }
            return identity;
        }

        public static string Vector3String(Vector3 lVector3)
        {
            return string.Concat(new string[]
			{
				"(",
				lVector3.x.ToString("f3"),
				",",
				lVector3.y.ToString("f3"),
				",",
				lVector3.z.ToString("f3"),
				")"
			});
        }

        public static string Vector4String(Vector4 lVector4)
        {
            return string.Concat(new string[]
			{
				"(",
				lVector4.x.ToString("f3"),
				",",
				lVector4.y.ToString("f3"),
				",",
				lVector4.z.ToString("f3"),
				",",
				lVector4.w.ToString("f3"),
				")"
			});
        }

        public static string QuaternionString(Quaternion lQuaternion)
        {
            return string.Concat(new string[]
			{
				"(",
				lQuaternion.x.ToString("f3"),
				",",
				lQuaternion.y.ToString("f3"),
				",",
				lQuaternion.z.ToString("f3"),
				",",
				lQuaternion.w.ToString("f3"),
				")"
			});
        }

        public static int FirstInt(string lJsonData)
        {
            string text = "";
            int num = 0;
            while (num < lJsonData.Length && char.IsDigit(lJsonData[num]))
            {
                text += lJsonData[num];
                num++;
            }
            return int.Parse(text);
        }
    }
}
