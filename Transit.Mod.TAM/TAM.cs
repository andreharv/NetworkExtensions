﻿using System;
using Transit.Framework.Mod;
using Transit.Framework.Prerequisites;

namespace Transit.Mod
{
    public sealed partial class TAM : TransitModBase
    {
        public override ulong WorkshopId
        {
            get { return 626024868ul; }
        }

        public override string Name
        {
            //get { return "Traffic++ V2"; }
            get { return "Transit Addons Mod"; }
        }

        public override string Description
        {
            get { return "Adds transit routing and restriction features.\n[GAME REBOOT REQUIRED]"; }
        }

        public override string Version
        {
            //get { return "2.0.0"; }
            get { return "0.0.1"; }
        }

        public override TransitModType Type
        {
            get { return TransitModType.Master; }
        }

        public override PrerequisiteType Requirements
        {
            get { return PrerequisiteType.PathFinding | PrerequisiteType.UI; }
        }
    }
}