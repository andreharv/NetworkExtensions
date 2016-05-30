using ColossalFramework;
using ColossalFramework.Math;
using System;
using UnityEngine;

namespace Transit.Framework.AI
{
    public class CustomMetroTrackBridgeAI : CustomMetroTrackBaseAI
    {
        public BuildingInfo m_bridgePillarInfo;

        public BuildingInfo m_middlePillarInfo;

        public int m_elevationCost = 2000;

        public float m_bridgePillarOffset;

        public float m_middlePillarOffset;

        public bool m_doubleLength;

        public bool m_canModify = true;

        public override bool ColorizeProps(InfoManager.InfoMode infoMode)
        {
            return infoMode == InfoManager.InfoMode.Pollution || base.ColorizeProps(infoMode);
        }

        public override int GetConstructionCost(Vector3 startPos, Vector3 endPos, float startHeight, float endHeight)
        {
            float num = VectorUtils.LengthXZ(endPos - startPos);
            float num2 = (Mathf.Max(0f, startHeight) + Mathf.Max(0f, endHeight)) * 0.5f;
            int num3 = Mathf.RoundToInt(num / 8f + 0.1f);
            int num4 = Mathf.RoundToInt(num2 / 4f + 0.1f);
            int result = this.m_constructionCost * num3 + this.m_elevationCost * num4;
            Singleton<EconomyManager>.instance.m_EconomyWrapper.OnGetConstructionCost(ref result, this.m_info.m_class.m_service, this.m_info.m_class.m_subService, this.m_info.m_class.m_level);
            return result;
        }

        public override void GetNodeBuilding(ushort nodeID, ref NetNode data, out BuildingInfo building, out float heightOffset)
        {
            if ((data.m_flags & NetNode.Flags.Outside) == NetNode.Flags.None)
            {
                if (this.m_middlePillarInfo != null && (data.m_flags & NetNode.Flags.Double) != NetNode.Flags.None)
                {
                    building = this.m_middlePillarInfo;
                    heightOffset = this.m_middlePillarOffset - 1f - this.m_middlePillarInfo.m_generatedInfo.m_size.y;
                    return;
                }
                if (this.m_bridgePillarInfo != null)
                {
                    building = this.m_bridgePillarInfo;
                    heightOffset = this.m_bridgePillarOffset - 1f - this.m_bridgePillarInfo.m_generatedInfo.m_size.y;
                    return;
                }
            }
            base.GetNodeBuilding(nodeID, ref data, out building, out heightOffset);
        }

        public override float GetNodeInfoPriority(ushort segmentID, ref NetSegment data)
        {
            if ((data.m_flags & NetSegment.Flags.Untouchable) != NetSegment.Flags.None)
            {
                return this.m_info.m_halfWidth + 3000f;
            }
            if ((Singleton<NetManager>.instance.m_nodes.m_buffer[(int)data.m_startNode].m_flags & NetNode.Flags.Outside) != NetNode.Flags.None)
            {
                return this.m_info.m_halfWidth + 2000f;
            }
            if ((Singleton<NetManager>.instance.m_nodes.m_buffer[(int)data.m_endNode].m_flags & NetNode.Flags.Outside) != NetNode.Flags.None)
            {
                return this.m_info.m_halfWidth + 2000f;
            }
            if (this.m_bridgePillarInfo == null || !this.m_canModify)
            {
                return this.m_info.m_halfWidth - 1000f;
            }
            return this.m_info.m_halfWidth;
        }

        public override bool RequireDoubleSegments()
        {
            return this.m_doubleLength;
        }

        public override bool CanModify()
        {
            return this.m_canModify;
        }

        public override void GetElevationLimits(out int min, out int max)
        {
            min = 0;
            max = 5;
        }

        public override bool IsOverground()
        {
            return true;
        }

        public override bool CanIntersect()
        {
            return false;
        }
    }
}