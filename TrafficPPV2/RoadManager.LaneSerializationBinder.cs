using System;
using System.Runtime.Serialization;
using Transit.Framework.Network;

namespace CSL_Traffic
{
    public static partial class RoadManager
    {
        public class LaneSerializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                if (typeName.Contains("Lane"))
                    return typeof(Lane);
                if (typeName.Contains("VehicleType"))
                    return typeof(ExtendedVehicleType);

                throw new SerializationException("Error on BindToType with type '" + typeName + "' and assembly '" + assemblyName + "'.");
            }
        }
    }
}
