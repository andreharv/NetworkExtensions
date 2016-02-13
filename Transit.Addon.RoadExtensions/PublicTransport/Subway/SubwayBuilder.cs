using System;
using System.Linq;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Addon.RoadExtensions.PublicTransport.SubwayUtils;
using System.Collections.Generic;

namespace Transit.Addon.RoadExtensions.PublicTransport.Subway
{
    public partial class SubwayBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 7; } }
        public int UIOrder { get { return 9; } }

        public string BasedPrefabName { get { return "Train Track"; } }
        public string Name { get { return "SubwayPlus"; } }
        public string DisplayName { get { return "Metro"; } }
        public string Description { get { return "A rapid transit solution offering above and underground urban transportation solutions."; } }
        public string ShortDescription { get { return "Inner-City Metro"; } }
        public string UICategory { get { return "PublicTransportTrain"; } }

        public string ThumbnailsPath { get { return @"PublicTransport\Subway\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"PublicTransport\Subway\infotooltip.png"; } }
        
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
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L);
            var owRoadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L_TUNNEL);

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
            info.m_hasParkingSpaces = false;
            //info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD);
            info.m_halfWidth = 3;//(version != NetInfoVersion.Slope && version != NetInfoVersion.Tunnel ? 8 : 11);
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition;
                //info.m_class = owRoadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD_TUNNEL);
            }
            else
            {
                //info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD);
            }

            //var propLanes = info.m_lanes.Where(l => l.m_laneProps != null && (l.m_laneProps.name.ToLower().Contains("left") || l.m_laneProps.name.ToLower().Contains("right"))).ToList();

            var remainingLanes = new List<NetInfo.Lane>();
            remainingLanes.AddRange(info
                .m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian || l.m_laneType == NetInfo.LaneType.None || l.m_laneType == NetInfo.LaneType.Parking));
            remainingLanes.AddRange(info
                .m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None)
                .Skip(1));

            info.m_lanes = remainingLanes.ToArray();
            info.m_class.m_layer = ItemClass.Layer.PublicTransport;
            info.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
            info.m_nodeConnectGroups = (NetInfo.ConnectGroup)9;
            info.m_nodes[1].m_connectGroup = (NetInfo.ConnectGroup)9; 
            var info2 = Prefabs.Find<NetInfo>("Train Track", false);
            info2.m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
            info2.m_nodeConnectGroups = NetInfo.ConnectGroup.NarrowTram;
            info2.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
            info.m_class = info2.m_class.Clone("NExtSingleTrack");
            info.m_class.m_level = (ItemClass.Level)7;
            info2.m_nodes[1].m_flagsForbidden = NetNode.Flags.OneWayIn;
            info2.m_class.m_level = ItemClass.Level.Level1;
            var owPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 3 / 2; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 3 / 2; // Charge by the lane?
            }

            var trainTrackAI = info.GetComponent<TrainTrackAI>();

            if (trainTrackAI != null)
            {
             
            }
        }
    }
}
