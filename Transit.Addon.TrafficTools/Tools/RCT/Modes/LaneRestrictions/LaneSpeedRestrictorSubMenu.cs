using ColossalFramework.UI;
using System.Collections.Generic;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.TrafficTools
{
    public class LaneSpeedRestrictorSubMenu : LaneRestrictorSubMenu
    {
        public override void RefreshPanel()
        {
            base.RefreshPanel();

            if (_atlas == null)
                _atlas = AssetManager.instance.GetThumbnails("UISpeedSigns", @"Tools\RCT\UI\UISpeedSigns.png");

            SpawnEntry(15);
            for (int i = 1; i < 15; i++)
                SpawnEntry((i + 2) * 10);

            DisableIcons();
        }

        protected UIButton SpawnEntry(int speed)
        {
            string name = speed.ToString();
            string tooltip = name + " km/h";

            string[] ts = { "", "Disabled", "Focused", "Hovered", "Pressed" };
            int thumbnailStateId = Mathf.Max(speed / 10 - 2, 0);
            int thumbnailRowId = thumbnailStateId / 5 + 1;

            int index = thumbnailStateId % ts.Length;
            string thumbnail = "UISPEEDSIGNS" + thumbnailRowId + ts[index];

            UIButton btn = SpawnEntry(name, tooltip, thumbnail, _atlas, /*tooltipBox*/null, true);

            // backgrounds
            btn.normalBgSprite = "UISPEEDSIGNS";
            btn.disabledBgSprite = "UISPEEDSIGNSDisabled";
            btn.focusedBgSprite = "UISPEEDSIGNSFocused";
            btn.hoveredBgSprite = btn.pressedBgSprite = "UISPEEDSIGNSHovered";

            // foregrounds
            btn.focusedFgSprite = btn.hoveredFgSprite = btn.pressedFgSprite = btn.disabledFgSprite = btn.normalFgSprite;

            // data
            btn.objectUserData = speed;

            btn.text = name;

            return btn;
        }

        protected override void UpdateIcons()
        {
            LaneRestrictorTool lrt = ToolsModifierControl.GetCurrentTool<LaneRestrictorTool>();
            if (lrt)
            {
                HashSet<uint> selectedLanes = lrt.SelectedLanes;

                if (selectedLanes.Count == 0)
                    DisableIcons();
                else
                    EnableIcons(selectedLanes);
            }
        }

        protected override void EnableIcons(HashSet<uint> selectedLanes)
        {
        }
    }
}
