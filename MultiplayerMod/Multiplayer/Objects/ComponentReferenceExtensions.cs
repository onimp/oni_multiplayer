using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Objects;

public static class ComponentReferenceExtensions {

    private static readonly HashSet<Type> movableObjectTypes = new() {
        typeof(MinionAssignablesProxy),
        typeof(MinionResume),
        typeof(Schedulable),
        typeof(ConsumableConsumer),
        typeof(ChoreConsumer)
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentReference GetReference(this KMonoBehaviour component)
        => component.Movable()
            ? new ComponentReference(component.gameObject.GetMultiplayerReference(), component.GetType())
            : new ComponentReference(component.gameObject.GetGridReference(), component.GetType());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentReference<T> GetReference<T>(this T component) where T : KMonoBehaviour
        => component.Movable()
            ? new ComponentReference<T>(component.gameObject.GetMultiplayerReference())
            : new ComponentReference<T>(component.gameObject.GetGridReference());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool Movable(this KMonoBehaviour kMonoBehaviour) =>
        movableObjectTypes.Contains(kMonoBehaviour.GetType());

}
