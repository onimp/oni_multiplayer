using System;
using System.Threading;
using DedicatedServer.Web;
using MultiplayerMod.Test.GameRuntime;
using NUnit.Framework;

namespace DedicatedServer.Game;

/// <summary>
/// "Test" that boots the game world and starts the web server.
/// Run via: dotnet test --filter ServerBoot --no-build
/// This is a workaround for the Mono/CoreCLR runtime issue — 
/// dotnet test uses the correct Mono runtime that supports Harmony transpilers.
/// </summary>
[TestFixture]
public class ServerBootTest : PlayableGameTest {

    [Test]
    public void ServerBoot() {
        var port = 8080;
        Console.WriteLine($"[ServerBoot] Game world loaded. Starting web server on port {port}...");

        var cts = new CancellationTokenSource();

        var server = new WebServer(port);
        var realWorld = new RealWorldState(Grid.WidthInCells, Grid.HeightInCells);
        server.SetRealWorldState(realWorld);
        server.Start(cts.Token);

        Console.WriteLine($"[ServerBoot] Web server running at http://localhost:{port}/");
        Console.WriteLine("[ServerBoot] Press Ctrl+C to stop (or kill the dotnet test process).");

        // Block forever — this "test" is actually a server
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
            cts.Cancel();
        };

        try {
            Thread.Sleep(Timeout.Infinite);
        } catch (ThreadInterruptedException) {
            // Cancelled
        }
    }
}
