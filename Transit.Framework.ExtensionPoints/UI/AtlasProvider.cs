using ColossalFramework.UI;
using System;
using System.Collections.Generic;

namespace Transit.Framework.ExtensionPoints.UI
{
    public class AtlasProvider
    {
        private static readonly IDictionary<string, UITextureAtlas> _customAtlases = new Dictionary<string, UITextureAtlas>(StringComparer.InvariantCultureIgnoreCase);

        public static void RegisterCustomAtlas(string atlasKey, UITextureAtlas atlas)
        {
            _customAtlases[atlasKey] = atlas;
        }

        public static bool HasCustomAtlas(string atlasKey)
        {
            return _customAtlases.ContainsKey(atlasKey);
        }

        public static UITextureAtlas GetCustomAtlas(string atlasKey)
        {
            return _customAtlases[atlasKey];
        }
    }
}
