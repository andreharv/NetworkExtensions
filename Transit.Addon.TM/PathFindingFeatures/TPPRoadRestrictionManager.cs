using System;
using System.Collections.Generic;
using ColossalFramework;
using Transit.Addon.TM.Data;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public class TPPRoadRestrictionManager : Singleton<TPPRoadRestrictionManager>,  IRoadRestrictionManager
    {
        private TAMLaneRestriction[] _laneRestrictions = null;

        public void Init(TAMLaneRestriction[] laneRestrictions)
        {
            _laneRestrictions = laneRestrictions;
            if (_laneRestrictions == null)
            {
                _laneRestrictions = new TAMLaneRestriction[NetManager.MAX_LANE_COUNT];
            }
        }

        public void Reset()
        {
            _laneRestrictions = null;
        }

        public bool IsLoaded()
        {
            return _laneRestrictions != null;
        }

        public TAMLaneRestriction[] GetAllLaneRestrictions()
        {
            return _laneRestrictions;
        }

        private TAMLaneRestriction CreateLaneRestriction(uint laneId)
        {
            var laneData = new TAMLaneRestriction()
            {
                LaneId = laneId
            };

            NetSegment segment = NetManager.instance.m_segments.m_buffer[NetManager.instance.m_lanes.m_buffer[laneId].m_segment];
            NetInfo netInfo = segment.Info;
            int laneCount = netInfo.m_lanes.Length;
            int laneIndex = 0;
            for (uint l = segment.m_lanes; laneIndex < laneCount && l != 0; laneIndex++)
            {
                if (l == laneId)
                    break;

                l = NetManager.instance.m_lanes.m_buffer[l].m_nextLane;
            }

            if (laneIndex < laneCount)
            {
                ExtendedNetInfoLane netInfoLane = netInfo.m_lanes[laneIndex] as ExtendedNetInfoLane;
                if (netInfoLane != null)
                    laneData.UnitTypes = netInfoLane.AllowedVehicleTypes;
            }

            _laneRestrictions[laneId] = laneData;

            return laneData;
        }

        /// <summary>
        /// Gets lane data for the given lane id. Create it if does not exist.
        /// Warning: This method is not thread-safe.
        /// </summary>
        /// <param name="laneId"></param>
        /// <returns></returns>
        private TAMLaneRestriction GetOrCreateLaneRestriction(uint laneId)
        {
            TAMLaneRestriction lane = _laneRestrictions[laneId];
            if (lane == null)
                lane = CreateLaneRestriction(laneId);

            return lane;
        }

        public ExtendedUnitType GetRestrictions(uint laneId)
        {
            TAMLaneRestriction lane = GetOrCreateLaneRestriction(laneId);
			if (lane == null)
				return ExtendedUnitType.RoadVehicle; // TODO is this default value saved anywhere?
			return lane.UnitTypes;
        }

        public void SetRestrictions(uint laneId, ExtendedUnitType vehicleRestrictions)
        {
            GetOrCreateLaneRestriction(laneId).UnitTypes = vehicleRestrictions;
        }

        public void ToggleRestrictions(uint laneId, ExtendedUnitType vehicleType)
        {
            GetOrCreateLaneRestriction(laneId).UnitTypes ^= vehicleType;
        }

        public bool CanUseLane(ushort segmentId, NetInfo segmentInfo, byte laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedUnitType unitType)
        {
            if ((unitType & TPPSupported.UNITS) == 0)
            {
                return true;
            }

            if (laneInfo == null)
            {
                return true;
            }

            if ((laneInfo.m_vehicleType & TPPSupported.VEHICLETYPES) == 0)
            {
                return true;
            }

            var restrictions = GetRestrictions(laneId);

            return (restrictions & unitType) != ExtendedUnitType.None;
        }
    }
}
