using System.Linq;
using System.Runtime.CompilerServices;
using MultiplayerMod.Core.Extensions;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjectsInitializer {

    private enum InternalIdType : long {
        KPrefabId = 1L << 32,
        Chore = 2L << 32
    }

    private readonly MultiplayerObjects objects;

    public MultiplayerObjectsInitializer(MultiplayerObjects objects) {
        this.objects = objects;
    }

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
            instance.Id = objects.Register(gameObject, GetInternalId(InternalIdType.KPrefabId, kPrefabId.InstanceID));
        }
    }

    private void AddChores() {
        Object.FindObjectsOfType<ChoreProvider>()
            .SelectMany(it => it.choreWorldMap)
            .SelectMany(it => it.Value)
            .ForEach(it => { objects.Register(it, GetInternalId(InternalIdType.Chore, it.id)); });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MultiplayerId GetInternalId(InternalIdType type, int id) => new((long) type | (uint) id);

}
