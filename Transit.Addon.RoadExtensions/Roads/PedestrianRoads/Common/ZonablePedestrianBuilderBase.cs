using System;
using System.Linq;
using Transit.Addon.RoadExtensions.Menus.Roads;
using Transit.Addon.RoadExtensions.Roads.Common;
using Transit.Framework;
using Transit.Framework.Network;
using Transit.Framework.Texturing;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.Roads.PedestrianRoads.Common
{
    public abstract class ZonablePedestrianBuilderBase : Activable
    {
        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_2L_GRAVEL; } }
        public string UICategory { get { return RExExtendedMenus.ROADS_PEDESTRIANS; } }

        public virtual void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Templates             //
            ///////////////////////////
            var onewayRoad = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L);
            var pedestrianVanilla = Prefabs.Find<NetInfo>(NetInfos.Vanilla.PED_PAVEMENT);
            var roadInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ROAD_2L_TREES);

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup8mNoSWMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            if (version == NetInfoVersion.Ground)
            {
                info.SetNakedGroundTexture(version);
            }

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_availableIn = ItemClass.Availability.All;
            info.m_surfaceLevel = 0;
            info.m_UnlockMilestone = onewayRoad.m_UnlockMilestone;
            info.m_class = roadInfo.m_class.Clone("NExtPedRoad");
            info.m_class.m_level = ItemClass.Level.Level5;
            info.m_halfWidth = 4;
            info.m_UnlockMilestone = roadInfo.m_UnlockMilestone;
            info.m_pavementWidth = 2;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        info.m_lanes = info
                            .m_lanes
                            .Where(l => l.m_laneType != NetInfo.LaneType.Parking)
                            .OrderBy(l => l.m_position)
                            .ToArray();

                        info.m_lanes[0].m_position = -4f;
                        info.m_lanes[0].m_width = 2f;
                        info.m_lanes[0].m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();

                        if (pedestrianVanilla != null)
                        {
                            info.m_lanes[0].m_laneProps.m_props = new NetLaneProps.Prop[1];
                            Array.Copy(pedestrianVanilla.m_lanes[0].m_laneProps.m_props, info.m_lanes[0].m_laneProps.m_props, 1);
                        }
                        else
                        {
                            info.m_lanes[0].m_laneProps.m_props = new NetLaneProps.Prop[0];
                        }
                        info.m_lanes[0].m_laneProps.m_props[0].m_position.x = 0.0f;
                        info.m_lanes[0].m_stopType = VehicleInfo.VehicleType.None;

                        info.m_lanes[3].m_position = 4f;
                        info.m_lanes[3].m_width = 2f;
                        info.m_lanes[3].m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                        info.m_lanes[3].m_laneProps.m_props = new NetLaneProps.Prop[0];
                        info.m_lanes[3].m_stopType = VehicleInfo.VehicleType.None;

                        info.m_lanes[1] = new ExtendedNetInfoLane(info.m_lanes[1], ExtendedVehicleType.ServiceVehicles);
                        info.m_lanes[1].m_position = -1.25f;
                        info.m_lanes[1].m_speedLimit = 0.3f;
                        info.m_lanes[1].m_stopType = VehicleInfo.VehicleType.None;
                        info.m_lanes[1].m_laneType = NetInfo.LaneType.Vehicle;

                        info.m_lanes[2] = new ExtendedNetInfoLane(info.m_lanes[2], ExtendedVehicleType.ServiceVehicles);
                        info.m_lanes[2].m_position = 1.25f;
                        info.m_lanes[2].m_speedLimit = 0.3f;
                        info.m_lanes[2].m_stopType = VehicleInfo.VehicleType.None;
                        info.m_lanes[2].m_laneType = NetInfo.LaneType.Vehicle;

                        var centerLane = info.m_lanes[3].ShallowClone();
                        centerLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                        centerLane.m_laneProps.m_props = new NetLaneProps.Prop[0];
                        centerLane.m_position = 0f;

                        info.m_lanes = info.m_lanes.Union(centerLane).ToArray();
                    }
                    break;

                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        NetInfo.Lane[] lanes = new NetInfo.Lane[3];
                        lanes[0] = info.m_lanes[0];
                        lanes[0].m_stopType = VehicleInfo.VehicleType.None;
                        lanes[0].m_laneProps = null;

                        // Backward Lane
                        lanes[1] = new ExtendedNetInfoLane(ExtendedVehicleType.ServiceVehicles);
                        lanes[1].m_position = -1.5f;
                        lanes[1].m_width = 2f;
                        lanes[1].m_speedLimit = 0.3f;
                        lanes[1].m_stopType = VehicleInfo.VehicleType.None;
                        lanes[1].m_direction = NetInfo.Direction.Backward;
                        lanes[1].m_laneType = NetInfo.LaneType.Vehicle;
                        lanes[1].m_vehicleType = VehicleInfo.VehicleType.Car;

                        // Forward Lane
                        lanes[2] = new ExtendedNetInfoLane(ExtendedVehicleType.ServiceVehicles);
                        lanes[2].m_position = 1.5f;
                        lanes[2].m_width = 2f;
                        lanes[2].m_speedLimit = 0.3f;
                        lanes[2].m_stopType = VehicleInfo.VehicleType.None;
                        lanes[2].m_laneType = NetInfo.LaneType.Vehicle;
                        lanes[2].m_vehicleType = VehicleInfo.VehicleType.Car;

                        info.m_lanes = lanes;
                    }
                    break;
            }
        }
    }
}
