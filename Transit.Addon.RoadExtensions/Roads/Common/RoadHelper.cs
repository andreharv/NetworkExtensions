using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads.Common
{
    public static partial class RoadHelper
    {
        public static void SetupNewSpeedLimitProps(this NetInfo info, int newSpeedLimit, int oldSpeedLimit)
        {
            var newSpeedLimitPI = Prefabs.Find<PropInfo>(newSpeedLimit + " Speed Limit", false);
            var oldSpeedLimitPI = Prefabs.Find<PropInfo>(oldSpeedLimit + " Speed Limit", false);

            if (newSpeedLimitPI == null || oldSpeedLimitPI == null)
            {
                return;
            }

            info.ReplaceProps(newSpeedLimitPI, oldSpeedLimitPI);
        }

        public static void ReplaceProps(this NetInfo info, PropInfo newPropInfo, PropInfo oldPropInfo)
        {
            if (newPropInfo == null || oldPropInfo == null)
            {
                return;
            }

            foreach (var lane in info.m_lanes.Where(l => l.m_laneProps != null))
            {
                if (lane.m_laneProps.m_props == null ||
                    lane.m_laneProps.m_props.Length == 0)
                {
                    continue;
                }

                var oldProp = lane
                    .m_laneProps
                    .m_props
                    .FirstOrDefault(prop => prop.m_prop == oldPropInfo);

                if (oldProp != null)
                {
                    var newSpeedLimitProp = oldProp.ShallowClone();
                    newSpeedLimitProp.m_prop = newPropInfo;
                    newSpeedLimitProp.m_finalProp = null;

                    var newPropsContent = new List<NetLaneProps.Prop>();
                    newPropsContent.AddRange(lane.m_laneProps.m_props.Where(prop => prop.m_prop != oldPropInfo));
                    newPropsContent.Add(newSpeedLimitProp);

                    var newProps = ScriptableObject.CreateInstance<NetLaneProps>();
                    newProps.name = lane.m_laneProps.name;
                    newProps.m_props = newPropsContent.ToArray();
                    lane.m_laneProps = newProps;
                }
            }
        }

        public static NetInfo.Lane GetLeftRoadShoulder(this NetInfo info)
        {
            return info.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
        }

        public static NetInfo.Lane GetRightRoadShoulder(this NetInfo info)
        {
            return info.m_lanes.Last(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
        }

        public static NetInfo.Lane GetMedianLane(this NetInfo info)
        {
            return info.m_lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.None && l.m_position == 0);
        }

        public static void RemoveProps(this ICollection<NetLaneProps.Prop> props, string[] namesOfPropsToRemove)
        {
            for (var i = 0; i < namesOfPropsToRemove.Length; i++)
            {
                var propsToRemove = props.Where(p => p.m_prop != null && p.m_prop.name.ToLower().Contains(namesOfPropsToRemove[i].ToLower())).ToList();
                if (propsToRemove.Count > 0)
                {
                    for (var j = 0; j < propsToRemove.Count; j++)
                    {
                        props.Remove(propsToRemove[j]);
                    }
                }
            }
        }
        public static void AddProps(this ICollection<NetLaneProps.Prop> props, ICollection<NetLaneProps.Prop> propsToAdd)
        {
            foreach (var propToAdd in propsToAdd)
            {
                props.Add(propToAdd.ShallowClone());
            }
        }
        /// <summary>
        /// Replaces a prop whose name is contained in the key with the propinfo in the value.
        /// </summary>
        /// <param name="props"></param>
        /// <param name="replacementPairs">key=prop name part to remove, value = propinfo to add</param>
        public static void ReplacePropInfo(this ICollection<NetLaneProps.Prop> props, KeyValuePair<string, PropInfo> replacementPair)
        {
            if (props.Any(p => p.m_prop.name.ToLower().Contains(replacementPair.Key.ToLower())))
            {
                var tempProp = new NetLaneProps.Prop();
                var propsToReplace = props.Where(p => p.m_prop.name.ToLower().Contains(replacementPair.Key.ToLower())).ToList();
                for (var i = 0; i < propsToReplace.Count; i++)
                {
                    tempProp = propsToReplace[i].ShallowClone();
                    props.Remove(propsToReplace[i]);
                    tempProp.m_prop = replacementPair.Value;
                    props.Add(tempProp);
                }
            }
        }

        public static void AddLeftWallLights(this ICollection<NetLaneProps.Prop> props, float pavementWidth)
        {
            var wallLightPropInfo = Prefabs.Find<PropInfo>("Wall Light Orange");
            var wallLightProp = new NetLaneProps.Prop();
            var wallPropXPos = (pavementWidth - 3) * -0.5f;
            wallLightProp.m_prop = wallLightPropInfo.ShallowClone();
            wallLightProp.m_probability = 100;
            wallLightProp.m_repeatDistance = 20;
            wallLightProp.m_segmentOffset = 0;
            wallLightProp.m_angle = 270;
            wallLightProp.m_position = new Vector3(wallPropXPos, 1.5f, 0);
            props.Add(wallLightProp);
        }

        public static void AddRightWallLights(this ICollection<NetLaneProps.Prop> props, float pavementWidth)
        {
            var wallLightPropInfo = Prefabs.Find<PropInfo>("Wall Light Orange");
            var wallLightProp = new NetLaneProps.Prop();
            var wallPropXPos = (pavementWidth - 3) * 0.5f;
            wallLightProp.m_prop = wallLightPropInfo.ShallowClone();
            wallLightProp.m_probability = 100;
            wallLightProp.m_repeatDistance = 20;
            wallLightProp.m_segmentOffset = 0;
            wallLightProp.m_angle = 90;
            wallLightProp.m_position = new Vector3(wallPropXPos, 1.5f, 0);
            props.Add(wallLightProp);
        }

        public static void TrimAboveGroundProps(this NetInfo info, NetInfoVersion version, bool removeRightStreetLights = false, bool removeLeftStreetLights = false)
        {
            foreach (var laneProps in info.m_lanes.Select(l => l.m_laneProps).Where(lpi => lpi != null))
            {
                var remainingProp = new List<NetLaneProps.Prop>();

                foreach (var prop in laneProps.m_props.Where(p => p.m_prop != null))
                {
                    if ((version == NetInfoVersion.Tunnel || version == NetInfoVersion.Slope)
                     && (prop.m_prop.name.ToLower().Contains("random")
                        || prop.m_prop.name.ToLower().Contains("manhole")
                        || prop.m_prop.name.ToLower().Contains("street name sign")))
                    {
                        continue;
                    }
                    if ((version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
                        && (prop.m_prop.name.ToLower().Contains("random")
                            || prop.m_prop.name.ToLower().Contains("manhole")))
                    {
                        continue;
                    }
                    if (version == NetInfoVersion.Slope && prop.m_prop.name.ToLower().Contains("street light"))
                    {
                        continue;
                    }

                    remainingProp.Add(prop);
                }

                laneProps.m_props = remainingProp.ToArray();
            }
        }

        public static void TrimArrowsProps(this NetInfo info)
        {
            foreach (var laneProps in info.m_lanes.Select(l => l.m_laneProps).Where(lpi => lpi != null))
            {
                var remainingProp = new List<NetLaneProps.Prop>();

                foreach (var prop in laneProps.m_props.Where(p => p.m_prop != null))
                {
                    if (prop.m_prop.name.ToLower().Contains("arrow"))
                    {
                        continue;
                    }

                    remainingProp.Add(prop);
                }

                laneProps.m_props = remainingProp.ToArray();
            }
        }
    }
}
