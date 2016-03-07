using Transit.Framework.Serialization;

namespace Transit.Addon.ToolsV2.LaneRouting.DataLegacy.TPP
{
    public class TPPLaneSerializer : DataSerializer<TPPLaneData[], TPPLaneDataSerializationBinder>
    {
        public override string DataId
        {
            get { return "Traffic++_RoadManager_Lanes"; }
        }
    }
}
