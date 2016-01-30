namespace Transit.Framework.Extenders.AI
{
    public interface IZoneBlocksCreator
    {
        void CreateZoneBlocks(NetInfo info, ushort segmentId, ref NetSegment segment);
    }
}
