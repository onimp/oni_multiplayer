using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.Chores.Driver;
using MultiplayerMod.Multiplayer.Chores.Events;
using MultiplayerMod.Multiplayer.CoreOperations;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.StateMachines;

namespace MultiplayerMod.Multiplayer.Chores;

[Dependency, UsedImplicitly]
public class ChoresPatcher {

    private static EventDispatcher events = null!;
    private static MultiplayerObjects objects = null!;

    private readonly Harmony harmony;
    private List<Type> supportedTypes = [];

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ChoresPatcher>();

    public ChoresPatcher(
        EventDispatcher events,
        Harmony harmony,
        StateMachinesPatcher patcher,
        MultiplayerObjects objects
    ) {
        ChoresPatcher.events = events;
        ChoresPatcher.objects = objects;
        this.harmony = harmony;
        ChoresMultiplayerConfiguration.Configuration
            .Select(it => it.StatesConfigurer)
            .NotNull()
            .ForEach(patcher.Register);
        events.Subscribe<RuntimeReadyEvent>(OnRuntimeReady);
    }

    private void OnRuntimeReady(RuntimeReadyEvent @event) {
        var postfix = new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => ChoreConstructorPostfix(null!, null!)));

        supportedTypes = ChoresMultiplayerConfiguration.Configuration
            .Select(it => it.ChoreType)
            .NotNull()
            .ToList();

        supportedTypes
            .Select(it => it.GetConstructors()[0])
            .ForEach(it => harmony.CreateProcessor(it).AddPostfix(postfix).Patch());

        log.Info($"{ChoresMultiplayerConfiguration.Configuration.Length} chore types patched");

        harmony.CreateProcessor(typeof(Chore).GetConstructors()[0])
            .AddPostfix(SymbolExtensions.GetMethodInfo(() => AddMultiplayerPreconditions(null!)))
            .Patch();

        harmony.CreateProcessor(typeof(Chore).GetMethod(nameof(Chore.Cleanup)))
            .AddPostfix(SymbolExtensions.GetMethodInfo(() => ChoreCleanup(null!)))
            .Patch();
    }

    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    [RequireMultiplayerMode(MultiplayerMode.Client)]
    private static void AddMultiplayerPreconditions(Chore __instance) {
        __instance.AddPrecondition(MultiplayerDriverChores.IsDriverBusy);
        __instance.AddPrecondition(MultiplayerDriverChores.IsMultiplayerChore);
    }

    public bool Supported(Chore chore) => supportedTypes.Contains(chore.GetType());

    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    private static void ChoreConstructorPostfix(Chore __instance, object[] __args) {
        var multiplayer = Dependencies.Get<MultiplayerGame>();
        switch (multiplayer.Mode) {
            case MultiplayerMode.Host:
                OnChoreCreated(__instance, __args);
                break;
            case MultiplayerMode.Client:
                CancelChore(__instance);
                break;
        }
    }

    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    private static void ChoreCleanup(Chore __instance) {
        objects.RemoveObject(__instance);
        events.Dispatch(new ChoreCleanupEvent(__instance));
    }

    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    private static void OnChoreCreated(Chore chore, object[] arguments) {
        var serializable = chore.GetSMI().stateMachine.serializable;
        var id = chore.Register(persistent: serializable == StateMachine.SerializeType.Never);
        events.Dispatch(new ChoreCreatedEvent(chore, id, chore.GetType(), arguments));
    }

    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void CancelChore(Chore chore) => Dependencies.Get<UnityTaskScheduler>().Run(
        () => { chore.Cancel($"Chore instantiation of type \"{chore.GetType().GetSignature()}\" is disabled"); }
    );

}
