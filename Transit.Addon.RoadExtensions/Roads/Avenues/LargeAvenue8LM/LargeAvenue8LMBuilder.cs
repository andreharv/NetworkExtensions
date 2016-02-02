using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads.Avenues.LargeAvenue8LM
{
    public partial class LargeAvenue8LMBuilder : Activable, INetInfoBuilderPart, INetInfoLateBuilder
    {
        public int Order { get { return 25; } }
        public int UIOrder { get { return 150; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "Eight-Lane Avenue"; } }
        public string DisplayName { get { return "Eight-Lane Road"; } }
        public string Description { get { return "An eight-lane road with paved median. Supports heavy urban traffic."; } }
        public string ShortDescription { get { return "No parking, zoneable, heavy urban traffic"; } }
        public string UICategory { get { return "RoadsLarge"; } }

        public string ThumbnailsPath { get { return @"Roads\Avenues\LargeAvenue8LM\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Avenues\LargeAvenue8LM\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        private const string MEDIAN_LANE_NAME = "Median";

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            var roadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L_TUNNEL);

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup32m3mSW2mMdnMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_pavementWidth = (version == NetInfoVersion.Slope || version == NetInfoVersion.Tunnel ? 4 : 3);
            info.m_halfWidth = (version == NetInfoVersion.Tunnel ? 17 : 16);

            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition;
                info.m_class = roadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_XLARGE_ROAD_TUNNEL);
            }
            else
            {
                info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_XLARGE_ROAD);
            }

            // Setting up lanes
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
                LanesToAdd = 2,
                LaneWidth = version == NetInfoVersion.Slope ? 2.75f : 3,
                PedPropOffsetX = version == NetInfoVersion.Slope ? 1.5f : 1f,
                CenterLane = CenterLaneType.Median,
                CenterLaneWidth = 2,
                BusStopOffset = 0f
            });

            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();

            // Adding median lane
            var medianLane = new NetInfo.Lane
            {
                m_position = 0,
                m_width = 2,
                m_vehicleType = VehicleInfo.VehicleType.None,
                m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>(),
            };

            medianLane.m_laneProps.name = MEDIAN_LANE_NAME;
            medianLane.m_laneProps.m_props = new NetLaneProps.Prop[0];

            info.m_lanes = info.m_lanes.Union(medianLane).ToArray();

            //Setting Up Props
            var leftRoadProps = leftPedLane.m_laneProps.m_props.ToList();
            var rightRoadProps = rightPedLane.m_laneProps.m_props.ToList();

            var replaceStreetLight = false;
            var removeStreetLight = false;

            if (ModernLightingPack.IsPluginActive)
            {
                if (version == NetInfoVersion.Ground ||
                    version == NetInfoVersion.Bridge ||
                    version == NetInfoVersion.Elevated)
                {
                    replaceStreetLight = true;
                }
            }

            if (replaceStreetLight ||
                version == NetInfoVersion.Bridge ||
                version == NetInfoVersion.Slope)
            {
                removeStreetLight = true;
            }

            if (removeStreetLight)
            {
                var propsToRemove = new[] { "street light" };
                leftRoadProps.RemoveProps(propsToRemove);
                rightRoadProps.RemoveProps(propsToRemove);
            }

            if (version == NetInfoVersion.Slope)
            {
                leftRoadProps.AddLeftWallLights(info.m_pavementWidth);
                rightRoadProps.AddRightWallLights(info.m_pavementWidth);
            }

            leftPedLane.m_laneProps.m_props = leftRoadProps.ToArray();
            rightPedLane.m_laneProps.m_props = rightRoadProps.ToArray();

            info.TrimAboveGroundProps(version);

            // AI
            var owPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost*4/3; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost*4/3; // Charge by the lane?
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }

        public void LateBuildUp(NetInfo info, NetInfoVersion version)
        {
            // Installing BridgePillar
            if (version == NetInfoVersion.Bridge)
            {
                //var bridgePillar = PrefabCollection<BuildingInfo>.FindLoaded(WorkshopId + ".CableStay32m_Data");
                //if (bridgePillar == null)
                var bridgePillar = PrefabCollection<BuildingInfo>.FindLoaded("BridgePillar.CableStay32m_Data");
                if (bridgePillar != null)
                {
                    var bridgeAI = info.GetComponent<RoadBridgeAI>();
                    if (bridgeAI != null)
                    {
                        bridgeAI.m_middlePillarInfo = bridgePillar;
                        bridgeAI.m_middlePillarOffset = 58;
                    }
                }
            }

            // Installing ModernLightingPack.StainlessAvenueLight
            if (ModernLightingPack.IsPluginActive)
            {
                if (version == NetInfoVersion.Ground ||
                    version == NetInfoVersion.Bridge ||
                    version == NetInfoVersion.Elevated)
                {
                    var aveLightInfo = PrefabCollection<PropInfo>.FindLoaded(ModernLightingPack.MOD_ID + ".StainlessAvenueLight_Data");

                    if (aveLightInfo != null)
                    {
                        var medianLane = info.m_lanes.First(l => l.m_laneProps != null && l.m_laneProps.name == MEDIAN_LANE_NAME);

                        var newProp = new NetLaneProps.Prop
                        {
                            m_prop = aveLightInfo,
                            m_finalProp = aveLightInfo,
                            m_position = new Vector3(0, 0, 0),
                            m_probability = 100,
                            m_repeatDistance = 40
                        };

                        medianLane.m_laneProps.m_props = medianLane
                            .m_laneProps
                            .m_props
                            .Union(newProp)
                            .ToArray();
                    }
                }
            }
        }
    }
}
