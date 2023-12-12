using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(TextAsset))]
public class TextAssetPatch {

    private static Dictionary<TextAsset, string> texts = new();

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("Internal_CreateInstance")]
    private static IEnumerable<CodeInstruction> TextAsset_Internal_CreateInstance(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // TextAsset
            new(OpCodes.Ldarg_1), // Text
            CodeInstruction.Call(typeof(TextAssetPatch), nameof(CreateTextAsset)),
            new(OpCodes.Ret)
        };
    }

    [UsedImplicitly]
    [HarmonyTranspiler]
    [HarmonyPatch("get_bytes")]
    private static IEnumerable<CodeInstruction> TextAsset_get_bytes(
        IEnumerable<CodeInstruction> instructions
    ) {
        return new List<CodeInstruction> {
            new(OpCodes.Ldarg_0), // TextAsset
            CodeInstruction.Call(typeof(TextAssetPatch), nameof(GetBytes)),
            new(OpCodes.Ret)
        };
    }

    public static void CreateTextAsset(TextAsset textAsset, string text) {
        texts[textAsset] = text;
    }

    public static byte[] GetBytes(TextAsset textAsset) {
        return texts.ContainsKey(textAsset) ? new UTF8Encoding().GetBytes(texts[textAsset]) : Array.Empty<byte>();
    }
}
