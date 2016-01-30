using System;
using ColossalFramework;
using Transit.Framework.Extenders.PathFinding;

namespace Transit.Addon.Tools.Core
{
    public class LanesManager : Singleton<LanesManager>, ILaneRoutingManager, ILaneSpeedManager
    {
        #region Lane Creation

        internal Lane[] sm_lanes = new Lane[NetManager.MAX_LANE_COUNT];

        public Lane CreateLane(uint laneId)
        {
            Lane lane = new Lane()
            {
                m_laneId = laneId
            };

            var segment = NetManager.instance.m_segments.m_buffer[NetManager.instance.m_lanes.m_buffer[laneId].m_segment];
            var netInfo = segment.Info;
            var laneCount = netInfo.m_lanes.Length;
            var laneIndex = 0;
            for (uint l = segment.m_lanes; laneIndex < laneCount && l != 0; laneIndex++)
            {
                if (l == laneId)
                    break;

                l = NetManager.instance.m_lanes.m_buffer[l].m_nextLane;
            }

            if (laneIndex < laneCount)
            {
                NetInfoLane netInfoLane = netInfo.m_lanes[laneIndex] as NetInfoLane;
                if (netInfoLane != null)
                    lane.m_vehicleTypes = netInfoLane.m_allowedVehicleTypes;

                lane.m_speed = netInfo.m_lanes[laneIndex].m_speedLimit;
            }

            NetManager.instance.m_lanes.m_buffer[laneId].m_flags |= Lane.CONTROL_BIT;

            sm_lanes[laneId] = lane;

            return lane;
        }

        public Lane GetLane(uint laneId, bool forceCreate = true)
        {
            Lane lane = sm_lanes[laneId];

            if (!forceCreate)
            {
                return lane;
            }

            if (lane == null || (NetManager.instance.m_lanes.m_buffer[laneId].m_flags & Lane.CONTROL_BIT) == 0)
                lane = CreateLane(laneId);

            return lane;
        }

        #endregion

        #region Lane Connections
        public bool AddLaneConnection(uint laneId, uint connectionId)
        {
            Lane lane = GetLane(laneId);
            GetLane(connectionId); // makes sure lane information is stored

            return lane.AddConnection(connectionId);
        }

        public bool RemoveLaneConnection(uint laneId, uint connectionId)
        {
            Lane lane = GetLane(laneId);

            return lane.RemoveConnection(connectionId);
        }

        public uint[] GetLaneConnections(uint laneId)
        {
            Lane lane = GetLane(laneId);

            return lane.GetConnectionsAsArray();
        }

        public bool CheckLaneConnection(uint from, uint to)
        {   
            Lane lane = GetLane(from, false);

            if (lane == null)
            {
                return true;
            }
            else
            {
                return lane.ConnectsTo(to);
            }
        }
        #endregion

        #region Vehicle Restrictions
        public bool CanUseLane(TAMVehicleType vehicleType, uint laneId)
        {            
            return (GetLane(laneId).m_vehicleTypes & vehicleType) != TAMVehicleType.None;
        }

        public TAMVehicleType GetVehicleRestrictions(uint laneId)
        {
            return GetLane(laneId).m_vehicleTypes;
        }

        public void SetVehicleRestrictions(uint laneId, TAMVehicleType vehicleRestrictions)
        {
            GetLane(laneId).m_vehicleTypes = vehicleRestrictions;
        }

        public void ToggleVehicleRestriction(uint laneId, TAMVehicleType vehicleType)
        {
            GetLane(laneId).m_vehicleTypes ^= vehicleType;
        }

        #endregion

        #region Lane Speeds

        public float GetLaneSpeed(uint laneId)
        {
            return GetLane(laneId).m_speed;
        }

        public void SetLaneSpeed(uint laneId, int speed)
        {
            GetLane(laneId).m_speed = (float)Math.Round(speed/50f, 2);
        }

        #endregion

        public bool CanLanesConnect(uint laneId1, uint laneId2)
        {
            return CheckLaneConnection(laneId1, laneId2);
            //&& 
            //LanesManager.CanUseLane(this.m_vehicleType, num2) && 
            //LanesManager.CanUseLane(this.m_vehicleType, item.m_laneID)
        }

        public float GetLaneSpeedLimit(NetInfo.Lane lane, uint laneId)
        {
            return GetLaneSpeed(laneId);
        }
    }
}
