using ColossalFramework.Threading;
using UnityEngine;

namespace NetworkExtensions2.Framework.Import
{
	internal class ImportTransitModel : ImportAssetLodded
	{
		private Shader m_TemplateShader;

		protected override AssetImporterTextureLoader.ResultType[] textureTypes
		{
			get
			{
				if (this.m_TemplateShader.name == "Custom/Net/Electricity")
				{
					return new AssetImporterTextureLoader.ResultType[0];
				}
				return new AssetImporterTextureLoader.ResultType[]
				{
				AssetImporterTextureLoader.ResultType.RGB,
				AssetImporterTextureLoader.ResultType.XYS,
				AssetImporterTextureLoader.ResultType.APR
				};
			}
		}

		protected override int textureAnisoLevel
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
				return this.m_Object.GetComponent<MeshFilter>().sharedMesh;
			}
		}

		public override Material material
		{
			get
			{
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
				return this.m_LODObject.GetComponent<MeshFilter>().sharedMesh;
			}
		}

		public override Material lodMaterial
		{
			get
			{
				return this.m_LODObject.GetComponent<MeshRenderer>().sharedMaterial;
			}
		}

		public ImportTransitModel(GameObject template, PreviewCamera camera, Shader templateShader) : base(template, camera)
		{
			this.m_LodTriangleTarget = 50;
			this.m_TemplateShader = templateShader;
		}

		protected override void InitializeObject()
		{
		}

		protected override void CreateInfo()
		{
		}

		public override float CalculateDefaultScale()
		{
			return 1f;
		}

		protected override void CopyMaterialProperties()
		{
			this.material = new Material(this.m_TemplateShader);
			if (this.material.HasProperty("_Color"))
			{
				this.material.SetColor("_Color", Color.gray);
			}
		}

		public override void FinalizeImport()
		{
			this.FinalizeLOD();
			if (this.m_Object.GetComponent<Renderer>() != null)
			{
				this.CompressTextures();
			}
			this.m_TaskWrapper = new MultiAsyncTaskWrapper(this.m_TaskNames, this.m_Tasks);
			LoadSaveStatus.activeTask = this.m_TaskWrapper;
		}

		protected override void CompressTextures()
		{
			Material sharedMaterial = this.m_Object.GetComponent<Renderer>().sharedMaterial;
			if (sharedMaterial.HasProperty("_MainTex"))
			{
				this.m_Tasks[0][0] = AssetImporterTextureLoader.CompressTexture(sharedMaterial, "_MainTex", false);
			}
			if (sharedMaterial.HasProperty("_XYSMap"))
			{
				this.m_Tasks[0][1] = AssetImporterTextureLoader.CompressTexture(sharedMaterial, "_XYSMap", true);
			}
			if (sharedMaterial.HasProperty("_ARPMap"))
			{
				this.m_Tasks[0][2] = AssetImporterTextureLoader.CompressTexture(sharedMaterial, "_APRMap", true);
			}
		}

		protected override void LoadTextures(Task<GameObject> modelLoad)
		{
			this.m_TextureTask = AssetImporterTextureLoader.LoadTextures(modelLoad, null, this.textureTypes, this.m_Path, this.m_Filename, false, this.textureAnisoLevel, true, false);
		}

		public override void DestroyAsset()
		{
			if (this.m_OriginalMesh != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_OriginalMesh);
				this.m_OriginalMesh = null;
			}
			if (this.m_OriginalLodMesh != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_OriginalLodMesh);
				this.m_OriginalLodMesh = null;
			}
			if (this.m_Object != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Object);
				this.m_Object = null;
			}
			if (this.m_LODObject != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_LODObject);
				this.m_LODObject = null;
			}
		}
	}
}
