using Transit.Framework;

namespace Transit.Addon.TrafficTools.RoadEditor.IntersectionEditors.TrafficLights
{
    public class TrafficLightsToolBuilder : Activable, IToolBuilder
    {
        public int Order { get { return 10; } }
        public int UIOrder { get { return 10; } }

        public string Name { get { return "Traffic Lights Editor"; } }
        public string DisplayName { get { return Name; } }
        public string Description { get { return "Allows you to customize entry and exit points in junctions."; } }
        public string UICategory { get { return "IntersectionEditors"; } }

        public string ThumbnailsPath { get { return @"RoadEditor\IntersectionEditors\TrafficLights\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"RoadEditor\IntersectionEditors\TrafficLights\infotooltip.png"; } }
    }
}

