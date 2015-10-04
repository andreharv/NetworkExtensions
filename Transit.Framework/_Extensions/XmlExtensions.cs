using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Transit.Framework
{
    public static class XmlExtensions
    {
        public static XmlElement AppendElement(this XmlDocument doc, string name, string innerText = null)
        {
            XmlElement newElement = doc.CreateElement(name);
            newElement.InnerText = innerText;
            doc.AppendChild(newElement);

            return newElement;
        }

        public static XmlElement AppendElement(this XmlElement element, string name, string innerText = null)
        {
            XmlElement newElement = element.OwnerDocument.CreateElement(name);
            newElement.InnerText = innerText;
            element.AppendChild(newElement);

            return newElement;
        }

        public static XmlAttribute AppendAttribute(this XmlElement element, string name, string value = null)
        {
            XmlAttribute newAttribute = element.OwnerDocument.CreateAttribute(name);
            newAttribute.Value = value;
            element.Attributes.Append(newAttribute);

            return newAttribute;
        }
    }
}
