using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using Transit.Framework.Builders;

namespace Transit.Framework
{
    public class AtlasManager : Singleton<AtlasManager>
    {
        private readonly IDictionary<string, UITextureAtlas> _atlases = new Dictionary<string, UITextureAtlas>(StringComparer.InvariantCultureIgnoreCase);

        public void Include<T>()
            where T: IAtlasBuilder, new()
        {
            var builder = new T();
            var atlas = builder.Build();

            foreach (var atlasKey in builder.Keys)
            {
                RegisterAtlas(atlasKey, atlas);
            }
        }

        public void RegisterAtlas(string atlasKey, UITextureAtlas atlas)
        {
            _atlases[atlasKey] = atlas;
        }

        public UITextureAtlas GetAtlas(string atlasKey)
        {
            if (_atlases.ContainsKey(atlasKey))
            {
                return null;
            }

            return _atlases[atlasKey];
        }
    }
}
