using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;
using UnityEngine;

namespace MultiplayerMod.Core.Unity;

public class MultiplayerMonoBehaviour : MonoBehaviour {

    private readonly DependencyContainer container = Runtime.Instance.Dependencies;

    protected virtual void Awake() => container.Inject(this);

}
