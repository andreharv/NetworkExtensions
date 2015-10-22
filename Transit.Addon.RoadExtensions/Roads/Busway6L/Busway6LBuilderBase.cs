using System.Linq;
using Transit.Addon.RoadExtensions.Menus;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;

namespace Transit.Addon.RoadExtensions.Roads.Busway6L
{
    public abstract class Busway6LBuilderBase : Activable
    {
        public string UICategory { get { return AdditionnalMenus.ROADS_BUSWAYS; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public virtual void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Templates             //
            ///////////////////////////
            var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_UnlockMilestone = highwayInfo.m_UnlockMilestone;

            var vehiculeLanes = info
                .m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Vehicle)
                .OrderBy(l => l.m_position)
                .ToArray();

            for (int i = 0; i < vehiculeLanes.Length; i++)
            {
                var lane = vehiculeLanes[i];

                switch (i)
                {
                    case 0:
                        lane.m_laneType = NetInfo.LaneType.TransportVehicle;
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        lane.m_laneType = NetInfo.LaneType.TransportVehicle;
                        break;
                }
            }
        }
    }
}
