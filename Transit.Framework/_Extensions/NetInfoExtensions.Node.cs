namespace Transit.Framework
{
    public static partial class NetInfoExtensions
{
        public static NetInfo SetAllNodesTexture(this NetInfo info, TexturesSet newTextures, TexturesSet newLODTextures = null)
        {
            foreach (var node in info.m_nodes)
            {
                node.SetTextures(newTextures, newLODTextures);
            }

            return info;
        }

        public static NetInfo.Node SetTextures(this NetInfo.Node node, TexturesSet newTextures, TexturesSet newLODTextures = null)
        {
            if (node.m_material != null)
            {
                node.m_material = node.m_material.Clone(newTextures);
            }

            if (node.m_nodeMaterial != null)
            {
                node.m_nodeMaterial = node.m_nodeMaterial.Clone(newTextures);
            }

            if (node.m_lodMaterial != null)
            {
                if (newLODTextures != null)
                {
                    node.m_lodMaterial = node.m_lodMaterial.Clone(newLODTextures);
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
    }
}
