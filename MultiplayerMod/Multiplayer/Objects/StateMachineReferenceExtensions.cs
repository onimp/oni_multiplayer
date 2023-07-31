using System.Reflection;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Objects;

public static class StateMachineReferenceExtensions {

    public static StateMachineReference GetReference(this StateMachine.Instance instance) => new(
        GetStateMachineController(instance).GetReference(),
        instance.GetType()
    );

    // `controller` field is defined in StateMachine<,,,>.GenericInstance. However cast is impossible due to unknown
    // generic argument types. So reflection is the most handy way to get its value :(
    private static StateMachineController GetStateMachineController(StateMachine.Instance instance) =>
        (StateMachineController) instance.GetType()
            .GetField("controller", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(instance);
}
