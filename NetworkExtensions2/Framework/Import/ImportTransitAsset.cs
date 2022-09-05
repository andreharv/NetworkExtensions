using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;
using ColossalFramework.Threading;
using System.Threading;
using ColossalFramework;
using Transit.Framework;

namespace NetworkExtensions2.Framework.Import
{
    public class ImportTransitAsset
    {
        private static bool m_ImportFinished;
        private static string m_ResourcePath;
        private static string[] m_ModelFilenames;
        private static string[] m_TextureFilenames;
        private static ImportTransitAsset[] m_ImportTransitAssets;
        private static Dictionary<string, Material> m_MaterialDict;
        public static Material FetchMaterial(string textureName)
        {
            if (m_MaterialDict == null)
                m_MaterialDict = new Dictionary<string, Material>();
            if (!m_MaterialDict.ContainsKey(textureName))
            {
                m_MaterialDict[textureName] = new Material(DefaultShader);
                if (m_MaterialDict[textureName].HasProperty("_Color"))
                    m_MaterialDict[textureName].SetColor("_Color", Color.gray);
            }

            return m_MaterialDict[textureName];

        }
        private static Shader m_DefaultShader;

        public static Shader DefaultShader
        {
            get
            {
                if (m_DefaultShader == null)
                    m_DefaultShader = Shader.Find("Custom/Net/Road");
                return m_DefaultShader;
            }
        }

        private static Dictionary<string, ResourceUnit> m_NewInfoResources;
        protected static Dictionary<string, ResourceUnit> NewInfoResources
        {
            get
            {
                if (m_NewInfoResources == null)
                    m_NewInfoResources = new Dictionary<string, ResourceUnit>();
                return m_NewInfoResources;
            }
            set { m_NewInfoResources = value; }
        }

        public delegate void ModelImportCallbackHandler(Mesh mesh, Material material, Mesh lodMesh, Material lodMaterial);
        private PreviewCamera m_PreviewCamera;
        protected PreviewCamera PreviewCamera
        {
            get
            {
                if (m_PreviewCamera == null)
                    m_PreviewCamera = new PreviewCamera();
                return m_PreviewCamera;
            }
        }

        private bool m_ModelReady;
        private static Dictionary<string, NEXTImportAssetModel> m_Resources;
        private static Dictionary<string, NEXTImportAssetTextureLoader> m_Textures;
        public static bool UptakeImportFiles(string path, AssetType type)
        {
            var swAll = new Stopwatch();
            swAll.Start();
            Debug.Log("Checkpoint1");
            if (!m_ImportFinished)
            {
                Debug.Log("Checkpoint6");
                var resourcePath = Path.Combine(path, GetAssetPath(type));

                if (Directory.Exists(resourcePath))
                {
                    Debug.Log("Checkpoint7");
                    if (m_ModelFilenames == null)
                    {
                        m_ModelFilenames = Directory.GetFiles(resourcePath, "*.fbx");
                    }
                    if (m_TextureFilenames == null)
                    {
                        m_TextureFilenames = Directory.GetFiles(resourcePath, "*.png");
                        Debug.Log("textureCount is " + m_TextureFilenames.Length);
                    }
                    if (m_ImportAssetModels == null)
                    {
                        NEXTImportAssetModel.AssetPath = path;
                        DevelopModels();
                    }
                    else
                    {
                        return UpdateModels();
                    }
                }
                else
                {
                    Debug.LogError("The specified path: " + m_ResourcePath + " does not exist");
                }
            }
            return false;
        }

        protected static Dictionary<string, NEXTImportAssetModel> m_ImportAssetModels;
        protected static void DevelopModels()
        {
            //var stockMaterial = Prefabs.Find<NetInfo>("Basic Road").m_segments[0].m_material;
            m_ImportAssetModels = new Dictionary<string, NEXTImportAssetModel>();
            var firstFileName = m_ModelFilenames[0];
            var path = firstFileName.Substring(0, firstFileName.LastIndexOf(Path.GetFileName(firstFileName)));
            var camera = new PreviewCamera();
            for (int i = 0; i < m_ModelFilenames.Length; i++)
            {
                var modelName = m_ModelFilenames[i];
                var modelBaseName = NEXTImportAssetModel.GetResourceModelName(modelName);
                var modelSuperBaseName = modelBaseName.Substring(0, modelBaseName.LastIndexOf("_"));
                Task<GameObject> modelTask = null;
                for (int j = 0; j < m_TextureFilenames.Length; j++)
                {
                    var textureName = m_TextureFilenames[j];
                    var resourceName = NEXTImportAssetModel.GetResourceName(modelName, textureName);
                    if (!m_ImportAssetModels.ContainsKey(resourceName))
                    {
                        var textureBaseName = NEXTImportAssetModel.GetResourceTextureName(textureName);
                        if (textureBaseName.StartsWith(modelSuperBaseName))
                        {
                            var importAssetModel = new NEXTImportAssetModel(camera, DefaultShader);
                            if (modelTask == null)
                                importAssetModel.Import(path, modelName);
                            else
                                importAssetModel.GameObjectLoader = modelTask;
                            importAssetModel.LoadTextures(modelName, textureName);
                            m_ImportAssetModels.Add(resourceName, importAssetModel);
                        }
                    }
                }
            }
        }
        protected static bool UpdateModels()
        {
            Debug.Log("Checkpoint10");
            if (m_ImportAssetModels == null)
                return false;
            var modelCount = m_ImportAssetModels.Count;
            for (int i = 0; i < modelCount; i++)
            {
                Debug.Log("Checkpoint10_" + i);
                var kvp = m_ImportAssetModels.ElementAt(i);
                var value = kvp.Value;
                if (value != null && Update(value))
                {
                    if (!NewInfoResources.ContainsKey(kvp.Key))
                    {
                        Debug.Log("Checkpoint10a_" + i);
                        var callback = new NEXTImportAssetModel.ImportAssetResourceCallback(OnAssetImported);
                        value.ResourcesReady(callback);
                        //m_ImportAssetModels[kvp.Key] = null;
                    }
                }
            }
            if (NewInfoResources.Count == m_ImportAssetModels.Count)
            {
                Debug.Log("CPum13");
                return true;
            }

            return false;
        }
        public static bool Update(NEXTImportAssetModel currentModel)
        {
            Debug.Log("Checkpoint11");
            var retval = true;
            if (currentModel.IsLoadingModel)
            {
                UnityEngine.Debug.Log("Is loading model");
                retval = false;
            }
            if (currentModel.IsLoadingTextures || !currentModel.ReadyForEditing)
            {
                Debug.Log("Checkpoint12");
                retval = false;
            }
            if (retval && currentModel.TextureLoadingFinished)
            {
                Debug.Log("Checkpoint12x");
                if (currentModel.Tasks == null)
                {
                    Debug.Log("Checkpoint12a " + currentModel.ResourceName + " isnull " + (currentModel.Object == null));
                    currentModel.FinalizeImport();
                }
            }
            if (currentModel.Tasks == null || currentModel.Tasks.isExecuting)
            {
                Debug.Log("Checkpoint14");
                retval = false;
            }

            Debug.Log("Checkpoint15");
            return retval;
        }
        private static string GetModelBasename(string modelFilename)
        {
            return Path.GetFileNameWithoutExtension(modelFilename);
        }
        private static string GetTextureBasename(string textureFilename)
        {
            var filename = Path.GetFileNameWithoutExtension(textureFilename);
            return filename.Substring(0, filename.LastIndexOf('_'));
        }
        //public void ImportAsset(Shader shader, string path, string filename)
        //{
        //    m_ModelName = filename;
        //    m_CurrentAsset = new TransitImportAssetModel(null, PreviewCamera, shader);
        //    m_CurrentAsset.Import(path, filename);
        //}

        public static Mesh GetMesh(string resourceName)
        {
            if (!NewInfoResources.ContainsKey(resourceName))
            {
                throw new Exception(String.Format("TFW: Resource {0} not found. There are {1} resources present with a key of {2}", resourceName, NewInfoResources.Count, NewInfoResources.Count > 0 ? NewInfoResources.ElementAt(0).Key : ""));
            }
            return NewInfoResources[resourceName].Mesh;
        }

        public static Material GetMaterial(string resourceName)
        {

            if (!NewInfoResources.ContainsKey(resourceName))
            {
                throw new Exception(String.Format("TFW: Resource {0} not found", resourceName));
            }
            return NewInfoResources[resourceName].Material;
        }

        public static ResourceUnit GetResource(string resourceName)
        {
            if (!NewInfoResources.ContainsKey(resourceName))
            {
                throw new Exception(String.Format("TFW: Resource {0} not found. There are {1} resources present with a key of {2}", resourceName, NewInfoResources.Count, NewInfoResources.Count > 0 ? NewInfoResources.ElementAt(0).Key : ""));
            }
            return NewInfoResources[resourceName];
        }
        private static string GetAssetPath(AssetType type)
        {
            return $@"Resources\{type}\";
        }

        public static void OnAssetImported(string resourceName, ResourceUnit resourceUnit)
        {
            NewInfoResources.Add(resourceName, resourceUnit);
        }
    }
    public enum AssetType
    {
        Roads
    }
}
