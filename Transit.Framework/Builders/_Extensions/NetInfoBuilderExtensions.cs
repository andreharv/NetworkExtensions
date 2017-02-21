using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework.Network;

namespace Transit.Framework.Builders
{
    public static class NetInfoBuilderExtensions
    {
        public static string GetBasedPrefabName(this INetInfoBuilder builder, NetInfoVersion version)
        {
            if (builder is INetInfoSpecificBaseBuilder)
            {
                return ((INetInfoSpecificBaseBuilder)builder).GetSpecificBasedPrefabName(version);
            }
            else
            {
                return NetInfos.Vanilla.GetPrefabName(builder.BasedPrefabName, version);
            }
        }

        public static string GetBuiltPrefabName(this INetInfoBuilder builder, NetInfoVersion version)
        {
            if (builder is INetInfoSpecificNameBuilder)
            {
                return ((INetInfoSpecificNameBuilder)builder).GetSpecificBuiltPrefabName(version);
            }
            else
            {
                return NetInfos.New.GetPrefabName(builder.Name, version);
            }
        }

        public static IEnumerable<NetInfo> BuildEmergencyFallback(this INetInfoBuilder builder)
        {
            return builder
                .SupportedVersions
                .ToCollection()
                .Select(niv => builder.BuildEmergencyFallbackVersion(niv));
        }

        private static NetInfo BuildEmergencyFallbackVersion(this INetInfoBuilder builder, NetInfoVersion version)
        {
            var basedPrefabName = builder.GetBasedPrefabName(version);
            var builtPrefabName = builder.GetBuiltPrefabName(version);

            return Prefabs
                .Find<NetInfo>(basedPrefabName)
                .Clone(builtPrefabName);
        }

        public static IEnumerable<NetInfo> Build(this INetInfoBuilder builder, ICollection<Action> lateOperations)
        {
            // Ground versions
            var groundInfo = builder.BuildVersion(NetInfoVersion.Ground, lateOperations);
            var groundGrassInfo = builder.BuildVersion(NetInfoVersion.GroundGrass, lateOperations);
            var groundTreesInfo = builder.BuildVersion(NetInfoVersion.GroundTrees, lateOperations);

            var groundInfos = new Dictionary<NetInfoVersion, NetInfo>();
            if (groundInfo != null)
                groundInfos.Add(NetInfoVersion.Ground, groundInfo);
            if (groundGrassInfo != null)
                groundInfos.Add(NetInfoVersion.GroundGrass, groundGrassInfo);
            if (groundTreesInfo != null)
                groundInfos.Add(NetInfoVersion.GroundTrees, groundTreesInfo);

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
                var mibs = mibp.MenuItemBuilders.ToList();

                foreach (var mainInfo in groundInfos)
                {
                    var mib = mibs.FirstOrDefault(m => ((IMenuItemVersionedBuilder)m).DefaultVersion == mainInfo.Key);
                    if (mib != null)
                        mainInfo.Value.SetMenuItemConfig(mib);
                    }
                }
            else
            {
                throw new Exception("Cannot set the menuitem on netinfo, either implement IMenuItemBuilder or IMenuItemBuildersProvider");
            }

            // Setup AI
            foreach (var mainInfo in groundInfos)
            {
                var ai = mainInfo.Value.GetComponent<RoadAI>();

                ai.m_elevatedInfo = elevatedInfo;
                ai.m_bridgeInfo = bridgeInfo;
                ai.m_tunnelInfo = tunnelInfo;
                ai.m_slopeInfo = slopeInfo;
            }

            // Returning
            foreach (var mainInfo in groundInfos)
            {
                yield return mainInfo.Value;
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
                var basedPrefabVersion = version;
                if (builder is IMultiNetInfoBuilderPart && ((IMultiNetInfoBuilderPart)builder).UseGroundBasedPrefabName)
                {
                    if (version == NetInfoVersion.GroundGrass || version == NetInfoVersion.GroundTrees)
                    {
                        basedPrefabVersion = NetInfoVersion.Ground;
                    }
                }
                var basedPrefabName = builder.GetBasedPrefabName(basedPrefabVersion);
                var builtPrefabName = builder.GetBuiltPrefabName(version);

                var info = Prefabs
                    .Find<NetInfo>(basedPrefabName)
                    .Clone(builtPrefabName);

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
