using ColossalFramework;
using System;
using UnityEngine;

namespace Transit.Framework.AI
{
    public class CustomMetroTrackAI : CustomMetroTrackBaseAI
    {
        public NetInfo m_connectedInfo;

        public NetInfo m_connectedElevatedInfo;

        public NetInfo m_elevatedInfo;

        public NetInfo m_bridgeInfo;

        public NetInfo m_slopeInfo;

        public NetInfo m_tunnelInfo;

        public override void GetEffectRadius(out float radius, out bool capped, out Color color)
        {
            radius = 0f;
            capped = false;
            color = new Color(0f, 0f, 0f, 0f);
        }

        public override void GetPlacementInfoMode(out InfoManager.InfoMode mode, out InfoManager.SubInfoMode subMode, float elevation)
        {
            if (elevation < -8f)
            {
                mode = InfoManager.InfoMode.Traffic;
                subMode = InfoManager.SubInfoMode.Default;
            }
            else
            {
                mode = InfoManager.InfoMode.None;
                subMode = InfoManager.SubInfoMode.Default;
            }
        }

        public override void CreateSegment(ushort segmentID, ref NetSegment data)
        {
            base.CreateSegment(segmentID, ref data);
        }

        public override float GetLengthSnap()
        {
            return 0f;
        }

        public override void GetElevationLimits(out int min, out int max)
        {
            min = ((this.m_tunnelInfo == null || (Singleton<ToolManager>.instance.m_properties.m_mode & ItemClass.Availability.GameAndAsset) == ItemClass.Availability.None) ? 0 : -3);
            max = ((this.m_elevatedInfo == null && this.m_bridgeInfo == null) ? 0 : 5);
        }

        public override NetInfo GetInfo(float minElevation, float maxElevation, float length, bool incoming, bool outgoing, bool curved, bool enableDouble, ref ToolBase.ToolErrors errors)
        {
            if (incoming || outgoing)
            {
                int num;
                int num2;
                Singleton<BuildingManager>.instance.CalculateOutsideConnectionCount(this.m_info.m_class.m_service, this.m_info.m_class.m_subService, out num, out num2);
                if ((incoming && num >= 4) || (outgoing && num2 >= 4))
                {
                    errors |= ToolBase.ToolErrors.TooManyConnections;
                }
                if (this.m_connectedElevatedInfo != null && maxElevation > 0.1f)
                {
                    return this.m_connectedElevatedInfo;
                }
                if (this.m_connectedInfo != null)
                {
                    return this.m_connectedInfo;
                }
            }
            if (maxElevation > 255f)
            {
                errors |= ToolBase.ToolErrors.HeightTooHigh;
            }
            if ((Singleton<ToolManager>.instance.m_properties.m_mode & ItemClass.Availability.GameAndAsset) != ItemClass.Availability.None)
            {
                if (this.m_tunnelInfo != null && maxElevation < -8f)
                {
                    return this.m_tunnelInfo;
                }
                if (this.m_slopeInfo != null && minElevation < -8f)
                {
                    return this.m_slopeInfo;
                }
            }
            if (this.m_bridgeInfo != null && maxElevation > 25f && length > 45f && !curved && (enableDouble || !this.m_bridgeInfo.m_netAI.RequireDoubleSegments()))
            {
                return this.m_bridgeInfo;
            }
            if (this.m_elevatedInfo != null && maxElevation > 0.1f)
            {
                return this.m_elevatedInfo;
            }
            if (maxElevation > 8f)
            {
                errors |= ToolBase.ToolErrors.HeightTooHigh;
            }
            return this.m_info;
        }

        public override float GetNodeInfoPriority(ushort segmentID, ref NetSegment data)
        {
            if ((data.m_flags & NetSegment.Flags.Untouchable) != NetSegment.Flags.None)
            {
                return this.m_info.m_halfWidth + 13000f;
            }
            if ((Singleton<NetManager>.instance.m_nodes.m_buffer[(int)data.m_startNode].m_flags & NetNode.Flags.Outside) != NetNode.Flags.None)
            {
                return this.m_info.m_halfWidth + 12000f;
            }
            if ((Singleton<NetManager>.instance.m_nodes.m_buffer[(int)data.m_endNode].m_flags & NetNode.Flags.Outside) != NetNode.Flags.None)
            {
                return this.m_info.m_halfWidth + 12000f;
            }
            return this.m_info.m_halfWidth + 11000f;
        }

        public override bool DisplayTempSegment()
        {
            return false;
        }

        public override bool SupportUnderground()
        {
            return this.m_tunnelInfo != null && (Singleton<ToolManager>.instance.m_properties.m_mode & ItemClass.Availability.GameAndAsset) != ItemClass.Availability.None;
        }

        public override ToolBase.ToolErrors CheckBuildPosition(bool test, bool visualize, bool overlay, bool autofix, ref NetTool.ControlPoint startPoint, ref NetTool.ControlPoint middlePoint, ref NetTool.ControlPoint endPoint, out BuildingInfo ownerBuilding, out Vector3 ownerPosition, out Vector3 ownerDirection, out int productionRate)
        {
            ToolBase.ToolErrors toolErrors = base.CheckBuildPosition(test, visualize, overlay, autofix, ref startPoint, ref middlePoint, ref endPoint, out ownerBuilding, out ownerPosition, out ownerDirection, out productionRate);
            if (test)
            {
                NetManager instance = Singleton<NetManager>.instance;
                ushort num = middlePoint.m_segment;
                if (startPoint.m_segment == num || endPoint.m_segment == num)
                {
                    num = 0;
                }
                if (num != 0)
                {
                    NetInfo info = instance.m_segments.m_buffer[(int)num].Info;
                    if (this.m_connectedInfo == info)
                    {
                        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                    }
                    if (this.m_elevatedInfo == info)
                    {
                        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                    }
                    if (this.m_bridgeInfo == info)
                    {
                        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                    }
                    if (this.m_tunnelInfo == info)
                    {
                        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                    }
                    if (this.m_slopeInfo == info)
                    {
                        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                    }
                }
                if (startPoint.m_node != 0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        ushort segment = instance.m_nodes.m_buffer[(int)startPoint.m_node].GetSegment(i);
                        if (segment != 0)
                        {
                            NetInfo info2 = instance.m_segments.m_buffer[(int)segment].Info;
                            if ((info2.m_vehicleTypes & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None)
                            {
                                toolErrors |= ToolBase.ToolErrors.CannotCrossTrack;
                            }
                        }
                    }
                }
                else if (startPoint.m_segment != 0)
                {
                    NetInfo info3 = instance.m_segments.m_buffer[(int)startPoint.m_segment].Info;
                    if ((info3.m_vehicleTypes & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None)
                    {
                        toolErrors |= ToolBase.ToolErrors.CannotCrossTrack;
                    }
                }
                if (endPoint.m_node != 0)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        ushort segment2 = instance.m_nodes.m_buffer[(int)endPoint.m_node].GetSegment(j);
                        if (segment2 != 0)
                        {
                            NetInfo info4 = instance.m_segments.m_buffer[(int)segment2].Info;
                            if ((info4.m_vehicleTypes & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None)
                            {
                                toolErrors |= ToolBase.ToolErrors.CannotCrossTrack;
                            }
                        }
                    }
                }
                else if (endPoint.m_segment != 0)
                {
                    NetInfo info5 = instance.m_segments.m_buffer[(int)endPoint.m_segment].Info;
                    if ((info5.m_vehicleTypes & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None)
                    {
                        toolErrors |= ToolBase.ToolErrors.CannotCrossTrack;
                    }
                }
            }
            return toolErrors;
        }

        public override bool CanUpgradeTo(NetInfo info)
        {
            return this.m_connectedInfo != info && this.m_elevatedInfo != info && this.m_bridgeInfo != info && this.m_tunnelInfo != info && this.m_slopeInfo != info && base.CanUpgradeTo(info);
        }
    }
}
