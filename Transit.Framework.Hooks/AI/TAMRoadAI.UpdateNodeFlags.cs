using System;
using ColossalFramework;
using ColossalFramework.Math;
using Transit.Framework.ExtensionPoints.AI;
using Transit.Framework.Redirection;
using UnityEngine;
using static Transit.Framework.NetInfoExtensions;

namespace Transit.Framework.Hooks.AI
{
    public partial class TAMRoadAI : RoadAI
    {
        // RoadBaseAI
        [RedirectFrom(typeof(RoadAI))]
        public override void UpdateNodeFlags(ushort nodeID, ref NetNode data)
        {
            NetNode.Flags flags = data.m_flags;
            uint num = 0u;
            int num2 = 0;
            NetManager instance = Singleton<NetManager>.instance;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            bool flag = this.WantTrafficLights();
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            int num6 = 0;
            int num7 = 0;
            int num11 = 0;
            int num12 = 0;
            int num13 = 0;
            for (int i = 0; i < 8; i++)
            {
                ushort segment = data.GetSegment(i);
                if (segment != 0)
                {
                    NetInfo info = instance.m_segments.m_buffer[(int)segment].Info;
                    if (info != null)
                    {
                        uint num8 = 1u << (int)info.m_class.m_level;
                        if ((num & num8) == 0u)
                        {
                            num |= num8;
                            num2++;
                        }
                        if (info.m_netAI.WantTrafficLights())
                        {
                            flag = true;
                        }
                        if ((info.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None != ((this.m_info.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None))
                        {
                            flag2 = true;
                        }
                        int num9 = 0;
                        int num10 = 0;
                        instance.m_segments.m_buffer[(int)segment].CountLanes(segment, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, VehicleInfo.VehicleType.Car | VehicleInfo.VehicleType.Tram, ref num9, ref num10);
                        if (instance.m_segments.m_buffer[(int)segment].m_endNode == nodeID == ((instance.m_segments.m_buffer[(int)segment].m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None))
                        {
                            if (num9 != 0)
                            {
                                num3++;
                                num4 += num9;
                                if (info.m_connectGroup != NetInfo.ConnectGroup.None)
                                {
                                    num13++;
                                    flag3 = true;
                                }
                            }
                        }
                        else if (num10 != 0)
                        {
                            num3++;
                            num4 += num10;
                            if (info.m_connectGroup != NetInfo.ConnectGroup.None)
                            {
                                num13++;
                                flag4 = true;
                            }
                        }
                        if (num9 != 0 || num10 != 0)
                        {
                            num5++;
                        }
                        if (info.m_class.m_service == ItemClass.Service.Road)
                        {
                            num6++;
                        }
                        else if ((info.m_vehicleTypes & VehicleInfo.VehicleType.Train) != VehicleInfo.VehicleType.None)
                        {
                            num7++;
                        }
                    }
                }
            }
            if (num6 >= 1 && num7 >= 1)
            {
                flags &= (NetNode.Flags.Created | NetNode.Flags.Deleted | NetNode.Flags.Original | NetNode.Flags.Disabled | NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend | NetNode.Flags.Junction | NetNode.Flags.Moveable | NetNode.Flags.Untouchable | NetNode.Flags.Outside | NetNode.Flags.Temporary | NetNode.Flags.Double | NetNode.Flags.Fixed | NetNode.Flags.OnGround | NetNode.Flags.Ambiguous | NetNode.Flags.Water | NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection | NetNode.Flags.Underground | NetNode.Flags.Transition | NetNode.Flags.LevelCrossing | NetNode.Flags.OneWayOut | NetNode.Flags.TrafficLights | NetNode.Flags.OneWayIn | NetNode.Flags.Heating | NetNode.Flags.Electricity | NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle | NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward);
                if (num6 >= 1 && num7 >= 2)
                {
                    flags |= (NetNode.Flags.LevelCrossing | NetNode.Flags.TrafficLights);
                }
                else
                {
                    flags &= ~(NetNode.Flags.LevelCrossing | NetNode.Flags.TrafficLights);
                }
                if (num2 >= 2 || flag2)
                {
                    flags |= NetNode.Flags.Transition;
                }
                else
                {
                    flags &= ~NetNode.Flags.Transition;
                }
            }
            else
            {
                flags &= ~NetNode.Flags.LevelCrossing;
                if (num2 >= 2 || flag2)
                {
                    flags |= NetNode.Flags.Transition;
                }
                else
                {
                    flags &= ~NetNode.Flags.Transition;
                }
                if (flag)
                {
                    flag = ((num3 > 2 || (num3 >= 2 && num5 >= 3 && num4 > 6)) && (flags & NetNode.Flags.Junction) != NetNode.Flags.None);
                }
                if ((flags & NetNode.Flags.CustomTrafficLights) != NetNode.Flags.None)
                {
                    if (!this.CanEnableTrafficLights(nodeID, ref data))
                    {
                        flags &= (NetNode.Flags.Created | NetNode.Flags.Deleted | NetNode.Flags.Original | NetNode.Flags.Disabled | NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend | NetNode.Flags.Junction | NetNode.Flags.Moveable | NetNode.Flags.Untouchable | NetNode.Flags.Outside | NetNode.Flags.Temporary | NetNode.Flags.Double | NetNode.Flags.Fixed | NetNode.Flags.OnGround | NetNode.Flags.Ambiguous | NetNode.Flags.Water | NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection | NetNode.Flags.Underground | NetNode.Flags.Transition | NetNode.Flags.LevelCrossing | NetNode.Flags.OneWayOut | NetNode.Flags.OneWayIn | NetNode.Flags.Heating | NetNode.Flags.Electricity | NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle | NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward);
                    }
                    else if (flag == ((data.m_flags & NetNode.Flags.TrafficLights) != NetNode.Flags.None))
                    {
                        flags &= (NetNode.Flags.Created | NetNode.Flags.Deleted | NetNode.Flags.Original | NetNode.Flags.Disabled | NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend | NetNode.Flags.Junction | NetNode.Flags.Moveable | NetNode.Flags.Untouchable | NetNode.Flags.Outside | NetNode.Flags.Temporary | NetNode.Flags.Double | NetNode.Flags.Fixed | NetNode.Flags.OnGround | NetNode.Flags.Ambiguous | NetNode.Flags.Water | NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection | NetNode.Flags.Underground | NetNode.Flags.Transition | NetNode.Flags.LevelCrossing | NetNode.Flags.OneWayOut | NetNode.Flags.TrafficLights | NetNode.Flags.OneWayIn | NetNode.Flags.Heating | NetNode.Flags.Electricity | NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle | NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward);
                    }
                }
                else if (flag)
                {
                    flags |= NetNode.Flags.TrafficLights;
                }
                else
                {
                    flags &= ~NetNode.Flags.TrafficLights;
                }
                if (num6 == 2 && num13 == 2)
                {
                    //if ((flags & NetNode.Flags.CustomTrafficLights) == NetNode.Flags.None)
                    //{
                    if ((flags & NetNode.Flags.TrafficLights) == NetNode.Flags.None)
                    {
                        if (flag3 && flag4)
                        {
                            flags |= NetNode.Flags.AsymForward;
                            flags &= ~NetNode.Flags.AsymBackward;
                        }
                        else if (flag3 != flag4)
                        {
                            flags |= NetNode.Flags.AsymBackward;
                            flags &= ~NetNode.Flags.AsymForward;
                        }
                    }
                    //}
                }
            }
            data.m_flags = flags;
        }

        private bool CanEnableTrafficLights(ushort nodeID, ref NetNode data)
        {
            if ((data.m_flags & NetNode.Flags.Junction) == NetNode.Flags.None)
            {
                return false;
            }
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            NetManager instance = Singleton<NetManager>.instance;
            for (int i = 0; i < 8; i++)
            {
                ushort segment = data.GetSegment(i);
                if (segment != 0)
                {
                    NetInfo info = instance.m_segments.m_buffer[(int)segment].Info;
                    if (info.m_class.m_service == ItemClass.Service.Road)
                    {
                        num++;
                    }
                    else if ((info.m_vehicleTypes & VehicleInfo.VehicleType.Train) != VehicleInfo.VehicleType.None)
                    {
                        num2++;
                    }
                    if (info.m_hasPedestrianLanes)
                    {
                        num3++;
                    }
                }
            }
            return (num < 1 || num2 < 1) && ((data.m_flags & NetNode.Flags.OneWayIn) == NetNode.Flags.None || num3 != 0);
        }
    }
}