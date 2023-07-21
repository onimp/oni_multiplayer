using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Game.Objects;
using MultiplayerMod.Game.World;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjectsConfigurator {

    public MultiplayerObjectsConfigurator() {
        KInstantiateEvents.Create += gameObject => gameObject.AddComponent<MultiplayerInstance>();

        KPrefabIdEvents.Deserialize += kPrefabId => {
            var data = kPrefabId.gameObject.AddComponent<MultiplayerInstance>();
            data.Id = new MultiplayerId(null, kPrefabId.InstanceID);
            MultiplayerGame.Objects.Register(data);
        };

        var identityProvider = Container.Register(new MultiplayerIdentityProvider());
        WorldGenSpawnerEvents.Spawned += () => { identityProvider.NextObjectId = KPrefabID.NextUniqueID; };
    }

}
