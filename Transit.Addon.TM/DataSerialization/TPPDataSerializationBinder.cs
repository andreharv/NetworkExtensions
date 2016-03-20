using System;
using System.Runtime.Serialization;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.Data.Legacy;
using Transit.Framework.Network;

namespace Transit.Addon.TM.DataSerialization
{
    public class TPPDataSerializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            // Legacy
            if (typeName.Contains("Lane"))
                return typeof(TPPLaneDataV1);
            if (typeName.Contains("VehicleType"))
                return typeof(TPPVehicleType);

            throw new SerializationException("Error on BindToType with type '" + typeName + "' and assembly '" + assemblyName + "'.");
        }
    }
}
