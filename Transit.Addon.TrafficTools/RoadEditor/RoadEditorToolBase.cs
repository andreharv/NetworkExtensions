using ColossalFramework.Math;
using UnityEngine;

namespace Transit.Addon.TrafficTools.RoadEditor
{
    public abstract partial class RoadEditorToolBase : ToolBase
    {
        protected enum Mode
        {
            SelectNode,
            SelectLane
        }
        
        protected Mode _mode;
        protected ushort _hoveredNode, _selectedNode;

        protected sealed override void Awake()
        {
            base.Awake(); // necessary to set toolController

            _mode = Mode.SelectNode;
            _hoveredNode = _selectedNode = 0;
            
            CustomAwake();
        }

        protected virtual void CustomAwake() { }

        protected override void OnToolUpdate()
        {
            // Toggle underground view
            if (Input.GetKeyUp(KeyCode.PageDown))
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.Traffic, InfoManager.SubInfoMode.Default);
            else if (Input.GetKeyUp(KeyCode.PageUp))
                InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);

            if (this.m_toolController.IsInsideUI)
                return;

            switch (_mode)
            {
                case Mode.SelectNode:
                    OnSelectNode();
                    break;
                case Mode.SelectLane:
                    OnSelectLane();
                    break;
                default:
                    break;
            }
        }

        protected virtual void OnSelectNode() { }
        protected virtual void OnSelectLane() { }

        public override void RenderOverlay(RenderManager.CameraInfo camera)
        {
            base.RenderOverlay(camera);

            switch (_mode)
            {
                case Mode.SelectNode:
                    OnRenderNode(camera);
                    break;
                case Mode.SelectLane:
                    OnRenderLane(camera);
                    break;
                default:
                    return;
            }
        }

        protected virtual void OnRenderNode(RenderManager.CameraInfo camera) { }
        protected virtual void OnRenderLane(RenderManager.CameraInfo camera) { }

        protected bool TrySelectNode(out ushort nodeId)
        {
            if (RaycastNode(out _hoveredNode) && Input.GetMouseButtonUp(0))
            {
                nodeId = _hoveredNode;
                return true;
            }

            nodeId = 0;
            return false;
        }

        protected bool RaycastNode(out ushort nodeId)
        {
            // I tried caching the raycast input, since it always has the same properties, but it causes weird issues
            RaycastInput raycastInput = new RaycastInput(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.farClipPlane);
            raycastInput.m_netService.m_service = ItemClass.Service.Road;
            raycastInput.m_netService.m_subService = ItemClass.SubService.None;
            raycastInput.m_netService.m_itemLayers = ItemClass.Layer.Default;
            raycastInput.m_ignoreNodeFlags = NetNode.Flags.None;
            raycastInput.m_ignoreSegmentFlags = NetSegment.Flags.None;
            raycastInput.m_ignoreTerrain = true;

            RaycastOutput output;
            if (RayCast(raycastInput, out output))
            {
                nodeId = output.m_netNode;

                if (nodeId == 0)
                {
                    NetManager netManager = NetManager.instance;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    NetSegment seg = netManager.m_segments.m_buffer[output.m_netSegment];

                    ushort[] segNodes = new ushort[] { seg.m_startNode, seg.m_endNode };
                    for (int i = 0; i < segNodes.Length; i++)
                    {
                        Bounds bounds = netManager.m_nodes.m_buffer[segNodes[i]].m_bounds;
                        if (bounds.IntersectRay(ray))
                        {
                            nodeId = segNodes[i];
                            break;
                        }
                    }
                }

                return nodeId != 0;
            }

            nodeId = 0;
            return false;
        }

        protected bool RaycastSegment(out ushort segmentId)
        {
            RaycastInput raycastInput = new RaycastInput(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.farClipPlane);
            raycastInput.m_netService.m_service = ItemClass.Service.Road;
            raycastInput.m_netService.m_subService = ItemClass.SubService.None;
            raycastInput.m_netService.m_itemLayers = ItemClass.Layer.Default;
            raycastInput.m_ignoreSegmentFlags = NetSegment.Flags.None;
            raycastInput.m_ignoreTerrain = true;

            RaycastOutput output;
            if (RayCast(raycastInput, out output))
            {
                segmentId = output.m_netSegment;

                return true;
            }

            segmentId = 0;
            return false;
        }

        protected bool RaycastBezier(Bezier3 bezier)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            float angle = Vector3.Angle(bezier.a, bezier.b);
            if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
            {
                angle = Vector3.Angle(bezier.b, bezier.c);
                if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
                {
                    angle = Vector3.Angle(bezier.c, bezier.d);
                    if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
                    {
                        // linear bezier
                        Bounds bounds = bezier.GetBounds();
                        bounds.Expand(0.5f);

                        return bounds.IntersectRay(mouseRay);
                    }
                }
            }

            // split bezier in 10 parts to correctly raycast curves
            const int amount = 10;
            const float size = 1f / amount;
            Bezier3 tempBezier;
            for (int i = 0; i < amount; i++)
            {
                tempBezier = bezier.Cut(i * size, (i + 1) * size);

                Bounds bounds = tempBezier.GetBounds();
                bounds.Expand(0.5f);

                if (bounds.IntersectRay(mouseRay))
                    return true;
            }

            return false;
        }
    }
}
