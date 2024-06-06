using System.Linq;
using MultiplayerMod.Multiplayer.StateMachines.Configuration.Parameters;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Configurers;

public abstract class StateMachineBaseConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef>(
    StateMachineRootConfigurer<TStateMachine, TStateMachineInstance, TMaster, TDef> root,
    StateMachineConfigurationContext context,
    TStateMachine stateMachine
)
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
{

    public readonly MultiplayerGame Game = context.Dependencies.Get<MultiplayerGame>();
    public MultiplayerMode MultiplayerMode => Game.Mode;
    public TStateMachine StateMachine { get; } = stateMachine;

    public GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.State AddState(
        GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.State parent,
        string name
    ) {
        if (parent.sm != StateMachine)
            throw new StateMachineConfigurationException($"State {parent.name} doesn't belong to {StateMachine.name}");
        var state = new GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.State();
        StateMachine.BindState(parent, state, name);
        state.sm = StateMachine;
        return state;
    }

    public StateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.Parameter<T?> AddMultiplayerParameter<T>(
        StateMachineMultiplayerParameterInfo<T> parameterInfo
    ) {
        if (parameterInfo.Shared) {
            var existingParameter = StateMachine.parameters.FirstOrDefault(it => it.name == parameterInfo.Name);
            if (existingParameter != null)
                return (StateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.Parameter<T?>)
                    existingParameter;
        }
        var parameter = new StateMachineMultiplayerParameter<TStateMachine, TStateMachineInstance, TMaster, TDef, T> {
            name = parameterInfo.Name,
            idx = StateMachine.parameters.Length,
            defaultValue = (T?) parameterInfo.DefaultValue
        };
        StateMachine.parameters = StateMachine.parameters.Append(parameter);
        return parameter;
    }

}
