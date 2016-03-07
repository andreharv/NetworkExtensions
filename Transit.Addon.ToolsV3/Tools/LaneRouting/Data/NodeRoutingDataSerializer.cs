using Transit.Framework.Serialization;

namespace Transit.Addon.ToolsV3.LaneRouting.Data
{
    public class NodeRoutingDataSerializer : DataSerializer<NodeRoutingData[], NodeRoutingDataSerializationBinder>
    {
        public override string DataId
        {
            get { return "TAM_NodeRoutingData"; }
        }
    }
}
