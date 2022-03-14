using ColossalFramework.Importers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkExtensions2.Framework.Import
{
    public class ModelImporter
    {
        private SceneImporter m_SceneImporter;
        public void Import()
        {
            m_SceneImporter = new SceneImporter();

        }
    }
}
