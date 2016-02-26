
namespace Transit.Framework.ExtensionPoints.AI
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
