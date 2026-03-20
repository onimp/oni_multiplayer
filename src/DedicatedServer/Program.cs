using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using DedicatedServer.Game;
using System.Threading.Tasks;
using DedicatedServer.Game;
using DedicatedServer.Web;

namespace DedicatedServer;

public static class Program {

    private const int DefaultPort = 8080;

    // DLL search paths for runtime assembly resolution
    private static readonly string[] assemblySearchPaths = GetAssemblySearchPaths();

    private static string[] GetAssemblySearchPaths() {
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
        var repoRoot = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..","..",".."));

        var paths = new List<string> {
            Path.Combine(repoRoot, "lib", "exposed"),
            Path.Combine(repoRoot, "lib", "runtime"),
            Path.Combine(repoRoot, "src", "MultiplayerMod", "bin", "Debug", "net48"),
            Path.Combine(repoRoot, "src", "MultiplayerMod.Test", "bin", "Debug", "net48"),
        };

        // Optional: additional managed DLL path via env variable
        var managedPath = Environment.GetEnvironmentVariable("ONI_MANAGED_PATH");
        if (!string.IsNullOrEmpty(managedPath)) paths.Add(managedPath);

        return paths.ToArray();
    }

    static Program() {
        AppDomain.CurrentDomain.AssemblyResolve += (_, e) => {
            var name = new AssemblyName(e.Name).Name + ".dll";
            foreach (var dir in assemblySearchPaths) {
                var path = Path.Combine(dir, name);
                if (File.Exists(path)) return Assembly.LoadFrom(path);
            }
            return null;
        };
    }

    public static void Main(string[] args) {
        var port = DefaultPort;
        foreach (var arg in args) {
            if (int.TryParse(arg, out var p)) port = p;
        }

        Console.WriteLine($"ONI Dedicated Server starting on port {port}...");

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
            cts.Cancel();
        };

        var loader = new GameLoader();
        loader.Boot();

        var server = new WebServer(port);
        server.SetRealWorldState(new RealWorldState(loader.World.Width, loader.World.Height, loader.World));
        server.Start(cts.Token);

        Console.WriteLine($"Web server running at http://localhost:{port}/");

        if (loader.World.SimRunning) {
            Console.WriteLine("Simulation tick loop started (200ms per tick)");
            new Thread(() => {
                while (!cts.IsCancellationRequested) {
                    try { loader.World.TickSimulation(); Thread.Sleep(200); }
                    catch (Exception ex) { Console.WriteLine($"[Tick] Error: {ex.Message}"); }
                }
            }) { IsBackground = true }.Start();
        }

        Console.WriteLine("Press Ctrl+C to stop.");
        try { Task.Delay(-1, cts.Token).Wait(); } catch (AggregateException) {}

        loader.Shutdown();
        Console.WriteLine("Shutting down...");
    }
}
