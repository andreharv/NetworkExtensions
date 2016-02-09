using System;
using Transit.Addon.TrafficTools.Common;
using Transit.Addon.TrafficTools.LaneRouting;
using Transit.Framework;

namespace Transit.Addon.TrafficTools.TrafficLights
{
    public class TrafficLightsToolBuilder : Activable, IToolBuilder
    {
        public int Order { get { return 10; } }
        public int UIOrder { get { return 10; } }

        public string Name { get { return "Traffic Lights Editor"; } }
        public string DisplayName { get { return Name; } }
        public string Description { get { return "Allows you to remove or place traffic lights on road intersections."; } }
        public string UICategory { get { return "IntersectionEditors"; } }

        public string ThumbnailsPath { get { return @"Tools\TrafficLights\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Tools\TrafficLights\infotooltip.png"; } }

        public Type ToolType { get { return typeof(LaneRoutingTool); } }
    }
}

