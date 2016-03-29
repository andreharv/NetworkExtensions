using System;
using System.Collections.Generic;
using System.Threading;
using ColossalFramework;
using Transit.Addon.TM.PathFindingFeatures;
using Transit.Addon.TM.Traffic;
using Transit.Framework;
using Transit.Framework.Network;

namespace Transit.Addon.TM.Data {

    public class Flags {

		public static readonly uint lfr = (uint)NetLane.Flags.LeftForwardRight;

		/// <summary>
		/// For each node: Defines if a traffic light exists or not. If no entry exists for a given node id, the game's default setting is used
		/// </summary>
		private static bool?[] nodeTrafficLightFlag = null;

		/// <summary>
		/// For each segment and node: Defines additional flags for segments at a node
		/// </summary>
		private static TMConfigurationV2.SegmentEndFlags[][] segmentNodeFlags = null;

		private static bool initDone = false;

		public static bool IsInitDone() {
			return initDone;
		}

		public static void resetTrafficLights(bool all) {
			for (ushort i = 0; i < Singleton<NetManager>.instance.m_nodes.m_size; ++i) {
				nodeTrafficLightFlag[i] = null;
				if (! all && TrafficPriority.IsPriorityNode(i))
					continue;
				Singleton<NetManager>.instance.UpdateNodeFlags(i);
			}
		}

		public static bool mayHaveTrafficLight(ushort nodeId) {
			if (nodeId <= 0)
				return false;

			if ((Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_flags & NetNode.Flags.Created) == NetNode.Flags.None) {
				//Log.Message($"Flags: Node {nodeId} may not have a traffic light (not created)");
				Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_flags = NetNode.Flags.None;
				return false;
			}

			ItemClass connectionClass = Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].Info.GetConnectionClass();
			if ((Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_flags & NetNode.Flags.Junction) == NetNode.Flags.None &&
				connectionClass.m_service != ItemClass.Service.PublicTransport
				) {
				Log._Debug($"Flags: Node {nodeId} may not have a traffic light");
				return false;
			}

			if (connectionClass == null ||
				(connectionClass.m_service != ItemClass.Service.Road &&
				connectionClass.m_service != ItemClass.Service.PublicTransport))
				return false;

			return true;
		}

		public static void setNodeTrafficLight(ushort nodeId, bool flag) {
			if (nodeId <= 0)
				return;

			Log._Debug($"Flags: Set node traffic light: {nodeId}={flag}");

			if (!mayHaveTrafficLight(nodeId)) {
				Log.Warning($"Flags: Refusing to add/delete traffic light to/from node: {nodeId} {flag}");
				return;
			}

			nodeTrafficLightFlag[nodeId] = flag;
			applyNodeTrafficLightFlag(nodeId);
		}

		internal static bool? isNodeTrafficLight(ushort nodeId) {
			if (nodeId <= 0)
				return false;

			if ((Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_flags & NetNode.Flags.Created) == NetNode.Flags.None)
				return false;

			return nodeTrafficLightFlag[nodeId];
		}

		public static bool getUTurnAllowed(ushort segmentId, bool startNode) {
			if (!IsInitDone())
				return false;

			int index = startNode ? 0 : 1;

			TMConfigurationV2.SegmentEndFlags[] nodeFlags = segmentNodeFlags[segmentId];
			if (nodeFlags == null || nodeFlags[index] == null || nodeFlags[index].uturnAllowed == null)
				return TMDataManager.Options.allowUTurns;
			return (bool)nodeFlags[index].uturnAllowed;
		}

		public static void setUTurnAllowed(ushort segmentId, bool startNode, bool value) {
			bool? valueToSet = value;
			if (value == TMDataManager.Options.allowUTurns)
				valueToSet = null;

			int index = startNode ? 0 : 1;
			if (segmentNodeFlags[segmentId][index] == null) {
				if (valueToSet == null)
					return;

				segmentNodeFlags[segmentId][index] = new TMConfigurationV2.SegmentEndFlags();
			}
			segmentNodeFlags[segmentId][index].uturnAllowed = valueToSet;
		}

		public static bool getStraightLaneChangingAllowed(ushort segmentId, bool startNode) {
			if (!IsInitDone())
				return false;

			int index = startNode ? 0 : 1;

			TMConfigurationV2.SegmentEndFlags[] nodeFlags = segmentNodeFlags[segmentId];
			if (nodeFlags == null || nodeFlags[index] == null || nodeFlags[index].straightLaneChangingAllowed == null)
				return TMDataManager.Options.allowLaneChangesWhileGoingStraight;
			return (bool)nodeFlags[index].straightLaneChangingAllowed;
		}

		public static void setStraightLaneChangingAllowed(ushort segmentId, bool startNode, bool value) {
			bool? valueToSet = value;
			if (value == TMDataManager.Options.allowLaneChangesWhileGoingStraight)
				valueToSet = null;

			int index = startNode ? 0 : 1;
			if (segmentNodeFlags[segmentId][index] == null) {
				if (valueToSet == null)
					return;
				segmentNodeFlags[segmentId][index] = new TMConfigurationV2.SegmentEndFlags();
			}
			segmentNodeFlags[segmentId][index].straightLaneChangingAllowed = valueToSet;
		}

		public static bool getEnterWhenBlockedAllowed(ushort segmentId, bool startNode) {
			if (!IsInitDone())
				return false;

			int index = startNode ? 0 : 1;

			TMConfigurationV2.SegmentEndFlags[] nodeFlags = segmentNodeFlags[segmentId];
			if (nodeFlags == null || nodeFlags[index] == null || nodeFlags[index].enterWhenBlockedAllowed == null)
				return TMDataManager.Options.allowEnterBlockedJunctions;
			return (bool)nodeFlags[index].enterWhenBlockedAllowed;
		}

		public static void setEnterWhenBlockedAllowed(ushort segmentId, bool startNode, bool value) {
			bool? valueToSet = value;
			if (value == TMDataManager.Options.allowEnterBlockedJunctions)
				valueToSet = null;

			int index = startNode ? 0 : 1;
			if (segmentNodeFlags[segmentId][index] == null) {
				if (valueToSet == null)
					return;
				segmentNodeFlags[segmentId][index] = new TMConfigurationV2.SegmentEndFlags();
			}
			segmentNodeFlags[segmentId][index].enterWhenBlockedAllowed = valueToSet;
		}

		internal static void SetSegmentEndFlags(ushort segmentId, bool startNode, TMConfigurationV2.SegmentEndFlags flags) {
			if (flags == null)
				return;

			int index = startNode ? 0 : 1;
			segmentNodeFlags[segmentId][index] = flags;
		}

		internal static TMConfigurationV2.SegmentEndFlags GetSegmentEndFlags(ushort segmentId, bool startNode) {
			int index = startNode ? 0 : 1;
			return segmentNodeFlags[segmentId][index];
		}

		public static void applyAllFlags() {
			for (ushort i = 0; i < nodeTrafficLightFlag.Length; ++i) {
				applyNodeTrafficLightFlag(i);
			}

		    TAMLaneRoutingManager.instance.ApplyAll();
		}

		public static void applyNodeTrafficLightFlag(ushort nodeId) {
			bool? flag = nodeTrafficLightFlag[nodeId];
			if (nodeId <= 0 || flag == null)
				return;

			bool mayHaveLight = mayHaveTrafficLight(nodeId);
			if ((bool)flag && mayHaveLight) {
				//Log.Message($"Adding traffic light @ node {nodeId}");
				Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_flags |= NetNode.Flags.TrafficLights;
			} else {
				//Log.Message($"Removing traffic light @ node {nodeId}");
				Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_flags &= ~NetNode.Flags.TrafficLights;
				if (!mayHaveLight) {
					Log.Warning($"Flags: Refusing to apply traffic light flag at node {nodeId}");
					nodeTrafficLightFlag[nodeId] = null;
				}
			}
		}

		internal static void OnLevelUnloading() {
			initDone = false;

			nodeTrafficLightFlag = null;
			segmentNodeFlags = null;
		}

		public static void OnBeforeLoadData() {
			if (initDone)
				return;
            
			//laneAllowedVehicleTypesArray = new ExtendedUnitType?[Singleton<NetManager>.instance.m_segments.m_size][];
			nodeTrafficLightFlag = new bool?[Singleton<NetManager>.instance.m_nodes.m_size];
			segmentNodeFlags = new TMConfigurationV2.SegmentEndFlags[Singleton<NetManager>.instance.m_segments.m_size][];
			for (int i = 0; i < segmentNodeFlags.Length; ++i) {
				segmentNodeFlags[i] = new TMConfigurationV2.SegmentEndFlags[2];
			}
			initDone = true;
		}
	}
}
