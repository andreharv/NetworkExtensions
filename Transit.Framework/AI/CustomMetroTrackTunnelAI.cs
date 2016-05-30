using System;
using UnityEngine;
namespace Transit.Framework.AI
{
    public class CustomMetroTrackTunnelAI : CustomMetroTrackBaseAI
    {
        public bool m_canModify = true;

        public override bool ColorizeProps(InfoManager.InfoMode infoMode)
        {
            return infoMode == InfoManager.InfoMode.Pollution || base.ColorizeProps(infoMode);
        }

        public override Color GetColor(ushort segmentID, ref NetSegment data, InfoManager.InfoMode infoMode)
        {
            if (infoMode == InfoManager.InfoMode.Transport)
            {
                return new Color(0.2f, 0.2f, 0.2f, 1f);
            }
            if (infoMode == InfoManager.InfoMode.Traffic)
            {
                return base.GetColor(segmentID, ref data, infoMode) * 0.2f;
            }
            return base.GetColor(segmentID, ref data, infoMode);
        }

        public override Color GetColor(ushort nodeID, ref NetNode data, InfoManager.InfoMode infoMode)
        {
            if (infoMode == InfoManager.InfoMode.Transport)
            {
                return new Color(0.2f, 0.2f, 0.2f, 1f);
            }
            if (infoMode == InfoManager.InfoMode.Traffic)
            {
                return base.GetColor(nodeID, ref data, infoMode) * 0.2f;
            }
            return base.GetColor(nodeID, ref data, infoMode);
        }

        public override float GetNodeInfoPriority(ushort segmentID, ref NetSegment data)
        {
            if (this.m_info.m_flattenTerrain)
            {
                return this.m_info.m_halfWidth + 9000f;
            }
            return this.m_info.m_halfWidth + 10000f;
        }

        public override bool IsUnderground()
        {
            return true;
        }

        public override bool BuildUnderground()
        {
            return !this.m_info.m_flattenTerrain;
        }

        public override bool CanModify()
        {
            return this.m_canModify;
        }

        public override void GetTerrainModifyRange(out float start, out float end)
        {
            start = ((!this.m_info.m_flattenTerrain) ? 0f : 0.25f);
            end = 1f;
        }

        public override void GetElevationLimits(out int min, out int max)
        {
            min = ((!this.m_info.m_flattenTerrain) ? -3 : 0);
            max = 0;
        }

        public override bool CanIntersect()
        {
            return false;
        }

        public override bool RaiseTerrain()
        {
            return true;
        }
    }
}