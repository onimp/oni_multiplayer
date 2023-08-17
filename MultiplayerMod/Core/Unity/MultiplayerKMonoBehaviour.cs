using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.Core.Unity;

public class MultiplayerKMonoBehaviour : KMonoBehaviour {

    protected override void OnPrefabInit() => Dependencies.Inject(this);

}
