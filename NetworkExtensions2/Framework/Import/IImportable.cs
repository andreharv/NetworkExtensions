using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkExtensions2.Framework.Import
{
    internal interface IImportable
    {
        void ImportAsset(Shader shader, string path, string filename);
        void Update();
    }
}
