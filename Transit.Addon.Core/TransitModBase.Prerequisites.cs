using System;
using Transit.Framework.Unsafe;
using UnityEngine;

namespace Transit.Addon.Core
{
    public partial class TransitModBase
    {
        private static bool s_isPrerequisitesInstalled = false;

        private static void InstallPrerequisites()
        {
            if (s_isPrerequisitesInstalled)
            {
                return;
            }

            try
            {
                Redirector.PerformRedirections();
                s_isPrerequisitesInstalled = true;
            }
            catch (Exception ex)
            {
                Debug.Log("TAM: Crashed-PrerequisitesInstallation");
                Debug.Log("TAM: " + ex.Message);
                Debug.Log("TAM: " + ex.ToString());
            }
        }

        private static void UninstallPrerequisites()
        {
            if (!s_isPrerequisitesInstalled)
            {
                return;
            }

            try
            {
                Redirector.RevertRedirections();
            }
            catch (Exception ex)
            {
                Debug.Log("TAM: Crashed-PrerequisitesUninstallation");
                Debug.Log("TAM: " + ex.Message);
                Debug.Log("TAM: " + ex.ToString());
            }

            s_isPrerequisitesInstalled = false;
        }
    }
}
