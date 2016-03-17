using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework.Network;

namespace Transit.Framework.Builders
{
    public static class NetInfoBuilderExtensions
    {
        public static IEnumerable<NetInfo> BuildEmergencyFallback(this INetInfoBuilder builder)
        {
            return builder
                .SupportedVersions
                .ToCollection()
                .Select(niv => builder.BuildEmergencyFallbackVersion(niv));
        }

        private static NetInfo BuildEmergencyFallbackVersion(this INetInfoBuilder builder, NetInfoVersion version)
        {
            var vanillaPrefabName = NetInfos.Vanilla.GetPrefabName(builder.BasedPrefabName, version);
            var newPrefabName = NetInfos.New.GetPrefabName(builder.Name, version);

            return Prefabs
                .Find<NetInfo>(vanillaPrefabName)
                .Clone(newPrefabName);
        }

        public static IEnumerable<NetInfo> Build(this INetInfoBuilder builder, ICollection<Action> lateOperations)
        {
            // Ground versions
            var groundInfo = builder.BuildVersion(NetInfoVersion.Ground, lateOperations);
            var groundGrassInfo = builder.BuildVersion(NetInfoVersion.GroundGrass, lateOperations);
            var groundTreesInfo = builder.BuildVersion(NetInfoVersion.GroundTrees, lateOperations);

            var groundInfos = new[] {groundInfo, groundGrassInfo, groundTreesInfo};
            groundInfos = groundInfos.Where(gi => gi != null).ToArray();

            if (!groundInfos.Any())
            {
                yield break;
            }

            // Other versions
            var elevatedInfo = builder.BuildVersion(NetInfoVersion.Elevated, lateOperations);
            var bridgeInfo = builder.BuildVersion(NetInfoVersion.Bridge, lateOperations);
            var tunnelInfo = builder.BuildVersion(NetInfoVersion.Tunnel, lateOperations);
            var slopeInfo = builder.BuildVersion(NetInfoVersion.Slope, lateOperations);

            // Setup MenuItems
            if (builder is IMenuItemBuilder)
            {
                if (groundInfos.Count() > 1)
                {
                    throw new Exception("Multiple netinfo menuitem cannot be build with the IMenuItemBuilder, use the IMenuItemBuildersProvider");
                }

                var mib = builder as IMenuItemBuilder;
                groundInfos[0].SetMenuItemConfig(mib);
            }
            else if (builder is IMenuItemBuildersProvider)
            {
                var mibp = builder as IMenuItemBuildersProvider;
                var mibs = mibp.MenuItemBuilders.ToDictionary(x => x.Name, StringComparer.InvariantCultureIgnoreCase);

                foreach (var mainInfo in groundInfos)
                {
                    if (mibs.ContainsKey(mainInfo.name))
                    {
                        var mib = mibs[mainInfo.name];
                        mainInfo.SetMenuItemConfig(mib);
                    }
                }
            }
            else
            {
                throw new Exception("Cannot set the menuitem on netinfo, either implement IMenuItemBuilder or IMenuItemBuildersProvider");
            }

            // Setup AI
            foreach (var mainInfo in groundInfos)
            {
                var tai = mainInfo.GetComponent<TrainTrackAI>();
                var rai = mainInfo.GetComponent<RoadAI>();

                if (tai != null)
                {
                    tai.m_elevatedInfo = elevatedInfo;
                    tai.m_bridgeInfo = bridgeInfo;
                    tai.m_tunnelInfo = tunnelInfo;
                    tai.m_slopeInfo = slopeInfo;
                }
                else
                {
                    rai.m_elevatedInfo = elevatedInfo;
                    rai.m_bridgeInfo = bridgeInfo;
                    rai.m_tunnelInfo = tunnelInfo;
                    rai.m_slopeInfo = slopeInfo;
                }
            }

            // Returning
            foreach (var mainInfo in groundInfos)
            {
                yield return mainInfo;
            }
            if (elevatedInfo != null) yield return elevatedInfo;
            if (bridgeInfo != null) yield return bridgeInfo;
            if (tunnelInfo != null) yield return tunnelInfo;
            if (slopeInfo != null) yield return slopeInfo;
        }

        public static NetInfo BuildVersion(this INetInfoBuilder builder, NetInfoVersion version, ICollection<Action> lateOperations)
        {
            if (builder.SupportedVersions.HasFlag(version))
            {
                var vanillaPrefabName = NetInfos.Vanilla.GetPrefabName(builder.BasedPrefabName, version);
                var newPrefabName = NetInfos.New.GetPrefabName(builder.Name, version);

                var info = Prefabs
                    .Find<NetInfo>(vanillaPrefabName)
                    .Clone(newPrefabName);

                builder.BuildUp(info, version);

                var lateBuilder = builder as INetInfoLateBuilder;
                if (lateBuilder != null)
                {
                    lateOperations.Add(() => lateBuilder.LateBuildUp(info, version));
                }

                return info;
            }
            else
            {
                return null;
            }
        }
    }
}
