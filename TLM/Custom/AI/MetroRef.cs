using ColossalFramework;
using System;
using UnityEngine;

public class MetroTrackAI : PlayerNetAI
{
    public TransportInfo m_transportInfo;

    public override Color GetColor(ushort segmentID, ref NetSegment data, InfoManager.InfoMode infoMode)
    {
        if (infoMode == InfoManager.InfoMode.Transport)
        {
            return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)this.m_transportInfo.m_transportType] * 0.2f;
        }
        if (infoMode == InfoManager.InfoMode.Traffic)
        {
            return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Mathf.Clamp01((float)data.m_trafficDensity * 0.01f)) * 0.2f;
        }
        if (infoMode != InfoManager.InfoMode.Maintenance)
        {
            return base.GetColor(segmentID, ref data, infoMode);
        }
        return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)this.m_transportInfo.m_transportType] * 0.2f;
    }

    public override Color GetColor(ushort nodeID, ref NetNode data, InfoManager.InfoMode infoMode)
    {
        if (infoMode == InfoManager.InfoMode.Transport)
        {
            return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)this.m_transportInfo.m_transportType] * 0.2f;
        }
        if (infoMode == InfoManager.InfoMode.Traffic)
        {
            NetManager instance = Singleton<NetManager>.instance;
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < 8; i++)
            {
                ushort segment = data.GetSegment(i);
                if (segment != 0)
                {
                    num += (int)instance.m_segments.m_buffer[(int)segment].m_trafficDensity;
                    num2++;
                }
            }
            if (num2 != 0)
            {
                num /= num2;
            }
            return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Mathf.Clamp01((float)num * 0.01f)) * 0.2f;
        }
        if (infoMode != InfoManager.InfoMode.Maintenance)
        {
            return base.GetColor(nodeID, ref data, infoMode);
        }
        return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)this.m_transportInfo.m_transportType] * 0.2f;
    }

    public override void GetPlacementInfoMode(out InfoManager.InfoMode mode, out InfoManager.SubInfoMode subMode, float elevation)
    {
        mode = InfoManager.InfoMode.Transport;
        subMode = InfoManager.SubInfoMode.Default;
    }

    public override void CreateSegment(ushort segmentID, ref NetSegment data)
    {
        base.CreateSegment(segmentID, ref data);
    }

    public override void SimulationStep(ushort segmentID, ref NetSegment data)
    {
        base.SimulationStep(segmentID, ref data);
        NetManager instance = Singleton<NetManager>.instance;
        float num = 0f;
        uint num2 = data.m_lanes;
        int num3 = 0;
        while (num3 < this.m_info.m_lanes.Length && num2 != 0u)
        {
            NetInfo.Lane lane = this.m_info.m_lanes[num3];
            if (lane.m_laneType == NetInfo.LaneType.Vehicle)
            {
                num += instance.m_lanes.m_buffer[(int)((UIntPtr)num2)].m_length;
            }
            num2 = instance.m_lanes.m_buffer[(int)((UIntPtr)num2)].m_nextLane;
            num3++;
        }
        int num4 = Mathf.RoundToInt(num) << 4;
        int num5 = 0;
        if (num4 != 0)
        {
            num5 = (int)((byte)Mathf.Min((int)(data.m_trafficBuffer * 100) / num4, 100));
        }
        data.m_trafficBuffer = 0;
        if (num5 > (int)data.m_trafficDensity)
        {
            data.m_trafficDensity = (byte)Mathf.Min((int)(data.m_trafficDensity + 5), num5);
        }
        else if (num5 < (int)data.m_trafficDensity)
        {
            data.m_trafficDensity = (byte)Mathf.Max((int)(data.m_trafficDensity - 5), num5);
        }
    }

    public override float GetLengthSnap()
    {
        return 0f;
    }

    public override void GetElevationLimits(out int min, out int max)
    {
        min = -2;
        max = 0;
    }

    public override float GetNodeInfoPriority(ushort segmentID, ref NetSegment data)
    {
        if ((data.m_flags & NetSegment.Flags.Untouchable) != NetSegment.Flags.None)
        {
            return this.m_info.m_halfWidth + 22000f;
        }
        return this.m_info.m_halfWidth + 21000f;
    }

    public override bool DisplayTempSegment()
    {
        return true;
    }

    public override bool IsUnderground()
    {
        return true;
    }

    public override bool BuildUnderground()
    {
        return true;
    }

    public override bool SupportUnderground()
    {
        return true;
    }

    public override void UpdateLanes(ushort segmentID, ref NetSegment data, bool loading)
    {
        base.UpdateLanes(segmentID, ref data, loading);
        if (!loading)
        {
            NetManager instance = Singleton<NetManager>.instance;
            int num = Mathf.Max((int)((data.m_bounds.min.x - 16f) / 64f + 135f), 0);
            int num2 = Mathf.Max((int)((data.m_bounds.min.z - 16f) / 64f + 135f), 0);
            int num3 = Mathf.Min((int)((data.m_bounds.max.x + 16f) / 64f + 135f), 269);
            int num4 = Mathf.Min((int)((data.m_bounds.max.z + 16f) / 64f + 135f), 269);
            for (int i = num2; i <= num4; i++)
            {
                for (int j = num; j <= num3; j++)
                {
                    ushort num5 = instance.m_nodeGrid[i * 270 + j];
                    int num6 = 0;
                    while (num5 != 0)
                    {
                        NetInfo info = instance.m_nodes.m_buffer[(int)num5].Info;
                        Vector3 position = instance.m_nodes.m_buffer[(int)num5].m_position;
                        float num7 = Mathf.Max(Mathf.Max(data.m_bounds.min.x - 16f - position.x, data.m_bounds.min.z - 16f - position.z), Mathf.Max(position.x - data.m_bounds.max.x - 16f, position.z - data.m_bounds.max.z - 16f));
                        if (num7 < 0f)
                        {
                            info.m_netAI.NearbyLanesUpdated(num5, ref instance.m_nodes.m_buffer[(int)num5]);
                        }
                        num5 = instance.m_nodes.m_buffer[(int)num5].m_nextGridNode;
                        if (++num6 >= 32768)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                            break;
                        }
                    }
                }
            }
        }
    }

    public override void GetRayCastHeights(ushort segmentID, ref NetSegment data, out float leftMin, out float rightMin, out float max)
    {
        leftMin = this.m_info.m_minHeight;
        rightMin = this.m_info.m_minHeight;
        max = 0f;
    }

    public override bool RaiseTerrain()
    {
        return true;
    }
}
