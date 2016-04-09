using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Transit.Addon.TM.Overlays.LaneRouting.Markers;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Framework;
using Transit.Framework.UI;
using UnityEngine;

namespace Transit.Addon.TM.Overlays.LaneRouting
{
    public class NodeRoutesOverlay : Singleton<NodeRoutesOverlay>
    {
        private ushort? _hoveredNodeId;
        private NodeRoutesMarker _selectedNodeMarker;
        private Dictionary<ushort, NodeRoutesMarker> _nodeMarkers = new Dictionary<ushort, NodeRoutesMarker>();

        public IEnumerator LoadMarkers()
        {
            while (!TAMLaneRoutingManager.instance.IsLoaded())
            {
                yield return new WaitForEndOfFrame();
            }

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

            _hoveredNodeId = ExtendedToolBase.RayCastNodeWithMoreThanOneSegment();
            var selectedNodeId = _selectedNodeMarker == null ? (ushort?)null : _selectedNodeMarker.NodeId;


            // -------------------------------------------------
            // Mouse moves
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (_hoveredNodeId != null)
            {
                if (_nodeMarkers.ContainsKey(_hoveredNodeId.Value))
                {
                    _nodeMarkers[_hoveredNodeId.Value].Update(mouseRay);
                }
            }

            if (_selectedNodeMarker != null &&
                _hoveredNodeId != selectedNodeId)
            {
                _selectedNodeMarker.Update(mouseRay);
            }


            // -------------------------------------------------
            // Mouse clicks
            if (inputEvent.MouseKeyCode != null)
            {
                // Left clicks
                if (inputEvent.MouseKeyCode == MouseKeyCode.LeftButton)
                {
                    Log.Info(">>>>>> LeftButton");
                    if (_hoveredNodeId != null && _hoveredNodeId != selectedNodeId)
                    {
                        if (_selectedNodeMarker != null)
                        {
                            _selectedNodeMarker.Unselect();
                            _selectedNodeMarker = null;
                        }

                        if (!_nodeMarkers.ContainsKey(_hoveredNodeId.Value))
                        {
                            _nodeMarkers[_hoveredNodeId.Value] = new NodeRoutesMarker(_hoveredNodeId.Value);
                        }

                        var node = _nodeMarkers[_hoveredNodeId.Value];

                        _selectedNodeMarker = node;
                        _selectedNodeMarker.Select();
                        Log.Info(">>>>>> SelectingNodeAnchor");
                        return;
                    }

                    if (_selectedNodeMarker != null)
                    {
                        if (_selectedNodeMarker.LeftClick())
                        {
                            return;
                        }
                    }
                }

                
                // Right clicks
                if (inputEvent.MouseKeyCode == MouseKeyCode.RightButton)
                {
                    Log.Info(">>>>>> RightButton");
                    if (_selectedNodeMarker != null)
                    {
                        if (_selectedNodeMarker.RightClick())
                        {
                            Log.Info(">>>>>> RightClick Handeled");
                            return;
                        }
                    }

                    if (_selectedNodeMarker != null)
                    {
                        Log.Info(">>>>>> UnselectingCurrentAnchor");
                        _selectedNodeMarker.Unselect();
                        _selectedNodeMarker = null;
                        return;
                    }
                }
            }
        }

        public void Enable()
        {
            UnselectEverything();
        }

        public void Disable()
        {
            UnselectEverything();
        }

        private void UnselectEverything()
        {
            _hoveredNodeId = null;

            if (_selectedNodeMarker != null)
            {
                _selectedNodeMarker.Unselect();
                _selectedNodeMarker = null;
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

            var selectedNodeId = _selectedNodeMarker == null ? (ushort?)null : _selectedNodeMarker.NodeId;
            if (_hoveredNodeId != null && _hoveredNodeId != selectedNodeId)
            {
                NetNode node = NetManager.instance.m_nodes.m_buffer[_hoveredNodeId.Value];
                RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, new Color(0f, 0f, 0.5f, 0.75f), node.m_position, 15f, node.m_position.y - 1f, node.m_position.y + 1f, true, true);
            }
        }
    }
}