using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using CSL_Traffic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;
using UnityEngine;

namespace CSL_Traffic
{
    public class Initializer : MonoBehaviour
    {

        static Queue<IEnumerator> sm_actionQueue = new Queue<IEnumerator>();
        static System.Object sm_queueLock = new System.Object();
        static bool sm_localizationInitialized;
        static readonly string[] sm_collectionPrefixes = new string[] { "", "Europe " };

        public static Dictionary<string, TextureInfo> sm_fileIndex = new Dictionary<string, TextureInfo>();
        Dictionary<string, Texture2D> m_customTextures;
        Dictionary<string, VehicleAI> m_replacedAIs;
        bool m_initialized;
        bool m_incompatibilityWarning;
        float m_gameStartedTime;
        int m_level;

        void Awake()
        {
            DontDestroyOnLoad(this);

            m_customTextures = new Dictionary<string, Texture2D>();
            m_replacedAIs = new Dictionary<string, VehicleAI>();
            //m_postLoadingActions = new Queue<Action>();

            LoadTextureIndex();
        }

        void Start()
        {
            if ((CSLTraffic.Options & OptionsManager.ModOptions.GhostMode) != OptionsManager.ModOptions.GhostMode)
            {
                ReplacePathManager();
                ReplaceTransportManager();
            }
        }

        void OnLevelWasLoaded(int level)
        {
            this.m_level = level;

            if (level == 6)
            {
                Logger.LogInfo("Game level was loaded. Options enabled: \n\t" + CSLTraffic.Options);

                m_initialized = false;

                while (!Monitor.TryEnter(sm_queueLock, SimulationManager.SYNCHRONIZE_TIMEOUT)) { }
                try
                {
                    sm_actionQueue.Clear();
                }
                finally
                {
                    Monitor.Exit(sm_queueLock);
                }

                m_replacedAIs.Clear();
                //m_postLoadingActions.Clear();
            }
        }

        public void OnLevelUnloading()
        {
            if ((CSLTraffic.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
            {
                for (uint i = 0; i < PrefabCollection<CitizenInfo>.LoadedCount(); i++)
                {
                    CitizenInfo cit = PrefabCollection<CitizenInfo>.GetLoaded(i);
                    cit.m_walkSpeed /= 0.25f;
                }
            }

            for (uint i = 0; i < PrefabCollection<VehicleInfo>.LoadedCount(); i++)
            {
                //SetRealisitcSpeeds(PrefabCollection<VehicleInfo>.GetLoaded(i), false);
                SetOriginalAI(PrefabCollection<VehicleInfo>.GetLoaded(i));
            }
        }

        void Update()
        {
            if (!m_initialized)
            {
                TryReplacePrefabs();
                return;
            }

            if ((CSLTraffic.Options & OptionsManager.ModOptions.GhostMode) == OptionsManager.ModOptions.GhostMode)
                return;

            if (!Singleton<LoadingManager>.instance.m_loadingComplete)
                return;
            else if (m_gameStartedTime == 0f)
                m_gameStartedTime = Time.realtimeSinceStartup;

            //while (m_postLoadingActions.Count > 0)
            //	m_postLoadingActions.Dequeue().Invoke();

            // contributed by Japa
            TransportTool transportTool = ToolsModifierControl.GetCurrentTool<TransportTool>();
            if (transportTool != null)
            {
                CustomTransportTool customTransportTool = ToolsModifierControl.SetTool<CustomTransportTool>();
                if (customTransportTool != null)
                {
                    customTransportTool.m_prefab = transportTool.m_prefab;
                }
            }

            // Checks if CustomPathManager have been replaced by another mod and prints a warning in the log
            // This check is only run in the first two minutes since game is loaded
            if (!m_incompatibilityWarning && (CSLTraffic.Options & OptionsManager.ModOptions.GhostMode) == OptionsManager.ModOptions.None)
            {
                if ((Time.realtimeSinceStartup - m_gameStartedTime) < 120f)
                {
                    CustomPathManager customPathManager = Singleton<PathManager>.instance as CustomPathManager;
                    if (customPathManager == null)
                    {
                        Logger.LogInfo("CustomPathManager not found! There's an incompatibility with another mod.");
                        UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Incompatibility Issue", "Traffic++ detected an incompatibility with another mod! You can continue playing but it's NOT recommended.", false);
                        m_incompatibilityWarning = true;
                    }
                }
                else
                    m_incompatibilityWarning = true;
            }
        }

        #region Initialization

        /*
         * In here I'm changing the prefabs to have my classes. This way, every time the game instantiates
         * a prefab that I've changed, that object will run my code.
         * The prefabs aren't available at the moment of creation of this class, that's why I keep trying to
         * run it on update. I want to make sure I make the switch as soon as they exist to prevent the game
         * from instantianting objects without my code.
         */
        void TryReplacePrefabs()
        {
            NetCollection beautificationNetCollection = null;
            NetCollection roadsNetCollection = null;
            NetCollection publicTansportNetCollection = null;
            VehicleCollection garbageVehicleCollection = null;
            VehicleCollection policeVehicleCollection = null;
            VehicleCollection publicTansportVehicleCollection = null;
            VehicleCollection healthCareVehicleCollection = null;
            VehicleCollection fireDepartmentVehicleCollection = null;
            VehicleCollection industrialVehicleCollection = null;
            VehicleCollection industrialFarmingVehicleCollection = null;
            VehicleCollection industrialForestryVehicleCollection = null;
            VehicleCollection industrialOilVehicleCollection = null;
            VehicleCollection industrialOreVehicleCollection = null;
            VehicleCollection residentialVehicleCollection = null;
            TransportCollection publicTransportTransportCollection = null;
            ToolController toolController = null;

            try
            {
                // NetCollections
                beautificationNetCollection = TryGetComponent<NetCollection>("Beautification");
                if (beautificationNetCollection == null)
                    return;

                roadsNetCollection = TryGetComponent<NetCollection>("Road");
                if (roadsNetCollection == null)
                    return;

                publicTansportNetCollection = TryGetComponent<NetCollection>("Public Transport");
                if (publicTansportNetCollection == null)
                    return;

                // VehicleCollections
                garbageVehicleCollection = TryGetComponent<VehicleCollection>("Garbage");
                if (garbageVehicleCollection == null)
                    return;

                policeVehicleCollection = TryGetComponent<VehicleCollection>("Police Department");
                if (policeVehicleCollection == null)
                    return;

                publicTansportVehicleCollection = TryGetComponent<VehicleCollection>("Public Transport");
                if (publicTansportVehicleCollection == null)
                    return;

                healthCareVehicleCollection = TryGetComponent<VehicleCollection>("Health Care");
                if (healthCareVehicleCollection == null)
                    return;

                fireDepartmentVehicleCollection = TryGetComponent<VehicleCollection>("Fire Department");
                if (fireDepartmentVehicleCollection == null)
                    return;

                industrialVehicleCollection = TryGetComponent<VehicleCollection>("Industrial");
                if (industrialVehicleCollection == null)
                    return;

                industrialFarmingVehicleCollection = TryGetComponent<VehicleCollection>("Industrial Farming");
                if (industrialFarmingVehicleCollection == null)
                    return;

                industrialForestryVehicleCollection = TryGetComponent<VehicleCollection>("Industrial Forestry");
                if (industrialForestryVehicleCollection == null)
                    return;

                industrialOilVehicleCollection = TryGetComponent<VehicleCollection>("Industrial Oil");
                if (industrialOilVehicleCollection == null)
                    return;

                industrialOreVehicleCollection = TryGetComponent<VehicleCollection>("Industrial Ore");
                if (industrialOreVehicleCollection == null)
                    return;

                residentialVehicleCollection = TryGetComponent<VehicleCollection>("Residential Low");
                if (residentialVehicleCollection == null)
                    return;

                // Transports
                publicTransportTransportCollection = TryGetComponent<TransportCollection>("Public Transport");
                if (publicTransportTransportCollection == null)
                    return;

                // Tools
                toolController = TryGetComponent<ToolController>("Tool Controller");
                if (toolController == null)
                    return;

            }
            catch (Exception e)
            {
                Logger.LogInfo("Unexpected " + e.GetType().Name + " getting required components: " + e.Message + "\n" + e.StackTrace + "\n");
                return;
            }

            Logger.LogInfo("Queueing prefabs for loading...");

            Singleton<LoadingManager>.instance.QueueLoadingAction(ActionWrapper(() =>
            {
                try
                {
                    if ((CSLTraffic.Options & OptionsManager.ModOptions.GhostMode) != OptionsManager.ModOptions.GhostMode && this.m_level == 6)
                    {
                        ReplaceVehicleAI(healthCareVehicleCollection);
                        ReplaceVehicleAI(publicTansportVehicleCollection);
                        ReplaceVehicleAI(industrialVehicleCollection);
                        ReplaceVehicleAI(industrialFarmingVehicleCollection);
                        ReplaceVehicleAI(industrialForestryVehicleCollection);
                        ReplaceVehicleAI(industrialOilVehicleCollection);
                        ReplaceVehicleAI(industrialOreVehicleCollection);
                        ReplaceVehicleAI(fireDepartmentVehicleCollection);
                        ReplaceVehicleAI(garbageVehicleCollection);
                        ReplaceVehicleAI(residentialVehicleCollection);
                        ReplaceVehicleAI(policeVehicleCollection);

                        StartCoroutine(HandleCustomVehicles());

                        ReplaceTransportLineAI<BusTransportLineAI>("Bus Line", publicTansportNetCollection, "Bus", publicTransportTransportCollection);

                        AddTool<CustomTransportTool>(toolController);

                        if ((CSLTraffic.Options & OptionsManager.ModOptions.BetaTestRoadCustomizerTool) == OptionsManager.ModOptions.BetaTestRoadCustomizerTool)
                            AddTool<RoadCustomizerTool>(toolController);

                        if ((CSLTraffic.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
                        {
                            for (uint i = 0; i < PrefabCollection<CitizenInfo>.LoadedCount(); i++)
                            {
                                CitizenInfo cit = PrefabCollection<CitizenInfo>.GetLoaded(i);
                                cit.m_walkSpeed *= 0.25f;
                            }
                        }
                    }

                    // Localization
                    UpdateLocalization();

                    AddQueuedActionsToLoadingQueue();

                    FileManager.ClearCache();

                }
                catch (KeyNotFoundException knf)
                {
                    Logger.LogInfo("Error initializing a prefab: " + knf.Message + "\n" + knf.StackTrace + "\n");
                }
                catch (Exception e)
                {
                    Logger.LogInfo("Unexpected " + e.GetType().Name + " initializing prefabs: " + e.Message + "\n" + e.StackTrace + "\n");
                }
            }));

            m_initialized = true;

            Logger.LogInfo("Prefabs queued for loading.");
        }

        // Replace the pathfinding system for mine
        void ReplacePathManager()
        {
            if (Singleton<PathManager>.instance as CustomPathManager != null)
                return;

            Logger.LogInfo("Replacing Path Manager");

            // Change PathManager to CustomPathManager
            FieldInfo sInstance = typeof(ColossalFramework.Singleton<PathManager>).GetFieldByName("sInstance");
            PathManager originalPathManager = ColossalFramework.Singleton<PathManager>.instance;
            CustomPathManager customPathManager = originalPathManager.gameObject.AddComponent<CustomPathManager>();
            customPathManager.SetOriginalValues(originalPathManager);

            // change the new instance in the singleton
            sInstance.SetValue(null, customPathManager);

            // change the manager in the SimulationManager
            FastList<ISimulationManager> managers = (FastList<ISimulationManager>)typeof(SimulationManager).GetFieldByName("m_managers").GetValue(null);
            managers.Remove(originalPathManager);
            managers.Add(customPathManager);

            // Destroy in 10 seconds to give time to all references to update to the new manager without crashing
            GameObject.Destroy(originalPathManager, 10f);

            Logger.LogInfo("Path Manager successfully replaced.");
        }

        void ReplaceTransportManager()
        {
            if (Singleton<TransportManager>.instance as CustomTransportManager != null)
                return;

            Logger.LogInfo("Replacing Transport Manager");

            // Change TransportManager to CustomTransportManager
            FieldInfo sInstance = typeof(ColossalFramework.Singleton<TransportManager>).GetFieldByName("sInstance");
            TransportManager originalTransportManager = ColossalFramework.Singleton<TransportManager>.instance;
            CustomTransportManager customTransportManager = originalTransportManager.gameObject.AddComponent<CustomTransportManager>();
            customTransportManager.SetOriginalValues(originalTransportManager);

            // change the new instance in the singleton
            sInstance.SetValue(null, customTransportManager);

            // change the manager in the SimulationManager
            FastList<ISimulationManager> managers = (FastList<ISimulationManager>)typeof(SimulationManager).GetFieldByName("m_managers").GetValue(null);
            managers.Remove(originalTransportManager);
            managers.Add(customTransportManager);

            // add to renderable managers
            IRenderableManager[] renderables;
            int count;
            RenderManager.GetManagers(out renderables, out count);
            if (renderables != null && count != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    TransportManager temp = renderables[i] as TransportManager;
                    if (temp != null && temp == originalTransportManager)
                    {
                        renderables[i] = customTransportManager;
                        break;
                    }
                }
            }
            else
            {
                RenderManager.RegisterRenderableManager(customTransportManager);
            }

            // Destroy in 10 seconds to give time to all references to update to the new manager without crashing
            GameObject.Destroy(originalTransportManager, 10f);

            Logger.LogInfo("Transport Manager successfully replaced.");
        }

        T TryGetComponent<T>(string name)
        {
            foreach (string prefix in sm_collectionPrefixes)
            {
                GameObject go = GameObject.Find(prefix + name);
                if (go != null)
                    return go.GetComponent<T>();
            }

            return default(T);
        }

        public static void QueuePrioritizedLoadingAction(Action action)
        {
            QueuePrioritizedLoadingAction(ActionWrapper(action));
        }

        public static void QueuePrioritizedLoadingAction(IEnumerator action)
        {
            while (!Monitor.TryEnter(sm_queueLock, SimulationManager.SYNCHRONIZE_TIMEOUT)) { }
            try
            {
                sm_actionQueue.Enqueue(action);
            }
            finally { Monitor.Exit(sm_queueLock); }
        }

        static void AddQueuedActionsToLoadingQueue()
        {
            LoadingManager loadingManager = Singleton<LoadingManager>.instance;
            object loadingLock = typeof(LoadingManager).GetFieldByName("m_loadingLock").GetValue(loadingManager);

            while (!Monitor.TryEnter(loadingLock, SimulationManager.SYNCHRONIZE_TIMEOUT)) { }
            try
            {
                FieldInfo mainThreadQueueField = typeof(LoadingManager).GetFieldByName("m_mainThreadQueue");
                Queue<IEnumerator> mainThreadQueue = (Queue<IEnumerator>)mainThreadQueueField.GetValue(loadingManager);
                if (mainThreadQueue != null)
                {
                    Queue<IEnumerator> newQueue = new Queue<IEnumerator>(mainThreadQueue.Count + 1);
                    newQueue.Enqueue(mainThreadQueue.Dequeue()); // currently running action must continue to be the first in the queue

                    while (!Monitor.TryEnter(sm_queueLock, SimulationManager.SYNCHRONIZE_TIMEOUT)) { }
                    try
                    {
                        while (sm_actionQueue.Count > 0)
                            newQueue.Enqueue(sm_actionQueue.Dequeue());
                    }
                    finally
                    {
                        Monitor.Exit(sm_queueLock);
                    }


                    while (mainThreadQueue.Count > 0)
                        newQueue.Enqueue(mainThreadQueue.Dequeue());

                    mainThreadQueueField.SetValue(loadingManager, newQueue);
                }
            }
            finally
            {
                Monitor.Exit(loadingLock);
            }
        }

        static IEnumerator ActionWrapper(Action a)
        {
            a.Invoke();
            yield break;
        }

        public static void QueueLoadingAction(Action action)
        {
            Singleton<LoadingManager>.instance.QueueLoadingAction(ActionWrapper(action));
        }

        public static void QueueLoadingAction(IEnumerator action)
        {
            Singleton<LoadingManager>.instance.QueueLoadingAction(action);
        }

        #endregion

        #region Clone Methods

        T ClonePrefab<T>(string prefabName, T[] prefabs, string newName, Transform customPrefabsHolder, bool replace = false, bool ghostMode = false) where T : PrefabInfo
        {
            T originalPrefab = prefabs.FirstOrDefault(p => p.name == prefabName);
            if (originalPrefab == null)
                return null;

            GameObject instance = GameObject.Instantiate<GameObject>(originalPrefab.gameObject);
            instance.name = newName;
            instance.transform.SetParent(customPrefabsHolder);
            instance.transform.localPosition = new Vector3(-7500, -7500, -7500);
            T newPrefab = instance.GetComponent<T>();
            //instance.SetActive(false);

            MethodInfo initMethod = GetCollectionType(typeof(T).Name).GetMethod("InitializePrefabs", BindingFlags.Static | BindingFlags.NonPublic);
            Initializer.QueuePrioritizedLoadingAction((IEnumerator)initMethod.Invoke(null, new object[] { newName, new[] { newPrefab }, new string[] { replace ? prefabName : null } }));

            if (ghostMode)
            {
                if (newPrefab.GetType() == typeof(NetInfo))
                    (newPrefab as NetInfo).m_availableIn = ItemClass.Availability.None;
                return null;
            }

            newPrefab.m_prefabInitialized = false;

            return newPrefab;
        }

        static NetLaneProps CloneNetLaneProps(string prefabName, int deltaSpace = 0)
        {
            NetLaneProps prefab = Resources.FindObjectsOfTypeAll<NetLaneProps>().FirstOrDefault(p => p.name == prefabName);
            if (prefab == null)
                return null;

            NetLaneProps newLaneProps = ScriptableObject.CreateInstance<NetLaneProps>();
            newLaneProps.m_props = new NetLaneProps.Prop[Mathf.Max(0, prefab.m_props.Length + deltaSpace)];
            Array.Copy(prefab.m_props, newLaneProps.m_props, Mathf.Min(newLaneProps.m_props.Length, prefab.m_props.Length));

            return newLaneProps;
        }

        static Type GetCollectionType(string prefabType)
        {
            switch (prefabType)
            {
                case "NetInfo":
                    return typeof(NetCollection);
                case "VehicleInfo":
                    return typeof(VehicleCollection);
                case "PropInfo":
                    return typeof(PropCollection);
                case "CitizenInfo":
                    return typeof(CitizenCollection);
                default:
                    return null;
            }
        }

        #endregion

        #region Vehicles

        void ReplaceVehicleAI(VehicleCollection collection)
        {
            foreach (VehicleInfo vehicle in collection.m_prefabs)
                ReplaceVehicleAI(vehicle);
        }

        void ReplaceVehicleAI(VehicleInfo info)
        {
            VehicleAI vAI = info.m_vehicleAI;
            if (vAI == null)
                return;

            Logger.LogInfo("Replacing " + info.name + "'s AI.");
            Type type = vAI.GetType();

            if (type == typeof(AmbulanceAI))
                ReplaceVehicleAI<CustomAmbulanceAI>(info);
            else if (type == typeof(BusAI))
                ReplaceVehicleAI<CustomBusAI>(info);
            else if (type == typeof(CargoTruckAI))
                ReplaceVehicleAI<CustomCargoTruckAI>(info);
            else if (type == typeof(FireTruckAI))
                ReplaceVehicleAI<CustomFireTruckAI>(info);
            else if (type == typeof(GarbageTruckAI))
                ReplaceVehicleAI<CustomGarbageTruckAI>(info);
            else if (type == typeof(HearseAI))
                ReplaceVehicleAI<CustomHearseAI>(info);
            else if (type == typeof(PassengerCarAI))
                ReplaceVehicleAI<CustomPassengerCarAI>(info);
            else if (type == typeof(PoliceCarAI))
                ReplaceVehicleAI<CustomPoliceCarAI>(info);
            else
                Logger.LogInfo("Replacing " + info.name + "'s AI failed.");
        }

        void ReplaceVehicleAI<T>(VehicleInfo vehicle) where T : VehicleAI
        {
            if (m_replacedAIs.ContainsKey(vehicle.name))
            {
                Logger.LogInfo("Error replacing " + vehicle.name + "'s AI. It has been replaced before");
                return;
            }

            VehicleAI originalAI = vehicle.GetComponent<VehicleAI>();
            T newAI = vehicle.gameObject.AddComponent<T>();
            CopyVehicleAIAttributes<T>(originalAI, newAI);
            m_replacedAIs[vehicle.name] = originalAI;

            vehicle.m_vehicleAI = newAI;
            newAI.m_info = vehicle;

            if ((CSLTraffic.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
            {
                SetRealisitcSpeeds(vehicle, true);
            }

            Logger.LogInfo("Successfully replaced " + vehicle.name + "'s AI.");
        }

        // TODO: set correct values on vehicles for realistic speeds
        void SetRealisitcSpeeds(VehicleInfo vehicle, bool activate)
        {
            float accelerationMultiplier;
            float maxSpeedMultiplier;
            switch (vehicle.name)
            {
                case "Ambulance":
                    accelerationMultiplier = 0.2f;
                    //vehicle.m_braking *= 0.3f;
                    //vehicle.m_turning *= 0.25f;
                    maxSpeedMultiplier = 0.5f;
                    break;
                case "Bus":
                case "Fire Truck":
                case "Garbage Truck":
                    accelerationMultiplier = 0.15f;
                    //vehicle.m_braking *= 0.25f;
                    //vehicle.m_turning *= 0.2f;
                    maxSpeedMultiplier = 0.5f;
                    break;
                case "Hearse":
                case "Police Car":
                    accelerationMultiplier = 0.25f;
                    //vehicle.m_braking *= 0.35f;
                    //vehicle.m_turning *= 0.3f;
                    maxSpeedMultiplier = 0.5f;
                    break;
                default:
                    accelerationMultiplier = 0.25f;
                    //vehicle.m_braking *= 0.35f;
                    //vehicle.m_turning *= 0.3f;
                    maxSpeedMultiplier = 0.5f;
                    break;
            }

            if (!activate)
            {
                accelerationMultiplier = 1f / accelerationMultiplier;
                maxSpeedMultiplier = 1f / maxSpeedMultiplier;
            }

            vehicle.m_acceleration *= accelerationMultiplier;
            vehicle.m_maxSpeed *= maxSpeedMultiplier;
        }

        void SetOriginalAI(VehicleInfo vehicle)
        {
            Logger.LogInfo("Resetting " + vehicle.name + "'s AI.");
            VehicleAI vAI = vehicle.m_vehicleAI;
            if (vAI == null || !this.m_replacedAIs.ContainsKey(vehicle.name))
            {
                Logger.LogInfo("Resetting " + vehicle.name + "'s AI failed.");
                return;
            }

            vehicle.m_vehicleAI = this.m_replacedAIs[vehicle.name];
            this.m_replacedAIs[vehicle.name].m_info = vehicle;
            Destroy(vAI);
        }

        void CopyVehicleAIAttributes<T>(VehicleAI from, T to)
        {
            foreach (FieldInfo fi in typeof(T).BaseType.GetFields())
            {
                fi.SetValue(to, fi.GetValue(from));
            }
        }

        IEnumerator HandleCustomVehicles()
        {
            uint index = 0;
            List<string> replacedVehicles = new List<string>();
            while (!Singleton<LoadingManager>.instance.m_loadingComplete)
            {
                while (PrefabCollection<VehicleInfo>.LoadedCount() > index)
                {
                    VehicleInfo info = PrefabCollection<VehicleInfo>.GetLoaded(index);
                    if (info != null && info.name.EndsWith("_Data") && !replacedVehicles.Contains(info.name))
                    {
                        replacedVehicles.Add(info.name);
                        ReplaceVehicleAI(info);
                    }

                    ++index;
                }

                yield return new WaitForEndOfFrame();
            }
        }
        #endregion

        #region Transports

        void ReplaceTransportLineAI<T>(string prefabName, NetCollection collection, string transportName, TransportCollection transportCollection)
        {
            if (transform.FindChild(prefabName) != null)
                return;

            NetInfo transportLine = ClonePrefab<NetInfo>(prefabName, collection.m_prefabs, prefabName, transform, true);
            if (transportLine == null)
                return;

            Destroy(transportLine.GetComponent<TransportLineAI>());
            transportLine.gameObject.AddComponent<BusTransportLineAI>();

            TransportInfo transportInfo = transportCollection.m_prefabs.FirstOrDefault(p => p.name == transportName);
            if (transportInfo == null)
                return;
            //throw new KeyNotFoundException(transportName + " Transport Info not found on " + transportCollection.name);

            transportInfo.m_netInfo = transportLine;
        }

        #endregion

        #region Tools

        void AddTool<T>(ToolController toolController) where T : ToolBase
        {
            if (toolController.GetComponent<T>() != null)
                return;

            toolController.gameObject.AddComponent<T>();

            // contributed by Japa
            FieldInfo toolControllerField = typeof(ToolController).GetField("m_tools", BindingFlags.Instance | BindingFlags.NonPublic);
            if (toolControllerField != null)
                toolControllerField.SetValue(toolController, toolController.GetComponents<ToolBase>());
            FieldInfo toolModifierDictionary = typeof(ToolsModifierControl).GetField("m_Tools", BindingFlags.Static | BindingFlags.NonPublic);
            if (toolModifierDictionary != null)
                toolModifierDictionary.SetValue(null, null); // to force a refresh
        }

        #endregion

        #region Textures
        [Flags]
        enum TextureType
        {
            Normal = 0,
            Bus = 1,
            BusBoth = 2,
            Node = 4,
            LOD = 8,
            BusLOD = 9,
            BusBothLOD = 10,
            NodeLOD = 12
        }

        static string[] sm_mapNames = new string[] { "_MainTex", "_XYSMap", "_ACIMap", "_APRMap" };

        bool ReplaceTextures(TextureInfo textureInfo, TextureType textureType, FileManager.Folder textureFolder, Material mat, int anisoLevel = 8, FilterMode filterMode = FilterMode.Trilinear, bool skipCache = false)
        {
            bool success = false;
            byte[] textureBytes;
            Texture2D tex = null;

            for (int i = 0; i < sm_mapNames.Length; i++)
            {
                if (mat.HasProperty(sm_mapNames[i]) && mat.GetTexture(sm_mapNames[i]) != null)
                {
                    string fileName = GetTextureName(sm_mapNames[i], textureInfo, textureType);
                    if (!String.IsNullOrEmpty(fileName) && !m_customTextures.TryGetValue(fileName, out tex))
                    {
                        if (FileManager.GetTextureBytes(fileName + ".png", textureFolder, skipCache, out textureBytes))
                        {
                            tex = new Texture2D(1, 1);
                            tex.LoadImage(textureBytes);
                        }
                        else if (fileName.Contains("-LOD"))
                        {
                            Texture2D original = mat.GetTexture(sm_mapNames[i]) as Texture2D;
                            if (original != null)
                            {
                                tex = new Texture2D(original.width, original.height);
                                tex.SetPixels(original.GetPixels());
                                tex.Apply();
                            }
                        }
                    }

                    if (tex != null)
                    {
                        tex.name = fileName;
                        tex.anisoLevel = anisoLevel;
                        tex.filterMode = filterMode;
                        mat.SetTexture(sm_mapNames[i], tex);
                        m_customTextures[tex.name] = tex;
                        success = true;
                        tex = null;
                    }
                }
            }

            return success;
        }

        string GetTextureName(string map, TextureInfo info, TextureType type)
        {
            switch (type)
            {
                case TextureType.Normal:
                    switch (map)
                    {
                        case "_MainTex": return info.mainTex;
                        case "_XYSMap": return info.xysTex;
                        case "_ACIMap": return info.aciTex;
                        case "_APRMap": return info.aprTex;
                    }
                    break;
                case TextureType.Bus:
                    switch (map)
                    {
                        case "_MainTex": return info.mainTexBus;
                        case "_XYSMap": return info.xysTexBus;
                        case "_ACIMap": return info.aciTexBus;
                        case "_APRMap": return info.aprTexBus;
                    }
                    break;
                case TextureType.BusBoth:
                    switch (map)
                    {
                        case "_MainTex": return info.mainTexBusBoth;
                        case "_XYSMap": return info.xysTexBusBoth;
                        case "_ACIMap": return info.aciTexBusBoth;
                        case "_APRMap": return info.aprTexBusBoth;
                    }
                    break;
                case TextureType.Node:
                    switch (map)
                    {
                        case "_MainTex": return info.mainTexNode;
                        case "_XYSMap": return info.xysTexNode;
                        case "_ACIMap": return info.aciTexNode;
                        case "_APRMap": return info.aprTexNode;
                    }
                    break;
                case TextureType.LOD:
                    switch (map)
                    {
                        case "_MainTex": return info.lodMainTex;
                        case "_XYSMap": return info.lodXysTex;
                        case "_ACIMap": return info.lodAciTex;
                        case "_APRMap": return info.lodAprTex;
                    }
                    break;
                case TextureType.BusLOD:
                    switch (map)
                    {
                        case "_MainTex": return info.lodMainTexBus;
                        case "_XYSMap": return info.lodXysTexBus;
                        case "_ACIMap": return info.lodAciTexBus;
                        case "_APRMap": return info.lodAprTexBus;
                    }
                    break;
                case TextureType.BusBothLOD:
                    switch (map)
                    {
                        case "_MainTex": return info.lodMainTexBusBoth;
                        case "_XYSMap": return info.lodXysTexBusBoth;
                        case "_ACIMap": return info.lodAciTexBusBoth;
                        case "_APRMap": return info.lodAprTexBusBoth;
                    }
                    break;
                case TextureType.NodeLOD:
                    switch (map)
                    {
                        case "_MainTex": return info.lodMainTexNode;
                        case "_XYSMap": return info.lodXysTexNode;
                        case "_ACIMap": return info.lodAciTexNode;
                        case "_APRMap": return info.lodAprTexNode;
                    }
                    break;
                default:
                    break;
            }

            return null;
        }

        #endregion

        // TODO: Put this in its own class
        void UpdateLocalization()
        {
            if (sm_localizationInitialized)
                return;

            Logger.LogInfo("Updating Localization.");

            try
            {
                // Localization
                Locale locale = (Locale)typeof(LocaleManager).GetFieldByName("m_Locale").GetValue(SingletonLite<LocaleManager>.instance);
                if (locale == null)
                    throw new KeyNotFoundException("Locale is null");

                // Road Customizer Tool Advisor
                Locale.Key k = new Locale.Key()
                {
                    m_Identifier = "TUTORIAL_ADVISER_TITLE",
                    m_Key = "RoadCustomizer"
                };
                locale.AddLocalizedString(k, "Road Customizer Tool");

                k = new Locale.Key()
                {
                    m_Identifier = "TUTORIAL_ADVISER",
                    m_Key = "RoadCustomizer"
                };
                locale.AddLocalizedString(k, "Vehicle and Speed Restrictions:\n\n" +
                                                "1. Hover over roads to display their lanes\n" +
                                                "2. Left-click to toggle selection of lane(s), right-click clears current selection(s)\n" +
                                                "3. With lanes selected, set vehicle and speed restrictions using the menu icons\n\n\n" +
                                                "Lane Changer:\n\n" +
                                                "1. Hover over roads and find an intersection (circle appears), then click to edit it\n" +
                                                "2. Entry points will be shown, click one to select it (right-click goes back to step 1)\n" +
                                                "3. Click the exit routes you wish to allow (right-click goes back to step 2)" +
                                                "\n\nUse PageUp/PageDown to toggle Underground View.");

                sm_localizationInitialized = true;
            }
            catch (ArgumentException e)
            {
                Logger.LogInfo("Unexpected " + e.GetType().Name + " updating localization: " + e.Message + "\n" + e.StackTrace + "\n");
            }

            Logger.LogInfo("Localization successfully updated.");
        }

        void LoadTextureIndex()
        {
            TextureInfo[] textureIndex = FileManager.GetTextureIndex();
            if (textureIndex == null)
                return;

            sm_fileIndex.Clear();
            foreach (TextureInfo item in textureIndex)
                sm_fileIndex.Add(item.name, item);
        }

        public class TextureInfo
        {
            [XmlAttribute]
            public string name;

            // normal
            public string mainTex = "";
            public string aprTex = "";
            public string xysTex = "";
            public string aciTex = "";
            public string lodMainTex = "";
            public string lodAprTex = "";
            public string lodXysTex = "";
            public string lodAciTex = "";

            // bus
            public string mainTexBus = "";
            public string aprTexBus = "";
            public string xysTexBus = "";
            public string aciTexBus = "";
            public string lodMainTexBus = "";
            public string lodAprTexBus = "";
            public string lodXysTexBus = "";
            public string lodAciTexBus = "";

            // busBoth
            public string mainTexBusBoth = "";
            public string aprTexBusBoth = "";
            public string xysTexBusBoth = "";
            public string aciTexBusBoth = "";
            public string lodMainTexBusBoth = "";
            public string lodAprTexBusBoth = "";
            public string lodXysTexBusBoth = "";
            public string lodAciTexBusBoth = "";

            // node
            public string mainTexNode = "";
            public string aprTexNode = "";
            public string xysTexNode = "";
            public string aciTexNode = "";
            public string lodMainTexNode = "";
            public string lodAprTexNode = "";
            public string lodXysTexNode = "";
            public string lodAciTexNode = "";
        }
    }
}
