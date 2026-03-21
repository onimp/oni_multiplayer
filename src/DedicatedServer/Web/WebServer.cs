using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DedicatedServer.Game;
using Newtonsoft.Json;

namespace DedicatedServer.Web;

/// <summary>
/// Lightweight HTTP server using HttpListener.
/// Serves static files from wwwroot/ and provides API endpoints.
/// </summary>
public class WebServer {

    private readonly HttpListener listener;
    private readonly int port;
    private readonly string wwwrootPath;
    private RealWorldState? realWorld;

    private static readonly Dictionary<string, string> MimeTypes = new() {
        { ".html", "text/html; charset=utf-8" },
        { ".css", "text/css; charset=utf-8" },
        { ".js", "application/javascript; charset=utf-8" },
        { ".json", "application/json; charset=utf-8" },
        { ".png", "image/png" },
        { ".svg", "image/svg+xml" },
        { ".ico", "image/x-icon" }
    };

    public WebServer(int port) {
        this.port = port;
        listener = new HttpListener();
        listener.Prefixes.Add($"http://*:{port}/");

        // Resolve wwwroot path relative to the executable
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        wwwrootPath = Path.Combine(baseDir, "..", "..", "..", "wwwroot");
        if (!Directory.Exists(wwwrootPath)) {
            // Fallback: try relative to working directory
            wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "src", "DedicatedServer", "wwwroot");
        }
        if (!Directory.Exists(wwwrootPath)) {
            wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

    }

    public void SetRealWorldState(RealWorldState state) {
        realWorld = state;
        Console.WriteLine("[WebServer] Switched to real game world data.");
    }

    public void Start(CancellationToken ct) {
        try {
            listener.Start();
        } catch (Exception ex) {
            Console.WriteLine($"[WebServer] Failed to start on port {port}: {ex.Message} — continuing without web server.");
            return;
        }
        Task.Run(() => ListenLoop(ct), ct);
    }

    private async Task ListenLoop(CancellationToken ct) {
        while (!ct.IsCancellationRequested) {
            try {
                var context = await listener.GetContextAsync();
                _ = Task.Run(() => HandleRequest(context), ct);
            } catch (HttpListenerException) when (ct.IsCancellationRequested) {
                break;
            } catch (ObjectDisposedException) {
                break;
            } catch (Exception ex) {
                Console.WriteLine($"[WebServer] Error: {ex.Message}");
            }
        }
    }

    private void HandleRequest(HttpListenerContext context) {
        var path = context.Request.Url?.AbsolutePath ?? "/";
        try {
            if (path.StartsWith("/api/")) {
                HandleApiRequest(context, path);
            } else {
                HandleStaticFile(context, path);
            }
        } catch (Exception ex) {
            Console.WriteLine($"[WebServer] Request error {path}: {ex.Message}");
            SendJson(context.Response, 500, new { error = ex.Message });
        }
    }

    private void HandleApiRequest(HttpListenerContext context, string path) {
        if (realWorld == null) {
            SendJson(context.Response, 503, new { error = "World not loaded yet" });
            return;
        }
        var sw = Stopwatch.StartNew();
        switch (path) {
            case "/api/health":
                SendJson(context.Response, 200, new {
                    status = "ok",
                    source = "game",
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                });
                break;
            case "/api/world": {
                // Pre-serialized bytes, cached per SimTick — avoids re-serializing 98K cells on every request
                var bytes = realWorld.GetWorldSnapshotBytes();
                sw.Stop();
                var acceptsGzip = context.Request.Headers["Accept-Encoding"]?.Contains("gzip") == true;
                Console.WriteLine($"[WebServer] /api/world: getBytes={sw.ElapsedMilliseconds}ms size={bytes.Length/1024}KB gzip={acceptsGzip}");
                SendPrebuiltJsonMaybeGzip(context.Response, 200, bytes, acceptsGzip);
                return; // already stopped sw
            }
            case "/api/elements":
                SendJson(context.Response, 200, realWorld.GetElements());
                break;
            case "/api/entities": {
                var bytes = realWorld.GetEntitiesBytes();
                sw.Stop();
                Console.WriteLine($"[WebServer] /api/entities: getBytes={sw.ElapsedMilliseconds}ms size={bytes.Length}B");
                SendPrebuiltJson(context.Response, 200, bytes);
                return;
            }
            case "/api/state": {
                // Pre-serialized bytes, cached with 200ms TTL
                var bytes = realWorld.GetGameStateBytes();
                sw.Stop();
                Console.WriteLine($"[WebServer] /api/state: getBytes={sw.ElapsedMilliseconds}ms size={bytes.Length}B");
                SendPrebuiltJson(context.Response, 200, bytes);
                return;
            }
            default:
                SendJson(context.Response, 404, new { error = "Unknown API endpoint" });
                break;
        }
        sw.Stop();
        Console.WriteLine($"[WebServer] {path}: {sw.ElapsedMilliseconds}ms");
    }

    private void HandleStaticFile(HttpListenerContext context, string path) {
        if (path == "/") path = "/index.html";

        var filePath = Path.Combine(wwwrootPath, path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        filePath = Path.GetFullPath(filePath);

        // Security: ensure we don't serve files outside wwwroot
        if (!filePath.StartsWith(Path.GetFullPath(wwwrootPath))) {
            SendJson(context.Response, 403, new { error = "Forbidden" });
            return;
        }

        if (!File.Exists(filePath)) {
            SendJson(context.Response, 404, new { error = "Not found" });
            return;
        }

        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        var contentType = MimeTypes.ContainsKey(ext) ? MimeTypes[ext] : "application/octet-stream";

        var content = File.ReadAllBytes(filePath);
        context.Response.StatusCode = 200;
        context.Response.ContentType = contentType;
        context.Response.ContentLength64 = content.Length;
        context.Response.OutputStream.Write(content, 0, content.Length);
        context.Response.OutputStream.Close();
    }

    private static void SendJson(HttpListenerResponse response, int statusCode, object data) {
        var json = JsonConvert.SerializeObject(data);
        var bytes = Encoding.UTF8.GetBytes(json);
        SendPrebuiltJson(response, statusCode, bytes);
    }

    /// <summary>
    /// Sends already-serialized JSON bytes directly — skips double-serialization for cached responses.
    /// </summary>
    private static void SendPrebuiltJson(HttpListenerResponse response, int statusCode, byte[] bytes) {
        response.StatusCode = statusCode;
        response.ContentType = "application/json; charset=utf-8";
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.ContentLength64 = bytes.Length;
        response.OutputStream.Write(bytes, 0, bytes.Length);
        response.OutputStream.Close();
    }

    /// <summary>
    /// Sends JSON bytes, optionally gzip-compressed if the client accepts it.
    /// For large payloads like /api/world (~1.5MB JSON → ~150KB gzip).
    /// </summary>
    private static void SendPrebuiltJsonMaybeGzip(HttpListenerResponse response, int statusCode, byte[] bytes, bool gzip) {
        response.StatusCode = statusCode;
        response.ContentType = "application/json; charset=utf-8";
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        if (gzip) {
            var sw = Stopwatch.StartNew();
            using var ms = new MemoryStream();
            using (var gz = new GZipStream(ms, CompressionLevel.Fastest)) {
                gz.Write(bytes, 0, bytes.Length);
            }
            var compressed = ms.ToArray();
            sw.Stop();
            Console.WriteLine($"[WebServer] gzip: {bytes.Length/1024}KB → {compressed.Length/1024}KB in {sw.ElapsedMilliseconds}ms");
            response.Headers.Add("Content-Encoding", "gzip");
            response.ContentLength64 = compressed.Length;
            response.OutputStream.Write(compressed, 0, compressed.Length);
        } else {
            response.ContentLength64 = bytes.Length;
            response.OutputStream.Write(bytes, 0, bytes.Length);
        }
        response.OutputStream.Close();
    }
}
