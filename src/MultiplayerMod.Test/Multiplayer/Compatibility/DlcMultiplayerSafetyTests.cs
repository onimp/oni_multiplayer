using MultiplayerMod.Multiplayer.Compatibility;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Compatibility;

[TestFixture]
public class DlcMultiplayerSafetyTests {

    [Test]
    public void BlocksRocketAndStarmapObjectSync() {
        var rocketMethod = typeof(RocketControlStation).GetMethod(nameof(RocketControlStation.RestrictWhenGrounded))!;
        var starmapMethod = typeof(StarmapScreen).GetMethod(nameof(StarmapScreen.SelectDestination))!;

        Assert.IsTrue(DlcMultiplayerSafety.ShouldBlockObjectSync(rocketMethod));
        Assert.IsTrue(DlcMultiplayerSafety.ShouldBlockObjectSync(starmapMethod));
        Assert.That(DlcMultiplayerSafety.GetBlockMessage(rocketMethod), Does.Contain("rocket"));
    }

    [Test]
    public void AllowsNonDlcObjectSync() {
        var method = typeof(Door).GetMethod(nameof(Door.QueueStateChange))!;

        Assert.IsFalse(DlcMultiplayerSafety.ShouldBlockObjectSync(method));
        Assert.IsNull(DlcMultiplayerSafety.GetBlockReason(method));
    }

    private class RocketControlStation {
        public void RestrictWhenGrounded() { }
    }

    private class StarmapScreen {
        public void SelectDestination() { }
    }

    private class Door {
        public void QueueStateChange() { }
    }

}
