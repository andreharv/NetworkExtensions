
namespace Transit.Framework.ExtensionPoints.AI.Networks
{
    public static class ZoneBlocksOffset
    {
        public static ZoneBlocksOffsetMode Mode = ZoneBlocksOffsetMode.Default;
    }

    public enum ZoneBlocksOffsetMode
    {
        Default,
        HalfCell,
        ForcedDefault
    }
}
