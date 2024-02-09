namespace MultiplayerMod.Core.Collections;

public class ObjectExtensionMap<TKey, TValue> : BidirectionalMap<TKey, TValue> {
    public ObjectExtensionMap() : base(new IdentityEqualityComparer<TKey>()) { }
}
