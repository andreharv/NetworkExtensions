using Transit.Framework.Texturing;
using UnityEngine;

namespace Transit.Framework
{
    public static partial class NetInfoExtensions
    {
        public static NetInfo SetAllNodesTexture(this NetInfo info, TexturesSet newTextures, LODTexturesSet newLODTextures = null)
        {
            foreach (var node in info.m_nodes)
            {
                node.SetTextures(newTextures, newLODTextures);
            }

            return info;
        }

        public static NetInfo.Node SetTextures(this NetInfo.Node node, TexturesSet newTextures, LODTexturesSet newLODTextures = null)
        {
            if (newTextures != null)
            {
                if (node.m_material != null)
                {
                    node.m_material = newTextures.CreateRoadMaterial(node.m_material);
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
    }
}
