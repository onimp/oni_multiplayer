using System;
using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class StateMachineReference(
    ComponentReference<StateMachineController> controllerReference,
    Type stateMachineType
) : TypedReference<StateMachine.Instance> {

    private ComponentReference<StateMachineController> ControllerReference { get; set; } = controllerReference;
    private Type StateMachineType { get; set; } = stateMachineType;

    public override StateMachine.Instance Resolve() => ControllerReference.Resolve().GetSMI(StateMachineType);

}

[Serializable]
public class ChoreStateMachineReference(Chore chore) : TypedReference<StateMachine.Instance> {

    private MultiplayerId id = Dependencies.Get<MultiplayerGame>().Objects[chore]!;

    public override StateMachine.Instance Resolve() => Dependencies.Get<MultiplayerGame>().Objects
        .Get<Chore>(id)
        .GetSMI();

    public StateMachine.Instance Get() => Resolve();

}
