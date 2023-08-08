using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game;
using MultiplayerMod.Game.Objects;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjectsConfigurator {
    
    public MultiplayerObjectsConfigurator() {
        KInstantiateEvents.Create += gameObject => gameObject.AddComponent<MultiplayerInstance>();
        GameEvents.GameStarted += () => {
            Container.Get<MultiplayerIdentityProvider>().NextObjectId = KPrefabID.NextUniqueID;
            var kPrefabIds = KPrefabIDTracker.Get().prefabIdMap.Values;
            foreach (var kPrefabId in kPrefabIds) {
                var gameObject = kPrefabId.gameObject;
                var instance = SaveLoader.Instance.loadedFromSave
                    ? gameObject.AddComponent<MultiplayerInstance>()
                    : gameObject.GetComponent<MultiplayerInstance>();
                instance.Id = new MultiplayerId(null, kPrefabId.InstanceID);
                MultiplayerGame.Objects.Register(instance);
            }
        };
    }

}
