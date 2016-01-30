using System;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Framework;
using Transit.Framework.Texturing;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads
{
    public abstract class ZonablePedestrianBuilderBase : Activable
    {
        public string BasedPrefabName { get { return NetInfos.Vanilla.PED_GRAVEL; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_PEDESTRIANS; } }

        public virtual void BuildUp(NetInfo pedestrianBridge, NetInfoVersion version)
        {
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    pedestrianBridge.SetAllSegmentsTexture(
                        new TextureSet
                           (null,
                            @"Roads\PedestrianRoads\Textures\Ground_Segment__AlphaMap.png"));
                    pedestrianBridge.SetAllNodesTexture(
                        new TextureSet
                           (null,
                            @"Roads\PedestrianRoads\Textures\Ground_Segment__AlphaMap.png"));
                    break;
            }


            ///////////////////////////
            // Templates             //
            ///////////////////////////
            var onewayRoad = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L);
            var pedestrianVanilla = Prefabs.Find<NetInfo>(NetInfos.Vanilla.PED_PAVEMENT);


            ///////////////////////////
            // Ground                //
            ///////////////////////////
            pedestrianBridge.m_UnlockMilestone = onewayRoad.m_UnlockMilestone;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var lanes = new NetInfo.Lane[4];
                        Array.Copy(pedestrianBridge.m_lanes, lanes, 2);
                        Array.Copy(pedestrianBridge.m_lanes, 4, lanes, 2, 2);
                        pedestrianBridge.m_lanes = lanes;

                        pedestrianBridge.m_lanes[0].m_position = -4f;
                        pedestrianBridge.m_lanes[0].m_width = 2f;

                        if (pedestrianVanilla != null)
                        {
                            pedestrianBridge.m_lanes[0].m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                            pedestrianBridge.m_lanes[0].m_laneProps.m_props = new NetLaneProps.Prop[1];
                            Array.Copy(pedestrianVanilla.m_lanes[0].m_laneProps.m_props, pedestrianBridge.m_lanes[0].m_laneProps.m_props, 1);
                            pedestrianBridge.m_lanes[0].m_laneProps.m_props[0].m_position.x = 0.0f;
                        }

                        pedestrianBridge.m_lanes[1].m_position = 4f;
                        pedestrianBridge.m_lanes[1].m_width = 2f;

                        const VehicleType vehiclesAllowed = VehicleType.ServiceVehicles;

                        pedestrianBridge.m_lanes[2] = new NetInfoLane(pedestrianBridge.m_lanes[2], vehiclesAllowed, NetInfoLane.SpecialLaneType.PedestrianLane);
                        pedestrianBridge.m_lanes[2].m_position = -1.25f;
                        pedestrianBridge.m_lanes[2].m_speedLimit = 0.3f;
                        pedestrianBridge.m_lanes[2].m_laneType = NetInfo.LaneType.Vehicle;

                        pedestrianBridge.m_lanes[3] = new NetInfoLane(pedestrianBridge.m_lanes[3], vehiclesAllowed, NetInfoLane.SpecialLaneType.PedestrianLane);
                        pedestrianBridge.m_lanes[3].m_position = 1.25f;
                        pedestrianBridge.m_lanes[3].m_speedLimit = 0.3f;
                        pedestrianBridge.m_lanes[3].m_laneType = NetInfo.LaneType.Vehicle;
                    }
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        NetInfo.Lane[] lanes = new NetInfo.Lane[3];
                        lanes[0] = pedestrianBridge.m_lanes[0];
                        lanes[0].m_laneProps = null;

                        const VehicleType vehiclesAllowed = VehicleType.ServiceVehicles;

                        // Backward Lane
                        lanes[1] = new NetInfoLane(vehiclesAllowed, NetInfoLane.SpecialLaneType.PedestrianLane);
                        lanes[1].m_position = -1.5f;
                        lanes[1].m_width = 2f;
                        lanes[1].m_speedLimit = 0.3f;
                        lanes[1].m_direction = NetInfo.Direction.Backward;
                        lanes[1].m_laneType = NetInfo.LaneType.Vehicle;
                        lanes[1].m_vehicleType = VehicleInfo.VehicleType.Car;

                        // Forward Lane
                        lanes[2] = new NetInfoLane(vehiclesAllowed, NetInfoLane.SpecialLaneType.PedestrianLane);
                        lanes[2].m_position = 1.5f;
                        lanes[2].m_width = 2f;
                        lanes[2].m_speedLimit = 0.3f;
                        lanes[2].m_laneType = NetInfo.LaneType.Vehicle;
                        lanes[2].m_vehicleType = VehicleInfo.VehicleType.Car;

                        pedestrianBridge.m_lanes = lanes;
                    }
                    break;
            }
        }
    }
}
