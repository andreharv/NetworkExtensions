using Transit.Framework;
using Transit.Framework.Network;
using Transit.Framework.Texturing;

namespace TransitAT.Addon.RoadExtensions.Roads.SmallHeavyRoads.TramSmallAve
{
    public partial class TramSmallAveBuilder
    {
        public static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            Transit.Addon.RoadExtensions.Roads.SmallHeavyRoads.SmallAvenue4L.SmallAvenue4LBuilder.SetupTextures(info, version);
        }
    }
}

