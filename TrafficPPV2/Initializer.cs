using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;
using Transit.Framework.Light;
using UnityEngine;

namespace CSL_Traffic
{
    public class Initializer : MonoBehaviour
    {

        static Queue<IEnumerator> sm_actionQueue = new Queue<IEnumerator>();
        static System.Object sm_queueLock = new System.Object();
        static bool sm_localizationInitialized;
        static readonly string[] sm_collectionPrefixes = new string[] { "", "Europe ", "Winter " };

        Dictionary<string, Texture2D> m_customTextures;
        Dictionary<string, VehicleAI> m_replacedAIs;
        bool m_initialized;
        float m_gameStartedTime;
        int m_level;

        void Awake()
        {
            DontDestroyOnLoad(this);

            m_customTextures = new Dictionary<string, Texture2D>();
            m_replacedAIs = new Dictionary<string, VehicleAI>();
            //m_postLoadingActions = new Queue<Action>();
        }

        void OnLevelWasLoaded(int level)
        {
            this.m_level = level;

            if (level == 6)
            {
                Logger.LogInfo("Game level was loaded. Options enabled: \n\t" + TrafficMod.Options);

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
            if ((TrafficMod.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
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
                if (TryReplacePrefabs())
                {
                    m_initialized = true;
                }
                else
                {
                    return;
                }
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
        private bool TryReplacePrefabs()
        {
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

            try
            {
                // TODO: Replace that by redirections
                // VehicleCollections
                garbageVehicleCollection = TryGetComponent<VehicleCollection>("Garbage");
                if (garbageVehicleCollection == null)
                    return false;
                
                policeVehicleCollection = TryGetComponent<VehicleCollection>("Police Department");
                if (policeVehicleCollection == null)
                    return false;

                publicTansportVehicleCollection = TryGetComponent<VehicleCollection>("Public Transport");
                if (publicTansportVehicleCollection == null)
                    return false;

                healthCareVehicleCollection = TryGetComponent<VehicleCollection>("Health Care");
                if (healthCareVehicleCollection == null)
                    return false;

                fireDepartmentVehicleCollection = TryGetComponent<VehicleCollection>("Fire Department");
                if (fireDepartmentVehicleCollection == null)
                    return false;

                industrialVehicleCollection = TryGetComponent<VehicleCollection>("Industrial");
                if (industrialVehicleCollection == null)
                    return false;

                industrialFarmingVehicleCollection = TryGetComponent<VehicleCollection>("Industrial Farming");
                if (industrialFarmingVehicleCollection == null)
                    return false;

                industrialForestryVehicleCollection = TryGetComponent<VehicleCollection>("Industrial Forestry");
                if (industrialForestryVehicleCollection == null)
                    return false;

                industrialOilVehicleCollection = TryGetComponent<VehicleCollection>("Industrial Oil");
                if (industrialOilVehicleCollection == null)
                    return false;

                industrialOreVehicleCollection = TryGetComponent<VehicleCollection>("Industrial Ore");
                if (industrialOreVehicleCollection == null)
                    return false;

                residentialVehicleCollection = TryGetComponent<VehicleCollection>("Residential Low");
                if (residentialVehicleCollection == null)
                    return false;
            }
            catch (Exception e)
            {
                Logger.LogInfo("Unexpected " + e.GetType().Name + " getting required components: " + e.Message + "\n" + e.StackTrace + "\n");
                return false;
            }

            Logger.LogInfo("Queueing prefabs for loading...");

            Singleton<LoadingManager>.instance.QueueLoadingAction(ActionWrapper(() =>
            {
                try
                {
                    if (this.m_level == 6)
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

                        if ((TrafficMod.Options & OptionsManager.ModOptions.BetaTestRoadCustomizerTool) == OptionsManager.ModOptions.BetaTestRoadCustomizerTool)
                            AddTool<RoadCustomizerTool>(ToolsModifierControl.toolController);

                        if ((TrafficMod.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
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

            Logger.LogInfo("Prefabs queued for loading.");

            return true;
        }

        // Replace the pathfinding system for mine
        void ReplacePathManager()
        {
            if (Singleton<PathManager>.instance as CustomPathManagerProxy != null)
                return;

            Logger.LogInfo("Replacing Path Manager");

            // Change PathManager to CustomPathManager
            FieldInfo sInstance = typeof(Singleton<PathManager>).GetFieldByName("sInstance");
            PathManager originalPathManager = Singleton<PathManager>.instance;
            CustomPathManagerProxy customPathManager = originalPathManager.gameObject.AddComponent<CustomPathManagerProxy>();
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

        T TryGetComponent<T>(string name) where T : MonoBehaviour
        {
            foreach (string prefix in sm_collectionPrefixes)
            {
                T[] objects = GameObject.FindObjectsOfType<T>();
                foreach (T o in objects)
                {
                    if (o.gameObject.name == prefix + name)
                    {
                        return o;
                    }
                }
            }
            Logger.LogError("Failed to find component: {0}", name);
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

            if ((TrafficMod.Options & OptionsManager.ModOptions.UseRealisticSpeeds) == OptionsManager.ModOptions.UseRealisticSpeeds)
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
