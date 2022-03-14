using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;

namespace NetworkExtensions2.Framework.Import
{
    public class ImportTransitAsset : IImportable
    {
        private static bool m_ImportFinished;
        private static string m_ResourcePath;
        private static List<string> m_Filenames;
        private static int m_FilenamesLength;
        private static ImportTransitAsset[] m_ImportTransitAssets;

        private static Shader m_DefaultShader;

        public static Shader DefaultShader
        {
            get { 
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
        private TransitImportAssetModel m_CurrentAsset;
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

        private ModelImportCallbackHandler m_ModelImportCallback;
        protected ModelImportCallbackHandler ModelImportCallback
        {
            get
            {
                if (m_ModelImportCallback == null)
                    m_ModelImportCallback = OnAssetImported;
                return m_ModelImportCallback;
            }
        }

        private bool m_ModelReady;
        private string m_ModelName;
        public static bool UptakeImportFiles(string path, AssetType type)
        {
            var swAll = new Stopwatch();
            swAll.Start();
            Debug.Log("Checkpoint1");
            if (!m_ImportFinished)
            {
                Debug.Log("Checkpoint6");
                if (string.IsNullOrEmpty(m_ResourcePath))
                    m_ResourcePath = Path.Combine(path, GetAssetPath(type));
                if (m_ImportTransitAssets != null || Directory.Exists(m_ResourcePath))
                {
                    Debug.Log("Checkpoint7");
                    if (m_Filenames == null)
                    {
                        m_Filenames = Directory.GetFiles(m_ResourcePath, "*.fbx").ToList();
                        m_FilenamesLength = m_Filenames.Count;
                    }

                    if (m_FilenamesLength > 0)
                    {
                        Debug.Log("Checkpoint8");
                        if (m_ImportTransitAssets == null)
                        {
                            Debug.Log("Checkpoint9");
                            m_ImportTransitAssets = new ImportTransitAsset[m_FilenamesLength];
                        }
                        for (int i = 0; i < m_FilenamesLength; i++)
                        {
                            Debug.Log("Checkpoint10");
                            var importTransitAsset = m_ImportTransitAssets[i];
                            var filename = m_Filenames[i];
                            if (importTransitAsset == null)
                            {
                                Debug.Log("Checkpoint11");
                                importTransitAsset = new ImportTransitAsset();
                                importTransitAsset.ImportAsset(DefaultShader, m_ResourcePath, filename);
                                m_ImportTransitAssets[i] = importTransitAsset;
                            }
                            else if (!m_NewInfoResources.ContainsKey(filename))
                            {
                                Debug.Log("Checkpoint12");
                                m_ImportTransitAssets[i].Update();
                            }
                        }
                        if (NewInfoResources.Count == m_FilenamesLength)
                        {
                            Debug.Log("Checkpoint13");
                            Debug.Log("newInfoResources Count:" + NewInfoResources.Count);
                            if (NewInfoResources.Count > 0)
                            {
                                var hi = NewInfoResources.First().Value;
                                Debug.Log("Mesh exists " + (hi.Mesh != null));
                                Debug.Log("lodMesh exists " + (hi.LodMesh != null));
                                Debug.Log("Material exists " + (hi.Material != null));
                                Debug.Log("lodMaterial exists " + (hi.LodMaterial != null));
                            }
                            swAll.Stop();
                            Debug.Log($"All RExModule Prereqs loaded in {swAll.ElapsedMilliseconds}ms");
                            return true;
                        }
                    }
                }
                else
                {
                    Debug.LogError("The specified path: " + m_ResourcePath + " does not exist");
                }
            }
            return false;
        }
        public void ImportAsset(Shader shader, string path, string filename)
        {
            m_ModelName = filename;
            m_CurrentAsset = new TransitImportAssetModel(null, PreviewCamera, shader);
            m_CurrentAsset.Import(path, filename);
        }

        public void Update()
        {
            m_ModelReady = true;
            if (m_CurrentAsset != null)
            {
                if (m_CurrentAsset.IsLoadingModel || m_CurrentAsset.IsLoadingTextures || !m_CurrentAsset.ReadyForEditing)
                {
                    UnityEngine.Debug.Log("Is loading model");
                    m_ModelReady = false;
                }
                if (m_CurrentAsset is TransitImportAssetModel)
                {
                    if (m_CurrentAsset.TextureLoadingFinished && m_CurrentAsset.Tasks == null)
                    {
                        Debug.Log("Doing finalization");
                        m_CurrentAsset.FinalizeImport();
                    }
                    if (m_CurrentAsset.Tasks == null || m_CurrentAsset.Tasks.isExecuting)
                    {
                        Debug.Log("No tasks or task is executing");
                        m_ModelReady = false;
                    }
                }
            }
            else
            {
                Debug.Log("currentasset is null");
                m_ModelReady = false;
            }
            if (m_ModelReady)
            {
                Debug.Log("model is ready");
                ResourcesReady();
            }
        }

        public static Mesh GetMesh(string resourcePath)
        {
            if (!NewInfoResources.ContainsKey(resourcePath))
            {
                throw new Exception(String.Format("TFW: Mesh {0} not found", resourcePath));
            }
            return NewInfoResources[resourcePath].Mesh;
        }

        public static Material GetMaterial(string resourcePath)
        {

            if (!NewInfoResources.ContainsKey(resourcePath))
            {
                throw new Exception(String.Format("TFW: Mesh {0} not found", resourcePath));
            }
            return NewInfoResources[resourcePath].Material;
        }

        private static string GetAssetPath(AssetType type)
        {
            return $@"Resources\{type}\";
    }
        private void ResourcesReady()
        {
            m_ModelReady = false;
            PreviewCamera.InfoRenderer.FullShading();
            m_CurrentAsset.ApplyTransform(new Vector3(100, 100, 100), new Vector3(270, 0, 0), false);
            if (ModelImportCallback != null)
            {
                ModelImportCallback(m_CurrentAsset.mesh, m_CurrentAsset.material, m_CurrentAsset.lodMesh, m_CurrentAsset.lodMaterial);
            }
            else
            {
                Debug.Log("ModelImportCallback is null");
            }
            m_CurrentAsset.DestroyAsset();
            m_CurrentAsset = null;
        }

        private void OnAssetImported(Mesh mesh, Material material, Mesh lodMesh, Material lodMaterial)
        {
            Debug.Log("A resource was created!");
            var modelName = Path.GetFileNameWithoutExtension(m_ModelName);
            NewInfoResources[modelName] = new ResourceUnit()
            {
                Mesh = mesh,
                LodMesh = lodMesh,
                Material = material,
                LodMaterial = lodMaterial
            };
        }
    }
    public enum AssetType
    {
        Roads
    }
}
