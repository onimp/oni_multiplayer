using System;
using System.Collections.Generic;

namespace MultiplayerMod.Core.Extensions;

public static class LinkedListExtensions {

    // ReSharper disable Unity.PerformanceAnalysis
    public static void ForEach<T>(this LinkedList<T> list, Action<T, LinkedListNode<T>> action) {
        var node = list.First;
        while (node != null) {
            var next = node.Next;
            action(node.Value, node);
            node = next;
        }
    }

}
