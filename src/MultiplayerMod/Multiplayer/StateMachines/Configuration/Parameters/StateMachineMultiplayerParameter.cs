using System.IO;

namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.Parameters;

/// <summary>
/// A multiplayer non-serializable state machine parameter (i.e. no data in save files)
/// </summary>
public class StateMachineMultiplayerParameter<TStateMachine, TStateMachineInstance, TMaster, TDef, TValue>
    : StateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.Parameter<TValue?>
    where TStateMachine : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>
    where TStateMachineInstance : GameStateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.GameInstance
    where TMaster : IStateMachineTarget
{

    public override StateMachine.Parameter.Context CreateContext() => new Context(this, defaultValue);

    public new class Context(StateMachine.Parameter parameter, TValue? defaultValue)
        : StateMachine<TStateMachine, TStateMachineInstance, TMaster, TDef>.Parameter<TValue?>.Context(
            parameter,
            defaultValue
        ) {

        public override void Serialize(BinaryWriter writer) { }

        public override void Deserialize(IReader reader, StateMachine.Instance smi) { }

        public override void ShowEditor(StateMachine.Instance smi) { }

        public override void ShowDevTool(StateMachine.Instance smi) { }

    }

}
