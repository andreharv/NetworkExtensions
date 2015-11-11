using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Transit.Addon.TrafficPP.Extensions;
using UnityEngine;

namespace Transit.Addon.TrafficPP
{
    public class Initializer : MonoBehaviour
    {
        static Queue<IEnumerator> sm_actionQueue = new Queue<IEnumerator>();
        static System.Object sm_queueLock = new System.Object();
        static bool sm_localizationInitialized;
        static readonly string[] sm_collectionPrefixes = { "", "Europe " };

        Dictionary<string, PrefabInfo> m_customPrefabs;
        Dictionary<string, Texture2D> m_customTextures;
        Dictionary<string, VehicleAI> m_replacedAIs;
        //Queue<Action> m_postLoadingActions;
        //UITextureAtlas m_thumbnailsTextureAtlas;
        bool m_initialized;
        bool m_incompatibilityWarning;
        float m_gameStartedTime;
        int m_level;

        void Awake()
        {
            DontDestroyOnLoad(this);

            m_customPrefabs = new Dictionary<string, PrefabInfo>();
            m_customTextures = new Dictionary<string, Texture2D>();
            m_replacedAIs = new Dictionary<string, VehicleAI>();
            //m_postLoadingActions = new Queue<Action>();
        }

        void Start()
        {
            if ((TrafficPPModule.Options & OptionsManager.ModOptions.GhostMode) != OptionsManager.ModOptions.GhostMode)
            {
                ReplacePathManager();
                ReplaceTransportManager();
            }
#if DEBUG
            //StartCoroutine(Print());
#endif
        }

        void OnLevelWasLoaded(int level)
        {
            this.m_level = level;

            if (level == 6)
            {
                Logger.LogInfo("Game level was loaded. Options enabled: \n\t" + TrafficPPModule.Options);

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

                m_customPrefabs.Clear();
                m_replacedAIs.Clear();
                //m_postLoadingActions.Clear();
            }
        }

        public void OnLevelUnloading()
        {
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

            if ((TrafficPPModule.Options & OptionsManager.ModOptions.GhostMode) == OptionsManager.ModOptions.GhostMode)
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
            if (!m_incompatibilityWarning && (TrafficPPModule.Options & OptionsManager.ModOptions.GhostMode) == OptionsManager.ModOptions.None)
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

#if DEBUG
            if (Input.GetKeyUp(KeyCode.KeypadPlus))
            {
                VehicleInfo vehicleInfo = null;
                Color color = default(Color);
                switch (count)
                {
                    case 0:
                        vehicleInfo = PrefabCollection<VehicleInfo>.FindLoaded("Lorry");
                        color = vehicleInfo.m_material.color;
                        break;
                    case 1:
                        vehicleInfo = PrefabCollection<VehicleInfo>.FindLoaded("Bus");
                        color = vehicleInfo.m_material.color;
                        break;
                    case 2:
                        vehicleInfo = PrefabCollection<VehicleInfo>.FindLoaded("Ambulance");
                        color = vehicleInfo.m_material.color;
                        break;
                    case 3:
                        vehicleInfo = PrefabCollection<VehicleInfo>.FindLoaded("Police Car");
                        color = vehicleInfo.m_material.color;
                        break;
                    case 4:
                        vehicleInfo = PrefabCollection<VehicleInfo>.FindLoaded("Fire Truck");
                        color = vehicleInfo.m_material.color;
                        break;
                    case 5:
                        vehicleInfo = PrefabCollection<VehicleInfo>.FindLoaded("Hearse");
                        color = vehicleInfo.m_material.color;
                        break;
                    case 6:
                        vehicleInfo = PrefabCollection<VehicleInfo>.FindLoaded("Garbage Truck");
                        color = vehicleInfo.m_material.color;
                        break;
                    case 7:
                        vehicleInfo = PrefabCollection<VehicleInfo>.FindLoaded("Sports-car");
                        color = Color.yellow;
                        break;
                    default:
                        break;
                }
                count = (count + 1) % 8;

                if (vehicleInfo == null)
                    Logger.LogInfo("Damn it!");
                else
                {
                    CreateVehicle(vehicleInfo.m_mesh, vehicleInfo.m_material, color);
                }
            }
#endif
        }

#if DEBUG
        int count = 0;
        GameObject vehicle;
        GameObject quad;
        void OnGUI()
        {
            if (Singleton<LoadingManager>.instance.m_loadingComplete)
            {
                if (GUI.Button(new Rect(10, 900, 150, 30), "Update Textures"))
                {
                    m_customTextures.Clear();
                    foreach (var item in m_customPrefabs.Values)
                    {
                        NetInfo netInfo = item as NetInfo;
                        if (netInfo.m_segments.Length == 0)
                            continue;
                    }

                    FileManager.ClearCache();
                }

                //if (GUI.Button(new Rect(10, 850, 150, 30), "Road Customizer"))
                //{
                //    //ToolsModifierControl.SetTool<RoadCustomizerTool>();
                //    //RoadCustomizerTool.InitializeUI();
                //}
                //if (GUI.Button(new Rect(10, 800, 150, 30), "Add Button"))
                //{
                //    RoadCustomizerTool.SetUIButton();
                //}
            }
        }

        void CreateVehicle(Mesh mesh, Material material, Color color)
        {
            if (vehicle != null)
                Destroy(vehicle);

            vehicle = new GameObject("Vehicle");
            vehicle.transform.position = new Vector3(0f, 131f, -10f);
            vehicle.transform.rotation = Quaternion.Euler(0f, 210f, 0f);
            MeshFilter mf = vehicle.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            MeshRenderer mr = vehicle.AddComponent<MeshRenderer>();
            material.color = color;
            mr.sharedMaterial = material;

            if (quad == null)
            {
                quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.position = new Vector3(0f, 130f, -10f);
                quad.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                quad.transform.localScale = new Vector3(100, 100);
                quad.GetComponent<Renderer>().sharedMaterial.color = new Color(255f, 203f, 219f);
            }

            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            cameraController.m_targetPosition = new Vector3(0f, 139.775f, 0f);
            cameraController.m_targetSize = 40;
            cameraController.m_targetAngle = new Vector2(0f, 0f);
        }
#endif

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
                    if ((TrafficPPModule.Options & OptionsManager.ModOptions.GhostMode) != OptionsManager.ModOptions.GhostMode && this.m_level == 6)
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

                        if ((TrafficPPModule.Options & OptionsManager.ModOptions.BetaTestRoadCustomizerTool) == OptionsManager.ModOptions.BetaTestRoadCustomizerTool)
                            AddTool<RoadCustomizerTool>(toolController);
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

        //IEnumerator Print()
        //{
        //    yield return new WaitForSeconds(30f);

        //    foreach (var item in Resources.FindObjectsOfTypeAll<GameObject>().Except(GameObject.FindObjectsOfType<GameObject>()))
        //    {
        //        if (item.transform.parent == null)
        //            printGameObjects(item);
        //    }
        //}

        //void printGameObjects(GameObject go, int depth = 0)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < depth; i++)
        //    {
        //        sb.Append(">");
        //    }
        //    sb.Append("> ");
        //    sb.Append(go.name);
        //    sb.Append("\n");

        //    System.IO.File.AppendAllText("MapScenePrefabs.txt", sb.ToString());

        //    printComponents(go, depth);

        //    foreach (Transform t in go.transform)
        //    {
        //        printGameObjects(t.gameObject, depth + 1);
        //    }
        //}

        //void printComponents(GameObject go, int depth)
        //{
        //    foreach (var item in go.GetComponents<Component>())
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        for (int i = 0; i < depth; i++)
        //        {
        //            sb.Append(" ");
        //        }
        //        sb.Append("  -- ");
        //        sb.Append(item.GetType().Name);
        //        sb.Append("\n");

        //        System.IO.File.AppendAllText("MapScenePrefabs.txt", sb.ToString());
        //    }
        //}


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

        T ClonePrefab<T>(string prefabName, string newName, Transform customPrefabsHolder, bool replace = false, bool ghostMode = false) where T : PrefabInfo
        {
            T[] prefabs = Resources.FindObjectsOfTypeAll<T>();
            return ClonePrefab<T>(prefabName, prefabs, newName, customPrefabsHolder, replace, ghostMode);
        }

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
                this.m_customPrefabs.Add(newName, originalPrefab);
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

#if DEBUG
        public static void DumpRenderTexture(RenderTexture rt, string pngOutPath)
        {
            var oldRT = RenderTexture.active;

            var tex = new Texture2D(rt.width, rt.height);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            File.WriteAllBytes(pngOutPath, tex.EncodeToPNG());
            RenderTexture.active = oldRT;
        }

        public static void DumpTextureToPNG(Texture previewTexture, string filename = null)
        {
            if (filename == null)
            {
                filename = "";
                var filenamePrefix = String.Format("rt_dump_{0}", previewTexture.name);
                if (!File.Exists(filenamePrefix + ".png"))
                {
                    filename = filenamePrefix + ".png";
                }
                else
                {
                    int i = 1;
                    while (File.Exists(String.Format("{0}_{1}.png", filenamePrefix, i)))
                    {
                        i++;
                    }

                    filename = String.Format("{0}_{1}.png", filenamePrefix, i);
                }
            }

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            if (previewTexture is RenderTexture)
            {
                DumpRenderTexture((RenderTexture)previewTexture, filename);
                //Log.Warning(String.Format("Texture dumped to \"{0}\"", filename));
            }
            else if (previewTexture is Texture2D)
            {
                var texture = previewTexture as Texture2D;
                byte[] bytes = null;

                try
                {
                    bytes = texture.EncodeToPNG();
                }
                catch (UnityException)
                {
                    //Log.Warning(String.Format("Texture \"{0}\" is marked as read-only, running workaround..", texture.name));
                }

                if (bytes == null)
                {
                    try
                    {
                        var rt = RenderTexture.GetTemporary(texture.width, texture.height, 0);
                        Graphics.Blit(texture, rt);
                        DumpRenderTexture(rt, filename);
                        RenderTexture.ReleaseTemporary(rt);
                        //Log.Warning(String.Format("Texture dumped to \"{0}\"", filename));
                    }
                    catch (Exception ex)
                    {
                        //Log.Error("There was an error while dumping the texture - " + ex.Message);
                    }

                    return;
                }

                File.WriteAllBytes(filename, bytes);
                //Log.Warning(String.Format("Texture dumped to \"{0}\"", filename));
            }
            else
            {
                //Log.Error(String.Format("Don't know how to dump type \"{0}\"", previewTexture.GetType()));
            }
        }
#endif
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

#if DEBUG
        #region SceneInspectionTools

        IEnumerator Print()
        {
            while (!LoadingManager.instance.m_loadingComplete)
                yield return new WaitForEndOfFrame();

            List<GameObject> sceneObjects = GameObject.FindObjectsOfType<GameObject>().ToList();
            foreach (var item in sceneObjects)
            {
                if (item.transform.parent == null)
                    PrintGameObjects(item, "MapScene_110b.txt");
            }

            List<GameObject> prefabs = Resources.FindObjectsOfTypeAll<GameObject>().Except(sceneObjects).ToList();
            foreach (var item in prefabs)
            {
                if (item.transform.parent == null)
                    PrintGameObjects(item, "MapScenePrefabs_110b.txt");
            }
        }

        public static void PrintGameObjects(GameObject go, string fileName, int depth = 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                sb.Append("\t");
            }

            sb.Append(go.name);
            sb.Append(" {\n");

            System.IO.File.AppendAllText(fileName, sb.ToString());

            PrintComponents(go, fileName, depth);

            foreach (Transform t in go.transform)
            {
                PrintGameObjects(t.gameObject, fileName, depth + 1);
            }

            sb = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                sb.Append("\t");
            }
            sb.Append("}\n\n");
            System.IO.File.AppendAllText(fileName, sb.ToString());
        }

        public static void PrintComponents(GameObject go, string fileName, int depth)
        {
            foreach (var item in go.GetComponents<Component>())
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < depth; i++)
                {
                    sb.Append("\t");
                }
                sb.Append("\t-- ");
                sb.Append(item.GetType().Name);
                sb.Append("\n");

                System.IO.File.AppendAllText(fileName, sb.ToString());
            }
        }

        #endregion
#endif

    }
}
