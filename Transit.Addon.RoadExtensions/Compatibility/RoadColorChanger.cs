using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ColossalFramework;
using ColossalFramework.Plugins;
using Transit.Framework;
using Transit.Framework.Modularity;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Compatibility
{
    public class RoadColorChanger : ICompatibilityPart
    {
        private const string COLOR_CHANGER_MOD_ID = "417585852";
        private const string CONFIG_PATH = "RoadColorConfig.xml";

        public string Name
        {
            get { return "RoadColorChanger for RoadExtensions"; }
        }

        public bool IsPluginActive
        {
            get { return Singleton<PluginManager>.instance.IsPluginActive(COLOR_CHANGER_MOD_ID); }
        }

        public void Setup(IEnumerable<NetInfo> newRoads)
        {
            var config = Configuration2.Deserialize(CONFIG_PATH);

            foreach (var ni in newRoads)
            {
                ni.m_color = new Color(config.highway_red, config.highway_green, config.highway_blue);
            }
        }

        public class Configuration2
        {
            public float highway_red = 0.25f;

            public float highway_green = 0.25f;

            public float highway_blue = 0.25f;

            public float large_road_red = 0.25f;

            public float large_road_green = 0.25f;

            public float large_road_blue = 0.25f;

            public float medium_road_red = 0.25f;

            public float medium_road_green = 0.25f;

            public float medium_road_blue = 0.25f;

            public float small_road_red = 0.25f;

            public float small_road_green = 0.25f;

            public float small_road_blue = 0.25f;

            public static Configuration2 Deserialize(string filename)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration2));
                try
                {
                    using (StreamReader streamReader = new StreamReader(filename))
                    {
                        return (Configuration2)xmlSerializer.Deserialize(streamReader);
                    }
                }
                catch
                {
                    return new Configuration2();
                }
            }
        }
    }
}
