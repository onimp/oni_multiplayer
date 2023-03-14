using System;

namespace MultiplayerMod.Multiplayer.Message
{
    [Serializable]
    public struct UserAction
    {
        public UserActionTypeEnum userActionType;
        public object Payload;

        public enum UserActionTypeEnum
        {
            // Speed control actions
            Pause,
            Unpause,
            SetSpeed,

            // UI Bottom right part buttons
            Attack,
            Build,
            UtilityBuild,
            WireBuild,
            Cancel,
            Capture,
            Clear,
            CopySettings,
            Debug,
            Deconstruct,
            Dig,
            Disinfect,
            EmptyPipe,
            Harvest,
            Mop,
            Place,
            Priority,
        }
    }
}
