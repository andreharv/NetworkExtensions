using System;
using System.Collections.Generic;
using Transit.Addon.TrafficTools.Common;
using UnityEngine;

namespace Transit.Addon.TrafficTools.RoadEditor.LaneRestrictions
{
    public class LaneRestrictorTool : RoadEditorToolBase
    {
        enum SelectMode
        {
            Single,
            SameDirection,
            All
        }

        public event Action OnLanesSelected;
        public event Action OnLanesDeselected;

        ushort _hoveredSegment;
        HashSet<uint> _hoveredLanes; // These are hashSets to easily and efficiently avoid duplicates
        HashSet<uint> _selectedLanes;// Order is not important
        SelectMode _selectMode;

        public HashSet<uint> SelectedLanes
        {
            get { return _selectedLanes; }
        }

        protected override void CustomAwake()
        {
            _hoveredSegment = 0;
            _hoveredLanes = new HashSet<uint>();
            _selectedLanes = new HashSet<uint>();
            _mode = Mode.SelectLane;
            _selectMode = SelectMode.Single;
        }

        protected override void OnRenderLane(RenderManager.CameraInfo camera)
        {
            foreach (uint laneId in _selectedLanes)
                RenderManager.instance.OverlayEffect.DrawLaneSelection(camera, laneId, Color.green);

            foreach (uint laneId in _hoveredLanes)
                RenderManager.instance.OverlayEffect.DrawLaneSelection(camera, laneId, Color.blue);
        }
    }
}
