using ColossalFramework.UI;
using System;
using System.Collections.Generic;

namespace Transit.Addon.TrafficTools
{
    public class LaneVehicleRestrictorSubMenu : LaneRestrictorSubMenu
    {
        public override void RefreshPanel()
        {
            base.RefreshPanel();

            
        }

        protected override void EnableIcons(HashSet<uint> selectedLanes)
        {
            LaneRestrictorTool lrt = ToolsModifierControl.GetCurrentTool<LaneRestrictorTool>();
            if (lrt == null)
            {
                DisableIcons();
                return;
            }

            for (int i = 0; i < _scrollablePanel.components.Count; i++)
            {
                UIButton btn = _scrollablePanel.components[i] as UIButton;

                //if (btnSpeed == (int)btn.objectUserData)
                //{
                //    selectedIndex = i;
                //    btn.state = UIButton.ButtonState.Focused;
                //}
                //else
                    btn.state = UIButton.ButtonState.Normal;

                btn.isEnabled = true;
            }
        }

        protected void SetToggleBehaviour(UIButton btn)
        {
            string iconName = btn.name;
            //UIUtils.SetThumbnails(iconName, sm_thumbnailCoords[iconName], btn.atlas, iconName == "Emergency" ? sm_emergencyVehicleThumbnailStates : sm_vehicleThumbnailStates);

            //btn.normalFgSprite = iconName + "Focused";
            //btn.focusedFgSprite = iconName + "Focused";
            //btn.hoveredFgSprite = iconName + "Focused";
            //btn.pressedFgSprite = iconName + "Focused";
            //btn.disabledFgSprite = iconName + "Disabled";

            btn.eventMouseEnter += (UIComponent comp, UIMouseEventParameter p) =>
            {
                if (btn.state == UIButton.ButtonState.Focused)
                {
                    if (String.IsNullOrEmpty(btn.stringUserData))
                        btn.focusedFgSprite = iconName + "Pressed";
                    else
                        btn.focusedFgSprite = iconName + "Hovered";
                }
            };

            btn.eventMouseLeave += (UIComponent comp, UIMouseEventParameter p) =>
            {
                if (String.IsNullOrEmpty(btn.stringUserData))
                    btn.focusedFgSprite = iconName;
                else
                    btn.focusedFgSprite = iconName + "Focused";
            };

            btn.eventMouseDown += (UIComponent comp, UIMouseEventParameter p) =>
            {
                if (btn.state == UIButton.ButtonState.Focused)
                {
                    if (String.IsNullOrEmpty(btn.stringUserData))
                        btn.focusedFgSprite = iconName + "Hovered";
                    else
                        btn.focusedFgSprite = iconName + "Pressed";
                }
            };
        }
    }
}
