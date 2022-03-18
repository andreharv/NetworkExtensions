using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NetworkExtensions2.Framework.Import
{
    internal static class ImportAllAssets
    {
        private static bool m_ImportFinished;
        private static string m_ResourcePath;
        private static string[] m_ModelFilenames;
        private static List<string> m_TextureFilenameBases;
        private static int m_FilenamesLength;
        private static Dictionary<string,ImportTransitAsset> m_ImportTransitAssetDict;
        private static Shader m_DefaultShader;
        public static Shader DefaultShader
        {
            get
            {
                if (m_DefaultShader == null)
                    m_DefaultShader = Shader.Find("Custom/Net/Road");
                return m_DefaultShader;
            }
        }
        private static Dictionary<string, ResourceUnit> m_NewInfoResources;
        public static Dictionary<string, ResourceUnit> NewInfoResources
        {
            get
            {
                if (m_NewInfoResources == null)
                    m_NewInfoResources = new Dictionary<string, ResourceUnit>();
                return m_NewInfoResources;
            }
            set { m_NewInfoResources = value; }
        }
        public static bool UptakeImportFiles(string path, AssetType type)
        {
            Debug.Log("Checkpoint6");
            if (string.IsNullOrEmpty(m_ResourcePath))
                m_ResourcePath = Path.Combine(path, GetAssetPath(type));
            if (m_ImportTransitAssetDict != null || Directory.Exists(m_ResourcePath))
            {
                Debug.Log("Checkpoint7");


                if (m_ModelFilenames == null)
                {
                    m_ModelFilenames = Directory.GetFiles(m_ResourcePath, "*.fbx");
                    m_FilenamesLength = m_ModelFilenames.Length;
                }
                if (m_TextureFilenameBases == null)
                {
                    m_TextureFilenameBases = new List<string>();
                    var textureFilenames = Directory.GetFiles(m_ResourcePath, "*.png");
                    for (int i = 0; i < textureFilenames.Length; i++)
                    {
                        var textureFilename = Path.GetFileNameWithoutExtension(textureFilenames[i]);
                        var textureFilenameBase = textureFilename.Substring(0, textureFilename.LastIndexOf("_"));
                        if (!m_TextureFilenameBases.Contains(textureFilenameBase))
                        {
                            m_TextureFilenameBases.Add(textureFilenameBase);
                        }
                    }
                }
                if (m_FilenamesLength > 0)
                {
                    Debug.Log("Checkpoint8");
                    if (m_ImportTransitAssetDict == null)
                    {
                        Debug.Log("Checkpoint9 filenamelength = " + m_FilenamesLength);
                        m_ImportTransitAssetDict = new Dictionary<string, ImportTransitAsset>();
                    }
                    for (int i = 0; i < m_FilenamesLength; i++)
                    {
                        var filename = m_ModelFilenames[i];
                        var filenameTrimmed = Path.GetFileNameWithoutExtension(filename);
                        var filenameBase = filenameTrimmed.Substring(0, filenameTrimmed.LastIndexOf("_"));
                        Debug.Log("Checkpoint9_" + i + " filenamebase is " + filenameBase);
                        for (int j = 0; j < m_TextureFilenameBases.Count; j++)
                        {
                            var textureFilenameBase = m_TextureFilenameBases[j];
                            Debug.Log("Checkpoint9_" + i + "_" + j);
                            if (textureFilenameBase.StartsWith(filenameBase + "_"))
                            {
                                Debug.Log("Checkpoint10");
                                var combinedKey = filenameTrimmed + textureFilenameBase;
                                if (!m_ImportTransitAssetDict.ContainsKey(combinedKey))
                                {
                                    Debug.Log("Checkpoint11");
                                    var importTransitAsset = new ImportTransitAsset();
                                    importTransitAsset.ImportCurrentAsset(Shader.Find("Custom/Net/Road"), m_ResourcePath, filename, textureFilenameBase);
                                    m_ImportTransitAssetDict.Add(combinedKey, importTransitAsset);
                                }
                                else
                                {
                                    var importTransitAsset = m_ImportTransitAssetDict[combinedKey];
                                    if (importTransitAsset.ResourceUnit == null)
                                    {
                                        Debug.Log("Checkpoint12");
                                        importTransitAsset.Update();
                                    }
                                    else if (!NewInfoResources.ContainsKey(combinedKey))
                                    {
                                        NewInfoResources.Add(combinedKey, importTransitAsset.ResourceUnit);
                                    }
                                }
                            }

                        }
                    }
                    if (NewInfoResources.Count == m_ImportTransitAssetDict.Count)
                    {
                        Debug.Log("Checkpoint13");
                        Debug.Log("newInfoResources Count:" + NewInfoResources.Count);
                        if (NewInfoResources.Count > 0)
                        {
                            var hi = NewInfoResources.First().Value;
                            Debug.Log("Mesh exists " + (hi.Mesh != null));
                            Debug.Log("lodMesh exists " + (hi.LodMesh != null));
                            Debug.Log("Material exists " + (hi.Material != null));
                            Debug.Log("lodMaterial exists " + (hi.LodMaterial != null));
                        }
                        return true;
                    }
                }
            }
            else
            {
                Debug.LogError("The specified path: " + m_ResourcePath + " does not exist");
            }
            return false;
        }
        public static Mesh GetMesh(string resourcePath)
        {
            if (!NewInfoResources.ContainsKey(resourcePath))
            {
                throw new Exception(String.Format("TFW: Mesh {0} not found", resourcePath));
            }
            return NewInfoResources[resourcePath].Mesh;
        }

        public static Material GetMaterial(string resourcePath)
        {

            if (!NewInfoResources.ContainsKey(resourcePath))
            {
                throw new Exception(String.Format("TFW: Mesh {0} not found", resourcePath));
            }
            return NewInfoResources[resourcePath].Material;
        }
        private static string GetAssetPath(AssetType type)
        {
            return $@"Resources\{type}\";
        }
    }
}
