using System;
using System.Runtime.Serialization;
using Transit.Addon.TM.Data;
using Transit.Framework.Network;

namespace Transit.Addon.TM.DataSerialization
{
    public class TAMDataSerializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName.Contains("LaneRestriction"))
                return typeof(TAMLaneRestriction);
            if (typeName.Contains("LaneRoute"))
                return typeof(TAMLaneRoute);
            if (typeName.Contains("LaneSpeedLimit"))
                return typeof(TAMLaneSpeedLimit);
            if (typeName.Contains("UnitType"))
                return typeof(ExtendedUnitType);

            throw new SerializationException("Error on BindToType with type '" + typeName + "' and assembly '" + assemblyName + "'.");
        }
    }
}
