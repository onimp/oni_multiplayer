using System;
using HarmonyLib;

namespace MultiplayerMod.Game.Objects;

[HarmonyPatch(typeof(KPrefabID))]
public static class KPrefabIdEvents {

    public static event Action<KPrefabID> Deserialize;

    // ReSharper disable once UnusedMember.Local
    [HarmonyPrefix]
    [HarmonyPatch(nameof(KPrefabID.OnDeserializedMethod))]
    private static void OnDeserializedPrefix(KPrefabID __instance) => Deserialize?.Invoke(__instance);

}
