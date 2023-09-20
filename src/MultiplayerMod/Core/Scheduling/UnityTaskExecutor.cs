using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Unity;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace MultiplayerMod.Core.Scheduling;

public class UnityTaskExecutor : MultiplayerMonoBehaviour {

    [InjectDependency]
    private UnityTaskScheduler scheduler = null!;

    private void LateUpdate() => scheduler.Tick();

}
