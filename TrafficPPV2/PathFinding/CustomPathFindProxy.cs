using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transit.Framework.Unsafe;

namespace CSL_Traffic
{
    public class CustomPathFindProxy : PathFind
    {
        [RedirectFrom(typeof(PathFind))]
        private void Awake()
        {
            // Disabling the Awake method
        }

        [RedirectFrom(typeof(PathFind))]
        private void OnDestroy()
        {
            // Disabling the Awake method
        }
    }
}
