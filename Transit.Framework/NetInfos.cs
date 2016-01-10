using System;

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
            public const string ROAD_4L = "Medium Road";
            public const string ROAD_4L_BRIDGE = "Medium Road Bridge";
            public const string ROAD_4L_SLOPE = "Medium Road Slope";
            public const string ROAD_6L = "Large Road";
            public const string ROAD_6L_TUNNEL = "Large Road Tunnel";
            public const string ROAD_6L_GRASS = "Large Road Decoration Grass";
            public const string ROAD_6L_TREES = "Large Road Decoration Trees";
            public const string ONEWAY_2L = "Oneway Road";
            public const string ONEWAY_2L_GRASS = "Oneway Road Decoration Grass";
            public const string ONEWAY_2L_TREES = "Oneway Road Decoration Trees";
            public const string ONEWAY_2L_TUNNEL = "Oneway Road Tunnel";
            public const string ONEWAY_6L = "Large Oneway";

            public const string HIGHWAY_RAMP = "HighwayRamp";
            public const string HIGHWAY_3L = "Highway";
            public const string HIGHWAY_3L_SLOPE = "Highway Slope";
            public const string HIGHWAY_3L_TUNNEL = "Highway Tunnel";
            public const string HIGHWAY_3L_BARRIER = "Highway Barrier";

            public const string LARGE_ROAD_BRIDGE_SUSPENSION_PILLAR = "LargeRoadBridgeSuspensionPillar";

            public const string PED_GRAVEL = ROAD_2L_GRAVEL; // Quick fix for the Pedestian zonable roads
            public const string PED_PAVEMENT = "Pedestrian Pavement";
            
            public static string GetPrefabName(string groundName, NetInfoVersion version)
            {
                switch (groundName)
                {
                    case ROAD_2L:
                    case ROAD_6L:

                    case ROAD_4L:

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

                    case HIGHWAY_RAMP:
                        switch (version)
                        {
                            case NetInfoVersion.Ground:
                                return groundName;
                            case NetInfoVersion.Elevated:
                                return groundName + NetInfoVersion.Elevated;
                            case NetInfoVersion.Tunnel:
                                return groundName + " " + NetInfoVersion.Tunnel;
                            case NetInfoVersion.Slope:
                                return groundName + " " + NetInfoVersion.Slope;
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
            public const string ZONEABLE_PED_PAVEMENT = "Zonable Pedestrian Pavement";

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

                // Legacy from T++
                if (groundName == ZONEABLE_PED_PAVEMENT)
                {
                    switch (version)
                    {
                        case NetInfoVersion.Ground:
                            return groundName;
                        default:
                            return string.Format("Zonable Pedestrian {0}", version);
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
        public const string NEXT_HIGHWAY1L_TUNNEL = "NExtHighwayTunnel1LTunnel";
        public const string NEXT_HIGHWAY2L = "NExtHighway2L";
        public const string NEXT_HIGHWAY2L_TUNNEL = "NExtHighwayTunnel2LTunnel";
        public const string NEXT_HIGHWAY4L = "NExtHighway4L";
        public const string NEXT_HIGHWAY4L_TUNNEL = "NExtHighwayTunnel4LTunnel";
        public const string NEXT_HIGHWAY5L = "NExtHighway5L";
        public const string NEXT_HIGHWAY5L_TUNNEL = "NExtHighwayTunnel5LTunnel";
        public const string NEXT_HIGHWAY6L = "NExtHighway6L";
        public const string NEXT_HIGHWAY6L_TUNNEL = "NExtHighwayTunnel6LTunnel";

        public const string NEXT_SMALL3L_ROAD = "NExtSmall3LRoad";
        public const string NEXT_SMALL3L_ROAD_TUNNEL = "NExtSmall3LRoadTunnel";
        public const string NEXT_SMALL4L_ROAD = "NExtSmall4LRoad";
        public const string NEXT_SMALL4L_ROAD_TUNNEL = "NExtSmall4LRoadTunnel";
        public const string NEXT_MEDIUM_ROAD = "NExtMediumRoad";
        public const string NEXT_MEDIUM_ROAD_TUNNEL = "NExtMediumRoadTunnel";
        public const string NEXT_MEDIUM_ROAD_TL = "NExtMediumRoadTL";
        public const string NEXT_MEDIUM_ROAD_TL_TUNNEL = "NExtMediumRoadTLTunnel";
		public const string NEXT_LARGE_ROAD = "NExtLargeRoad";
        public const string NEXT_PED_ROAD = "NExtPedRoad";
    }
}
