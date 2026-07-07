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
    private readonly List<IChoreConfigurer> configurers;

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ChoresPatcher>();

    public ChoresPatcher(
        EventDispatcher events,
        Harmony harmony,
        StateMachinesPatcher patcher,
        MultiplayerObjects objects,
        List<IChoreConfigurer> configurers
    ) {
        ChoresPatcher.events = events;
        ChoresPatcher.objects = objects;
        this.harmony = harmony;
        this.configurers = configurers;
        events.Subscribe<RuntimeReadyEvent>(OnRuntimeReady);
    }

    private void OnRuntimeReady(RuntimeReadyEvent @event) {
        var postfix = new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => ChoreConstructorPostfix(null!, null!)));

        supportedTypes = configurers.Select(it => it.ChoreType).NotNull().ToList();
        supportedTypes
            .Select(it => it.GetConstructors()[0])
            .ForEach(it => harmony.CreateProcessor(it).AddPostfix(postfix).Patch());

        log.Info($"{supportedTypes.Count} chore types patched:\n\t{string.Join("\n\t", supportedTypes.Select(it => it.GetSignature()))}");

        // U57 inserted an abstract `Chore` facade above the old concrete base; the real constructor
        // and Cleanup body now live on StandardChoreBase, which every chore still passes through.
        // typeof(Chore).GetConstructors() is now empty (abstract, no public ctor) and Chore.Cleanup
        // is abstract, so both patches must target StandardChoreBase instead.
        harmony.CreateProcessor(typeof(StandardChoreBase).GetConstructors()[0])
            .AddPostfix(SymbolExtensions.GetMethodInfo(() => AddMultiplayerPreconditions(null!)))
            .Patch();

        harmony.CreateProcessor(typeof(StandardChoreBase).GetConstructors()[0])
            .AddPostfix(SymbolExtensions.GetMethodInfo(() => AddOwnershipPrecondition(null!)))
            .Patch();

        harmony.CreateProcessor(typeof(StandardChoreBase).GetMethod(nameof(StandardChoreBase.Cleanup)))
            .AddPostfix(SymbolExtensions.GetMethodInfo(() => ChoreCleanup(null!)))
            .Patch();
    }

    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    [RequireMultiplayerMode(MultiplayerMode.Client)]
    private static void AddMultiplayerPreconditions(Chore __instance) {
        __instance.AddPrecondition(MultiplayerDriverChores.IsDriverBusy);
        __instance.AddPrecondition(MultiplayerDriverChores.IsMultiplayerChore);
    }

    // Phase 2: only the host scores chores, so the per-player work-preference precondition is host-only and
    // scoped to work chores (dig/build/sweep/...). It self-gates when the feature is disabled, so it can be
    // added unconditionally here and toggled at runtime.
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    [RequireMultiplayerMode(MultiplayerMode.Host)]
    private static void AddOwnershipPrecondition(Chore __instance) {
        if (IsWorkChore(__instance.GetType()))
            __instance.AddPrecondition(Ownership.ChoreOwnershipPreconditions.PrefersOwnerChore);
    }

    public bool Supported(Chore chore) => supportedTypes.Contains(chore.GetType()) || IsWorkChore(chore.GetType());

    // WorkChore<WorkableType> backs almost all duplicant labor (dig / build / sweep / deliver / ...).
    // It's an open generic we cannot list or patch per-instantiation: on Mono all reference-type
    // WorkChore<T> share one native code body, so Harmony-patching a single closed WorkChore<T>
    // corrupts the generic type parameter for every other WorkChore<T> (observed as "X does not have
    // component WorkerOilRefiller" spam). Instead we recognize it structurally here and replicate it
    // lazily from a recipe at driver-assignment time (see ChoreDriverSynchronization) — the client
    // rebuilds it via reflection Invoke, which is safe (only *patching* the generic ctor is unsafe).
    public static bool IsWorkChore(Type choreType) =>
        choreType.IsGenericType && choreType.GetGenericTypeDefinition() == typeof(WorkChore<>);

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
        // Capture the shared id BEFORE the index drops the chore, so the host can broadcast which
        // replicated chore ended (HostEventsBinder -> CompleteChore) and clients end their copy in lockstep.
        var id = objects.Get(__instance)?.Id;
        objects.RemoveObject(__instance);
        events.Dispatch(new ChoreCleanupEvent(__instance, id));
    }

    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    private static void OnChoreCreated(Chore chore, object[] arguments) {
        var serializable = ((StandardChoreBase) chore).GetSMI().stateMachine.serializable;
        var id = chore.Register(persistent: serializable == StateMachine.SerializeType.Never);
        events.Dispatch(new ChoreCreatedEvent(chore, id, chore.GetType(), arguments));
    }

    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void CancelChore(Chore chore) => Dependencies.Get<UnityTaskScheduler>().Run(
        () => { chore.Cancel($"Chore instantiation of type \"{chore.GetType().GetSignature()}\" is disabled"); }
    );

}
