using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Transit.Framework
{
    public static class EnumExtensions
    {
        /// <summary>
        /// No type-safety check.
        /// </summary>
        public static bool HasFlag(this Enum e1, Enum e2)
        {
            ulong e = Convert.ToUInt64(e1);
            ulong f = Convert.ToUInt64(e2);

            return (e & f) == f;
        }

        /// <summary>
        /// Appends elements to the given element with the enum values except the first
        /// </summary>
        public static void ToXml(this Enum e, XmlElement xmlElement)
        {
            string[] names = Enum.GetNames(e.GetType()).Skip(1).ToArray();
            string[] values = e.ToString().Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string name in names)
            {
                xmlElement.AppendElement(name, values.Contains(name) ? "True" : "False");
            }
        }

        /// <summary>
        /// Loads an enum from the given xml element.
        /// </summary>
        public static object FromXml(this Enum e, XmlElement xmlElement)
        {
            string[] names = Enum.GetNames(e.GetType());
            List<string> values = new List<string>();

            foreach (XmlNode node in xmlElement.ChildNodes)
            {
                string nodeName = node.Name;
                string nodeValue = node.InnerText;

                if (names.Contains(nodeName) && nodeValue == "True")
                {
                    values.Add(nodeName);
                }
            }

            string value = string.Join(", ", values.ToArray());
            return Enum.Parse(e.GetType(), value);
        }
    }
}
