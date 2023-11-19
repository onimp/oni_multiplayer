using System.Runtime.CompilerServices;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Objects;

public static class ComponentReferenceExtensions {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentReference GetReference(this KMonoBehaviour component) => new(
        component.gameObject.GetReference(),
        component.GetType()
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentReference<T> GetReference<T>(this T component) where T : KMonoBehaviour =>
        new(component.gameObject.GetReference());
}
