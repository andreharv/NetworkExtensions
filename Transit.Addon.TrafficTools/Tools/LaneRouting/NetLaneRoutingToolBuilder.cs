using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.TrafficTools.Common;
using Transit.Addon.TrafficTools.LaneRouting.Markers;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.TrafficTools.LaneRouting
{
    public class NetLaneRoutingToolBuilder : Activable, IToolBuilder
    {
        public int Order { get { return 10; } }
        public int UIOrder { get { return 10; } }

        public string Name { get { return "Intersection Routing"; } }
        public string DisplayName { get { return Name; } }
        public string Description { get { return "Allows you to customize entry and exit points in junctions."; } }
        public string UICategory { get { return "IntersectionEditors"; } }

        public string ThumbnailsPath { get { return @"Tools\LaneRouting\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Tools\LaneRouting\infotooltip.png"; } }

        public Type ToolType { get { return typeof (NetLaneRoutingTool); } }
    }

    public class NetLaneRoutingTool : NetNodeEditorToolBase<NodeRoutingMarker>
    {
        private NodeRoutingMarker _selectedMarker;

        protected override void OnMarkerClicked(NodeRoutingMarker marker, MouseKeyCode code)
        {
            switch (code)
            {
                case MouseKeyCode.LeftButton:
                    if (_selectedMarker != marker)
                    {
                        if (_selectedMarker != null)
                        {
                            _selectedMarker.UnSelect();
                        }
                    }

                    if (marker.IsEnabled)
                    {
                        _selectedMarker = marker;
                        _selectedMarker.OnClicked();
                    }
                    break;
                case MouseKeyCode.RightButton:
                    break;
            }
        }

        protected override void OnNonMarkerClicked(MouseKeyCode code)
        {
            if (_selectedMarker != null)
            {
                _selectedMarker.UnSelect();
                _selectedMarker = null;
            }
        }

        public override void RenderOverlay(RenderManager.CameraInfo camera)
        {
            base.RenderOverlay(camera);

            if (_selectedMarker != null)
            {
                _selectedMarker.OnRendered(camera);
            }
        }

        public static bool RayCastSegmentAndNode(out RaycastOutput output)
        {
            RaycastInput input = new RaycastInput(Camera.main.ScreenPointToRay(Input.mousePosition), Camera.main.farClipPlane);
            input.m_netService.m_service = ItemClass.Service.Road;
            input.m_netService.m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
            input.m_ignoreSegmentFlags = NetSegment.Flags.None;
            input.m_ignoreNodeFlags = NetNode.Flags.None;
            input.m_ignoreTerrain = true;

            return RayCast(input, out output);
        }

        public static bool RayCastSegmentAndNode(out ushort netSegment, out ushort netNode)
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
    }
}

