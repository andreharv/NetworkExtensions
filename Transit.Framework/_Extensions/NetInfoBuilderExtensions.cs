using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework.Interfaces;
using Transit.Framework.Modularity;

namespace Transit.Framework
{
    public static class NetInfoBuilderExtensions
    {
        public static IEnumerable<NetInfo> Build(this INetInfoBuilder builder)
        {
            // Ground versions
            var groundInfo = builder.BuildVersion(NetInfoVersion.Ground);
            var groundGrassInfo = builder.BuildVersion(NetInfoVersion.GroundGrass);
            var groundTreesInfo = builder.BuildVersion(NetInfoVersion.GroundTrees);

            var groundInfos = new[] {groundInfo, groundGrassInfo, groundTreesInfo};
            groundInfos = groundInfos.Where(gi => gi != null).ToArray();

            if (!groundInfos.Any())
            {
                yield break;
            }

            // Other versions
            var elevatedInfo = builder.BuildVersion(NetInfoVersion.Elevated);
            var bridgeInfo = builder.BuildVersion(NetInfoVersion.Bridge);
            var tunnelInfo = builder.BuildVersion(NetInfoVersion.Tunnel);
            var slopeInfo = builder.BuildVersion(NetInfoVersion.Slope);

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
                var ai = mainInfo.GetComponent<RoadAI>();

                ai.m_elevatedInfo = elevatedInfo;
                ai.m_bridgeInfo = bridgeInfo;
                ai.m_tunnelInfo = tunnelInfo;
                ai.m_slopeInfo = slopeInfo;
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

        public static NetInfo BuildVersion(this INetInfoBuilder builder, NetInfoVersion version)
        {
            if (builder.SupportedVersions.HasFlag(version))
            {
                var vanillaPrefabName = NetInfos.Vanilla.GetPrefabName(builder.BasedPrefabName, version);
                var newPrefabName = NetInfos.New.GetPrefabName(builder.Name, version);

                var info = Prefabs
                    .Find<NetInfo>(vanillaPrefabName)
                    .Clone(newPrefabName);

                builder.BuildUp(info, version);

                return info;
            }
            else
            {
                return null;
            }
        }
    }
}
