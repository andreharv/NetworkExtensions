using System.Linq;
using Transit.Framework;
using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.Roads.Highway6L
{
    public partial class Highway6LBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 61; } }
        public int UIOrder { get { return 60; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ONEWAY_6L; } }
        public string Name { get { return "Large Highway"; } }
        public string DisplayName { get { return "Six-Lane Highway"; } }
        public string Description { get { return "A six-lane, one-way road suitable for very high and dense traffic between metropolitan areas. Lanes going the opposite direction need to be built separately. Highway does not allow zoning next to it!"; } }
        public string UICategory { get { return "RoadsHighway"; } }

        public string ThumbnailsPath { get { return @"Roads\Highway6L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Highway6L\infotooltip.png"; } }

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


            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup32mMesh(version);


            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);


            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;
            info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY6L);
            info.m_surfaceLevel = 0;
            info.m_createPavement = !(version == NetInfoVersion.Ground || version == NetInfoVersion.Tunnel);
            info.m_createGravel = version == NetInfoVersion.Ground;
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;
            info.m_halfWidth = (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge ? 14 : 16);
            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;
            info.m_pavementWidth = 2;
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
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
            var leftHwLaneProps = leftHwLane.m_laneProps.m_props.ToList();
            var rightHwLaneProps = rightHwLane.m_laneProps.m_props.ToList();

            // Lightning
            HighwayHelper.SetHighwayLights(leftHwLaneProps, rightHwLaneProps, version);
            if (version == NetInfoVersion.Slope)
            {
                leftHwLaneProps.AddLeftWallLights();
                rightHwLaneProps.AddRightWallLights();
            }

            leftHwLane.m_laneProps.m_props = leftHwLaneProps.ToArray();
            rightHwLane.m_laneProps.m_props = rightHwLaneProps.ToArray();

            info.TrimNonHighwayProps(false, false);


            ///////////////////////////
            // AI                    //
            ///////////////////////////
            var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (hwPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost * 6 / 3;
                playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost * 6 / 3;
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
