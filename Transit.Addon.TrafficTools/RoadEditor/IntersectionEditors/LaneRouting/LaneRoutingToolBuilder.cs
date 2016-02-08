using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;

namespace Transit.Addon.TrafficTools.RoadEditor.IntersectionEditors.LaneRouting
{
    public class LaneRoutingToolBuilder : Activable, IToolBuilder
    {
        public int Order { get { return 10; } }
        public int UIOrder { get { return 10; } }

        public string Name { get { return "Intersection Routing"; } }
        public string DisplayName { get { return Name; } }
        public string Description { get { return "Allows you to customize entry and exit points in junctions."; } }
        public string UICategory { get { return "IntersectionEditors"; } }

        public string ThumbnailsPath { get { return @"RoadEditor\IntersectionEditors\LaneRouting\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"RoadEditor\IntersectionEditors\LaneRouting\infotooltip.png"; } }
    }
}

