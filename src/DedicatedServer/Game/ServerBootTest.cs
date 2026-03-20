using System;
using System.Threading;
using DedicatedServer.Web;
using MultiplayerMod.Test.Environment.Unity;
using NUnit.Framework;

namespace DedicatedServer.Game;

/// <summary>
/// "Test" that boots the game world and starts the web server.
/// Run via: dotnet test --filter ServerBoot --no-build
/// This is a workaround for the Mono/CoreCLR runtime issue —
/// dotnet test uses the correct Mono runtime that supports Harmony transpilers.
/// </summary>
[TestFixture]
public class ServerBootTest {

    [OneTimeSetUp]
    public void Setup() {
        // Install Unity patches FIRST — must happen before any game class is loaded.
        // ElementLoader has a static initializer that calls Application.streamingAssetsPath,
        // which is an InternalCall that doesn't exist without Unity.
        UnityTestRuntime.Install();
    }

    [Test]
    public void ServerBoot() {
        var port = 8080;
        var loader = new GameLoader();

        Console.WriteLine("[ServerBoot] Booting game world with real WorldGen + SimDLL...");
        loader.Boot();

        var gridWidth = loader.Width;
        var gridHeight = loader.Height;

        Console.WriteLine($"[ServerBoot] Game world ready ({gridWidth}x{gridHeight}), SimDLL: {loader.SimRunning}");
        Console.WriteLine("[ServerBoot] Starting web server...");

        var cts = new CancellationTokenSource();
        var server = new WebServer(port);
        var realWorld = new RealWorldState(gridWidth, gridHeight, loader);
        server.SetRealWorldState(realWorld);
        server.Start(cts.Token);

        Console.WriteLine($"[ServerBoot] Web server running at http://localhost:{port}/");
        Console.WriteLine("[ServerBoot] Press Ctrl+C to stop.");

        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
            cts.Cancel();
        };

        // Tick simulation in background if SimDLL is running
        if (loader.SimRunning) {
            Console.WriteLine("[ServerBoot] Starting simulation tick loop (200ms per tick)...");
            var tickThread = new Thread(() => {
                while (!cts.IsCancellationRequested) {
                    try {
                        loader.TickSimulation();
                        Thread.Sleep(200);
                    } catch (Exception ex) {
                        Console.WriteLine($"[ServerBoot] Tick error: {ex.Message}");
                    }
                }
            }) { IsBackground = true };
            tickThread.Start();
        }

        try {
            Thread.Sleep(Timeout.Infinite);
        } catch (ThreadInterruptedException) {
            // Cancelled
        }

        loader.Shutdown();
    }
}
