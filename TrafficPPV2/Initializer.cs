using ColossalFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CSL_Traffic
{
    public class Initializer : MonoBehaviour
    {
        bool m_initialized;
        float m_gameStartedTime;
        int m_level;

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        void OnLevelWasLoaded(int level)
        {
            this.m_level = level;

            if (level == 6)
            {
                Logger.LogInfo("Game level was loaded. Options enabled: \n\t" + Mod.Options);

                m_initialized = false;
            }
        }

        public void OnLevelUnloading()
        {
            if ((Mod.Options & ModOptions.UseRealisticSpeeds) == ModOptions.UseRealisticSpeeds)
            {
                for (uint i = 0; i < PrefabCollection<CitizenInfo>.LoadedCount(); i++)
                {
                    CitizenInfo cit = PrefabCollection<CitizenInfo>.GetLoaded(i);
                    cit.m_walkSpeed /= 0.25f;
                }
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
            Logger.LogInfo("Queueing prefabs for loading...");

            Singleton<LoadingManager>.instance.QueueLoadingAction(ActionWrapper(() =>
            {
                try
                {
                    if (this.m_level == 6)
                    {
                        if ((Mod.Options & ModOptions.RoadCustomizerTool) == ModOptions.RoadCustomizerTool)
                            AddTool<RoadCustomizerTool>(ToolsModifierControl.toolController);

                        if ((Mod.Options & ModOptions.UseRealisticSpeeds) == ModOptions.UseRealisticSpeeds)
                        {
                            for (uint i = 0; i < PrefabCollection<CitizenInfo>.LoadedCount(); i++)
                            {
                                CitizenInfo cit = PrefabCollection<CitizenInfo>.GetLoaded(i);
                                cit.m_walkSpeed *= 0.25f;
                            }
                        }
                    }

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

        static IEnumerator ActionWrapper(Action a)
        {
            a.Invoke();
            yield break;
        }

        #endregion

        #region Vehicles

        //// TODO: set correct values on vehicles for realistic speeds
        //void SetRealisitcSpeeds(VehicleInfo vehicle, bool activate)
        //{
        //    float accelerationMultiplier;
        //    float maxSpeedMultiplier;
        //    switch (vehicle.name)
        //    {
        //        case "Ambulance":
        //            accelerationMultiplier = 0.2f;
        //            //vehicle.m_braking *= 0.3f;
        //            //vehicle.m_turning *= 0.25f;
        //            maxSpeedMultiplier = 0.5f;
        //            break;
        //        case "Bus":
        //        case "Fire Truck":
        //        case "Garbage Truck":
        //            accelerationMultiplier = 0.15f;
        //            //vehicle.m_braking *= 0.25f;
        //            //vehicle.m_turning *= 0.2f;
        //            maxSpeedMultiplier = 0.5f;
        //            break;
        //        case "Hearse":
        //        case "Police Car":
        //            accelerationMultiplier = 0.25f;
        //            //vehicle.m_braking *= 0.35f;
        //            //vehicle.m_turning *= 0.3f;
        //            maxSpeedMultiplier = 0.5f;
        //            break;
        //        default:
        //            accelerationMultiplier = 0.25f;
        //            //vehicle.m_braking *= 0.35f;
        //            //vehicle.m_turning *= 0.3f;
        //            maxSpeedMultiplier = 0.5f;
        //            break;
        //    }

        //    if (!activate)
        //    {
        //        accelerationMultiplier = 1f / accelerationMultiplier;
        //        maxSpeedMultiplier = 1f / maxSpeedMultiplier;
        //    }

        //    vehicle.m_acceleration *= accelerationMultiplier;
        //    vehicle.m_maxSpeed *= maxSpeedMultiplier;
        //}

        void CopyVehicleAIAttributes<T>(VehicleAI from, T to)
        {
            foreach (FieldInfo fi in typeof(T).BaseType.GetFields())
            {
                fi.SetValue(to, fi.GetValue(from));
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
    }
}
