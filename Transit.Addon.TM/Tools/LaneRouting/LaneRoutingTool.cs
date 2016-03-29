using ColossalFramework.Math;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Framework.Network;
using UnityEngine;

namespace Transit.Addon.TM.Tools.LaneRouting
{
    public class LaneRoutingTool : ToolBase
    {
        private const NetNode.Flags CUSTOMIZED_NODE_FLAG = (NetNode.Flags)(1 << 28);

        private class NodeLaneMarker
        {
            public ushort NodeId { get; set; }
            public Vector3 Position { get; set; }
            public bool IsSource { get; set; }
            public uint LaneId { get; set; }
            public float Size { get; set; }
            public Color Color { get; set; }
            public FastList<NodeLaneMarker> Connections { get; private set; }

            public NodeLaneMarker()
            {
                Size = 1f;
                Connections = new FastList<NodeLaneMarker>();
            }
        }

        private struct Segment
        {
            public ushort SegmentId { get; set; }
            public ushort TargetNodeId { get; set; }
        }

        private ushort m_hoveredSegment;
        private ushort m_hoveredNode;
        private ushort m_selectedNode;
        private NodeLaneMarker m_selectedMarker;
        private readonly Dictionary<ushort, FastList<NodeLaneMarker>> m_nodeMarkers = new Dictionary<ushort, FastList<NodeLaneMarker>>();
        private readonly Dictionary<ushort, Segment> m_segments = new Dictionary<ushort, Segment>();

        protected override void Awake()
        {
            base.Awake();

            StartCoroutine(LoadMarkers());
        }

        private IEnumerator LoadMarkers()
        {
            while (!TAMLaneRoutingManager.instance.IsLoaded())
            {
                yield return new WaitForEndOfFrame();
            }

            var nodesList = new HashSet<ushort>();
            foreach (var route in TAMLaneRoutingManager.instance.GetAllRoutes())
            {
                if (route == null)
                    continue;

                if (route.Connections.Any())
                    nodesList.Add(route.NodeId);
            }

            foreach (var nodeId in nodesList)
                SetNodeMarkers(nodeId);
        }

        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            if (Input.GetKeyUp(KeyCode.PageDown))
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.Traffic, InfoManager.SubInfoMode.Default);
            else if (Input.GetKeyUp(KeyCode.PageUp))
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);

            if (m_toolController.IsInsideUI)
                return;

            if (m_selectedNode != 0)
            {
                HandleIntersectionRouting();
                return;
            }

            if (!RayCastSegmentAndNode(out m_hoveredSegment, out m_hoveredNode))
            {
                m_segments.Clear();
                return;
            }


            if (m_hoveredSegment != 0)
            {
                NetSegment segment = NetManager.instance.m_segments.m_buffer[m_hoveredSegment];
                NetNode startNode = NetManager.instance.m_nodes.m_buffer[segment.m_startNode];
                NetNode endNode = NetManager.instance.m_nodes.m_buffer[segment.m_endNode];
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (startNode.CountSegments() > 1)
                {
                    Bounds bounds = startNode.m_bounds;
                    if (m_hoveredNode != 0)
                        bounds.extents /= 2f;
                    if (bounds.IntersectRay(mouseRay))
                    {
                        m_hoveredSegment = 0;
                        m_hoveredNode = segment.m_startNode;
                    }
                }

                if (m_hoveredSegment != 0 && endNode.CountSegments() > 1)
                {
                    Bounds bounds = endNode.m_bounds;
                    if (m_hoveredNode != 0)
                        bounds.extents /= 2f;
                    if (bounds.IntersectRay(mouseRay))
                    {
                        m_hoveredSegment = 0;
                        m_hoveredNode = segment.m_endNode;
                    }
                }

                if (m_hoveredSegment != 0)
                {
                    m_hoveredNode = 0;
                    if (!m_segments.ContainsKey(m_hoveredSegment))
                    {
                        m_segments.Clear();
                        SetSegments(m_hoveredSegment);
                        SetLaneMarkers();
                    }
                }

            }
            else if (m_hoveredNode != 0 && NetManager.instance.m_nodes.m_buffer[m_hoveredNode].CountSegments() < 2)
            {
                m_hoveredNode = 0;
            }

            if (m_hoveredSegment == 0)
            {
                m_segments.Clear();
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_selectedNode = m_hoveredNode;
                m_hoveredNode = 0;

                if (m_selectedNode != 0)
                    SetNodeMarkers(m_selectedNode, true);
            }
        }

        private void HandleIntersectionRouting()
        {
            FastList<NodeLaneMarker> nodeMarkers;
            if (m_nodeMarkers.TryGetValue(m_selectedNode, out nodeMarkers))
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                NodeLaneMarker hoveredMarker = null;
                Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
                for (int i = 0; i < nodeMarkers.m_size; i++)
                {
                    NodeLaneMarker marker = nodeMarkers.m_buffer[i];

                    if (!IsActive(marker))
                        continue;

                    bounds.center = marker.Position;
                    if (bounds.IntersectRay(mouseRay))
                    {
                        hoveredMarker = marker;
                        marker.Size = 2f;
                    }
                    else
                        marker.Size = 1f;
                }

                if (hoveredMarker != null && Input.GetMouseButtonUp(0))
                {
                    if (m_selectedMarker == null)
                    {
                        m_selectedMarker = hoveredMarker;
                    }
                    else if (TAMLaneRoutingManager.instance.RemoveLaneConnection(m_selectedMarker.LaneId, hoveredMarker.LaneId))
                    {
                        m_selectedMarker.Connections.Remove(hoveredMarker);
                    }
                    else if (TAMLaneRoutingManager.instance.AddLaneConnection(m_selectedMarker.LaneId, hoveredMarker.LaneId))
                    {
                        m_selectedMarker.Connections.Add(hoveredMarker);
                    }
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (m_selectedMarker != null)
                    m_selectedMarker = null;
                else
                    m_selectedNode = 0;
            }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            m_hoveredNode = m_hoveredSegment = 0;
            m_selectedNode = 0;
            m_selectedMarker = null;
            m_segments.Clear();
        }

        private bool IsActive(NodeLaneMarker marker)
        {
            if (m_selectedMarker != null && (marker.IsSource || NetManager.instance.m_lanes.m_buffer[m_selectedMarker.LaneId].m_segment == NetManager.instance.m_lanes.m_buffer[marker.LaneId].m_segment))
                return false;
            else if (m_selectedMarker == null && !marker.IsSource)
                return false;

            return true;
        }

        public void SetNodeMarkers(ushort nodeId, bool overwrite = false)
        {
            if (nodeId == 0)
                return;

            if (!m_nodeMarkers.ContainsKey(nodeId) || (NetManager.instance.m_nodes.m_buffer[nodeId].m_flags & CUSTOMIZED_NODE_FLAG) != CUSTOMIZED_NODE_FLAG || overwrite)
            {
                FastList<NodeLaneMarker> nodeMarkers = new FastList<NodeLaneMarker>();
                SetNodeMarkers(nodeId, nodeMarkers);
                m_nodeMarkers[nodeId] = nodeMarkers;

                NetManager.instance.m_nodes.m_buffer[nodeId].m_flags |= CUSTOMIZED_NODE_FLAG;
            }
        }

        private void SetNodeMarkers(ushort nodeId, FastList<NodeLaneMarker> nodeMarkers)
        {
            NetNode node = NetManager.instance.m_nodes.m_buffer[nodeId];
            int offsetMultiplier = node.CountSegments() <= 2 ? 3 : 1;
            ushort segmentId = node.m_segment0;
            for (int i = 0; i < 8 && segmentId != 0; i++)
            {
                NetSegment segment = NetManager.instance.m_segments.m_buffer[segmentId];
                bool isEndNode = segment.m_endNode == nodeId;
                Vector3 offset = segment.FindDirection(segmentId, nodeId) * offsetMultiplier;
                NetInfo.Lane[] lanes = segment.Info.m_lanes;
                uint laneId = segment.m_lanes;
                for (int j = 0; j < lanes.Length && laneId != 0; j++)
                {
                    //if ((lanes[j].m_laneType & (NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle)) != NetInfo.LaneType.None)
                    if ((lanes[j].m_laneType & NetInfo.LaneType.Vehicle) == NetInfo.LaneType.Vehicle)
                    {
                        Vector3 pos = Vector3.zero;
                        NetInfo.Direction laneDir = ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ? lanes[j].m_finalDirection : NetInfo.InvertDirection(lanes[j].m_finalDirection);

                        bool isSource = false;
                        if (isEndNode)
                        {
                            if ((laneDir & (NetInfo.Direction.Forward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Forward)
                                isSource = true;
                            pos = NetManager.instance.m_lanes.m_buffer[laneId].m_bezier.d;
                        }
                        else
                        {
                            if ((laneDir & (NetInfo.Direction.Backward | NetInfo.Direction.Avoid)) == NetInfo.Direction.Backward)
                                isSource = true;
                            pos = NetManager.instance.m_lanes.m_buffer[laneId].m_bezier.a;
                        }

                        nodeMarkers.Add(new NodeLaneMarker()
                        {
                            LaneId = laneId,
                            NodeId = nodeId,
                            Position = pos + offset,
                            Color = colors[nodeMarkers.m_size],
                            IsSource = isSource,
                        });
                    }

                    laneId = NetManager.instance.m_lanes.m_buffer[laneId].m_nextLane;
                }

                segmentId = segment.GetRightSegment(nodeId);
                if (segmentId == node.m_segment0)
                    segmentId = 0;
            }

            for (int i = 0; i < nodeMarkers.m_size; i++)
            {
                if (!nodeMarkers.m_buffer[i].IsSource)
                    continue;

                uint[] connections = TAMLaneRoutingManager.instance.GetLaneConnections(nodeMarkers.m_buffer[i].LaneId);
                if (connections == null || connections.Length == 0)
                    continue;

                for (int j = 0; j < nodeMarkers.m_size; j++)
                {
                    if (nodeMarkers.m_buffer[j].IsSource)
                        continue;

                    if (connections.Contains(nodeMarkers.m_buffer[j].LaneId))
                        nodeMarkers.m_buffer[i].Connections.Add(nodeMarkers.m_buffer[j]);
                }
            }
        }

        private void SetLaneMarkers()
        {
            if (m_segments.Count == 0)
                return;

            NetSegment segment = NetManager.instance.m_segments.m_buffer[m_segments.Values.First().SegmentId];
            NetInfo info = segment.Info;
            int laneCount = info.m_lanes.Length;
            bool bothWays = info.m_hasBackwardVehicleLanes && info.m_hasForwardVehicleLanes;
            bool isInverted = false;

            foreach (Segment seg in m_segments.Values)
            {
                segment = NetManager.instance.m_segments.m_buffer[seg.SegmentId];
                uint laneId = segment.m_lanes;

                if (bothWays)
                {
                    isInverted = seg.TargetNodeId == segment.m_startNode;
                    if ((segment.m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.Invert)
                        isInverted = !isInverted;
                }

                for (int j = 0; j < laneCount && laneId != 0; j++)
                {
                    NetLane lane = NetManager.instance.m_lanes.m_buffer[laneId];

                    //if ((info.m_lanes[j].m_laneType & (NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle)) != NetInfo.LaneType.None)
                    if ((info.m_lanes[j].m_laneType & NetInfo.LaneType.Vehicle) == NetInfo.LaneType.Vehicle)
                    {
                        Bezier3 bezier = lane.m_bezier;
                        bezier.GetBounds().Expand(1f);

                        int index = j;
                        if (bothWays && isInverted)
                            index += (j % 2 == 0) ? 1 : -1;
                    }

                    laneId = lane.m_nextLane;
                }
            }
        }

        private void SetSegments(ushort segmentId)
        {
            NetSegment segment = NetManager.instance.m_segments.m_buffer[segmentId];
            Segment seg = new Segment()
            {
                SegmentId = segmentId,
                TargetNodeId = segment.m_endNode
            };

            m_segments[segmentId] = seg;

            ushort infoIndex = segment.m_infoIndex;
            NetNode node = NetManager.instance.m_nodes.m_buffer[segment.m_startNode];
            if (node.CountSegments() == 2)
                SetSegments(node.m_segment0 == segmentId ? node.m_segment1 : node.m_segment0, infoIndex, ref seg);

            node = NetManager.instance.m_nodes.m_buffer[segment.m_endNode];
            if (node.CountSegments() == 2)
                SetSegments(node.m_segment0 == segmentId ? node.m_segment1 : node.m_segment0, infoIndex, ref seg);
        }

        private void SetSegments(ushort segmentId, ushort infoIndex, ref Segment previousSeg)
        {
            NetSegment segment = NetManager.instance.m_segments.m_buffer[segmentId];

            if (segment.m_infoIndex != infoIndex || m_segments.ContainsKey(segmentId))
                return;

            Segment seg = default(Segment);
            seg.SegmentId = segmentId;

            NetSegment previousSegment = NetManager.instance.m_segments.m_buffer[previousSeg.SegmentId];
            ushort nextNode;
            if ((segment.m_startNode == previousSegment.m_endNode) || (segment.m_startNode == previousSegment.m_startNode))
            {
                nextNode = segment.m_endNode;
                seg.TargetNodeId = segment.m_startNode == previousSeg.TargetNodeId ? segment.m_endNode : segment.m_startNode;
            }
            else
            {
                nextNode = segment.m_startNode;
                seg.TargetNodeId = segment.m_endNode == previousSeg.TargetNodeId ? segment.m_startNode : segment.m_endNode;
            }

            m_segments[segmentId] = seg;

            NetNode node = NetManager.instance.m_nodes.m_buffer[nextNode];
            if (node.CountSegments() == 2)
                SetSegments(node.m_segment0 == segmentId ? node.m_segment1 : node.m_segment0, infoIndex, ref seg);
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            if (m_selectedNode != 0)
            {
                FastList<NodeLaneMarker> nodeMarkers;
                if (m_nodeMarkers.TryGetValue(m_selectedNode, out nodeMarkers))
                {
                    Vector3 nodePos = NetManager.instance.m_nodes.m_buffer[m_selectedNode].m_position;
                    for (int i = 0; i < nodeMarkers.m_size; i++)
                    {
                        NodeLaneMarker laneMarker = nodeMarkers.m_buffer[i];

                        for (int j = 0; j < laneMarker.Connections.m_size; j++)
                            RenderLane(cameraInfo, laneMarker.Position, laneMarker.Connections.m_buffer[j].Position, nodePos, laneMarker.Color);

                        if (m_selectedMarker != laneMarker && !IsActive(laneMarker))
                            continue;

                        if (m_selectedMarker == laneMarker)
                        {
                            RaycastOutput output;
                            if (RayCastSegmentAndNode(out output))
                            {
                                RenderLane(cameraInfo, m_selectedMarker.Position, output.m_hitPos, nodePos, m_selectedMarker.Color);
                                m_selectedMarker.Size = 2f;
                            }
                        }

                        RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, laneMarker.Color, laneMarker.Position, laneMarker.Size, laneMarker.Position.y - 1f, laneMarker.Position.y + 1f, true, true);
                    }
                }
            }

            foreach (ushort node in m_nodeMarkers.Keys)
            {
                if (node == m_selectedNode || (NetManager.instance.m_nodes.m_buffer[node].m_flags & CUSTOMIZED_NODE_FLAG) != CUSTOMIZED_NODE_FLAG)
                    continue;

                FastList<NodeLaneMarker> list = m_nodeMarkers[node];
                Vector3 nodePos = NetManager.instance.m_nodes.m_buffer[node].m_position;
                for (int i = 0; i < list.m_size; i++)
                {
                    NodeLaneMarker laneMarker = list.m_buffer[i];
                    Color color = laneMarker.Color;
                    color.a = 0.75f;

                    for (int j = 0; j < laneMarker.Connections.m_size; j++)
                    {
                        if (((NetLane.Flags)NetManager.instance.m_lanes.m_buffer[laneMarker.Connections.m_buffer[j].LaneId].m_flags & NetLane.Flags.Created) == NetLane.Flags.Created)
                            RenderLane(cameraInfo, laneMarker.Position, laneMarker.Connections.m_buffer[j].Position, nodePos, color);
                    }

                }
            }

            if (m_hoveredNode != 0)
            {
                NetNode node = NetManager.instance.m_nodes.m_buffer[m_hoveredNode];
                RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, new Color(0f, 0f, 0.5f, 0.75f), node.m_position, 15f, node.m_position.y - 1f, node.m_position.y + 1f, true, true);
            }
        }

        private void RenderLane(RenderManager.CameraInfo cameraInfo, Vector3 start, Vector3 end, Vector3 middlePoint, Color color, float size = 0.1f)
        {
            Bezier3 bezier;
            bezier.a = start;
            bezier.d = end;
            NetSegment.CalculateMiddlePoints(bezier.a, (middlePoint - bezier.a).normalized, bezier.d, (middlePoint - bezier.d).normalized, false, false, out bezier.b, out bezier.c);

            RenderManager.instance.OverlayEffect.DrawBezier(cameraInfo, color, bezier, size, 0, 0, Mathf.Min(bezier.a.y, bezier.d.y) - 1f, Mathf.Max(bezier.a.y, bezier.d.y) + 1f, true, true);
        }

        private bool RayCastSegmentAndNode(out RaycastOutput output)
        {
            RaycastInput input = new RaycastInput(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.farClipPlane);
            input.m_netService.m_service = ItemClass.Service.Road;
            input.m_netService.m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
            input.m_ignoreSegmentFlags = NetSegment.Flags.None;
            input.m_ignoreNodeFlags = NetNode.Flags.None;
            input.m_ignoreTerrain = true;

            return RayCast(input, out output);
        }

        private bool RayCastSegmentAndNode(out ushort netSegment, out ushort netNode)
        {
            RaycastOutput output;
            if (RayCastSegmentAndNode(out output))
            {
                netSegment = output.m_netSegment;
                netNode = output.m_netNode;

                if (NetManager.instance.m_segments.m_buffer[netSegment].Info.m_lanes.FirstOrDefault(l => (l.m_vehicleType & VehicleInfo.VehicleType.Car) == VehicleInfo.VehicleType.Car) == null)
                    netSegment = 0;

                return true;
            }

            netSegment = 0;
            netNode = 0;
            return false;
        }

        private static readonly Color32[] colors = new Color32[]
		{
			new Color32(161, 64, 206, 255), 
			new Color32(79, 251, 8, 255), 
			new Color32(243, 96, 44, 255), 
			new Color32(45, 106, 105, 255), 
			new Color32(253, 165, 187, 255), 
			new Color32(90, 131, 14, 255), 
			new Color32(58, 20, 70, 255), 
			new Color32(248, 246, 183, 255), 
			new Color32(255, 205, 29, 255), 
			new Color32(91, 50, 18, 255), 
			new Color32(76, 239, 155, 255), 
			new Color32(241, 25, 130, 255), 
			new Color32(125, 197, 240, 255), 
			new Color32(57, 102, 187, 255), 
			new Color32(160, 27, 61, 255), 
			new Color32(167, 251, 107, 255), 
			new Color32(165, 94, 3, 255), 
			new Color32(204, 18, 161, 255), 
			new Color32(208, 136, 237, 255), 
			new Color32(232, 211, 202, 255), 
			new Color32(45, 182, 15, 255), 
			new Color32(8, 40, 47, 255), 
			new Color32(249, 172, 142, 255), 
			new Color32(248, 99, 101, 255), 
			new Color32(180, 250, 208, 255), 
			new Color32(126, 25, 77, 255), 
			new Color32(243, 170, 55, 255), 
			new Color32(47, 69, 126, 255), 
			new Color32(50, 105, 70, 255), 
			new Color32(156, 49, 1, 255), 
			new Color32(233, 231, 255, 255), 
			new Color32(107, 146, 253, 255), 
			new Color32(127, 35, 26, 255), 
			new Color32(240, 94, 222, 255), 
			new Color32(58, 28, 24, 255), 
			new Color32(165, 179, 240, 255), 
			new Color32(239, 93, 145, 255), 
			new Color32(47, 110, 138, 255), 
			new Color32(57, 195, 101, 255), 
			new Color32(124, 88, 213, 255), 
			new Color32(252, 220, 144, 255), 
			new Color32(48, 106, 224, 255), 
			new Color32(90, 109, 28, 255), 
			new Color32(56, 179, 208, 255), 
			new Color32(239, 73, 177, 255), 
			new Color32(84, 60, 2, 255), 
			new Color32(169, 104, 238, 255), 
			new Color32(97, 201, 238, 255), 
		};
    }
}
