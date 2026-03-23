using DedicatedServer.Game;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DedicatedServer.Test;

/// <summary>
/// Tests for RealWorldState.BuildGameStateDto — verifies the JSON shape of the
/// /api/state response, specifically that <c>bootErrorCount</c> is present and
/// is a numeric value.
/// </summary>
[TestFixture]
public class GameStateDtoTests {

    private static string Serialize(int bootErrorCount = 0) {
        var dto = RealWorldState.BuildGameStateDto(
            tick:           100,
            cycle:          3,
            speed:          1,
            paused:         false,
            worldWidth:     256,
            worldHeight:    384,
            duplicantCount: 3,
            buildingCount:  42,
            entityCount:    1234,
            source:         "test",
            serverUps:      60,
            cycleTime:      300f,
            isNight:        false,
            bootErrorCount: bootErrorCount);
        return JsonConvert.SerializeObject(dto);
    }

    // ── bootErrorCount field presence ─────────────────────────────────────────

    [Test]
    public void GameStateDto_ContainsBootErrorCount_Field() {
        var obj = JObject.Parse(Serialize(bootErrorCount: 0));
        Assert.That(obj["bootErrorCount"], Is.Not.Null,
            "bootErrorCount field must be present in /api/state JSON");
    }

    [Test]
    public void GameStateDto_BootErrorCount_IsNumericType() {
        var obj = JObject.Parse(Serialize(bootErrorCount: 5));
        Assert.That(obj["bootErrorCount"]!.Type, Is.EqualTo(JTokenType.Integer),
            "bootErrorCount must be serialised as a JSON integer");
    }

    [Test]
    public void GameStateDto_BootErrorCount_ReflectsZeroWhenNoErrors() {
        var obj = JObject.Parse(Serialize(bootErrorCount: 0));
        Assert.That(obj["bootErrorCount"]!.Value<int>(), Is.EqualTo(0));
    }

    [Test]
    public void GameStateDto_BootErrorCount_ReflectsNonZeroCount() {
        var obj = JObject.Parse(Serialize(bootErrorCount: 2225));
        Assert.That(obj["bootErrorCount"]!.Value<int>(), Is.EqualTo(2225));
    }

    // ── integration: BootDiagnostics.ErrorCount flows into the DTO ────────────

    [Test]
    public void GameStateDto_BootErrorCount_MatchesBootDiagnosticsErrorCount() {
        BootDiagnostics.Reset();
        BootDiagnostics.Record();
        BootDiagnostics.Record();

        var obj = JObject.Parse(Serialize(bootErrorCount: BootDiagnostics.ErrorCount));
        Assert.That(obj["bootErrorCount"]!.Value<int>(), Is.EqualTo(2));

        BootDiagnostics.Reset();
    }

    // ── other required fields are still present ───────────────────────────────

    [Test]
    public void GameStateDto_ContainsAllRequiredFields() {
        var obj = JObject.Parse(Serialize());
        foreach (var field in new[] { "tick", "cycle", "speed", "paused",
            "worldWidth", "worldHeight", "duplicantCount", "buildingCount",
            "entityCount", "source", "serverUps", "cycleTime", "isNight",
            "bootErrorCount" }) {
            Assert.That(obj[field], Is.Not.Null, $"Field '{field}' must be present");
        }
    }
}
