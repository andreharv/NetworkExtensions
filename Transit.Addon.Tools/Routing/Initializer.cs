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
using Transit.Addon.Tools.Tools;
using Transit.Framework;
using UnityEngine;

namespace Transit.Addon.Tools
{
    public class Initializer : MonoBehaviour
    {
        static Queue<IEnumerator> sm_actionQueue = new Queue<IEnumerator>();
        static System.Object sm_queueLock = new System.Object();
        static bool sm_localizationInitialized;
        static readonly string[] sm_collectionPrefixes = { "", "Europe " };

        Dictionary<string, PrefabInfo> m_customPrefabs;
        Dictionary<string, VehicleAI> m_replacedAIs;
        bool m_initialized;
        bool m_incompatibilityWarning;
        float m_gameStartedTime;
        int m_level;

        void Awake()
        {
            DontDestroyOnLoad(this);

            m_customPrefabs = new Dictionary<string, PrefabInfo>();
            m_replacedAIs = new Dictionary<string, VehicleAI>();
        }

        void Start()
        {
            ReplaceTransportManager();
#if DEBUG
            //StartCoroutine(Print());
#endif
        }

        void OnLevelWasLoaded(int level)
        {
            this.m_level = level;

            if (level == 6)
            {
                Logger.LogInfo("Game level was loaded. Options enabled: \n\t" + ToolModule.ActiveOptions);

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
            }
        }

        void Update()
        {
            if (!m_initialized)
            {
                TryReplacePrefabs();
                return;
            }

            if (!Singleton<LoadingManager>.instance.m_loadingComplete)
                return;
            else if (m_gameStartedTime == 0f)
                m_gameStartedTime = Time.realtimeSinceStartup;
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
                this.m_customPrefabs.Add(newName, originalPrefab);
                return null;
            }

            newPrefab.m_prefabInitialized = false;

            return newPrefab;
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
