using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.PathFindingFeatures;

namespace Transit.Addon.TM.Overlays.LaneRouting.Markers
{
    public class LaneRoutesMarker
    {
        private readonly TAMLaneRoute _model;

        public LaneAnchorMarker OriginArchor { get; private set; }
        public ICollection<LaneAnchorMarker> DestinationArchors { get; private set; }

        public LaneRoutesMarker(TAMLaneRoute model, LaneAnchorMarker originArchor, IEnumerable<LaneAnchorMarker> destinationArchors)
        {
            _model = model;
            OriginArchor = originArchor;
            DestinationArchors = new HashSet<LaneAnchorMarker>(destinationArchors);
        }

        public bool AddDestination(LaneAnchorMarker destination)
        {
            var succeeded = _model.AddConnection(destination.LaneId);

            if (!DestinationArchors.Contains(destination))
            {
                DestinationArchors.Add(destination);
            }

            TAMLaneRoutingManager.instance.UpdateLaneArrows(_model.LaneId, _model.NodeId);

            return succeeded;
        }

        public bool RemoveDestination(LaneAnchorMarker destination)
        {
            var succeeded = _model.RemoveConnection(destination.LaneId);

            if (DestinationArchors.Contains(destination))
            {
                DestinationArchors.Remove(destination);
            }

            TAMLaneRoutingManager.instance.UpdateLaneArrows(_model.LaneId, _model.NodeId);

            return succeeded;
        }
    }
}
