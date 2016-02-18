using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.BusRoads.Busway6L
{
    public class Busway6LBuilder : Activable, IMultiNetInfoBuilderPart, ITrafficPlusPlusPart
    {
        public string Name { get { return NetInfos.New.BUSWAY_6L; } }
        public string DisplayName { get { return "[BETA] Bus Lanes for Six-Lane Road"; } }
        public string BasedPrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public int Order { get { return 130; } }
        public string ShortDescription { get { return "Parking, zoneable, medium traffic, bus lanes"; } }
        public NetInfoVersion SupportedVersions { get { return NetInfoVersion.AllWithDecoration; } }

        public IEnumerable<IMenuItemBuilder> MenuItemBuilders
        {
            get
            {
                yield return new MenuItemBuilder
                {
                    UICategory = RExExtendedMenus.ROADS_BUSWAYS,
                    UIOrder = 40,
                    Name = "Large Road With Bus Lanes",
                    DisplayName = "[BETA] Large Road With Bus Lanes",
                    Description = "A six-lane, two-way road suitable for cars with dedicated bus lanes.",
                    ThumbnailsPath = @"Roads\BusRoads\Busway6L\thumbnails.png",
                    InfoTooltipPath = @"Roads\BusRoads\Busway6L\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = RExExtendedMenus.ROADS_BUSWAYS,
                    UIOrder = 41,
                    Name = "Large Road Decoration Grass With Bus Lanes",
                    DisplayName = "[BETA] Large Road Decoration Grass With Bus Lanes",
                    Description = "A six-lane, two-way road suitable for cars with decorative grass and dedicated bus lanes.",
                    ThumbnailsPath = @"Roads\BusRoads\Busway6L\thumbnails_grass.png",
                    InfoTooltipPath = @"Roads\BusRoads\Busway6L\infotooltip_grass.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = RExExtendedMenus.ROADS_BUSWAYS,
                    UIOrder = 42,
                    Name = "Large Road Decoration Trees With Bus Lanes",
                    DisplayName = "[BETA] Large Road Decoration Grass With Bus Lanes",
                    Description = "A six-lane, two-way road suitable for cars with decorative trees and dedicated bus lanes.",
                    ThumbnailsPath = @"Roads\BusRoads\Busway6L\thumbnails_trees.png",
                    InfoTooltipPath = @"Roads\BusRoads\Busway6L\infotooltip_trees.png"
                };
            }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            switch (segment.m_forwardRequired)
                            {
                                case NetSegment.Flags.StopLeft:
                                case NetSegment.Flags.StopRight:
                                    segment.SetTextures(
                                        new TextureSet
                                           (@"Roads\BusRoads\Busway6L\Textures\Ground_Segment__MainTex.png",
                                            @"Roads\BusRoads\Busway6L\Textures\Ground_Segment__AlphaMap.png"),
                                        new LODTextureSet
                                           (@"Roads\BusRoads\Busway6L\Textures\Ground_SegmentLOD_Bus__MainTex.png",
                                            @"Roads\BusRoads\Busway6L\Textures\Ground_SegmentLOD_Bus__AlphaMap.png",
                                            @"Roads\BusRoads\Busway6L\Textures\Ground_SegmentLOD_Bus__XYSMap.png"));
                                    break;

                                case NetSegment.Flags.StopBoth:
                                    segment.SetTextures(
                                        new TextureSet
                                           (@"Roads\BusRoads\Busway6L\Textures\Ground_Segment__MainTex.png",
                                            @"Roads\BusRoads\Busway6L\Textures\Ground_Segment__AlphaMap.png"),
                                        new LODTextureSet
                                           (@"Roads\BusRoads\Busway6L\Textures\Ground_SegmentLOD_BusBoth__MainTex.png",
                                            @"Roads\BusRoads\Busway6L\Textures\Ground_SegmentLOD_BusBoth__AlphaMap.png",
                                            @"Roads\BusRoads\Busway6L\Textures\Ground_SegmentLOD_BusBoth__XYSMap.png"));
                                    break;

                                default:
                                    segment.SetTextures(
                                        new TextureSet
                                           (@"Roads\BusRoads\Busway6L\Textures\Ground_Segment__MainTex.png",
                                            @"Roads\BusRoads\Busway6L\Textures\Ground_Segment__AlphaMap.png"),
                                        new LODTextureSet
                                           (@"Roads\BusRoads\Busway6L\Textures\Ground_SegmentLOD__MainTex.png",
                                            @"Roads\BusRoads\Busway6L\Textures\Ground_SegmentLOD__AlphaMap.png",
                                            @"Roads\BusRoads\Busway6L\Textures\Ground_SegmentLOD__XYSMap.png"));
                                    break;
                            }
                        }
                    }
                    break;

                case NetInfoVersion.GroundGrass:
                case NetInfoVersion.GroundTrees:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            switch (segment.m_forwardRequired)
                            {
                                case NetSegment.Flags.StopLeft:
                                case NetSegment.Flags.StopRight:
                                    segment.SetTextures(
                                        new TextureSet
                                           (@"Roads\BusRoads\Busway6L\Textures_Grass\Ground_Segment_Bus__MainTex.png",
                                            @"Roads\BusRoads\Busway6L\Textures_Grass\Ground_Segment_Bus__AlphaMap.png"),
                                        new LODTextureSet
                                           (@"Roads\BusRoads\Busway6L\Textures_Grass\Ground_SegmentLOD_Bus__MainTex.png",
                                            @"Roads\BusRoads\Busway6L\Textures_Grass\Ground_SegmentLOD_Bus__AlphaMap.png",
                                            @"Roads\BusRoads\Busway6L\Textures_Grass\Ground_SegmentLOD_Bus__XYSMap.png"));
                                    break;

                                case NetSegment.Flags.StopBoth:
                                    segment.SetTextures(
                                        new TextureSet
                                           (@"Roads\BusRoads\Busway6L\Textures_Grass\Ground_Segment_BusBoth__MainTex.png",
                                            @"Roads\BusRoads\Busway6L\Textures_Grass\Ground_Segment_BusBoth__AlphaMap.png"),
                                        new LODTextureSet
                                           (@"Roads\BusRoads\Busway6L\Textures_Grass\Ground_SegmentLOD_BusBoth__MainTex.png",
                                            @"Roads\BusRoads\Busway6L\Textures_Grass\Ground_SegmentLOD_BusBoth__AlphaMap.png",
                                            @"Roads\BusRoads\Busway6L\Textures_Grass\Ground_SegmentLOD_BusBoth__XYSMap.png"));
                                    break;

                                default:
                                    segment.SetTextures(
                                        new TextureSet
                                           (@"Roads\BusRoads\Busway6L\Textures_Grass\Ground_Segment__MainTex.png",
                                            @"Roads\BusRoads\Busway6L\Textures_Grass\Ground_Segment__AlphaMap.png"),
                                        new LODTextureSet
                                           (@"Roads\BusRoads\Busway6L\Textures_Grass\Ground_SegmentLOD__MainTex.png",
                                            @"Roads\BusRoads\Busway6L\Textures_Grass\Ground_SegmentLOD__AlphaMap.png",
                                            @"Roads\BusRoads\Busway6L\Textures_Grass\Ground_SegmentLOD__XYSMap.png"));
                                    break;
                            }
                        }
                    }
                    break;

                case NetInfoVersion.Bridge:
                case NetInfoVersion.Elevated:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            segment.SetTextures(
                                new TextureSet
                                    (@"Roads\BusRoads\Busway6L\Textures\Elevated_Segment__MainTex.png",
                                     @"Roads\BusRoads\Busway6L\Textures\Elevated_Segment__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\BusRoads\Busway6L\Textures\Elevated_SegmentLOD__MainTex.png",
                                     @"Roads\BusRoads\Busway6L\Textures\Elevated_SegmentLOD__AlphaMap.png",
                                     @"Roads\BusRoads\Busway6L\Textures\Elevated_SegmentLOD__XYSMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Slope:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            segment.SetTextures(
                                new TextureSet
                                    (@"Roads\BusRoads\Busway6L\Textures\Slope_Segment__MainTex.png",
                                     @"Roads\BusRoads\Busway6L\Textures\Slope_Segment__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\BusRoads\Busway6L\Textures\Slope_SegmentLOD__MainTex.png",
                                     @"Roads\BusRoads\Busway6L\Textures\Slope_SegmentLOD__AlphaMap.png",
                                     @"Roads\BusRoads\Busway6L\Textures\Slope_SegmentLOD__XYS.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Tunnel:
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

            var vehiculeLanes = info
                .m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Vehicle)
                .OrderBy(l => l.m_position)
                .ToArray();

            for (int i = 0; i < vehiculeLanes.Length; i++)
            {
                var lane = vehiculeLanes[i];

                switch (i)
                {
                    case 0:
                        lane.m_laneType = NetInfo.LaneType.TransportVehicle;
                        lane.SetBusLaneProps();
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        lane.m_laneType = NetInfo.LaneType.TransportVehicle;
                        lane.SetBusLaneProps();
                        break;
                }
            }
        }
    }
}
