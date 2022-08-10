using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;
using UnityEngine;
using System.Diagnostics;
using NetworkExtensions2.Framework.Import;

#if DEBUG
using Debug = Transit.Framework.Debug;
#else
using Debug = UnityEngine.Debug;
#endif

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        [UsedImplicitly]
        private class RoadsInstaller : Installer<RExModule>
        {
            protected override bool ValidatePrerequisites(RExModule host)
            {
                var prereq = ValidateRequiredNetCollections() && ImportTransitAsset.UptakeImportFiles(host.AssetPath, AssetType.Roads);
                Debug.Log("Cx prereq is: " + prereq);
                    return prereq;
            }
            private static bool ValidateRequiredNetCollections()
            {
                foreach (var req in RequiredNetCollections)
                {
                    if (GameObject.Find(req) == null)
                    {
                        return false;
                    }
                }

                var netColl = FindObjectsOfType<NetCollection>();
                if (netColl == null || !netColl.Any())
                {
                    return false;
                }
                if (RequiredNetCollections.Any(r => netColl.Any(n => n.name == r) == false))
                {
                    return false;
                }
                return true;
            }

            protected override void Install(RExModule host)
            {
                InstallPropInfos(host);
                var swAll = new Stopwatch();
                swAll.Start();
                InstallNetInfos(host);
                swAll.Stop();
                Debug.Log($"All RExModule NetInfos in {swAll.ElapsedMilliseconds}ms");
                InstallNetInfosModifiers(host);
                InstallCompatibilities(host);
            }
            private static void InstallPropInfos(RExModule host)
            {
                var newInfos = new List<PropInfo>();
                var piBuilders = host.Parts
                    .OfType<IPrefabBuilder<PropInfo>>()
                    .WhereActivated()
                    .ToArray();
                foreach (var piBuilder in piBuilders)
                {
                    var builder = piBuilder;
                    Stopwatch sw = new Stopwatch();
                    Loading.QueueAction(() =>
                    {
                        try
                        {
                            sw.Start();
                            newInfos.Add(builder.Build());
                            sw.Stop();
                            Debug.Log(string.Format("REx: Prop {0} installed in {1}ms", builder.Name, sw.ElapsedMilliseconds));
                        }
                        catch (Exception ex)
                        {
                            Debug.Log(string.Format("REx: Crashed-Prop builder {0}", builder.Name));
                            Debug.Log("REx: " + ex.Message);
                            Debug.Log("REx: " + ex.ToString());

                            Debug.Log(string.Format("REx: Fallbacking-Prop builder {0}", builder.Name));
                            try
                            {
                                newInfos.Add(builder.BuildEmergencyFallback());
                            }
                            catch (Exception exFallback)
                            {
                                Debug.Log(string.Format("REx: Crashed-Fallback Prop builder {0}", builder.Name));
                                Debug.Log("REx: " + exFallback.Message);
                                Debug.Log("REx: " + exFallback.ToString());
                            }
                        }
                    });
                }

                Loading.QueueAction(() =>
                {
                    if (newInfos.Count > 0)
                    {
                        PrefabCollection<PropInfo>.InitializePrefabs(PROP_COLLECTION_NAME, newInfos.ToArray(), null);
                        PrefabCollection<PropInfo>.BindPrefabs();
                    }
                });
            }

            private static readonly Dictionary<string, NetInfo> m_BasedPrefabs = new Dictionary<string, NetInfo>();
            private static readonly Dictionary<NetInfoVersion, NetInfo> m_Infos = new Dictionary<NetInfoVersion, NetInfo>();
            private void InstallNetInfos(RExModule host)
            {
                var newInfos = new List<NetInfo>();

                var niBuilders = host.Parts
                    .OfType<INetInfoBuilder>()
                    .WhereActivated()
                    .ToArray();

                var lateOperations = new List<Action>();
                foreach (var niBuilder in niBuilders)
                {
                    var builder = niBuilder;

                    Loading.QueueAction(() =>
                    {
                        Stopwatch sw = new Stopwatch();
                        try
                        {
                            sw.Start();
                            var isMultiMenu = builder is IMenuItemBuildersProvider;
                            var isSingleMenu = builder is IMenuItemBuilder;
                            m_Infos.Clear();
                            foreach (NetInfoVersion version in Enum.GetValues(typeof(NetInfoVersion)))
                            {
                                NetInfo info = null;
                                if (version != NetInfoVersion.All && version != NetInfoVersion.AllWithDecoration && builder.SupportedVersions.HasFlag(version))
                                {
                                    var basedPrefabName = builder.GetBasedPrefabName(version);
                                    var newPrefabName = builder.GetBuiltPrefabName(version);
                                    if (m_BasedPrefabs.ContainsKey(basedPrefabName) == false)
                                    {
                                        m_BasedPrefabs.Add(basedPrefabName, Resources.FindObjectsOfTypeAll<NetInfo>().FirstOrDefault(netInfo => netInfo.name == basedPrefabName));
                                    }
                                    info = m_BasedPrefabs[basedPrefabName].Clone(newPrefabName, transform);

                                    builder.BuildUp(info, version);

                                    var lateBuilder = builder as INetInfoLateBuilder;
                                    if (lateBuilder != null)
                                    {
                                        lateOperations.Add(() => lateBuilder.LateBuildUp(info, version));
                                    }
                                    m_Infos.Add(version, info);
                                }

                            }
                            List<NetInfo> groundInfos = new List<NetInfo>();
                            foreach (var kvp in m_Infos)
                            {
                                if (kvp.Value != null)
                                {
                                    if (kvp.Key == NetInfoVersion.Ground || kvp.Key == NetInfoVersion.GroundGrass || kvp.Key == NetInfoVersion.GroundTrees)
                                    {
                                        groundInfos.Add(kvp.Value);
                                    }
                                }
                            }
                            if (isSingleMenu)
                            {
                                if (m_Infos.ContainsKey(NetInfoVersion.GroundGrass) || m_Infos.ContainsKey(NetInfoVersion.GroundTrees))
                                {
                                    throw new Exception("Multiple netinfo menuitem cannot be build with the IMenuItemBuilder, use the IMenuItemBuildersProvider");
                                }
                                else
                                {
                                    var mib = builder as IMenuItemBuilder;
                                    m_Infos[NetInfoVersion.Ground].SetMenuItemConfig(mib);
                                }

                            }
                            else if (builder is IMenuItemBuildersProvider)
                            {
                                var mibp = builder as IMenuItemBuildersProvider;
                                var mibs = mibp.MenuItemBuilders.ToDictionary(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
                                foreach (var info in groundInfos)
                                {
                                    if (mibs.ContainsKey(info.name))
                                    {
                                        var mib = mibs[info.name];
                                        info.SetMenuItemConfig(mib);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("Cannot set the menuitem on netinfo, either implement IMenuItemBuilder or IMenuItemBuildersProvider");
                            }
                            foreach (var info in groundInfos)
                            {
                                var ai = info.GetComponent<RoadAI>();

                                ai.m_elevatedInfo = m_Infos.ContainsKey(NetInfoVersion.Elevated) ? m_Infos[NetInfoVersion.Elevated] : null;
                                ai.m_bridgeInfo = m_Infos.ContainsKey(NetInfoVersion.Bridge) ? m_Infos[NetInfoVersion.Bridge] : null;
                                ai.m_tunnelInfo = m_Infos.ContainsKey(NetInfoVersion.Tunnel) ? m_Infos[NetInfoVersion.Tunnel] : null;
                                ai.m_slopeInfo = m_Infos.ContainsKey(NetInfoVersion.Slope) ? m_Infos[NetInfoVersion.Slope] : null;
                            }

                            newInfos.AddRange(m_Infos.Values);
                            sw.Stop();

                            Debug.Log(string.Format("REx: {0} installed in {1}ms", builder.Name, sw.ElapsedMilliseconds));
                        }
                        catch (Exception ex)
                        {

                            Debug.Log(string.Format("REx: Crashed-Network builder {0}", builder.Name));
                            Debug.Log("REx: " + ex.Message);
                            Debug.Log("REx: " + ex.ToString());

                            Debug.Log(string.Format("REx: Fallbacking-Network builder {0}", builder.Name));
                            try
                            {
                                newInfos.AddRange(builder.BuildEmergencyFallback());
                            }
                            catch (Exception exFallback)
                            {
                                Debug.Log(string.Format("REx: Crashed-Fallback Network builder {0}", builder.Name));
                                Debug.Log("REx: " + exFallback.Message);
                                Debug.Log("REx: " + exFallback.ToString());
                            }
                        }
                    });
                }

                Loading.QueueAction(() =>
                {
                    PrefabCollection<NetInfo>.InitializePrefabs(NET_COLLECTION_NAME, newInfos.ToArray(), null);
                    PrefabCollection<NetInfo>.BindPrefabs();
                });

                Loading.QueueAction(() =>
                {
                    host._lateOperations = lateOperations;
                });
            }

            private static void InstallNetInfosModifiers(RExModule host)
            {
                var niModifiers = host.Parts
                    .OfType<INetInfoModifier>()
                    .WhereActivated()
                    .ToArray();

                foreach (var nimodifier in niModifiers)
                {
                    var modifier = nimodifier;
                    Stopwatch sw = new Stopwatch();
                    Loading.QueueAction(() =>
                    {
                        try
                        {
                            sw.Start();
                            modifier.ModifyExistingNetInfo();
                            sw.Stop();
                            Debug.Log(string.Format("REx: {0} modifications applied in {1}ms", modifier.Name, sw.ElapsedMilliseconds));
                        }
                        catch (Exception ex)
                        {
                            Debug.Log(string.Format("REx: Crashed-Network modifiers {0}", modifier.Name));
                            Debug.Log("REx: " + ex.Message);
                            Debug.Log("REx: " + ex.ToString());
                        }
                    });
                }
            }

            private static void InstallCompatibilities(RExModule host)
            {
                var swAll = new Stopwatch();
                swAll.Start();
                Stopwatch sw = new Stopwatch();
                Loading.QueueAction(() =>
                {
                    var compParts = host.Parts
                        .OfType<ICompatibilityPart>()
                        .ToArray();
                    sw.Stop();
                    Debug.Log($"Compatibilities got in {sw.ElapsedMilliseconds}ms");
                    foreach (var compatibilityPart in compParts)
                    {
                        Stopwatch sw2 = new Stopwatch();
                        try
                        {
                            sw2.Start();
                            if (compatibilityPart.IsPluginActive)
                            {
                                compatibilityPart.Setup(host._roads.m_prefabs);
                                sw2.Stop();
                                Debug.Log(string.Format("REx: {0} compatibility activated in {1}", compatibilityPart.Name, sw2.ElapsedMilliseconds));
                            }
                            else
                            {
                                sw2.Stop();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Log(string.Format("REx: Crashed-CompatibilitySupport {0}", compatibilityPart.Name));
                            Debug.Log("REx: " + ex.Message);
                            Debug.Log("REx: " + ex.ToString());
                        }
                    }
                });
                swAll.Stop();
                Debug.Log($"All Compatibilities in {swAll.ElapsedMilliseconds}ms");
            }
        }
    }
}
