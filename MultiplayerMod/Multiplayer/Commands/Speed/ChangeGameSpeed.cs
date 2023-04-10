using System;

namespace MultiplayerMod.Multiplayer.Commands.Speed;

[Serializable]
public class ChangeGameSpeed : IMultiplayerCommand {

    private int speed;

    public ChangeGameSpeed(int speed) {
        this.speed = speed;
    }

    public void Execute() {
        SpeedControlScreen.Instance.SetSpeed(speed);
    }

}
