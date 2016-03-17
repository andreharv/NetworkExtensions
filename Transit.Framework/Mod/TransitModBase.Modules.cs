using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Framework.Modularity;
using UnityEngine;

#if DEBUG
using Debug = Transit.Framework.Debug;
#endif

namespace Transit.Framework.Mod
{
    public partial class TransitModBase
    {
        public IEnumerable<IModule> Modules
        {
            get { return ModuleManager.instance.GetOrCreateModules(this); }
        }
    }
}
