using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Multiplayer.StateMachines.Configuration;
using MultiplayerMod.Test.Environment;
using MultiplayerMod.Test.GameRuntime;
using NUnit.Framework;
using static MultiplayerMod.Test.Core.Patch.Compatibility.PatchesCompatibilityMetadata;

namespace MultiplayerMod.Test.Core.Patch.Compatibility;

[TestFixture]
public class PatchesCompatibility : PlayableGameTest {

    [ConfigureDependencies, UsedImplicitly]
    private static void SetUp(DependencyContainerBuilder builder) {
        builder.AddStateMachineAndChoreConfigurers();
    }

    [Test]
    [Ignore("Manual run only")]
    public void Generate() {
        var methods = GetChoresStateMachines();
        var metadata = string.Join(",\n", methods);
        Console.WriteLine(metadata);
    }

    private static List<string> GetChoresStateMachines() {
        var context = new StateMachineConfigurationContext(DependencyContainer);
        var configurers = DependencyContainer.Get<List<IStateMachineConfigurer>>();
        configurers.ForEach(it => it.Configure(context));

        return context.Configurations
            .Select(it => it.StateMachineType)
            .Select(it => it.GetMethod(nameof(StateMachine.InitializeStates))!)
            .Select(
                it => (
                    method: it,
                    signature: it.GetSignature(SignatureOptions.NoDeclaringType),
                    hash: HashAlgorithm.ComputeHash(it).ToHexString()
                )
            )
            .Select(
                it =>
                    $"new(" +
                    $"typeof({it.method.DeclaringType!.GetSignature(SignatureOptions.Namespace)}), " +
                    $"\"{it.signature}\", " +
                    $"\"{it.hash}\"" +
                    $")"
            )
            .ToList();
    }

    /// <summary>
    /// Boots game world and serves real Grid data via HTTP for the visualizer.
    /// Run: dotnet test --filter ServerBoot --no-build -c Debug
    /// </summary>
    [Test, Timeout(0)]
    public void ServerBoot() {
        var port = 8080;
        Console.WriteLine($"[ServerBoot] Grid: {Grid.WidthInCells}x{Grid.HeightInCells}");
        var listener = new System.Net.HttpListener();
        listener.Prefixes.Add($"http://*:{port}/");
        listener.Start();
        Console.WriteLine($"[ServerBoot] http://localhost:{port}/");
        var wwwroot = FindWwwroot();
        Console.WriteLine($"[ServerBoot] wwwroot: {wwwroot ?? "NOT FOUND"}");
        var tick = 0;
        while (true) {
            var ctx = listener.GetContext();
            var path = ctx.Request.Url?.AbsolutePath ?? "/";
            try {
                if (path == "/api/health") SendJson(ctx, new { status = "ok", source = "game", tick });
                else if (path == "/api/world") SendJson(ctx, GetWorldData(ref tick));
                else if (path == "/api/entities") SendJson(ctx, new { tick, entities = new object[0] });
                else if (path == "/api/state") SendJson(ctx, new { tick, cycle = GameClock.Instance?.GetCycle() + 1 ?? 1, speed = 1, paused = false, worldWidth = Grid.WidthInCells, worldHeight = Grid.HeightInCells, duplicantCount = 0, buildingCount = 0, source = "game" });
                else ServeStatic(ctx, path, wwwroot);
            } catch (Exception ex) { try { SendJson(ctx, new { error = ex.Message }, 500); } catch { } }
        }
    }

    private static unsafe object GetWorldData(ref int tick) {
        tick++;
        var w = Grid.WidthInCells; var h = Grid.HeightInCells;
        var numCells = w * h;
        var cells = new object[numCells];
        for (var i = 0; i < numCells; i++) {
            cells[i] = new {
                element = Grid.elementIdx != null ? (int)Grid.elementIdx[i] : 0,
                temperature = Grid.temperature != null ? Math.Round(Grid.temperature[i], 1) : 0.0,
                mass = 0.0
            };
        }
        return new { width = w, height = h, tick, cells };
    }

    private static void SendJson(System.Net.HttpListenerContext ctx, object data, int status = 200) {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        ctx.Response.StatusCode = status; ctx.Response.ContentType = "application/json";
        ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        ctx.Response.ContentLength64 = bytes.Length;
        ctx.Response.OutputStream.Write(bytes, 0, bytes.Length); ctx.Response.OutputStream.Close();
    }

    private static void ServeStatic(System.Net.HttpListenerContext ctx, string path, string? root) {
        if (root == null) { SendJson(ctx, new { error = "No wwwroot" }, 404); return; }
        if (path == "/") path = "/index.html";
        var file = System.IO.Path.Combine(root, path.TrimStart('/'));
        if (!System.IO.File.Exists(file)) { SendJson(ctx, new { error = "Not found" }, 404); return; }
        var ext = System.IO.Path.GetExtension(file).ToLower();
        var mime = ext == ".html" ? "text/html" : ext == ".js" ? "application/javascript" : ext == ".css" ? "text/css" : "application/octet-stream";
        var bytes = System.IO.File.ReadAllBytes(file);
        ctx.Response.StatusCode = 200; ctx.Response.ContentType = mime;
        ctx.Response.ContentLength64 = bytes.Length;
        ctx.Response.OutputStream.Write(bytes, 0, bytes.Length); ctx.Response.OutputStream.Close();
    }

    private static string? FindWwwroot() {
        var dir = AppDomain.CurrentDomain.BaseDirectory;
        for (var i = 0; i < 8; i++) { var c = System.IO.Path.Combine(dir, "src", "DedicatedServer", "wwwroot"); if (System.IO.Directory.Exists(c)) return c; dir = System.IO.Path.Combine(dir, ".."); }
        return null;
    }

    [Test]
    public void Verify() {
        var possiblyIncompatible = new List<string>();
        foreach (var type in Metadata.Keys) {
            var methods = type.GetAllMethods()
                .Where(it => it.HasMethodBody())
                .Select(it => new MethodMetadata(type, it.GetSignature(SignatureOptions.NoDeclaringType), HashAlgorithm.ComputeHash(it).ToHexString()))
                .ToHashSet();
            var diff = Metadata[type]
                .Where(it => !methods.Contains(it.Value))
                .Select(it => $"{type.GetSignature(SignatureOptions.Namespace)}::{it.Value.Signature}");
            possiblyIncompatible.AddRange(diff);
        }
        if (possiblyIncompatible.Count > 0)
            Assert.Fail($"Possibly incompatible original methods found:\n{string.Join("\n", possiblyIncompatible)}\n");
    }

}
