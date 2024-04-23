using System.Collections.Generic;

namespace MultiplayerMod.Core.Extensions;

public static class SetExtensions {

    public static void AddRange<T>(this ISet<T> set, IEnumerable<T> items) => items.ForEach(it => set.Add(it));

}
