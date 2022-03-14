using ColossalFramework;
using ColossalFramework.Importers;
using ColossalFramework.Threading;
using Newtonsoft.Json;
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
        private Shader m_TemplateShader;

        //protected static Dictionary<AssetImporterTextureLoader.ResultType, string[]> m_NeedSources = new Dictionary<AssetImporterTextureLoader.ResultType, string[]>
        //{
        //    { AssetImporterTextureLoader.ResultType.RGB, new string[]{"_d"} },
        //    { AssetImporterTextureLoader.ResultType.XYS, new string[]{"_n", "_s" } },
        //    { AssetImporterTextureLoader.ResultType.ACI, new string[]{"_a", "_c", "_i" } },
        //    { AssetImporterTextureLoader.ResultType.XYCA, new string[]{"_n", "_c", "_a" } },
        //    { AssetImporterTextureLoader.ResultType.APR, new string[]{"_a", "_p", "_r"} }
        //};

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

        // AssetImporterTextureLoader
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

        public override void Import(string path, string filename)
        {
            if (this.m_Object != null)
            {
                this.DestroyAsset();
                this.m_Object = null;
            }
            this.m_Filename = filename;
            this.m_Path = path;
            GetTextureBaseNames(path);
            this.m_IsLoadingModel = true;
            this.m_IsImportedAsset = true;
            this.m_Importer = new SceneImporter();
            this.CreateLODObject();
            this.m_Importer.filePath = Path.Combine(path, filename);
            this.m_Importer.importSkinMesh = true;
            if (m_GameObjectTaskDict == null)
                m_GameObjectTaskDict = new Dictionary<string, Task<GameObject>>();
            if (m_GameObjectTaskDict.ContainsKey(filename))
            {
                GenerateAssetData(m_GameObjectTaskDict[filename].result);
            }
            else
            {
                m_GameObjectTaskDict[filename] = m_Importer.ImportAsync(new Action<GameObject>(this.GenerateAssetData));
            }
            ImportTextures(m_GameObjectTaskDict[filename]);
        }
        private static string[] m_TextureBaseNames;
        private static void GetTextureBaseNames(string path)
        {
            var pngFiles = Directory.GetFiles(path, "*.png");
            var textureBaseNames = new List<string>();
            foreach (var pngFile in pngFiles)
            {
                if (!pngFile.Contains("_"))
                {
                    Debug.LogError("png file " + pngFile + " not of the proper format and will be omitted");
                    continue;
                }
                var pngFileTrimmed = pngFile.Substring(0, pngFile.LastIndexOf("_"));
                if (textureBaseNames.IndexOf(pngFileTrimmed) == -1)
                {
                    textureBaseNames.Add(pngFileTrimmed);
                }
            }
            m_TextureBaseNames = textureBaseNames.ToArray();
            Debug.Log("textureBaseNames: " + m_TextureBaseNames.Length);
        }

        public void ImportTextures(Task<GameObject> modelLoad)
        {
            this.m_TextureTask = LoadTextures(modelLoad, null, textureTypes, this.m_Path, this.m_Filename, false, this.textureAnisoLevel, true, false);
        }

        private static Dictionary<string, Task<GameObject>> m_GameObjectTaskDict;
        public static Task LoadTextures(Task<GameObject> modelLoad, GameObject model, AssetImporterTextureLoader.ResultType[] results, string path, string modelName, bool lod, int anisoLevel = 1, bool generateDummy = false, bool generatePadding = false)
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
                    string baseName = Path.Combine(path, Path.GetFileNameWithoutExtension(modelName)) + ((!lod) ? string.Empty : "_lod");
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
                        Color[] texData = CallBuildTexture(array3, results[m], width, height, !lod);
                        AssetImporterTextureLoader.ResultType result = results[m];
                        string sampler = ResultSamplers[(int)result];
                        Color def = ResultDefaults[(int)result];
                        bool resultLinear = ResultLinear[sampler];
                        ThreadHelper.dispatcher.Dispatch(delegate
                        {
                            Texture2D texture2D;
                            if (texData != null)
                            {
                                if (sampler.Equals("_APRMap"))
                                {
                                    Debug.Log("APR has texdata");
                                }
                                texture2D = new Texture2D(width, height, sampler.Equals("_XYCAMap") ? TextureFormat.RGBA32 : TextureFormat.RGB24, false, resultLinear);
                                texture2D.SetPixels(texData);
                                texture2D.anisoLevel = anisoLevel;
                                texture2D.Apply();
                            }
                            else if (generateDummy && width > 0 && height > 0)
                            {
                                if (sampler.Equals("_APRMap"))
                                {
                                    Debug.Log("APR hasn't texdata");
                                }
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
                                if (sampler.Equals("_APRMap"))
                                {
                                    Debug.Log("APR is null");
                                }
                                texture2D = null;
                            }
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

        //protected static Task LoadTextures1(Task<GameObject> modelLoad, string path, string filename, bool lod, int textureAnisoLevel, AssetImporterTextureLoader.ResultType[] argTextureTypes)
        //{
        //    CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
        //    {
        //            "***Creating Texture processing Thread  [",
        //            Thread.CurrentThread.Name,
        //            Thread.CurrentThread.ManagedThreadId,
        //            "]"
        //    }));
        //    Task result = ThreadHelper.taskDistributor.Dispatch(delegate
        //    {
        //        try
        //        {
        //            CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
        //            {
        //            "******Loading Textures [",
        //            Thread.CurrentThread.Name,
        //            Thread.CurrentThread.ManagedThreadId,
        //            "]"
        //            }));
        //            var filenameWithoutExtension = filename.Substring(filename.LastIndexOf("\\") + 1, filename.LastIndexOf(".") - filename.LastIndexOf("\\") - 1);
        //            Debug.Log("filenamewithoutextension " + filenameWithoutExtension);
        //            var modelNameValue = filenameWithoutExtension.Split('_');
        //            if (modelNameValue.Length > 1)
        //            {
        //                var resourceType = modelNameValue[0];
        //                Debug.Log("parts " + modelNameValue[0] + " & " + modelNameValue[1]);
        //                var num = -1;
        //                var canParse = int.TryParse(modelNameValue[1], out num);
        //                Debug.Log("trying to parse " + modelNameValue[1] + " to int. Passed? " + canParse + ". The result is " + num);
        //                var size = int.Parse(modelNameValue[1]);
        //                var textureRequiredSignatures = new List<string>();
        //                for (int i = 0; i < argTextureTypes.Length; i++)
        //                {
        //                    textureRequiredSignatures.AddRange(m_NeedSources[argTextureTypes[i]]);
        //                }
        //                if (textureRequiredSignatures.Count > 0)
        //                {
        //                    if (m_TextureTaskDict == null)
        //                        m_TextureTaskDict = new Dictionary<string, Task<Image>>();
        //                    for (int i = 0; i < m_TextureBaseNames.Length; i++)
        //                    {
        //                        for (int j = 0; j < textureRequiredSignatures.Count; j++)
        //                        {
        //                            var textureFile = Path.Combine(path, m_TextureBaseNames[i] + textureRequiredSignatures[j] + ".png");
        //                            if (!m_TextureTaskDict.ContainsKey(textureFile))
        //                            {
        //                                m_TextureTaskDict[textureFile] = LoadTexture(textureFile);
        //                            }
        //                        }
        //                    }
        //                    m_TextureTaskDict.Values.ToArray().WaitAll();
        //                }
        //            }
        //            modelLoad.Wait();
        //            var model = modelLoad.result;
        //            CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
        //            {
        //                "******Finished loading Textures  [",
        //                Thread.CurrentThread.Name,
        //                Thread.CurrentThread.ManagedThreadId,
        //                "]"
        //            }));
        //            Image[] images = ExtractImages(m_TextureTaskDict.Values.ToArray());
        //            int width = 0;
        //            int height = 0;
        //            for (int l = 0; l < images.Length; l++)
        //            {
        //                if (images[l] != null && images[l].width > 0 && images[l].height > 0)
        //                {
        //                    width = images[l].width;
        //                    height = images[l].height;
        //                    break;
        //                }
        //            }
        //            for (int m = 0; m < argTextureTypes.Length; m++)
        //            {
        //                Color[] texData = BuildTexture(images, argTextureTypes[m], width, height, !lod);
        //                AssetImporterTextureLoader.ResultType resultType = argTextureTypes[m];
        //                string sampler = ResultSamplers[(int)resultType];
        //                Color def = ResultDefaults[(int)resultType];
        //                bool resultLinear = ResultLinear[sampler];
        //                ThreadHelper.dispatcher.Dispatch(delegate
        //                {
        //                    Texture2D texture2D;
        //                    if (texData != null)
        //                    {
        //                        if (sampler.Equals("_XYCAMap"))
        //                        {
        //                            texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false, resultLinear);
        //                        }
        //                        else
        //                        {
        //                            texture2D = new Texture2D(width, height, TextureFormat.RGB24, false, resultLinear);
        //                        }
        //                        texture2D.SetPixels(texData);
        //                        texture2D.anisoLevel = textureAnisoLevel;
        //                        texture2D.Apply();
        //                    }
        //                    else if (width > 0 && height > 0)
        //                    {
        //                        texture2D = new Texture2D(width, height, TextureFormat.RGB24, false, resultLinear);
        //                        if (resultType == AssetImporterTextureLoader.ResultType.XYS)
        //                        {
        //                            def.b = 1f - def.b;
        //                        }
        //                        else if (resultType == AssetImporterTextureLoader.ResultType.APR)
        //                        {
        //                            def.r = 1f - def.r;
        //                            def.g = 1f - def.g;
        //                        }
        //                        for (int n = 0; n < height; n++)
        //                        {
        //                            for (int num2 = 0; num2 < width; num2++)
        //                            {
        //                                texture2D.SetPixel(num2, n, def);
        //                            }
        //                        }
        //                        texture2D.anisoLevel = textureAnisoLevel;
        //                        texture2D.Apply();
        //                    }
        //                    else
        //                    {
        //                        texture2D = null;
        //                    }

        //                    AssetImporterTextureLoader.ApplyTexture(model, sampler, texture2D);
        //                });
        //            }
        //            CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
        //            {
        //        "******Finished applying Textures  [",
        //        Thread.CurrentThread.Name,
        //        Thread.CurrentThread.ManagedThreadId,
        //        "]"
        //            }));
        //        }
        //        catch (Exception ex)
        //        {
        //            CODebugBase<LogChannel>.Error(LogChannel.AssetImporter, string.Concat(new object[]
        //            {
        //        ex.GetType(),
        //        " ",
        //        ex.Message,
        //        " ",
        //        ex.StackTrace
        //            }));
        //        }
        //    });
        //    CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
        //    {
        //"***Created Texture processing Thread [",
        //Thread.CurrentThread.Name,
        //Thread.CurrentThread.ManagedThreadId,
        //"]"
        //    }));
        //    return result;
        //}
        // AssetImporterTextureLoader
        //protected static void CombineChannels(Color[] output, Color[] r, Color[] g, Color[] b, Color[] a, bool ralpha, bool galpha, bool balpha, bool rinvert, bool ginvert, bool binvert, bool ainvert, Color defaultColor)
        //{
        //    for (int i = 0; i < output.Length; i++)
        //    {
        //        float num = (r == null) ? defaultColor.r : ((!ralpha) ? r[i].r : r[i].a);
        //        float num2 = (g == null) ? defaultColor.g : ((!galpha) ? g[i].g : g[i].a);
        //        float num3 = (b == null) ? defaultColor.b : ((!balpha) ? b[i].b : b[i].a);
        //        float num4 = (a == null) ? defaultColor.a : a[i].a;
        //        if (rinvert)
        //        {
        //            num = 1f - num;
        //        }
        //        if (ginvert)
        //        {
        //            num2 = 1f - num2;
        //        }
        //        if (ainvert)
        //        {
        //            num4 = 1f - num4;
        //        }
        //        if (binvert)
        //        {
        //            num3 = 1f - num3;
        //        }
        //        output[i] = new Color(num, num2, num3, num4);
        //    }
        //}
        //// AssetImporterTextureLoader
        //protected static bool CheckResolutions(int width, int height, params Image[] images)
        //{
        //    for (int i = 0; i < images.Length; i++)
        //    {
        //        if (images[i] != null && (images[i].width != width || images[i].height != height))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //protected static Color[] BuildTexture(Image[] sources, AssetImporterTextureLoader.ResultType type, int width, int height, bool nullAllowed)
        //{
        //    Debug.Log("Checkpointa");
        //    List<AssetImporterTextureLoader.SourceType> list = NeededSources[(int)type];
        //    Debug.Log("Checkpointb");
        //    Image[] array = new Image[list.Count];
        //    Debug.Log("Checkpointc count " + list.Count());
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        Debug.Log("Checkpointc_" + i + " sources[" + list[i] + "]");
        //        array[i] = sources[(int)list[i]];
        //    }
        //    Debug.Log("Checkpointd");
        //    if (nullAllowed)
        //    {
        //        Debug.Log("Checkpointe");
        //        bool flag = false;
        //        Debug.Log("Checkpointf");
        //        for (int j = 0; j < array.Length; j++)
        //        {
        //            if (array[j] != null)
        //            {
        //                flag = true;
        //            }
        //        }
        //        Debug.Log("Checkpointg");
        //        if (!flag)
        //        {
        //            return null;
        //        }
        //        Debug.Log("Checkpointh");
        //    }
        //    if (CheckResolutions(width, height, array))
        //    {
        //        Debug.Log("Checkpointi");
        //        Color[] array2 = new Color[width * height];
        //        switch (type)
        //        {
        //            case AssetImporterTextureLoader.ResultType.RGB:
        //                {
        //                    Debug.Log("In RGB");
        //                    Color[] array3 = (array[0] == null) ? null : array[0].GetColors();
        //                    Debug.Log("In RGB2 " + ResultDefaults.Length + " " + (int)type);
        //                    CombineChannels(array2, array3, array3, array3, null, false, false, false, false, false, false, false, ResultDefaults[(int)type]);
        //                    Debug.Log("In RGB3");
        //                    break;
        //                }
        //            case AssetImporterTextureLoader.ResultType.XYS:
        //                {
        //                    Debug.Log("In XYS");
        //                    Color[] array4 = (array[0] == null) ? null : array[0].GetColors();
        //                    Debug.Log("In XYS2");
        //                    if (array[1] != null)
        //                    {
        //                        array[1].Convert(TextureFormat.Alpha8);
        //                    }
        //                    Debug.Log("In XYS3");
        //                    Color[] b = (array[1] == null) ? null : array[1].GetColors();
        //                    Debug.Log("In XYS4");
        //                    CombineChannels(array2, array4, array4, b, null, false, false, true, false, false, true, false, ResultDefaults[(int)type]);
        //                    Debug.Log("In XYS5");
        //                    break;
        //                }
        //            case AssetImporterTextureLoader.ResultType.ACI:
        //                {
        //                    if (array[0] != null)
        //                    {
        //                        array[0].Convert(TextureFormat.Alpha8);
        //                    }
        //                    Color[] array5 = (array[0] == null) ? null : array[0].GetColors();
        //                    if (array[1] != null)
        //                    {
        //                        array[1].Convert(TextureFormat.Alpha8);
        //                    }
        //                    Color[] array6 = (array[1] == null) ? null : array[1].GetColors();
        //                    if (array[2] != null)
        //                    {
        //                        array[2].Convert(TextureFormat.Alpha8);
        //                    }
        //                    Color[] b2 = (array[2] == null) ? null : array[2].GetColors();
        //                    CombineChannels(array2, array5, array6, b2, null, true, true, true, true, true, false, false, ResultDefaults[(int)type]);
        //                    break;
        //                }
        //            case AssetImporterTextureLoader.ResultType.XYCA:
        //                {
        //                    Color[] array4 = (array[0] == null) ? null : array[0].GetColors();
        //                    if (array[1] != null)
        //                    {
        //                        array[1].Convert(TextureFormat.Alpha8);
        //                    }
        //                    Color[] array6 = (array[1] == null) ? null : array[1].GetColors();
        //                    if (array[2] != null)
        //                    {
        //                        array[2].Convert(TextureFormat.Alpha8);
        //                    }
        //                    Color[] array5 = (array[2] == null) ? null : array[2].GetColors();
        //                    CombineChannels(array2, array4, array4, array6, array5, false, false, true, false, false, true, true, ResultDefaults[(int)type]);
        //                    break;
        //                }
        //            case AssetImporterTextureLoader.ResultType.APR:
        //                {
        //                    Debug.Log("In APR");
        //                    if (array[0] != null)
        //                    {
        //                        array[0].Convert(TextureFormat.Alpha8);
        //                    }
        //                    Debug.Log("In APR2");
        //                    Color[] array5 = (array[0] == null) ? null : array[0].GetColors();
        //                    Debug.Log("In APR3");
        //                    if (array[1] != null)
        //                    {
        //                        array[1].Convert(TextureFormat.Alpha8);
        //                    }
        //                    Debug.Log("In APR4");
        //                    Color[] g = (array[1] == null) ? null : array[1].GetColors();
        //                    Debug.Log("In APR5");
        //                    if (array[2] != null)
        //                    {
        //                        array[2].Convert(TextureFormat.Alpha8);
        //                    }
        //                    Debug.Log("In APR6");
        //                    Color[] b3 = (array[2] == null) ? null : array[2].GetColors();
        //                    Debug.Log("In APR7");
        //                    CombineChannels(array2, array5, g, b3, null, true, true, true, true, true, false, false, ResultDefaults[(int)type]);
        //                    Debug.Log("In APR8");
        //                    break;
        //                }
        //        }
        //        CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
        //        {
        //    "******Finished loading & processing textures [",
        //    Thread.CurrentThread.Name,
        //    Thread.CurrentThread.ManagedThreadId,
        //    "]"
        //        }));
        //        return array2;
        //    }
        //    CODebugBase<LogChannel>.Error(LogChannel.AssetImporter, "Texture error: resolutions don't match or textures missing");
        //    return null;
        //}
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

        protected static Task<Image> LoadTexture(string textureFile)
        {
            if (!File.Exists(textureFile))
            {
                CODebugBase<LogChannel>.Warn(LogChannel.AssetImporter, "Texture missing");
                return null;
            }

            Task<Image> task = new Task<Image>(() =>
            {
                try
                {
                    CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
                    {
                        "*********Loading texture [",
                        Thread.CurrentThread.Name,
                        Thread.CurrentThread.ManagedThreadId,
                        "]"
                    }));
                    Image image = new Image(textureFile);
                    Image result;
                    if (image != null && image.width > 0 && image.height > 0)
                    {
                        CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
                        {
                            "*********Finished loading texture [",
                            Thread.CurrentThread.Name,
                            Thread.CurrentThread.ManagedThreadId,
                            "]"
                        }));
                        result = image;
                        return result;
                    }
                    CODebugBase<LogChannel>.Warn(LogChannel.AssetImporter, "Texture: resolution error or texture missing");
                    result = null;
                    return result;
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
                return null;
            });
            sTaskDistributor.Dispatch(task);
            return task;
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
            //this.FinalizeLOD();
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
    public enum ResourceType
    {
        Road
    }
}
