using System;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class StateMachineReference(
    ComponentReference<StateMachineController> controllerReference,
    Type stateMachineInstanceType
) : TypedReference<StateMachine.Instance> {

    private ComponentReference<StateMachineController> ControllerReference { get; set; } = controllerReference;
    private Type StateMachineInstanceType { get; set; } = stateMachineInstanceType;

    public override StateMachine.Instance Resolve() => ControllerReference.Resolve().GetSMI(StateMachineInstanceType);

}

[Serializable]
[DependenciesStaticTarget]
public class ChoreStateMachineReference(Chore chore) : TypedReference<StateMachine.Instance> {

    [InjectDependency]
    private static MultiplayerObjects objects = null!;

    private MultiplayerId id = objects.Get(chore)!.Id;

    public override StateMachine.Instance Resolve() => objects.Get<Chore>(id)!.GetSMI();

    public StateMachine.Instance Get() => Resolve();

}
