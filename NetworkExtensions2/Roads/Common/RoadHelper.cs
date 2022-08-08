using ColossalFramework;
using NetworkExtensions2.Roads.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework;
using UnityEngine;
#if DEBUG
using Debug = Transit.Framework.Debug;
#endif
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
            return info.m_lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
        }

        public static NetInfo.Lane GetRightRoadShoulder(this NetInfo info)
        {
            return info.m_lanes.LastOrDefault(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
        }

        public static NetInfo.Lane GetMedianLane(this NetInfo info)
        {
            return info.m_lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.None && l.m_position == 0);
        }

        public static void RemoveProps(this ICollection<NetLaneProps.Prop> props, params string[] namesOfPropsToRemove)
        {
            foreach (var t in namesOfPropsToRemove)
            {
                var propsToRemove = props.Where(p => p.m_prop != null && p.m_prop.name.ToLower().Contains(t.ToLower())).ToList();
                if (propsToRemove.Count > 0)
                {
                    foreach (var t1 in propsToRemove)
                    {
                        props.Remove(t1);
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
        public static void AddProp(this ICollection<NetLaneProps.Prop> props, NetLaneProps.Prop propToAdd)
        {
            props.Add(propToAdd.ShallowClone());
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
        public static void SetBikeLaneProps(this NetInfo.Lane lane)
        {

            var prop = Prefabs.Find<PropInfo>("BikeLaneText", false);

            if (prop == null)
            {
                Debug.Log("BikeLaneText doesnt exist");
                return;
            }

            if (lane.m_laneProps == null)
            {
                lane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                lane.m_laneProps.m_props = new NetLaneProps.Prop[] { };
            }
            else
            {
                lane.m_laneProps = lane.m_laneProps.Clone();
            }

            var tempProps = lane.m_laneProps.m_props.ToList();
            tempProps.RemoveProps("arrow");
            lane.m_laneProps.m_props = tempProps.ToArray();


            lane.m_laneProps.m_props = lane
                .m_laneProps
                .m_props
                    .Union(new NetLaneProps.Prop
                    {
                        m_prop = prop,
                        m_position = new Vector3(0f, 0f, 7.5f),
                        m_angle = 180f,
                        m_segmentOffset = -1f,
                        m_minLength = 8f,
                        m_startFlagsRequired = NetNode.Flags.Junction
                    })
                .ToArray();
        }
        public static void SetSharedLaneProps(this NetInfo.Lane lane)
        {

            var prop = Prefabs.Find<PropInfo>("SharedLaneText", false);

            if (prop == null)
            {
                Debug.Log("SharedLaneText doesnt exist");
                return;
            }

            if (lane.m_laneProps == null)
            {
                lane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                lane.m_laneProps.m_props = new NetLaneProps.Prop[] { };
            }
            else
            {
                lane.m_laneProps = lane.m_laneProps.Clone();
            }

            var tempProps = lane.m_laneProps.m_props.ToList();
            tempProps.RemoveProps("arrow");
            lane.m_laneProps.m_props = tempProps.ToArray();


            lane.m_laneProps.m_props = lane
                .m_laneProps
                .m_props
                    .Union(new NetLaneProps.Prop
                    {
                        m_prop = prop,
                        m_position = new Vector3(0f, 0f, 7.5f),
                        m_angle = 180f,
                        m_segmentOffset = -1f,
                        m_minLength = 8f,
                        m_startFlagsRequired = NetNode.Flags.Junction
                    })
                .ToArray();
        }
        public static NetInfo.Segment[] CreateSegments(out float totalWidth, params LaneRecipe[] laneRecipes)
        {
            var segments = new NetInfo.Segment[laneRecipes.Length];
            float previousModelWidth = 0;
            totalWidth = 0f;
            for (int i = 0; i < laneRecipes.Length; i++)
            {
                var laneRecipe = laneRecipes[i];
                var modelWidth = GetModelWidth(laneRecipe.ModelName);
                totalWidth += modelWidth;
            }
            var currentPosition = 0.5f * -totalWidth;
            for (int i = 0; i < laneRecipes.Length; i++)
            {
                var laneRecipe = laneRecipes[i];
                var modelWidth = GetModelWidth(laneRecipe.ModelName);
                currentPosition += 0.5f * (previousModelWidth + modelWidth);
                previousModelWidth = modelWidth;
                var inverted = currentPosition > 0;
                var segment = laneRecipe.TemplateSegment.ShallowClone();
                segments[i] = segment.SetFlagsDefault().SetNetResources(laneRecipe.ModelName, laneRecipe.TextureName, currentPosition, inverted);
            }
            return segments;
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
    }
}
