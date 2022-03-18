using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkExtensions2.Framework._Extensions
{
    public static class MeshExtensions
    {
        public static void Transform(this Mesh source, Mesh dest, Vector3 offset)
        {
            Vector3[] vertices = source.vertices;
            Vector3[] normals = source.normals;
            Color[] colors = source.colors;
            Vector2[] uv = source.uv;
            Vector4[] tangents = source.tangents;
            int[] triangles = source.triangles;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] += offset;
            }
            dest.Clear();
            dest.vertices = vertices;
            dest.normals = normals;
            dest.colors = colors;
            dest.uv = uv;
            dest.tangents = tangents;
            dest.triangles = triangles;
        }
    }
}
