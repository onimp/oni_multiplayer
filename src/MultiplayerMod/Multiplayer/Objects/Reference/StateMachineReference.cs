using System;
using System.Reflection;
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

    private static readonly MethodInfo getSmiMethod = typeof(StandardChoreBase).GetMethod(
        "GetSMI",
        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
        null,
        Type.EmptyTypes,
        null
    )!;

    private MultiplayerId id = objects.Get(chore)!.Id;

    public override StateMachine.Instance Resolve() =>
        (StateMachine.Instance) getSmiMethod.Invoke(objects.Get<Chore>(id)!, null)!;

    public StateMachine.Instance Get() => Resolve();

}
