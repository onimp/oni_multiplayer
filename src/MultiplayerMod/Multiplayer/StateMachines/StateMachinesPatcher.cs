using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;

namespace MultiplayerMod.Multiplayer.StateMachines;

[Dependency, UsedImplicitly]
public class StateMachinesPatcher {

    private static Dictionary<Type, ConfigurerExecutor> executors = new();

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<StateMachinesPatcher>();
    private readonly Harmony harmony;
    private readonly List<StateMachineConfigurer> configurers = [];

    private bool configured;

    public StateMachinesPatcher(EventDispatcher events, Harmony harmony) {
        this.harmony = harmony;
        events.Subscribe<RuntimeReadyEvent>(OnRuntimeReady);
    }

    public void Register(StateMachineConfigurer configurer) {
        if (configured) {
            var name = configurer.StateMachineType.GetPrettyName();
            throw new StateMachineConfigurationException(
                $"Unable to register a configurer for {name}: state machines already configured"
            );
        }
        configurers.Add(configurer);
    }

    private void OnRuntimeReady(RuntimeReadyEvent @event) {
        executors = configurers.ToDictionary(it => it.StateMachineType, it => new ConfigurerExecutor(it));
        var prefix = new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => InitializeStatesPrefix(null!)));
        var postfix = new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => InitializeStatesPostfix(null!)));
        configurers.Select(it => it.StateMachineType)
            .Select(it => it.GetMethod(nameof(StateMachine.InitializeStates)))
            .ForEach(it => harmony.CreateProcessor(it).AddPrefix(prefix).AddPostfix(postfix).Patch());
        configured = true;
        log.Info($"{configurers.Count} state machine types patched");
    }

    private static void InitializeStatesPrefix(StateMachine __instance) {
        var executor = executors[__instance.GetType()];
        executor.Apply(__instance);
    }

    private static void InitializeStatesPostfix(StateMachine __instance) {
        var executor = executors[__instance.GetType()];
        executor.Execute(StateMachineConfigurationPhase.ControlFlowReset);
        executor.Execute(StateMachineConfigurationPhase.PostConfiguration);
    }

    private class ConfigurerExecutor(StateMachineConfigurer configurer) {

        private StateMachineConfiguration configuration = null!;

        public void Apply(StateMachine sm) => configuration = configurer.Configure(sm);

        public void Execute(StateMachineConfigurationPhase phase) => configuration.Actions
            .Where(it => it.Phase == phase)
            .ForEach(it => it.Action());

    }

}
