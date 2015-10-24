//using System;
//using Transit.Addon.RoadExtensions.Menus;
//using Transit.Framework;
//using Transit.Framework.Modularity;
//using UnityEngine;

//namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads
//{
//    public abstract class ZonablePedestrianBuilderBase : NetInfoBuilderBase
//    {
//        public string TemplatePrefabName { get { return NetInfos.Vanilla.PED_GRAVEL; } }
//        public string UICategory { get { return AdditionnalMenus.ROADS_PEDESTRIANS; } }

//        public virtual void BuildUp(NetInfo info, NetInfoVersion version)
//        {
//            #region Original Code from T++

//            ///////////////////////////
//            // Ground                //
//            ///////////////////////////
//            //NetInfo onewayRoad = roadsCollection.m_prefabs.FirstOrDefault(p => p.name == "Oneway Road");
//            //if (onewayRoad != null)
//            //    pedestrianRoad.m_UnlockMilestone = onewayRoad.m_UnlockMilestone;

//            //int nLanes = 4;
//            //if ((CSLTraffic.Options & OptionsManager.ModOptions.DisableCentralLaneOnPedestrianRoads) == OptionsManager.ModOptions.None)
//            //    nLanes++;

//            //NetInfo.Lane[] lanes = new NetInfo.Lane[nLanes];
//            //Array.Copy(pedestrianRoad.m_lanes, lanes, 2);
//            //Array.Copy(pedestrianRoad.m_lanes, 4, lanes, 2, 2);
//            //pedestrianRoad.m_lanes = lanes;

//            //pedestrianRoad.m_lanes[0].m_position = -4f;
//            //pedestrianRoad.m_lanes[0].m_width = 2f;

//            //NetInfo pedestrianVanilla = beautificationCollection.m_prefabs.FirstOrDefault(p => p.name == "Pedestrian Pavement");
//            //if (pedestrianVanilla != null)
//            //{
//            //    pedestrianRoad.m_lanes[0].m_laneProps = NetLaneProps.CreateInstance<NetLaneProps>();
//            //    pedestrianRoad.m_lanes[0].m_laneProps.m_props = new NetLaneProps.Prop[1];
//            //    Array.Copy(pedestrianVanilla.m_lanes[0].m_laneProps.m_props, pedestrianRoad.m_lanes[0].m_laneProps.m_props, 1);
//            //    pedestrianRoad.m_lanes[0].m_laneProps.m_props[0].m_position.x = 0.0f;
//            //}

//            //pedestrianRoad.m_lanes[1].m_position = 4f;
//            //pedestrianRoad.m_lanes[1].m_width = 2f;

//            //RoadManager.VehicleType vehiclesAllowed = RoadManager.VehicleType.ServiceVehicles;
//            //if ((CSLTraffic.Options & OptionsManager.ModOptions.AllowTrucksInPedestrianRoads) == OptionsManager.ModOptions.AllowTrucksInPedestrianRoads)
//            //    vehiclesAllowed |= RoadManager.VehicleType.CargoTruck;
//            //if ((CSLTraffic.Options & OptionsManager.ModOptions.AllowResidentsInPedestrianRoads) == OptionsManager.ModOptions.AllowResidentsInPedestrianRoads)
//            //    vehiclesAllowed |= RoadManager.VehicleType.PassengerCar;

//            //pedestrianRoad.m_lanes[2] = new NetInfoLane(pedestrianRoad.m_lanes[2], vehiclesAllowed, NetInfoLane.SpecialLaneType.PedestrianLane);
//            //pedestrianRoad.m_lanes[2].m_position = -1.25f;
//            //pedestrianRoad.m_lanes[2].m_speedLimit = 0.3f;
//            //pedestrianRoad.m_lanes[2].m_laneType = NetInfo.LaneType.Vehicle;
//            ////pedestrianRoad.m_lanes[2].m_laneProps = laneProps;

//            //pedestrianRoad.m_lanes[3] = new NetInfoLane(pedestrianRoad.m_lanes[3], vehiclesAllowed, NetInfoLane.SpecialLaneType.PedestrianLane);
//            //pedestrianRoad.m_lanes[3].m_position = 1.25f;
//            //pedestrianRoad.m_lanes[3].m_speedLimit = 0.3f;
//            //pedestrianRoad.m_lanes[3].m_laneType = NetInfo.LaneType.Vehicle;
//            ////pedestrianRoad.m_lanes[5].m_laneProps = laneProps;

//            //if (nLanes == 5)
//            //{
//            //    pedestrianRoad.m_lanes[4] = CloneLane(pedestrianRoad.m_lanes[0]);
//            //    pedestrianRoad.m_lanes[4].m_position = 0f;
//            //    pedestrianRoad.m_lanes[4].m_width = 5f;
//            //    pedestrianRoad.m_lanes[4].m_laneProps = null;
//            //}


//            ///////////////////////////
//            // Bridge                //
//            ///////////////////////////
//            //NetInfo.Lane[] lanes = new NetInfo.Lane[3];
//            //lanes[0] = pedestrianBridge.m_lanes[0];
//            //lanes[0].m_laneProps = null;

//            //RoadManager.VehicleType vehiclesAllowed = RoadManager.VehicleType.ServiceVehicles;
//            //if ((CSLTraffic.Options & OptionsManager.ModOptions.AllowTrucksInPedestrianRoads) == OptionsManager.ModOptions.AllowTrucksInPedestrianRoads)
//            //    vehiclesAllowed |= RoadManager.VehicleType.CargoTruck;
//            //if ((CSLTraffic.Options & OptionsManager.ModOptions.AllowResidentsInPedestrianRoads) == OptionsManager.ModOptions.AllowResidentsInPedestrianRoads)
//            //    vehiclesAllowed |= RoadManager.VehicleType.PassengerCar;

//            //// Backward Lane
//            //lanes[1] = new NetInfoLane(vehiclesAllowed, NetInfoLane.SpecialLaneType.PedestrianLane);
//            //lanes[1].m_position = -1.5f;
//            //lanes[1].m_width = 2f;
//            //lanes[1].m_speedLimit = 0.3f;
//            //lanes[1].m_direction = NetInfo.Direction.Backward;
//            //lanes[1].m_laneType = NetInfo.LaneType.Vehicle;
//            //lanes[1].m_vehicleType = VehicleInfo.VehicleType.Car;

//            //// Forward Lane
//            //lanes[2] = new NetInfoLane(vehiclesAllowed, NetInfoLane.SpecialLaneType.PedestrianLane);
//            //lanes[2].m_position = 1.5f;
//            //lanes[2].m_width = 2f;
//            //lanes[2].m_speedLimit = 0.3f;
//            //lanes[2].m_laneType = NetInfo.LaneType.Vehicle;
//            //lanes[2].m_vehicleType = VehicleInfo.VehicleType.Car;

//            //pedestrianBridge.m_lanes = lanes;

//            #endregion

//            ///////////////////////////
//            // Texturing             //
//            ///////////////////////////
//            switch (version)
//            {
//                case NetInfoVersion.Ground:
//                    info.SetAllSegmentsTexture(
//                        new TexturesSet
//                           (null,
//                            @"Roads\PedestrianRoads\Textures\Ground_Segment__AlphaMap.png"));
//                    info.SetAllNodesTexture(
//                        new TexturesSet
//                           (null,
//                            @"Roads\PedestrianRoads\Textures\Ground_Segment__AlphaMap.png"));
//                    break;
//            }

//            ///////////////////////////
//            // Templates             //
//            ///////////////////////////
//            var onewayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L);
//            var pedestrianVanilla = Prefabs.Find<NetInfo>(NetInfos.Vanilla.PED_PAVEMENT);

//            ///////////////////////////
//            // Set up                //
//            ///////////////////////////
//            info.m_setVehicleFlags = Vehicle.Flags.None;
//            info.m_UnlockMilestone = onewayInfo.m_UnlockMilestone;

//            switch (version)
//            {
//                case NetInfoVersion.Ground:
//                    {
//                        info.m_class = ScriptableObject.CreateInstance<ItemClass>();
//                        info.m_class.m_service = ItemClass.Service.Road;
//                        info.m_class.m_level = ItemClass.Level.Level1;

//                        NetInfo.Lane[] lanes = new NetInfo.Lane[5];
//                        Array.Copy(info.m_lanes, lanes, 2);
//                        Array.Copy(info.m_lanes, 4, lanes, 2, 2);
//                        info.m_lanes = lanes;

//                        info.m_lanes[0].m_position = -4f;
//                        info.m_lanes[0].m_width = 2f;

//                        info.m_lanes[0].m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
//                        info.m_lanes[0].m_laneProps.m_props = new NetLaneProps.Prop[1];
//                        Array.Copy(pedestrianVanilla.m_lanes[0].m_laneProps.m_props, info.m_lanes[0].m_laneProps.m_props, 1);
//                        info.m_lanes[0].m_laneProps.m_props[0].m_position.x = 0.0f;

//                        info.m_lanes[1].m_position = 4f;
//                        info.m_lanes[1].m_width = 2f;

//                        info.m_lanes[2].m_position = -1.25f;
//                        info.m_lanes[2].m_speedLimit = 0.3f;
//                        info.m_lanes[2].m_laneType = NetInfo.LaneType.Pedestrian;

//                        info.m_lanes[3].m_position = 1.25f;
//                        info.m_lanes[3].m_speedLimit = 0.3f;
//                        info.m_lanes[3].m_laneType = NetInfo.LaneType.Pedestrian;

//                        info.m_lanes[4] = info.m_lanes[0].ClonePedLane();
//                        info.m_lanes[4].m_position = 0f;
//                        info.m_lanes[4].m_width = 5f;
//                        info.m_lanes[4].m_laneProps = null;
//                    }
//                    break;

//                case NetInfoVersion.Elevated:
//                case NetInfoVersion.Bridge:
//                    {
//                        info.m_class = ScriptableObject.CreateInstance<ItemClass>();
//                        info.m_class.m_service = ItemClass.Service.Road;
//                        info.m_class.m_level = ItemClass.Level.Level1;

//                        NetInfo.Lane[] lanes = new NetInfo.Lane[3];
//                        lanes[0] = info.m_lanes[0];
//                        lanes[0].m_laneProps = null;

//                        // Backward Lane
//                        lanes[1] = new NetInfo.Lane();
//                        lanes[1].m_position = -1.5f;
//                        lanes[1].m_width = 2f;
//                        lanes[1].m_speedLimit = 0.3f;
//                        lanes[1].m_direction = NetInfo.Direction.Backward;
//                        lanes[1].m_laneType = NetInfo.LaneType.Pedestrian;

//                        // Forward Lane
//                        lanes[2] = new NetInfo.Lane();
//                        lanes[2].m_position = 1.5f;
//                        lanes[2].m_width = 2f;
//                        lanes[2].m_speedLimit = 0.3f;
//                        lanes[1].m_direction = NetInfo.Direction.Forward;
//                        lanes[2].m_laneType = NetInfo.LaneType.Pedestrian;

//                        info.m_lanes = lanes;
//                    }
//                    break;
//            }
//        }
//    }
//}
