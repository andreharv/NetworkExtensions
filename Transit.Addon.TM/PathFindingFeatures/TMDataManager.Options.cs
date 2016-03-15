using Transit.Addon.TM.State;

namespace Transit.Addon.TM.PathFindingFeatures
{
    public static partial class TMDataManager
    {
        public static void ApplyOptions(byte[] options)
        {
            if (options.Length >= 1)
            {
                Options.setSimAccuracy(options[0]);
            }

            if (options.Length >= 2)
            {
                Options.setLaneChangingRandomization(options[1]);
            }

            if (options.Length >= 3)
            {
                Options.setRecklessDrivers(options[2]);
            }

            if (options.Length >= 4)
            {
                Options.setRelaxedBusses(options[3] == (byte)1);
            }

            if (options.Length >= 5)
            {
                Options.setNodesOverlay(options[4] == (byte)1);
            }

            if (options.Length >= 6)
            {
                Options.setMayEnterBlockedJunctions(options[5] == (byte)1);
            }

            if (options.Length >= 7)
            {
#if !TAM
				if (!TrafficManagerModule.IsPathManagerCompatible) {
					Options.setAdvancedAI(false);
				} else {
#endif
                Options.setAdvancedAI(options[6] == (byte)1);
#if !TAM
				}
#endif
            }

            if (options.Length >= 8)
            {
                Options.setHighwayRules(options[7] == (byte)1);
            }

            if (options.Length >= 9)
            {
                Options.setPrioritySignsOverlay(options[8] == (byte)1);
            }

            if (options.Length >= 10)
            {
                Options.setTimedLightsOverlay(options[9] == (byte)1);
            }

            if (options.Length >= 11)
            {
                Options.setSpeedLimitsOverlay(options[10] == (byte)1);
            }

            if (options.Length >= 12)
            {
                Options.setVehicleRestrictionsOverlay(options[11] == (byte)1);
            }

            if (options.Length >= 13)
            {
                Options.setStrongerRoadConditionEffects(options[12] == (byte)1);
            }

            if (options.Length >= 14)
            {
                Options.setAllowUTurns(options[13] == (byte)1);
            }

            if (options.Length >= 15)
            {
                Options.setAllowLaneChangesWhileGoingStraight(options[14] == (byte)1);
            }

            if (options.Length >= 16)
            {
                Options.setEnableDespawning(options[15] == (byte)1);
            }

            if (options.Length >= 17)
            {
                Options.setDynamicPathRecalculation(options[16] == (byte)1);
            }
        }

        public static byte[] CreateOptions()
        {
            return new[]
            {
                (byte) Options.simAccuracy,
                (byte) Options.laneChangingRandomization,
                (byte) Options.recklessDrivers,
                (byte) (Options.relaxedBusses ? 1 : 0),
                (byte) (Options.nodesOverlay ? 1 : 0),
                (byte) (Options.allowEnterBlockedJunctions ? 1 : 0),
                (byte) (Options.advancedAI ? 1 : 0),
                (byte) (Options.highwayRules ? 1 : 0),
                (byte) (Options.prioritySignsOverlay ? 1 : 0),
                (byte) (Options.timedLightsOverlay ? 1 : 0),
                (byte) (Options.speedLimitsOverlay ? 1 : 0),
                (byte) (Options.vehicleRestrictionsOverlay ? 1 : 0),
                (byte) (Options.strongerRoadConditionEffects ? 1 : 0),
                (byte) (Options.allowUTurns ? 1 : 0),
                (byte) (Options.allowLaneChangesWhileGoingStraight ? 1 : 0),
                (byte) (Options.enableDespawning ? 1 : 0),
                (byte) (Options.dynamicPathRecalculation ? 1 : 0)
            };
        }
    }
}
