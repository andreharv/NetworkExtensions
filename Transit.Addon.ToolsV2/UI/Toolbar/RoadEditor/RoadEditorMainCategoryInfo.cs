using System;
using ColossalFramework;
using ColossalFramework.UI;
using System.Collections.Generic;
using Transit.Addon.ToolsV2.Common;
using Transit.Addon.ToolsV2.LaneRouting;
//using Transit.Addon.ToolsV2.TrafficLights;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;
using Transit.Framework.UI.Toolbar.Menus;

namespace Transit.Addon.ToolsV2.UI.Toolbar.RoadEditor
{
    public class RoadEditorMainCategoryInfo
    {
        public const string NAME = "RoadEditorMainCategory";

        public string Name { get { return NAME; } }
        public int Order { get { return 10; } }
        public Type PanelType { get { return typeof(RoadEditorMainPanel); } }
    }

    public class RoadEditorMainPanel : CustomScrollPanelBase
    {
        public override void RefreshPanel()
        {
            base.RefreshPanel();

            IEnumerable<IToolBuilder> toolBuilders = new IToolBuilder[]
            {
                new LaneRoutingToolBuilder(),
                //new TrafficLightsToolBuilder(),
            };

            foreach (var builder in toolBuilders)
            {
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
