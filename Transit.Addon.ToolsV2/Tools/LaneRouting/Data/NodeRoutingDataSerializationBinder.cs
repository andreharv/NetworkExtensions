using System;
using System.Runtime.Serialization;

namespace Transit.Addon.ToolsV2.LaneRouting.Data
{
    public class NodeRoutingDataSerializationBinder : SerializationBinder
    {
        private static readonly string s_nodeDataType = typeof(NodeRoutingData).Name.ToUpperInvariant();
        private static readonly string s_laneDataType = typeof(LaneRoutingData).Name.ToUpperInvariant();

        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName.ToUpperInvariant().Contains(s_nodeDataType))
                return typeof(NodeRoutingData);
            if (typeName.ToUpperInvariant().Contains(s_laneDataType))
                return typeof(LaneRoutingData);

            throw new SerializationException("Error on BindToType with type '" + typeName + "' and assembly '" + assemblyName + "'.");
        }
    }
}
