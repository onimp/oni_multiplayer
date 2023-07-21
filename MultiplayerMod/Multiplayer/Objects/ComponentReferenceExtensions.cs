using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Objects;

public static class ComponentReferenceExtensions {

    private static HashSet<Type> MovableObjectTypes = new() {
        typeof(MinionAssignablesProxy),
        typeof(MinionResume),
        typeof(Schedulable),
        typeof(ConsumableConsumer),
        typeof(ChoreConsumer)
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentReference GetReference(this KMonoBehaviour component)
        => IsMovableObject(component)
            ? new ComponentReference(component.gameObject.GetMultiplayerReference(), component.GetType())
            : new ComponentReference(component.gameObject.GetGridReference(), component.GetType());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsMovableObject(this KMonoBehaviour kMonoBehaviour) =>
        MovableObjectTypes.Contains(kMonoBehaviour.GetType());

}
