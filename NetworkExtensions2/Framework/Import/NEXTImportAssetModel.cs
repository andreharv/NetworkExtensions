using ColossalFramework;
using ColossalFramework.Importers;
using ColossalFramework.Threading;
using NetworkExtensions2.Framework._Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Transit.Framework;
using UnityEngine;
using Debug = UnityEngine.Debug;
namespace NetworkExtensions2.Framework.Import
{
    public class NEXTImportAssetModel : ImportAssetLodded
    {
        public delegate void ImportAssetResourceCallback(string resourceName, ResourceUnit resourceUnit);
        public delegate GameObject TextureLoaderModelCallback(GameObject go);
        private Material m_TemplateMaterial;
        private Shader m_TemplateShader;
        public static string AssetPath { get; set; }

        private Material m_NewMaterial;
        private Material NewMaterial
        {
            get
            {
                if (m_NewMaterial == null)
                    m_NewMaterial = new Material(m_TemplateShader);
                return m_NewMaterial;
            }
        }
        private GameObject m_GameObject;
        public string ResourceName { get; private set; }
        //private static Dictionary<string, Task<Task<GameObject>>> m_GameObjectCache;

        protected override AssetImporterTextureLoader.ResultType[] textureTypes
        {
            get
            {
                if (this.m_TemplateShader.name == "Custom/Net/Electricity")
                {
                    return new AssetImporterTextureLoader.ResultType[0];
                }
                return staticTextureTypes;
            }
        }
        private static AssetImporterTextureLoader.ResultType[] staticTextureTypes
        {
            get
            {
                return new AssetImporterTextureLoader.ResultType[]
                {
                    AssetImporterTextureLoader.ResultType.RGB,
                    AssetImporterTextureLoader.ResultType.XYS,
                    AssetImporterTextureLoader.ResultType.APR
                };
            }
        }
        public NEXTImportAssetModel(PreviewCamera camera, Shader templateShader, GameObject template = null) : base(template, camera)
        {
            this.m_LodTriangleTarget = 50;
            //this.m_TemplateMaterial = templateMaterial;
            this.m_TemplateShader = templateShader;
        }

        public Task ImportAsyncTask { get; set; }
        public Task<GameObject> GameObjectLoader { get; set; }
        public override void Import(string path, string filename)
        {
            if (GameObjectLoader == null)
            {
                if (m_Object != null)
                {
                    DestroyAsset();
                    m_Object = null;
                }
                m_Filename = filename;
                m_Path = path;
                m_IsLoadingModel = true;
                m_IsImportedAsset = true;
                m_Importer = new SceneImporter();
                CreateLODObject();
                m_Importer.filePath = Path.Combine(m_Path, m_Filename);
                m_Importer.importSkinMesh = true;
                var taskDistributor = new TaskDistributor(filename);
                ImportAsyncTask = taskDistributor.Dispatch(() =>
                {
                    Debug.Log("Model " + filename + " waiting...");
                    Threading.WaitOneAre(Threading.AreType.Model);
                    GameObjectLoader = m_Importer.ImportAsync();
                    Debug.Log("Model " + filename + " importing...");
                });
            }

        }

        protected static TaskDistributor sTaskDistributor = new TaskDistributor("SubTaskDistributor2");
        protected static Dictionary<string, Task<GameObject>> m_GameObjectLoaderCache;

        public static Dictionary<string, Task<GameObject>> GameObjectLoaderCache
        {
            get
            {
                if (m_GameObjectLoaderCache == null)
                    m_GameObjectLoaderCache = new Dictionary<string, Task<GameObject>>();
                return m_GameObjectLoaderCache;
            }
            set { m_GameObjectLoaderCache = value; }
        }

        protected static Dictionary<string, Task<Dictionary<string, Texture2D>>> m_TextureLoaderCache;
        public static Dictionary<string, Task<Dictionary<string, Texture2D>>> TextureLoaderCache
        {
            get
            {
                if (m_TextureLoaderCache == null)
                    m_TextureLoaderCache = new Dictionary<string, Task<Dictionary<string, Texture2D>>>();
                return m_TextureLoaderCache;
            }
            set { m_TextureLoaderCache = value; }
        }

        public void LoadTextures(string modelName, string textureName)
        {
            ResourceName = GetResourceName(modelName, textureName);              
            var baseTextureName = textureName.Substring(0, textureName.LastIndexOf("_"));
            var callback = new TextureLoaderModelCallback(GenerateAssetData2);
            var taskDistributor = new TaskDistributor(baseTextureName);
            taskDistributor.Dispatch(() =>
            {
                if (ImportAsyncTask != null && !ImportAsyncTask.hasEnded)
                    ImportAsyncTask.Wait();
                Debug.Log("Processing texture " + baseTextureName + " on " + modelName);
                m_TextureTask = NEXTImportAssetTextureLoader.ProcessTextures(GameObjectLoader, staticTextureTypes, baseTextureName, false, callback, staticTextureAnisoLevel, true);
            });
                //    taskDistributor.Dispatch(() =>
        //    {
        //        if (ImportAsyncTask != null && !ImportAsyncTask.hasEnded)
        //            ImportAsyncTask.Wait();


                
        //        //Dictionary<string, Task<Texture2D>> processedTextures = processedTexturesTask.result;

                
        //        //foreach (var textureKvp in processedTextures)
        //        //{
        //        //    if (textureKvp.Value != null && !textureKvp.Value.hasEnded)
        //        //        textureKvp.Value.Wait();
        //        //    Debug.Log(textureKvp.Key + " is named " + textureKvp.Value.result.name);
        //        //    AssetImporterTextureLoader.ApplyTexture(model, textureKvp.Key, textureKvp.Value.result);
        //        //}
        //    });
        }
        public static string GetResourceName(string modelName, string textureName)
        {
            return $"{GetResourceModelName(modelName)}|{GetResourceTextureName(textureName)}";
        }
        public static string GetResourceModelName(string modelName)
        {
            var modelExtension = Path.GetExtension(modelName);
            if (modelExtension.Length > 0)
                modelName = modelName.Replace(modelExtension, "");
            return modelName.Replace(AssetPath + "\\", "");
        }
        public static string GetResourceTextureName(string textureName, bool trimAssetPath = true)
        {
            var textureExtension = Path.GetExtension(textureName);
            if (textureExtension.Length > 0)
                textureName = textureName.Replace(textureExtension, "");
            var textureSuffixPattern = @"_[A-Za-z]\b";
            var lol = Regex.Replace(textureName, textureSuffixPattern, "");
            if (trimAssetPath)
                lol = lol.Replace(trimAssetPath ? AssetPath + "\\" : "", "");
            return lol;
        }

        protected override void GenerateAssetData(GameObject instance)
        {
            if (instance == null)
            {
                m_IsLoadingModel = false;
                m_ReadyForEditing = false;
                return;
            }
            Debug.Log("CPGAD1");
            var newInstance = UnityEngine.Object.Instantiate(instance);
            Debug.Log("CPGAD1 m_Object is null" + (m_Object == null));
            m_Object = newInstance;
            Debug.Log("GAD1 " + ResourceName);
            m_Object.SetActive(value: false);
            Debug.Log("CPGAD2");
            newInstance.transform.localScale = Vector3.one;
            Debug.Log("CPGAD3");
            Mesh sharedMesh = GetSharedMesh(m_Object);
            Debug.Log("CPGAD4");
            if (sharedMesh == null)
            {
                Debug.Log("CPcb1 sharedMeshIsNull!");
                m_IsLoadingModel = false;
                m_ReadyForEditing = false;
                return;
            }
            Debug.Log("CPGAD5");
            SetMesh2(newInstance, sharedMesh, true);
            Debug.Log("CPcb2");
            sharedMesh.name = Path.GetFileNameWithoutExtension(m_Filename);
            Debug.Log("CPcb2a");
            m_OriginalMesh = RuntimeMeshUtils.CopyMesh(sharedMesh);
            Debug.Log("CPcb2b");
            CreateInfo();
            Debug.Log("CPcb2c");
            CopyMaterialProperties();
            Debug.Log("CPcb3");
            m_IsLoadingModel = false;
            m_ReadyForEditing = true;
        }
        protected GameObject GenerateAssetData2(GameObject instance)
        { 
            if (instance == null)
            {
                m_IsLoadingModel = false;
                m_ReadyForEditing = false;
                return null;
            }

            var newInstance = UnityEngine.Object.Instantiate(instance);
            m_Object = newInstance;
            m_Object.SetActive(false);
            newInstance.transform.localScale = Vector3.one;
            Mesh sharedMesh = GetSharedMesh(m_Object);
            if (sharedMesh == null)
            {
                m_IsLoadingModel = false;
                m_ReadyForEditing = false;
                return null;
            }

            var sharedMesh2 = SetMesh2(newInstance, sharedMesh, true);
            Debug.Log("CPcb2a");
            m_OriginalMesh = RuntimeMeshUtils.CopyMesh(sharedMesh2);
            Debug.Log("CPcb2b");
            CreateInfo();
            Debug.Log("CPcb2c");
            //CopyMaterialProperties();
            Debug.Log("CPcb3");
            m_IsLoadingModel = false;
            m_ReadyForEditing = true;
            return newInstance;
        }
        public override float CalculateDefaultScale()
        {
            return 1f;
        }
        protected override void CompressTextures()
        {
            Material sharedMaterial = this.m_Object.GetComponent<Renderer>().sharedMaterial;
            if (sharedMaterial.HasProperty("_MainTex"))
            {
                Debug.Log("CPct1");
                this.m_Tasks[0][0] = AssetImporterTextureLoader.CompressTexture(sharedMaterial, "_MainTex", false);
            }
            if (sharedMaterial.HasProperty("_XYSMap"))
            {
                Debug.Log("CPct2");
                this.m_Tasks[0][1] = AssetImporterTextureLoader.CompressTexture(sharedMaterial, "_XYSMap", true);
            }
            if (sharedMaterial.HasProperty("_ARPMap"))
            {
                Debug.Log("CPct3");
                this.m_Tasks[0][2] = AssetImporterTextureLoader.CompressTexture(sharedMaterial, "_APRMap", true);
            }
        }
        public override void FinalizeImport()
        {
            Debug.Log("CPFI1");
            this.FinalizeLOD();
            Debug.Log("CPFI2");
            if (this.m_Object.GetComponent<Renderer>() != null)
            {
                Debug.Log("CPFI2a");
                this.CompressTextures();
            }
            Debug.Log("CPFI3");
            this.m_TaskWrapper = new MultiAsyncTaskWrapper(this.m_TaskNames, this.m_Tasks);
            Debug.Log("CPFI4");
            LoadSaveStatus.activeTask = this.m_TaskWrapper;
            Debug.Log("CPFI5");
        }

        protected override void CopyMaterialProperties()
        {
            this.material = new Material(this.m_TemplateShader);
            if (this.material.HasProperty("_Color"))
            {
                this.material.SetColor("_Color", Color.gray);
            }
        }

        public override void DestroyAsset()
        {
            if (this.m_OriginalMesh != null)
            {
                Debug.Log("ResourceMesh " + ResourceName);
                UnityEngine.Object.DestroyImmediate(this.m_OriginalMesh);
                this.m_OriginalMesh = null;
            }
            if (this.m_OriginalLodMesh != null)
            {
                Debug.Log("ResourceLodMesh " + ResourceName);
                UnityEngine.Object.DestroyImmediate(this.m_OriginalLodMesh);
                this.m_OriginalLodMesh = null;
            }
            //if (this.m_Object != null)
            //{
            //    Debug.Log("ResourceObject is null for " + ResourceName + (m_Object == null));
            //    UnityEngine.Object.DestroyImmediate(this.m_Object);
            //    this.m_Object = null;
            //}
            if (this.m_LODObject != null)
            {
                Debug.Log("ResourceLODObject " + ResourceName);
                UnityEngine.Object.DestroyImmediate(this.m_LODObject);
                this.m_LODObject = null;
            }
        }
        protected static Mesh SetMesh2(GameObject go, Mesh mesh, bool instantiate = false)
        {
            Mesh mesh2;
            if (instantiate)
            {
                mesh2 = new Mesh();
                mesh.Transform(mesh2, new Vector3(0, 0, 0));
            }
            else
            {
                mesh2 = mesh;
            }
            MeshFilter component = go.GetComponent<MeshFilter>();
            if (component != null)
            {
                component.sharedMesh = mesh2;
            }
            return mesh2;
        }
        public void ResourcesReady(ImportAssetResourceCallback callback)
        {
            PreviewCamera.InfoRenderer.FullShading();
            ApplyTransform(new Vector3(100, 100, 100), new Vector3(270, 0, 0), false);
            if (callback != null)
            {
                var resourceUnit = new ResourceUnit(mesh, material, lodMesh, lodMaterial);
                callback(ResourceName, resourceUnit);
            }
            else
            {
                Debug.Log("ModelImportCallback is null");
            }
            DestroyAsset();
        }

        protected override int textureAnisoLevel
        {
            get
            {
                return staticTextureAnisoLevel;
            }
        }
        private static int staticTextureAnisoLevel
        {
            get
            {
                return 8;
            }
        }
        public override Mesh mesh
        {
            get
            {
                return this.m_Object?.GetComponent<MeshFilter>()?.sharedMesh;
            }
        }

        public override Material material
        {
            get
            {
                var mat = this.m_Object.GetComponent<MeshRenderer>().sharedMaterial;
                Debug.Log("MainTex is null " + (mat.GetTexture("_MainTex") == null));
                Debug.Log("APRMap is null " + (mat.GetTexture("_APRMap") == null));
                Debug.Log("XYSMap is null " + (mat.GetTexture("_XYSMap") == null));
                return this.m_Object.GetComponent<MeshRenderer>().sharedMaterial;
            }
            set
            {
                this.m_Object.GetComponent<MeshRenderer>().sharedMaterial = value;
            }
        }

        public override Mesh lodMesh
        {
            get
            {
                return this.m_LODObject?.GetComponent<MeshFilter>()?.sharedMesh;
            }
        }

        public override Material lodMaterial
        {
            get
            {
                return this.m_LODObject?.GetComponent<MeshRenderer>()?.sharedMaterial;
            }
        }

        protected override void InitializeObject()
        {
        }

        protected override void CreateInfo()
        {
        }
    }
}
