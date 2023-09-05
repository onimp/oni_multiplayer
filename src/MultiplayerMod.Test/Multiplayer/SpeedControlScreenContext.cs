using System.Reflection;
using JetBrains.Annotations;
using MultiplayerMod.Game.Context;

namespace MultiplayerMod.Test.Multiplayer;

[UsedImplicitly]
public class SpeedControlScreenContext : IGameContext {

    private SpeedControlScreen? instance;

    private readonly PropertyInfo property = typeof(SpeedControlScreen).GetProperty(
        nameof(SpeedControlScreen.Instance),
        BindingFlags.Static | BindingFlags.Public
    )!;

    // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation
    public void Apply() {
        instance = SpeedControlScreen.Instance;
        property.SetValue(null, new SpeedControlScreen());
    }

    public void Restore() {
        property.SetValue(null, instance);
    }

}
