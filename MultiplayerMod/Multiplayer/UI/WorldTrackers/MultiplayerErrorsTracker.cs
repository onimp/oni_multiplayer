using MultiplayerMod.Multiplayer.Debug;

namespace MultiplayerMod.Multiplayer.UI.WorldTrackers;

public class MultiplayerErrorsTracker : WorldTracker {

    public MultiplayerErrorsTracker(int worldId) : base(worldId) { }

    public override void UpdateData() {
        AddPoint(WorldDebugSnapshotRunner.ErrorsCount);
    }

    public override string FormatValueString(float value) => value.ToString();

}
