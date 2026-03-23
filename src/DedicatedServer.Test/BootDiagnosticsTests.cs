using DedicatedServer.Game;
using NUnit.Framework;

namespace DedicatedServer.Test;

/// <summary>
/// Tests for BootDiagnostics — the thread-safe boot-error counter that feeds
/// <c>bootErrorCount</c> in the /api/state response.
/// </summary>
[TestFixture]
public class BootDiagnosticsTests {

    [SetUp]
    public void SetUp() => BootDiagnostics.Reset();

    [Test]
    public void ErrorCount_StartsAtZero() {
        Assert.That(BootDiagnostics.ErrorCount, Is.EqualTo(0));
    }

    [Test]
    public void Record_IncrementsErrorCountByOne() {
        BootDiagnostics.Record();
        Assert.That(BootDiagnostics.ErrorCount, Is.EqualTo(1));
    }

    [Test]
    public void Record_MultipleCallsAccumulate() {
        BootDiagnostics.Record();
        BootDiagnostics.Record();
        BootDiagnostics.Record();
        Assert.That(BootDiagnostics.ErrorCount, Is.EqualTo(3));
    }

    [Test]
    public void Reset_ClearsCounterToZero() {
        BootDiagnostics.Record();
        BootDiagnostics.Record();
        BootDiagnostics.Reset();
        Assert.That(BootDiagnostics.ErrorCount, Is.EqualTo(0));
    }

    [Test]
    public void Record_AfterReset_StartsFromZero() {
        BootDiagnostics.Record();
        BootDiagnostics.Reset();
        BootDiagnostics.Record();
        Assert.That(BootDiagnostics.ErrorCount, Is.EqualTo(1));
    }
}
