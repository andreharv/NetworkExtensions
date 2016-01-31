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

        public static void CenterProps(this ICollection<NetLaneProps.Prop> props, string[] propsToCenter, float originalLanePos)
        {
            for (var i = 0; i < propsToCenter.Length; i++)
            {
                if (props.Any(p => p.m_prop.name.ToLower().Contains(propsToCenter[i].ToLower())))
                {
                    var propToCenter = props.First(p => p.m_prop.name.ToLower().Contains(propsToCenter[i].ToLower()));
                    propToCenter.m_position.x = -1 * originalLanePos;
                    propToCenter.m_position.z = originalLanePos > 0 ? 0.26f : 0;
                    //var propToRemove = props.First(p => p.m_prop.name.ToLower().Contains(propsToCenter[i].ToLower()));
                    //var tempProp = propToRemove.ShallowClone();
                    //props.Remove(propToRemove);

                    //props.Add(tempProp);
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
