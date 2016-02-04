using System;
using System.Collections.Generic;

namespace Transit.Framework.ExtensionPoints.AI
{
    public class RoadZoneBlocksCreationManager
    {
        private static readonly IDictionary<Type, IZoneBlocksCreator> _instances = new Dictionary<Type, IZoneBlocksCreator>();
        private static readonly IDictionary<string, IZoneBlocksCreator> _customCreators = new Dictionary<string, IZoneBlocksCreator>(StringComparer.InvariantCultureIgnoreCase);

        public static void RegisterCustomCreator<T>(string netinfoName)
            where T : IZoneBlocksCreator, new()
        {
            var creatorType = typeof (T);

            if (!_instances.ContainsKey(creatorType))
            {
                _instances[creatorType] = new T();
            }
            
            _customCreators[netinfoName] = _instances[creatorType];
        }

        public static bool HasCustomCreator(string netinfoName)
        {
            return _customCreators.ContainsKey(netinfoName);
        }

        public static IZoneBlocksCreator GetCustomCreator(string netinfoName)
        {
            return _customCreators[netinfoName];
        }
    }
}
