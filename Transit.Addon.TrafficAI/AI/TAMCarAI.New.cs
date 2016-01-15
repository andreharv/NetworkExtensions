using System;
using System.Runtime.CompilerServices;
using ColossalFramework.Math;
using Transit.Framework.Unsafe;
using UnityEngine;

namespace Transit.Addon.TrafficAI.AI
{
    public class TAMCarAI : VehicleAI
    {
        // This is the 31th bit in Vehicle.Flags. If CO adds more flags, we'll need to review this
        public const Vehicle.Flags AvoidingCongestionFlag = (Vehicle.Flags)((int)Vehicle.Flags.Transition << 1);

        [RedirectFrom(typeof(CarAI), (ulong)TrafficAIModule.Options.CongestionAvoidance)]
        public override void SimulationStep(ushort vehicleID, ref Vehicle vehicleData, ref Vehicle.Frame frameData, ushort leaderID, ref Vehicle leaderData, int lodPhysics)
        {
            uint currentFrameIndex = SimulationManager.instance.m_currentFrameIndex;

            // apply half of the last calculated velocity. The first half is applied when it's calculated, at the end of this method
            frameData.m_position += frameData.m_velocity * 0.5f;
            frameData.m_swayPosition += frameData.m_swayVelocity * 0.5f;

            float acceleration = this.m_info.m_acceleration;
            float braking = this.m_info.m_braking;
            float speed = frameData.m_velocity.magnitude;
            Vector3 rawDirection = (Vector3)vehicleData.m_targetPos0 - frameData.m_position;
            float sqrDistance = rawDirection.sqrMagnitude;
            float maxDistance = (speed + acceleration) * (0.5f + 0.5f * (speed + acceleration) / braking) + this.m_info.m_generatedInfo.m_size.z * 0.5f;
            float maxSpeed = Mathf.Max(speed + acceleration, 5f);

            if (lodPhysics >= 2 && (currentFrameIndex >> 4 & 3) == (vehicleID & 3))
            {
                maxSpeed *= 2f;
            }

            float num3 = Mathf.Max((maxDistance - maxSpeed) / 3f, 1f);
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
                        uint laneID = PathManager.GetLaneID(pathPos);
                        if (laneID != 0u)
                        {
                            netManager.m_lanes.m_buffer[laneID].ReserveSpace(vehicleLength);
                        }
                    }
                }

                if ((currentFrameIndex >> 4 & 15) == (leaderID & 15))
                {
                    uint pathUnitID = leaderData.m_path;
                    int pathID = pathIndex >> 1;
                    bool invalid, congested = true;
                    bool congestionAvoidanceEnabled = true;
                    int congestedLanes = 0;
                    for (int j = 0; j < 5; ++j)
                    {
                        if (PathUnit.GetNextPosition(ref pathUnitID, ref pathID, out pathPos, out invalid))
                        {
                            uint laneID3 = PathManager.GetLaneID(pathPos);
                            if (laneID3 != 0u && !netManager.m_lanes.m_buffer[laneID3].CheckSpace(vehicleLength))
                            {
                                ++congestedLanes;
                                continue;
                            }

                            if (congestionAvoidanceEnabled)
                                continue;
                        }
                        else if (invalid)
                        {
                            this.InvalidPath(vehicleID, ref vehicleData, leaderID, ref leaderData);
                        }

                        congested = false;
                        break;
                    }

                    if (congestionAvoidanceEnabled)
                    {
                        if (congestedLanes >= 2 && (leaderData.m_flags & AvoidingCongestionFlag) == 0)
                        {
                            leaderData.m_flags |= AvoidingCongestionFlag;
                            this.InvalidPath(vehicleID, ref vehicleData, leaderID, ref leaderData);
                        }
                        else if (congestedLanes == 0 && (leaderData.m_flags & AvoidingCongestionFlag) != 0)
                        {
                            leaderData.m_flags &= ~AvoidingCongestionFlag;
                        }
                        else if (congestedLanes == 5)
                        {
                            leaderData.m_flags |= Vehicle.Flags.Congestion;
                        }
                    }
                    else if (congested)
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
                dir = VectorUtils.NormalizeXZ(rawDirection, out dist); // returns the distance before normalizing
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
                            if (newDirection.x == 0f) // This cannot happen! Sign returns either -1 or 1.
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
                        this.CheckOtherVehicles(vehicleID, ref vehicleData, ref frameData, ref targetSpeed, ref blocked, ref collisionPush, maxDistance, braking * 0.9f, lodPhysics);
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

            vehicleData.m_blockCounter = blocked ? (byte)Mathf.Min(vehicleData.m_blockCounter + 1, 255) : (byte)0;

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
            else if (finalSpeed > 0.1f && finalVelocity.sqrMagnitude > 0.01f)
            {
                frameData.m_rotation = Quaternion.LookRotation(finalVelocity);
            }
            else if (finalSpeed < -0.1f && finalVelocity.sqrMagnitude > 0.01f)
            {
                frameData.m_rotation = Quaternion.LookRotation(-finalVelocity);
            }

            base.SimulationStep(vehicleID, ref vehicleData, ref frameData, leaderID, ref leaderData, lodPhysics);
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
    }
}
