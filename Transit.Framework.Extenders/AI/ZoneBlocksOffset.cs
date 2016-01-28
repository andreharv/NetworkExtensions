using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;

namespace Transit.Framework.Extenders.AI
{
    public static class ZoneBlocksOffset
    {
        public static ZoneBlocksOffsetMode Mode = ZoneBlocksOffsetMode.Default;
    }

    public enum ZoneBlocksOffsetMode
    {
        Default,
        HalfCell
    }
}
