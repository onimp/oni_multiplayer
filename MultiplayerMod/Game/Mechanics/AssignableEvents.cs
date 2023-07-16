using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.Mechanics;

[HarmonyPatch(typeof(Assignable))]
public static class AssignableEvents {

    public static event Action<Assignable, IAssignableIdentity?>? Assign;

    // ReSharper disable once UnusedMember.Global, InconsistentNaming
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Assignable.Assign))]
    public static void AssignPostfix(Assignable __instance, IAssignableIdentity new_assignee) =>
        PatchControl.RunIfEnabled(() => { Assign?.Invoke(__instance, new_assignee); });

    // ReSharper disable once UnusedMember.Global
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Assignable.Unassign))]
    public static void UnassignPostfix(Assignable __instance) =>
        PatchControl.RunIfEnabled(() => { Assign?.Invoke(__instance, null); });

}
