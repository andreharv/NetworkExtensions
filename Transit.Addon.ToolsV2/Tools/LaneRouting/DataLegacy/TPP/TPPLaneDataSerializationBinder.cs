using System;
using System.Runtime.Serialization;

namespace Transit.Addon.ToolsV2.LaneRouting.DataLegacy.TPP
{
    public class TPPLaneDataSerializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName.Contains("Lane"))
                return typeof(TPPLaneData);
            if (typeName.Contains("VehicleType"))
                return typeof(TPPVehicleType);

            throw new SerializationException("Error on BindToType with type '" + typeName + "' and assembly '" + assemblyName + "'.");
        }
    }
}
