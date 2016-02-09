using ColossalFramework;
using ColossalFramework.UI;
using System.Collections.Generic;
using Transit.Addon.TrafficTools.Common;
using Transit.Addon.TrafficTools.LaneRouting;
//using Transit.Addon.TrafficTools.TrafficLights;
using Transit.Framework;
using Transit.Framework.Interfaces;
using Transit.Framework.UI;

namespace Transit.Addon.TrafficTools.Menus.RoadEditor
{
    public class IntersectionEditorsPanel : CustomScrollPanelBase
    {
        public override void RefreshPanel()
        {
            base.RefreshPanel();

            IEnumerable<IToolBuilder> toolBuilders = new IToolBuilder[]
            {
                new NetLaneRoutingToolBuilder(),
                //new TrafficLightsToolBuilder(),
            };

            foreach (var builder in toolBuilders)
            {
                toolController.AddTool<NetLaneRoutingTool>();

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
