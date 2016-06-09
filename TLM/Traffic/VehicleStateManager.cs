using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficManager.Custom.AI;
using UnityEngine;

namespace TrafficManager.Traffic {
	class VehicleStateManager {
		/// <summary>
		/// Known vehicles and their current known positions. Index: vehicle id
		/// </summary>
		private static VehicleState[] VehicleStates = null;

		static VehicleStateManager() {
			VehicleStates = new VehicleState[VehicleManager.MAX_VEHICLE_COUNT];
			for (ushort i = 0; i < VehicleManager.MAX_VEHICLE_COUNT; ++i) {
				VehicleStates[i] = new VehicleState(i);
			}
		}

		public static VehicleState GetVehicleState(ushort vehicleId) {
			VehicleState ret = VehicleStates[vehicleId];
			if (ret.Valid)
				return ret;
			return null;
		}

		public static VehiclePosition GetVehiclePosition(ushort vehicleId) {
			VehicleState state = GetVehicleState(vehicleId);
			if (state == null)
				return null;
			return state.GetCurrentPosition();
		}

		internal static void OnLevelUnloading() {
			for (int i = 0; i < VehicleStates.Length; ++i)
				VehicleStates[i].Valid = false;
		}

		internal static void UpdateVehiclePos(ushort vehicleId, ref Vehicle vehicleData, ref PathUnit.Position currentPos) {
			VehicleStates[vehicleId].UpdatePosition(ref vehicleData, ref currentPos);
		}

		internal static void UpdateVehiclePos(ushort vehicleId, ref Vehicle vehicleData) {
			VehicleStates[vehicleId].UpdatePosition(ref vehicleData);
		}

		internal static void LogTraffic(ushort vehicleId, ref Vehicle vehicleData, bool logSpeed) {
			VehiclePosition pos = GetVehiclePosition(vehicleId);
			if (pos == null)
				return;

			CustomRoadAI.AddTraffic(pos.SourceSegmentId, pos.SourceLaneIndex, (ushort)Mathf.RoundToInt(vehicleData.CalculateTotalLength(vehicleId)), logSpeed ? (ushort?)Mathf.RoundToInt(vehicleData.GetLastFrameData().m_velocity.magnitude) : null);
		}

		internal static void OnReleaseVehicle(ushort vehicleId, ref Vehicle vehicleData) {
			VehicleStates[vehicleId].Reset();
		}

		internal static void OnPathFindReady(ushort vehicleId, ref Vehicle vehicleData) {
			//Log._Debug($"VehicleStateManager: OnPathFindReady({vehicleId})");
			VehicleStates[vehicleId].OnPathFindReady(ref vehicleData);
		}

		internal static void InitAllVehicles() {
			//Log._Debug("VehicleStateManager: InitAllVehicles()");
			VehicleManager vehicleManager = Singleton<VehicleManager>.instance;
			for (ushort i = 0; i < vehicleManager.m_vehicles.m_size; ++i) {
				try {
					OnPathFindReady(i, ref Singleton<VehicleManager>.instance.m_vehicles.m_buffer[i]);
				} catch (Exception e) {
					Log.Error("VehicleStateManager: InitAllVehicles Error: " + e.ToString());
				}
			}
		}
	}
}
