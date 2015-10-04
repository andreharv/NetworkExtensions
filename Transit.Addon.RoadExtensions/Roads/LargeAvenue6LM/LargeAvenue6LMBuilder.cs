using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Transit.Framework;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.LargeAvenue6LM
{
    public class LargeAvenue6LMBuilder : NetInfoBuilderBase, INetInfoBuilder
    {
        public int Order { get { return 25; } }
        public int Priority { get { return 25; } }

        public string TemplatePrefabName { get { return NetInfos.Vanilla.AVENUE_4L; } }
        public string Name { get { return "Large Avenue M"; } }
        public string DisplayName { get { return "Six-Lane Road with Median"; } }
        public string CodeName { get { return "LARGEAVENUE_6LM"; } }
        public string Description { get { return "A six-lane road. Supports heavy traffic."; } }
        public string UICategory { get { return "RoadsLarge"; } }
        
        public string ThumbnailsPath    { get { return @"Roads\LargeAvenue6LM\thumbnails.png"; } }
        public string InfoTooltipPath   { get { return @"Roads\LargeAvenue6LM\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.Ground; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            // I$|H3lls7rik3r sandbox
        }
    }
}
