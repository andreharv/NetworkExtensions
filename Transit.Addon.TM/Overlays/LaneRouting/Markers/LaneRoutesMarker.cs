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

        public LaneRoutesMarker(TAMLaneRoute model, LaneAnchorMarker originArchor)
        {
            Model = model;
            OriginArchor = originArchor;
            DestinationArchors = new HashSet<LaneAnchorMarker>();
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

        public void SetDestinations(NetLane.Flags directions, IEnumerable<LaneAnchorMarker> allAnchors)
        {
            Model.Connections = NetManager
                .instance
                .GetConnectingLanes(Model.LaneId, directions)
                .Distinct()
                .ToArray();

            SyncDestinations(allAnchors);

            TAMLaneRoutingManager.instance.UpdateLaneArrows(Model.LaneId, Model.NodeId);
        }

        public void SyncDestinations(IEnumerable<LaneAnchorMarker> allAnchors)
        {
            var allDestinationTable = allAnchors
                .Where(a => !a.IsOrigin)
                .Where(a => a != OriginArchor)
                .ToDictionary(a => a.LaneId);

            var destinationAnchors = Model
                .Connections
                .Select(id =>
                    allDestinationTable.ContainsKey(id) ?
                    allDestinationTable[id] :
                    null)
                .Where(a => a != null)
                .Distinct()
                .ToArray();

            DestinationArchors = new HashSet<LaneAnchorMarker>(destinationAnchors);
        }

        /// <summary>
        /// Returns true if is still relevant
        /// N.B. Will cause a desync of DestinationArchors with the Model, use SyncDestinations afterward
        /// </summary>
        public bool Scrub()
        {
            if (TAMLaneRoutingManager.instance.ScrubRoute(Model))
            {
                TAMLaneRoutingManager.instance.UpdateLaneArrows(Model.LaneId, Model.NodeId);
                return true;
            }
            else
            {
                TAMLaneRoutingManager.instance.DestroyRoute(Model);
                return false;
            }
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
