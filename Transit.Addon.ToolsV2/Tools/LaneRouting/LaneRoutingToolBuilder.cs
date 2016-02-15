using Transit.Framework.Builders;
using Transit.Framework.ExtensionPoints.PathFinding;

namespace Transit.Addon.ToolsV2.LaneRouting
{
    public class LaneRoutingToolBuilder : ToolBuilder<LaneRoutingTool>, IToolBuilder
    {
        public int Order { get { return 10; } }
        public int UIOrder { get { return 10; } }

        public string Name { get { return "Lane Routing"; } }
        public string DisplayName { get { return Name; } }
        public string Description { get { return "Allows you to customize entry and exit points in intersections and junctions."; } }
        public string UICategory { get { return "IntersectionEditors"; } }

        public string ThumbnailsPath { get { return @"Tools\LaneRouting\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Tools\LaneRouting\infotooltip.png"; } }

        protected override void OnToolInstalled(LaneRoutingTool tool)
        {
            PathFindingManager.instance.DefineCustomLaneRouting(LaneRoutingManager.instance);
        }

        protected override void OnToolUninstalling(LaneRoutingTool tool)
        {
            PathFindingManager.instance.DisableCustomLaneRouting();
        }
    }
}

