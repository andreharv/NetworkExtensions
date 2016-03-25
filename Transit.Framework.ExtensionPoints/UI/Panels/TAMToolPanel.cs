using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;
using System;

namespace Transit.Framework.ExtensionPoints.UI.Panels
{
    public class TAMToolPanel : GeneratedScrollPanel
    {
        public IEnumerable<IToolBuilder> ToolBuilders { get; set; }

        public override ItemClass.Service service
        {
            get { return ItemClass.Service.None; }
        }

        public override void RefreshPanel()
        {
            base.RefreshPanel();

            _index = 0;

            if (ToolBuilders != null)
            {
                foreach (var builder in ToolBuilders.OrderOrderables())
                {
                    SpawnEntry(builder);
                }
            }
        }

        private int _index = 0;

        private class ToolInfo
        {
            public string Name { get; set; }
            public UITextureAtlas ThumbnailAtlas { get; set; }
            public string Thumbnail { get; set; }
            public UITextureAtlas InfoTooltipAtlas { get; set; }
            public string InfoTooltip { get; set; }
            public Type ToolType { get; set; }
        }

        private UIButton SpawnEntry(IToolBuilder builder)
        {
            var info = new ToolInfo()
            {
                Name = builder.GetCodeName(),
                ToolType = builder.ToolType
            };

            if (!builder.ThumbnailsPath.IsNullOrWhiteSpace())
            {
                var thumbnails = AssetManager.instance.GetThumbnails(info.Name, builder.ThumbnailsPath);
                info.ThumbnailAtlas = thumbnails;
                info.Thumbnail = thumbnails.name;
            }

            if (!builder.InfoTooltipPath.IsNullOrWhiteSpace())
            {
                var infoTips = AssetManager.instance.GetInfoTooltip(info.Name, builder.InfoTooltipPath);
                info.InfoTooltipAtlas = infoTips;
                info.InfoTooltip = infoTips.name;
            }

            string tooltip = TooltipHelper.Format(new string[]
	        {
		        "title",
                builder.DisplayName,
		        "sprite",
                info.Name,
		        "text",
                builder.Description
	        });

	        var button = base.CreateButton(info.Name, tooltip, info.Name, _index++, info.ThumbnailAtlas, tooltipBox, true);
            button.objectUserData = info;
            button.eventTooltipEnter += OnTooltipEnter;

            return button;
        }

        public override void OnTooltipEnter(UIComponent comp, UIMouseEventParameter p)
        {
            UIButton uIButton = comp as UIButton;
            if (uIButton != null)
            {
                ToolInfo prefabInfo = uIButton.objectUserData as ToolInfo;
                if (prefabInfo != null)
                {
                    UISprite uISprite = uIButton.tooltipBox.Find<UISprite>("Sprite");

                    if (prefabInfo.InfoTooltipAtlas != null)
                    {
                        uISprite.atlas = prefabInfo.InfoTooltipAtlas;
                    }
                    if (!string.IsNullOrEmpty(prefabInfo.InfoTooltip) && uISprite.atlas[prefabInfo.InfoTooltip] != null)
                    {
                        uISprite.spriteName = prefabInfo.InfoTooltip;
                    }
                    else
                    {
                        uISprite.spriteName = "ThumbnailBuildingDefault";
                    }
                }
                else
                {
                    UISprite uISprite2 = uIButton.tooltipBox.Find<UISprite>("Sprite");
                    if (uISprite2 != null)
                    {
                        uISprite2.atlas = this.m_DefaultInfoTooltipAtlas;
                    }
                }
            }
        }

        protected override void OnButtonClicked(UIComponent comp)
        {
            var info = comp.objectUserData as ToolInfo;
            if (info != null)
            {
                toolController.SetTool(info.ToolType);
            }
        }
    }
}
