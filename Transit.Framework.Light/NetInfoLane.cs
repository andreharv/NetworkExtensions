using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework;

namespace Transit.Framework.Light
{
    public class NetInfoLane : NetInfo.Lane
    {
        public ExtendedVehicleType m_allowedVehicleTypes;

        public NetInfoLane(ExtendedVehicleType vehicleTypes)
        {
            this.m_allowedVehicleTypes = vehicleTypes;
        }

        public NetInfoLane(NetInfo.Lane lane) : this(lane, ExtendedVehicleType.All) { }

        public NetInfoLane(NetInfo.Lane lane, ExtendedVehicleType vehicleTypes) : this(vehicleTypes)
        {
            this.ShallowCloneFrom(lane);
        }
    }

    public static class Helper
    {
        public static T ShallowCloneFrom<T>(this T destination, T source, params string[] omitMembers)
            where T : new()
        {
            foreach (FieldInfo f in source.GetType().GetAllFields(true))
            {
                if (omitMembers.Contains(f.Name))
                {
                    continue;
                }

                f.SetValue(destination, f.GetValue(source));
            }

            return destination;
        }
    }
}
