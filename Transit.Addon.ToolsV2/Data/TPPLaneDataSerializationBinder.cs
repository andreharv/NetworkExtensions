using System;
using System.Runtime.Serialization;
using Transit.Framework.Network;

namespace Transit.Addon.ToolsV2.Data
{
    public class TPPLaneDataSerializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName.Contains("LaneDataV2"))
                return typeof(TPPLaneDataV2);
            if (typeName.Contains("UnitType"))
                return typeof(ExtendedUnitType);

            // Legacy
            if (typeName.Contains("Lane"))
                return typeof(TPPLaneDataV1);
            if (typeName.Contains("VehicleType"))
                return typeof(TPPVehicleType);

            throw new SerializationException("Error on BindToType with type '" + typeName + "' and assembly '" + assemblyName + "'.");
        }
    }
}
