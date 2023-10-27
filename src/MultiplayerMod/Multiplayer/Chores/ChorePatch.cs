using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.CoreOperations;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Chores;

[HarmonyPatch(typeof(Chore))]
public class ChorePatch {

    private static HashSet<Type> supportedChores = new() {
        typeof(WorkChore<Diggable>)
    };

    [HarmonyPostfix]
    [HarmonyPatch(
        MethodType.Constructor,
        typeof(ChoreType),
        typeof(IStateMachineTarget),
        typeof(ChoreProvider),
        typeof(bool),
        typeof(Action<Chore>),
        typeof(Action<Chore>),
        typeof(Action<Chore>),
        typeof(PriorityScreen.PriorityClass),
        typeof(int),
        typeof(bool),
        typeof(bool),
        typeof(int),
        typeof(bool),
        typeof(ReportManager.ReportType)
    )]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    [RequireMultiplayerMode(MultiplayerMode.Client)]
    private static void ChoreConstructor(Chore __instance) {
        if (!supportedChores.Contains(__instance.GetType()))
            return;

        __instance.AddPrecondition(ChoreMultiplayerPreconditions.HasHostSelectedDriver);
        AccessTools.PropertySetter(typeof(Chore), nameof(Chore.IsPreemptable))
            .Invoke(__instance, new object[] { false });
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Chore.Begin))]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    [RequireMultiplayerMode(MultiplayerMode.Host)]
    private static void BeginChore(Chore __instance, Chore.Precondition.Context context) {
        if (!supportedChores.Contains(__instance.GetType()))
            return;

        var choreDriverRef = context.consumerState.choreDriver.GetReference2();
        var choreInstanceType = __instance.GetType();
        var choreTypeId = __instance.choreType.Id;
        var providerRef = __instance.provider.GetReference2();
        var worldId = __instance.gameObject.GetMyParentWorldId();
        var targetRef = __instance.target.gameObject.GetReference();
        var preemptable = __instance.IsPreemptable;

        Dependencies.Get<IMultiplayerClient>().Send(
            new SetChoreDriver(choreDriverRef, choreInstanceType, choreTypeId, providerRef, worldId, targetRef, preemptable)
        );
    }

}
