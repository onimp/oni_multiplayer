using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MultiplayerMod.Core.Extensions;

public static class TypeExtensions {

    public static MethodInfo[] GetAllMethods(this Type type) => type.GetMethods(
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
    );

    public static IEnumerable<Type> GetInheritedTypes(this Type type) {
        var current = type;
        while (current != typeof(object) && current != null) {
            yield return current;

            current = current.BaseType;
        }
    }

    public static bool Inherits(this Type self, Type type) => self.GetInheritedTypes().Any(it => it == type);

}
