using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;

namespace Transit.Framework.ExtensionPoints.UI.Toolbar.Panels
{
    public class TAMMenuCategoryPanel : GeneratedScrollPanel
    {
        public IEnumerable<IToolBuilder> ToolBuilders { get; set; }

        public override ItemClass.Service service
        {
            get { return ItemClass.Service.None; }
        }

        public override void RefreshPanel()
        {
            base.RefreshPanel();

            if (ToolBuilders != null)
            {
                foreach (var builder in ToolBuilders)
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
        }

        //protected virtual UIButton SpawnToolButton(string name, string tooltip, string thumbnail, UITextureAtlas atlas, UIComponent tooltipBox, bool enabled)
        //{
        //    if (string.IsNullOrEmpty(thumbnail) || atlas[thumbnail] == null)
        //    {
        //        thumbnail = "ThumbnailBuildingDefault";
        //    }
        //    return this.CreateButton(name, tooltip, thumbnail, -1, atlas, tooltipBox, enabled);
        //}

        //protected UIButton CreateButton(string name, string tooltip, string baseIconName, int index, UITextureAtlas atlas, UIComponent tooltipBox, bool enabled)
        //{
        //    UIButton uIButton;
        //    if (this.m_ScrollablePanel.childCount > this.m_ObjectIndex)
        //    {
        //        uIButton = (this.m_ScrollablePanel.components[this.m_ObjectIndex] as UIButton);
        //    }
        //    else
        //    {
        //        GameObject asGameObject = UITemplateManager.GetAsGameObject(GeneratedScrollPanel.kPlaceableItemTemplate);
        //        uIButton = (this.m_ScrollablePanel.AttachUIComponent(asGameObject) as UIButton);
        //    }
        //    uIButton.gameObject.GetComponent<TutorialUITag>().tutorialTag = name;
        //    uIButton.text = string.Empty;
        //    uIButton.name = name;
        //    uIButton.tooltipAnchor = UITooltipAnchor.Anchored;
        //    uIButton.tabStrip = true;
        //    uIButton.horizontalAlignment = UIHorizontalAlignment.Center;
        //    uIButton.verticalAlignment = UIVerticalAlignment.Middle;
        //    uIButton.pivot = UIPivotPoint.TopCenter;
        //    if (atlas != null)
        //    {
        //        uIButton.atlas = atlas;
        //    }
        //    if (index != -1)
        //    {
        //        uIButton.zOrder = index;
        //    }
        //    uIButton.verticalAlignment = this.buttonsAlignment;
        //    uIButton.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
        //    uIButton.normalFgSprite = baseIconName;
        //    uIButton.focusedFgSprite = baseIconName + "Focused";
        //    uIButton.hoveredFgSprite = baseIconName + "Hovered";
        //    uIButton.pressedFgSprite = baseIconName + "Pressed";
        //    uIButton.disabledFgSprite = baseIconName + "Disabled";
        //    UIComponent uIComponent = (uIButton.childCount <= 0) ? null : uIButton.components[0];
        //    if (uIComponent != null)
        //    {
        //        uIComponent.isVisible = false;
        //    }
        //    uIButton.isEnabled = enabled;
        //    uIButton.tooltip = tooltip;
        //    uIButton.tooltipBox = tooltipBox;
        //    uIButton.group = base.component;
        //    this.m_ObjectIndex++;
        //    return uIButton;
        //}

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
