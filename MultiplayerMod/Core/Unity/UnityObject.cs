using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Core.Unity;

public abstract class UnityObject {

    private static IntPtr NonZeroPtr = new(1);

    public static GameObject CreateStaticWithComponent<T>() where T : Component =>
        CreateStaticWithComponents(typeof(T));

    public static GameObject CreateStaticWithComponent<T1, T2>() where T1 : Component where T2 : Component =>
        CreateStaticWithComponents(typeof(T1), typeof(T2));

    private static GameObject CreateStaticWithComponents(params Type[] components) {
        var gameObject = new GameObject(null, components);
        Object.DontDestroyOnLoad(gameObject);
        return gameObject;
    }

    public static void Destroy(GameObject gameObject) {
        Object.Destroy(gameObject);
    }

    public static T CreateStub<T>() where T : MonoBehaviour, new() => new () {
        m_CachedPtr = NonZeroPtr // Support for != and == for Unity objects
    };

}
