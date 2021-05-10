using System.Linq;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Addon.RoadExtensions.Roads.Highways;
using Transit.Addon.RoadExtensions.Roads.Highways.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Highways.HighwayL1R2
{
    public partial class HighwayL1R2Builder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 47; } }
        public int UIOrder { get { return 40; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.HIGHWAY_3L; } }
        public string Name { get { return "AsymHighwayL1R2"; } }
        public string DisplayName { get { return "1+2 Lane Asymmetrical Highway"; } }
        public string Description { get { return "An asymmetrical highway with one left lane and two right lanes.  Note, dragging this highway backwards reverses its orientation."; } }
        public string ShortDescription { get { return "No parking, not zoneable, medium to high traffic"; } }
        public string UICategory { get { return "RoadsHighway"; } }


        public string ThumbnailsPath { get { return @"Roads\Highways\HighwayL1R2\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Highways\HighwayL1R2\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var highwayTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_TUNNEL);


            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup22mMesh(version);


            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;
            //info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAYL1R2);
            info.m_surfaceLevel = 0;
            info.m_createPavement = !(version == NetInfoVersion.Ground || version == NetInfoVersion.Tunnel);
            info.m_createGravel = version == NetInfoVersion.Ground;
            info.m_averageVehicleLaneSpeed = 1.8f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;
            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;
            info.m_halfWidth = 11;
            info.m_pavementWidth = 2;
            info.m_maxBuildAngle = 90;
            info.m_maxBuildAngleCos = 0;
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                info.m_class = highwayTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAYL1R2_TUNNEL);
            }
            else
            {
                info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAYL1R2);
            }


            ///////////////////////////
            // Set up lanes          //
            ///////////////////////////
            info.SetupHighwayLanes();
            var leftHwLane = info.SetHighwayLeftShoulder(highwayInfo, version);
            var rightHwLane = info.SetHighwayRightShoulder(highwayInfo, version);
            var vehicleLanes = info.SetHighwayVehicleLanes(0, true);
            foreach (var lane in vehicleLanes)
            {
                lane.m_speedLimit = 1.8f;
            }


            ///////////////////////////
            // Set up props          //
            ///////////////////////////
            var leftHwLaneProps = leftHwLane.m_laneProps.m_props.ToList();
            var rightHwLaneProps = rightHwLane.m_laneProps.m_props.ToList();

            if (version == NetInfoVersion.Slope)
            {
                leftHwLaneProps.SetHighwaySignsSlope();
                rightHwLaneProps.SetHighwaySignsSlope();
            }

            // Lightning
            rightHwLaneProps.SetHighwayRightLights(version);
            if (version == NetInfoVersion.Slope)
            {
                leftHwLaneProps.AddLeftWallLights(1);
                rightHwLaneProps.AddRightWallLights(-1);
            }

            leftHwLaneProps.RemoveProps("100 Speed Limit"); // Since we dont have the 90km/h limit prop
            rightHwLaneProps.RemoveProps("100 Speed Limit"); // Since we dont have the 90km/h limit prop

            leftHwLane.m_laneProps.m_props = leftHwLaneProps.ToArray();
            rightHwLane.m_laneProps.m_props = rightHwLaneProps.ToArray();

            info.TrimNonHighwayProps(false, false, true);

            ///////////////////////////
            // AI                    //
            ///////////////////////////
            var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (hwPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost * 4 / 3;
                playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost * 4 / 3;
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_highwayRules = true;
                roadBaseAI.m_trafficLights = false;
                roadBaseAI.m_accumulateSnow = false;
            }

            var roadAI = info.GetComponent<RoadAI>();

            if (roadAI != null)
            {
                roadAI.m_enableZoning = false;
            }
        }
    }
}