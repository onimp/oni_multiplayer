using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using DedicatedServer.Game;
using System.Threading.Tasks;
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
        // WorldState is a singleton created once by WorldBuilder.Create() — NOT per-request.
        server.SetRealWorldState(loader.World.WorldState);
        server.Start(cts.Token);

        Console.WriteLine($"Web server running at http://localhost:{port}/");

        if (loader.World.SimRunning) {
            // Run tick loop on the MAIN thread — this is required for correct game behavior.
            // Game.MainThread = Thread.CurrentThread is captured at static init time (main thread).
            // Game.IsOnMainThread() checks current thread == MainThread. If the tick loop runs
            // on a background thread, IsOnMainThread() returns false →
            //   DoPreconditions sets skippedPreconditions = true → IsComplete() returns false →
            //   CollectChores tries incomplete_contexts.Add(item) → NPE (null passed by 4-arg overload).
            Console.WriteLine("State machine tick loop running on main thread (press Ctrl+C to stop)");
            var tickLoop = loader.World.TickLoop;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (!cts.IsCancellationRequested) {
                sw.Restart();
                try {
                    tickLoop.Update(1f / 60f); // 60 UPS — one subtick per call, mirrors real game SimEveryTick rhythm
                }
                finally {
                    // Target exactly 60 UPS = 16.666...ms per tick.
                    // Old code used 16ms (= 62.5 UPS target), and Thread.Sleep jitter
                    // on macOS/Linux pushed actual UPS down to ~54.
                    // Fix: sleep the integer milliseconds, then spin-wait the sub-ms remainder
                    // so the actual cycle lands on the 16.666ms boundary precisely.
                    const double targetMs = 1000.0 / 60.0;  // 16.666...
                    var remainingMs = targetMs - sw.Elapsed.TotalMilliseconds;
                    if (remainingMs > 2.5) Thread.Sleep((int)(remainingMs - 1.5));
                    while (sw.Elapsed.TotalMilliseconds < targetMs) { /* spin <1ms for precision */ }
                }
            }
        } else {
            Console.WriteLine("Press Ctrl+C to stop.");
            try { Task.Delay(-1, cts.Token).Wait(); } catch (AggregateException) {}
        }

        loader.Shutdown();
        Console.WriteLine("Shutting down...");
    }
}
