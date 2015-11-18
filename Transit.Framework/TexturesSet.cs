using ColossalFramework;
using UnityEngine;

namespace Transit.Framework
{
    public class TexturesSet
    {
        private Texture2D _mainTex;
        public Texture2D MainTex
        {
            get
            {
                if (_mainTex == null)
                {
                    if (!_mainTexPath.IsNullOrWhiteSpace())
                    {
                        _mainTex = AssetManager.instance.GetTexture(_mainTexPath, _isLODSet? TextureType.LOD : TextureType.Default);
                    }
                }

                return _mainTex;
            }
        }

        private Texture2D _aprMap;
        public Texture2D APRMap
        {
            get
            {
                if (_aprMap == null)
                {
                    if (!_aprMapPath.IsNullOrWhiteSpace())
                    {
                        _aprMap = AssetManager.instance.GetTexture(_aprMapPath, _isLODSet ? TextureType.LOD : TextureType.Default);
                    }
                }

                return _aprMap;
            }
        }

        private Texture2D _xysMap;
        public Texture2D XYSMap
        {
            get
            {
                if (_xysMap == null)
                {
                    if (!_xysMapPath.IsNullOrWhiteSpace())
                    {
                        _xysMap = AssetManager.instance.GetTexture(_xysMapPath, _isLODSet ? TextureType.LOD : TextureType.Default);
                    }
                }

                return _xysMap;
            }
        }

        private readonly string _mainTexPath;
        private readonly string _aprMapPath;
        private readonly string _xysMapPath;
        private readonly bool _isLODSet;

        public TexturesSet(string mainTexPath = null, string aprMapPath = null, string xysMapPath = null)
        {
            _mainTexPath = mainTexPath;
            _aprMapPath = aprMapPath;
            _xysMapPath = xysMapPath;
        }

        protected TexturesSet(string mainTexPath = null, string aprMapPath = null, string xysMapPath = null, bool isLODSet = false)
        {
            _mainTexPath = mainTexPath;
            _aprMapPath = aprMapPath;
            _xysMapPath = xysMapPath;

            _isLODSet = isLODSet;
        }
    }

    public class LODTexturesSet : TexturesSet
    {
        public LODTexturesSet(string mainTexPath = null, string aprMapPath = null, string xysMapPath = null)
            : base(mainTexPath, aprMapPath, xysMapPath, true)
        {
        }
    }
}
