using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Addon.RoadExtensions
{
    public class Options
    {
        private const string FILENAME = "NetworkExtensionsConfig.xml";

        private static Options s_instance;
        public static Options Instance 
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = Load();
                    s_instance.Save();
                }

                return s_instance;
            }
        }

        public readonly IDictionary<string, bool> PartsEnabled = new Dictionary<string, bool>();

        public bool IsPartEnabled(IActivablePart part)
        {
            var partName = part.GetSerializableName();

            var isEnabled = true;
            if (PartsEnabled.ContainsKey(partName))
            {
                isEnabled = PartsEnabled[partName];
            }

            return isEnabled;
        }

        public void Save()
        {
            if (RExModule.GetPath() == RExModule.PATH_NOT_FOUND)
            {
                return;
            }

            Debug.Log(string.Format("REx: Saving config at {0}", FILENAME));

            try
            {
                var xDoc = new XmlDocument();
                var settings = xDoc.CreateElement("NetworkExtensionsSettings");

                xDoc.AppendChild(settings);

                foreach (var part in PartsEnabled)
                {
                    var xmlElem = xDoc.CreateElement(part.Key);
                    xmlElem.InnerText = part.Value.ToString();

                    settings.AppendChild(xmlElem);
                }

                xDoc.Save(FILENAME);
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("REx: Crashed saving config at {0} {1}", FILENAME, ex));
            }
        }

        private static Options Load()
        {
            if (RExModule.GetPath() == RExModule.PATH_NOT_FOUND)
            {
                return new Options();
            }

            Debug.Log(string.Format("REx: Loading config at {0}", FILENAME));

            if (!File.Exists(FILENAME))
            {
                return new Options();
            }

            try
            {
                var configuration = new Options();
                var xDoc = new XmlDocument();
                xDoc.Load(FILENAME);

                if (xDoc.DocumentElement == null)
                {
                    return configuration;
                }

                foreach (XmlNode node in xDoc.DocumentElement.ChildNodes)
                {
                    var nodeValue = true;

                    if (!bool.TryParse(node.InnerText, out nodeValue))
                    {
                        nodeValue = true;
                    }

                    configuration.PartsEnabled[node.Name] = nodeValue;
                }

                return configuration;
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("REx: Crashed load config at {0} {1}", FILENAME, ex));
                return new Options();
            }
        }
    }
}
