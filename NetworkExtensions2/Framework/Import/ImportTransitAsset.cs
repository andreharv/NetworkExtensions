using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkExtensions2.Framework.Import
{
    public class ImportTransitAsset : ThreadingExtensionBase
    {
        public delegate void ModelImportCallbackHandler(Mesh mesh, Material material, Mesh lodMesh, Material lodMaterial);
        private  ModelImportCallbackHandler m_ModelImportCallbackHandler;
        private TransitImportAssetModel m_CurrentAsset;
        private PreviewCamera m_PreviewCamera;
        private bool m_ModelReady = true;
        private bool m_HadModelLastTime = false;

        public void ImportAsset(Shader shader, string path, string filename)
        {
            if (m_ModelImportCallbackHandler == null)
                m_ModelImportCallbackHandler = OnModelImported;
            m_CurrentAsset = new TransitImportAssetModel(null, m_PreviewCamera, shader);
            m_CurrentAsset.Import(path, filename);
            m_CurrentAsset.ApplyTransform(new Vector3(100, 100, 100), new Vector3(0, 270, 0), false);
        }

        public void ImportAsset()
        {

        }

        private void OnModelImported(Mesh mesh, Material material, Mesh lodMesh, Material lodMaterial)
        {
            Debug.Log("WE GOT HERE!");
        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            m_ModelReady = true;
            if (m_CurrentAsset != null)
            {
                if (m_CurrentAsset.IsLoadingModel || m_CurrentAsset.IsLoadingTextures || !m_CurrentAsset.ReadyForEditing)
                {
                    m_ModelReady = false;
                }
                if(m_CurrentAsset is TransitImportAssetModel)
                {
                    if (m_CurrentAsset.TextureLoadingFinished && m_CurrentAsset.Tasks == null)
                    {
                        m_CurrentAsset.FinalizeImport();
                    }
                    if (m_CurrentAsset.Tasks == null || m_CurrentAsset.Tasks.isExecuting)
                    {
                        m_ModelReady = false;
                    }
                }
            }
        }
    }
}
