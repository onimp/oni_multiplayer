using System;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands;

public static class ArgumentUtils {

    public static object?[] WrapObjects(object?[] objects) {
        return objects.Select(WrapObject).ToArray();
    }

    public static object?[] UnWrapObjects(object?[] objects) {
        return objects.Select(UnWrapObject).ToArray();
    }

    public static object? WrapObject(object? obj) {
        return obj switch {
            GameObject gameObject => gameObject.GetReference(),
            KMonoBehaviour kMonoBehaviour => kMonoBehaviour.GetReference(),
            Delegate action => new DelegateRef(action.GetType(), WrapObject(action.Target), action.Method),
            _ => obj
        };
    }

    public static object? UnWrapObject(object? obj) {
        return obj switch {
            GameObjectReference gameObjectReference => gameObjectReference.GetGameObject(),
            ComponentReference reference => reference.GetComponent(),
            DelegateRef delegateRef => Delegate.CreateDelegate(
                delegateRef.DelegateType,
                UnWrapObject(delegateRef.Target),
                delegateRef.MethodInfo
            ),
            _ => obj
        };
    }

    [Serializable]
    public record DelegateRef(
        Type DelegateType,
        object? Target,
        MethodInfo MethodInfo
    );
}
