using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Addon.RoadExtensions.Props;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using UnityEngine;
using ColossalFramework.Plugins;
using System;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif
namespace Transit.Addon.RoadExtensions.Roads.Avenues.LargeAvenue8L2BusLanes
{
    public partial class LargeAvenue8L2BusLanesBuilder : Activable, IMultiNetInfoBuilderPart
    {
        public int Order { get { return 28; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "FourDevidedLaneAvenue2Bus"; } }
        public string DisplayName { get { return "Six-Lane Road with Bus Line And Parking"; } }
        public string Description { get { return "A six-lane road with bus lane. Supports heavy urban traffic."; } }
        public string ShortDescription { get { return "Parking, zoneable, heavy urban traffic"; } }


        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground | NetInfoVersion.GroundGrass | NetInfoVersion.GroundTrees; }
        }

        public IEnumerable<IMenuItemBuilder> MenuItemBuilders
        {
            get
            {
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 28,
                    Name = "FourDevidedLaneAvenue2Bus",
                    DisplayName = "Six-Lane Road with Bus Line And Parking",
                    Description = "A six-lane road with bus lane. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue8L2BusLanes\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue8L2BusLanes\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 30,
                    Name = "FourDevidedLaneAvenue2Bus Decoration Trees",
                    DisplayName = "Six-Lane Road with Bus Line And Parking ",
                    Description = "A six-lane road with bus lane. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue8L2BusLanes\thumbnails_trees.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue8L2BusLanes\infotooltip_trees.png"
                };

                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 29,
                    Name = "FourDevidedLaneAvenue2Bus Decoration Grass",
                    DisplayName = "Six-Lane Road with Bus Line And Parking",
                    Description = "A six-lane road with bus lane. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue8L2BusLanes\thumbnails_grass.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue8L2BusLanes\infotooltip_grass.png"
                };
            }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var roadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_4L_TUNNEL);
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup32m3mSW2x3mMdnBusMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition | CitizenInstance.Flags.Underground;
                info.m_class = roadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_XLARGE_ROAD_TUNNEL);
            }
            else
            {
                info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_XLARGE_ROAD);
            }

            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.m_pavementWidth = 2.5f;
                    info.m_halfWidth = 16;
                    break;
                case NetInfoVersion.Slope:
                    info.m_pavementWidth = 4;
                    info.m_halfWidth = 16;
                    break;
                case NetInfoVersion.Tunnel:
                    info.m_pavementWidth = 5;
                    info.m_halfWidth = 17;
                    break;
                default:
                    info.m_pavementWidth = 4.5f;
                    info.m_halfWidth = 16;
                    break;
            }

            info.m_class = roadInfo.m_class.Clone("FourDevidedLaneAvenue4Parking");
            info.m_canCrossLanes = true;
            // Setting up lanes        

            var carLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle).ToList();

            var maxPosition = carLanes.Max(l => l.m_position);
            var outerCarlanes = carLanes.Where(l => Math.Abs(l.m_position) == maxPosition).ToList();
            for (int i = 0; i < carLanes.Count; i++)
            {
                var sign = Math.Sign(carLanes[i].m_position);
carLanes[i].m_speedLimit = 1;

                if (carLanes[i].m_position == sign * maxPosition)
                {
                    carLanes[i].m_position = sign * 10;
                    carLanes[i].m_stopType = VehicleInfo.VehicleType.None;
                    carLanes[i].m_stopOffset = 0;
                }
                else if (carLanes[i].m_position != sign * 1.5f)
                {
                    carLanes[i].m_position = sign * 1.5f;
                    carLanes[i].m_laneType = NetInfo.LaneType.TransportVehicle;
                    carLanes[i].m_stopType = VehicleInfo.VehicleType.Car;
                    carLanes[i].m_stopOffset = 0;
                }
                else
                {
                    carLanes[i].m_position = sign * 7;
                    carLanes[i].m_stopType = VehicleInfo.VehicleType.None;
                    carLanes[i].m_stopOffset = -sign * 0;
                }
            }

            PropInfo avenueLight = Prefabs.Find<PropInfo>(LargeAvenueMedianLightBuilder.NAME, false);
            PropInfo pedLight = Prefabs.Find<PropInfo>("Traffic Light Pedestrian", false);
            var pedLanes = new List<NetInfo.Lane>();
            var laneList = new List<NetInfo.Lane>();
            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();

            laneList.Add(leftPedLane);
            laneList.Add(rightPedLane);
            var hasExtraPavement = version == NetInfoVersion.GroundGrass || version == NetInfoVersion.GroundTrees;
            for (int i = 0; i < laneList.Count; i++)
            {
                var thePedLane = laneList[i];
                var sign = Math.Sign(thePedLane.m_position);
                if (thePedLane?.m_laneProps?.m_props == null)
                {
                    continue;
                }
                var pedLaneProps = thePedLane.m_laneProps.m_props.ToList();
                thePedLane.m_position = sign * 14.75f;
                thePedLane.m_width = 2.5f;

                thePedLane.m_stopType = VehicleInfo.VehicleType.None;
                var propFound = false;
                var intPedLane = thePedLane.ShallowClone();
                intPedLane.m_position = sign * 4.25f;
                intPedLane.m_width = 2.5f;
                intPedLane.m_stopType = VehicleInfo.VehicleType.Car;
                //intPedLane.m_centerPlatform = true;
                intPedLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                var intPedLaneProps = new List<NetLaneProps.Prop>();
                var propCount = pedLaneProps.Count;
                if (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel)
                {
                    for (var j = 0; j < propCount; j++)
                    {
                        var prop = pedLaneProps[j];
                        if (prop?.m_prop != null)
                        {
                            if (prop.m_prop.name == "Traffic Light 01")
                            {
                                intPedLaneProps.Add(prop);
                                prop.m_prop = pedLight;
                                prop.m_finalProp = pedLight;
                                prop.m_position = new Vector3(-sign * 1, prop.m_position.y, prop.m_position.z);
                            }
                            else if (prop.m_prop.name == "New Street Light")
                            {
                                if (propFound == false)
                                {
                                    propFound = true;
                                    var newProp = new NetLaneProps.Prop()
                                    {
                                        m_prop = avenueLight,
                                        m_finalProp = avenueLight,
                                        m_angle = prop.m_angle,
                                        m_position = Vector3.zero,
                                        m_probability = prop.m_probability,
                                        m_repeatDistance = 2 * prop.m_repeatDistance,
                                        m_segmentOffset = i - 0.2f
                                    };
                                    intPedLaneProps.Add(newProp);
                                }
                            }
                            else if (!prop.m_prop.name.ToLower().Contains("random"))
                            {
                                intPedLaneProps.Add(prop);
                                prop.m_position = new Vector3(hasExtraPavement ? -sign * 1 : prop.m_position.x / 3, prop.m_position.y, prop.m_position.z);
                            }
                            else
                            {
                                prop.m_position = new Vector3(hasExtraPavement ? -sign * 2 : -sign * 1, prop.m_position.y, prop.m_position.z);
                            }
                        }
                        else if (prop?.m_tree != null)
                        {
                            var newProp = prop.ShallowClone();
                            newProp.m_position = new Vector3(0, prop.m_position.y, prop.m_position.z);
                            intPedLaneProps.Add(newProp);
                            prop.m_position = new Vector3(-sign * 2, prop.m_position.y, prop.m_position.z);
                        }
                        else
                        {
                            intPedLaneProps.Add(prop);
                        }
                    }

                    intPedLane.m_laneProps.m_props = intPedLaneProps.ToArray();
                }

                pedLanes.Add(intPedLane);
                pedLaneProps.RemoveProps("New Street Light");
                pedLaneProps.RemoveProps("Bus Stop Large");
                if (version == NetInfoVersion.Slope)
                {
                    if (i == 0)
                    {
                        pedLaneProps.AddLeftWallLights(info.m_pavementWidth);
                    }
                    else
                    {
                        pedLaneProps.AddRightWallLights(info.m_pavementWidth);
                    }
                }
                thePedLane.m_laneProps.m_props = pedLaneProps.ToArray();
            }

            pedLanes.Add(leftPedLane);
            pedLanes.Add(rightPedLane);

            var tempLanes = new List<NetInfo.Lane>();
            tempLanes.AddRange(pedLanes);
            tempLanes.AddRange(carLanes);

            if (version == NetInfoVersion.Ground)
            {
                var parking = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Parking).ToList();

                foreach (var lane in parking)
                {
                    lane.m_position = Math.Sign(lane.m_position) * 12.5f;
                }
                tempLanes.AddRange(parking);
            }

            info.m_lanes = tempLanes.ToArray();
            // AI
            var owPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 3; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 3; // Charge by the lane?
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }

    }
}

