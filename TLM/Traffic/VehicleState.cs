#define USEPATHWAITCOUNTERx
#define DEBUGVSTATEx
#define PATHFORECASTx
#define PATHRECALCx
#define DEBUGREGx

using System;
using ColossalFramework;
using UnityEngine;
using System.Collections.Generic;
using TrafficManager.TrafficLight;

namespace TrafficManager.Traffic {
	public class VehicleState {
#if DEBUGVSTATE
		private static readonly ushort debugVehicleId = 6316;
#endif
		private static readonly VehicleInfo.VehicleType HANDLED_VEHICLE_TYPES = VehicleInfo.VehicleType.Car | VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Tram;
		public static readonly int STATE_UPDATE_SHIFT = 6;

		private VehicleJunctionTransitState junctionTransitState;
		public VehicleJunctionTransitState JunctionTransitState {
			get { return junctionTransitState; }
			set {
				LastStateUpdate = Singleton<SimulationManager>.instance.m_currentFrameIndex >> VehicleState.STATE_UPDATE_SHIFT;
				junctionTransitState = value;
			}
		}

		public uint LastStateUpdate {
			get; private set;
		}

		public uint LastPositionUpdate {
			get; private set;
		}

		public float TotalLength {
			get; private set;
		}

		private ushort VehicleId;
		public int WaitTime = 0;
		public float ReduceSpeedByValueToYield;
		private bool valid = false;

		public bool Valid {
			get {
				if ((Singleton<VehicleManager>.instance.m_vehicles.m_buffer[VehicleId].m_flags & Vehicle.Flags.Created) == 0) {
					return false;
				}
				return valid;
			}
			set { valid = value; }
		}
		public bool Emergency;
#if PATHRECALC
		public uint LastPathRecalculation = 0;
		public ushort LastPathRecalculationSegmentId = 0;
		public bool PathRecalculationRequested { get; internal set; } = false;
#endif
		public ExtVehicleType VehicleType;


#if PATHFORECAST
		private LinkedList<VehiclePosition> VehiclePositions; // the last element holds the current position
		private LinkedListNode<VehiclePosition> CurrentPosition;
		private int numRegisteredAhead = 0;
#endif
		public SegmentEnd CurrentSegmentEnd {
			get; internal set;
		}
		public ushort PreviousVehicleIdOnSegment {
			get; internal set;
		}
		public ushort NextVehicleIdOnSegment {
			get; internal set;
		}
		public float CurrentMaxSpeed { get; internal set; }

#if USEPATHWAITCOUNTER
		public ushort PathWaitCounter { get; internal set; } = 0;
#endif

		public VehicleState(ushort vehicleId) {
			this.VehicleId = vehicleId;
			Reset();
		}

		public bool UsePathForecast() {
			return false;
			//return (VehicleType & ExtVehicleType.RailVehicle) != ExtVehicleType.None;
		}

		protected int GetMaxNumUpcomingTrafficLightSegmentsToRegisterAt() {
			if (!UsePathForecast())
				return 0;
			/*if ((VehicleType & (ExtVehicleType.RoadVehicle | ExtVehicleType.Tram)) != ExtVehicleType.None)
				return 2;*/
			if ((VehicleType & ExtVehicleType.RailVehicle) != ExtVehicleType.None)
				return 5;
			return 0;
		}

		public void Reset() {
#if DEBUGVSTATE
			bool debug = VehicleId == debugVehicleId;
			if (debug)
				Log._Debug($"Reset() Resetting vehicle {VehicleId}");
#endif
#if PATHFORECAST
			if (VehiclePositions != null) {
				while (VehiclePositions.First != null) {
					VehiclePosition pos = VehiclePositions.First.Value;
					SegmentEnd end = TrafficPriority.GetPrioritySegment(pos.TransitNodeId, pos.SourceSegmentId);
					if (end != null)
						end.UnregisterVehicle(VehicleId);
					VehiclePositions.RemoveFirst();
				}
			}
			CurrentPosition = null;
			VehiclePositions = null;
			numRegisteredAhead = 0;
#endif

			Unlink();

			Valid = false;
			//LastPathRecalculation = 0;
			TotalLength = 0f;
			VehicleType = ExtVehicleType.None;
			WaitTime = 0;
			JunctionTransitState = VehicleJunctionTransitState.None;
			Emergency = false;
			LastStateUpdate = 0;
			CurrentMaxSpeed = 0;
		}

		internal void Unlink() {
			if (PreviousVehicleIdOnSegment != 0) {
				VehicleStateManager._GetVehicleState(PreviousVehicleIdOnSegment).NextVehicleIdOnSegment = NextVehicleIdOnSegment;
			} else if (CurrentSegmentEnd != null && CurrentSegmentEnd.FirstRegisteredVehicleId == VehicleId) {
				CurrentSegmentEnd.FirstRegisteredVehicleId = NextVehicleIdOnSegment;
			}

			if (NextVehicleIdOnSegment != 0) {
				VehicleStateManager._GetVehicleState(NextVehicleIdOnSegment).PreviousVehicleIdOnSegment = PreviousVehicleIdOnSegment;
			}

			NextVehicleIdOnSegment = 0;
			PreviousVehicleIdOnSegment = 0;
			CurrentSegmentEnd = null;
		}

		private void Link(SegmentEnd end) {
			ushort oldFirstRegVehicleId = end.FirstRegisteredVehicleId;
			if (oldFirstRegVehicleId != 0) {
				VehicleStateManager._GetVehicleState(oldFirstRegVehicleId).PreviousVehicleIdOnSegment = VehicleId;
				NextVehicleIdOnSegment = oldFirstRegVehicleId;
			}
			end.FirstRegisteredVehicleId = VehicleId;
			CurrentSegmentEnd = end;
		}

		public delegate void PathPositionProcessor(ref PathUnit.Position pos);
		public delegate void DualPathPositionProcessor(ref PathUnit.Position firstPos, ref PathUnit.Position secondPos);
		public delegate void QuadPathPositionProcessor(ref Vehicle firstVehicleData, ref PathUnit.Position firstVehicleFirstPos, ref PathUnit.Position firstVehicleSecondPos, ref PathUnit.Position secondVehicleFirstPos, ref PathUnit.Position secondVehicleSecondPos);

		public bool ProcessCurrentPathPosition(ref Vehicle vehicleData, PathPositionProcessor processor) {
			return ProcessPathUnit(ref Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path], (byte)(vehicleData.m_pathPositionIndex >> 1), processor);
		}

		public bool ProcessCurrentPathPosition(ref Vehicle vehicleData, byte index, PathPositionProcessor processor) {
			return ProcessPathUnit(ref Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path], index, processor);
		}

		public bool ProcessCurrentAndNextPathPosition(ref Vehicle vehicleData, DualPathPositionProcessor processor) {
			return ProcessCurrentAndNextPathPosition(ref vehicleData, (byte)(vehicleData.m_pathPositionIndex >> 1), processor);
		}

		public bool ProcessCurrentAndNextPathPosition(ref Vehicle vehicleData, byte index, DualPathPositionProcessor processor) {
			if (index < 11)
				return ProcessPathUnitPair(ref Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path], ref Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path], index, processor);
			else
				return ProcessPathUnitPair(ref Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path], ref Singleton<PathManager>.instance.m_pathUnits.m_buffer[Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path].m_nextPathUnit], index, processor);
		}

		public bool ProcessCurrentAndNextPathPositionAndOtherVehicleCurrentAndNextPathPosition(ref Vehicle vehicleData, ref PathUnit.Position otherVehicleCurPos, ref PathUnit.Position otherVehicleNextPos, QuadPathPositionProcessor processor) {
			return ProcessCurrentAndNextPathPositionAndOtherVehicleCurrentAndNextPathPosition(ref vehicleData, (byte)(vehicleData.m_pathPositionIndex >> 1), ref otherVehicleCurPos, ref otherVehicleNextPos, processor);
		}

		public bool ProcessCurrentAndNextPathPositionAndOtherVehicleCurrentAndNextPathPosition(ref Vehicle vehicleData, byte index, ref PathUnit.Position otherVehicleCurPos, ref PathUnit.Position otherVehicleNextPos, QuadPathPositionProcessor processor) {
			if (index < 11)
				return ProcessPathUnitQuad(ref vehicleData, ref Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path], ref Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path], index, ref otherVehicleCurPos, ref otherVehicleNextPos, processor);
			else
				return ProcessPathUnitQuad(ref vehicleData, ref Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path], ref Singleton<PathManager>.instance.m_pathUnits.m_buffer[Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path].m_nextPathUnit], index, ref otherVehicleCurPos, ref otherVehicleNextPos, processor);
		}

		public PathUnit.Position GetCurrentPathPosition(ref Vehicle vehicleData) {
			return Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path].GetPosition((byte)(vehicleData.m_pathPositionIndex >> 1));
		}

		public PathUnit.Position GetNextPathPosition(ref Vehicle vehicleData) {
			byte index = (byte)((vehicleData.m_pathPositionIndex >> 1) + 1);
			if (index <= 11)
				return Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path].GetPosition(index);
			else
				return Singleton<PathManager>.instance.m_pathUnits.m_buffer[Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path].m_nextPathUnit].GetPosition(0);
		}

		private bool ProcessPathUnitPair(ref PathUnit unit1, ref PathUnit unit2, byte index, DualPathPositionProcessor processor) {
			if ((unit1.m_pathFindFlags & PathUnit.FLAG_READY) == 0) {
				return false;
			}

			if ((unit2.m_pathFindFlags & PathUnit.FLAG_READY) == 0) {
				return false;
			}

			switch (index) {
				case 0:
					processor(ref unit1.m_position00, ref unit1.m_position01);
					break;
				case 1:
					processor(ref unit1.m_position01, ref unit1.m_position02);
					break;
				case 2:
					processor(ref unit1.m_position02, ref unit1.m_position03);
					break;
				case 3:
					processor(ref unit1.m_position03, ref unit1.m_position04);
					break;
				case 4:
					processor(ref unit1.m_position04, ref unit1.m_position05);
					break;
				case 5:
					processor(ref unit1.m_position05, ref unit1.m_position06);
					break;
				case 6:
					processor(ref unit1.m_position06, ref unit1.m_position07);
					break;
				case 7:
					processor(ref unit1.m_position07, ref unit1.m_position08);
					break;
				case 8:
					processor(ref unit1.m_position08, ref unit1.m_position09);
					break;
				case 9:
					processor(ref unit1.m_position09, ref unit1.m_position10);
					break;
				case 10:
					processor(ref unit1.m_position10, ref unit1.m_position11);
					break;
				case 11:
					processor(ref unit1.m_position11, ref unit2.m_position00);
					break;
				default:
					return false;
			}

			return true;
		}

		private bool ProcessPathUnitQuad(ref Vehicle firstVehicleData, ref PathUnit unit1, ref PathUnit unit2, byte index, ref PathUnit.Position otherPos1, ref PathUnit.Position otherPos2, QuadPathPositionProcessor processor) {
			if ((unit1.m_pathFindFlags & PathUnit.FLAG_READY) == 0) {
				return false;
			}

			if ((unit2.m_pathFindFlags & PathUnit.FLAG_READY) == 0) {
				return false;
			}

			switch (index) {
				case 0:
					processor(ref firstVehicleData, ref unit1.m_position00, ref unit1.m_position01, ref otherPos1, ref otherPos2);
					break;
				case 1:
					processor(ref firstVehicleData, ref unit1.m_position01, ref unit1.m_position02, ref otherPos1, ref otherPos2);
					break;
				case 2:
					processor(ref firstVehicleData, ref unit1.m_position02, ref unit1.m_position03, ref otherPos1, ref otherPos2);
					break;
				case 3:
					processor(ref firstVehicleData, ref unit1.m_position03, ref unit1.m_position04, ref otherPos1, ref otherPos2);
					break;
				case 4:
					processor(ref firstVehicleData, ref unit1.m_position04, ref unit1.m_position05, ref otherPos1, ref otherPos2);
					break;
				case 5:
					processor(ref firstVehicleData, ref unit1.m_position05, ref unit1.m_position06, ref otherPos1, ref otherPos2);
					break;
				case 6:
					processor(ref firstVehicleData, ref unit1.m_position06, ref unit1.m_position07, ref otherPos1, ref otherPos2);
					break;
				case 7:
					processor(ref firstVehicleData, ref unit1.m_position07, ref unit1.m_position08, ref otherPos1, ref otherPos2);
					break;
				case 8:
					processor(ref firstVehicleData, ref unit1.m_position08, ref unit1.m_position09, ref otherPos1, ref otherPos2);
					break;
				case 9:
					processor(ref firstVehicleData, ref unit1.m_position09, ref unit1.m_position10, ref otherPos1, ref otherPos2);
					break;
				case 10:
					processor(ref firstVehicleData, ref unit1.m_position10, ref unit1.m_position11, ref otherPos1, ref otherPos2);
					break;
				case 11:
					processor(ref firstVehicleData, ref unit1.m_position11, ref unit2.m_position00, ref otherPos1, ref otherPos2);
					break;
				default:
					return false;
			}

			return true;
		}

		internal bool HasPath(ref Vehicle vehicleData) {
			return (Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path].m_pathFindFlags & PathUnit.FLAG_READY) != 0;
		}

		private bool ProcessPathUnit(ref PathUnit unit, byte index, PathPositionProcessor processor) {
			if ((unit.m_pathFindFlags & PathUnit.FLAG_READY) == 0) {
				return false;
			}

			switch (index) {
				case 0:
					processor(ref unit.m_position00);
					break;
				case 1:
					processor(ref unit.m_position01);
					break;
				case 2:
					processor(ref unit.m_position02);
					break;
				case 3:
					processor(ref unit.m_position03);
					break;
				case 4:
					processor(ref unit.m_position04);
					break;
				case 5:
					processor(ref unit.m_position05);
					break;
				case 6:
					processor(ref unit.m_position06);
					break;
				case 7:
					processor(ref unit.m_position07);
					break;
				case 8:
					processor(ref unit.m_position08);
					break;
				case 9:
					processor(ref unit.m_position09);
					break;
				case 10:
					processor(ref unit.m_position10);
					break;
				case 11:
					processor(ref unit.m_position11);
					break;
				default:
					return false;
			}
			return true;
		}

		/*public VehiclePosition GetCurrentPosition() {
			LinkedListNode<VehiclePosition> firstNode = CurrentPosition;
			if (firstNode == null)
				return null;
			return firstNode.Value;
		}*/

		internal void UpdatePosition(ref Vehicle vehicleData) {
			if (vehicleData.m_path <= 0) {
				Reset();
				return;
			}

#if DEBUGVSTATE
			//bool debug = (VehicleId & 63) == 17;
			bool debug = VehicleId == debugVehicleId;
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

#if PATHFORECAST
			if (UsePathForecast()) {
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
				} else {

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
							Log._Debug($"UpdatePosition: Vehicle {VehicleId}: Removed position {CurrentPosition.Value.SourceSegmentId}, {CurrentPosition.Value.TransitNodeId} (lane {CurrentPosition.Value.SourceLaneIndex}). Remaining: {VehiclePositions.Count}");
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
						Log._Debug($"UpdatePosition: Vehicle {VehicleId}: checking for segment end (2) @ {posNode.Value.SourceSegmentId}, {posNode.Value.TransitNodeId}, lane {posNode.Value.SourceLaneIndex} // currentPos: {currentPos.m_segment} @ lane {currentPos.m_lane}");
#endif

					//if (posNode.Value.SourceSegmentId == currentPos.m_segment && posNode.Value.SourceLaneIndex == currentPos.m_lane) {
					SegmentEnd end = TrafficPriority.GetPrioritySegment(posNode.Value.TransitNodeId, posNode.Value.SourceSegmentId);
					if (end != null) {
						TrafficLightSimulation nodeSim = TrafficLightSimulation.GetNodeSimulation(end.NodeId);
						if (i == 0 || (nodeSim != null && nodeSim.IsTimedLight())) {
#if DEBUGVSTATE
								if (debug)
									Log._Debug($"UpdatePosition: Vehicle {VehicleId}: checking for segment end (1) @ {posNode.Value.SourceSegmentId}, {posNode.Value.TransitNodeId}, lane {posNode.Value.SourceLaneIndex} // currentPos: {currentPos.m_segment} @ lane {currentPos.m_lane}");
#endif
							end.RegisterVehicle(VehicleId, ref vehicleData/*, posNode.Value*/);
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
			} else {
#endif
				SegmentEnd end = null;
				ProcessCurrentAndNextPathPosition(ref vehicleData, delegate (ref PathUnit.Position currentPosition, ref PathUnit.Position nextPosition) {
					end = TrafficPriority.GetPrioritySegment(GetTransitNodeId(ref currentPosition, ref nextPosition), currentPosition.m_segment);
				});


				if (CurrentSegmentEnd != end) {
					if (CurrentSegmentEnd != null) {
#if DEBUGREG
					Log._Debug($"VehicleState.UpdatePosition: (---) Removing vehicle {VehicleId} from priority segment {CurrentSegmentEnd.SegmentId}");
#endif
						Unlink();
					}

					WaitTime = 0;
					if (end != null) {
#if DEBUGREG
					Log._Debug($"VehicleState.UpdatePosition: (+++) Registering vehicle {VehicleId} at priority segment {end.SegmentId}");
#endif
						Link(end);
						JunctionTransitState = VehicleJunctionTransitState.Enter;
					} else {
						JunctionTransitState = VehicleJunctionTransitState.None;
					}
				}
#if PATHFORECAST
			}
#endif

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
				Log._Debug($"UpdatePosition: Vehicle {VehicleId}: Update finished. Current position: {CurrentPosition?.Value.SourceSegmentId} -> {CurrentPosition?.Value.TransitNodeId}");
			}
#endif
		}

		internal static ushort GetTransitNodeId(ref PathUnit.Position curPos, ref PathUnit.Position nextPos) {
			// note: does not check if curPos and nextPos are successive path positions
			NetManager netManager = Singleton<NetManager>.instance;
			ushort transitNodeId;
			if (curPos.m_offset == 0) {
				transitNodeId = netManager.m_segments.m_buffer[curPos.m_segment].m_startNode;
			} else if (curPos.m_offset == 255) {
				transitNodeId = netManager.m_segments.m_buffer[curPos.m_segment].m_endNode;
			} else if (nextPos.m_offset == 0) {
				transitNodeId = netManager.m_segments.m_buffer[nextPos.m_segment].m_startNode;
			} else {
				transitNodeId = netManager.m_segments.m_buffer[nextPos.m_segment].m_endNode;
			}
			return transitNodeId;
		}

		internal void OnPathFindReady(ref Vehicle vehicleData) {
#if DEBUGVSTATE
			bool debug = VehicleId == debugVehicleId;
#endif

			Reset();

			if ((vehicleData.m_flags & Vehicle.Flags.Created) == 0) {
#if DEBUGVSTATE
				if (debug)
					Log.Warning($"OnPathFindReady: Vehicle {VehicleId} is not created!");
#endif
				return;
			}

			// determine vehicle type
			ExtVehicleType? type = VehicleStateManager.DetermineVehicleType(ref vehicleData);
			if (type == null) {
#if DEBUGVSTATE
				if (debug)
					Log.Warning($"OnPathFindReady: Could not determine vehicle type of {VehicleId}!");
#endif
				return;
			}
			VehicleType = (ExtVehicleType)type;

			if ((vehicleData.Info.m_vehicleType & HANDLED_VEHICLE_TYPES) == VehicleInfo.VehicleType.None) {
				// vehicle type is not handled by TM:PE
#if DEBUGVSTATE
				if (debug)
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
				if (debug)
					Log.Warning($"OnPathFindReady: Vehicle {VehicleId} does not have a valid path!");
#endif
				return;
			}

			if ((Singleton<PathManager>.instance.m_pathUnits.m_buffer[vehicleData.m_path].m_pathFindFlags & PathUnit.FLAG_READY) == 0) {
#if DEBUGVSTATE
				if (debug)
					Log.Warning($"OnPathFindReady: Vehicle {VehicleId} does not have finished path-finding!");
#endif
				return;
			}

			ReduceSpeedByValueToYield = UnityEngine.Random.Range(16f, 28f);
			Emergency = (vehicleData.m_flags & Vehicle.Flags.Emergency2) != 0;
			TotalLength = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[VehicleId].CalculateTotalLength(VehicleId);

#if DEBUGVSTATE
			if (debug)
				Log._Debug($"OnPathFindReady: Vehicle {VehicleId} Type: {VehicleType} TotalLength: {TotalLength}");
#endif
#if PATHFORECAST
			if (UsePathForecast()) {
				// calculate vehicle transits
				VehiclePositions = new LinkedList<VehiclePosition>();

				var currentPathUnitId = vehicleData.m_path;
				int pathPositionIndex = 0;

				// find first path unit
				PathUnit.Position curPos = default(PathUnit.Position);
				if (!Singleton<PathManager>.instance.m_pathUnits.m_buffer[currentPathUnitId].GetPosition(pathPositionIndex++, out curPos)) { // if this returns false, there is no next path unit
#if DEBUGVSTATE
					if (debug)
						Log._Debug($"OnPathFindReady: Vehicle {VehicleId}: Could not get first path position");
#endif
					return;
				}

				ushort prevSegmentId = curPos.m_segment;
				byte prevLaneIndex = curPos.m_lane;
				byte prevOffset = curPos.m_offset;

				if (prevSegmentId <= 0) {
#if DEBUGVSTATE
					if (debug)
						Log.Warning($"OnPathFindReady: Vehicle {VehicleId} -- no previous segmentId! Path {currentPathUnitId}, pathPosIndex {vehicleData.m_pathPositionIndex}, m_segment {curPos.m_segment}, m_lane {curPos.m_lane}");
#endif
					return;
				}

				/*ushort targetNodeId;
				if (curPos.m_offset == 0) {
					targetNodeId = netManager.m_segments.m_buffer[prevSegmentId].m_startNode;
				} else {
					targetNodeId = netManager.m_segments.m_buffer[prevSegmentId].m_endNode;
				}*/

				//Log._Debug($"OnPathFindReady: Vehicle {VehicleId}: prevSegmentId {prevSegmentId} prevLaneIndex {prevLaneIndex} prevTargetNodeId {prevTargetNodeId}");

				// evaluate upcoming path units
				while (true) {
					//Log._Debug($"OnPathFindReady: Vehicle {VehicleId}: Evaluating path pos index {nextPathPos}, path unit {nextPathUnitId}");
					if (pathPositionIndex > 11) {
						// go to next path unit
						pathPositionIndex = 0;
						currentPathUnitId = Singleton<PathManager>.instance.m_pathUnits.m_buffer[currentPathUnitId].m_nextPathUnit;
						if (currentPathUnitId <= 0)
							break;
					}

					if (!Singleton<PathManager>.instance.m_pathUnits.m_buffer[currentPathUnitId].GetPosition(pathPositionIndex++, out curPos)) { // if this returns false, there is no next path unit
						break;
					}

					ushort nextSegmentId = curPos.m_segment;
					byte nextLaneIndex = curPos.m_lane;
					byte nextOffset = curPos.m_offset;
					if (nextSegmentId <= 0)
						break;

					ushort transitNodeId;
					if (prevOffset == 0) {
						transitNodeId = netManager.m_segments.m_buffer[prevSegmentId].m_startNode;
					} else if (prevOffset == 255) {
						transitNodeId = netManager.m_segments.m_buffer[prevSegmentId].m_endNode;
					} else if (nextOffset == 0) {
						transitNodeId = netManager.m_segments.m_buffer[nextSegmentId].m_startNode;
					} else {
						transitNodeId = netManager.m_segments.m_buffer[nextSegmentId].m_endNode;
					}

#if DEBUGVSTATE
					if (debug)
						Log._Debug($"OnPathFindReady: Vehicle {VehicleId}: prevSegmentId {prevSegmentId} prevLaneIndex {prevLaneIndex} transitNodeId {transitNodeId} nextSegmentId {nextSegmentId} nextLaneIndex {nextLaneIndex}");
#endif

					VehiclePositions.AddLast(new VehiclePosition(prevSegmentId, prevLaneIndex, transitNodeId, nextSegmentId, nextLaneIndex));

					// prepare next iteration
					prevSegmentId = nextSegmentId;
					prevLaneIndex = nextLaneIndex;
					prevOffset = nextOffset;
				}

				CurrentPosition = VehiclePositions.First;
#if DEBUGVSTATE
				if (debug) {
					Log._Debug($"OnPathFindReady: Vehicle {VehicleId}: Vehicle is valid now. {VehiclePositions.Count} vehicle positions. Current position: {CurrentPosition?.Value.SourceSegmentId} -> {CurrentPosition?.Value.TransitNodeId}");
					if (GetCurrentPosition() == null) {
						Log.Warning($"OnPathFindReady: Vehicle {VehicleId}: Could not determine vehicle position!");
					}
				}
#endif
			} else {
				VehiclePositions = null;
				CurrentPosition = null;
			}
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
	}
}
