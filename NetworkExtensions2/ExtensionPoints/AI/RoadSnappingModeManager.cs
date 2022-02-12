using System;
using System.Collections.Generic;

namespace Transit.Framework.ExtensionPoints.AI
{
    public class RoadSnappingModeManager
    {
        private static readonly IDictionary<Type, IRoadSnappingMode> _instances = new Dictionary<Type, IRoadSnappingMode>();
        private static readonly IDictionary<string, IRoadSnappingMode> _customSnappings = new Dictionary<string, IRoadSnappingMode>(StringComparer.InvariantCultureIgnoreCase);

        public static void RegisterCustomSnapping<T>(string netinfoName)
            where T : IRoadSnappingMode, new()
        {
            var snappingType = typeof (T);

            if (!_instances.ContainsKey(snappingType))
            {
                _instances[snappingType] = new T();
            }

            _customSnappings[netinfoName] = _instances[snappingType];
        }

        public static bool HasCustomSnapping(string netinfoName)
        {
            return _customSnappings.ContainsKey(netinfoName);
        }

        public static IRoadSnappingMode GetCustomSnapping(string netinfoName)
        {
            return _customSnappings[netinfoName];
        }
    }
}
