using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Usage: PatchInternalCalls <coremodule.dll> <output.dll> <dedicatedserver.dll>
// Patches InternalCall methods in CoreModule to call managed implementations from DedicatedServer.

if (args.Length < 3) throw new Exception("Usage: PatchInternalCalls <coremodule.dll> <output.dll> <dedicatedserver.dll>");
var inputPath = args[0];
var outputPath = args[1];
var runtimeDllPath = args[2];

Console.WriteLine($"Patching: {inputPath}");
Console.WriteLine($"Runtime:  {runtimeDllPath}");

var resolver = new DefaultAssemblyResolver();
resolver.AddSearchDirectory(Path.GetDirectoryName(inputPath)!);
resolver.AddSearchDirectory(Path.GetDirectoryName(runtimeDllPath)!);

var module = ModuleDefinition.ReadModule(inputPath, new ReaderParameters { AssemblyResolver = resolver });

// Load DedicatedServer assembly to find UnityRuntime methods
var runtimeAsm = AssemblyDefinition.ReadAssembly(runtimeDllPath, new ReaderParameters { AssemblyResolver = resolver });
var runtimeType = runtimeAsm.MainModule.Types.First(t => t.FullName == "DedicatedServer.Game.UnityRuntime");

// Build method map: "MethodName" -> MethodReference imported into CoreModule
var runtimeMethods = new Dictionary<string, MethodReference>();
foreach (var m in runtimeType.Methods.Where(m => m.IsPublic && m.IsStatic)) {
    runtimeMethods[m.Name] = module.ImportReference(m);
    Console.WriteLine($"  Found runtime method: {m.Name}({string.Join(", ", m.Parameters.Select(p => p.ParameterType.Name))})");
}

// Map: "UnityType::InternalCallName" -> "RuntimeMethodName"
var callMap = new Dictionary<string, string> {
    ["UnityEngine.GameObject::Internal_CreateGameObject"] = "CreateGameObject",
    ["UnityEngine.GameObject::Internal_AddComponentWithType"] = "AddComponent",
    ["UnityEngine.GameObject::Find"] = "Find",
    ["UnityEngine.Component::get_gameObject"] = "GetGameObject",
    ["UnityEngine.Component::get_transform"] = "GetTransform",
    ["UnityEngine.ScriptableObject::CreateScriptableObject"] = "CreateScriptableObject",
    ["UnityEngine.ScriptableObject::CreateScriptableObjectInstanceFromType"] = "CreateScriptableObjectInstanceFromType",
    ["UnityEngine.Object::GetName"] = "GetName",
    ["UnityEngine.Object::SetName"] = "SetName",
    ["UnityEngine.Object::GetInstanceID"] = "GetInstanceID",
    ["UnityEngine.Application::get_streamingAssetsPath"] = "GetStreamingAssetsPath",
    ["UnityEngine.Application::get_dataPath"] = "GetDataPath",
    ["UnityEngine.Application::get_persistentDataPath"] = "GetPersistentDataPath",
    ["UnityEngine.Application::get_consoleLogPath"] = "GetConsoleLogPath",
    ["UnityEngine.Application::get_isPlaying"] = "GetIsPlaying",
    ["UnityEngine.Application::get_platform"] = "GetPlatform",
    ["UnityEngine.SystemInfo::GetProcessorCount"] = "GetProcessorCount",
    ["UnityEngine.SystemInfo::GetSystemMemorySize"] = "GetSystemMemorySize",
    ["UnityEngine.ResourcesAPIInternal::FindObjectsOfTypeAll"] = "FindObjectsOfTypeAll",
    ["UnityEngine.ResourcesAPIInternal::Load"] = "LoadResource",
    ["UnityEngine.MonoBehaviour::IsObjectMonoBehaviour"] = "IsObjectMonoBehaviour",
    ["UnityEngine.MonoBehaviour::StartCoroutineManaged2"] = "StartCoroutineManaged2",
    ["UnityEngine.TextAsset::Internal_CreateInstance"] = "CreateTextAsset",
    ["UnityEngine.TextAsset::get_text"] = "GetTextAssetText",
    ["UnityEngine.TextAsset::GetText"] = "GetTextAssetText",
    ["UnityEngine.TextAsset::get_bytes"] = "GetTextAssetBytes",
    ["UnityEngine.TextAsset::GetBytes"] = "GetTextAssetBytes",
};

// Patch all InternalCall methods
int patched = 0, delegated = 0;
foreach (var type in module.Types.SelectMany<TypeDefinition, TypeDefinition>(FlattenTypes)) {
    foreach (var method in type.Methods) {
        if (!method.IsInternalCall) continue;

        method.ImplAttributes &= ~MethodImplAttributes.InternalCall;
        method.ImplAttributes |= MethodImplAttributes.IL;
        method.Body = new MethodBody(method);
        var il = method.Body.GetILProcessor();

        var key = $"{type.FullName}::{method.Name}";
        if (callMap.TryGetValue(key, out var runtimeMethodName) && runtimeMethods.TryGetValue(runtimeMethodName, out var target)) {
            // Delegate to UnityRuntime static method — push all args then call
            foreach (var param in method.Parameters)
                il.Append(il.Create(OpCodes.Ldarg, param));
            if (method.HasThis)
                il.Body.Instructions.Insert(0, il.Create(OpCodes.Ldarg_0));
            il.Append(il.Create(OpCodes.Call, target));
            il.Append(il.Create(OpCodes.Ret));
            delegated++;
            Console.WriteLine($"  [DELEGATE] {key} -> UnityRuntime.{runtimeMethodName}");
        } else {
            // Default stub: return default
            if (method.ReturnType.FullName == "System.Void") {
                il.Append(il.Create(OpCodes.Ret));
            } else if (method.ReturnType.IsValueType) {
                var local = new VariableDefinition(method.ReturnType);
                method.Body.Variables.Add(local);
                il.Append(il.Create(OpCodes.Ldloca_S, local));
                il.Append(il.Create(OpCodes.Initobj, method.ReturnType));
                il.Append(il.Create(OpCodes.Ldloc_0));
                il.Append(il.Create(OpCodes.Ret));
            } else {
                il.Append(il.Create(OpCodes.Ldnull));
                il.Append(il.Create(OpCodes.Ret));
            }
        }
        patched++;
    }
}

// --- Rewrite managed methods that use unsafe native pointers ---
var objectType = module.Types.First(t => t.FullName == "UnityEngine.Object");
var getInstanceID = objectType.Methods.FirstOrDefault(m => m.Name == "GetInstanceID" && !m.IsStatic && m.Parameters.Count == 0);
if (getInstanceID != null) {
    var m_CachedPtr = objectType.Fields.First(f => f.Name == "m_CachedPtr");
    getInstanceID.Body = new MethodBody(getInstanceID);
    var il2 = getInstanceID.Body.GetILProcessor();
    // return (int)m_CachedPtr
    il2.Append(il2.Create(OpCodes.Ldarg_0));
    il2.Append(il2.Create(OpCodes.Ldfld, m_CachedPtr));
    il2.Append(il2.Create(OpCodes.Conv_I4));
    il2.Append(il2.Create(OpCodes.Ret));
    Console.WriteLine("  [REWRITE] UnityEngine.Object::GetInstanceID -> (int)m_CachedPtr");
}

module.Write(outputPath);
Console.WriteLine($"Patched {patched} methods ({delegated} delegated to UnityRuntime) -> {outputPath}");

IEnumerable<TypeDefinition> FlattenTypes(TypeDefinition type) {
    yield return type;
    foreach (var nested in type.NestedTypes.SelectMany<TypeDefinition, TypeDefinition>(FlattenTypes))
        yield return nested;
}
