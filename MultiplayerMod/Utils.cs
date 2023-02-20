using System;
using System.Reflection;

namespace MultiplayerMod
{

    public static class Utils
    {
        public static object InvokePrivate(this object instance, string methodName, params object[] args)
        {
            var method = instance.GetType().GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            return method!.Invoke(instance, args);
        }

        public static object InvokePrivate(
            this object instance,
            Type instanceType,
            string methodName,
            params object[] args
        )
        {
            var method = instanceType.GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            return method!.Invoke(instance, args);
        }

        public static object InvokePrivate<T>(
            this T instance,
            string methodName,
            Type[] genericTypes,
            params object[] args
        )
        {
            var method = typeof(T).GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            method = method!.MakeGenericMethod(genericTypes);
            return method.Invoke(instance, args);
        }

        public static object Invoke(
            this object instance,
            string methodName,
            Type[] types,
            params object[] args
        )
        {
            var method = instance.GetType().GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.Instance,
                null,
                CallingConventions.Any,
                types,
                null
            );
            return method!.Invoke(instance, args);
        }

        public static object Invoke(this object instance, string methodName, params object[] args)
        {
            var method = instance.GetType().GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.Instance
            );
            return method!.Invoke(instance, args);
        }

        public static void InvokePrivateStatic(this Type type, string methodName, Type[] types, params object[] args)
        {
            var method = type.GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                null,
                CallingConventions.Any,
                types,
                null
            );
            method?.Invoke(null, args);
        }

        public static T GetPrivateField<T>(this object instance, string fieldName)
        {
            var type = instance.GetType();
            return (T)type.GetField(
                fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance
            )!.GetValue(instance);
        }

        public static T2 GetPrivateField<T, T2>(this T instance, string fieldName)
        {
            return (T2)typeof(T).GetField(
                fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance
            )!.GetValue(instance);
        }

        public static T GetField<T>(this object instance, string fieldName)
        {
            var type = instance.GetType();
            var fieldInfo = type.GetField(
                fieldName,
                BindingFlags.Public | BindingFlags.Instance
            );
            return (T)fieldInfo!.GetValue(instance);
        }

        public static void SetField(this object instance, string fieldName, object value)
        {
            var type = instance.GetType();
            type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance)!
                .SetValue(instance, value);
        }

        public static T GetProperty<T>(this object instance, string fieldName)
        {
            var type = instance.GetType();
            return (T)type.GetProperty(
                fieldName,
                BindingFlags.Public | BindingFlags.Instance
            )!.GetValue(instance);
        }

        public static T GetPrivateProperty<T>(this object instance, string fieldName)
        {
            var type = instance.GetType();
            return (T)type.GetProperty(
                fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance
            )!.GetValue(instance);
        }

        public static T2 GetPrivateProperty<T, T2>(this T instance, string fieldName)
        {
            return (T2)typeof(T).GetProperty(
                fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance
            )!.GetValue(instance);
        }

        public static Type GetNestedType(this Type parentType, string subTypeName)
        {
            return parentType.GetNestedType(
                subTypeName,
                BindingFlags.NonPublic | BindingFlags.Instance
            );
        }

        public static T CreateNestedTypeInstance<T>(this Type parentType, string subTypeName, params object[] args)
        {
            return (T)Activator.CreateInstance(GetNestedType(parentType, subTypeName), args);
        }
    }

}
