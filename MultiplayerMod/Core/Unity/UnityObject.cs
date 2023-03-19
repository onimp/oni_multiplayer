using UnityEngine;

namespace MultiplayerMod.Core.Unity;

public abstract class UnityObject {

    public static GameObject CreateStaticWithComponent<T>() where T : Component {
        var gameObject = new GameObject();
        gameObject.AddComponent<T>();
        Object.DontDestroyOnLoad(gameObject);
        return gameObject;
    }

    public static void Destroy(GameObject gameObject) {
        Object.Destroy(gameObject);
    }

}
