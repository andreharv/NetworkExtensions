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
using System.IO;

#if DEBUG
using Debug = Transit.Framework.Debug;
#else
using Debug = UnityEngine.Debug;
#endif

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        private const string ROAD_RESOURCES = @"Resources\Roads\";
        private static Dictionary<string, ResourceUnit> m_NewInfoResources;
        protected static Dictionary<string, ResourceUnit> NewInfoResources
        {
            get
            {
                if (m_NewInfoResources == null)
                    m_NewInfoResources = new Dictionary<string, ResourceUnit>();
                return m_NewInfoResources;
            }
            set { m_NewInfoResources = value; }
        }

        [UsedImplicitly]
        private class RoadsInstaller : Installer<RExModule>
        {
            private static bool m_CallbackSet;
            private static bool m_ImportFinished;
            private static string m_ResourcePath;
            private static string[] m_Filenames;
            private static int m_FilenamesLength;
            private static ImportTransitAsset[] m_ImportTransitAssets;
            protected override bool ValidatePrerequisites(RExModule host)
            {
                return ValidateRequiredNetCollections() && ValidateResourceImports(host);
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
            private static bool ValidateResourceImports(RExModule host)
            {
                var swAll = new Stopwatch();
                swAll.Start();
                Debug.Log("Checkpoint1");
                if (!m_ImportFinished)
                {
                    Debug.Log("Checkpoint2");
                    if (!m_CallbackSet)
                    {
                        Debug.Log("Checkpoint3");
                        m_CallbackSet = true;
                        ImportTransitAsset.CallbackCalled += (sender, args) =>
                        {
                            Debug.Log("Checkpoint4");
                            NewInfoResources.Add(args.Name, args.ResourceUnit);
                            Debug.Log("netinforesource added!");
                            var fileCount = ImportAllTransitAssets.FileCount;
                            if (fileCount > 0 && NewInfoResources.Count == fileCount)
                            {
                                Debug.Log("Checkpoint5");
                                m_ImportFinished = true;
                            }
                        };
                    }

                    Debug.Log("Checkpoint6");
                    if (string.IsNullOrEmpty(m_ResourcePath))
                        m_ResourcePath = Path.Combine(host.AssetPath, ROAD_RESOURCES);
                    if (m_ImportTransitAssets != null || Directory.Exists(m_ResourcePath))
                    {
                        Debug.Log("Checkpoint7");
                        if (m_Filenames == null)
                        {
                            m_Filenames = Directory.GetFiles(m_ResourcePath, "*.fbx");
                            m_FilenamesLength = m_Filenames.Length;
                        }

                        if (m_FilenamesLength > 0)
                        {
                            Debug.Log("Checkpoint8");
                            var shader = Shader.Find("Custom/Net/Road");
                            if (m_ImportTransitAssets == null)
                            {
                                Debug.Log("Checkpoint9");
                                m_ImportTransitAssets = new ImportTransitAsset[m_FilenamesLength];
                            }
                            for (int i = 0; i < m_FilenamesLength; i++)
                            {
                                Debug.Log("Checkpoint10");
                                var importTransitAsset = m_ImportTransitAssets[i];
                                var filename = m_Filenames[i];
                                if (importTransitAsset == null)
                                {
                                    Debug.Log("Checkpoint11");
                                    importTransitAsset = new ImportTransitAsset();
                                    importTransitAsset.ImportAsset(shader, m_ResourcePath, filename);
                                    m_ImportTransitAssets[i] = importTransitAsset;
                                }
                                else if (!m_NewInfoResources.ContainsKey(filename))
                                {
                                    Debug.Log("Checkpoint12");
                                    m_ImportTransitAssets[i].Update();
                                }
                            }
                            if (NewInfoResources.Count == m_FilenamesLength)
                            {
                                Debug.Log("Checkpoint13");
                                Debug.Log("newInfoResources Count:" + NewInfoResources.Count);
                                if (NewInfoResources.Count > 0)
                                {
                                    var hi = NewInfoResources.First().Value;
                                    Debug.Log("Mesh exists " + (hi.Mesh != null));
                                    Debug.Log("lodMesh exists " + (hi.LodMesh != null));
                                    Debug.Log("Material exists " + (hi.Material != null));
                                    Debug.Log("lodMaterial exists " + (hi.LodMaterial != null));
                                }
                                swAll.Stop();
                                Debug.Log($"All RExModule Prereqs loaded in {swAll.ElapsedMilliseconds}ms");
                                return true;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("The specified path: " + m_ResourcePath + " does not exist");
                    }
                }
                return false;
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
                    var props = host._props = host._container.AddComponent<PropCollection>();
                    props.name = PROP_COLLECTION_NAME;
                    if (newInfos.Count > 0)
                    {
                        props.m_prefabs = newInfos.ToArray();
                        PrefabCollection<PropInfo>.InitializePrefabs(props.name, props.m_prefabs, new string[] { });
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
                    var roads = host._roads = host._container.AddComponent<NetCollection>();
                    roads.name = NET_COLLECTION_NAME;
                    if (newInfos.Count > 0)
                    {
                        roads.m_prefabs = newInfos.ToArray();
                        PrefabCollection<NetInfo>.InitializePrefabs(roads.name, roads.m_prefabs, new string[] { });
                        PrefabCollection<NetInfo>.BindPrefabs();
                    }
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
