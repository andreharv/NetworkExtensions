using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Globalization;
using NetworkExtensions2.Roads.Common;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
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
        public static bool HasAsymmetricalLanes(this NetInfo info)
        {
            var forwardLanes = info.m_lanes.Where(l => l.m_direction == NetInfo.Direction.Forward).OrderBy(l => Math.Abs(l.m_position)).ToList();
            var backwardLanes = info.m_lanes.Where(l => l.m_direction == NetInfo.Direction.Backward).OrderBy(l => Math.Abs(l.m_position)).ToList();
            if (backwardLanes.Count() > 0 && forwardLanes.Count() > 0)
            {
                if (forwardLanes.Count() == backwardLanes.Count())
                {
                    for (int i = 0; i < forwardLanes.Count(); i++)
                    {
                        if (Math.Abs(forwardLanes[i].m_position) != Math.Abs(forwardLanes[i].m_position))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        public static void GenerateNetInfoProperties(this NetInfo info, NetInfo.Segment defaultSegment, params NetStrip[] netStrips)
        {
            
            float previousModelWidth = 0;
            var totalWidth = 0f;
            for (int i = 0; i < netStrips.Length; i++)
            {
                var netStrip = netStrips[i];
                if (!netStrip.OverlayPrevious)
                    totalWidth += GetModelWidth(netStrip.ModelName);
            }
            info.m_halfWidth = totalWidth / 2;
            var currentPosition = 0.5f * -totalWidth;
            var segments = new NetInfo.Segment[netStrips.Length];
            var lanes = new NetInfo.Lane[netStrips.Length];
            var modelPedestrianLane = info.m_lanes.FirstOrDefault(l => l.m_laneProps != null && l.m_laneType == NetInfo.LaneType.Pedestrian);
            var modelRoadLane = info.m_lanes.FirstOrDefault(l => l.m_laneProps != null && l.m_laneType == NetInfo.LaneType.Vehicle);
            for (int i = 0; i < netStrips.Length; i++)
            {
                var netStrip = netStrips[i];
                var modelWidth = GetModelWidth(netStrip.ModelName);
                var laneType = GetModelLaneType(netStrip.ModelName);
                if (!netStrip.OverlayPrevious)
                {
                    currentPosition += 0.5f * (previousModelWidth + modelWidth);
                    previousModelWidth = modelWidth;
                }

                var inverted = currentPosition > 0;
                var segment = netStrip.TemplateSegment != null ? netStrip.TemplateSegment.ShallowClone() : defaultSegment.ShallowClone();
                segments[i] = segment.SetFlagsDefault().SetNetResources(netStrip.ModelName, netStrip.TextureName, currentPosition, inverted);

                NetInfo.Lane lane = null;
                switch (laneType)
                {
                    case NetInfo.LaneType.Vehicle:
                        lane = modelRoadLane.CloneWithoutStops();
                        break;
                    case NetInfo.LaneType.Pedestrian:
                        lane = modelPedestrianLane.CloneWithoutStops();
                        break;
                    default:
                        lane = null;
                        break;
                }
                if (lane != null)
                {
                    lane.m_direction = inverted ? NetInfo.Direction.Backward : NetInfo.Direction.Forward;
                    lane.m_position = currentPosition;
                    lane.m_width = modelWidth;
                    lanes[i] = lane;
                }

            }
            info.m_segments = segments;
            info.m_lanes = lanes;


        }
        private static float GetModelWidth(string modelName)
        {
            if (modelName != null && modelName.Contains("_"))
            {
                var numString = modelName.Substring(modelName.LastIndexOf("_") + 1);
                return float.Parse(numString);
            }
            return -1;
        }
        private static NetInfo.LaneType GetModelLaneType(string modelName)
        {
            if (modelName != null && modelName.Contains("_"))
            {
                var modelNamePrefix = modelName.Substring(0, modelName.IndexOf("_"));
                switch (modelNamePrefix)
                {
                    case "Curb":
                    case "Median":
                        return NetInfo.LaneType.Pedestrian;
                    case "Road":
                        return NetInfo.LaneType.Vehicle;
                    default:
                        return NetInfo.LaneType.None;
                }
            }
            return NetInfo.LaneType.None;
        }
    }
}
