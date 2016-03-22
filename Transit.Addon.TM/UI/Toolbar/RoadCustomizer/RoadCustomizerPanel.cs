using System;
using System.Collections;
using ColossalFramework.UI;
using Transit.Addon.TM.Tools;
using Transit.Addon.TM.Tools.RoadCustomizer;
using Transit.Addon.TM.UI.Toolbar.RoadCustomizer.Textures;
using Transit.Framework;
using Transit.Framework.Network;
using UnityEngine;

namespace Transit.Addon.TM.UI.Toolbar.RoadCustomizer
{
    class RoadCustomizerPanel : MonoBehaviour
    {
        private static readonly string kItemTemplate = "PlaceableItemTemplate";

        public UITextureAtlas m_atlas;
        private UIScrollablePanel m_scrollablePanel;
        private int m_objectIndex, m_selectedIndex;

        private void Awake()
        {
            this.m_atlas = AtlasManager.instance.GetAtlas(RoadCustomizerAtlasBuilder.ID);
            this.m_scrollablePanel = GetComponentInChildren<UIScrollablePanel>();
            this.m_scrollablePanel.autoLayoutStart = LayoutStart.TopLeft;
            UIScrollbar scrollbar = this.GetComponentInChildren<UIScrollbar>();
            if (scrollbar != null)
                scrollbar.incrementAmount = 109;
            this.m_objectIndex = m_selectedIndex = 0;
        }

        public void AttachLaneCustomizationEvents(RoadCustomizerTool tool)
        {
            tool.OnStartLaneCustomization += EnableIcons;
            tool.OnEndLaneCustomization += DisableIcons;
        }

        private void OnEnable()
        {				
            this.RefreshPanel();
        }

        void EnableIcons()
        {
            RoadCustomizerTool rct = ToolsModifierControl.GetCurrentTool<RoadCustomizerTool>();
            if (rct != null)
            {
                ExtendedUnitType restrictions = rct.GetCurrentVehicleRestrictions();
                
                for (int i = 0; i < this.m_scrollablePanel.components.Count; i++)
                {
                    UIButton btn = this.m_scrollablePanel.components[i] as UIButton;
                    ExtendedUnitType vehicleType = (ExtendedUnitType)btn.objectUserData;

                    if ((vehicleType & restrictions) == vehicleType)
                    {
                        btn.stringUserData = "Selected";
                        btn.normalFgSprite = btn.name;
                        btn.focusedFgSprite = btn.name;
                        btn.hoveredFgSprite = btn.name + "90%";
                        btn.pressedFgSprite = btn.name + "80%";
                    }
                    else if (vehicleType == ExtendedUnitType.EmergencyVehicle && (restrictions & ExtendedUnitType.Emergency) == ExtendedUnitType.Emergency)
                    {
                        btn.stringUserData = "Emergency";
                        btn.hoveredFgSprite = btn.name + "90%";
                        btn.pressedFgSprite = btn.name + "80%";
                        StartCoroutine("EmergencyLights", btn);
                    }
                    else
                    {
                        btn.stringUserData = null;
                        btn.normalFgSprite = btn.name + "Deselected";
                        btn.focusedFgSprite = btn.name + "Deselected";
                        btn.hoveredFgSprite = btn.name + "80%";
                        btn.pressedFgSprite = btn.name + "90%";
                    }
                    btn.state = UIButton.ButtonState.Normal;

                    btn.isEnabled = true;
                }
            }

        }

        void DisableIcons()
        {
            for (int i = 0; i < this.m_scrollablePanel.components.Count; i++)
            {
                UIButton btn = this.m_scrollablePanel.components[i] as UIButton;
                btn.state = UIButton.ButtonState.Disabled;
                btn.isEnabled = false;
            }
            StopCoroutine("EmergencyLights");
        }

        public void RefreshPanel()
        {
            this.PopulateAssets();
        }

        public void PopulateAssets()
        {
            this.m_objectIndex = 0;
            this.SpawnEntry("PassengerCar", "PassengerCar", null, null, false, false).objectUserData = ExtendedUnitType.PassengerCar;
            this.SpawnEntry("Bus", "Bus", null, null, false, false).objectUserData = ExtendedUnitType.Bus;
            this.SpawnEntry("CargoTruck", "CargoTruck", null, null, false, false).objectUserData = ExtendedUnitType.CargoTruck;
            this.SpawnEntry("GarbageTruck", "GarbageTruck", null, null, false, false).objectUserData = ExtendedUnitType.GarbageTruck;
            this.SpawnEntry("Hearse", "Hearse", null, null, false, false).objectUserData = ExtendedUnitType.Hearse;
            this.SpawnEntry("Emergency", "Emergency", null, null, false, false).objectUserData = ExtendedUnitType.EmergencyVehicle;
        }

        protected UIButton SpawnEntry(string name, string tooltip, string thumbnail, UITextureAtlas atlas, bool enabled, bool grouped)
        {
            if (atlas == null)
            {
                atlas = this.m_atlas;
            }
            if (string.IsNullOrEmpty(thumbnail) || atlas[thumbnail] == null)
            {
                thumbnail = "ThumbnailBuildingDefault";
            }
            return this.CreateButton(name, tooltip, name, -1, atlas, null, enabled, grouped);
        }

        protected UIButton CreateButton(string name, string tooltip, string baseIconName, int index, UITextureAtlas atlas, UIComponent tooltipBox, bool enabled, bool grouped)
        {
            UIButton btn;
            if (this.m_scrollablePanel.childCount > this.m_objectIndex)
            {
                btn = (this.m_scrollablePanel.components[this.m_objectIndex] as UIButton);
            }
            else
            {
                GameObject asGameObject = UITemplateManager.GetAsGameObject(RoadCustomizerPanel.kItemTemplate);
                btn = (this.m_scrollablePanel.AttachUIComponent(asGameObject) as UIButton);
                btn.eventClick += OnClick;
            }
            btn.gameObject.GetComponent<TutorialUITag>().tutorialTag = name;
            btn.text = string.Empty;
            btn.name = name;
            btn.tooltipAnchor = UITooltipAnchor.Anchored;
            btn.tabStrip = true;
            btn.horizontalAlignment = UIHorizontalAlignment.Center;
            btn.verticalAlignment = UIVerticalAlignment.Middle;
            btn.pivot = UIPivotPoint.TopCenter;
            if (atlas != null)
            {
                btn.atlas = atlas;
                SetVehicleButtonsThumbnails(btn);

            }
            if (index != -1)
            {
                btn.zOrder = index;
            }
            btn.verticalAlignment = UIVerticalAlignment.Bottom;
            btn.foregroundSpriteMode = UIForegroundSpriteMode.Fill;

            UIComponent uIComponent = (btn.childCount <= 0) ? null : btn.components[0];
            if (uIComponent != null)
            {
                uIComponent.isVisible = false;
            }
            btn.isEnabled = enabled;
            btn.state = UIButton.ButtonState.Disabled;
            btn.tooltip = tooltip;
            btn.tooltipBox = tooltipBox;
            btn.group = grouped ? this.m_scrollablePanel : null;
            this.m_objectIndex++;
            return btn;
        }

        protected void SetVehicleButtonsThumbnails(UIButton btn)
        {
            string iconName = btn.name;

            btn.normalFgSprite = iconName;
            btn.focusedFgSprite = iconName;
            btn.hoveredFgSprite = iconName;
            btn.pressedFgSprite = iconName;
            btn.disabledFgSprite = iconName + "Disabled";

            btn.eventMouseEnter += (UIComponent comp, UIMouseEventParameter p) =>
            {
                if (btn.state == UIButton.ButtonState.Focused)
                {
                    if (String.IsNullOrEmpty(btn.stringUserData))
                        btn.focusedFgSprite = iconName + "80%";
                    else
                        btn.focusedFgSprite = iconName + "90%";
                }
            };

            btn.eventMouseLeave += (UIComponent comp, UIMouseEventParameter p) =>
            {
                if (btn.state == UIButton.ButtonState.Focused)
                {
                    if (String.IsNullOrEmpty(btn.stringUserData))
                        btn.focusedFgSprite = iconName + "Deselected";
                    else
                        btn.focusedFgSprite = iconName;
                }
            };

            btn.eventMouseDown += (UIComponent comp, UIMouseEventParameter p) =>
            {
                if (btn.state == UIButton.ButtonState.Focused)
                {
                    if (String.IsNullOrEmpty(btn.stringUserData))
                        btn.focusedFgSprite = iconName + "90%";
                    else
                        btn.focusedFgSprite = iconName + "80%";
                }
            };

        }

        protected void OnButtonClicked(UIButton btn)
        {
            ExtendedUnitType vehicleType = (ExtendedUnitType)btn.objectUserData;
            if (vehicleType != ExtendedUnitType.None)
            {
                if (String.IsNullOrEmpty(btn.stringUserData))
                {
                    btn.stringUserData = "Selected";
                    btn.normalFgSprite = btn.name;
                    btn.focusedFgSprite = btn.name;
                    btn.hoveredFgSprite = btn.name + "90%";
                    btn.pressedFgSprite = btn.name + "80%";
                }
                else if (vehicleType == ExtendedUnitType.EmergencyVehicle && btn.stringUserData != "Emergency")
                {
                    btn.stringUserData = "Emergency";
                    StartCoroutine("EmergencyLights", btn);
                }
                else
                {
                    if (vehicleType == ExtendedUnitType.EmergencyVehicle)
                        StopCoroutine("EmergencyLights");

                    btn.stringUserData = null;
                    btn.normalFgSprite = btn.name + "Deselected";
                    btn.focusedFgSprite = btn.name + "Deselected";
                    btn.hoveredFgSprite = btn.name + "80%";
                    btn.pressedFgSprite = btn.name + "90%";
                }

                RoadCustomizerTool rct = ToolsModifierControl.GetCurrentTool<RoadCustomizerTool>();
                if (rct != null)
                {
                    if (btn.stringUserData == "Emergency")
                        rct.ToggleRestriction(vehicleType ^ ExtendedUnitType.Emergency);
                    else if (vehicleType == ExtendedUnitType.EmergencyVehicle && btn.stringUserData == null)
                        rct.ToggleRestriction(ExtendedUnitType.Emergency);
                    else
                        rct.ToggleRestriction(vehicleType);		
                }
            }
        }

        protected void OnClick(UIComponent comp, UIMouseEventParameter p)
        {
            p.Use();
            UIButton uIButton = comp as UIButton;
            if (uIButton != null && uIButton.parent == this.m_scrollablePanel)
            {
                this.OnButtonClicked(uIButton);
                this.m_selectedIndex = this.m_scrollablePanel.components.IndexOf(uIButton);
            }
        }

        IEnumerator EmergencyLights(UIButton btn)
        {
            int n = 0;
            do
            {
                yield return new WaitForEndOfFrame();
                while (this.m_scrollablePanel.isVisible)
                {
                    if (btn.normalFgSprite == btn.name || btn.normalFgSprite.Contains("Lights"))
                        btn.normalFgSprite = btn.name + "Lights" + n;
                    if (btn.focusedFgSprite == btn.name || btn.focusedFgSprite.Contains("Lights"))
                        btn.focusedFgSprite = btn.name + "Lights" + n;

                    n = (n + 1) % 2;

                    yield return new WaitForSeconds(0.25f);
                }
            } while (!this.m_scrollablePanel.isVisible);
        }
    }
}
