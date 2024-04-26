using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Patch.ControlFlow;
using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Dsl;

public class StateMachineConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
{

    private readonly List<StateMachineConfigurationAction> actions = [];

    private readonly Lazy<MultiplayerGame> game = new(Dependencies.Get<MultiplayerGame>);
    public MultiplayerGame Game => game.Value;

    public void Phase(StateMachineConfigurationPhase phase, System.Action action) =>
        actions.Add(new StateMachineConfigurationAction(phase, action));

    public void PostConfigure(System.Action action) => Phase(StateMachineConfigurationPhase.PostConfiguration, action);

    public void Suppress(Expression<System.Action> expression) {
        var customizer = Dependencies.Get<ControlFlowCustomizer>();
        var (state, method) = ExtractMethodCallInfo(expression);
        customizer.Detour(state, method, state);
        Phase(StateMachineConfigurationPhase.ControlFlowReset, () => customizer.Reset(state));
    }

    public StateMachineConfiguration GetConfiguration() => new(typeof(TMaster), typeof(TStateMachine), actions);

    private (StateMachine.BaseState, MethodInfo) ExtractMethodCallInfo(LambdaExpression expression) {
        if (expression.Body is not MethodCallExpression body)
            throw new InvalidStateExpressionException("Only a method call expression is supported");

        if (body.Object is not MemberExpression member)
            throw new InvalidStateExpressionException("Only a field or property access is supported");

        if (member.Expression is not ConstantExpression constantExpression)
            throw new InvalidStateExpressionException("Only a constant expression of closure instance is supported");

        var closure = constantExpression.Value;
        var stateType = typeof(StateMachine.BaseState);
        var stateField = closure.GetType()
            .GetFields()
            .FirstOrDefault(it => stateType.IsAssignableFrom(it.FieldType));

        if (stateField == null)
            throw new InvalidStateExpressionException(
                $"Unable to find a field of type \"{stateType.GetPrettyName()}\" in the closure \"{closure.GetType()}\""
            );

        var state = stateField.GetValue(closure);
        return ((StateMachine.BaseState) state, body.Method);
    }

}
