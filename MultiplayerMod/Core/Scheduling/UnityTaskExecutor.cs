using MultiplayerMod.Core.Dependency;
using UnityEngine;

namespace MultiplayerMod.Core.Scheduling;

public class UnityTaskExecutor : MonoBehaviour {

    private void LateUpdate() => Container.Get<UnityTaskScheduler>().Tick();

}
