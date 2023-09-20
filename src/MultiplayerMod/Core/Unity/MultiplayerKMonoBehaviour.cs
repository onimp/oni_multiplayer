using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Core.Unity;

public class MultiplayerKMonoBehaviour : KMonoBehaviour {

    private readonly IDependencyInjector injector = Dependencies.Get<IDependencyInjector>();

    protected override void OnPrefabInit() => injector.Inject(this);

}
