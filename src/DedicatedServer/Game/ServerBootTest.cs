using System;
using System.Threading;
using DedicatedServer.Web;
using MultiplayerMod.Test.Environment.Unity;
using NUnit.Framework;

namespace DedicatedServer.Game;

[TestFixture]
public class ServerBootTest {
    [OneTimeSetUp]
    public void Setup() => UnityTestRuntime.Install();

    [Test]
    public void ServerBoot() {
        var loader = new GameLoader();
        loader.Boot();
        var cts = new CancellationTokenSource();
        var server = new WebServer(8080);
        server.SetRealWorldState(new RealWorldState(loader.Width, loader.Height, loader));
        server.Start(cts.Token);
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };
        if (loader.SimRunning) {
            new Thread(() => { while (!cts.IsCancellationRequested) { try { loader.TickSimulation(); Thread.Sleep(200); } catch {} } }) { IsBackground = true }.Start();
        }
        Console.WriteLine($"[Server] http://localhost:8080/ ({loader.Width}x{loader.Height}, SimDLL: {loader.SimRunning})");
        try { Thread.Sleep(Timeout.Infinite); } catch (ThreadInterruptedException) {}
        loader.Shutdown();
    }
}
