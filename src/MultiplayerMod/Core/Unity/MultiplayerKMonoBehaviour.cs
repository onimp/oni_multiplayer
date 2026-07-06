using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Core.Unity;

public class MultiplayerKMonoBehaviour : KMonoBehaviour {

    private readonly IDependencyInjector injector = Dependencies.Get<IDependencyInjector>();

    protected override void OnPrefabInit() => injector.Inject(this);

    // Re-run field injection on demand. Normally injection happens once in OnPrefabInit, but a host-spawned
    // object can be acted on before Unity activates it and fires OnPrefabInit (e.g. FertilityMonitor.LayEgg
    // instantiates the egg inactive, then a postfix registers it the same frame). Idempotent.
    protected void EnsureInjected() => injector.Inject(this);

}
