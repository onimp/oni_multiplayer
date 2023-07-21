using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Game.Mechanics.Minions;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Game.Mechanics.Printing;

[HarmonyPatch(typeof(Telepad))]
public static class TelepadEvents {

    public static event Action<AcceptDeliveryEventArgs>? AcceptDelivery;
    public static event Action<GameObjectReference>? Reject;

    // ReSharper disable once UnusedMember.Local
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Telepad.OnAcceptDelivery))]
    private static IEnumerable<CodeInstruction> PatchAcceptDelivery(IEnumerable<CodeInstruction> instructions) {
        using var source = instructions.GetEnumerator();
        var result = new List<CodeInstruction>();
        result.AddConditional(source, it => it.opcode == OpCodes.Ret, false);

        result.Add(new CodeInstruction(OpCodes.Ldarg_0).WithLabels(source.Current!.labels)); // this
        result.Add(new CodeInstruction(OpCodes.Ldarg_1)); // delivery
        result.Add(new CodeInstruction(OpCodes.Ldloc_1)); // go
        result.Add(CodeInstruction.Call(typeof(TelepadEvents), nameof(OnAcceptDelivery)));

        result.Add(source.Current!.Clone());
        result.AddConditional(source, _ => false);

        return result;
    }

    private static void OnAcceptDelivery(Telepad telepad, ITelepadDeliverable deliverable, GameObject gameObject) =>
        PatchControl.RunIfEnabled(
            () => AcceptDelivery?.Invoke(
                new AcceptDeliveryEventArgs(
                    telepad.gameObject.GetGridReference(),
                    deliverable,
                    gameObject.GetComponent<MultiplayerInstance>().Register(),
                    gameObject.GetComponent<MinionIdentity>().GetMultiplayerInstance().Register()
                )
            )
        );

    // ReSharper disable once UnusedMember.Local
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Telepad.RejectAll))]
    private static void OnRejectAll(Telepad __instance) => PatchControl.RunIfEnabled(
        () => Reject?.Invoke(__instance.gameObject.GetGridReference())
    );

}
