using System;
using Transit.Framework.Modularity;

namespace Transit.Addon.ToolsV3
{
    [Module("Transit.Addon.Mod")]
    public partial class ToolModuleV3 : ModuleBase
    {
        public override string Name
        {
            get { return "Tools V3"; }
        }
    }
}
