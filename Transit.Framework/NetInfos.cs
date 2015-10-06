using System;
using Transit.Framework.Modularity;

namespace Transit.Framework
{
    public static class NetInfos
    {
        public static class Vanilla
        {
            public const string ROAD_2L = "Basic Road";
            public const string ROAD_6L = "Large Road";

            public const string AVENUE_4L = "Medium Road";

            public const string ONEWAY_2L = "Oneway Road";
            public const string ONEWAY_6L = "Large Oneway";

            public const string HIGHWAY_3L = "Highway";

            public static string GetPrefabName(string groundName, NetInfoVersion version)
            {
                switch (groundName)
                {
                    case ROAD_2L:
                    case ROAD_6L:

                    case AVENUE_4L:

                    case ONEWAY_2L:

                    case HIGHWAY_3L:
                        switch (version)
                        {
                            case NetInfoVersion.Ground:
                                return groundName;
                            case NetInfoVersion.Elevated:
                            case NetInfoVersion.Bridge:
                            case NetInfoVersion.Tunnel:
                            case NetInfoVersion.Slope:
                                return groundName + " " + version;
                            default:
                                throw new NotImplementedException();
                        }

                    case ONEWAY_6L:
                        switch (version)
                        {
                            case NetInfoVersion.Ground:
                                return groundName;
                            case NetInfoVersion.Elevated:
                                return groundName + " " + NetInfoVersion.Elevated;
                            case NetInfoVersion.Bridge:
                                return groundName + " " + NetInfoVersion.Bridge;
                            case NetInfoVersion.Tunnel:
                                return groundName + " Road Tunnel";
                            case NetInfoVersion.Slope:
                                return groundName + " Road Slope";
                            default:
                                throw new NotImplementedException();
                        }

                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }

    public static class NetInfoClasses
    {
        public const string NEXT_HIGHWAY = "NExtHighway";
        public const string NEXT_SMALL3L_ROAD = "NExtSmall3LRoad";
        public const string NEXT_SMALL4L_ROAD = "NExtSmall4LRoad";
        public const string NEXT_MEDIUM_ROAD = "NExtMediumRoad";
		public const string NEXT_LARGE_ROAD = "NExtLargeRoad";
    }
}
