using UnityEngine;

namespace Transit.Addon.TrafficTools.RoadEditor.LaneRouting
{
    public class LaneRoutingTool : RoadEditorToolBase
    {
        public enum Template
        {
            None,
            Roundabout
        }

        protected uint _entryLane, _exitLane;
        protected ushort _hoveredSegment;
        protected Template _template;

        public Template TemplateMode
        {
            get { return _template; }
            set
            {
                _mode = Mode.SelectNode;
                _template = value;
            }
        }

        protected override void CustomAwake()
        {
            _entryLane = _exitLane = 0;
            _hoveredSegment = 0;
            _template = Template.None;
        }

        protected override void OnRenderNode(RenderManager.CameraInfo camera)
        {
            if (_hoveredNode == 0)
                return;

            NetNode node = NetManager.instance.m_nodes.m_buffer[_hoveredNode];
            Color color = node.CountSegments() > 1 ? Color.green : Color.red;
            RenderNode(camera, _hoveredNode, color);

            // TODO: render connections on this node
        }
    }
}
