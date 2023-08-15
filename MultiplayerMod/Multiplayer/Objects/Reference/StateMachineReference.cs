using System;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class StateMachineReference {

    private ComponentReference<StateMachineController> ControllerReference { get; set; }
    private Type StateMachineType { get; set; }

    public StateMachineReference(
        ComponentReference<StateMachineController> controllerReference,
        Type stateMachineType
    ) {
        ControllerReference = controllerReference;
        StateMachineType = stateMachineType;
    }

    public StateMachine.Instance? GetInstance() => ControllerReference.GetComponent().GetSMI(StateMachineType);

}
