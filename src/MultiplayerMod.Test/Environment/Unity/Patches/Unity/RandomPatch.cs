using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(UnityEngine.Random))]
public class RandomPatch {

    private static readonly Random random = new();

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_value")]
    private static IEnumerable<CodeInstruction> Random_get_value(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            CodeInstruction.Call(typeof(RandomPatch), nameof(Random)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("RandomRangeInt")]
    private static IEnumerable<CodeInstruction> Random_RandomRangeInt(IEnumerable<CodeInstruction> instructions) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // minInclusive
            new(OpCodes.Ldarg_1), // maxExclusive
            CodeInstruction.Call(typeof(RandomPatch), nameof(RandomRangeInt)),
            new(OpCodes.Ret)
        };
    }

    public static int RandomRangeInt(int minInclusive, int maxExclusive) {
        return random.Next(minInclusive, maxExclusive);
    }

    public static float Random() {
        return (float)random.NextDouble();
    }
}
