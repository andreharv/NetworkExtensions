using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.Math;
using ColossalFramework.UI;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Addon.TM.UI.Toolbar.RoadCustomizer;
using Transit.Addon.TM.UI.Toolbar.RoadCustomizer.Textures;
using Transit.Framework;
using Transit.Framework.Network;
using UnityEngine;

namespace Transit.Addon.TM.Tools.LaneRestriction
{
	public class LaneRestrictionTool : ToolBase
	{
		private class SegmentLaneMarker
		{
			public uint m_lane;
			public int m_laneIndex;
			public float m_size = 1f;
			public Bezier3 m_bezier;
			public Bounds[] m_bounds;

			public bool IntersectRay(Ray ray)
			{
				if (m_bounds == null)
					CalculateBounds();

				foreach (Bounds bounds in m_bounds)
				{
					if (bounds.IntersectRay(ray))
						return true;
				}

				return false;
			}

			void CalculateBounds()
			{
				float angle = Vector3.Angle(m_bezier.a, m_bezier.b);
				if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
				{
					angle = Vector3.Angle(m_bezier.b, m_bezier.c);
					if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
					{
						angle = Vector3.Angle(m_bezier.c, m_bezier.d);
						if (Mathf.Approximately(angle, 0f) || Mathf.Approximately(angle, 180f))
						{
							// linear bezier
							Bounds bounds = m_bezier.GetBounds();
							bounds.Expand(1f);
							m_bounds = new Bounds[] { bounds };
							return;
						}
					}
				}                
				
				// split bezier in 10 parts to correctly raycast curves
				Bezier3 bezier;
				int amount = 10;
				m_bounds = new Bounds[amount];
				float size = 1f / amount;
				for (int i = 0; i < amount; i++)
				{
					bezier = m_bezier.Cut(i * size, (i+1) * size);
					
					Bounds bounds = bezier.GetBounds();
					bounds.Expand(1f);
					m_bounds[i] = bounds;
				}
				
			}
		}

        private struct Segment
		{
			public ushort m_segmentId;
			public ushort m_targetNode;
		}

        private ushort m_hoveredSegment;
        private ushort m_hoveredNode;
        private readonly Dictionary<ushort, Segment> m_segments = new Dictionary<ushort, Segment>();
        private readonly Dictionary<int, FastList<SegmentLaneMarker>> m_hoveredLaneMarkers = new Dictionary<int, FastList<SegmentLaneMarker>>();
        private readonly List<SegmentLaneMarker> m_selectedLaneMarkers = new List<SegmentLaneMarker>();
        private int m_hoveredLanes;
        private UIButton m_toolButton;

        protected override void Awake()
        {
            base.Awake();
            
            StartCoroutine(CreateToolButton());
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

			if (m_hoveredSegment != 0)
			{
				HandleLaneCustomization();
			}

			if (!RayCastSegmentAndNode(out m_hoveredSegment, out m_hoveredNode))
			{
				// clear lanes
				if (Input.GetMouseButtonUp(1))
				{
					m_selectedLaneMarkers.Clear();
					if (OnEndLaneCustomization != null)
						OnEndLaneCustomization();
				}
					
				m_segments.Clear();
				m_hoveredLaneMarkers.Clear();
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
					if (bounds.IntersectRay(mouseRay))
					{
						m_hoveredSegment = 0;
					}
				}

				if (m_hoveredSegment != 0 && endNode.CountSegments() > 1)
				{
					Bounds bounds = endNode.m_bounds;
					if (bounds.IntersectRay(mouseRay))
					{
						m_hoveredSegment = 0;
					}
				}

				if (m_hoveredSegment != 0)
				{
					if (!m_segments.ContainsKey(m_hoveredSegment))
					{
						m_segments.Clear();
						SetSegments(m_hoveredSegment);
						SetLaneMarkers();
					}
				}
				else if (Input.GetMouseButtonUp(1))
				{
					// clear lane selection
					m_selectedLaneMarkers.Clear();
					if (OnEndLaneCustomization != null)
						OnEndLaneCustomization();
				}
						
			}

			if (m_hoveredSegment == 0)
			{
				m_segments.Clear();
				m_hoveredLaneMarkers.Clear();
			}
		}

        private void HandleLaneCustomization()
		{
			// Handle lane settings
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			m_hoveredLanes = ushort.MaxValue;
			foreach (FastList<SegmentLaneMarker> laneMarkers in m_hoveredLaneMarkers.Values)
			{
				if (laneMarkers.m_size == 0)
					continue;

				for (int i = 0; i < laneMarkers.m_size; i++)
				{
					SegmentLaneMarker marker = laneMarkers.m_buffer[i];
					if (NetManager.instance.m_lanes.m_buffer[marker.m_lane].m_segment != m_hoveredSegment)
						continue;

					if (marker.IntersectRay(mouseRay))
					{
						m_hoveredLanes = marker.m_laneIndex;
						break;
					}
				}

				if (m_hoveredLanes != ushort.MaxValue)
					break;
			}

			if (m_hoveredLanes != ushort.MaxValue && Input.GetMouseButtonUp(0))
			{
				SegmentLaneMarker[] hoveredMarkers = m_hoveredLaneMarkers[m_hoveredLanes].ToArray();
				HashSet<uint> hoveredLanes = new HashSet<uint>(hoveredMarkers.Select(m => m.m_lane));
				if (m_selectedLaneMarkers.RemoveAll(m => hoveredLanes.Contains(m.m_lane)) == 0)
				{
					bool firstLane = false;
					if (m_selectedLaneMarkers.Count == 0 && OnStartLaneCustomization != null)
						firstLane = true;

					m_selectedLaneMarkers.AddRange(hoveredMarkers);

					if (firstLane)
						OnStartLaneCustomization();
				}
				else if (m_selectedLaneMarkers.Count == 0 && OnEndLaneCustomization != null)
					OnEndLaneCustomization();
			}
			else if (Input.GetMouseButtonUp(1))
			{
				m_selectedLaneMarkers.Clear();
				if (OnEndLaneCustomization != null)
					OnEndLaneCustomization();
			}
		}

        private float time = 0;
		protected override void OnEnable()
		{
			base.OnEnable();

			// hack to stop bug that disables and enables this tool the first time the panel is clicked
			if (Time.realtimeSinceStartup - time < 0.2f)
			{
				time = 0;
				return;
			}

			m_hoveredSegment = 0;
			m_selectedLaneMarkers.Clear();
			m_segments.Clear();
			m_hoveredLaneMarkers.Clear();
			if (OnEndLaneCustomization != null)
				OnEndLaneCustomization();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			time = Time.realtimeSinceStartup;
			//m_selectedLaneMarkers.Clear();
			//if (OnEndLaneCustomization != null)
			//	OnEndLaneCustomization();
		}

        private void SetLaneMarkers()
		{
			m_hoveredLaneMarkers.Clear();
			if (m_segments.Count == 0)
				return;

			NetSegment segment = NetManager.instance.m_segments.m_buffer[m_segments.Values.First().m_segmentId];
			NetInfo info = segment.Info;
			int laneCount = info.m_lanes.Length;
			bool bothWays = info.m_hasBackwardVehicleLanes && info.m_hasForwardVehicleLanes;
			bool isInverted = false;

			for (ushort i = 0; i < laneCount; i++)
				m_hoveredLaneMarkers[i] = new FastList<SegmentLaneMarker>();

			foreach (Segment seg in m_segments.Values)
			{
				segment = NetManager.instance.m_segments.m_buffer[seg.m_segmentId];
				uint laneId = segment.m_lanes;

				if (bothWays)
				{
					isInverted = seg.m_targetNode == segment.m_startNode;
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

						m_hoveredLaneMarkers[index].Add(new SegmentLaneMarker()
						{
							m_bezier = bezier,
							m_lane = laneId,
							m_laneIndex = index
						});
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
				m_segmentId = segmentId,
				m_targetNode = segment.m_endNode
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
			seg.m_segmentId = segmentId;

			NetSegment previousSegment = NetManager.instance.m_segments.m_buffer[previousSeg.m_segmentId];
			ushort nextNode;
			if ((segment.m_startNode == previousSegment.m_endNode) || (segment.m_startNode == previousSegment.m_startNode))
			{
				nextNode = segment.m_endNode;
				seg.m_targetNode = segment.m_startNode == previousSeg.m_targetNode ? segment.m_endNode : segment.m_startNode;
			}            
			else
			{
				nextNode = segment.m_startNode;
				seg.m_targetNode = segment.m_endNode == previousSeg.m_targetNode ? segment.m_startNode : segment.m_endNode;
			}    

			m_segments[segmentId] = seg;

			NetNode node = NetManager.instance.m_nodes.m_buffer[nextNode];
			if (node.CountSegments() == 2)
				SetSegments(node.m_segment0 == segmentId ? node.m_segment1 : node.m_segment0, infoIndex, ref seg);
		}

		public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
		{
			base.RenderOverlay(cameraInfo);
            
			foreach (KeyValuePair<int, FastList<SegmentLaneMarker>> keyValuePair in m_hoveredLaneMarkers)
			{
				bool renderBig = false;
				if (m_hoveredLanes == keyValuePair.Key)
					renderBig = true;

				FastList<SegmentLaneMarker> laneMarkers = keyValuePair.Value;
				for (int i = 0; i < laneMarkers.m_size; i++)
				{
					RenderManager.instance.OverlayEffect.DrawBezier(cameraInfo, new Color(0f, 0f, 1f, 0.75f), laneMarkers.m_buffer[i].m_bezier, renderBig ? 2f : laneMarkers.m_buffer[i].m_size, 0, 0, Mathf.Min(laneMarkers.m_buffer[i].m_bezier.a.y, laneMarkers.m_buffer[i].m_bezier.d.y) - 1f, Mathf.Max(laneMarkers.m_buffer[i].m_bezier.a.y, laneMarkers.m_buffer[i].m_bezier.d.y) + 1f, true, false);
				}
			}

			foreach (SegmentLaneMarker marker in m_selectedLaneMarkers)
			{
				RenderManager.instance.OverlayEffect.DrawBezier(cameraInfo, new Color(0f, 1f, 0f, 0.75f), marker.m_bezier, 2f, 0, 0, Mathf.Min(marker.m_bezier.a.y, marker.m_bezier.d.y) - 1f, Mathf.Max(marker.m_bezier.a.y, marker.m_bezier.d.y) + 1f, true, false);
			}
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

		#region Road Customizer

		public event System.Action OnStartLaneCustomization;
		public event System.Action OnEndLaneCustomization;

        private bool AnyLaneSelected { get { return m_selectedLaneMarkers.Count > 0; } }

		public ExtendedUnitType GetCurrentVehicleRestrictions()
		{
			if (!AnyLaneSelected)
				return ExtendedUnitType.None;

            return TAMRestrictionManager.instance.GetRestrictions(m_selectedLaneMarkers[0].m_lane, ExtendedUnitType.RoadVehicle);
		}

		public ExtendedUnitType ToggleRestriction(ExtendedUnitType vehicleType)
		{
			if (!AnyLaneSelected)
				return ExtendedUnitType.None;

			var restrictions = TAMRestrictionManager.instance.GetRestrictions(m_selectedLaneMarkers[0].m_lane, ExtendedUnitType.RoadVehicle);

            restrictions ^= vehicleType;

			foreach (SegmentLaneMarker lane in m_selectedLaneMarkers)
                TAMRestrictionManager.instance.SetRestrictions(lane.m_lane, restrictions);

			return restrictions;
		}

		#endregion

		#region UI

		public bool InitializeUI(UIButton button)
		{
			GameObject container = GameObject.Find("TSContainer");
			if (container == null)
				return false;

			GameObject panel = UITemplateManager.GetAsGameObject("ScrollableSubPanelTemplate");
			if (panel == null)
				return false;

			container.GetComponent<UIComponent>().AttachUIComponent(panel);
			panel.GetComponent<UIPanel>().relativePosition = Vector3.zero;
			panel.GetComponent<UIPanel>().isVisible = false;
			GameObject gtsContainer = panel.transform.GetChild(0).gameObject;
			panel.GetComponent<UIPanel>().AttachUIComponent(gtsContainer);
			GameObject groupToolstrip = panel.transform.GetChild(1).gameObject;
			panel.GetComponent<UIPanel>().AttachUIComponent(groupToolstrip);

			GameObject vehiclePanelObj = UITemplateManager.GetAsGameObject("ScrollablePanelTemplate");
			if (vehiclePanelObj == null)
				return false;

			UIComponent comp = gtsContainer.GetComponent<UIComponent>();
			if (comp == null)
				return false;
			comp.AttachUIComponent(vehiclePanelObj);
			comp.relativePosition = Vector3.zero;
			vehiclePanelObj.GetComponent<UIPanel>().AttachUIComponent(vehiclePanelObj.transform.GetChild(0).gameObject);
			vehiclePanelObj.GetComponent<UIPanel>().relativePosition = Vector3.zero;
			vehiclePanelObj.GetComponent<UIPanel>().isVisible = true;
			vehiclePanelObj.GetComponent<UIPanel>().isInteractive = true;
			vehiclePanelObj.transform.GetChild(0).gameObject.GetComponent<UIComponent>().relativePosition = new Vector3(50f, 0f);

			GameObject speedPanelObj = UITemplateManager.GetAsGameObject("ScrollablePanelTemplate");
			if (speedPanelObj == null)
				return false;
			comp.AttachUIComponent(speedPanelObj);
			speedPanelObj.GetComponent<UIPanel>().AttachUIComponent(speedPanelObj.transform.GetChild(0).gameObject);
			speedPanelObj.GetComponent<UIPanel>().relativePosition = Vector3.zero;
			speedPanelObj.GetComponent<UIPanel>().isInteractive = true;
			speedPanelObj.transform.GetChild(0).gameObject.GetComponent<UIComponent>().relativePosition = new Vector3(50f, 0f);

			// add RoadCustomizerGroupPanel to panel
			panel.AddComponent<RoadCustomizerGroupPanel>();

			// add RoadCustomizerPanel to scrollablePanel
		    vehiclePanelObj
		        .AddComponent<RoadCustomizerPanel>()
		        .AttachLaneCustomizationEvents(this);

            speedPanelObj
                .AddComponent<RoadCustomizerPanel>()
                .AttachLaneCustomizationEvents(this);

            button.eventClick += delegate(UIComponent component, UIMouseEventParameter eventParam)
			{
				//roadsPanel.isVisible = false;
				panel.SetActive(true);
				panel.GetComponent<UIPanel>().isVisible = true;
				//vehiclePanel.GetComponent<UIPanel>().isVisible = true;
			};

			return true;
		}

        private IEnumerator CreateToolButton()
		{
			while (m_toolButton == null)
			{
				m_toolButton = TryCreateToolButton();
				yield return new WaitForEndOfFrame();
			}
		}

        private UIButton TryCreateToolButton()
		{
			//GameObject roadsOptionPanel = GameObject.Find("RoadsOptionPanel(RoadsPanel)");
			//if (roadsOptionPanel == null)
			//    return null;

			//UITabstrip tabstrip = roadsOptionPanel.GetComponentInChildren<UITabstrip>();
			//if (tabstrip == null)
			//    return null;

			GameObject mainToolStrip = GameObject.Find("MainToolstrip");
			if (mainToolStrip == null)
				return null;

			UITabstrip tabstrip = mainToolStrip.GetComponent<UITabstrip>();
			if (tabstrip == null)
				return null;
            if (tabstrip.tabs == null)
                return null;
            if (!tabstrip.tabs.Any())
                return null;

            UIButton roadsButton = (UIButton)tabstrip.tabs.First();

			UIButton btn = mainToolStrip.GetComponent<UIComponent>().AddUIComponent<UIButton>();
			
			btn.name = "RoadCustomizer";
			btn.text = "";
			btn.tooltip = "Road Customizer Tool";
			btn.size = roadsButton.size;
			btn.playAudioEvents = true;

			btn.disabledBgSprite = "rctBg";// roadsButton.disabledBgSprite;
			btn.focusedBgSprite = "rctBg" + "Focused";// roadsButton.focusedBgSprite;
			btn.hoveredBgSprite = "rctBg" + "Hovered";// roadsButton.hoveredBgSprite;
			btn.normalBgSprite = "rctBg";// roadsButton.normalBgSprite;
			btn.pressedBgSprite = "rctBg" + "Pressed";// roadsButton.pressedBgSprite;

			btn.atlas = AtlasManager.instance.GetAtlas(RoadCustomizerAtlasBuilder.ID);
			btn.atlas.AddSprites(roadsButton.atlas.sprites);
			btn.foregroundSpriteMode = UIForegroundSpriteMode.Fill;

			btn.disabledFgSprite = "rct";
			btn.focusedFgSprite = "rct";
			btn.hoveredFgSprite = "rct";
			btn.normalFgSprite = "rct";
			btn.pressedFgSprite = "rct";
			btn.group = roadsButton.group;

			//btn.eventClick += delegate(UIComponent component, UIMouseEventParameter eventParam)
			//{
			//	//ToolsModifierControl.SetTool<RoadCustomizerTool>();
			//	//StartCoroutine(SetRoadCustomizerTool());
			//	ToolsModifierControl.SetTool<RoadCustomizerTool>();
			//};

			btn.eventButtonStateChanged += delegate(UIComponent component, UIButton.ButtonState value)
			{
				if (value == UIButton.ButtonState.Focused)
				{
					if (ToolsModifierControl.GetCurrentTool<DefaultTool>() != null)
						ToolsModifierControl.SetTool<LaneRestrictionTool>();
					else
						StartCoroutine(SetRoadCustomizerTool());
				}
				else if (value == UIButton.ButtonState.Normal)
				{
					//if (ToolsModifierControl.GetCurrentTool<RoadCustomizerTool>() != null)
					ToolsModifierControl.SetTool<DefaultTool>();
				}
			};

			InitializeUI(btn);

			return btn;
		}

        private IEnumerator SetRoadCustomizerTool()
		{
			ToolsModifierControl.SetTool<LaneRestrictionTool>();   

			while (ToolsModifierControl.GetCurrentTool<LaneRestrictionTool>() != null)
				yield return new WaitForEndOfFrame();

			ToolsModifierControl.SetTool<LaneRestrictionTool>();
		}

        #endregion
	}
}
