using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Dsl;

public class StateMachineConfigurerDsl {
    public static class Conditions {
        public static Func<bool> MultiplayerMode(MultiplayerMode mode) =>
            () => Dependencies.Get<MultiplayerGame>().Mode == mode;
    }
}

public class StateMachineConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef> : StateMachineConfigurerDsl
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
{

    private readonly List<StateMachineConfigurationAction> actions = [];

    public void Phase(StateMachineConfigurationPhase phase, System.Action action) =>
        actions.Add(new StateMachineConfigurationAction(phase, action));

    public void PreConfigure(System.Action action) => Phase(StateMachineConfigurationPhase.PreConfiguration, action);

    public void PostConfigure(System.Action action) => Phase(StateMachineConfigurationPhase.PostConfiguration, action);

    public void Conditional(
        Func<bool> condition,
        Action<StateMachineConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>> scopedAction
    ) {
        var scopedDsl = new StateMachineConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>();
        scopedAction(scopedDsl);
        var scopedActions = scopedDsl.actions.Select(
            it => it with {
                Action = () => {
                    if (condition())
                        it.Action();
                }
            }
        );
        actions.AddRange(scopedActions);
    }

    public StateMachineConfiguration GetConfiguration() => new(typeof(TMaster), typeof(TStateMachine), actions);

}
