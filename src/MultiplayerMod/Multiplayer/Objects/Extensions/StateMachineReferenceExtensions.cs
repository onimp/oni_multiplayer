using MultiplayerMod.Core.Reflection;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Objects.Extensions;

public static class StateMachineReferenceExtensions {

    public static StateMachineReference GetReference(this StateMachine.Instance instance) => new(
        GetStateMachineController(instance).GetReference(),
        instance.GetType()
    );

    // `controller` field is defined in StateMachine<,,,>.GenericInstance. However cast is impossible due to unknown
    // generic argument types. So reflection is the most handy way to get its value :(
    private static StateMachineController GetStateMachineController(StateMachine.Instance instance) =>
        instance.GetFieldValue<StateMachineController>("controller");

}
