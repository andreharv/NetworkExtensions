using System.Collections.Generic;
using Transit.Framework.Texturing;
using UnityEngine;

namespace Transit.Framework
{
    public static partial class NetInfoExtensions
    {
        public static NetInfo SetAllNodesTexture(this NetInfo info, TextureSet newTextures, LODTextureSet newLODTextures = null)
        {
            for(var i = 0; i < info.m_nodes.Length; i++)
            {
                info.m_nodes[i].SetTextures(newTextures, newLODTextures);
            }

            return info;
        }

        public static NetInfo.Node SetTextures(this NetInfo.Node node, TextureSet newTextures, LODTextureSet newLODTextures = null)
        {
            if (newTextures != null)
            {
                if (node.m_material != null)
                {
                    node.m_material = newTextures.CreateRoadMaterial(node.m_material);
                    node.m_nodeMaterial = node.m_material;
                }
            }

            if (newLODTextures != null)
            {
                if (node.m_lodMaterial != null)
                {
                    node.m_lodMaterial = newLODTextures.CreateRoadMaterial(node.m_lodMaterial);
                }
            }

            return node;
        }

        public static NetInfo.Node SetMeshes(this NetInfo.Node node, string newMeshPath, string newLODMeshPath = null)
        {
            node.m_mesh = AssetManager.instance.GetMesh(newMeshPath);
            node.m_nodeMesh = node.m_mesh;
            if (newLODMeshPath != null)
            {
                node.m_lodMesh = AssetManager.instance.GetMesh(newLODMeshPath);
            }

            return node;
        }


        public static NetInfo.Node SetFlags(this NetInfo.Node node, NetNode.Flags required, NetNode.Flags forbidden)
        {
            node.m_flagsRequired = required;
            node.m_flagsForbidden = forbidden;

            return node;
        }

        public static NetInfo.Node SetConsistentUVs(this NetInfo.Node node, bool isPowerLines = true)
        {
            var colors = new List<Color>();
            var colors32 = new List<Color32>();
            Color color;
            Color32 color32;
            if (isPowerLines)
            {
                color = new Color(0, 0, 0, 255);
                color32 = new Color32(0, 0, 0, 255);
            }
            else
            {
                color = new Color(255, 0, 255, 255);
                color32 = new Color32(255, 0, 255, 255);
            }
            for (int i = 0; i < node.m_mesh.vertexCount; i++)
            {
                colors.Add(color);
                colors32.Add(color32);
            }
            node.m_mesh.colors = colors.ToArray();
            node.m_mesh.colors32 = colors32.ToArray();

            colors = new List<Color>();
            colors32 = new List<Color32>();
            for (int i = 0; i < node.m_lodMesh.vertexCount; i++)
            {
                colors.Add(color);
                colors32.Add(color32);
            }
            node.m_lodMesh.colors = colors.ToArray();
            node.m_lodMesh.colors32 = colors32.ToArray();

            return node;
        }
    }
}
