#define DEBUGVSTATEx

using System;
using ColossalFramework;
using UnityEngine;
using System.Collections.Generic;
using TrafficManager.TrafficLight;

namespace TrafficManager.Traffic {
	public class VehicleState {
		private static readonly VehicleInfo.VehicleType HANDLED_VEHICLE_TYPES = VehicleInfo.VehicleType.Car | VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Tram;

		private VehicleJunctionTransitState junctionTransitState;
		public VehicleJunctionTransitState JunctionTransitState {
			get { return junctionTransitState; }
			set {
				LastStateUpdate = Singleton<SimulationManager>.instance.m_currentFrameIndex;
				junctionTransitState = value;
			}
		}

		public uint LastStateUpdate {
			get; private set;
		}

		public uint LastPositionUpdate {
			get; private set;
		}

		public bool FrontVehicle {
			get; internal set;
		}

		public float TotalLength {
			get; private set;
		}

		private ushort VehicleId;
		public int WaitTime = 0;
		public float ReduceSpeedByValueToYield;

		public bool Valid;
		public bool Emergency;
		public uint LastPathRecalculation;
		public ExtVehicleType VehicleType;

		private LinkedList<VehiclePosition> VehiclePositions; // the last element holds the current position
		private LinkedListNode<VehiclePosition> CurrentPosition;
		private int numRegisteredAhead = 0;

		public VehicleState(ushort vehicleId) {
			this.VehicleId = vehicleId;
			Reset();
		}

		public void Reset() {
#if DEBUGVSTATE
			Log._Debug($"Reset() Resetting vehicle {VehicleId}");
#endif
			if (VehiclePositions != null) {
				while (VehiclePositions.First != null) {
					VehiclePosition pos = VehiclePositions.First.Value;
					SegmentEnd end = TrafficPriority.GetPrioritySegment(pos.TransitNodeId, pos.SourceSegmentId);
					if (end != null)
						end.UnregisterVehicle(VehicleId);
					VehiclePositions.RemoveFirst();
				}
			}
			Valid = false;
			CurrentPosition = null;
			VehiclePositions = null;
			LastPathRecalculation = 0;
			TotalLength = 0f;
			FrontVehicle = false;
			VehicleType = ExtVehicleType.None;
			WaitTime = 0;
			JunctionTransitState = VehicleJunctionTransitState.None;
			Emergency = false;
			LastStateUpdate = 0;
			numRegisteredAhead = 0;
		}

		public VehiclePosition GetCurrentPosition() {
			LinkedListNode<VehiclePosition> firstNode = CurrentPosition;
			if (firstNode == null)
				return null;
			return firstNode.Value;
		}

		internal void UpdatePosition(ref Vehicle vehicleData) {
			if (vehicleData.m_path <= 0)
				return;

			PathUnit.Position currentPos = Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path].GetPosition(vehicleData.m_pathPositionIndex >> 1);
			UpdatePosition(ref vehicleData, ref currentPos);
		}

		internal void UpdatePosition(ref Vehicle vehicleData, ref PathUnit.Position currentPos) {
#if DEBUG
			bool debug = false;// (VehicleId & 63) == 17;
#endif

			LastPositionUpdate = Singleton<SimulationManager>.instance.m_currentFrameIndex;
#if DEBUGVSTATE
			if (debug)
				Log._Debug($"UpdatePosition({VehicleId}) called");
#endif

			if (!Valid)
				return;

#if DEBUGVSTATE
			if (debug)
				Log._Debug($"UpdatePosition: Vehicle {VehicleId} is valid");
#endif

			if ((vehicleData.Info.m_vehicleType & HANDLED_VEHICLE_TYPES) == VehicleInfo.VehicleType.None) {
				// vehicle type is not handled by TM:PE
				Reset();
				return;
			}

#if DEBUGVSTATE
			if (debug)
				Log._Debug($"UpdatePosition: Vehicle {VehicleId} is handled");
#endif

			LinkedListNode<VehiclePosition> currentVehiclePosNode = CurrentPosition;
			if (CurrentPosition == null) {
#if DEBUGVSTATE
				if (debug)
					Log._Debug($"UpdatePosition: Vehicle {VehicleId} does not have a valid current position.");
#endif
				return;
			}

			// check if position has changed
			NetManager netManager = Singleton<NetManager>.instance;

			if (currentPos.m_segment == CurrentPosition.Value.SourceSegmentId && currentPos.m_lane == CurrentPosition.Value.SourceLaneIndex) {
#if DEBUGVSTATE
				if (debug)
					Log._Debug($"UpdatePosition: Vehicle {VehicleId} is still up-to-date (seg. {currentPos.m_segment} @ lane {currentPos.m_lane}).");
#endif
				return;
			}

#if DEBUGVSTATE
			if (debug)
				Log._Debug($"UpdatePosition: Vehicle {VehicleId}: Update necessary. currentPos: seg. {currentPos.m_segment} @ lane {currentPos.m_lane}");
#endif
			this.WaitTime = 0;
			this.JunctionTransitState = VehicleJunctionTransitState.None;

			// unregister the vehicle at previous segments
			ushort currentSegmentId = currentPos.m_segment;
			byte currentLaneIndex = currentPos.m_lane;

			while (CurrentPosition.Value.SourceSegmentId != currentSegmentId || CurrentPosition.Value.SourceLaneIndex != currentLaneIndex) {
				SegmentEnd end = TrafficPriority.GetPrioritySegment(CurrentPosition.Value.TransitNodeId, CurrentPosition.Value.SourceSegmentId);
				if (end != null)
					end.UnregisterVehicle(VehicleId);
#if DEBUGVSTATE
				if (debug)
					Log._Debug($"UpdatePosition: Vehicle {VehicleId}: Removed position {pos.SourceSegmentId}, {pos.TransitNodeId} (lane {pos.SourceLaneIndex}). Remaining: {VehiclePositions.Count}");
#endif
				--numRegisteredAhead;
				CurrentPosition = CurrentPosition.Next;

				if (CurrentPosition == null) {
#if DEBUGVSTATE
					if (debug)
						Log.Error($"UpdatePosition: Vehicle {VehicleId}: Could not reach current position!");
#endif
					return;
				}
			}

			if (numRegisteredAhead < 0)
				numRegisteredAhead = 0;

			int maxNumRegSegments = GetMaxNumUpcomingTrafficLightSegmentsToRegisterAt();

			// register the vehicle at upcoming segments
#if DEBUGVSTATE
			if (debug)
				Log._Debug($"UpdatePosition: Vehicle {VehicleId}: Registering at upcoming segments. numRegisteredAhead {numRegisteredAhead}. position count: {VehiclePositions.Count}");
#endif

			LinkedListNode<VehiclePosition> posNode = CurrentPosition;
			for (int i = 0; i < maxNumRegSegments; ++i) {
#if DEBUGVSTATE
				if (debug)
					Log._Debug($"UpdatePosition: Vehicle {VehicleId}: checking for segment end (2) @ {pos.SourceSegmentId}, {pos.TransitNodeId}, lane {pos.SourceLaneIndex} // currentPos: {currentPos.m_segment} @ lane {currentPos.m_lane}");
#endif

				//if (posNode.Value.SourceSegmentId == currentPos.m_segment && posNode.Value.SourceLaneIndex == currentPos.m_lane) {
					SegmentEnd end = TrafficPriority.GetPrioritySegment(posNode.Value.TransitNodeId, posNode.Value.SourceSegmentId);
					if (end != null) {
						TrafficLightSimulation nodeSim = TrafficLightSimulation.GetNodeSimulation(end.NodeId);
						if (i == 0 || (nodeSim != null && nodeSim.IsTimedLight())) {
#if DEBUGVSTATE
							if (debug)
								Log._Debug($"UpdatePosition: Vehicle {VehicleId}: checking for segment end (1) @ {pos.SourceSegmentId}, {pos.TransitNodeId}, lane {pos.SourceLaneIndex} // currentPos: {currentPos.m_segment} @ lane {currentPos.m_lane}");
#endif
							end.RegisterVehicle(VehicleId, ref vehicleData, posNode.Value);
						}
					}
				//}

				if (i >= numRegisteredAhead)
					++numRegisteredAhead;

				posNode = posNode.Next;
				if (posNode == null) {
#if DEBUGVSTATE
					if (debug)
						Log._Debug($"UpdatePosition: Vehicle {VehicleId}: posNode is null (3)");
#endif
					return;
				}
			}

			/*if (!registeredAtCurrentSegment) {
				VehiclePosition pos = GetCurrentPosition();
				if (pos == null)
					return;
				SegmentEnd end = TrafficPriority.GetPrioritySegment(pos.TransitNodeId, pos.SourceSegmentId);
				if (end == null)
					return;
				end.UpdateApproachingVehicles();
			}*/
#if DEBUGVSTATE
			if (debug) {
				currentVehiclePosition = GetCurrentPosition();
				Log._Debug($"UpdatePosition: Vehicle {VehicleId}: Update finished. Current position: {currentVehiclePosition?.SourceSegmentId} -> {currentVehiclePosition?.TransitNodeId}");
			}
#endif
		}

		protected int GetMaxNumUpcomingTrafficLightSegmentsToRegisterAt() {
			if ((VehicleType & (ExtVehicleType.RoadVehicle | ExtVehicleType.Tram)) != ExtVehicleType.None)
				return 2;
			if ((VehicleType & ExtVehicleType.RailVehicle) != ExtVehicleType.None)
				return 5;
			return 0;
		}

		internal void OnPathFindReady(ref Vehicle vehicleData) {
			Reset();

			if ((vehicleData.m_flags & Vehicle.Flags.Created) == 0) {
#if DEBUGVSTATE
				Log.Warning($"OnPathFindReady: Vehicle {VehicleId} is not created!");
#endif
				return;
			}

			// determine vehicle type
			ExtVehicleType? type = DetermineVehicleType(ref vehicleData);
			if (type == null) {
#if DEBUGVSTATE
				Log.Warning($"OnPathFindReady: Could not determine vehicle type of {VehicleId}!");
#endif
				return;
			}
			VehicleType = (ExtVehicleType)type;

			if ((vehicleData.Info.m_vehicleType & HANDLED_VEHICLE_TYPES) == VehicleInfo.VehicleType.None) {
				// vehicle type is not handled by TM:PE
#if DEBUGVSTATE
				Log.Warning($"OnPathFindReady: Vehicle {VehicleId} is not handled by TM:PE!");
#endif
				return;
			}

			// update current and upcoming vehicle positions
			var netManager = Singleton<NetManager>.instance;
			//Vector3 lastFrameVehiclePos = vehicleData.GetLastFrameData().m_position;

			/*if ((vehicleData.m_flags & Vehicle.Flags.Created) == 0) {
				TrafficPriority.RemoveVehicleFromSegments(vehicleId);
				return;
			}*/ // TODO move to VehicleStateManager

			// we have seen the vehicle
			//LastFrame = Singleton<SimulationManager>.instance.m_currentFrameIndex;

			if (vehicleData.m_path <= 0) {
#if DEBUGVSTATE
				Log.Warning($"OnPathFindReady: Vehicle {VehicleId} does not have a valid path!");
#endif
				return;
			}

			if ((Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path].m_pathFindFlags & PathUnit.FLAG_READY) == 0) {
#if DEBUGVSTATE
				Log.Warning($"OnPathFindReady: Vehicle {VehicleId} does not have finished path-finding!");
#endif
				return;
			}

			ReduceSpeedByValueToYield = UnityEngine.Random.Range(16f, 28f);
			Emergency = (vehicleData.m_flags & Vehicle.Flags.Emergency2) != 0;
			TotalLength = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[VehicleId].CalculateTotalLength(VehicleId);

#if DEBUGVSTATE
			Log._Debug($"OnPathFindReady: Vehicle {VehicleId} Type: {VehicleType} TotalLength: {TotalLength}");
#endif

			// calculate vehicle transits
			VehiclePositions = new LinkedList<VehiclePosition>();

			var currentPathUnitId = vehicleData.m_path;
			int pathPositionIndex = 0;
			PathUnit.Position curPos = Singleton<PathManager>.instance.m_pathUnits.m_buffer[currentPathUnitId].GetPosition(pathPositionIndex++);
			ushort prevSegmentId = curPos.m_segment;
			byte prevLaneIndex = curPos.m_lane;

			if (prevSegmentId <= 0) {
#if DEBUGVSTATE
				Log.Warning($"OnPathFindReady: Vehicle {VehicleId} -- no previous segmentId! Path {currentPathUnitId}, pathPosIndex {vehicleData.m_pathPositionIndex}, m_segment {curPos.m_segment}, m_lane {curPos.m_lane}");
#endif
				return;
			}

			ushort prevTargetNodeId;
			if (curPos.m_offset == 0) {
				prevTargetNodeId = netManager.m_segments.m_buffer[prevSegmentId].m_startNode;
			} else {
				prevTargetNodeId = netManager.m_segments.m_buffer[prevSegmentId].m_endNode;
			}

			//Log._Debug($"OnPathFindReady: Vehicle {VehicleId}: prevSegmentId {prevSegmentId} prevLaneIndex {prevLaneIndex} prevTargetNodeId {prevTargetNodeId}");

			// evaluate upcoming path units
			uint nextPathUnitId = currentPathUnitId;
			while (true) {
				//Log._Debug($"OnPathFindReady: Vehicle {VehicleId}: Evaluating path pos index {nextPathPos}, path unit {nextPathUnitId}");
				if (pathPositionIndex > 11) {
					// go to next path unit
					pathPositionIndex = 0;
					nextPathUnitId = Singleton<PathManager>.instance.m_pathUnits.m_buffer[nextPathUnitId].m_nextPathUnit;
					if (nextPathUnitId <= 0)
						break;
				}

				PathUnit.Position nextPos = default(PathUnit.Position);
				if (!Singleton<PathManager>.instance.m_pathUnits.m_buffer[nextPathUnitId].GetPosition(pathPositionIndex, out nextPos)) { // if this returns false, there is no next path unit
					break;
				}

				ushort nextSegmentId = nextPos.m_segment;
				byte nextLaneIndex = nextPos.m_lane;
				if (nextSegmentId <= 0)
					break;

				//Log._Debug($"OnPathFindReady: Vehicle {VehicleId}: prevSegmentId {prevSegmentId} prevLaneIndex {prevLaneIndex} prevTargetNodeId {prevTargetNodeId} nextSegmentId {nextSegmentId} nextLaneIndex {nextLaneIndex}");

				VehiclePositions.AddLast(new VehiclePosition(prevSegmentId, prevLaneIndex, prevTargetNodeId, nextSegmentId, nextLaneIndex));

				// prepare next iteration
				if (nextPos.m_offset == 0) {
					prevTargetNodeId = netManager.m_segments.m_buffer[nextSegmentId].m_startNode;
				} else {
					prevTargetNodeId = netManager.m_segments.m_buffer[nextSegmentId].m_endNode;
				}
				prevSegmentId = nextSegmentId;
				prevLaneIndex = nextLaneIndex;

				pathPositionIndex++;
			}

			CurrentPosition = VehiclePositions.First;
#if DEBUGVSTATE
			Log._Debug($"OnPathFindReady: Vehicle {VehicleId}: Vehicle is valid now. {VehiclePositions.Count} vehicle positions.");
#endif
			Valid = true;

			//prioritySegment.AddVehicle(vehicleId, upcomingVehiclePos); // TODO move to VehicleStateManager

			/*if (logVehicleLength && vehicleData.m_leadingVehicle == 0 && realTimePositions.Count > 0 && realTimePositions[0].m_segment != 0) {
				// add traffic to lane
				uint laneId = PathManager.GetLaneID(realTimePositions[0]);
				//Log._Debug($"HandleVehicle: adding traffic to segment {realTimePositions[0].m_segment}, lane {realTimePositions[0].m_lane}");
				CustomRoadAI.AddTraffic(laneId, Singleton<NetManager>.instance.m_segments.m_buffer[realTimePositions[0].m_segment].Info.m_lanes[realTimePositions[0].m_lane], (ushort)Mathf.RoundToInt(vehicleData.CalculateTotalLength(vehicleId)), logVehicleSpeed ? (ushort?)Mathf.RoundToInt(lastFrameData.m_velocity.magnitude) : null);
			}*/ // TODO move to VehicleStateManager
		}

		private static ExtVehicleType? DetermineVehicleType(ref Vehicle vehicleData) {
			if ((vehicleData.m_flags & Vehicle.Flags.Emergency2) != 0)
				return ExtVehicleType.Emergency;

			VehicleAI ai = vehicleData.Info.m_vehicleAI;
			return DetermineVehicleTypeFromAIType(ai, false);
		}

		private static ExtVehicleType? DetermineVehicleTypeFromAIType(VehicleAI ai, bool emergencyOnDuty) {
			if (emergencyOnDuty)
				return ExtVehicleType.Emergency;

			switch (ai.m_info.m_vehicleType) {
				case VehicleInfo.VehicleType.Bicycle:
					return ExtVehicleType.Bicycle;
				case VehicleInfo.VehicleType.Car:
					if (ai is PassengerCarAI)
						return ExtVehicleType.PassengerCar;
					if (ai is AmbulanceAI || ai is FireTruckAI || ai is PoliceCarAI || ai is HearseAI || ai is GarbageTruckAI || ai is MaintenanceTruckAI || ai is SnowTruckAI) {
						return ExtVehicleType.Service;
					}
					if (ai is CarTrailerAI)
						return ExtVehicleType.None;
					if (ai is BusAI)
						return ExtVehicleType.Bus;
					if (ai is TaxiAI)
						return ExtVehicleType.Taxi;
					if (ai is CargoTruckAI)
						return ExtVehicleType.CargoTruck;
					break;
				case VehicleInfo.VehicleType.Metro:
				case VehicleInfo.VehicleType.Train:
					if (ai is PassengerTrainAI)
						return ExtVehicleType.PassengerTrain;
					//if (ai is CargoTrainAI)
					return ExtVehicleType.CargoTrain;
				//break;
				case VehicleInfo.VehicleType.Tram:
					return ExtVehicleType.Tram;
				case VehicleInfo.VehicleType.Ship:
					if (ai is PassengerShipAI)
						return ExtVehicleType.PassengerShip;
					//if (ai is CargoShipAI)
					return ExtVehicleType.CargoShip;
				//break;
				case VehicleInfo.VehicleType.Plane:
					//if (ai is PassengerPlaneAI)
					return ExtVehicleType.PassengerPlane;
					//break;
			}
#if DEBUGVSTATE
			Log._Debug($"Could not determine vehicle type from ai type: {ai.GetType().ToString()}");
#endif
			return null;
		}
	}
}
