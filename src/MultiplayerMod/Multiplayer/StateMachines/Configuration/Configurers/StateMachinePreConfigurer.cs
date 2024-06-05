using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MultiplayerMod.Core.Patch.ControlFlow;
using MultiplayerMod.Core.Patch.ControlFlow.Evaluators;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using static MultiplayerMod.Multiplayer.StateMachines.Configuration.StateMachineConfigurationPhase;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;

public class StateMachinePreConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    StateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef> root,
    StateMachineConfigurationContext context
) : StateMachineBaseConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>(root, context)
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
{

    private readonly ControlFlowCustomizer customizer = Dependencies.Get<ControlFlowCustomizer>();

    private readonly StateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef> rootConfigurer =
        root;

    public void PostConfigure(
        Action<StateMachinePostConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>, TStateMachine> action
    ) => rootConfigurer.PostConfigure(action);

    public void Suppress(Expression<System.Action> expression) {
        var (state, method) = ExtractMethodCallInfo(expression);
        var scope = typeof(TStateMachine).GetMethod(nameof(StateMachine.InitializeStates))!;
        rootConfigurer.AddAction(
            ControlFlowApply,
            _ => customizer.Detour(state, method, state, new MethodBoundedDetour(scope))
        );
        rootConfigurer.AddAction(ControlFlowReset, _ => customizer.Reset(state));
    }

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
