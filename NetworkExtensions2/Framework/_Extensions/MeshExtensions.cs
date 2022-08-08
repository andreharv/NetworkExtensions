using System.Linq;
using UnityEngine;

namespace NetworkExtensions2.Framework._Extensions
{
    public static class MeshExtensions
    {
        public static void Transform(this Mesh source, Mesh dest, Vector3 offset, bool invert = false)
        {
            Vector3[] vertices = source.vertices;
            Vector3[] normals = source.normals;
            Color[] colors = source.colors;
            Vector2[] uv = source.uv;
            Vector4[] tangents = source.tangents;
            int[] triangles = source.triangles;
            if (offset != Vector3.zero)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    if (invert)
                    {
                        vertices[i] = new Vector3(-vertices[i].x, vertices[i].y, vertices[i].z);
                        //uv[i] = new Vector2(-normals[i].x, normals[i].y);
                    }
                    vertices[i] += offset;
                }
            }
            dest.Clear();
            dest.vertices = vertices;
            dest.normals = normals;
            dest.colors = colors;
            dest.uv = uv;
            dest.tangents = tangents;
            dest.triangles = invert ? triangles.Reverse().ToArray() : triangles;
            //if (invert)
                //dest.RecalculateNormals();
        }
    }
}