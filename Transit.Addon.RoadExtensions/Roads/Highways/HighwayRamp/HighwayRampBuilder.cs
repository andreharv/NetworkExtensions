using System.Linq;
using Transit.Addon.RoadExtensions.Highways;
using Transit.Addon.RoadExtensions.Roads.Highways.Common;
using Transit.Framework;
using Transit.Framework.Builders;

namespace Transit.Addon.RoadExtensions.Roads.Highways.HighwayRamp
{
    public partial class HighwayRampBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 37; } }
        public int UIOrder { get { return 18; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.HIGHWAY_RAMP; } }
        public string Name { get { return "Highway On-Off Ramp"; } }
        public string DisplayName { get { return "Highway On/Off Ramp"; } }
        public string Description { get { return "Connects highways to the city streets and other highways with ramps."; } }
        public string ShortDescription { get { return "A highway on/off ramp (one lane)"; } }
        public string UICategory { get { return "RoadsHighway"; } }

        public string ThumbnailsPath { get { return @"Roads\Highways\HighwayRamp\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Highways\HighwayRamp\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground | NetInfoVersion.Elevated | NetInfoVersion.Slope | NetInfoVersion.Tunnel; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_RAMP);
            var highwayTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_TUNNEL);
            //var basicRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L);


            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup10mMesh(version);


            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);


            ///////////////////////////
            // Set up                //
            ///////////////////////////

            //info.m_availableIn = ItemClass.Availability.All;
            //info.m_surfaceLevel = 0;
            //info.m_createPavement = version != NetInfoVersion.Ground && version != NetInfoVersion.Tunnel;
            //info.m_createGravel = version == NetInfoVersion.Ground;
            //info.m_averageVehicleLaneSpeed = 2f;
            //info.m_hasParkingSpaces = false;
            //info.m_hasPedestrianLanes = false;
            //info.m_halfWidth = 8;
            //info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;
            //info.m_pavementWidth = 2;
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
            //    info.m_class = highwayTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY2L_TUNNEL);
            }
            //else
            //{
            //    info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY2L);
            //}

            ///////////////////////////
            // Set up lanes          //
            ///////////////////////////
            if (version == NetInfoVersion.Slope || version == NetInfoVersion.Tunnel)
            {
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

                leftHwLaneProps.AddRange(highwayInfo.GetLeftTrafficLights(version));
                rightHwLaneProps.AddRange(highwayInfo.GetRightTrafficLights(version));

                // Lightning
                rightHwLaneProps.SetHighwayRightLights(version);
                if (version == NetInfoVersion.Slope)
                {
                    leftHwLaneProps.AddLeftWallLights();
                    rightHwLaneProps.AddRightWallLights();
                }

                leftHwLane.m_laneProps.m_props = leftHwLaneProps.ToArray();
                rightHwLane.m_laneProps.m_props = rightHwLaneProps.ToArray();

                info.TrimNonHighwayProps();
            }
            ///////////////////////////
            // AI                    //
            ///////////////////////////

            //var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
            //var playerNetAI = info.GetComponent<PlayerNetAI>();

            //if (hwPlayerNetAI != null && playerNetAI != null)
            //{
            //    playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost * 2 / 3;
            //    playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost * 2 / 3;
            //}

            //var roadBaseAI = info.GetComponent<RoadBaseAI>();

            //if (roadBaseAI != null)
            //{
            //    roadBaseAI.m_highwayRules = true;
            //    roadBaseAI.m_trafficLights = false;
            //}

            //var roadAI = info.GetComponent<RoadAI>();

            //if (roadAI != null)
            //{
            //    roadAI.m_enableZoning = false;
            //}
        }
    }
}
