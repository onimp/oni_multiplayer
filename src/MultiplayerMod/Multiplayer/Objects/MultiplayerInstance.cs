using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Unity;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerInstance : MultiplayerKMonoBehaviour {

    [InjectDependency, UsedImplicitly]
    private MultiplayerGame multiplayer = null!;

    public MultiplayerId? Id { get; set; }

    protected override void OnCleanUp() {
        if (Id != null)
            multiplayer.Objects.Remove(Id);
    }

    public MultiplayerId Register() => multiplayer.Objects.Register(gameObject, Id);

    public void Redirect(MultiplayerInstance destination) {
        if (Id == null)
            return;
        destination.Id = Id;
        multiplayer.Objects[Id] = destination.gameObject;
        Id = null;
    }

}
