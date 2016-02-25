using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using Transit.Framework.Modularity;
using Transit.Framework.Redirection;

namespace Transit.Addon.PathFinding
{
    public partial class PathfindingModule
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
            {
                Redirector.PerformRedirections((ulong)PathfindingOptions);
            }
        }

        public override void OnReleased()
        {
            Redirector.RevertRedirections();
        }
    }
}
