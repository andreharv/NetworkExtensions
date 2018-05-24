using System.Linq;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Addon.RoadExtensions.Roads.Highways;
using Transit.Addon.RoadExtensions.Roads.Highways.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Highways.Highway2L2W
{
    public partial class Highway2L2WBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 48; } }
        public int UIOrder { get { return 40; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.HIGHWAY_3L; } }
        public string Name { get { return "Highway2L2W"; } }
        public string DisplayName { get { return "Four-Lane National Highway"; } }
        public string Description { get { return "A four-lane national highway accommodating medium traffic. Highway does not allow zoning next to it!"; } }
        public string ShortDescription { get { return "No parking, not zoneable, medium traffic"; } }
        public string UICategory { get { return "RoadsHighway"; } }

        public string ThumbnailsPath { get { return @"Roads\Highways\Highway2L2W\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Highways\Highway2L2W\infotooltip.png"; } }

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
            info.Setup24mMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;
            //info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY2L2W);
            info.m_surfaceLevel = 0;
            info.m_createPavement = !(version == NetInfoVersion.Ground || version == NetInfoVersion.Tunnel);
            info.m_createGravel = version == NetInfoVersion.Ground;
            info.m_averageVehicleLaneSpeed = 1.8f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;
            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;
            info.m_halfWidth = (version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated) ? 11 : 12;
            info.m_pavementWidth = 2f;
            info.m_maxBuildAngle = 90;
            info.m_maxBuildAngleCos = 0;
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                info.m_class = highwayTunnelInfo.m_class.Clone(info.name + version.ToString() + "Class");
            }
            else
            {
                info.m_class = highwayInfo.m_class.Clone(info.name + version.ToString() + "Class");
            }


            ///////////////////////////
            // Set up lanes          //
            ///////////////////////////
            info.SetRoadLanes(version, new LanesConfiguration
            {
                LanePositionOffst = -2,
                IsTwoWay = true,
                LaneWidth = 4,
                LanesToAdd = 1,
                SpeedLimit = 1.8f
            });

            ///////////////////////////
            // Set up props          //
            ///////////////////////////
            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();

            var leftRoadProps = leftPedLane?.m_laneProps.m_props.ToList();
            var rightRoadProps = rightPedLane?.m_laneProps.m_props.ToList();

            if (version == NetInfoVersion.Slope)
            {
                leftRoadProps?.AddLeftWallLights(info.m_pavementWidth);
                rightRoadProps?.AddRightWallLights(info.m_pavementWidth);
            }
            if (leftPedLane != null && leftPedLane.m_laneProps != null)
                leftPedLane.m_laneProps.m_props = leftRoadProps.ToArray();
            if (rightPedLane != null && rightPedLane.m_laneProps != null)
                rightPedLane.m_laneProps.m_props = rightRoadProps.ToArray();
            info.TrimNonHighwayProps(version == NetInfoVersion.Ground);
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