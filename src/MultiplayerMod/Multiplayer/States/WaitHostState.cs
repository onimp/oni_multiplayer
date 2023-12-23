using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace MultiplayerMod.Multiplayer.States;

public class
    WaitHostState<StateMachineType, StateMachineInstanceType, MasterType, DefType> : StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
    where StateMachineInstanceType : StateMachine.Instance
    where MasterType : IStateMachineTarget {

    [UsedImplicitly] public WaitStateParam<bool> TransitionAllowed { get; }
    [UsedImplicitly] public WaitStateParam<string?> TargetState { get; }
    [UsedImplicitly] public WaitStateParam<Dictionary<int, object>> ParametersArgs { get; }

    public WaitHostState(StateMachine sm) {
        name = StatesManager.StateName;
        TransitionAllowed = InitParam(sm, false);
        TargetState = InitParam(sm, (string?) null);
        ParametersArgs = InitParam(sm, new Dictionary<int, object>());

        enterActions = new List<StateMachine.Action>
            { new("Wait for host transition", new Callback(TransitIfAllowed)) };

        var root = sm.GetType().GetField("root").GetValue(sm);
        sm.GetType().GetMethod("BindState").Invoke(sm, new[] { root, this, name });
    }

    [UsedImplicitly]
    public void AllowTransition(StateMachine.Instance smi, string? target, Dictionary<int, object> args) {
        TransitionAllowed.Set(true, smi);
        TargetState.Set(target, smi);
        ParametersArgs.Set(args, smi);
        TransitIfAllowed(smi);
    }

    private void TransitIfAllowed(StateMachine.Instance smi) {
        if (smi.GetCurrentState() != this) {
            return;
        }
        if (!TransitionAllowed.Get(smi)) {
            return;
        }
        foreach (var (parameterIndex, value) in ParametersArgs.Get(smi)) {
            var parameterContext = smi.parameterContexts[parameterIndex];
            parameterContext.GetType().GetMethod("Set")!.Invoke(parameterContext, new[] { value, smi, false });
        }
        smi.GoTo(TargetState.Get(smi));
    }

    private WaitStateParam<ParamType> InitParam<ParamType>(StateMachine sm, ParamType value) {
        var param = new WaitStateParam<ParamType>(value) {
            name = nameof(WaitStateParam<ParamType>),
            idx = sm.parameters.Length
        };
        sm.parameters = sm.parameters.Append<StateMachine.Parameter>(param);
        return param;
    }

    [UsedImplicitly]
    public class WaitStateParam<ParameterType> : StateMachine.Parameter {
        private readonly ParameterType defaultValue;

        public WaitStateParam(ParameterType defaultValue) {
            this.defaultValue = defaultValue;
        }

        public ParameterType Get(StateMachine.Instance smi) =>
            ((WaitStateContext<ParameterType>) smi.GetParameterContext(this)).Value;

        public void Set(ParameterType value, StateMachine.Instance smi) =>
            ((WaitStateContext<ParameterType>) smi.GetParameterContext(this)).Set(value);

        public override Context CreateContext() => new WaitStateContext<ParameterType>(this, defaultValue);
    }

    private class WaitStateContext<ParameterType> : StateMachine.Parameter.Context {
        public ParameterType Value;

        public WaitStateContext(WaitStateParam<ParameterType> parameter, ParameterType defaultValue) :
            base(parameter) {
            Value = defaultValue;
        }

        public void Set(ParameterType value) {
            if (EqualityComparer<ParameterType>.Default.Equals(value, Value))
                return;

            Value = value;
        }

        public override void Serialize(BinaryWriter _) { }

        public override void Deserialize(IReader reader, StateMachine.Instance _) { }

        public override void ShowEditor(StateMachine.Instance _) { }

        public override void ShowDevTool(StateMachine.Instance _) { }
    }
}
