using ColossalFramework;
using ColossalFramework.Importers;
using ColossalFramework.IO;
using ColossalFramework.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace NetworkExtensions2.Framework.Import
{
    internal class TransitImportAssetModel : ImportAssetLodded
    {
        public ResourceUnit ResourceUnit { get; private set; }
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

        protected static readonly List<AssetImporterTextureLoader.SourceType>[] NeededSources = new List<AssetImporterTextureLoader.SourceType>[]
        {
            new List<AssetImporterTextureLoader.SourceType>
            {
                AssetImporterTextureLoader.SourceType.DIFFUSE
            },
            new List<AssetImporterTextureLoader.SourceType>
            {
                AssetImporterTextureLoader.SourceType.NORMAL,
                AssetImporterTextureLoader.SourceType.SPECULAR
            },
            new List<AssetImporterTextureLoader.SourceType>
            {
                AssetImporterTextureLoader.SourceType.ALPHA,
                AssetImporterTextureLoader.SourceType.COLOR,
                AssetImporterTextureLoader.SourceType.ILLUMINATION
            },
            new List<AssetImporterTextureLoader.SourceType>
            {
                AssetImporterTextureLoader.SourceType.NORMAL,
                AssetImporterTextureLoader.SourceType.COLOR,
                AssetImporterTextureLoader.SourceType.ALPHA
            },
            new List<AssetImporterTextureLoader.SourceType>
            {
                AssetImporterTextureLoader.SourceType.ALPHA,
                AssetImporterTextureLoader.SourceType.PAVEMENT,
                AssetImporterTextureLoader.SourceType.ROAD
            }
        };

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

        private static string[] m_ResultSamplers;

        protected static string[] ResultSamplers
        {
            get
            {
                if (m_ResultSamplers == null)
                {
                    var fieldInfo = GetTextureLoaderField("ResultSamplers");
                    var obj = fieldInfo?.GetValue(null);
                    if (obj != null)
                    {
                        m_ResultSamplers = ((IEnumerable<String>)obj).Cast<object>().Select(x => x.ToString()).ToArray();
                    }
                }
                return m_ResultSamplers;
            }
        }

        private static Color[] m_ResultDefaults;

        protected static Color[] ResultDefaults
        {
            get
            {
                if (m_ResultDefaults == null)
                {
                    var fieldInfo = GetTextureLoaderField("ResultDefaults");
                    var obj = fieldInfo?.GetValue(null);
                    if (obj != null)
                    {
                        m_ResultDefaults = ((IEnumerable<Color>)obj).Cast<object>().Select(x => (Color)x).ToArray();
                    }
                }
                return m_ResultDefaults;
            }
        }
        private static Dictionary<string, bool> m_ResultLinear;

        protected static Dictionary<string, bool> ResultLinear
        {
            get
            {
                if (m_ResultLinear == null)
                {
                    var fieldInfo = GetTextureLoaderField("ResultLinear");
                    var obj = fieldInfo?.GetValue(null);
                    if (obj != null)
                    {
                        m_ResultLinear = obj as Dictionary<string, bool>;
                    }
                }
                return m_ResultLinear;
            }
        }

        private static FieldInfo GetTextureLoaderField(string fieldName)
        {
            return typeof(AssetImporterTextureLoader).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
        }
        private static MethodInfo GetTextureLoaderMethod(string methodName)
        {
            return typeof(AssetImporterTextureLoader).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        }

        protected static Color[] CallBuildTexture(Image[] sources, AssetImporterTextureLoader.ResultType type, int width, int height, bool nullAllowed)
        {
            var methodInfo = GetTextureLoaderMethod("BuildTexture");
            var obj = methodInfo?.Invoke(null, new object[] { sources, type, width, height, nullAllowed });
            Color[] result = null;
            if (obj != null)
            {
                result = ((IEnumerable<Color>)obj).Cast<object>().Select(x => (Color)x).ToArray();
            }
            return result;
        }

        protected static Task<Image> CallLoadTexture(AssetImporterTextureLoader.SourceType source, string baseName)
        {
            var methodInfo = GetTextureLoaderMethod("LoadTexture");
            var obj = methodInfo?.Invoke(null, new object[] { source, baseName });
            Task<Image> result = null;
            if (obj != null)
            {
                result = (Task<Image>)obj;
            }
            return result;
        }

        protected static Image[] CallExtractImages(Task<Image>[] tasks)
        {
            var methodInfo = GetTextureLoaderMethod("ExtractImages");
            var obj = methodInfo?.Invoke(null, new object[] { tasks });
            Image[] result = null;
            if (obj != null)
            {
                result = ((IEnumerable<Image>)obj).Cast<object>().Select(x => (Image)x).ToArray();
            }
            return result;
        }

        public TransitImportAssetModel(GameObject template, PreviewCamera camera, Shader templateShader) : base(template, camera)
        {
            this.m_LodTriangleTarget = 50;
            this.m_TemplateShader = templateShader;
        }

        private string m_TextureFilenameBase;

        public void ImportAsset(string path, string modelFilename, string textureFileNameBase)
        {
            if (this.m_Object != null)
            {
                Debug.Log("Doing another runnn");
                this.DestroyAsset();
                this.m_Object = null;
            }
            this.m_Filename = modelFilename;
            m_TextureFilenameBase = textureFileNameBase;
            this.m_Path = path;
            this.m_IsLoadingModel = true;
            this.m_IsImportedAsset = true;
            this.m_Importer = new SceneImporter();
            this.CreateLODObject();
            this.m_Importer.filePath = Path.Combine(path, modelFilename);
            this.m_Importer.importSkinMesh = true;
            CreateModelAndTextures(modelFilename);
        }
        public void CreateModelAndTextures(string modelFilename)
        {
            var modelLoad = m_Importer.ImportAsync(new Action<GameObject>(GenerateAssetData));
            ImportTextures(modelLoad);
            //if (m_GameObjectTaskDict == null)
            //    m_GameObjectTaskDict = new Dictionary<string, Task<GameObject>>();
            //if (!m_GameObjectTaskDict.ContainsKey(modelFilename))
            //{
            //    m_GameObjectTaskDict.Add(modelFilename, m_Importer.ImportAsync(null));
            //}
            //Debug.Log("gameObjectTaskDict Count " + m_GameObjectTaskDict.Count);
            //ImportTextures(m_GameObjectTaskDict[modelFilename]);
        }

        protected void GenerateAssetData2(GameObject instance)
        {
            GenerateAssetData(UnityEngine.Object.Instantiate(instance));
        }
        protected override void GenerateAssetData(GameObject instance)
        {
            Debug.Log("CheckpointZ");
            if (instance == null)
            {
                Debug.Log("CheckpointY");
                m_IsLoadingModel = false;
                m_ReadyForEditing = false;
                return;
            }
            
            m_Object = instance;
            Debug.Log("m_Object in " + GetResourceName());
            Debug.Log("m_Object is null " + (m_Object == null));
            m_Object.SetActive(value: false);
            instance.transform.localScale = Vector3.one;
            Mesh sharedMesh = GetSharedMesh(m_Object);
            if (sharedMesh == null)
            {
                Debug.Log("CheckpointW");
                m_IsLoadingModel = false;
                m_ReadyForEditing = false;
                return;
            }
            Debug.Log("CheckpointV");
            m_OriginalMesh = RuntimeMeshUtils.CopyMesh(sharedMesh);
            Debug.Log("CheckpointU");
            CreateInfo();
            Debug.Log("CheckpointT");
            //CopyMaterialProperties();
            Debug.Log("CheckpointS");
            m_IsLoadingModel = false;
            m_ReadyForEditing = true;
            Debug.Log("CheckpointR");
        }
        public string GetResourceName()
        {
            return Path.GetFileNameWithoutExtension(m_Filename) + m_TextureFilenameBase;
        }

        private static Dictionary<string, Texture2D> m_TextureDict;
        protected static Dictionary<string, Texture2D> TextureDict
        {
            get
            {
                if (m_TextureDict == null)
                    m_TextureDict = new Dictionary<string, Texture2D>();
                return m_TextureDict;
            }
        }

        public void ImportTextures(Task<GameObject> modelLoad, Action<GameObject> callback = null)
        {
            this.m_TextureTask = LoadTextures(this, modelLoad, null, textureTypes, this.m_Path, m_TextureFilenameBase, false, this.textureAnisoLevel, true, callback);
        }

        //private static Dictionary<string, Task<GameObject>> m_GameObjectTaskDict;

        public static Task LoadTextures(TransitImportAssetModel instance, Task<GameObject> modelLoad, GameObject model, AssetImporterTextureLoader.ResultType[] results, string path, string textureFileNameBase, bool lod, int anisoLevel = 1, bool generateDummy = false, Action<GameObject> callback = null)
        {
            CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
            {
                "***Creating Texture processing Thread  [",
                Thread.CurrentThread.Name,
                Thread.CurrentThread.ManagedThreadId,
                "]"
            }));
            Task taskResult = ThreadHelper.taskDistributor.Dispatch(delegate
            {
                try
                {
                    CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
                    {
                        "******Loading Textures [",
                        Thread.CurrentThread.Name,
                        Thread.CurrentThread.ManagedThreadId,
                        "]"
                    }));
                    string baseName = Path.Combine(path, textureFileNameBase) + ((!lod) ? string.Empty : "_lod");
                    bool[] array = new bool[8];
                    for (int i = 0; i < results.Length; i++)
                    {
                        int num = (int)results[i];
                        for (int j = 0; j < NeededSources[num].Count; j++)
                        {
                            array[(int)NeededSources[num][j]] = true;
                        }
                    }
                    Task<Image>[] array2 = new Task<Image>[array.Length];
                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k])
                        {
                            array2[k] = CallLoadTexture((AssetImporterTextureLoader.SourceType)k, baseName);
                        }
                    }
                    
                    array2.WaitAll();
                    if (modelLoad != null)
                    {
                        modelLoad.Wait();
                        model = modelLoad.result;
                        instance.GenerateAssetData2(model);
                    }
                    CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
                    {
                        "******Finished loading Textures  [",
                        Thread.CurrentThread.Name,
                        Thread.CurrentThread.ManagedThreadId,
                        "]"
                    }));
                    Image[] array3 = ExtractImages(array2);
                    int width = 0;
                    int height = 0;
                    for (int l = 0; l < array3.Length; l++)
                    {
                        if (array3[l] != null && array3[l].width > 0 && array3[l].height > 0)
                        {
                            width = array3[l].width;
                            height = array3[l].height;
                            break;
                        }
                    }
                    for (int m = 0; m < results.Length; m++)
                    {
                        Debug.Log("CheckpointA");
                        AssetImporterTextureLoader.ResultType result = results[m];
                        Debug.Log("CheckpointB");
                        string sampler = ResultSamplers[(int)result];
                        Debug.Log("CheckpointC");
                        Color def = ResultDefaults[(int)result];
                        Debug.Log("CheckpointD");
                        bool resultLinear = ResultLinear[sampler];
                        Debug.Log("CheckpointE");
                        string textureKey = baseName + sampler;
                        Debug.Log("CheckpointF");
                        Color[] texData = CallBuildTexture(array3, results[m], width, height, !lod);
                        ThreadHelper.dispatcher.Dispatch(delegate
                        {
                            Texture2D texture2D;
                            if (TextureDict.ContainsKey(textureKey))
                            {
                                Debug.Log(sampler + " was cached");
                                texture2D = TextureDict[textureKey];
                            }
                            else
                            {

                                if (texData != null)
                                {
                                    Debug.Log(sampler + " has texdata");
                                    texture2D = new Texture2D(width, height, sampler.Equals("_XYCAMap") ? TextureFormat.RGBA32 : TextureFormat.RGB24, false, resultLinear);
                                    texture2D.SetPixels(texData);
                                    texture2D.anisoLevel = anisoLevel;
                                    texture2D.Apply();
                                }
                                else if (generateDummy && width > 0 && height > 0)
                                {
                                    Debug.Log(sampler + " hasn't texdata");
                                    texture2D = new Texture2D(width, height, TextureFormat.RGB24, false, ResultLinear[sampler]);
                                    if (result == AssetImporterTextureLoader.ResultType.XYS)
                                    {
                                        def.b = 1f - def.b;
                                    }
                                    else if (result == AssetImporterTextureLoader.ResultType.APR)
                                    {
                                        def.r = 1f - def.r;
                                        def.g = 1f - def.g;
                                    }
                                    for (int n = 0; n < height; n++)
                                    {
                                        for (int num2 = 0; num2 < width; num2++)
                                        {
                                            texture2D.SetPixel(num2, n, def);
                                        }
                                    }
                                    texture2D.anisoLevel = anisoLevel;
                                    texture2D.Apply();
                                }
                                else
                                {
                                    Debug.Log(sampler + " is null");
                                    texture2D = null;
                                }
                                TextureDict.Add(textureKey, texture2D);
                            }
                            Debug.Log("CheckpointG");
                            AssetImporterTextureLoader.ApplyTexture(model, sampler, texture2D);
                        });
                    }
                    CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
                    {
                        "******Finished applying Textures  [",
                        Thread.CurrentThread.Name,
                        Thread.CurrentThread.ManagedThreadId,
                        "]"
                    }));
                }
                catch (Exception ex)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.AssetImporter, string.Concat(new object[]
                    {
                        ex.GetType(),
                        " ",
                        ex.Message,
                        " ",
                        ex.StackTrace
                    }));
                }
            });
            CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
            {
                "***Created Texture processing Thread [",
                Thread.CurrentThread.Name,
                Thread.CurrentThread.ManagedThreadId,
                "]"
            }));
            return taskResult;
        }

        protected static Image[] ExtractImages(Task<Image>[] tasks)
        {
            Image[] array = new Image[tasks.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i] != null)
                {
                    array[i] = tasks[i].result;
                }
            }
            return array;
        }

        protected static TaskDistributor sTaskDistributor = new TaskDistributor("SubTaskDistributor");
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
            Debug.Log(m_Filename + " Checkpoint1");
            this.FinalizeLOD();
            Debug.Log(m_Filename + " Checkpoint2");
            if (m_Object == null)
            {
                Debug.Log("m_Object is null");
            }
            if (this.m_Object.GetComponent<Renderer>() != null)
            {
                Debug.Log(m_Filename + " Checkpoint3");
                this.CompressTextures();
                Debug.Log(m_Filename + " Checkpoint4");
            }
            this.m_TaskWrapper = new MultiAsyncTaskWrapper(this.m_TaskNames, this.m_Tasks);
            Debug.Log(m_Filename + " Checkpoint5");
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
                Debug.Log("m_Object nulled " + GetResourceName());
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
    public enum ResourceType
    {
        Road
    }
}
