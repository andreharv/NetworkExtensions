using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Globalization;
using Transit.Framework;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.LargeAvenue8L
{
    public class LargeAvenue8LBuilder : NetInfoBuilderBase, INetInfoBuilder
    {
        public int Order { get { return 50; } }
        public int Priority { get { return 30; } }

        public string TemplatePrefabName { get { return NetInfos.Vanilla.ROAD_6L; } }
        public string Name { get { return "XLarge Avenue"; } }
        public string DisplayName { get { return "Eight-Lane Road"; } }
        public string CodeName { get { return "LARGEAVENUE_8L"; } }
        public string Description { get { return "A eight-lane road. Supports heavy traffic."; } }
        public string UICategory { get { return "RoadsLarge"; } }
        
        public string ThumbnailsPath    { get { return @"Roads\LargeAvenue8L\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"Roads\LargeAvenue8L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            // clarencechenct sandbox <--

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            //switch (version)
            //{
            //    case NetInfoVersion.Ground:
            //        info.SetAllSegmentsTexture(
            //            new TexturesSet
            //               (@"NewNetwork\LargeAvenue8L\Textures\Ground_Segment__MainTex.png",
            //                @"NewNetwork\LargeAvenue8L\Textures\Ground_Segment__AlphaMap.png"));
            //        break;
            //}

            foreach (var lanes in info.m_lanes)
            {
                // Do something
            }
        }
    }
}
