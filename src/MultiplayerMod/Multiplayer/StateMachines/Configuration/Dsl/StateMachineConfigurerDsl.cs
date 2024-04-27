using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Patch.ControlFlow;
using MultiplayerMod.Core.Patch.ControlFlow.Evaluators;
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
        var scope = typeof(TStateMachine).GetMethod(nameof(StateMachine.InitializeStates))!;
        customizer.Detour(state, method, state, new MethodBoundedDetour(scope));
        Phase(StateMachineConfigurationPhase.ControlFlowReset, () => customizer.Reset(state));
    }

    public StateMachineConfiguration GetConfiguration() => new(typeof(TMaster), typeof(TStateMachine), actions);

    private StateMachine.BaseState ExtractStateInstance(MemberExpression memberExpression) {
        var chain = new LinkedList<FieldInfo>();

        System.Linq.Expressions.Expression current = memberExpression;
        while (current is MemberExpression expression) {
            if (expression.Member.MemberType != MemberTypes.Field)
                throw new InvalidStateExpressionException("Only a closure field access chain is supported");

            chain.AddFirst((FieldInfo) expression.Member);
            current = expression.Expression;
        }

        if (current is not ConstantExpression constantExpression)
            throw new InvalidStateExpressionException("Only a constant expression of closure instance is supported");

        return (StateMachine.BaseState) chain.Aggregate(constantExpression.Value, (obj, field) => field.GetValue(obj));
    }

    private (StateMachine.BaseState, MethodInfo) ExtractMethodCallInfo(LambdaExpression expression) {
        if (expression.Body is not MethodCallExpression body)
            throw new InvalidStateExpressionException("Only a method call expression is supported");

        if (body.Object is not MemberExpression memberExpression)
            throw new InvalidStateExpressionException("Only a field or property access is supported");

        return (ExtractStateInstance(memberExpression), body.Method);
    }

}
