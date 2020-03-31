using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Addon.RoadExtensions.Props;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Network;
using UnityEngine;
#if DEBUG
using Debug = Transit.Framework.Debug;
#endif
namespace Transit.Addon.RoadExtensions.Roads.Avenues.LargeAvenue8LSideParking
{
    public partial class LargeAvenue8LSideParkingBuilder : Activable, IMultiNetInfoBuilderPart
    {
        public int Order { get { return 25; } }

        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "FourDevidedLaneAvenue4Parking"; } }
        public string DisplayName { get { return "Four-Devided-Lane Avenue With Parking"; } }
        public string Description { get { return "A four-lane road with paved median. Supports heavy urban traffic."; } }
        public string ShortDescription { get { return "Parking, zoneable, heavy urban traffic"; } }


        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground | NetInfoVersion.GroundGrass | NetInfoVersion.GroundTrees; }
        }

        public IEnumerable<IMenuItemBuilder> MenuItemBuilders
        {
            get
            {
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 25,
                    Name = "FourDevidedLaneAvenue4Parking",
                    DisplayName = "Four-Devided-Lane Avenue With Parking",
                    Description = "A four-lane road with paved median. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue8LSideParking\thumbnails.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue8LSideParking\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 26,
                    Name = "FourDevidedLaneAvenue4Parking Decoration Grass",
                    DisplayName = "Four-Devided-Lane Avenue With Parking and Grass",
                    Description = "A four-lane road with paved median. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue8LSideParking\thumbnails_grass.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue8LSideParking\infotooltip_grass.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = "RoadsMedium",
                    UIOrder = 27,
                    Name = "FourDevidedLaneAvenue4Parking Decoration Trees",
                    DisplayName = "Four-Devided-Lane Avenue With 4 Parking",
                    Description = "A four-lane road with paved median. Supports heavy urban traffic.",
                    ThumbnailsPath = @"Roads\Avenues\LargeAvenue8LSideParking\thumbnails_trees.png",
                    InfoTooltipPath = @"Roads\Avenues\LargeAvenue8LSideParking\infotooltip_trees.png"
                };
            }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var roadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_4L_TUNNEL);
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_6L);
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup32m3mSW2x3mMdnMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = true;
            info.m_pavementWidth = (version == NetInfoVersion.Slope || version == NetInfoVersion.Tunnel ? 4 : 3);
            info.m_halfWidth = (version == NetInfoVersion.Tunnel ? 17 : 16);


            info.m_class = roadInfo.m_class.Clone("FourDevidedLaneAvenue4Parking");
            info.m_canCrossLanes = false;
            // Setting up lanes        

            var carLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle).ToList();
            //var pedLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ToList();
            var parking = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Parking).ToList();

            foreach (var lane in parking)
            {
                lane.m_position = Math.Sign(lane.m_position) * 12.5f;
            }
            //for (var i = 0; i < 2; i++)
            //{
            //    var parkLane = parking[i].ShallowClone();
            //    var sign = Math.Sign(parking[i].m_position);
            //    parkLane.m_position = sign * 12f;
            //    parking
            //}


            var maxPosition = carLanes.Max(l => l.m_position);
            var outerCarlanes = carLanes.Where(l => Math.Abs(l.m_position) == maxPosition).ToList();
            for (int i = 0; i < carLanes.Count; i++)
            {
                if (Math.Abs(carLanes[i].m_position) == maxPosition)
                {
                    var sign = Math.Sign(carLanes[i].m_position);
                    carLanes[i].m_position = sign * 10.25f;
                    carLanes[i].m_width = 2.5f;
                    carLanes[i].m_speedLimit = 0.6f;
                    carLanes[i].m_stopType = VehicleInfo.VehicleType.None;
                }
                else if(Math.Abs(carLanes[i].m_position) == 4.5f)
                {
                    carLanes[i].m_stopType = VehicleInfo.VehicleType.Car;
                    carLanes[i].m_stopOffset = 3;
                }
            }

            var leftPedLane = info.GetLeftRoadShoulder();
            var rightPedLane = info.GetRightRoadShoulder();

            var leftPedLaneProps = leftPedLane.m_laneProps.m_props.ToList();
            var rightPedLaneProps = rightPedLane.m_laneProps.m_props.ToList();

            leftPedLane.m_laneProps.m_props = leftPedLaneProps.ToArray();
            leftPedLane.m_stopType = VehicleInfo.VehicleType.None;
            rightPedLane.m_laneProps.m_props = rightPedLaneProps.ToArray();
            rightPedLane.m_stopType = VehicleInfo.VehicleType.None;

            var pedLanes = new List<NetInfo.Lane>();
            pedLanes.Add(leftPedLane);
            pedLanes.Add(rightPedLane);
            var leftIntPedLane = leftPedLane.ShallowClone();
            var rightIntPedLane = rightPedLane.ShallowClone();
            leftIntPedLane.m_position = Math.Sign(leftIntPedLane.m_position) * 7.5f;
            leftIntPedLane.m_width = 3;
            leftIntPedLane.m_centerPlatform = true;
            leftIntPedLane.m_stopType = VehicleInfo.VehicleType.Car;
            rightIntPedLane.m_position = Math.Sign(rightIntPedLane.m_position) * 7.5f;
            rightIntPedLane.m_width = 3;
            rightIntPedLane.m_centerPlatform = true;
            rightIntPedLane.m_stopType = VehicleInfo.VehicleType.Car;
            pedLanes.Add(leftIntPedLane);
            pedLanes.Add(rightIntPedLane);
            //carLanes[4].m_position += 1;
            var tempLanes = new List<NetInfo.Lane>();
            tempLanes.AddRange(pedLanes);
            //  tempLanes.AddRange(pedkLanes);
            tempLanes.AddRange(carLanes);
            tempLanes.AddRange(parking);
            info.m_lanes = tempLanes.ToArray();



            // AI
            var owPlayerNetAI = roadInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();

            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 2; // Charge by the lane?
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 2; // Charge by the lane?
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
                roadBaseAI.m_trafficLights = true;
            }
        }

    }
}

