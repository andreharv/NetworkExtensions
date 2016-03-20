using System.Collections.Generic;
using ColossalFramework;
using Transit.Addon.TM.Data;
using Transit.Addon.TM.Traffic;
using Transit.Framework.Network;

namespace Transit.Addon.TM.PathFindingFeatures {
	public partial class TAMRoadRestrictionManager {

		public static bool IsAllowed(ExtendedUnitType? allowedTypes, ExtendedUnitType vehicleType) {
			return allowedTypes == null || ((ExtendedUnitType)allowedTypes & vehicleType) != ExtendedUnitType.None;
		}

		public static bool IsBicycleAllowed(ExtendedUnitType? allowedTypes) {
			return IsAllowed(allowedTypes, ExtendedUnitType.Bicycle);
		}

		public static bool IsBusAllowed(ExtendedUnitType? allowedTypes) {
			return IsAllowed(allowedTypes, ExtendedUnitType.Bus);
		}

		public static bool IsCargoTrainAllowed(ExtendedUnitType? allowedTypes) {
			return IsAllowed(allowedTypes, ExtendedUnitType.CargoTrain);
		}

		public static bool IsCargoTruckAllowed(ExtendedUnitType? allowedTypes) {
			return IsAllowed(allowedTypes, ExtendedUnitType.CargoTruck);
		}

		public static bool IsEmergencyAllowed(ExtendedUnitType? allowedTypes) {
			return IsAllowed(allowedTypes, ExtendedUnitType.Emergency);
		}

		public static bool IsPassengerCarAllowed(ExtendedUnitType? allowedTypes) {
			return IsAllowed(allowedTypes, ExtendedUnitType.PassengerCar);
		}

		public static bool IsPassengerTrainAllowed(ExtendedUnitType? allowedTypes) {
			return IsAllowed(allowedTypes, ExtendedUnitType.PassengerTrain);
		}

		public static bool IsServiceAllowed(ExtendedUnitType? allowedTypes) {
			return IsAllowed(allowedTypes, ExtendedUnitType.ServiceVehicle);
		}

		public static bool IsTaxiAllowed(ExtendedUnitType? allowedTypes) {
			return IsAllowed(allowedTypes, ExtendedUnitType.Taxi);
		}

		public static bool IsTramAllowed(ExtendedUnitType? allowedTypes) {
			return IsAllowed(allowedTypes, ExtendedUnitType.Tram);
		}

		public static bool IsRailVehicleAllowed(ExtendedUnitType? allowedTypes) {
			return IsAllowed(allowedTypes, ExtendedUnitType.Train);
		}

		public static bool IsRoadVehicleAllowed(ExtendedUnitType? allowedTypes) {
			return IsAllowed(allowedTypes, ExtendedUnitType.RoadVehicle);
		}
	}
}
