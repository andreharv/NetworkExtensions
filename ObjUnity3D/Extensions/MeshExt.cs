using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ObjUnity3D
{
    public static class MeshExt
    {
        internal const int MESH_BINARY_HEADER_SIZE = 20;

        internal const short MESH_BINARY_SIGNATURE = 245;

        internal const short MESH_BINARY_VERSION = 1;

        public static void RecalculateTangents(this Mesh lMesh)
        {
            int[] triangles = lMesh.triangles;
            Vector3[] vertices = lMesh.vertices;
            Vector2[] uv = lMesh.uv;
            if (uv.Length == 0)
            {
                return;
            }
            Vector3[] normals = lMesh.normals;
            int num = triangles.Length;
            int num2 = vertices.Length;
            Vector3[] array = new Vector3[num2];
            Vector3[] array2 = new Vector3[num2];
            Vector4[] array3 = new Vector4[num2];
            for (long num3 = 0L; num3 < (long)num; num3 += 3L)
            {
                long num4 = (long)triangles[(int)(checked((IntPtr)num3))];
                long num5 = (long)triangles[(int)(checked((IntPtr)(unchecked(num3 + 1L))))];
                long num6 = (long)triangles[(int)(checked((IntPtr)(unchecked(num3 + 2L))))];
                Vector3 vector;
                Vector3 vector2;
                Vector3 vector3;
                Vector2 vector4;
                Vector2 vector5;
                Vector2 vector6;
                checked
                {
                    vector = vertices[(int)((IntPtr)num4)];
                    vector2 = vertices[(int)((IntPtr)num5)];
                    vector3 = vertices[(int)((IntPtr)num6)];
                    vector4 = uv[(int)((IntPtr)num4)];
                    vector5 = uv[(int)((IntPtr)num5)];
                    vector6 = uv[(int)((IntPtr)num6)];
                }
                float num7 = vector2.x - vector.x;
                float num8 = vector3.x - vector.x;
                float num9 = vector2.y - vector.y;
                float num10 = vector3.y - vector.y;
                float num11 = vector2.z - vector.z;
                float num12 = vector3.z - vector.z;
                float num13 = vector5.x - vector4.x;
                float num14 = vector6.x - vector4.x;
                float num15 = vector5.y - vector4.y;
                float num16 = vector6.y - vector4.y;
                float num17 = 1f / (num13 * num16 - num14 * num15);
                Vector3 b = new Vector3((num16 * num7 - num15 * num8) * num17, (num16 * num9 - num15 * num10) * num17, (num16 * num11 - num15 * num12) * num17);
                Vector3 b2 = new Vector3((num13 * num8 - num14 * num7) * num17, (num13 * num10 - num14 * num9) * num17, (num13 * num12 - num14 * num11) * num17);
                checked
                {
                    array[(int)((IntPtr)num4)] += b;
                    array[(int)((IntPtr)num5)] += b;
                    array[(int)((IntPtr)num6)] += b;
                    array2[(int)((IntPtr)num4)] += b2;
                    array2[(int)((IntPtr)num5)] += b2;
                    array2[(int)((IntPtr)num6)] += b2;
                }
            }
            for (long num18 = 0L; num18 < (long)num2; num18 += 1L)
            {
                checked
                {
                    Vector3 lhs = normals[(int)((IntPtr)num18)];
                    Vector3 rhs = array[(int)((IntPtr)num18)];
                    Vector3.OrthoNormalize(ref lhs, ref rhs);
                    array3[(int)((IntPtr)num18)].x = rhs.x;
                    array3[(int)((IntPtr)num18)].y = rhs.y;
                    array3[(int)((IntPtr)num18)].z = rhs.z;
                    array3[(int)((IntPtr)num18)].w = ((Vector3.Dot(Vector3.Cross(lhs, rhs), array2[(int)((IntPtr)num18)]) < 0f) ? -1f : 1f);
                }
            }
            lMesh.tangents = array3;
        }

        public static void LoadOBJ(this Mesh lMesh, OBJData lData)
        {
            List<Vector3> list = new List<Vector3>();
            List<Vector3> list2 = new List<Vector3>();
            List<Vector2> list3 = new List<Vector2>();
            List<int>[] array = new List<int>[lData.m_Groups.Count];
            Dictionary<OBJFaceVertex, int> dictionary = new Dictionary<OBJFaceVertex, int>();
            bool flag = lData.m_Normals.Count > 0;
            bool flag2 = lData.m_UVs.Count > 0;
            lMesh.subMeshCount = lData.m_Groups.Count;
            for (int i = 0; i < lData.m_Groups.Count; i++)
            {
                OBJGroup oBJGroup = lData.m_Groups[i];
                array[i] = new List<int>();
                for (int j = 0; j < oBJGroup.Faces.Count; j++)
                {
                    OBJFace oBJFace = oBJGroup.Faces[j];
                    for (int k = 1; k < oBJFace.Count - 1; k++)
                    {
                        int[] array2 = new int[]
						{
							0,
							k,
							k + 1
						};
                        for (int l = 0; l < array2.Length; l++)
                        {
                            int i2 = array2[l];
                            OBJFaceVertex oBJFaceVertex = oBJFace[i2];
                            int item = -1;
                            if (!dictionary.TryGetValue(oBJFaceVertex, out item))
                            {
                                dictionary[oBJFaceVertex] = list.Count;
                                item = list.Count;
                                list.Add(lData.m_Vertices[oBJFaceVertex.m_VertexIndex]);
                                if (flag2)
                                {
                                    list3.Add(lData.m_UVs[oBJFaceVertex.m_UVIndex]);
                                }
                                if (flag)
                                {
                                    list2.Add(lData.m_Normals[oBJFaceVertex.m_NormalIndex]);
                                }
                            }
                            array[i].Add(item);
                        }
                    }
                }
            }
            lMesh.triangles = new int[0];
            lMesh.vertices = list.ToArray();
            lMesh.uv = list3.ToArray();
            lMesh.normals = list2.ToArray();
            if (!flag)
            {
                lMesh.RecalculateNormals();
            }
            lMesh.RecalculateTangents();
            for (int m = 0; m < lData.m_Groups.Count; m++)
            {
                lMesh.SetTriangles(array[m].ToArray(), m);
            }
        }

        public static void LoadOBJ(this Mesh lMesh, OBJData lData, string subOject)
        {
            List<Vector3> list = new List<Vector3>();
            List<Vector3> list2 = new List<Vector3>();
            List<Vector2> list3 = new List<Vector2>();
            List<int> list4 = new List<int>();
            Dictionary<OBJFaceVertex, int> dictionary = new Dictionary<OBJFaceVertex, int>();
            bool flag = lData.m_Normals.Count > 0;
            bool flag2 = lData.m_UVs.Count > 0;
            for (int i = 0; i < lData.m_Groups.Count; i++)
            {
                OBJGroup oBJGroup = lData.m_Groups[i];
                if (!(oBJGroup.m_Name != subOject))
                {
                    for (int j = 0; j < oBJGroup.Faces.Count; j++)
                    {
                        OBJFace oBJFace = oBJGroup.Faces[j];
                        for (int k = 1; k < oBJFace.Count - 1; k++)
                        {
                            int[] array = new int[]
							{
								0,
								k,
								k + 1
							};
                            for (int l = 0; l < array.Length; l++)
                            {
                                int i2 = array[l];
                                OBJFaceVertex oBJFaceVertex = oBJFace[i2];
                                int item = -1;
                                if (!dictionary.TryGetValue(oBJFaceVertex, out item))
                                {
                                    dictionary[oBJFaceVertex] = list.Count;
                                    item = list.Count;
                                    list.Add(lData.m_Vertices[oBJFaceVertex.m_VertexIndex]);
                                    if (flag2)
                                    {
                                        list3.Add(lData.m_UVs[oBJFaceVertex.m_UVIndex]);
                                    }
                                    if (flag)
                                    {
                                        list2.Add(lData.m_Normals[oBJFaceVertex.m_NormalIndex]);
                                    }
                                }
                                list4.Add(item);
                            }
                        }
                    }
                }
            }
            if (list4.Count == 0)
            {
                return;
            }
            lMesh.vertices = list.ToArray();
            lMesh.triangles = list4.ToArray();
            lMesh.uv = list3.ToArray();
            lMesh.normals = list2.ToArray();
            if (!flag)
            {
                lMesh.RecalculateNormals();
            }
            lMesh.RecalculateTangents();
        }

        public static OBJData EncodeOBJ(this Mesh lMesh)
        {
            OBJData oBJData = new OBJData
            {
                m_Vertices = new List<Vector3>(lMesh.vertices),
                m_UVs = new List<Vector2>(lMesh.uv),
                m_Normals = new List<Vector3>(lMesh.normals),
                m_UV2s = new List<Vector2>(lMesh.uv2),
                m_Colors = new List<Color>(lMesh.colors)
            };
            for (int i = 0; i < lMesh.subMeshCount; i++)
            {
                int[] triangles = lMesh.GetTriangles(i);
                OBJGroup oBJGroup = new OBJGroup(lMesh.name + "_" + i.ToString());
                for (int j = 0; j < triangles.Length; j += 3)
                {
                    OBJFace oBJFace = new OBJFace();
                    oBJFace.AddVertex(new OBJFaceVertex
                    {
                        m_VertexIndex = (oBJData.m_Vertices.Count > 0) ? triangles[j] : -1,
                        m_UVIndex = (oBJData.m_UVs.Count > 0) ? triangles[j] : -1,
                        m_NormalIndex = (oBJData.m_Normals.Count > 0) ? triangles[j] : -1,
                        m_UV2Index = (oBJData.m_UV2s.Count > 0) ? triangles[j] : -1,
                        m_ColorIndex = (oBJData.m_Colors.Count > 0) ? triangles[j] : -1
                    });
                    oBJFace.AddVertex(new OBJFaceVertex
                    {
                        m_VertexIndex = (oBJData.m_Vertices.Count > 0) ? triangles[j + 1] : -1,
                        m_UVIndex = (oBJData.m_UVs.Count > 0) ? triangles[j + 1] : -1,
                        m_NormalIndex = (oBJData.m_Normals.Count > 0) ? triangles[j + 1] : -1,
                        m_UV2Index = (oBJData.m_UV2s.Count > 0) ? triangles[j + 1] : -1,
                        m_ColorIndex = (oBJData.m_Colors.Count > 0) ? triangles[j + 1] : -1
                    });
                    oBJFace.AddVertex(new OBJFaceVertex
                    {
                        m_VertexIndex = (oBJData.m_Vertices.Count > 0) ? triangles[j + 2] : -1,
                        m_UVIndex = (oBJData.m_UVs.Count > 0) ? triangles[j + 2] : -1,
                        m_NormalIndex = (oBJData.m_Normals.Count > 0) ? triangles[j + 2] : -1,
                        m_UV2Index = (oBJData.m_UV2s.Count > 0) ? triangles[j + 2] : -1,
                        m_ColorIndex = (oBJData.m_Colors.Count > 0) ? triangles[j + 2] : -1
                    });
                    oBJGroup.AddFace(oBJFace);
                }
                oBJData.m_Groups.Add(oBJGroup);
            }
            return oBJData;
        }

        public static bool LoadBinary(this Mesh lMesh, byte[] lData)
        {
            int num = Marshal.SizeOf(typeof(Vector2));
            int num2 = Marshal.SizeOf(typeof(Vector3));
            int num3 = Marshal.SizeOf(typeof(Vector4));
            int num4 = Marshal.SizeOf(typeof(Matrix4x4));
            int num5 = Marshal.SizeOf(typeof(BoneWeight));
            int num6 = Marshal.SizeOf(typeof(Color));
            int num7 = 20;
            if (lData == null || lData.Length < 20)
            {
                return false;
            }
            short num8 = BitConverter.ToInt16(lData, 0);
            short num9 = BitConverter.ToInt16(lData, 2);
            if (num8 != 245 || num9 != 1)
            {
                return false;
            }
            lMesh.Clear();
            int num10 = BitConverter.ToInt32(lData, 4);
            int num11 = BitConverter.ToInt32(lData, 8);
            int num12 = BitConverter.ToInt32(lData, 12);
            byte b = lData[16];
            bool flag = (b & 1) > 0;
            bool flag2 = (b & 2) > 0;
            bool flag3 = (b & 4) > 0;
            bool flag4 = (b & 8) > 0;
            bool flag5 = (b & 16) > 0;
            bool flag6 = (b & 32) > 0;
            bool flag7 = (b & 64) > 0;
            bool flag8 = (b & 128) > 0;
            Vector3[] array = new Vector3[num10];
            int num13 = array.Length * num2;
            GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
            Marshal.Copy(lData, num7, gCHandle.AddrOfPinnedObject(), num13);
            gCHandle.Free();
            num7 += num13;
            lMesh.vertices = array;
            if (flag)
            {
                Vector2[] array2 = new Vector2[num10];
                num13 = array2.Length * num;
                gCHandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
                Marshal.Copy(lData, num7, gCHandle.AddrOfPinnedObject(), num13);
                gCHandle.Free();
                num7 += num13;
                lMesh.uv = array2;
                Debug.Log("UV Count : " + array2.Length);
            }
            if (flag2)
            {
                Vector2[] array2 = new Vector2[num10];
                num13 = array2.Length * num;
                gCHandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
                Marshal.Copy(lData, num7, gCHandle.AddrOfPinnedObject(), num13);
                gCHandle.Free();
                num7 += num13;
                lMesh.uv2 = array2;
                Debug.Log("UV1 Count : " + array2.Length);
            }
            if (flag3)
            {
                Vector2[] array2 = new Vector2[num10];
                num13 = array2.Length * num;
                gCHandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
                Marshal.Copy(lData, num7, gCHandle.AddrOfPinnedObject(), num13);
                gCHandle.Free();
                num7 += num13;
                lMesh.uv2 = array2;
                Debug.Log("UV2 Count : " + array2.Length);
            }
            if (flag4)
            {
                Vector3[] array3 = new Vector3[num10];
                num13 = array3.Length * num2;
                gCHandle = GCHandle.Alloc(array3, GCHandleType.Pinned);
                Marshal.Copy(lData, num7, gCHandle.AddrOfPinnedObject(), num13);
                gCHandle.Free();
                num7 += num13;
                lMesh.normals = array3;
                Debug.Log("Normal Count : " + array3.Length);
            }
            if (flag5)
            {
                Vector4[] array4 = new Vector4[num10];
                num13 = array4.Length * num3;
                gCHandle = GCHandle.Alloc(array4, GCHandleType.Pinned);
                Marshal.Copy(lData, num7, gCHandle.AddrOfPinnedObject(), num13);
                gCHandle.Free();
                num7 += num13;
                lMesh.tangents = array4;
                Debug.Log("Tangents Count : " + array4.Length);
            }
            if (flag6)
            {
                Color[] array5 = new Color[num10];
                num13 = array5.Length * num6;
                gCHandle = GCHandle.Alloc(array5, GCHandleType.Pinned);
                Marshal.Copy(lData, num7, gCHandle.AddrOfPinnedObject(), num13);
                gCHandle.Free();
                num7 += num13;
                lMesh.colors = array5;
            }
            if (flag7)
            {
                Matrix4x4[] array6 = new Matrix4x4[num10];
                num13 = array6.Length * num4;
                gCHandle = GCHandle.Alloc(array6, GCHandleType.Pinned);
                Marshal.Copy(lData, num7, gCHandle.AddrOfPinnedObject(), num13);
                gCHandle.Free();
                num7 += num13;
                lMesh.bindposes = array6;
            }
            if (flag8)
            {
                BoneWeight[] array7 = new BoneWeight[num10];
                num13 = array7.Length * num5;
                gCHandle = GCHandle.Alloc(array7, GCHandleType.Pinned);
                Marshal.Copy(lData, num7, gCHandle.AddrOfPinnedObject(), num13);
                gCHandle.Free();
                num7 += num13;
                lMesh.boneWeights = array7;
            }
            int[] array8 = new int[num11];
            num13 = array8.Length * 4;
            Buffer.BlockCopy(lData, num7, array8, 0, num13);
            num7 += num13;
            lMesh.triangles = array8;
            for (int i = 0; i < num12; i++)
            {
                int num14 = BitConverter.ToInt32(lData, num7);
                num7 += 4;
                array8 = new int[num14];
                num13 = array8.Length * 4;
                Buffer.BlockCopy(lData, num7, array8, 0, num13);
                num7 += num13;
                if (array8.Length > 0 && array8.Length % 3 == 0)
                {
                    lMesh.SetTriangles(array8, i);
                }
            }
            return true;
        }

        public static byte[] EncodeBinary(this Mesh lMesh)
        {
            int num = Marshal.SizeOf(typeof(Vector2));
            int num2 = Marshal.SizeOf(typeof(Vector3));
            int num3 = Marshal.SizeOf(typeof(Vector4));
            int num4 = Marshal.SizeOf(typeof(Matrix4x4));
            int num5 = Marshal.SizeOf(typeof(BoneWeight));
            int num6 = Marshal.SizeOf(typeof(Color));
            int num7 = 20;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            bool flag7 = false;
            bool flag8 = false;
            byte[] array = new byte[num7];
            Vector3[] vertices = lMesh.vertices;
            Int32Converter int32Converter = vertices.Length;
            int num8 = vertices.Length * num2;
            Array.Resize<byte>(ref array, num7 + num8);
            GCHandle gCHandle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
            Marshal.Copy(gCHandle.AddrOfPinnedObject(), array, num7, num8);
            gCHandle.Free();
            num7 += num8;
            Vector2[] array2 = lMesh.uv;
            if (array2.Length > 0)
            {
                flag = true;
                num8 = array2.Length * num;
                Array.Resize<byte>(ref array, num7 + num8);
                gCHandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
                Marshal.Copy(gCHandle.AddrOfPinnedObject(), array, num7, num8);
                gCHandle.Free();
                num7 += num8;
            }
            array2 = lMesh.uv2;
            if (array2.Length > 0)
            {
                flag2 = true;
                num8 = array2.Length * num;
                Array.Resize<byte>(ref array, num7 + num8);
                gCHandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
                Marshal.Copy(gCHandle.AddrOfPinnedObject(), array, num7, num8);
                gCHandle.Free();
                num7 += num8;
            }
            array2 = lMesh.uv2;
            if (array2.Length > 0)
            {
                flag3 = true;
                num8 = array2.Length * num;
                Array.Resize<byte>(ref array, num7 + num8);
                gCHandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
                Marshal.Copy(gCHandle.AddrOfPinnedObject(), array, num7, num8);
                gCHandle.Free();
                num7 += num8;
            }
            Vector3[] normals = lMesh.normals;
            if (normals.Length > 0)
            {
                flag4 = true;
                num8 = normals.Length * num2;
                Array.Resize<byte>(ref array, num7 + num8);
                gCHandle = GCHandle.Alloc(normals, GCHandleType.Pinned);
                Marshal.Copy(gCHandle.AddrOfPinnedObject(), array, num7, num8);
                gCHandle.Free();
                num7 += num8;
            }
            Vector4[] tangents = lMesh.tangents;
            if (tangents.Length > 0)
            {
                flag5 = true;
                num8 = tangents.Length * num3;
                Array.Resize<byte>(ref array, num7 + num8);
                gCHandle = GCHandle.Alloc(tangents, GCHandleType.Pinned);
                Marshal.Copy(gCHandle.AddrOfPinnedObject(), array, num7, num8);
                gCHandle.Free();
                num7 += num8;
            }
            Color[] colors = lMesh.colors;
            if (colors.Length > 0)
            {
                flag6 = true;
                num8 = colors.Length * num6;
                Array.Resize<byte>(ref array, num7 + num8);
                gCHandle = GCHandle.Alloc(colors, GCHandleType.Pinned);
                Marshal.Copy(gCHandle.AddrOfPinnedObject(), array, num7, num8);
                gCHandle.Free();
                num7 += num8;
            }
            Matrix4x4[] bindposes = lMesh.bindposes;
            if (bindposes.Length > 0)
            {
                flag7 = true;
                num8 = bindposes.Length * num4;
                Array.Resize<byte>(ref array, num7 + num8);
                gCHandle = GCHandle.Alloc(bindposes, GCHandleType.Pinned);
                Marshal.Copy(gCHandle.AddrOfPinnedObject(), array, num7, num8);
                gCHandle.Free();
                num7 += num8;
            }
            BoneWeight[] boneWeights = lMesh.boneWeights;
            if (boneWeights.Length > 0)
            {
                flag8 = true;
                num8 = boneWeights.Length * num5;
                Array.Resize<byte>(ref array, num7 + num8);
                gCHandle = GCHandle.Alloc(boneWeights, GCHandleType.Pinned);
                Marshal.Copy(gCHandle.AddrOfPinnedObject(), array, num7, num8);
                gCHandle.Free();
                num7 += num8;
            }
            int[] triangles = lMesh.triangles;
            Int32Converter int32Converter2 = triangles.Length;
            num8 = triangles.Length * 4;
            Array.Resize<byte>(ref array, num7 + num8);
            Buffer.BlockCopy(triangles, 0, array, num7, num8);
            num7 += num8;
            Int32Converter value = lMesh.subMeshCount;
            for (int i = 0; i < value; i++)
            {
                triangles = lMesh.GetTriangles(i);
                Int32Converter int32Converter3 = triangles.Length;
                num8 = 4 + triangles.Length * 4;
                Array.Resize<byte>(ref array, num7 + num8);
                array[num7] = int32Converter3.Byte1;
                array[num7 + 1] = int32Converter3.Byte2;
                array[num7 + 2] = int32Converter3.Byte3;
                array[num7 + 3] = int32Converter3.Byte4;
                Buffer.BlockCopy(triangles, 0, array, num7, num8 - 4);
                num7 += num8;
            }
            array[0] = 245;
            array[1] = 0;
            array[2] = 1;
            array[3] = 0;
            array[4] = int32Converter.Byte1;
            array[5] = int32Converter.Byte2;
            array[6] = int32Converter.Byte3;
            array[7] = int32Converter.Byte4;
            array[8] = int32Converter2.Byte1;
            array[9] = int32Converter2.Byte2;
            array[10] = int32Converter2.Byte3;
            array[11] = int32Converter2.Byte4;
            array[12] = value.Byte1;
            array[13] = value.Byte2;
            array[14] = value.Byte3;
            array[15] = value.Byte4;
            array[16] = (byte)((flag ? 1 : 0) | (flag2 ? 2 : 0) | (flag3 ? 4 : 0) | (flag4 ? 8 : 0) | (flag5 ? 16 : 0) | (flag6 ? 32 : 0) | (flag7 ? 64 : 0) | (flag8 ? 128 : 0));
            return array;
        }
    }
}
