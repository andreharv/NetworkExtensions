using System;

namespace Transit.Addon.ToolsV2.Common
{
    [Flags]
    public enum UIState
    {
        Default = 0,
        //Hidden = 1,
        Disabled = 2,
        Hovered = 4,
        Selected = 8
    }
}
