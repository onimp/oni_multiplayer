using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerMod.Core.Unity;

public static class GameObjectExtension {
    public static void DestroyOnLoad(this GameObject go) {
        SceneManager.MoveGameObjectToScene(go, SceneManager.GetActiveScene());
    }
}
