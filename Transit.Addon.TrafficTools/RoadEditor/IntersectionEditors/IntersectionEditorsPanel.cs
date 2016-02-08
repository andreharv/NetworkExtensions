using System.Collections;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.UI;
using Transit.Addon.TrafficTools.RoadEditor.IntersectionEditors.LaneRouting;
using Transit.Addon.TrafficTools.RoadEditor.IntersectionEditors.TrafficLights;
using Transit.Framework;
using Transit.Framework.Interfaces;
using Transit.Framework.UI;

namespace Transit.Addon.TrafficTools.RoadEditor.IntersectionEditors
{
    public class IntersectionEditorsPanel : CustomScrollPanelBase
    {
        public override void RefreshPanel()
        {
            base.RefreshPanel();

            IEnumerable<IToolBuilder> toolBuilders = new IToolBuilder[]
            {
                new LaneRoutingToolBuilder(),
                new TrafficLightsToolBuilder(), 
            };

            foreach (var builder in toolBuilders)
            {
                var toolName = builder.GetCodeName();
                UITextureAtlas atlas = null;

                if (!builder.ThumbnailsPath.IsNullOrWhiteSpace())
                {
                    atlas = AssetManager.instance.GetThumbnails(toolName, builder.ThumbnailsPath);
                }

                var info = new PrefabInfo();
                if (!builder.InfoTooltipPath.IsNullOrWhiteSpace())
                {
                    var infoTips = AssetManager.instance.GetInfoTooltip(builder.GetCodeName(), builder.InfoTooltipPath);
                    info.m_InfoTooltipAtlas = infoTips;
                    info.m_InfoTooltipThumbnail = infoTips.name;
                }

                UIButton button = SpawnEntry(toolName, toolName, toolName, atlas, tooltipBox, true);
                button.objectUserData = info;// LaneRoutingTool.Template.None;
                button.eventTooltipEnter += OnTooltipEnter;
            }
        }

        protected override void OnButtonClicked(UIComponent comp)
        {
            //LaneRoutingTool lrt = ToolsModifierControl.GetCurrentTool<LaneRoutingTool>();
            //if (lrt != null)
            //    lrt.TemplateMode = (LaneRoutingTool.Template)comp.objectUserData;
        }
    }
}
