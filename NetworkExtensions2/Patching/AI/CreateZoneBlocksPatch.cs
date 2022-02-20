using ColossalFramework;
using ColossalFramework.Math;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using Transit.Framework.ExtensionPoints.AI;
using UnityEngine;

namespace NetworkExtensions2.Patching
{
    [HarmonyPatch(typeof(RoadAI))]
    [HarmonyPatch(nameof(RoadAI.CreateZoneBlocks))]
    internal static class CreateZoneBlocksPatch
    {
        public static void Apply(Harmony harmony)
        {
            var prefix = typeof(CreateZoneBlocksPatch).GetMethod(nameof(CreateZoneBlocksPatch.Prefix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(OriginalMethod, new HarmonyMethod(prefix));
        }
        public static void Revert(Harmony harmony)
        {
            harmony.Unpatch(OriginalMethod, HarmonyPatchType.Prefix);
        }

        static MethodInfo OriginalMethod => typeof(RoadAI).GetMethod("CreateZoneBlocks");

        private const float MIN_HALFWIDTH_DEFAULT = 8f;

        static bool Prefix(RoadAI __instance, ushort segment, ref NetSegment data)
        {
            try
            {
                if (RoadZoneBlocksCreationManager.HasCustomCreator(__instance.m_info.name))
                {
                    RoadZoneBlocksCreationManager
                        .GetCustomCreator(__instance.m_info.name)
                        .CreateZoneBlocks(__instance.m_info, segment, ref data);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("TAM: Crashed-CreateZoneBlocks");
                Debug.Log("TAM: " + ex.Message);
                Debug.Log("TAM: " + ex.ToString());
            }

            return true;
        }
    }
}
        //static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        //{

        //    int flagLoc = -1;
        //    bool minWidthSet = false;
        //    bool negate = false;
        //    bool flag;
        //    var isStraight = typeof(NetSegment).GetMethod(nameof(NetSegment.IsStraight), new Type[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(Vector3) });
        //    var mathMax = typeof(Mathf).GetMethod(nameof(Mathf.Max), new Type[] { typeof(float), typeof(float) });
        //    var roundToInt = typeof(Mathf).GetMethod(nameof(Mathf.RoundToInt), new Type[] { typeof(float) });
        //    var codes = new List<CodeInstruction>(instructions);
        //    var codesCount = codes.Count;
        //    for (int i = 0; i < codesCount; i++)
        //    {
        //        var code = codes[i];
        //        if (flagLoc > -1)
        //        {
        //            Debug.Log("checkpoint9");
        //            if (minWidthSet && code.opcode == OpCodes.Ldloc_S && flagLoc == (int)code.operand)
        //            {
        //                Debug.Log("checkpoint10");
        //                Debug.Log("checking label " + code.labels.Count);
        //                if (code.labels.Count > 0)
        //                {
        //                    for (int j = 0; j < code.labels.Count; j++)
        //                    {
        //                        Debug.Log("code labels " + (j + 1) + ": " + code.labels[j]);
        //                    }
        //                }
        //            }
        //            else if (code.opcode == OpCodes.Ldc_R4)
        //            {
        //                Debug.Log("checkpoint5");
        //                if (i + 4 < codesCount)
        //                {
        //                    Debug.Log("checkpoint6");
        //                    var mathMaxCode = codes[i + 4];
        //                    if (mathMaxCode.opcode == OpCodes.Call && mathMaxCode.Calls(mathMax))
        //                    {
        //                        Debug.Log("checkpoint7");
        //                        var floatOperand = (float)code.operand;
        //                        if (floatOperand == 8f)
        //                        {
        //                            Debug.Log("checkpoint8");
        //                            code.operand = 6f;
        //                            minWidthSet = true;
        //                            i += 4;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else if (code.opcode == OpCodes.Call && code.Calls(isStraight))
        //        {
        //            if (i + 3 < codesCount)
        //            {
        //                Debug.Log("checkpoint1");
        //                if (codes[i + 2].opcode == OpCodes.Ceq)
        //                {
        //                    Debug.Log("checkpoint2");
        //                    if (codes[i + 3].opcode == OpCodes.Stloc_S)
        //                    {

        //                        //Debug.Log("flagOperand " + flagOperand);
        //                        //flagLoc = (int)flagOperand;
        //                        //i += 3;
        //                        //Debug.Log("checkpoint3");
        //                        //if (codes[i + 1].opcode == OpCodes.Ldc_I4_0)
        //                        //{
        //                        //    Debug.Log("checkpoin4");
        //                        //    negate = true;
        //                        //}
        //                    }
        //                }

        //            }

        //        }
        //    }
        //    return codes.AsEnumerable();
        //}
    //}
//}
