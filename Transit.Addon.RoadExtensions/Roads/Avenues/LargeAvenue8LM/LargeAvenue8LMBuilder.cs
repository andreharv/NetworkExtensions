using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Addon.RoadExtensions.Props;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using UnityEngine;
using static Transit.Framework.NetInfoExtensions;
#if DEBUG
using Debug = Transit.Framework.Debug;
#endif
namespace Transit.Addon.RoadExtensions.Roads.Avenues.LargeAvenue8LM
{
    public partial class LargeAvenue8LMBuilder : Activable, INetInfoBuilderPart, INetInfoLateBuilder
    {
        public int Order { get { return 26; } }
        public int UIOrder { get { return 1500; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_4L; } }
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

        public void SetupRoadLanes(NetInfo info, NetInfoVersion version)
		{

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
            info.Setup32m3mSW2mMdnMesh(version);
            
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);
//info.SetupConnectGroup("3mSW2mMdn", ConnextGroup.FourPlusFour);
            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_pavementWidth = (version == NetInfoVersion.Slope || version == NetInfoVersion.Tunnel ? 4 : 3);
            info.m_halfWidth = (version == NetInfoVersion.Tunnel ? 17 : 16);
            info.SetupConnectGroup("3mSW2mMdn", ConnextGroup.FourPlusFour, ConnextGroup.ThreeMidL, ConnextGroup.TwoPlusTwo);
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

            // Setting up lanes
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
                LanesToAdd = 4,
                LaneWidth = version == NetInfoVersion.Slope ? 2.75f : 3,
                PedPropOffsetX = version == NetInfoVersion.Slope ? 1.5f : 1f,
                CenterLane = CenterLaneType.Median,
                CenterLaneWidth = 2,
                BusStopOffset = 0f
            });
            //SetupRoadLanes(info, version);

            var medianLane = info.GetMedianLane();
            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();

            // Fix for T++ legacy support (reordering)
            if (medianLane != null)
            {
                info.m_lanes = info
                    .m_lanes
                    .Except(medianLane)
                    .Union(medianLane)
                    .ToArray();
            }

            //Setting Up Props
            var leftPedLaneProps = leftPedLane.m_laneProps.m_props.ToList();
            var rightPedLaneProps = rightPedLane.m_laneProps.m_props.ToList();
            var medianPedLaneProps = medianLane?.m_laneProps?.m_props.ToList();

            if (version != NetInfoVersion.Tunnel)
            {
                var medianStreetLight = medianPedLaneProps?.FirstOrDefault(p => p.m_prop.name.ToLower().Contains("avenue light"));
                if (medianStreetLight != null)
                {
                    medianStreetLight.m_finalProp = 
                    medianStreetLight.m_prop = Prefabs.Find<PropInfo>(LargeAvenueMedianLightBuilder.NAME);
                }
            }

            if (medianPedLaneProps != null)
            {
                medianPedLaneProps.RemoveProps("50 Speed Limit");
            }

            if (version == NetInfoVersion.Slope)
            {
                leftPedLaneProps.AddLeftWallLights(info.m_pavementWidth);
                rightPedLaneProps.AddRightWallLights(info.m_pavementWidth);
            }

            leftPedLane.m_laneProps.m_props = leftPedLaneProps.ToArray();
            rightPedLane.m_laneProps.m_props = rightPedLaneProps.ToArray();
            if (medianLane?.m_laneProps != null && medianPedLaneProps != null)
            {
                medianLane.m_laneProps.m_props = medianPedLaneProps.ToArray();
            }

            
            info.TrimAboveGroundProps(version);

            // AI
            var owPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 2; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 2; // Charge by the lane?
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }

        public void LateBuildUp(NetInfo info, NetInfoVersion version)
        {
            if (version == NetInfoVersion.Bridge)
            {
                var bridgePillar = Prefabs.Find<BuildingInfo>($"{Tools.PackageName("BridgePillar")}.CableStay32m_Data");
                if (bridgePillar == null)
                {
                    Debug.Log($"{info.name}: CableStay32m Pillar not found!");
                }
                else
                { 
                    var bridgeAI = info.GetComponent<RoadBridgeAI>();
                    if (bridgeAI != null)
                    {
                        bridgeAI.m_doubleLength = true;
                        bridgeAI.m_bridgePillarInfo = null;
                        bridgeAI.m_middlePillarInfo = bridgePillar;
                        bridgeAI.m_middlePillarOffset = 58;
                    }
                }
            }
        }
    }
}

