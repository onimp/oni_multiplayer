using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DedicatedServer.Game;
using DedicatedServer.Web;
using MultiplayerMod.Test.Environment.Unity;

namespace DedicatedServer;

public static class Program {

    private const int DefaultPort = 8080;

    // DLL search paths for runtime assembly resolution
    private static readonly string[] assemblySearchPaths = GetAssemblySearchPaths();

    private static string[] GetAssemblySearchPaths() {
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
        var repoRoot = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..","..",".."));
        var steamPath = Environment.GetEnvironmentVariable("ONI_MANAGED_PATH")
            ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Library", "Application Support", "Steam", "steamapps", "common",
                "OxygenNotIncluded", "OxygenNotIncluded.app", "Contents", "Resources", "Data", "Managed"
            );

        return new[] {
            Path.Combine(repoRoot, "lib", "exposed"),
            Path.Combine(repoRoot, "lib", "runtime"),
            steamPath,
            Path.Combine(repoRoot, "src", "MultiplayerMod", "bin", "Debug", "net48"),
            Path.Combine(repoRoot, "src", "MultiplayerMod.Test", "bin", "Debug", "net48"),
        };
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

        // Install Unity patches BEFORE any game class is loaded.
        // ElementLoader has a static initializer that calls Application.streamingAssetsPath.
        UnityTestRuntime.Install();

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
            cts.Cancel();
        };

        var loader = new GameLoader();
        loader.Boot();

        var server = new WebServer(port);
        server.SetRealWorldState(new RealWorldState(loader.Width, loader.Height, loader));
        server.Start(cts.Token);

        Console.WriteLine($"Web server running at http://localhost:{port}/");

        // Tick simulation in background
        if (loader.SimRunning) {
            Console.WriteLine("Simulation tick loop started (200ms per tick)");
            var tickThread = new Thread(() => {
                while (!cts.IsCancellationRequested) {
                    try {
                        loader.TickSimulation();
                        Thread.Sleep(200);
                    } catch (Exception ex) {
                        Console.WriteLine($"[Tick] Error: {ex.Message}");
                    }
                }
            }) { IsBackground = true };
            tickThread.Start();
        }

        Console.WriteLine("Press Ctrl+C to stop.");

        try {
            Task.Delay(-1, cts.Token).Wait();
        } catch (AggregateException) {
            // Cancelled
        }

        loader.Shutdown();
        Console.WriteLine("Shutting down...");
    }
}
