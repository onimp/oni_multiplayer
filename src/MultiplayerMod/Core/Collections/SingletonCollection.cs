using System.Collections;
using System.Collections.Generic;

namespace MultiplayerMod.Core.Collections;

public class SingletonCollection<T> : IReadOnlyCollection<T> {
    private readonly T value;

    public int Count => 1;

    public SingletonCollection(T value) {
        this.value = value;
    }

    public IEnumerator<T> GetEnumerator() {
        yield return value;
    }

    IEnumerator IEnumerable.GetEnumerator() {
        yield return value;
    }
}
