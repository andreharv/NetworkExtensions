using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transit.Framework.Redirection;
using TrafficManager.Manager.Impl;
using ColossalFramework;
using System.Reflection;

namespace Transit.Framework.Hooks.AI
{
    public partial class TAMRoadAI : RoadAI
    {
        [RedirectFrom(typeof(RoadAI))]
        public override void UpdateNodeFlags(ushort nodeID, ref NetNode data)
        {
            NetNode.Flags flags1 = data.m_flags;
            uint num1 = 0;
            int num2 = 0;
            NetManager instance = Singleton<NetManager>.instance;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            bool flag1 = this.WantTrafficLights();
            bool flag2 = false;
            int num6 = 0;
            int num7 = 0;
            int num9 = 0;
            for (int index = 0; index < 8; ++index)
            {
                ushort segment = data.GetSegment(index);
                if ((int)segment != 0)
                {
                    NetInfo info = instance.m_segments.m_buffer[(int)segment].Info;
                    if (info != null)
                    {
                        uint num8 = 1U << (int)(info.m_class.m_level & (ItemClass.Level)31);
                        if (((int)num1 & (int)num8) == 0)
                        {
                            num1 |= num8;
                            ++num2;
                        }
                        if (info.m_netAI.WantTrafficLights())
                            flag1 = true;
                        if ((info.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None != ((this.m_info.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None))
                            flag2 = true;
                        int forward = 0;
                        int backward = 0;
                        instance.m_segments.m_buffer[(int)segment].CountLanes(segment, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, VehicleInfo.VehicleType.Car | VehicleInfo.VehicleType.Tram, ref forward, ref backward);
                        if ((int)instance.m_segments.m_buffer[(int)segment].m_endNode == (int)nodeID)
                        {
                            if (forward != 0)
                            {
                                ++num3;
                                num4 += forward;
                            }
                        }
                        else if (backward != 0)
                        {
                            ++num3;
                            num4 += backward;
                        }
                        if (forward != 0 || backward != 0)
                            ++num5;
                        if (info.m_class.m_service == ItemClass.Service.Road)
                            ++num6;
                        else if ((info.m_vehicleTypes & VehicleInfo.VehicleType.Train) != VehicleInfo.VehicleType.None)
                            ++num7;

                        if (info.name == "FourDevidedLaneAvenue4Parking")
                        {
                            ++num9;
                        }
                    }
                }
            }
            NetNode.Flags flags2;
            if (num6 >= 1 && num7 >= 1)
            {
                NetNode.Flags flags3 = flags1 & (NetNode.Flags.OneWayOutTrafficLights | NetNode.Flags.UndergroundTransition | NetNode.Flags.Created | NetNode.Flags.Deleted | NetNode.Flags.Original | NetNode.Flags.Disabled | NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend | NetNode.Flags.Junction | NetNode.Flags.Moveable | NetNode.Flags.Untouchable | NetNode.Flags.Outside | NetNode.Flags.Temporary | NetNode.Flags.Double | NetNode.Flags.Fixed | NetNode.Flags.OnGround | NetNode.Flags.Ambiguous | NetNode.Flags.Water | NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection | NetNode.Flags.LevelCrossing | NetNode.Flags.OneWayIn | NetNode.Flags.Heating | NetNode.Flags.Electricity | NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle | NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward);
                NetNode.Flags flags4 = num6 < 1 || num7 < 2 ? flags3 & (NetNode.Flags.UndergroundTransition | NetNode.Flags.Created | NetNode.Flags.Deleted | NetNode.Flags.Original | NetNode.Flags.Disabled | NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend | NetNode.Flags.Junction | NetNode.Flags.Moveable | NetNode.Flags.Untouchable | NetNode.Flags.Outside | NetNode.Flags.Temporary | NetNode.Flags.Double | NetNode.Flags.Fixed | NetNode.Flags.OnGround | NetNode.Flags.Ambiguous | NetNode.Flags.Water | NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection | NetNode.Flags.OneWayOut | NetNode.Flags.OneWayIn | NetNode.Flags.Heating | NetNode.Flags.Electricity | NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle | NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward | NetNode.Flags.CustomTrafficLights) : flags3 | NetNode.Flags.LevelCrossing | NetNode.Flags.TrafficLights;
                flags2 = num2 >= 2 || flag2 ? flags4 | NetNode.Flags.Transition : flags4 & (NetNode.Flags.OneWayOutTrafficLights | NetNode.Flags.Created | NetNode.Flags.Deleted | NetNode.Flags.Original | NetNode.Flags.Disabled | NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend | NetNode.Flags.Junction | NetNode.Flags.Moveable | NetNode.Flags.Untouchable | NetNode.Flags.Outside | NetNode.Flags.Temporary | NetNode.Flags.Double | NetNode.Flags.Fixed | NetNode.Flags.OnGround | NetNode.Flags.Ambiguous | NetNode.Flags.Water | NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection | NetNode.Flags.Underground | NetNode.Flags.LevelCrossing | NetNode.Flags.OneWayIn | NetNode.Flags.Heating | NetNode.Flags.Electricity | NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle | NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward | NetNode.Flags.CustomTrafficLights);
            }
            else
            {
                NetNode.Flags flags3 = flags1 & (NetNode.Flags.OneWayOutTrafficLights | NetNode.Flags.UndergroundTransition | NetNode.Flags.Created | NetNode.Flags.Deleted | NetNode.Flags.Original | NetNode.Flags.Disabled | NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend | NetNode.Flags.Junction | NetNode.Flags.Moveable | NetNode.Flags.Untouchable | NetNode.Flags.Outside | NetNode.Flags.Temporary | NetNode.Flags.Double | NetNode.Flags.Fixed | NetNode.Flags.OnGround | NetNode.Flags.Ambiguous | NetNode.Flags.Water | NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection | NetNode.Flags.OneWayIn | NetNode.Flags.Heating | NetNode.Flags.Electricity | NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle | NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward | NetNode.Flags.CustomTrafficLights);
                flags2 = num2 >= 2 || flag2 ? flags3 | NetNode.Flags.Transition : flags3 & (NetNode.Flags.OneWayOutTrafficLights | NetNode.Flags.Created | NetNode.Flags.Deleted | NetNode.Flags.Original | NetNode.Flags.Disabled | NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend | NetNode.Flags.Junction | NetNode.Flags.Moveable | NetNode.Flags.Untouchable | NetNode.Flags.Outside | NetNode.Flags.Temporary | NetNode.Flags.Double | NetNode.Flags.Fixed | NetNode.Flags.OnGround | NetNode.Flags.Ambiguous | NetNode.Flags.Water | NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection | NetNode.Flags.Underground | NetNode.Flags.LevelCrossing | NetNode.Flags.OneWayIn | NetNode.Flags.Heating | NetNode.Flags.Electricity | NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle | NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward | NetNode.Flags.CustomTrafficLights);
                if (flag1)
                    flag1 = (num3 > 2 || num3 >= 2 && num5 >= 3 && num4 > 6) && (flags2 & NetNode.Flags.Junction) != NetNode.Flags.None;
                if ((flags2 & NetNode.Flags.CustomTrafficLights) != NetNode.Flags.None)
                {
                    if (!this.CanEnableTrafficLights(nodeID, ref data))
                        flags2 &= NetNode.Flags.UndergroundTransition | NetNode.Flags.Created | NetNode.Flags.Deleted | NetNode.Flags.Original | NetNode.Flags.Disabled | NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend | NetNode.Flags.Junction | NetNode.Flags.Moveable | NetNode.Flags.Untouchable | NetNode.Flags.Outside | NetNode.Flags.Temporary | NetNode.Flags.Double | NetNode.Flags.Fixed | NetNode.Flags.OnGround | NetNode.Flags.Ambiguous | NetNode.Flags.Water | NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection | NetNode.Flags.LevelCrossing | NetNode.Flags.OneWayOut | NetNode.Flags.OneWayIn | NetNode.Flags.Heating | NetNode.Flags.Electricity | NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle | NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward;
                    else if (flag1 == ((data.m_flags & NetNode.Flags.TrafficLights) != NetNode.Flags.None))
                        flags2 &= NetNode.Flags.OneWayOutTrafficLights | NetNode.Flags.UndergroundTransition | NetNode.Flags.Created | NetNode.Flags.Deleted | NetNode.Flags.Original | NetNode.Flags.Disabled | NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend | NetNode.Flags.Junction | NetNode.Flags.Moveable | NetNode.Flags.Untouchable | NetNode.Flags.Outside | NetNode.Flags.Temporary | NetNode.Flags.Double | NetNode.Flags.Fixed | NetNode.Flags.OnGround | NetNode.Flags.Ambiguous | NetNode.Flags.Water | NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection | NetNode.Flags.LevelCrossing | NetNode.Flags.OneWayIn | NetNode.Flags.Heating | NetNode.Flags.Electricity | NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle | NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward;
                }
                else if (flag1)
                    flags2 |= NetNode.Flags.TrafficLights;
                else
                    flags2 &= NetNode.Flags.UndergroundTransition | NetNode.Flags.Created | NetNode.Flags.Deleted | NetNode.Flags.Original | NetNode.Flags.Disabled | NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend | NetNode.Flags.Junction | NetNode.Flags.Moveable | NetNode.Flags.Untouchable | NetNode.Flags.Outside | NetNode.Flags.Temporary | NetNode.Flags.Double | NetNode.Flags.Fixed | NetNode.Flags.OnGround | NetNode.Flags.Ambiguous | NetNode.Flags.Water | NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection | NetNode.Flags.LevelCrossing | NetNode.Flags.OneWayOut | NetNode.Flags.OneWayIn | NetNode.Flags.Heating | NetNode.Flags.Electricity | NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle | NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward | NetNode.Flags.CustomTrafficLights;
            }
            data.m_flags = flags2;
            //if (num9 == 2)
            //{
            //    System.Reflection.MethodInfo getLanesMethod = LaneConnectionManager.Instance.GetType().GetMethod("GetLaneConnections", BindingFlags.NonPublic | BindingFlags.Instance);
            //    System.Reflection.MethodInfo removeLaneMethod = LaneConnectionManager.Instance.GetType().GetMethod("RemoveLaneConnection", BindingFlags.NonPublic | BindingFlags.Instance);
            //    if (getLanesMethod != null && removeLaneMethod != null)
            //    {
            //        UnityEngine.Debug.Log("YOUR METHOD IS GOOOOOOD");
            //        for (int i = 0; i < 2; i++)
            //        {
            //            ushort segment = data.GetSegment(i);
            //            if ((int)segment != 0)
            //            {
            //                uint num10 = instance.m_segments.m_buffer[(int)segment].m_lanes;
            //                for (int j = 0; j <= 1; j++)
            //                {
            //                    bool isStartNode = j == 0;
            //                    object[] args = new object[2];
            //                    args[0] = num10;
            //                    args[1] = isStartNode;
            //                    object lanesObj = getLanesMethod.Invoke(LaneConnectionManager.Instance, args);
            //                    IEnumerable<uint> ie = lanesObj as IEnumerable<uint>;
            //                    if (ie != null)
            //                    {
            //                        uint[] lanes = ie.ToArray();
            //                        Debug.Log(lanes.Count() + "To Remove");
            //                        if (lanes.Length > 0)
            //                        {
            //                            for (int k = 0; k < lanes.Length; k++)
            //                            {
            //                                uint ln = lanes[k];
            //                                object[] args2 = new object[3];
            //                                args2[0] = num10;
            //                                args2[1] = ln;
            //                                args2[2] = isStartNode;
            //                                removeLaneMethod.Invoke(LaneConnectionManager.Instance, args2);
            //                                UnityEngine.Debug.Log("Removed!!!");
            //                                //Transit.Framework.Debug.Log("TIS LANE" + ln + "FOR " + m_info.name);
            //                            }
            //                        }
            //                    }

            //                }
            //                num10 = instance.m_lanes.m_buffer[num10].m_nextLane;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        UnityEngine.Debug.Log("METHODS MALLOOOOO!!!");
            //    }
            //}
        }

        private bool CanEnableTrafficLights(ushort nodeID, ref NetNode data)
        {
            if ((data.m_flags & NetNode.Flags.Junction) == NetNode.Flags.None)
                return false;
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            NetManager instance = Singleton<NetManager>.instance;
            for (int index = 0; index < 8; ++index)
            {
                ushort segment = data.GetSegment(index);
                if ((int)segment != 0)
                {
                    NetInfo info = instance.m_segments.m_buffer[(int)segment].Info;
                    if (info.m_class.m_service == ItemClass.Service.Road)
                        ++num1;
                    else if ((info.m_vehicleTypes & VehicleInfo.VehicleType.Train) != VehicleInfo.VehicleType.None)
                        ++num2;
                    if (info.m_hasPedestrianLanes)
                        ++num3;
                }
            }
            return (num1 < 1 || num2 < 1) && ((data.m_flags & NetNode.Flags.OneWayIn) == NetNode.Flags.None || num3 != 0);
        }
    }
}





