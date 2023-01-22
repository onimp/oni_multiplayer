using System;
using System.Reflection;

namespace MultiplayerMod
{
    public static class Utils
    {
        public static void InvokePrivate(this object instance, string methodName, params object[] args)
        {
            var method = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(instance, args);
        }

        public static void InvokePrivateStatic(this Type type, string methodName, Type[] types, params object[] args)
        {
            var method = type.GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null,
                CallingConventions.Any,
                types, null);
            method?.Invoke(null, args);
        }

        public static T GetPrivateField<T>(this object instance, string fieldName)
        {
            var type = instance.GetType();
            return (T)type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(instance);
        }
    }
}