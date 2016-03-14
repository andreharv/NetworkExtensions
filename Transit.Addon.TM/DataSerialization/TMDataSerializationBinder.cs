using System;
using System.Runtime.Serialization;
using TrafficManager.Traffic;

namespace TrafficManager.DataSerialization
{
    public class TMDataSerializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName.Contains("CustomSegmentLight"))
                return typeof(Configuration.CustomSegmentLight);
            if (typeName.Contains("CustomSegmentLights"))
                return typeof(Configuration.CustomSegmentLights);
            if (typeName.Contains("LaneSpeedLimit"))
                return typeof(Configuration.LaneSpeedLimit);
            if (typeName.Contains("LaneVehicleTypes"))
                return typeof(Configuration.LaneVehicleTypes);
            if (typeName.Contains("SegmentNodeConf"))
                return typeof(Configuration.SegmentNodeConf);
            if (typeName.Contains("SegmentNodeFlags"))
                return typeof(Configuration.SegmentNodeFlags);
            if (typeName.Contains("TimedTrafficLights"))
                return typeof(Configuration.TimedTrafficLights);
            if (typeName.Contains("TimedTrafficLightsStep"))
                return typeof(Configuration.TimedTrafficLightsStep);
            if (typeName.Contains("Configuration"))
                return typeof(Configuration);
            if (typeName.Contains("ExtVehicleType"))
                return typeof(ExtVehicleType);
            if (typeName.Contains("Configuration"))
                return typeof(Configuration);

            var type = Type.GetType(typeName, false, true);

            if (type != null)
            {
                return type;
            }

            throw new SerializationException("Error on BindToType with type '" + typeName + "' and assembly '" + assemblyName + "'.");
        }
    }
}
