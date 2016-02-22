using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Packaging;
using ObjUnity3D;
using Transit.Framework.Texturing;
using UnityEngine;

namespace Transit.Framework
{
    public enum TextureType
    {
        Default,
        LOD,
        UI
    }

    public class AssetManager : Singleton<AssetManager>
    {
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
                var assetFullPath = assetFile
                    .FullName;

                var assetName = assetFile
                    .FullName
                    .ToLowerInvariant()
                    .Replace(modPath.ToLowerInvariant(), "")
                    .Replace("\\", "/")
                    .TrimStart("/")
                    .TrimEnd(assetFile.Extension);

                switch (assetFile.Extension.ToLowerInvariant())
                {
                    case ".png":
                        yield return () =>
                        {
                            _allTexturesRaw[assetName] = LoadTextureData(assetFullPath);
                        };
                        break;

                    case ".obj":
                        yield return () =>
                        {
                            _allMeshes[assetName] = LoadMesh(assetFullPath, assetName);
                        };
                        break;
                }
            }
        }

        public IEnumerable<Action> CreatePackageLoadingSequence(string packageName, string assetprefix, bool isOptional = true)
        {
            var package = PackageManager.GetPackage(packageName);
            if (package == null)
            {
                if (isOptional)
                {
                    yield break;
                }
                else
                {
                    throw new Exception(String.Format("TFW: Package {0} not found", packageName));
                }
            }

            foreach (var filteredAsset in package.FilterAssets(Package.AssetType.StaticMesh, Package.AssetType.Texture))
            {
                var asset = filteredAsset;
                var assetName = asset
                    .name
                    .TrimStart(assetprefix);

                if (asset.type == Package.AssetType.Texture)
                {
                    yield return () =>
                    {
                        _allTextures[assetName] = asset.Instantiate<Texture2D>();
                    };

                    continue;
                }

                if (asset.type == Package.AssetType.StaticMesh)
                {
                    yield return () =>
                    {
                        _allMeshes[assetName] = asset.Instantiate<Mesh>();
                    };

                    continue;
                }
            }
        }

        public IEnumerable<AssetInfo> LoadAllAssets(string modPath)
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

                switch (assetFile.Extension.ToLower())
                {
                    case ".png":
                        yield return new AssetInfo(assetRelativePath, AssetType.Texture, LoadTextureData(assetFullPath));
                        break;

                    case ".obj":
                        yield return new AssetInfo(assetRelativePath, AssetType.Mesh, LoadMesh(assetFullPath, assetName));
                        break;
                }
            }
        }

        private static byte[] LoadTextureData(string fullPath)
        {
            return File.ReadAllBytes(fullPath);
        }

        private static Mesh LoadMesh(string fullPath, string meshName)
        {
            var mesh = new Mesh();
            using (var fileStream = File.Open(fullPath, FileMode.Open))
            {
                mesh.LoadOBJ(OBJLoader.LoadOBJ(fileStream));
            }
            mesh.Optimize();
            mesh.name = meshName;

            return mesh;
        }

        public Texture2D GetTexture(string path, TextureType type)
        {
            if (path.IsNullOrWhiteSpace())
            {
                return null;
            }

            var trimmedPath = path
                .Replace("\\", "/")
                .ToLowerInvariant()
                .TrimEnd(".png");

            if (!_allTexturesRaw.ContainsKey(trimmedPath))
            {
                throw new Exception(String.Format("TFW: Texture {0} not found", trimmedPath));
            }

            if (type == TextureType.Default)
            {
                if (!_allTextures.ContainsKey(trimmedPath))
                {
                    _allTextures[trimmedPath] = _allTexturesRaw[trimmedPath].AsTexture(Path.GetFileNameWithoutExtension(trimmedPath), type);
                }

                return _allTextures[trimmedPath];
            }

            return _allTexturesRaw[trimmedPath].AsTexture(Path.GetFileNameWithoutExtension(trimmedPath), type);
        }

        public Texture2D GetEditableTexture(string path, string textureName = null)
        {
            if (path.IsNullOrWhiteSpace())
            {
                return null;
            }

            var trimmedPath = path
                .Replace("\\", "/")
                .ToLowerInvariant()
                .TrimEnd(".png");

            if (!_allTexturesRaw.ContainsKey(trimmedPath))
            {
                throw new Exception(String.Format("TFW: Texture {0} not found", trimmedPath));
            }

            return _allTexturesRaw[trimmedPath].AsEditableTexture(textureName);
        }

        public Mesh GetMesh(string path)
        {
            if (path.IsNullOrWhiteSpace())
            {
                return null;
            }

            var assetName = path
                .Replace("\\", "/")
                .ToLowerInvariant()
                .TrimEnd(".obj");

            if (!_allMeshes.ContainsKey(assetName))
            {
                throw new Exception(String.Format("TFW: Mesh {0} not found", assetName));
            }

            return _allMeshes[assetName];
        }
    }
}
