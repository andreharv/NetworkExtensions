using ColossalFramework.Importers;
using ColossalFramework.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NetworkExtensions2.Framework.Import
{
    public class NEXTImportAssetModel : ImportAssetLodded
    {
        private Shader m_TemplateShader;
        private GameObject m_GameObject;
        private Task<GameObject> m_ModelLoad;

        public GameObject FetchGameObject(bool instantiate)
        {
            if (m_GameObject == null)
            {
                if (m_ModelLoad == null)
                {
                    throw new Exception("Import has not been run yet! No gameObject available");
                }
                m_ModelLoad.Wait();
                m_GameObject = m_ModelLoad.result;
            }
            if (instantiate)
            {
                return UnityEngine.Object.Instantiate(m_GameObject);
            }
            return m_GameObject;
        }
        public NEXTImportAssetModel(GameObject template, PreviewCamera camera, Shader templateShader) : base(template, camera)
        {
            this.m_LodTriangleTarget = 50;
            this.m_TemplateShader = templateShader;
        }
        public override void Import(string path, string filename)
        {
            if (this.m_Object != null)
            {
                this.DestroyAsset();
                this.m_Object = null;
            }
            this.m_Filename = filename;
            this.m_Path = path;
            this.m_IsLoadingModel = true;
            this.m_IsImportedAsset = true;
            this.m_Importer = new SceneImporter();
            this.CreateLODObject();
            this.m_Importer.filePath = Path.Combine(path, filename);
            this.m_Importer.importSkinMesh = true;
            m_ModelLoad = m_Importer.ImportAsync(new Action<GameObject>(GenerateAssetData));
        }
        protected override void GenerateAssetData(GameObject instance)
        {
            if (instance == null)
            {
                m_IsLoadingModel = false;
                m_ReadyForEditing = false;
                return;
            }
            var newInstance = UnityEngine.Object.Instantiate(instance);
            m_Object = newInstance;
            m_Object.SetActive(value: false);
            newInstance.transform.localScale = Vector3.one;
            Mesh sharedMesh = GetSharedMesh(m_Object);
            if (sharedMesh == null)
            {
                m_IsLoadingModel = false;
                m_ReadyForEditing = false;
                return;
            }
            SetMesh(newInstance, sharedMesh, true);
            sharedMesh.name = Path.GetFileNameWithoutExtension(m_Filename);
            m_OriginalMesh = RuntimeMeshUtils.CopyMesh(sharedMesh);
            CreateInfo();
            CopyMaterialProperties();
            m_IsLoadingModel = false;
            m_ReadyForEditing = true;
        }
        protected override void CreateInfo()
        {
            throw new NotImplementedException();
        }

        protected override void InitializeObject()
        {
            throw new NotImplementedException();
        }
    }
}
