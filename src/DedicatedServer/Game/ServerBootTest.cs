using System;
using System.Threading;
using DedicatedServer.Web;
using MultiplayerMod.Test.Environment.Unity;
using NUnit.Framework;

namespace DedicatedServer.Game;

/// <summary>
/// Entry point for the dedicated server.
/// Uses NUnit test runner as a workaround — dotnet test correctly configures
/// Mono runtime for Harmony transpilers on macOS (direct mono execution hangs).
/// Run via: dotnet test --filter ServerBoot --no-build
/// </summary>
[TestFixture]
public class ServerBootTest {

    [OneTimeSetUp]
    public void Setup() {
        // Install Unity patches BEFORE any game class is loaded
        UnityTestRuntime.Install();
    }

    [Test]
    public void ServerBoot() {
        var port = 8080;
        var loader = new GameLoader();

        Console.WriteLine("[Server] Booting game world...");
        loader.Boot();

        Console.WriteLine($"[Server] World ready ({loader.Width}x{loader.Height}), SimDLL: {loader.SimRunning}");

        var cts = new CancellationTokenSource();
        var server = new WebServer(port);
        server.SetRealWorldState(new RealWorldState(loader.Width, loader.Height, loader));
        server.Start(cts.Token);

        Console.WriteLine($"[Server] http://localhost:{port}/");

        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

        if (loader.SimRunning) {
            var tickThread = new Thread(() => {
                while (!cts.IsCancellationRequested) {
                    try { loader.TickSimulation(); Thread.Sleep(200); }
                    catch (Exception ex) { Console.WriteLine($"[Tick] {ex.Message}"); }
                }
            }) { IsBackground = true };
            tickThread.Start();
        }

        try { Thread.Sleep(Timeout.Infinite); }
        catch (ThreadInterruptedException) { }

        loader.Shutdown();
    }
}
