#define DEBUGFRONTVEHx
#define DEBUGREGx
#define DEBUGMETRICx

using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using TrafficManager.Traffic;
using TrafficManager.TrafficLight;
using TrafficManager.Custom.AI;
using TrafficManager.Util;
using System.Threading;

/// <summary>
/// A segment end describes a directional traffic segment connected to a controlled node
/// (having custom traffic lights or priority signs).
/// </summary>
namespace TrafficManager.Traffic {
	public class SegmentEnd : IObserver<SegmentGeometry> {
		public enum PriorityType {
			None = 0,
			Main = 1,
			Stop = 2,
			Yield = 3
		}

		public ushort NodeId {
			get; private set;
		}

		public ushort SegmentId {
			get; private set;
		}

		private bool startNode = false;

		private int numLanes = 0;

		public PriorityType Type;

		private IDisposable geometryUnsubscriber;

		/// <summary>
		/// Vehicles that are traversing or will traverse this segment
		/// </summary>
		private HashSet<ushort> registeredVehicles;

		private bool cleanupRequested = false;

		/// <summary>
		/// Vehicles that are traversing or will traverse this segment
		/// </summary>
		//private ushort[] frontVehicleIds;

		/// <summary>
		/// Number of vehicles / vehicle length goint to a certain segment
		/// </summary>
		private Dictionary<ushort, uint> numVehiclesGoingToSegmentId;

		public SegmentEnd(ushort nodeId, ushort segmentId, PriorityType type) {
			NodeId = nodeId;
			SegmentId = segmentId;
			Type = type;
			registeredVehicles = new HashSet<ushort>();
			SegmentGeometry geometry = CustomRoadAI.GetSegmentGeometry(segmentId);
			OnUpdate(geometry);
			geometryUnsubscriber = geometry.Subscribe(this);
		}

		~SegmentEnd() {
			Destroy();
		}

		internal void RequestCleanup() {
			cleanupRequested = true;
		}

		internal void SimulationStep() {
			if (cleanupRequested) {
#if DEBUG
				//Log._Debug($"Cleanup of SegmentEnd {SegmentId} @ {NodeId} requested. Performing cleanup now.");
#endif
				ushort[] regVehs = registeredVehicles.ToArray();
				foreach (ushort vehicleId in regVehs) {
					if ((Singleton<VehicleManager>.instance.m_vehicles.m_buffer[vehicleId].m_flags & Vehicle.Flags.Created) == 0) {
						UnregisterVehicle(vehicleId);
						continue;
					}

					VehicleState state = VehicleStateManager.GetVehicleState(vehicleId);
					if (state == null) {
						UnregisterVehicle(vehicleId);
						continue;
					}

					VehiclePosition pos = state.GetCurrentPosition();
					if (pos == null) {
						UnregisterVehicle(vehicleId);
						continue;
					}
				}

				cleanupRequested = false;
			}
		}

		/// <summary>
		/// Calculates for each segment the number of cars going to this segment.
		/// We use integer arithmetic for better performance.
		/// </summary>
		public Dictionary<ushort, uint> GetVehicleMetricGoingToSegment(float? minSpeed, ExtVehicleType? vehicleTypes=null, ExtVehicleType separateVehicleTypes=ExtVehicleType.None, bool debug = false) {
			VehicleManager vehicleManager = Singleton<VehicleManager>.instance;
			NetManager netManager = Singleton<NetManager>.instance;

			for (var s = 0; s < 8; s++) {
				ushort segmentId = netManager.m_nodes.m_buffer[NodeId].GetSegment(s);

				if (segmentId == 0 || segmentId == SegmentId)
					continue;

				if (!numVehiclesGoingToSegmentId.ContainsKey(segmentId))
					continue;

				numVehiclesGoingToSegmentId[segmentId] = 0;
			}

#if DEBUGMETRIC
			Log._Debug($"GetVehicleMetricGoingToSegment: Segment {SegmentId}, Node {NodeId}. Target segments: {string.Join(", ", numVehiclesGoingToSegmentId.Keys.Select(x => x.ToString()).ToArray())}, Registered Vehicles: {string.Join(", ", GetRegisteredVehicles().Select(x => x.ToString()).ToArray())}");
#endif

			foreach (ushort vehicleId in GetRegisteredVehicles()) {
#if DEBUGMETRIC
				Log._Debug($"GetVehicleMetricGoingToSegment: Checking vehicle {vehicleId}");
#endif
				if ((Singleton<VehicleManager>.instance.m_vehicles.m_buffer[vehicleId].m_flags & Vehicle.Flags.Created) == 0) {
#if DEBUGMETRIC
					Log._Debug($"GetVehicleMetricGoingToSegment: Checking vehicle {vehicleId}: vehicle is invalid");
#endif
					RequestCleanup();
					continue;
				}

				VehicleState state = VehicleStateManager.GetVehicleState(vehicleId);
				if (state == null) {
#if DEBUGMETRIC
					Log._Debug($"GetVehicleMetricGoingToSegment: Checking vehicle {vehicleId}: state is null");
#endif
					RequestCleanup();
					continue;
				}

				VehiclePosition pos = state.GetCurrentPosition();
				if (pos == null) {
#if DEBUGMETRIC
					Log._Debug($"GetVehicleMetricGoingToSegment: Checking vehicle {vehicleId}: pos is null");
#endif
					RequestCleanup();
					continue;
				}

				/*if (pos.SourceSegmentId != SegmentId || pos.TransitNodeId != NodeId) {
#if DEBUGMETRIC
					Log._Debug($"GetVehicleMetricGoingToSegment: Checking vehicle {vehicleId}: position does not match: Expected {SegmentId} @ {NodeId}, Got {pos.SourceSegmentId} @ {pos.TransitNodeId}");
#endif
					RequestCleanup();
					continue;
				}*/

				/*if (minSpeed != null && vehicleManager.m_vehicles.m_buffer[vehicleId].GetLastFrameVelocity().magnitude < minSpeed)
					continue;*/

				if (vehicleTypes != null) {
					if (vehicleTypes == ExtVehicleType.None) {
						if ((state.VehicleType & separateVehicleTypes) != ExtVehicleType.None) {
							// we want all vehicles that do not have separate traffic lights
#if DEBUGMETRIC
							Log._Debug($"GetVehicleMetricGoingToSegment: Checking vehicle {vehicleId}: Not a special vehicle type");
#endif
							continue;
						}
					} else {
						if ((state.VehicleType & vehicleTypes) == ExtVehicleType.None) {
#if DEBUGMETRIC
							Log._Debug($"GetVehicleMetricGoingToSegment: Checking vehicle {vehicleId}: Not a generic vehicle type");
#endif
							continue;
						}
					}
				}

				//debug = vehicleManager.m_vehicles.m_buffer[vehicleId].Info.m_vehicleType == VehicleInfo.VehicleType.Tram;
#if DEBUGMETRIC
				/*if (debug) {
					Log._Debug($"getNumCarsGoingToSegment: Handling vehicle {vehicleId} going from {carPos.FromSegment}/{SegmentId} to {carPos.ToSegment}. carState={globalPos.CarState}. lastUpdate={globalPos.LastCarStateUpdate}");
                }*/
#endif

				uint avgSegmentLength = (uint)netManager.m_segments.m_buffer[SegmentId].m_averageLength;
				uint normLength = (uint)(state.TotalLength * 100u) / avgSegmentLength;

#if DEBUGMETRIC
				/*if (debug) {
					Log._Debug($"getNumCarsGoingToSegment: NormLength of vehicle {vehicleId} going to {carPos.ToSegment}: {avgSegmentLength} -> {normLength}");
				}*/
#endif

				if (numVehiclesGoingToSegmentId.ContainsKey(pos.TargetSegmentId)) {
					numVehiclesGoingToSegmentId[pos.TargetSegmentId] += normLength;
#if DEBUGMETRIC
					Log._Debug($"GetVehicleMetricGoingToSegment: Checking vehicle {vehicleId}: ADDING VEHICLE");
#endif
				}
				// "else" must not happen (incoming one-way)
			}

			return numVehiclesGoingToSegmentId;
		}

		internal void RegisterVehicle(ushort vehicleId, ref Vehicle vehicleData, VehiclePosition pos) {
			if (pos.TransitNodeId != NodeId || pos.SourceSegmentId != SegmentId) {
				Log.Warning($"Refusing to add vehicle {vehicleId} to SegmentEnd {SegmentId} @ {NodeId} (given: {pos.SourceSegmentId} @ {pos.TransitNodeId}).");
				return;
			}

#if DEBUGREG
			Log._Debug($"RegisterVehicle({vehicleId}): Registering vehicle {vehicleId} at segment {SegmentId}, {NodeId}. number of vehicles: {registeredVehicles.Count}");
#endif

			registeredVehicles.Add(vehicleId);
			/*if (isCurrentSegment)
				DetermineFrontVehicles();*/
		}

		internal void UnregisterVehicle(ushort vehicleId) {
#if DEBUGREG
			Log._Debug($"UnregisterVehicle({vehicleId}): Removing vehicle {vehicleId} from segment {SegmentId}, {NodeId}. number of vehicles: {registeredVehicles.Count}");
#endif

			registeredVehicles.Remove(vehicleId);
			//DetermineFrontVehicles();
		}

		/*internal void UpdateApproachingVehicles() {
			DetermineFrontVehicles();
		}*/

		internal HashSet<ushort> GetRegisteredVehicles() {
#if DEBUGREG
			Log._Debug($"GetRegisteredVehicles: Segment {SegmentId}. { string.Join(", ", registeredVehicles.Select(x => x.ToString()).ToArray())}");
#endif
			return registeredVehicles;
		}

		internal int GetRegisteredVehicleCount() {
			return registeredVehicles.Count;
		}

		/*internal ushort[] GetFrontVehicleIds() {
//#if DEBUGFRONTVEH
			Log._Debug($"GetFrontVehicles: Segment {SegmentId}. { string.Join(", ", frontVehicleIds.Select(x => x.ToString()).ToArray())}");
//#endif
			return frontVehicleIds;
		}*/

		internal void Destroy() {
			if (geometryUnsubscriber != null)
				geometryUnsubscriber.Dispose();
		}

		public void OnUpdate(SegmentGeometry geometry) {
			startNode = Singleton<NetManager>.instance.m_segments.m_buffer[SegmentId].m_startNode == NodeId;
			numLanes = Singleton<NetManager>.instance.m_segments.m_buffer[SegmentId].Info.m_lanes.Length;
			numVehiclesGoingToSegmentId = new Dictionary<ushort, uint>(7);
			//frontVehicleIds = new ushort[numLanes];
			ushort[] outgoingSegmentIds = geometry.GetOutgoingSegments(startNode);
			foreach (ushort otherSegmentId in outgoingSegmentIds)
				numVehiclesGoingToSegmentId[otherSegmentId] = 0;
		}

		/// <summary>
		/// For each lane determines the front vehicle
		/// </summary>
		/*private void DetermineFrontVehicles() {
#if DEBUGFRONTVEH
			Log._Debug($"DetermineFrontVehicles() of segment {SegmentId}, Node {NodeId}, startNode {startNode}");
#endif

			NetManager netManager = Singleton<NetManager>.instance;
			VehicleManager vehManager = Singleton<VehicleManager>.instance;

			byte[] frontLaneOffset = new byte[numLanes];

			if (startNode) {
				for (int i = 0; i < numLanes; ++i) {
					frontLaneOffset[i] = 255;
				}
			}

			foreach (KeyValuePair<ushort, VehiclePosition> e in registeredVehicles) {
				ushort vehicleId = e.Key;

				if ((vehManager.m_vehicles.m_buffer[vehicleId].m_flags & Vehicle.Flags.Created) == 0) {
#if DEBUGFRONTVEH
					Log.Warning(String.Format("Invalid vehicle id %d.", vehicleId));
#endif
					continue;
				}

				VehicleState state = VehicleStateManager.GetVehicleState(vehicleId);
				if (state == null) {
#if DEBUGFRONTVEH
					Log.Warning(String.Format("Could not retrieve vehicle state of vehicle id %d.", vehicleId));
#endif
					continue;
				}

				state.FrontVehicle = false;

				VehiclePosition pos = state.GetCurrentPosition();
				if (pos == null) {
#if DEBUGFRONTVEH
					Log.Warning(String.Format("Could not retrieve vehicle position of vehicle id %d.", vehicleId));
#endif
					continue;
				}

				if (pos.SourceSegmentId != SegmentId || pos.TransitNodeId != NodeId) {
#if DEBUGFRONTVEH
					Log.Warning(String.Format("Vehicle id %d is not located on segment %d.", vehicleId, SegmentId));
#endif
					continue;
				}

				byte offset = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[vehicleId].m_lastPathOffset;
#if DEBUGFRONTVEH
				Log._Debug($"DetermineFrontVehicles() of segment {SegmentId}, Node {NodeId}, startNode {startNode}. vehicleId {vehicleId} offset {offset}");
#endif
				if (pos.SourceLaneIndex < numLanes && (startNode ^ offset > frontLaneOffset[pos.SourceLaneIndex])) {
					frontLaneOffset[pos.SourceLaneIndex] = offset;
					frontVehicleIds[pos.SourceLaneIndex] = vehicleId;
#if DEBUGFRONTVEH
					Log._Debug($"DetermineFrontVehicles() of segment {SegmentId}, Node {NodeId}, startNode {startNode}. Set vehicleId {vehicleId} as front vehicle @ lane {pos.SourceLaneIndex}, offset {offset}");
#endif
				}
			}

			for (int i = 0; i < numLanes; ++i) {
				if (frontVehicleIds[i] == 0)
					continue;
				ushort vehicleId = frontVehicleIds[i];

				VehicleState state = VehicleStateManager.GetVehicleState(vehicleId);
				if (state == null)
					continue;

				state.FrontVehicle = true;
			}
		}*/
	}
}
