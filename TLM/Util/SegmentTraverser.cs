using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficManager.Custom.AI;
using TrafficManager.Traffic;

namespace TrafficManager.Util {
	public class SegmentTraverser {
		public enum TraverseDirection {
			Both,
			Incoming,
			Outgoing
		}

		/// <summary>
		/// Performs a Depth-First traversal over the cached segment geometry structure. At each traversed segment, the given `visitor` is notified. It then can update the current `state`.
		/// </summary>
		/// <param name="initialSegmentGeometry">Specifies the segment at which the traversal should start.</param>
		/// <param name="nextNodeIsStartNode">Specifies if the next node to traverse is the start node of the initial segment.</param>
		/// <param name="direction">Specifies if traffic should be able to flow towards the initial segment (Incoming) or should be able to flow from the initial segment (Outgoing) or in both directions (Both).</param>
		/// <param name="maximumDepth">Specifies the maximum depth to explore. At a depth of 0, no segment will be traversed (event the initial segment will be omitted).</param>
		/// <param name="visitor">Specifies the stateful visitor that should be notified as soon as a traversable segment (which has not been traversed before) is found.</param>
		public static void Traverse(SegmentGeometry initialSegmentGeometry, bool nextNodeIsStartNode, TraverseDirection direction, int maximumDepth, IVisitor<SegmentGeometry> visitor) {
			if (maximumDepth <= 0) {
				return;
			}

			if (visitor.Visit(initialSegmentGeometry)) {
				HashSet<ushort> visitedSegmentIds = new HashSet<ushort>();
				visitedSegmentIds.Add(initialSegmentGeometry.SegmentId);

				TraverseRec(initialSegmentGeometry, nextNodeIsStartNode, direction, maximumDepth - 1, visitor, visitedSegmentIds);
			}
		}

		private static void TraverseRec(SegmentGeometry prevSegmentGeometry, bool prevNodeIsStartNode, TraverseDirection direction, int maximumDepth, IVisitor<SegmentGeometry> visitor, HashSet<ushort> visitedSegmentIds) {
			if (maximumDepth <= 0) {
				return;
			}

			// collect next segment ids to traverse
			ushort[] nextSegmentIds;
			switch (direction) {
				case TraverseDirection.Both:
				default:
					nextSegmentIds = prevSegmentGeometry.GetConnectedSegments(prevNodeIsStartNode);
					break;
				case TraverseDirection.Incoming:
					nextSegmentIds = prevSegmentGeometry.GetIncomingSegments(prevNodeIsStartNode);
					break;
				case TraverseDirection.Outgoing:
					nextSegmentIds = prevSegmentGeometry.GetOutgoingSegments(prevNodeIsStartNode);
					break;
			}

			ushort prevNodeId = prevSegmentGeometry.GetNodeId(prevNodeIsStartNode);

			// explore next segments
			foreach (ushort nextSegmentId in nextSegmentIds) {
				if (visitedSegmentIds.Contains(nextSegmentId))
					continue;
				visitedSegmentIds.Add(nextSegmentId);

				SegmentGeometry nextSegmentGeometry = CustomRoadAI.GetSegmentGeometry(nextSegmentId);
				if (visitor.Visit(nextSegmentGeometry)) {

					bool nextNodeIsStartNode = nextSegmentGeometry.StartNodeId() != prevNodeId;
					TraverseRec(nextSegmentGeometry, nextNodeIsStartNode, direction, maximumDepth - 1, visitor, visitedSegmentIds);
				}
			}
		}
	}
}
