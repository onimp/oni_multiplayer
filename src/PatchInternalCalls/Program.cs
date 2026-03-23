using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Usage: PatchInternalCalls <coremodule.dll> <output.dll> <dedicatedserver.dll> [<assembly-csharp.dll>]
// Patches InternalCall methods in CoreModule to call managed implementations from DedicatedServer.
// Optional 4th arg: patches Assembly-CSharp.dll (Assets.GetAnim fallback to stub) in-place.

if (args.Length < 3) throw new Exception("Usage: PatchInternalCalls <coremodule.dll> <output.dll> <dedicatedserver.dll> [<assembly-csharp.dll>]");
var inputPath = args[0];
var outputPath = args[1];
var runtimeDllPath = args[2];

Console.WriteLine($"Patching: {inputPath}");
Console.WriteLine($"Runtime:  {runtimeDllPath}");

var resolver = new DefaultAssemblyResolver();
resolver.AddSearchDirectory(Path.GetDirectoryName(inputPath)!);
resolver.AddSearchDirectory(Path.GetDirectoryName(runtimeDllPath)!);

// ReadSymbols = false: prevents Cecil from probing for PDB/MDB files, which can cause
// BadImageFormatException when the host process arch (ARM64/x64) differs from the
// assembly's PE format (PE32 x86 from net48 Prefer32Bit=true default).
var noSymbols = new ReaderParameters { AssemblyResolver = resolver, ReadSymbols = false };
var module = ModuleDefinition.ReadModule(inputPath, noSymbols);

// Load DedicatedServer assembly to find UnityRuntime methods
var runtimeAsm = AssemblyDefinition.ReadAssembly(runtimeDllPath, noSymbols);
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
    ["UnityEngine.Component::get_transform"] = "GetTransformFromComponent",
    ["UnityEngine.GameObject::get_transform"] = "GetTransformFromGameObject",
    ["UnityEngine.GameObject::get_activeInHierarchy"] = "GetActive",
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
    // Object.Instantiate (clone)
    ["UnityEngine.Object::Internal_CloneSingle"] = "CloneSingle",
    ["UnityEngine.Object::Internal_CloneSingleWithParent"] = "CloneSingleWithParent",
    ["UnityEngine.Object::Internal_CloneSingleWithParams"] = "CloneSingleWithParams",
    // Note: Internal_InstantiateSingle / Internal_InstantiateSingleWithParent are managed wrappers,
    // not InternalCalls — they are rewritten separately below (after the InternalCall loop).
    // Object.Destroy
    ["UnityEngine.Object::Destroy"] = "DestroyObject",
    ["UnityEngine.Object::DestroyImmediate"] = "DestroyImmediate",
    // GameObject.SetActive
    ["UnityEngine.GameObject::SetActive"] = "SetActive",
    ["UnityEngine.GameObject::get_activeSelf"] = "GetActive",
    // Behaviour.enabled
    ["UnityEngine.Behaviour::get_enabled"] = "GetEnabled",
    ["UnityEngine.Behaviour::set_enabled"] = "SetEnabled",
    // DontDestroyOnLoad
    ["UnityEngine.Object::DontDestroyOnLoad"] = "DontDestroyOnLoad",
    // GetComponent family — InternalCalls stubbed to null by default, must delegate
    // GetComponentFastPath is called by the managed GetComponent<T>() generic method
    ["UnityEngine.GameObject::GetComponentFastPath"] = "GetComponentFastPath",
    ["UnityEngine.Component::GetComponentFastPath"] = "GetComponentFastPathFromComponent",
    // TryGetComponent<T>(out T) compiles to TryGetComponentFastPath — same out-ptr protocol.
    // Missing entry caused MissingMethodException in BaseMinionConfig.BaseOnSpawn line 384.
    ["UnityEngine.GameObject::TryGetComponentFastPath"] = "TryGetComponentFastPath",
    ["UnityEngine.GameObject::GetComponentInChildren"] = "GetComponentInChildren",
    // Transform position — MUST be patched via Cecil (not Harmony) because Harmony on these
    // _Injected methods causes a deadlock on Mono (same as KMonoBehaviour subclasses).
    // get_position_Injected(out Vector3 ret) → GetPosition(Transform self, out Vector3 result)
    // set_position_Injected(ref Vector3 value) → SetPositionFromTransform(Transform self, ref Vector3 position)
    ["UnityEngine.Transform::get_position_Injected"] = "GetPosition",
    ["UnityEngine.Transform::set_position_Injected"] = "SetPositionFromTransform",
    ["UnityEngine.Transform::get_localPosition_Injected"] = "GetPosition",
    ["UnityEngine.Transform::set_localPosition_Injected"] = "SetPositionFromTransform",
    // Time — static 0f in headless without this; breaks all timers (sleep, ThoughtGraph, etc.)
    // timeScale always returns 1f; frameCount increments every Update() for PathGrid caching.
    ["UnityEngine.Time::get_time"]               = "GetTime",
    ["UnityEngine.Time::get_deltaTime"]          = "GetDeltaTime",
    ["UnityEngine.Time::get_fixedDeltaTime"]     = "GetFixedDeltaTime",
    ["UnityEngine.Time::get_unscaledTime"]       = "GetUnscaledTime",
    ["UnityEngine.Time::get_unscaledDeltaTime"]  = "GetUnscaledDeltaTime",
    ["UnityEngine.Time::get_timeScale"]          = "GetTimeScale",
    ["UnityEngine.Time::get_frameCount"]         = "GetFrameCount",
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

// --- Rewrite managed Instantiate wrappers to call CloneSingleWithParams ---
// Internal_InstantiateSingle and Internal_InstantiateSingleWithParent are managed wrappers
// (not InternalCalls themselves) that delegate to _Injected variants via ref-typed params.
// We replace them with direct calls to UnityRuntime to bypass the _Injected InternalCalls.
if (runtimeMethods.TryGetValue("CloneSingleWithParams", out var cloneWithParams)) {
    var instantiateSingle = objectType.Methods.FirstOrDefault(m =>
        m.Name == "Internal_InstantiateSingle" && m.Parameters.Count == 3 && !m.IsInternalCall);
    if (instantiateSingle != null) {
        instantiateSingle.Body = new MethodBody(instantiateSingle);
        var ilInst = instantiateSingle.Body.GetILProcessor();
        ilInst.Append(ilInst.Create(OpCodes.Ldarg_0));
        ilInst.Append(ilInst.Create(OpCodes.Ldarg_1));
        ilInst.Append(ilInst.Create(OpCodes.Ldarg_2));
        ilInst.Append(ilInst.Create(OpCodes.Call, cloneWithParams));
        ilInst.Append(ilInst.Create(OpCodes.Ret));
        Console.WriteLine("  [REWRITE] UnityEngine.Object::Internal_InstantiateSingle -> UnityRuntime.CloneSingleWithParams");
    }
}
if (runtimeMethods.TryGetValue("InstantiateSingleWithParent", out var instantiateWithParent)) {
    var instantiateSingleWithParent = objectType.Methods.FirstOrDefault(m =>
        m.Name == "Internal_InstantiateSingleWithParent" && m.Parameters.Count == 4 && !m.IsInternalCall);
    if (instantiateSingleWithParent != null) {
        instantiateSingleWithParent.Body = new MethodBody(instantiateSingleWithParent);
        var ilInst = instantiateSingleWithParent.Body.GetILProcessor();
        ilInst.Append(ilInst.Create(OpCodes.Ldarg_0));
        ilInst.Append(ilInst.Create(OpCodes.Ldarg_1));
        ilInst.Append(ilInst.Create(OpCodes.Ldarg_2));
        ilInst.Append(ilInst.Create(OpCodes.Ldarg_3));
        ilInst.Append(ilInst.Create(OpCodes.Call, instantiateWithParent));
        ilInst.Append(ilInst.Create(OpCodes.Ret));
        Console.WriteLine("  [REWRITE] UnityEngine.Object::Internal_InstantiateSingleWithParent -> UnityRuntime.InstantiateSingleWithParent");
    }
}

// Write to a temp file first to avoid Cecil lazy-load conflict.
// Cecil uses deferred token reads; if inputPath == outputPath, truncating the file
// for writing corrupts in-flight lazy reads -> BadImageFormatException at MetadataBuilder.
// Writing to a separate path avoids the conflict; File.Move is atomic on the same FS.
var tempOutputPath = outputPath + ".tmp";
module.Write(tempOutputPath);
File.Move(tempOutputPath, outputPath, overwrite: true);
Console.WriteLine($"Patched {patched} methods ({delegated} delegated to UnityRuntime) -> {outputPath}");

// Optional: patch Assembly-CSharp.dll (Assets.GetAnim null fallback to stub, render no-ops)
if (args.Length >= 4) {
    var asmCSharpPath = args[3];
    PatchAssemblyCSharp(asmCSharpPath, runtimeDllPath);
}
// Optional: patch Assembly-CSharp-firstpass.dll (more render no-ops in base classes)
if (args.Length >= 5) {
    var asmFirstpassPath = args[4];
    PatchAssemblyCSharpFirstpass(asmFirstpassPath);
}

void PatchAssemblyCSharp(string asmPath, string runtimePath) {
    Console.WriteLine($"\nPatching Assembly-CSharp: {asmPath}");
    var asmResolver = new DefaultAssemblyResolver();
    asmResolver.AddSearchDirectory(Path.GetDirectoryName(asmPath)!);
    asmResolver.AddSearchDirectory(Path.GetDirectoryName(runtimePath)!);

    // Read into memory first so we can write to the same path without file-lock conflicts
    var asmBytes = File.ReadAllBytes(asmPath);
    var asmModule = ModuleDefinition.ReadModule(new MemoryStream(asmBytes), new ReaderParameters { AssemblyResolver = asmResolver, ReadSymbols = false });

    // Patch Assets::GetAnim to return stub when anim not found.
    // IMPORTANT: do NOT add DedicatedServer as assembly reference — Assembly-CSharp already
    // depends on DedicatedServer (it's the main app), adding the reverse creates a circular
    // dependency that breaks Mono's assembly loader and manifests as spurious InternalCall failures.
    // Solution: create the stub inline using types Assembly-CSharp already references.
    var assetsType = asmModule.Types.FirstOrDefault(t => t.FullName == "Assets");
    if (assetsType == null) { Console.WriteLine("  [WARN] Assets type not found in Assembly-CSharp"); return; }

    var getAnimMethod = assetsType.Methods.FirstOrDefault(m => m.Name == "GetAnim" && m.Parameters.Count == 1);
    if (getAnimMethod == null) { Console.WriteLine("  [WARN] Assets::GetAnim not found"); return; }

    // KAnimFile type (return type of GetAnim, already in Assembly-CSharp's scope)
    var kanimFileTypeRef = getAnimMethod.ReturnType;

    // Add private static field: KAnimFile __headlessStub
    var stubField = new FieldDefinition("__headlessStub",
        FieldAttributes.Private | FieldAttributes.Static, kanimFileTypeRef);
    assetsType.Fields.Add(stubField);
    var stubFieldRef = (FieldReference)asmModule.ImportReference(stubField);

    // Build method ref for: KAnimFile::.ctor() — KAnimFile is in Assembly-CSharp-firstpass
    // which Assembly-CSharp already references. No new assembly refs, no value types (RuntimeTypeHandle).
    var asmFirstpassRef = asmModule.AssemblyReferences.FirstOrDefault(r => r.Name == "Assembly-CSharp-firstpass");
    if (asmFirstpassRef == null) { Console.WriteLine("  [WARN] Assembly-CSharp-firstpass reference not found"); return; }

    var kanimTypeRef = new TypeReference("", "KAnimFile", asmModule, asmFirstpassRef);
    var kanimCtorRef = new MethodReference(".ctor", asmModule.TypeSystem.Void, kanimTypeRef) { HasThis = true };

    // Rewrite Assets::GetAnim:
    //   if (__headlessStub == null)
    //       __headlessStub = new KAnimFile();   // calls patched ScriptableObject .ctor InternalCall
    //   return __headlessStub;
    // Uses only existing assembly references — no circular DedicatedServer dependency, no value types.
    getAnimMethod.Body = new MethodBody(getAnimMethod);
    var il = getAnimMethod.Body.GetILProcessor();

    // if (__headlessStub != null) goto RETURN
    il.Append(il.Create(OpCodes.Ldsfld, stubFieldRef));
    var retInstr = il.Create(OpCodes.Ldsfld, stubFieldRef);
    il.Append(il.Create(OpCodes.Brtrue_S, retInstr));

    // __headlessStub = new KAnimFile()
    il.Append(il.Create(OpCodes.Newobj, kanimCtorRef));
    il.Append(il.Create(OpCodes.Stsfld, stubFieldRef));

    // RETURN:
    il.Append(retInstr);
    il.Append(il.Create(OpCodes.Ret));

    // Patch rendering/animation methods to no-op (headless: no rendering needed).
    // These methods access KAnimFile.GetData() which returns null on stub anims, causing NPE
    // during entity prefab creation. Making them no-ops is correct for headless mode.
    // KBatchedAnimController.LoadAnims — accesses GetData().name on stub → NPE
    NoopMethod(asmModule, "KBatchedAnimController", "LoadAnims", 0,
        "KBatchedAnimController::LoadAnims -> no rendering");
    // KAnimControllerBase.AddAnimOverrides / RemoveAnimOverrides — call GetData().build.symbols → NPE
    NoopMethod(asmModule, "KAnimControllerBase", "AddAnimOverrides", 2,
        "KAnimControllerBase::AddAnimOverrides -> no rendering");
    NoopMethod(asmModule, "KAnimControllerBase", "RemoveAnimOverrides", 1,
        "KAnimControllerBase::RemoveAnimOverrides -> no rendering");
    // SymbolOverrideControllerUtil.ApplySymbolOverridesByAffix — calls GetData().build.symbols → NPE
    NoopMethod(asmModule, "SymbolOverrideControllerUtil", "ApplySymbolOverridesByAffix", 5,
        "SymbolOverrideControllerUtil::ApplySymbolOverridesByAffix -> no rendering");

    asmModule.Write(asmPath);
    Console.WriteLine($"  Assembly-CSharp patched -> {asmPath}");
}

void PatchAssemblyCSharpFirstpass(string asmPath) {
    // Currently no patches needed for Assembly-CSharp-firstpass — kept for future use.
    Console.WriteLine($"\nPatching Assembly-CSharp-firstpass: {asmPath} (no patches needed)");
}

void NoopMethod(ModuleDefinition mod, string typeName, string methodName, int paramCount, string label) {
    var type = mod.Types.FirstOrDefault(t => t.Name == typeName);
    if (type == null) { Console.WriteLine($"  [WARN] {typeName} not found"); return; }
    var method = type.Methods.FirstOrDefault(m => m.Name == methodName && m.Parameters.Count == paramCount);
    if (method == null) { Console.WriteLine($"  [WARN] {typeName}::{methodName}({paramCount} params) not found"); return; }
    method.Body = new MethodBody(method);
    var il = method.Body.GetILProcessor();
    if (method.ReturnType.FullName != "System.Void") {
        if (method.ReturnType.IsValueType) {
            var local = new VariableDefinition(method.ReturnType);
            method.Body.Variables.Add(local);
            il.Append(il.Create(OpCodes.Ldloca_S, local));
            il.Append(il.Create(OpCodes.Initobj, method.ReturnType));
            il.Append(il.Create(OpCodes.Ldloc_0));
        } else {
            il.Append(il.Create(OpCodes.Ldnull));
        }
    }
    il.Append(il.Create(OpCodes.Ret));
    Console.WriteLine($"  [NOOP] {label}");
}

IEnumerable<TypeDefinition> FlattenTypes(TypeDefinition type) {
    yield return type;
    foreach (var nested in type.NestedTypes.SelectMany<TypeDefinition, TypeDefinition>(FlattenTypes))
        yield return nested;
}
