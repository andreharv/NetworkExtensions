using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using Transit.Framework.Texturing;
using TransitAT.Addon.RoadExtensions.Roads.Common;
using UnityEngine;

namespace TransitAT.Addon.RoadExtensions.Roads.SmallHeavyRoads.TramSmallAve
{
    public partial class TramSmallAveBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 10; } }
        public int UIOrder { get { return 12; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L_TRAM; } }
        public string Name { get { return "Tram Small Avenue"; } }
        public string DisplayName { get { return "Small Four-Lane Road With Tram"; } }
        public string Description { get { return "A four-lane road with a tram line. Supports medium traffic."; } }
        public string ShortDescription { get { return "No parking, zoneable, medium traffic"; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_SMALL_HV; } }

        public string ThumbnailsPath { get { return @"Roads\SmallHeavyRoads\TramSmallAve\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\SmallHeavyRoads\TramSmallAve\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var basicTramInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L_TRAM);
            var basicTramTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.GetPrefabName(NetInfos.Vanilla.ROAD_2L_TRAM, NetInfoVersion.Tunnel));
            var smallAveInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.GetPrefabName(NetInfos.Vanilla.ROAD_4L_SMALL, version));
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            var sMesh = new List<Mesh>();
            var sMaterials = new List<Material>();
            var nMesh = new List<Mesh>();
            var sNodes = new List<Material>();
            var segments = new List<NetInfo.Segment>();
            var nodes = new List<NetInfo.Node>();
            segments.AddRange(info.m_segments);
            nodes.AddRange(info.m_nodes);
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var count = Math.Min(segments.Count(), 6);
                        for (var i = 0; i < count; i++)
                        {
                            var index = Math.Min(2, i);
                            segments[i].m_mesh = smallAveInfo.m_segments[index].ShallowClone().m_mesh;
                            segments[i].m_lodMesh = smallAveInfo.m_segments[index].ShallowClone().m_lodMesh;
                            if (i > 0 && i < 3)
                            {
                                segments[i].SetTextures
                                    (new TextureSet
                                        (@"Roads\SmallHeavyRoads\TramSmallAve\Textures\Ground_Segment_Single__MainTex.png",
                                        @"Roads\SmallHeavyRoads\TramSmallAve\Textures\Ground_Segment_Single__AlphaMap.png"));
                            }
                            else if (i == 5)
                            {
                                segments[i].SetTextures
                                    (new TextureSet
                                        (@"Roads\SmallHeavyRoads\TramSmallAve\Textures\Ground_Segment_Double__MainTex.png",
                                        @"Roads\SmallHeavyRoads\TramSmallAve\Textures\Ground_Segment_Double__AlphaMap.png"));
                            }
                            else
                            {
                                segments[i].m_material = smallAveInfo.m_segments[index].ShallowClone().m_material;
                                segments[i].m_lodMaterial = smallAveInfo.m_segments[index].ShallowClone().m_lodMaterial;
                            }
                        }
                        nodes[0].m_mesh = smallAveInfo.m_nodes[0].ShallowClone().m_mesh;
                        nodes[0].m_lodMesh = smallAveInfo.m_nodes[0].ShallowClone().m_lodMesh;
                        nodes[0].m_material = smallAveInfo.m_nodes[0].ShallowClone().m_material;
                        nodes[0].m_lodMaterial = smallAveInfo.m_nodes[0].ShallowClone().m_lodMaterial;
                    }
                    break;
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        segments[0].m_mesh = smallAveInfo.m_segments[0].ShallowClone().m_mesh;
                        segments[0].m_lodMesh = smallAveInfo.m_segments[0].ShallowClone().m_lodMesh;
                        segments[0].m_material = smallAveInfo.m_segments[0].ShallowClone().m_material;
                        segments[0].m_lodMaterial = smallAveInfo.m_segments[0].ShallowClone().m_lodMaterial;

                        nodes[0].m_mesh = smallAveInfo.m_nodes[0].ShallowClone().m_mesh;
                        nodes[0].m_lodMesh = smallAveInfo.m_nodes[0].ShallowClone().m_lodMesh;
                        nodes[0].m_material = smallAveInfo.m_nodes[0].ShallowClone().m_material;
                        nodes[0].m_lodMaterial = smallAveInfo.m_nodes[0].ShallowClone().m_lodMaterial;
                    }
                    break;
                case NetInfoVersion.Slope:
                    {
                        segments[0].m_mesh = smallAveInfo.m_segments[0].ShallowClone().m_mesh;
                        segments[0].m_lodMesh = smallAveInfo.m_segments[0].ShallowClone().m_lodMesh;
                        segments[0].m_material = smallAveInfo.m_segments[0].ShallowClone().m_material;
                        segments[0].m_lodMaterial = smallAveInfo.m_segments[0].ShallowClone().m_lodMaterial;

                        segments[1].m_mesh = smallAveInfo.m_segments[1].ShallowClone().m_mesh;
                        segments[1].m_lodMesh = smallAveInfo.m_segments[1].ShallowClone().m_lodMesh;
                        segments[1].m_material = smallAveInfo.m_segments[1].ShallowClone().m_material;
                        segments[1].m_lodMaterial = smallAveInfo.m_segments[1].ShallowClone().m_lodMaterial;

                        if (segments.Count() > 2)
                        {
                            segments.Insert(2, smallAveInfo.m_segments[2].ShallowClone());
                        }
                        else
                        {
                            segments.Add(smallAveInfo.m_segments[2].ShallowClone());
                        }

                        nodes[0].m_mesh = smallAveInfo.m_nodes[0].ShallowClone().m_mesh;
                        nodes[0].m_lodMesh = smallAveInfo.m_nodes[0].ShallowClone().m_lodMesh;
                        nodes[0].m_material = smallAveInfo.m_nodes[0].ShallowClone().m_material;
                        nodes[0].m_lodMaterial = smallAveInfo.m_nodes[0].ShallowClone().m_lodMaterial;

                        nodes[1].m_mesh = smallAveInfo.m_nodes[1].ShallowClone().m_mesh;
                        nodes[1].m_lodMesh = smallAveInfo.m_nodes[1].ShallowClone().m_lodMesh;
                        nodes[1].m_material = smallAveInfo.m_nodes[1].ShallowClone().m_material;
                        nodes[1].m_lodMaterial = smallAveInfo.m_nodes[1].ShallowClone().m_lodMaterial;
                        if (nodes.Count() > 2)
                        {
                            nodes.Insert(2, smallAveInfo.m_nodes[2].ShallowClone());
                        }
                        else
                        {
                            nodes.Add(smallAveInfo.m_nodes[2].ShallowClone());
                        }
                        //var railNode = basicTramInfo.m_nodes[1].ShallowClone();
                        //railNode.SetFlags(NetNode.Flags.Underground, NetNode.Flags.None);
                        //var wireNode = basicTramInfo.m_nodes[2].ShallowClone();
                        //wireNode.SetFlags(NetNode.Flags.Underground, NetNode.Flags.None);
                        //nodes.Add(railNode);
                        //nodes.Add(wireNode);
                    }
                    break;
                case NetInfoVersion.Tunnel:
                    {
                        segments[0].m_mesh = smallAveInfo.m_segments[0].ShallowClone().m_mesh;
                        segments[0].m_lodMesh = smallAveInfo.m_segments[0].ShallowClone().m_lodMesh;
                        segments[0].m_material = smallAveInfo.m_segments[0].ShallowClone().m_material;
                        segments[0].m_lodMaterial = smallAveInfo.m_segments[0].ShallowClone().m_lodMaterial;

                        segments.Add(smallAveInfo.m_segments[1].ShallowClone());
                        segments.Add(basicTramInfo.m_segments[6]);
                        segments.Add(basicTramInfo.m_segments[7]);

                        nodes[0].m_mesh = smallAveInfo.m_nodes[0].ShallowClone().m_mesh;
                        nodes[0].m_lodMesh = smallAveInfo.m_nodes[0].ShallowClone().m_lodMesh;
                        nodes[0].m_material = smallAveInfo.m_nodes[0].ShallowClone().m_material;
                        nodes[0].m_lodMaterial = smallAveInfo.m_nodes[0].ShallowClone().m_lodMaterial;

                        nodes.Add(smallAveInfo.m_nodes[1].ShallowClone());
                        nodes.Add(basicTramInfo.m_nodes[1].ShallowClone());
                        nodes.Add(basicTramInfo.m_nodes[2].ShallowClone());
                        //if (nodes.Count() > 1)
                        //{
                        //    nodes.Insert(1, smallAveInfo.m_nodes[1]);
                        //}
                        //else
                        //{
                        //    nodes.Add(smallAveInfo.m_nodes[1]);
                        //}

                        //var railNode = basicTramInfo.m_nodes[1].ShallowClone();
                        //railNode.SetFlags(NetNode.Flags.Underground, NetNode.Flags.None);
                        //var wireNode = basicTramInfo.m_nodes[2].ShallowClone();
                        //wireNode.SetFlags(NetNode.Flags.Underground, NetNode.Flags.None);
                        //nodes.Add(railNode);
                        //nodes.Add(wireNode);

                        //var railSegment = basicTramInfo.m_segments[6].ShallowClone();
                        //railSegment.SetFlagsDefault();
                        //var wireSegment = basicTramInfo.m_segments[7].ShallowClone();
                        //wireSegment.SetFlagsDefault();
                        //segments.Add(railSegment);
                        //segments.Add(wireSegment);
                    }
                    break;
            }
            info.m_segments = segments.ToArray();
            info.m_nodes = nodes.ToArray();

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            //SetupTextures(info, version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_pavementWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 2 : 5);
            info.m_halfWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 8 : 11);
            info.m_dlcRequired = SteamHelper.DLC_BitMask.SnowFallDLC;

            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition | CitizenInstance.Flags.Underground;
                info.m_class = basicTramTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL4LTRAM_ROAD_TUNNEL);
            }
            else
            {
                info.m_class = basicTramInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL4LTRAM_ROAD);
            }

            // Setting up lanes
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
                LanesToAdd = 2,
                PedPropOffsetX = 0.5f,
                SpeedLimit = 1.0f
            });
            var lanes = info.m_lanes.ToList();
            for (var i = 0; i < lanes.Count(); i++)
            {
                if ((lanes[i].m_vehicleType & VehicleInfo.VehicleType.Tram) != 0)
                {
                    if (Math.Abs(lanes[i].m_position) == 4.5f)
                    {
                        lanes[i].m_vehicleType = VehicleInfo.VehicleType.Car;
                    }
                    else
                    {
                        lanes[i].m_stopOffset = Math.Sign(lanes[i].m_position) * 3;
                    }
                }
            }
            info.m_lanes = lanes.ToArray();
            var leftPedLane = basicTramInfo.GetLeftRoadShoulder();
            var rightPedLane = basicTramInfo.GetRightRoadShoulder();

            //Setting Up Props
            var leftRoadProps = leftPedLane.m_laneProps.m_props.ToList();
            var rightRoadProps = rightPedLane.m_laneProps.m_props.ToList();

            if (version == NetInfoVersion.Slope)
            {
                leftRoadProps.AddLeftWallLights(info.m_pavementWidth);
                rightRoadProps.AddRightWallLights(info.m_pavementWidth);
            }

            var leftProps = new List<NetLaneProps.Prop>();
            var rightProps = new List<NetLaneProps.Prop>();
            leftProps.AddRange(leftRoadProps.Where(p => p != null));
            rightProps.AddRange(rightRoadProps.Where(p => p != null));
            if (version == NetInfoVersion.Ground)
            {
                for (var i = 0; i < leftProps.Count(); i++)
                {
                    if (leftProps[i].m_prop.name == "Tram Pole Side")
                    {
                        leftProps[i].m_position.x = -0.5f;
                    }
                    else
                    {
                        leftProps[i].m_position.x -= 0.5f;
                    }
                }
                for (var i = 0; i < rightProps.Count(); i++)
                {
                    if (rightProps[i].m_prop.name == "Tram Pole Side")
                    {
                        rightProps[i].m_position.x = 0.5f;
                    }
                    else
                    {
                        rightProps[i].m_position.x += 0.5f;
                    }
                }
            }
            info.GetLeftRoadShoulder().m_laneProps.m_props = leftProps.ToArray();
            info.GetRightRoadShoulder().m_laneProps.m_props = rightProps.ToArray();

            //info.TrimAboveGroundProps(version);
            info.SetupNewSpeedLimitProps(50, 40);

            // AI
            var owPlayerNetAI = basicTramInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 2; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 2; // Charge by the lane?
            }

            // TODO: make it configurable
            var roadBaseAI = info.GetComponent<RoadBaseAI>();
            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = false;
            }
        }
    }
}
