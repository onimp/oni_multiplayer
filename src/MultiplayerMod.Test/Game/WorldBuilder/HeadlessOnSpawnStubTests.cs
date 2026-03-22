using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Unit tests for the headless singleton stubs initialized by WorldBuilder.InitializeWorld()
/// to prevent NPEs in OxygenBreather.OnSpawn() and Traits.OnSpawn().
///
/// Root cause:
///   OxygenBreather.OnSpawn() line 142:
///     NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this)
///     → NameDisplayScreen.Instance is null in headless → NPE logged for every dupe.
///
///   Traits.OnSpawn() line 41:
///     SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 15)
///     → SaveLoader.Instance is null in headless → NPE logged for every dupe.
///
/// Fix (WorldBuilder.InitializeWorld(), end of method):
///   1. new GameObject("NameDisplayScreen").AddComponent{NameDisplayScreen}().InitializeComponent()
///      → OnPrefabInit sets NameDisplayScreen.Instance = this → no more null dereference.
///   2. new GameObject("SaveLoader").AddComponent{SaveLoader}().InitializeComponent()
///      → OnPrefabInit sets SaveLoader.Instance = this.
///      GameInfo backing field is then set to (7, 37) via reflection so
///      IsVersionOlderThan(7, 15) returns false → Traits.OnSpawn() exits early (no legacy migration).
///
/// Tests here use SaveGame.GameInfo directly (a plain C# struct from Assembly-CSharp)
/// to verify the version-check logic without requiring a full game boot.
/// </summary>
[TestFixture]
[Parallelizable]
public class HeadlessOnSpawnStubTests {

    // ─── SaveLoader.GameInfo version check ─────────────────────────────────────

    [Test]
    public void SaveGameInfo_ModernVersion_7_37_IsNotOlderThan_7_15() {
        // Mirrors what WorldBuilder sets on the headless SaveLoader stub.
        // Traits.OnSpawn() line 41: if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 15)) return;
        // With (7,37): IsVersionOlderThan(7,15) = false → !false = true → returns early. Correct.
        var info = default(SaveGame.GameInfo);
        info.saveMajorVersion = 7;
        info.saveMinorVersion = 37;

        Assert.That(info.IsVersionOlderThan(7, 15), Is.False,
            "Modern save (7.37) must NOT be older than (7,15) — Traits.OnSpawn exits early, no legacy migration");
    }

    [Test]
    public void SaveGameInfo_DefaultZeroed_IsOlderThan_7_15() {
        // Default struct (all fields = 0) behaves as a very old save.
        // This confirms WHY we must set saveMajorVersion=7, saveMinorVersion=37:
        // without it, IsVersionOlderThan(7,15) = true → Traits.OnSpawn enters migration path.
        var info = default(SaveGame.GameInfo);

        Assert.That(info.IsVersionOlderThan(7, 15), Is.True,
            "Zeroed GameInfo (saveMajorVersion=0) is older than (7,15) — confirms default needs override");
    }

    [Test]
    public void SaveGameInfo_Boundary_7_15_IsNotOlderThan_7_15() {
        // Exactly (7,15) — the boundary.  IsVersionOlderThan checks strict: saveMinorVersion < 15.
        var info = default(SaveGame.GameInfo);
        info.saveMajorVersion = 7;
        info.saveMinorVersion = 15;

        Assert.That(info.IsVersionOlderThan(7, 15), Is.False,
            "Version (7,15) is not older than (7,15) — saveMinorVersion < minor is false");
    }

    [Test]
    public void SaveGameInfo_7_14_IsOlderThan_7_15() {
        // One minor version below boundary → older.
        var info = default(SaveGame.GameInfo);
        info.saveMajorVersion = 7;
        info.saveMinorVersion = 14;

        Assert.That(info.IsVersionOlderThan(7, 15), Is.True,
            "Version (7,14) is older than (7,15)");
    }
}
