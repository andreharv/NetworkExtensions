using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;

namespace NetworkExtensions2.Framework.Import
{
    public class ImportTransitAsset
    {
        public ResourceUnit ResourceUnit { get; private set; }
        private TransitImportAssetModel m_CurrentAsset;
        private bool m_ResourceReady;
        public bool ResourceReady => m_ResourceReady;
        private string m_ModelName;
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
        
        public void ImportCurrentAsset(Shader shader, string path, string filename, string textureFilename)
        {
            m_ModelName = filename;
            m_CurrentAsset = new TransitImportAssetModel(null, PreviewCamera, shader);
            m_CurrentAsset.ImportAsset(path, filename, textureFilename);
        }
        public string ResourceName()
        {
            if (m_CurrentAsset == null)
                return null;
            return m_CurrentAsset.GetResourceName();
        }
        public void Update()
        {
            Debug.Log("m_Object null " + m_CurrentAsset.GetResourceName() + " " + (m_CurrentAsset.Object == null));
            m_ResourceReady = true;
            if (m_CurrentAsset != null)
            {
                if (m_CurrentAsset.IsLoadingModel || m_CurrentAsset.IsLoadingTextures || !m_CurrentAsset.ReadyForEditing)
                {
                    UnityEngine.Debug.Log("Is loading model");
                    m_ResourceReady = false;
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
                        m_ResourceReady = false;
                    }
                }
            }
            else
            {
                Debug.Log("currentasset is null");
                m_ResourceReady = false;
            }
            if (m_ResourceReady)
            {
                Debug.Log("model is ready");
                OnResourcesReady();
            }
        }

        private void OnResourcesReady()
        {
            PreviewCamera.InfoRenderer.FullShading();
            m_CurrentAsset.ApplyTransform(new Vector3(100, 100, 100), new Vector3(270, 0, 0), false);
            ResourceUnit = new ResourceUnit(
                m_CurrentAsset.mesh, 
                m_CurrentAsset.material, 
                m_CurrentAsset.lodMesh, 
                m_CurrentAsset.lodMaterial
            );

            m_CurrentAsset.DestroyAsset();
            m_CurrentAsset = null;
        }


    }
    public enum AssetType
    {
        Roads
    }
}
