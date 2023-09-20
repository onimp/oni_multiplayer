using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;
using UnityEngine;

namespace MultiplayerMod.Core.Unity;

public class MultiplayerMonoBehaviour : MonoBehaviour {

    private readonly IDependencyInjector injector = Runtime.Instance.Dependencies.Get<IDependencyInjector>();

    protected virtual void Awake() => injector.Inject(this);

}
