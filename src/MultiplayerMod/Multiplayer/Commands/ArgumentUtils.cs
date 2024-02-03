using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands;

public static class ArgumentUtils {

    public static object?[] WrapObjects(object?[] objects) {
        return objects.Select(WrapObject).ToArray();
    }

    public static object?[] UnWrapObjects(object?[] objects) {
        return objects.Select(UnWrapObject).ToArray();
    }

    public static object? WrapObject(object? obj) {
        return obj switch {
            null => null,
            GameObject gameObject => gameObject.GetReference(),
            KMonoBehaviour kMonoBehaviour => kMonoBehaviour.GetReference(),
            Delegate action => new DelegateRef(action.GetType(), WrapObject(action.Target), action.Method),
            FetchOrder2 order2 => new FetchOrder2Ref(order2),
            _ => obj.GetType().IsSerializable || SerializationSurrogates.HasSurrogate(obj.GetType())
                ? obj
                : throw new Exception($"Type {obj.GetType()} is not serializable")
        };
    }

    public static object? UnWrapObject(object? obj) => obj is Reference reference ? reference.ResolveRaw() : obj;

    [Serializable]
    public record DelegateRef(
        Type DelegateType,
        object? Target,
        MethodInfo MethodInfo
    ) : Reference {
        public object ResolveRaw() {
            return Delegate.CreateDelegate(
                DelegateType,
                UnWrapObject(Target),
                MethodInfo
            );
        }
    }

    [Serializable]
    public record FetchOrder2Ref(
        HashSet<Tag> Tags,
        FetchList2Ref? List2Ref,
        ComponentReference<CreatureDeliveryPoint>? CreatureDeliveryPointReference
    ) : Reference {
        public FetchOrder2Ref(FetchOrder2 fetchOrder2) : this(
            fetchOrder2.Tags,
            fetchOrder2.OnComplete.Target is FetchList2 fetchList2 ? new FetchList2Ref(fetchList2) : null,
            fetchOrder2.OnComplete.Target is CreatureDeliveryPoint creatureDeliveryPoint
                ? creatureDeliveryPoint.GetReference()
                : null
        ) { }

        public object ResolveRaw() {
            var list = List2Ref?.GetFetchList2();
            var creatureDeliveryPoint = CreatureDeliveryPointReference?.GetComponent();

            var fetchOrders = list?.FetchOrders ?? creatureDeliveryPoint?.fetches;
            if (fetchOrders == null) {
                return new FetchOrder2(null, null, FetchChore.MatchCriteria.MatchTags, null, null, null, 0);
            }
            return fetchOrders.Single(order => order.Tags.SequenceEqual(Tags));
        }

        public virtual bool Equals(FetchOrder2Ref? other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Tags.SequenceEqual(other.Tags) && Equals(List2Ref, other.List2Ref) && Equals(
                CreatureDeliveryPointReference,
                other.CreatureDeliveryPointReference
            );
        }
    };

    [Serializable]
    public record FetchList2Ref(
        ComponentReference<Storage> StorageReference,
        ChoreType ComplexFabricatorChoreType,
        StateMachineReference? StateMachineReference,
        ComponentReference<ComplexFabricator>? ComplexFabricatorReference,
        ComponentReference<Constructable>? ConstructableReference,
        FilteredStorageRef? FilteredStorageReference,
        ComponentReference<ManualDeliveryKG>? ManualDeliveryKGReference
    ) {

        public FetchList2Ref(FetchList2 fetchList2) : this(
            fetchList2.Destination.GetReference(),
            fetchList2.choreType,
            fetchList2.OnComplete.Target is StateMachine.Instance smi ? smi.GetReference() : null,
            fetchList2.OnComplete.Target is ComplexFabricator complexFabricator
                ? complexFabricator.GetReference()
                : null,
            fetchList2.OnComplete.Target is Constructable constructable ? constructable.GetReference() : null,
            fetchList2.OnComplete.Target is FilteredStorage storage ? new FilteredStorageRef(storage) : null,
            fetchList2.OnComplete.Target is ManualDeliveryKG manualDeliveryKg ? manualDeliveryKg.GetReference() : null
        ) { }

        public FetchList2? GetFetchList2() {
            var fetchListList = ComplexFabricatorReference?.GetComponent().fetchListList ??
                                StateMachineReference?.Resolve()?.dataTable.OfType<FetchList2>().ToList();
            return fetchListList?.Single(
                       fetchList => Equals(fetchList.Destination.GetReference(), StorageReference) &&
                                    fetchList.choreType == ComplexFabricatorChoreType
                   ) ??
                   ConstructableReference?.GetComponent().fetchList ??
                   FilteredStorageReference?.GetFilteredStorage().fetchList ??
                   ManualDeliveryKGReference?.GetComponent().fetchList;
        }
    }

    [Serializable]
    public record FilteredStorageRef(ComponentReference RootReference) {

        public FilteredStorageRef(FilteredStorage filteredStorage) : this(
            filteredStorage.root.GetReference()
        ) { }

        public FilteredStorage GetFilteredStorage() {
            var root = RootReference.Resolve();
            var type = root!.GetType();
            var field = type.GetField("storageFilter") ??
                        type.GetField("filteredStorage") ??
                        type.GetField("foodStorageFilter");
            return (FilteredStorage) field.GetValue(root);
        }
    }

    [Serializable]
    public record GameStateMachineFetchListRef() { }
}
