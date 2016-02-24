using ColossalFramework;
using UnityEngine;

namespace Transit.Framework.Texturing
{
    public class TextureSet
    {
        public Texture2D MainTex { get { return _mainTexProvider != null ? _mainTexProvider.GetTexture() : null; } }
        public Texture2D APRMap  { get { return _aprMapProvider  != null ? _aprMapProvider.GetTexture()  : null; } }
        public Texture2D XYSMap  { get { return _xysMapProvider  != null ? _xysMapProvider.GetTexture()  : null; } }

        private readonly ITextureProvider _mainTexProvider;
        private readonly ITextureProvider _aprMapProvider;
        private readonly ITextureProvider _xysMapProvider;

        public TextureSet(
            ITextureProvider mainTexProvider = null,
            ITextureProvider aprMapProvider = null,
            ITextureProvider xysMapProvider = null)
        {
            _mainTexProvider = mainTexProvider;
            _aprMapProvider = aprMapProvider;
            _xysMapProvider = xysMapProvider;
        }

        public TextureSet(string mainTexPath = null, string aprMapPath = null, string xysMapPath = null) :
            this(mainTexPath, aprMapPath, xysMapPath, false)
        {
        }

        protected TextureSet(string mainTexPath = null, string aprMapPath = null, string xysMapPath = null, bool isLODSet = false)
        {
            if (!mainTexPath.IsNullOrWhiteSpace())
            {
                _mainTexProvider = new PathTextureProvider(mainTexPath, isLODSet ? TextureType.LOD : TextureType.Default);
            }
            if (!aprMapPath.IsNullOrWhiteSpace())
            {
                _aprMapProvider = new PathTextureProvider(aprMapPath, isLODSet ? TextureType.LOD : TextureType.Default);
            }
            if (!xysMapPath.IsNullOrWhiteSpace())
            {
                _xysMapProvider = new PathTextureProvider(xysMapPath, isLODSet ? TextureType.LOD : TextureType.Default);
            }
        }
    }

    public class LODTextureSet : TextureSet
    {
        public LODTextureSet(string mainTexPath = null, string aprMapPath = null, string xysMapPath = null)
            : base(mainTexPath, aprMapPath, xysMapPath, true)
        {
        }
    }
}
