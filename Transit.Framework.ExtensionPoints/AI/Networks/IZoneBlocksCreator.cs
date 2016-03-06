namespace Transit.Framework.ExtensionPoints.AI.Networks
{
    public interface IZoneBlocksCreator
    {
        void CreateZoneBlocks(NetInfo info, ushort segmentId, ref NetSegment segment);
    }
}
