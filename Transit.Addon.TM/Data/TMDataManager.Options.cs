namespace Transit.Addon.TM.Data
{
    public static partial class TMDataManager
    {
        public static void ApplyOptions(byte[] options)
        {
            if (options.Length >= 1)
            {
                OptionManager.setSimAccuracy(options[0]);
            }

            if (options.Length >= 2)
            {
                OptionManager.setLaneChangingRandomization(options[1]);
            }

            if (options.Length >= 3)
            {
                OptionManager.setRecklessDrivers(options[2]);
            }

            if (options.Length >= 4)
            {
                OptionManager.setRelaxedBusses(options[3] == (byte)1);
            }

            if (options.Length >= 5)
            {
                OptionManager.setNodesOverlay(options[4] == (byte)1);
            }

            if (options.Length >= 6)
            {
                OptionManager.setMayEnterBlockedJunctions(options[5] == (byte)1);
            }

            if (options.Length >= 7)
            {
#if !TAM
				if (!TrafficManagerModule.IsPathManagerCompatible) {
					Options.setAdvancedAI(false);
				} else {
#endif
                OptionManager.setAdvancedAI(options[6] == (byte)1);
#if !TAM
				}
#endif
            }

            if (options.Length >= 8)
            {
                OptionManager.setHighwayRules(options[7] == (byte)1);
            }

            if (options.Length >= 9)
            {
                OptionManager.setPrioritySignsOverlay(options[8] == (byte)1);
            }

            if (options.Length >= 10)
            {
                OptionManager.setTimedLightsOverlay(options[9] == (byte)1);
            }

            if (options.Length >= 11)
            {
                OptionManager.setSpeedLimitsOverlay(options[10] == (byte)1);
            }

            if (options.Length >= 12)
            {
                OptionManager.setVehicleRestrictionsOverlay(options[11] == (byte)1);
            }

            if (options.Length >= 13)
            {
                OptionManager.setStrongerRoadConditionEffects(options[12] == (byte)1);
            }

            if (options.Length >= 14)
            {
                OptionManager.setAllowUTurns(options[13] == (byte)1);
            }

            if (options.Length >= 15)
            {
                OptionManager.setAllowLaneChangesWhileGoingStraight(options[14] == (byte)1);
            }

            if (options.Length >= 16)
            {
                OptionManager.setEnableDespawning(options[15] == (byte)1);
            }

            if (options.Length >= 17)
            {
                OptionManager.setDynamicPathRecalculation(options[16] == (byte)1);
            }
        }

        public static byte[] CreateOptions()
        {
            return new[]
            {
                (byte) OptionManager.simAccuracy,
                (byte) OptionManager.laneChangingRandomization,
                (byte) OptionManager.recklessDrivers,
                (byte) (OptionManager.relaxedBusses ? 1 : 0),
                (byte) (OptionManager.nodesOverlay ? 1 : 0),
                (byte) (OptionManager.allowEnterBlockedJunctions ? 1 : 0),
                (byte) (OptionManager.advancedAI ? 1 : 0),
                (byte) (OptionManager.highwayRules ? 1 : 0),
                (byte) (OptionManager.prioritySignsOverlay ? 1 : 0),
                (byte) (OptionManager.timedLightsOverlay ? 1 : 0),
                (byte) (OptionManager.speedLimitsOverlay ? 1 : 0),
                (byte) (OptionManager.vehicleRestrictionsOverlay ? 1 : 0),
                (byte) (OptionManager.strongerRoadConditionEffects ? 1 : 0),
                (byte) (OptionManager.allowUTurns ? 1 : 0),
                (byte) (OptionManager.allowLaneChangesWhileGoingStraight ? 1 : 0),
                (byte) (OptionManager.enableDespawning ? 1 : 0),
                (byte) (OptionManager.dynamicPathRecalculation ? 1 : 0)
            };
        }
    }
}
