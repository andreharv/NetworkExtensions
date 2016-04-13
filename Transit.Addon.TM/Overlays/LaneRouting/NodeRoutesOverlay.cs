using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Transit.Addon.TM.Overlays.LaneRouting.Markers;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Framework.UI;
using UnityEngine;

namespace Transit.Addon.TM.Overlays.LaneRouting
{
    public class NodeRoutesOverlay : Singleton<NodeRoutesOverlay>
    {
        private NodeRoutesMarker _selectedNodeMarker;
        private Dictionary<ushort, NodeRoutesMarker> _nodeMarkers;

        public void LoadMarkers()
        { 
            _nodeMarkers = new Dictionary<ushort, NodeRoutesMarker>();
            var nodesList = new HashSet<ushort>();
            foreach (var route in TAMLaneRoutingManager.instance.GetAllRoutes())
            {
                if (route == null)
                    continue;

                if (route.Connections.Any())
                    nodesList.Add(route.NodeId);
            }

            foreach (var nodeId in nodesList)
            {
                _nodeMarkers[nodeId] = new NodeRoutesMarker(nodeId);
            }
        }

        public void Reset()
        {
            _selectedNodeMarker = null;
            _nodeMarkers = null;
        }

        public bool IsLoaded()
        {
            return _nodeMarkers != null;
        }

        public NodeRoutesMarker GetMarker(ushort nodeId)
        {
            if (_nodeMarkers.ContainsKey(nodeId))
            {
                return _nodeMarkers[nodeId];
            }

            return null;
        }

        public NodeRoutesMarker GetOrCreateMarker(ushort nodeId)
        {
            if (!_nodeMarkers.ContainsKey(nodeId))
            {
                _nodeMarkers[nodeId] = new NodeRoutesMarker(nodeId);
            }

            return _nodeMarkers[nodeId];
        }

        public void SelectMarker(ushort nodeId)
        {
            _selectedNodeMarker = GetOrCreateMarker(nodeId);
            _selectedNodeMarker.Select();
        }

        public void UnselectMarker(ushort nodeId)
        {
            if (_nodeMarkers.ContainsKey(nodeId))
            {
                _nodeMarkers[nodeId].Unselect();
            }

            UnselectCurrentMarker();
        }

        public void UnselectCurrentMarker()
        {
            if (_selectedNodeMarker != null)
            {
                _selectedNodeMarker.Unselect();
                _selectedNodeMarker = null;
            }
        }

        public void UpdateMarker(ushort nodeId, InputEvent inputEvent)
        {
            if (_nodeMarkers.ContainsKey(nodeId))
            {
                _nodeMarkers[nodeId].Update(inputEvent.MouseRay);
            }
        }

        public void Update(InputEvent inputEvent)
        {
            if (inputEvent.KeyCode != null)
            {
                if (inputEvent.KeyCode == KeyCode.PageDown)
                {
                    InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.Traffic, InfoManager.SubInfoMode.Default);
                } 
                else if (inputEvent.KeyCode == KeyCode.PageUp)
                {
                    InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
                }
            }
        }

        public void Render(RenderManager.CameraInfo cameraInfo)
        {
            if (_selectedNodeMarker != null)
            {
                _selectedNodeMarker.Render(cameraInfo);
            }
            else
            {
                foreach (var kvp in _nodeMarkers)
                {
                    kvp.Value.Render(cameraInfo);
                }
            }
        }
    }
}