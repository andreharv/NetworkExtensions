using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Framework
{
    public static partial class NetInfoExtensions
    {
        public static NetInfo Clone(this NetInfo originalNetInfo, string newName)
        {
            //Debug.Log(String.Format("TFW: Cloning {0} -> {1}", originalNetInfo.name, newName));

            var gameObject = Object.Instantiate(originalNetInfo.gameObject);
            gameObject.transform.parent = originalNetInfo.gameObject.transform; // N.B. This line is evil and removing it is killoing the game's performances
            gameObject.name = newName;

            var info = gameObject.GetComponent<NetInfo>();
            info.m_prefabInitialized = false;

            //Debug.Log(String.Format("TFW: Cloning completed {0} -> {1}", originalNetInfo.name, newName));

            return info;
        }

        public static void DisplayLaneProps(this NetInfo info)
        {
            foreach (var lane in info.m_lanes)
            {
                if (lane.m_laneProps != null)
                {
                    Debug.Log(string.Format("TFW: Lane name {0}", lane.m_laneProps.name));

                    if (lane.m_laneProps.m_props != null)
                    {
                        foreach (var prop in lane.m_laneProps.m_props)
                        {
                            if (prop.m_prop != null)
                            {
                                Debug.Log(string.Format("TFW:     Prop name {0}", prop.m_prop.name));
                            }
                        }
                    }
                }
            }
        }
    }
}
