using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;

namespace Transit.Framework.ExtensionPoints.UI
{
    public class AtlasProvider : Singleton<AtlasProvider>
    {
        private readonly IDictionary<string, UITextureAtlas> _customAtlases = new Dictionary<string, UITextureAtlas>(StringComparer.InvariantCultureIgnoreCase);

        public void RegisterCustomAtlas(string atlasKey, UITextureAtlas atlas)
        {
            _customAtlases[atlasKey] = atlas;
        }

        public bool HasCustomAtlas(string atlasKey)
        {
            return _customAtlases.ContainsKey(atlasKey);
        }

        public UITextureAtlas GetCustomAtlas(string atlasKey)
        {
            return _customAtlases[atlasKey];
        }
    }
}
