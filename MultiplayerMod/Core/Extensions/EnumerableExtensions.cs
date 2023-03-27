using System;
using System.Collections.Generic;

namespace MultiplayerMod.Core.Extensions;

public static class EnumerableExtensions {

    // ReSharper disable Unity.PerformanceAnalysis
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
        foreach (var item in enumerable)
            action(item);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void ForEachIndexed<T>(this IEnumerable<T> enumerable, Action<int, T> action) {
        var index = 0;
        foreach (var item in enumerable)
            action(index++, item);
    }

}
