using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Transit.Addon.TM.Data;
using Transit.Framework;
using Transit.Framework.ExtensionPoints.PathFindingFeatures.Contracts;
using Transit.Framework.Network;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public partial class TAMRoadRestrictionManager : Singleton<TAMRoadRestrictionManager>,  IRoadRestrictionManager
    {
        private TAMLaneRestriction[] _laneRestrictions = null;

        public void Init()
        {
            _laneRestrictions = new TAMLaneRestriction[NetManager.MAX_LANE_COUNT];
        }

        public void Load(TAMLaneRestriction[] laneRestrictions, bool overrideIfExist)
        {
            if (laneRestrictions != null)
            {
                foreach (var restriction in laneRestrictions)
                {
                    Load(restriction, overrideIfExist);
                }
            }
        }

        public void Load(TAMLaneRestriction restriction, bool overrideIfExist)
        {
            if (restriction != null)
            {
                Log.Info(">>>>>> Loading " + restriction);

                if (overrideIfExist)
                {
                    _laneRestrictions[restriction.LaneId] = restriction;
                }
                else
                {
                    if (_laneRestrictions[restriction.LaneId] == null ||
                        _laneRestrictions[restriction.LaneId].UnitTypes == null)
                    {
                        _laneRestrictions[restriction.LaneId] = restriction;
                    }
                }
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

            var netInfoLane = NetManager.instance.GetLaneInfo(laneId) as ExtendedNetInfoLane;
            if (netInfoLane != null)
            {
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

        public ExtendedUnitType GetRestrictions(uint laneId, ExtendedUnitType fallback)
        {
            var unitTypes = GetOrCreateLaneRestriction(laneId).UnitTypes;

            if (unitTypes != null)
            {
                return unitTypes.Value;
            }

            return fallback;
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
            if (/*((unitType & ExtendedUnitType.Bus) == ExtendedUnitType.Bus) &&*/ (laneId == 181631 || laneId == 434))
            {
                Log.Info(laneId + ">>>>>>>> unitType " + unitType);

                if ((unitType & TAMSupported.UNITS) == 0)
                {
                    Log.Info(laneId + ">>>>>>>> Unsupported");
                    // unsupported type. allow.
                    return true;
                }

                if (laneInfo == null)
                {
                    Log.Info(laneId + ">>>>>>>> laneInfo null");
                    return true;
                }

                var laneUnitType = laneInfo.GetUnitType();
                if (laneUnitType == ExtendedUnitType.None)
                {
                    Log.Info(laneId + ">>>>>>>> laneUnitType none");
                    return true;
                }

                var restrictions = GetRestrictions(laneId, laneUnitType);
                Log.Info(laneId + ">>>>>>>> Restrictions " + restrictions);


                Log.Info(laneId + ">>>>>>>> CanUse " + ((restrictions & unitType) != ExtendedUnitType.None));

                return (restrictions & unitType) != ExtendedUnitType.None;
            }

            {
                if ((unitType & TAMSupported.UNITS) == 0)
                {
                    // unsupported type. allow.
                    return true;
                }

                if (laneInfo == null)
                {
                    return true;
                }

                var laneUnitType = laneInfo.GetUnitType();
                if (laneUnitType == ExtendedUnitType.None)
                {
                    return true;
                }

                var restrictions = GetRestrictions(laneId, laneUnitType);

                return (restrictions & unitType) != ExtendedUnitType.None;
            }
        }
    }
}
