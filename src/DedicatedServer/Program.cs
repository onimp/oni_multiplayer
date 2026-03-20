using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DedicatedServer.Web;

namespace DedicatedServer;

public static class Program {

    private const int DefaultPort = 8080;

    public static void Main(string[] args) {
        var port = args.Length > 0 && int.TryParse(args[0], out var p) ? p : DefaultPort;
        Console.WriteLine($"ONI Dedicated Server starting on port {port}...");

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
            cts.Cancel();
        };

        var server = new WebServer(port);
        server.Start(cts.Token);

        Console.WriteLine($"Web server running at http://localhost:{port}/");
        Console.WriteLine("Press Ctrl+C to stop.");

        try {
            Task.Delay(-1, cts.Token).Wait();
        } catch (AggregateException) {
            // Cancelled
        }

        Console.WriteLine("Shutting down...");
    }
}
