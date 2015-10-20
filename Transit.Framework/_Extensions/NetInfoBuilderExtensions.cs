using System;
using System.Collections.Generic;
using Transit.Framework.Modularity;

namespace Transit.Framework
{
    public static class NetInfoBuilderExtensions
    {
        public static IEnumerable<NetInfo> Build(this INetInfoBuilder builder)
        {
            var newNetInfos = new List<NetInfo>();

            // Ground version--------------------------------------------------
            var mainInfo = builder.BuildVersion(NetInfoVersion.Ground, newNetInfos);
            mainInfo.SetMenuItemConfig(builder);

            // Other versions -------------------------------------------------
            var mainInfoAI = mainInfo.GetComponent<RoadAI>();

            mainInfoAI.m_elevatedInfo = builder.BuildVersion(NetInfoVersion.Elevated, newNetInfos);
            mainInfoAI.m_bridgeInfo = builder.BuildVersion(NetInfoVersion.Bridge, newNetInfos);
            mainInfoAI.m_tunnelInfo = builder.BuildVersion(NetInfoVersion.Tunnel, newNetInfos);
            mainInfoAI.m_slopeInfo = builder.BuildVersion(NetInfoVersion.Slope, newNetInfos);

            return newNetInfos;
        }

        public static NetInfo BuildVersion(this INetInfoBuilder builder, NetInfoVersion version, ICollection<NetInfo> holdingCollection)
        {
            var result = BuildVersion(builder, version);

            if (result != null)
            {
                holdingCollection.Add(result);
            }

            return result;
        }

        public static NetInfo BuildVersion(this INetInfoBuilder builder, NetInfoVersion version)
        {
            if (builder.SupportedVersions.HasFlag(version))
            {
                var vanillaPrefabName = NetInfos.Vanilla.GetPrefabName(builder.TemplateName, version);
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

        public static NetInfo BuildVersion(this IMultiNetInfoBuilder builder, NetInfoVersionExtended version)
        {
            if (builder.SupportedVersions.HasFlag(version))
            {
                var vanillaPrefabName = NetInfos.Vanilla.GetPrefabName(builder.TemplateName, version);
                var newPrefabName = NetInfos.New.GetPrefabName(builder.Name, version);

                var info = Prefabs
                    .Find<NetInfo>(vanillaPrefabName)
                    .Clone(newPrefabName);

                builder.BuildUp(info, version);

                switch (version)
                {
                    case NetInfoVersionExtended.Ground:
                    case NetInfoVersionExtended.GroundGrass:
                    case NetInfoVersionExtended.GroundTrees:
                        var menuItemConfig = builder.GetMenuItemConfig(version);
                        info.SetMenuItemConfig(menuItemConfig);
                        break;
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
