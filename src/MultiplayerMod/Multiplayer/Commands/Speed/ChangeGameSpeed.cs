using System;

namespace MultiplayerMod.Multiplayer.Commands.Speed;

[Serializable]
public class ChangeGameSpeed : MultiplayerCommand {

    private int speed;

    public ChangeGameSpeed(int speed) {
        this.speed = speed;
    }

    public override void Execute() {
        SpeedControlScreen.Instance.SetSpeed(speed);
    }

}
