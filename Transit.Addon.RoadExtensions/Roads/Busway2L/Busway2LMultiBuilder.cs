using System;
using System.Collections.Generic;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Framework;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.Busway2L
{
    public class Busway2LBuilder : Activable, IMultiNetInfoBuilder
    {
        public string Name { get { return "Small Busway"; } }
        public string DisplayName { get { return Name; } }

        public string TemplateName { get { return NetInfos.Vanilla.ROAD_2L; } }

        public int Order { get { return 110; } }


        public NetInfoVersionExtended SupportedVersions
        {
            get { return NetInfoVersionExtended.All; }
        }

        public IEnumerable<NetInfo> Build()
        {
            // Ground versions
            var groundInfo = this.BuildVersion(NetInfoVersionExtended.Ground);
            var groundGrassInfo = this.BuildVersion(NetInfoVersionExtended.GroundGrass);
            var groundTreesInfo = this.BuildVersion(NetInfoVersionExtended.GroundTrees);

            // Other versions
            var elevatedInfo = this.BuildVersion(NetInfoVersionExtended.Elevated);
            var bridgeInfo = this.BuildVersion(NetInfoVersionExtended.Bridge);
            var tunnelInfo = this.BuildVersion(NetInfoVersionExtended.Tunnel);
            var slopeInfo = this.BuildVersion(NetInfoVersionExtended.Slope);

            // AI Setup
            var groundInfoAI = groundInfo.GetComponent<RoadAI>();
            var groundGrassInfoAI = groundGrassInfo.GetComponent<RoadAI>();
            var groundTreesInfoAI = groundTreesInfo.GetComponent<RoadAI>();
            
            foreach (var ai in new[] { groundInfoAI, groundGrassInfoAI, groundTreesInfoAI })
            {
                ai.m_elevatedInfo = elevatedInfo;
                ai.m_bridgeInfo = bridgeInfo;
                ai.m_tunnelInfo = tunnelInfo;
                ai.m_slopeInfo = slopeInfo;
            }

            // Returning
            yield return groundInfo;
            yield return groundGrassInfo;
            yield return groundTreesInfo;
            yield return elevatedInfo;
            yield return bridgeInfo;
            yield return tunnelInfo;
            yield return slopeInfo;
        }

        public IMenuItemConfig GetMenuItemConfig(NetInfoVersionExtended version)
        {
            switch (version)
            {
                case NetInfoVersionExtended.Ground:
                    return new MenuItemConfig
                    {
                        UICategory = AdditionnalMenus.ROADS_BUSWAYS,
                        UIOrder = 20,
                        Name = "Small Busway",
                        DisplayName = "Busway",
                        Description = "A two-lane, two-way road suitable for buses only. Busway does not allow zoning next to it!",
                        ThumbnailsPath = @"Roads\Busway2L\thumbnails.png",
                        InfoTooltipPath = @"Roads\Busway2L\infotooltip.png"
                    };
                case NetInfoVersionExtended.GroundGrass:
                    return new MenuItemConfig
                    {
                        UICategory = AdditionnalMenus.ROADS_BUSWAYS,
                        UIOrder = 21,
                        Name = "Small Busway Decoration Grass",
                        DisplayName = "Busway with Grass",
                        Description = "A two-lane, two-way road with decorative grass suitable for buses only. Busway does not allow zoning next to it!",
                        ThumbnailsPath = @"Roads\Busway2L\thumbnails_grass.png",
                        InfoTooltipPath = @"Roads\Busway2L\infotooltip_grass.png"
                    };
                case NetInfoVersionExtended.GroundTrees:
                    return new MenuItemConfig
                    {
                        UICategory = AdditionnalMenus.ROADS_BUSWAYS,
                        UIOrder = 22,
                        Name = "Small Busway Decoration Trees",
                        DisplayName = "Busway with Trees",
                        Description = "A two-lane, two-way road with decorative trees suitable for buses only. Busway does not allow zoning next to it!",
                        ThumbnailsPath = @"Roads\Busway2L\thumbnails_trees.png",
                        InfoTooltipPath = @"Roads\Busway2L\infotooltip_trees.png"
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        public void BuildUp(NetInfo info, NetInfoVersionExtended version)
        {
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersionExtended.Ground:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            switch (segment.m_forwardRequired)
                            {
                                case NetSegment.Flags.StopLeft:
                                case NetSegment.Flags.StopRight:
                                    segment.SetTextures(
                                        new TexturesSet
                                            (@"Roads\Busway2L\Textures\Ground_Segment__MainTex.png",
                                             @"Roads\Busway2L\Textures\Ground_Segment__AlphaMap.png"),
                                        new TexturesSet
                                            (@"Roads\Busway2L\Textures\Ground_SegmentLOD_Bus__MainTex.png",
                                             @"Roads\Busway2L\Textures\Ground_SegmentLOD_Bus__AlphaMap.png",
                                             @"Roads\Busway2L\Textures\Ground_SegmentLOD__XYSMap.png"));
                                    break;

                                case NetSegment.Flags.StopBoth:
                                    segment.SetTextures(
                                        new TexturesSet
                                            (@"Roads\Busway2L\Textures\Ground_Segment__MainTex.png",
                                             @"Roads\Busway2L\Textures\Ground_Segment__AlphaMap.png"),
                                        new TexturesSet
                                            (@"Roads\Busway2L\Textures\Ground_SegmentLOD_BusBoth__MainTex.png",
                                             @"Roads\Busway2L\Textures\Ground_SegmentLOD_BusBoth__AlphaMap.png",
                                             @"Roads\Busway2L\Textures\Ground_SegmentLOD__XYSMap.png"));
                                    break;

                                default:
                                    segment.SetTextures(
                                        new TexturesSet
                                            (@"Roads\Busway2L\Textures\Ground_Segment__MainTex.png",
                                             @"Roads\Busway2L\Textures\Ground_Segment__AlphaMap.png"),
                                        new TexturesSet
                                            (@"Roads\Busway2L\Textures\Ground_SegmentLOD__MainTex.png",
                                             @"Roads\Busway2L\Textures\Ground_SegmentLOD__AlphaMap.png",
                                             @"Roads\Busway2L\Textures\Ground_SegmentLOD__XYSMap.png"));
                                    break;
                            }
                        }
                    }
                    break;

                case NetInfoVersionExtended.GroundGrass:
                case NetInfoVersionExtended.GroundTrees:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            switch (segment.m_forwardRequired)
                            {
                                case NetSegment.Flags.StopLeft:
                                case NetSegment.Flags.StopRight:
                                    segment.SetTextures(
                                        new TexturesSet
                                            (@"Roads\Busway2L\Textures_Grass\Ground_Segment__MainTex.png",
                                             @"Roads\Busway2L\Textures_Grass\Ground_Segment_Bus__AlphaMap.png"));
                                    break;

                                case NetSegment.Flags.StopBoth:
                                    segment.SetTextures(
                                        new TexturesSet
                                            (@"Roads\Busway2L\Textures_Grass\Ground_Segment__MainTex.png",
                                             @"Roads\Busway2L\Textures_Grass\Ground_Segment_BusBoth__AlphaMap.png"));
                                    break;

                                default:
                                    segment.SetTextures(
                                        new TexturesSet
                                            (@"Roads\Busway2L\Textures_Grass\Ground_Segment__MainTex.png",
                                             @"Roads\Busway2L\Textures_Grass\Ground_Segment__AlphaMap.png"));
                                    break;
                            }
                        }
                    }
                    break;

                case NetInfoVersionExtended.Bridge:
                case NetInfoVersionExtended.Elevated:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            segment.SetTextures(
                                new TexturesSet
                                    (@"Roads\Busway2L\Textures\Elevated_Segment__MainTex.png",
                                     @"Roads\Busway2L\Textures\Elevated_Segment__AlphaMap.png"),
                                new TexturesSet
                                    (@"Roads\Busway2L\Textures\Elevated_SegmentLOD__MainTex.png",
                                     @"Roads\Busway2L\Textures\Elevated_SegmentLOD__AlphaMap.png",
                                     @"Roads\Busway2L\Textures\Elevated_SegmentLOD__XYSMap.png"));
                        }
                    }
                    break;

                case NetInfoVersionExtended.Slope:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            segment.SetTextures(
                                new TexturesSet
                                    (@"Roads\Busway2L\Textures\Slope_Segment__MainTex.png",
                                     @"Roads\Busway2L\Textures\Slope_Segment__AlphaMap.png"),
                                new TexturesSet
                                    (@"Roads\Busway2L\Textures\Slope_SegmentLOD__MainTex.png",
                                     @"Roads\Busway2L\Textures\Slope_SegmentLOD__AlphaMap.png",
                                     @"Roads\Busway2L\Textures\Slope_SegmentLOD__XYS.png"));
                        }
                    }
                    break;
                case NetInfoVersionExtended.Tunnel:
                    break;
            }

            ///////////////////////////
            // Templates             //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;

            foreach (var lane in info.m_lanes)
            {
                if (lane.m_laneType == NetInfo.LaneType.Vehicle)
                {
                    lane.m_speedLimit = 1.6f;
                    lane.m_laneType = NetInfo.LaneType.TransportVehicle;
                }
            }

            var roadBaseAI = info.GetComponent<RoadBaseAI>();

            if (roadBaseAI != null)
            {
            }

            var roadAI = info.GetComponent<RoadAI>();

            if (roadAI != null)
            {
                roadAI.m_enableZoning = false;
            }
        }
    }
}
