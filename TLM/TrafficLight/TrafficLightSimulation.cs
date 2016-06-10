using System;
using ColossalFramework;
using TrafficManager.Traffic;
using System.Collections.Generic;
using TrafficManager.State;
using TrafficManager.Custom.AI;
using System.Linq;

namespace TrafficManager.TrafficLight {
	public class TrafficLightSimulation {
		/// <summary>
		/// For each node id: traffic light simulation assigned to the node
		/// </summary>
		public static Dictionary<ushort, TrafficLightSimulation> LightSimulationByNodeId = new Dictionary<ushort, TrafficLightSimulation>();

		/// <summary>
		/// Timed traffic light by node id
		/// </summary>
		public TimedTrafficLights TimedLight {
			get; private set;
		} = null;

		public readonly ushort nodeId;

		private bool manualTrafficLights = false;

		public void SetupManualTrafficLight() {
			if (IsTimedLight())
				return;
			manualTrafficLights = true;

			setupLiveSegments();
		}

		public void DestroyManualTrafficLight() {
			if (IsTimedLight())
				return;
			manualTrafficLights = false;

			destroyLiveSegments();
		}

		public void SetupTimedTrafficLight(List<ushort> nodeGroup) {
			if (IsManualLight())
				DestroyManualTrafficLight();

			TimedLight = new TimedTrafficLights(nodeId, nodeGroup);

			//setupLiveSegments();
		}

		public void DestroyTimedTrafficLight() {
			var timedLight = TimedLight;
			TimedLight = null;

			if (timedLight != null)
				timedLight.Stop();

			if (!IsManualLight() && timedLight != null)
				timedLight.Destroy();
		}

        public TrafficLightSimulation(ushort nodeId) {
            this.nodeId = nodeId;
		}

		public bool IsTimedLight() {
			return TimedLight != null;
		}

		public bool IsManualLight() {
			return manualTrafficLights;
		}

		public bool IsTimedLightActive() {
			return IsTimedLight() && TimedLight.IsStarted();
		}

		public bool IsSimulationActive() {
			return IsManualLight() || IsTimedLightActive();
		}

		public static void SimulationStep() {
			try {
				foreach (KeyValuePair<ushort, TrafficLightSimulation> e in LightSimulationByNodeId) {
					try {
						var nodeSim = e.Value;
						if (nodeSim.IsTimedLightActive())
							nodeSim.TimedLight.SimulationStep();
					} catch (Exception ex) {
						Log.Warning($"Error occured while simulating traffic light @ node {e.Key}: {ex.ToString()}");
					}
				}
			} catch (Exception ex) {
				// TODO the dictionary was modified (probably a segment connected to a traffic light was changed/removed). rework this
				Log.Warning($"Error occured while iterating over traffic light simulations: {ex.ToString()}");
			}
		}

		/// <summary>
		/// Stops & destroys the traffic light simulation(s) at this node (group)
		/// </summary>
		public void Destroy(bool destroyGroup) {
			if (TimedLight != null) {
				List<ushort> oldNodeGroup = new List<ushort>(TimedLight.NodeGroup);
				foreach (var timedNodeId in oldNodeGroup) {
					var otherNodeSim = GetNodeSimulation(timedNodeId);
					if (otherNodeSim == null) {
						continue;
					}

					if (destroyGroup || timedNodeId == nodeId) {
						//Log._Debug($"Slave: Removing simulation @ node {timedNodeId}");
						otherNodeSim.DestroyTimedTrafficLight();
						otherNodeSim.DestroyManualTrafficLight();
						LightSimulationByNodeId.Remove(timedNodeId);
					} else {
						if (!otherNodeSim.IsTimedLight()) {
							Log.Warning($"Unable to destroy timed traffic light of group. Node {timedNodeId} is not a timed traffic light.");
						} else {
							otherNodeSim.TimedLight.RemoveNodeFromGroup(nodeId);
						}
					}
				}
			}

			//Flags.setNodeTrafficLight(nodeId, false);
			DestroyTimedTrafficLight();
			DestroyManualTrafficLight();
			LightSimulationByNodeId.Remove(nodeId);
		}

		/// <summary>
		/// Adds a traffic light simulation to the node with the given id
		/// </summary>
		/// <param name="nodeId"></param>
		public static TrafficLightSimulation AddNodeToSimulation(ushort nodeId) {
			if (LightSimulationByNodeId.ContainsKey(nodeId)) {
				return LightSimulationByNodeId[nodeId];
			}
			LightSimulationByNodeId.Add(nodeId, new TrafficLightSimulation(nodeId));
			return LightSimulationByNodeId[nodeId];
		}

		public static void RemoveNodeFromSimulation(ushort nodeId, bool destroyGroup) {
			if (!LightSimulationByNodeId.ContainsKey(nodeId))
				return;
			TrafficLightSimulation.LightSimulationByNodeId[nodeId].Destroy(destroyGroup);
		}

		public static TrafficLightSimulation GetNodeSimulation(ushort nodeId) {
			if (LightSimulationByNodeId.ContainsKey(nodeId)) {
				return LightSimulationByNodeId[nodeId];
			}

			return null;
		}

		internal static void OnLevelUnloading() {
			LightSimulationByNodeId.Clear();
		}

		internal void handleNewSegments() {
			if (IsTimedLight())
				TimedLight.handleNewSegments();
		}

		internal void housekeeping(bool mayDeleteSegmentLights) {
			for (var s = 0; s < 8; s++) {
				var segmentId = Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].GetSegment(s);

				if (segmentId == 0) continue;
				if (TrafficLight.CustomTrafficLights.IsSegmentLight(nodeId, segmentId)) {
					TrafficLight.CustomTrafficLights.GetSegmentLights(nodeId, segmentId).housekeeping(mayDeleteSegmentLights);
				}
			}

			if (IsTimedLight()) {
				TimedLight.housekeeping(mayDeleteSegmentLights);
			}
		}

		private void setupLiveSegments() {
			for (var s = 0; s < 8; s++) {
				var segmentId = Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].GetSegment(s);

				if (segmentId == 0)
					continue;
				CustomRoadAI.GetSegmentGeometry(segmentId)?.Recalculate(true, true);
				TrafficLight.CustomTrafficLights.AddSegmentLights(nodeId, segmentId);
			}
		}

		private void destroyLiveSegments() {
			for (var s = 0; s < 8; s++) {
				var segmentId = Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].GetSegment(s);

				if (segmentId == 0) continue;
				if (TrafficLight.CustomTrafficLights.IsSegmentLight(nodeId, segmentId)) {
					TrafficLight.CustomTrafficLights.RemoveSegmentLight(nodeId, segmentId);
				}
			}
		}
	}
}
