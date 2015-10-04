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

        private readonly IDictionary<string, byte[]> _allTextures = new Dictionary<string, byte[]>();
        private readonly IDictionary<string, Mesh> _allMeshes = new Dictionary<string, Mesh>();

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

                if (_allTextures.ContainsKey(assetRelativePath))
                {
                    continue;
                }

                switch (assetFile.Extension.ToLower())
                {
                    case ".png":
                        yield return () =>
                        {
                            _allTextures[assetRelativePath] = LoadTexturePNG(assetFullPath);
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

        private static Texture2D LoadTexturePNG(string fullPath, string textureName)
        {
            var texture = new Texture2D(1, 1);
            texture.name = Path.GetFileNameWithoutExtension(textureName);
            texture.LoadImage(File.ReadAllBytes(fullPath));
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

        public Texture2D GetTexture(string path)
        {
            if (path.IsNullOrWhiteSpace())
            {
                return null;
            }

            var trimmedPath = path
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);

            if (!_allTextures.ContainsKey(trimmedPath))
            {
                throw new Exception(String.Format("TFW: Texture {0} not found", trimmedPath));
            }

            return CreateTexture(_allTextures[trimmedPath], Path.GetFileNameWithoutExtension(trimmedPath));
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
