using System.Linq;
using MultiplayerMod.Core.Extensions;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjectsInitializer(MultiplayerObjects objects) {

    public void Initialize() {
        AddPrefabs();
        AddChores();
    }

    private void AddPrefabs() {
        var kPrefabIds = KPrefabIDTracker.Get().prefabIdMap.Values;
        foreach (var kPrefabId in kPrefabIds) {
            if (kPrefabId == null)
                return;

            var gameObject = kPrefabId.gameObject;
            var instance = gameObject.GetComponent<MultiplayerInstance>();
            instance.Id = objects.Register(
                gameObject,
                new MultiplayerId(InternalMultiplayerIdType.KPrefabId, kPrefabId.InstanceID)
            );
        }
    }

    private void AddChores() {
        Object.FindObjectsOfType<ChoreProvider>()
            .SelectMany(it => it.choreWorldMap)
            .SelectMany(it => it.Value)
            .ForEach(it => { objects.Register(it, new MultiplayerId(InternalMultiplayerIdType.Chore, it.id)); });
    }

}
