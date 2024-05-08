using System;
using System.Reflection;

namespace MultiplayerMod.Core.Extensions;

public static class TypeExtensions {

    public static MethodInfo[] GetAllMethods(this Type type) => type.GetMethods(
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
    );

}
