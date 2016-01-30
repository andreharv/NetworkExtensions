namespace Transit.Framework.Builders
{
    public interface INetInfoLateBuilder : INetInfoBuilder
    {
        void LateBuildUp(NetInfo info, NetInfoVersion version);
    }
}
