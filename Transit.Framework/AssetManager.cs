using System;
using System.Collections.Generic;
using System.IO;
using ColossalFramework;
using ObjUnity3D;
using UnityEngine;

namespace Transit.Framework
{
    public class AssetManager : Singleton<AssetManager>
    {
#if DEBUG
        private readonly ICollection<Texture2D> _specialTextures = new List<Texture2D>();
        public ICollection<Texture2D> SpecialTextures { get { return _specialTextures; } }
#endif

        private readonly IDictionary<string, byte[]> _allTexturesRaw = new Dictionary<string, byte[]>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, Texture2D> _allTextures = new Dictionary<string, Texture2D>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, Mesh> _allMeshes = new Dictionary<string, Mesh>(StringComparer.InvariantCultureIgnoreCase);

        public IEnumerable<Action> CreateLoadingSequence(string modPath)
        {
            var modDirectory = new DirectoryInfo(modPath);

            var files = new List<FileInfo>();
            files.AddRange(modDirectory.GetFiles("*.png", SearchOption.AllDirectories));
            files.AddRange(modDirectory.GetFiles("*.obj", SearchOption.AllDirectories));

            foreach (var assetFile in files)
            {
                var assetFullPath = assetFile.FullName;
                var assetRelativePath = assetFile.FullName.Replace(modPath, "").TrimStart(new[] { '\\', '/' });
                var assetName = assetFile.Name;

                if (_allTexturesRaw.ContainsKey(assetRelativePath))
                {
                    continue;
                }

                switch (assetFile.Extension.ToLower())
                {
                    case ".png":
                        yield return () =>
                        {
                            _allTexturesRaw[assetRelativePath] = LoadTexturePNG(assetFullPath);
                        };
                        break;

                    case ".obj":
                        yield return () =>
                        {
                            _allMeshes[assetRelativePath] = LoadMesh(assetFullPath, assetName);
                        };
                        break;
                }
            }
        }

        private static byte[] LoadTexturePNG(string fullPath)
        {
            return File.ReadAllBytes(fullPath);
        }

        private static Texture2D CreateTexture(byte[] textureBytes, string textureName)
        {
            var texture = new Texture2D(1, 1);
            texture.name = Path.GetFileNameWithoutExtension(textureName);
            texture.LoadImage(textureBytes);
            texture.anisoLevel = 8;
            texture.filterMode = FilterMode.Trilinear;
            texture.Apply();
            return texture;
        }

        private static Mesh LoadMesh(string fullPath, string meshName)
        {
            var mesh = new Mesh();
            using (var fileStream = File.Open(fullPath, FileMode.Open))
            {
                mesh.LoadOBJ(OBJLoader.LoadOBJ(fileStream));
            }
            mesh.Optimize();
            mesh.name = Path.GetFileNameWithoutExtension(meshName);

            return mesh;
        }

        public Texture2D GetTexture(string path, bool useCache = false)
        {
            if (path.IsNullOrWhiteSpace())
            {
                return null;
            }

            var trimmedPath = path
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);

            if (!_allTexturesRaw.ContainsKey(trimmedPath))
            {
                throw new Exception(String.Format("TFW: Texture {0} not found", trimmedPath));
            }

            if (useCache)
            {
                if (!_allTextures.ContainsKey(trimmedPath))
                {
                    _allTextures[trimmedPath] = CreateTexture(_allTexturesRaw[trimmedPath], Path.GetFileNameWithoutExtension(trimmedPath));
                }

                return _allTextures[trimmedPath];
            }

            return CreateTexture(_allTexturesRaw[trimmedPath], Path.GetFileNameWithoutExtension(trimmedPath));
        }

        public Mesh GetMesh(string path)
        {
            if (path.IsNullOrWhiteSpace())
            {
                return null;
            }

            var trimmedPath = path
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);

            if (!_allMeshes.ContainsKey(trimmedPath))
            {
                throw new Exception(String.Format("TFW: Mesh {0} not found", trimmedPath));
            }

            return _allMeshes[trimmedPath];
        }
    }
}
