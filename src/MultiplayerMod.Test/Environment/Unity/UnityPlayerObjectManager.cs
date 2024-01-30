using System;
using MultiplayerMod.Core.Unity;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Test.Environment.Unity;

public static class UnityPlayerObjectManager {

    public static void Allocate(Object obj) {
        obj.m_CachedPtr = UnityPlayerObject.CreateEmpty();
    }

    public static void Release(Object obj) {
        UnityPlayerObject.Free(obj.m_CachedPtr);
        obj.m_CachedPtr = IntPtr.Zero;
    }

}
