using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;

namespace Transit.Framework.Extenders.AI
{
    public class RoadSnappingModeProvider : Singleton<RoadSnappingModeProvider>
    {
        private readonly IDictionary<Type, IRoadSnappingMode> _instances = new Dictionary<Type, IRoadSnappingMode>();
        private readonly IDictionary<string, IRoadSnappingMode> _customSnappings = new Dictionary<string, IRoadSnappingMode>(StringComparer.InvariantCultureIgnoreCase);

        public void RegisterCustomSnapping<T>(string netinfoName)
            where T : IRoadSnappingMode, new()
        {
            var snappingType = typeof (T);

            if (!_instances.ContainsKey(snappingType))
            {
                _instances[snappingType] = new T();
            }

            _customSnappings[netinfoName] = _instances[snappingType];
        }

        public bool HasCustomSnapping(string netinfoName)
        {
            return _customSnappings.ContainsKey(netinfoName);
        }

        public IRoadSnappingMode GetCustomSnapping(string netinfoName)
        {
            return _customSnappings[netinfoName];
        }
    }
}
