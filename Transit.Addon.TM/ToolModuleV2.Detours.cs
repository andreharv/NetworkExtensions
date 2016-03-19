using System;
using System.Reflection;
using Transit.Addon.TM.AI;
using Transit.Addon.TM.UI;
using UnityEngine;
using System.Collections.Generic;
using ColossalFramework.UI;
using ColossalFramework.Math;
using Transit.Framework.Redirection;

namespace Transit.Addon.TM {
    public partial class ToolModuleV2 {
		public class Detour {
			public MethodInfo OriginalMethod;
			public MethodInfo CustomMethod;
			public RedirectCallsState Redirect;

			public Detour(MethodInfo originalMethod, MethodInfo customMethod) {
				this.OriginalMethod = originalMethod;
				this.CustomMethod = customMethod;
				this.Redirect = RedirectionHelper.RedirectCalls(originalMethod, customMethod);
			}
		}

        public static ToolModuleV2 Instance;
#if !TAM
		public static bool IsPathManagerCompatible = true;
#endif
		public static bool IsPathManagerReplaced = false;
        public bool DetourInited { get; set; }
        public bool NodeSimulationLoaded { get; set; }
		public List<Detour> Detours { get; set; }
		public BaseUI UI { get; set; }

		public void revertDetours() {
			if (Instance.DetourInited) {
				Log.Info("Revert detours");
				foreach (Detour d in Detours) {
					RedirectionHelper.RevertRedirect(d.OriginalMethod, d.Redirect);
				}
                Instance.DetourInited = false;
				Detours.Clear();
			}
		}

		public void initDetours() {
			if (!ToolModuleV2.Instance.DetourInited) {
				Log.Info("Init detours");
				bool detourFailed = false;

				Log.Info("Redirecting Vehicle AI Calculate Segment Calls (1)");
				try {
					Detours.Add(new Detour(typeof(VehicleAI).GetMethod("CalculateSegmentPosition",
							BindingFlags.NonPublic | BindingFlags.Instance,
							null,
							new[]
							{
								typeof (ushort),
								typeof (Vehicle).MakeByRefType(),
								typeof (PathUnit.Position),
								typeof (PathUnit.Position),
								typeof (uint),
								typeof (byte),
								typeof (PathUnit.Position),
								typeof (uint),
								typeof (byte),
								typeof (int),
								typeof (Vector3).MakeByRefType(),
								typeof (Vector3).MakeByRefType(),
								typeof (float).MakeByRefType()
							},
							null),
							typeof(CustomVehicleAI).GetMethod("CustomCalculateSegmentPosition")));
				} catch (Exception) {
					Log.Error("Could not redirect VehicleAI::CalculateSegmentPosition (1).");
					detourFailed = true;
				}


				Log.Info("Redirecting Vehicle AI Calculate Segment Calls (2)");
				try {
					Detours.Add(new Detour(typeof(VehicleAI).GetMethod("CalculateSegmentPosition",
							BindingFlags.NonPublic | BindingFlags.Instance,
							null,
							new[]
							{
								typeof (ushort),
								typeof (Vehicle).MakeByRefType(),
								typeof (PathUnit.Position),
								typeof (uint),
								typeof (byte),
								typeof (Vector3).MakeByRefType(),
								typeof (Vector3).MakeByRefType(),
								typeof (float).MakeByRefType()
							},
							null),
							typeof(CustomVehicleAI).GetMethod("CustomCalculateSegmentPositionPathFinder")));
				} catch (Exception) {
					Log.Error("Could not redirect VehicleAI::CalculateSegmentPosition (2).");
					detourFailed = true;
				}

				Log.Info("Redirecting TramBaseAI Calculate Segment Calls (2)");
				try {
					Detours.Add(new Detour(typeof(TramBaseAI).GetMethod("CalculateSegmentPosition",
							BindingFlags.NonPublic | BindingFlags.Instance,
							null,
							new[]
							{
								typeof (ushort),
								typeof (Vehicle).MakeByRefType(),
								typeof (PathUnit.Position),
								typeof (uint),
								typeof (byte),
								typeof (Vector3).MakeByRefType(),
								typeof (Vector3).MakeByRefType(),
								typeof (float).MakeByRefType()
							},
							null),
							typeof(CustomTramBaseAI).GetMethod("CustomCalculateSegmentPositionPathFinder")));
				} catch (Exception) {
					Log.Error("Could not redirect TramBaseAI::CalculateSegmentPosition (2).");
					detourFailed = true;
				}

				Log.Info("Redirecting RoadBaseAI.SimulationStep for nodes");
				try {
					Detours.Add(new Detour(typeof(RoadBaseAI).GetMethod("SimulationStep", new[] { typeof(ushort), typeof(NetNode).MakeByRefType() }),
						typeof(CustomRoadAI).GetMethod("CustomNodeSimulationStep")));
				} catch (Exception) {
					Log.Error("Could not redirect RoadBaseAI::SimulationStep.");
					detourFailed = true;
				}

				Log.Info("Redirecting RoadBaseAI.SimulationStep for segments");
				try {
					Detours.Add(new Detour(typeof(RoadBaseAI).GetMethod("SimulationStep", new[] { typeof(ushort), typeof(NetSegment).MakeByRefType() }),
						typeof(CustomRoadAI).GetMethod("CustomSegmentSimulationStep")));
				} catch (Exception) {
					Log.Error("Could not redirect RoadBaseAI::SimulationStep.");
				}

				Log.Info("Redirecting Human AI Calls");
				try {
					Detours.Add(new Detour(typeof(HumanAI).GetMethod("CheckTrafficLights",
							BindingFlags.NonPublic | BindingFlags.Instance,
							null,
							new[] { typeof(ushort), typeof(ushort) },
							null),
							typeof(CustomHumanAI).GetMethod("CustomCheckTrafficLights")));
				} catch (Exception) {
					Log.Error("Could not redirect HumanAI::CheckTrafficLights.");
					detourFailed = true;
				}

				Log.Info("Redirecting CarAI Simulation Step Calls");
				try {
					Detours.Add(new Detour(typeof(CarAI).GetMethod("SimulationStep",
								new[] {
									typeof (ushort),
									typeof (Vehicle).MakeByRefType(),
									typeof (Vector3)
								}),
								typeof(CustomCarAI).GetMethod("TrafficManagerSimulationStep")));
				} catch (Exception) {
					Log.Error("Could not redirect CarAI::SimulationStep.");
					detourFailed = true;
				}

				Log.Info("Redirecting PassengerCarAI Simulation Step Calls");
				try {
					Detours.Add(new Detour(typeof(PassengerCarAI).GetMethod("SimulationStep",
							new[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(Vector3) }),
							typeof(CustomPassengerCarAI).GetMethod("CustomSimulationStep")));
				} catch (Exception) {
					Log.Error("Could not redirect PassengerCarAI::SimulationStep.");
					detourFailed = true;
				}

				Log.Info("Redirecting CargoTruckAI Simulation Step Calls");
				try {
					Detours.Add(new Detour(typeof(CargoTruckAI).GetMethod("SimulationStep",
								new[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(Vector3) }),
								typeof(CustomCargoTruckAI).GetMethod("CustomSimulationStep")));
				} catch (Exception) {
					Log.Error("Could not redirect CargoTruckAI::SimulationStep.");
					detourFailed = true;
				}

				Log.Info("Redirecting TrainAI Simulation Step Calls");
				try {
					Detours.Add(new Detour(typeof(TrainAI).GetMethod("SimulationStep",
								new[] {
									typeof (ushort),
									typeof (Vehicle).MakeByRefType(),
									typeof (Vector3)
								}),
								typeof(CustomTrainAI).GetMethod("TrafficManagerSimulationStep")));
				} catch (Exception) {
					Log.Error("Could not redirect TrainAI::SimulationStep.");
					detourFailed = true;
				}

				Log.Info("Redirection TramBaseAI::SimulationStep calls");
				try {
					Detours.Add(new Detour(typeof(TramBaseAI).GetMethod("SimulationStep",
							BindingFlags.Public | BindingFlags.Instance,
							null,
							new[]
							{
								typeof (ushort),
								typeof (Vehicle).MakeByRefType(),
								typeof (Vector3),
							},
							null), typeof(CustomTramBaseAI).GetMethod("CustomSimulationStep")));
				} catch (Exception) {
					Log.Error("Could not redirect TramBaseAI::SimulationStep");
					detourFailed = true;
				}

				/*++i;
				Log._Debug("Redirecting Train AI Calculate Segment Calls");
				try {
					TrafficManagerModule.Instance.OriginalMethods[i] = typeof(TrainAI).GetMethod("CalculateSegmentPosition",
							BindingFlags.NonPublic | BindingFlags.Instance,
							null,
							new[]
							{
								typeof (ushort),
								typeof (Vehicle).MakeByRefType(),
								typeof (PathUnit.Position),
								typeof (PathUnit.Position),
								typeof (uint),
								typeof (byte),
								typeof (PathUnit.Position),
								typeof (uint),
								typeof (byte),
								typeof (Vector3).MakeByRefType(),
								typeof (Vector3).MakeByRefType(),
								typeof (float).MakeByRefType()
							},
							null);
					TrafficManagerModule.Instance.CustomMethods[i] = typeof(CustomTrainAI).GetMethod("TmCalculateSegmentPosition");
					TrafficManagerModule.Instance.CustomRedirects[i] = RedirectionHelper.RedirectCalls(TrafficManagerModule.Instance.OriginalMethods[i], TrafficManagerModule.Instance.CustomMethods[i]);
				} catch (Exception) {
					Log.Error("Could not redirect TrainAI::CalculateSegmentPosition (1)");
					detourFailed = true;
				}*/

				Log.Info("Redirecting Car AI Calculate Segment Calls");
				try {
					Detours.Add(new Detour(typeof(CarAI).GetMethod("CalculateSegmentPosition",
							BindingFlags.NonPublic | BindingFlags.Instance,
							null,
							new[]
							{
								typeof (ushort),
								typeof (Vehicle).MakeByRefType(),
								typeof (PathUnit.Position),
								typeof (PathUnit.Position),
								typeof (uint),
								typeof (byte),
								typeof (PathUnit.Position),
								typeof (uint),
								typeof (byte),
								typeof (int),
								typeof (Vector3).MakeByRefType(),
								typeof (Vector3).MakeByRefType(),
								typeof (float).MakeByRefType()
							},
							null),
							typeof(CustomCarAI).GetMethod("CustomCalculateSegmentPosition")));
				} catch (Exception) {
					Log.Error("Could not redirect CarAI::CalculateSegmentPosition.");
					detourFailed = true;
				}

				Log.Info("Redirection TramBaseAI Calculate Segment Position calls");
				try {
					Detours.Add(new Detour(typeof(TramBaseAI).GetMethod("CalculateSegmentPosition",
							BindingFlags.NonPublic | BindingFlags.Instance,
							null,
							new[]
							{
									typeof (ushort),
									typeof (Vehicle).MakeByRefType(),
									typeof (PathUnit.Position),
									typeof (PathUnit.Position),
									typeof (uint),
									typeof (byte),
									typeof (PathUnit.Position),
									typeof (uint),
									typeof (byte),
									typeof (int),
									typeof (Vector3).MakeByRefType(),
									typeof (Vector3).MakeByRefType(),
									typeof (float).MakeByRefType()
							},
							null),
							typeof(CustomTramBaseAI).GetMethod("CustomCalculateSegmentPosition")));
				} catch (Exception) {
					Log.Error("Could not redirect TramBaseAI::CalculateSegmentPosition");
					detourFailed = true;
				}

#if TAM
                if (true) {
#else
                if (IsPathManagerCompatible) {
#endif
                    Log.Info("Redirection CarAI Calculate Segment Position calls for non-Traffic++");
					try {
						Detours.Add(new Detour(typeof(CarAI).GetMethod("CalculateSegmentPosition",
								BindingFlags.NonPublic | BindingFlags.Instance,
								null,
								new[]
								{
									typeof (ushort),
									typeof (Vehicle).MakeByRefType(),
									typeof (PathUnit.Position),
									typeof (uint),
									typeof (byte),
									typeof (Vector3).MakeByRefType(),
									typeof (Vector3).MakeByRefType(),
									typeof (float).MakeByRefType()
								},
								null),
								typeof(CustomCarAI).GetMethod("CustomCalculateSegmentPositionPathFinder")));
					} catch (Exception) {
						Log.Error("Could not redirect CarAI::CalculateSegmentPosition");
						detourFailed = true;
					}

					Log.Info("Redirection TrainAI Calculate Segment Position calls for non-Traffic++");
					try {
						Detours.Add(new Detour(typeof(TrainAI).GetMethod("CalculateSegmentPosition",
								BindingFlags.NonPublic | BindingFlags.Instance,
								null,
								new[]
								{
									typeof (ushort),
									typeof (Vehicle).MakeByRefType(),
									typeof (PathUnit.Position),
									typeof (uint),
									typeof (byte),
									typeof (Vector3).MakeByRefType(),
									typeof (Vector3).MakeByRefType(),
									typeof (float).MakeByRefType()
								},
								null),
								typeof(CustomTrainAI).GetMethod("TmCalculateSegmentPositionPathFinder")));
					} catch (Exception) {
						Log.Error("Could not redirect TrainAI::CalculateSegmentPosition (2)");
						detourFailed = true;
					}

                    //	Log.Info("Redirection AmbulanceAI::StartPathFind calls");
                    //	try {
                    //		Detours.Add(new Detour(typeof(AmbulanceAI).GetMethod("StartPathFind",
                    //				BindingFlags.NonPublic | BindingFlags.Instance,
                    //				null,
                    //				new[]
                    //				{
                    //					typeof (ushort),
                    //					typeof (Vehicle).MakeByRefType(),
                    //					typeof (Vector3),
                    //					typeof (Vector3),
                    //					typeof (bool),
                    //					typeof (bool),
                    //					typeof (bool)
                    //				},
                    //				null),
                    //				typeof(CustomAmbulanceAI).GetMethod("CustomStartPathFind")));
                    //	} catch (Exception) {
                    //		Log.Error("Could not redirect AmbulanceAI::StartPathFind");
                    //		detourFailed = true;
                    //	}

                    //	Log.Info("Redirection BusAI::StartPathFind calls");
                    //	try {
                    //		Detours.Add(new Detour(typeof(BusAI).GetMethod("StartPathFind",
                    //				BindingFlags.NonPublic | BindingFlags.Instance,
                    //				null,
                    //				new[]
                    //				{
                    //					typeof (ushort),
                    //					typeof (Vehicle).MakeByRefType(),
                    //					typeof (Vector3),
                    //					typeof (Vector3),
                    //					typeof (bool),
                    //					typeof (bool),
                    //					typeof (bool)
                    //				},
                    //				null),
                    //				typeof(CustomBusAI).GetMethod("CustomStartPathFind")));
                    //	} catch (Exception) {
                    //		Log.Error("Could not redirect BusAI::StartPathFind");
                    //		detourFailed = true;
                    //	}

                    //	Log.Info("Redirection CarAI::StartPathFind calls");
                    //	try {
                    //		Detours.Add(new Detour(typeof(CarAI).GetMethod("StartPathFind",
                    //				BindingFlags.NonPublic | BindingFlags.Instance,
                    //				null,
                    //				new[]
                    //				{
                    //					typeof (ushort),
                    //					typeof (Vehicle).MakeByRefType(),
                    //					typeof (Vector3),
                    //					typeof (Vector3),
                    //					typeof (bool),
                    //					typeof (bool),
                    //					typeof (bool)
                    //				},
                    //				null),
                    //				typeof(CustomCarAI).GetMethod("CustomStartPathFind")));
                    //	} catch (Exception) {
                    //		Log.Error("Could not redirect CarAI::StartPathFind");
                    //		detourFailed = true;
                    //	}

                    //	Log.Info("Redirection CargoTruckAI::StartPathFind calls");
                    //	try {
                    //		Detours.Add(new Detour(typeof(CargoTruckAI).GetMethod("StartPathFind",
                    //				BindingFlags.NonPublic | BindingFlags.Instance,
                    //				null,
                    //				new[]
                    //				{
                    //					typeof (ushort),
                    //					typeof (Vehicle).MakeByRefType(),
                    //					typeof (Vector3),
                    //					typeof (Vector3),
                    //					typeof (bool),
                    //					typeof (bool),
                    //					typeof (bool)
                    //				},
                    //				null),
                    //				typeof(CustomCargoTruckAI).GetMethod("CustomStartPathFind")));
                    //	} catch (Exception) {
                    //		Log.Error("Could not redirect CargoTruckAI::StartPathFind");
                    //		detourFailed = true;
                    //	}

                    //	Log.Info("Redirection FireTruckAI::StartPathFind calls");
                    //	try {
                    //		Detours.Add(new Detour(typeof(FireTruckAI).GetMethod("StartPathFind",
                    //				BindingFlags.NonPublic | BindingFlags.Instance,
                    //				null,
                    //				new[]
                    //				{
                    //					typeof (ushort),
                    //					typeof (Vehicle).MakeByRefType(),
                    //					typeof (Vector3),
                    //					typeof (Vector3),
                    //					typeof (bool),
                    //					typeof (bool),
                    //					typeof (bool)
                    //				},
                    //				null),
                    //				typeof(CustomFireTruckAI).GetMethod("CustomStartPathFind")));
                    //	} catch (Exception) {
                    //		Log.Error("Could not redirect FireTruckAI::StartPathFind");
                    //		detourFailed = true;
                    //	}

                    //	Log.Info("Redirection PassengerCarAI::StartPathFind calls");
                    //	try {
                    //		Detours.Add(new Detour(typeof(PassengerCarAI).GetMethod("StartPathFind",
                    //				BindingFlags.NonPublic | BindingFlags.Instance,
                    //				null,
                    //				new[]
                    //				{
                    //					typeof (ushort),
                    //					typeof (Vehicle).MakeByRefType(),
                    //					typeof (Vector3),
                    //					typeof (Vector3),
                    //					typeof (bool),
                    //					typeof (bool),
                    //					typeof (bool)
                    //				},
                    //				null),
                    //				typeof(CustomPassengerCarAI).GetMethod("CustomStartPathFind")));
                    //	} catch (Exception) {
                    //		Log.Error("Could not redirect PassengerCarAI::StartPathFind");
                    //		detourFailed = true;
                    //	}

                    //	Log.Info("Redirection PoliceCarAI::StartPathFind calls");
                    //	try {
                    //		Detours.Add(new Detour(typeof(PoliceCarAI).GetMethod("StartPathFind",
                    //				BindingFlags.NonPublic | BindingFlags.Instance,
                    //				null,
                    //				new[]
                    //				{
                    //					typeof (ushort),
                    //					typeof (Vehicle).MakeByRefType(),
                    //					typeof (Vector3),
                    //					typeof (Vector3),
                    //					typeof (bool),
                    //					typeof (bool),
                    //					typeof (bool)
                    //				},
                    //				null),
                    //				typeof(CustomPoliceCarAI).GetMethod("CustomStartPathFind")));
                    //	} catch (Exception) {
                    //		Log.Error("Could not redirect PoliceCarAI::StartPathFind");
                    //		detourFailed = true;
                    //	}

                    //	Log.Info("Redirection TaxiAI::StartPathFind calls");
                    //	try {
                    //		Detours.Add(new Detour(typeof(TaxiAI).GetMethod("StartPathFind",
                    //				BindingFlags.NonPublic | BindingFlags.Instance,
                    //				null,
                    //				new[]
                    //				{
                    //					typeof (ushort),
                    //					typeof (Vehicle).MakeByRefType(),
                    //					typeof (Vector3),
                    //					typeof (Vector3),
                    //					typeof (bool),
                    //					typeof (bool),
                    //					typeof (bool)
                    //				},
                    //				null),
                    //				typeof(CustomTaxiAI).GetMethod("CustomStartPathFind")));
                    //	} catch (Exception) {
                    //		Log.Error("Could not redirect TaxiAI::StartPathFind");
                    //		detourFailed = true;
                    //	}

                    //	Log.Info("Redirection TrainAI::StartPathFind calls");
                    //	try {
                    //		Detours.Add(new Detour(typeof(TrainAI).GetMethod("StartPathFind",
                    //				BindingFlags.NonPublic | BindingFlags.Instance,
                    //				null,
                    //				new[]
                    //				{
                    //					typeof (ushort),
                    //					typeof (Vehicle).MakeByRefType(),
                    //					typeof (Vector3),
                    //					typeof (Vector3),
                    //					typeof (bool),
                    //					typeof (bool)
                    //				},
                    //				null),
                    //				typeof(CustomTrainAI).GetMethod("CustomStartPathFind")));
                    //	} catch (Exception) {
                    //		Log.Error("Could not redirect TrainAI::StartPathFind");
                    //		detourFailed = true;
                    //	}

                    //	Log.Info("Redirection CitizenAI::StartPathFind calls");
                    //	try {
                    //		Detours.Add(new Detour(typeof(CitizenAI).GetMethod("StartPathFind",
                    //				BindingFlags.NonPublic | BindingFlags.Instance,
                    //				null,
                    //				new[]
                    //				{
                    //					typeof (ushort),
                    //					typeof (CitizenInstance).MakeByRefType(),
                    //					typeof (Vector3),
                    //					typeof (Vector3),
                    //					typeof (VehicleInfo)
                    //				},
                    //				null),
                    //				typeof(CustomCitizenAI).GetMethod("CustomStartPathFind")));
                    //	} catch (Exception) {
                    //		Log.Error("Could not redirect CitizenAI::StartPathFind");
                    //		detourFailed = true;
                    //	}

                    //	Log.Info("Redirection TransportLineAI::StartPathFind calls");
                    //	try {
                    //		Detours.Add(new Detour(typeof(TransportLineAI).GetMethod("StartPathFind",
                    //				BindingFlags.Public | BindingFlags.Static,
                    //				null,
                    //				new[]
                    //				{
                    //					typeof (ushort),
                    //					typeof (NetSegment).MakeByRefType(),
                    //					typeof (ItemClass.Service),
                    //					typeof (VehicleInfo.VehicleType),
                    //					typeof (bool)
                    //				},
                    //				null),
                    //				typeof(CustomTransportLineAI).GetMethod("CustomStartPathFind")));
                    //	} catch (Exception) {
                    //		Log.Error("Could not redirect TransportLineAI::StartPathFind");
                    //		detourFailed = true;
                    //	}

                    //	Log.Info("Redirection TramBaseAI::StartPathFind calls");
                    //	try {
                    //		Detours.Add(new Detour(typeof(TramBaseAI).GetMethod("StartPathFind",
                    //				BindingFlags.NonPublic | BindingFlags.Instance,
                    //				null,
                    //				new[]
                    //				{
                    //					typeof (ushort),
                    //					typeof (Vehicle).MakeByRefType(),
                    //					typeof (Vector3),
                    //					typeof (Vector3),
                    //					typeof (bool),
                    //					typeof (bool)
                    //				},
                    //				null),
                    //				typeof(CustomTramBaseAI).GetMethod("CustomStartPathFind")));
                    //	} catch (Exception) {
                    //		Log.Error("Could not redirect TramBaseAI::StartPathFind");
                    //		detourFailed = true;
                    //	}
                }

                Log.Info("Redirection RoadBaseAI::SetTrafficLightState calls");
				try {
					Detours.Add(new Detour(typeof(RoadBaseAI).GetMethod("SetTrafficLightState",
							BindingFlags.Public | BindingFlags.Static,
							null,
							new[]
							{
									typeof (ushort),
									typeof (NetSegment).MakeByRefType(),
									typeof (uint),
									typeof (RoadBaseAI.TrafficLightState),
									typeof (RoadBaseAI.TrafficLightState),
									typeof (bool),
									typeof (bool)
							},
							null),
							typeof(CustomRoadAI).GetMethod("CustomSetTrafficLightState")));
				} catch (Exception) {
					Log.Error("Could not redirect RoadBaseAI::SetTrafficLightState");
					detourFailed = true;
				}

				Log.Info("Reverse-Redirection CustomTrainAI::CheckOverlap calls (1)");
				try {
					Detours.Add(new Detour(typeof(CustomTrainAI).GetMethod("CheckOverlap",
							BindingFlags.NonPublic | BindingFlags.Static,
							null,
							new[]
							{
									typeof (ushort),
									typeof (Vehicle).MakeByRefType(),
									typeof (Segment3),
									typeof (ushort)
							},
							null),
							typeof(TrainAI).GetMethod("CheckOverlap",
								BindingFlags.NonPublic | BindingFlags.Static,
								null,
								new[]
								{
									typeof (ushort),
									typeof (Vehicle).MakeByRefType(),
									typeof (Segment3),
									typeof (ushort)
								},
								null)));
				} catch (Exception) {
					Log.Error("Could not reverse-redirect CustomRoadBaseAI::CheckOverlap (1)");
					detourFailed = true;
				}

				Log.Info("Reverse-Redirection CustomTrainAI::CheckOverlap calls (2)");
				try {
					Detours.Add(new Detour(typeof(CustomTrainAI).GetMethod("CheckOverlap",
							BindingFlags.NonPublic | BindingFlags.Static,
							null,
							new[]
							{
									typeof (ushort),
									typeof (Vehicle).MakeByRefType(),
									typeof (Segment3),
									typeof (ushort),
									typeof (ushort),
									typeof (Vehicle).MakeByRefType(),
									typeof (bool).MakeByRefType(),
									typeof (Vector3),
									typeof (Vector3)
							},
							null), typeof(TrainAI).GetMethod("CheckOverlap",
							BindingFlags.NonPublic | BindingFlags.Static,
							null,
							new[]
							{
									typeof (ushort),
									typeof (Vehicle).MakeByRefType(),
									typeof (Segment3),
									typeof (ushort),
									typeof (ushort),
									typeof (Vehicle).MakeByRefType(),
									typeof (bool).MakeByRefType(),
									typeof (Vector3),
									typeof (Vector3)
							},
							null)));
				} catch (Exception) {
					Log.Error("Could not reverse-redirect CustomRoadBaseAI::CheckOverlap (2)");
					detourFailed = true;
				}
				
				Log.Info("Redirection TrainAI::CheckNextLane calls");
				try {
					Detours.Add(new Detour(typeof(TrainAI).GetMethod("CheckNextLane",
							BindingFlags.NonPublic | BindingFlags.Instance,
							null,
							new[]
							{
									typeof (ushort),
									typeof (Vehicle).MakeByRefType(),
									typeof (float).MakeByRefType(),
									typeof (PathUnit.Position),
									typeof (uint),
									typeof (byte),
									typeof (PathUnit.Position),
									typeof (uint),
									typeof (byte),
									typeof (Bezier3)
							},
							null),
							typeof(CustomTrainAI).GetMethod("CustomCheckNextLane")));
				} catch (Exception) {
					Log.Error("Could not redirect TrainAI::CheckNextLane");
					detourFailed = true;
				}

				if (detourFailed) {
					Log.Info("Detours failed");
					UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Incompatibility Issue", "Traffic Manager: President Edition detected an incompatibility with another mod! You can continue playing but it's NOT recommended. Traffic Manager will not work as expected.", true);
				} else {
					Log.Info("Detours successful");
				}

				Instance.DetourInited = true;
			}
		}
	}
}
