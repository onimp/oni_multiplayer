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
using MultiplayerMod.Multiplayer.Chores.Events;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.StateMachines;

namespace MultiplayerMod.Multiplayer.Chores;

[Dependency, UsedImplicitly]
public class ChoresPatcher {

    private static EventDispatcher events = null!;

    private readonly Harmony harmony;
    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ChoresPatcher>();

    public ChoresPatcher(EventDispatcher events, Harmony harmony, StateMachinesPatcher patcher) {
        ChoresPatcher.events = events;
        this.harmony = harmony;
        ChoresMultiplayerConfiguration.Configuration
            .Select(it => it.StatesConfigurer)
            .NotNull()
            .ForEach(patcher.Register);
        events.Subscribe<RuntimeReadyEvent>(OnRuntimeReady);
    }

    private void OnRuntimeReady(RuntimeReadyEvent @event) {
        var postfix = new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => ChoreConstructorPostfix(null!, null!)));
        ChoresMultiplayerConfiguration.Configuration
            .Select(it => it.ChoreType)
            .NotNull()
            .Select(it => it.GetConstructors()[0])
            .ForEach(it => harmony.CreateProcessor(it).AddPostfix(postfix).Patch());
        log.Info($"{ChoresMultiplayerConfiguration.Configuration.Length} chore types patched");
    }

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
