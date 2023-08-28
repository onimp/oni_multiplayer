using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Core.Unity;

public class MultiplayerKMonoBehaviour : KMonoBehaviour {

    private readonly DependencyContainer container = Runtime.Instance.Dependencies;

    protected override void OnPrefabInit() => container.Inject(this);

}
