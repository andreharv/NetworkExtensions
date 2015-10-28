namespace Transit.Framework
{
    public static partial class NetInfoExtensions
    {
        public static NetInfo SetAllSegmentsTexture(this NetInfo info, TexturesSet newTextures, LODTexturesSet newLODTextures = null)
        {
            foreach (var segment in info.m_segments)
            {
                segment.SetTextures(newTextures, newLODTextures);
            }

            return info;
        }

        public static NetInfo.Segment SetTextures(this NetInfo.Segment segment, TexturesSet newTextures, LODTexturesSet newLODTextures = null)
        {
            if (segment.m_material != null)
            {
                segment.m_material = segment.m_material.Clone(newTextures);
            }

            if (segment.m_segmentMaterial != null)
            {
                segment.m_segmentMaterial = segment.m_segmentMaterial.Clone(newTextures);
            }

            if (segment.m_lodMaterial != null)
            {
                if (newLODTextures != null)
                {
                    segment.m_lodMaterial = segment.m_lodMaterial.Clone(newLODTextures);
                }
            }

            return segment;
        }

        public static NetInfo.Segment SetMeshes(this NetInfo.Segment segment, string newMeshPath, string newLODMeshPath = null)
        {
            segment.m_mesh = AssetManager.instance.GetMesh(newMeshPath);

            if (newLODMeshPath != null)
            {
                segment.m_lodMesh = AssetManager.instance.GetMesh(newLODMeshPath);
            }

            return segment;
        }


        public static NetInfo.Segment SetFlagsDefault(this NetInfo.Segment segment)
        {
            segment.m_backwardForbidden = NetSegment.Flags.None;
            segment.m_backwardRequired = NetSegment.Flags.None;

            segment.m_forwardForbidden = NetSegment.Flags.None;
            segment.m_forwardRequired = NetSegment.Flags.None;

            return segment;
        }
    }
}
