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

    public override StateMachine.Instance Resolve() =>
        ControllerReference.Resolve().GetSMI(StateMachineInstanceType) ?? throw new ObjectNotFoundException(this);

}

[Serializable]
[DependenciesStaticTarget]
public class ChoreStateMachineReference(Chore chore) : TypedReference<StateMachine.Instance> {

    [InjectDependency]
    private static MultiplayerObjects objects = null!;

    private MultiplayerId id = objects.Get(chore)!.Id;

    // The referenced chore may already have been cleaned up on the client (e.g. a dig completed and the
    // WorkChore was removed) by the time a state-machine command for it arrives. Surface that as an
    // ObjectNotFoundException - which CommandExceptionHandler logs and skips - instead of NRE-crashing
    // the client on the null chore. The chore can also still exist while its state machine has already
    // been stopped, in which case GetSMI() returns null; a null instance blows up downstream in
    // StateMachineRuntimeTools (ConditionalWeakTable rejects a null key), so treat it the same way.
    public override StateMachine.Instance Resolve() {
        var chore = objects.Get<Chore>(id);
        if (chore == null)
            throw new ObjectNotFoundException(this);
        return ((StandardChoreBase) chore).GetSMI() ?? throw new ObjectNotFoundException(this);
    }

    public StateMachine.Instance Get() => Resolve();

}
