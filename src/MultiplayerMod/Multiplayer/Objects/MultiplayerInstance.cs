using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Unity;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerInstance : MultiplayerKMonoBehaviour {

    [InjectDependency, UsedImplicitly]
    private MultiplayerObjects objects = null!;

    private MultiplayerObject? multiplayerObject;
    public MultiplayerId? Id => multiplayerObject?.Id;

    public bool Valid => multiplayerObject != null && objects.Valid(multiplayerObject);

    protected override void OnCleanUp() {
        if (multiplayerObject != null)
            objects.Remove(multiplayerObject.Id);
    }

    public MultiplayerId Register(MultiplayerId? id = null) {
        // The injected `objects` reference is populated in OnPrefabInit, which Unity only fires once the
        // GameObject is activated. A host lifecycle spawn can register the object before that happens -
        // FertilityMonitor.LayEgg KInstantiates the egg inactive and CritterLayEggSynchronizer replicates it
        // in the same-frame postfix - so ensure injection has run first, otherwise objects.Register NREs.
        if (objects == null!)
            EnsureInjected();
        multiplayerObject = objects!.Register(gameObject, id);
        return multiplayerObject.Id;
    }

    public void Redirect(MultiplayerInstance destination) {
        if (multiplayerObject == null)
            return;
        destination.multiplayerObject = multiplayerObject;
        objects.Register(destination.gameObject, multiplayerObject.Id, multiplayerObject.Persistent);
        multiplayerObject = null;
    }

}
