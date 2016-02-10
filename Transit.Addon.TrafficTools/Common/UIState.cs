using System;

namespace Transit.Addon.TrafficTools.Common
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
