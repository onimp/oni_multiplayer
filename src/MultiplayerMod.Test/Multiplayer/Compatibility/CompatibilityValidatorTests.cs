using MultiplayerMod.Multiplayer.Compatibility;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Compatibility;

[TestFixture]
public class CompatibilityValidatorTests {

    private readonly CompatibilityValidator validator = new();

    [Test]
    public void MatchingFingerprintsAreCompatible() {
        var fingerprint = CreateFingerprint();

        var result = validator.Validate(fingerprint, fingerprint);

        Assert.IsTrue(result.Compatible);
        Assert.IsEmpty(result.Mismatches);
    }

    [Test]
    public void DlcMismatchIsRejected() {
        var host = CreateFingerprint(loadedDlcIds: new[] { "EXPANSION1_ID", "DLC2_ID" });
        var client = CreateFingerprint(loadedDlcIds: new[] { "EXPANSION1_ID" });

        var result = validator.Validate(host, client);

        Assert.IsFalse(result.Compatible);
        Assert.That(result.ToUserMessage(), Does.Contain("Loaded DLC/content IDs differ"));
    }

    [Test]
    public void ModMismatchIsRejected() {
        var host = CreateFingerprint(activeMods: new[] { new ModFingerprint("multiplayerMod", "Multiplayer", "1.0.0") });
        var client = CreateFingerprint(activeMods: new[] { new ModFingerprint("multiplayerMod", "Multiplayer", "1.0.1") });

        var result = validator.Validate(host, client);

        Assert.IsFalse(result.Compatible);
        Assert.That(result.ToUserMessage(), Does.Contain("Active mod list differs"));
    }

    [Test]
    public void EmptyClientActiveSaveDlcDoesNotRejectJoinBeforeWorldTransfer() {
        var host = CreateFingerprint(activeDlcIds: new[] { "EXPANSION1_ID", "DLC2_ID" });
        var client = CreateFingerprint(activeDlcIds: System.Array.Empty<string>());

        var result = validator.Validate(host, client);

        Assert.IsTrue(result.Compatible);
    }

    private static CompatibilityFingerprint CreateFingerprint(
        string[]? loadedDlcIds = null,
        string[]? activeDlcIds = null,
        ModFingerprint[]? activeMods = null
    ) => new(
        CompatibilityFingerprint.CurrentProtocolVersion,
        "dlc-preview",
        "737790",
        "release",
        loadedDlcIds ?? new[] { "EXPANSION1_ID" },
        activeDlcIds ?? new[] { "EXPANSION1_ID" },
        activeMods ?? new[] { new ModFingerprint("multiplayerMod", "Multiplayer", "1.0.0") }
    );

}
