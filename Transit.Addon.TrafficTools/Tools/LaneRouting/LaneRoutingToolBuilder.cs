using System;
using System.Collections.Generic;
using Transit.Addon.TrafficTools.Common;
using Transit.Addon.TrafficTools.LaneRouting.Markers;
using Transit.Addon.TrafficTools._Extensions;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.TrafficTools.LaneRouting
{
    public class LaneRoutingToolBuilder : Activable, IToolBuilder
    {
        public int Order { get { return 10; } }
        public int UIOrder { get { return 10; } }

        public string Name { get { return "Intersection Routing"; } }
        public string DisplayName { get { return Name; } }
        public string Description { get { return "Allows you to customize entry and exit points in junctions."; } }
        public string UICategory { get { return "IntersectionEditors"; } }

        public string ThumbnailsPath { get { return @"Tools\LaneRouting\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Tools\LaneRouting\infotooltip.png"; } }

        public Type ToolType { get { return typeof (LaneRoutingTool); } }
    }

    public class LaneRoutingTool : RoadNodeEditorToolBase
    {
        //protected uint _entryLane, _exitLane;
        //protected ushort _hoveredNode;

        //protected override void CustomAwake()
        //{
        //    _entryLane = _exitLane = 0;
        //    _hoveredNode = 0;
        //}

        //protected override void OnRenderNode(RenderManager.CameraInfo camera)
        //{
        //    if (_hoveredNode == 0)
        //        return;

        //    NetNode node = NetManager.instance.m_nodes.m_buffer[_hoveredNode];
        //    Color color = node.CountSegments() > 1 ? Color.green : Color.red;
        //    RenderManager.instance.OverlayEffect.DrawNodeSelection(camera, _hoveredNode, color);

        //    // TODO: render connections on this node
        //}

        private ushort? _selectedNodeId;
        private readonly IDictionary<ushort, FastList<LaneEdgeMarker>> _nodeLaneEdges = new Dictionary<ushort, FastList<LaneEdgeMarker>>();

        protected override void OnNodeClicked(ushort nodeId, MouseKeyCode code)
        {
            switch (code)
            {
                case MouseKeyCode.LeftButton:
                    if (_selectedNodeId != nodeId)
                    {
                        _selectedNodeId = nodeId;

                        if (_selectedNodeId != null)
                        {
                            FastList<LaneEdgeMarker> nodeMarkers;
                            if (!_nodeLaneEdges.TryGetValue(_selectedNodeId.Value, out nodeMarkers))
                            {
                                CreateLaneEdges(_selectedNodeId.Value);
                            }
                            else
                            {
                                UpdateLaneEdges(_selectedNodeId.Value);
                            }
                        }
                    }
                    break;

                case MouseKeyCode.RightButton:
                    _selectedNodeId = null;
                    break;
            }
        }

        protected override void OnNodeHovered(ushort nodeId)
        {
            base.OnNodeHovered(nodeId);

            if (_selectedNodeId != null && _selectedNodeId == nodeId)
            {
                FastList<LaneEdgeMarker> laneEdgeMarkers;
                if (_nodeLaneEdges.TryGetValue(_selectedNodeId.Value, out laneEdgeMarkers))
                {
                    Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
                    foreach (var laneMarker in laneEdgeMarkers)
                    {
                        bounds.center = laneMarker.Position;
                        if (bounds.IntersectRay(mouseRay))
                        {
                            laneMarker.SetState(UIState.Hovered);
                        }
                        else
                        {
                            laneMarker.SetState(UIState.Default);
                        }
                    }
                }
            }
        }

        bool RayCastSegmentAndNode(out RaycastOutput output)
        {
            RaycastInput input = new RaycastInput(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.farClipPlane);
            input.m_netService.m_service = ItemClass.Service.Road;
            input.m_netService.m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
            input.m_ignoreSegmentFlags = NetSegment.Flags.None;
            input.m_ignoreNodeFlags = NetNode.Flags.None;
            input.m_ignoreTerrain = true;

            return RayCast(input, out output);
        }

        private void CreateLaneEdges(ushort selectedNodeId)
        {
            var markers = new FastList<LaneEdgeMarker>();

            var allNodes = NetManager.instance.m_nodes;
            var allSegments = NetManager.instance.m_segments;
            var allLanes = NetManager.instance.m_lanes;

            var node = allNodes.m_buffer[selectedNodeId];
            var nodeOffsetMultiplier = node.CountSegments() <= 2 ? 3 : 1;
            var segmentId = node.m_segment0;

            var anchorColorId = 0;

            for (var i = 0; i < 8 && segmentId != 0; i++)
            {
                var segment = allSegments.m_buffer[segmentId];
                var segmentOffset = segment.FindDirection(segmentId, selectedNodeId) * nodeOffsetMultiplier;
                var isEndNode = segment.m_endNode == selectedNodeId;
                var netInfo = segment.Info;
                var netInfoLanes = netInfo.m_lanes;
                var laneId = segment.m_lanes;

                for (var j = 0; j < netInfoLanes.Length && laneId != 0; j++)
                {
                    var lane = netInfoLanes[j];

                    if ((lane.m_laneType & NetInfo.LaneType.Vehicle) == NetInfo.LaneType.Vehicle)
                    {
                        var lanePos = Vector3.zero;
                        var laneDir =
                            ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ?
                            lane.m_finalDirection :
                            NetInfo.InvertDirection(lane.m_finalDirection);

                        var isOrigin = false;
                        if (isEndNode)
                        {
                            if ((laneDir & (NetInfo.Direction.Forward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Forward)
                                isOrigin = true;
                            lanePos = allLanes.m_buffer[laneId].m_bezier.d;
                        }
                        else
                        {
                            if ((laneDir & (NetInfo.Direction.Backward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Backward)
                                isOrigin = true;
                            lanePos = allLanes.m_buffer[laneId].m_bezier.a;
                        }

                        markers.Add(new LaneEdgeMarker(
                            laneId,
                            lanePos + segmentOffset,
                            isOrigin,
                            isOrigin ? anchorColorId : anchorColorId + 1));
                    }

                    laneId = allLanes.m_buffer[laneId].m_nextLane;
                }

                anchorColorId += 2;
                segmentId = segment.GetRightSegment(selectedNodeId);
                if (segmentId == node.m_segment0)
                    segmentId = 0;
            }

            //for (int i = 0; i < markers.m_size; i++)
            //{
            //    if (!markers.m_buffer[i].IsSource)
            //        continue;

            //    uint[] connections = LanesManager.instance.GetLaneConnections(markers.m_buffer[i].LaneId);
            //    if (connections == null || connections.Length == 0)
            //        continue;

            //    for (int j = 0; j < markers.m_size; j++)
            //    {
            //        if (markers.m_buffer[j].IsSource)
            //            continue;

            //        if (connections.Contains(markers.m_buffer[j].LaneId))
            //            markers.m_buffer[i].Connections.Add(markers.m_buffer[j]);
            //    }
            //}

            _nodeLaneEdges[selectedNodeId] = markers;
        }

        private void UpdateLaneEdges(ushort selectedNodeId)
        {

        }

        public override void RenderOverlay(RenderManager.CameraInfo camera)
        {
            if (_selectedNodeId != null)
            {
                var selectedNode = NetManager.instance.GetNode(_selectedNodeId.Value);

                if (selectedNode != null)
                {
                    RenderSelectedNode(camera, selectedNode.Value, _selectedNodeId.Value);
                }
            }

            base.RenderOverlay(camera);
        }

        private void RenderSelectedNode(RenderManager.CameraInfo camera, NetNode selectedNode, ushort selectedNodeId)
        {
            FastList<LaneEdgeMarker> laneEdgeMarkers;
            if (_nodeLaneEdges.TryGetValue(selectedNodeId, out laneEdgeMarkers))
            {
                //Vector3 nodePos = selectedNode.m_position;
                foreach (var laneMarker in laneEdgeMarkers)
                {
                    //for (int j = 0; j < laneMarker.Connections.m_size; j++)
                    //    RenderLane(camera, laneMarker.Position, laneMarker.Connections.m_buffer[j].Position, nodePos, laneMarker.Color);

                    //if (m_selectedMarker != laneMarker && !IsActive(laneMarker))
                    //    continue;

                    //if (m_selectedMarker == laneMarker)
                    //{
                    //    RaycastOutput output;
                    //    if (RayCastSegmentAndNode(out output))
                    //    {
                    //        RenderLane(cameraInfo, m_selectedMarker.Position, output.m_hitPos, nodePos, m_selectedMarker.Color);
                    //        m_selectedMarker.Select();
                    //    }
                    //}

                    laneMarker.Render(camera);
                }
            }
        }
    }
}

