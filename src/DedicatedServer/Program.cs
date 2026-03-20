using System;
using System.IO;
using System.Reflection;
using System.Threading;
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
        var useMock = false;

        foreach (var arg in args) {
            if (int.TryParse(arg, out var p)) port = p;
            if (arg == "--mock") useMock = true;
        }

        Console.WriteLine($"ONI Dedicated Server starting on port {port}...");

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
            cts.Cancel();
        };

        // Start web server first (serves mock data initially)
        var server = new WebServer(port);
        server.Start(cts.Token);
        Console.WriteLine($"Web server running at http://localhost:{port}/");

        GameLoader? gameLoader = null;

        if (!useMock) {
            try {
                Console.WriteLine("Loading game DLLs...");
                gameLoader = new GameLoader();
                gameLoader.Boot();
                server.SetRealWorldState(new RealWorldState(gameLoader.Width, gameLoader.Height, gameLoader));
                Console.WriteLine("Game world loaded. Visualizer showing real data.");
            } catch (Exception ex) {
                Console.WriteLine($"[ERROR] Failed to load game: {ex.Message}");
                Console.WriteLine($"[ERROR] {ex.StackTrace}");
                Console.WriteLine("Falling back to mock data. Use --mock to skip game loading.");
            }
        } else {
            Console.WriteLine("Mock mode: using generated world data.");
        }

        Console.WriteLine("Press Ctrl+C to stop.");

        try {
            Task.Delay(-1, cts.Token).Wait();
        } catch (AggregateException) {
            // Cancelled
        }

        gameLoader?.Shutdown();
        Console.WriteLine("Shutting down...");
    }
}
