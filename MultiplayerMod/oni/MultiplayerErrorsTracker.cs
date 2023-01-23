using MultiplayerMod.multiplayer.effect;

namespace MultiplayerMod.oni
{
    public class MultiplayerErrorsTracker : WorldTracker
    {
        public MultiplayerErrorsTracker(int worldID)
            : base(worldID)
        {
        }

        public override void UpdateData()
        {
            AddPoint(WorldDebugDiffer.ErrorsCount);
        }

        public override string FormatValueString(float value) => value.ToString();
    }
}