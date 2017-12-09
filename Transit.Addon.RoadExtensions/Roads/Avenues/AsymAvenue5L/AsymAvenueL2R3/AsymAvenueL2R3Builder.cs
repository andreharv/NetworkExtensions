using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Roads.Avenues.AsymAvenue5L;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;
using Transit.Framework.Network;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads.SmallRoads.BasicRoadMdn
{
    public partial class AsymAvenueL2R3Builder : Activable, IMultiNetInfoBuilderPart, INetInfoSpecificBaseBuilder
    {
        public int Order { get { return 10; } }
        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "AsymAvenueL2R3"; } }
        public string DisplayName { get { return "Five-Lane Asymmetrical Road: (2+3)"; } }
        public string ShortDescription { get { return "Zoneable, medium to high traffic"; } }
        public NetInfoVersion SupportedVersions { get { return NetInfoVersion.AllWithDecorationAndPavement; } }
        public string GetSpecificBasedPrefabName(NetInfoVersion version)
        {
            if (version == NetInfoVersion.GroundGrass || version == NetInfoVersion.GroundTrees || version == NetInfoVersion.GroundPavement)
            {
                return "AsymAvenueL2R3";
            }
            return NetInfos.Vanilla.GetPrefabName(BasedPrefabName, version);
        }
        public IEnumerable<IMenuItemBuilder> MenuItemBuilders
        {
            get
            {
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 60,
                    Name = "AsymAvenueL2R3",
                    DisplayName = "Five-Lane Asymmetrical Road: (2+3)",
                    Description = "An asymmetrical road with two left lane and three right lanes.  Note, dragging this road backwards reverses its orientation.",
                    ThumbnailsPath = @"Roads\Avenues\AsymAvenue5L\AsymAvenueL2R3\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\AsymAvenue5L\AsymAvenueL2R3\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 61,
                    Name = "AsymAvenueL2R3Grass",
                    DisplayName = "Five-Lane Asymmetrical Road Grass: (2+3)",
                    Description = "An asymmetrical road with two left lane and three right lanes.  Note, dragging this road backwards reverses its orientation.",
                    ThumbnailsPath = @"Roads\Avenues\AsymAvenue5L\AsymAvenueL2R3\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\AsymAvenue5L\AsymAvenueL2R3\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 62,
                    Name = "AsymAvenueL2R3Trees",
                    DisplayName = "Five-Lane Asymmetrical Road Trees: (2+3)",
                    Description = "An asymmetrical road with two left lane and three right lanes.  Note, dragging this road backwards reverses its orientation.",
                    ThumbnailsPath = @"Roads\Avenues\AsymAvenue5L\AsymAvenueL2R3\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\AsymAvenue5L\AsymAvenueL2R3\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 62,
                    Name = "AsymAvenueL2R3Pavement",
                    DisplayName = "Five-Lane Asymmetrical Road Pavement: (2+3)",
                    Description = "An asymmetrical road with two left lane and three right lanes.  Note, dragging this road backwards reverses its orientation.",
                    ThumbnailsPath = @"Roads\Avenues\AsymAvenue5L\AsymAvenueL2R3\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\AsymAvenue5L\AsymAvenueL2R3\infotooltip.png"
                };
            }
        }
        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var owRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            var owRoadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L_TUNNEL);
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup32m5mSW3mMdn(version, LanesLayoutStyle.AsymL2R3);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            info.SetupTextures(version, LanesLayoutStyle.AsymL2R3);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = (version == NetInfoVersion.Ground);
            switch (version)
            {
                case NetInfoVersion.Ground:
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    info.m_pavementWidth = 5;
                    break;
                default:
                    info.m_pavementWidth = 7;
                    break;
            }

                info.m_halfWidth = (version != NetInfoVersion.Elevated && version != NetInfoVersion.Bridge ? 16 : 14);
                info.m_canCrossLanes = false;
                if (version == NetInfoVersion.Tunnel)
                {
                    info.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                    info.m_setCitizenFlags = CitizenInstance.Flags.Transition | CitizenInstance.Flags.Underground;
                    info.m_class = owRoadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL4L_ROAD_TUNNEL);

                }
                else
                {
                    info.m_class = owRoadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL4L_ROAD);
                }
            var isGroundPaved = version == NetInfoVersion.GroundGrass || version == NetInfoVersion.GroundTrees || version == NetInfoVersion.GroundPavement;
                // Setting up lanes
                info.SetRoadLanes(version, new LanesConfiguration
                {
                    IsTwoWay = true,
                    LanesToAdd = isGroundPaved ? 0 : -1,
                    PedPropOffsetX = 0.5f,
                    BusStopOffset = 3,
                    CenterLane = CenterLaneType.Median,
                    LayoutStyle = LanesLayoutStyle.AsymL2R3
                });

            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();
            var leftRoadProps = leftPedLane.m_laneProps.m_props.ToList();
            var rightRoadProps = rightPedLane.m_laneProps.m_props.ToList();
            if (isGroundPaved)
            {
                for (var i = 0; i < rightRoadProps.Count; i++)
                {
                    rightRoadProps[i].m_position.x -= 2;
                }
                for (var i = 0; i < leftRoadProps.Count; i++)
                {
                    leftRoadProps[i].m_position.x += 2;
                }
            }
            if (version == NetInfoVersion.GroundTrees)
            {
                var leftTreeProp = new NetLaneProps.Prop()
                {
                    m_tree = Prefabs.Find<TreeInfo>("Tree2variant"),
                    m_repeatDistance = 20,
                    m_probability = 100,
                };
                leftTreeProp.m_position.x = 3.5f;
                leftRoadProps.Add(leftTreeProp);

                var rightTreeProp = new NetLaneProps.Prop()
                {
                    m_tree = Prefabs.Find<TreeInfo>("Tree2variant"),
                    m_repeatDistance = 20,
                    m_probability = 100,
                };
                rightTreeProp.m_position.x = -3.5f;
                rightRoadProps.Add(rightTreeProp);
            }
            else if (version == NetInfoVersion.Slope)
            {
                leftRoadProps.AddLeftWallLights(info.m_pavementWidth);
                rightRoadProps.AddRightWallLights(info.m_pavementWidth);
            }
            leftPedLane.m_laneProps.m_props = leftRoadProps.ToArray();
            rightPedLane.m_laneProps.m_props = rightRoadProps.ToArray();

            // AI
            var owPlayerNetAI = owRoadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 5 / 6; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 5 / 6; // Charge by the lane?
            }

            // TODO: make it configurable
            var roadBaseAI = info.GetComponent<RoadBaseAI>();
            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }
    }
}
