using ColossalFramework;
using ColossalFramework.Importers;
using ColossalFramework.Threading;
using NetworkExtensions2.Framework._Extensions;
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
    public class NEXTImportAssetTextureLoader : AssetImporterTextureLoader
    {
        public static Task ProcessTextures(Task<GameObject> gameObjectLoader, AssetImporterTextureLoader.ResultType[] results, string textureName, bool lod, NEXTImportAssetModel.TextureLoaderModelCallback callback, int anisoLevel = 1, bool generateDummy = false)
        {
            CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
            {
                    "***Creating Texture processing Thread  [",
                    Thread.CurrentThread.Name,
                    Thread.CurrentThread.ManagedThreadId,
                    "]"
            }));

            Task textureResult = ThreadHelper.taskDistributor.Dispatch(delegate
            {
                try
                {
                    var textureBasename = NEXTImportAssetModel.GetResourceTextureName(textureName, false);
                    var trimmedTextureBaseName = NEXTImportAssetModel.GetResourceTextureName(textureName);
                    CODebugBase<LogChannel>.VerboseLog(LogChannel.AssetImporter, string.Concat(new object[]
                    {
                            "******Loading Textures for " + textureBasename + "[",
                            Thread.CurrentThread.Name,
                            Thread.CurrentThread.ManagedThreadId,
                            "]"
                    }));
                    string baseName = textureBasename + ((!lod) ? string.Empty : "_lod");
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
                            array2[k] = LoadTexture((SourceType)k, baseName);
                        }
                    }
                    array2.WaitAll();

                    if (!gameObjectLoader.hasEnded)
                    {
                        gameObjectLoader.Wait();
                        Threading.SetAre(Threading.AreType.Model);
                    }
                    GameObject model = callback(gameObjectLoader.result);

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

                    var texture2dTasks = new Task[results.Length];
                    for (int m = 0; m < results.Length; m++)
                    {
                        Color[] texData = BuildTexture(array3, results[m], width, height, !lod);
                        ResultType result = results[m];
                        string sampler = ResultSamplers[(int)result];
                        Color def = ResultDefaults[(int)result];
                        bool resultLinear = ResultLinear[sampler];
                        texture2dTasks[m] = ThreadHelper.dispatcher.Dispatch(delegate
                        {
                            Texture2D texture2D;
                            if (texData != null)
                            {
                                texture2D = new Texture2D(width, height, sampler.Equals("_XYCAMap") ? TextureFormat.RGBA32 : TextureFormat.RGB24, false, resultLinear);
                                texture2D.SetPixels(texData);
                                texture2D.anisoLevel = anisoLevel;
                                texture2D.Apply();
                            }
                            else if (generateDummy && width > 0 && height > 0)
                            {
                                texture2D = new Texture2D(width, height, TextureFormat.RGB24, false, ResultLinear[sampler]);
                                if (result == ResultType.XYS)
                                {
                                    def.b = 1f - def.b;
                                }
                                else if (result == ResultType.APR)
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
                                texture2D = null;
                            }
                            Debug.Log("Sampler " + sampler + " is null " + (texture2D == null));
                            ApplyTexture(model, sampler, texture2D);
                        });
                    }
                    texture2dTasks.WaitAll();
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
            return textureResult;
        }
    }
}
