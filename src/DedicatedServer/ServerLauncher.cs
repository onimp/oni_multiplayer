using System;
using System.IO;
using System.Reflection;

/// <summary>
/// Minimal launcher for DedicatedServer. Loads DedicatedServer.exe via reflection
/// to work around a Harmony/MonoMod self-test deadlock that occurs when the
/// DedicatedServer assembly is the main process entry point on standalone Mono.
///
/// Build: mcs ServerLauncher.cs -out:ServerLauncher.exe
/// Run:   mono ServerLauncher.exe [port]
/// </summary>
class ServerLauncher {
    static void Main(string[] args) {
        var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
        var serverExe = Path.Combine(exeDir, "DedicatedServer.exe");

        if (!File.Exists(serverExe)) {
            Console.Error.WriteLine($"DedicatedServer.exe not found in {exeDir}");
            Environment.Exit(1);
        }

        var asm = Assembly.LoadFrom(serverExe);
        var mainMethod = asm.GetType("DedicatedServer.Program")
            ?.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);

        if (mainMethod == null) {
            Console.Error.WriteLine("Could not find DedicatedServer.Program.Main");
            Environment.Exit(1);
        }

        mainMethod.Invoke(null, new object[] { args });
    }
}
