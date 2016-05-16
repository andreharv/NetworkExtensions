using System.Collections.Generic;
using System.Linq;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Framework;
using Transit.Framework.UI.Ingame;
using UnityEngine;

namespace Transit.Addon.TM.Overlays.LaneRouting.Markers
{
    public class LaneRoutesMarker : UIMarker
    {
        private const float ROUTE_WIDTH = 0.25f;

        public TAMLaneRoute Model { get; private set; }
        public Vector3 NodePosition { get { return NetManager.instance.m_nodes.m_buffer[Model.NodeId].m_position; } }

        public LaneAnchorMarker OriginArchor { get; private set; }
        public ICollection<LaneAnchorMarker> DestinationArchors { get; private set; }

        public LaneRoutesMarker(TAMLaneRoute model, LaneAnchorMarker originArchor, IEnumerable<LaneAnchorMarker> destinationArchors)
        {
            Model = model;
            OriginArchor = originArchor;
            DestinationArchors = new HashSet<LaneAnchorMarker>(destinationArchors);
        }

        public bool AddDestination(LaneAnchorMarker destination)
        {
            var succeeded = Model.AddConnection(destination.LaneId);

            if (!DestinationArchors.Contains(destination))
            {
                DestinationArchors.Add(destination);
            }

            TAMLaneRoutingManager.instance.UpdateLaneArrows(Model.LaneId, Model.NodeId);

            return succeeded;
        }

        public bool RemoveDestination(LaneAnchorMarker destination)
        {
            var succeeded = Model.RemoveConnection(destination.LaneId);

            if (DestinationArchors.Contains(destination))
            {
                DestinationArchors.Remove(destination);
            }

            TAMLaneRoutingManager.instance.UpdateLaneArrows(Model.LaneId, Model.NodeId);

            return succeeded;
        }

        public void SetDestinations(IEnumerable<LaneAnchorMarker> destinationArchors)
        {
            DestinationArchors = destinationArchors.ToList();

            Model.Connections = DestinationArchors
                .Select(a => a.LaneId)
                .Distinct()
                .ToArray();

            TAMLaneRoutingManager.instance.UpdateLaneArrows(Model.LaneId, Model.NodeId);
        }

        protected override void OnRendered(RenderManager.CameraInfo cameraInfo)
        {
            if (IsSelected)
            {
                foreach (var connection in DestinationArchors)
                {
                    RenderManager.instance.OverlayEffect.DrawRouting(cameraInfo, OriginArchor.Position, connection.Position, NodePosition, OriginArchor.Color, ROUTE_WIDTH);
                }
            }
            else
            {
                foreach (var connection in DestinationArchors)
                {
                    RenderManager.instance.OverlayEffect.DrawRouting(cameraInfo, OriginArchor.Position, connection.Position, NodePosition, OriginArchor.Color.Dim(75), ROUTE_WIDTH);
                }
            }
        }
    }
}
