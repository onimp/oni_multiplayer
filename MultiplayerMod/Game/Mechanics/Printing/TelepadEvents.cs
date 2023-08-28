using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Game.Mechanics.Minions;
using MultiplayerMod.Game.UI.Screens;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Game.Mechanics.Printing;

[HarmonyPatch(typeof(Telepad))]
public static class TelepadEvents {

    public static event Action<AcceptDeliveryEventArgs>? AcceptDelivery;
    public static event Action<ComponentReference<Telepad>>? Reject;

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

    [RequireExecutionLevel(ExecutionLevel.Gameplay)]
    private static void OnAcceptDelivery(Telepad telepad, ITelepadDeliverable deliverable, GameObject gameObject) {
        ImmigrantScreenPatch.Deliverables = null;
        AcceptDelivery?.Invoke(
            new AcceptDeliveryEventArgs(
                telepad.GetReference(),
                deliverable,
                gameObject.GetComponent<MultiplayerInstance>().Register(),
                gameObject.GetComponent<MinionIdentity>()?.GetMultiplayerInstance().Register()
            )
        );
    }

    // ReSharper disable once UnusedMember.Local
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Telepad.RejectAll))]
    [RequireExecutionLevel(ExecutionLevel.Gameplay)]
    private static void OnRejectAll(Telepad __instance) {
        ImmigrantScreenPatch.Deliverables = null;
        Reject?.Invoke(__instance.GetReference());
    }

    [Serializable]
    public record AcceptDeliveryEventArgs(
        ComponentReference<Telepad> Target,
        ITelepadDeliverable Deliverable,
        MultiplayerId GameObjectId,
        MultiplayerId? ProxyId
    );

}
