using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ObjUnity3D
{
    public class OBJLoader
    {
        private static OBJData m_OBJData = null;

        private static OBJMaterial m_CurrentMaterial = null;

        private static OBJGroup m_CurrentGroup = null;

        private static readonly Dictionary<string, Action<string>> m_ParseOBJActionDictionary;

        private static readonly Dictionary<string, Action<string>> m_ParseMTLActionDictionary;

        public static OBJData LoadOBJ(Stream lStream)
        {
            OBJLoader.m_OBJData = new OBJData();
            OBJLoader.m_CurrentMaterial = null;
            OBJLoader.m_CurrentGroup = null;
            StreamReader streamReader = new StreamReader(lStream);
            Action<string> action = null;
            while (!streamReader.EndOfStream)
            {
                string text = streamReader.ReadLine();
                if (!StringExt.IsNullOrWhiteSpace(text) && text[0] != '#')
                {
                    string[] array = text.Trim().Split(null, 2);
                    if (array.Length >= 2)
                    {
                        string text2 = array[0].Trim();
                        string obj = array[1].Trim();
                        action = null;
                        OBJLoader.m_ParseOBJActionDictionary.TryGetValue(text2.ToLowerInvariant(), out action);
                        if (action != null)
                        {
                            action(obj);
                        }
                    }
                }
            }
            OBJData oBJData = OBJLoader.m_OBJData;
            OBJLoader.m_OBJData = null;
            return oBJData;
        }

        public static void ExportOBJ(OBJData lData, Stream lStream)
        {
            StreamWriter streamWriter = new StreamWriter(lStream);
            streamWriter.WriteLine(string.Format("# File exported by Unity3D version {0}", Application.unityVersion));
            for (int i = 0; i < lData.m_Vertices.Count; i++)
            {
                TextWriter arg_88_0 = streamWriter;
                string arg_83_0 = "v {0} {1} {2}";
                float x = lData.m_Vertices[i].x;
                object arg_83_1 = x.ToString("n8");
                float y = lData.m_Vertices[i].y;
                object arg_83_2 = y.ToString("n8");
                float z = lData.m_Vertices[i].z;
                arg_88_0.WriteLine(string.Format(arg_83_0, arg_83_1, arg_83_2, z.ToString("n8")));
            }
            for (int j = 0; j < lData.m_UVs.Count; j++)
            {
                TextWriter arg_EC_0 = streamWriter;
                string arg_E7_0 = "vt {0} {1}";
                float x2 = lData.m_UVs[j].x;
                object arg_E7_1 = x2.ToString("n5");
                float y2 = lData.m_UVs[j].y;
                arg_EC_0.WriteLine(string.Format(arg_E7_0, arg_E7_1, y2.ToString("n5")));
            }
            for (int k = 0; k < lData.m_UV2s.Count; k++)
            {
                TextWriter arg_150_0 = streamWriter;
                string arg_14B_0 = "vt2 {0} {1}";
                float x3 = lData.m_UVs[k].x;
                object arg_14B_1 = x3.ToString("n5");
                float y3 = lData.m_UVs[k].y;
                arg_150_0.WriteLine(string.Format(arg_14B_0, arg_14B_1, y3.ToString("n5")));
            }
            for (int l = 0; l < lData.m_Normals.Count; l++)
            {
                TextWriter arg_1D7_0 = streamWriter;
                string arg_1D2_0 = "vn {0} {1} {2}";
                float x4 = lData.m_Normals[l].x;
                object arg_1D2_1 = x4.ToString("n8");
                float y4 = lData.m_Normals[l].y;
                object arg_1D2_2 = y4.ToString("n8");
                float z2 = lData.m_Normals[l].z;
                arg_1D7_0.WriteLine(string.Format(arg_1D2_0, arg_1D2_1, arg_1D2_2, z2.ToString("n8")));
            }
            for (int m = 0; m < lData.m_Colors.Count; m++)
            {
                TextWriter arg_2A1_0 = streamWriter;
                string arg_29C_0 = "vc {0} {1} {2} {3}";
                object[] array = new object[4];
                object[] arg_22D_0 = array;
                int arg_22D_1 = 0;
                float r = lData.m_Colors[m].r;
                arg_22D_0[arg_22D_1] = r.ToString("n8");
                object[] arg_251_0 = array;
                int arg_251_1 = 1;
                float g = lData.m_Colors[m].g;
                arg_251_0[arg_251_1] = g.ToString("n8");
                object[] arg_275_0 = array;
                int arg_275_1 = 2;
                float b = lData.m_Colors[m].b;
                arg_275_0[arg_275_1] = b.ToString("n8");
                object[] arg_299_0 = array;
                int arg_299_1 = 3;
                float a = lData.m_Colors[m].a;
                arg_299_0[arg_299_1] = a.ToString("n8");
                arg_2A1_0.WriteLine(string.Format(arg_29C_0, array));
            }
            for (int n = 0; n < lData.m_Groups.Count; n++)
            {
                streamWriter.WriteLine(string.Format("g {0}", lData.m_Groups[n].m_Name));
                for (int num = 0; num < lData.m_Groups[n].Faces.Count; num++)
                {
                    streamWriter.WriteLine(string.Format("f {0} {1} {2}", lData.m_Groups[n].Faces[num].ToString(0), lData.m_Groups[n].Faces[num].ToString(1), lData.m_Groups[n].Faces[num].ToString(2)));
                }
            }
            streamWriter.Flush();
        }

        private static void PushOBJMaterial(string lMaterialName)
        {
            OBJLoader.m_CurrentMaterial = new OBJMaterial(lMaterialName);
            OBJLoader.m_OBJData.m_Materials.Add(OBJLoader.m_CurrentMaterial);
        }

        private static void PushOBJGroup(string lGroupName)
        {
            OBJLoader.m_CurrentGroup = new OBJGroup(lGroupName);
            OBJLoader.m_OBJData.m_Groups.Add(OBJLoader.m_CurrentGroup);
        }

        private static void PushOBJGroupIfNeeded()
        {
            if (OBJLoader.m_CurrentGroup == null)
            {
                OBJLoader.PushOBJGroup("default");
            }
        }

        private static void PushOBJFace(string lFaceLine)
        {
            OBJLoader.PushOBJGroupIfNeeded();
            string[] array = lFaceLine.Split(new char[]
            {
                ' '
            }, StringSplitOptions.RemoveEmptyEntries);
            OBJFace oBJFace = new OBJFace();
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string lVertexString = array2[i];
                oBJFace.ParseVertex(lVertexString);
            }
            OBJLoader.m_CurrentGroup.AddFace(oBJFace);
        }

        static OBJLoader()
        {
            // Note: this type is marked as 'beforefieldinit'.
            Dictionary<string, Action<string>> dictionary = new Dictionary<string, Action<string>>();
            dictionary.Add("mtllib", delegate(string lEntry)
            {
            });
            dictionary.Add("usemtl", delegate(string lEntry)
            {
                OBJLoader.PushOBJGroupIfNeeded();
                OBJLoader.m_CurrentGroup.m_Material = OBJLoader.m_OBJData.m_Materials.SingleOrDefault((OBJMaterial lX) => lX.m_Name.EqualsInvariantCultureIgnoreCase(lEntry));
            });
            dictionary.Add("v", delegate(string lEntry)
            {
                OBJLoader.m_OBJData.m_Vertices.Add(Utils.ParseVector3String(lEntry, ' '));
            });
            dictionary.Add("vn", delegate(string lEntry)
            {
                OBJLoader.m_OBJData.m_Normals.Add(Utils.ParseVector3String(lEntry, ' '));
            });
            dictionary.Add("vt", delegate(string lEntry)
            {
                OBJLoader.m_OBJData.m_UVs.Add(Utils.ParseVector2String(lEntry, ' '));
            });
            dictionary.Add("vt2", delegate(string lEntry)
            {
                OBJLoader.m_OBJData.m_UV2s.Add(Utils.ParseVector2String(lEntry, ' '));
            });
            dictionary.Add("vc", delegate(string lEntry)
            {
                OBJLoader.m_OBJData.m_Colors.Add(Utils.ParseVector4String(lEntry, ' ').ToColor());
            });
            dictionary.Add("f", new Action<string>(OBJLoader.PushOBJFace));
            dictionary.Add("g", new Action<string>(OBJLoader.PushOBJGroup));
            OBJLoader.m_ParseOBJActionDictionary = dictionary;
            Dictionary<string, Action<string>> dictionary2 = new Dictionary<string, Action<string>>();
            dictionary2.Add("newmtl", new Action<string>(OBJLoader.PushOBJMaterial));
            dictionary2.Add("Ka", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_AmbientColor = Utils.ParseVector3String(lEntry, ' ').ToColor();
            });
            dictionary2.Add("Kd", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_DiffuseColor = Utils.ParseVector3String(lEntry, ' ').ToColor();
            });
            dictionary2.Add("Ks", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_SpecularColor = Utils.ParseVector3String(lEntry, ' ').ToColor();
            });
            dictionary2.Add("Ns", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_SpecularCoefficient = lEntry.ParseInvariantFloat();
            });
            dictionary2.Add("d", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_Transparency = lEntry.ParseInvariantFloat();
            });
            dictionary2.Add("Tr", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_Transparency = lEntry.ParseInvariantFloat();
            });
            dictionary2.Add("illum", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_IlluminationModel = lEntry.ParseInvariantInt();
            });
            dictionary2.Add("map_Ka", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_AmbientTextureMap = lEntry;
            });
            dictionary2.Add("map_Kd", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_DiffuseTextureMap = lEntry;
            });
            dictionary2.Add("map_Ks", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_SpecularTextureMap = lEntry;
            });
            dictionary2.Add("map_Ns", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_SpecularHighlightTextureMap = lEntry;
            });
            dictionary2.Add("map_d", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_AlphaTextureMap = lEntry;
            });
            dictionary2.Add("map_bump", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_BumpMap = lEntry;
            });
            dictionary2.Add("bump", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_BumpMap = lEntry;
            });
            dictionary2.Add("disp", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_DisplacementMap = lEntry;
            });
            dictionary2.Add("decal", delegate(string lEntry)
            {
                OBJLoader.m_CurrentMaterial.m_StencilDecalMap = lEntry;
            });
            OBJLoader.m_ParseMTLActionDictionary = dictionary2;
        }
    }
}
