using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using static Transit.Framework.NetInfoExtensions;

namespace Transit.Addon.RoadExtensions.Roads.Avenues.AsymAvenue6L.AsymAvenueL2R4
{
    public partial class AsymAvenueL2R4Builder : Activable, IMultiNetInfoBuilderPart, INetInfoSpecificBaseBuilder
    {
        public int Order { get { return 10; } }
        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "AsymAvenueL2R4"; } }
        public string DisplayName { get { return "Five-Lane Asymmetrical Road: (2+4)"; } }
        public string ShortDescription { get { return "Zoneable, medium to high traffic"; } }
        public NetInfoVersion SupportedVersions { get { return NetInfoVersion.AllWithDecorationAndPavement; } }
        public string GetSpecificBasedPrefabName(NetInfoVersion version)
        {
            if (version == NetInfoVersion.GroundGrass || version == NetInfoVersion.GroundTrees || version == NetInfoVersion.GroundPavement)
            {
                return "AsymAvenueL2R4";
            }
            return NetInfos.Vanilla.GetPrefabName(BasedPrefabName, version);
        }
        public IEnumerable<IMenuItemBuilder> MenuItemBuilders
        {
            get
            {
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 61,
                    Name = "AsymAvenueL2R4",
                    DisplayName = "Six-Lane Asymmetrical Road: (2+4)",
                    Description = "An asymmetrical road with two left lane and four right lanes.  Note, dragging this road backwards reverses its orientation.",
                    ThumbnailsPath = @"Roads\Avenues\AsymAvenue6L\AsymAvenueL2R4\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\AsymAvenue6L\AsymAvenueL2R4\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 62,
                    Name = "AsymAvenueL2R4 Decoration Grass",
                    DisplayName = "Six-Lane Asymmetrical Road Grass: (2+4)",
                    Description = "An asymmetrical road with two left lane and four right lanes.  Note, dragging this road backwards reverses its orientation.",
                    ThumbnailsPath = @"Roads\Avenues\AsymAvenue6L\AsymAvenueL2R4\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\AsymAvenue6L\AsymAvenueL2R4\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 63,
                    Name = "AsymAvenueL2R4 Decoration Trees",
                    DisplayName = "Six-Lane Asymmetrical Road Trees: (2+4)",
                    Description = "An asymmetrical road with two left lane and four right lanes.  Note, dragging this road backwards reverses its orientation.",
                    ThumbnailsPath = @"Roads\Avenues\AsymAvenue6L\AsymAvenueL2R4\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\AsymAvenue6L\AsymAvenueL2R4\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 64,
                    Name = "AsymAvenueL2R4 Decoration Pavement",
                    DisplayName = "Six-Lane Asymmetrical Road Pavement: (2+4)",
                    Description = "An asymmetrical road with two left lane and four right lanes.  Note, dragging this road backwards reverses its orientation.",
                    ThumbnailsPath = @"Roads\Avenues\AsymAvenue6L\AsymAvenueL2R4\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\AsymAvenue6L\AsymAvenueL2R4\infotooltip.png"
                };
            }
        }
        

        public void SetupRoadLanes(NetInfo info, NetInfoVersion version)
        {
            info.SetRoadLanes(version, new LanesConfiguration
            {
                IsTwoWay = true,
                PedPropOffsetX = 0.5f,
                BusStopOffset = 3,
                LayoutStyle = LanesLayoutStyle.AsymL2R4,
                CenterLane = CenterLaneType.Median,
                CenterLaneWidth = 2,
            });
            info.DoBuildupMulti(version);
        }
        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var owRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            var owRoadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L_TUNNEL);
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup32m4mSW2mMdn(version, LanesLayoutStyle.AsymL2R4);
            
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            info.SetupTextures(version, LanesLayoutStyle.AsymL2R4);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_enableMiddleNodes = true;
            info.m_connectGroup = ConnextGroup.TwoPlusFour.GetConnectGroup();
            info.m_hasParkingSpaces = version == NetInfoVersion.Ground;
            info.m_pavementWidth = (version == NetInfoVersion.Ground || version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated ? 4 : 6);
            info.m_halfWidth = (version != NetInfoVersion.Elevated && version != NetInfoVersion.Bridge ? 16 : 14);
            info.SetupConnectGroup("4mSw2mMdn", ConnextGroup.TwoPlusFour, ConnextGroup.TwoPlusTwo,ConnextGroup.ThreePlusThree, ConnextGroup.ThreeMidL, ConnextGroup.FourPlusFour);
            info.m_canCrossLanes = false;
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition | CitizenInstance.Flags.Underground;
                info.m_class = owRoadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL4L_ROAD_TUNNEL);
            }
            else
            {
                info.m_class = owRoadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL4L_ROAD);
            }

            SetupRoadLanes(info, version);
            info.SetupLaneProps(version);

            info.SetupNewSpeedLimitProps(50, 40);

            // AI
            var owPlayerNetAI = owRoadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost; // Charge by the lane?
            }

            // TODO: make it configurable
            var roadBaseAI = info.GetComponent<RoadBaseAI>();
            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }
    }
}
