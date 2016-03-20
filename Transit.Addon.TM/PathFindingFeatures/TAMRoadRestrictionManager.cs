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
    public class TAMRoadRestrictionManager : Singleton<TAMRoadRestrictionManager>,  IRoadRestrictionManager
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

        public static bool IsAllowed(ExtendedUnitType? allowedTypes, ExtendedUnitType vehicleType)
        {
            return allowedTypes == null || ((ExtendedUnitType)allowedTypes & vehicleType) != ExtendedUnitType.None;
        }

        public static bool IsBicycleAllowed(ExtendedUnitType? allowedTypes)
        {
            return IsAllowed(allowedTypes, ExtendedUnitType.Bicycle);
        }

        public static bool IsBusAllowed(ExtendedUnitType? allowedTypes)
        {
            return IsAllowed(allowedTypes, ExtendedUnitType.Bus);
        }

        public static bool IsCargoTrainAllowed(ExtendedUnitType? allowedTypes)
        {
            return IsAllowed(allowedTypes, ExtendedUnitType.CargoTrain);
        }

        public static bool IsCargoTruckAllowed(ExtendedUnitType? allowedTypes)
        {
            return IsAllowed(allowedTypes, ExtendedUnitType.CargoTruck);
        }

        public static bool IsEmergencyAllowed(ExtendedUnitType? allowedTypes)
        {
            return IsAllowed(allowedTypes, ExtendedUnitType.Emergency);
        }

        public static bool IsPassengerCarAllowed(ExtendedUnitType? allowedTypes)
        {
            return IsAllowed(allowedTypes, ExtendedUnitType.PassengerCar);
        }

        public static bool IsPassengerTrainAllowed(ExtendedUnitType? allowedTypes)
        {
            return IsAllowed(allowedTypes, ExtendedUnitType.PassengerTrain);
        }

        public static bool IsServiceAllowed(ExtendedUnitType? allowedTypes)
        {
            return IsAllowed(allowedTypes, ExtendedUnitType.ServiceVehicle);
        }

        public static bool IsTaxiAllowed(ExtendedUnitType? allowedTypes)
        {
            return IsAllowed(allowedTypes, ExtendedUnitType.Taxi);
        }

        public static bool IsTramAllowed(ExtendedUnitType? allowedTypes)
        {
            return IsAllowed(allowedTypes, ExtendedUnitType.Tram);
        }

        public static bool IsRailVehicleAllowed(ExtendedUnitType? allowedTypes)
        {
            return IsAllowed(allowedTypes, ExtendedUnitType.Train);
        }

        public static bool IsRoadVehicleAllowed(ExtendedUnitType? allowedTypes)
        {
            return IsAllowed(allowedTypes, ExtendedUnitType.RoadVehicle);
        }

        public bool CanUseLane(ushort segmentId, NetInfo segmentInfo, byte laneIndex, uint laneId, NetInfo.Lane laneInfo, ExtendedUnitType unitType)
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
