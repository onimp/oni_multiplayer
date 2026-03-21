using System;
using System.IO;
using System.Reflection;

/// <summary>
/// Minimal launcher for DedicatedServer. Calls AppleSiliconHarmony Patcher
/// before loading DedicatedServer to fix W^X memory protection on macOS.
/// </summary>
class ServerLauncher {
    static void Main(string[] args) {
        var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";

        // NUnit3TestAdapter and Microsoft.NET.Test.Sdk are project dependencies for
        // running tests via 'dotnet test'. They bring in TestPlatform DLLs at runtime.
        // When running as a standalone server those DLLs are not present — return null
        // so the CLR treats them as unavailable (types from them are never used directly).
        AppDomain.CurrentDomain.AssemblyResolve += (_, e) => {
            var n = new AssemblyName(e.Name).Name ?? "";
            if (n.StartsWith("Microsoft.TestPlatform") || n.StartsWith("Microsoft.VisualStudio.TestPlatform"))
                return null;
            return null;
        };

        // Fix Apple Silicon / Rosetta W^X before Harmony loads
        try {
            var patcherDll = Path.Combine(exeDir, "AppleSiliconHarmony.dll");
            if (File.Exists(patcherDll)) {
                var asm = Assembly.LoadFrom(patcherDll);
                var patchMethod = asm.GetType("Anatawa12.HarmonyAppleSilicon.Patcher")
                    ?.GetMethod("Patch", BindingFlags.Public | BindingFlags.Static);
                patchMethod?.Invoke(null, null);
                Console.WriteLine("[Launcher] AppleSiliconHarmony patch applied");
            }
        } catch (Exception ex) {
            Console.WriteLine($"[Launcher] AppleSiliconHarmony: {ex.Message}");
        }

        var serverExe = Path.Combine(exeDir, "DedicatedServer.exe");
        if (!File.Exists(serverExe)) {
            Console.Error.WriteLine($"DedicatedServer.exe not found in {exeDir}");
            Environment.Exit(1);
        }

        Console.WriteLine($"Loading {serverExe}...");
        var serverAsm = Assembly.LoadFrom(serverExe);
        var mainMethod = serverAsm.GetType("DedicatedServer.Program")
            ?.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);

        if (mainMethod == null) {
            Console.Error.WriteLine("Could not find DedicatedServer.Program.Main");
            Environment.Exit(1);
        }

        mainMethod.Invoke(null, new object[] { args });
    }
}
