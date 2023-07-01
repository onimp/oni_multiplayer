using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerInstance : KMonoBehaviour {

    public long? Id { get; set; }

    protected override void OnCleanUp() {
        if (Id != null)
            MultiplayerGame.Objects.Remove(Id.Value);
    }

}
