using System.Collections;
using System.Collections.Generic;

namespace MultiplayerMod.Core.Collections;

public class BidirectionalMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> {

    private readonly Dictionary<TKey, TValue> keyMap;
    private readonly Dictionary<TValue, TKey> valueMap;

    public BidirectionalMap() {
        keyMap = new Dictionary<TKey, TValue>();
        valueMap = new Dictionary<TValue, TKey>();
    }

    public BidirectionalMap(IEqualityComparer<TKey> keyComparer) : this(
        keyComparer,
        EqualityComparer<TValue>.Default
    ) { }

    public BidirectionalMap(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer) {
        keyMap = new Dictionary<TKey, TValue>(keyComparer);
        valueMap = new Dictionary<TValue, TKey>(valueComparer);
    }

    public TValue this[TKey key] {
        get => keyMap[key];
        set {
            keyMap[key] = value;
            valueMap[value] = key;
        }
    }

    public TKey this[TValue key] {
        get => valueMap[key];
        set {
            keyMap[value] = key;
            valueMap[key] = value;
        }
    }

    public bool TryGetKey(TValue value, out TKey? key) => valueMap.TryGetValue(value, out key);

    public bool TryGetValue(TKey key, out TValue? value) => keyMap.TryGetValue(key, out value);

    public bool Remove(TKey key) {
        if (!keyMap.TryGetValue(key, out var value))
            return false;

        keyMap.Remove(key);
        valueMap.Remove(value);
        return true;
    }

    public bool Remove(TValue value) {
        if (!valueMap.TryGetValue(value, out var key))
            return false;

        keyMap.Remove(key);
        valueMap.Remove(value);
        return true;
    }

    public void Clear() {
        keyMap.Clear();
        valueMap.Clear();
    }

    public IEnumerable<KeyValuePair<TKey, TValue>> GetEnumerableByKey() => keyMap;

    public IEnumerable<KeyValuePair<TValue, TKey>> GetEnumerableByValue() => valueMap;

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => keyMap.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}
