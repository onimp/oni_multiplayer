using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Core.Unity;

public abstract class UnityObject {

    private static IntPtr NonZeroPtr = Marshal.AllocHGlobal(1);

    public static GameObject CreateStaticWithComponent<T>() where T : Component =>
        CreateWithComponents(false, typeof(T));

    public static GameObject CreateWithComponent<T1, T2>() where T1 : Component where T2 : Component =>
        CreateWithComponents(true, typeof(T1), typeof(T2));

    private static GameObject CreateWithComponents(bool destroyOnLoad, params Type[] components) {
        var gameObject = new GameObject(null, components);
        if (!destroyOnLoad)
            Object.DontDestroyOnLoad(gameObject);
        return gameObject;
    }

    public static void Destroy(GameObject gameObject) {
        Object.Destroy(gameObject);
    }

    public static T CreateStub<T>() where T : MonoBehaviour, new() => new() {
        m_CachedPtr = NonZeroPtr // Support for != and == for Unity objects
    };

}
