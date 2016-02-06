using System.Collections.Generic;
using ColossalFramework.UI;
using Transit.Framework.UI;

namespace Transit.Addon.TrafficTools.RoadEditor.LaneRestrictions
{
    public abstract class LaneRestrictorSubMenu : ToolbarSubMenu
    {
        protected UITextureAtlas _atlas;

        protected override void OnEnable()
        {
            base.OnEnable();

            LaneRestrictorTool lrt = ToolsModifierControl.GetTool<LaneRestrictorTool>();
            if (lrt != null)
            {
                lrt.OnLanesDeselected += UpdateIcons;
                lrt.OnLanesSelected += UpdateIcons;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            LaneRestrictorTool lrt = ToolsModifierControl.GetTool<LaneRestrictorTool>();
            if (lrt != null)
            {
                lrt.OnLanesDeselected -= UpdateIcons;
                lrt.OnLanesSelected -= UpdateIcons;
            }
        }

        protected virtual void UpdateIcons()
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

        protected virtual void DisableIcons()
        {
            for (int i = 0; i < _scrollablePanel.components.Count; i++)
            {
                UIButton btn = _scrollablePanel.components[i] as UIButton;
                btn.state = UIButton.ButtonState.Disabled;
                btn.isEnabled = false;
            }
        }

        protected virtual void EnableIcons(HashSet<uint> selectedLanes)
        {
            for (int i = 0; i < _scrollablePanel.components.Count; i++)
            {
                UIButton btn = _scrollablePanel.components[i] as UIButton;
                btn.state = selectedIndex == i ? UIButton.ButtonState.Focused : UIButton.ButtonState.Normal;
                btn.isEnabled = true;
            }
        }
    }
}
