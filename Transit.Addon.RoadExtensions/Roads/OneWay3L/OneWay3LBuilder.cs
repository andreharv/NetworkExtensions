using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Roads.Roads;

namespace Transit.Addon.RoadExtensions.Roads.OneWay3L
{
    public partial class OneWay3LBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 8; } }
        public int UIOrder { get { return 10; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
        public string Name { get { return "Oneway3L"; } }
        public string DisplayName { get { return "Three-Lane Oneway"; } }
        public string Description { get { return "A three-lane one-way road without parkings spaces. Supports medium traffic."; } }
        public string UICategory { get { return AdditionnalMenus.ROADS_SMALL_HV; } }

        public string ThumbnailsPath { get { return @"Roads\OneWay3L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Roads\OneWay3L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_SLOPE);
            var owRoadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L);

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup16m3mSWMesh(version, highwayInfo);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);
            
            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            info.m_class = owRoadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD);
            info.m_pavementWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 3 : 6);
            info.m_halfWidth = (version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 8 : 11);
            info.m_class.m_level = ItemClass.Level.Level3; // To make sure they dont fit with the 4L Small Roads
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition;
            }

            // Setting up lanes
            info.SetRoadLanes(version, 1);

            //Setting Up Props
            //var leftHWProps = info.GetLeftHWProps();
            //var rightHWProps = info.GetRightHWProps();

            //if (version == NetInfoVersion.Slope)
            //{
            //    leftHWProps.AddLeftWallLights();
            //    rightHWProps.AddRightWallLights();
            //}

            
            //var propLanes = info.m_lanes.Where(l => l.m_laneProps != null && (l.m_laneProps.name.ToLower().Contains("left") || l.m_laneProps.name.ToLower().Contains("right"))).ToList();

            var owPlayerNetAI = owRoadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 3 / 2; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 3 / 2; // Charge by the lane?
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }
    }
}
