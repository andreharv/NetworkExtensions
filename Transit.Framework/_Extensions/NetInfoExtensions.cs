using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Globalization;
using UnityEngine;

#if DEBUG

#endif

namespace Transit.Framework
{
    public static partial class NetInfoExtensions
    {
        public static void DisplayLaneProps(this NetInfo info)
        {
            foreach (var lane in info.m_lanes)
            {
                if (lane.m_laneProps != null)
                {
                    Log.Info(string.Format("TFW: Lane name {0}", lane.m_laneProps.name));

                    if (lane.m_laneProps.m_props != null)
                    {
                        foreach (var prop in lane.m_laneProps.m_props)
                        {
                            if (prop.m_prop != null)
                            {
                                Log.Info(string.Format("TFW:     Prop name {0}", prop.m_prop.name));
                            }
                        }
                    }
                }
            }
        }

        public static NetInfo.Lane FindLane(this NetInfo info, Func<string, bool> predicate, bool crashOnNotFound = true)
        {
            var lane = info
                .m_lanes
                .Where(l => l != null && l.m_laneProps != null && l.m_laneProps.name != null && l.m_laneProps.m_props != null)
                .FirstOrDefault(l => predicate(l.m_laneProps.name.ToLower()));

            if (lane == null)
            {
                if (crashOnNotFound)
                {
                    throw new Exception("TFW: Lane not found");
                }
            }

            return lane;
        }

        public static NetInfo.Lane FindLane(this NetInfo info, NetInfo.LaneType predicate, bool crashOnNotFound = true)
        {
            var lane = info
                .m_lanes
                .FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.Vehicle);

            if (lane == null)
            {
                if (crashOnNotFound)
                {
                    throw new Exception("TFW: Lane not found");
                }
            }

            return lane;
        }

        public static void ModifyTitle(this NetInfo info, string newTitle)
        {
            var localizedStringsField = typeof(Locale).GetFieldByName("m_LocalizedStrings");
            var locale = SingletonLite<LocaleManager>.instance.GetLocale();
            var localizedStrings = (Dictionary<Locale.Key, string>)localizedStringsField.GetValue(locale);

            var kvp =
                localizedStrings
                .FirstOrDefault(kvpInternal =>
                    kvpInternal.Key.m_Identifier == "NET_TITLE" &&
                    kvpInternal.Key.m_Key == info.name);

            if (!Equals(kvp, default(KeyValuePair<Locale.Key, string>)))
            {
                localizedStrings[kvp.Key] = newTitle;
            }
        }
    }
}
