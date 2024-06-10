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
using static MultiplayerMod.Multiplayer.StateMachines.Configuration.StateMachineConfigurationPhase;

namespace MultiplayerMod.Multiplayer.StateMachines;

[Dependency, UsedImplicitly]
public class StateMachinesPatcher {

    private static Dictionary<Type, Runner> runners = new();

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<StateMachinesPatcher>();
    private readonly Harmony harmony;
    private readonly List<StateMachineConfigurer> configurers = [];
    private readonly StateMachineConfigurationContext context;

    public StateMachinesPatcher(EventDispatcher events, Harmony harmony, IDependencyContainer container) {
        this.harmony = harmony;
        context = new StateMachineConfigurationContext(container);
        events.Subscribe<RuntimeReadyEvent>(OnRuntimeReady);
    }

    public void Register(StateMachineConfigurer configurer) {
        if (context.Locked) {
            var name = configurer.StateMachineType.GetSignature();
            throw new StateMachineConfigurationException(
                $"Unable to register a configurer for {name}: state machines already configured"
            );
        }
        configurers.Add(configurer);
    }

    private void OnRuntimeReady(RuntimeReadyEvent @event) {
        var prefix = new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => InitializeStatesPrefix(null!)));
        var postfix = new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => InitializeStatesPostfix(null!)));
        configurers.ForEach(it => it.Configure(context));
        runners = context.Configurations.ToDictionary(it => it.StateMachineType, it => new Runner(it));
        runners.Keys
            .Select(it => it.GetMethod(nameof(StateMachine.InitializeStates)))
            .ForEach(it => harmony.CreateProcessor(it).AddPrefix(prefix).AddPostfix(postfix).Patch());
        context.Lock();
        log.Info($"{context.Configurations.Count} state machine types patched");
    }

    private static void InitializeStatesPrefix(StateMachine __instance) {
        var runner = runners[__instance.GetType()];
        runner.StartPhase(PreConfiguration, __instance);
        runner.StartPhase(ControlFlowApply, __instance);
    }

    private static void InitializeStatesPostfix(StateMachine __instance) {
        var runner = runners[__instance.GetType()];
        runner.StartPhase(ControlFlowReset, __instance);
        runner.StartPhase(PostConfiguration, __instance);
    }

    private class Runner(StateMachineConfiguration configuration) {

        public void StartPhase(StateMachineConfigurationPhase phase, StateMachine stateMachine) =>
            AssertConsistency(phase, RunActions(phase, stateMachine));

        private int RunActions(StateMachineConfigurationPhase phase, StateMachine stateMachine) {
            var length = configuration.Actions.Count;
            for (var i = 0; i < length; i++) {
                var action = configuration.Actions[i];
                if (action.Phase == phase)
                    action.Configure(stateMachine);
            }
            return length;
        }

        private void AssertConsistency(StateMachineConfigurationPhase phase, int length) {
            var newLength = configuration.Actions.Count;
            for (var i = length; i < newLength; i++) {
                var action = configuration.Actions[i];
                if (action.Phase <= phase)
                    throw new InvalidConfigurationPhaseException(configuration.StateMachineType, action.Phase, phase);
            }
        }

    }

}
