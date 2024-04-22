using System.Collections.Generic;

namespace MultiplayerMod.Core.Extensions;

public static class DictionaryExtensions {

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        where TValue : new()
    {
        if (dictionary.TryGetValue(key, out var val))
            return val;

        val = new TValue();
        dictionary.Add(key, val);
        return val;
    }

}
