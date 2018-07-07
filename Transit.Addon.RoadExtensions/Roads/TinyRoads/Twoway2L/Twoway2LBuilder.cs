using System;
using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.TinyRoads.Twoway2L
{
    public class Twoway2LBuilder : Activable, INetInfoBuilderPart
    {
        public const string NAME = "Two-Lane Twoway";

        public int Order { get { return 0; } }
        public int UIOrder { get { return 6; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L; } }
        public string Name { get { return NAME; } }
        public string DisplayName { get { return NAME; } }
        public string CodeName { get { return "Road_2L"; } }
        public string Description { get { return "A two-lane road suitable for neighborhood traffic."; } }
        public string ShortDescription { get { return "No parking, zoneable, neighborhood traffic"; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_TINY; } }

        public string ThumbnailsPath { get { return @"Roads\TinyRoads\Twoway2L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\TinyRoads\Twoway2L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L);

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup8m1mSWMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    info.SetAllSegmentsTexture(
                        new TextureSet
                           (@"Roads\TinyRoads\Twoway2L\Textures\Ground_Segment__MainTex.png",
                            @"Roads\TinyRoads\Twoway2L\Textures\Ground_Segment__AlphaMap.png"),
                        new LODTextureSet(
                            @"Roads\TinyRoads\Twoway2L\Textures\Ground_Segment__MainTex_LOD.png",
                            @"Roads\TinyRoads\Twoway2L\Textures\Ground_Segment__AlphaMap_LOD.png",
                            @"Roads\TinyRoads\Twoway2L\Textures\Ground_Segment__XYSMap_LOD.png"));
                    break;
            }
            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_halfWidth = 4;
            info.m_pavementWidth = 1f;
            info.m_surfaceLevel = 0;
            info.m_class = roadInfo.m_class.Clone("NExt2LTwoway");
            info.m_class.m_level = (ItemClass.Level)5; //New level
            info.m_lanes = info.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.Parking)
                .ToArray();

            var pedLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
            var roadLanes = info.m_lanes.Where(l => l.m_laneType != NetInfo.LaneType.Pedestrian && l.m_laneType != NetInfo.LaneType.None);

            foreach (var pedLane in pedLanes)
            {
                pedLane.m_verticalOffset = 0.25f;
            }

            foreach (var roadLane in roadLanes)
            {
                roadLane.m_verticalOffset = 0.1f;
            }

            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
                SpeedLimit = 0.8f,
                BusStopOffset = 0f,
                PedLaneOffset = -0.25f,
                PedPropOffsetX = 2.75f
            });
            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();
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


            //info.SetupNewSpeedLimitProps(30, 40);
            
            var originPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (playerNetAI != null && originPlayerNetAI != null)
            {
                playerNetAI.m_constructionCost = originPlayerNetAI.m_constructionCost * 1 / 2;
                playerNetAI.m_maintenanceCost = originPlayerNetAI.m_maintenanceCost * 1 / 2;
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();
            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = false;
            }
        }
    }
}
