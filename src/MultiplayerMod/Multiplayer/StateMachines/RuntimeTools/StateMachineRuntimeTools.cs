using System.Runtime.CompilerServices;
using MultiplayerMod.Core.Reflection;
using MultiplayerMod.Game.NameOf;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Parameters;

namespace MultiplayerMod.Multiplayer.StateMachines.RuntimeTools;

public class StateMachineRuntimeTools {

    private static readonly ConditionalWeakTable<StateMachine.Instance, StateMachineRuntimeTools> cache = new();

    private readonly StateMachine.Instance instance;

    private StateMachineRuntimeTools(StateMachine.Instance instance) {
        this.instance = instance;
    }

    public StateMachineRuntimeParameter<T>? FindParameter<T>(StateMachineMultiplayerParameterInfo<T> parameterInfo) {
        var parameters = instance.stateMachine.parameters;

        // There are not many parameters, caching is probably unjustified
        for (var i = 0; i < parameters.Length; i++) {
            if (parameters[i].name == parameterInfo.Name)
                return new StateMachineRuntimeParameter<T>(instance, parameters[i]);
        }
        return null;
    }

    public void GoToState(string? name) {
        if (name != null) {
            var state = instance.stateMachine.GetState(name);
            if (state == null)
                throw new StateMachineStateNotFoundException(instance.stateMachine, name);
            instance.GoTo(state);
        }
        else
            instance.GoTo((StateMachine.BaseState?) null);
    }

    public StateMachineController GetController() => (StateMachineController) instance
        .GetFieldValue(nameof(StateMachineMemberReference.Instance.controller));

    public static StateMachineRuntimeTools Get(StateMachine.Instance instance) {
        if (cache.TryGetValue(instance, out var tools))
            return tools;
        tools = new StateMachineRuntimeTools(instance);
        cache.Add(instance, tools);
        return tools;
    }

}
