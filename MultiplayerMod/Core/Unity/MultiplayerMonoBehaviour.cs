using MultiplayerMod.Core.Dependency;
using UnityEngine;

namespace MultiplayerMod.Core.Unity;

public class MultiplayerMonoBehaviour : MonoBehaviour {

    protected virtual void Awake() => Dependencies.Inject(this);

}
