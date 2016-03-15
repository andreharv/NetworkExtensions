using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transit.Addon.TM.State;
using Transit.Addon.TM.Data;
using Transit.Framework;

namespace Transit.Addon.TM.Traffic {
	class VehicleRestrictionsManager {
		/// <summary>
		/// Determines the allowed vehicle types that may approach the given node from the given segment.
		/// </summary>
		/// <param name="segmentId"></param>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		internal static TMVehicleType GetAllowedVehicleTypes(ushort segmentId, ushort nodeId) {
			TMVehicleType ret = TMVehicleType.None;
			foreach (TMVehicleType vehicleType in GetAllowedVehicleTypesAsSet(segmentId, nodeId)) {
				ret |= vehicleType;
			}
			return ret;
		}

		/// <summary>
		/// Determines the allowed vehicle types that may approach the given node from the given segment.
		/// </summary>
		/// <param name="segmentId"></param>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		internal static HashSet<TMVehicleType> GetAllowedVehicleTypesAsSet(ushort segmentId, ushort nodeId) {
			HashSet<TMVehicleType> ret = new HashSet<TMVehicleType>();

			NetManager netManager = Singleton<NetManager>.instance;
			if (segmentId == 0 || (netManager.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None ||
				nodeId == 0 || (netManager.m_nodes.m_buffer[nodeId].m_flags & NetNode.Flags.Created) == NetNode.Flags.None)
				return ret;

			var dir = NetInfo.Direction.Forward;
			var dir2 = ((netManager.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None) ? dir : NetInfo.InvertDirection(dir);
			var dir3 = TrafficPriority.IsLeftHandDrive() ? NetInfo.InvertDirection(dir2) : dir2;

			NetInfo segmentInfo = netManager.m_segments.m_buffer[segmentId].Info;
			uint curLaneId = netManager.m_segments.m_buffer[segmentId].m_lanes;
			int numLanes = segmentInfo.m_lanes.Length;
			uint laneIndex = 0;
			while (laneIndex < numLanes && curLaneId != 0u) {
				NetInfo.Lane laneInfo = segmentInfo.m_lanes[laneIndex];
				ushort toNodeId = (laneInfo.m_direction == dir3) ? netManager.m_segments.m_buffer[segmentId].m_endNode : netManager.m_segments.m_buffer[segmentId].m_startNode;

				if (toNodeId == nodeId) {
					TMVehicleType vehicleTypes = GetAllowedVehicleTypes(segmentId, laneIndex, curLaneId, laneInfo);
					if (vehicleTypes != TMVehicleType.None)
						ret.Add(vehicleTypes);
				}
				curLaneId = netManager.m_lanes.m_buffer[curLaneId].m_nextLane;
				++laneIndex;
			}

			return ret;
        }

        /// <summary>
        /// Determines the allowed vehicle types for the given segment and lane.
        /// </summary>
        /// <param name="segmentId"></param>
        /// <param name="laneIndex"></param>
        /// <param name="laneId"></param>
        /// <param name="laneInfo"></param>
        /// <returns></returns>
        internal static TMVehicleType GetAllowedVehicleTypes(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo) {
			if (Flags.IsInitDone()) {
				TMVehicleType?[] fastArray = Flags.laneAllowedVehicleTypesArray[segmentId];
				if (fastArray != null)
                {
                    if(laneIndex < fastArray.Length && fastArray[laneIndex] != null)
                    {
                        return (TMVehicleType)fastArray[laneIndex];
                    }
                }
			}

			TMVehicleType ret = TMVehicleType.None;
			if ((laneInfo.m_vehicleType & VehicleInfo.VehicleType.Bicycle) != VehicleInfo.VehicleType.None)
				ret |= TMVehicleType.Bicycle;
			if ((laneInfo.m_vehicleType & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None)
				ret |= TMVehicleType.Tram;
			if ((laneInfo.m_laneType & NetInfo.LaneType.TransportVehicle) != NetInfo.LaneType.None)
				ret |= TMVehicleType.RoadPublicTransport | TMVehicleType.Emergency;
			else if ((laneInfo.m_vehicleType & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None)
				ret |= TMVehicleType.RoadVehicle;
			if ((laneInfo.m_vehicleType & (VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Metro)) != VehicleInfo.VehicleType.None)
				ret |= TMVehicleType.RailVehicle;
			if ((laneInfo.m_vehicleType & VehicleInfo.VehicleType.Ship) != VehicleInfo.VehicleType.None)
				ret |= TMVehicleType.Ship;
			if ((laneInfo.m_vehicleType & VehicleInfo.VehicleType.Plane) != VehicleInfo.VehicleType.None)
				ret |= TMVehicleType.Plane;

			return ret;
		}

		/// <summary>
		/// Sets the allowed vehicle types for the given segment and lane.
		/// </summary>
		/// <param name="segmentId"></param>
		/// <param name="laneIndex"></param>
		/// <param name="laneId"></param>
		/// <param name="allowedTypes"></param>
		/// <returns></returns>
		internal static bool SetAllowedVehicleTypes(ushort segmentId, uint laneIndex, uint laneId, TMVehicleType allowedTypes) {
			if (segmentId == 0)
				return false;
			if ((Singleton<NetManager>.instance.m_segments.m_buffer[segmentId].m_flags & NetSegment.Flags.Created) == NetSegment.Flags.None)
				return false;
			if (((NetLane.Flags)Singleton<NetManager>.instance.m_lanes.m_buffer[laneId].m_flags & NetLane.Flags.Created) == NetLane.Flags.None)
				return false;

			Flags.setLaneAllowedVehicleTypes(segmentId, laneIndex, laneId, allowedTypes);
			return true;
		}

		/// <summary>
		/// Adds the given vehicle type to the set of allowed vehicles at the specified lane
		/// </summary>
		/// <param name="segmentId"></param>
		/// <param name="laneIndex"></param>
		/// <param name="laneId"></param>
		/// <param name="laneInfo"></param>
		/// <param name="road"></param>
		/// <param name="vehicleType"></param>
		public static void AddAllowedType(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo, TMVehicleType vehicleType) {
			TMVehicleType allowedTypes = GetAllowedVehicleTypes(segmentId, laneIndex, laneId, laneInfo);
			allowedTypes |= vehicleType;
			Flags.setLaneAllowedVehicleTypes(segmentId, laneIndex, laneId, allowedTypes);
		}

		/// <summary>
		/// Removes the given vehicle type from the set of allowed vehicles at the specified lane
		/// </summary>
		/// <param name="segmentId"></param>
		/// <param name="laneIndex"></param>
		/// <param name="laneId"></param>
		/// <param name="laneInfo"></param>
		/// <param name="road"></param>
		/// <param name="vehicleType"></param>
		public static void RemoveAllowedType(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo, TMVehicleType vehicleType) {
			TMVehicleType allowedTypes = GetAllowedVehicleTypes(segmentId, laneIndex, laneId, laneInfo);
			allowedTypes &= ~vehicleType;
			Flags.setLaneAllowedVehicleTypes(segmentId, laneIndex, laneId, allowedTypes);
		}

		public static void ToggleAllowedType(ushort segmentId, uint laneIndex, uint laneId, NetInfo.Lane laneInfo, TMVehicleType vehicleType, bool add) {
			if (add)
				AddAllowedType(segmentId, laneIndex, laneId, laneInfo, vehicleType);
			else
				RemoveAllowedType(segmentId, laneIndex, laneId, laneInfo, vehicleType);
		}

		public static bool IsAllowed(TMVehicleType? allowedTypes, TMVehicleType vehicleType) {
			return allowedTypes == null || ((TMVehicleType)allowedTypes & vehicleType) != TMVehicleType.None;
		}

		public static bool IsBicycleAllowed(TMVehicleType? allowedTypes) {
			return IsAllowed(allowedTypes, TMVehicleType.Bicycle);
		}

		public static bool IsBusAllowed(TMVehicleType? allowedTypes) {
			return IsAllowed(allowedTypes, TMVehicleType.Bus);
		}

		public static bool IsCargoTrainAllowed(TMVehicleType? allowedTypes) {
			return IsAllowed(allowedTypes, TMVehicleType.CargoTrain);
		}

		public static bool IsCargoTruckAllowed(TMVehicleType? allowedTypes) {
			return IsAllowed(allowedTypes, TMVehicleType.CargoTruck);
		}

		public static bool IsEmergencyAllowed(TMVehicleType? allowedTypes) {
			return IsAllowed(allowedTypes, TMVehicleType.Emergency);
		}

		internal static TMVehicleType GetAllowedVehicleTypes(object selectedSegment, uint selectedLaneIndex, uint selectedLaneId, NetInfo.Lane selectedLaneInfo) {
			throw new NotImplementedException();
		}

		public static bool IsPassengerCarAllowed(TMVehicleType? allowedTypes) {
			return IsAllowed(allowedTypes, TMVehicleType.PassengerCar);
		}

		public static bool IsPassengerTrainAllowed(TMVehicleType? allowedTypes) {
			return IsAllowed(allowedTypes, TMVehicleType.PassengerTrain);
		}

		public static bool IsServiceAllowed(TMVehicleType? allowedTypes) {
			return IsAllowed(allowedTypes, TMVehicleType.Service);
		}

		public static bool IsTaxiAllowed(TMVehicleType? allowedTypes) {
			return IsAllowed(allowedTypes, TMVehicleType.Taxi);
		}

		public static bool IsTramAllowed(TMVehicleType? allowedTypes) {
			return IsAllowed(allowedTypes, TMVehicleType.Tram);
		}

		public static bool IsRailVehicleAllowed(TMVehicleType? allowedTypes) {
			return IsAllowed(allowedTypes, TMVehicleType.RailVehicle);
		}

		public static bool IsRoadVehicleAllowed(TMVehicleType? allowedTypes) {
			return IsAllowed(allowedTypes, TMVehicleType.RoadVehicle);
		}

		public static bool IsRailLane(NetInfo.Lane laneInfo) {
			return (laneInfo.m_vehicleType & VehicleInfo.VehicleType.Train) != VehicleInfo.VehicleType.None;
		}

		public static bool IsRoadLane(NetInfo.Lane laneInfo) {
			return (laneInfo.m_vehicleType & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None;
		}
	}
}
