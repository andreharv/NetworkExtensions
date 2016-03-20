using System;
using Transit.Framework.Modularity;

namespace Transit.Addon.ToolsV3
{
    [Module("Transit.Mod.TAM")]
    public partial class ToolModuleV3 : ModuleBase
    {
        public override string Name
        {
            get { return "Tools V3"; }
        }
    }
}
