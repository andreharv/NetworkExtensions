using System;
using Transit.Framework.Builders;

namespace Transit.Framework
{
    public static class NetInfos
    {
        public static class Vanilla
        {
            public const string ROAD_2L_GRAVEL = "Gravel Road";
            public const string ROAD_2L = "Basic Road";
            public const string ROAD_2L_GRASS = "Basic Road Decoration Grass";
            public const string ROAD_2L_TREES = "Basic Road Decoration Trees";
            public const string ROAD_6L = "Large Road";
            public const string ROAD_6L_GRASS = "Large Road Decoration Grass";
            public const string ROAD_6L_TREES = "Large Road Decoration Trees";

            public const string AVENUE_4L = "Medium Road";

            public const string ONEWAY_2L = "Oneway Road";
            public const string ONEWAY_2L_GRASS = "Oneway Road Decoration Grass";
            public const string ONEWAY_2L_TREES = "Oneway Road Decoration Trees";
            public const string ONEWAY_6L = "Large Oneway";

            public const string HIGHWAY_3L = "Highway";

            public const string PED_GRAVEL = ROAD_2L_GRAVEL; // Quick fix for the Pedestian zonable roads
            public const string PED_PAVEMENT = "Pedestrian Pavement";
            
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
                            case NetInfoVersion.GroundGrass:
                                return groundName + " Decoration Grass";
                            case NetInfoVersion.GroundTrees:
                                return groundName + " Decoration Trees";
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

                    case PED_GRAVEL:
                        switch (version)
                        {
                            case NetInfoVersion.Ground:
                                return groundName;
                            case NetInfoVersion.Elevated:
                            case NetInfoVersion.Bridge:
                                return "Pedestrian Elevated";
                            default:
                                throw new NotImplementedException();
                        }

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static class New
        {
            // Legacy from T++
            public const string BUSWAY_6L = "Large Road With Bus Lanes";

            public static string GetPrefabName(string groundName, NetInfoVersion version)
            {
                // Legacy from T++
                if (groundName == BUSWAY_6L)
                {
                    switch (version)
                    {
                        case NetInfoVersion.Ground:
                            return groundName;
                        case NetInfoVersion.GroundGrass:
                            return "Large Road Decoration Grass With Bus Lanes";
                        case NetInfoVersion.GroundTrees:
                            return "Large Road Decoration Trees With Bus Lanes";
                        default:
                            return string.Format("Large Road {0} With Bus Lanes", version);
                    }
                }

                switch (version)
                {
                    case NetInfoVersion.Ground:
                        return groundName;
                    case NetInfoVersion.GroundGrass:
                        return groundName + " Decoration Grass";
                    case NetInfoVersion.GroundTrees:
                        return groundName + " Decoration Trees";

                    default:
                        return groundName + " " + version;
                }
            }
        }
    }

    public static class NetInfoClasses
    {
        public const string NEXT_HIGHWAY1L = "NExtHighway1L";
        public const string NEXT_HIGHWAY2L = "NExtHighway2L";
        public const string NEXT_HIGHWAY4L = "NExtHighway4L";
        public const string NEXT_HIGHWAY5L = "NExtHighway5L";
        public const string NEXT_HIGHWAY6L = "NExtHighway6L";

        public const string NEXT_SMALL3L_ROAD = "NExtSmall3LRoad";
        public const string NEXT_SMALL4L_ROAD = "NExtSmall4LRoad";
        public const string NEXT_MEDIUM_ROAD = "NExtMediumRoad";
        public const string NEXT_PED_ROAD = "NExtPedRoad";
    }
}
