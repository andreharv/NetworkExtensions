using System;
using System.Reflection;
using Transit.Framework;
using Transit.Framework.Unsafe;
using Transit.Addon.RoadExtensions.Menus;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Addon.RoadExtensions.Install
{
    public class Initializer : Installer
    {
#if DEBUG
        private int _frameNb = 0;
#endif

        protected override bool ValidatePrerequisites()
        {
#if DEBUG
            if (_frameNb++ < 20) // Giving some time for the UI to refresh **NB. Putting this constant higher than 100 causes wierd behavior**
            {
                return false;
            }
#endif

            return true;
        }

        protected override void Install()
        {
            var version = GetType().Assembly.GetName().Version;
            Debug.Log(string.Format("REx: Version {0}", version));

            InstallRedirections();
        }

        private static bool s_redirectionsInstalled;

        private void InstallRedirections()
        {
            if (s_redirectionsInstalled)
            {
                return;
            }

            try
            {
                InstallRoadsGroupPanelRedirect();
            }
            catch (Exception ex)
            {
                Debug.Log("REx: Crashed-RedirectionsInstall");
                Debug.Log("REx: " + ex.Message);
                Debug.Log("REx: " + ex.ToString());
            }
            finally
            {
                s_redirectionsInstalled = true;
            }
        }

        private static RedirectCallsState s_rmoRedirect;

        private void InstallRoadsGroupPanelRedirect()
        {
            var originalMethod = typeof(RoadsGroupPanel).GetMethod("GetCategoryOrder", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (originalMethod == null)
            {
                Debug.Log("REx: Cannot find the GetCategoryOrder original method, continuing");
                return;
            }

            var newMethod = typeof(RExRoadsGroupPanel).GetMethod("GetCategoryOrder", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (newMethod == null)
            {
                Debug.Log("REx: Cannot find the GetCategoryOrder new method, continuing");
                return;
            }

            s_rmoRedirect = RedirectionHelper.RedirectCalls(originalMethod, newMethod);
        }
    }
}
