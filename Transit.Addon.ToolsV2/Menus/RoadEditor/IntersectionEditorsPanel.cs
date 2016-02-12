using ColossalFramework;
using ColossalFramework.UI;
using System.Collections.Generic;
using Transit.Addon.ToolsV2.Common;
using Transit.Addon.ToolsV2.LaneRouting;
//using Transit.Addon.ToolsV2.TrafficLights;
using Transit.Framework;
using Transit.Framework.Interfaces;
using Transit.Framework.UI;

namespace Transit.Addon.ToolsV2.Menus.RoadEditor
{
    public class IntersectionEditorsPanel : CustomScrollPanelBase
    {
        public override void RefreshPanel()
        {
            base.RefreshPanel();

            IEnumerable<IToolBuilder> toolBuilders = new IToolBuilder[]
            {
                new RoutingToolBuilder(),
                //new TrafficLightsToolBuilder(),
            };

            foreach (var builder in toolBuilders)
            {
                toolController.AddTool<RoutingTool>();

                var toolName = builder.GetCodeName();
                UITextureAtlas atlas = null;

                if (!builder.ThumbnailsPath.IsNullOrWhiteSpace())
                {
                    atlas = AssetManager.instance.GetThumbnails(toolName, builder.ThumbnailsPath);
                }

                //var info = new PrefabInfo();
                //if (!builder.InfoTooltipPath.IsNullOrWhiteSpace())
                //{
                //    var infoTips = AssetManager.instance.GetInfoTooltip(builder.GetCodeName(), builder.InfoTooltipPath);
                //    info.m_InfoTooltipAtlas = infoTips;
                //    info.m_InfoTooltipThumbnail = infoTips.name;
                //}

                UIButton button = SpawnEntry(toolName, toolName, toolName, atlas, tooltipBox, true);
                button.objectUserData = builder;
                button.eventTooltipEnter += OnTooltipEnter;
            }
        }

        protected override void OnButtonClicked(UIComponent comp)
        {
            var builder = comp.objectUserData as IToolBuilder;

            if (builder != null)
            {
                toolController.SetTool(builder.ToolType);
            }
        }
    }
}
