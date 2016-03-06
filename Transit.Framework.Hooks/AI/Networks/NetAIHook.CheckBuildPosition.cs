using System;
using System.Runtime.CompilerServices;
using ColossalFramework;
using ColossalFramework.Math;
using Transit.Framework.ExtensionPoints.AI;
using Transit.Framework.ExtensionPoints.AI.Networks;
using Transit.Framework.Prerequisites;
using Transit.Framework.Redirection;
using UnityEngine;

namespace Transit.Framework.Hooks.AI.Networks
{
    public partial class NetAIHook : NetAI
    {
        [RedirectFrom(typeof(NetAI), (ulong)PrerequisiteType.NetworkAI)]
#pragma warning disable 108,114
        public virtual ToolBase.ToolErrors CheckBuildPosition(bool test, bool visualize, bool overlay, bool autofix,
#pragma warning restore 108,114
            ref NetTool.ControlPoint startPoint, ref NetTool.ControlPoint middlePoint, ref NetTool.ControlPoint endPoint,
            out BuildingInfo ownerBuilding, out Vector3 ownerPosition, out Vector3 ownerDirection,
            out int productionRate)
        {
            ownerBuilding = null;
            ownerPosition = Vector3.zero;
            ownerDirection = Vector3.forward;
            productionRate = 0;
            ToolBase.ToolErrors toolErrors = ToolBase.ToolErrors.None;
            if (test)
            {
                ushort num = middlePoint.m_segment;
                if (startPoint.m_segment == num || endPoint.m_segment == num)
                {
                    num = 0;
                }
                if (num != 0 && Singleton<NetManager>.instance.m_segments.m_buffer[(int) num].Info == this.m_info)
                {
                    if (ZoneBlocksOffset.Mode == ZoneBlocksOffsetMode.Default)
                    {
                        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                    }
                    else
                    {
                        // Do nothing
                    }
                }
            }
            if (autofix && this.m_info.m_enableBendingSegments)
            {
                Vector3 vector = middlePoint.m_direction;
                Vector3 vector2 = -endPoint.m_direction;
                Vector3 vector3 = middlePoint.m_position;
                float minNodeDistance = this.m_info.GetMinNodeDistance();
                for (int i = 0; i < 3; i++)
                {
                    bool flag = false;
                    bool flag2 = false;
                    if (startPoint.m_node != 0)
                    {
                        if (ForceValidDirection(this.m_info, ref vector, startPoint.m_node,
                            ref Singleton<NetManager>.instance.m_nodes.m_buffer[(int) startPoint.m_node]))
                        {
                            flag = true;
                        }
                    }
                    else if (startPoint.m_segment != 0 &&
                             ForceValidDirection(this.m_info, startPoint.m_position, ref vector,
                                 startPoint.m_segment,
                                 ref Singleton<NetManager>.instance.m_segments.m_buffer[(int) startPoint.m_segment]))
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        Line2 line = Line2.XZ(startPoint.m_position, startPoint.m_position + vector);
                        Line2 line2 = Line2.XZ(endPoint.m_position, endPoint.m_position + vector2);
                        bool flag3 = true;
                        float num2;
                        float num3;
                        if (line.Intersect(line2, out num2, out num3) && num2 >= minNodeDistance &&
                            num3 >= minNodeDistance)
                        {
                            vector3 = startPoint.m_position + vector*num2;
                            flag3 = false;
                        }
                        if (flag3)
                        {
                            Vector3 vector4 = endPoint.m_position - startPoint.m_position;
                            Vector3 vector5 = vector;
                            vector4.y = 0f;
                            vector5.y = 0f;
                            float num4 = Vector3.SqrMagnitude(vector4);
                            vector4 = Vector3.Normalize(vector4);
                            float num5 = Mathf.Min(1.17809725f, Mathf.Acos(Vector3.Dot(vector4, vector5)));
                            float d = Mathf.Sqrt(0.5f*num4/Mathf.Max(0.001f, 1f - Mathf.Cos(3.14159274f - 2f*num5)));
                            vector3 = startPoint.m_position + vector5*d;
                            vector2 = vector3 - endPoint.m_position;
                            vector2.y = 0f;
                            vector2.Normalize();
                        }
                    }
                    if (endPoint.m_node != 0)
                    {
                        if (ForceValidDirection(this.m_info, ref vector2, endPoint.m_node,
                            ref Singleton<NetManager>.instance.m_nodes.m_buffer[(int) endPoint.m_node]))
                        {
                            flag2 = true;
                        }
                    }
                    else if (endPoint.m_segment != 0 &&
                             ForceValidDirection(this.m_info, endPoint.m_position, ref vector2, endPoint.m_segment,
                                 ref Singleton<NetManager>.instance.m_segments.m_buffer[(int) endPoint.m_segment]))
                    {
                        flag2 = true;
                    }
                    if (flag2)
                    {
                        Line2 line3 = Line2.XZ(startPoint.m_position, startPoint.m_position + vector);
                        Line2 line4 = Line2.XZ(endPoint.m_position, endPoint.m_position + vector2);
                        bool flag4 = true;
                        float num6;
                        float num7;
                        if (line3.Intersect(line4, out num6, out num7) && num6 >= minNodeDistance &&
                            num7 >= minNodeDistance)
                        {
                            vector3 = endPoint.m_position + vector2*num7;
                            flag4 = false;
                        }
                        if (flag4)
                        {
                            Vector3 vector6 = startPoint.m_position - endPoint.m_position;
                            Vector3 vector7 = vector2;
                            vector6.y = 0f;
                            vector7.y = 0f;
                            float num8 = Vector3.SqrMagnitude(vector6);
                            vector6 = Vector3.Normalize(vector6);
                            float num9 = Mathf.Min(1.17809725f, Mathf.Acos(Vector3.Dot(vector6, vector7)));
                            float d2 = Mathf.Sqrt(0.5f*num8/Mathf.Max(0.001f, 1f - Mathf.Cos(3.14159274f - 2f*num9)));
                            vector3 = endPoint.m_position + vector7*d2;
                            vector = vector3 - startPoint.m_position;
                            vector.y = 0f;
                            vector.Normalize();
                        }
                    }
                    if (!flag && !flag2)
                    {
                        middlePoint.m_direction = vector;
                        endPoint.m_direction = -vector2;
                        middlePoint.m_position = vector3;
                        break;
                    }
                }
            }
            return toolErrors;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(NetAI), (ulong)PrerequisiteType.NetworkAI)]
        private static bool ForceValidDirection(NetInfo info, ref Vector3 direction, ushort nodeID, ref NetNode node)
        {
            throw new NotImplementedException("ForceValidDirection is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(NetAI), (ulong)PrerequisiteType.NetworkAI)]
        private static bool ForceValidDirection(NetInfo info, Vector3 position, ref Vector3 direction, ushort segmentID, ref NetSegment segment)
        {
            throw new NotImplementedException("ForceValidDirection is target of redirection and is not implemented.");
        }
    }
}