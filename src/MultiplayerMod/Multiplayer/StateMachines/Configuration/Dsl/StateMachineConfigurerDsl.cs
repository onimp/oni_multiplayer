using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MultiplayerMod.Core.Patch.ControlFlow;
using MultiplayerMod.Core.Patch.ControlFlow.Evaluators;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using static MultiplayerMod.Multiplayer.StateMachines.Configuration.StateMachineConfigurationPhase;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Dsl;

[SuppressMessage("ReSharper", "UnusedTypeParameter")]
// ReSharper disable once TypeParameterCanBeVariant
public interface IStateMachineConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef> {
    void PreConfigure(Action<TStateMachine> action);
    void PostConfigure(Action<TStateMachine> action);
    void Suppress(Expression<System.Action> expression);
    void Inline(StateMachineConfigurer configurer);
}

public class StateMachineConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    StateMachineConfigurationContext context
) : IStateMachineConfigurerDsl<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget {

    private readonly StateMachineConfiguration configuration = context.GetConfiguration<TStateMachine>();

    private readonly ControlFlowCustomizer customizer = Dependencies.Get<ControlFlowCustomizer>();
    public MultiplayerGame Game { get; init; } = Dependencies.Get<MultiplayerGame>();

    private void AddAction(StateMachineConfigurationPhase phase, Action<TStateMachine> action) =>
        configuration.Actions.Add(
            new StateMachineConfigurationAction(phase, stateMachine => action((TStateMachine) stateMachine))
        );

    public void PreConfigure(Action<TStateMachine> action) => AddAction(PreConfiguration, action);

    public void PostConfigure(Action<TStateMachine> action) => AddAction(PostConfiguration, action);

    public void Suppress(Expression<System.Action> expression) {
        var (state, method) = ExtractMethodCallInfo(expression);
        var scope = typeof(TStateMachine).GetMethod(nameof(StateMachine.InitializeStates))!;
        AddAction(ControlFlowApply, _ => customizer.Detour(state, method, state, new MethodBoundedDetour(scope)));
        AddAction(ControlFlowReset, _ => customizer.Reset(state));
    }

    public void Inline(StateMachineConfigurer configurer) => configurer.Configure(context);

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
