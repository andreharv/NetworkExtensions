using System;
using System.Linq;
using Transit.Framework;
using Transit.Framework.Modularity;
using Transit.Addon.RoadExtensions.Menus;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads.LargeAvenue8L
{
    public class LargeAvenue8LBuilder : NetInfoBuilderBase, INetInfoBuilder
    {
        public int Order { get { return 50; } }
        public int Priority { get { return 30; } }

        public string TemplatePrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "XLarge Avenue"; } }
        public string DisplayName { get { return "Eight-Lane Road"; } }
        public string CodeName { get { return "LARGEAVENUE_8L"; } }
		public string Description { get { return "An eight-lane road without parking spaces. Supports very heavy traffic."; } }
        public string UICategory { get { return "RoadsLarge"; } }
        
        public string ThumbnailsPath    { get { return @"Roads\LargeAvenue8L\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"Roads\LargeAvenue8L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

		public void BuildUp(NetInfo info, NetInfoVersion version)
		{
			///////////////////////////
			// Template              //
			///////////////////////////
			var largeRoadInfo = Prefabs.Find<NetInfo> (NetInfos.Vanilla.ROAD_6L);


			///////////////////////////
			// Texturing             //
			///////////////////////////
			switch (version) {
			case NetInfoVersion.Ground:
				info.SetAllSegmentsTexture (
					new TexturesSet (
						@"Roads\LargeAvenue8L\Textures\Ground_Segment__MainTex.png",
						@"Roads\LargeAvenue8L\Textures\Ground_Segment__AlphaMap.png"));
				break;
			}


			///////////////////////////
			// Set up                //
			///////////////////////////
			info.m_class = largeRoadInfo.m_class.Clone(NetInfoClasses.NEXT_LARGE_ROAD);
			info.m_UnlockMilestone = largeRoadInfo.m_UnlockMilestone;
			info.m_hasParkingSpaces = false;

			//Setting up Lanes
			var vehicleLaneTypes = new[]
			{
				NetInfo.LaneType.Vehicle,
				NetInfo.LaneType.PublicTransport,
				NetInfo.LaneType.CargoVehicle,
				NetInfo.LaneType.TransportVehicle
			};

			var vehicleLanes = info.m_lanes
				.Where(l =>
					l.m_laneType.HasFlag(NetInfo.LaneType.Parking) ||
					vehicleLaneTypes.Contains(l.m_laneType))
				.OrderBy(l => l.m_position)
				.ToArray();

			for (int i = 0; i < vehicleLanes.Length; i++)
			{
				var lane = vehicleLanes [i];

				if (lane.m_laneType.HasFlag (NetInfo.LaneType.Parking))
				{
					int closestVehicleLaneId;

					if (i - 1 >= 0 && vehicleLaneTypes.Contains (vehicleLanes [i - 1].m_laneType))
					{
						closestVehicleLaneId = i - 1;
					}
					else if (i + 1 < vehicleLanes.Length && vehicleLaneTypes.Contains (vehicleLanes [i + 1].m_laneType))
					{
						closestVehicleLaneId = i + 1;
					}
					else
					{
						continue; // Not supposed to happen
					}

					var closestVehicleLane = vehicleLanes [closestVehicleLaneId];

					SetLane(lane, closestVehicleLane);
				}
			}
			if (version == NetInfoVersion.Ground)
			{
				var brPlayerNetAI = largeRoadInfo.GetComponent<PlayerNetAI>();
				var playerNetAI = info.GetComponent<PlayerNetAI>();

				if (brPlayerNetAI != null && playerNetAI != null)
				{
					playerNetAI.m_constructionCost = brPlayerNetAI.m_constructionCost * 125 / 100; // 25% increase
					playerNetAI.m_maintenanceCost = brPlayerNetAI.m_maintenanceCost * 125 / 100; // 25% increase
				}

			}
			else // Same as the original large road specs
			{

			}
		}

		private static void SetLane(NetInfo.Lane newLane, NetInfo.Lane closestLane)
		{
			newLane.m_direction = closestLane.m_direction;
			newLane.m_finalDirection = closestLane.m_finalDirection;
			newLane.m_allowConnect = closestLane.m_allowConnect;
			newLane.m_allowStop = closestLane.m_allowStop;
			if (closestLane.m_allowStop)
			{
				closestLane.m_allowStop = false;
				closestLane.m_stopOffset = 0;
			}
			if (newLane.m_allowStop)
			{
				if (newLane.m_position < 0)
				{
					newLane.m_stopOffset = -0.3f;
				}
				else
				{
					newLane.m_stopOffset = 0.3f;
				}
			}

			newLane.m_laneType = closestLane.m_laneType;
			newLane.m_similarLaneCount = closestLane.m_similarLaneCount = closestLane.m_similarLaneCount + 1;
			newLane.m_similarLaneIndex = closestLane.m_similarLaneIndex + 1;
			newLane.m_speedLimit = closestLane.m_speedLimit;
			newLane.m_vehicleType = closestLane.m_vehicleType;
			newLane.m_verticalOffset = closestLane.m_verticalOffset;
			newLane.m_width = closestLane.m_width;

			NetLaneProps templateLaneProps;
			if (closestLane.m_laneProps != null)
			{
				templateLaneProps = closestLane.m_laneProps;
			}
			else
			{
				templateLaneProps = ScriptableObject.CreateInstance<NetLaneProps>();
			}

			if (templateLaneProps.m_props == null)
			{
				templateLaneProps.m_props = new NetLaneProps.Prop[0];
			}

			if (newLane.m_laneProps == null)
			{
				newLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
			}

			newLane.m_laneProps.m_props = templateLaneProps
				.m_props
				.Select(p => p.ShallowClone())
				.ToArray();
		}
    }
}
