using System.Collections.Generic;
using System.Linq;
//using SingleTrainTrack.Meshes;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Addon.RoadExtensions.PublicTransport.RailUtils;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using UnityEngine;

namespace DoubleTrainTrack.Rail2LOW
{
    public partial class Rail2LOWBuilder : Activable, INetInfoBuilderPart
    {
        public int Order { get { return 7; } }
        public int UIOrder { get { return 9; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.TRAINTRACK; } }
        public string Name { get { return "Rail2L"; } }
        public string DisplayName { get { return "One-Way Double Rail Track"; } }
        public string Description { get { return "Dual one way rail tracks that can be connected to conventional rail."; } }
        public string ShortDescription { get { return "Double Rail Track"; } }
        public string UICategory { get { return "PublicTransportTrain"; } }

        public string ThumbnailsPath { get { return @"PublicTransport\Rail\Rail1L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"PublicTransport\Rail\Rail1L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var railVersionName = string.Format("{0} {1}", NetInfos.Vanilla.TRAINTRACK, (version == NetInfoVersion.Ground ? string.Empty : version.ToString())).Trim();
            var railInfo = Prefabs.Find<NetInfo>(railVersionName);
            info.m_class = railInfo.m_class.Clone("NExtDoubleTrackOW");
            info.m_halfWidth = 4.999f;
            info.Setup10mMesh(version);
            var lanes = new List<NetInfo.Lane>();
            lanes.AddRange(info.m_lanes.ToList());
            for (int i = 0; i < lanes.Count; i++)
            {
                if (lanes[i].m_direction == NetInfo.Direction.Backward)
                {
                    lanes[i].m_direction = NetInfo.Direction.Forward;
                }
            }
            info.m_lanes = lanes.ToArray();
            info.m_connectGroup = NetInfo.ConnectGroup.WideTram;
            info.m_nodeConnectGroups = NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.NarrowTram;
            var railInfos = new List<NetInfo>();
            railInfos.Add(railInfo);
            railInfos.Add(Prefabs.Find<NetInfo>(NetInfos.Vanilla.TRAIN_STATION_TRACK, false));
            railInfos.Add(Prefabs.Find<NetInfo>("Train Cargo Track", false));
            for (int i = 0; i < railInfos.Count; i++)
            {
                var ri = railInfos[i];
                //info.m_nodes[1].m_connectGroup = (NetInfo.ConnectGroup)9; 
                ri.m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
                railInfo.m_nodeConnectGroups = NetInfo.ConnectGroup.NarrowTram;
                if (railInfo.m_nodes.Length > 1)
                {
                    railInfo.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
                }

            }

        }
    }
}