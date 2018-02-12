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
namespace Transit.Addon.RoadExtensions.Roads.Avenues.LargeAvenue6LM
{
    public partial class LargeAvenue6LMBuilder : Activable, IMultiNetInfoBuilderPart, INetInfoSpecificBaseBuilder, INetInfoLateBuilder
    {
        public int Order { get { return 25; } }
        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_4L; } }
        public string Name { get { return "Six-Lane Avenue Median"; } }
        public string DisplayName { get { return "Six-Lane Road with Median"; } }
        public string ShortDescription { get { return "No parking, zoneable, heavy urban traffic"; } }
        public NetInfoVersion SupportedVersions { get { return NetInfoVersion.AllWithDecorationAndPavement; } }
        public string GetSpecificBasedPrefabName(NetInfoVersion version)
        {
            if (version == NetInfoVersion.GroundGrass || version == NetInfoVersion.GroundTrees || version == NetInfoVersion.GroundPavement)
            {
                return "Six-Lane Avenue Median";
            }
            return NetInfos.Vanilla.GetPrefabName(BasedPrefabName, version);
        }
        public IEnumerable<IMenuItemBuilder> MenuItemBuilders
        {
            get
            {
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsLarge",
                    UIOrder = 61,
                    Name = "Six-Lane Avenue Median",
                    DisplayName = "Six-Lane Road With Median Road",
                    Description = "A Six-lane road with paved median. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue6LM\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue6LM\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsLarge",
                    UIOrder = 62,
                    Name = "Six-Lane Avenue Median Decoration Grass",
                    DisplayName = "Six-Lane Road With Median Grass",
                    Description = "A Six-lane road with paved median. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue6LM\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue6LM\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsLarge",
                    UIOrder = 63,
                    Name = "Six-Lane Avenue Median Decoration Trees",
                    DisplayName = "Six-Lane Road With Median Trees",
                    Description = "A Six-lane road with paved median. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue6LM\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue6LM\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsLarge",
                    UIOrder = 64,
                    Name = "Six-Lane Avenue Median Decoration Pavement",
                    DisplayName = "Six-Lane Road With Median Pavement",
                    Description = "A Six-lane road with paved median. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue6LM\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue6LM\infotooltip.png"
                };
            }
        }

        public void SetupRoadLanes(NetInfo info, NetInfoVersion version)
        {
                info.SetRoadLanes(version, new LanesConfiguration
                {
                    IsTwoWay = true,
                    LaneWidth = 3,
                    LanesToAdd = version.IsGroundDecorated() ? 0 : 2,
                    PedPropOffsetX = 1.5f,
                    CenterLane = CenterLaneType.Median,
                    CenterLaneWidth = 4,
                    BusStopOffset = 3
                });
            info.DoBuildupMulti(version);
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
            info.Setup32m3mSW4mMdnMesh(version);
            
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = version == NetInfoVersion.Ground;
            if (version == NetInfoVersion.Ground)
            {
                info.m_pavementWidth = 3;
            }
            else if (version == NetInfoVersion.Tunnel)
            {
                info.m_pavementWidth = 6;
            }
            else
            {
                info.m_pavementWidth = 5;
            }
            info.m_halfWidth = (version == NetInfoVersion.Tunnel ? 17 : 16);
            info.SetupConnectGroup("3mSW4mMdn", ConnextGroup.ThreePlusThree, ConnextGroup.TwoPlusTwo, ConnextGroup.ThreeMidL, ConnextGroup.FourPlusFour);
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

            SetupRoadLanes(info, version);
            info.SetupLaneProps(version);
            var medianLane = info.GetMedianLane();

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
                foreach (var building in BuildingCollection.FindObjectsOfType<BuildingInfo>())
                {
                    if (building.name.ToLower().Contains("pillar"))
                    {
                        Debug.Log($"PILLARNAME = {building.name}");
                    }
                }
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

