using System.Linq;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.Roads.Avenues.LargeAvenue8LM
{
    public partial class LargeAvenue8LMBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 25; } }
        public int UIOrder { get { return 0; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "Eight-Lane Avenue"; } }
        public string DisplayName { get { return "Eight-Lane Road"; } }
        public string Description { get { return "An eight-lane road with paved median. Supports heavy urban traffic."; } }
        public string ShortDescription { get { return "Zoneable, heavy urban traffic"; } }
        public string UICategory { get { return "RoadsLarge"; } }

        public string ThumbnailsPath { get { return @"Roads\Avenues\LargeAvenue8LM\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Avenues\LargeAvenue8LM\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            //var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_SLOPE);
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            var roadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L_TUNNEL);
            var bridgeInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_4L_BRIDGE).Clone("temp");

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
            info.m_pavementWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 3 : 4);
            info.m_halfWidth = 16;

            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition;
                info.m_class = roadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_MEDIUM_ROAD_TUNNEL);
            }
            else if (version == NetInfoVersion.Bridge)
            {
                info.m_class = bridgeInfo.m_class.Clone(NetInfoClasses.NEXT_MEDIUM_ROAD);
            }
            else
            {
                info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_MEDIUM_ROAD);
            }

            // Setting up lanes
            info.SetRoadLanes(version, new LanesConfiguration()
            {
                IsTwoWay = true,
                LanesToAdd = 2,
                LaneWidth = version == NetInfoVersion.Slope ? 2.75f : 3,
                PedPropOffsetX = 1,
                CenterLane = CenterLaneType.Median,
                CenterLaneWidth = 2
            });
            var leftPedLane = info.GetLeftRoadShoulder(roadInfo, version);
            var rightPedLane = info.GetRightRoadShoulder(roadInfo, version);
            //Setting Up Props
            var leftRoadProps = leftPedLane.m_laneProps.m_props.ToList();
            var rightRoadProps = rightPedLane.m_laneProps.m_props.ToList();

            if (version == NetInfoVersion.Slope)
            {
                leftRoadProps.AddLeftWallLights(info.m_pavementWidth);
                rightRoadProps.AddRightWallLights(info.m_pavementWidth);
            }

            leftPedLane.m_laneProps.m_props = leftRoadProps.ToArray();
            rightPedLane.m_laneProps.m_props = rightRoadProps.ToArray();

            info.TrimAboveGroundProps(version);

            if (version == NetInfoVersion.Bridge)
            {
                var bridgePillarInfo = Prefabs.Find<BuildingInfo>(NetInfos.Vanilla.LARGE_ROAD_BRIDGE_SUSPENSION_PILLAR).Clone("LargeRoadBridgeSuspensionPillarPlus");
                var infoRoadBridgeAI = (RoadBridgeAI)info.GetComponent<NetAI>();
                if (infoRoadBridgeAI != null && bridgePillarInfo != null)
                {
                    var tempBridgePillarInfo = bridgePillarInfo;
                    tempBridgePillarInfo.m_mesh = AssetManager.instance.GetMesh(@"Buildings/Pillars/16m/Meshes/Pillar.obj");
                    infoRoadBridgeAI.m_bridgePillarInfo = tempBridgePillarInfo;
                    //ai.m_bridgePillarInfo.m_mesh = 
                    //ai.m_bridgePillarInfo.m_lodMesh = AssetManager.instance.GetMesh(@"Buildings/Pillars/16m/Meshes/Pillar_LOD.obj");
                }
            }

            //var propLanes = info.m_lanes.Where(l => l.m_laneProps != null && (l.m_laneProps.name.ToLower().Contains("left") || l.m_laneProps.name.ToLower().Contains("right"))).ToList();
            var owPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 4 / 3; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 4 / 3; // Charge by the lane?
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }
    }
}
