using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Transit.Addon.RoadExtensions.Compatibility;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Framework.Modularity;
using UnityEngine;

#if DEBUG

#endif

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        [UsedImplicitly]
        private class RoadsInstaller : Installer<RExModule>
        {
            protected override bool ValidatePrerequisites()
            {
                var roadObject = GameObject.Find(ROAD_NETCOLLECTION);
                if (roadObject == null)
                {
                    return false;
                }

                var netColl = FindObjectsOfType<NetCollection>();
                if (netColl == null || !netColl.Any())
                {
                    return false;
                }

                var roadCollFound = false;
                foreach (var col in netColl)
                {
                    if (col.name == ROAD_NETCOLLECTION)
                    {
                        roadCollFound = true;
                    }
                }

                if (!roadCollFound)
                {
                    return false;
                }

                return true;
            }

            protected override void Install(RExModule host)
            {
                InstallPropInfos(host);
                InstallNetInfos(host);
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

                if (TrafficPlusPlus.IsPluginActive)
                {
                    piBuilders = piBuilders
                        .Where(t => !(t is ITrafficPlusPlusPart))
                        .ToArray();
                }

                foreach (var piBuilder in piBuilders)
                {
                    var builder = piBuilder;

                    Loading.QueueAction(() =>
                    {
                        try
                        {
                            newInfos.Add(builder.Build());

                            Log.Info(string.Format("REx: Prop {0} installed", builder.Name));
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("REx: Crashed-Prop builder {0}", builder.Name));
                            Log.Error("REx: " + ex.Message);
                            Log.Error("REx: " + ex.ToString());

                            Log.Info(string.Format("REx: Fallbacking-Prop builder {0}", builder.Name));
                            try
                            {
                                newInfos.Add(builder.BuildEmergencyFallback());
                            }
                            catch (Exception exFallback)
                            {
                                Log.Error(string.Format("REx: Crashed-Fallback Prop builder {0}", builder.Name));
                                Log.Error("REx: " + exFallback.Message);
                                Log.Error("REx: " + exFallback.ToString());
                            }
                        }
                    });
                }

                Loading.QueueAction(() =>
                {
                    var props = host._props = host._container.AddComponent<PropCollection>();
                    props.name = REX_PROPCOLLECTION;
                    if (newInfos.Count > 0)
                    {
                        props.m_prefabs = newInfos.ToArray();
                        PrefabCollection<PropInfo>.InitializePrefabs(props.name, props.m_prefabs, new string[] {});
                        PrefabCollection<PropInfo>.BindPrefabs();
                    }
                });
            }

            private static void InstallNetInfos(RExModule host)
            {
                var newInfos = new List<NetInfo>();

                var niBuilders = host.Parts
                    .OfType<INetInfoBuilder>()
                    .WhereActivated()
                    .ToArray();

                if (TrafficPlusPlus.IsPluginActive)
                {
                    niBuilders = niBuilders
                        .Where(t => !(t is ITrafficPlusPlusPart))
                        .ToArray();
                }

                var lateOperations = new List<Action>();

                foreach (var niBuilder in niBuilders)
                {
                    var builder = niBuilder;

                    Loading.QueueAction(() =>
                    {
                        try
                        {
                            newInfos.AddRange(builder.Build(lateOperations));

                            Log.Info(string.Format("REx: {0} installed", builder.Name));
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("REx: Crashed-Network builder {0}", builder.Name));
                            Log.Error("REx: " + ex.Message);
                            Log.Error("REx: " + ex.ToString());

                            Log.Info(string.Format("REx: Fallbacking-Network builder {0}", builder.Name));
                            try
                            {
                                newInfos.AddRange(builder.BuildEmergencyFallback());
                            }
                            catch (Exception exFallback)
                            {
                                Log.Error(string.Format("REx: Crashed-Fallback Network builder {0}", builder.Name));
                                Log.Error("REx: " + exFallback.Message);
                                Log.Error("REx: " + exFallback.ToString());
                            }
                        }
                    });
                }

                Loading.QueueAction(() =>
                {
                    var roads = host._roads = host._container.AddComponent<NetCollection>();
                    roads.name = REX_NETCOLLECTION;
                    if (newInfos.Count > 0)
                    {
                        roads.m_prefabs = newInfos.ToArray();
                        PrefabCollection<NetInfo>.InitializePrefabs(roads.name, roads.m_prefabs, new string[] {});
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

                    Loading.QueueAction(() =>
                    {
                        try
                        {
                            modifier.ModifyExistingNetInfo();

                            Log.Info(string.Format("REx: {0} modifications applied", modifier.Name));
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("REx: Crashed-Network modifiers {0}", modifier.Name));
                            Log.Error("REx: " + ex.Message);
                            Log.Error("REx: " + ex.ToString());
                        }
                    });
                }
            }

            private static void InstallCompatibilities(RExModule host)
            {
                Loading.QueueAction(() =>
                {
                    var compParts = host.Parts
                        .OfType<ICompatibilityPart>()
                        .ToArray();

                    foreach (var compatibilityPart in compParts)
                    {
                        try
                        {
                            if (compatibilityPart.IsPluginActive)
                            {
                                compatibilityPart.Setup(host._roads.m_prefabs);

                                Log.Info(string.Format("REx: {0} compatibility activated", compatibilityPart.Name));
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("REx: Crashed-CompatibilitySupport {0}", compatibilityPart.Name));
                            Log.Error("REx: " + ex.Message);
                            Log.Error("REx: " + ex.ToString());
                        }
                    }
                });
            }
        }
    }
}
