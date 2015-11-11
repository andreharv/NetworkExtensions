using System;
using ICities;
using Transit.Framework;
using Debug = Transit.Framework.Debug;
#if DEBUG

#endif

namespace Transit.Addon
{
    public partial class Mod : IUserMod
    {
        private const UInt64 WORKSHOP_ID = 478820060;

        private static string s_path = null;
        public static string GetPath()
        {
            if (s_path == null)
            {
                s_path = Assets.GetPath("Transit Addon Mod", WORKSHOP_ID);

                if (s_path != Assets.PATH_NOT_FOUND)
                {
                    Debug.Log("REx: Mod path " + s_path);
                }
                else
                {
                    Debug.Log("REx: Path not found");
                }
            }

            return s_path;
        }
    }
}
