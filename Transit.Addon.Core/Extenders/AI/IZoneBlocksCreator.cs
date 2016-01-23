namespace Transit.Addon.Core.Extenders.AI
{
    public interface IZoneBlocksCreator
    {
        void CreateZoneBlocks(NetInfo info, ushort segmentId, ref NetSegment segment);
    }
}
