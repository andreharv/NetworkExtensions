using System.Linq;
using Transit.Addon.RoadExtensions.Roads.DivHighways.Common;
using Transit.Addon.RoadExtensions.Roads.Highways.Common;
using Transit.Framework;
using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.Roads.DivHighways.DivHighway4L
{
    public partial class DivHighway4LBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 30; } }
        public int UIOrder { get { return 10; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_4L; } }
        public string Name { get { return "Four-Lane Rural Highway"; } }
        public string DisplayName { get { return "Four-Lane National Road"; } }
        public string Description { get { return "A four-lane, two-way road suitable for low traffic between areas. National Road does not allow zoning next to it!"; } }
        public string ShortDescription { get { return "No parking, not zoneable, low traffic"; } }
        public string UICategory { get { return "RoadsHighway"; } }

        public string ThumbnailsPath { get { return @"Roads\Highways\Highway1L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Highways\Highway1L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var highwayTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_TUNNEL);
            var basicRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_4L);


            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup32mGrassMesh(version);
            

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;
            info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY1L);
            info.m_class.m_level = (ItemClass.Level)6;
            info.m_surfaceLevel = 0;
            info.m_createPavement = version != NetInfoVersion.Ground && version != NetInfoVersion.Tunnel;
            info.m_createGravel = version == NetInfoVersion.Ground;
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;
            info.m_halfWidth = 16;
            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;
            info.m_pavementWidth = 2;
            info.m_requireDirectRenderers = true;

            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
                info.m_class = highwayTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY1L_TUNNEL);
            }
            else
            {
                info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY1L);
            }

            ///////////////////////////
            // Set up lanes          //
            ///////////////////////////
            info.SetupHighwayLanes();
            var leftHwLane = info.SetHighwayLeftShoulder(highwayInfo, version);
            var rightHwLane = info.SetHighwayRightShoulder(highwayInfo, version);
            var vehicleLanes = info.SetHighwayVehicleLanes();


            ///////////////////////////
            // Set up props          //
            ///////////////////////////

            // Shoulder lanes ---------------------------------------------------------------------
            var leftHwLaneProps = leftHwLane.m_laneProps.m_props.ToList();
            var rightHwLaneProps = rightHwLane.m_laneProps.m_props.ToList();

            foreach (var prop in leftHwLaneProps.Where(lp => lp.m_flagsForbidden == NetLane.Flags.Inverted))
            {
                prop.m_startFlagsForbidden = NetNode.Flags.None;
                prop.m_startFlagsRequired = NetNode.Flags.None;
                prop.m_endFlagsForbidden = NetNode.Flags.None;
                prop.m_endFlagsRequired = NetNode.Flags.Transition;
                prop.m_angle = 180;
                prop.m_position.z *= -1;
                prop.m_segmentOffset *= -1;
            }


            // Set traffic lights
            leftHwLaneProps.Trim(lp => 
                lp != null &&
                lp.m_prop != null &&
                lp.m_prop.name != null &&
                lp.m_prop.name.Contains("Traffic"));
            rightHwLaneProps.Trim(lp =>
                lp != null &&
                lp.m_prop != null &&
                lp.m_prop.name != null &&
                lp.m_prop.name.Contains("Traffic"));

            leftHwLaneProps.AddRange(basicRoadInfo.GetLeftTrafficLights(version));
            rightHwLaneProps.AddRange(basicRoadInfo.GetRightTrafficLights(version));


            // Lightning
            rightHwLaneProps.SetHighwayRightLights(version);
            if (version == NetInfoVersion.Slope)
            {
                leftHwLaneProps.AddLeftWallLights();
                rightHwLaneProps.AddRightWallLights();
            }

            leftHwLane.m_laneProps.m_props = leftHwLaneProps.ToArray();
            rightHwLane.m_laneProps.m_props = rightHwLaneProps.ToArray();


            info.TrimNonHighwayProps(version == NetInfoVersion.Ground);


            ///////////////////////////
            // AI                    //
            ///////////////////////////
            var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (hwPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost * 2 / 3;
                playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost * 2 / 3;
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_highwayRules = true;
                roadBaseAI.m_trafficLights = false;
            }

            var roadAI = info.GetComponent<RoadAI>();

            if (roadAI != null)
            {
                roadAI.m_enableZoning = false;
            }
        }
    }
}
