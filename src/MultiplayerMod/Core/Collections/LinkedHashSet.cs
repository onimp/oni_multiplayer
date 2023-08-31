using System.Collections;
using System.Collections.Generic;

namespace MultiplayerMod.Core.Collections;

public class LinkedHashSet<T> : ICollection<T>, IReadOnlyCollection<T> {

    public int Count => list.Count;

    public bool IsReadOnly => false;

    private readonly Dictionary<T, LinkedListNode<T>> dictionary = new();
    private readonly LinkedList<T> list = new();

    public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item) {
        if (dictionary.ContainsKey(item))
            return;

        dictionary[item] = list.AddLast(item);
    }

    public void Clear() {
        list.Clear();
        dictionary.Clear();
    }

    public bool Contains(T item) => dictionary.ContainsKey(item);

    public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

    public bool Remove(T item) {
        if (!dictionary.TryGetValue(item, out var node))
            return false;

        list.Remove(node);
        return true;
    }

}
