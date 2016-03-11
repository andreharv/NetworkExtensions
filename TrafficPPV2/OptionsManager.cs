using ColossalFramework;
using ICities;
using System;
using System.IO;
using System.Xml.Serialization;

namespace CSL_Traffic
{
    public class OptionsManager
    {
        public struct Options
        {
            public bool realisticSpeeds;
            public bool noDespawn;
            public bool betaTestRoadCustomizer;
        }
    }
}
