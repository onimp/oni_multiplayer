using MultiplayerMod.Core.Extensions;

namespace MultiplayerMod.Game.Context;

public class GameContextComposite : IGameContext {

    private readonly IGameContext[] contexts;

    public GameContextComposite(params IGameContext[] contexts) {
        this.contexts = contexts;
    }

    public void Apply() {
        contexts.ForEach(it => it.Apply());
    }

    public void Restore() {
        contexts.ForEach(it => it.Restore());
    }

}
