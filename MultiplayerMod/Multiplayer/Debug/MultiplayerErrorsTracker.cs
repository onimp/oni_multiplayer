namespace MultiplayerMod.Multiplayer.Debug;

public class MultiplayerErrorsTracker : WorldTracker {

    public MultiplayerErrorsTracker(int worldId) : base(worldId) { }

    public override void UpdateData() {
        AddPoint(WorldDebugSnapshotRunner.ErrorsCount);
    }

    public override string FormatValueString(float value) => value.ToString();

}
