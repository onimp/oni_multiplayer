using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Loader;
using UnityEngine;

namespace MultiplayerMod.Core.Scheduling;

public class UnityTaskSchedulerLoader : IModComponentLoader {

    private Logging.Logger log = new(typeof(UnityTaskSchedulerLoader));

    public void OnLoad(Harmony harmony) {
        log.Debug("Creating task scheduler...");
        Container.Register<UnityTaskScheduler>();
        CreateTaskExecutor();
    }

    private void CreateTaskExecutor() {
        var gameObject = new GameObject();
        gameObject.AddComponent<UnityTaskExecutor>();
        Object.DontDestroyOnLoad(gameObject);
    }

}
