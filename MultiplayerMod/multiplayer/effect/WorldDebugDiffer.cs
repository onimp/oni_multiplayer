using System;
using MultiplayerMod.multiplayer.message;

namespace MultiplayerMod.multiplayer.effect
{
    public class WorldDebugDiffer : KMonoBehaviour, IRenderEveryTick
    {
        private WorldDebugInfo? _currentInfo;
        public event Action<WorldDebugInfo> OnDebugInfoAvailable;

        private const int CheckEveryFrame = 400;
        public static WorldDebugInfo? LastServerInfo { private get; set; }

        public void RenderEveryTick(float dt)
        {
            if (GameClock.Instance.GetFrame() % CheckEveryFrame == 0)
            {
                CompareIfApplicable();
                _currentInfo = WorldDebugInfo.CurrentDebugInfo();
                OnDebugInfoAvailable?.Invoke(_currentInfo!.Value);
                CompareIfApplicable();
            }
        }

        private void CompareIfApplicable()
        {
            if (LastServerInfo == null || LastServerInfo?.worldFrame != _currentInfo?.worldFrame) return;
            _currentInfo?.Compare(LastServerInfo!.Value);
            LastServerInfo = null;
        }
    }
}