#define DEBUGHKx

using System;
using System.Collections.Generic;
using ColossalFramework;
using Transit.Addon.TM.Traffic;
using UnityEngine;
using Transit.Addon.TM.AI;
using System.Linq;
using Transit.Addon.TM.Data;

namespace Transit.Addon.TM.TrafficLight {
	public class CustomSegmentLights : ICloneable {
		private ushort nodeId;
		private ushort segmentId;

		private static readonly TMVehicleType mainVehicleType = TMVehicleType.None;

		public ushort NodeId {
			get { return nodeId; }
			private set { nodeId = value; }
		}

		public ushort SegmentId {
			get { return segmentId; }
			set { segmentId = value; housekeeping(true); }
		}

		public uint LastChangeFrame;

		public Dictionary<TMVehicleType, CustomSegmentLight> CustomLights {
			get; private set;
		} = new Dictionary<TMVehicleType, CustomSegmentLight>();

		public LinkedList<TMVehicleType> VehicleTypes {
			get; private set;
		} = new LinkedList<TMVehicleType>();

		/// <summary>
		/// Vehicles types that have their own traffic light
		/// </summary>
		public TMVehicleType SeparateVehicleTypes {
			get; private set;
		} = TMVehicleType.None;

		public RoadBaseAI.TrafficLightState? PedestrianLightState {
			get {
				if (pedestrianLightState == null)
					return null; // no pedestrian crossing at this point

				if (ManualPedestrianMode)
					return pedestrianLightState;
				else {
					return GetAutoPedestrianLightState();
				}
			}
			set {
				if (pedestrianLightState == null) {
#if DEBUGHK
					Log._Debug($"CustomSegmentLights: Refusing to change pedestrian light at segment {segmentId}");
#endif
                    return;
				}
				//Log._Debug($"CustomSegmentLights: Setting pedestrian light at segment {segmentId}");
				pedestrianLightState = value;
			}
		}

		public RoadBaseAI.TrafficLightState GetAutoPedestrianLightState() {
			RoadBaseAI.TrafficLightState? vehicleState = null;
			foreach (KeyValuePair<TMVehicleType, CustomSegmentLight> e in CustomLights) {
				RoadBaseAI.TrafficLightState visualState = e.Value.GetVisualLightState();
				if (vehicleState == null || visualState != RoadBaseAI.TrafficLightState.Red)
					vehicleState = visualState;
				if (vehicleState == RoadBaseAI.TrafficLightState.Green)
					break;
			}
			if (vehicleState == null) {
				vehicleState = RoadBaseAI.TrafficLightState.Red;
			}
			return CustomSegmentLight.InvertLight((RoadBaseAI.TrafficLightState)vehicleState);
		}

		public bool ManualPedestrianMode {
			get; set;
		} = false;

		protected RoadBaseAI.TrafficLightState? pedestrianLightState = null;
		private TMVehicleType autoPedestrianVehicleType = TMVehicleType.None;
		protected CustomSegmentLight mainSegmentLight = null;

		protected CustomSegmentLights(ushort nodeId, ushort segmentId) {
			this.nodeId = nodeId;
			this.segmentId = segmentId;
			PedestrianLightState = null;
			OnChange();
		}

		public CustomSegmentLights(ushort nodeId, ushort segmentId, RoadBaseAI.TrafficLightState mainState) {
			this.nodeId = nodeId;
			this.segmentId = segmentId;
			PedestrianLightState = null;
			OnChange();

			RoadBaseAI.TrafficLightState pedState = mainState == RoadBaseAI.TrafficLightState.Green
				? RoadBaseAI.TrafficLightState.Red
				: RoadBaseAI.TrafficLightState.Green;
			housekeeping(false, mainState, mainState, mainState, pedState);
		}

		public CustomSegmentLights(ushort nodeId, ushort segmentId, RoadBaseAI.TrafficLightState mainState, RoadBaseAI.TrafficLightState leftState, RoadBaseAI.TrafficLightState rightState, RoadBaseAI.TrafficLightState pedState) {
			this.nodeId = nodeId;
			this.segmentId = segmentId;
			PedestrianLightState = null;
			OnChange();

			housekeeping(false, mainState, leftState, rightState, pedState);
		}

		private static TMVehicleType[] singleLaneVehicleTypes = new TMVehicleType[] { TMVehicleType.Tram, TMVehicleType.Bus | TMVehicleType.Taxi, TMVehicleType.Service, TMVehicleType.CargoTruck };

		internal void housekeeping(bool mayDelete, RoadBaseAI.TrafficLightState mainState = RoadBaseAI.TrafficLightState.Red, RoadBaseAI.TrafficLightState leftState = RoadBaseAI.TrafficLightState.Red, RoadBaseAI.TrafficLightState rightState = RoadBaseAI.TrafficLightState.Red, RoadBaseAI.TrafficLightState pedState = RoadBaseAI.TrafficLightState.Red) {
			// we intentionally never delete vehicle types (because we may want to retain traffic light states if a segment is upgraded or replaced)

			HashSet<TMVehicleType> setupLights = new HashSet<TMVehicleType>();
			HashSet<TMVehicleType> allAllowedTypes = VehicleRestrictionsManager.GetAllowedVehicleTypesAsSet(segmentId, nodeId);
			TMVehicleType allAllowedMask = VehicleRestrictionsManager.GetAllowedVehicleTypes(segmentId, nodeId);
			SeparateVehicleTypes = TMVehicleType.None;
#if DEBUGHK
			Log._Debug($"CustomSegmentLights: housekeeping @ seg. {segmentId}, node {nodeId}, allAllowedTypes={string.Join(", ", allAllowedTypes.Select(x => x.ToString()).ToArray())}");
#endif
			bool addPedestrianLight = false;
			uint numLights = 0;
			foreach (TMVehicleType allowedTypes in allAllowedTypes) {
				foreach (TMVehicleType mask in singleLaneVehicleTypes) {
					if (setupLights.Contains(mask))
						continue;

					if ((allowedTypes & mask) != TMVehicleType.None && (allowedTypes & ~(mask | TMVehicleType.Emergency)) == TMVehicleType.None) {
#if DEBUGHK
						Log._Debug($"CustomSegmentLights: housekeeping @ seg. {segmentId}, node {nodeId}: adding {mask} light");
#endif

						if (!CustomLights.ContainsKey(mask)) {
							CustomLights.Add(mask, new TrafficLight.CustomSegmentLight(this, nodeId, segmentId, mainState, leftState, rightState));
							VehicleTypes.AddFirst(mask);
						}
						++numLights;
						addPedestrianLight = true;
						autoPedestrianVehicleType = mask;
						mainSegmentLight = CustomLights[mask];
						setupLights.Add(mask);
						SeparateVehicleTypes |= mask;
						break;
					}
				}
			}

			if (allAllowedTypes.Count > numLights) {
#if DEBUGHK
				Log._Debug($"CustomSegmentLights: housekeeping @ seg. {segmentId}, node {nodeId}: adding main vehicle light: {mainVehicleType}");
#endif

				// traffic lights for cars
				if (!CustomLights.ContainsKey(mainVehicleType)) {
					CustomLights.Add(mainVehicleType, new TrafficLight.CustomSegmentLight(this, nodeId, segmentId, mainState, leftState, rightState));
					VehicleTypes.AddFirst(mainVehicleType);
				}
				autoPedestrianVehicleType = mainVehicleType;
				mainSegmentLight = CustomLights[mainVehicleType];
				addPedestrianLight = allAllowedMask == TMVehicleType.None || (allAllowedMask & ~TMVehicleType.RailVehicle) != TMVehicleType.None;
			} else {
				addPedestrianLight = true;
			}

#if DEBUGHK
			if (addPedestrianLight) {
				Log._Debug($"CustomSegmentLights: housekeeping @ seg. {segmentId}, node {nodeId}: adding ped. light");
			}
#endif

			if (mayDelete) {
				// delete traffic lights for non-existing configurations
				HashSet<TMVehicleType> vehicleTypesToDelete = new HashSet<TMVehicleType>();
				foreach (KeyValuePair<TMVehicleType, CustomSegmentLight> e in CustomLights) {
					if (e.Key == mainVehicleType)
						continue;
					if (!setupLights.Contains(e.Key))
						vehicleTypesToDelete.Add(e.Key);
				}

				foreach (TMVehicleType vehicleType in vehicleTypesToDelete) {
#if DEBUGHK
					Log._Debug($"Deleting traffic light for {vehicleType} at segment {segmentId}, node {nodeId}");
#endif
					CustomLights.Remove(vehicleType);
					VehicleTypes.Remove(vehicleType);
				}
			}

			if (CustomLights.ContainsKey(mainVehicleType) && VehicleTypes.First.Value != mainVehicleType) {
				VehicleTypes.Remove(mainVehicleType);
				VehicleTypes.AddFirst(mainVehicleType);
			}

			if (addPedestrianLight) {
#if DEBUGHK
				Log._Debug($"CustomSegmentLights: housekeeping @ seg. {segmentId}, node {nodeId}: adding pedestrian light");
#endif
				if (pedestrianLightState == null)
					pedestrianLightState = pedState;
			} else {
				pedestrianLightState = null;
			}
		}

		public void UpdateVisuals() {
			if (mainSegmentLight == null)
				return;
			/*CustomSegmentLight visualLight = null;
			if (!CustomLights.TryGetValue(ExtVehicleType.RoadVehicle, out visualLight) && !CustomLights.TryGetValue(ExtVehicleType.RailVehicle, out visualLight))
				return;*/

			mainSegmentLight.UpdateVisuals();
		}
		
		public object Clone() {
			CustomSegmentLights clone = new CustomSegmentLights(nodeId, segmentId);
			foreach (KeyValuePair<TMVehicleType, CustomSegmentLight> e in CustomLights) {
				clone.CustomLights.Add(e.Key, (CustomSegmentLight)e.Value.Clone());
			}
			clone.pedestrianLightState = pedestrianLightState;
			clone.ManualPedestrianMode = ManualPedestrianMode;
			clone.VehicleTypes = new LinkedList<TMVehicleType>(VehicleTypes);
			clone.LastChangeFrame = LastChangeFrame;
			clone.autoPedestrianVehicleType = autoPedestrianVehicleType;
			//if (autoPedestrianVehicleType != ExtVehicleType.None) {
			clone.CustomLights.TryGetValue(clone.autoPedestrianVehicleType, out clone.mainSegmentLight);
			//clone.mainSegmentLight = clone.CustomLights[autoPedestrianVehicleType];
			//}
			clone.housekeeping(false);
			return clone;
		}

		internal CustomSegmentLight GetCustomLight(TMVehicleType vehicleType) {
			foreach (KeyValuePair<TMVehicleType, CustomSegmentLight> e in CustomLights) {
				if ((e.Key & vehicleType) != TMVehicleType.None)
					return e.Value;
			}

			/*if (vehicleType != ExtVehicleType.None)
				Log._Debug($"No traffic light for vehicle type {vehicleType} defined at segment {segmentId}, node {nodeId}.");*/
			
			return mainSegmentLight;
		}

		internal void MakeRed() {
			foreach (KeyValuePair<TMVehicleType, CustomSegmentLight> e in CustomLights) {
				e.Value.MakeRed();
			}
		}

		internal void MakeRedOrGreen() {
			foreach (KeyValuePair<TMVehicleType, CustomSegmentLight> e in CustomLights) {
				e.Value.MakeRedOrGreen();
			}
		}

		internal void SetLights(CustomSegmentLights otherLights) {
			foreach (KeyValuePair<TMVehicleType, CustomSegmentLight> e in otherLights.CustomLights) {
				CustomSegmentLight ourLight = null;
				if (!CustomLights.TryGetValue(e.Key, out ourLight))
					continue;

				ourLight.LightMain = e.Value.LightMain;
				ourLight.LightLeft = e.Value.LightLeft;
				ourLight.LightRight = e.Value.LightRight;
				//ourLight.LightPedestrian = e.Value.LightPedestrian;
			}
			pedestrianLightState = otherLights.pedestrianLightState;
			ManualPedestrianMode = otherLights.ManualPedestrianMode;
		}

		internal void ChangeLightPedestrian() {
			if (PedestrianLightState != null) {
				var invertedLight = PedestrianLightState == RoadBaseAI.TrafficLightState.Green
					? RoadBaseAI.TrafficLightState.Red
					: RoadBaseAI.TrafficLightState.Green;

				PedestrianLightState = invertedLight;
				UpdateVisuals();
			}
		}

		private static uint getCurrentFrame() {
			return Singleton<SimulationManager>.instance.m_currentFrameIndex >> 6;
		}

		public uint LastChange() {
			return getCurrentFrame() - LastChangeFrame;
		}

		public void OnChange() {
			LastChangeFrame = getCurrentFrame();
		}
	}
}
