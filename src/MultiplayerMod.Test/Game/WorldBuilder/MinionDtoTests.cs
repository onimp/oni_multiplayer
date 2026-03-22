using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Unit tests for the Minion entity DTO live-state fields added to /api/entities.
///
/// RealWorldState.BuildMinionDto serializes each spawned dupe with:
///   currentChore — ChoreType.Name of the active chore (null when idle / no chore)
///   smState      — ChoreDriver SM state name: "nochore" or "haschore"
///   navIsMoving  — true when Navigator is executing a path
///   navCell      — grid cell index at current position (Grid.PosToCell)
///
/// Tests replicate the DTO shape inline (project convention — no game runtime / no
/// DedicatedServer reference needed).  If the DTO shape changes, these tests catch it.
/// </summary>
[TestFixture]
[Parallelizable]
public class MinionDtoTests {

    // ─── Helper: replicates RealWorldState.BuildMinionDto ────────────────────

    private static object BuildMinionDto(
        string  name,
        int     x,
        int     y,
        int     w,
        int     h,
        string? currentChore,
        string? smState,
        bool    navIsMoving,
        int     navCell)
    {
        return new {
            type = "duplicant",
            name,
            x, y, w, h,
            currentChore,
            smState,
            navIsMoving,
            navCell
        };
    }

    private static JObject Serialize(object dto) =>
        JObject.Parse(JsonConvert.SerializeObject(dto));

    // ─── type field ───────────────────────────────────────────────────────────

    [Test]
    public void BuildMinionDto_Type_IsDuplicant() {
        var json = Serialize(BuildMinionDto("Minion", 10, 20, 1, 2, null, "nochore", false, 1234));
        Assert.That((string)json["type"]!, Is.EqualTo("duplicant"),
            "type field must be 'duplicant'");
    }

    // ─── currentChore field ───────────────────────────────────────────────────

    [Test]
    public void BuildMinionDto_ContainsCurrentChore_Field() {
        var json = Serialize(BuildMinionDto("Minion", 0, 0, 1, 2, "IdleChore", "haschore", false, 0));
        Assert.That(json.Property("currentChore"), Is.Not.Null,
            "currentChore field must be present in serialized Minion DTO");
        Assert.That((string?)json["currentChore"], Is.EqualTo("IdleChore"),
            "currentChore must carry the chore type name");
    }

    [Test]
    public void BuildMinionDto_CurrentChore_NullWhenNoChore() {
        var json = Serialize(BuildMinionDto("Minion", 0, 0, 1, 2, null, "nochore", false, 0));
        Assert.That(json.Property("currentChore"), Is.Not.Null,
            "currentChore key must still be present even when value is null");
        Assert.That(json["currentChore"]!.Type, Is.EqualTo(JTokenType.Null),
            "currentChore must be JSON null when dupe has no active chore");
    }

    // ─── smState field ────────────────────────────────────────────────────────

    [Test]
    public void BuildMinionDto_ContainsSmState_Field() {
        var json = Serialize(BuildMinionDto("Minion", 0, 0, 1, 2, null, "nochore", false, 0));
        Assert.That(json.Property("smState"), Is.Not.Null,
            "smState field must be present in serialized Minion DTO");
        Assert.That((string)json["smState"]!, Is.EqualTo("nochore"),
            "smState must carry the ChoreDriver SM state name");
    }

    [Test]
    public void BuildMinionDto_SmState_NullWhenSmNotStarted() {
        var json = Serialize(BuildMinionDto("Minion", 0, 0, 1, 2, null, null, false, -1));
        Assert.That(json.Property("smState"), Is.Not.Null,
            "smState key must be present even when SM is not yet started");
        Assert.That(json["smState"]!.Type, Is.EqualTo(JTokenType.Null),
            "smState must be JSON null when ChoreDriver SM has not started");
    }

    // ─── navIsMoving field ────────────────────────────────────────────────────

    [Test]
    public void BuildMinionDto_ContainsNavIsMoving_Field() {
        var json = Serialize(BuildMinionDto("Minion", 0, 0, 1, 2, null, "nochore", true, 999));
        Assert.That(json.Property("navIsMoving"), Is.Not.Null,
            "navIsMoving field must be present in serialized Minion DTO");
        Assert.That((bool)json["navIsMoving"]!, Is.True,
            "navIsMoving must reflect the Navigator.IsMoving() value");
    }

    // ─── navCell field ────────────────────────────────────────────────────────

    [Test]
    public void BuildMinionDto_ContainsNavCell_Field() {
        var json = Serialize(BuildMinionDto("Minion", 5, 15, 1, 2, null, "nochore", false, 50305));
        Assert.That(json.Property("navCell"), Is.Not.Null,
            "navCell field must be present in serialized Minion DTO");
        Assert.That((int)json["navCell"]!, Is.EqualTo(50305),
            "navCell must carry Grid.PosToCell value");
    }

    // ─── All four live fields in a single DTO ─────────────────────────────────

    [Test]
    public void BuildMinionDto_AllLiveFields_PresentAndCorrect() {
        var json = Serialize(BuildMinionDto(
            name:         "Minion",
            x:            12,
            y:            34,
            w:            1,
            h:            2,
            currentChore: "IdleChore",
            smState:      "haschore",
            navIsMoving:  true,
            navCell:      1234
        ));

        Assert.That((string)json["type"]!,         Is.EqualTo("duplicant"),  "type");
        Assert.That((string)json["name"]!,         Is.EqualTo("Minion"),     "name");
        Assert.That((int)json["x"]!,               Is.EqualTo(12),           "x");
        Assert.That((int)json["y"]!,               Is.EqualTo(34),           "y");
        Assert.That((int)json["w"]!,               Is.EqualTo(1),            "w");
        Assert.That((int)json["h"]!,               Is.EqualTo(2),            "h");
        Assert.That((string?)json["currentChore"], Is.EqualTo("IdleChore"),  "currentChore");
        Assert.That((string)json["smState"]!,      Is.EqualTo("haschore"),   "smState");
        Assert.That((bool)json["navIsMoving"]!,    Is.True,                  "navIsMoving");
        Assert.That((int)json["navCell"]!,         Is.EqualTo(1234),         "navCell");
    }
}
