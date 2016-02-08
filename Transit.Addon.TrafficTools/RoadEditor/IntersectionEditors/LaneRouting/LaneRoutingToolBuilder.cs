using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.TrafficTools.RoadEditor.IntersectionEditors.LaneRouting
{
    public class LaneRoutingToolBuilder : Activable, IToolBuilder
    {
        public int Order { get { return 10; } }
        public int UIOrder { get { return 10; } }

        public string Name { get { return "Intersection Routing"; } }
        public string DisplayName { get { return Name; } }
        public string Description { get { return "Allows you to customize entry and exit points in junctions."; } }
        public string UICategory { get { return "IntersectionEditors"; } }

        public string ThumbnailsPath { get { return @"RoadEditor\IntersectionEditors\LaneRouting\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"RoadEditor\IntersectionEditors\LaneRouting\infotooltip.png"; } }
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
    }
}

