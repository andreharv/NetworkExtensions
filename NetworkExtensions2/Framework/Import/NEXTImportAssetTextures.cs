using ColossalFramework;
using ColossalFramework.Importers;
using ColossalFramework.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

namespace NetworkExtensions2.Framework.Import
{
    public class NEXTImportAssetTextures
    {
        private string m_BaseTextureName;
        public NEXTImportAssetTextures(string argBaseTextureName)
        {
            m_BaseTextureName = argBaseTextureName;
        }
        public Task LoadTextures(Task<GameObject> modelLoad, GameObject model, AssetImporterTextureLoader.ResultType[] results, string path, string modelName, bool lod, int anisoLevel = 1, bool generateDummy = false, bool generatePadding = false)
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
                    Image[] array3 = CallExtractImages(array2);
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
    }

}
