using System.Diagnostics;
using System.Linq;
using Transit.Addon.RoadExtensions.Roads.Highways;
using Transit.Addon.RoadExtensions.Roads.Highways.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;

namespace Transit.Addon.RoadExtensions.Roads.Highways.Highway5L
{
    public partial class Highway5LBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 50; } }
        public int UIOrder { get { return 50; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ONEWAY_6L; } }
        public string Name { get { return "Five-Lane Highway"; } }
        public string DisplayName { get { return "Five-Lane Highway"; } }
        public string Description { get { return "A five-lane, one-way road suitable for very high and dense traffic between metropolitan areas. Lanes going the opposite direction need to be built separately. Highway does not allow zoning next to it!"; } }
        public string UICategory { get { return "RoadsHighway"; } }
        public string ShortDescription { get { return "No parking, not zoneable, high traffic"; } }

        public string ThumbnailsPath { get { return @"Roads\Highways\Highway5L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\Highways\Highway5L\infotooltip.png"; } }
        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            var sw = new Stopwatch();
            sw.Start();
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);
            var highwayTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_TUNNEL);
            sw.Stop();
            Framework.Debug.Log($"Templates in {sw.ElapsedMilliseconds}ms");
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            var swa = new Stopwatch();
            swa.Start();
            info.Setup28mMesh(version);
            swa.Stop();
            Framework.Debug.Log($"Modeling in {swa.ElapsedMilliseconds}ms");

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            var swb = new Stopwatch();
            swb.Start();
            SetupTextures(info, version);
            swb.Stop();
            Framework.Debug.Log($"Textures in {swb.ElapsedMilliseconds}ms");

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            var sw2 = new Stopwatch();
            sw2.Start();
            info.m_availableIn = ItemClass.Availability.All;
            //info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY5L);
            info.m_surfaceLevel = 0;
            info.m_createPavement = !(version == NetInfoVersion.Ground || version == NetInfoVersion.Tunnel);
            info.m_createGravel = version == NetInfoVersion.Ground;
            info.m_averageVehicleLaneSpeed = 2f;
            info.m_hasParkingSpaces = false;
            info.m_hasPedestrianLanes = false;
            info.m_halfWidth = (version == NetInfoVersion.Slope ? 16 : 14);
            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;
            info.m_pavementWidth = 2;
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                info.m_class = highwayTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY5L_TUNNEL);
            }
            else
            {
                info.m_class = highwayInfo.m_class.Clone(NetInfoClasses.NEXT_HIGHWAY5L);
            }

            sw2.Stop();
            Framework.Debug.Log($"Set up in {sw2.ElapsedMilliseconds}ms");
            ///////////////////////////
            // Set up lanes          //
            ///////////////////////////
            var sw3 = new Stopwatch();
            sw3.Start();
            info.SetupHighwayLanes();
            var leftHwLane = info.SetHighwayLeftShoulder(highwayInfo, version);
            var rightHwLane = info.SetHighwayRightShoulder(highwayInfo, version);
            var vehicleLanes = info.SetHighwayVehicleLanes(-1);


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
            sw3.Stop();
            Framework.Debug.Log($"Lanes and props in {sw3.ElapsedMilliseconds}ms");

            ///////////////////////////
            // AI                    //
            ///////////////////////////
            var sw4 = new Stopwatch();
            sw4.Start();
            var hwPlayerNetAI = highwayInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (hwPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = hwPlayerNetAI.m_constructionCost * 5 / 3;
                playerNetAI.m_maintenanceCost = hwPlayerNetAI.m_maintenanceCost * 5 / 3;
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
            sw4.Stop();
            Framework.Debug.Log($"AI in {sw4.ElapsedMilliseconds}ms");
        }
    }
}
