using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Light;
using Transit.Framework.Texturing;

namespace Transit.Addon.RoadExtensions.Roads.BusRoads.Busway2L1W
{
    public class Busway2L1WBuilder : Activable, IMultiNetInfoBuilderPart, ITrafficPlusPlusPart
    {
        public string Name { get { return "Small Busway OneWay"; } }
        public string DisplayName { get { return "[BETA] Busway OneWay"; } }
        public string BasedPrefabName { get { return NetInfos.Vanilla.ONEWAY_2L; } }
        public int Order { get { return 120; } }
        public string ShortDescription { get { return "No parking, not zoneable, buses only [Traffic++ V2 required]"; } }
        public NetInfoVersion SupportedVersions { get { return NetInfoVersion.AllWithDecoration; } }

        public IEnumerable<IMenuItemBuilder> MenuItemBuilders
        {
            get
            {
                yield return new MenuItemBuilder
                {
                    UICategory = RExExtendedMenus.ROADS_BUSWAYS,
                    UIOrder = 20,
                    Name = "Small Busway OneWay",
                    DisplayName = "[BETA] OneWay Busway",
                    Description = "A two-lane, one-way road suitable for buses only. Busway does not allow zoning next to it!",
                    ThumbnailsPath = @"Roads\BusRoads\Busway2L1W\thumbnails.png",
                    InfoTooltipPath = @"Roads\BusRoads\Busway2L1W\infotooltip.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = RExExtendedMenus.ROADS_BUSWAYS,
                    UIOrder = 21,
                    Name = "Small Busway OneWay Decoration Grass",
                    DisplayName = "[BETA] OneWay Busway with Grass",
                    Description = "A two-lane, one-way road with decorative grass suitable for buses only. Busway does not allow zoning next to it!",
                    ThumbnailsPath = @"Roads\BusRoads\Busway2L1W\thumbnails_grass.png",
                    InfoTooltipPath = @"Roads\BusRoads\Busway2L1W\infotooltip_grass.png"
                };
                yield return new MenuItemBuilder
                {
                    UICategory = RExExtendedMenus.ROADS_BUSWAYS,
                    UIOrder = 22,
                    Name = "Small Busway OneWay Decoration Trees",
                    DisplayName = "[BETA] OneWay Busway with Trees",
                    Description = "A two-lane, one-way road with decorative trees suitable for buses only. Busway does not allow zoning next to it!",
                    ThumbnailsPath = @"Roads\BusRoads\Busway2L1W\thumbnails_trees.png",
                    InfoTooltipPath = @"Roads\BusRoads\Busway2L1W\infotooltip_trees.png"
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
                                           (@"Roads\BusRoads\Busway2L1W\Textures\Ground_Segment__MainTex.png",
                                            @"Roads\BusRoads\Busway2L\Textures\Ground_Segment__AlphaMap.png"),
                                        new LODTextureSet
                                           (@"Roads\BusRoads\Busway2L1W\Textures\Ground_SegmentLOD_Bus__MainTex.png",
                                            @"Roads\BusRoads\Busway2L\Textures\Ground_SegmentLOD_Bus__AlphaMap.png",
                                            @"Roads\BusRoads\Busway2L\Textures\Ground_SegmentLOD__XYSMap.png"));
                                    break;

                                case NetSegment.Flags.StopBoth:
                                    segment.SetTextures(
                                        new TextureSet
                                           (@"Roads\BusRoads\Busway2L1W\Textures\Ground_Segment__MainTex.png",
                                            @"Roads\BusRoads\Busway2L\Textures\Ground_Segment__AlphaMap.png"),
                                        new LODTextureSet
                                           (@"Roads\BusRoads\Busway2L1W\Textures\Ground_SegmentLOD_BusBoth__MainTex.png",
                                            @"Roads\BusRoads\Busway2L\Textures\Ground_SegmentLOD_BusBoth__AlphaMap.png",
                                            @"Roads\BusRoads\Busway2L\Textures\Ground_SegmentLOD__XYSMap.png"));
                                    break;

                                default:
                                    segment.SetTextures(
                                        new TextureSet
                                           (@"Roads\BusRoads\Busway2L1W\Textures\Ground_Segment__MainTex.png",
                                            @"Roads\BusRoads\Busway2L\Textures\Ground_Segment__AlphaMap.png"),
                                        new LODTextureSet
                                           (@"Roads\BusRoads\Busway2L1W\Textures\Ground_SegmentLOD__MainTex.png",
                                            @"Roads\BusRoads\Busway2L\Textures\Ground_SegmentLOD__AlphaMap.png",
                                            @"Roads\BusRoads\Busway2L\Textures\Ground_SegmentLOD__XYSMap.png"));
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
                                           (@"Roads\BusRoads\Busway2L1W\Textures_Grass\Ground_Segment__MainTex.png",
                                            @"Roads\BusRoads\Busway2L\Textures_Grass\Ground_Segment_Bus__AlphaMap.png"),
                                        new LODTextureSet
                                            (@"Roads\BusRoads\Busway2L1W\Textures_Grass\Ground_SegmentLOD_Bus__MainTex.png",
                                             @"Roads\BusRoads\Busway2L\Textures_Grass\Ground_SegmentLOD_Bus__AlphaMap.png",
                                             @"Roads\BusRoads\Busway2L\Textures_Grass\Ground_SegmentLOD__XYSMap.png"));
                                    break;

                                case NetSegment.Flags.StopBoth:
                                    segment.SetTextures(
                                        new TextureSet
                                           (@"Roads\BusRoads\Busway2L1W\Textures_Grass\Ground_Segment__MainTex.png",
                                            @"Roads\BusRoads\Busway2L\Textures_Grass\Ground_Segment_BusBoth__AlphaMap.png"),
                                        new LODTextureSet
                                            (@"Roads\BusRoads\Busway2L1W\Textures_Grass\Ground_SegmentLOD_BusBoth__MainTex.png",
                                             @"Roads\BusRoads\Busway2L\Textures_Grass\Ground_SegmentLOD_BusBoth__AlphaMap.png",
                                             @"Roads\BusRoads\Busway2L\Textures_Grass\Ground_SegmentLOD__XYSMap.png"));
                                    break;

                                default:
                                    segment.SetTextures(
                                        new TextureSet
                                           (@"Roads\BusRoads\Busway2L1W\Textures_Grass\Ground_Segment__MainTex.png",
                                            @"Roads\BusRoads\Busway2L\Textures_Grass\Ground_Segment__AlphaMap.png"),
                                        new LODTextureSet
                                            (@"Roads\BusRoads\Busway2L1W\Textures_Grass\Ground_SegmentLOD__MainTex.png",
                                             @"Roads\BusRoads\Busway2L\Textures_Grass\Ground_SegmentLOD__AlphaMap.png",
                                             @"Roads\BusRoads\Busway2L\Textures_Grass\Ground_SegmentLOD__XYSMap.png"));
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
                                    (@"Roads\BusRoads\Busway2L1W\Textures\Elevated_Segment__MainTex.png",
                                     @"Roads\BusRoads\Busway2L\Textures\Elevated_Segment__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\BusRoads\Busway2L1W\Textures\Elevated_SegmentLOD__MainTex.png",
                                     @"Roads\BusRoads\Busway2L\Textures\Elevated_SegmentLOD__AlphaMap.png",
                                     @"Roads\BusRoads\Busway2L\Textures\Elevated_SegmentLOD__XYSMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Slope:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            segment.SetTextures(
                                new TextureSet
                                    (@"Roads\BusRoads\Busway2L1W\Textures\Slope_Segment__MainTex.png",
                                     @"Roads\BusRoads\Busway2L\Textures\Slope_Segment__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Roads\BusRoads\Busway2L1W\Textures\Slope_SegmentLOD__MainTex.png",
                                     @"Roads\BusRoads\Busway2L\Textures\Slope_SegmentLOD__AlphaMap.png",
                                     @"Roads\BusRoads\Busway2L\Textures\Slope_SegmentLOD__XYS.png"));
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

            info.m_lanes = info.m_lanes.Where(l => l.m_laneType != NetInfo.LaneType.Parking).ToArray();
            for (int i = 0; i < info.m_lanes.Count(); i++)
            {
                var lane = info.m_lanes[i];

                if (lane.m_laneType == NetInfo.LaneType.Vehicle)
                {
                    if (version == NetInfoVersion.Ground)
                    {
                        if (lane.m_position < 0f)
                        {
                            lane.m_position -= 1f;
                            lane.m_stopOffset += 1f;
                        }
                        else
                        {
                            lane.m_position += 1f;
                            lane.m_stopOffset -= 1f;
                        }
                    }

                    lane.m_speedLimit = 1.6f;
                    lane.m_laneType = NetInfo.LaneType.TransportVehicle; 
                    lane.SetBusLaneProps();

                    info.m_lanes[i] = new NetInfoLane(lane, ExtendedVehicleType.Bus | ExtendedVehicleType.EmergencyVehicles);
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
