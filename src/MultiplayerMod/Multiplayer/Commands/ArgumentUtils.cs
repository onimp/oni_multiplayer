using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands;

public static class ArgumentUtils {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(ArgumentUtils));

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

    public static object? UnWrapObject(object? obj) => obj is Reference reference ? reference.Resolve() : obj;

    // Like UnWrapObjects, but tolerates individual references that don't resolve on this client by
    // substituting null instead of throwing. Used only for chores that have a graceful per-argument fallback
    // for a missing reference (e.g. SleepChore's bed -> floor locator): a plain UnWrapObjects lets the first
    // unresolved reference abort the whole chore before that fallback can run.
    public static object?[] UnWrapObjectsTolerant(object?[] objects) {
        return objects.Select(obj => {
            try {
                return UnWrapObject(obj);
            } catch (ObjectNotFoundException) {
                return null;
            }
        }).ToArray();
    }

    [Serializable]
    public record DelegateRef(
        Type DelegateType,
        object? Target,
        MethodInfo MethodInfo
    ) : Reference {
        public object Resolve() {
            // RCE mitigation: MethodInfo + DelegateType arrive from an untrusted peer, so this is a direct
            // "invoke arbitrary method" primitive. Only reconstruct a delegate when both the delegate type
            // and the method's declaring type are trusted (mod / ONI / Unity) — otherwise skip gracefully
            // via ObjectNotFoundException, the designed command-skip path.
            var declaringType = MethodInfo.DeclaringType;
            if (declaringType == null ||
                !NetworkMessageSerializationBinder.IsAllowedType(declaringType) ||
                !NetworkMessageSerializationBinder.IsAllowedType(DelegateType)) {
                log.Warning(
                    "Refusing to reconstruct delegate onto untrusted method " +
                    $"'{declaringType?.FullName ?? "<null>"}.{MethodInfo.Name}' " +
                    $"(delegate type '{DelegateType.FullName}') — possible RCE."
                );
                throw new ObjectNotFoundException(this);
            }
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

        public object Resolve() {
            var list = List2Ref?.GetFetchList2();
            var creatureDeliveryPoint = CreatureDeliveryPointReference?.Resolve();

            var fetchOrders = list?.FetchOrders ?? creatureDeliveryPoint?.fetches;
            if (fetchOrders == null) {
                return new FetchOrder2(null, Tags, FetchChore.MatchCriteria.MatchTags, null, null, null, 1);
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

        public override int GetHashCode() {
            var hashCode = Tags.GetHashCode();
            hashCode = hashCode * 397 ^ (List2Ref != null ? List2Ref.GetHashCode() : 0);
            hashCode = hashCode * 397 ^ (CreatureDeliveryPointReference != null
                ? CreatureDeliveryPointReference.GetHashCode()
                : 0);
            return hashCode;
        }
    }

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
            var fetchListList = ComplexFabricatorReference?.Resolve().fetchListList ??
                                StateMachineReference?.Resolve()?.dataTable.OfType<FetchList2>().ToList();
            return fetchListList?.Single(
                       fetchList => Equals(fetchList.Destination.GetReference(), StorageReference) &&
                                    fetchList.choreType == ComplexFabricatorChoreType
                   ) ??
                   ConstructableReference?.Resolve().fetchList ??
                   FilteredStorageReference?.GetFilteredStorage().fetchList ??
                   ManualDeliveryKGReference?.Resolve().fetchList;
        }
    }

    [Serializable]
    public record FilteredStorageRef(ComponentReference RootReference) {

        private static BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        public FilteredStorageRef(FilteredStorage filteredStorage) : this(
            filteredStorage.root.GetReference()
        ) { }

        public FilteredStorage GetFilteredStorage() {
            var root = RootReference.Resolve();
            var type = root!.GetType();

            var field = type.GetField("storageFilter", bindingFlags) ??
                        type.GetField("filteredStorage", bindingFlags) ??
                        type.GetField("foodStorageFilter", bindingFlags);
            return (FilteredStorage) field!.GetValue(root);
        }
    }

    [Serializable]
    public record GameStateMachineFetchListRef;

}
