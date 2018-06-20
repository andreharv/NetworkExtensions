using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.TinyRoads.AlleyCulDeSac
{
	public class AlleyCulDeSacBuilder : Activable, INetInfoBuilderPart
	{
		public const string NAME = "Tiny Cul-De-Sac";

		public int Order { get { return 1; } }
		public int UIOrder { get { return 10; } }

		public string BasedPrefabName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
		public string Name { get { return NAME; } }
		public string DisplayName { get { return NAME; } }
		public string CodeName { get { return "AlleyCulDeSac"; } }
		public string Description { get { return "A one-lane, oneway road suitable for neighborhood traffic."; } }
		public string ShortDescription { get { return "No parking, zoneable, neighborhood traffic"; } }
		public string UICategory { get { return RExExtendedMenus.ROADS_TINY; } }

		public string ThumbnailsPath { get { return @"Roads\TinyRoads\AlleyCulDeSac\thumbnails.png"; } }
		public string InfoTooltipPath { get { return @"Roads\TinyRoads\AlleyCulDeSac\infotooltip.png"; } }

		public NetInfoVersion SupportedVersions
		{
			get { return NetInfoVersion.Ground; }
		}

		public void BuildUp(NetInfo info, NetInfoVersion version)
		{
			///////////////////////////
			// Template              //
			///////////////////////////
			var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L);

			///////////////////////////
			// 3DModeling            //
			///////////////////////////
			info.Setup8m1p5mSW1SMesh(version, LanesLayoutStyle.AsymL1R2);

			///////////////////////////
			// Texturing             //
			///////////////////////////
			switch (version)
			{
				case NetInfoVersion.Ground:
					info.SetAllSegmentsTexture(
						new TextureSet
						   (@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment__MainTex.png",
							@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment__APRMap.png"),
						new LODTextureSet
						   (@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment_LOD__MainTex.png",
							@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment_LOD__APRMap.png",
							@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_LOD__XYSMap.png"));

					info.SetAllNodesTexture(
						new TextureSet
						   (@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment__MainTex.png",
							@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment__APRMap.png"),
						new LODTextureSet
						   (@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment_LOD__MainTex.png",
							@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_Segment_LOD__APRMap.png",
							@"Roads\TinyRoads\AlleyCulDeSac\Textures\Ground_LOD__XYSMap.png"));
					break;
			}
            info.m_availableIn = ItemClass.Availability.None;
			info.m_hasParkingSpaces = true;
			info.m_connectGroup = NetInfo.ConnectGroup.SingleTrain;
			info.m_nodeConnectGroups = NetInfo.ConnectGroup.SingleTrain;
			info.m_halfWidth = 4f;
			info.m_pavementWidth = 1.5f;
			info.m_surfaceLevel = 0;
			info.m_flatJunctions = true;
			info.m_flattenTerrain = true;
			info.m_class = roadInfo.m_class.Clone("NExt1LOnewayWithParking");
			var parkLane = info.m_lanes.Last(l => l.m_laneType == NetInfo.LaneType.Parking).CloneWithoutStops();
			info.m_class.m_level = (ItemClass.Level)5; //New level
			parkLane.m_width = 2f;
			info.m_lanes = info.m_lanes
				.Where(l => l.m_laneType != NetInfo.LaneType.Parking)
				.ToArray();

			var laneWidth = 3f;
			info.SetRoadLanes(version, new LanesConfiguration
			{
				IsTwoWay = false,
				LaneWidth = laneWidth,
				LanesToAdd = -1,
				SpeedLimit = 0.6f,
				BusStopOffset = 0f,
				PedLaneOffset = -0.75f,
				PedPropOffsetX = 2.25f,
				LanePositionOffst = -2
			});
			info.SetupNewSpeedLimitProps(30, 40);
			info.m_lanes.First(l => l.m_laneType == NetInfo.LaneType.Vehicle).m_position = (info.m_halfWidth - info.m_pavementWidth - (0.5f * laneWidth)) * -1;
			parkLane.m_position = (info.m_halfWidth - info.m_pavementWidth - (0.5f * parkLane.m_width));
			var tempLanes = new List<NetInfo.Lane>();
			tempLanes.AddRange(info.m_lanes);
			tempLanes.Add(parkLane);
			info.m_lanes = tempLanes.ToArray();

			var pedLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ToList();
			var roadLanes = info.m_lanes.Where(l => l.m_laneType != NetInfo.LaneType.Pedestrian && l.m_laneType != NetInfo.LaneType.None);

			for (int i = 0; i < pedLanes.Count(); i++)
			{
				pedLanes[i].m_verticalOffset = 0.25f;
			}

			foreach (var roadLane in roadLanes)
			{
				roadLane.m_verticalOffset = 0.1f;
			}
			for (var i = 0; i < info.m_lanes.Count(); i++)
			{
				if (info.m_lanes[i]?.m_laneProps?.m_props != null)
				{
					for (var j = 0; j < info.m_lanes[i].m_laneProps.m_props.Count(); j++)
					{
						var prop = info?.m_lanes[i]?.m_laneProps?.m_props[j];
						if (prop != null)
						{
							if (prop.m_prop.name.Contains("Street Light"))
							{
								prop.m_probability = 0;
							}
							//info.m_lanes[i].m_laneProps.m_props[j].m_probability = 0;
						}
					}
				}

			}

			var originPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
			var playerNetAI = info.GetComponent<PlayerNetAI>();

			if (playerNetAI != null && originPlayerNetAI != null)
			{
				playerNetAI.m_constructionCost = originPlayerNetAI.m_constructionCost * 1 / 2;
				playerNetAI.m_maintenanceCost = originPlayerNetAI.m_maintenanceCost * 1 / 2;
			}

			var roadBaseAI = info.GetComponent<RoadBaseAI>();
			if (roadBaseAI != null)
			{
				roadBaseAI.m_trafficLights = false;
			}
		}
	}
}
