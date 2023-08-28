namespace MultiplayerMod.Game.Context;

public interface IGameContext {
    void Apply();
    void Restore();
}
