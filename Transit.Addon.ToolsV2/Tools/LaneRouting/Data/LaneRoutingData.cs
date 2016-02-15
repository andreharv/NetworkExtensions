using System;
using System.Diagnostics.CodeAnalysis;

namespace Transit.Addon.ToolsV2.LaneRouting.Data
{
    [Serializable]
    public class LaneRoutingData
    {
        public ushort OriginSegmentId { get; set; }
        public uint OriginLaneId { get; set; }

        public ushort DestinationSegmentId { get; set; }
        public uint DestinationLaneId { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LaneRoutingData) obj);
        }

        protected bool Equals(LaneRoutingData other)
        {
            return 
                OriginSegmentId == other.OriginSegmentId && 
                OriginLaneId == other.OriginLaneId && 
                DestinationSegmentId == other.DestinationSegmentId && 
                DestinationLaneId == other.DestinationLaneId;
        }

        public override int GetHashCode()
        {
            return (GetOriginUniqueId() + "-" + GetDestinationUniqueId()).GetHashCode();
        }

        public string GetOriginUniqueId()
        {
            return OriginLaneId + "." + OriginSegmentId;
        }

        public string GetDestinationUniqueId()
        {
            return OriginLaneId + "." + OriginSegmentId;
        }

        public override string ToString()
        {
            return GetOriginUniqueId() + " -> " + GetDestinationUniqueId();
        }
    }
}
