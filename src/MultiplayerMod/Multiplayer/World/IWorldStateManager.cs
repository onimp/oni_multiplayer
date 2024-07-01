using MultiplayerMod.Multiplayer.World.Data;

namespace MultiplayerMod.Multiplayer.World;

public interface IWorldStateManager {
    void SaveState(WorldState data);
    void LoadState(WorldState data);
}
