using System;
using ColossalFramework;
using ColossalFramework.Plugins;
using Transit.Framework;
using Transit.Framework.Mod;
using Transit.Framework.Modularity;
using System.Collections.Generic;
using System.Linq;

namespace NetworkExtensions
{
    public sealed partial class Mod : TransitModBase
    {
        public override string Name
        {
            get { return "Network Extensions 2"; }
        }

        public override string Description
        {
            get { return "A continuation of the successful Network Extensions Project"; }
        }

        public override string Version
        {
            get { return "1.0.0"; }
        }

        private const string NEXT_2_ID = "812125426";
        private bool? _isNEXT2Installed;

        public bool IsNEXT2Installed
        {
            get
            {
                if (_isNEXT2Installed == null)
                {
                    _isNEXT2Installed = Singleton<PluginManager>.instance.IsPluginInstalled(NEXT_2_ID);
                }

                return _isNEXT2Installed.Value;
            }
        }
    }
}
