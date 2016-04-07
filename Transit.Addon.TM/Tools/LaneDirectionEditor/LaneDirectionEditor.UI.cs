using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using Transit.Addon.TM.AI;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Addon.TM.Traffic;
using Transit.Addon.TM.UI;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.TM.Tools.LaneDirectionEditor
{
    public partial class LaneDirectionEditor
    {
        protected override void OnToolGUI(Event e)
        {
            //Log._Debug($"OnToolGUI");

            try
            {
                var guiColor = GUI.color;
                guiColor.a = 0.9f;
                GUI.color = guiColor;

                DrawEditPanel(e);
            }
            catch (Exception ex)
            {
                Log.Error("GUI Error: " + ex.ToString());
            }
        }

        private void DrawEditPanel(Event e)
        {
            _cursorInSecondaryPanel = false;

            if (_selectedNodeId == 0 || _selectedSegmentId == 0) return;
            
            int numLanes = NetManager.instance.GetInboundLanesAtNode(_selectedSegmentId, _selectedNodeId).Count();
            if (numLanes <= 0)
            {
                _selectedNodeId = 0;
                _selectedSegmentId = 0;
                return;
            }

            var style = new GUIStyle
            {
                normal = { background = _secondPanelTexture },
                alignment = TextAnchor.MiddleCenter,
                border =
                {
                    bottom = 2,
                    top = 2,
                    right = 2,
                    left = 2
                }
            };

            Vector3 nodePos = Singleton<NetManager>.instance.m_nodes.m_buffer[_selectedNodeId].m_position;
            var screenPos = Camera.main.WorldToScreenPoint(nodePos);
            screenPos.y = Screen.height - screenPos.y;
            //Log._Debug($"node pos of {SelectedNodeId}: {nodePos.ToString()} {screenPos.ToString()}");
            if (screenPos.z < 0)
                return;
            var camPos = Singleton<SimulationManager>.instance.m_simulationView.m_position;
            var diff = nodePos - camPos;

            if (diff.magnitude > TrafficManagerTool.PriorityCloseLod)
                return; // do not draw if too distant

            int width = Math.Max(3 * 128 + 20, numLanes * 128);
            var windowRect3 = new Rect(screenPos.x - width / 2, screenPos.y - 70, width, 130);
            GUILayout.Window(250, windowRect3, DrawEditPanelInternal, "", style);
            _cursorInSecondaryPanel = windowRect3.Contains(Event.current.mousePosition);
        }

        private void DrawEditPanelInternal(int num)
        {
            var info = Singleton<NetManager>.instance.m_segments.m_buffer[_selectedSegmentId].Info;

            List<object[]> laneList = TrafficManagerTool.GetSortedVehicleLanes(_selectedSegmentId, info, _selectedNodeId);
            SegmentGeometry geometry = CustomRoadAI.GetSegmentGeometry(_selectedSegmentId);

            GUILayout.BeginHorizontal();

            for (var i = 0; i < laneList.Count; i++)
            {
                var flags = (NetLane.Flags)Singleton<NetManager>.instance.m_lanes.m_buffer[(uint)laneList[i][0]].m_flags;

                var style1 = new GUIStyle("button");
                var style2 = new GUIStyle("button")
                {
                    normal = { textColor = new Color32(255, 0, 0, 255) },
                    hover = { textColor = new Color32(255, 0, 0, 255) },
                    focused = { textColor = new Color32(255, 0, 0, 255) }
                };

                var laneStyle = new GUIStyle { contentOffset = new Vector2(12f, 0f) };

                var laneTitleStyle = new GUIStyle
                {
                    contentOffset = new Vector2(36f, 2f),
                    normal = { textColor = new Color(1f, 1f, 1f) }
                };

                GUILayout.BeginVertical(laneStyle);
                GUILayout.Label(Translation.GetString("Lane") + " " + (i + 1), laneTitleStyle);
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                //if (!TAMLaneRoutingManager.instance.ApplyLaneDirection((uint)laneList[i][0])) {
                //                TAMLaneRoutingManager.instance.RemoveLaneDirection((uint)laneList[i][0]);
                //}
                if (GUILayout.Button("←", ((flags & NetLane.Flags.Left) == NetLane.Flags.Left ? style1 : style2), GUILayout.Width(35), GUILayout.Height(25)))
                {
                    if (!TAMLaneRoutingManager.instance.ToggleLaneDirection((uint)laneList[i][0], TAMLaneDirection.Left) && _selectedNodeId > 0)
                        ShowTooltip(Translation.GetString("Lane_Arrow_Changer_Disabled"), Singleton<NetManager>.instance.m_nodes.m_buffer[_selectedNodeId].m_position);
                }
                if (GUILayout.Button("↑", ((flags & NetLane.Flags.Forward) == NetLane.Flags.Forward ? style1 : style2), GUILayout.Width(25), GUILayout.Height(35)))
                {
                    if (!TAMLaneRoutingManager.instance.ToggleLaneDirection((uint)laneList[i][0], TAMLaneDirection.Forward) && _selectedNodeId > 0)
                        ShowTooltip(Translation.GetString("Lane_Arrow_Changer_Disabled"), Singleton<NetManager>.instance.m_nodes.m_buffer[_selectedNodeId].m_position);
                }
                if (GUILayout.Button("→", ((flags & NetLane.Flags.Right) == NetLane.Flags.Right ? style1 : style2), GUILayout.Width(35), GUILayout.Height(25)))
                {
                    if (!TAMLaneRoutingManager.instance.ToggleLaneDirection((uint)laneList[i][0], TAMLaneDirection.Right) && _selectedNodeId > 0)
                        ShowTooltip(Translation.GetString("Lane_Arrow_Changer_Disabled"), Singleton<NetManager>.instance.m_nodes.m_buffer[_selectedNodeId].m_position);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();

            bool startNode = Singleton<NetManager>.instance.m_segments.m_buffer[_selectedSegmentId].m_startNode == _selectedNodeId;

            if (!geometry.AreHighwayRulesEnabled(startNode))
            {
                if (!geometry.IsOneWay())
                {
                    Flags.setUTurnAllowed(_selectedSegmentId, startNode, GUILayout.Toggle(Flags.getUTurnAllowed(_selectedSegmentId, startNode), Translation.GetString("Allow_u-turns") + " (BETA feature)", new GUILayoutOption[] { }));
                }
                if (geometry.HasOutgoingStraightSegment(_selectedNodeId))
                {
                    Flags.setStraightLaneChangingAllowed(_selectedSegmentId, startNode, GUILayout.Toggle(Flags.getStraightLaneChangingAllowed(_selectedSegmentId, startNode), Translation.GetString("Allow_lane_changing_for_vehicles_going_straight"), new GUILayoutOption[] { }));
                }
                Flags.setEnterWhenBlockedAllowed(_selectedSegmentId, startNode, GUILayout.Toggle(Flags.getEnterWhenBlockedAllowed(_selectedSegmentId, startNode), Translation.GetString("Allow_vehicles_to_enter_a_blocked_junction"), new GUILayoutOption[] { }));
            }

            GUILayout.EndVertical();
        }

        private void ShowTooltip(string text, Vector3 position)
        {
            if (text == null)
                return;

            UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Info", text, false);

            /*tooltipStartFrame = currentFrame;
			tooltipText = text;
			tooltipWorldPos = position;*/
        }
    }
}
