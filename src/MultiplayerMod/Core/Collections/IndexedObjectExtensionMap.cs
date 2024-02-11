namespace MultiplayerMod.Core.Collections;

public class IndexedObjectExtensionMap<TKey, TValue> : BidirectionalMap<TKey, TValue> {
    public IndexedObjectExtensionMap() : base(new IdentityEqualityComparer<TKey>()) { }
}
