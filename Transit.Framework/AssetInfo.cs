using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Transit.Framework
{
    public enum AssetType
    {
        Texture,
        Mesh
    }

    public class AssetInfo
    {
        public string RelativePath { get; private set; }
        public AssetType Type { get; private set; }
        public object Asset { get; private set; }

        public AssetInfo(string relativePath, AssetType type, object asset)
        {
            RelativePath = relativePath;
            Type = type;
            Asset = asset;
        }
    }
}
