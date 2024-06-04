using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.Chores.Commands;
using MultiplayerMod.Multiplayer.Chores.Events;
using MultiplayerMod.Multiplayer.CoreOperations;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.StateMachines;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Chores;

[Core.Dependency.Dependency, UsedImplicitly]
public class ChoresPatcher {

    private static EventDispatcher events = null!;
    private static IMultiplayerServer server = null!;
    private static List<Type> choreTypes = [];

    private readonly Harmony harmony;
    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ChoresPatcher>();
    private readonly ConditionalWeakTable<ChoreDriver, BoxedValue<bool>> driverSynchronizationState = new();

    public ChoresPatcher(
        EventDispatcher events,
        Harmony harmony,
        StateMachinesPatcher patcher,
        IMultiplayerServer server
    ) {
        ChoresPatcher.events = events;
        ChoresPatcher.server = server;
        this.harmony = harmony;
        ChoresMultiplayerConfiguration.Configuration
            .Select(it => it.StatesConfigurer)
            .NotNull()
            .ForEach(patcher.Register);
        events.Subscribe<RuntimeReadyEvent>(OnRuntimeReady);
    }

    private void OnRuntimeReady(RuntimeReadyEvent @event) {
        var postfix = new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => ChoreConstructorPostfix(null!, null!)));

        choreTypes = ChoresMultiplayerConfiguration.Configuration
            .Select(it => it.ChoreType)
            .NotNull()
            .ToList();

        choreTypes
            .Select(it => it.GetConstructors()[0])
            .ForEach(it => harmony.CreateProcessor(it).AddPostfix(postfix).Patch());

        log.Info($"{ChoresMultiplayerConfiguration.Configuration.Length} chore types patched");

        harmony.CreateProcessor(typeof(Chore).GetConstructors()[0])
            .AddPostfix(SymbolExtensions.GetMethodInfo(() => AddMultiplayerPreconditions(null!))).Patch();

        ChoreDriverEvents.ChoreSet += ChoreDriverOnChoreSet;
    }

    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    [RequireMultiplayerMode(MultiplayerMode.Client)]
    private static void AddMultiplayerPreconditions(Chore __instance) {
        __instance.AddPrecondition(DriverChoresQueue.IsDriverBusy);
    }

    private static bool Supported(Chore chore) => choreTypes.Contains(chore.GetType());

    private void ChoreDriverOnChoreSet(
        ChoreDriver driver,
        Chore? previousChore,
        ref Chore.Precondition.Context context
    ) {
        var synchronized = driverSynchronizationState.GetValue(driver, _ => new BoxedValue<bool>(false));
        var shouldDesyncQueue = synchronized.Value && previousChore != null && !Supported(previousChore);
        if (shouldDesyncQueue) {
            server.Send(new ClearChoresQueue(driver), MultiplayerCommandOptions.SkipHost);
            synchronized.Value = false;
        }

        if (!Supported(context.chore))
            return;

        var queueCommand = new QueueChore(driver, context.consumerState.consumer, context.chore, context.data);
        server.Send(queueCommand, MultiplayerCommandOptions.SkipHost);
        synchronized.Value = true;
    }

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
    private static void OnChoreCreated(Chore chore, object[] arguments) {
        var id = chore.Register();
        events.Dispatch(new ChoreCreatedEvent(id, chore.GetType(), arguments));
    }

    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void CancelChore(Chore chore) => Dependencies.Get<UnityTaskScheduler>().Run(
        () => { chore.Cancel($"Chore instantiation of type \"{chore.GetType().GetSignature()}\" is disabled"); }
    );

}
