using System.Collections.Generic;

namespace MultiplayerMod.Core.Extensions;

public static class ListExtensions {

    // ReSharper disable Unity.PerformanceAnalysis
    public static void Deconstruct<T>(this IList<T> list, out T first) {
        first = list[0];
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void Deconstruct<T>(this IList<T> list, out T first, out T second) {
        first = list[0];
        second = list[1];
    }

}
