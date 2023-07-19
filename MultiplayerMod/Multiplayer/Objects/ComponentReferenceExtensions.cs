using System.Runtime.CompilerServices;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Objects;

public static class ComponentReferenceExtensions {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentReference GetMultiplayerReference(this KMonoBehaviour component) =>
        new(component.gameObject.GetMultiplayerReference(), component.GetType());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentReference GetGridReference(this KMonoBehaviour component) =>
        new(component.gameObject.GetGridReference(), component.GetType());
}
