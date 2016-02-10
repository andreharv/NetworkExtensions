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
    }
}

