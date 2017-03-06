using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Transit.Framework.Interfaces;
using Transit.Framework.Network;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#else
using Debug = UnityEngine.Debug;
#endif

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

        //public static IEnumerable<NetInfo> Build(this INetInfoBuilder builder, ICollection<Action> lateOperations)
        //{
        //    // Ground versions

        //    var groundInfo = builder.BuildVersion(NetInfoVersion.Ground, lateOperations);
        //    var groundGrassInfo = builder.BuildVersion(NetInfoVersion.GroundGrass, lateOperations);
        //    var groundTreesInfo = builder.BuildVersion(NetInfoVersion.GroundTrees, lateOperations);

        //    var groundInfos = new[] { groundInfo, groundGrassInfo, groundTreesInfo };
        //    groundInfos = groundInfos.Where(gi => gi != null).ToArray();

        //    if (!groundInfos.Any())
        //    {
        //        yield break;
        //    }

        //    // Other versions
        //    var elevatedInfo = builder.BuildVersion(NetInfoVersion.Elevated, lateOperations);
        //    var bridgeInfo = builder.BuildVersion(NetInfoVersion.Bridge, lateOperations);
        //    var tunnelInfo = builder.BuildVersion(NetInfoVersion.Tunnel, lateOperations);
        //    var slopeInfo = builder.BuildVersion(NetInfoVersion.Slope, lateOperations);

        //    // Setup MenuItems
        //    var swMb = new Stopwatch();
        //    swMb.Start();
        //    if (builder is IMenuItemBuilder)
        //    {
        //        if (groundInfos.Count() > 1)
        //        {
        //            throw new Exception("Multiple netinfo menuitem cannot be build with the IMenuItemBuilder, use the IMenuItemBuildersProvider");
        //        }

        //        var mib = builder as IMenuItemBuilder;
        //        groundInfos[0].SetMenuItemConfig(mib);
        //    }
        //    else if (builder is IMenuItemBuildersProvider)
        //    {
        //        var mibp = builder as IMenuItemBuildersProvider;
        //        var mibs = mibp.MenuItemBuilders.ToDictionary(x => x.Name, StringComparer.InvariantCultureIgnoreCase);

        //        foreach (var mainInfo in groundInfos)
        //        {
        //            if (mibs.ContainsKey(mainInfo.name))
        //            {
        //                var mib = mibs[mainInfo.name];
        //                mainInfo.SetMenuItemConfig(mib);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception("Cannot set the menuitem on netinfo, either implement IMenuItemBuilder or IMenuItemBuildersProvider");
        //    }
        //    swMb.Stop();
        //    Debug.Log($"{builder.Name} menu item built in {swMb.ElapsedMilliseconds}");
        //    // Setup AI
        //    foreach (var mainInfo in groundInfos)
        //    {
        //        var ai = mainInfo.GetComponent<RoadAI>();

        //        ai.m_elevatedInfo = elevatedInfo;
        //        ai.m_bridgeInfo = bridgeInfo;
        //        ai.m_tunnelInfo = tunnelInfo;
        //        ai.m_slopeInfo = slopeInfo;
        //    }

        //    // Returning
        //    foreach (var mainInfo in groundInfos)
        //    {
        //        yield return mainInfo;
        //    }
        //    if (elevatedInfo != null) yield return elevatedInfo;
        //    if (bridgeInfo != null) yield return bridgeInfo;
        //    if (tunnelInfo != null) yield return tunnelInfo;
        //    if (slopeInfo != null) yield return slopeInfo;
        //}

        public static NetInfo BuildVersion(this INetInfoBuilder builder, NetInfoVersion version, ICollection<Action> lateOperations)
        {
            if (builder.SupportedVersions.HasFlag(version))
            {
                var sw = new Stopwatch();
                var basedPrefabName = builder.GetBasedPrefabName(version);
                var builtPrefabName = builder.GetBuiltPrefabName(version);

                sw.Start();
                var info = Prefabs
                    .Find<NetInfo>(basedPrefabName)
                    .Clone(builtPrefabName);
                sw.Stop();
                Debug.Log($"cloned {builder.BasedPrefabName} as {builder.Name} in {sw.ElapsedMilliseconds}");
                builder.BuildUp(info, version);

                var lateBuilder = builder as INetInfoLateBuilder;
                if (lateBuilder != null)
                {
                    var swl = new Stopwatch();
                    swl.Start();
                    lateOperations.Add(() => lateBuilder.LateBuildUp(info, version));
                    swl.Stop();
                    Debug.Log($"cloned {builder.BasedPrefabName} in {swl.ElapsedMilliseconds}");
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
