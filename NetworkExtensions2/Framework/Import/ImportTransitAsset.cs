using ColossalFramework.Threading;
using ICities;
using System;
using System.IO;
using UnityEngine;

namespace NetworkExtensions2.Framework.Import
{
    public class ImportTransitAsset : IImportable
    {

        protected static TaskDistributor sTaskDistributor = new TaskDistributor("SubTaskDistributor");
        protected static readonly float timeout = 120;
        public delegate void ModelImportCallbackHandler(Mesh mesh, Material material, Mesh lodMesh, Material lodMaterial);
        public static event EventHandler<ResourceImportArgs> CallbackCalled;
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

        public void ImportAsset(Shader shader, string path, string filename)
        {
            m_ModelName = filename;
            m_CurrentAsset = new TransitImportAssetModel(null, PreviewCamera, shader);
            m_CurrentAsset.Import(path, filename);

            //var delta = 0.05f;
            //var counter = 0f;
            //Task<bool> getResourceTask = null;
            //getResourceTask = new Task<bool>(() =>
            //{
            //    while (!m_OnModelImportFinsihed && counter <= timeout)
            //    {
            //        counter += delta;
            //        getResourceTask.WaitForSeconds(delta);
            //    }
            //    return m_OnModelImportFinsihed;
            //});
            //sTaskDistributor.Dispatch(getResourceTask);
            //return getResourceTask;
        }
        private void ResourcesReady()
        {
            m_ModelReady = false;
            PreviewCamera.InfoRenderer.FullShading();
            m_CurrentAsset.ApplyTransform(new Vector3(100, 100, 100), new Vector3(0, 270, 0), false);
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
            ResourceImportArgs args = new ResourceImportArgs();
            args.Name = Path.GetFileNameWithoutExtension(m_ModelName);
            args.ResourceUnit = new ResourceUnit()
            {
                Mesh = mesh,
                LodMesh = lodMesh,
                Material = material,
                LodMaterial = lodMaterial
            };
            CallbackCalled?.Invoke(this, args);
        }
        public void Update()
        {
            m_ModelReady = true;
            if (m_CurrentAsset != null)
            {
                if (m_CurrentAsset.IsLoadingModel || m_CurrentAsset.IsLoadingTextures || !m_CurrentAsset.ReadyForEditing)
                {
                    Debug.Log("Is loading model");
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
    }
    public class ResourceImportArgs : EventArgs
    {
        public string Name { get; set; }
        public ResourceUnit ResourceUnit { get; set; }
    }
}
