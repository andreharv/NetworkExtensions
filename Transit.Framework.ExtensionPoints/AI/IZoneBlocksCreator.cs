namespace Transit.Framework.ExtensionPoints.AI
{
    public interface IZoneBlocksCreator
    {
        void CreateZoneBlocks(NetInfo info, ushort segmentId, ref NetSegment segment);
    }
}
