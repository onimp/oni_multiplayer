using System;

namespace MultiplayerMod.multiplayer.message
{
    [Serializable]
    public struct UserAction
    {

        public UserActionTypeEnum userActionType;
        public object Payload;
        
        public enum UserActionTypeEnum
        {
            MouseMove
        }
    }
}