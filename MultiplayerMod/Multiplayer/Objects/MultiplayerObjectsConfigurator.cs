using MultiplayerMod.Game.Objects;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjectsConfigurator {

    public MultiplayerObjectsConfigurator() {
        KInstantiateEvents.Create += gameObject => MultiplayerGame.Objects.Add(gameObject.AddComponent<MultiplayerInstance>());
        KPrefabIdEvents.Deserialize += kPrefabId => {
            var data = kPrefabId.gameObject.AddComponent<MultiplayerInstance>();
            MultiplayerGame.Objects.Add(data);
            data.Id = kPrefabId.InstanceID;
        };
    }

}
