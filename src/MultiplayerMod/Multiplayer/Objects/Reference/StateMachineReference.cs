using System;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class StateMachineReference : TypedReference<StateMachine.Instance> {

    private ComponentReference<StateMachineController> ControllerReference { get; set; }
    private Type StateMachineType { get; set; }

    public StateMachineReference(
        ComponentReference<StateMachineController> controllerReference,
        Type stateMachineType
    ) {
        ControllerReference = controllerReference;
        StateMachineType = stateMachineType;
    }

    public override StateMachine.Instance Resolve() => ControllerReference.Resolve().GetSMI(StateMachineType);

}
