using ColossalFramework.Math;
using System;
using System.Runtime.CompilerServices;
using Transit.Framework.Unsafe;
using UnityEngine;

using VehicleType = VehicleInfo.VehicleType;
using LaneType = NetInfo.LaneType;

namespace Transit.Addon.TrafficAI.AI
{
    public class TAMCarAI : VehicleAI
    {
        // This is the 31th bit in Vehicle.Flags. If CO adds more flags, we'll need to review this
        public const Vehicle.Flags AvoidingCongestionFlag = (Vehicle.Flags)((int)Vehicle.Flags.Transition << 1);

        [RedirectFrom(typeof(CarAI), (uint)TrafficAIModule.Options.CongestionAvoidance)]
        public override void SimulationStep(ushort vehicleID, ref Vehicle vehicleData, ref Vehicle.Frame frameData, ushort leaderID, ref Vehicle leaderData, int lodPhysics)
        {
            uint currentFrameIndex = SimulationManager.instance.m_currentFrameIndex;

            frameData.m_position += frameData.m_velocity * 0.5f;
            frameData.m_swayPosition += frameData.m_swayVelocity * 0.5f;

            float acceleration = this.m_info.m_acceleration;
            float braking = this.m_info.m_braking;
            float speed = frameData.m_velocity.magnitude;
            Vector3 rawDirection = (Vector3)vehicleData.m_targetPos0 - frameData.m_position;
            float sqrDistance = rawDirection.sqrMagnitude;
            float minDistance = (speed + acceleration) * (0.5f + 0.5f * (speed + acceleration) / braking) + this.m_info.m_generatedInfo.m_size.z * 0.5f;
            float maxSpeed = Mathf.Max(speed + acceleration, 5f);

            if (lodPhysics >= 2 && (currentFrameIndex >> 4 & 3) == (vehicleID & 3))
            {
                maxSpeed *= 2f;
            }

            float num3 = Mathf.Max((minDistance - maxSpeed) / 3f, 1f);
            float minSqrDistanceA = maxSpeed * maxSpeed;
            float minSqrDistanceB = num3 * num3;
            int index = 0;
            bool flag = false;

            if ((sqrDistance < minSqrDistanceA || vehicleData.m_targetPos3.w < 0.01f) && (leaderData.m_flags & (Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped)) == Vehicle.Flags.None)
            {
                if (leaderData.m_path != 0u)
                {
                    base.UpdatePathTargetPositions(vehicleID, ref vehicleData, frameData.m_position, ref index, 4, minSqrDistanceA, minSqrDistanceB);
                    if ((leaderData.m_flags & Vehicle.Flags.Spawned) == Vehicle.Flags.None)
                    {
                        frameData = vehicleData.m_frame0;
                        return;
                    }
                }

                if ((leaderData.m_flags & Vehicle.Flags.WaitingPath) == Vehicle.Flags.None)
                {
                    while (index < 4)
                    {
                        float minSqrDistance;
                        Vector3 refPos;
                        if (index == 0)
                        {
                            minSqrDistance = minSqrDistanceA;
                            refPos = frameData.m_position;
                            flag = true;
                        }
                        else
                        {
                            minSqrDistance = minSqrDistanceB;
                            refPos = vehicleData.GetTargetPos(index - 1);
                        }

                        int previousIndex = index;
                        this.UpdateBuildingTargetPositions(vehicleID, ref vehicleData, refPos, leaderID, ref leaderData, ref index, minSqrDistance);
                        if (index == previousIndex)
                        {
                            break;
                        }
                    }

                    if (index != 0)
                    {
                        Vector4 targetPos = vehicleData.GetTargetPos(index - 1);
                        while (index < 4)
                        {
                            vehicleData.SetTargetPos(index++, targetPos);
                        }
                    }
                }

                rawDirection = (Vector3)vehicleData.m_targetPos0 - frameData.m_position;
                sqrDistance = rawDirection.sqrMagnitude;
            }

            if (leaderData.m_path != 0u && (leaderData.m_flags & Vehicle.Flags.WaitingPath) == Vehicle.Flags.None)
            {
                NetManager netManager = NetManager.instance;
                PathManager pathManager = PathManager.instance;

                byte pathIndex = leaderData.m_pathPositionIndex;
                byte lastTValue = leaderData.m_lastPathOffset;
                if (pathIndex == 255)
                {
                    pathIndex = 0;
                }
                float vehicleLength = 1f + leaderData.CalculateTotalLength(leaderID);
                
                PathUnit.Position pathPos;
                if (pathManager.m_pathUnits.m_buffer[leaderData.m_path].GetPosition(pathIndex >> 1, out pathPos))
                {
                    netManager.m_segments.m_buffer[pathPos.m_segment].AddTraffic(Mathf.RoundToInt(vehicleLength * 2.5f));

                    bool reservedSpace = false;
                    if ((pathIndex & 1) == 0 || lastTValue == 0)
                    {
                        uint laneID = PathManager.GetLaneID(pathPos);
                        if (laneID != 0u)
                        {
                            Vector3 lanePos = netManager.m_lanes.m_buffer[laneID].CalculatePosition((float)pathPos.m_offset * 0.003921569f);
                            float minDist = 0.5f * speed * speed / this.m_info.m_braking + this.m_info.m_generatedInfo.m_size.z * 0.5f;
                            if (Vector3.Distance(frameData.m_position, lanePos) >= minDist - 1f)
                            {
                                netManager.m_lanes.m_buffer[laneID].ReserveSpace(vehicleLength);
                                reservedSpace = true;
                            }
                        }
                    }

                    if (!reservedSpace && pathManager.m_pathUnits.m_buffer[leaderData.m_path].GetNextPosition(pathIndex >> 1, out pathPos))
                    {
                        uint nextLaneID = PathManager.GetLaneID(pathPos);
                        if (nextLaneID != 0u)
                        {
                            netManager.m_lanes.m_buffer[nextLaneID].ReserveSpace(vehicleLength);
                        }
                    }
                }

                if ((currentFrameIndex >> 4 & 15) == (leaderID & 15))
                {
                    bool congested = false;
                    uint pathUnitID = leaderData.m_path;
                    int pathID = pathIndex >> 1;
                    int j = 0;
                    while (j < 5)
                    {
                        bool invalid;
                        if (PathUnit.GetNextPosition(ref pathUnitID, ref pathID, out pathPos, out invalid))
                        {
                            uint laneID3 = PathManager.GetLaneID(pathPos);
                            if (laneID3 != 0u && !netManager.m_lanes.m_buffer[(int)((UIntPtr)laneID3)].CheckSpace(vehicleLength))
                            {
                                j++;
                                continue;
                            }
                        }
                        if (invalid)
                        {
                            this.InvalidPath(vehicleID, ref vehicleData, leaderID, ref leaderData);
                        }
                        congested = true;
                        break;
                    }
                    if (!congested)
                    {
                        leaderData.m_flags |= Vehicle.Flags.Congestion;
                    }
                }
            }
            float targetSpeed;
            if ((leaderData.m_flags & Vehicle.Flags.Stopped) != Vehicle.Flags.None)
            {
                targetSpeed = 0f;
            }
            else
            {
                targetSpeed = vehicleData.m_targetPos0.w;
            }
            Quaternion rotation = Quaternion.Inverse(frameData.m_rotation);
            rawDirection = rotation * rawDirection;
            Vector3 velocity = rotation * frameData.m_velocity;
            Vector3 dir = Vector3.forward;
            Vector3 newVelocity = Vector3.zero;
            Vector3 collisionPush = Vector3.zero;
            float finalSpeed = 0f;
            float steerAngle = 0f;
            bool blocked = false;
            float dist = 0f;
            if (sqrDistance > 1f)
            {
                dir = VectorUtils.NormalizeXZ(rawDirection, out dist);
                if (dist > 1f)
                {
                    Vector3 newDirection = rawDirection;
                    maxSpeed = Mathf.Max(speed, 2f);
                    minSqrDistanceA = maxSpeed * maxSpeed;
                    if (sqrDistance > minSqrDistanceA)
                    {
                        newDirection *= maxSpeed / Mathf.Sqrt(sqrDistance);
                    }
                    bool reversing = false;
                    if (newDirection.z < Mathf.Abs(newDirection.x))
                    {
                        if (newDirection.z < 0f)
                        {
                            reversing = true;
                        }
                        float absX = Mathf.Abs(newDirection.x);
                        if (absX < 1f)
                        {
                            newDirection.x = Mathf.Sign(newDirection.x);
                            if (newDirection.x == 0f)
                            {
                                newDirection.x = 1f;
                            }
                            absX = 1f;
                        }
                        newDirection.z = absX;
                    }
                    float dist2;
                    dir = VectorUtils.NormalizeXZ(newDirection, out dist2);
                    dist = Mathf.Min(dist, dist2);
                    float curve = 1.57079637f * (1f - dir.z);
                    if (dist > 1f)
                    {
                        curve /= dist;
                    }
                    float targetDist = dist;
                    if (vehicleData.m_targetPos0.w < 0.1f)
                    {
                        targetSpeed = this.CalculateTargetSpeed(vehicleID, ref vehicleData, 1000f, curve);
                        targetSpeed = Mathf.Min(targetSpeed, CalculateMaxSpeed(targetDist, Mathf.Min(vehicleData.m_targetPos0.w, vehicleData.m_targetPos1.w), braking * 0.9f));
                    }
                    else
                    {
                        targetSpeed = Mathf.Min(targetSpeed, this.CalculateTargetSpeed(vehicleID, ref vehicleData, 1000f, curve));
                        targetSpeed = Mathf.Min(targetSpeed, CalculateMaxSpeed(targetDist, vehicleData.m_targetPos1.w, braking * 0.9f));
                    }
                    targetDist += VectorUtils.LengthXZ(vehicleData.m_targetPos1 - vehicleData.m_targetPos0);
                    targetSpeed = Mathf.Min(targetSpeed, CalculateMaxSpeed(targetDist, vehicleData.m_targetPos2.w, braking * 0.9f));
                    targetDist += VectorUtils.LengthXZ(vehicleData.m_targetPos2 - vehicleData.m_targetPos1);
                    targetSpeed = Mathf.Min(targetSpeed, CalculateMaxSpeed(targetDist, vehicleData.m_targetPos3.w, braking * 0.9f));
                    targetDist += VectorUtils.LengthXZ(vehicleData.m_targetPos3 - vehicleData.m_targetPos2);
                    if (vehicleData.m_targetPos3.w < 0.01f)
                    {
                        targetDist = Mathf.Max(0f, targetDist - this.m_info.m_generatedInfo.m_size.z * 0.5f);
                    }
                    targetSpeed = Mathf.Min(targetSpeed, CalculateMaxSpeed(targetDist, 0f, braking * 0.9f));
                    if (!DisableCollisionCheck(leaderID, ref leaderData))
                    {
                        this.CheckOtherVehicles(vehicleID, ref vehicleData, ref frameData, ref targetSpeed, ref blocked, ref collisionPush, minDistance, braking * 0.9f, lodPhysics);
                    }
                    if (reversing)
                    {
                        targetSpeed = -targetSpeed;
                    }
                    if (targetSpeed < speed)
                    {
                        float deltaSpeed = Mathf.Max(acceleration, Mathf.Min(braking, speed));
                        finalSpeed = Mathf.Max(targetSpeed, speed - deltaSpeed);
                    }
                    else
                    {
                        float deltaSpeed = Mathf.Max(acceleration, Mathf.Min(braking, -speed));
                        finalSpeed = Mathf.Min(targetSpeed, speed + deltaSpeed);
                    }
                }
            }
            else if (speed < 0.1f && flag && this.ArriveAtDestination(leaderID, ref leaderData))
            {
                leaderData.Unspawn(leaderID);
                if (leaderID == vehicleID)
                {
                    frameData = leaderData.m_frame0;
                }
                return;
            }
            if ((leaderData.m_flags & Vehicle.Flags.Stopped) == Vehicle.Flags.None && targetSpeed < 0.1f)
            {
                blocked = true;
            }
            if (blocked)
            {
                vehicleData.m_blockCounter = (byte)Mathf.Min((int)(vehicleData.m_blockCounter + 1), 255);
            }
            else
            {
                vehicleData.m_blockCounter = 0;
            }
            if (dist > 1f)
            {
                steerAngle = Mathf.Asin(dir.x) * Mathf.Sign(finalSpeed);
                newVelocity = dir * finalSpeed;
            }
            else
            {
                finalSpeed = 0f;
                Vector3 b4 = Vector3.ClampMagnitude(rawDirection * 0.5f - velocity, braking);
                newVelocity = velocity + b4;
            }
            bool flag7 = (currentFrameIndex + (uint)leaderID & 16u) != 0u;
            Vector3 deltaVelocity = newVelocity - velocity;
            Vector3 finalVelocity = frameData.m_rotation * newVelocity;
            frameData.m_velocity = finalVelocity + collisionPush;
            frameData.m_position += frameData.m_velocity * 0.5f;
            frameData.m_swayVelocity = frameData.m_swayVelocity * (1f - this.m_info.m_dampers) - deltaVelocity * (1f - this.m_info.m_springs) - frameData.m_swayPosition * this.m_info.m_springs;
            frameData.m_swayPosition += frameData.m_swayVelocity * 0.5f;
            frameData.m_steerAngle = steerAngle;
            frameData.m_travelDistance += newVelocity.z;
            frameData.m_lightIntensity.x = 5f;
            frameData.m_lightIntensity.y = ((deltaVelocity.z >= -0.1f) ? 0.5f : 5f);
            frameData.m_lightIntensity.z = ((steerAngle >= -0.1f || !flag7) ? 0f : 5f);
            frameData.m_lightIntensity.w = ((steerAngle <= 0.1f || !flag7) ? 0f : 5f);
            frameData.m_underground = ((vehicleData.m_flags & Vehicle.Flags.Underground) != Vehicle.Flags.None);
            frameData.m_transition = ((vehicleData.m_flags & Vehicle.Flags.Transition) != Vehicle.Flags.None);
            if ((vehicleData.m_flags & Vehicle.Flags.Parking) != Vehicle.Flags.None && dist <= 1f && flag)
            {
                Vector3 forward = vehicleData.m_targetPos1 - vehicleData.m_targetPos0;
                if (forward.sqrMagnitude > 0.01f)
                {
                    frameData.m_rotation = Quaternion.LookRotation(forward);
                }
            }
            else if (finalSpeed > 0.1f)
            {
                if (finalVelocity.sqrMagnitude > 0.01f)
                {
                    frameData.m_rotation = Quaternion.LookRotation(finalVelocity);
                }
            }
            else if (finalSpeed < -0.1f && finalVelocity.sqrMagnitude > 0.01f)
            {
                frameData.m_rotation = Quaternion.LookRotation(-finalVelocity);
            }
            base.SimulationStep(vehicleID, ref vehicleData, ref frameData, leaderID, ref leaderData, lodPhysics);
        }

        [RedirectFrom(typeof(CarAI))]
        protected override void CalculateSegmentPosition(ushort vehicleID, ref Vehicle vehicleData, PathUnit.Position nextPosition, PathUnit.Position position, uint laneID, byte offset, PathUnit.Position prevPos, uint prevLaneID, byte prevOffset, out Vector3 pos, out Vector3 dir, out float maxSpeed)
        {
            NetManager netManager = NetManager.instance;
            netManager.m_lanes.m_buffer[laneID].CalculatePositionAndDirection((float)offset * 0.003921569f, out pos, out dir);
            Vehicle.Frame lastFrameData = vehicleData.GetLastFrameData();
            Vector3 currentPos = lastFrameData.m_position;
            Vector3 previousPos = netManager.m_lanes.m_buffer[prevLaneID].CalculatePosition((float)prevOffset * 0.003921569f);

            float minDistance = 0.5f * lastFrameData.m_velocity.sqrMagnitude / this.m_info.m_braking + this.m_info.m_generatedInfo.m_size.z * 0.5f;

            if (Vector3.Distance(currentPos, previousPos) >= minDistance - 1f)
            {
                Segment3 segment;
                segment.a = pos;
                ushort startNode;
                ushort endNode;
                if (offset < position.m_offset)
                {
                    segment.b = pos + dir.normalized * this.m_info.m_generatedInfo.m_size.z;
                    startNode = netManager.m_segments.m_buffer[position.m_segment].m_startNode;
                    endNode = netManager.m_segments.m_buffer[position.m_segment].m_endNode;
                }
                else
                {
                    segment.b = pos - dir.normalized * this.m_info.m_generatedInfo.m_size.z;
                    startNode = netManager.m_segments.m_buffer[position.m_segment].m_endNode;
                    endNode = netManager.m_segments.m_buffer[position.m_segment].m_startNode;
                }

                ushort previousNode;
                if (prevOffset == 0)
                {
                    previousNode = netManager.m_segments.m_buffer[prevPos.m_segment].m_startNode;
                }
                else
                {
                    previousNode = netManager.m_segments.m_buffer[prevPos.m_segment].m_endNode;
                }

                if (startNode == previousNode)
                {
                    NetNode.Flags nodeFlags = netManager.m_nodes.m_buffer[startNode].m_flags;
                    NetLane.Flags laneFlags = (NetLane.Flags)netManager.m_lanes.m_buffer[prevLaneID].m_flags;

                    bool hasTrafficLights = (nodeFlags & NetNode.Flags.TrafficLights) != NetNode.Flags.None;
                    bool isLevelCrossing = (nodeFlags & NetNode.Flags.LevelCrossing) != NetNode.Flags.None;
                    bool isJoinedJunction = (laneFlags & NetLane.Flags.JoinedJunction) != NetLane.Flags.None;

                    if ((nodeFlags & (NetNode.Flags.Junction | NetNode.Flags.OneWayOut | NetNode.Flags.OneWayIn)) == NetNode.Flags.Junction && netManager.m_nodes.m_buffer[startNode].CountSegments() != 2)
                    {
                        float vehicleLength = vehicleData.CalculateTotalLength(vehicleID) + 2f;
                        if (!netManager.m_lanes.m_buffer[laneID].CheckSpace(vehicleLength))
                        {
                            bool laneHasSpace = false;
                            if (nextPosition.m_segment != 0 && netManager.m_lanes.m_buffer[laneID].m_length < 30f)
                            {
                                NetNode.Flags endNodeFlags = netManager.m_nodes.m_buffer[endNode].m_flags;
                                if ((endNodeFlags & (NetNode.Flags.Junction | NetNode.Flags.OneWayOut | NetNode.Flags.OneWayIn)) != NetNode.Flags.Junction || netManager.m_nodes.m_buffer[endNode].CountSegments() == 2)
                                {
                                    uint nextLaneID = PathManager.GetLaneID(nextPosition);
                                    if (nextLaneID != 0u)
                                    {
                                        laneHasSpace = netManager.m_lanes.m_buffer[nextLaneID].CheckSpace(vehicleLength);
                                    }
                                }
                            }

                            if (!laneHasSpace)
                            {
                                maxSpeed = 0f;
                                return;
                            }
                        }
                    }

                    if (hasTrafficLights && (!isJoinedJunction || isLevelCrossing))
                    {
                        uint currentFrameIndex = SimulationManager.instance.m_currentFrameIndex;

                        uint previousNodeOverMaxNodes = (uint)(((int)previousNode << 8) / 32768);
                        uint tValue = currentFrameIndex - previousNodeOverMaxNodes & 255u;
                        NetInfo seg = netManager.m_nodes.m_buffer[startNode].Info;
                        RoadBaseAI.TrafficLightState vehicleLightState;
                        RoadBaseAI.TrafficLightState pedestrianLightState;
                        bool vehicles;
                        bool pedestrians;
                        RoadBaseAI.GetTrafficLightState(previousNode, ref netManager.m_segments.m_buffer[prevPos.m_segment], currentFrameIndex - previousNodeOverMaxNodes, 
                                                        out vehicleLightState, out pedestrianLightState, out vehicles, out pedestrians);
                        if (!vehicles && tValue >= 196u)
                        {
                            vehicles = true;
                            RoadBaseAI.SetTrafficLightState(previousNode, ref netManager.m_segments.m_buffer[prevPos.m_segment], currentFrameIndex - previousNodeOverMaxNodes, vehicleLightState, pedestrianLightState, vehicles, pedestrians);
                        }
                        if ((vehicleData.m_flags & Vehicle.Flags.Emergency2) == Vehicle.Flags.None || seg.m_class.m_service != ItemClass.Service.Road)
                        {
                            switch (vehicleLightState)
                            {
                                case RoadBaseAI.TrafficLightState.RedToGreen:
                                    if (tValue < 60u)
                                    {
                                        maxSpeed = 0f;
                                        return;
                                    }
                                    break;
                                case RoadBaseAI.TrafficLightState.Red:
                                    maxSpeed = 0f;
                                    return;
                                case RoadBaseAI.TrafficLightState.GreenToRed:
                                    if (tValue >= 30u)
                                    {
                                        maxSpeed = 0f;
                                        return;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            NetInfo currentSegment = netManager.m_segments.m_buffer[(int)position.m_segment].Info;
            if (currentSegment.m_lanes != null && currentSegment.m_lanes.Length > (int)position.m_lane)
            {
                maxSpeed = this.CalculateTargetSpeed(vehicleID, ref vehicleData, currentSegment.m_lanes[(int)position.m_lane].m_speedLimit, netManager.m_lanes.m_buffer[(int)((UIntPtr)laneID)].m_curve);
            }
            else
            {
                maxSpeed = this.CalculateTargetSpeed(vehicleID, ref vehicleData, 1f, 0f);
            }
        }

        [RedirectFrom(typeof(AmbulanceAI))]
        [RedirectFrom(typeof(BusAI))]
        [RedirectFrom(typeof(CarAI))]
        [RedirectFrom(typeof(FireTruckAI))]
        [RedirectFrom(typeof(PoliceCarAI))]
        protected virtual bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData, Vector3 startPos, Vector3 endPos, bool startBothWays, bool endBothWays)
        {
            VehicleInfo info = this.m_info;
            bool allowUnderground = (vehicleData.m_flags & (Vehicle.Flags.Underground | Vehicle.Flags.Transition)) != Vehicle.Flags.None;
            PathUnit.Position startPosA;
            PathUnit.Position startPosB;
            float sqrDistStartA;
            float sqrDistStartB;
            PathUnit.Position endPosA;
            PathUnit.Position endPosB;
            float sqrDistEndA;
            float sqrDistEndB;
            if (PathManager.FindPathPosition(startPos, ItemClass.Service.Road, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, info.m_vehicleType, allowUnderground, false, 32f, out startPosA, out startPosB, out sqrDistStartA, out sqrDistStartB) && PathManager.FindPathPosition(endPos, ItemClass.Service.Road, NetInfo.LaneType.Vehicle | NetInfo.LaneType.TransportVehicle, info.m_vehicleType, false, false, 32f, out endPosA, out endPosB, out sqrDistEndA, out sqrDistEndB))
            {
                if (!startBothWays || sqrDistStartA < 10f)
                {
                    startPosB = default(PathUnit.Position);
                }
                if (!endBothWays || sqrDistEndA < 10f)
                {
                    endPosB = default(PathUnit.Position);
                }
                uint path;
                if (Singleton<PathManager>.instance.CreatePath(out path, ref Singleton<SimulationManager>.instance.m_randomizer, Singleton<SimulationManager>.instance.m_currentBuildIndex, startPosA, startPosB, endPosA, endPosB, NetInfo.LaneType.Vehicle, info.m_vehicleType, 20000f, this.IsHeavyVehicle(), this.IgnoreBlocked(vehicleID, ref vehicleData), false, false))
                {
                    if (vehicleData.m_path != 0u)
                    {
                        Singleton<PathManager>.instance.ReleasePath(vehicleData.m_path);
                    }
                    vehicleData.m_path = path;
                    vehicleData.m_flags |= Vehicle.Flags.WaitingPath;
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(CarAI))]
        private static float CalculateMaxSpeed(float targetDistance, float targetSpeed, float maxBraking)
        {
            throw new NotImplementedException("CalculateMaxSpeed is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(CarAI))]
        private static bool DisableCollisionCheck(ushort vehicleID, ref Vehicle vehicleData)
        {
            throw new NotImplementedException("DisableCollisionCheck is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(CarAI))]
        private void CheckOtherVehicles(ushort vehicleID, ref Vehicle vehicleData, ref Vehicle.Frame frameData, ref float maxSpeed, ref bool blocked, ref Vector3 collisionPush, float maxDistance, float maxBraking, int lodPhysics)
        {
            throw new NotImplementedException("CheckOtherVehicles is target of redirection and is not implemented.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectTo(typeof(CarAI))]
        protected virtual bool IsHeavyVehicle()
        {
            throw new NotImplementedException("IsHeavyVehicle is target of redirection and is not implemented.");
        }
    }
}
