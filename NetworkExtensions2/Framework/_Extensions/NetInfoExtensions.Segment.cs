using ColossalFramework;
using NetworkExtensions2.Framework._Extensions;
using NetworkExtensions2.Framework.Import;
using System.Collections.Generic;
using Transit.Framework.Texturing;
using UnityEngine;

namespace Transit.Framework
{
    public static partial class NetInfoExtensions
    {
        public static NetInfo SetAllSegmentsTexture(this NetInfo info, TextureSet newTextures, LODTextureSet newLODTextures = null)
        {
            for (var i = 0; i < info.m_segments.Length; i++)
            {
                info.m_segments[i].SetTextures(newTextures, newLODTextures);
            }

            return info;
        }

        public static NetInfo.Segment SetTextures(this NetInfo.Segment segment, TextureSet newTextures, LODTextureSet newLODTextures = null)
        {
            if (newTextures != null)
            {
                if (segment.m_material != null)
                {
                    segment.m_material = newTextures.CreateRoadMaterial(segment.m_material);
                    segment.m_segmentMaterial = segment.m_material;
                }
            }

            if (newLODTextures != null)
            {
                if (segment.m_lodMaterial != null)
                {
                    segment.m_lodMaterial = newLODTextures.CreateRoadMaterial(segment.m_lodMaterial);
                }
            }

            return segment;
        }

        public static NetInfo.Segment SetTextures(this NetInfo.Segment segment, Material newMaterial, Material newLodMaterial = null)
        {
            if (newMaterial != null)
            {
                var newMaterialClone = newMaterial.CreateRoadMaterial(segment.m_material);
                segment.m_material = newMaterialClone;
                segment.m_segmentMaterial = newMaterialClone;
            }

            if (newLodMaterial != null && segment.m_lodMaterial != null)
            {
                segment.m_lodMaterial = newLodMaterial.CreateRoadMaterial(segment.m_lodMaterial);
            }

            return segment;
        }

        public static NetInfo.Segment SetMeshes(this NetInfo.Segment segment, string newMeshPath, string newLODMeshPath = null)
        {
            segment.m_mesh = AssetManager.instance.GetMesh(newMeshPath);
            segment.m_segmentMesh = segment.m_mesh;
            if (newLODMeshPath != null)
            {
                segment.m_lodMesh = AssetManager.instance.GetMesh(newLODMeshPath);
            }

            return segment;
        }

        public static NetInfo.Segment SetMeshes2(this NetInfo.Segment segment, string newMeshPath)
        {
            var mesh = ImportTransitAsset.GetMesh(newMeshPath);
            segment.m_mesh = mesh;
            segment.m_segmentMesh = mesh;

            return segment;
        }
        public static NetInfo.Segment SetNetResources(this NetInfo.Segment segment, string baseMeshName, string baseTextureName, float offset = 0, bool invert = false)
        {
            return segment.SetResources(@"Resources\Roads\", baseMeshName, baseTextureName, offset, invert);
        }
        public static NetInfo.Segment SetResources(this NetInfo.Segment segment, string subDir, string baseMeshName, string baseTextureName, float offset = 0, bool invert = false)
        {
            baseMeshName = subDir + baseMeshName;
            baseTextureName = subDir + baseTextureName;
            var resourceName = NEXTImportAssetModel.GetResourceName(baseMeshName, baseTextureName);
            var mesh = ImportTransitAsset.GetMesh(resourceName);
            var newMesh = RuntimeMeshUtils.CopyMesh(mesh);
            if (offset != 0)
                mesh.Transform(newMesh, new Vector3(offset, 0, 0), invert);
            if (!mesh.name.EndsWith("1"))
            {
                newMesh.name = newMesh.name + "1";
            }

            var material = ImportTransitAsset.GetMaterial(resourceName);

            segment.m_mesh = newMesh.ShallowClone();
            segment.m_segmentMesh = newMesh.ShallowClone();
            segment.SetTextures(material);
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

        public static NetInfo.Segment SetConsistentUVs(this NetInfo.Segment segment, bool isPowerLines = true)
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

            for (int i = 0; i < segment.m_mesh.vertexCount; i++)
            {
                colors.Add(color);
                colors32.Add(color32);
            }
            segment.m_mesh.colors = colors.ToArray();
            segment.m_mesh.colors32 = colors32.ToArray();

            colors = new List<Color>();
            colors32 = new List<Color32>();
            for (int i = 0; i < segment.m_lodMesh.vertexCount; i++)
            {
                colors.Add(color);
                colors32.Add(color32);
            }
            segment.m_lodMesh.colors = colors.ToArray();
            segment.m_lodMesh.colors32 = colors32.ToArray();

            return segment;
        }
    }
}
